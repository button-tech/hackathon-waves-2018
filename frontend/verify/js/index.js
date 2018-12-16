const backendURL = "http://localhost:3000";
const telegramServiceURL = "";
const masterAddress = "3N1yku2yYUB1QkQKsRR3fX5Dv9LxJFWk2gm";

async function GetDataFromIPFS(data){
        const rawResponse = await fetch('http://localhost:8080/get/'+data);
        return rawResponse.json();
}

async function GetRootHashFromBlockchain(address){
    let url = "https://testnodes.wavesnodes.com/addresses/data/" + address + "/rootHash"
    const response = await fetch(url);
    return response.json();
}

function getInfoFromInputs(){
    let ownerPubKey = document.getElementByID("ownp").value;
    let partnerPubKey = document.getElementByID("partp").value;
    let documentHash = document.getElementByID("dochash").value;
    let ownerSignature = document.getElementByID("ownsign").value;
    let partnerSignature = document.getElementByID("partsign").value;
    
    let leave = CryptoJS.SHA256(documentHash+ownerSignature+partnerSignature)

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
    console.log(tree.verify(proof, leaf, value.substring(2)))
}

async function getUserData() {

}

function getDocumentsByNickname(nickname) {
    return query("GET", `${backendURL}/documents/owner/${nickname}`);
}

function verifySignature(hash, signature) {
    return key.verify(hash, signature, 'string', 'hex')
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
