const {Tree, Document} = require("./../db/db");
const MerkleTree = require("./../merkletree/merkletree");
const dataTx = require("../waves/dataTx");

async function addDocument(name, nickname, encryptedData, hash, requiredCountOfSignatures, digest) {
    await Document.create(name, nickname, encryptedData, hash, requiredCountOfSignatures, [digest]);
}

async function updateDocumentSignature(hash, signature) {
    const doc = await Document.get.byFileHash(hash);
    doc.signatures.push(signature);
    await Document.update.signature.byHash(doc.signatures);
}

async function updateDocumentIndex(hash, index) {
    await Document.update.index(hash, index);
}

async function pushTreeToDB(signedDocument) {
    const {hash, signatures} = signedDocument;
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
    await updateDocumentIndex(hash, lastIndex);
    await pushRootHashToBlockchain(tree.root);
    await Tree.createOrUpdate(tree.tree, tree.root);
}

async function pushRootHashToBlockchain(rootHash, seed) {

    let signedData = dataTx.SignDataTx([{
        key:"rootHash", value:rootHash
    }],seed);

    await dataTx.SendSignTX(signedData)
    
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

async function getDocumentByNickname(id) {
    return Document.get.byNickname(id);
}

async function getAllDocumnts() {
    return Document.get.all();
}