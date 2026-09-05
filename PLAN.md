# Working Game Development Plan

## Purpose

This is a working exploration and development plan.

It is deliberately not a fixed game specification.

The project should remain easy to redirect during its early stages. Camera behavior, visual style, exact construction mechanics, progression, island design, survival systems, automation, enemies and multiplayer details should remain open until prototypes give us evidence.

The central development principle is:

**First prove that building is enjoyable. Then prove that weather makes building meaningful. Only then build the larger survival and automation game around those systems.**

The project should grow through playable experiments and decision gates rather than through a large predetermined feature list.

---

# Current working vision

The current concept is a 3D, third-person construction and survival game set primarily on a finite island or similarly bounded environment.

Construction is the central activity.

The player builds using a modular, brick-inspired construction language. The system is inspired by the usefulness of construction toys rather than by exact LEGO geometry.

Pieces do not need:

* LEGO dimensions
* LEGO proportions
* LEGO studs
* LEGO appearance
* LEGO terminology

The important characteristics are:

* modular pieces
* predictable dimensions
* reliable connections
* reuse
* composability
* a clear construction vocabulary

Small primitive elements can be combined into reusable components.

Those components can then be combined into larger structures such as:

* houses
* bridges
* towers
* workshops
* mills
* storage buildings
* defensive structures
* machines

Eventually, building should progress from architecture into mechanical construction and automation.

Weather, particularly storms, becomes the initial external pressure.

The player should eventually progress approximately through:

```text
survive nature
      |
      v
understand nature
      |
      v
build around nature
      |
      v
harness nature
      |
      v
automate using nature
```

Enemies may eventually exist, but enemies and combat are deliberately not part of the early design.

---

# Platform priorities

Primary target:

**Windows desktop**

Secondary possibilities:

* Linux if supporting it is inexpensive
* macOS if supporting it is inexpensive
* browser if practical later
* other platforms only if there is eventually a reason

Browser support is now explicitly a nice-to-have.

Browser requirements must not dictate early technical decisions.

Mobile is not currently a target.

First-person perspective is not a target and should never be required.

---

# Technology decision

The provisional technology choice is:

**Unity + C#**

We will begin by making the actual prototype in Unity.

We will not initially duplicate the prototype in Bevy.

The prototype uses Unity 6000.6.0f1, the latest editor selected for this project.

## Why Unity currently wins

Unity provides mature support for most of the things this project is likely to need:

* Windows builds
* 3D scenes
* cameras
* physics
* colliders
* procedural meshes
* materials
* terrain
* particles
* audio
* profiling
* debugging
* scene editing
* prefab-like reusable content
* input handling
* editor extensions
* asset import
* eventual multiplayer

This lets development focus primarily on the game's unusual systems rather than rebuilding common 3D-engine infrastructure.

For AI-assisted development, Unity also has a major advantage: there is a large amount of mature C# and Unity knowledge available, and most common 3D game problems have established Unity-shaped solutions.

---

# Bevy remains the primary fallback

Bevy is not rejected.

It becomes the alternative if Unity produces real development friction.

Reasons Bevy remains attractive:

* Rust
* explicit source-driven architecture
* ECS
* strong compiler feedback
* very little hidden editor state
* excellent fit for systems-heavy simulations
* open-source engine
* native Windows builds
* possible browser support later

Bevy could become preferable if the project increasingly looks like:

```text
Storm
WeatherExposure
Structure
Connection
Material
Machine
PowerProducer
PowerConsumer
Pipe
Fluid
Storage
Resource
Damage
```

and Unity's object/editor model starts getting in the way.

We should reconsider Bevy only because of actual problems, not theoretical elegance.

---

# Godot remains a reserve option

Godot remains technically suitable.

It offers:

* a proper 3D engine
* strong editor integration
* open-source licensing
* C# or typed GDScript
* built-in high-level networking
* desktop and browser export possibilities

However, there is no current reason to prototype in three engines.

Godot should only be reconsidered if:

* Unity feels excessively heavy
* Bevy requires too much engine infrastructure
* Godot offers an obviously better solution to a problem we encounter

---

# Technologies not currently being considered

## Phaser

Not appropriate because the project is fundamentally 3D.

## Three.js

Possible, but would require us to construct considerably more game-engine infrastructure.

## Babylon.js

Still a good browser-first 3D technology, but its main advantage is less important now that Windows is the primary platform.

## PlayCanvas

Same reasoning as Babylon.js.

## Raw wgpu

Technically capable but strategically wrong.

We want to build a game, not spend the project building a rendering engine.

## Unreal

Not impossible, but currently offers complexity we do not need.

It should only be reconsidered if the project eventually develops requirements that strongly favor it.

---

# Unity development philosophy

Using Unity does not mean putting all game logic into MonoBehaviours and Inspector references.

We should deliberately make the Unity project friendly to:

* source control
* automated code generation
* AI-assisted coding
* unit testing
* debugging
* eventual networking
* possible future engine migration

