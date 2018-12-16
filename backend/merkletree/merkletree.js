const MerkleTree = require('merkletreejs');
const SHA256 = require('crypto-js/sha256');
const CryptoJS = require('crypto-js');

function buildMerkleTree(leaves, hashFunc) {
    const tree = new MerkleTree(leaves, hashFunc);
    const root = "0x" + tree.getRoot().toString('hex');
    return {
        tree: tree,
        root: root
    }
}

function getLeaves(tree) {
    return tree.leaves.map(l => l.toString('hex'));
}

function rebuildMerkleTree(tree, newLeaves, hashFunc) {
    let leaves = getLeaves(tree).concat(newLeaves);
    return buildMerkleTree(leaves, hashFunc);
}

function getProof(tree, leaf) {
    const leaves = tree.leaves.map(l => l.toString('hex'));
    const t = buildMerkleTree(leaves, SHA256);
    const proof =  t.tree.getProof(leaf);
    // console.log(verify(proof, "02d6314ca701459a2bc04c28163e8b0e03d2ce2ee06460803348798c8ecad1e0", "5d0cfb787eb2f575b94d79fc6cde138202826cf92b3960f47a4f40893b6d87cb", SHA256))
    return proof;
}

module.exports = {
    buildMerkleTree: buildMerkleTree,
    rebuildMerkleTree: rebuildMerkleTree,
    getProof: getProof,
    getLeaves: getLeaves,
    SHA256: SHA256
};