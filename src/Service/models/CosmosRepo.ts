import { Container, CosmosClient, FeedResponse } from '@azure/cosmos'
import { Address } from './Address';

export class CosmosRepo {
    private _container: Container;

    constructor(container: Container) {
        this._container = container;
    }
    
    public static async factory() : Promise<CosmosRepo> {
        let cosmosClient = new CosmosClient(process.env.CONN_STRING!);
        let db = await cosmosClient.databases.createIfNotExists({ id: process.env.DB_NAME! });
        let cont = await db.database.containers.createIfNotExists({ id: process.env.COLL_NAME! });
        return new CosmosRepo(cont.container);
    }

    public async getById(id: string) : Promise<Address> {
        return await (await this._container.item(id).read<Address>()).resource as Address;
    }

    public async queryByZip(zipCode: string) : Promise<Address[]> {
        let q: string = `SELECT * FROM c`; // All where clauses don't work here
        // https://docs.microsoft.com/en-us/javascript/api/@azure/cosmos/feedoptions?view=azure-node-latest#@azure-cosmos-feedoptions-partitionkey
        let c = await this._container.items.query(q, { partitionKey: zipCode }).fetchAll();

        return c.resources as Address[];
    }

    public async queryByState(state: string) : Promise<Address[]> {
        let q: string = `SELECT * FROM c WHERE c.state = ${state}`;
        let c: FeedResponse<Address>;
        
        try {
            c = await this._container.items.query<Address>(q).fetchAll();
        } catch (error) {
            return [];
        }

        return c.resources as Address[];
    }

    public async getAll() : Promise<Address[]> {
        return await (await this._container.items.readAll<Address>().fetchAll()).resources as Address[];
    }

    public async upsert(item: Address) : Promise<Address> {
        return await (await this._container.items.upsert<Address>(item)).resource as Address;
    }

    public async delete(item: Address) : Promise<boolean> {
        let resp = await this._container.item(item.id, item.state).delete<Address>();

        if (resp.statusCode == 200) {
            return true;
        } else {
            return false;
        }
    }
}