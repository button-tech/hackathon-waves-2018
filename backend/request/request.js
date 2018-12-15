const rp = require('request-promise');

async function SetToIpfs(dOwner, dPartner){
    var options = {
        method: 'POST',
        uri: 'https://api.telegram.org/bot'+TOKEN+'/sendMessage',
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