The game should have a logical model that is cleaner than the Unity scene hierarchy.

Unity should provide:

* rendering
* physics
* audio
* input
* cameras
* editor
* platform abstraction

Our code should provide:

* construction rules
* structural simulation
* weather
* resources
* automation
* machines
* progression
* world state
* commands
* save/load logic

---

# Minimize hidden Unity state

Avoid building systems where important behavior depends upon dozens of undocumented Inspector values.

Prefer explicit configuration.

Use Inspector configuration where it genuinely makes sense, but make important values understandable and discoverable.

Potential tools include:

* ScriptableObjects for definitions
* serializable configuration classes
* generated test scenes
* editor validation
* custom inspectors where useful
* automated setup tools
* runtime debug views

We should avoid workflows that repeatedly require instructions such as:

```text
Create this GameObject.
Add these six components.
Drag this reference here.
Set this number to 3.71.
Change this obscure checkbox.
```

Where practical, we should automate repetitive editor setup.

---

# Unity editor tools

It may be useful to build our own small development menu early.

For example:

```text
Tools
  Stormstead
    Create Prototype Scene
    Reset Prototype Scene
    Generate Primitive Pieces
    Generate Materials
    Generate Test Structure
    Generate Stress-Test Structure
    Validate Piece Definitions
    Validate Connections
    Run Structural Test
```

The actual game name is not decided. "Stormstead" remains a temporary working name.

These tools should exist only when they reduce development friction.

We should not spend weeks building custom editor infrastructure.

---

# Keep simulation separate from presentation

A major architectural rule is:

**The visual object should not be the authoritative game state.**

For example, a building piece should logically be something similar to:

```text
PieceId
PieceType
Material
Position
Rotation
Connections
Damage
State
```

Its GameObject is its visual representation.

Similarly:

```text
World state
    |
    v
Unity representation
```

rather than:

```text
Unity hierarchy
    |
    v
implicitly becomes game state
```

This will help with:

* saves
* debugging
* rebuilding visual state
* structural simulation
* multiplayer
* testing
* optimization

---

# Stable identifiers

Important world entities should eventually have stable identifiers.

Examples:

```text
PieceId
StructureId
PlayerId
MachineId
ContainerId
LibraryComponentId
```

Do not rely on transient Unity instance IDs as persistent game identity.

This becomes important for:

* save files
* structural graphs
* machine networks
* multiplayer
* debugging

---

# Command-oriented architecture

User input should not directly modify authoritative game state.

Avoid:

```text
mouse click
    |
Instantiate wall
    |
remove wood
```

Prefer:

```text
mouse click
    |
    v
PlacePieceCommand
    |
    v
validation
    |
    v
game simulation
    |
    v
world state changes
    |
    v
Unity representation updates
```

Conceptually:

```text
PlacePieceCommand
{
    PlayerId
    PieceType
    Material
    Position
    Rotation
}
```

The simulation decides:

* whether placement is legal
* whether the player has resources
* whether connections are valid
* what state is created

This architecture is useful even in purely single-player development.

---

# Multiplayer readiness without multiplayer

Multiplayer should not be implemented during the early prototypes.

However, we should avoid architectural choices that make multiplayer unnecessarily difficult later.

The main rule is:

**Single-player actions should already pass through the same logical command and validation layer that a future network client would use.**

Single player:

```text
local input
    |
command
    |
local authoritative simulation
    |
world state
```

Future multiplayer:

```text
client input
    |
command
    |
network
    |
host authoritative simulation
    |
world state
    |
replication
```

This does not require networking code today.

It merely prevents input code from becoming inseparable from simulation code.

---

# Probable future multiplayer model

If multiplayer is eventually implemented, the default design should probably be small cooperative multiplayer.

Likely model:

```text
             HOST
      authoritative world
       /       |       \
      /        |        \
 Player A   Player B   Player C
```

One player can act as both:

* server/authority
* normal player

Dedicated servers should not be required initially.

Pure peer-to-peer simulation is not currently the preferred model.

Lockstep deterministic simulation is also not currently preferred.

---

# Host authority

The future host should probably determine authoritative state for:

* resource counts
* building validity
* placement
* removal
* structural integrity
* storms
* damage
* machines
* production
* storage
* enemies
* loot

Clients request actions.

The host validates them.

This is substantially easier to reason about than attempting to keep several independent structural/weather simulations perfectly synchronized.

---

# Unity multiplayer path

Unity currently provides Netcode for GameObjects as its high-level networking SDK for the GameObject/MonoBehaviour workflow. It sits above the underlying transport layer.

That makes it a logical future candidate for this game.

There is also Netcode for Entities, but there is currently no reason to adopt a DOTS networking architecture merely because it exists.

Do not install or design around networking packages during the early construction prototype unless doing so becomes necessary.

Networking is a later feature.

---

# Why this game may network relatively well

Most construction state is static most of the time.

A house containing thousands of pieces does not require thousands of positions to be transmitted every frame.

Network events could eventually look more like:

