MessagePack data structures and WebSocket based communications for the sonosthesia project

## Fix Pack Dependency

Currently this package creates a dependency problem as it centralizes the need for MessagePack data structures for all packages which have a need for them. To fix this:

- Pack should contain the data structures for all packages which have a need with the Packed prefix
- Each package which has the need for message pack objects should not have Pack as a dependency as it is heavy but it should have a conditional compile which contain converters (Packed to non Packed) and asset importers (for Audio analysis file for example)