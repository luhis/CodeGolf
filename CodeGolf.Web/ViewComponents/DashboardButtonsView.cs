namespace CodeGolf.Web.ViewComponents
{
    using CodeGolf.Service.Dtos;

    using Microsoft.AspNetCore.Mvc;

    using Optional;

    public class DashboardButtonsView : ViewComponent
    {
        private static ButtonState GetState(Option<HoleDto> hole)
        {
            return hole.Match(
                isHole =>
                    {
                        if (isHole.ClosedAt.HasValue)
                        {
                            if (isHole.HasNext)
                            {
                                return ButtonState.NextHole;
                            }
                            else
                            {
                                return ButtonState.EndGame;
                            }
                        }
                        else
                        {
                            return ButtonState.EndHole;
                        }
                    },
                () => ButtonState.StartGame);
        }

        public IViewComponentResult Invoke(Option<HoleDto> hole)
        {
            var state = GetState(hole);
            return this.View(state);
        }
    }

    public enum ButtonState
    {
        StartGame,
        EndHole,
        NextHole,
        EndGame,
        Reset
    }
}