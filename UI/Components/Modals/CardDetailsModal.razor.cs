using Blazored.TextEditor;
using Domain.Dtos.Comment;
using Domain.Dtos.State;
using Microsoft.AspNetCore.Components;
using Services.External;
using UI.Services;
using Modal = Blazorise.Modal;

namespace UI.Components.Modals;

public partial class CardDetailsModal : ComponentBase
{
    [Inject] public IStateService StateService { get; set; }
    [Inject] public ICommentService CommentService { get; set; }
    [Inject] public CustomAuthStateProvider AuthStateProvider { get; set; }
    [Inject] public IUserService UserService { get; set; }

    [Parameter] public Guid CardId { get; set; }
    [Parameter] public EventCallback OnCancelClicked { get; set; }

    private Modal ModalRef { get; set; }
    private BlazoredTextEditor QuillHtml { get; set; }
    private BlazoredTextEditor NewCommentEditor { get; set; }
    private CommentDeleteConfirmationModal _deleteConfirmationModal;
    private Guid commentToDeleteId;

    private List<CommentDto> Comments { get; set; } = [];

    private string NewComment { get; set; } = string.Empty;

    private bool _isAddingComment;

    private StateDto State { get; set; } = new()
    {
        Id = new Guid(),
        CardId = new Guid(),
        Description = string.Empty,
        Priority = Priority.Low,
        Status = Status.Pending
    };

    private UpdateStateDto _updateState = new();

    public async Task ShowAsync()
    {
        await ModalRef.Show();

        await LoadStateAsync();
        await LoadCommentsAsync();

        await QuillHtml.LoadHTMLContent(State.Description);
    }

    private async Task LoadCommentsAsync()
    {
        try
        {
            var token = await AuthStateProvider.GetToken();
            Comments = (await CommentService.GetByCardIdAsync(CardId, $"Bearer {token}"))
                .ToList();

            foreach (var comment in Comments)
            {
                var user = await UserService.GetById(comment.UserId);
                comment.CreatedBy = $"{user.FirstName} {user.LastName}";
            }
            
            StateHasChanged();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error loading comments: {exception.Message}");
        }
    }

    private async Task LoadStateAsync()
    {
        try
        {
            State = await StateService.GetByCardIdAsync(CardId);
            StateHasChanged();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error loading state: {exception.Message}");
        }
    }

    public async Task Hide()
    {
        _isAddingComment = false;
        await ModalRef.Hide();
    }

    private async Task UpdateStateAsync()
    {
        var updateState = new UpdateStateDto
        {
            Id = State.Id,
            CardId = CardId,
            Status = State.Status,
            Priority = State.Priority,
            Description = await QuillHtml.GetHTML()
        };
        try
        {
            await StateService.UpdateAsync(CardId, updateState);
            
            await Hide();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error updating state: {e.Message}");
        }
    }

    private async Task OnCancelAsync()
    {
        await Hide();

        if (OnCancelClicked.HasDelegate)
        {
            await OnCancelClicked.InvokeAsync();
        }
    }

    private void StartAddingComment()
    {
        _isAddingComment = true;
    }

    private void CancelAddingComment()
    {
        _isAddingComment = false;
    }

    private async Task CreateCommentAsync()
    {
        var userId = await AuthStateProvider.GetUserIdAsync();

        var content = await NewCommentEditor.GetHTML();

        if (string.IsNullOrWhiteSpace(content) || content == "<p><br></p>")
        {
            return;
        }

        var newComment = new CreateCommentDto
        {
            CardId = CardId,
            UserId = userId,
            Content = await NewCommentEditor.GetHTML()
        };

        try
        {
            var token = await AuthStateProvider.GetToken();
            await CommentService.CreateAsync(newComment, $"Bearer {token}");
            await LoadCommentsAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error creating comment: {e.Message}");
        }

        CancelAddingComment();
    }

    private string FormatSting(DateTimeOffset createdAtUtc)
    {
        var diff = DateTimeOffset.Now - createdAtUtc;

        return diff < TimeSpan.FromMinutes(3) ? "just now" : createdAtUtc.ToString("g");
    }
    
    private void OnDeleteClicked(Guid commentId)
    {
        commentToDeleteId = commentId;
        _deleteConfirmationModal.Show();
    }
    
    private async Task ConfirmDeleteCommentAsync()
    {
        var token = await AuthStateProvider.GetToken();
        await CommentService.DeleteAsync(commentToDeleteId, $"Bearer {token}");
        
        await LoadCommentsAsync();
    }
}