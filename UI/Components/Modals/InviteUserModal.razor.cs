using System.Net;
using Blazorise;
using Blazorise.DataGrid;
using Domain.Dtos.Project;
using Domain.Dtos.Shared;
using Domain.Dtos.User;
using Domain.Enums;
using Microsoft.AspNetCore.Components;
using Refit;
using services.External;
using UI.Services;

namespace UI.Components.Modals;

public partial class InviteUserModal : ComponentBase
{
    [Parameter] public Guid ProjectId { get; set; }
    [Inject] public IMessageService MessageService { get; set; }
    [Inject] public IToastService ToastService { get; set; }
    [Inject] public IProjectService ProjectService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public INotificationService NotificationService { get; set; }
    [Inject] public CustomAuthStateProvider AuthStateProvider { get; set; }
    
    public Guid CurrentUserId { get; set; }
    
    private Modal ModalRef;

    private List<ProjectMemberModel> Users { get; set; } = new();
    private int TotalUsers { get; set; }
    private ProjectMemberModel selectedMember;
    private string InviteEmail { get; set; } = string.Empty;
    private string selectedValue;

    private Role SelectedRoleOption { get; set; } = Role.Viewer;

    private PaginationParameters PaginationParameters { get; set; } = new();

    protected override async Task OnInitializedAsync() => await LoadProjectMembersAsync();

    private async Task LoadProjectMembersAsync()
    {
        try
        {
            var token = await AuthStateProvider.GetToken();
            var result = await ProjectService.GetProjectMembers(ProjectId, PaginationParameters, $"Bearer {token}");
            Users = result.Items;
            TotalUsers = result.TotalCount;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading project members: {ex.Message}");
        }
    }

    private async Task InviteMemberAsync()
    {
        try
        {
            var token = await AuthStateProvider.GetToken();
            var command = new AddUserToProjectCommandDto
            {
                Email = InviteEmail,
                ProjectId = ProjectId,
                Role = SelectedRoleOption.ToString()
            };

            var response = await ProjectService.AddUserToProjectAsync(ProjectId, command, $"Bearer {token}");
            if (response.IsSuccess)
            {
                await LoadProjectMembersAsync();
                await ShowSuccessToast($"{command.Email} invited successfully");
            }
            else
            {
                await ShowErrorNotification(response.Error);
            }

            InviteEmail = string.Empty;
            SelectedRoleOption = Role.Viewer;
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
        {
            await ShowErrorNotification("You do not have permission to perform this action.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inviting member: {ex.Message}");
        }
    }

    private async Task OnRowRemoved(ProjectMemberModel member)
    {
        try
        {
            if (!await MessageService.Confirm($"Are you sure you want to remove {member.Email}?", "Confirm"))
            {
                return;
            }

            var token = await AuthStateProvider.GetToken();
            var userId = await AuthStateProvider.GetUserIdAsync();
            
            var command = new RemoveUserFromProjectDto
            {
                UserId = member.Id, 
                ProjectId = ProjectId,
                CurrentUserId = userId
            };
            
            var response = await ProjectService.RemoveUserFromProjectAsync(ProjectId, command, $"Bearer {token}");

            if (response.IsSuccess)
            {
                await ShowSuccessToast($"{member.Email} removed successfully");
            }
            else
            {
                await ShowErrorNotification(response.Error);
            }

            await LoadProjectMembersAsync();
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
        {
            await ShowErrorNotification("You do not have permission to remove this user.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing member: {ex.Message}");
        }
    }

    private async Task OnRowUpdated(SavedRowItem<ProjectMemberModel, Dictionary<string, object>> args)
    {
        try
        {
            var token = await AuthStateProvider.GetToken();
            var command = new ChangeUserRoleDto
            {
                UserId = args.Item.Id,
                ProjectId = ProjectId,
                Role = selectedValue
            };

            await ProjectService.ChangeUserRoleAsync(ProjectId, args.Item.Id, command, $"Bearer {token}");
            await LoadProjectMembersAsync();
            await ShowSuccessToast($"{args.Item.Email} role updated successfully");
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
        {
            await ShowErrorNotification("You do not have permission to change this user's role.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating member role: {ex.Message}");
        }
    }

    private Task OnSelectedValueChanged(string value, ProjectMemberModel member)
    {
        selectedValue = value;
        return Task.CompletedTask;
    }

    private Task ShowErrorNotification(string message)
    {
        return NotificationService.Error(message);
    }

    private Task ShowSuccessToast(string message)
    {
        return ToastService.Success(message);
    }
    
    public async Task Show()
    {
        await ModalRef.Show();
    }

    public async Task Hide()
    {
        await ModalRef.Hide();
    }
}