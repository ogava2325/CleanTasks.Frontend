using Blazorise;
using Domain.Dtos.Card;
using Domain.Dtos.Column;
using Domain.Dtos.State;
using Microsoft.AspNetCore.Components;
using Services.External;
using UI.Components.Modals;

namespace UI.Components.Pages;

public partial class Kanban : ComponentBase
{
    [Inject] private ICardService CardService { get; set; }
    [Inject] private IStateService StateService { get; set; }
    [Inject] private IColumnService ColumnService { get; set; }
    [Parameter] public Guid ProjectId { get; set; }

    private List<CardDto> _cards = [];
    private List<ColumnDto> _columns = [];
   
    private readonly CreateCardDto _newCard = new();
    private readonly CreateColumnDto _newColumn = new();
    
    private bool _isAddingColumn;
    private Guid? _activeColumnId; 
    
    private Guid _selectedCardId;
    
    private CardDetailsModal _cardDetailsModalRef;
    
    protected override async Task OnInitializedAsync()
    {
        await LoadCardsAsync();
        await LoadColumnsAsync();
    }
    
    private async Task LoadCardsAsync()
    {
        try
        {
            _cards = (await CardService.GetAll())
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
            var cardId = await CardService.CreateAsync(newCardDto);
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
            await CardService.UpdateAsync(updatedCard.Id, updateDto);
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
    
    private async Task ShowModal(CardDto cardDto)
    {
        _selectedCardId = cardDto.Id;
        await _cardDetailsModalRef.ShowAsync();
    }

    private async Task HideModal()
    {
        await _cardDetailsModalRef.Hide();
    }
}