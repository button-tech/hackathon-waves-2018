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
    create: (name, nicknameOwner, nicknamePartner, dataOwner ,dataPartner, hash, requiredCountOfSignatures, signatures, timestampOwner) => {
        return document.create({
            name: name,
            nicknameOwner: nicknameOwner,
            nicknamePartner: nicknamePartner,
            dataOwner: dataOwner,
            dataPartner: dataPartner,
            hash: hash,
            requiredCountOfSignatures: requiredCountOfSignatures,
            signatures: signatures,
            timestampOwner: timestampOwner
        });
    },
    update: {
      signature: {
          byIndex: (index, signatures, timestampPartner) => {
              return document.updateOne({
                  index: index
              }, { signatures: signatures, timestampPartner: timestampPartner } );
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
          },
          byID: (id, signatures, timestampPartner) => {
              return document.updateOne({
                  _id: id
              }, { signatures: signatures, timestampPartner: timestampPartner } );
          }
      },
      index: (id, index) => {
          return document.updateOne({
              _id: id
          }, { index: index } );
      }
    },
    get: {
        byNicknameOwner: (nickname) => {
            return document.find({nicknameOwner: nickname}).exec();
        },
        byNicknamePartner: (nickname) => {
            return document.find({nicknamePartner: nickname}).exec();
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
        byID: (id) => {
            return document.findOne({_id: id}).exec();
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