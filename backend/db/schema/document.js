const mongoose = require('mongoose');
const Schema = mongoose.Schema;
const Document = new Schema({
    index: {
        type: Number
    },
    name: {
      type: String
    },
    dataOwner: {
        type: String
    },
    dataPartner: {
        type: String
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
    nicknameOwner: {
      type: String
    },
    nicknamePartner: {
      type: String
    },
    timestampOwner: {
        type: Date,
        default: Date.now()
    },
    timestampPartner: {
        type: Date
    },
    ipfsDataHashOwner:{
        type:String
    },
    ipfsDataHashPartner:{
        type:String
    }
}, {
    versionKey: false
});

module.exports = mongoose.model('documents', Document);