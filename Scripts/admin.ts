console.log('Hello Admin');

const frame = (<HTMLIFrameElement>query('#file_browser iframe'));
const loadingContainer = query('#file_browser div.file-explorer > div');

frame.onload = (e) => {
    frame.style['display'] = frame.src == '' ? 'none' : 'block';
    loadingContainer.style['display'] = 'none';
};

frame.onloadstart = (e) => console.log(e);

const closeFileBrowser = (id: number): void => {
    query('#file_browser').style['display'] = 'none';

    frame.src = '';

    if (id == -1) return;

    ajax({
        url: `/Oreo/ReloadImage?id=${id}`,
        response: (res) => {
            let response = res.asJSON();

            if (!response) return;

            let image = <HTMLImageElement>query(`#img_${id} > img`);

            if (!image) return;

            image.src = response['source'];
        }
    });
};

const modifyOreoImage = (id: number): void => {
    loadingContainer.style['display'] = 'grid';

    window.scrollTo({
        top: 0,
        left: 0
    });

    query('#file_browser div.file-selector-ribbon > a').setAttribute('onclick', `closeFileBrowser(${id})`);
    query('#file_browser').style['display'] = 'grid';

    frame.src = `/Admin/Internal/Files?oreoId=${id}`;
};

const saveOreo = (id: number): void => {
    let loading = findLoading(`#oreo_${id}`);
    activateLoadingElement(loading);

    let title = (<HTMLInputElement>query(`#oreo_title_${id}`)).value;
    let flavour = (<HTMLInputElement>query(`#oreo_flavour_${id}`)).value;
    let layers = parseInt((<HTMLInputElement>query(`#oreo_layers_${id}`)).value);
    
    ajax({
        url: '/Oreo/SaveData',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            id: id,
            title: title,
            flavour: flavour,
            layers: layers
        }),
        response: (res) => {
            deactivateLoadingElement(loading);
        }
    });
};
