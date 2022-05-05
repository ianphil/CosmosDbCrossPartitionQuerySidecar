import { Container, CosmosClient, PartitionKey } from '@azure/cosmos'
import { City } from './City';

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

    public async get(id: string) : Promise<any> {
        let q: string = `SELECT * FROM c`;
        let c1 = await this._container.items.query(q, { partitionKey: "GA" }).fetchAll();
        // return c.resources[0] as City;

        let c = await this._container.item("1", "GA").read<City>();
        
        if (c.statusCode === 200) {
            return c.resource as City;
        } else {
            console.log(`Trouble getting id ${id} from cosmos.`)
        }
    }

    public async getAll() : Promise<City[]> {
        return await (await this._container.items.readAll<City>().fetchAll()).resources as City[];
    }

    public async upsert(item: City) : Promise<City> {
        return await (await this._container.items.upsert<City>(item)).resource as City;
    }

    public async delete(item: City) : Promise<boolean> {
        let resp = await this._container.item(item.id).delete<City>();

        if (resp.statusCode == 200) {
            return true;
        } else {
            return false;
        }
    }
}