```text
Place piece 4817 at grid position X
Remove piece 318
Damage piece 972
Change container contents
Start machine
Stop machine
Piece 198 collapses
```

Player movement requires continuous synchronization.

Some machines may require synchronization.

Weather state requires synchronization.

But most buildings are event-driven.

This is favorable for eventual multiplayer.

---

# Networking reusable components

Player-created library components should also remain representable as data.

Example:

```text
LibraryComponent
{
    Id
    Name
    Pieces[]
}
```

A multiplayer placement could eventually conceptually be:

```text
PlaceAssemblyCommand
{
    LibraryComponentId
    Position
    Rotation
}
```

The authoritative simulation can expand that assembly into primitive pieces.

We should not transmit arbitrary generated meshes when a component can be represented by its logical definition.

---

# Current scope boundary for multiplayer

For now:

```text
MULTIPLAYER

Architecturally considered.
Not implemented.
```

Do not add:

* lobby systems
* Steam networking
* relay servers
* NAT traversal
* dedicated servers
* matchmaking
* prediction
* lag compensation
* synchronization code

until the single-player game has demonstrated that its core mechanics are enjoyable.

---

# Initial development stage

We will now begin directly in Unity.

There is no longer a mandatory Unity-versus-Bevy technology spike.

The Unity project itself is the first prototype.

It is intentionally disposable.

We should not hesitate to rewrite large parts of it.

---

# Initial Unity project goals

The first prototype should contain approximately:

```text
empty/simple terrain

directional light

simple sky

basic shadows

placeholder player

third-person camera

mouse picking

construction grid

ghost piece preview

rotation

placement

deletion

several primitive construction shapes
```

No gameplay loop yet.

No survival.

No resources.

No storm.

No enemies.

No crafting.

No inventory.

No multiplayer.

---

# First performance sanity test

Even the crude prototype should eventually test many pieces.

Initial target:

```text
1000+ visible construction pieces
```

This is not a performance specification.

It merely catches obviously bad architecture.

Later stress tests can increase this substantially.

---

# Camera laboratory

The camera is a core mechanic.

It must be prototyped rather than decided theoretically.

First person must never be necessary.

Possible camera experiments:

```text
F1  closer third-person
F2  medium third-person
F3  high third-person
F4  building orbit camera
F5  orthographic/isometric experiment
```

These are temporary experiments.

We may ultimately keep only one or two.

---

# Normal gameplay camera

Likely characteristics:

* third-person perspective
* player visible
* orbit around player
* controllable yaw
* constrained pitch
* zoom
* camera collision
* camera cannot enter first person
* enough elevation to inspect surroundings
* comfortable for extended play

Exact values are intentionally undecided.

---

# Building camera

Building may use a somewhat different camera.

Potential behavior:

* orbit around nearby construction
* zoom further out
* limited movement around the current construction area
* remain logically attached to the player's vicinity
* improve visibility of snap points
* possibly fade obstructing objects
* possibly temporarily hide roofs or walls when useful

The goal is to preserve the feeling of inhabiting a 3D world while removing unnecessary camera frustration during construction.

---

# Orthographic and 2.5D experiment

An orthographic or isometric camera remains worth testing.

It is no longer the expected final presentation.

Its value is experimental.

If it makes construction unexpectedly pleasant, parts of its behavior may influence building mode.

---

# Camera evaluation task

Test serious camera candidates using the same structure:

```text
two-floor house
interior staircase
pitched roof
chimney
balcony
porch
```

Evaluate:

* placing exterior walls
* working indoors
* attaching roofs
* modifying the back side of a structure
* building above the player
* working below an overhang
* moving around completed construction
* looking at the finished building as a place rather than a diagram

We should choose by playing, not by preference expressed beforehand.

---

# Player representation

The game needs a physical third-person player entity.

A conventional human character is deliberately avoided initially.

Reasons:

* modelling
* rigging
* walking cycles
* animation blending
* clothing
* attack animations
* visual consistency
* large asset pipeline

Prototype character:

```text
capsule
sphere
simple floating geometric object
```

Possible eventual direction:

**small non-humanoid guardian spirit**

Possible characteristics:

* central body
* floating fragments
* hovering motion
* light source or glow
* no legs
* minimal or no skeletal animation
* procedural movement
* building effects
* carried items floating nearby

The character design remains open.

---

# Procedural animation preference

Where appropriate, use mathematical or procedural animation instead of frame-heavy authored animation.

Examples:

* floating
* bobbing
* leaning
* rotating
* stretching
* fragment movement
* machinery
* wheels
* shafts
* gears
* swaying vegetation

This keeps the art problem aligned with programming.

---

# Visual exploration

The initial art direction should remain cheap to produce and deterministic.

The broad contrast is:

```text
NATURE

irregular
organic
low-poly
procedural variation


CONSTRUCTION

regular
precise
modular
geometric
```

The player's settlement gradually introduces geometric order into the island.

---

# Brick-inspired construction language

Construction is based on modular pieces.

