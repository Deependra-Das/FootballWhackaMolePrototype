# FootballWhackaMolePrototype
Football Swipe Shooting + Whack-a-Mole Timing game prototype developed in Unity

## Swipe-to-Velocity Mapping
The PlayerFootballController uses Unity Input Actions for touchscreen and mouse input to get the press position and release position, then calculates the swipe vector between them. Swipe direction determines the horizontal launch direction, while swipe distance determines the launch speed. The swipe value is clamped between minimum and maximum distances. A fixed vertical velocity is then added to create a consistent arc.

## Key Tunable Parameters
- PlayerFootballController exposes _minSwipeDistance, _maxSwipeDistance, _minLaunchSpeed, _maxLaunchSpeed, and _verticalLaunchSpeed. 
- GameplayManager exposes _sessionDuration, _moleSpawnInterval, and _fastMoleChanceRate to control game duration, mole spawn frequency, and fast-mole probability. 
- BaseMole exposes _popUpHeight, _popUpDuration, and _popDownDuration to control mole movement and animation timing.
- Both NormalMole & FastMole expose _visibleDuration & _score parameters to control the duration for which moles remain visible and the points that can be scored by hitting them using ball.


## Script/Component Structure
- PlayerFootballController handles input, swipe calculation, shooting, trajectory preview, and collision detection. 
- PlayerManager handles football spawning, respawning.
- GameplayManager controls the game session, mole spawning, scoring, and restart flow.
- BaseMole handles mole movement and hit detection while declaring some parameters abstract for children to define.
- FastMole & NomralMole inherits from BaseMole and defines _visibleDuration & _score parameters.
- ScoreService manages score updates and events.
- UIManager handles the gameplay UI to handle restart button interaction, updating score and timer text.
- GameplayFeedbackUIManager handles displaying reusable world-space hit and miss feedback at the collision location.
- GameManager is the root script that initializes and registers services using Service Locator and injects dependencies required.
- EventBusService is helping with managing events publication, subscription & unsubscription. 

## Production Refactoring
- I would smooth out the player ball controls & improve swiping logic.
- I would reduce singleton dependencies, and use more event-driven communication.
- I would introduce object pooling for feedback VFX and shadegraph to show feedback both on moles and ball.
- I would also separate input, physics, gameplay, and presentation logic further and add stronger validation and error handling some of which I ignored due to time constraint.

## Design Decisions
- I chose to keep the vertical launch velocity fixed while using swipe distance for shot strength. This makes the football's arc predictable while still giving the player control over direction and power.
- I chose to keep the hit feedback simple by using a world space canvas and moving it to place at the contact of the ball based on hit/miss.

## Gameplay Video
https://youtu.be/j_V3uXcal3I

