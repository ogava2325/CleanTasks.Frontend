using System.Net;
using Blazorise;
using Domain.Dtos.Card;
using Domain.Dtos.Column;
using Domain.Dtos.Project;
using Domain.Dtos.State;
using Domain.Dtos.User;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Refit;
using services.External;
using UI.Components.Modals;
using UI.Components.OffCanvas;
using UI.Services;

namespace UI.Components.Pages;

public partial class Kanban : ComponentBase
{
    [Inject] private ICardService CardService { get; set; }
    [Inject] private IStateService StateService { get; set; }
    [Inject] private IColumnService ColumnService { get; set; }
    [Inject] private IProjectService ProjectService { get; set; }
    [Inject] public CustomAuthStateProvider AuthStateProvider { get; set; }
    [Inject] public INotificationService NotificationService { get; set; }
    [Parameter] public Guid ProjectId { get; set; }

    private List<CardDto> _cards = [];
    private List<ColumnDto> _columns = [];

    private readonly CreateCardDto _newCard = new();
    private readonly CreateColumnDto _newColumn = new();

    private bool _isAddingColumn;
    private Guid? _activeColumnId;

    private Guid _selectedCardId;

    private CardDetailsModal CardDetailsModalRef { get; set; } = default!;
    private OffCanvasMenu OffCanvasMenuRef { get; set; } = default!;
    private InviteUserModal InviteUserModalRef { get; set; } = default!;

    private CommentDeleteConfirmationModal _deleteConfirmationModal;
    private Guid columnToDeleteId;

    private ProjectDto CurrentProject { get; set; } = new();

    private bool IsEditingTitle { get; set; }
    private string EditTitle { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await LoadCardsAsync();
        await LoadColumnsAsync();
        await LoadProjectAsync();
    }

    private async Task LoadProjectAsync()
    {
        try
        {
            var token = await AuthStateProvider.GetToken();
            CurrentProject = await ProjectService.GetById(ProjectId, $"Bearer {token}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading project: {e.Message}");
        }
    }

    private async Task LoadCardsAsync()
    {
        try
        {
            var token = await AuthStateProvider.GetToken();
            
            _cards = (await CardService.GetAll($"Bearer {token}"))
                .OrderBy(c => c.RowIndex)
                .ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading cards: {e.Message}");
        }
    }

    private async Task LoadColumnsAsync()
    {
        try
        {
            _columns = (await ColumnService.GetByProjectId(ProjectId))
                .OrderBy(c => c.CreatedAtUtc)
                .ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading columns: {e.Message}");
        }
    }

    private void StartAddingCard(Guid columnId)
    {
        _activeColumnId = columnId;
        _newCard.Title = "";
    }

    private void CancelAddingCard()
    {
        _activeColumnId = null;
        _newCard.Title = "";
    }

    private void StartAddingColumn()
    {
        _isAddingColumn = true;
        _newColumn.Title = "";
    }

    private void CancelAddingColumn()
    {
        _isAddingColumn = false;
        _newColumn.Title = "";
    }