The system should not attempt to copy LEGO exactly.

Potential experiments:

* completely studless pieces
* subtle connection marks
* square connection indicators
* recessed connection marks
* no visible connectors
* connectors visible only during building
* seams between pieces
* bevelled edges
* chunky proportions
* muted materials
* more toy-like colors
* more natural materials

Current preference:

**studless modular construction pieces with visible seams and slightly bevelled edges**

This remains experimental.

---

# Art pipeline

Avoid making hand-painted artwork a requirement.

Prefer:

* procedural geometry
* simple materials
* flat or restrained PBR materials
* vertex colors
* procedural variation
* bevels
* shadows
* ambient lighting
* fog
* particles
* procedural animation

Blender should remain available for exceptional cases.

It should not initially become the primary content-production workflow.

---

# Procedural natural assets

Nature can initially be created from simple geometry.

Examples:

```text
tree
    trunk cylinders
    low-poly foliage

rock
    distorted low-poly mesh

grass
    simple instanced geometry

water
    plane + material/shader

cloud
    simple procedural forms
```

Procedural generation provides stylistic consistency that independent AI-generated images do not.

---

# Terrain strategy

Do not begin with voxels.

The current game is primarily about construction, not arbitrary excavation.

Use:

* Unity terrain
* generated terrain meshes
* or another ordinary terrain solution

depending upon which works best during prototyping.

Voxel terrain would introduce unnecessary complexity:

* chunks
* chunk meshing
* dynamic collision
* storage
* lighting
* LOD
* potentially large world data

If unrestricted digging later proves essential, reconsider this decision.

---

# Finite world

The world should initially be finite.

A finite island encourages familiarity.

Eventually the player should recognize places such as:

```text
oak grove
windy hill
river bend
marsh
beach
sheltered valley
old bridge
```

The island should feel like somewhere the player lives rather than disposable procedural terrain.

Exact size remains open.

---

# Finite does not mean exhaustible

Resources should be capable of renewal.

Possible mechanisms:

* tree regrowth
* plant regrowth
* driftwood
* storm debris
* fallen trees
* exposed resources
* washed-up objects

A finite environment can remain viable indefinitely.

---

# Environmental change

Storms may create small persistent changes.

Potential examples:

```text
fallen tree
new driftwood
temporary flooding
lightning damage
exposed rock
washed-up wreckage
minor erosion
```

Avoid major terrain deformation initially.

The purpose is to make the island feel alive, not to build a geological simulation.

---

# Construction hierarchy

The building system should distinguish three levels.

```text
primitive pieces
      |
      v
reusable library components
      |
      v
complete structures
```

This is one of the most important current ideas.

---

# Primitive construction grammar

Primitive pieces form the basic vocabulary.

Illustrative examples:

```text
brick
long brick
plate
beam
pillar
panel
slope
corner
cylinder
rod
```

Possible logical dimensions:

```text
brick       1 x 1 x 1
long brick  1 x 1 x 2
beam        1 x 1 x 4
pillar      1 x 1 x 3
plate       2 x 2 x 0.25
panel       4 x 0.25 x 3
slope       2 x 2 x 1
```

These dimensions are examples only.

The actual construction unit must be tested.

---

# Construction scale experiment

A major early question is how granular construction should be.

Too small:

* tedious
* enormous piece counts
* slow building
* difficult camera interaction

Too large:

* insufficient creativity
* prefab-like
* loses construction-toy feeling

We should test at least:

* small brick scale
* medium architectural module scale
* mixture of both

The final answer may be hierarchical.

---

# Shapes and materials are separate

Conceptually:

```text
shape = beam
material = wood
```

rather than every material requiring a completely unrelated piece definition.

Materials may eventually affect:

* appearance
* mass
* support
* connection strength
* durability
* fire resistance
* water resistance
* temperature behavior
* resource cost

Do not implement every material property initially.

---

# Reusable player library

This should be prototyped surprisingly early.

The player can select several primitive pieces and save the selection as a reusable component.

Example:

```text
primitive pieces
      |
      v

four beams
two plates
one slope
      |
      v

SAVE AS COMPONENT
      |
      v

"Small Roof Support"
```

The component can later be placed repeatedly.

Internally, it remains composed of primitives.

---

# Example personal library

The player might eventually create:

```text
Small Window
Large Window
Roof Support
Roof Corner
Stone Arch
Basic Chimney
Tall Chimney
Staircase
Porch Section
Bridge Support
Warehouse Door
Pump Assembly
Windmill Gearbox
```

These are the player's own designs.

This reduces repetitive work without replacing creativity.

---

# Library editing questions

Future questions include:

* can components be edited?
* can components contain other components?
* can they be mirrored?
* can they be rotated?
* can they be parameterized?
* does changing a definition update existing placed copies?
* can placed copies diverge?
* can the player share component definitions?

Do not decide all of these now.

---

# Built-in components

There may eventually be both:

```text
built-in components
```

and:

```text
player-created components
```

Built-in examples:

