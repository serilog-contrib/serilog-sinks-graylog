# serilog-sinks-graylog-fork

## What
This is a fork of serilog-sinks-graylog that aims to provide a stable release schedule and enteprise grade performance, as well as a semver based versioning strategy.

# Notable changes include
1. Based on Serilog's PeriodicBatchingSink 

1. Instance based Transport clients (UDP/HTTP). These clients are in scope for the duration of the logger's lifetime. The original implementation created and disposed of a new client per message send, which created a mass allocation of local ephemeral ports, causing issues with enterprise grade firewalls and load balancers.

1. Message property name normalization. Property names on GELF messages can be normalized to a variety of formats (as is, Camel-case, etc.).

1. Overall simplification and reduction of unnecessary project abstractions.
