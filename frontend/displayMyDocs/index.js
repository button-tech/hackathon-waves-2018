const backendURL = "http://localhost:3000";
const ipfsServiceURL = 'http://localhost:8080/get/';

async function getDocumentsByNickname(nickname) {
    return (await query("GET", `${backendURL}/documents/owner/${nickname}`)).result.documents;
}

(async () => {
    const nickname = "fsdfsdf";
    const documents = await getDocumentsByNickname(nickname);
    let counter = 1;
    for (d of documents) {
        document.getElementById("tbl").innerHTML += `
                            <tr>
                        <th scope="row">${counter}</th>
                        <td>${d.nicknamePartner}</td>
                        <td>${d.signatures[1]}</td>
                        <td>0x${d.hash}</td>
                        <td><a href="#">Verify</a></td>
                    </tr>
        `;
        counter++;
    }
})();

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