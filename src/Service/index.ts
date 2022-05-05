import fastify from 'fastify'
import * as dotenv from 'dotenv'
import { ICosmosRepository } from './repositories/ICosmosRepository'
import { cosmosRepositoryFactory } from './factories/CosmosRepositoryFactory'
import { Address } from './models/Address'

dotenv.config()
const server = fastify()
const repository: ICosmosRepository = cosmosRepositoryFactory();

server.post('/upsert', async (request, reply) => {
    const addresses = request.body as Address[];
    addresses.forEach(address => {
        repository.upsert(address);
    });

    reply.status(200).send();
});

server.get('/queryByZip', async (request, reply) => {
    const zip = (request.body as Address).zipCode;
    const addresses = repository.queryByZip(zip);
    
    reply.status(200).send(addresses);
});

server.get('/queryByState', async (request, reply) => {
    const state = (request.body as Address).state;
    const addresses = repository.queryByState(state);
    
    reply.status(200).send(addresses);
});

server.listen(8080, '0.0.0.0', (err, address) => {
    if (err) {
        console.error(err)
        process.exit(1)
    }
    console.log(`Server listening at ${address}`)
})