* simple wall
* basic fireplace
* door
* basic roof
* workbench

Player-created examples:

* customized windows
* unusual roof supports
* machine assemblies
* decorative architecture

Whether built-in components are necessary remains open.

---

# Discovery as construction vocabulary

Progression may unlock new construction concepts rather than simply stronger equipment.

Examples:

```text
hinge
bearing
axle
pipe
pipe elbow
valve
gear
pulley
grating
spring
arch
new slope
```

A small newly discovered component may unlock many possibilities.

Example:

```text
BEARING
   |
   +-- water wheel
   +-- windmill
   +-- crane
   +-- grindstone
   +-- sawmill
```

Another:

```text
PIPE ELBOW
   |
   +-- rainwater collection
   +-- drainage
   +-- irrigation
   +-- pumping
   +-- reservoirs
```

This could make exploration partly about discovering new construction vocabulary.

---

# How discoveries occur

Possible mechanisms:

* exploration
* ruins
* experimentation
* milestones
* storms
* washed-up debris
* research
* crafting discoveries

Do not decide the system yet.

---

# Initial construction prototype

After basic movement and camera work, construction becomes the first serious feature.

Target functionality:

```text
piece selection
ghost preview
grid snapping
connection snapping
rotation
placement
removal
copy
multi-selection eventually
save/load
reusable component creation
```

Initially:

* infinite resources
* no gathering
* no crafting costs
* no inventory restrictions

The construction mechanic must prove itself independently.

---

# Initial primitive set

Approximately 10 to 20 pieces should be enough.

Potential starting set:

```text
cube
half-height block
long block
beam
pillar
plate
wall panel
small slope
large slope
corner slope
cylinder
rod
```

Do not create dozens of pieces before understanding the grammar.

---

# Snapping

Snapping may be more important than piece variety.

Possible modes:

* world grid
* surface snap
* connection-point snap
* fixed-angle rotation
* temporary snap override
* free rotation where appropriate

The system should help the player without constantly refusing reasonable constructions.

---

# Construction test

The prototype should be tested by building something intentionally more complex than a cube.

Suggested structure:

```text
two-storey house
interior staircase
pitched roof
chimney
balcony
porch
small bridge
tower
```

Then deliberately modify it.

Tests:

* extend room
* move staircase
* replace roof
* repeat windows
* remove wall
* rebuild balcony
* copy structural supports
* save and reuse assemblies

Success criterion:

**After completing the required test structure, we still voluntarily want to continue building.**

If not, remain here.

Do not add storms to disguise weak building mechanics.

---

# Structural integrity

Structural integrity comes after construction feels good.

Do not initially simulate realistic engineering.

Structures should use a simplified logical graph.

Conceptually:

```text
ground
  |
foundation
  |
pillar
  |
beam
 /  \
roof roof
```

Possible properties:

```text
mass
support capacity
connection strength
material
orientation
weather exposure
damage
```

Support can propagate through connected pieces.

---

# Structural debugging

During development, visualize support.

Possible representation:

```text
green   strong
yellow  marginal
red     weak
```

The system must be understandable.

If developers cannot explain why a roof collapsed, players certainly cannot.

This debug mode may later evolve into an in-game construction tool.

---

# Physics is presentation

Do not use Unity's rigid-body simulation as the authority deciding whether buildings are structurally valid.

Instead:

```text
logical structural simulation
        |
        v
connection fails
        |
        v
piece detaches
        |
        v
Unity physics makes debris fall
```

Simulation determines what happened.

Physics makes it visually satisfying.

---

# Weather prototype

Once building and structural integrity work, add exactly one controllable storm.

Initial development control can literally be:

```text
START STORM
```

Parameters:

```text
wind direction
wind strength
rain intensity
duration
```

Nothing else is necessary initially.

---

# Storm design principle

Damage should be explainable.

Bad:

```text
10 percent random chance that wall disappears
```

Good:

```text
wind comes from northwest
roof has high exposure
beam support is inadequate
connection overload occurs
roof section fails
```

The player should be able to learn from damage.

---

# Architecture as gameplay

Examples of lessons storms might teach:

* roof span is too long
* entrance faces prevailing wind
* chimney lacks support
* roof angle handles snow poorly
* foundation is too low
* drainage is insufficient
* exposed wall needs reinforcement
* storage building is badly sheltered

The desirable reaction after a storm is:

**I know how I want to rebuild this.**

---

# Weather systems for later

Possible later additions:

* stronger wind
* rain
* temperature
* snow
* snow load
* flooding
* drainage
* lightning
* fire
* waves
* storm surge
* falling trees

Add them individually.

Each weather system must create interesting building decisions.

---

# First actual gameplay loop

Only after construction and storms interact successfully should resource constraints arrive.

Potential loop:

```text
calm
  |
  v
explore
  |
  v
gather
  |
  v
build
  |
  v
forecast
  |
  v
prepare
  |
  v
storm
  |
  v
inspect
  |
  v
repair / redesign
  |
  +------> calm
```

