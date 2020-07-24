capsule(() => {
    let upload_img = <HTMLInputElement>query('#upload_img');
    let upload_preview = <HTMLImageElement>query('#upload_preview');
    let a_upload = query('div.upload-a div a');
    let loading = findLoading('div.upload-a');

    upload_img.onchange = (e) => {
        if (upload_img.files && upload_img.files.length > 0) {
            let reader = new FileReader();

            reader.onload = (e) => {
                upload_preview.src = e.target.result.toString();

                upload_preview.style['display'] = 'block';
            };

            reader.readAsDataURL(upload_img.files[0]);
        } else {
            upload_preview.style['display'] = 'none';
        }
    };

    a_upload.addEventListener('click', (e) => {
        if (upload_preview.src && upload_preview.src.length > 0) {
            a_upload.style['display'] = 'none';

            activateLoadingElement(loading);

            ajax({
                url: '/Admin/Internal/UploadFile',
                method: 'POST',
                response: (res) => {
                    a_upload.style['display'] = 'initial';

                    deactivateLoadingElement(loading);
                }
            });
        }
    });
});

capsule(() => {
    let available_filter = <HTMLInputElement>query("#available_filter");
    let available_images = queryAll('div.selection-available div');

    available_filter.onkeyup = () => {
        available_images.forEach((element) => {
            if (!available_filter.value || available_filter.value == '') element.style['display'] = 'grid';

            element.style['display'] = 'none';

            try {
                if (element.getAttribute('data-name').match(available_filter.value)) element.style['display'] = 'grid';
            } catch(error) { } finally { }
        });
    };
});

var saving: boolean = false;

const selectImage = (id: number): void => {
    if (saving) return;

    let data_name = query(`#image_${id}`)?.getAttribute('data-name');
    let image = <HTMLImageElement>query(`#image_${id} > img`);

    if (!data_name || !image) return;

    let current = <HTMLImageElement>query('div.current-display-save > img');

    current.src = image.src;

    let a_saveContainer = query('div.current-display-save > div');
    let a_save = query('div.current-display-save > div > a');

    a_saveContainer.setAttribute('data-selected', data_name);

    if (current.getAttribute('data-name') != data_name) {
        a_save.setAttribute('onclick', `saveImage(${window.location.search.substr('?oreoId='.length)}, ${id})`);
        a_saveContainer.style['display'] = 'grid';
    } else {
        a_save.setAttribute('onclick', '');
        a_saveContainer.style['display'] = 'none';
    }
};

const saveImage = (oreoId: number, imageId: number) => {
    saving = true;

    let current = query('div.current-display-save > img');
    let a_saveContainer = query('div.current-display-save > div');
    let a_save = query('div.current-display-save > div > a');
    let loading = findLoading('div.current-display-save > div');

    a_save.style['display'] = 'none';

    activateLoadingElement(loading);

    ajax({
        url: '/Oreo/SaveImage',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            oreoId: oreoId,
            imageId: imageId
        }),
        response: (res) => {
            current.setAttribute('data-name', a_saveContainer.getAttribute('data-selected'));
            a_saveContainer.setAttribute('data-selected', '');
            deactivateLoadingElement(loading);
            saving = false;
        }
    });
};
