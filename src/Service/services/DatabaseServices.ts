import { Address } from "../models/Address";
import { IDatabaseService } from "./IDatabaseService";

export class CosmosService implements IDatabaseService {
    upsert(address: Address): Promise<Address> {
        throw new Error("Method not implemented.");
    }
    queryByZip(zipCode: string): Promise<Address[]> {
        throw new Error("Method not implemented.");
    }
    queryByState(state: string): Promise<Address[]> {
        throw new Error("Method not implemented.");
    }

}

export class SidecarService implements IDatabaseService {
    upsert(address: Address): Promise<Address> {
        throw new Error("Method not implemented.");
    }
    queryByZip(zipCode: string): Promise<Address[]> {
        throw new Error("Method not implemented.");
    }
    queryByState(state: string): Promise<Address[]> {
        throw new Error("Method not implemented.");
    }

}