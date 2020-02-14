init();

async function init() {
    var specInfo = await getSpecInfoAsync();
    console.log(specInfo);
    // Replace default topbar if more than one spec is present
    if (specInfo && specInfo.length > 1) {
        addStyles();
        renderTopbar(specInfo);
        onApiVersionChanged();
    }
}

function addStyles() {
    var style = document.createElement("style");
    style.type = "text/css";
    style.innerHTML = getStyles();
    document.getElementsByTagName("head")[0].appendChild(style);
}

async function getSpecInfoAsync() {
    return await fetch("/redoc/redoc-documents-spec.json")
        .then(async response => {
            return await response
                .json()
                .catch(err => console.log("response error: ", err));
        })
        .catch(error => console.log("fetch error: ", error));
}

async function getLogoUri() {
    const path = "/redoc/logo.";
    const extensions = ["svg", "png", "jpg"];
    for (let index = 0; index < extensions.length; index++) {
        const logoUri = await fetch(path + extensions[index]).then(response =>
            response.status === 404 ? undefined : path + extensions[index]
        );
        if (logoUri) return logoUri;
    }
    return undefined;
}

function onApiVersionChanged() {
    const apiVersion = document.getElementById("select").value;
    Redoc.init(apiVersion, { noAutoAuth: true });
}

async function renderTopbar(specInfo) {
    var logoUri;
    var optionsTags = specInfo.map(
        spec => `<option value="${spec.Path}">${spec.Title}</option>`
    );
    var customLogoUri = await getLogoUri();

    console.log(customLogoUri);
    if (customLogoUri) {
        console.log("customimage");
        logoUri = customLogoUri;
    } else {
        logoUri = "/redoc/redoc-logo.png";
        console.log("defaultimage");
    }

    var customTopbar = document.createElement("div");
    customTopbar.classList.add("custom-topbar");
    customTopbar.style.display = "block";
    customTopbar.innerHTML = `<div class="wrapper">
            <div class="topbar-wrapper">
                <a class="link"><img src="${logoUri}" height="40" /></a>
                <form class="download-url-wrapper">
                    <label class="select-label" for="select">
                        <span>Select a spec</span>
                        <select id="select" onchange="onApiVersionChanged()">
                            ${optionsTags.join(" ")}
                        </select>
                    </label>
                </form>
            </div>
        </div>`;
    document.body.insertBefore(customTopbar, document.body.firstChild);

    var preSelectedSpecValue = specInfo.find(spec => spec.Selected);
    if (!preSelectedSpecValue) {
        preSelectedSpecValue = specInfo[0];
    }
    document.getElementById("select").value = preSelectedSpecValue.Path;
}

