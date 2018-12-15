const {Tree, Document} = require("./../db/db");
const MerkleTree = require("./../merkletree/merkletree");

async function isDocumentSigned(hash) {
    const {signatures, requiredCountOfSignatures} = await Document.get.byFileHash(hash);
    return signatures.length === requiredCountOfSignatures;
}

function getTree() {
    return Tree.get();
}

async function addDocument(
    name,
    nicknameOwner,
    nicknamePartner,
    encryptedDataOwner,
    encryptedDataPartner,
    hash,
    requiredCountOfSignatures,
    digest,
    timestampOwner
) {
    await Document.create(
        name,
        nicknameOwner,
        nicknamePartner,
        encryptedDataOwner,
        encryptedDataPartner,
        hash,
        requiredCountOfSignatures,
        [digest],
        timestampOwner
    );
}

async function updateDocumentSignatureAndTimestamp(id, signature, timestamp) {
    const doc = await Document.get.byID(id);
    const signatures = doc.signatures.push(signature);
    await Document.update.signature.byID(id, signatures, timestamp);
}

async function pushTreeToDB(id, document) {
    const { hash, signatures } = document;
    const concatSignatures = signatures.reduce((acc, val) => acc + val);
    const newLeave = MerkleTree.SHA256(hash + concatSignatures);
    let old = await Tree.get();
    let tree;
    if (old === null) {
        tree = MerkleTree.buildMerkleTree([newLeave, MerkleTree.SHA256(null)], MerkleTree.SHA256);
    } else {
        tree = MerkleTree.rebuildMerkleTree(old.tree, [newLeave], MerkleTree.SHA256);
    }
    const lastIndex = getLeaves(tree.tree).length;
    await updateDocumentIndex(id, lastIndex);
    await pushRootHashToBlockchain(tree.root);
    await Tree.createOrUpdate(tree.tree, tree.root);
}

async function updateDocumentIndex(id, index) {
    await Document.update.index(id, index);
}

async function pushRootHashToBlockchain(rootHash) {

}

async function getDocumentByName(id) {
    return Document.get.byFileName(id);
}

async function getDocumentByHash(id) {
    return Document.get.byFileHash(id);
}

async function getDocumentByIndex(id) {
    return Document.get.byIndex(id);
}

async function getDocumentByID(id) {
    return Document.get.byID(id);
}

async function getDocumentByNicknameOwner(id) {
    return Document.get.byNicknameOwner(id);
}

async function getDocumentByNicknamePartner(id) {
    return Document.get.byNicknamePartner(id);
}

async function getAllDocumnts() {
    return Document.get.all();
}

module.exports = {
    isDocumentSigned: isDocumentSigned,
    addDocument: addDocument,
    updateDocumentSignatureAndTimestamp: updateDocumentSignatureAndTimestamp,
    updateDocumentIndex: updateDocumentIndex,
    pushTreeToDB: pushTreeToDB,
    pushRootHashToBlockchain: pushRootHashToBlockchain,
    getDocumentByName: getDocumentByName,
    getDocumentByHash: getDocumentByHash,
    getDocumentByIndex: getDocumentByIndex,
    getDocumentByID: getDocumentByID,
    getDocumentByNicknameOwner: getDocumentByNicknameOwner,
    getDocumentByNicknamePartner: getDocumentByNicknamePartner,
    getAllDocumnts: getAllDocumnts,
    getTree: getTree
};