const mongoose = require('mongoose');
const Schema = mongoose.Schema;
const Document = new Schema({
    index: {
        type: Number
    },
    name: {
      type: String
    },
    data: {
        type: Object
    },
    hash: {
      type: String
    },
    requiredCountOfSignatures: {
      type: Number
    },
    signatures: {
      type: Object
    },
    nickname: {
      type: String
    },
    timestamp: {
        type: Date,
        default: Date.now()
    }
}, {
    versionKey: false
});

module.exports = mongoose.model('documents', Document);