function getStyles() {
    return `/* Custom Modifications */

    .menu-content {
        background-color: rgba(255, 255, 255, 0) !important;
    }
    
    .menu-item-title {
        text-transform: none !important;
    }

    .topbar {
        display: none;
    }
    
    .custom-topbar {
        box-sizing: border-box;
        padding: 8px 30px;
        border-bottom: 1px solid lightgray;
    }
    
    .wrapper {
        box-sizing: border-box;
        color: rgb(59, 65, 81);
        font-family: Open Sans, sans-serif;
        margin-bottom: 0px;
        margin-left: 0px;
        margin-right: 0px;
        margin-top: 0px;
        padding-bottom: 0px;
        padding-left: 20px;
        padding-right: 20px;
        padding-top: 0px;
    }
    
    .topbar-wrapper {
        align-items: center;
        box-sizing: border-box;
        color: rgb(59, 65, 81);
        display: flex;
        font-family: Open Sans, sans-serif;
        -moz-box-align: center;
    }
    
    .link {
        align-items: center;
        background-color: rgba(0, 0, 0, 0);
        box-sizing: border-box;
        color: rgb(255, 255, 255);
        display: flex;
        flex-basis: 0%;
        flex-grow: 1;
        flex-shrink: 1;
        font-family: Titillium Web, sans-serif;
        font-size: 24px;
        font-weight: 700;
        max-width: 300px;
        text-decoration: none;
        text-decoration-color: rgb(255, 255, 255);
        text-decoration-line: none;
        text-decoration-style: solid;
        transition-delay: 0s;
        transition-duration: 0.15s;
        transition-property: color;
        transition-timing-function: ease-in;
        -moz-box-align: center;
        -moz-box-flex: 1;
    }
    
    .download-url-wrapper {
        box-sizing: border-box;
        color: rgb(59, 65, 81);
        display: flex;
        flex-basis: 0%;
        flex-grow: 3;
        flex-shrink: 1;
        font-family: Open Sans, sans-serif;
        justify-content: flex-end;
        -moz-box-flex: 3;
        -moz-box-pack: end;
    }
    
    .select-label {
        align-items: center;
        box-sizing: border-box;
        display: flex;
        font-family: Titillium Web, sans-serif;
        font-size: 12px;
        font-weight: 700;
        margin-bottom: 0px;
        margin-left: 0px;
        margin-right: 0px;
        margin-top: 0px;
        max-width: 600px;
        -moz-box-align: center;
        padding: 0 10px 0 0;
        white-space: nowrap;
    }
    
        .select-label > span {
            font-size: 16px;
            -webkit-box-flex: 1;
            -ms-flex: 1;
            flex: 1;
            padding: 0 10px 0 0;
            text-align: right;
        }
    
    #select {
        background-attachment: scroll;
        background-clip: border-box;
        background-color: rgb(247, 247, 247);
        background-image: url("data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyMCAyMCI+ICAgIDxwYXRoIGQ9Ik0xMy40MTggNy44NTljLjI3MS0uMjY4LjcwOS0uMjY4Ljk3OCAwIC4yNy4yNjguMjcyLjcwMSAwIC45NjlsLTMuOTA4IDMuODNjLS4yNy4yNjgtLjcwNy4yNjgtLjk3OSAwbC0zLjkwOC0zLjgzYy0uMjctLjI2Ny0uMjctLjcwMSAwLS45NjkuMjcxLS4yNjguNzA5LS4yNjguOTc4IDBMMTAgMTFsMy40MTgtMy4xNDF6Ii8+PC9zdmc+");
        background-origin: padding-box;
        background-position: calc(-10px + 100%) 50%;
        background-position-x: calc(100% - 10px);
        background-position-y: 50%;
        background-repeat: no-repeat;
        background-size: 20px auto;
        border-bottom-color: rgb(84, 127, 0);
        border-bottom-left-radius: 4px;
        border-bottom-right-radius: 4px;
        border-bottom-style: solid;
        border-bottom-width: 1.6px;
        border-image-outset: 0;
        border-image-repeat: stretch stretch;
        border-image-slice: 100%;
        border-image-source: none;
        border-image-width: 1;
        border-left-color: rgb(84, 127, 0);
        border-left-style: solid;
        border-left-width: 1.6px;
        border-right-color: rgb(84, 127, 0);
        border-right-style: solid;
        border-right-width: 1.6px;
        border-top-color: rgb(84, 127, 0);
        border-top-left-radius: 4px;
        border-top-right-radius: 4px;
        border-top-style: solid;
        border-top-width: 1.6px;
        box-shadow: none;
        box-sizing: border-box;
        color: rgb(59, 65, 81);
        flex-basis: 0%;
        flex-grow: 2;
        flex-shrink: 1;
        font-family: Titillium Web, sans-serif;
        font-size: 14px;
        font-weight: 700;
        line-height: 16.1px;
        margin-bottom: 0px;
        margin-left: 0px;
        margin-right: 0px;
        margin-top: 0px;
        outline-color: rgb(59, 65, 81);
        outline-style: none;
        outline-width: 0px;
        padding-bottom: 5px;
        padding-left: 10px;
        padding-right: 40px;
        padding-top: 5px;
        text-transform: none;
        -moz-appearance: none;
        -moz-box-flex: 2;
    }`;
}