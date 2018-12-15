const { data } = require('waves-transactions')

const seed = 'huge pool glance daring own category scout dignity impulse field furnace pencil mistake spawn track'

const params = {
  data: [
    { key: 'integerVal', value: 1 },
    { key: 'booleanVal', value: true },
    { key: 'stringVal', value: 'hello' },
    { key: 'binaryVal', value: [1, 2, 3, 4] },
  ],
}

const signedDataTx = data(params, seed)
console.log(signedDataTx)