// General
// -----------

/**
 * This function serves the purpose that i can forEach either over a array or a NodeList
 * and get the respective item with its index in the array or list
 * 
 * @param array Either a native array or a NodeList
 * @param callback The callback gives the element as first parameter and the index as second optional parameter
*/
const forEach = <T>(array: T[], callback: (item: T, index?: number) => void) => {
    for (let i = 0, l = array.length; i < l; i++) {
        callback(array[i], i);
    }
};

/**
 * This function gets the current scroll offset on the vertical axis
 */
const getScrollY = (): number => {
    return window.scrollY | window.pageYOffset;
}

/**
 * Capsulate code to prevent variable or function leakage
 * 
 * @param fn The "callback" to execute capsulated code
 */
const capsule = (fn: () => void): void => {
    fn();
};

const query = <T extends HTMLElement>(docquery: string): T => {
    return document.querySelector(docquery);
};

const queryAll = <T extends HTMLElement>(docquery: string): NodeListOf<T> => {
    return document.querySelectorAll(docquery);
};

const queryForEach = <T extends HTMLElement>(docquery: string, callback: (item: T, index?: number) => void): void => {
    let collection = document.querySelectorAll(docquery);

    for (let i = 0, l = collection.length; i < l; i++) {
        callback(<T>collection[i], i);
    }
}

// Ajax
// --------
class AjaxResponse {

    private data: any;

    constructor(data: any) {
        this.data = data;
    }

    public as<T>(): T {
        return this.data;
    }

    public asJSON(): Object {
        return typeof this.data == 'object' ? this.data : (typeof this.data == 'string' ? JSON.parse(this.data) : null);
    }

}

interface AjaxOptions {
    url: string,
    response?: (data: AjaxResponse) => void
    data?: any,
    error?: (error: any) => void,
    method?: string,
    contentType?: string
    headers?: Object
}

/**
 * Fire a ajax call, this is a JQuery Ajax wrap
 * 
 * @param url The url
 * @param response The response function
 * @param method The method, GET by default
 * @param error The error function
 */
const ajax = (options: AjaxOptions): void => {
    let _options: AjaxOptions = {
        method: 'GET',
        error: (e) => console.error(e.responseText),
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        ...options
    };

    // @ts-ignore
    $.ajax({
        url: _options.url,
        method: _options.method,
        success: (res) => _options.response(new AjaxResponse(res)),
        error: _options.error,
        data: _options.data,
        contentType: _options.contentType
    });
};

// Loading
// -----------

/**
 * This will find the next closest loading element
 * 
 * @param query The query to search from (will end looking like '<query> div[id^="sys_loading_"]')
 */
const findLoading = (query: string): HTMLElement => {
    return document.querySelector(`${query} div[id^="sys_loading_"]`);
};

const activateLoading = (id: number | string): void => {
    let loadingElement = <HTMLElement>document.querySelector(`#sys_loading_${id}`);

    if (!loadingElement) return;

    activateLoadingElement(loadingElement);
};

const deactivateLoading = (id: number | string): void => {
    deactivateLoadingElement(document.querySelector(`#sys_loading_${id}`));
};

const activateLoadingElement = (e: HTMLElement): void => {
    if (!e) return;

    let displayValue = e.getAttribute('data-display');

    if (!displayValue) return;

    e.style['display'] = displayValue;
};

const deactivateLoadingElement = (e: HTMLElement): void => {
    if (e) e.style['display'] = 'none';
};