This is the point where the construction experiment begins becoming a game.

---

# Resources

Keep early resources few.

Likely:

```text
wood
stone
```

Possible later additions:

```text
fiber
clay
metal
glass
```

Avoid dozens of interchangeable crafting materials.

Complexity should come primarily from what can be built.

---

# Gathering

Initial gathering should be simple.

Examples:

```text
tree -> wood
rock -> stone
driftwood -> wood
```

Gathering itself does not need to become a complicated action game.

---

# Survival

Possible early survival mechanics:

```text
shelter
temperature
fire
weather exposure
```

Hunger is not required.

Survival systems should exist because they make buildings meaningful, not because other survival games have them.

---

# Nature becomes technology

Later progression should convert environmental threats into tools.

Wind:

```text
early
    damages structures

later
    powers windmill
```

Rain:

```text
early
    exposure
    flooding
    cold

later
    roof collection
    reservoirs
    irrigation
```

Water:

```text
early
    obstacle
    flooding

later
    water wheel
    pumping
```

Fire:

```text
early
    survival heat

later
    furnace
    processing
    metallurgy
```

This should guide progression more strongly than a conventional arbitrary technology tree.

---

# Automation

Automation is a later major direction.

Potential systems:

```text
water wheel
windmill
axle
bearing
shaft
gear
belt
pump
pipe
storage
furnace
sawmill
grain mill
workshop
crane
reservoir
```

Moving machinery also makes settlements feel alive without requiring humanoid NPCs.

---

# Mechanical chains

Example:

```text
river
  |
water wheel
  |
shaft
  |
gearbox
  |
shaft
  |
sawmill
```

Water system:

```text
rain
  |
roof
  |
gutter
  |
tank
  |
pump
  |
pipe
  |
irrigation
```

The simulation does not need to model real engineering perfectly.

It should be visually and logically understandable.

---

# Machines should fit the construction language

Where practical, machines should be assemblies rather than unrelated magical objects.

For example:

```text
water wheel
    wheel
    axle
    bearings
    frame
```

This unifies:

* architecture
* construction
* progression
* mechanical systems
* visual language

Exactly how much mechanical freedom is appropriate remains open.

We must avoid accidentally building CAD software.

---

# Enemies

Enemies remain deferred.

Current plan:

```text
ENEMIES

Probably later.
Do not design yet.
```

When enemies eventually appear, they should reinforce building.

Possible characteristics:

* attack structures
* threaten resources
* react to weather
* react to heat
* require defensive architecture

Prefer creature designs that do not create large humanoid animation requirements.

Possible movement styles:

* floating
* rolling
* hopping
* burrowing
* slithering

---

# Combat

Combat is not assumed.

Do not implement weapons merely because this resembles a survival game.

Weather is the initial antagonist.

If the game later clearly benefits from combat, design it then.

---

# Audio

Audio eventually becomes important for atmosphere.

Potential sounds:

* wind
* rain
* thunder
* waves
* fire
* wood creaking
* stone impact
* stressed structures
* water wheels
* gears
* machinery
* forests

Audio can provide significant atmosphere without creating additional visual asset problems.

Music comes later.

---

# Save architecture

Save/load should be introduced relatively early.

Do not serialize arbitrary Unity object graphs as the long-term save format.

Prefer logical data.

Possible state:

```text
world identifier
world generation parameters
piece definitions
piece instances
piece damage
materials
player position
player library
containers
machines
world changes
resource state
```

A building piece should be reproducible from data.

---

# Rebuilding visual state

Ideally:

```text
save data
    |
    v
logical world
    |
    v
Unity GameObjects / meshes
```

This means the visual scene can be reconstructed from authoritative world state.

This architecture is useful for both saves and eventual networking.

---

# Data-driven content

Piece types and materials should eventually be mostly data-defined.

Conceptual piece definition:

```text
id
name
geometry type
dimensions
snap points
connection types
material compatibility
structural properties
resource cost
tags
```

This lets us add construction vocabulary without scattering special cases throughout gameplay code.

---

# Performance architecture

Do not optimize prematurely.

However, do not assume every visible primitive will permanently require:

```text
one GameObject
one MonoBehaviour
one Update()
```

Possible later strategies:

* GPU instancing
* mesh batching
* static structure merging
* chunking
* spatial partitioning
* sleeping inactive systems
* event-driven structural updates
* simplified distant rendering

The logical piece model should not depend upon the final rendering optimization.

---

# Avoid per-frame simulation where events suffice

Most systems should react to changes rather than constantly recompute everything.

Example:

Structural integrity recalculates when:

* piece added
* piece removed
* piece damaged
* storm load changes significantly
* structural connection changes

Not necessarily every frame.

Similarly, machine networks should preferably update based on state changes or controlled simulation ticks.

This helps both performance and future networking.

---

# Rendering hybrid possibility

The logical construction grid and visible result do not have to remain identical forever.

Internally:

