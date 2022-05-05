import fastify from 'fastify'
import { Greeter } from './models/Greeter'
import * as dotenv from 'dotenv'

dotenv.config()
const server = fastify()

// process.env.CONN_STRING

server.get('/ping', async (request, reply) => {
    let greeter = new Greeter()
    return greeter.greet()
})

server.listen(8080, '0.0.0.0', (err, address) => {
    if (err) {
        console.error(err)
        process.exit(1)
    }
    console.log(`Server listening at ${address}`)
})
