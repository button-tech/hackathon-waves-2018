const backendURL = "http://localhost:3000";
const telegramServiceURL = "";

async function GetDataFromIPFS(data){
        const rawResponse = await fetch('http://localhost:8080/get/'+data);
        return rawResponse.json();
}

async function GetRootHashFromBlockchain(address){
    let url = "https://testnodes.wavesnodes.com/addresses/data/" + address + "/rootHash"
    const response = await fetch(url);
    return response.json();
}

// TODO
async function Verify(hash, numberid, id, address){
    let doc = await fetch("http://localhost:3000/download/" + id)
    const concatSignatures = doc.signatures.reduce((acc, val) => acc + val)
    let leave = CryptoJS.SHA256(doc.hash + concatSignatures)
    let proof = await fetch("http://localhost:3000/proof/" + numberid  + "/" + hash)
    let root = await GetRootHashFromBlockchain(address)
    verify(proof, leave, root ,CryptoJS.SHA256)
}

async function getUserData() {

}

function getDocumentsByNickname(nickname) {
    return query("GET", `${backendURL}/documents/owner/${nickname}`);
}

function verifySignature(encryptedDocument, signature) {
    return key.verify(encryptedDocument, signature, 'string', 'hex')
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
