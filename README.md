# Azure Cosmos DB Cross-Partition Query Sidecar
Deploy components of an application into a separate process or container to provide isolation and encapsulation. This pattern can also enable applications to be composed of heterogeneous components and technologies.

## Problem
Currently (2022-05-12) the Azure Cosmos DB JavaScript SDK does not support the ability to query documents across partitions in a container. This is needed functionality for many of our customers and partners. After speaking with the team building the product and reviewing the road-map, it was decided that the best course of action is to hand off the query functionality to a sidecar until the feature is implemented at a later date. 

## Solution
This repository is a POC of the [Sidecar pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/sidecar) detailed in the Azure Architecture Center. 

![Sidecar Diagram](https://docs.microsoft.com/en-us/azure/architecture/patterns/_images/sidecar.png)

 1. Primary Application: TypeScript Service
 2. Sidecar: C# code that executes the cross-partition query

 The TS service is an HTTP endpoint that will upsert directly to the CosmosDB collection but will also call the Sidecar to query for requested information.
 
 Callstack:
 
 `index.ts` calls to the `CosmosRepositoryFactory.ts` and decides based on a flag to use the direct cosmos connection or to handoff requests to the sidecar.
 
 `index.ts` then accepts requests and calls into the `CosmosRepository.ts` that has the `SidecarService.ts` injected (this is over the `CosmosService.ts` based on the factory flag).
 
 It makes an HTTP call out to the Sidecar container.
 
 The sidecar controller hands off to the Cosmos service that issues the query to the Azure Service and returns the response back to the controller and back to the calling code in the `SidecarService.ts` as an HTTP response.