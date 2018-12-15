const { data, broadcast } = require('waves-transactions')

// Example of data in params 
// const params = {
//   data: [
//     { key: 'integerVal', value: 1 },
//     { key: 'booleanVal', value: true },
//     { key: 'stringVal', value: 'hello' },
//     { key: 'binaryVal', value: [1, 2, 3, 4] },
//   ]
// }

function SignDataTx(dataArray,seed){
    const params = {
        data:dataArray
    }
    return data(params,seed)
}

function SendSignTX(SignDataTx){
    return broadcast(SignDataTx, "https://testnode1.wavesnodes.com")
}

module.exports = {
    SignDataTx:SignDataTx,
    SendSignTX:SendSignTX
}