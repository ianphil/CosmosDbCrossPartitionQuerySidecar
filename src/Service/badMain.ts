import { Address } from "./models/Address";
import {BadCosmosRepo} from "./repositories/BadCosmosRepo";
import * as dotenv from 'dotenv'
import assert from "assert";

dotenv.config()

let run = async () => {
    let repo = await BadCosmosRepo.factory();

    let addresses: Address[] = [
        { id: "1", street: "1 Microsoft Way", city: "Redmond", state: "WA", zipCode: "98052" },
        { id: "2", street: "2 Microsoft Way", city: "Redmond", state: "WA", zipCode: "98052" },
        { id: "3", street: "555 110th Ave NE", city: "Bellevue", state: "WA", zipCode: "98004" }
    ];
    
    addresses.forEach(address => {
        repo.upsert(address);
        console.log(`Created city: ${address.street}`);
    });

    let msftRedmondAddresses = await repo.queryByZip("98052");
    assert.equal(msftRedmondAddresses.length, 2);

    let msftInWashington = await repo.queryByState("WA");
    assert.equal(msftInWashington.length, 3, `Cross Partition QueryByState returned: ${msftInWashington.length} not 3`);
}

run();