    private async Task CreateCardAsync(Guid columnId)
    {
        var newCardDto = new CreateCardDto
        {
            Title = _newCard.Title,
            ColumnId = columnId,
            RowIndex = GetNextRowIndex(columnId)
        };

        try
        {
            var token =  await AuthStateProvider.GetToken();
            var cardId = await CardService.CreateAsync(newCardDto, $"Bearer {token}");
            
            await LoadCardsAsync();

            try
            {
                var newState = new CreateStateDto
                {
                    CardId = cardId,
                    Status = Status.Pending,
                    Priority = Priority.Low,
                    Description = string.Empty
                };

                await StateService.CreateAsync(newState);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error creating states for the card: {e.Message}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error creating card: {e.Message}");
        }

        CancelAddingCard();
    }

    private async Task CreateColumnAsync()
    {
        var newColumnDto = new CreateColumnDto
        {
            Title = _newColumn.Title,
            ProjectId = ProjectId
        };

        try
        {
            await ColumnService.CreateAsync(newColumnDto);
            await LoadColumnsAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error creating column: {e.Message}");
        }

        CancelAddingColumn();
    }

    private async Task ItemDropped(DraggableDroppedEventArgs<CardDto> dropItem)
    {
        var card = dropItem.Item;
        var newColumnId = Guid.Parse(dropItem.DropZoneName);

        if (card.ColumnId != newColumnId)
        {
            card.ColumnId = newColumnId;
        }

        card.RowIndex = dropItem.IndexInZone;

        var affectedCards = _cards
            .Where(c => c.ColumnId == newColumnId && c.Id != card.Id)
            .OrderBy(c => c.RowIndex)
            .ToList();

        affectedCards.Insert(card.RowIndex, card);

        for (var i = 0; i < affectedCards.Count; i++)
        {
            affectedCards[i].RowIndex = i;
        }

        _cards = _cards
            .Where(c => c.ColumnId != newColumnId) // Keep other columns unchanged
            .Concat(affectedCards) // Insert updated column
            .ToList();

        var updateTasks = affectedCards.Select(async updatedCard =>
        {
            var updateDto = new UpdateCardDto
            {
                Id = updatedCard.Id,
                Title = updatedCard.Title,
                ColumnId = updatedCard.ColumnId,
                RowIndex = updatedCard.RowIndex
            };
            
            var token = await AuthStateProvider.GetToken();
            
            await CardService.UpdateAsync(updatedCard.Id, updateDto, $"Bearer {token}");
        });

        try
        {
            await Task.WhenAll(updateTasks);
            StateHasChanged();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error updating card order: {e.Message}");
        }
    }

    private int GetNextRowIndex(Guid columnId)
    {
        return _cards
            .Where(c => c.ColumnId == columnId)
            .Select(c => c.RowIndex)
            .DefaultIfEmpty(0)
            .Max() + 1;
    }

    private async Task ShowCardDetailsModal(CardDto cardDto)
    {
        _selectedCardId = cardDto.Id;
        await CardDetailsModalRef.ShowAsync();
    }

    private async Task HideCardDetailsModal()
    {
        await CardDetailsModalRef.Hide();
    }

    private void OnDeleteClicked(Guid columnId)
    {
        columnToDeleteId = columnId;
        _deleteConfirmationModal.Show();
    }

    private async Task OnInviteUserClicked(Guid userId)
    {
        var token = await AuthStateProvider.GetToken();

        var addUserCommand = new AddUserToProjectsDto()
        {
            ProjectId = ProjectId,
            UserId = userId
        };

        try
        {
            await ProjectService.AddUserToProjectAsync(ProjectId, addUserCommand, $"Bearer {token}");
        }
        catch (ApiException e)
        {
            if (e.StatusCode == HttpStatusCode.Forbidden)
            {
                await ShowErrorNotification("You don't have permission to add users to this project.");
            }

            Console.WriteLine($"Error adding user to project: {e.Message}");
        }
    }

    private async Task ShowInviteUserModal()
    {
        await InviteUserModalRef.Show();
    }

    private async Task HideInviteUserModal()
    {
        await InviteUserModalRef.Hide();
    }

    private async Task ConfirmDeleteCommentAsync()
    {
        var columnsHasCards = _cards.Any(c => c.ColumnId == columnToDeleteId);

        if (columnsHasCards)
        {
            await ShowErrorNotification("You cannot delete a column that contains cards.");
            return;
        }

        var token = await AuthStateProvider.GetToken();
        await ColumnService.DeleteAsync(columnToDeleteId, $"Bearer {token}");

        await LoadColumnsAsync();
    }

    private async Task ShowOffcanvas()
    {
        await OffCanvasMenuRef.ShowOffcanvasAsync();
    }

    private async Task HideOffcanvas()
    {
        await OffCanvasMenuRef.HideOffcanvasAsync();
    }

    private Task ShowErrorNotification(string message)
    {
        return NotificationService.Error(message);
    }
    
    private void StartEditingTitle()
    {
        IsEditingTitle = true;
        EditTitle = CurrentProject.Title;
    }
    
    private async Task HandleTitleKeyDown(KeyboardEventArgs e)
    {
        switch (e.Key)
        {
            case "Enter":
                CurrentProject.Title = EditTitle;
                await UpdateProjectTitleAsync();
            
                IsEditingTitle = false;
                break;
            case "Escape":
                IsEditingTitle = false;
                break;
        }
    }

    private async Task UpdateProjectTitleAsync()
    {
        var projectToUpdate = new UpdateProjectDto
        {
            Id = CurrentProject.Id,
            Title = EditTitle,
            Description = CurrentProject.Description,
            UserId = await AuthStateProvider.GetUserIdAsync()
        };

        try
        {
            var token = await AuthStateProvider.GetToken();
            await ProjectService.UpdateAsync(CurrentProject.Id, projectToUpdate, $"Bearer {token}");
            await LoadProjectAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error saving project title: {e.Message}");
        }
    }
}