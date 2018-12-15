require('./connector');
const tree = require('./schema/tree');
const document = require('./schema/document');

const Tree = {
    createOrUpdate: (merkleTree, root) => {
        return tree.updateOne({key: "tree"}, {tree: merkleTree, root: root, timestamp: Date.now()}, {upsert: true});
    },
    get: () => {
        return tree.findOne({key: "tree"}).exec();
    }
};

const Document = {
    create: (index, name, nickname, data, hash, requiredCountOfSignatures, signatures) => {
        return document.create({name: name, nickname: nickname, data: data, hash: hash, requiredCountOfSignatures: requiredCountOfSignatures, signatures: signatures});
    },
    update: {
      signature: {
          byIndex: (index, signatures) => {
              return document.updateOne({
                  index: index
              }, { signatures: signatures } );
          },
          byName: (name, signatures) => {
              return document.updateOne({
                  name: name
              }, { signatures: signatures } );
          },
          byHash: (hash, signatures) => {
              return document.updateOne({
                  hash: hash
              }, { signatures: signatures } );
          }
      },
      index: (hash, index) => {
          return document.updateOne({
              hash: hash
          }, { index: index } );
      }
    },
    get: {
        byNickname: (nickname) => {
            return document.find({nickname: nickname}).exec();
        },
        byIndex: (index) => {
            return document.findOne({index: index}).exec();
        },
        byFileName: (name) => {
            return document.find({name: name}).exec();
        },
        byFileHash: (hash) => {
            return document.findOne({hash: hash}).exec();
        },
        all: () => {
            return document.find({}).exec();
        }
    }
};

module.exports = {
    Tree: Tree,
    Document: Document
};