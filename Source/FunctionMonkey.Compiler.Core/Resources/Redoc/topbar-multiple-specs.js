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
  return await fetch("./openapi/redoc-documents-spec.json")
    .then(async response => {
      return await response
        .json()
        .catch(err => console.log("response error: ", err));
    })
    .catch(error => console.log("fetch error: ", error));
}

async function getLogoUri() {
  const path = "./openapi/Resources.Redoc.logo.";
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
    logoUri =
      "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAmoAAADICAYAAABYmc2zAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyhpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNi1jMTM4IDc5LjE1OTgyNCwgMjAxNi8wOS8xNC0wMTowOTowMSAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTcgKE1hY2ludG9zaCkiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6MkI4N0FBRkRGN0E3MTFFNzk2RDhEOTU0MTZCQjVFOTciIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6MkI4N0FBRkVGN0E3MTFFNzk2RDhEOTU0MTZCQjVFOTciPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDoyQjg3QUFGQkY3QTcxMUU3OTZEOEQ5NTQxNkJCNUU5NyIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDoyQjg3QUFGQ0Y3QTcxMUU3OTZEOEQ5NTQxNkJCNUU5NyIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/PmtYIvwAADDsSURBVHja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";
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
  var body = document.body;
  document.body.insertBefore(customTopbar, body.firstChild);

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
