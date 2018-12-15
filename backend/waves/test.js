const dataTx = require('./dataTx')

async function pushRootHashToBlockchain(rootHash, seed) {

    let signedData = dataTx.SignDataTx([{
        key:"rootHash", value:rootHash
    }],seed);

    console.log(signedData)

    return await dataTx.SendSignTX(signedData)
}

check = pushRootHashToBlockchain("1232312313213","huge pool glance daring own category scout dignity impulse field furnace pencil mistake spawn track")
.then(console.log)