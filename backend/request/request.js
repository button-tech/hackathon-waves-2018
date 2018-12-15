const rp = require('request-promise');

async function SetToIpfs(dOwner, dPartner){
    var options = {
        method: 'POST',
        uri: 'http://localhost:8080/set',
        body: {
           "document_owner":dOwner,
           "document_partner":dPartner
        },
        json: true 
    };
    const resp = await rp.post(options);
    return resp
}

module.exports = {
    SetToIpfs:SetToIpfs
}