```text
[wood][wood][wood][wood]
[wood][wood][wood][wood]
[wood][wood][wood][wood]
```

Later rendering could represent this as a cleaner unified wall.

When damaged, individual modules could become visible again.

This might eventually produce a more natural architectural appearance while retaining simple construction data.

Do not implement this initially.

---

# Island generation

Do not begin with a sophisticated procedural world generator.

The first environment only needs enough terrain variation for construction.

Possible components:

```text
coast
hill
forest
clearing
water
possibly river
```

Later, generation may create finite islands with recognizable regions.

Infinite generation is not the goal.

---

# Development sequence

The stages below are decision gates rather than promises.

---

# Unity foundation prototype

Deliver:

```text
Unity 6000.6.0f1 project
Windows build
basic scene
player placeholder
third-person camera
lighting
simple terrain
basic input
basic debug infrastructure
```

Questions:

* Is Unity pleasant enough to work with?
* Does the source/editor balance feel manageable?
* Can we keep important state explicit?
* Can AI-assisted iteration work efficiently?

If Unity causes significant architectural or workflow friction, this is the first opportunity to reconsider Bevy.

---

# Camera exploration

Deliver:

```text
several switchable third-person cameras
camera collision
building camera experiment
orthographic experiment
```

Stop when:

We have at least one camera setup that feels suitable for extensive construction.

---

# Visual construction experiment

Deliver:

```text
several primitive shapes
several material treatments
seams
bevel experiments
connection indicator experiments
nature/construction contrast
```

Questions:

* studs or no studs?
* how chunky?
* how visible are seams?
* how toy-like?
* how natural?
* what scale feels right?

---

# Construction toy

Deliver:

```text
piece palette
ghost placement
snap
rotate
delete
copy
save/load
10-20 primitives
several materials
player-created reusable components
```

No resource limitations.

Stop when:

**Building itself is entertaining.**

---

# Structural prototype

Deliver:

```text
connection graph
support calculation
structural visualization
piece failure
physics debris
```

Stop when:

Weak and strong construction choices produce predictable results.

---

# Storm prototype

Deliver:

```text
wind
rain
storm controls
structural loading
damage
visual effects
audio if useful
```

Stop when:

Storms make us want to improve our buildings.

---

# Finite island loop

Deliver:

```text
small island
renewable resources
basic gathering
forecast
preparation
storm
repair
```

Stop when:

Repeated calm/storm cycles remain interesting.

---

# Survival depth

Potential additions:

```text
fire
shelter
temperature
drainage
water
additional materials
```

Only add systems that improve construction decisions.

---

# Harnessing nature

Potential additions:

```text
rain collection
windmill
water wheel
pump
basic mechanical power
```

Stop when:

Technology creates meaningful new architecture.

---

# Automation

Potential additions:

```text
power transmission
shafts
gears
belts
pipes
storage
production machines
processing chains
```

Stop periodically and verify that the game remains understandable.

---

# Multiplayer experiment

Only after the single-player core works.

Possible first multiplayer experiment:

```text
one host
one client

both can:
    move
    place a piece
    remove a piece
    see the same structure
```

Nothing else.

Then gradually synchronize:

```text
resources
damage
storms
machines
storage
```

Do not attempt to network the complete game in one step.

---

# Later expansion

Only after the preceding systems produce an enjoyable game:

```text
enemies
defenses
combat if useful
additional island types
more weather
larger progression
story or lore
browser build
Linux/macOS polish
multiplayer services
```

---

# Decision gates

## Unity gate

Is Unity helping more than it is hindering?

If yes, continue.

If no, identify the exact problem and evaluate whether Bevy solves it.

Do not migrate based purely on aesthetic preference for Rust.

---

# Camera gate

Can detailed construction be performed comfortably without entering first person?

Current result:

* F2 medium third-person is the default general camera.
* F1 close third-person is retained for detailed work.
* F3 high, F4 building orbit, and F5 isometric were less effective and remain experiments rather than primary modes.

---

# Construction scale gate

Should players place:

* small bricks
* medium modules
* large components
* or a mixture?

---

# Visual style gate

Should pieces visibly remain construction blocks, or should rendering increasingly disguise them as natural building materials?

---

# Library gate

Does creating reusable components feel like an important creative mechanic?

If yes, promote it to a central progression system.

---

# Structural gate

Does structural integrity create interesting architectural decisions?

If it mostly creates irritation, simplify it.

---

# Storm gate

Does weather create reasons to redesign?

If storms merely destroy work, redesign storms.

---

# Resource gate

Do resources make construction decisions interesting?

If they only create grinding, simplify gathering.

---

# Automation gate

Does automation create interesting architecture and satisfying systems?

Avoid unnecessary engineering complexity.

---

# Multiplayer gate

Does the game actually benefit from multiplayer enough to justify the additional complexity?

Multiplayer capability is desirable.

Multiplayer implementation is not mandatory.

---

# Enemy gate

Does the existing game need enemies?

Do not assume the answer is yes.

---

# Explicit current decisions

