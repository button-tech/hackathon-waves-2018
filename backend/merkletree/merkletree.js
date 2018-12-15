const MerkleTree = require('merkletreejs');
const SHA256 = require('crypto-js/sha256');

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
    return tree.getProof(leaf)
}

module.exports = {
    buildMerkleTree: buildMerkleTree,
    rebuildMerkleTree: rebuildMerkleTree,
    getProof: getProof,
    getLeaves: getLeaves,
    SHA256: SHA256
};