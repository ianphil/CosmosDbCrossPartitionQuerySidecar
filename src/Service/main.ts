import { City } from "./models/City";
import {CosmosRepo} from "./models/CosmosRepo";
import * as dotenv from 'dotenv'
import { Console } from "console";

dotenv.config()

let run = async () => {
    let repo = await CosmosRepo.factory();

    let cities: City[] = [
        { id: "1", name: "Flowery Branch", state: "GA", isCapitol: false },
        { id: "2", name: "Duluth", state: "GA", isCapitol: false },
        { id: "3", name: "Buford", state: "GA", isCapitol: false },
        { id: "4", name: "Atlanta", state: "GA", isCapitol: true },
        { id: "5", name: "Seattle", state: "WA", isCapitol: true },
        { id: "6", name: "Bellevue", state: "WA", isCapitol: false }
    ]
    
    // cities.forEach(city => {
    //     repo.upsert(city);
    //     console.log(`Created city: ${city.name}`);
    // });

    let seattle: City = await repo.get("1");
    console.log(`GetById: ${seattle.name}`);

    let cityList: City[] = await repo.getAll();
    cityList.forEach(city => {
        console.log(`Retrieved city: ${city.name}`)
    });
}

run();