The following are currently considered decided enough to work from:

```text
Windows first

3D

third-person

no required first-person perspective

Unity first

C#

finite environment

construction is central

modular brick-inspired construction language

player-created reusable component library

nature and construction visually distinct

weather eventually tests structures

structural simulation should be logical, not pure physics

automation eventually matters

humanoid artwork is avoided early

multiplayer should be architecturally possible later
```

---

# Explicit non-decisions

These remain open:

| Topic                       | Status                     |
| --------------------------- | -------------------------- |
| Final engine forever        | Open                       |
| Bevy migration              | Only if justified          |
| Exact camera                | Open                       |
| Exact construction scale    | Open                       |
| Studs                       | Open                       |
| Exact block proportions     | Open                       |
| Final visual style          | Open                       |
| Final player appearance     | Open                       |
| Island size                 | Open                       |
| Island generation algorithm | Open                       |
| Resource list               | Open                       |
| Hunger                      | Probably unnecessary early |
| Detailed survival model     | Open                       |
| Tech tree                   | Open                       |
| Discovery system            | Open                       |
| Machine complexity          | Open                       |
| Enemies                     | Deferred                   |
| Combat                      | Deferred                   |
| Multiplayer implementation  | Deferred                   |
| Multiplayer player count    | Open                       |
| Dedicated server            | Not planned initially      |
| Story                       | Open                       |
| Browser                     | Nice-to-have later         |
| Mobile                      | Not targeted               |
| Voxel terrain               | Not planned                |
| Full terrain deformation    | Not planned                |

---

# Explicitly outside early scope

Avoid:

* multiplayer implementation
* Steam integration
* matchmaking
* dedicated servers
* peer-to-peer networking work
* mobile controls
* giant worlds
* infinite terrain
* humanoid NPC systems
* large animation pipelines
* elaborate combat
* huge crafting trees
* hundreds of resources
* realistic structural engineering
* realistic fluid dynamics
* custom renderer
* voxel terrain
* procedural generation for its own sake
* elaborate story
* quest systems
* browser optimization

until the core has demonstrated value.

---

# Testing philosophy

Each development stage should answer questions.

Completing features is not sufficient.

---

# Construction questions

```text
Do I enjoy placing pieces?

Is snapping predictable?

Does the camera fight me?

Are pieces too small?

Are they too large?

Do reusable components reduce boring repetition?

Do reusable components retain creativity?

Do I want to keep building after the test is finished?
```

---

# Visual questions

```text
Does the modular style look intentional?

Does it look too much like a toy?

Does nature contrast pleasantly with construction?

Are materials attractive without an expensive art pipeline?

Does the world look good from normal gameplay distance?
```

---

# Structural questions

```text
Can I predict which structure is weak?

Can I understand why something failed?

Can I deliberately reinforce it?

Does structural integrity encourage creativity?

Or does it mostly restrict creativity?
```

---

# Storm questions

```text
Did the forecast change how I prepared?

Did the storm expose an architectural weakness?

Did damage make sense?

Did I want to redesign afterward?

Was repair satisfying rather than tedious?
```

---

# Resource questions

```text
Do resources create meaningful choices?

Do they encourage exploration?

Or do they merely stop me from building?
```

---

# Automation questions

```text
Does automation create new architectural goals?

Do moving machines make the settlement feel alive?

Are mechanical relationships understandable?

Is the system deep without becoming tedious?
```

---

# Multiplayer architecture questions

Even before networking exists, periodically ask:

```text
Can this world change be represented as a command?

Is authoritative state separate from visual state?

Does this entity have stable identity?

Could this change be replicated without transmitting an entire scene?

Does this gameplay system silently depend on local input?
```

If the answers remain good, later networking will be substantially easier.

---

# Core architectural rule

Where there is a choice between:

```text
large handcrafted asset requirement
```

and:

```text
deterministic system generating consistent results
```

prefer the deterministic system unless it clearly harms gameplay.

Examples:

* procedural geometry
* procedural vegetation
* reusable construction assemblies
* procedural materials
* mathematical animation
* structural graphs
* data-driven piece definitions
* simple non-humanoid creatures
* mechanically driven machine animation

The difficult parts of this project should primarily be things we can program.

---

# Immediate next milestone

The next milestone is deliberately tiny.

Create a Unity 6000.6.0f1 Windows project containing:

```text
simple terrain

directional light

basic sky

placeholder player

third-person camera

basic movement

mouse raycast

construction ghost

one cube-like primitive

one beam

one plate

one slope

grid/connection snapping

rotation

place

delete
```

No inventory.

No resources.

No storm.

No survival.

No multiplayer.

No polished graphics.

No procedural island.

No final architecture.

Then add several selectable camera experiments.

Then begin testing construction scale and visual style.

The first meaningful success is not:

```text
"We have implemented a survival game."
```

It is:

```text
"I built a completely unnecessary little structure
because placing and combining these pieces was enjoyable."
```

Everything else depends upon reaching that point.
