/*
 * Clipboard wrapper
 * @version: 3.0.1
 * @author: Preline
 * @license: Preline Libraries (https://preline.com/licenses)
 * Copyright 2024 Preline
 */

const globalIsDark = localStorage.getItem("hs-clipboard-is-dark");
if (!globalIsDark) localStorage.setItem("hs-clipboard-is-dark", "dark");

const globalIsJsx = localStorage.getItem("hs-clipboard-is-jsx");
if (!globalIsJsx) localStorage.setItem("hs-clipboard-is-jsx", "html");

const HSClipboard = {
  collection: [],
  dataAttributeName: "data-hs-clipboard-options",
  defaults: {
    type: null,
    tooltip: false,
    contentTarget: null,
    classChangeTarget: null,
    defaultClass: null,
    successText: null,
    successClass: null,
    originalTitle: null,
  },

  init(el, options = {}, id) {
    let items;

    if (el instanceof HTMLElement) items = [el];
    else if (el instanceof Object) items = el;
    else items = document.querySelectorAll(el);

    for (let i = 0; i < items.length; i += 1) {
      if (!items[i].id)
        items[i].id = `copy-markup-${i + 1 < 10 ? "0" : ""}${i + 1}`;

      this.addToCollection(items[i], options, id || items[i].id);
    }

    if (!this.collection.length) {
      return false;
    }

    this._init();
  },

  _init: function () {
    for (let i = 0; i < this.collection.length; i += 1) {
      if (this.collection[i].hasOwnProperty("$initializedEl")) continue;
      else this.collection[i].$initializedEl = null;

      let { $el, $initializedEl, options } = this.collection[i];

      /* Start : Init */
      if (options.contentTarget) this.setShortcodes(options);
      if (options.toggleLight)
        this._buildToggleLight(
          document.querySelector(options.toggleLight),
          this.collection[i].id
        );
      if (options.toggleJsx)
        this._buildToggleJsx(
          document.querySelector(options.toggleJsx),
          this.collection[i].id
        );

      $initializedEl = new ClipboardJS($el, options);

      $initializedEl.on("success", () => {
        const clipboardDefault = $el.querySelector(".js-clipboard-default");
        const clipboardSuccess = $el.querySelector(".js-clipboard-success");
        const clipboardSuccessTarget = $el.querySelector(
          ".js-clipboard-success-text"
        );
        const successText = options.successText;
        const tooltip = options.tooltip;
        let tempSuccessText;

        if (clipboardSuccessTarget) {
          tempSuccessText = clipboardSuccessTarget.textContent;
          clipboardSuccessTarget.textContent = successText;
        }
        if (clipboardDefault && clipboardSuccess) {
          clipboardDefault.style.display = "none";
          clipboardSuccess.style.display = "block";
        }
        if (tooltip) HSTooltip.show($el);

        setTimeout(function () {
          if (clipboardSuccessTarget && tempSuccessText)
            clipboardSuccessTarget.textContent = tempSuccessText;
          if (tooltip) HSTooltip.hide($el);
          if (clipboardDefault && clipboardSuccess) {
            clipboardSuccess.style.display = "";
            clipboardDefault.style.display = "";
          }
        }, 800);
      });

      /* End : Init */
    }

    this._forceChangeTheme(globalIsDark);
    this._forceChangeMarkupType(globalIsJsx);
  },

  _buildToggleLight(el, id) {
    const instance = this._getInstanceById(id);

    if (this._isFormElement(el)) {
      el.addEventListener("change", (evt) => {
        instance.options.isDark = evt.target.checked;

        if (instance.options.isDarkGlobally) {
          this.collection.map((item) => {
            if (item.options.toggleLight !== `#${el.getAttribute("id")}`) {
              const toggle = document.querySelector(item.options.toggleLight);

              if (toggle) {
                toggle.checked = item.options.isDark = !item.options.isDark;

                localStorage.setItem(
                  "hs-clipboard-is-dark",
                  item.options.isDark ? "dark" : "light"
                );
              }
            }
          });
        }
      });
    } else {
      el.addEventListener("click", () => {
        instance.options.isDark = !instance.options.isDark;
      });
    }
  },

  _buildToggleJsx(el, id) {
    const instance = this._getInstanceById(id);

    if (this._isFormElement(el)) {
      el.addEventListener("change", (evt) => {
        instance.options.isJsx = evt.target.checked;

        if (instance.options.isJsxGlobally) {
          this.collection.map((item) => {
            if (item.options.toggleJsx !== `#${el.getAttribute("id")}`) {
              const toggle = document.querySelector(item.options.toggleJsx);

              if (toggle) {
                toggle.checked = item.options.isJsx = !item.options.isJsx;

                localStorage.setItem(
                  "hs-clipboard-is-jsx",
                  item.options.isJsx ? "jsx" : "html"
                );
              }
            }
          });
        }
      });
    } else {
      el.addEventListener("click", () => {
        instance.options.isDark = !instance.options.isDark;
      });
    }
  },

  _getInstanceById(id) {
    return this.collection.find((el) => el.id === id);
  },

  _forceChangeTheme(theme = "dark") {
    const isDark = theme === "dark" ? true : false;

    this.collection.map((item) => {
      const toggle = document.querySelector(item.options.toggleLight);

      if (toggle) toggle.checked = item.options.isDark = isDark;
    });
  },

  _forceChangeMarkupType(markupType = "html") {
    const isJsx = markupType === "jsx" ? true : false;

    this.collection.map((item) => {
      const toggle = document.querySelector(item.options.toggleJsx);

      if (toggle) toggle.checked = item.options.isJsx = isJsx;
    });
  },

  _removeDarkClasses(str) {
    let temp = str.replace(
      /\bdark:[^\s>]*\burl\(['"]?[^'"\)]*['"]?\)/g,
      (match) => match.substring(0, match.indexOf("url(")) + "url()"
    );

    return temp.replace(/(\b|\s)dark:[^"'\s]+/g, "");
  },

  // _convertHtmlToJsx(str) {
  //   const jsxMap = {
  //     "class": "className",
  //     "for": "htmlFor",
  //     "stroke-width": "strokeWidth",
  //     "stroke-linecap": "strokeLinecap",
  //     "stroke-linejoin": "strokeLinejoin",
  //     "view-box": "viewBox",
  //     "clip-path": "clipPath",
  //     "fill-rule": "fillRule",
  //     "stop-color": "stopColor",
  //     "stop-opacity": "stopOpacity",
  //   };

  //   let temp = str.replace(/(\b[a-z\-]+)=/gi, (_, attr) => {
  //     return (jsxMap[attr] || attr) + "=";
  //   });

  //   return temp;
  // },

  _convertHtmlToJsx(str) {
    const attributes = {
      "class": "className",
      "for": "htmlFor",
      "stroke-width": "strokeWidth",
      "stroke-linecap": "strokeLinecap",
      "stroke-linejoin": "strokeLinejoin",
      "view-box": "viewBox",
      "clip-path": "clipPath",
      "fill-rule": "fillRule",
      "stop-color": "stopColor",
      "stop-opacity": "stopOpacity",
    };
    const selfClosingTags = [
      "area", "base", "br", "col", "embed", "hr", "img",
      "input", "keygen", "link", "meta", "param", "source",
      "track", "wbr"
    ];
    let temp = str.replace(/(\b[a-z\-]+)=/gi, (_, attr) => {
      return (attributes[attr] || attr) + '=';
    });
    const selfClosingRegex = new RegExp(
      `<\\s*(${selfClosingTags.join("|")})([^>]*?)>`,
      'gi'
    );

    temp = temp.replace(selfClosingRegex, (match, tag, attrs) => {
      if (/\s*\/\s*$/.test(attrs)) {
        return match;
      }

      return `<${tag}${attrs.trim() ? ' ' + attrs.trim() : ''} />`;
    });
    temp = temp.replace(/<!--([\s\S]*?)-->/g, (match, commentContent) => {
      return `{/*${commentContent}*/}`;
    });

    return temp;
  },

  _isFormElement(el) {
    return (
      el.tagName === "SELECT" ||
      el.tagName === "INPUT" ||
      el.tagName === "TEXTAREA"
    );
  },

  // Public methods
  addToCollection(item, options, id) {
    (options = Object.assign(
      {
        shortCodes: {},
        isDark: globalIsDark ?? true,
        isDarkGlobally: true,
        isJsx: globalIsJsx ?? false,
        isJsxGlobally: true,
      },
      this.defaults,
      item.hasAttribute(this.dataAttributeName)
        ? JSON.parse(item.getAttribute(this.dataAttributeName))
        : {},
      options
    )),
      this.collection.push({
        $el: item,
        id: id || null,
        options: Object.assign({}, options, {
          windowWidth: window.outerWidth,
          defaultText: item.lastChild.nodeValue,
          title: item.getAttribute("data-bs-original-title"),
          container: !!this.defaults.container
            ? document.querySelector(this.defaults.container)
            : false,
          text: (button) => {
            const id = button.getAttribute("id");
            const dataSettings = JSON.parse(
              button.getAttribute("data-hs-clipboard-options")
            );
            const instance = this._getInstanceById(id);
            const markup = instance.options.isDark
              ? (instance.options.isJsx ? options.shortCodes[dataSettings.contentTarget].jsxDark : options.shortCodes[dataSettings.contentTarget].dark)
              : (instance.options.isJsx ? options.shortCodes[dataSettings.contentTarget].jsxLight : options.shortCodes[dataSettings.contentTarget].light);

            // Uncomment the code below for debugging
            // console.log(id, markup);

            return markup;
          },
        }),
      });
  },

  setShortcodes(params) {
    const contentTarget = document.querySelector(params.contentTarget);
    const toggleLight = document.querySelector(params?.toggleLight) || null;
    const toggleJsx = document.querySelector(params?.toggleJsx) || null;

    if (!contentTarget) return false;

    params.shortCodes[params.contentTarget] = {
      dark: null,
      light: null,
      jsxDark: null,
      jsxLight: null,
    };

    if (this._isFormElement(contentTarget)) params.shortCodes[params.contentTarget].dark = contentTarget.value;
    else params.shortCodes[params.contentTarget].dark = contentTarget.textContent;

    if (toggleJsx) params.shortCodes[params.contentTarget].jsxDark = this._convertHtmlToJsx(contentTarget.textContent);

    if (params?.lightOnly || toggleLight) {
      params.shortCodes[params.contentTarget].light = this._removeDarkClasses(contentTarget.textContent);
      params.shortCodes[params.contentTarget].jsxLight = this._convertHtmlToJsx(this._removeDarkClasses(contentTarget.textContent));
    }
  },

  getItems() {
    let newCollection = [];

    for (let i = 0; i < this.collection.length; i += 1)
      newCollection.push(this.collection[i].$initializedEl);

    return newCollection;
  },

  getItem(item) {
    if (typeof item === "number") {
      return this.collection[item].$initializedEl;
    } else {
      return this.collection.find((el) => {
        return el.id === item;
      }).$initializedEl;
    }
  },
};
