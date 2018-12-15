const mongoose = require('mongoose');
const Schema = mongoose.Schema;
const Tree = new Schema({
    key: {
      type: String,
      default: "tree"
    },
    tree: {
        type: Object
    },
    root: {
        type: String
    },
    timestamp: {
        type: Date,
        default: Date.now()
    }
}, {
    versionKey: false
});

module.exports = mongoose.model('tree', Tree);