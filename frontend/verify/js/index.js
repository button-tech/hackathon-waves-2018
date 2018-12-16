const backendURL = "http://localhost:3000";
const ipfsServiceURL = 'http://localhost:8080/get/';
const telegramServiceURL = "";
const masterAddress = "3N1yku2yYUB1QkQKsRR3fX5Dv9LxJFWk2gm";

async function GetDataFromIPFS(commitHash){
        const rawResponse = await fetch(`${ipfsServiceURL}/${commitHash}`);
        return rawResponse.json();
}

async function GetRootHashFromBlockchain(address){
    let url = "https://testnodes.wavesnodes.com/addresses/data/" + address + "/rootHash"
    const response = await fetch(url);
    return response.json();
}

async function getInfoFromInputs(){
    let ownerPubKey = document.getElementById("ownp").value;
    let partnerPubKey = document.getElementById("partp").value;
    let documentHash = document.getElementById("dochash").value;
    let ownerSignature = document.getElementById("ownsign").value;
    let partnerSignature = document.getElementById("partsign").value;

    try {
        const isVerify = await Verify(documentHash);
        if (isVerify) {
            document.getElementById("is-verify-proof").innerHTML = `
        <h1 style="color: green !important;">Yes</h1>`;
        } else {
            document.getElementById("is-verify-proof").innerHTML = `
        <h1 style="color: red !important;">No</h1>`
        }

        if (
            verifySignature(getPubKeyObj(ownerPubKey), documentHash, ownerSignature) &&
            verifySignature(getPubKeyObj(partnerPubKey), documentHash, partnerSignature)
        ) {
            document.getElementById("is-verify-signatures").innerHTML = `
            <h1 style="color: green !important;">Yes</h1>`;
        } else {
            document.getElementById("is-verify-signatures").innerHTML = `
                        <h1 style="color: red !important;">No</h1>`;
        }
    } catch (e) {
        console.log(e);
            document.getElementById("is-verify-proof").innerHTML = `
        <h1 style="color: red !important;">No</h1>`

            document.getElementById("is-verify-signatures").innerHTML = `
                        <h1 style="color: red !important;">No</h1>`;
    }
}

function getPubKeyObj(pubKey) {
    const key = new NodeRSA();
    return key.importKey(pubKey, 'public');
}

function verifySignature(key, hash, signature) {
    return key.verify(hash, signature, 'hex', 'hex')
}

async function Verify(hash) {
    const { index, signatures, timestampOwner, timestampPartner } = (await query("GET", `${backendURL}/getDocument/${hash}`)).result.document;
    const concatSignatures = signatures.reduce((acc, val) => acc + val);
    const leaf = CryptoJS.SHA256(hash + concatSignatures + timestampOwner + timestampPartner).toString();
    let response = await fetch(`${backendURL}/proof/${index}/${hash}`);
    const proof = (await response.json()).result.proof;
    const { value } = await GetRootHashFromBlockchain(masterAddress);
    const tree = new MerkleTree.MerkleTree();
    tree.hashAlgo = CryptoJS.SHA256;
    return tree.verify(proof, leaf, value.substring(2));
}

async function getUserData() {

}

function getDocumentsByNickname(nickname) {
    return query("GET", `${backendURL}/documents/owner/${nickname}`);
}

/**
 * Request to server side
 * @param method Using method
 * @param url URL to send
 * @param data request data
 * @returns {Promise<*>}
 */
async function query(method, url, data) {
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": url,
        "method": method,
        "processData": false,
    };

    if (data) {
        settings.data = data;
        settings.headers = {
            "Content-Type": "application/json"
        };
    }

    const result = await $.ajax(settings);
    return result;
};
