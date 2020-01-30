init();

async function init() {
    var specInfo = await getSpecInfoAsync();
    // Replace default topbar if more than one spec is present
    if (specInfo) {
        disableDefaultTopbar();
        renderTopbar(specInfo);
        onApiVersionChanged();
    }
}

function disableDefaultTopbar() {
    var style = document.createElement('style');
    style.type = 'text/css';
    style.innerHTML = '.topbar { display: none; }';
    document.getElementsByTagName('head')[0].appendChild(style);
}

async function getSpecInfoAsync() {
    return await fetch('./apidoc/openapi-documents-spec.json').then(async (response) => {
        return await response.json().catch(err => console.log('response error: ', err));
    }).catch(error => console.log('fetch error: ', error))
}

function onApiVersionChanged() {
    const ui = SwaggerUIBundle({
        url: document.getElementById('select').value,
        dom_id: '#swagger-ui',
        deepLinking: true,
        presets: [
            SwaggerUIBundle.presets.apis,
            SwaggerUIStandalonePreset
        ],
        plugins: [
            SwaggerUIBundle.plugins.DownloadUrl
        ],
        layout: "StandaloneLayout"
    })
    window.ui = ui
}

function renderTopbar(specInfo) {
    var optionsTags = specInfo.map(spec => `<option value="${spec.Path}">${spec.Title}</option>`);

    var customTopbar = document.createElement('div');
    customTopbar.classList.add('topbar');
    customTopbar.style.display = 'block';
    customTopbar.innerHTML =
        `<div class="wrapper">
            <div class="topbar-wrapper">
                <a class="link"><img src="./apidoc/Resources.OpenApi.app-logo-small.svg" height="40" /></a>
                <form class="download-url-wrapper">
                    <label class="select-label" for="select">
                        <span>Select a spec</span>
                        <select id="select" onchange="onApiVersionChanged()">
                            ${optionsTags.join(' ')}
                        </select>
                    </label>
                </form>
            </div>
        </div>`;
    document.body.insertBefore(customTopbar, document.getElementById('swagger-ui'));

    var preSelectedSpecValue = specInfo.find(spec => spec.Selected);
    if (!preSelectedSpecValue) {
        preSelectedSpecValue = specInfo[0];
    }
    document.getElementById('select').value = preSelectedSpecValue.Path;
}