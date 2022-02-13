"use strict";

// Usage
// sendFormData(...)
//  .then(data => ...)
//  .catch(error => ...)
function sendJsonData(url, data = {}, method = 'POST') {
    return new Promise((resolve, reject) => {
        fetch(url, {
            method: method,
            headers: {
                'XSRF-TOKEN': document.getElementsByName('__RequestVerificationToken')[0].value,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(response => {
                if (response.ok) {
                    response.json()
                        .then(data => { resolve(data); })
                        .catch(error => { resolve() });
                    return;
                }
                if (response.status == 400) {
                    response.text()
                        .then(data => {
                            try {
                                const dataJson = JSON.parse(data);
                                reject({ status: 400, validation: dataJson, message: "" });
                            }
                            catch (e) {
                                reject({ status: 400, message: data, validation: {} });
                            }
                        })
                        .catch(error => {
                            reject({ status: 400, message: "Fehler beim Verarbeiten der Daten.", validation: {} });
                        });
                    return;
                }
                reject({ status: response.status, message: "Fehler beim Verarbeiten der Daten.", validation: {} });
            })
            .catch(error => {
                reject({ status: 0, message: "Fehler beim Senden der Daten.", validation: {} });
            });
    });

}

function getHandlerUrl(handler, getParams) {
    getParams = getParams || {};
    let url = window.location.href.indexOf("?") != -1
        ? window.location.href + "&handler=" + handler
        : window.location.href + "?handler=" + handler
    url = Object.keys(getParams).reduce((prev, param) =>
        prev + "&" + param + "=" + getParams[param]
        , url);
    return url;
}

function copyToClipboard(data, withHeader = true) {
    if (!data[0]) { return; }
    const header = Object.keys(data[0]);
    const headerCount = header.length;
    let text = "";
    let i = 0;

    if (withHeader) {
        for (const h of header) {
            text += h;
            if (++i < headerCount) { text += '\t'; }
        }
        text += "\r\n";
    }

    for (const row of data) {
        i = 0;
        for (const key in row) {
            text += row[key];
            if (++i < headerCount) { text += '\t'; }
        }
        text += "\r\n";
    }
    return navigator.clipboard.writeText(text);
}
