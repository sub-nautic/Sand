using Playground.Player.Interaction;

public class WalkingAreaMarker : Interactable
{
    private Game game;

    private const float MIN_INTERACTION_AMOUNT = 0.75f;

    private void Awake()
    {
        InteractionStartEvent += OnInteractionStartEvent;
        HoverStartEvent += OnHoverStartEvent;

        game = FindObjectOfType<Game>();
    }

    private void OnHoverStartEvent(IInteractor interactor)
    {
        if (interactor.InteractionAmount > MIN_INTERACTION_AMOUNT)
        {
            game.GameCharacter.MovementInteractionEvent?.Invoke(interactor.HoverHitPosition);
        }
    }

    private void OnInteractionStartEvent(IInteractor interactor)
    {
        game.GameCharacter.MovementInteractionEvent?.Invoke(interactor.HitPosition);
    }
}
