# InfinityLoop

## This is a joint connecting game demo with 3 levels and 2 difficulty modes after the completion.

- The start screen has only one infinity button that starts the first level
- The user is presented with the next level upon completion
- The initial, free playing game mode progress is saved and once the user completes a level, exists the app and presses play again, he will see the gamne level he hasn't completed yet
- Once the user completes all 3 levels, new buttons will appear on the main menu
  - Step button that lets the user play in "step" restricted game mode (one rotation counts as one step)
  - Clock button that lets the user play in time restricted way game mode
- Normal game mode without the fail condition is also still available

## This project was completed in very limited time for demonstration purposes and has many code and structure HACK's.
- The main LevelManager script is too bloated and should be refractored
- The click events should be substituted for mobile touch events
- Custom Unity events should be implemented for win condition, fail condition, etc.
- Framework for random level generation and sprite swapping would be nice to have
