async function request(urlParams, method, data) {

    const settings = {
        method: method,
        cache: "no-cache",
        headers: {
            "Content-Type": "application/json; charset=utf-8",
        },
        body: JSON.stringify(data),
    };

    let response;

    if (method === "GET") {
        response = await fetch(urlParams);
    } else {
        response = await fetch(urlParams, settings);
    }
    return response.json();
};