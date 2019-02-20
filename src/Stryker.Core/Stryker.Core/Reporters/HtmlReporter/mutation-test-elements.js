/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = "./src/index.ts");
/******/ })
/************************************************************************/
/******/ ({

/***/ "./node_modules/css-loader/dist/cjs.js!./node_modules/highlight.js/styles/default.css":
/*!********************************************************************************************!*\
  !*** ./node_modules/css-loader/dist/cjs.js!./node_modules/highlight.js/styles/default.css ***!
  \********************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(/*! ../../css-loader/dist/runtime/api.js */ "./node_modules/css-loader/dist/runtime/api.js")(false);
// Module
exports.push([module.i, "/*\n\nOriginal highlight.js style (c) Ivan Sagalaev <maniac@softwaremaniacs.org>\n\n*/\n\n.hljs {\n  display: block;\n  overflow-x: auto;\n  padding: 0.5em;\n  background: #F0F0F0;\n}\n\n\n/* Base color: saturation 0; */\n\n.hljs,\n.hljs-subst {\n  color: #444;\n}\n\n.hljs-comment {\n  color: #888888;\n}\n\n.hljs-keyword,\n.hljs-attribute,\n.hljs-selector-tag,\n.hljs-meta-keyword,\n.hljs-doctag,\n.hljs-name {\n  font-weight: bold;\n}\n\n\n/* User color: hue: 0 */\n\n.hljs-type,\n.hljs-string,\n.hljs-number,\n.hljs-selector-id,\n.hljs-selector-class,\n.hljs-quote,\n.hljs-template-tag,\n.hljs-deletion {\n  color: #880000;\n}\n\n.hljs-title,\n.hljs-section {\n  color: #880000;\n  font-weight: bold;\n}\n\n.hljs-regexp,\n.hljs-symbol,\n.hljs-variable,\n.hljs-template-variable,\n.hljs-link,\n.hljs-selector-attr,\n.hljs-selector-pseudo {\n  color: #BC6060;\n}\n\n\n/* Language color: hue: 90; */\n\n.hljs-literal {\n  color: #78A960;\n}\n\n.hljs-built_in,\n.hljs-bullet,\n.hljs-code,\n.hljs-addition {\n  color: #397300;\n}\n\n\n/* Meta color: hue: 200 */\n\n.hljs-meta {\n  color: #1f7199;\n}\n\n.hljs-meta-string {\n  color: #4d99bf;\n}\n\n\n/* Misc effects */\n\n.hljs-emphasis {\n  font-style: italic;\n}\n\n.hljs-strong {\n  font-weight: bold;\n}\n", ""]);



/***/ }),

/***/ "./node_modules/css-loader/dist/runtime/api.js":
/*!*****************************************************!*\
  !*** ./node_modules/css-loader/dist/runtime/api.js ***!
  \*****************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


/*
  MIT License http://www.opensource.org/licenses/mit-license.php
  Author Tobias Koppers @sokra
*/
// css base code, injected by the css-loader
module.exports = function (useSourceMap) {
  var list = []; // return the list of modules as css string

  list.toString = function toString() {
    return this.map(function (item) {
      var content = cssWithMappingToString(item, useSourceMap);

      if (item[2]) {
        return '@media ' + item[2] + '{' + content + '}';
      } else {
        return content;
      }
    }).join('');
  }; // import a list of modules into the list


  list.i = function (modules, mediaQuery) {
    if (typeof modules === 'string') {
      modules = [[null, modules, '']];
    }

    var alreadyImportedModules = {};

    for (var i = 0; i < this.length; i++) {
      var id = this[i][0];

      if (id != null) {
        alreadyImportedModules[id] = true;
      }
    }

    for (i = 0; i < modules.length; i++) {
      var item = modules[i]; // skip already imported module
      // this implementation is not 100% perfect for weird media query combinations
      // when a module is imported multiple times with different media queries.
      // I hope this will never occur (Hey this way we have smaller bundles)

      if (item[0] == null || !alreadyImportedModules[item[0]]) {
        if (mediaQuery && !item[2]) {
          item[2] = mediaQuery;
        } else if (mediaQuery) {
          item[2] = '(' + item[2] + ') and (' + mediaQuery + ')';
        }

        list.push(item);
      }
    }
  };

  return list;
};

function cssWithMappingToString(item, useSourceMap) {
  var content = item[1] || '';
  var cssMapping = item[3];

  if (!cssMapping) {
    return content;
  }

  if (useSourceMap && typeof btoa === 'function') {
    var sourceMapping = toComment(cssMapping);
    var sourceURLs = cssMapping.sources.map(function (source) {
      return '/*# sourceURL=' + cssMapping.sourceRoot + source + ' */';
    });
    return [content].concat(sourceURLs).concat([sourceMapping]).join('\n');
  }

  return [content].join('\n');
} // Adapted from convert-source-map (MIT)


function toComment(sourceMap) {
  // eslint-disable-next-line no-undef
  var base64 = btoa(unescape(encodeURIComponent(JSON.stringify(sourceMap))));
  var data = 'sourceMappingURL=data:application/json;charset=utf-8;base64,' + base64;
  return '/*# ' + data + ' */';
}

/***/ }),

/***/ "./node_modules/highlight.js/lib/highlight.js":
/*!****************************************************!*\
  !*** ./node_modules/highlight.js/lib/highlight.js ***!
  \****************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

/*
Syntax highlighting with language autodetection.
https://highlightjs.org/
*/

(function(factory) {

  // Find the global object for export to both the browser and web workers.
  var globalObject = typeof window === 'object' && window ||
                     typeof self === 'object' && self;

  // Setup highlight.js for different environments. First is Node.js or
  // CommonJS.
  if(true) {
    factory(exports);
  } else {}

}(function(hljs) {
  // Convenience variables for build-in objects
  var ArrayProto = [],
      objectKeys = Object.keys;

  // Global internal variables used within the highlight.js library.
  var languages = {},
      aliases   = {};

  // Regular expressions used throughout the highlight.js library.
  var noHighlightRe    = /^(no-?highlight|plain|text)$/i,
      languagePrefixRe = /\blang(?:uage)?-([\w-]+)\b/i,
      fixMarkupRe      = /((^(<[^>]+>|\t|)+|(?:\n)))/gm;

  var spanEndTag = '</span>';

  // Global options used when within external APIs. This is modified when
  // calling the `hljs.configure` function.
  var options = {
    classPrefix: 'hljs-',
    tabReplace: null,
    useBR: false,
    languages: undefined
  };


  /* Utility functions */

  function escape(value) {
    return value.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
  }

  function tag(node) {
    return node.nodeName.toLowerCase();
  }

  function testRe(re, lexeme) {
    var match = re && re.exec(lexeme);
    return match && match.index === 0;
  }

  function isNotHighlighted(language) {
    return noHighlightRe.test(language);
  }

  function blockLanguage(block) {
    var i, match, length, _class;
    var classes = block.className + ' ';

    classes += block.parentNode ? block.parentNode.className : '';

    // language-* takes precedence over non-prefixed class names.
    match = languagePrefixRe.exec(classes);
    if (match) {
      return getLanguage(match[1]) ? match[1] : 'no-highlight';
    }

    classes = classes.split(/\s+/);

    for (i = 0, length = classes.length; i < length; i++) {
      _class = classes[i];

      if (isNotHighlighted(_class) || getLanguage(_class)) {
        return _class;
      }
    }
  }

  function inherit(parent) {  // inherit(parent, override_obj, override_obj, ...)
    var key;
    var result = {};
    var objects = Array.prototype.slice.call(arguments, 1);

    for (key in parent)
      result[key] = parent[key];
    objects.forEach(function(obj) {
      for (key in obj)
        result[key] = obj[key];
    });
    return result;
  }

  /* Stream merging */

  function nodeStream(node) {
    var result = [];
    (function _nodeStream(node, offset) {
      for (var child = node.firstChild; child; child = child.nextSibling) {
        if (child.nodeType === 3)
          offset += child.nodeValue.length;
        else if (child.nodeType === 1) {
          result.push({
            event: 'start',
            offset: offset,
            node: child
          });
          offset = _nodeStream(child, offset);
          // Prevent void elements from having an end tag that would actually
          // double them in the output. There are more void elements in HTML
          // but we list only those realistically expected in code display.
          if (!tag(child).match(/br|hr|img|input/)) {
            result.push({
              event: 'stop',
              offset: offset,
              node: child
            });
          }
        }
      }
      return offset;
    })(node, 0);
    return result;
  }

  function mergeStreams(original, highlighted, value) {
    var processed = 0;
    var result = '';
    var nodeStack = [];

    function selectStream() {
      if (!original.length || !highlighted.length) {
        return original.length ? original : highlighted;
      }
      if (original[0].offset !== highlighted[0].offset) {
        return (original[0].offset < highlighted[0].offset) ? original : highlighted;
      }

      /*
      To avoid starting the stream just before it should stop the order is
      ensured that original always starts first and closes last:

      if (event1 == 'start' && event2 == 'start')
        return original;
      if (event1 == 'start' && event2 == 'stop')
        return highlighted;
      if (event1 == 'stop' && event2 == 'start')
        return original;
      if (event1 == 'stop' && event2 == 'stop')
        return highlighted;

      ... which is collapsed to:
      */
      return highlighted[0].event === 'start' ? original : highlighted;
    }

    function open(node) {
      function attr_str(a) {return ' ' + a.nodeName + '="' + escape(a.value).replace('"', '&quot;') + '"';}
      result += '<' + tag(node) + ArrayProto.map.call(node.attributes, attr_str).join('') + '>';
    }

    function close(node) {
      result += '</' + tag(node) + '>';
    }

    function render(event) {
      (event.event === 'start' ? open : close)(event.node);
    }

    while (original.length || highlighted.length) {
      var stream = selectStream();
      result += escape(value.substring(processed, stream[0].offset));
      processed = stream[0].offset;
      if (stream === original) {
        /*
        On any opening or closing tag of the original markup we first close
        the entire highlighted node stack, then render the original tag along
        with all the following original tags at the same offset and then
        reopen all the tags on the highlighted stack.
        */
        nodeStack.reverse().forEach(close);
        do {
          render(stream.splice(0, 1)[0]);
          stream = selectStream();
        } while (stream === original && stream.length && stream[0].offset === processed);
        nodeStack.reverse().forEach(open);
      } else {
        if (stream[0].event === 'start') {
          nodeStack.push(stream[0].node);
        } else {
          nodeStack.pop();
        }
        render(stream.splice(0, 1)[0]);
      }
    }
    return result + escape(value.substr(processed));
  }

  /* Initialization */

  function expand_mode(mode) {
    if (mode.variants && !mode.cached_variants) {
      mode.cached_variants = mode.variants.map(function(variant) {
        return inherit(mode, {variants: null}, variant);
      });
    }
    return mode.cached_variants || (mode.endsWithParent && [inherit(mode)]) || [mode];
  }

  function compileLanguage(language) {

    function reStr(re) {
        return (re && re.source) || re;
    }

    function langRe(value, global) {
      return new RegExp(
        reStr(value),
        'm' + (language.case_insensitive ? 'i' : '') + (global ? 'g' : '')
      );
    }

    function compileMode(mode, parent) {
      if (mode.compiled)
        return;
      mode.compiled = true;

      mode.keywords = mode.keywords || mode.beginKeywords;
      if (mode.keywords) {
        var compiled_keywords = {};

        var flatten = function(className, str) {
          if (language.case_insensitive) {
            str = str.toLowerCase();
          }
          str.split(' ').forEach(function(kw) {
            var pair = kw.split('|');
            compiled_keywords[pair[0]] = [className, pair[1] ? Number(pair[1]) : 1];
          });
        };

        if (typeof mode.keywords === 'string') { // string
          flatten('keyword', mode.keywords);
        } else {
          objectKeys(mode.keywords).forEach(function (className) {
            flatten(className, mode.keywords[className]);
          });
        }
        mode.keywords = compiled_keywords;
      }
      mode.lexemesRe = langRe(mode.lexemes || /\w+/, true);

      if (parent) {
        if (mode.beginKeywords) {
          mode.begin = '\\b(' + mode.beginKeywords.split(' ').join('|') + ')\\b';
        }
        if (!mode.begin)
          mode.begin = /\B|\b/;
        mode.beginRe = langRe(mode.begin);
        if (mode.endSameAsBegin)
          mode.end = mode.begin;
        if (!mode.end && !mode.endsWithParent)
          mode.end = /\B|\b/;
        if (mode.end)
          mode.endRe = langRe(mode.end);
        mode.terminator_end = reStr(mode.end) || '';
        if (mode.endsWithParent && parent.terminator_end)
          mode.terminator_end += (mode.end ? '|' : '') + parent.terminator_end;
      }
      if (mode.illegal)
        mode.illegalRe = langRe(mode.illegal);
      if (mode.relevance == null)
        mode.relevance = 1;
      if (!mode.contains) {
        mode.contains = [];
      }
      mode.contains = Array.prototype.concat.apply([], mode.contains.map(function(c) {
        return expand_mode(c === 'self' ? mode : c);
      }));
      mode.contains.forEach(function(c) {compileMode(c, mode);});

      if (mode.starts) {
        compileMode(mode.starts, parent);
      }

      var terminators =
        mode.contains.map(function(c) {
          return c.beginKeywords ? '\\.?(' + c.begin + ')\\.?' : c.begin;
        })
        .concat([mode.terminator_end, mode.illegal])
        .map(reStr)
        .filter(Boolean);
      mode.terminators = terminators.length ? langRe(terminators.join('|'), true) : {exec: function(/*s*/) {return null;}};
    }

    compileMode(language);
  }

  /*
  Core highlighting function. Accepts a language name, or an alias, and a
  string with the code to highlight. Returns an object with the following
  properties:

  - relevance (int)
  - value (an HTML string with highlighting markup)

  */
  function highlight(name, value, ignore_illegals, continuation) {

    function escapeRe(value) {
      return new RegExp(value.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&'), 'm');
    }

    function subMode(lexeme, mode) {
      var i, length;

      for (i = 0, length = mode.contains.length; i < length; i++) {
        if (testRe(mode.contains[i].beginRe, lexeme)) {
          if (mode.contains[i].endSameAsBegin) {
            mode.contains[i].endRe = escapeRe( mode.contains[i].beginRe.exec(lexeme)[0] );
          }
          return mode.contains[i];
        }
      }
    }

    function endOfMode(mode, lexeme) {
      if (testRe(mode.endRe, lexeme)) {
        while (mode.endsParent && mode.parent) {
          mode = mode.parent;
        }
        return mode;
      }
      if (mode.endsWithParent) {
        return endOfMode(mode.parent, lexeme);
      }
    }

    function isIllegal(lexeme, mode) {
      return !ignore_illegals && testRe(mode.illegalRe, lexeme);
    }

    function keywordMatch(mode, match) {
      var match_str = language.case_insensitive ? match[0].toLowerCase() : match[0];
      return mode.keywords.hasOwnProperty(match_str) && mode.keywords[match_str];
    }

    function buildSpan(classname, insideSpan, leaveOpen, noPrefix) {
      var classPrefix = noPrefix ? '' : options.classPrefix,
          openSpan    = '<span class="' + classPrefix,
          closeSpan   = leaveOpen ? '' : spanEndTag;

      openSpan += classname + '">';

      return openSpan + insideSpan + closeSpan;
    }

    function processKeywords() {
      var keyword_match, last_index, match, result;

      if (!top.keywords)
        return escape(mode_buffer);

      result = '';
      last_index = 0;
      top.lexemesRe.lastIndex = 0;
      match = top.lexemesRe.exec(mode_buffer);

      while (match) {
        result += escape(mode_buffer.substring(last_index, match.index));
        keyword_match = keywordMatch(top, match);
        if (keyword_match) {
          relevance += keyword_match[1];
          result += buildSpan(keyword_match[0], escape(match[0]));
        } else {
          result += escape(match[0]);
        }
        last_index = top.lexemesRe.lastIndex;
        match = top.lexemesRe.exec(mode_buffer);
      }
      return result + escape(mode_buffer.substr(last_index));
    }

    function processSubLanguage() {
      var explicit = typeof top.subLanguage === 'string';
      if (explicit && !languages[top.subLanguage]) {
        return escape(mode_buffer);
      }

      var result = explicit ?
                   highlight(top.subLanguage, mode_buffer, true, continuations[top.subLanguage]) :
                   highlightAuto(mode_buffer, top.subLanguage.length ? top.subLanguage : undefined);

      // Counting embedded language score towards the host language may be disabled
      // with zeroing the containing mode relevance. Usecase in point is Markdown that
      // allows XML everywhere and makes every XML snippet to have a much larger Markdown
      // score.
      if (top.relevance > 0) {
        relevance += result.relevance;
      }
      if (explicit) {
        continuations[top.subLanguage] = result.top;
      }
      return buildSpan(result.language, result.value, false, true);
    }

    function processBuffer() {
      result += (top.subLanguage != null ? processSubLanguage() : processKeywords());
      mode_buffer = '';
    }

    function startNewMode(mode) {
      result += mode.className? buildSpan(mode.className, '', true): '';
      top = Object.create(mode, {parent: {value: top}});
    }

    function processLexeme(buffer, lexeme) {

      mode_buffer += buffer;

      if (lexeme == null) {
        processBuffer();
        return 0;
      }

      var new_mode = subMode(lexeme, top);
      if (new_mode) {
        if (new_mode.skip) {
          mode_buffer += lexeme;
        } else {
          if (new_mode.excludeBegin) {
            mode_buffer += lexeme;
          }
          processBuffer();
          if (!new_mode.returnBegin && !new_mode.excludeBegin) {
            mode_buffer = lexeme;
          }
        }
        startNewMode(new_mode, lexeme);
        return new_mode.returnBegin ? 0 : lexeme.length;
      }

      var end_mode = endOfMode(top, lexeme);
      if (end_mode) {
        var origin = top;
        if (origin.skip) {
          mode_buffer += lexeme;
        } else {
          if (!(origin.returnEnd || origin.excludeEnd)) {
            mode_buffer += lexeme;
          }
          processBuffer();
          if (origin.excludeEnd) {
            mode_buffer = lexeme;
          }
        }
        do {
          if (top.className) {
            result += spanEndTag;
          }
          if (!top.skip && !top.subLanguage) {
            relevance += top.relevance;
          }
          top = top.parent;
        } while (top !== end_mode.parent);
        if (end_mode.starts) {
          if (end_mode.endSameAsBegin) {
            end_mode.starts.endRe = end_mode.endRe;
          }
          startNewMode(end_mode.starts, '');
        }
        return origin.returnEnd ? 0 : lexeme.length;
      }

      if (isIllegal(lexeme, top))
        throw new Error('Illegal lexeme "' + lexeme + '" for mode "' + (top.className || '<unnamed>') + '"');

      /*
      Parser should not reach this point as all types of lexemes should be caught
      earlier, but if it does due to some bug make sure it advances at least one
      character forward to prevent infinite looping.
      */
      mode_buffer += lexeme;
      return lexeme.length || 1;
    }

    var language = getLanguage(name);
    if (!language) {
      throw new Error('Unknown language: "' + name + '"');
    }

    compileLanguage(language);
    var top = continuation || language;
    var continuations = {}; // keep continuations for sub-languages
    var result = '', current;
    for(current = top; current !== language; current = current.parent) {
      if (current.className) {
        result = buildSpan(current.className, '', true) + result;
      }
    }
    var mode_buffer = '';
    var relevance = 0;
    try {
      var match, count, index = 0;
      while (true) {
        top.terminators.lastIndex = index;
        match = top.terminators.exec(value);
        if (!match)
          break;
        count = processLexeme(value.substring(index, match.index), match[0]);
        index = match.index + count;
      }
      processLexeme(value.substr(index));
      for(current = top; current.parent; current = current.parent) { // close dangling modes
        if (current.className) {
          result += spanEndTag;
        }
      }
      return {
        relevance: relevance,
        value: result,
        language: name,
        top: top
      };
    } catch (e) {
      if (e.message && e.message.indexOf('Illegal') !== -1) {
        return {
          relevance: 0,
          value: escape(value)
        };
      } else {
        throw e;
      }
    }
  }

  /*
  Highlighting with language detection. Accepts a string with the code to
  highlight. Returns an object with the following properties:

  - language (detected language)
  - relevance (int)
  - value (an HTML string with highlighting markup)
  - second_best (object with the same structure for second-best heuristically
    detected language, may be absent)

  */
  function highlightAuto(text, languageSubset) {
    languageSubset = languageSubset || options.languages || objectKeys(languages);
    var result = {
      relevance: 0,
      value: escape(text)
    };
    var second_best = result;
    languageSubset.filter(getLanguage).filter(autoDetection).forEach(function(name) {
      var current = highlight(name, text, false);
      current.language = name;
      if (current.relevance > second_best.relevance) {
        second_best = current;
      }
      if (current.relevance > result.relevance) {
        second_best = result;
        result = current;
      }
    });
    if (second_best.language) {
      result.second_best = second_best;
    }
    return result;
  }

  /*
  Post-processing of the highlighted markup:

  - replace TABs with something more useful
  - replace real line-breaks with '<br>' for non-pre containers

  */
  function fixMarkup(value) {
    return !(options.tabReplace || options.useBR)
      ? value
      : value.replace(fixMarkupRe, function(match, p1) {
          if (options.useBR && match === '\n') {
            return '<br>';
          } else if (options.tabReplace) {
            return p1.replace(/\t/g, options.tabReplace);
          }
          return '';
      });
  }

  function buildClassName(prevClassName, currentLang, resultLang) {
    var language = currentLang ? aliases[currentLang] : resultLang,
        result   = [prevClassName.trim()];

    if (!prevClassName.match(/\bhljs\b/)) {
      result.push('hljs');
    }

    if (prevClassName.indexOf(language) === -1) {
      result.push(language);
    }

    return result.join(' ').trim();
  }

  /*
  Applies highlighting to a DOM node containing code. Accepts a DOM node and
  two optional parameters for fixMarkup.
  */
  function highlightBlock(block) {
    var node, originalStream, result, resultNode, text;
    var language = blockLanguage(block);

    if (isNotHighlighted(language))
        return;

    if (options.useBR) {
      node = document.createElementNS('http://www.w3.org/1999/xhtml', 'div');
      node.innerHTML = block.innerHTML.replace(/\n/g, '').replace(/<br[ \/]*>/g, '\n');
    } else {
      node = block;
    }
    text = node.textContent;
    result = language ? highlight(language, text, true) : highlightAuto(text);

    originalStream = nodeStream(node);
    if (originalStream.length) {
      resultNode = document.createElementNS('http://www.w3.org/1999/xhtml', 'div');
      resultNode.innerHTML = result.value;
      result.value = mergeStreams(originalStream, nodeStream(resultNode), text);
    }
    result.value = fixMarkup(result.value);

    block.innerHTML = result.value;
    block.className = buildClassName(block.className, language, result.language);
    block.result = {
      language: result.language,
      re: result.relevance
    };
    if (result.second_best) {
      block.second_best = {
        language: result.second_best.language,
        re: result.second_best.relevance
      };
    }
  }

  /*
  Updates highlight.js global options with values passed in the form of an object.
  */
  function configure(user_options) {
    options = inherit(options, user_options);
  }

  /*
  Applies highlighting to all <pre><code>..</code></pre> blocks on a page.
  */
  function initHighlighting() {
    if (initHighlighting.called)
      return;
    initHighlighting.called = true;

    var blocks = document.querySelectorAll('pre code');
    ArrayProto.forEach.call(blocks, highlightBlock);
  }

  /*
  Attaches highlighting to the page load event.
  */
  function initHighlightingOnLoad() {
    addEventListener('DOMContentLoaded', initHighlighting, false);
    addEventListener('load', initHighlighting, false);
  }

  function registerLanguage(name, language) {
    var lang = languages[name] = language(hljs);
    if (lang.aliases) {
      lang.aliases.forEach(function(alias) {aliases[alias] = name;});
    }
  }

  function listLanguages() {
    return objectKeys(languages);
  }

  function getLanguage(name) {
    name = (name || '').toLowerCase();
    return languages[name] || languages[aliases[name]];
  }

  function autoDetection(name) {
    var lang = getLanguage(name);
    return lang && !lang.disableAutodetect;
  }

  /* Interface definition */

  hljs.highlight = highlight;
  hljs.highlightAuto = highlightAuto;
  hljs.fixMarkup = fixMarkup;
  hljs.highlightBlock = highlightBlock;
  hljs.configure = configure;
  hljs.initHighlighting = initHighlighting;
  hljs.initHighlightingOnLoad = initHighlightingOnLoad;
  hljs.registerLanguage = registerLanguage;
  hljs.listLanguages = listLanguages;
  hljs.getLanguage = getLanguage;
  hljs.autoDetection = autoDetection;
  hljs.inherit = inherit;

  // Common regexps
  hljs.IDENT_RE = '[a-zA-Z]\\w*';
  hljs.UNDERSCORE_IDENT_RE = '[a-zA-Z_]\\w*';
  hljs.NUMBER_RE = '\\b\\d+(\\.\\d+)?';
  hljs.C_NUMBER_RE = '(-?)(\\b0[xX][a-fA-F0-9]+|(\\b\\d+(\\.\\d*)?|\\.\\d+)([eE][-+]?\\d+)?)'; // 0x..., 0..., decimal, float
  hljs.BINARY_NUMBER_RE = '\\b(0b[01]+)'; // 0b...
  hljs.RE_STARTERS_RE = '!|!=|!==|%|%=|&|&&|&=|\\*|\\*=|\\+|\\+=|,|-|-=|/=|/|:|;|<<|<<=|<=|<|===|==|=|>>>=|>>=|>=|>>>|>>|>|\\?|\\[|\\{|\\(|\\^|\\^=|\\||\\|=|\\|\\||~';

  // Common modes
  hljs.BACKSLASH_ESCAPE = {
    begin: '\\\\[\\s\\S]', relevance: 0
  };
  hljs.APOS_STRING_MODE = {
    className: 'string',
    begin: '\'', end: '\'',
    illegal: '\\n',
    contains: [hljs.BACKSLASH_ESCAPE]
  };
  hljs.QUOTE_STRING_MODE = {
    className: 'string',
    begin: '"', end: '"',
    illegal: '\\n',
    contains: [hljs.BACKSLASH_ESCAPE]
  };
  hljs.PHRASAL_WORDS_MODE = {
    begin: /\b(a|an|the|are|I'm|isn't|don't|doesn't|won't|but|just|should|pretty|simply|enough|gonna|going|wtf|so|such|will|you|your|they|like|more)\b/
  };
  hljs.COMMENT = function (begin, end, inherits) {
    var mode = hljs.inherit(
      {
        className: 'comment',
        begin: begin, end: end,
        contains: []
      },
      inherits || {}
    );
    mode.contains.push(hljs.PHRASAL_WORDS_MODE);
    mode.contains.push({
      className: 'doctag',
      begin: '(?:TODO|FIXME|NOTE|BUG|XXX):',
      relevance: 0
    });
    return mode;
  };
  hljs.C_LINE_COMMENT_MODE = hljs.COMMENT('//', '$');
  hljs.C_BLOCK_COMMENT_MODE = hljs.COMMENT('/\\*', '\\*/');
  hljs.HASH_COMMENT_MODE = hljs.COMMENT('#', '$');
  hljs.NUMBER_MODE = {
    className: 'number',
    begin: hljs.NUMBER_RE,
    relevance: 0
  };
  hljs.C_NUMBER_MODE = {
    className: 'number',
    begin: hljs.C_NUMBER_RE,
    relevance: 0
  };
  hljs.BINARY_NUMBER_MODE = {
    className: 'number',
    begin: hljs.BINARY_NUMBER_RE,
    relevance: 0
  };
  hljs.CSS_NUMBER_MODE = {
    className: 'number',
    begin: hljs.NUMBER_RE + '(' +
      '%|em|ex|ch|rem'  +
      '|vw|vh|vmin|vmax' +
      '|cm|mm|in|pt|pc|px' +
      '|deg|grad|rad|turn' +
      '|s|ms' +
      '|Hz|kHz' +
      '|dpi|dpcm|dppx' +
      ')?',
    relevance: 0
  };
  hljs.REGEXP_MODE = {
    className: 'regexp',
    begin: /\//, end: /\/[gimuy]*/,
    illegal: /\n/,
    contains: [
      hljs.BACKSLASH_ESCAPE,
      {
        begin: /\[/, end: /\]/,
        relevance: 0,
        contains: [hljs.BACKSLASH_ESCAPE]
      }
    ]
  };
  hljs.TITLE_MODE = {
    className: 'title',
    begin: hljs.IDENT_RE,
    relevance: 0
  };
  hljs.UNDERSCORE_TITLE_MODE = {
    className: 'title',
    begin: hljs.UNDERSCORE_IDENT_RE,
    relevance: 0
  };
  hljs.METHOD_GUARD = {
    // excludes method names from keyword processing
    begin: '\\.\\s*' + hljs.UNDERSCORE_IDENT_RE,
    relevance: 0
  };

  return hljs;
}));


/***/ }),

/***/ "./node_modules/highlight.js/lib/languages/cs.js":
/*!*******************************************************!*\
  !*** ./node_modules/highlight.js/lib/languages/cs.js ***!
  \*******************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = function(hljs) {
  var KEYWORDS = {
    keyword:
      // Normal keywords.
      'abstract as base bool break byte case catch char checked const continue decimal ' +
      'default delegate do double enum event explicit extern finally fixed float ' +
      'for foreach goto if implicit in int interface internal is lock long nameof ' +
      'object operator out override params private protected public readonly ref sbyte ' +
      'sealed short sizeof stackalloc static string struct switch this try typeof ' +
      'uint ulong unchecked unsafe ushort using virtual void volatile while ' +
      // Contextual keywords.
      'add alias ascending async await by descending dynamic equals from get global group into join ' +
      'let on orderby partial remove select set value var where yield',
    literal:
      'null false true'
  };
  var NUMBERS = {
    className: 'number',
    variants: [
      { begin: '\\b(0b[01\']+)' },
      { begin: '(-?)\\b([\\d\']+(\\.[\\d\']*)?|\\.[\\d\']+)(u|U|l|L|ul|UL|f|F|b|B)' },
      { begin: '(-?)(\\b0[xX][a-fA-F0-9\']+|(\\b[\\d\']+(\\.[\\d\']*)?|\\.[\\d\']+)([eE][-+]?[\\d\']+)?)' }
    ],
    relevance: 0
  };
  var VERBATIM_STRING = {
    className: 'string',
    begin: '@"', end: '"',
    contains: [{begin: '""'}]
  };
  var VERBATIM_STRING_NO_LF = hljs.inherit(VERBATIM_STRING, {illegal: /\n/});
  var SUBST = {
    className: 'subst',
    begin: '{', end: '}',
    keywords: KEYWORDS
  };
  var SUBST_NO_LF = hljs.inherit(SUBST, {illegal: /\n/});
  var INTERPOLATED_STRING = {
    className: 'string',
    begin: /\$"/, end: '"',
    illegal: /\n/,
    contains: [{begin: '{{'}, {begin: '}}'}, hljs.BACKSLASH_ESCAPE, SUBST_NO_LF]
  };
  var INTERPOLATED_VERBATIM_STRING = {
    className: 'string',
    begin: /\$@"/, end: '"',
    contains: [{begin: '{{'}, {begin: '}}'}, {begin: '""'}, SUBST]
  };
  var INTERPOLATED_VERBATIM_STRING_NO_LF = hljs.inherit(INTERPOLATED_VERBATIM_STRING, {
    illegal: /\n/,
    contains: [{begin: '{{'}, {begin: '}}'}, {begin: '""'}, SUBST_NO_LF]
  });
  SUBST.contains = [
    INTERPOLATED_VERBATIM_STRING,
    INTERPOLATED_STRING,
    VERBATIM_STRING,
    hljs.APOS_STRING_MODE,
    hljs.QUOTE_STRING_MODE,
    NUMBERS,
    hljs.C_BLOCK_COMMENT_MODE
  ];
  SUBST_NO_LF.contains = [
    INTERPOLATED_VERBATIM_STRING_NO_LF,
    INTERPOLATED_STRING,
    VERBATIM_STRING_NO_LF,
    hljs.APOS_STRING_MODE,
    hljs.QUOTE_STRING_MODE,
    NUMBERS,
    hljs.inherit(hljs.C_BLOCK_COMMENT_MODE, {illegal: /\n/})
  ];
  var STRING = {
    variants: [
      INTERPOLATED_VERBATIM_STRING,
      INTERPOLATED_STRING,
      VERBATIM_STRING,
      hljs.APOS_STRING_MODE,
      hljs.QUOTE_STRING_MODE
    ]
  };

  var TYPE_IDENT_RE = hljs.IDENT_RE + '(<' + hljs.IDENT_RE + '(\\s*,\\s*' + hljs.IDENT_RE + ')*>)?(\\[\\])?';

  return {
    aliases: ['csharp', 'c#'],
    keywords: KEYWORDS,
    illegal: /::/,
    contains: [
      hljs.COMMENT(
        '///',
        '$',
        {
          returnBegin: true,
          contains: [
            {
              className: 'doctag',
              variants: [
                {
                  begin: '///', relevance: 0
                },
                {
                  begin: '<!--|-->'
                },
                {
                  begin: '</?', end: '>'
                }
              ]
            }
          ]
        }
      ),
      hljs.C_LINE_COMMENT_MODE,
      hljs.C_BLOCK_COMMENT_MODE,
      {
        className: 'meta',
        begin: '#', end: '$',
        keywords: {
          'meta-keyword': 'if else elif endif define undef warning error line region endregion pragma checksum'
        }
      },
      STRING,
      NUMBERS,
      {
        beginKeywords: 'class interface', end: /[{;=]/,
        illegal: /[^\s:,]/,
        contains: [
          hljs.TITLE_MODE,
          hljs.C_LINE_COMMENT_MODE,
          hljs.C_BLOCK_COMMENT_MODE
        ]
      },
      {
        beginKeywords: 'namespace', end: /[{;=]/,
        illegal: /[^\s:]/,
        contains: [
          hljs.inherit(hljs.TITLE_MODE, {begin: '[a-zA-Z](\\.?\\w)*'}),
          hljs.C_LINE_COMMENT_MODE,
          hljs.C_BLOCK_COMMENT_MODE
        ]
      },
      {
        // [Attributes("")]
        className: 'meta',
        begin: '^\\s*\\[', excludeBegin: true, end: '\\]', excludeEnd: true,
        contains: [
          {className: 'meta-string', begin: /"/, end: /"/}
        ]
      },
      {
        // Expression keywords prevent 'keyword Name(...)' from being
        // recognized as a function definition
        beginKeywords: 'new return throw await else',
        relevance: 0
      },
      {
        className: 'function',
        begin: '(' + TYPE_IDENT_RE + '\\s+)+' + hljs.IDENT_RE + '\\s*\\(', returnBegin: true,
        end: /\s*[{;=]/, excludeEnd: true,
        keywords: KEYWORDS,
        contains: [
          {
            begin: hljs.IDENT_RE + '\\s*\\(', returnBegin: true,
            contains: [hljs.TITLE_MODE],
            relevance: 0
          },
          {
            className: 'params',
            begin: /\(/, end: /\)/,
            excludeBegin: true,
            excludeEnd: true,
            keywords: KEYWORDS,
            relevance: 0,
            contains: [
              STRING,
              NUMBERS,
              hljs.C_BLOCK_COMMENT_MODE
            ]
          },
          hljs.C_LINE_COMMENT_MODE,
          hljs.C_BLOCK_COMMENT_MODE
        ]
      }
    ]
  };
};

/***/ }),

/***/ "./node_modules/highlight.js/lib/languages/java.js":
/*!*********************************************************!*\
  !*** ./node_modules/highlight.js/lib/languages/java.js ***!
  \*********************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = function(hljs) {
  var JAVA_IDENT_RE = '[\u00C0-\u02B8a-zA-Z_$][\u00C0-\u02B8a-zA-Z_$0-9]*';
  var GENERIC_IDENT_RE = JAVA_IDENT_RE + '(<' + JAVA_IDENT_RE + '(\\s*,\\s*' + JAVA_IDENT_RE + ')*>)?';
  var KEYWORDS =
    'false synchronized int abstract float private char boolean var static null if const ' +
    'for true while long strictfp finally protected import native final void ' +
    'enum else break transient catch instanceof byte super volatile case assert short ' +
    'package default double public try this switch continue throws protected public private ' +
    'module requires exports do';

  // https://docs.oracle.com/javase/7/docs/technotes/guides/language/underscores-literals.html
  var JAVA_NUMBER_RE = '\\b' +
    '(' +
      '0[bB]([01]+[01_]+[01]+|[01]+)' + // 0b...
      '|' +
      '0[xX]([a-fA-F0-9]+[a-fA-F0-9_]+[a-fA-F0-9]+|[a-fA-F0-9]+)' + // 0x...
      '|' +
      '(' +
        '([\\d]+[\\d_]+[\\d]+|[\\d]+)(\\.([\\d]+[\\d_]+[\\d]+|[\\d]+))?' +
        '|' +
        '\\.([\\d]+[\\d_]+[\\d]+|[\\d]+)' +
      ')' +
      '([eE][-+]?\\d+)?' + // octal, decimal, float
    ')' +
    '[lLfF]?';
  var JAVA_NUMBER_MODE = {
    className: 'number',
    begin: JAVA_NUMBER_RE,
    relevance: 0
  };

  return {
    aliases: ['jsp'],
    keywords: KEYWORDS,
    illegal: /<\/|#/,
    contains: [
      hljs.COMMENT(
        '/\\*\\*',
        '\\*/',
        {
          relevance : 0,
          contains : [
            {
              // eat up @'s in emails to prevent them to be recognized as doctags
              begin: /\w+@/, relevance: 0
            },
            {
              className : 'doctag',
              begin : '@[A-Za-z]+'
            }
          ]
        }
      ),
      hljs.C_LINE_COMMENT_MODE,
      hljs.C_BLOCK_COMMENT_MODE,
      hljs.APOS_STRING_MODE,
      hljs.QUOTE_STRING_MODE,
      {
        className: 'class',
        beginKeywords: 'class interface', end: /[{;=]/, excludeEnd: true,
        keywords: 'class interface',
        illegal: /[:"\[\]]/,
        contains: [
          {beginKeywords: 'extends implements'},
          hljs.UNDERSCORE_TITLE_MODE
        ]
      },
      {
        // Expression keywords prevent 'keyword Name(...)' from being
        // recognized as a function definition
        beginKeywords: 'new throw return else',
        relevance: 0
      },
      {
        className: 'function',
        begin: '(' + GENERIC_IDENT_RE + '\\s+)+' + hljs.UNDERSCORE_IDENT_RE + '\\s*\\(', returnBegin: true, end: /[{;=]/,
        excludeEnd: true,
        keywords: KEYWORDS,
        contains: [
          {
            begin: hljs.UNDERSCORE_IDENT_RE + '\\s*\\(', returnBegin: true,
            relevance: 0,
            contains: [hljs.UNDERSCORE_TITLE_MODE]
          },
          {
            className: 'params',
            begin: /\(/, end: /\)/,
            keywords: KEYWORDS,
            relevance: 0,
            contains: [
              hljs.APOS_STRING_MODE,
              hljs.QUOTE_STRING_MODE,
              hljs.C_NUMBER_MODE,
              hljs.C_BLOCK_COMMENT_MODE
            ]
          },
          hljs.C_LINE_COMMENT_MODE,
          hljs.C_BLOCK_COMMENT_MODE
        ]
      },
      JAVA_NUMBER_MODE,
      {
        className: 'meta', begin: '@[A-Za-z]+'
      }
    ]
  };
};

/***/ }),

/***/ "./node_modules/highlight.js/lib/languages/javascript.js":
/*!***************************************************************!*\
  !*** ./node_modules/highlight.js/lib/languages/javascript.js ***!
  \***************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = function(hljs) {
  var IDENT_RE = '[A-Za-z$_][0-9A-Za-z$_]*';
  var KEYWORDS = {
    keyword:
      'in of if for while finally var new function do return void else break catch ' +
      'instanceof with throw case default try this switch continue typeof delete ' +
      'let yield const export super debugger as async await static ' +
      // ECMAScript 6 modules import
      'import from as'
    ,
    literal:
      'true false null undefined NaN Infinity',
    built_in:
      'eval isFinite isNaN parseFloat parseInt decodeURI decodeURIComponent ' +
      'encodeURI encodeURIComponent escape unescape Object Function Boolean Error ' +
      'EvalError InternalError RangeError ReferenceError StopIteration SyntaxError ' +
      'TypeError URIError Number Math Date String RegExp Array Float32Array ' +
      'Float64Array Int16Array Int32Array Int8Array Uint16Array Uint32Array ' +
      'Uint8Array Uint8ClampedArray ArrayBuffer DataView JSON Intl arguments require ' +
      'module console window document Symbol Set Map WeakSet WeakMap Proxy Reflect ' +
      'Promise'
  };
  var NUMBER = {
    className: 'number',
    variants: [
      { begin: '\\b(0[bB][01]+)' },
      { begin: '\\b(0[oO][0-7]+)' },
      { begin: hljs.C_NUMBER_RE }
    ],
    relevance: 0
  };
  var SUBST = {
    className: 'subst',
    begin: '\\$\\{', end: '\\}',
    keywords: KEYWORDS,
    contains: []  // defined later
  };
  var TEMPLATE_STRING = {
    className: 'string',
    begin: '`', end: '`',
    contains: [
      hljs.BACKSLASH_ESCAPE,
      SUBST
    ]
  };
  SUBST.contains = [
    hljs.APOS_STRING_MODE,
    hljs.QUOTE_STRING_MODE,
    TEMPLATE_STRING,
    NUMBER,
    hljs.REGEXP_MODE
  ]
  var PARAMS_CONTAINS = SUBST.contains.concat([
    hljs.C_BLOCK_COMMENT_MODE,
    hljs.C_LINE_COMMENT_MODE
  ]);

  return {
    aliases: ['js', 'jsx'],
    keywords: KEYWORDS,
    contains: [
      {
        className: 'meta',
        relevance: 10,
        begin: /^\s*['"]use (strict|asm)['"]/
      },
      {
        className: 'meta',
        begin: /^#!/, end: /$/
      },
      hljs.APOS_STRING_MODE,
      hljs.QUOTE_STRING_MODE,
      TEMPLATE_STRING,
      hljs.C_LINE_COMMENT_MODE,
      hljs.C_BLOCK_COMMENT_MODE,
      NUMBER,
      { // object attr container
        begin: /[{,]\s*/, relevance: 0,
        contains: [
          {
            begin: IDENT_RE + '\\s*:', returnBegin: true,
            relevance: 0,
            contains: [{className: 'attr', begin: IDENT_RE, relevance: 0}]
          }
        ]
      },
      { // "value" container
        begin: '(' + hljs.RE_STARTERS_RE + '|\\b(case|return|throw)\\b)\\s*',
        keywords: 'return throw case',
        contains: [
          hljs.C_LINE_COMMENT_MODE,
          hljs.C_BLOCK_COMMENT_MODE,
          hljs.REGEXP_MODE,
          {
            className: 'function',
            begin: '(\\(.*?\\)|' + IDENT_RE + ')\\s*=>', returnBegin: true,
            end: '\\s*=>',
            contains: [
              {
                className: 'params',
                variants: [
                  {
                    begin: IDENT_RE
                  },
                  {
                    begin: /\(\s*\)/,
                  },
                  {
                    begin: /\(/, end: /\)/,
                    excludeBegin: true, excludeEnd: true,
                    keywords: KEYWORDS,
                    contains: PARAMS_CONTAINS
                  }
                ]
              }
            ]
          },
          { // E4X / JSX
            begin: /</, end: /(\/\w+|\w+\/)>/,
            subLanguage: 'xml',
            contains: [
              {begin: /<\w+\s*\/>/, skip: true},
              {
                begin: /<\w+/, end: /(\/\w+|\w+\/)>/, skip: true,
                contains: [
                  {begin: /<\w+\s*\/>/, skip: true},
                  'self'
                ]
              }
            ]
          }
        ],
        relevance: 0
      },
      {
        className: 'function',
        beginKeywords: 'function', end: /\{/, excludeEnd: true,
        contains: [
          hljs.inherit(hljs.TITLE_MODE, {begin: IDENT_RE}),
          {
            className: 'params',
            begin: /\(/, end: /\)/,
            excludeBegin: true,
            excludeEnd: true,
            contains: PARAMS_CONTAINS
          }
        ],
        illegal: /\[|%/
      },
      {
        begin: /\$[(.]/ // relevance booster for a pattern common to JS libs: `$(something)` and `$.something`
      },
      hljs.METHOD_GUARD,
      { // ES6 class
        className: 'class',
        beginKeywords: 'class', end: /[{;=]/, excludeEnd: true,
        illegal: /[:"\[\]]/,
        contains: [
          {beginKeywords: 'extends'},
          hljs.UNDERSCORE_TITLE_MODE
        ]
      },
      {
        beginKeywords: 'constructor get set', end: /\{/, excludeEnd: true
      }
    ],
    illegal: /#(?!!)/
  };
};

/***/ }),

/***/ "./node_modules/highlight.js/lib/languages/scala.js":
/*!**********************************************************!*\
  !*** ./node_modules/highlight.js/lib/languages/scala.js ***!
  \**********************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = function(hljs) {

  var ANNOTATION = { className: 'meta', begin: '@[A-Za-z]+' };

  // used in strings for escaping/interpolation/substitution
  var SUBST = {
    className: 'subst',
    variants: [
      {begin: '\\$[A-Za-z0-9_]+'},
      {begin: '\\${', end: '}'}
    ]
  };

  var STRING = {
    className: 'string',
    variants: [
      {
        begin: '"', end: '"',
        illegal: '\\n',
        contains: [hljs.BACKSLASH_ESCAPE]
      },
      {
        begin: '"""', end: '"""',
        relevance: 10
      },
      {
        begin: '[a-z]+"', end: '"',
        illegal: '\\n',
        contains: [hljs.BACKSLASH_ESCAPE, SUBST]
      },
      {
        className: 'string',
        begin: '[a-z]+"""', end: '"""',
        contains: [SUBST],
        relevance: 10
      }
    ]

  };

  var SYMBOL = {
    className: 'symbol',
    begin: '\'\\w[\\w\\d_]*(?!\')'
  };

  var TYPE = {
    className: 'type',
    begin: '\\b[A-Z][A-Za-z0-9_]*',
    relevance: 0
  };

  var NAME = {
    className: 'title',
    begin: /[^0-9\n\t "'(),.`{}\[\]:;][^\n\t "'(),.`{}\[\]:;]+|[^0-9\n\t "'(),.`{}\[\]:;=]/,
    relevance: 0
  };

  var CLASS = {
    className: 'class',
    beginKeywords: 'class object trait type',
    end: /[:={\[\n;]/,
    excludeEnd: true,
    contains: [
      {
        beginKeywords: 'extends with',
        relevance: 10
      },
      {
        begin: /\[/,
        end: /\]/,
        excludeBegin: true,
        excludeEnd: true,
        relevance: 0,
        contains: [TYPE]
      },
      {
        className: 'params',
        begin: /\(/,
        end: /\)/,
        excludeBegin: true,
        excludeEnd: true,
        relevance: 0,
        contains: [TYPE]
      },
      NAME
    ]
  };

  var METHOD = {
    className: 'function',
    beginKeywords: 'def',
    end: /[:={\[(\n;]/,
    excludeEnd: true,
    contains: [NAME]
  };

  return {
    keywords: {
      literal: 'true false null',
      keyword: 'type yield lazy override def with val var sealed abstract private trait object if forSome for while throw finally protected extends import final return else break new catch super class case package default try this match continue throws implicit'
    },
    contains: [
      hljs.C_LINE_COMMENT_MODE,
      hljs.C_BLOCK_COMMENT_MODE,
      STRING,
      SYMBOL,
      TYPE,
      METHOD,
      CLASS,
      hljs.C_NUMBER_MODE,
      ANNOTATION
    ]
  };
};

/***/ }),

/***/ "./node_modules/highlight.js/lib/languages/typescript.js":
/*!***************************************************************!*\
  !*** ./node_modules/highlight.js/lib/languages/typescript.js ***!
  \***************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = function(hljs) {
  var JS_IDENT_RE = '[A-Za-z$_][0-9A-Za-z$_]*';
  var KEYWORDS = {
    keyword:
      'in if for while finally var new function do return void else break catch ' +
      'instanceof with throw case default try this switch continue typeof delete ' +
      'let yield const class public private protected get set super ' +
      'static implements enum export import declare type namespace abstract ' +
      'as from extends async await',
    literal:
      'true false null undefined NaN Infinity',
    built_in:
      'eval isFinite isNaN parseFloat parseInt decodeURI decodeURIComponent ' +
      'encodeURI encodeURIComponent escape unescape Object Function Boolean Error ' +
      'EvalError InternalError RangeError ReferenceError StopIteration SyntaxError ' +
      'TypeError URIError Number Math Date String RegExp Array Float32Array ' +
      'Float64Array Int16Array Int32Array Int8Array Uint16Array Uint32Array ' +
      'Uint8Array Uint8ClampedArray ArrayBuffer DataView JSON Intl arguments require ' +
      'module console window document any number boolean string void Promise'
  };

  var DECORATOR = {
    className: 'meta',
    begin: '@' + JS_IDENT_RE,
  };

  var ARGS =
  {
    begin: '\\(',
    end: /\)/,
    keywords: KEYWORDS,
    contains: [
      'self',
      hljs.QUOTE_STRING_MODE,
      hljs.APOS_STRING_MODE,
      hljs.NUMBER_MODE
    ]
  };

  var PARAMS = {
    className: 'params',
    begin: /\(/, end: /\)/,
    excludeBegin: true,
    excludeEnd: true,
    keywords: KEYWORDS,
    contains: [
      hljs.C_LINE_COMMENT_MODE,
      hljs.C_BLOCK_COMMENT_MODE,
      DECORATOR,
      ARGS
    ]
  };

  return {
    aliases: ['ts'],
    keywords: KEYWORDS,
    contains: [
      {
        className: 'meta',
        begin: /^\s*['"]use strict['"]/
      },
      hljs.APOS_STRING_MODE,
      hljs.QUOTE_STRING_MODE,
      { // template string
        className: 'string',
        begin: '`', end: '`',
        contains: [
          hljs.BACKSLASH_ESCAPE,
          {
            className: 'subst',
            begin: '\\$\\{', end: '\\}'
          }
        ]
      },
      hljs.C_LINE_COMMENT_MODE,
      hljs.C_BLOCK_COMMENT_MODE,
      {
        className: 'number',
        variants: [
          { begin: '\\b(0[bB][01]+)' },
          { begin: '\\b(0[oO][0-7]+)' },
          { begin: hljs.C_NUMBER_RE }
        ],
        relevance: 0
      },
      { // "value" container
        begin: '(' + hljs.RE_STARTERS_RE + '|\\b(case|return|throw)\\b)\\s*',
        keywords: 'return throw case',
        contains: [
          hljs.C_LINE_COMMENT_MODE,
          hljs.C_BLOCK_COMMENT_MODE,
          hljs.REGEXP_MODE,
          {
            className: 'function',
            begin: '(\\(.*?\\)|' + hljs.IDENT_RE + ')\\s*=>', returnBegin: true,
            end: '\\s*=>',
            contains: [
              {
                className: 'params',
                variants: [
                  {
                    begin: hljs.IDENT_RE
                  },
                  {
                    begin: /\(\s*\)/,
                  },
                  {
                    begin: /\(/, end: /\)/,
                    excludeBegin: true, excludeEnd: true,
                    keywords: KEYWORDS,
                    contains: [
                      'self',
                      hljs.C_LINE_COMMENT_MODE,
                      hljs.C_BLOCK_COMMENT_MODE
                    ]
                  }
                ]
              }
            ]
          }
        ],
        relevance: 0
      },
      {
        className: 'function',
        begin: 'function', end: /[\{;]/, excludeEnd: true,
        keywords: KEYWORDS,
        contains: [
          'self',
          hljs.inherit(hljs.TITLE_MODE, { begin: JS_IDENT_RE }),
          PARAMS
        ],
        illegal: /%/,
        relevance: 0 // () => {} is more typical in TypeScript
      },
      {
        beginKeywords: 'constructor', end: /\{/, excludeEnd: true,
        contains: [
          'self',
          PARAMS
        ]
      },
      { // prevent references like module.id from being higlighted as module definitions
        begin: /module\./,
        keywords: { built_in: 'module' },
        relevance: 0
      },
      {
        beginKeywords: 'module', end: /\{/, excludeEnd: true
      },
      {
        beginKeywords: 'interface', end: /\{/, excludeEnd: true,
        keywords: 'interface extends'
      },
      {
        begin: /\$[(.]/ // relevance booster for a pattern common to JS libs: `$(something)` and `$.something`
      },
      {
        begin: '\\.' + hljs.IDENT_RE, relevance: 0 // hack: prevents detection of keywords after dots
      },
      DECORATOR,
      ARGS
    ]
  };
};

/***/ }),

/***/ "./node_modules/lit-element/lib/css-tag.js":
/*!*************************************************!*\
  !*** ./node_modules/lit-element/lib/css-tag.js ***!
  \*************************************************/
/*! exports provided: supportsAdoptingStyleSheets, CSSResult, unsafeCSS, css */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "supportsAdoptingStyleSheets", function() { return supportsAdoptingStyleSheets; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "CSSResult", function() { return CSSResult; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "unsafeCSS", function() { return unsafeCSS; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "css", function() { return css; });
/**
@license
Copyright (c) 2019 The Polymer Project Authors. All rights reserved.
This code may only be used under the BSD style license found at
http://polymer.github.io/LICENSE.txt The complete set of authors may be found at
http://polymer.github.io/AUTHORS.txt The complete set of contributors may be
found at http://polymer.github.io/CONTRIBUTORS.txt Code distributed by Google as
part of the polymer project is also subject to an additional IP rights grant
found at http://polymer.github.io/PATENTS.txt
*/
const supportsAdoptingStyleSheets = ('adoptedStyleSheets' in Document.prototype) &&
    ('replace' in CSSStyleSheet.prototype);
const constructionToken = Symbol();
class CSSResult {
    constructor(cssText, safeToken) {
        if (safeToken !== constructionToken) {
            throw new Error('CSSResult is not constructable. Use `unsafeCSS` or `css` instead.');
        }
        this.cssText = cssText;
    }
    // Note, this is a getter so that it's lazy. In practice, this means
    // stylesheets are not created until the first element instance is made.
    get styleSheet() {
        if (this._styleSheet === undefined) {
            // Note, if `adoptedStyleSheets` is supported then we assume CSSStyleSheet
            // is constructable.
            if (supportsAdoptingStyleSheets) {
                this._styleSheet = new CSSStyleSheet();
                this._styleSheet.replaceSync(this.cssText);
            }
            else {
                this._styleSheet = null;
            }
        }
        return this._styleSheet;
    }
    toString() {
        return this.cssText;
    }
}
/**
 * Wrap a value for interpolation in a css tagged template literal.
 *
 * This is unsafe because untrusted CSS text can be used to phone home
 * or exfiltrate data to an attacker controlled site. Take care to only use
 * this with trusted input.
 */
const unsafeCSS = (value) => {
    return new CSSResult(String(value), constructionToken);
};
const textFromCSSResult = (value) => {
    if (value instanceof CSSResult) {
        return value.cssText;
    }
    else {
        throw new Error(`Value passed to 'css' function must be a 'css' function result: ${value}. Use 'unsafeCSS' to pass non-literal values, but
            take care to ensure page security.`);
    }
};
/**
 * Template tag which which can be used with LitElement's `style` property to
 * set element styles. For security reasons, only literal string values may be
 * used. To incorporate non-literal values `unsafeCSS` may be used inside a
 * template string part.
 */
const css = (strings, ...values) => {
    const cssText = values.reduce((acc, v, idx) => acc + textFromCSSResult(v) + strings[idx + 1], strings[0]);
    return new CSSResult(cssText, constructionToken);
};
//# sourceMappingURL=css-tag.js.map

/***/ }),

/***/ "./node_modules/lit-element/lib/decorators.js":
/*!****************************************************!*\
  !*** ./node_modules/lit-element/lib/decorators.js ***!
  \****************************************************/
/*! exports provided: customElement, property, query, queryAll, eventOptions */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "customElement", function() { return customElement; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "property", function() { return property; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "query", function() { return query; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "queryAll", function() { return queryAll; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "eventOptions", function() { return eventOptions; });
/**
 * @license
 * Copyright (c) 2017 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */
const legacyCustomElement = (tagName, clazz) => {
    window.customElements.define(tagName, clazz);
    // Cast as any because TS doesn't recognize the return type as being a
    // subtype of the decorated class when clazz is typed as
    // `Constructor<HTMLElement>` for some reason.
    // `Constructor<HTMLElement>` is helpful to make sure the decorator is
    // applied to elements however.
    // tslint:disable-next-line:no-any
    return clazz;
};
const standardCustomElement = (tagName, descriptor) => {
    const { kind, elements } = descriptor;
    return {
        kind,
        elements,
        // This callback is called once the class is otherwise fully defined
        finisher(clazz) {
            window.customElements.define(tagName, clazz);
        }
    };
};
/**
 * Class decorator factory that defines the decorated class as a custom element.
 *
 * @param tagName the name of the custom element to define
 */
const customElement = (tagName) => (classOrDescriptor) => (typeof classOrDescriptor === 'function') ?
    legacyCustomElement(tagName, classOrDescriptor) :
    standardCustomElement(tagName, classOrDescriptor);
const standardProperty = (options, element) => {
    // When decorating an accessor, pass it through and add property metadata.
    // Note, the `hasOwnProperty` check in `createProperty` ensures we don't
    // stomp over the user's accessor.
    if (element.kind === 'method' && element.descriptor &&
        !('value' in element.descriptor)) {
        return Object.assign({}, element, { finisher(clazz) {
                clazz.createProperty(element.key, options);
            } });
    }
    else {
        // createProperty() takes care of defining the property, but we still
        // must return some kind of descriptor, so return a descriptor for an
        // unused prototype field. The finisher calls createProperty().
        return {
            kind: 'field',
            key: Symbol(),
            placement: 'own',
            descriptor: {},
            // When @babel/plugin-proposal-decorators implements initializers,
            // do this instead of the initializer below. See:
            // https://github.com/babel/babel/issues/9260 extras: [
            //   {
            //     kind: 'initializer',
            //     placement: 'own',
            //     initializer: descriptor.initializer,
            //   }
            // ],
            // tslint:disable-next-line:no-any decorator
            initializer() {
                if (typeof element.initializer === 'function') {
                    this[element.key] = element.initializer.call(this);
                }
            },
            finisher(clazz) {
                clazz.createProperty(element.key, options);
            }
        };
    }
};
const legacyProperty = (options, proto, name) => {
    proto.constructor
        .createProperty(name, options);
};
/**
 * A property decorator which creates a LitElement property which reflects a
 * corresponding attribute value. A `PropertyDeclaration` may optionally be
 * supplied to configure property features.
 *
 * @ExportDecoratedItems
 */
function property(options) {
    // tslint:disable-next-line:no-any decorator
    return (protoOrDescriptor, name) => (name !== undefined) ?
        legacyProperty(options, protoOrDescriptor, name) :
        standardProperty(options, protoOrDescriptor);
}
/**
 * A property decorator that converts a class property into a getter that
 * executes a querySelector on the element's renderRoot.
 */
const query = _query((target, selector) => target.querySelector(selector));
/**
 * A property decorator that converts a class property into a getter
 * that executes a querySelectorAll on the element's renderRoot.
 */
const queryAll = _query((target, selector) => target.querySelectorAll(selector));
const legacyQuery = (descriptor, proto, name) => {
    Object.defineProperty(proto, name, descriptor);
};
const standardQuery = (descriptor, element) => ({
    kind: 'method',
    placement: 'prototype',
    key: element.key,
    descriptor,
});
/**
 * Base-implementation of `@query` and `@queryAll` decorators.
 *
 * @param queryFn exectute a `selector` (ie, querySelector or querySelectorAll)
 * against `target`.
 * @suppress {visibility} The descriptor accesses an internal field on the
 * element.
 */
function _query(queryFn) {
    return (selector) => (protoOrDescriptor, 
    // tslint:disable-next-line:no-any decorator
    name) => {
        const descriptor = {
            get() {
                return queryFn(this.renderRoot, selector);
            },
            enumerable: true,
            configurable: true,
        };
        return (name !== undefined) ?
            legacyQuery(descriptor, protoOrDescriptor, name) :
            standardQuery(descriptor, protoOrDescriptor);
    };
}
const standardEventOptions = (options, element) => {
    return Object.assign({}, element, { finisher(clazz) {
            Object.assign(clazz.prototype[element.key], options);
        } });
};
const legacyEventOptions = 
// tslint:disable-next-line:no-any legacy decorator
(options, proto, name) => {
    Object.assign(proto[name], options);
};
/**
 * Adds event listener options to a method used as an event listener in a
 * lit-html template.
 *
 * @param options An object that specifis event listener options as accepted by
 * `EventTarget#addEventListener` and `EventTarget#removeEventListener`.
 *
 * Current browsers support the `capture`, `passive`, and `once` options. See:
 * https://developer.mozilla.org/en-US/docs/Web/API/EventTarget/addEventListener#Parameters
 *
 * @example
 *
 *     class MyElement {
 *
 *       clicked = false;
 *
 *       render() {
 *         return html`<div @click=${this._onClick}`><button></button></div>`;
 *       }
 *
 *       @eventOptions({capture: true})
 *       _onClick(e) {
 *         this.clicked = true;
 *       }
 *     }
 */
const eventOptions = (options) => 
// Return value typed as any to prevent TypeScript from complaining that
// standard decorator function signature does not match TypeScript decorator
// signature
// TODO(kschaaf): unclear why it was only failing on this decorator and not
// the others
((protoOrDescriptor, name) => (name !== undefined) ?
    legacyEventOptions(options, protoOrDescriptor, name) :
    standardEventOptions(options, protoOrDescriptor));
//# sourceMappingURL=decorators.js.map

/***/ }),

/***/ "./node_modules/lit-element/lib/updating-element.js":
/*!**********************************************************!*\
  !*** ./node_modules/lit-element/lib/updating-element.js ***!
  \**********************************************************/
/*! exports provided: defaultConverter, notEqual, UpdatingElement */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "defaultConverter", function() { return defaultConverter; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "notEqual", function() { return notEqual; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "UpdatingElement", function() { return UpdatingElement; });
/**
 * @license
 * Copyright (c) 2017 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */
/**
 * When using Closure Compiler, JSCompiler_renameProperty(property, object) is
 * replaced at compile time by the munged name for object[property]. We cannot
 * alias this function, so we have to use a small shim that has the same
 * behavior when not compiling.
 */
window.JSCompiler_renameProperty =
    (prop, _obj) => prop;
const defaultConverter = {
    toAttribute(value, type) {
        switch (type) {
            case Boolean:
                return value ? '' : null;
            case Object:
            case Array:
                // if the value is `null` or `undefined` pass this through
                // to allow removing/no change behavior.
                return value == null ? value : JSON.stringify(value);
        }
        return value;
    },
    fromAttribute(value, type) {
        switch (type) {
            case Boolean:
                return value !== null;
            case Number:
                return value === null ? null : Number(value);
            case Object:
            case Array:
                return JSON.parse(value);
        }
        return value;
    }
};
/**
 * Change function that returns true if `value` is different from `oldValue`.
 * This method is used as the default for a property's `hasChanged` function.
 */
const notEqual = (value, old) => {
    // This ensures (old==NaN, value==NaN) always returns false
    return old !== value && (old === old || value === value);
};
const defaultPropertyDeclaration = {
    attribute: true,
    type: String,
    converter: defaultConverter,
    reflect: false,
    hasChanged: notEqual
};
const microtaskPromise = Promise.resolve(true);
const STATE_HAS_UPDATED = 1;
const STATE_UPDATE_REQUESTED = 1 << 2;
const STATE_IS_REFLECTING_TO_ATTRIBUTE = 1 << 3;
const STATE_IS_REFLECTING_TO_PROPERTY = 1 << 4;
const STATE_HAS_CONNECTED = 1 << 5;
/**
 * Base element class which manages element properties and attributes. When
 * properties change, the `update` method is asynchronously called. This method
 * should be supplied by subclassers to render updates as desired.
 */
class UpdatingElement extends HTMLElement {
    constructor() {
        super();
        this._updateState = 0;
        this._instanceProperties = undefined;
        this._updatePromise = microtaskPromise;
        this._hasConnectedResolver = undefined;
        /**
         * Map with keys for any properties that have changed since the last
         * update cycle with previous values.
         */
        this._changedProperties = new Map();
        /**
         * Map with keys of properties that should be reflected when updated.
         */
        this._reflectingProperties = undefined;
        this.initialize();
    }
    /**
     * Returns a list of attributes corresponding to the registered properties.
     * @nocollapse
     */
    static get observedAttributes() {
        // note: piggy backing on this to ensure we're finalized.
        this.finalize();
        const attributes = [];
        // Use forEach so this works even if for/of loops are compiled to for loops
        // expecting arrays
        this._classProperties.forEach((v, p) => {
            const attr = this._attributeNameForProperty(p, v);
            if (attr !== undefined) {
                this._attributeToPropertyMap.set(attr, p);
                attributes.push(attr);
            }
        });
        return attributes;
    }
    /**
     * Ensures the private `_classProperties` property metadata is created.
     * In addition to `finalize` this is also called in `createProperty` to
     * ensure the `@property` decorator can add property metadata.
     */
    /** @nocollapse */
    static _ensureClassProperties() {
        // ensure private storage for property declarations.
        if (!this.hasOwnProperty(JSCompiler_renameProperty('_classProperties', this))) {
            this._classProperties = new Map();
            // NOTE: Workaround IE11 not supporting Map constructor argument.
            const superProperties = Object.getPrototypeOf(this)._classProperties;
            if (superProperties !== undefined) {
                superProperties.forEach((v, k) => this._classProperties.set(k, v));
            }
        }
    }
    /**
     * Creates a property accessor on the element prototype if one does not exist.
     * The property setter calls the property's `hasChanged` property option
     * or uses a strict identity check to determine whether or not to request
     * an update.
     * @nocollapse
     */
    static createProperty(name, options = defaultPropertyDeclaration) {
        // Note, since this can be called by the `@property` decorator which
        // is called before `finalize`, we ensure storage exists for property
        // metadata.
        this._ensureClassProperties();
        this._classProperties.set(name, options);
        // Do not generate an accessor if the prototype already has one, since
        // it would be lost otherwise and that would never be the user's intention;
        // Instead, we expect users to call `requestUpdate` themselves from
        // user-defined accessors. Note that if the super has an accessor we will
        // still overwrite it
        if (options.noAccessor || this.prototype.hasOwnProperty(name)) {
            return;
        }
        const key = typeof name === 'symbol' ? Symbol() : `__${name}`;
        Object.defineProperty(this.prototype, name, {
            // tslint:disable-next-line:no-any no symbol in index
            get() {
                // tslint:disable-next-line:no-any no symbol in index
                return this[key];
            },
            set(value) {
                // tslint:disable-next-line:no-any no symbol in index
                const oldValue = this[name];
                // tslint:disable-next-line:no-any no symbol in index
                this[key] = value;
                this.requestUpdate(name, oldValue);
            },
            configurable: true,
            enumerable: true
        });
    }
    /**
     * Creates property accessors for registered properties and ensures
     * any superclasses are also finalized.
     * @nocollapse
     */
    static finalize() {
        if (this.hasOwnProperty(JSCompiler_renameProperty('finalized', this)) &&
            this.finalized) {
            return;
        }
        // finalize any superclasses
        const superCtor = Object.getPrototypeOf(this);
        if (typeof superCtor.finalize === 'function') {
            superCtor.finalize();
        }
        this.finalized = true;
        this._ensureClassProperties();
        // initialize Map populated in observedAttributes
        this._attributeToPropertyMap = new Map();
        // make any properties
        // Note, only process "own" properties since this element will inherit
        // any properties defined on the superClass, and finalization ensures
        // the entire prototype chain is finalized.
        if (this.hasOwnProperty(JSCompiler_renameProperty('properties', this))) {
            const props = this.properties;
            // support symbols in properties (IE11 does not support this)
            const propKeys = [
                ...Object.getOwnPropertyNames(props),
                ...(typeof Object.getOwnPropertySymbols === 'function') ?
                    Object.getOwnPropertySymbols(props) :
                    []
            ];
            // This for/of is ok because propKeys is an array
            for (const p of propKeys) {
                // note, use of `any` is due to TypeSript lack of support for symbol in
                // index types
                // tslint:disable-next-line:no-any no symbol in index
                this.createProperty(p, props[p]);
            }
        }
    }
    /**
     * Returns the property name for the given attribute `name`.
     * @nocollapse
     */
    static _attributeNameForProperty(name, options) {
        const attribute = options.attribute;
        return attribute === false ?
            undefined :
            (typeof attribute === 'string' ?
                attribute :
                (typeof name === 'string' ? name.toLowerCase() : undefined));
    }
    /**
     * Returns true if a property should request an update.
     * Called when a property value is set and uses the `hasChanged`
     * option for the property if present or a strict identity check.
     * @nocollapse
     */
    static _valueHasChanged(value, old, hasChanged = notEqual) {
        return hasChanged(value, old);
    }
    /**
     * Returns the property value for the given attribute value.
     * Called via the `attributeChangedCallback` and uses the property's
     * `converter` or `converter.fromAttribute` property option.
     * @nocollapse
     */
    static _propertyValueFromAttribute(value, options) {
        const type = options.type;
        const converter = options.converter || defaultConverter;
        const fromAttribute = (typeof converter === 'function' ? converter : converter.fromAttribute);
        return fromAttribute ? fromAttribute(value, type) : value;
    }
    /**
     * Returns the attribute value for the given property value. If this
     * returns undefined, the property will *not* be reflected to an attribute.
     * If this returns null, the attribute will be removed, otherwise the
     * attribute will be set to the value.
     * This uses the property's `reflect` and `type.toAttribute` property options.
     * @nocollapse
     */
    static _propertyValueToAttribute(value, options) {
        if (options.reflect === undefined) {
            return;
        }
        const type = options.type;
        const converter = options.converter;
        const toAttribute = converter && converter.toAttribute ||
            defaultConverter.toAttribute;
        return toAttribute(value, type);
    }
    /**
     * Performs element initialization. By default captures any pre-set values for
     * registered properties.
     */
    initialize() {
        this._saveInstanceProperties();
    }
    /**
     * Fixes any properties set on the instance before upgrade time.
     * Otherwise these would shadow the accessor and break these properties.
     * The properties are stored in a Map which is played back after the
     * constructor runs. Note, on very old versions of Safari (<=9) or Chrome
     * (<=41), properties created for native platform properties like (`id` or
     * `name`) may not have default values set in the element constructor. On
     * these browsers native properties appear on instances and therefore their
     * default value will overwrite any element default (e.g. if the element sets
     * this.id = 'id' in the constructor, the 'id' will become '' since this is
     * the native platform default).
     */
    _saveInstanceProperties() {
        // Use forEach so this works even if for/of loops are compiled to for loops
        // expecting arrays
        this.constructor
            ._classProperties.forEach((_v, p) => {
            if (this.hasOwnProperty(p)) {
                const value = this[p];
                delete this[p];
                if (!this._instanceProperties) {
                    this._instanceProperties = new Map();
                }
                this._instanceProperties.set(p, value);
            }
        });
    }
    /**
     * Applies previously saved instance properties.
     */
    _applyInstanceProperties() {
        // Use forEach so this works even if for/of loops are compiled to for loops
        // expecting arrays
        // tslint:disable-next-line:no-any
        this._instanceProperties.forEach((v, p) => this[p] = v);
        this._instanceProperties = undefined;
    }
    connectedCallback() {
        this._updateState = this._updateState | STATE_HAS_CONNECTED;
        // Ensure connection triggers an update. Updates cannot complete before
        // connection and if one is pending connection the `_hasConnectionResolver`
        // will exist. If so, resolve it to complete the update, otherwise
        // requestUpdate.
        if (this._hasConnectedResolver) {
            this._hasConnectedResolver();
            this._hasConnectedResolver = undefined;
        }
        else {
            this.requestUpdate();
        }
    }
    /**
     * Allows for `super.disconnectedCallback()` in extensions while
     * reserving the possibility of making non-breaking feature additions
     * when disconnecting at some point in the future.
     */
    disconnectedCallback() {
    }
    /**
     * Synchronizes property values when attributes change.
     */
    attributeChangedCallback(name, old, value) {
        if (old !== value) {
            this._attributeToProperty(name, value);
        }
    }
    _propertyToAttribute(name, value, options = defaultPropertyDeclaration) {
        const ctor = this.constructor;
        const attr = ctor._attributeNameForProperty(name, options);
        if (attr !== undefined) {
            const attrValue = ctor._propertyValueToAttribute(value, options);
            // an undefined value does not change the attribute.
            if (attrValue === undefined) {
                return;
            }
            // Track if the property is being reflected to avoid
            // setting the property again via `attributeChangedCallback`. Note:
            // 1. this takes advantage of the fact that the callback is synchronous.
            // 2. will behave incorrectly if multiple attributes are in the reaction
            // stack at time of calling. However, since we process attributes
            // in `update` this should not be possible (or an extreme corner case
            // that we'd like to discover).
            // mark state reflecting
            this._updateState = this._updateState | STATE_IS_REFLECTING_TO_ATTRIBUTE;
            if (attrValue == null) {
                this.removeAttribute(attr);
            }
            else {
                this.setAttribute(attr, attrValue);
            }
            // mark state not reflecting
            this._updateState = this._updateState & ~STATE_IS_REFLECTING_TO_ATTRIBUTE;
        }
    }
    _attributeToProperty(name, value) {
        // Use tracking info to avoid deserializing attribute value if it was
        // just set from a property setter.
        if (this._updateState & STATE_IS_REFLECTING_TO_ATTRIBUTE) {
            return;
        }
        const ctor = this.constructor;
        const propName = ctor._attributeToPropertyMap.get(name);
        if (propName !== undefined) {
            const options = ctor._classProperties.get(propName) || defaultPropertyDeclaration;
            // mark state reflecting
            this._updateState = this._updateState | STATE_IS_REFLECTING_TO_PROPERTY;
            this[propName] =
                // tslint:disable-next-line:no-any
                ctor._propertyValueFromAttribute(value, options);
            // mark state not reflecting
            this._updateState = this._updateState & ~STATE_IS_REFLECTING_TO_PROPERTY;
        }
    }
    /**
     * Requests an update which is processed asynchronously. This should
     * be called when an element should update based on some state not triggered
     * by setting a property. In this case, pass no arguments. It should also be
     * called when manually implementing a property setter. In this case, pass the
     * property `name` and `oldValue` to ensure that any configured property
     * options are honored. Returns the `updateComplete` Promise which is resolved
     * when the update completes.
     *
     * @param name {PropertyKey} (optional) name of requesting property
     * @param oldValue {any} (optional) old value of requesting property
     * @returns {Promise} A Promise that is resolved when the update completes.
     */
    requestUpdate(name, oldValue) {
        let shouldRequestUpdate = true;
        // if we have a property key, perform property update steps.
        if (name !== undefined && !this._changedProperties.has(name)) {
            const ctor = this.constructor;
            const options = ctor._classProperties.get(name) || defaultPropertyDeclaration;
            if (ctor._valueHasChanged(this[name], oldValue, options.hasChanged)) {
                // track old value when changing.
                this._changedProperties.set(name, oldValue);
                // add to reflecting properties set
                if (options.reflect === true &&
                    !(this._updateState & STATE_IS_REFLECTING_TO_PROPERTY)) {
                    if (this._reflectingProperties === undefined) {
                        this._reflectingProperties = new Map();
                    }
                    this._reflectingProperties.set(name, options);
                }
                // abort the request if the property should not be considered changed.
            }
            else {
                shouldRequestUpdate = false;
            }
        }
        if (!this._hasRequestedUpdate && shouldRequestUpdate) {
            this._enqueueUpdate();
        }
        return this.updateComplete;
    }
    /**
     * Sets up the element to asynchronously update.
     */
    async _enqueueUpdate() {
        // Mark state updating...
        this._updateState = this._updateState | STATE_UPDATE_REQUESTED;
        let resolve;
        const previousUpdatePromise = this._updatePromise;
        this._updatePromise = new Promise((res) => resolve = res);
        // Ensure any previous update has resolved before updating.
        // This `await` also ensures that property changes are batched.
        await previousUpdatePromise;
        // Make sure the element has connected before updating.
        if (!this._hasConnected) {
            await new Promise((res) => this._hasConnectedResolver = res);
        }
        // Allow `performUpdate` to be asynchronous to enable scheduling of updates.
        const result = this.performUpdate();
        // Note, this is to avoid delaying an additional microtask unless we need
        // to.
        if (result != null &&
            typeof result.then === 'function') {
            await result;
        }
        resolve(!this._hasRequestedUpdate);
    }
    get _hasConnected() {
        return (this._updateState & STATE_HAS_CONNECTED);
    }
    get _hasRequestedUpdate() {
        return (this._updateState & STATE_UPDATE_REQUESTED);
    }
    get hasUpdated() {
        return (this._updateState & STATE_HAS_UPDATED);
    }
    /**
     * Performs an element update.
     *
     * You can override this method to change the timing of updates. For instance,
     * to schedule updates to occur just before the next frame:
     *
     * ```
     * protected async performUpdate(): Promise<unknown> {
     *   await new Promise((resolve) => requestAnimationFrame(() => resolve()));
     *   super.performUpdate();
     * }
     * ```
     */
    performUpdate() {
        // Mixin instance properties once, if they exist.
        if (this._instanceProperties) {
            this._applyInstanceProperties();
        }
        if (this.shouldUpdate(this._changedProperties)) {
            const changedProperties = this._changedProperties;
            this.update(changedProperties);
            this._markUpdated();
            if (!(this._updateState & STATE_HAS_UPDATED)) {
                this._updateState = this._updateState | STATE_HAS_UPDATED;
                this.firstUpdated(changedProperties);
            }
            this.updated(changedProperties);
        }
        else {
            this._markUpdated();
        }
    }
    _markUpdated() {
        this._changedProperties = new Map();
        this._updateState = this._updateState & ~STATE_UPDATE_REQUESTED;
    }
    /**
     * Returns a Promise that resolves when the element has completed updating.
     * The Promise value is a boolean that is `true` if the element completed the
     * update without triggering another update. The Promise result is `false` if
     * a property was set inside `updated()`. This getter can be implemented to
     * await additional state. For example, it is sometimes useful to await a
     * rendered element before fulfilling this Promise. To do this, first await
     * `super.updateComplete` then any subsequent state.
     *
     * @returns {Promise} The Promise returns a boolean that indicates if the
     * update resolved without triggering another update.
     */
    get updateComplete() {
        return this._updatePromise;
    }
    /**
     * Controls whether or not `update` should be called when the element requests
     * an update. By default, this method always returns `true`, but this can be
     * customized to control when to update.
     *
     * * @param _changedProperties Map of changed properties with old values
     */
    shouldUpdate(_changedProperties) {
        return true;
    }
    /**
     * Updates the element. This method reflects property values to attributes.
     * It can be overridden to render and keep updated element DOM.
     * Setting properties inside this method will *not* trigger
     * another update.
     *
     * * @param _changedProperties Map of changed properties with old values
     */
    update(_changedProperties) {
        if (this._reflectingProperties !== undefined &&
            this._reflectingProperties.size > 0) {
            // Use forEach so this works even if for/of loops are compiled to for
            // loops expecting arrays
            this._reflectingProperties.forEach((v, k) => this._propertyToAttribute(k, this[k], v));
            this._reflectingProperties = undefined;
        }
    }
    /**
     * Invoked whenever the element is updated. Implement to perform
     * post-updating tasks via DOM APIs, for example, focusing an element.
     *
     * Setting properties inside this method will trigger the element to update
     * again after this update cycle completes.
     *
     * * @param _changedProperties Map of changed properties with old values
     */
    updated(_changedProperties) {
    }
    /**
     * Invoked when the element is first updated. Implement to perform one time
     * work on the element after update.
     *
     * Setting properties inside this method will trigger the element to update
     * again after this update cycle completes.
     *
     * * @param _changedProperties Map of changed properties with old values
     */
    firstUpdated(_changedProperties) {
    }
}
/**
 * Marks class as having finished creating properties.
 */
UpdatingElement.finalized = true;
//# sourceMappingURL=updating-element.js.map

/***/ }),

/***/ "./node_modules/lit-element/lit-element.js":
/*!*************************************************!*\
  !*** ./node_modules/lit-element/lit-element.js ***!
  \*************************************************/
/*! exports provided: html, svg, TemplateResult, SVGTemplateResult, LitElement, defaultConverter, notEqual, UpdatingElement, customElement, property, query, queryAll, eventOptions, supportsAdoptingStyleSheets, CSSResult, unsafeCSS, css */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LitElement", function() { return LitElement; });
/* harmony import */ var lit_html__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! lit-html */ "./node_modules/lit-html/lit-html.js");
/* harmony import */ var lit_html_lib_shady_render__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! lit-html/lib/shady-render */ "./node_modules/lit-html/lib/shady-render.js");
/* harmony import */ var _lib_updating_element_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./lib/updating-element.js */ "./node_modules/lit-element/lib/updating-element.js");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "defaultConverter", function() { return _lib_updating_element_js__WEBPACK_IMPORTED_MODULE_2__["defaultConverter"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "notEqual", function() { return _lib_updating_element_js__WEBPACK_IMPORTED_MODULE_2__["notEqual"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "UpdatingElement", function() { return _lib_updating_element_js__WEBPACK_IMPORTED_MODULE_2__["UpdatingElement"]; });

/* harmony import */ var _lib_decorators_js__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./lib/decorators.js */ "./node_modules/lit-element/lib/decorators.js");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "customElement", function() { return _lib_decorators_js__WEBPACK_IMPORTED_MODULE_3__["customElement"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "property", function() { return _lib_decorators_js__WEBPACK_IMPORTED_MODULE_3__["property"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "query", function() { return _lib_decorators_js__WEBPACK_IMPORTED_MODULE_3__["query"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "queryAll", function() { return _lib_decorators_js__WEBPACK_IMPORTED_MODULE_3__["queryAll"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "eventOptions", function() { return _lib_decorators_js__WEBPACK_IMPORTED_MODULE_3__["eventOptions"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "html", function() { return lit_html__WEBPACK_IMPORTED_MODULE_0__["html"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "svg", function() { return lit_html__WEBPACK_IMPORTED_MODULE_0__["svg"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "TemplateResult", function() { return lit_html__WEBPACK_IMPORTED_MODULE_0__["TemplateResult"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "SVGTemplateResult", function() { return lit_html__WEBPACK_IMPORTED_MODULE_0__["SVGTemplateResult"]; });

/* harmony import */ var _lib_css_tag_js__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./lib/css-tag.js */ "./node_modules/lit-element/lib/css-tag.js");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "supportsAdoptingStyleSheets", function() { return _lib_css_tag_js__WEBPACK_IMPORTED_MODULE_4__["supportsAdoptingStyleSheets"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "CSSResult", function() { return _lib_css_tag_js__WEBPACK_IMPORTED_MODULE_4__["CSSResult"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "unsafeCSS", function() { return _lib_css_tag_js__WEBPACK_IMPORTED_MODULE_4__["unsafeCSS"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "css", function() { return _lib_css_tag_js__WEBPACK_IMPORTED_MODULE_4__["css"]; });

/**
 * @license
 * Copyright (c) 2017 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */








// IMPORTANT: do not change the property name or the assignment expression.
// This line will be used in regexes to search for LitElement usage.
// TODO(justinfagnani): inject version number at build time
(window['litElementVersions'] || (window['litElementVersions'] = []))
    .push('2.0.1');
/**
 * Minimal implementation of Array.prototype.flat
 * @param arr the array to flatten
 * @param result the accumlated result
 */
function arrayFlat(styles, result = []) {
    for (let i = 0, length = styles.length; i < length; i++) {
        const value = styles[i];
        if (Array.isArray(value)) {
            arrayFlat(value, result);
        }
        else {
            result.push(value);
        }
    }
    return result;
}
/** Deeply flattens styles array. Uses native flat if available. */
const flattenStyles = (styles) => styles.flat ? styles.flat(Infinity) : arrayFlat(styles);
class LitElement extends _lib_updating_element_js__WEBPACK_IMPORTED_MODULE_2__["UpdatingElement"] {
    /** @nocollapse */
    static finalize() {
        super.finalize();
        // Prepare styling that is stamped at first render time. Styling
        // is built from user provided `styles` or is inherited from the superclass.
        this._styles =
            this.hasOwnProperty(JSCompiler_renameProperty('styles', this)) ?
                this._getUniqueStyles() :
                this._styles || [];
    }
    /** @nocollapse */
    static _getUniqueStyles() {
        // Take care not to call `this.styles` multiple times since this generates
        // new CSSResults each time.
        // TODO(sorvell): Since we do not cache CSSResults by input, any
        // shared styles will generate new stylesheet objects, which is wasteful.
        // This should be addressed when a browser ships constructable
        // stylesheets.
        const userStyles = this.styles;
        const styles = [];
        if (Array.isArray(userStyles)) {
            const flatStyles = flattenStyles(userStyles);
            // As a performance optimization to avoid duplicated styling that can
            // occur especially when composing via subclassing, de-duplicate styles
            // preserving the last item in the list. The last item is kept to
            // try to preserve cascade order with the assumption that it's most
            // important that last added styles override previous styles.
            const styleSet = flatStyles.reduceRight((set, s) => {
                set.add(s);
                // on IE set.add does not return the set.
                return set;
            }, new Set());
            // Array.from does not work on Set in IE
            styleSet.forEach((v) => styles.unshift(v));
        }
        else if (userStyles) {
            styles.push(userStyles);
        }
        return styles;
    }
    /**
     * Performs element initialization. By default this calls `createRenderRoot`
     * to create the element `renderRoot` node and captures any pre-set values for
     * registered properties.
     */
    initialize() {
        super.initialize();
        this.renderRoot = this.createRenderRoot();
        // Note, if renderRoot is not a shadowRoot, styles would/could apply to the
        // element's getRootNode(). While this could be done, we're choosing not to
        // support this now since it would require different logic around de-duping.
        if (window.ShadowRoot && this.renderRoot instanceof window.ShadowRoot) {
            this.adoptStyles();
        }
    }
    /**
     * Returns the node into which the element should render and by default
     * creates and returns an open shadowRoot. Implement to customize where the
     * element's DOM is rendered. For example, to render into the element's
     * childNodes, return `this`.
     * @returns {Element|DocumentFragment} Returns a node into which to render.
     */
    createRenderRoot() {
        return this.attachShadow({ mode: 'open' });
    }
    /**
     * Applies styling to the element shadowRoot using the `static get styles`
     * property. Styling will apply using `shadowRoot.adoptedStyleSheets` where
     * available and will fallback otherwise. When Shadow DOM is polyfilled,
     * ShadyCSS scopes styles and adds them to the document. When Shadow DOM
     * is available but `adoptedStyleSheets` is not, styles are appended to the
     * end of the `shadowRoot` to [mimic spec
     * behavior](https://wicg.github.io/construct-stylesheets/#using-constructed-stylesheets).
     */
    adoptStyles() {
        const styles = this.constructor._styles;
        if (styles.length === 0) {
            return;
        }
        // There are three separate cases here based on Shadow DOM support.
        // (1) shadowRoot polyfilled: use ShadyCSS
        // (2) shadowRoot.adoptedStyleSheets available: use it.
        // (3) shadowRoot.adoptedStyleSheets polyfilled: append styles after
        // rendering
        if (window.ShadyCSS !== undefined && !window.ShadyCSS.nativeShadow) {
            window.ShadyCSS.ScopingShim.prepareAdoptedCssText(styles.map((s) => s.cssText), this.localName);
        }
        else if (_lib_css_tag_js__WEBPACK_IMPORTED_MODULE_4__["supportsAdoptingStyleSheets"]) {
            this.renderRoot.adoptedStyleSheets =
                styles.map((s) => s.styleSheet);
        }
        else {
            // This must be done after rendering so the actual style insertion is done
            // in `update`.
            this._needsShimAdoptedStyleSheets = true;
        }
    }
    connectedCallback() {
        super.connectedCallback();
        // Note, first update/render handles styleElement so we only call this if
        // connected after first update.
        if (this.hasUpdated && window.ShadyCSS !== undefined) {
            window.ShadyCSS.styleElement(this);
        }
    }
    /**
     * Updates the element. This method reflects property values to attributes
     * and calls `render` to render DOM via lit-html. Setting properties inside
     * this method will *not* trigger another update.
     * * @param _changedProperties Map of changed properties with old values
     */
    update(changedProperties) {
        super.update(changedProperties);
        const templateResult = this.render();
        if (templateResult instanceof lit_html__WEBPACK_IMPORTED_MODULE_0__["TemplateResult"]) {
            this.constructor
                .render(templateResult, this.renderRoot, { scopeName: this.localName, eventContext: this });
        }
        // When native Shadow DOM is used but adoptedStyles are not supported,
        // insert styling after rendering to ensure adoptedStyles have highest
        // priority.
        if (this._needsShimAdoptedStyleSheets) {
            this._needsShimAdoptedStyleSheets = false;
            this.constructor._styles.forEach((s) => {
                const style = document.createElement('style');
                style.textContent = s.cssText;
                this.renderRoot.appendChild(style);
            });
        }
    }
    /**
     * Invoked on each update to perform rendering tasks. This method must return
     * a lit-html TemplateResult. Setting properties inside this method will *not*
     * trigger the element to update.
     */
    render() {
    }
}
/**
 * Ensure this class is marked as `finalized` as an optimization ensuring
 * it will not needlessly try to `finalize`.
 */
LitElement.finalized = true;
/**
 * Render method used to render the lit-html TemplateResult to the element's
 * DOM.
 * @param {TemplateResult} Template to render.
 * @param {Element|DocumentFragment} Node into which to render.
 * @param {String} Element name.
 * @nocollapse
 */
LitElement.render = lit_html_lib_shady_render__WEBPACK_IMPORTED_MODULE_1__["render"];
//# sourceMappingURL=lit-element.js.map

/***/ }),

/***/ "./node_modules/lit-html/lib/default-template-processor.js":
/*!*****************************************************************!*\
  !*** ./node_modules/lit-html/lib/default-template-processor.js ***!
  \*****************************************************************/
/*! exports provided: DefaultTemplateProcessor, defaultTemplateProcessor */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DefaultTemplateProcessor", function() { return DefaultTemplateProcessor; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "defaultTemplateProcessor", function() { return defaultTemplateProcessor; });
/* harmony import */ var _parts_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./parts.js */ "./node_modules/lit-html/lib/parts.js");
/**
 * @license
 * Copyright (c) 2017 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */

/**
 * Creates Parts when a template is instantiated.
 */
class DefaultTemplateProcessor {
    /**
     * Create parts for an attribute-position binding, given the event, attribute
     * name, and string literals.
     *
     * @param element The element containing the binding
     * @param name  The attribute name
     * @param strings The string literals. There are always at least two strings,
     *   event for fully-controlled bindings with a single expression.
     */
    handleAttributeExpressions(element, name, strings, options) {
        const prefix = name[0];
        if (prefix === '.') {
            const comitter = new _parts_js__WEBPACK_IMPORTED_MODULE_0__["PropertyCommitter"](element, name.slice(1), strings);
            return comitter.parts;
        }
        if (prefix === '@') {
            return [new _parts_js__WEBPACK_IMPORTED_MODULE_0__["EventPart"](element, name.slice(1), options.eventContext)];
        }
        if (prefix === '?') {
            return [new _parts_js__WEBPACK_IMPORTED_MODULE_0__["BooleanAttributePart"](element, name.slice(1), strings)];
        }
        const comitter = new _parts_js__WEBPACK_IMPORTED_MODULE_0__["AttributeCommitter"](element, name, strings);
        return comitter.parts;
    }
    /**
     * Create parts for a text-position binding.
     * @param templateFactory
     */
    handleTextExpression(options) {
        return new _parts_js__WEBPACK_IMPORTED_MODULE_0__["NodePart"](options);
    }
}
const defaultTemplateProcessor = new DefaultTemplateProcessor();
//# sourceMappingURL=default-template-processor.js.map

/***/ }),

/***/ "./node_modules/lit-html/lib/directive.js":
/*!************************************************!*\
  !*** ./node_modules/lit-html/lib/directive.js ***!
  \************************************************/
/*! exports provided: directive, isDirective */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "directive", function() { return directive; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "isDirective", function() { return isDirective; });
/**
 * @license
 * Copyright (c) 2017 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */
const directives = new WeakMap();
/**
 * Brands a function as a directive so that lit-html will call the function
 * during template rendering, rather than passing as a value.
 *
 * @param f The directive factory function. Must be a function that returns a
 * function of the signature `(part: Part) => void`. The returned function will
 * be called with the part object
 *
 * @example
 *
 * ```
 * import {directive, html} from 'lit-html';
 *
 * const immutable = directive((v) => (part) => {
 *   if (part.value !== v) {
 *     part.setValue(v)
 *   }
 * });
 * ```
 */
// tslint:disable-next-line:no-any
const directive = (f) => ((...args) => {
    const d = f(...args);
    directives.set(d, true);
    return d;
});
const isDirective = (o) => {
    return typeof o === 'function' && directives.has(o);
};
//# sourceMappingURL=directive.js.map

/***/ }),

/***/ "./node_modules/lit-html/lib/dom.js":
/*!******************************************!*\
  !*** ./node_modules/lit-html/lib/dom.js ***!
  \******************************************/
/*! exports provided: isCEPolyfill, reparentNodes, removeNodes */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "isCEPolyfill", function() { return isCEPolyfill; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "reparentNodes", function() { return reparentNodes; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "removeNodes", function() { return removeNodes; });
/**
 * @license
 * Copyright (c) 2017 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */
/**
 * True if the custom elements polyfill is in use.
 */
const isCEPolyfill = window.customElements !== undefined &&
    window.customElements.polyfillWrapFlushCallback !==
        undefined;
/**
 * Reparents nodes, starting from `startNode` (inclusive) to `endNode`
 * (exclusive), into another container (could be the same container), before
 * `beforeNode`. If `beforeNode` is null, it appends the nodes to the
 * container.
 */
const reparentNodes = (container, start, end = null, before = null) => {
    let node = start;
    while (node !== end) {
        const n = node.nextSibling;
        container.insertBefore(node, before);
        node = n;
    }
};
/**
 * Removes nodes, starting from `startNode` (inclusive) to `endNode`
 * (exclusive), from `container`.
 */
const removeNodes = (container, startNode, endNode = null) => {
    let node = startNode;
    while (node !== endNode) {
        const n = node.nextSibling;
        container.removeChild(node);
        node = n;
    }
};
//# sourceMappingURL=dom.js.map

/***/ }),

/***/ "./node_modules/lit-html/lib/modify-template.js":
/*!******************************************************!*\
  !*** ./node_modules/lit-html/lib/modify-template.js ***!
  \******************************************************/
/*! exports provided: removeNodesFromTemplate, insertNodeIntoTemplate */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "removeNodesFromTemplate", function() { return removeNodesFromTemplate; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "insertNodeIntoTemplate", function() { return insertNodeIntoTemplate; });
/* harmony import */ var _template_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./template.js */ "./node_modules/lit-html/lib/template.js");
/**
 * @license
 * Copyright (c) 2017 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */
/**
 * @module shady-render
 */

const walkerNodeFilter = 133 /* NodeFilter.SHOW_{ELEMENT|COMMENT|TEXT} */;
/**
 * Removes the list of nodes from a Template safely. In addition to removing
 * nodes from the Template, the Template part indices are updated to match
 * the mutated Template DOM.
 *
 * As the template is walked the removal state is tracked and
 * part indices are adjusted as needed.
 *
 * div
 *   div#1 (remove) <-- start removing (removing node is div#1)
 *     div
 *       div#2 (remove)  <-- continue removing (removing node is still div#1)
 *         div
 * div <-- stop removing since previous sibling is the removing node (div#1,
 * removed 4 nodes)
 */
function removeNodesFromTemplate(template, nodesToRemove) {
    const { element: { content }, parts } = template;
    const walker = document.createTreeWalker(content, walkerNodeFilter, null, false);
    let partIndex = nextActiveIndexInTemplateParts(parts);
    let part = parts[partIndex];
    let nodeIndex = -1;
    let removeCount = 0;
    const nodesToRemoveInTemplate = [];
    let currentRemovingNode = null;
    while (walker.nextNode()) {
        nodeIndex++;
        const node = walker.currentNode;
        // End removal if stepped past the removing node
        if (node.previousSibling === currentRemovingNode) {
            currentRemovingNode = null;
        }
        // A node to remove was found in the template
        if (nodesToRemove.has(node)) {
            nodesToRemoveInTemplate.push(node);
            // Track node we're removing
            if (currentRemovingNode === null) {
                currentRemovingNode = node;
            }
        }
        // When removing, increment count by which to adjust subsequent part indices
        if (currentRemovingNode !== null) {
            removeCount++;
        }
        while (part !== undefined && part.index === nodeIndex) {
            // If part is in a removed node deactivate it by setting index to -1 or
            // adjust the index as needed.
            part.index = currentRemovingNode !== null ? -1 : part.index - removeCount;
            // go to the next active part.
            partIndex = nextActiveIndexInTemplateParts(parts, partIndex);
            part = parts[partIndex];
        }
    }
    nodesToRemoveInTemplate.forEach((n) => n.parentNode.removeChild(n));
}
const countNodes = (node) => {
    let count = (node.nodeType === 11 /* Node.DOCUMENT_FRAGMENT_NODE */) ? 0 : 1;
    const walker = document.createTreeWalker(node, walkerNodeFilter, null, false);
    while (walker.nextNode()) {
        count++;
    }
    return count;
};
const nextActiveIndexInTemplateParts = (parts, startIndex = -1) => {
    for (let i = startIndex + 1; i < parts.length; i++) {
        const part = parts[i];
        if (Object(_template_js__WEBPACK_IMPORTED_MODULE_0__["isTemplatePartActive"])(part)) {
            return i;
        }
    }
    return -1;
};
/**
 * Inserts the given node into the Template, optionally before the given
 * refNode. In addition to inserting the node into the Template, the Template
 * part indices are updated to match the mutated Template DOM.
 */
function insertNodeIntoTemplate(template, node, refNode = null) {
    const { element: { content }, parts } = template;
    // If there's no refNode, then put node at end of template.
    // No part indices need to be shifted in this case.
    if (refNode === null || refNode === undefined) {
        content.appendChild(node);
        return;
    }
    const walker = document.createTreeWalker(content, walkerNodeFilter, null, false);
    let partIndex = nextActiveIndexInTemplateParts(parts);
    let insertCount = 0;
    let walkerIndex = -1;
    while (walker.nextNode()) {
        walkerIndex++;
        const walkerNode = walker.currentNode;
        if (walkerNode === refNode) {
            insertCount = countNodes(node);
            refNode.parentNode.insertBefore(node, refNode);
        }
        while (partIndex !== -1 && parts[partIndex].index === walkerIndex) {
            // If we've inserted the node, simply adjust all subsequent parts
            if (insertCount > 0) {
                while (partIndex !== -1) {
                    parts[partIndex].index += insertCount;
                    partIndex = nextActiveIndexInTemplateParts(parts, partIndex);
                }
                return;
            }
            partIndex = nextActiveIndexInTemplateParts(parts, partIndex);
        }
    }
}
//# sourceMappingURL=modify-template.js.map

/***/ }),

/***/ "./node_modules/lit-html/lib/part.js":
/*!*******************************************!*\
  !*** ./node_modules/lit-html/lib/part.js ***!
  \*******************************************/
/*! exports provided: noChange, nothing */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "noChange", function() { return noChange; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "nothing", function() { return nothing; });
/**
 * @license
 * Copyright (c) 2018 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */
/**
 * A sentinel value that signals that a value was handled by a directive and
 * should not be written to the DOM.
 */
const noChange = {};
/**
 * A sentinel value that signals a NodePart to fully clear its content.
 */
const nothing = {};
//# sourceMappingURL=part.js.map

/***/ }),

/***/ "./node_modules/lit-html/lib/parts.js":
/*!********************************************!*\
  !*** ./node_modules/lit-html/lib/parts.js ***!
  \********************************************/
/*! exports provided: isPrimitive, AttributeCommitter, AttributePart, NodePart, BooleanAttributePart, PropertyCommitter, PropertyPart, EventPart */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "isPrimitive", function() { return isPrimitive; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AttributeCommitter", function() { return AttributeCommitter; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AttributePart", function() { return AttributePart; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "NodePart", function() { return NodePart; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "BooleanAttributePart", function() { return BooleanAttributePart; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PropertyCommitter", function() { return PropertyCommitter; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "PropertyPart", function() { return PropertyPart; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "EventPart", function() { return EventPart; });
/* harmony import */ var _directive_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./directive.js */ "./node_modules/lit-html/lib/directive.js");
/* harmony import */ var _dom_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./dom.js */ "./node_modules/lit-html/lib/dom.js");
/* harmony import */ var _part_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./part.js */ "./node_modules/lit-html/lib/part.js");
/* harmony import */ var _template_instance_js__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./template-instance.js */ "./node_modules/lit-html/lib/template-instance.js");
/* harmony import */ var _template_result_js__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./template-result.js */ "./node_modules/lit-html/lib/template-result.js");
/* harmony import */ var _template_js__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./template.js */ "./node_modules/lit-html/lib/template.js");
/**
 * @license
 * Copyright (c) 2017 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */
/**
 * @module lit-html
 */






const isPrimitive = (value) => {
    return (value === null ||
        !(typeof value === 'object' || typeof value === 'function'));
};
/**
 * Sets attribute values for AttributeParts, so that the value is only set once
 * even if there are multiple parts for an attribute.
 */
class AttributeCommitter {
    constructor(element, name, strings) {
        this.dirty = true;
        this.element = element;
        this.name = name;
        this.strings = strings;
        this.parts = [];
        for (let i = 0; i < strings.length - 1; i++) {
            this.parts[i] = this._createPart();
        }
    }
    /**
     * Creates a single part. Override this to create a differnt type of part.
     */
    _createPart() {
        return new AttributePart(this);
    }
    _getValue() {
        const strings = this.strings;
        const l = strings.length - 1;
        let text = '';
        for (let i = 0; i < l; i++) {
            text += strings[i];
            const part = this.parts[i];
            if (part !== undefined) {
                const v = part.value;
                if (v != null &&
                    (Array.isArray(v) ||
                        // tslint:disable-next-line:no-any
                        typeof v !== 'string' && v[Symbol.iterator])) {
                    for (const t of v) {
                        text += typeof t === 'string' ? t : String(t);
                    }
                }
                else {
                    text += typeof v === 'string' ? v : String(v);
                }
            }
        }
        text += strings[l];
        return text;
    }
    commit() {
        if (this.dirty) {
            this.dirty = false;
            this.element.setAttribute(this.name, this._getValue());
        }
    }
}
class AttributePart {
    constructor(comitter) {
        this.value = undefined;
        this.committer = comitter;
    }
    setValue(value) {
        if (value !== _part_js__WEBPACK_IMPORTED_MODULE_2__["noChange"] && (!isPrimitive(value) || value !== this.value)) {
            this.value = value;
            // If the value is a not a directive, dirty the committer so that it'll
            // call setAttribute. If the value is a directive, it'll dirty the
            // committer if it calls setValue().
            if (!Object(_directive_js__WEBPACK_IMPORTED_MODULE_0__["isDirective"])(value)) {
                this.committer.dirty = true;
            }
        }
    }
    commit() {
        while (Object(_directive_js__WEBPACK_IMPORTED_MODULE_0__["isDirective"])(this.value)) {
            const directive = this.value;
            this.value = _part_js__WEBPACK_IMPORTED_MODULE_2__["noChange"];
            directive(this);
        }
        if (this.value === _part_js__WEBPACK_IMPORTED_MODULE_2__["noChange"]) {
            return;
        }
        this.committer.commit();
    }
}
class NodePart {
    constructor(options) {
        this.value = undefined;
        this._pendingValue = undefined;
        this.options = options;
    }
    /**
     * Inserts this part into a container.
     *
     * This part must be empty, as its contents are not automatically moved.
     */
    appendInto(container) {
        this.startNode = container.appendChild(Object(_template_js__WEBPACK_IMPORTED_MODULE_5__["createMarker"])());
        this.endNode = container.appendChild(Object(_template_js__WEBPACK_IMPORTED_MODULE_5__["createMarker"])());
    }
    /**
     * Inserts this part between `ref` and `ref`'s next sibling. Both `ref` and
     * its next sibling must be static, unchanging nodes such as those that appear
     * in a literal section of a template.
     *
     * This part must be empty, as its contents are not automatically moved.
     */
    insertAfterNode(ref) {
        this.startNode = ref;
        this.endNode = ref.nextSibling;
    }
    /**
     * Appends this part into a parent part.
     *
     * This part must be empty, as its contents are not automatically moved.
     */
    appendIntoPart(part) {
        part._insert(this.startNode = Object(_template_js__WEBPACK_IMPORTED_MODULE_5__["createMarker"])());
        part._insert(this.endNode = Object(_template_js__WEBPACK_IMPORTED_MODULE_5__["createMarker"])());
    }
    /**
     * Appends this part after `ref`
     *
     * This part must be empty, as its contents are not automatically moved.
     */
    insertAfterPart(ref) {
        ref._insert(this.startNode = Object(_template_js__WEBPACK_IMPORTED_MODULE_5__["createMarker"])());
        this.endNode = ref.endNode;
        ref.endNode = this.startNode;
    }
    setValue(value) {
        this._pendingValue = value;
    }
    commit() {
        while (Object(_directive_js__WEBPACK_IMPORTED_MODULE_0__["isDirective"])(this._pendingValue)) {
            const directive = this._pendingValue;
            this._pendingValue = _part_js__WEBPACK_IMPORTED_MODULE_2__["noChange"];
            directive(this);
        }
        const value = this._pendingValue;
        if (value === _part_js__WEBPACK_IMPORTED_MODULE_2__["noChange"]) {
            return;
        }
        if (isPrimitive(value)) {
            if (value !== this.value) {
                this._commitText(value);
            }
        }
        else if (value instanceof _template_result_js__WEBPACK_IMPORTED_MODULE_4__["TemplateResult"]) {
            this._commitTemplateResult(value);
        }
        else if (value instanceof Node) {
            this._commitNode(value);
        }
        else if (Array.isArray(value) ||
            // tslint:disable-next-line:no-any
            value[Symbol.iterator]) {
            this._commitIterable(value);
        }
        else if (value === _part_js__WEBPACK_IMPORTED_MODULE_2__["nothing"]) {
            this.value = _part_js__WEBPACK_IMPORTED_MODULE_2__["nothing"];
            this.clear();
        }
        else {
            // Fallback, will render the string representation
            this._commitText(value);
        }
    }
    _insert(node) {
        this.endNode.parentNode.insertBefore(node, this.endNode);
    }
    _commitNode(value) {
        if (this.value === value) {
            return;
        }
        this.clear();
        this._insert(value);
        this.value = value;
    }
    _commitText(value) {
        const node = this.startNode.nextSibling;
        value = value == null ? '' : value;
        if (node === this.endNode.previousSibling &&
            node.nodeType === 3 /* Node.TEXT_NODE */) {
            // If we only have a single text node between the markers, we can just
            // set its value, rather than replacing it.
            // TODO(justinfagnani): Can we just check if this.value is primitive?
            node.data = value;
        }
        else {
            this._commitNode(document.createTextNode(typeof value === 'string' ? value : String(value)));
        }
        this.value = value;
    }
    _commitTemplateResult(value) {
        const template = this.options.templateFactory(value);
        if (this.value instanceof _template_instance_js__WEBPACK_IMPORTED_MODULE_3__["TemplateInstance"] &&
            this.value.template === template) {
            this.value.update(value.values);
        }
        else {
            // Make sure we propagate the template processor from the TemplateResult
            // so that we use its syntax extension, etc. The template factory comes
            // from the render function options so that it can control template
            // caching and preprocessing.
            const instance = new _template_instance_js__WEBPACK_IMPORTED_MODULE_3__["TemplateInstance"](template, value.processor, this.options);
            const fragment = instance._clone();
            instance.update(value.values);
            this._commitNode(fragment);
            this.value = instance;
        }
    }
    _commitIterable(value) {
        // For an Iterable, we create a new InstancePart per item, then set its
        // value to the item. This is a little bit of overhead for every item in
        // an Iterable, but it lets us recurse easily and efficiently update Arrays
        // of TemplateResults that will be commonly returned from expressions like:
        // array.map((i) => html`${i}`), by reusing existing TemplateInstances.
        // If _value is an array, then the previous render was of an
        // iterable and _value will contain the NodeParts from the previous
        // render. If _value is not an array, clear this part and make a new
        // array for NodeParts.
        if (!Array.isArray(this.value)) {
            this.value = [];
            this.clear();
        }
        // Lets us keep track of how many items we stamped so we can clear leftover
        // items from a previous render
        const itemParts = this.value;
        let partIndex = 0;
        let itemPart;
        for (const item of value) {
            // Try to reuse an existing part
            itemPart = itemParts[partIndex];
            // If no existing part, create a new one
            if (itemPart === undefined) {
                itemPart = new NodePart(this.options);
                itemParts.push(itemPart);
                if (partIndex === 0) {
                    itemPart.appendIntoPart(this);
                }
                else {
                    itemPart.insertAfterPart(itemParts[partIndex - 1]);
                }
            }
            itemPart.setValue(item);
            itemPart.commit();
            partIndex++;
        }
        if (partIndex < itemParts.length) {
            // Truncate the parts array so _value reflects the current state
            itemParts.length = partIndex;
            this.clear(itemPart && itemPart.endNode);
        }
    }
    clear(startNode = this.startNode) {
        Object(_dom_js__WEBPACK_IMPORTED_MODULE_1__["removeNodes"])(this.startNode.parentNode, startNode.nextSibling, this.endNode);
    }
}
/**
 * Implements a boolean attribute, roughly as defined in the HTML
 * specification.
 *
 * If the value is truthy, then the attribute is present with a value of
 * ''. If the value is falsey, the attribute is removed.
 */
class BooleanAttributePart {
    constructor(element, name, strings) {
        this.value = undefined;
        this._pendingValue = undefined;
        if (strings.length !== 2 || strings[0] !== '' || strings[1] !== '') {
            throw new Error('Boolean attributes can only contain a single expression');
        }
        this.element = element;
        this.name = name;
        this.strings = strings;
    }
    setValue(value) {
        this._pendingValue = value;
    }
    commit() {
        while (Object(_directive_js__WEBPACK_IMPORTED_MODULE_0__["isDirective"])(this._pendingValue)) {
            const directive = this._pendingValue;
            this._pendingValue = _part_js__WEBPACK_IMPORTED_MODULE_2__["noChange"];
            directive(this);
        }
        if (this._pendingValue === _part_js__WEBPACK_IMPORTED_MODULE_2__["noChange"]) {
            return;
        }
        const value = !!this._pendingValue;
        if (this.value !== value) {
            if (value) {
                this.element.setAttribute(this.name, '');
            }
            else {
                this.element.removeAttribute(this.name);
            }
        }
        this.value = value;
        this._pendingValue = _part_js__WEBPACK_IMPORTED_MODULE_2__["noChange"];
    }
}
/**
 * Sets attribute values for PropertyParts, so that the value is only set once
 * even if there are multiple parts for a property.
 *
 * If an expression controls the whole property value, then the value is simply
 * assigned to the property under control. If there are string literals or
 * multiple expressions, then the strings are expressions are interpolated into
 * a string first.
 */
class PropertyCommitter extends AttributeCommitter {
    constructor(element, name, strings) {
        super(element, name, strings);
        this.single =
            (strings.length === 2 && strings[0] === '' && strings[1] === '');
    }
    _createPart() {
        return new PropertyPart(this);
    }
    _getValue() {
        if (this.single) {
            return this.parts[0].value;
        }
        return super._getValue();
    }
    commit() {
        if (this.dirty) {
            this.dirty = false;
            // tslint:disable-next-line:no-any
            this.element[this.name] = this._getValue();
        }
    }
}
class PropertyPart extends AttributePart {
}
// Detect event listener options support. If the `capture` property is read
// from the options object, then options are supported. If not, then the thrid
// argument to add/removeEventListener is interpreted as the boolean capture
// value so we should only pass the `capture` property.
let eventOptionsSupported = false;
try {
    const options = {
        get capture() {
            eventOptionsSupported = true;
            return false;
        }
    };
    // tslint:disable-next-line:no-any
    window.addEventListener('test', options, options);
    // tslint:disable-next-line:no-any
    window.removeEventListener('test', options, options);
}
catch (_e) {
}
class EventPart {
    constructor(element, eventName, eventContext) {
        this.value = undefined;
        this._pendingValue = undefined;
        this.element = element;
        this.eventName = eventName;
        this.eventContext = eventContext;
        this._boundHandleEvent = (e) => this.handleEvent(e);
    }
    setValue(value) {
        this._pendingValue = value;
    }
    commit() {
        while (Object(_directive_js__WEBPACK_IMPORTED_MODULE_0__["isDirective"])(this._pendingValue)) {
            const directive = this._pendingValue;
            this._pendingValue = _part_js__WEBPACK_IMPORTED_MODULE_2__["noChange"];
            directive(this);
        }
        if (this._pendingValue === _part_js__WEBPACK_IMPORTED_MODULE_2__["noChange"]) {
            return;
        }
        const newListener = this._pendingValue;
        const oldListener = this.value;
        const shouldRemoveListener = newListener == null ||
            oldListener != null &&
                (newListener.capture !== oldListener.capture ||
                    newListener.once !== oldListener.once ||
                    newListener.passive !== oldListener.passive);
        const shouldAddListener = newListener != null && (oldListener == null || shouldRemoveListener);
        if (shouldRemoveListener) {
            this.element.removeEventListener(this.eventName, this._boundHandleEvent, this._options);
        }
        if (shouldAddListener) {
            this._options = getOptions(newListener);
            this.element.addEventListener(this.eventName, this._boundHandleEvent, this._options);
        }
        this.value = newListener;
        this._pendingValue = _part_js__WEBPACK_IMPORTED_MODULE_2__["noChange"];
    }
    handleEvent(event) {
        if (typeof this.value === 'function') {
            this.value.call(this.eventContext || this.element, event);
        }
        else {
            this.value.handleEvent(event);
        }
    }
}
// We copy options because of the inconsistent behavior of browsers when reading
// the third argument of add/removeEventListener. IE11 doesn't support options
// at all. Chrome 41 only reads `capture` if the argument is an object.
const getOptions = (o) => o &&
    (eventOptionsSupported ?
        { capture: o.capture, passive: o.passive, once: o.once } :
        o.capture);
//# sourceMappingURL=parts.js.map

/***/ }),

/***/ "./node_modules/lit-html/lib/render.js":
/*!*********************************************!*\
  !*** ./node_modules/lit-html/lib/render.js ***!
  \*********************************************/
/*! exports provided: parts, render */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "parts", function() { return parts; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "render", function() { return render; });
/* harmony import */ var _dom_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./dom.js */ "./node_modules/lit-html/lib/dom.js");
/* harmony import */ var _parts_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./parts.js */ "./node_modules/lit-html/lib/parts.js");
/* harmony import */ var _template_factory_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./template-factory.js */ "./node_modules/lit-html/lib/template-factory.js");
/**
 * @license
 * Copyright (c) 2017 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */
/**
 * @module lit-html
 */



const parts = new WeakMap();
/**
 * Renders a template to a container.
 *
 * To update a container with new values, reevaluate the template literal and
 * call `render` with the new result.
 *
 * @param result a TemplateResult created by evaluating a template tag like
 *     `html` or `svg`.
 * @param container A DOM parent to render to. The entire contents are either
 *     replaced, or efficiently updated if the same result type was previous
 *     rendered there.
 * @param options RenderOptions for the entire render tree rendered to this
 *     container. Render options must *not* change between renders to the same
 *     container, as those changes will not effect previously rendered DOM.
 */
const render = (result, container, options) => {
    let part = parts.get(container);
    if (part === undefined) {
        Object(_dom_js__WEBPACK_IMPORTED_MODULE_0__["removeNodes"])(container, container.firstChild);
        parts.set(container, part = new _parts_js__WEBPACK_IMPORTED_MODULE_1__["NodePart"](Object.assign({ templateFactory: _template_factory_js__WEBPACK_IMPORTED_MODULE_2__["templateFactory"] }, options)));
        part.appendInto(container);
    }
    part.setValue(result);
    part.commit();
};
//# sourceMappingURL=render.js.map

/***/ }),

/***/ "./node_modules/lit-html/lib/shady-render.js":
/*!***************************************************!*\
  !*** ./node_modules/lit-html/lib/shady-render.js ***!
  \***************************************************/
/*! exports provided: html, svg, TemplateResult, render */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "render", function() { return render; });
/* harmony import */ var _dom_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./dom.js */ "./node_modules/lit-html/lib/dom.js");
/* harmony import */ var _modify_template_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./modify-template.js */ "./node_modules/lit-html/lib/modify-template.js");
/* harmony import */ var _render_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./render.js */ "./node_modules/lit-html/lib/render.js");
/* harmony import */ var _template_factory_js__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./template-factory.js */ "./node_modules/lit-html/lib/template-factory.js");
/* harmony import */ var _template_instance_js__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./template-instance.js */ "./node_modules/lit-html/lib/template-instance.js");
/* harmony import */ var _template_result_js__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./template-result.js */ "./node_modules/lit-html/lib/template-result.js");
/* harmony import */ var _template_js__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./template.js */ "./node_modules/lit-html/lib/template.js");
/* harmony import */ var _lit_html_js__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../lit-html.js */ "./node_modules/lit-html/lit-html.js");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "html", function() { return _lit_html_js__WEBPACK_IMPORTED_MODULE_7__["html"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "svg", function() { return _lit_html_js__WEBPACK_IMPORTED_MODULE_7__["svg"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "TemplateResult", function() { return _lit_html_js__WEBPACK_IMPORTED_MODULE_7__["TemplateResult"]; });

/**
 * @license
 * Copyright (c) 2017 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */
/**
 * Module to add shady DOM/shady CSS polyfill support to lit-html template
 * rendering. See the [[render]] method for details.
 *
 * @module shady-render
 * @preferred
 */
/**
 * Do not remove this comment; it keeps typedoc from misplacing the module
 * docs.
 */








// Get a key to lookup in `templateCaches`.
const getTemplateCacheKey = (type, scopeName) => `${type}--${scopeName}`;
let compatibleShadyCSSVersion = true;
if (typeof window.ShadyCSS === 'undefined') {
    compatibleShadyCSSVersion = false;
}
else if (typeof window.ShadyCSS.prepareTemplateDom === 'undefined') {
    console.warn(`Incompatible ShadyCSS version detected.` +
        `Please update to at least @webcomponents/webcomponentsjs@2.0.2 and` +
        `@webcomponents/shadycss@1.3.1.`);
    compatibleShadyCSSVersion = false;
}
/**
 * Template factory which scopes template DOM using ShadyCSS.
 * @param scopeName {string}
 */
const shadyTemplateFactory = (scopeName) => (result) => {
    const cacheKey = getTemplateCacheKey(result.type, scopeName);
    let templateCache = _template_factory_js__WEBPACK_IMPORTED_MODULE_3__["templateCaches"].get(cacheKey);
    if (templateCache === undefined) {
        templateCache = {
            stringsArray: new WeakMap(),
            keyString: new Map()
        };
        _template_factory_js__WEBPACK_IMPORTED_MODULE_3__["templateCaches"].set(cacheKey, templateCache);
    }
    let template = templateCache.stringsArray.get(result.strings);
    if (template !== undefined) {
        return template;
    }
    const key = result.strings.join(_template_js__WEBPACK_IMPORTED_MODULE_6__["marker"]);
    template = templateCache.keyString.get(key);
    if (template === undefined) {
        const element = result.getTemplateElement();
        if (compatibleShadyCSSVersion) {
            window.ShadyCSS.prepareTemplateDom(element, scopeName);
        }
        template = new _template_js__WEBPACK_IMPORTED_MODULE_6__["Template"](result, element);
        templateCache.keyString.set(key, template);
    }
    templateCache.stringsArray.set(result.strings, template);
    return template;
};
const TEMPLATE_TYPES = ['html', 'svg'];
/**
 * Removes all style elements from Templates for the given scopeName.
 */
const removeStylesFromLitTemplates = (scopeName) => {
    TEMPLATE_TYPES.forEach((type) => {
        const templates = _template_factory_js__WEBPACK_IMPORTED_MODULE_3__["templateCaches"].get(getTemplateCacheKey(type, scopeName));
        if (templates !== undefined) {
            templates.keyString.forEach((template) => {
                const { element: { content } } = template;
                // IE 11 doesn't support the iterable param Set constructor
                const styles = new Set();
                Array.from(content.querySelectorAll('style')).forEach((s) => {
                    styles.add(s);
                });
                Object(_modify_template_js__WEBPACK_IMPORTED_MODULE_1__["removeNodesFromTemplate"])(template, styles);
            });
        }
    });
};
const shadyRenderSet = new Set();
/**
 * For the given scope name, ensures that ShadyCSS style scoping is performed.
 * This is done just once per scope name so the fragment and template cannot
 * be modified.
 * (1) extracts styles from the rendered fragment and hands them to ShadyCSS
 * to be scoped and appended to the document
 * (2) removes style elements from all lit-html Templates for this scope name.
 *
 * Note, <style> elements can only be placed into templates for the
 * initial rendering of the scope. If <style> elements are included in templates
 * dynamically rendered to the scope (after the first scope render), they will
 * not be scoped and the <style> will be left in the template and rendered
 * output.
 */
const prepareTemplateStyles = (renderedDOM, template, scopeName) => {
    shadyRenderSet.add(scopeName);
    // Move styles out of rendered DOM and store.
    const styles = renderedDOM.querySelectorAll('style');
    // If there are no styles, skip unnecessary work
    if (styles.length === 0) {
        // Ensure prepareTemplateStyles is called to support adding
        // styles via `prepareAdoptedCssText` since that requires that
        // `prepareTemplateStyles` is called.
        window.ShadyCSS.prepareTemplateStyles(template.element, scopeName);
        return;
    }
    const condensedStyle = document.createElement('style');
    // Collect styles into a single style. This helps us make sure ShadyCSS
    // manipulations will not prevent us from being able to fix up template
    // part indices.
    // NOTE: collecting styles is inefficient for browsers but ShadyCSS
    // currently does this anyway. When it does not, this should be changed.
    for (let i = 0; i < styles.length; i++) {
        const style = styles[i];
        style.parentNode.removeChild(style);
        condensedStyle.textContent += style.textContent;
    }
    // Remove styles from nested templates in this scope.
    removeStylesFromLitTemplates(scopeName);
    // And then put the condensed style into the "root" template passed in as
    // `template`.
    Object(_modify_template_js__WEBPACK_IMPORTED_MODULE_1__["insertNodeIntoTemplate"])(template, condensedStyle, template.element.content.firstChild);
    // Note, it's important that ShadyCSS gets the template that `lit-html`
    // will actually render so that it can update the style inside when
    // needed (e.g. @apply native Shadow DOM case).
    window.ShadyCSS.prepareTemplateStyles(template.element, scopeName);
    if (window.ShadyCSS.nativeShadow) {
        // When in native Shadow DOM, re-add styling to rendered content using
        // the style ShadyCSS produced.
        const style = template.element.content.querySelector('style');
        renderedDOM.insertBefore(style.cloneNode(true), renderedDOM.firstChild);
    }
    else {
        // When not in native Shadow DOM, at this point ShadyCSS will have
        // removed the style from the lit template and parts will be broken as a
        // result. To fix this, we put back the style node ShadyCSS removed
        // and then tell lit to remove that node from the template.
        // NOTE, ShadyCSS creates its own style so we can safely add/remove
        // `condensedStyle` here.
        template.element.content.insertBefore(condensedStyle, template.element.content.firstChild);
        const removes = new Set();
        removes.add(condensedStyle);
        Object(_modify_template_js__WEBPACK_IMPORTED_MODULE_1__["removeNodesFromTemplate"])(template, removes);
    }
};
/**
 * Extension to the standard `render` method which supports rendering
 * to ShadowRoots when the ShadyDOM (https://github.com/webcomponents/shadydom)
 * and ShadyCSS (https://github.com/webcomponents/shadycss) polyfills are used
 * or when the webcomponentsjs
 * (https://github.com/webcomponents/webcomponentsjs) polyfill is used.
 *
 * Adds a `scopeName` option which is used to scope element DOM and stylesheets
 * when native ShadowDOM is unavailable. The `scopeName` will be added to
 * the class attribute of all rendered DOM. In addition, any style elements will
 * be automatically re-written with this `scopeName` selector and moved out
 * of the rendered DOM and into the document `<head>`.
 *
 * It is common to use this render method in conjunction with a custom element
 * which renders a shadowRoot. When this is done, typically the element's
 * `localName` should be used as the `scopeName`.
 *
 * In addition to DOM scoping, ShadyCSS also supports a basic shim for css
 * custom properties (needed only on older browsers like IE11) and a shim for
 * a deprecated feature called `@apply` that supports applying a set of css
 * custom properties to a given location.
 *
 * Usage considerations:
 *
 * * Part values in `<style>` elements are only applied the first time a given
 * `scopeName` renders. Subsequent changes to parts in style elements will have
 * no effect. Because of this, parts in style elements should only be used for
 * values that will never change, for example parts that set scope-wide theme
 * values or parts which render shared style elements.
 *
 * * Note, due to a limitation of the ShadyDOM polyfill, rendering in a
 * custom element's `constructor` is not supported. Instead rendering should
 * either done asynchronously, for example at microtask timing (for example
 * `Promise.resolve()`), or be deferred until the first time the element's
 * `connectedCallback` runs.
 *
 * Usage considerations when using shimmed custom properties or `@apply`:
 *
 * * Whenever any dynamic changes are made which affect
 * css custom properties, `ShadyCSS.styleElement(element)` must be called
 * to update the element. There are two cases when this is needed:
 * (1) the element is connected to a new parent, (2) a class is added to the
 * element that causes it to match different custom properties.
 * To address the first case when rendering a custom element, `styleElement`
 * should be called in the element's `connectedCallback`.
 *
 * * Shimmed custom properties may only be defined either for an entire
 * shadowRoot (for example, in a `:host` rule) or via a rule that directly
 * matches an element with a shadowRoot. In other words, instead of flowing from
 * parent to child as do native css custom properties, shimmed custom properties
 * flow only from shadowRoots to nested shadowRoots.
 *
 * * When using `@apply` mixing css shorthand property names with
 * non-shorthand names (for example `border` and `border-width`) is not
 * supported.
 */
const render = (result, container, options) => {
    const scopeName = options.scopeName;
    const hasRendered = _render_js__WEBPACK_IMPORTED_MODULE_2__["parts"].has(container);
    const needsScoping = container instanceof ShadowRoot &&
        compatibleShadyCSSVersion && result instanceof _template_result_js__WEBPACK_IMPORTED_MODULE_5__["TemplateResult"];
    // Handle first render to a scope specially...
    const firstScopeRender = needsScoping && !shadyRenderSet.has(scopeName);
    // On first scope render, render into a fragment; this cannot be a single
    // fragment that is reused since nested renders can occur synchronously.
    const renderContainer = firstScopeRender ? document.createDocumentFragment() : container;
    Object(_render_js__WEBPACK_IMPORTED_MODULE_2__["render"])(result, renderContainer, Object.assign({ templateFactory: shadyTemplateFactory(scopeName) }, options));
    // When performing first scope render,
    // (1) We've rendered into a fragment so that there's a chance to
    // `prepareTemplateStyles` before sub-elements hit the DOM
    // (which might cause them to render based on a common pattern of
    // rendering in a custom element's `connectedCallback`);
    // (2) Scope the template with ShadyCSS one time only for this scope.
    // (3) Render the fragment into the container and make sure the
    // container knows its `part` is the one we just rendered. This ensures
    // DOM will be re-used on subsequent renders.
    if (firstScopeRender) {
        const part = _render_js__WEBPACK_IMPORTED_MODULE_2__["parts"].get(renderContainer);
        _render_js__WEBPACK_IMPORTED_MODULE_2__["parts"].delete(renderContainer);
        if (part.value instanceof _template_instance_js__WEBPACK_IMPORTED_MODULE_4__["TemplateInstance"]) {
            prepareTemplateStyles(renderContainer, part.value.template, scopeName);
        }
        Object(_dom_js__WEBPACK_IMPORTED_MODULE_0__["removeNodes"])(container, container.firstChild);
        container.appendChild(renderContainer);
        _render_js__WEBPACK_IMPORTED_MODULE_2__["parts"].set(container, part);
    }
    // After elements have hit the DOM, update styling if this is the
    // initial render to this container.
    // This is needed whenever dynamic changes are made so it would be
    // safest to do every render; however, this would regress performance
    // so we leave it up to the user to call `ShadyCSSS.styleElement`
    // for dynamic changes.
    if (!hasRendered && needsScoping) {
        window.ShadyCSS.styleElement(container.host);
    }
};
//# sourceMappingURL=shady-render.js.map

/***/ }),

/***/ "./node_modules/lit-html/lib/template-factory.js":
/*!*******************************************************!*\
  !*** ./node_modules/lit-html/lib/template-factory.js ***!
  \*******************************************************/
/*! exports provided: templateFactory, templateCaches */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "templateFactory", function() { return templateFactory; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "templateCaches", function() { return templateCaches; });
/* harmony import */ var _template_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./template.js */ "./node_modules/lit-html/lib/template.js");
/**
 * @license
 * Copyright (c) 2017 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */

/**
 * The default TemplateFactory which caches Templates keyed on
 * result.type and result.strings.
 */
function templateFactory(result) {
    let templateCache = templateCaches.get(result.type);
    if (templateCache === undefined) {
        templateCache = {
            stringsArray: new WeakMap(),
            keyString: new Map()
        };
        templateCaches.set(result.type, templateCache);
    }
    let template = templateCache.stringsArray.get(result.strings);
    if (template !== undefined) {
        return template;
    }
    // If the TemplateStringsArray is new, generate a key from the strings
    // This key is shared between all templates with identical content
    const key = result.strings.join(_template_js__WEBPACK_IMPORTED_MODULE_0__["marker"]);
    // Check if we already have a Template for this key
    template = templateCache.keyString.get(key);
    if (template === undefined) {
        // If we have not seen this key before, create a new Template
        template = new _template_js__WEBPACK_IMPORTED_MODULE_0__["Template"](result, result.getTemplateElement());
        // Cache the Template for this key
        templateCache.keyString.set(key, template);
    }
    // Cache all future queries for this TemplateStringsArray
    templateCache.stringsArray.set(result.strings, template);
    return template;
}
const templateCaches = new Map();
//# sourceMappingURL=template-factory.js.map

/***/ }),

/***/ "./node_modules/lit-html/lib/template-instance.js":
/*!********************************************************!*\
  !*** ./node_modules/lit-html/lib/template-instance.js ***!
  \********************************************************/
/*! exports provided: TemplateInstance */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "TemplateInstance", function() { return TemplateInstance; });
/* harmony import */ var _dom_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./dom.js */ "./node_modules/lit-html/lib/dom.js");
/* harmony import */ var _template_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./template.js */ "./node_modules/lit-html/lib/template.js");
/**
 * @license
 * Copyright (c) 2017 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */
/**
 * @module lit-html
 */


/**
 * An instance of a `Template` that can be attached to the DOM and updated
 * with new values.
 */
class TemplateInstance {
    constructor(template, processor, options) {
        this._parts = [];
        this.template = template;
        this.processor = processor;
        this.options = options;
    }
    update(values) {
        let i = 0;
        for (const part of this._parts) {
            if (part !== undefined) {
                part.setValue(values[i]);
            }
            i++;
        }
        for (const part of this._parts) {
            if (part !== undefined) {
                part.commit();
            }
        }
    }
    _clone() {
        // When using the Custom Elements polyfill, clone the node, rather than
        // importing it, to keep the fragment in the template's document. This
        // leaves the fragment inert so custom elements won't upgrade and
        // potentially modify their contents by creating a polyfilled ShadowRoot
        // while we traverse the tree.
        const fragment = _dom_js__WEBPACK_IMPORTED_MODULE_0__["isCEPolyfill"] ?
            this.template.element.content.cloneNode(true) :
            document.importNode(this.template.element.content, true);
        const parts = this.template.parts;
        let partIndex = 0;
        let nodeIndex = 0;
        const _prepareInstance = (fragment) => {
            // Edge needs all 4 parameters present; IE11 needs 3rd parameter to be
            // null
            const walker = document.createTreeWalker(fragment, 133 /* NodeFilter.SHOW_{ELEMENT|COMMENT|TEXT} */, null, false);
            let node = walker.nextNode();
            // Loop through all the nodes and parts of a template
            while (partIndex < parts.length && node !== null) {
                const part = parts[partIndex];
                // Consecutive Parts may have the same node index, in the case of
                // multiple bound attributes on an element. So each iteration we either
                // increment the nodeIndex, if we aren't on a node with a part, or the
                // partIndex if we are. By not incrementing the nodeIndex when we find a
                // part, we allow for the next part to be associated with the current
                // node if neccessasry.
                if (!Object(_template_js__WEBPACK_IMPORTED_MODULE_1__["isTemplatePartActive"])(part)) {
                    this._parts.push(undefined);
                    partIndex++;
                }
                else if (nodeIndex === part.index) {
                    if (part.type === 'node') {
                        const part = this.processor.handleTextExpression(this.options);
                        part.insertAfterNode(node.previousSibling);
                        this._parts.push(part);
                    }
                    else {
                        this._parts.push(...this.processor.handleAttributeExpressions(node, part.name, part.strings, this.options));
                    }
                    partIndex++;
                }
                else {
                    nodeIndex++;
                    if (node.nodeName === 'TEMPLATE') {
                        _prepareInstance(node.content);
                    }
                    node = walker.nextNode();
                }
            }
        };
        _prepareInstance(fragment);
        if (_dom_js__WEBPACK_IMPORTED_MODULE_0__["isCEPolyfill"]) {
            document.adoptNode(fragment);
            customElements.upgrade(fragment);
        }
        return fragment;
    }
}
//# sourceMappingURL=template-instance.js.map

/***/ }),

/***/ "./node_modules/lit-html/lib/template-result.js":
/*!******************************************************!*\
  !*** ./node_modules/lit-html/lib/template-result.js ***!
  \******************************************************/
/*! exports provided: TemplateResult, SVGTemplateResult */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "TemplateResult", function() { return TemplateResult; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SVGTemplateResult", function() { return SVGTemplateResult; });
/* harmony import */ var _dom_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./dom.js */ "./node_modules/lit-html/lib/dom.js");
/* harmony import */ var _template_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./template.js */ "./node_modules/lit-html/lib/template.js");
/**
 * @license
 * Copyright (c) 2017 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */
/**
 * @module lit-html
 */


/**
 * The return type of `html`, which holds a Template and the values from
 * interpolated expressions.
 */
class TemplateResult {
    constructor(strings, values, type, processor) {
        this.strings = strings;
        this.values = values;
        this.type = type;
        this.processor = processor;
    }
    /**
     * Returns a string of HTML used to create a `<template>` element.
     */
    getHTML() {
        const endIndex = this.strings.length - 1;
        let html = '';
        for (let i = 0; i < endIndex; i++) {
            const s = this.strings[i];
            // This exec() call does two things:
            // 1) Appends a suffix to the bound attribute name to opt out of special
            // attribute value parsing that IE11 and Edge do, like for style and
            // many SVG attributes. The Template class also appends the same suffix
            // when looking up attributes to create Parts.
            // 2) Adds an unquoted-attribute-safe marker for the first expression in
            // an attribute. Subsequent attribute expressions will use node markers,
            // and this is safe since attributes with multiple expressions are
            // guaranteed to be quoted.
            const match = _template_js__WEBPACK_IMPORTED_MODULE_1__["lastAttributeNameRegex"].exec(s);
            if (match) {
                // We're starting a new bound attribute.
                // Add the safe attribute suffix, and use unquoted-attribute-safe
                // marker.
                html += s.substr(0, match.index) + match[1] + match[2] +
                    _template_js__WEBPACK_IMPORTED_MODULE_1__["boundAttributeSuffix"] + match[3] + _template_js__WEBPACK_IMPORTED_MODULE_1__["marker"];
            }
            else {
                // We're either in a bound node, or trailing bound attribute.
                // Either way, nodeMarker is safe to use.
                html += s + _template_js__WEBPACK_IMPORTED_MODULE_1__["nodeMarker"];
            }
        }
        return html + this.strings[endIndex];
    }
    getTemplateElement() {
        const template = document.createElement('template');
        template.innerHTML = this.getHTML();
        return template;
    }
}
/**
 * A TemplateResult for SVG fragments.
 *
 * This class wraps HTMl in an `<svg>` tag in order to parse its contents in the
 * SVG namespace, then modifies the template to remove the `<svg>` tag so that
 * clones only container the original fragment.
 */
class SVGTemplateResult extends TemplateResult {
    getHTML() {
        return `<svg>${super.getHTML()}</svg>`;
    }
    getTemplateElement() {
        const template = super.getTemplateElement();
        const content = template.content;
        const svgElement = content.firstChild;
        content.removeChild(svgElement);
        Object(_dom_js__WEBPACK_IMPORTED_MODULE_0__["reparentNodes"])(content, svgElement.firstChild);
        return template;
    }
}
//# sourceMappingURL=template-result.js.map

/***/ }),

/***/ "./node_modules/lit-html/lib/template.js":
/*!***********************************************!*\
  !*** ./node_modules/lit-html/lib/template.js ***!
  \***********************************************/
/*! exports provided: marker, nodeMarker, markerRegex, boundAttributeSuffix, Template, isTemplatePartActive, createMarker, lastAttributeNameRegex */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "marker", function() { return marker; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "nodeMarker", function() { return nodeMarker; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "markerRegex", function() { return markerRegex; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "boundAttributeSuffix", function() { return boundAttributeSuffix; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "Template", function() { return Template; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "isTemplatePartActive", function() { return isTemplatePartActive; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "createMarker", function() { return createMarker; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "lastAttributeNameRegex", function() { return lastAttributeNameRegex; });
/**
 * @license
 * Copyright (c) 2017 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */
/**
 * An expression marker with embedded unique key to avoid collision with
 * possible text in templates.
 */
const marker = `{{lit-${String(Math.random()).slice(2)}}}`;
/**
 * An expression marker used text-positions, multi-binding attributes, and
 * attributes with markup-like text values.
 */
const nodeMarker = `<!--${marker}-->`;
const markerRegex = new RegExp(`${marker}|${nodeMarker}`);
/**
 * Suffix appended to all bound attribute names.
 */
const boundAttributeSuffix = '$lit$';
/**
 * An updateable Template that tracks the location of dynamic parts.
 */
class Template {
    constructor(result, element) {
        this.parts = [];
        this.element = element;
        let index = -1;
        let partIndex = 0;
        const nodesToRemove = [];
        const _prepareTemplate = (template) => {
            const content = template.content;
            // Edge needs all 4 parameters present; IE11 needs 3rd parameter to be
            // null
            const walker = document.createTreeWalker(content, 133 /* NodeFilter.SHOW_{ELEMENT|COMMENT|TEXT} */, null, false);
            // Keeps track of the last index associated with a part. We try to delete
            // unnecessary nodes, but we never want to associate two different parts
            // to the same index. They must have a constant node between.
            let lastPartIndex = 0;
            while (walker.nextNode()) {
                index++;
                const node = walker.currentNode;
                if (node.nodeType === 1 /* Node.ELEMENT_NODE */) {
                    if (node.hasAttributes()) {
                        const attributes = node.attributes;
                        // Per
                        // https://developer.mozilla.org/en-US/docs/Web/API/NamedNodeMap,
                        // attributes are not guaranteed to be returned in document order.
                        // In particular, Edge/IE can return them out of order, so we cannot
                        // assume a correspondance between part index and attribute index.
                        let count = 0;
                        for (let i = 0; i < attributes.length; i++) {
                            if (attributes[i].value.indexOf(marker) >= 0) {
                                count++;
                            }
                        }
                        while (count-- > 0) {
                            // Get the template literal section leading up to the first
                            // expression in this attribute
                            const stringForPart = result.strings[partIndex];
                            // Find the attribute name
                            const name = lastAttributeNameRegex.exec(stringForPart)[2];
                            // Find the corresponding attribute
                            // All bound attributes have had a suffix added in
                            // TemplateResult#getHTML to opt out of special attribute
                            // handling. To look up the attribute value we also need to add
                            // the suffix.
                            const attributeLookupName = name.toLowerCase() + boundAttributeSuffix;
                            const attributeValue = node.getAttribute(attributeLookupName);
                            const strings = attributeValue.split(markerRegex);
                            this.parts.push({ type: 'attribute', index, name, strings });
                            node.removeAttribute(attributeLookupName);
                            partIndex += strings.length - 1;
                        }
                    }
                    if (node.tagName === 'TEMPLATE') {
                        _prepareTemplate(node);
                    }
                }
                else if (node.nodeType === 3 /* Node.TEXT_NODE */) {
                    const data = node.data;
                    if (data.indexOf(marker) >= 0) {
                        const parent = node.parentNode;
                        const strings = data.split(markerRegex);
                        const lastIndex = strings.length - 1;
                        // Generate a new text node for each literal section
                        // These nodes are also used as the markers for node parts
                        for (let i = 0; i < lastIndex; i++) {
                            parent.insertBefore((strings[i] === '') ? createMarker() :
                                document.createTextNode(strings[i]), node);
                            this.parts.push({ type: 'node', index: ++index });
                        }
                        // If there's no text, we must insert a comment to mark our place.
                        // Else, we can trust it will stick around after cloning.
                        if (strings[lastIndex] === '') {
                            parent.insertBefore(createMarker(), node);
                            nodesToRemove.push(node);
                        }
                        else {
                            node.data = strings[lastIndex];
                        }
                        // We have a part for each match found
                        partIndex += lastIndex;
                    }
                }
                else if (node.nodeType === 8 /* Node.COMMENT_NODE */) {
                    if (node.data === marker) {
                        const parent = node.parentNode;
                        // Add a new marker node to be the startNode of the Part if any of
                        // the following are true:
                        //  * We don't have a previousSibling
                        //  * The previousSibling is already the start of a previous part
                        if (node.previousSibling === null || index === lastPartIndex) {
                            index++;
                            parent.insertBefore(createMarker(), node);
                        }
                        lastPartIndex = index;
                        this.parts.push({ type: 'node', index });
                        // If we don't have a nextSibling, keep this node so we have an end.
                        // Else, we can remove it to save future costs.
                        if (node.nextSibling === null) {
                            node.data = '';
                        }
                        else {
                            nodesToRemove.push(node);
                            index--;
                        }
                        partIndex++;
                    }
                    else {
                        let i = -1;
                        while ((i = node.data.indexOf(marker, i + 1)) !==
                            -1) {
                            // Comment node has a binding marker inside, make an inactive part
                            // The binding won't work, but subsequent bindings will
                            // TODO (justinfagnani): consider whether it's even worth it to
                            // make bindings in comments work
                            this.parts.push({ type: 'node', index: -1 });
                        }
                    }
                }
            }
        };
        _prepareTemplate(element);
        // Remove text binding nodes after the walk to not disturb the TreeWalker
        for (const n of nodesToRemove) {
            n.parentNode.removeChild(n);
        }
    }
}
const isTemplatePartActive = (part) => part.index !== -1;
// Allows `document.createComment('')` to be renamed for a
// small manual size-savings.
const createMarker = () => document.createComment('');
/**
 * This regex extracts the attribute name preceding an attribute-position
 * expression. It does this by matching the syntax allowed for attributes
 * against the string literal directly preceding the expression, assuming that
 * the expression is in an attribute-value position.
 *
 * See attributes in the HTML spec:
 * https://www.w3.org/TR/html5/syntax.html#attributes-0
 *
 * "\0-\x1F\x7F-\x9F" are Unicode control characters
 *
 * " \x09\x0a\x0c\x0d" are HTML space characters:
 * https://www.w3.org/TR/html5/infrastructure.html#space-character
 *
 * So an attribute is:
 *  * The name: any character except a control character, space character, ('),
 *    ("), ">", "=", or "/"
 *  * Followed by zero or more space characters
 *  * Followed by "="
 *  * Followed by zero or more space characters
 *  * Followed by:
 *    * Any character except space, ('), ("), "<", ">", "=", (`), or
 *    * (") then any non-("), or
 *    * (') then any non-(')
 */
const lastAttributeNameRegex = /([ \x09\x0a\x0c\x0d])([^\0-\x1F\x7F-\x9F \x09\x0a\x0c\x0d"'>=/]+)([ \x09\x0a\x0c\x0d]*=[ \x09\x0a\x0c\x0d]*(?:[^ \x09\x0a\x0c\x0d"'`<>=]*|"[^"]*|'[^']*))$/;
//# sourceMappingURL=template.js.map

/***/ }),

/***/ "./node_modules/lit-html/lit-html.js":
/*!*******************************************!*\
  !*** ./node_modules/lit-html/lit-html.js ***!
  \*******************************************/
/*! exports provided: DefaultTemplateProcessor, defaultTemplateProcessor, directive, isDirective, removeNodes, reparentNodes, noChange, nothing, AttributeCommitter, AttributePart, BooleanAttributePart, EventPart, isPrimitive, NodePart, PropertyCommitter, PropertyPart, parts, render, templateCaches, templateFactory, TemplateInstance, SVGTemplateResult, TemplateResult, createMarker, isTemplatePartActive, Template, html, svg */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "html", function() { return html; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "svg", function() { return svg; });
/* harmony import */ var _lib_default_template_processor_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./lib/default-template-processor.js */ "./node_modules/lit-html/lib/default-template-processor.js");
/* harmony import */ var _lib_template_result_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./lib/template-result.js */ "./node_modules/lit-html/lib/template-result.js");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "DefaultTemplateProcessor", function() { return _lib_default_template_processor_js__WEBPACK_IMPORTED_MODULE_0__["DefaultTemplateProcessor"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "defaultTemplateProcessor", function() { return _lib_default_template_processor_js__WEBPACK_IMPORTED_MODULE_0__["defaultTemplateProcessor"]; });

/* harmony import */ var _lib_directive_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./lib/directive.js */ "./node_modules/lit-html/lib/directive.js");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "directive", function() { return _lib_directive_js__WEBPACK_IMPORTED_MODULE_2__["directive"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "isDirective", function() { return _lib_directive_js__WEBPACK_IMPORTED_MODULE_2__["isDirective"]; });

/* harmony import */ var _lib_dom_js__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./lib/dom.js */ "./node_modules/lit-html/lib/dom.js");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "removeNodes", function() { return _lib_dom_js__WEBPACK_IMPORTED_MODULE_3__["removeNodes"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "reparentNodes", function() { return _lib_dom_js__WEBPACK_IMPORTED_MODULE_3__["reparentNodes"]; });

/* harmony import */ var _lib_part_js__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./lib/part.js */ "./node_modules/lit-html/lib/part.js");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "noChange", function() { return _lib_part_js__WEBPACK_IMPORTED_MODULE_4__["noChange"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "nothing", function() { return _lib_part_js__WEBPACK_IMPORTED_MODULE_4__["nothing"]; });

/* harmony import */ var _lib_parts_js__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./lib/parts.js */ "./node_modules/lit-html/lib/parts.js");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "AttributeCommitter", function() { return _lib_parts_js__WEBPACK_IMPORTED_MODULE_5__["AttributeCommitter"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "AttributePart", function() { return _lib_parts_js__WEBPACK_IMPORTED_MODULE_5__["AttributePart"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "BooleanAttributePart", function() { return _lib_parts_js__WEBPACK_IMPORTED_MODULE_5__["BooleanAttributePart"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "EventPart", function() { return _lib_parts_js__WEBPACK_IMPORTED_MODULE_5__["EventPart"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "isPrimitive", function() { return _lib_parts_js__WEBPACK_IMPORTED_MODULE_5__["isPrimitive"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "NodePart", function() { return _lib_parts_js__WEBPACK_IMPORTED_MODULE_5__["NodePart"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "PropertyCommitter", function() { return _lib_parts_js__WEBPACK_IMPORTED_MODULE_5__["PropertyCommitter"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "PropertyPart", function() { return _lib_parts_js__WEBPACK_IMPORTED_MODULE_5__["PropertyPart"]; });

/* harmony import */ var _lib_render_js__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./lib/render.js */ "./node_modules/lit-html/lib/render.js");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "parts", function() { return _lib_render_js__WEBPACK_IMPORTED_MODULE_6__["parts"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "render", function() { return _lib_render_js__WEBPACK_IMPORTED_MODULE_6__["render"]; });

/* harmony import */ var _lib_template_factory_js__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./lib/template-factory.js */ "./node_modules/lit-html/lib/template-factory.js");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "templateCaches", function() { return _lib_template_factory_js__WEBPACK_IMPORTED_MODULE_7__["templateCaches"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "templateFactory", function() { return _lib_template_factory_js__WEBPACK_IMPORTED_MODULE_7__["templateFactory"]; });

/* harmony import */ var _lib_template_instance_js__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./lib/template-instance.js */ "./node_modules/lit-html/lib/template-instance.js");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "TemplateInstance", function() { return _lib_template_instance_js__WEBPACK_IMPORTED_MODULE_8__["TemplateInstance"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "SVGTemplateResult", function() { return _lib_template_result_js__WEBPACK_IMPORTED_MODULE_1__["SVGTemplateResult"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "TemplateResult", function() { return _lib_template_result_js__WEBPACK_IMPORTED_MODULE_1__["TemplateResult"]; });

/* harmony import */ var _lib_template_js__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./lib/template.js */ "./node_modules/lit-html/lib/template.js");
/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "createMarker", function() { return _lib_template_js__WEBPACK_IMPORTED_MODULE_9__["createMarker"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "isTemplatePartActive", function() { return _lib_template_js__WEBPACK_IMPORTED_MODULE_9__["isTemplatePartActive"]; });

/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, "Template", function() { return _lib_template_js__WEBPACK_IMPORTED_MODULE_9__["Template"]; });

/**
 * @license
 * Copyright (c) 2017 The Polymer Project Authors. All rights reserved.
 * This code may only be used under the BSD style license found at
 * http://polymer.github.io/LICENSE.txt
 * The complete set of authors may be found at
 * http://polymer.github.io/AUTHORS.txt
 * The complete set of contributors may be found at
 * http://polymer.github.io/CONTRIBUTORS.txt
 * Code distributed by Google as part of the polymer project is also
 * subject to an additional IP rights grant found at
 * http://polymer.github.io/PATENTS.txt
 */
/**
 *
 * Main lit-html module.
 *
 * Main exports:
 *
 * -  [[html]]
 * -  [[svg]]
 * -  [[render]]
 *
 * @module lit-html
 * @preferred
 */
/**
 * Do not remove this comment; it keeps typedoc from misplacing the module
 * docs.
 */




// TODO(justinfagnani): remove line when we get NodePart moving methods








// IMPORTANT: do not change the property name or the assignment expression.
// This line will be used in regexes to search for lit-html usage.
// TODO(justinfagnani): inject version number at build time
(window['litHtmlVersions'] || (window['litHtmlVersions'] = [])).push('1.0.0');
/**
 * Interprets a template literal as an HTML template that can efficiently
 * render to and update a container.
 */
const html = (strings, ...values) => new _lib_template_result_js__WEBPACK_IMPORTED_MODULE_1__["TemplateResult"](strings, values, 'html', _lib_default_template_processor_js__WEBPACK_IMPORTED_MODULE_0__["defaultTemplateProcessor"]);
/**
 * Interprets a template literal as an SVG template that can efficiently
 * render to and update a container.
 */
const svg = (strings, ...values) => new _lib_template_result_js__WEBPACK_IMPORTED_MODULE_1__["SVGTemplateResult"](strings, values, 'svg', _lib_default_template_processor_js__WEBPACK_IMPORTED_MODULE_0__["defaultTemplateProcessor"]);
//# sourceMappingURL=lit-html.js.map

/***/ }),

/***/ "./src/components/mutation-test-report-app.ts":
/*!****************************************************!*\
  !*** ./src/components/mutation-test-report-app.ts ***!
  \****************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
const lit_element_1 = __webpack_require__(/*! lit-element */ "./node_modules/lit-element/lit-element.js");
const helpers_1 = __webpack_require__(/*! ../helpers */ "./src/helpers.ts");
const style_1 = __webpack_require__(/*! ../style */ "./src/style/index.ts");
let MutationTestReportAppComponent = class MutationTestReportAppComponent extends lit_element_1.LitElement {
    constructor() {
        super(...arguments);
        this.updatePath = (event) => {
            this.path = event.detail;
            this.updateContext();
        };
    }
    connectedCallback() {
        super.connectedCallback();
        if (this.src) {
            this.loadData(this.src)
                .catch(error => this.errorMessage = error.toString());
        }
        else {
            this.errorMessage = 'Source not set. Please point the `src` attribute to the mutation test report data.';
        }
    }
    loadData(src) {
        return __awaiter(this, void 0, void 0, function* () {
            const res = yield fetch(src);
            this.report = yield res.json();
            this.updateContext();
        });
    }
    updateContext() {
        if (this.path) {
            const pathQueue = this.path.slice();
            let newContext = this.report;
            let pathPart;
            while (pathPart = pathQueue.shift()) {
                if (helpers_1.isDirectoryResult(newContext)) {
                    newContext = newContext.childResults.find(child => child.name === pathPart);
                }
                else {
                    newContext = undefined;
                    break;
                }
            }
            this.context = newContext;
            if (!this.context) {
                this.errorMessage = `404 - ${this.path.join('/')} not found`;
            }
            else {
                this.errorMessage = undefined;
            }
        }
    }
    render() {
        return lit_element_1.html `
    <mutation-test-report-router @path-changed="${this.updatePath}"></mutation-test-report-router>
    <div class="container">
      <div class="row">
        <div class="col-md-12">
          ${this.renderTitle()}
          <mutation-test-report-breadcrumb .path="${this.path}"></mutation-test-report-breadcrumb>
          ${this.renderErrorMessage()}
          ${this.renderMutationTestReport()}
        </div>
      </div>
    </div>
    `;
    }
    renderTitle() {
        if (this.context) {
            return lit_element_1.html `<h1 class="display-1">${this.context.name}</h1>`;
        }
        else {
            return undefined;
        }
    }
    renderErrorMessage() {
        if (this.errorMessage) {
            return lit_element_1.html `
      <div class="alert alert-danger" role="alert">
        ${this.errorMessage}
      </div>
        `;
        }
        else {
            return lit_element_1.html ``;
        }
    }
    renderMutationTestReport() {
        if (this.context) {
            return lit_element_1.html `<mutation-test-report-result .currentPath="${this.path}" .model="${this.context}"></mutation-test-report-result>`;
        }
        else {
            return '';
        }
    }
};
MutationTestReportAppComponent.styles = [
    style_1.bootstrap,
    lit_element_1.css `
    :host {
      line-height: 1.15;
      margin: 0;
      font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, "Noto Sans", sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji";
      font-size: 1rem;
      font-weight: 400;
      line-height: 1.5;
      color: #212529;
      text-align: left;
      background-color: #fff;
    }
    `
];
__decorate([
    lit_element_1.property()
], MutationTestReportAppComponent.prototype, "src", void 0);
__decorate([
    lit_element_1.property()
], MutationTestReportAppComponent.prototype, "errorMessage", void 0);
__decorate([
    lit_element_1.property()
], MutationTestReportAppComponent.prototype, "context", void 0);
__decorate([
    lit_element_1.property()
], MutationTestReportAppComponent.prototype, "path", void 0);
MutationTestReportAppComponent = __decorate([
    lit_element_1.customElement('mutation-test-report-app')
], MutationTestReportAppComponent);
exports.MutationTestReportAppComponent = MutationTestReportAppComponent;


/***/ }),

/***/ "./src/components/mutation-test-report-breadcrumb.ts":
/*!***********************************************************!*\
  !*** ./src/components/mutation-test-report-breadcrumb.ts ***!
  \***********************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
const lit_element_1 = __webpack_require__(/*! lit-element */ "./node_modules/lit-element/lit-element.js");
const style_1 = __webpack_require__(/*! ../style */ "./src/style/index.ts");
let MutationTestReportBreadcrumbComponent = class MutationTestReportBreadcrumbComponent extends lit_element_1.LitElement {
    render() {
        return lit_element_1.html `
        <ol class='breadcrumb'>
          ${this.renderRootItem()}
          ${this.path ? this.renderBreadcrumbItems(this.path) : ''}
        </ol>
    `;
    }
    renderRootItem() {
        if (this.path && this.path.length) {
            return this.renderLink('All files', '#');
        }
        else {
            return this.renderActiveItem('All files');
        }
    }
    renderBreadcrumbItems(path) {
        return path.map((item, index) => {
            if (index === path.length - 1) {
                return this.renderActiveItem(item);
            }
            else {
                return this.renderLink(item, `#${path.filter((_, i) => i <= index).join('/')}`);
            }
        });
    }
    renderActiveItem(title) {
        return lit_element_1.html `<li class="breadcrumb-item active" aria-current="page">${title}</li>`;
    }
    renderLink(title, url) {
        return lit_element_1.html `<li class="breadcrumb-item"><a href="${url}">${title}</a></li>`;
    }
};
MutationTestReportBreadcrumbComponent.styles = [style_1.bootstrap];
__decorate([
    lit_element_1.property()
], MutationTestReportBreadcrumbComponent.prototype, "path", void 0);
MutationTestReportBreadcrumbComponent = __decorate([
    lit_element_1.customElement('mutation-test-report-breadcrumb')
], MutationTestReportBreadcrumbComponent);
exports.MutationTestReportBreadcrumbComponent = MutationTestReportBreadcrumbComponent;


/***/ }),

/***/ "./src/components/mutation-test-report-file-legend.ts":
/*!************************************************************!*\
  !*** ./src/components/mutation-test-report-file-legend.ts ***!
  \************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
const lit_element_1 = __webpack_require__(/*! lit-element */ "./node_modules/lit-element/lit-element.js");
const style_1 = __webpack_require__(/*! ../style */ "./src/style/index.ts");
let MutationTestReportFileLegendComponent = class MutationTestReportFileLegendComponent extends lit_element_1.LitElement {
    constructor() {
        super(...arguments);
        this.collapsed = true;
        this.filters = [];
        this.toggleOpenAll = () => {
            this.collapsed = !this.collapsed;
            if (this.collapsed) {
                this.dispatchEvent(new CustomEvent('collapse-all'));
            }
            else {
                this.dispatchEvent(new CustomEvent('open-all'));
            }
        };
    }
    get collapseButtonText() {
        if (this.collapsed) {
            return 'Expand all';
        }
        else {
            return 'Collapse all';
        }
    }
    updated(changedProperties) {
        if (changedProperties.has('mutants')) {
            this.updateModel();
        }
    }
    updateModel() {
        this.filters = ["Killed" /* Killed */, "Survived" /* Survived */, "NoCoverage" /* NoCoverage */, "Timeout" /* Timeout */, "CompileError" /* CompileError */, "RuntimeError" /* RuntimeError */]
            .map(status => ({
            enabled: ["Survived" /* Survived */, "NoCoverage" /* NoCoverage */, "Timeout" /* Timeout */].some(s => s === status),
            numberOfMutants: this.mutants.filter(m => m.status === status).length,
            status
        }));
        this.dispatchFiltersChangedEvent();
    }
    checkboxClicked(filter) {
        filter.enabled = !filter.enabled;
        this.dispatchFiltersChangedEvent();
    }
    dispatchFiltersChangedEvent() {
        this.dispatchEvent(new CustomEvent('filters-changed', { detail: this.filters }));
    }
    render() {
        return lit_element_1.html `
      <div class='row legend'>
        <form class='col-md-12' novalidate='novalidate'>
          ${this.filters.map(filter => lit_element_1.html `
          <div class="form-check form-check-inline">
            <label class="form-check-label">
              <input class="form-check-input" type="checkbox" ?checked="${filter.enabled}" value="${filter.status}" @input="${() => this.checkboxClicked(filter)}">
              ${filter.status} (${filter.numberOfMutants})
            </label>
          </div>
          `)}
          <button @click="${this.toggleOpenAll}" class="btn btn-sm btn-secondary" role="link">${this.collapseButtonText}</button>
        </form>
      </div>
    `;
    }
};
MutationTestReportFileLegendComponent.styles = [
    lit_element_1.css `
      .legend{
        position: sticky;
        top: 0;
        background: #FFF;
      }
  `, style_1.bootstrap
];
__decorate([
    lit_element_1.property()
], MutationTestReportFileLegendComponent.prototype, "mutants", void 0);
__decorate([
    lit_element_1.property()
], MutationTestReportFileLegendComponent.prototype, "collapseButtonText", null);
__decorate([
    lit_element_1.property()
], MutationTestReportFileLegendComponent.prototype, "collapsed", void 0);
__decorate([
    lit_element_1.property()
], MutationTestReportFileLegendComponent.prototype, "filters", void 0);
MutationTestReportFileLegendComponent = __decorate([
    lit_element_1.customElement('mutation-test-report-file-legend')
], MutationTestReportFileLegendComponent);
exports.MutationTestReportFileLegendComponent = MutationTestReportFileLegendComponent;


/***/ }),

/***/ "./src/components/mutation-test-report-file.ts":
/*!*****************************************************!*\
  !*** ./src/components/mutation-test-report-file.ts ***!
  \*****************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const lit_element_1 = __webpack_require__(/*! lit-element */ "./node_modules/lit-element/lit-element.js");
const highlight_1 = __importDefault(__webpack_require__(/*! highlight.js/lib/highlight */ "./node_modules/highlight.js/lib/highlight.js"));
const javascript_1 = __importDefault(__webpack_require__(/*! highlight.js/lib/languages/javascript */ "./node_modules/highlight.js/lib/languages/javascript.js"));
const scala_1 = __importDefault(__webpack_require__(/*! highlight.js/lib/languages/scala */ "./node_modules/highlight.js/lib/languages/scala.js"));
const java_1 = __importDefault(__webpack_require__(/*! highlight.js/lib/languages/java */ "./node_modules/highlight.js/lib/languages/java.js"));
const cs_1 = __importDefault(__webpack_require__(/*! highlight.js/lib/languages/cs */ "./node_modules/highlight.js/lib/languages/cs.js"));
const typescript_1 = __importDefault(__webpack_require__(/*! highlight.js/lib/languages/typescript */ "./node_modules/highlight.js/lib/languages/typescript.js"));
const helpers_1 = __webpack_require__(/*! ../helpers */ "./src/helpers.ts");
const mutation_test_report_mutant_1 = __webpack_require__(/*! ./mutation-test-report-mutant */ "./src/components/mutation-test-report-mutant.ts");
const style_1 = __webpack_require__(/*! ../style */ "./src/style/index.ts");
highlight_1.default.registerLanguage('javascript', javascript_1.default);
highlight_1.default.registerLanguage('typescript', typescript_1.default);
highlight_1.default.registerLanguage('cs', cs_1.default);
highlight_1.default.registerLanguage('java', java_1.default);
highlight_1.default.registerLanguage('scala', scala_1.default);
let MutationTestReportFileComponent = class MutationTestReportFileComponent extends lit_element_1.LitElement {
    constructor() {
        super(...arguments);
        this.openAll = () => {
            this.forEachMutantComponent(mutantComponent => mutantComponent.open = true);
        };
        this.collapseAll = () => {
            this.forEachMutantComponent(mutantComponent => mutantComponent.open = false);
        };
        this.filtersChanged = (event) => {
            const enabledMutantStates = event.detail
                .filter(mutantFilter => mutantFilter.enabled)
                .map(mutantFilter => mutantFilter.status);
            this.forEachMutantComponent(mutantComponent => {
                mutantComponent.show = enabledMutantStates.some(state => mutantComponent.status === state);
            });
        };
    }
    forEachMutantComponent(action) {
        for (const mutantComponent of this.root.querySelectorAll('mutation-test-report-mutant')) {
            if (mutantComponent instanceof mutation_test_report_mutant_1.MutationTestReportMutantComponent) {
                action(mutantComponent);
            }
        }
    }
    render() {
        return lit_element_1.html `
          <mutation-test-report-file-legend @filters-changed="${this.filtersChanged}" @open-all="${this.openAll}" @collapse-all="${this.collapseAll}"
            .mutants="${this.model.mutants}"></mutation-test-report-file-legend>
          <pre><code class="lang-${this.model.language} hljs" .innerHTML="${this.renderCode()}"></code></pre>
        `;
    }
    get root() {
        return this.shadowRoot || this;
    }
    renderCode() {
        const code = document.createElement('code');
        code.classList.add(`lang-${this.model.language}`);
        code.innerHTML = this.annotatedCode();
        highlight_1.default.highlightBlock(code);
        return code.innerHTML;
    }
    annotatedCode() {
        const lines = helpers_1.lines(this.model.source);
        const currentCursorMutantStatuses = {
            killed: 0,
            noCoverage: 0,
            survived: 0,
            timeout: 0
        };
        const adjustCurrentMutantResult = (valueToAdd) => (mutant) => {
            switch (mutant.status) {
                case "Killed" /* Killed */:
                    currentCursorMutantStatuses.killed += valueToAdd;
                    break;
                case "Survived" /* Survived */:
                    currentCursorMutantStatuses.survived += valueToAdd;
                    break;
                case "Timeout" /* Timeout */:
                    currentCursorMutantStatuses.timeout += valueToAdd;
                    break;
                case "NoCoverage" /* NoCoverage */:
                    currentCursorMutantStatuses.noCoverage += valueToAdd;
                    break;
            }
        };
        const determineBackground = () => {
            if (currentCursorMutantStatuses.survived > 0) {
                return helpers_1.getContextClassForStatus("Survived" /* Survived */) + '-light';
            }
            else if (currentCursorMutantStatuses.noCoverage > 0) {
                return helpers_1.getContextClassForStatus("NoCoverage" /* NoCoverage */) + '-light';
            }
            else if (currentCursorMutantStatuses.timeout > 0) {
                return helpers_1.getContextClassForStatus("Timeout" /* Timeout */) + '-light';
            }
            else if (currentCursorMutantStatuses.killed > 0) {
                return helpers_1.getContextClassForStatus("Killed" /* Killed */) + '-light';
            }
            return null;
        };
        const annotateCharacter = (char, line, column) => {
            const mutantsStarting = this.model.mutants.filter(m => m.location.start.line === line && m.location.start.column === column);
            const mutantsEnding = this.model.mutants.filter(m => m.location.end.line === line && m.location.end.column === column);
            mutantsStarting.forEach(adjustCurrentMutantResult(1));
            mutantsEnding.forEach(adjustCurrentMutantResult(-1));
            const isStart = line === helpers_1.LINE_START_INDEX && column === helpers_1.COLUMN_START_INDEX;
            const isEnd = line === lines.length + helpers_1.LINE_START_INDEX - 1 && column === lines[line - helpers_1.LINE_START_INDEX].length + helpers_1.COLUMN_START_INDEX - 1;
            const backgroundColorAnnotation = mutantsStarting.length || mutantsEnding.length || isStart ? `<span class="bg-${determineBackground()}">` : '';
            const backgroundColorEndAnnotation = ((mutantsStarting.length || mutantsEnding.length) && !isStart) || isEnd ? '</span>' : '';
            const mutantsAnnotations = mutantsStarting.map(m => `<mutation-test-report-mutant mutantId="${m.id}" mutatorName="${m.mutatorName}" status="${m.status}"><span slot="replacement">${m.replacement}</span><span slot="actual">`);
            const originalCodeEndAnnotations = mutantsEnding.map(() => `</span></mutation-test-report-mutant>`);
            return `${backgroundColorEndAnnotation}${originalCodeEndAnnotations.join('')}${mutantsAnnotations.join('')}${backgroundColorAnnotation}${helpers_1.escapeHtml(char)}`;
        };
        return walkString(this.model.source, annotateCharacter);
    }
};
MutationTestReportFileComponent.styles = [
    style_1.highlightJS,
    style_1.bootstrap,
    lit_element_1.css `
    .bg-danger-light {
      background-color: #f2dede;
    }
    .bg-success-light {
        background-color: #dff0d8;
    }
    .bg-warning-light {
        background-color: #fcf8e3;
    }
    `
];
__decorate([
    lit_element_1.property()
], MutationTestReportFileComponent.prototype, "model", void 0);
MutationTestReportFileComponent = __decorate([
    lit_element_1.customElement('mutation-test-report-file')
], MutationTestReportFileComponent);
exports.MutationTestReportFileComponent = MutationTestReportFileComponent;
/**
 * Walks a string. Executes a function on each character of the string (except for new lines and carriage returns)
 * @param source the string to walk
 * @param fn The function to execute on each character of the string
 */
function walkString(source, fn) {
    const results = [];
    let column = helpers_1.COLUMN_START_INDEX;
    let row = helpers_1.LINE_START_INDEX;
    for (let i = 0; i < source.length; i++) { // tslint:disable-line:prefer-for-of
        if (column === helpers_1.COLUMN_START_INDEX && source[i] === helpers_1.CARRIAGE_RETURN) {
            continue;
        }
        if (source[i] === helpers_1.NEW_LINE) {
            row++;
            column = helpers_1.COLUMN_START_INDEX;
            results.push(helpers_1.NEW_LINE);
            continue;
        }
        results.push(fn(source[i], row, column++));
    }
    return results.join('');
}


/***/ }),

/***/ "./src/components/mutation-test-report-mutant.ts":
/*!*******************************************************!*\
  !*** ./src/components/mutation-test-report-mutant.ts ***!
  \*******************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
const lit_element_1 = __webpack_require__(/*! lit-element */ "./node_modules/lit-element/lit-element.js");
const helpers_1 = __webpack_require__(/*! ../helpers */ "./src/helpers.ts");
const style_1 = __webpack_require__(/*! ../style */ "./src/style/index.ts");
let MutationTestReportMutantComponent = class MutationTestReportMutantComponent extends lit_element_1.LitElement {
    constructor() {
        super(...arguments);
        this.show = true;
        this.open = false;
    }
    render() {
        // This part is newline significant, as it is rendered in a <code> block.
        // No unnecessary new lines
        return lit_element_1.html `${this.renderButton()}${this.renderCode()}`;
    }
    renderButton() {
        if (this.show) {
            return lit_element_1.html `<span class="badge badge-${this.open ? 'info' : helpers_1.getContextClassForStatus(this.status)}" @click="${() => this.open = !this.open}"
  title="${this.mutatorName}">${this.mutantId}</button>`;
        }
        else {
            return undefined;
        }
    }
    renderCode() {
        return lit_element_1.html `${this.renderReplacement()}${this.renderActual()}`;
    }
    renderActual() {
        const actualCodeSlot = lit_element_1.html `<slot name="actual"></slot>`;
        return lit_element_1.html `<span class="${this.open && this.show ? 'disabled-code' : ''}">${actualCodeSlot}</span>`;
    }
    renderReplacement() {
        const replacementSlot = lit_element_1.html `<slot class="replacement" name="replacement"></slot>`;
        return lit_element_1.html `<span class="badge badge-info" ?hidden="${!this.open || !this.show}">${replacementSlot}</span>`;
    }
};
MutationTestReportMutantComponent.styles = [
    style_1.bootstrap,
    lit_element_1.css `
    .badge {
      cursor: pointer;
    }
    .disabled-code {
      text-decoration: line-through;
    }
    ::slotted(.hljs-string) {
      color: #fff;
    }
  `
];
__decorate([
    lit_element_1.property()
], MutationTestReportMutantComponent.prototype, "mutantId", void 0);
__decorate([
    lit_element_1.property()
], MutationTestReportMutantComponent.prototype, "mutatorName", void 0);
__decorate([
    lit_element_1.property()
], MutationTestReportMutantComponent.prototype, "status", void 0);
__decorate([
    lit_element_1.property()
], MutationTestReportMutantComponent.prototype, "show", void 0);
__decorate([
    lit_element_1.property()
], MutationTestReportMutantComponent.prototype, "open", void 0);
MutationTestReportMutantComponent = __decorate([
    lit_element_1.customElement('mutation-test-report-mutant')
], MutationTestReportMutantComponent);
exports.MutationTestReportMutantComponent = MutationTestReportMutantComponent;


/***/ }),

/***/ "./src/components/mutation-test-report-result.ts":
/*!*******************************************************!*\
  !*** ./src/components/mutation-test-report-result.ts ***!
  \*******************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
const lit_element_1 = __webpack_require__(/*! lit-element */ "./node_modules/lit-element/lit-element.js");
const helpers_1 = __webpack_require__(/*! ../helpers */ "./src/helpers.ts");
const style_1 = __webpack_require__(/*! ../style */ "./src/style/index.ts");
let MutationTestReportResultComponent = class MutationTestReportResultComponent extends lit_element_1.LitElement {
    render() {
        return lit_element_1.html `
    <div class='row'>
      <div class='totals col-sm-11'>
        <mutation-test-report-totals .currentPath="${this.currentPath}" .model="${this.model}"></mutation-test-report-totals>
      </div>
    </div>
    ${this.renderFileResult()}
    `;
    }
    renderFileResult() {
        if (helpers_1.isFileResult(this.model)) {
            return lit_element_1.html `
        <mutation-test-report-file .model="${this.model}"></mutation-test-report-file>
      `;
        }
        else {
            return lit_element_1.html ``;
        }
    }
};
MutationTestReportResultComponent.styles = [style_1.bootstrap];
__decorate([
    lit_element_1.property()
], MutationTestReportResultComponent.prototype, "model", void 0);
__decorate([
    lit_element_1.property()
], MutationTestReportResultComponent.prototype, "currentPath", void 0);
MutationTestReportResultComponent = __decorate([
    lit_element_1.customElement('mutation-test-report-result')
], MutationTestReportResultComponent);
exports.MutationTestReportResultComponent = MutationTestReportResultComponent;


/***/ }),

/***/ "./src/components/mutation-test-report-router.ts":
/*!*******************************************************!*\
  !*** ./src/components/mutation-test-report-router.ts ***!
  \*******************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
const lit_element_1 = __webpack_require__(/*! lit-element */ "./node_modules/lit-element/lit-element.js");
let MutationTestReportRouterComponent = class MutationTestReportRouterComponent extends lit_element_1.LitElement {
    constructor() {
        super(...arguments);
        this.updatePath = () => {
            const pathAsString = window.location.hash.substr(1);
            const path = pathAsString.length ? pathAsString.split('/') : [];
            this.dispatchEvent(new CustomEvent('path-changed', { detail: path }));
        };
    }
    connectedCallback() {
        super.connectedCallback();
        window.addEventListener('hashchange', this.updatePath);
        this.updatePath();
    }
};
MutationTestReportRouterComponent = __decorate([
    lit_element_1.customElement('mutation-test-report-router')
], MutationTestReportRouterComponent);
exports.MutationTestReportRouterComponent = MutationTestReportRouterComponent;


/***/ }),

/***/ "./src/components/mutation-test-report-totals.ts":
/*!*******************************************************!*\
  !*** ./src/components/mutation-test-report-totals.ts ***!
  \*******************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
const lit_element_1 = __webpack_require__(/*! lit-element */ "./node_modules/lit-element/lit-element.js");
const helpers_1 = __webpack_require__(/*! ../helpers */ "./src/helpers.ts");
const style_1 = __webpack_require__(/*! ../style */ "./src/style/index.ts");
let MutationTestReportTotalsComponent = class MutationTestReportTotalsComponent extends lit_element_1.LitElement {
    get childResults() {
        if (helpers_1.isDirectoryResult(this.model)) {
            return this.model.childResults;
        }
        else {
            return [];
        }
    }
    render() {
        return lit_element_1.html `
          <table class="table table-sm table-hover table-bordered table-no-top">
            ${this.renderHead()}
            ${this.renderBody()}
          </table>
      `;
    }
    renderHead() {
        return lit_element_1.html `<thead>
  <tr>
    <th style='width: 20%'>
      <div><span>File / Directory</span></div>
    </th>
    <th colspan='2'>
      <div><span>Mutation score</span></div>
    </th>
    ${this.renderTotalsColumns()}
  </tr>
</thead>`;
    }
    renderBody() {
        return lit_element_1.html `
    <tbody>
      ${this.renderRow(this.model, false)}
      ${this.childResults.map(child => lit_element_1.html `${this.renderRow(child, true)}`)}
    </tbody>`;
    }
    renderRow(subject, hyperlink) {
        const mutationScoreRounded = subject.mutationScore.toFixed(2);
        const coloringClass = this.determineColoringClass(subject);
        const style = `width: ${mutationScoreRounded}%`;
        return lit_element_1.html `
    <tr>
      <td>${hyperlink ? lit_element_1.html `<a href="${this.link(subject.name)}">${subject.name}</a>` : lit_element_1.html `<span>${subject.name}</span>`}</td>
      <td>
        <div class="progress">
          <div class="progress-bar bg-${coloringClass}" role="progressbar" aria-valuenow="${mutationScoreRounded}"
            aria-valuemin="0" aria-valuemax="100" .style="${style}">
            ${mutationScoreRounded}%
          </div>
        </div>
      </td>
      <th class="text-center text-${coloringClass}">${mutationScoreRounded}</th>
      ${Object.keys(this.model.totals).map(title => lit_element_1.html `<td class="text-center">${subject.totals[title]}</td>`)}
    </tr>
    `;
    }
    link(to) {
        if (this.currentPath && this.currentPath.length) {
            return `#${this.currentPath.join('/')}/${to}`;
        }
        else {
            return `#${to}`;
        }
    }
    renderTotalsColumns() {
        return lit_element_1.html `
        ${Object.keys(this.model.totals).map(title => lit_element_1.html `<th class='rotate text-center' style='width: 50px'>
          <div><span>${title}</span></div>
        </th>`)}
    `;
    }
    determineColoringClass(subject) {
        switch (subject.health) {
            case "Danger" /* Danger */:
                return 'danger';
            case "Good" /* Good */:
                return 'success';
            case "Warning" /* Warning */:
                return 'warning';
            default:
                return 'secondary';
        }
    }
};
MutationTestReportTotalsComponent.styles = [style_1.bootstrap,
    lit_element_1.css `
    th.rotate {
      /* Something you can count on */
      height: 50px;
      white-space: nowrap;
      padding-bottom: 10px;
    }

    th.rotate > div {
      transform:
      translate(27px, 0px)
      rotate(325deg);
      width: 30px;
    }

    .table-no-top>thead>tr>th {
      border-width: 0;
    }

    .table-no-top {
      border-width: 0;
    }
  `];
__decorate([
    lit_element_1.property()
], MutationTestReportTotalsComponent.prototype, "model", void 0);
__decorate([
    lit_element_1.property()
], MutationTestReportTotalsComponent.prototype, "currentPath", void 0);
__decorate([
    lit_element_1.property()
], MutationTestReportTotalsComponent.prototype, "childResults", null);
MutationTestReportTotalsComponent = __decorate([
    lit_element_1.customElement('mutation-test-report-totals')
], MutationTestReportTotalsComponent);
exports.MutationTestReportTotalsComponent = MutationTestReportTotalsComponent;


/***/ }),

/***/ "./src/helpers.ts":
/*!************************!*\
  !*** ./src/helpers.ts ***!
  \************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
function isDirectoryResult(result) {
    return Boolean(result && result.childResults);
}
exports.isDirectoryResult = isDirectoryResult;
function isFileResult(result) {
    return Boolean(result && result.mutants);
}
exports.isFileResult = isFileResult;
function getContextClassForStatus(status) {
    switch (status) {
        case "Killed" /* Killed */:
            return 'success';
        case "NoCoverage" /* NoCoverage */:
        case "Survived" /* Survived */:
            return 'danger';
        case "Timeout" /* Timeout */:
            return 'warning';
        case "RuntimeError" /* RuntimeError */:
        case "CompileError" /* CompileError */:
            return 'secondary';
    }
}
exports.getContextClassForStatus = getContextClassForStatus;
exports.COLUMN_START_INDEX = 1;
exports.LINE_START_INDEX = 1;
exports.NEW_LINE = '\n';
exports.CARRIAGE_RETURN = '\r';
function lines(content) {
    return content.split(exports.NEW_LINE).map(line => line.endsWith(exports.CARRIAGE_RETURN) ? line.substr(0, line.length - 1) : line);
}
exports.lines = lines;
function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}
exports.escapeHtml = escapeHtml;


/***/ }),

/***/ "./src/index.ts":
/*!**********************!*\
  !*** ./src/index.ts ***!
  \**********************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
__webpack_require__(/*! ./components/mutation-test-report-app */ "./src/components/mutation-test-report-app.ts");
__webpack_require__(/*! ./components/mutation-test-report-file */ "./src/components/mutation-test-report-file.ts");
__webpack_require__(/*! ./components/mutation-test-report-totals */ "./src/components/mutation-test-report-totals.ts");
__webpack_require__(/*! ./components/mutation-test-report-result */ "./src/components/mutation-test-report-result.ts");
__webpack_require__(/*! ./components/mutation-test-report-breadcrumb */ "./src/components/mutation-test-report-breadcrumb.ts");
__webpack_require__(/*! ./components/mutation-test-report-router */ "./src/components/mutation-test-report-router.ts");
__webpack_require__(/*! ./components/mutation-test-report-mutant */ "./src/components/mutation-test-report-mutant.ts");
__webpack_require__(/*! ./components/mutation-test-report-file-legend */ "./src/components/mutation-test-report-file-legend.ts");


/***/ }),

/***/ "./src/style/bootstrap.scss":
/*!**********************************!*\
  !*** ./src/style/bootstrap.scss ***!
  \**********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(/*! ../../node_modules/css-loader/dist/runtime/api.js */ "./node_modules/css-loader/dist/runtime/api.js")(false);
// Module
exports.push([module.i, "/*!\n * Bootstrap Reboot v4.3.1 (https://getbootstrap.com/)\n * Copyright 2011-2019 The Bootstrap Authors\n * Copyright 2011-2019 Twitter, Inc.\n * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)\n * Forked from Normalize.css, licensed MIT (https://github.com/necolas/normalize.css/blob/master/LICENSE.md)\n */\n*,\n*::before,\n*::after {\n  box-sizing: border-box; }\n\nhtml {\n  font-family: sans-serif;\n  line-height: 1.15;\n  -webkit-text-size-adjust: 100%;\n  -webkit-tap-highlight-color: rgba(0, 0, 0, 0); }\n\narticle, aside, figcaption, figure, footer, header, hgroup, main, nav, section {\n  display: block; }\n\nbody {\n  margin: 0;\n  font-family: -apple-system, BlinkMacSystemFont, \"Segoe UI\", Roboto, \"Helvetica Neue\", Arial, \"Noto Sans\", sans-serif, \"Apple Color Emoji\", \"Segoe UI Emoji\", \"Segoe UI Symbol\", \"Noto Color Emoji\";\n  font-size: 1rem;\n  font-weight: 400;\n  line-height: 1.5;\n  color: #212529;\n  text-align: left;\n  background-color: #fff; }\n\n[tabindex=\"-1\"]:focus {\n  outline: 0 !important; }\n\nhr {\n  box-sizing: content-box;\n  height: 0;\n  overflow: visible; }\n\nh1, h2, h3, h4, h5, h6 {\n  margin-top: 0;\n  margin-bottom: 0.5rem; }\n\np {\n  margin-top: 0;\n  margin-bottom: 1rem; }\n\nabbr[title],\nabbr[data-original-title] {\n  text-decoration: underline;\n  text-decoration: underline dotted;\n  cursor: help;\n  border-bottom: 0;\n  text-decoration-skip-ink: none; }\n\naddress {\n  margin-bottom: 1rem;\n  font-style: normal;\n  line-height: inherit; }\n\nol,\nul,\ndl {\n  margin-top: 0;\n  margin-bottom: 1rem; }\n\nol ol,\nul ul,\nol ul,\nul ol {\n  margin-bottom: 0; }\n\ndt {\n  font-weight: 700; }\n\ndd {\n  margin-bottom: .5rem;\n  margin-left: 0; }\n\nblockquote {\n  margin: 0 0 1rem; }\n\nb,\nstrong {\n  font-weight: bolder; }\n\nsmall {\n  font-size: 80%; }\n\nsub,\nsup {\n  position: relative;\n  font-size: 75%;\n  line-height: 0;\n  vertical-align: baseline; }\n\nsub {\n  bottom: -.25em; }\n\nsup {\n  top: -.5em; }\n\na {\n  color: #007bff;\n  text-decoration: none;\n  background-color: transparent; }\n  a:hover {\n    color: #0056b3;\n    text-decoration: underline; }\n\na:not([href]):not([tabindex]) {\n  color: inherit;\n  text-decoration: none; }\n  a:not([href]):not([tabindex]):hover, a:not([href]):not([tabindex]):focus {\n    color: inherit;\n    text-decoration: none; }\n  a:not([href]):not([tabindex]):focus {\n    outline: 0; }\n\npre,\ncode,\nkbd,\nsamp {\n  font-family: SFMono-Regular, Menlo, Monaco, Consolas, \"Liberation Mono\", \"Courier New\", monospace;\n  font-size: 1em; }\n\npre {\n  margin-top: 0;\n  margin-bottom: 1rem;\n  overflow: auto; }\n\nfigure {\n  margin: 0 0 1rem; }\n\nimg {\n  vertical-align: middle;\n  border-style: none; }\n\nsvg {\n  overflow: hidden;\n  vertical-align: middle; }\n\ntable {\n  border-collapse: collapse; }\n\ncaption {\n  padding-top: 0.75rem;\n  padding-bottom: 0.75rem;\n  color: #6c757d;\n  text-align: left;\n  caption-side: bottom; }\n\nth {\n  text-align: inherit; }\n\nlabel {\n  display: inline-block;\n  margin-bottom: 0.5rem; }\n\nbutton {\n  border-radius: 0; }\n\nbutton:focus {\n  outline: 1px dotted;\n  outline: 5px auto -webkit-focus-ring-color; }\n\ninput,\nbutton,\nselect,\noptgroup,\ntextarea {\n  margin: 0;\n  font-family: inherit;\n  font-size: inherit;\n  line-height: inherit; }\n\nbutton,\ninput {\n  overflow: visible; }\n\nbutton,\nselect {\n  text-transform: none; }\n\nselect {\n  word-wrap: normal; }\n\nbutton,\n[type=\"button\"],\n[type=\"reset\"],\n[type=\"submit\"] {\n  -webkit-appearance: button; }\n\nbutton:not(:disabled),\n[type=\"button\"]:not(:disabled),\n[type=\"reset\"]:not(:disabled),\n[type=\"submit\"]:not(:disabled) {\n  cursor: pointer; }\n\nbutton::-moz-focus-inner,\n[type=\"button\"]::-moz-focus-inner,\n[type=\"reset\"]::-moz-focus-inner,\n[type=\"submit\"]::-moz-focus-inner {\n  padding: 0;\n  border-style: none; }\n\ninput[type=\"radio\"],\ninput[type=\"checkbox\"] {\n  box-sizing: border-box;\n  padding: 0; }\n\ninput[type=\"date\"],\ninput[type=\"time\"],\ninput[type=\"datetime-local\"],\ninput[type=\"month\"] {\n  -webkit-appearance: listbox; }\n\ntextarea {\n  overflow: auto;\n  resize: vertical; }\n\nfieldset {\n  min-width: 0;\n  padding: 0;\n  margin: 0;\n  border: 0; }\n\nlegend {\n  display: block;\n  width: 100%;\n  max-width: 100%;\n  padding: 0;\n  margin-bottom: .5rem;\n  font-size: 1.5rem;\n  line-height: inherit;\n  color: inherit;\n  white-space: normal; }\n\nprogress {\n  vertical-align: baseline; }\n\n[type=\"number\"]::-webkit-inner-spin-button,\n[type=\"number\"]::-webkit-outer-spin-button {\n  height: auto; }\n\n[type=\"search\"] {\n  outline-offset: -2px;\n  -webkit-appearance: none; }\n\n[type=\"search\"]::-webkit-search-decoration {\n  -webkit-appearance: none; }\n\n::-webkit-file-upload-button {\n  font: inherit;\n  -webkit-appearance: button; }\n\noutput {\n  display: inline-block; }\n\nsummary {\n  display: list-item;\n  cursor: pointer; }\n\ntemplate {\n  display: none; }\n\n[hidden] {\n  display: none !important; }\n\n.container {\n  width: 100%;\n  padding-right: 15px;\n  padding-left: 15px;\n  margin-right: auto;\n  margin-left: auto; }\n  @media (min-width: 576px) {\n    .container {\n      max-width: 540px; } }\n  @media (min-width: 768px) {\n    .container {\n      max-width: 720px; } }\n  @media (min-width: 992px) {\n    .container {\n      max-width: 960px; } }\n  @media (min-width: 1200px) {\n    .container {\n      max-width: 1140px; } }\n\n.container-fluid {\n  width: 100%;\n  padding-right: 15px;\n  padding-left: 15px;\n  margin-right: auto;\n  margin-left: auto; }\n\n.row {\n  display: flex;\n  flex-wrap: wrap;\n  margin-right: -15px;\n  margin-left: -15px; }\n\n.no-gutters {\n  margin-right: 0;\n  margin-left: 0; }\n  .no-gutters > .col,\n  .no-gutters > [class*=\"col-\"] {\n    padding-right: 0;\n    padding-left: 0; }\n\n.col-1, .col-2, .col-3, .col-4, .col-5, .col-6, .col-7, .col-8, .col-9, .col-10, .col-11, .col-12, .col,\n.col-auto, .col-sm-1, .col-sm-2, .col-sm-3, .col-sm-4, .col-sm-5, .col-sm-6, .col-sm-7, .col-sm-8, .col-sm-9, .col-sm-10, .col-sm-11, .col-sm-12, .col-sm,\n.col-sm-auto, .col-md-1, .col-md-2, .col-md-3, .col-md-4, .col-md-5, .col-md-6, .col-md-7, .col-md-8, .col-md-9, .col-md-10, .col-md-11, .col-md-12, .col-md,\n.col-md-auto, .col-lg-1, .col-lg-2, .col-lg-3, .col-lg-4, .col-lg-5, .col-lg-6, .col-lg-7, .col-lg-8, .col-lg-9, .col-lg-10, .col-lg-11, .col-lg-12, .col-lg,\n.col-lg-auto, .col-xl-1, .col-xl-2, .col-xl-3, .col-xl-4, .col-xl-5, .col-xl-6, .col-xl-7, .col-xl-8, .col-xl-9, .col-xl-10, .col-xl-11, .col-xl-12, .col-xl,\n.col-xl-auto {\n  position: relative;\n  width: 100%;\n  padding-right: 15px;\n  padding-left: 15px; }\n\n.col {\n  flex-basis: 0;\n  flex-grow: 1;\n  max-width: 100%; }\n\n.col-auto {\n  flex: 0 0 auto;\n  width: auto;\n  max-width: 100%; }\n\n.col-1 {\n  flex: 0 0 8.33333%;\n  max-width: 8.33333%; }\n\n.col-2 {\n  flex: 0 0 16.66667%;\n  max-width: 16.66667%; }\n\n.col-3 {\n  flex: 0 0 25%;\n  max-width: 25%; }\n\n.col-4 {\n  flex: 0 0 33.33333%;\n  max-width: 33.33333%; }\n\n.col-5 {\n  flex: 0 0 41.66667%;\n  max-width: 41.66667%; }\n\n.col-6 {\n  flex: 0 0 50%;\n  max-width: 50%; }\n\n.col-7 {\n  flex: 0 0 58.33333%;\n  max-width: 58.33333%; }\n\n.col-8 {\n  flex: 0 0 66.66667%;\n  max-width: 66.66667%; }\n\n.col-9 {\n  flex: 0 0 75%;\n  max-width: 75%; }\n\n.col-10 {\n  flex: 0 0 83.33333%;\n  max-width: 83.33333%; }\n\n.col-11 {\n  flex: 0 0 91.66667%;\n  max-width: 91.66667%; }\n\n.col-12 {\n  flex: 0 0 100%;\n  max-width: 100%; }\n\n.order-first {\n  order: -1; }\n\n.order-last {\n  order: 13; }\n\n.order-0 {\n  order: 0; }\n\n.order-1 {\n  order: 1; }\n\n.order-2 {\n  order: 2; }\n\n.order-3 {\n  order: 3; }\n\n.order-4 {\n  order: 4; }\n\n.order-5 {\n  order: 5; }\n\n.order-6 {\n  order: 6; }\n\n.order-7 {\n  order: 7; }\n\n.order-8 {\n  order: 8; }\n\n.order-9 {\n  order: 9; }\n\n.order-10 {\n  order: 10; }\n\n.order-11 {\n  order: 11; }\n\n.order-12 {\n  order: 12; }\n\n.offset-1 {\n  margin-left: 8.33333%; }\n\n.offset-2 {\n  margin-left: 16.66667%; }\n\n.offset-3 {\n  margin-left: 25%; }\n\n.offset-4 {\n  margin-left: 33.33333%; }\n\n.offset-5 {\n  margin-left: 41.66667%; }\n\n.offset-6 {\n  margin-left: 50%; }\n\n.offset-7 {\n  margin-left: 58.33333%; }\n\n.offset-8 {\n  margin-left: 66.66667%; }\n\n.offset-9 {\n  margin-left: 75%; }\n\n.offset-10 {\n  margin-left: 83.33333%; }\n\n.offset-11 {\n  margin-left: 91.66667%; }\n\n@media (min-width: 576px) {\n  .col-sm {\n    flex-basis: 0;\n    flex-grow: 1;\n    max-width: 100%; }\n  .col-sm-auto {\n    flex: 0 0 auto;\n    width: auto;\n    max-width: 100%; }\n  .col-sm-1 {\n    flex: 0 0 8.33333%;\n    max-width: 8.33333%; }\n  .col-sm-2 {\n    flex: 0 0 16.66667%;\n    max-width: 16.66667%; }\n  .col-sm-3 {\n    flex: 0 0 25%;\n    max-width: 25%; }\n  .col-sm-4 {\n    flex: 0 0 33.33333%;\n    max-width: 33.33333%; }\n  .col-sm-5 {\n    flex: 0 0 41.66667%;\n    max-width: 41.66667%; }\n  .col-sm-6 {\n    flex: 0 0 50%;\n    max-width: 50%; }\n  .col-sm-7 {\n    flex: 0 0 58.33333%;\n    max-width: 58.33333%; }\n  .col-sm-8 {\n    flex: 0 0 66.66667%;\n    max-width: 66.66667%; }\n  .col-sm-9 {\n    flex: 0 0 75%;\n    max-width: 75%; }\n  .col-sm-10 {\n    flex: 0 0 83.33333%;\n    max-width: 83.33333%; }\n  .col-sm-11 {\n    flex: 0 0 91.66667%;\n    max-width: 91.66667%; }\n  .col-sm-12 {\n    flex: 0 0 100%;\n    max-width: 100%; }\n  .order-sm-first {\n    order: -1; }\n  .order-sm-last {\n    order: 13; }\n  .order-sm-0 {\n    order: 0; }\n  .order-sm-1 {\n    order: 1; }\n  .order-sm-2 {\n    order: 2; }\n  .order-sm-3 {\n    order: 3; }\n  .order-sm-4 {\n    order: 4; }\n  .order-sm-5 {\n    order: 5; }\n  .order-sm-6 {\n    order: 6; }\n  .order-sm-7 {\n    order: 7; }\n  .order-sm-8 {\n    order: 8; }\n  .order-sm-9 {\n    order: 9; }\n  .order-sm-10 {\n    order: 10; }\n  .order-sm-11 {\n    order: 11; }\n  .order-sm-12 {\n    order: 12; }\n  .offset-sm-0 {\n    margin-left: 0; }\n  .offset-sm-1 {\n    margin-left: 8.33333%; }\n  .offset-sm-2 {\n    margin-left: 16.66667%; }\n  .offset-sm-3 {\n    margin-left: 25%; }\n  .offset-sm-4 {\n    margin-left: 33.33333%; }\n  .offset-sm-5 {\n    margin-left: 41.66667%; }\n  .offset-sm-6 {\n    margin-left: 50%; }\n  .offset-sm-7 {\n    margin-left: 58.33333%; }\n  .offset-sm-8 {\n    margin-left: 66.66667%; }\n  .offset-sm-9 {\n    margin-left: 75%; }\n  .offset-sm-10 {\n    margin-left: 83.33333%; }\n  .offset-sm-11 {\n    margin-left: 91.66667%; } }\n\n@media (min-width: 768px) {\n  .col-md {\n    flex-basis: 0;\n    flex-grow: 1;\n    max-width: 100%; }\n  .col-md-auto {\n    flex: 0 0 auto;\n    width: auto;\n    max-width: 100%; }\n  .col-md-1 {\n    flex: 0 0 8.33333%;\n    max-width: 8.33333%; }\n  .col-md-2 {\n    flex: 0 0 16.66667%;\n    max-width: 16.66667%; }\n  .col-md-3 {\n    flex: 0 0 25%;\n    max-width: 25%; }\n  .col-md-4 {\n    flex: 0 0 33.33333%;\n    max-width: 33.33333%; }\n  .col-md-5 {\n    flex: 0 0 41.66667%;\n    max-width: 41.66667%; }\n  .col-md-6 {\n    flex: 0 0 50%;\n    max-width: 50%; }\n  .col-md-7 {\n    flex: 0 0 58.33333%;\n    max-width: 58.33333%; }\n  .col-md-8 {\n    flex: 0 0 66.66667%;\n    max-width: 66.66667%; }\n  .col-md-9 {\n    flex: 0 0 75%;\n    max-width: 75%; }\n  .col-md-10 {\n    flex: 0 0 83.33333%;\n    max-width: 83.33333%; }\n  .col-md-11 {\n    flex: 0 0 91.66667%;\n    max-width: 91.66667%; }\n  .col-md-12 {\n    flex: 0 0 100%;\n    max-width: 100%; }\n  .order-md-first {\n    order: -1; }\n  .order-md-last {\n    order: 13; }\n  .order-md-0 {\n    order: 0; }\n  .order-md-1 {\n    order: 1; }\n  .order-md-2 {\n    order: 2; }\n  .order-md-3 {\n    order: 3; }\n  .order-md-4 {\n    order: 4; }\n  .order-md-5 {\n    order: 5; }\n  .order-md-6 {\n    order: 6; }\n  .order-md-7 {\n    order: 7; }\n  .order-md-8 {\n    order: 8; }\n  .order-md-9 {\n    order: 9; }\n  .order-md-10 {\n    order: 10; }\n  .order-md-11 {\n    order: 11; }\n  .order-md-12 {\n    order: 12; }\n  .offset-md-0 {\n    margin-left: 0; }\n  .offset-md-1 {\n    margin-left: 8.33333%; }\n  .offset-md-2 {\n    margin-left: 16.66667%; }\n  .offset-md-3 {\n    margin-left: 25%; }\n  .offset-md-4 {\n    margin-left: 33.33333%; }\n  .offset-md-5 {\n    margin-left: 41.66667%; }\n  .offset-md-6 {\n    margin-left: 50%; }\n  .offset-md-7 {\n    margin-left: 58.33333%; }\n  .offset-md-8 {\n    margin-left: 66.66667%; }\n  .offset-md-9 {\n    margin-left: 75%; }\n  .offset-md-10 {\n    margin-left: 83.33333%; }\n  .offset-md-11 {\n    margin-left: 91.66667%; } }\n\n@media (min-width: 992px) {\n  .col-lg {\n    flex-basis: 0;\n    flex-grow: 1;\n    max-width: 100%; }\n  .col-lg-auto {\n    flex: 0 0 auto;\n    width: auto;\n    max-width: 100%; }\n  .col-lg-1 {\n    flex: 0 0 8.33333%;\n    max-width: 8.33333%; }\n  .col-lg-2 {\n    flex: 0 0 16.66667%;\n    max-width: 16.66667%; }\n  .col-lg-3 {\n    flex: 0 0 25%;\n    max-width: 25%; }\n  .col-lg-4 {\n    flex: 0 0 33.33333%;\n    max-width: 33.33333%; }\n  .col-lg-5 {\n    flex: 0 0 41.66667%;\n    max-width: 41.66667%; }\n  .col-lg-6 {\n    flex: 0 0 50%;\n    max-width: 50%; }\n  .col-lg-7 {\n    flex: 0 0 58.33333%;\n    max-width: 58.33333%; }\n  .col-lg-8 {\n    flex: 0 0 66.66667%;\n    max-width: 66.66667%; }\n  .col-lg-9 {\n    flex: 0 0 75%;\n    max-width: 75%; }\n  .col-lg-10 {\n    flex: 0 0 83.33333%;\n    max-width: 83.33333%; }\n  .col-lg-11 {\n    flex: 0 0 91.66667%;\n    max-width: 91.66667%; }\n  .col-lg-12 {\n    flex: 0 0 100%;\n    max-width: 100%; }\n  .order-lg-first {\n    order: -1; }\n  .order-lg-last {\n    order: 13; }\n  .order-lg-0 {\n    order: 0; }\n  .order-lg-1 {\n    order: 1; }\n  .order-lg-2 {\n    order: 2; }\n  .order-lg-3 {\n    order: 3; }\n  .order-lg-4 {\n    order: 4; }\n  .order-lg-5 {\n    order: 5; }\n  .order-lg-6 {\n    order: 6; }\n  .order-lg-7 {\n    order: 7; }\n  .order-lg-8 {\n    order: 8; }\n  .order-lg-9 {\n    order: 9; }\n  .order-lg-10 {\n    order: 10; }\n  .order-lg-11 {\n    order: 11; }\n  .order-lg-12 {\n    order: 12; }\n  .offset-lg-0 {\n    margin-left: 0; }\n  .offset-lg-1 {\n    margin-left: 8.33333%; }\n  .offset-lg-2 {\n    margin-left: 16.66667%; }\n  .offset-lg-3 {\n    margin-left: 25%; }\n  .offset-lg-4 {\n    margin-left: 33.33333%; }\n  .offset-lg-5 {\n    margin-left: 41.66667%; }\n  .offset-lg-6 {\n    margin-left: 50%; }\n  .offset-lg-7 {\n    margin-left: 58.33333%; }\n  .offset-lg-8 {\n    margin-left: 66.66667%; }\n  .offset-lg-9 {\n    margin-left: 75%; }\n  .offset-lg-10 {\n    margin-left: 83.33333%; }\n  .offset-lg-11 {\n    margin-left: 91.66667%; } }\n\n@media (min-width: 1200px) {\n  .col-xl {\n    flex-basis: 0;\n    flex-grow: 1;\n    max-width: 100%; }\n  .col-xl-auto {\n    flex: 0 0 auto;\n    width: auto;\n    max-width: 100%; }\n  .col-xl-1 {\n    flex: 0 0 8.33333%;\n    max-width: 8.33333%; }\n  .col-xl-2 {\n    flex: 0 0 16.66667%;\n    max-width: 16.66667%; }\n  .col-xl-3 {\n    flex: 0 0 25%;\n    max-width: 25%; }\n  .col-xl-4 {\n    flex: 0 0 33.33333%;\n    max-width: 33.33333%; }\n  .col-xl-5 {\n    flex: 0 0 41.66667%;\n    max-width: 41.66667%; }\n  .col-xl-6 {\n    flex: 0 0 50%;\n    max-width: 50%; }\n  .col-xl-7 {\n    flex: 0 0 58.33333%;\n    max-width: 58.33333%; }\n  .col-xl-8 {\n    flex: 0 0 66.66667%;\n    max-width: 66.66667%; }\n  .col-xl-9 {\n    flex: 0 0 75%;\n    max-width: 75%; }\n  .col-xl-10 {\n    flex: 0 0 83.33333%;\n    max-width: 83.33333%; }\n  .col-xl-11 {\n    flex: 0 0 91.66667%;\n    max-width: 91.66667%; }\n  .col-xl-12 {\n    flex: 0 0 100%;\n    max-width: 100%; }\n  .order-xl-first {\n    order: -1; }\n  .order-xl-last {\n    order: 13; }\n  .order-xl-0 {\n    order: 0; }\n  .order-xl-1 {\n    order: 1; }\n  .order-xl-2 {\n    order: 2; }\n  .order-xl-3 {\n    order: 3; }\n  .order-xl-4 {\n    order: 4; }\n  .order-xl-5 {\n    order: 5; }\n  .order-xl-6 {\n    order: 6; }\n  .order-xl-7 {\n    order: 7; }\n  .order-xl-8 {\n    order: 8; }\n  .order-xl-9 {\n    order: 9; }\n  .order-xl-10 {\n    order: 10; }\n  .order-xl-11 {\n    order: 11; }\n  .order-xl-12 {\n    order: 12; }\n  .offset-xl-0 {\n    margin-left: 0; }\n  .offset-xl-1 {\n    margin-left: 8.33333%; }\n  .offset-xl-2 {\n    margin-left: 16.66667%; }\n  .offset-xl-3 {\n    margin-left: 25%; }\n  .offset-xl-4 {\n    margin-left: 33.33333%; }\n  .offset-xl-5 {\n    margin-left: 41.66667%; }\n  .offset-xl-6 {\n    margin-left: 50%; }\n  .offset-xl-7 {\n    margin-left: 58.33333%; }\n  .offset-xl-8 {\n    margin-left: 66.66667%; }\n  .offset-xl-9 {\n    margin-left: 75%; }\n  .offset-xl-10 {\n    margin-left: 83.33333%; }\n  .offset-xl-11 {\n    margin-left: 91.66667%; } }\n\nh1, h2, h3, h4, h5, h6,\n.h1, .h2, .h3, .h4, .h5, .h6 {\n  margin-bottom: 0.5rem;\n  font-weight: 500;\n  line-height: 1.2; }\n\nh1, .h1 {\n  font-size: 2.5rem; }\n\nh2, .h2 {\n  font-size: 2rem; }\n\nh3, .h3 {\n  font-size: 1.75rem; }\n\nh4, .h4 {\n  font-size: 1.5rem; }\n\nh5, .h5 {\n  font-size: 1.25rem; }\n\nh6, .h6 {\n  font-size: 1rem; }\n\n.lead {\n  font-size: 1.25rem;\n  font-weight: 300; }\n\n.display-1 {\n  font-size: 6rem;\n  font-weight: 300;\n  line-height: 1.2; }\n\n.display-2 {\n  font-size: 5.5rem;\n  font-weight: 300;\n  line-height: 1.2; }\n\n.display-3 {\n  font-size: 4.5rem;\n  font-weight: 300;\n  line-height: 1.2; }\n\n.display-4 {\n  font-size: 3.5rem;\n  font-weight: 300;\n  line-height: 1.2; }\n\nhr {\n  margin-top: 1rem;\n  margin-bottom: 1rem;\n  border: 0;\n  border-top: 1px solid rgba(0, 0, 0, 0.1); }\n\nsmall,\n.small {\n  font-size: 80%;\n  font-weight: 400; }\n\nmark,\n.mark {\n  padding: 0.2em;\n  background-color: #fcf8e3; }\n\n.list-unstyled {\n  padding-left: 0;\n  list-style: none; }\n\n.list-inline {\n  padding-left: 0;\n  list-style: none; }\n\n.list-inline-item {\n  display: inline-block; }\n  .list-inline-item:not(:last-child) {\n    margin-right: 0.5rem; }\n\n.initialism {\n  font-size: 90%;\n  text-transform: uppercase; }\n\n.blockquote {\n  margin-bottom: 1rem;\n  font-size: 1.25rem; }\n\n.blockquote-footer {\n  display: block;\n  font-size: 80%;\n  color: #6c757d; }\n  .blockquote-footer::before {\n    content: \"\\2014\\00A0\"; }\n\n.breadcrumb {\n  display: flex;\n  flex-wrap: wrap;\n  padding: 0.75rem 1rem;\n  margin-bottom: 1rem;\n  list-style: none;\n  background-color: #e9ecef;\n  border-radius: 0.25rem; }\n\n.breadcrumb-item + .breadcrumb-item {\n  padding-left: 0.5rem; }\n  .breadcrumb-item + .breadcrumb-item::before {\n    display: inline-block;\n    padding-right: 0.5rem;\n    color: #6c757d;\n    content: \"/\"; }\n\n.breadcrumb-item + .breadcrumb-item:hover::before {\n  text-decoration: underline; }\n\n.breadcrumb-item + .breadcrumb-item:hover::before {\n  text-decoration: none; }\n\n.breadcrumb-item.active {\n  color: #6c757d; }\n\n.table {\n  width: 100%;\n  margin-bottom: 1rem;\n  color: #212529; }\n  .table th,\n  .table td {\n    padding: 0.75rem;\n    vertical-align: top;\n    border-top: 1px solid #dee2e6; }\n  .table thead th {\n    vertical-align: bottom;\n    border-bottom: 2px solid #dee2e6; }\n  .table tbody + tbody {\n    border-top: 2px solid #dee2e6; }\n\n.table-sm th,\n.table-sm td {\n  padding: 0.3rem; }\n\n.table-bordered {\n  border: 1px solid #dee2e6; }\n  .table-bordered th,\n  .table-bordered td {\n    border: 1px solid #dee2e6; }\n  .table-bordered thead th,\n  .table-bordered thead td {\n    border-bottom-width: 2px; }\n\n.table-borderless th,\n.table-borderless td,\n.table-borderless thead th,\n.table-borderless tbody + tbody {\n  border: 0; }\n\n.table-striped tbody tr:nth-of-type(odd) {\n  background-color: rgba(0, 0, 0, 0.05); }\n\n.table-hover tbody tr:hover {\n  color: #212529;\n  background-color: rgba(0, 0, 0, 0.075); }\n\n.table-primary,\n.table-primary > th,\n.table-primary > td {\n  background-color: #b8daff; }\n\n.table-primary th,\n.table-primary td,\n.table-primary thead th,\n.table-primary tbody + tbody {\n  border-color: #7abaff; }\n\n.table-hover .table-primary:hover {\n  background-color: #9fcdff; }\n  .table-hover .table-primary:hover > td,\n  .table-hover .table-primary:hover > th {\n    background-color: #9fcdff; }\n\n.table-secondary,\n.table-secondary > th,\n.table-secondary > td {\n  background-color: #d6d8db; }\n\n.table-secondary th,\n.table-secondary td,\n.table-secondary thead th,\n.table-secondary tbody + tbody {\n  border-color: #b3b7bb; }\n\n.table-hover .table-secondary:hover {\n  background-color: #c8cbcf; }\n  .table-hover .table-secondary:hover > td,\n  .table-hover .table-secondary:hover > th {\n    background-color: #c8cbcf; }\n\n.table-success,\n.table-success > th,\n.table-success > td {\n  background-color: #c3e6cb; }\n\n.table-success th,\n.table-success td,\n.table-success thead th,\n.table-success tbody + tbody {\n  border-color: #8fd19e; }\n\n.table-hover .table-success:hover {\n  background-color: #b1dfbb; }\n  .table-hover .table-success:hover > td,\n  .table-hover .table-success:hover > th {\n    background-color: #b1dfbb; }\n\n.table-info,\n.table-info > th,\n.table-info > td {\n  background-color: #bee5eb; }\n\n.table-info th,\n.table-info td,\n.table-info thead th,\n.table-info tbody + tbody {\n  border-color: #86cfda; }\n\n.table-hover .table-info:hover {\n  background-color: #abdde5; }\n  .table-hover .table-info:hover > td,\n  .table-hover .table-info:hover > th {\n    background-color: #abdde5; }\n\n.table-warning,\n.table-warning > th,\n.table-warning > td {\n  background-color: #ffeeba; }\n\n.table-warning th,\n.table-warning td,\n.table-warning thead th,\n.table-warning tbody + tbody {\n  border-color: #ffdf7e; }\n\n.table-hover .table-warning:hover {\n  background-color: #ffe8a1; }\n  .table-hover .table-warning:hover > td,\n  .table-hover .table-warning:hover > th {\n    background-color: #ffe8a1; }\n\n.table-danger,\n.table-danger > th,\n.table-danger > td {\n  background-color: #f5c6cb; }\n\n.table-danger th,\n.table-danger td,\n.table-danger thead th,\n.table-danger tbody + tbody {\n  border-color: #ed969e; }\n\n.table-hover .table-danger:hover {\n  background-color: #f1b0b7; }\n  .table-hover .table-danger:hover > td,\n  .table-hover .table-danger:hover > th {\n    background-color: #f1b0b7; }\n\n.table-light,\n.table-light > th,\n.table-light > td {\n  background-color: #fdfdfe; }\n\n.table-light th,\n.table-light td,\n.table-light thead th,\n.table-light tbody + tbody {\n  border-color: #fbfcfc; }\n\n.table-hover .table-light:hover {\n  background-color: #ececf6; }\n  .table-hover .table-light:hover > td,\n  .table-hover .table-light:hover > th {\n    background-color: #ececf6; }\n\n.table-dark,\n.table-dark > th,\n.table-dark > td {\n  background-color: #c6c8ca; }\n\n.table-dark th,\n.table-dark td,\n.table-dark thead th,\n.table-dark tbody + tbody {\n  border-color: #95999c; }\n\n.table-hover .table-dark:hover {\n  background-color: #b9bbbe; }\n  .table-hover .table-dark:hover > td,\n  .table-hover .table-dark:hover > th {\n    background-color: #b9bbbe; }\n\n.table-active,\n.table-active > th,\n.table-active > td {\n  background-color: rgba(0, 0, 0, 0.075); }\n\n.table-hover .table-active:hover {\n  background-color: rgba(0, 0, 0, 0.075); }\n  .table-hover .table-active:hover > td,\n  .table-hover .table-active:hover > th {\n    background-color: rgba(0, 0, 0, 0.075); }\n\n.table .thead-dark th {\n  color: #fff;\n  background-color: #343a40;\n  border-color: #454d55; }\n\n.table .thead-light th {\n  color: #495057;\n  background-color: #e9ecef;\n  border-color: #dee2e6; }\n\n.table-dark {\n  color: #fff;\n  background-color: #343a40; }\n  .table-dark th,\n  .table-dark td,\n  .table-dark thead th {\n    border-color: #454d55; }\n  .table-dark.table-bordered {\n    border: 0; }\n  .table-dark.table-striped tbody tr:nth-of-type(odd) {\n    background-color: rgba(255, 255, 255, 0.05); }\n  .table-dark.table-hover tbody tr:hover {\n    color: #fff;\n    background-color: rgba(255, 255, 255, 0.075); }\n\n@media (max-width: 575.98px) {\n  .table-responsive-sm {\n    display: block;\n    width: 100%;\n    overflow-x: auto;\n    -webkit-overflow-scrolling: touch; }\n    .table-responsive-sm > .table-bordered {\n      border: 0; } }\n\n@media (max-width: 767.98px) {\n  .table-responsive-md {\n    display: block;\n    width: 100%;\n    overflow-x: auto;\n    -webkit-overflow-scrolling: touch; }\n    .table-responsive-md > .table-bordered {\n      border: 0; } }\n\n@media (max-width: 991.98px) {\n  .table-responsive-lg {\n    display: block;\n    width: 100%;\n    overflow-x: auto;\n    -webkit-overflow-scrolling: touch; }\n    .table-responsive-lg > .table-bordered {\n      border: 0; } }\n\n@media (max-width: 1199.98px) {\n  .table-responsive-xl {\n    display: block;\n    width: 100%;\n    overflow-x: auto;\n    -webkit-overflow-scrolling: touch; }\n    .table-responsive-xl > .table-bordered {\n      border: 0; } }\n\n.table-responsive {\n  display: block;\n  width: 100%;\n  overflow-x: auto;\n  -webkit-overflow-scrolling: touch; }\n  .table-responsive > .table-bordered {\n    border: 0; }\n\n.badge {\n  display: inline-block;\n  padding: 0.25em 0.4em;\n  font-size: 75%;\n  font-weight: 700;\n  line-height: 1;\n  text-align: center;\n  white-space: nowrap;\n  vertical-align: baseline;\n  border-radius: 0.25rem;\n  transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out, border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out; }\n  @media (prefers-reduced-motion: reduce) {\n    .badge {\n      transition: none; } }\n  a.badge:hover, a.badge:focus {\n    text-decoration: none; }\n  .badge:empty {\n    display: none; }\n\n.btn .badge {\n  position: relative;\n  top: -1px; }\n\n.badge-pill {\n  padding-right: 0.6em;\n  padding-left: 0.6em;\n  border-radius: 10rem; }\n\n.badge-primary {\n  color: #fff;\n  background-color: #007bff; }\n  a.badge-primary:hover, a.badge-primary:focus {\n    color: #fff;\n    background-color: #0062cc; }\n  a.badge-primary:focus, a.badge-primary.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.5); }\n\n.badge-secondary {\n  color: #fff;\n  background-color: #6c757d; }\n  a.badge-secondary:hover, a.badge-secondary:focus {\n    color: #fff;\n    background-color: #545b62; }\n  a.badge-secondary:focus, a.badge-secondary.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(108, 117, 125, 0.5); }\n\n.badge-success {\n  color: #fff;\n  background-color: #28a745; }\n  a.badge-success:hover, a.badge-success:focus {\n    color: #fff;\n    background-color: #1e7e34; }\n  a.badge-success:focus, a.badge-success.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.5); }\n\n.badge-info {\n  color: #fff;\n  background-color: #17a2b8; }\n  a.badge-info:hover, a.badge-info:focus {\n    color: #fff;\n    background-color: #117a8b; }\n  a.badge-info:focus, a.badge-info.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(23, 162, 184, 0.5); }\n\n.badge-warning {\n  color: #212529;\n  background-color: #ffc107; }\n  a.badge-warning:hover, a.badge-warning:focus {\n    color: #212529;\n    background-color: #d39e00; }\n  a.badge-warning:focus, a.badge-warning.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(255, 193, 7, 0.5); }\n\n.badge-danger {\n  color: #fff;\n  background-color: #dc3545; }\n  a.badge-danger:hover, a.badge-danger:focus {\n    color: #fff;\n    background-color: #bd2130; }\n  a.badge-danger:focus, a.badge-danger.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.5); }\n\n.badge-light {\n  color: #212529;\n  background-color: #f8f9fa; }\n  a.badge-light:hover, a.badge-light:focus {\n    color: #212529;\n    background-color: #dae0e5; }\n  a.badge-light:focus, a.badge-light.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(248, 249, 250, 0.5); }\n\n.badge-dark {\n  color: #fff;\n  background-color: #343a40; }\n  a.badge-dark:hover, a.badge-dark:focus {\n    color: #fff;\n    background-color: #1d2124; }\n  a.badge-dark:focus, a.badge-dark.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(52, 58, 64, 0.5); }\n\n.btn {\n  display: inline-block;\n  font-weight: 400;\n  color: #212529;\n  text-align: center;\n  vertical-align: middle;\n  user-select: none;\n  background-color: transparent;\n  border: 1px solid transparent;\n  padding: 0.375rem 0.75rem;\n  font-size: 1rem;\n  line-height: 1.5;\n  border-radius: 0.25rem;\n  transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out, border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out; }\n  @media (prefers-reduced-motion: reduce) {\n    .btn {\n      transition: none; } }\n  .btn:hover {\n    color: #212529;\n    text-decoration: none; }\n  .btn:focus, .btn.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25); }\n  .btn.disabled, .btn:disabled {\n    opacity: 0.65; }\n\na.btn.disabled,\nfieldset:disabled a.btn {\n  pointer-events: none; }\n\n.btn-primary {\n  color: #fff;\n  background-color: #007bff;\n  border-color: #007bff; }\n  .btn-primary:hover {\n    color: #fff;\n    background-color: #0069d9;\n    border-color: #0062cc; }\n  .btn-primary:focus, .btn-primary.focus {\n    box-shadow: 0 0 0 0.2rem rgba(38, 143, 255, 0.5); }\n  .btn-primary.disabled, .btn-primary:disabled {\n    color: #fff;\n    background-color: #007bff;\n    border-color: #007bff; }\n  .btn-primary:not(:disabled):not(.disabled):active, .btn-primary:not(:disabled):not(.disabled).active,\n  .show > .btn-primary.dropdown-toggle {\n    color: #fff;\n    background-color: #0062cc;\n    border-color: #005cbf; }\n    .btn-primary:not(:disabled):not(.disabled):active:focus, .btn-primary:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-primary.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(38, 143, 255, 0.5); }\n\n.btn-secondary {\n  color: #fff;\n  background-color: #6c757d;\n  border-color: #6c757d; }\n  .btn-secondary:hover {\n    color: #fff;\n    background-color: #5a6268;\n    border-color: #545b62; }\n  .btn-secondary:focus, .btn-secondary.focus {\n    box-shadow: 0 0 0 0.2rem rgba(130, 138, 145, 0.5); }\n  .btn-secondary.disabled, .btn-secondary:disabled {\n    color: #fff;\n    background-color: #6c757d;\n    border-color: #6c757d; }\n  .btn-secondary:not(:disabled):not(.disabled):active, .btn-secondary:not(:disabled):not(.disabled).active,\n  .show > .btn-secondary.dropdown-toggle {\n    color: #fff;\n    background-color: #545b62;\n    border-color: #4e555b; }\n    .btn-secondary:not(:disabled):not(.disabled):active:focus, .btn-secondary:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-secondary.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(130, 138, 145, 0.5); }\n\n.btn-success {\n  color: #fff;\n  background-color: #28a745;\n  border-color: #28a745; }\n  .btn-success:hover {\n    color: #fff;\n    background-color: #218838;\n    border-color: #1e7e34; }\n  .btn-success:focus, .btn-success.focus {\n    box-shadow: 0 0 0 0.2rem rgba(72, 180, 97, 0.5); }\n  .btn-success.disabled, .btn-success:disabled {\n    color: #fff;\n    background-color: #28a745;\n    border-color: #28a745; }\n  .btn-success:not(:disabled):not(.disabled):active, .btn-success:not(:disabled):not(.disabled).active,\n  .show > .btn-success.dropdown-toggle {\n    color: #fff;\n    background-color: #1e7e34;\n    border-color: #1c7430; }\n    .btn-success:not(:disabled):not(.disabled):active:focus, .btn-success:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-success.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(72, 180, 97, 0.5); }\n\n.btn-info {\n  color: #fff;\n  background-color: #17a2b8;\n  border-color: #17a2b8; }\n  .btn-info:hover {\n    color: #fff;\n    background-color: #138496;\n    border-color: #117a8b; }\n  .btn-info:focus, .btn-info.focus {\n    box-shadow: 0 0 0 0.2rem rgba(58, 176, 195, 0.5); }\n  .btn-info.disabled, .btn-info:disabled {\n    color: #fff;\n    background-color: #17a2b8;\n    border-color: #17a2b8; }\n  .btn-info:not(:disabled):not(.disabled):active, .btn-info:not(:disabled):not(.disabled).active,\n  .show > .btn-info.dropdown-toggle {\n    color: #fff;\n    background-color: #117a8b;\n    border-color: #10707f; }\n    .btn-info:not(:disabled):not(.disabled):active:focus, .btn-info:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-info.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(58, 176, 195, 0.5); }\n\n.btn-warning {\n  color: #212529;\n  background-color: #ffc107;\n  border-color: #ffc107; }\n  .btn-warning:hover {\n    color: #212529;\n    background-color: #e0a800;\n    border-color: #d39e00; }\n  .btn-warning:focus, .btn-warning.focus {\n    box-shadow: 0 0 0 0.2rem rgba(222, 170, 12, 0.5); }\n  .btn-warning.disabled, .btn-warning:disabled {\n    color: #212529;\n    background-color: #ffc107;\n    border-color: #ffc107; }\n  .btn-warning:not(:disabled):not(.disabled):active, .btn-warning:not(:disabled):not(.disabled).active,\n  .show > .btn-warning.dropdown-toggle {\n    color: #212529;\n    background-color: #d39e00;\n    border-color: #c69500; }\n    .btn-warning:not(:disabled):not(.disabled):active:focus, .btn-warning:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-warning.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(222, 170, 12, 0.5); }\n\n.btn-danger {\n  color: #fff;\n  background-color: #dc3545;\n  border-color: #dc3545; }\n  .btn-danger:hover {\n    color: #fff;\n    background-color: #c82333;\n    border-color: #bd2130; }\n  .btn-danger:focus, .btn-danger.focus {\n    box-shadow: 0 0 0 0.2rem rgba(225, 83, 97, 0.5); }\n  .btn-danger.disabled, .btn-danger:disabled {\n    color: #fff;\n    background-color: #dc3545;\n    border-color: #dc3545; }\n  .btn-danger:not(:disabled):not(.disabled):active, .btn-danger:not(:disabled):not(.disabled).active,\n  .show > .btn-danger.dropdown-toggle {\n    color: #fff;\n    background-color: #bd2130;\n    border-color: #b21f2d; }\n    .btn-danger:not(:disabled):not(.disabled):active:focus, .btn-danger:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-danger.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(225, 83, 97, 0.5); }\n\n.btn-light {\n  color: #212529;\n  background-color: #f8f9fa;\n  border-color: #f8f9fa; }\n  .btn-light:hover {\n    color: #212529;\n    background-color: #e2e6ea;\n    border-color: #dae0e5; }\n  .btn-light:focus, .btn-light.focus {\n    box-shadow: 0 0 0 0.2rem rgba(216, 217, 219, 0.5); }\n  .btn-light.disabled, .btn-light:disabled {\n    color: #212529;\n    background-color: #f8f9fa;\n    border-color: #f8f9fa; }\n  .btn-light:not(:disabled):not(.disabled):active, .btn-light:not(:disabled):not(.disabled).active,\n  .show > .btn-light.dropdown-toggle {\n    color: #212529;\n    background-color: #dae0e5;\n    border-color: #d3d9df; }\n    .btn-light:not(:disabled):not(.disabled):active:focus, .btn-light:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-light.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(216, 217, 219, 0.5); }\n\n.btn-dark {\n  color: #fff;\n  background-color: #343a40;\n  border-color: #343a40; }\n  .btn-dark:hover {\n    color: #fff;\n    background-color: #23272b;\n    border-color: #1d2124; }\n  .btn-dark:focus, .btn-dark.focus {\n    box-shadow: 0 0 0 0.2rem rgba(82, 88, 93, 0.5); }\n  .btn-dark.disabled, .btn-dark:disabled {\n    color: #fff;\n    background-color: #343a40;\n    border-color: #343a40; }\n  .btn-dark:not(:disabled):not(.disabled):active, .btn-dark:not(:disabled):not(.disabled).active,\n  .show > .btn-dark.dropdown-toggle {\n    color: #fff;\n    background-color: #1d2124;\n    border-color: #171a1d; }\n    .btn-dark:not(:disabled):not(.disabled):active:focus, .btn-dark:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-dark.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(82, 88, 93, 0.5); }\n\n.btn-outline-primary {\n  color: #007bff;\n  border-color: #007bff; }\n  .btn-outline-primary:hover {\n    color: #fff;\n    background-color: #007bff;\n    border-color: #007bff; }\n  .btn-outline-primary:focus, .btn-outline-primary.focus {\n    box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.5); }\n  .btn-outline-primary.disabled, .btn-outline-primary:disabled {\n    color: #007bff;\n    background-color: transparent; }\n  .btn-outline-primary:not(:disabled):not(.disabled):active, .btn-outline-primary:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-primary.dropdown-toggle {\n    color: #fff;\n    background-color: #007bff;\n    border-color: #007bff; }\n    .btn-outline-primary:not(:disabled):not(.disabled):active:focus, .btn-outline-primary:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-primary.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.5); }\n\n.btn-outline-secondary {\n  color: #6c757d;\n  border-color: #6c757d; }\n  .btn-outline-secondary:hover {\n    color: #fff;\n    background-color: #6c757d;\n    border-color: #6c757d; }\n  .btn-outline-secondary:focus, .btn-outline-secondary.focus {\n    box-shadow: 0 0 0 0.2rem rgba(108, 117, 125, 0.5); }\n  .btn-outline-secondary.disabled, .btn-outline-secondary:disabled {\n    color: #6c757d;\n    background-color: transparent; }\n  .btn-outline-secondary:not(:disabled):not(.disabled):active, .btn-outline-secondary:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-secondary.dropdown-toggle {\n    color: #fff;\n    background-color: #6c757d;\n    border-color: #6c757d; }\n    .btn-outline-secondary:not(:disabled):not(.disabled):active:focus, .btn-outline-secondary:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-secondary.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(108, 117, 125, 0.5); }\n\n.btn-outline-success {\n  color: #28a745;\n  border-color: #28a745; }\n  .btn-outline-success:hover {\n    color: #fff;\n    background-color: #28a745;\n    border-color: #28a745; }\n  .btn-outline-success:focus, .btn-outline-success.focus {\n    box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.5); }\n  .btn-outline-success.disabled, .btn-outline-success:disabled {\n    color: #28a745;\n    background-color: transparent; }\n  .btn-outline-success:not(:disabled):not(.disabled):active, .btn-outline-success:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-success.dropdown-toggle {\n    color: #fff;\n    background-color: #28a745;\n    border-color: #28a745; }\n    .btn-outline-success:not(:disabled):not(.disabled):active:focus, .btn-outline-success:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-success.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.5); }\n\n.btn-outline-info {\n  color: #17a2b8;\n  border-color: #17a2b8; }\n  .btn-outline-info:hover {\n    color: #fff;\n    background-color: #17a2b8;\n    border-color: #17a2b8; }\n  .btn-outline-info:focus, .btn-outline-info.focus {\n    box-shadow: 0 0 0 0.2rem rgba(23, 162, 184, 0.5); }\n  .btn-outline-info.disabled, .btn-outline-info:disabled {\n    color: #17a2b8;\n    background-color: transparent; }\n  .btn-outline-info:not(:disabled):not(.disabled):active, .btn-outline-info:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-info.dropdown-toggle {\n    color: #fff;\n    background-color: #17a2b8;\n    border-color: #17a2b8; }\n    .btn-outline-info:not(:disabled):not(.disabled):active:focus, .btn-outline-info:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-info.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(23, 162, 184, 0.5); }\n\n.btn-outline-warning {\n  color: #ffc107;\n  border-color: #ffc107; }\n  .btn-outline-warning:hover {\n    color: #212529;\n    background-color: #ffc107;\n    border-color: #ffc107; }\n  .btn-outline-warning:focus, .btn-outline-warning.focus {\n    box-shadow: 0 0 0 0.2rem rgba(255, 193, 7, 0.5); }\n  .btn-outline-warning.disabled, .btn-outline-warning:disabled {\n    color: #ffc107;\n    background-color: transparent; }\n  .btn-outline-warning:not(:disabled):not(.disabled):active, .btn-outline-warning:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-warning.dropdown-toggle {\n    color: #212529;\n    background-color: #ffc107;\n    border-color: #ffc107; }\n    .btn-outline-warning:not(:disabled):not(.disabled):active:focus, .btn-outline-warning:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-warning.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(255, 193, 7, 0.5); }\n\n.btn-outline-danger {\n  color: #dc3545;\n  border-color: #dc3545; }\n  .btn-outline-danger:hover {\n    color: #fff;\n    background-color: #dc3545;\n    border-color: #dc3545; }\n  .btn-outline-danger:focus, .btn-outline-danger.focus {\n    box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.5); }\n  .btn-outline-danger.disabled, .btn-outline-danger:disabled {\n    color: #dc3545;\n    background-color: transparent; }\n  .btn-outline-danger:not(:disabled):not(.disabled):active, .btn-outline-danger:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-danger.dropdown-toggle {\n    color: #fff;\n    background-color: #dc3545;\n    border-color: #dc3545; }\n    .btn-outline-danger:not(:disabled):not(.disabled):active:focus, .btn-outline-danger:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-danger.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.5); }\n\n.btn-outline-light {\n  color: #f8f9fa;\n  border-color: #f8f9fa; }\n  .btn-outline-light:hover {\n    color: #212529;\n    background-color: #f8f9fa;\n    border-color: #f8f9fa; }\n  .btn-outline-light:focus, .btn-outline-light.focus {\n    box-shadow: 0 0 0 0.2rem rgba(248, 249, 250, 0.5); }\n  .btn-outline-light.disabled, .btn-outline-light:disabled {\n    color: #f8f9fa;\n    background-color: transparent; }\n  .btn-outline-light:not(:disabled):not(.disabled):active, .btn-outline-light:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-light.dropdown-toggle {\n    color: #212529;\n    background-color: #f8f9fa;\n    border-color: #f8f9fa; }\n    .btn-outline-light:not(:disabled):not(.disabled):active:focus, .btn-outline-light:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-light.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(248, 249, 250, 0.5); }\n\n.btn-outline-dark {\n  color: #343a40;\n  border-color: #343a40; }\n  .btn-outline-dark:hover {\n    color: #fff;\n    background-color: #343a40;\n    border-color: #343a40; }\n  .btn-outline-dark:focus, .btn-outline-dark.focus {\n    box-shadow: 0 0 0 0.2rem rgba(52, 58, 64, 0.5); }\n  .btn-outline-dark.disabled, .btn-outline-dark:disabled {\n    color: #343a40;\n    background-color: transparent; }\n  .btn-outline-dark:not(:disabled):not(.disabled):active, .btn-outline-dark:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-dark.dropdown-toggle {\n    color: #fff;\n    background-color: #343a40;\n    border-color: #343a40; }\n    .btn-outline-dark:not(:disabled):not(.disabled):active:focus, .btn-outline-dark:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-dark.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(52, 58, 64, 0.5); }\n\n.btn-link {\n  font-weight: 400;\n  color: #007bff;\n  text-decoration: none; }\n  .btn-link:hover {\n    color: #0056b3;\n    text-decoration: underline; }\n  .btn-link:focus, .btn-link.focus {\n    text-decoration: underline;\n    box-shadow: none; }\n  .btn-link:disabled, .btn-link.disabled {\n    color: #6c757d;\n    pointer-events: none; }\n\n.btn-lg {\n  padding: 0.5rem 1rem;\n  font-size: 1.25rem;\n  line-height: 1.5;\n  border-radius: 0.3rem; }\n\n.btn-sm {\n  padding: 0.25rem 0.5rem;\n  font-size: 0.875rem;\n  line-height: 1.5;\n  border-radius: 0.2rem; }\n\n.btn-block {\n  display: block;\n  width: 100%; }\n  .btn-block + .btn-block {\n    margin-top: 0.5rem; }\n\ninput[type=\"submit\"].btn-block,\ninput[type=\"reset\"].btn-block,\ninput[type=\"button\"].btn-block {\n  width: 100%; }\n\n.fade {\n  transition: opacity 0.15s linear; }\n  @media (prefers-reduced-motion: reduce) {\n    .fade {\n      transition: none; } }\n  .fade:not(.show) {\n    opacity: 0; }\n\n.collapse:not(.show) {\n  display: none; }\n\n.collapsing {\n  position: relative;\n  height: 0;\n  overflow: hidden;\n  transition: height 0.35s ease; }\n  @media (prefers-reduced-motion: reduce) {\n    .collapsing {\n      transition: none; } }\n\n.form-control {\n  display: block;\n  width: 100%;\n  height: calc(1.5em + 0.75rem + 2px);\n  padding: 0.375rem 0.75rem;\n  font-size: 1rem;\n  font-weight: 400;\n  line-height: 1.5;\n  color: #495057;\n  background-color: #fff;\n  background-clip: padding-box;\n  border: 1px solid #ced4da;\n  border-radius: 0.25rem;\n  transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out; }\n  @media (prefers-reduced-motion: reduce) {\n    .form-control {\n      transition: none; } }\n  .form-control::-ms-expand {\n    background-color: transparent;\n    border: 0; }\n  .form-control:focus {\n    color: #495057;\n    background-color: #fff;\n    border-color: #80bdff;\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25); }\n  .form-control::placeholder {\n    color: #6c757d;\n    opacity: 1; }\n  .form-control:disabled, .form-control[readonly] {\n    background-color: #e9ecef;\n    opacity: 1; }\n\nselect.form-control:focus::-ms-value {\n  color: #495057;\n  background-color: #fff; }\n\n.form-control-file,\n.form-control-range {\n  display: block;\n  width: 100%; }\n\n.col-form-label {\n  padding-top: calc(0.375rem + 1px);\n  padding-bottom: calc(0.375rem + 1px);\n  margin-bottom: 0;\n  font-size: inherit;\n  line-height: 1.5; }\n\n.col-form-label-lg {\n  padding-top: calc(0.5rem + 1px);\n  padding-bottom: calc(0.5rem + 1px);\n  font-size: 1.25rem;\n  line-height: 1.5; }\n\n.col-form-label-sm {\n  padding-top: calc(0.25rem + 1px);\n  padding-bottom: calc(0.25rem + 1px);\n  font-size: 0.875rem;\n  line-height: 1.5; }\n\n.form-control-plaintext {\n  display: block;\n  width: 100%;\n  padding-top: 0.375rem;\n  padding-bottom: 0.375rem;\n  margin-bottom: 0;\n  line-height: 1.5;\n  color: #212529;\n  background-color: transparent;\n  border: solid transparent;\n  border-width: 1px 0; }\n  .form-control-plaintext.form-control-sm, .form-control-plaintext.form-control-lg {\n    padding-right: 0;\n    padding-left: 0; }\n\n.form-control-sm {\n  height: calc(1.5em + 0.5rem + 2px);\n  padding: 0.25rem 0.5rem;\n  font-size: 0.875rem;\n  line-height: 1.5;\n  border-radius: 0.2rem; }\n\n.form-control-lg {\n  height: calc(1.5em + 1rem + 2px);\n  padding: 0.5rem 1rem;\n  font-size: 1.25rem;\n  line-height: 1.5;\n  border-radius: 0.3rem; }\n\nselect.form-control[size], select.form-control[multiple] {\n  height: auto; }\n\ntextarea.form-control {\n  height: auto; }\n\n.form-group {\n  margin-bottom: 1rem; }\n\n.form-text {\n  display: block;\n  margin-top: 0.25rem; }\n\n.form-row {\n  display: flex;\n  flex-wrap: wrap;\n  margin-right: -5px;\n  margin-left: -5px; }\n  .form-row > .col,\n  .form-row > [class*=\"col-\"] {\n    padding-right: 5px;\n    padding-left: 5px; }\n\n.form-check {\n  position: relative;\n  display: block;\n  padding-left: 1.25rem; }\n\n.form-check-input {\n  position: absolute;\n  margin-top: 0.3rem;\n  margin-left: -1.25rem; }\n  .form-check-input:disabled ~ .form-check-label {\n    color: #6c757d; }\n\n.form-check-label {\n  margin-bottom: 0; }\n\n.form-check-inline {\n  display: inline-flex;\n  align-items: center;\n  padding-left: 0;\n  margin-right: 0.75rem; }\n  .form-check-inline .form-check-input {\n    position: static;\n    margin-top: 0;\n    margin-right: 0.3125rem;\n    margin-left: 0; }\n\n.valid-feedback {\n  display: none;\n  width: 100%;\n  margin-top: 0.25rem;\n  font-size: 80%;\n  color: #28a745; }\n\n.valid-tooltip {\n  position: absolute;\n  top: 100%;\n  z-index: 5;\n  display: none;\n  max-width: 100%;\n  padding: 0.25rem 0.5rem;\n  margin-top: .1rem;\n  font-size: 0.875rem;\n  line-height: 1.5;\n  color: #fff;\n  background-color: rgba(40, 167, 69, 0.9);\n  border-radius: 0.25rem; }\n\n.was-validated .form-control:valid, .form-control.is-valid {\n  border-color: #28a745;\n  padding-right: calc(1.5em + 0.75rem);\n  background-image: url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 8 8'%3e%3cpath fill='%2328a745' d='M2.3 6.73L.6 4.53c-.4-1.04.46-1.4 1.1-.8l1.1 1.4 3.4-3.8c.6-.63 1.6-.27 1.2.7l-4 4.6c-.43.5-.8.4-1.1.1z'/%3e%3c/svg%3e\");\n  background-repeat: no-repeat;\n  background-position: center right calc(0.375em + 0.1875rem);\n  background-size: calc(0.75em + 0.375rem) calc(0.75em + 0.375rem); }\n  .was-validated .form-control:valid:focus, .form-control.is-valid:focus {\n    border-color: #28a745;\n    box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.25); }\n  .was-validated .form-control:valid ~ .valid-feedback,\n  .was-validated .form-control:valid ~ .valid-tooltip, .form-control.is-valid ~ .valid-feedback,\n  .form-control.is-valid ~ .valid-tooltip {\n    display: block; }\n\n.was-validated textarea.form-control:valid, textarea.form-control.is-valid {\n  padding-right: calc(1.5em + 0.75rem);\n  background-position: top calc(0.375em + 0.1875rem) right calc(0.375em + 0.1875rem); }\n\n.was-validated .custom-select:valid, .custom-select.is-valid {\n  border-color: #28a745;\n  padding-right: calc((1em + 0.75rem) * 3 / 4 + 1.75rem);\n  background: url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 4 5'%3e%3cpath fill='%23343a40' d='M2 0L0 2h4zm0 5L0 3h4z'/%3e%3c/svg%3e\") no-repeat right 0.75rem center/8px 10px, url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 8 8'%3e%3cpath fill='%2328a745' d='M2.3 6.73L.6 4.53c-.4-1.04.46-1.4 1.1-.8l1.1 1.4 3.4-3.8c.6-.63 1.6-.27 1.2.7l-4 4.6c-.43.5-.8.4-1.1.1z'/%3e%3c/svg%3e\") #fff no-repeat center right 1.75rem/calc(0.75em + 0.375rem) calc(0.75em + 0.375rem); }\n  .was-validated .custom-select:valid:focus, .custom-select.is-valid:focus {\n    border-color: #28a745;\n    box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.25); }\n  .was-validated .custom-select:valid ~ .valid-feedback,\n  .was-validated .custom-select:valid ~ .valid-tooltip, .custom-select.is-valid ~ .valid-feedback,\n  .custom-select.is-valid ~ .valid-tooltip {\n    display: block; }\n\n.was-validated .form-control-file:valid ~ .valid-feedback,\n.was-validated .form-control-file:valid ~ .valid-tooltip, .form-control-file.is-valid ~ .valid-feedback,\n.form-control-file.is-valid ~ .valid-tooltip {\n  display: block; }\n\n.was-validated .form-check-input:valid ~ .form-check-label, .form-check-input.is-valid ~ .form-check-label {\n  color: #28a745; }\n\n.was-validated .form-check-input:valid ~ .valid-feedback,\n.was-validated .form-check-input:valid ~ .valid-tooltip, .form-check-input.is-valid ~ .valid-feedback,\n.form-check-input.is-valid ~ .valid-tooltip {\n  display: block; }\n\n.was-validated .custom-control-input:valid ~ .custom-control-label, .custom-control-input.is-valid ~ .custom-control-label {\n  color: #28a745; }\n  .was-validated .custom-control-input:valid ~ .custom-control-label::before, .custom-control-input.is-valid ~ .custom-control-label::before {\n    border-color: #28a745; }\n\n.was-validated .custom-control-input:valid ~ .valid-feedback,\n.was-validated .custom-control-input:valid ~ .valid-tooltip, .custom-control-input.is-valid ~ .valid-feedback,\n.custom-control-input.is-valid ~ .valid-tooltip {\n  display: block; }\n\n.was-validated .custom-control-input:valid:checked ~ .custom-control-label::before, .custom-control-input.is-valid:checked ~ .custom-control-label::before {\n  border-color: #34ce57;\n  background-color: #34ce57; }\n\n.was-validated .custom-control-input:valid:focus ~ .custom-control-label::before, .custom-control-input.is-valid:focus ~ .custom-control-label::before {\n  box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.25); }\n\n.was-validated .custom-control-input:valid:focus:not(:checked) ~ .custom-control-label::before, .custom-control-input.is-valid:focus:not(:checked) ~ .custom-control-label::before {\n  border-color: #28a745; }\n\n.was-validated .custom-file-input:valid ~ .custom-file-label, .custom-file-input.is-valid ~ .custom-file-label {\n  border-color: #28a745; }\n\n.was-validated .custom-file-input:valid ~ .valid-feedback,\n.was-validated .custom-file-input:valid ~ .valid-tooltip, .custom-file-input.is-valid ~ .valid-feedback,\n.custom-file-input.is-valid ~ .valid-tooltip {\n  display: block; }\n\n.was-validated .custom-file-input:valid:focus ~ .custom-file-label, .custom-file-input.is-valid:focus ~ .custom-file-label {\n  border-color: #28a745;\n  box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.25); }\n\n.invalid-feedback {\n  display: none;\n  width: 100%;\n  margin-top: 0.25rem;\n  font-size: 80%;\n  color: #dc3545; }\n\n.invalid-tooltip {\n  position: absolute;\n  top: 100%;\n  z-index: 5;\n  display: none;\n  max-width: 100%;\n  padding: 0.25rem 0.5rem;\n  margin-top: .1rem;\n  font-size: 0.875rem;\n  line-height: 1.5;\n  color: #fff;\n  background-color: rgba(220, 53, 69, 0.9);\n  border-radius: 0.25rem; }\n\n.was-validated .form-control:invalid, .form-control.is-invalid {\n  border-color: #dc3545;\n  padding-right: calc(1.5em + 0.75rem);\n  background-image: url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='%23dc3545' viewBox='-2 -2 7 7'%3e%3cpath stroke='%23dc3545' d='M0 0l3 3m0-3L0 3'/%3e%3ccircle r='.5'/%3e%3ccircle cx='3' r='.5'/%3e%3ccircle cy='3' r='.5'/%3e%3ccircle cx='3' cy='3' r='.5'/%3e%3c/svg%3E\");\n  background-repeat: no-repeat;\n  background-position: center right calc(0.375em + 0.1875rem);\n  background-size: calc(0.75em + 0.375rem) calc(0.75em + 0.375rem); }\n  .was-validated .form-control:invalid:focus, .form-control.is-invalid:focus {\n    border-color: #dc3545;\n    box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25); }\n  .was-validated .form-control:invalid ~ .invalid-feedback,\n  .was-validated .form-control:invalid ~ .invalid-tooltip, .form-control.is-invalid ~ .invalid-feedback,\n  .form-control.is-invalid ~ .invalid-tooltip {\n    display: block; }\n\n.was-validated textarea.form-control:invalid, textarea.form-control.is-invalid {\n  padding-right: calc(1.5em + 0.75rem);\n  background-position: top calc(0.375em + 0.1875rem) right calc(0.375em + 0.1875rem); }\n\n.was-validated .custom-select:invalid, .custom-select.is-invalid {\n  border-color: #dc3545;\n  padding-right: calc((1em + 0.75rem) * 3 / 4 + 1.75rem);\n  background: url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 4 5'%3e%3cpath fill='%23343a40' d='M2 0L0 2h4zm0 5L0 3h4z'/%3e%3c/svg%3e\") no-repeat right 0.75rem center/8px 10px, url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='%23dc3545' viewBox='-2 -2 7 7'%3e%3cpath stroke='%23dc3545' d='M0 0l3 3m0-3L0 3'/%3e%3ccircle r='.5'/%3e%3ccircle cx='3' r='.5'/%3e%3ccircle cy='3' r='.5'/%3e%3ccircle cx='3' cy='3' r='.5'/%3e%3c/svg%3E\") #fff no-repeat center right 1.75rem/calc(0.75em + 0.375rem) calc(0.75em + 0.375rem); }\n  .was-validated .custom-select:invalid:focus, .custom-select.is-invalid:focus {\n    border-color: #dc3545;\n    box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25); }\n  .was-validated .custom-select:invalid ~ .invalid-feedback,\n  .was-validated .custom-select:invalid ~ .invalid-tooltip, .custom-select.is-invalid ~ .invalid-feedback,\n  .custom-select.is-invalid ~ .invalid-tooltip {\n    display: block; }\n\n.was-validated .form-control-file:invalid ~ .invalid-feedback,\n.was-validated .form-control-file:invalid ~ .invalid-tooltip, .form-control-file.is-invalid ~ .invalid-feedback,\n.form-control-file.is-invalid ~ .invalid-tooltip {\n  display: block; }\n\n.was-validated .form-check-input:invalid ~ .form-check-label, .form-check-input.is-invalid ~ .form-check-label {\n  color: #dc3545; }\n\n.was-validated .form-check-input:invalid ~ .invalid-feedback,\n.was-validated .form-check-input:invalid ~ .invalid-tooltip, .form-check-input.is-invalid ~ .invalid-feedback,\n.form-check-input.is-invalid ~ .invalid-tooltip {\n  display: block; }\n\n.was-validated .custom-control-input:invalid ~ .custom-control-label, .custom-control-input.is-invalid ~ .custom-control-label {\n  color: #dc3545; }\n  .was-validated .custom-control-input:invalid ~ .custom-control-label::before, .custom-control-input.is-invalid ~ .custom-control-label::before {\n    border-color: #dc3545; }\n\n.was-validated .custom-control-input:invalid ~ .invalid-feedback,\n.was-validated .custom-control-input:invalid ~ .invalid-tooltip, .custom-control-input.is-invalid ~ .invalid-feedback,\n.custom-control-input.is-invalid ~ .invalid-tooltip {\n  display: block; }\n\n.was-validated .custom-control-input:invalid:checked ~ .custom-control-label::before, .custom-control-input.is-invalid:checked ~ .custom-control-label::before {\n  border-color: #e4606d;\n  background-color: #e4606d; }\n\n.was-validated .custom-control-input:invalid:focus ~ .custom-control-label::before, .custom-control-input.is-invalid:focus ~ .custom-control-label::before {\n  box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25); }\n\n.was-validated .custom-control-input:invalid:focus:not(:checked) ~ .custom-control-label::before, .custom-control-input.is-invalid:focus:not(:checked) ~ .custom-control-label::before {\n  border-color: #dc3545; }\n\n.was-validated .custom-file-input:invalid ~ .custom-file-label, .custom-file-input.is-invalid ~ .custom-file-label {\n  border-color: #dc3545; }\n\n.was-validated .custom-file-input:invalid ~ .invalid-feedback,\n.was-validated .custom-file-input:invalid ~ .invalid-tooltip, .custom-file-input.is-invalid ~ .invalid-feedback,\n.custom-file-input.is-invalid ~ .invalid-tooltip {\n  display: block; }\n\n.was-validated .custom-file-input:invalid:focus ~ .custom-file-label, .custom-file-input.is-invalid:focus ~ .custom-file-label {\n  border-color: #dc3545;\n  box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25); }\n\n.form-inline {\n  display: flex;\n  flex-flow: row wrap;\n  align-items: center; }\n  .form-inline .form-check {\n    width: 100%; }\n  @media (min-width: 576px) {\n    .form-inline label {\n      display: flex;\n      align-items: center;\n      justify-content: center;\n      margin-bottom: 0; }\n    .form-inline .form-group {\n      display: flex;\n      flex: 0 0 auto;\n      flex-flow: row wrap;\n      align-items: center;\n      margin-bottom: 0; }\n    .form-inline .form-control {\n      display: inline-block;\n      width: auto;\n      vertical-align: middle; }\n    .form-inline .form-control-plaintext {\n      display: inline-block; }\n    .form-inline .input-group,\n    .form-inline .custom-select {\n      width: auto; }\n    .form-inline .form-check {\n      display: flex;\n      align-items: center;\n      justify-content: center;\n      width: auto;\n      padding-left: 0; }\n    .form-inline .form-check-input {\n      position: relative;\n      flex-shrink: 0;\n      margin-top: 0;\n      margin-right: 0.25rem;\n      margin-left: 0; }\n    .form-inline .custom-control {\n      align-items: center;\n      justify-content: center; }\n    .form-inline .custom-control-label {\n      margin-bottom: 0; } }\n\n@keyframes progress-bar-stripes {\n  from {\n    background-position: 1rem 0; }\n  to {\n    background-position: 0 0; } }\n\n.progress {\n  display: flex;\n  height: 1rem;\n  overflow: hidden;\n  font-size: 0.75rem;\n  background-color: #e9ecef;\n  border-radius: 0.25rem; }\n\n.progress-bar {\n  display: flex;\n  flex-direction: column;\n  justify-content: center;\n  color: #fff;\n  text-align: center;\n  white-space: nowrap;\n  background-color: #007bff;\n  transition: width 0.6s ease; }\n  @media (prefers-reduced-motion: reduce) {\n    .progress-bar {\n      transition: none; } }\n\n.progress-bar-striped {\n  background-image: linear-gradient(45deg, rgba(255, 255, 255, 0.15) 25%, transparent 25%, transparent 50%, rgba(255, 255, 255, 0.15) 50%, rgba(255, 255, 255, 0.15) 75%, transparent 75%, transparent);\n  background-size: 1rem 1rem; }\n\n.progress-bar-animated {\n  animation: progress-bar-stripes 1s linear infinite; }\n  @media (prefers-reduced-motion: reduce) {\n    .progress-bar-animated {\n      animation: none; } }\n\n.d-none {\n  display: none !important; }\n\n.d-inline {\n  display: inline !important; }\n\n.d-inline-block {\n  display: inline-block !important; }\n\n.d-block {\n  display: block !important; }\n\n.d-table {\n  display: table !important; }\n\n.d-table-row {\n  display: table-row !important; }\n\n.d-table-cell {\n  display: table-cell !important; }\n\n.d-flex {\n  display: flex !important; }\n\n.d-inline-flex {\n  display: inline-flex !important; }\n\n@media (min-width: 576px) {\n  .d-sm-none {\n    display: none !important; }\n  .d-sm-inline {\n    display: inline !important; }\n  .d-sm-inline-block {\n    display: inline-block !important; }\n  .d-sm-block {\n    display: block !important; }\n  .d-sm-table {\n    display: table !important; }\n  .d-sm-table-row {\n    display: table-row !important; }\n  .d-sm-table-cell {\n    display: table-cell !important; }\n  .d-sm-flex {\n    display: flex !important; }\n  .d-sm-inline-flex {\n    display: inline-flex !important; } }\n\n@media (min-width: 768px) {\n  .d-md-none {\n    display: none !important; }\n  .d-md-inline {\n    display: inline !important; }\n  .d-md-inline-block {\n    display: inline-block !important; }\n  .d-md-block {\n    display: block !important; }\n  .d-md-table {\n    display: table !important; }\n  .d-md-table-row {\n    display: table-row !important; }\n  .d-md-table-cell {\n    display: table-cell !important; }\n  .d-md-flex {\n    display: flex !important; }\n  .d-md-inline-flex {\n    display: inline-flex !important; } }\n\n@media (min-width: 992px) {\n  .d-lg-none {\n    display: none !important; }\n  .d-lg-inline {\n    display: inline !important; }\n  .d-lg-inline-block {\n    display: inline-block !important; }\n  .d-lg-block {\n    display: block !important; }\n  .d-lg-table {\n    display: table !important; }\n  .d-lg-table-row {\n    display: table-row !important; }\n  .d-lg-table-cell {\n    display: table-cell !important; }\n  .d-lg-flex {\n    display: flex !important; }\n  .d-lg-inline-flex {\n    display: inline-flex !important; } }\n\n@media (min-width: 1200px) {\n  .d-xl-none {\n    display: none !important; }\n  .d-xl-inline {\n    display: inline !important; }\n  .d-xl-inline-block {\n    display: inline-block !important; }\n  .d-xl-block {\n    display: block !important; }\n  .d-xl-table {\n    display: table !important; }\n  .d-xl-table-row {\n    display: table-row !important; }\n  .d-xl-table-cell {\n    display: table-cell !important; }\n  .d-xl-flex {\n    display: flex !important; }\n  .d-xl-inline-flex {\n    display: inline-flex !important; } }\n\n@media print {\n  .d-print-none {\n    display: none !important; }\n  .d-print-inline {\n    display: inline !important; }\n  .d-print-inline-block {\n    display: inline-block !important; }\n  .d-print-block {\n    display: block !important; }\n  .d-print-table {\n    display: table !important; }\n  .d-print-table-row {\n    display: table-row !important; }\n  .d-print-table-cell {\n    display: table-cell !important; }\n  .d-print-flex {\n    display: flex !important; }\n  .d-print-inline-flex {\n    display: inline-flex !important; } }\n\n.flex-row {\n  flex-direction: row !important; }\n\n.flex-column {\n  flex-direction: column !important; }\n\n.flex-row-reverse {\n  flex-direction: row-reverse !important; }\n\n.flex-column-reverse {\n  flex-direction: column-reverse !important; }\n\n.flex-wrap {\n  flex-wrap: wrap !important; }\n\n.flex-nowrap {\n  flex-wrap: nowrap !important; }\n\n.flex-wrap-reverse {\n  flex-wrap: wrap-reverse !important; }\n\n.flex-fill {\n  flex: 1 1 auto !important; }\n\n.flex-grow-0 {\n  flex-grow: 0 !important; }\n\n.flex-grow-1 {\n  flex-grow: 1 !important; }\n\n.flex-shrink-0 {\n  flex-shrink: 0 !important; }\n\n.flex-shrink-1 {\n  flex-shrink: 1 !important; }\n\n.justify-content-start {\n  justify-content: flex-start !important; }\n\n.justify-content-end {\n  justify-content: flex-end !important; }\n\n.justify-content-center {\n  justify-content: center !important; }\n\n.justify-content-between {\n  justify-content: space-between !important; }\n\n.justify-content-around {\n  justify-content: space-around !important; }\n\n.align-items-start {\n  align-items: flex-start !important; }\n\n.align-items-end {\n  align-items: flex-end !important; }\n\n.align-items-center {\n  align-items: center !important; }\n\n.align-items-baseline {\n  align-items: baseline !important; }\n\n.align-items-stretch {\n  align-items: stretch !important; }\n\n.align-content-start {\n  align-content: flex-start !important; }\n\n.align-content-end {\n  align-content: flex-end !important; }\n\n.align-content-center {\n  align-content: center !important; }\n\n.align-content-between {\n  align-content: space-between !important; }\n\n.align-content-around {\n  align-content: space-around !important; }\n\n.align-content-stretch {\n  align-content: stretch !important; }\n\n.align-self-auto {\n  align-self: auto !important; }\n\n.align-self-start {\n  align-self: flex-start !important; }\n\n.align-self-end {\n  align-self: flex-end !important; }\n\n.align-self-center {\n  align-self: center !important; }\n\n.align-self-baseline {\n  align-self: baseline !important; }\n\n.align-self-stretch {\n  align-self: stretch !important; }\n\n@media (min-width: 576px) {\n  .flex-sm-row {\n    flex-direction: row !important; }\n  .flex-sm-column {\n    flex-direction: column !important; }\n  .flex-sm-row-reverse {\n    flex-direction: row-reverse !important; }\n  .flex-sm-column-reverse {\n    flex-direction: column-reverse !important; }\n  .flex-sm-wrap {\n    flex-wrap: wrap !important; }\n  .flex-sm-nowrap {\n    flex-wrap: nowrap !important; }\n  .flex-sm-wrap-reverse {\n    flex-wrap: wrap-reverse !important; }\n  .flex-sm-fill {\n    flex: 1 1 auto !important; }\n  .flex-sm-grow-0 {\n    flex-grow: 0 !important; }\n  .flex-sm-grow-1 {\n    flex-grow: 1 !important; }\n  .flex-sm-shrink-0 {\n    flex-shrink: 0 !important; }\n  .flex-sm-shrink-1 {\n    flex-shrink: 1 !important; }\n  .justify-content-sm-start {\n    justify-content: flex-start !important; }\n  .justify-content-sm-end {\n    justify-content: flex-end !important; }\n  .justify-content-sm-center {\n    justify-content: center !important; }\n  .justify-content-sm-between {\n    justify-content: space-between !important; }\n  .justify-content-sm-around {\n    justify-content: space-around !important; }\n  .align-items-sm-start {\n    align-items: flex-start !important; }\n  .align-items-sm-end {\n    align-items: flex-end !important; }\n  .align-items-sm-center {\n    align-items: center !important; }\n  .align-items-sm-baseline {\n    align-items: baseline !important; }\n  .align-items-sm-stretch {\n    align-items: stretch !important; }\n  .align-content-sm-start {\n    align-content: flex-start !important; }\n  .align-content-sm-end {\n    align-content: flex-end !important; }\n  .align-content-sm-center {\n    align-content: center !important; }\n  .align-content-sm-between {\n    align-content: space-between !important; }\n  .align-content-sm-around {\n    align-content: space-around !important; }\n  .align-content-sm-stretch {\n    align-content: stretch !important; }\n  .align-self-sm-auto {\n    align-self: auto !important; }\n  .align-self-sm-start {\n    align-self: flex-start !important; }\n  .align-self-sm-end {\n    align-self: flex-end !important; }\n  .align-self-sm-center {\n    align-self: center !important; }\n  .align-self-sm-baseline {\n    align-self: baseline !important; }\n  .align-self-sm-stretch {\n    align-self: stretch !important; } }\n\n@media (min-width: 768px) {\n  .flex-md-row {\n    flex-direction: row !important; }\n  .flex-md-column {\n    flex-direction: column !important; }\n  .flex-md-row-reverse {\n    flex-direction: row-reverse !important; }\n  .flex-md-column-reverse {\n    flex-direction: column-reverse !important; }\n  .flex-md-wrap {\n    flex-wrap: wrap !important; }\n  .flex-md-nowrap {\n    flex-wrap: nowrap !important; }\n  .flex-md-wrap-reverse {\n    flex-wrap: wrap-reverse !important; }\n  .flex-md-fill {\n    flex: 1 1 auto !important; }\n  .flex-md-grow-0 {\n    flex-grow: 0 !important; }\n  .flex-md-grow-1 {\n    flex-grow: 1 !important; }\n  .flex-md-shrink-0 {\n    flex-shrink: 0 !important; }\n  .flex-md-shrink-1 {\n    flex-shrink: 1 !important; }\n  .justify-content-md-start {\n    justify-content: flex-start !important; }\n  .justify-content-md-end {\n    justify-content: flex-end !important; }\n  .justify-content-md-center {\n    justify-content: center !important; }\n  .justify-content-md-between {\n    justify-content: space-between !important; }\n  .justify-content-md-around {\n    justify-content: space-around !important; }\n  .align-items-md-start {\n    align-items: flex-start !important; }\n  .align-items-md-end {\n    align-items: flex-end !important; }\n  .align-items-md-center {\n    align-items: center !important; }\n  .align-items-md-baseline {\n    align-items: baseline !important; }\n  .align-items-md-stretch {\n    align-items: stretch !important; }\n  .align-content-md-start {\n    align-content: flex-start !important; }\n  .align-content-md-end {\n    align-content: flex-end !important; }\n  .align-content-md-center {\n    align-content: center !important; }\n  .align-content-md-between {\n    align-content: space-between !important; }\n  .align-content-md-around {\n    align-content: space-around !important; }\n  .align-content-md-stretch {\n    align-content: stretch !important; }\n  .align-self-md-auto {\n    align-self: auto !important; }\n  .align-self-md-start {\n    align-self: flex-start !important; }\n  .align-self-md-end {\n    align-self: flex-end !important; }\n  .align-self-md-center {\n    align-self: center !important; }\n  .align-self-md-baseline {\n    align-self: baseline !important; }\n  .align-self-md-stretch {\n    align-self: stretch !important; } }\n\n@media (min-width: 992px) {\n  .flex-lg-row {\n    flex-direction: row !important; }\n  .flex-lg-column {\n    flex-direction: column !important; }\n  .flex-lg-row-reverse {\n    flex-direction: row-reverse !important; }\n  .flex-lg-column-reverse {\n    flex-direction: column-reverse !important; }\n  .flex-lg-wrap {\n    flex-wrap: wrap !important; }\n  .flex-lg-nowrap {\n    flex-wrap: nowrap !important; }\n  .flex-lg-wrap-reverse {\n    flex-wrap: wrap-reverse !important; }\n  .flex-lg-fill {\n    flex: 1 1 auto !important; }\n  .flex-lg-grow-0 {\n    flex-grow: 0 !important; }\n  .flex-lg-grow-1 {\n    flex-grow: 1 !important; }\n  .flex-lg-shrink-0 {\n    flex-shrink: 0 !important; }\n  .flex-lg-shrink-1 {\n    flex-shrink: 1 !important; }\n  .justify-content-lg-start {\n    justify-content: flex-start !important; }\n  .justify-content-lg-end {\n    justify-content: flex-end !important; }\n  .justify-content-lg-center {\n    justify-content: center !important; }\n  .justify-content-lg-between {\n    justify-content: space-between !important; }\n  .justify-content-lg-around {\n    justify-content: space-around !important; }\n  .align-items-lg-start {\n    align-items: flex-start !important; }\n  .align-items-lg-end {\n    align-items: flex-end !important; }\n  .align-items-lg-center {\n    align-items: center !important; }\n  .align-items-lg-baseline {\n    align-items: baseline !important; }\n  .align-items-lg-stretch {\n    align-items: stretch !important; }\n  .align-content-lg-start {\n    align-content: flex-start !important; }\n  .align-content-lg-end {\n    align-content: flex-end !important; }\n  .align-content-lg-center {\n    align-content: center !important; }\n  .align-content-lg-between {\n    align-content: space-between !important; }\n  .align-content-lg-around {\n    align-content: space-around !important; }\n  .align-content-lg-stretch {\n    align-content: stretch !important; }\n  .align-self-lg-auto {\n    align-self: auto !important; }\n  .align-self-lg-start {\n    align-self: flex-start !important; }\n  .align-self-lg-end {\n    align-self: flex-end !important; }\n  .align-self-lg-center {\n    align-self: center !important; }\n  .align-self-lg-baseline {\n    align-self: baseline !important; }\n  .align-self-lg-stretch {\n    align-self: stretch !important; } }\n\n@media (min-width: 1200px) {\n  .flex-xl-row {\n    flex-direction: row !important; }\n  .flex-xl-column {\n    flex-direction: column !important; }\n  .flex-xl-row-reverse {\n    flex-direction: row-reverse !important; }\n  .flex-xl-column-reverse {\n    flex-direction: column-reverse !important; }\n  .flex-xl-wrap {\n    flex-wrap: wrap !important; }\n  .flex-xl-nowrap {\n    flex-wrap: nowrap !important; }\n  .flex-xl-wrap-reverse {\n    flex-wrap: wrap-reverse !important; }\n  .flex-xl-fill {\n    flex: 1 1 auto !important; }\n  .flex-xl-grow-0 {\n    flex-grow: 0 !important; }\n  .flex-xl-grow-1 {\n    flex-grow: 1 !important; }\n  .flex-xl-shrink-0 {\n    flex-shrink: 0 !important; }\n  .flex-xl-shrink-1 {\n    flex-shrink: 1 !important; }\n  .justify-content-xl-start {\n    justify-content: flex-start !important; }\n  .justify-content-xl-end {\n    justify-content: flex-end !important; }\n  .justify-content-xl-center {\n    justify-content: center !important; }\n  .justify-content-xl-between {\n    justify-content: space-between !important; }\n  .justify-content-xl-around {\n    justify-content: space-around !important; }\n  .align-items-xl-start {\n    align-items: flex-start !important; }\n  .align-items-xl-end {\n    align-items: flex-end !important; }\n  .align-items-xl-center {\n    align-items: center !important; }\n  .align-items-xl-baseline {\n    align-items: baseline !important; }\n  .align-items-xl-stretch {\n    align-items: stretch !important; }\n  .align-content-xl-start {\n    align-content: flex-start !important; }\n  .align-content-xl-end {\n    align-content: flex-end !important; }\n  .align-content-xl-center {\n    align-content: center !important; }\n  .align-content-xl-between {\n    align-content: space-between !important; }\n  .align-content-xl-around {\n    align-content: space-around !important; }\n  .align-content-xl-stretch {\n    align-content: stretch !important; }\n  .align-self-xl-auto {\n    align-self: auto !important; }\n  .align-self-xl-start {\n    align-self: flex-start !important; }\n  .align-self-xl-end {\n    align-self: flex-end !important; }\n  .align-self-xl-center {\n    align-self: center !important; }\n  .align-self-xl-baseline {\n    align-self: baseline !important; }\n  .align-self-xl-stretch {\n    align-self: stretch !important; } }\n\n.m-0 {\n  margin: 0 !important; }\n\n.mt-0,\n.my-0 {\n  margin-top: 0 !important; }\n\n.mr-0,\n.mx-0 {\n  margin-right: 0 !important; }\n\n.mb-0,\n.my-0 {\n  margin-bottom: 0 !important; }\n\n.ml-0,\n.mx-0 {\n  margin-left: 0 !important; }\n\n.m-1 {\n  margin: 0.25rem !important; }\n\n.mt-1,\n.my-1 {\n  margin-top: 0.25rem !important; }\n\n.mr-1,\n.mx-1 {\n  margin-right: 0.25rem !important; }\n\n.mb-1,\n.my-1 {\n  margin-bottom: 0.25rem !important; }\n\n.ml-1,\n.mx-1 {\n  margin-left: 0.25rem !important; }\n\n.m-2 {\n  margin: 0.5rem !important; }\n\n.mt-2,\n.my-2 {\n  margin-top: 0.5rem !important; }\n\n.mr-2,\n.mx-2 {\n  margin-right: 0.5rem !important; }\n\n.mb-2,\n.my-2 {\n  margin-bottom: 0.5rem !important; }\n\n.ml-2,\n.mx-2 {\n  margin-left: 0.5rem !important; }\n\n.m-3 {\n  margin: 1rem !important; }\n\n.mt-3,\n.my-3 {\n  margin-top: 1rem !important; }\n\n.mr-3,\n.mx-3 {\n  margin-right: 1rem !important; }\n\n.mb-3,\n.my-3 {\n  margin-bottom: 1rem !important; }\n\n.ml-3,\n.mx-3 {\n  margin-left: 1rem !important; }\n\n.m-4 {\n  margin: 1.5rem !important; }\n\n.mt-4,\n.my-4 {\n  margin-top: 1.5rem !important; }\n\n.mr-4,\n.mx-4 {\n  margin-right: 1.5rem !important; }\n\n.mb-4,\n.my-4 {\n  margin-bottom: 1.5rem !important; }\n\n.ml-4,\n.mx-4 {\n  margin-left: 1.5rem !important; }\n\n.m-5 {\n  margin: 3rem !important; }\n\n.mt-5,\n.my-5 {\n  margin-top: 3rem !important; }\n\n.mr-5,\n.mx-5 {\n  margin-right: 3rem !important; }\n\n.mb-5,\n.my-5 {\n  margin-bottom: 3rem !important; }\n\n.ml-5,\n.mx-5 {\n  margin-left: 3rem !important; }\n\n.p-0 {\n  padding: 0 !important; }\n\n.pt-0,\n.py-0 {\n  padding-top: 0 !important; }\n\n.pr-0,\n.px-0 {\n  padding-right: 0 !important; }\n\n.pb-0,\n.py-0 {\n  padding-bottom: 0 !important; }\n\n.pl-0,\n.px-0 {\n  padding-left: 0 !important; }\n\n.p-1 {\n  padding: 0.25rem !important; }\n\n.pt-1,\n.py-1 {\n  padding-top: 0.25rem !important; }\n\n.pr-1,\n.px-1 {\n  padding-right: 0.25rem !important; }\n\n.pb-1,\n.py-1 {\n  padding-bottom: 0.25rem !important; }\n\n.pl-1,\n.px-1 {\n  padding-left: 0.25rem !important; }\n\n.p-2 {\n  padding: 0.5rem !important; }\n\n.pt-2,\n.py-2 {\n  padding-top: 0.5rem !important; }\n\n.pr-2,\n.px-2 {\n  padding-right: 0.5rem !important; }\n\n.pb-2,\n.py-2 {\n  padding-bottom: 0.5rem !important; }\n\n.pl-2,\n.px-2 {\n  padding-left: 0.5rem !important; }\n\n.p-3 {\n  padding: 1rem !important; }\n\n.pt-3,\n.py-3 {\n  padding-top: 1rem !important; }\n\n.pr-3,\n.px-3 {\n  padding-right: 1rem !important; }\n\n.pb-3,\n.py-3 {\n  padding-bottom: 1rem !important; }\n\n.pl-3,\n.px-3 {\n  padding-left: 1rem !important; }\n\n.p-4 {\n  padding: 1.5rem !important; }\n\n.pt-4,\n.py-4 {\n  padding-top: 1.5rem !important; }\n\n.pr-4,\n.px-4 {\n  padding-right: 1.5rem !important; }\n\n.pb-4,\n.py-4 {\n  padding-bottom: 1.5rem !important; }\n\n.pl-4,\n.px-4 {\n  padding-left: 1.5rem !important; }\n\n.p-5 {\n  padding: 3rem !important; }\n\n.pt-5,\n.py-5 {\n  padding-top: 3rem !important; }\n\n.pr-5,\n.px-5 {\n  padding-right: 3rem !important; }\n\n.pb-5,\n.py-5 {\n  padding-bottom: 3rem !important; }\n\n.pl-5,\n.px-5 {\n  padding-left: 3rem !important; }\n\n.m-n1 {\n  margin: -0.25rem !important; }\n\n.mt-n1,\n.my-n1 {\n  margin-top: -0.25rem !important; }\n\n.mr-n1,\n.mx-n1 {\n  margin-right: -0.25rem !important; }\n\n.mb-n1,\n.my-n1 {\n  margin-bottom: -0.25rem !important; }\n\n.ml-n1,\n.mx-n1 {\n  margin-left: -0.25rem !important; }\n\n.m-n2 {\n  margin: -0.5rem !important; }\n\n.mt-n2,\n.my-n2 {\n  margin-top: -0.5rem !important; }\n\n.mr-n2,\n.mx-n2 {\n  margin-right: -0.5rem !important; }\n\n.mb-n2,\n.my-n2 {\n  margin-bottom: -0.5rem !important; }\n\n.ml-n2,\n.mx-n2 {\n  margin-left: -0.5rem !important; }\n\n.m-n3 {\n  margin: -1rem !important; }\n\n.mt-n3,\n.my-n3 {\n  margin-top: -1rem !important; }\n\n.mr-n3,\n.mx-n3 {\n  margin-right: -1rem !important; }\n\n.mb-n3,\n.my-n3 {\n  margin-bottom: -1rem !important; }\n\n.ml-n3,\n.mx-n3 {\n  margin-left: -1rem !important; }\n\n.m-n4 {\n  margin: -1.5rem !important; }\n\n.mt-n4,\n.my-n4 {\n  margin-top: -1.5rem !important; }\n\n.mr-n4,\n.mx-n4 {\n  margin-right: -1.5rem !important; }\n\n.mb-n4,\n.my-n4 {\n  margin-bottom: -1.5rem !important; }\n\n.ml-n4,\n.mx-n4 {\n  margin-left: -1.5rem !important; }\n\n.m-n5 {\n  margin: -3rem !important; }\n\n.mt-n5,\n.my-n5 {\n  margin-top: -3rem !important; }\n\n.mr-n5,\n.mx-n5 {\n  margin-right: -3rem !important; }\n\n.mb-n5,\n.my-n5 {\n  margin-bottom: -3rem !important; }\n\n.ml-n5,\n.mx-n5 {\n  margin-left: -3rem !important; }\n\n.m-auto {\n  margin: auto !important; }\n\n.mt-auto,\n.my-auto {\n  margin-top: auto !important; }\n\n.mr-auto,\n.mx-auto {\n  margin-right: auto !important; }\n\n.mb-auto,\n.my-auto {\n  margin-bottom: auto !important; }\n\n.ml-auto,\n.mx-auto {\n  margin-left: auto !important; }\n\n@media (min-width: 576px) {\n  .m-sm-0 {\n    margin: 0 !important; }\n  .mt-sm-0,\n  .my-sm-0 {\n    margin-top: 0 !important; }\n  .mr-sm-0,\n  .mx-sm-0 {\n    margin-right: 0 !important; }\n  .mb-sm-0,\n  .my-sm-0 {\n    margin-bottom: 0 !important; }\n  .ml-sm-0,\n  .mx-sm-0 {\n    margin-left: 0 !important; }\n  .m-sm-1 {\n    margin: 0.25rem !important; }\n  .mt-sm-1,\n  .my-sm-1 {\n    margin-top: 0.25rem !important; }\n  .mr-sm-1,\n  .mx-sm-1 {\n    margin-right: 0.25rem !important; }\n  .mb-sm-1,\n  .my-sm-1 {\n    margin-bottom: 0.25rem !important; }\n  .ml-sm-1,\n  .mx-sm-1 {\n    margin-left: 0.25rem !important; }\n  .m-sm-2 {\n    margin: 0.5rem !important; }\n  .mt-sm-2,\n  .my-sm-2 {\n    margin-top: 0.5rem !important; }\n  .mr-sm-2,\n  .mx-sm-2 {\n    margin-right: 0.5rem !important; }\n  .mb-sm-2,\n  .my-sm-2 {\n    margin-bottom: 0.5rem !important; }\n  .ml-sm-2,\n  .mx-sm-2 {\n    margin-left: 0.5rem !important; }\n  .m-sm-3 {\n    margin: 1rem !important; }\n  .mt-sm-3,\n  .my-sm-3 {\n    margin-top: 1rem !important; }\n  .mr-sm-3,\n  .mx-sm-3 {\n    margin-right: 1rem !important; }\n  .mb-sm-3,\n  .my-sm-3 {\n    margin-bottom: 1rem !important; }\n  .ml-sm-3,\n  .mx-sm-3 {\n    margin-left: 1rem !important; }\n  .m-sm-4 {\n    margin: 1.5rem !important; }\n  .mt-sm-4,\n  .my-sm-4 {\n    margin-top: 1.5rem !important; }\n  .mr-sm-4,\n  .mx-sm-4 {\n    margin-right: 1.5rem !important; }\n  .mb-sm-4,\n  .my-sm-4 {\n    margin-bottom: 1.5rem !important; }\n  .ml-sm-4,\n  .mx-sm-4 {\n    margin-left: 1.5rem !important; }\n  .m-sm-5 {\n    margin: 3rem !important; }\n  .mt-sm-5,\n  .my-sm-5 {\n    margin-top: 3rem !important; }\n  .mr-sm-5,\n  .mx-sm-5 {\n    margin-right: 3rem !important; }\n  .mb-sm-5,\n  .my-sm-5 {\n    margin-bottom: 3rem !important; }\n  .ml-sm-5,\n  .mx-sm-5 {\n    margin-left: 3rem !important; }\n  .p-sm-0 {\n    padding: 0 !important; }\n  .pt-sm-0,\n  .py-sm-0 {\n    padding-top: 0 !important; }\n  .pr-sm-0,\n  .px-sm-0 {\n    padding-right: 0 !important; }\n  .pb-sm-0,\n  .py-sm-0 {\n    padding-bottom: 0 !important; }\n  .pl-sm-0,\n  .px-sm-0 {\n    padding-left: 0 !important; }\n  .p-sm-1 {\n    padding: 0.25rem !important; }\n  .pt-sm-1,\n  .py-sm-1 {\n    padding-top: 0.25rem !important; }\n  .pr-sm-1,\n  .px-sm-1 {\n    padding-right: 0.25rem !important; }\n  .pb-sm-1,\n  .py-sm-1 {\n    padding-bottom: 0.25rem !important; }\n  .pl-sm-1,\n  .px-sm-1 {\n    padding-left: 0.25rem !important; }\n  .p-sm-2 {\n    padding: 0.5rem !important; }\n  .pt-sm-2,\n  .py-sm-2 {\n    padding-top: 0.5rem !important; }\n  .pr-sm-2,\n  .px-sm-2 {\n    padding-right: 0.5rem !important; }\n  .pb-sm-2,\n  .py-sm-2 {\n    padding-bottom: 0.5rem !important; }\n  .pl-sm-2,\n  .px-sm-2 {\n    padding-left: 0.5rem !important; }\n  .p-sm-3 {\n    padding: 1rem !important; }\n  .pt-sm-3,\n  .py-sm-3 {\n    padding-top: 1rem !important; }\n  .pr-sm-3,\n  .px-sm-3 {\n    padding-right: 1rem !important; }\n  .pb-sm-3,\n  .py-sm-3 {\n    padding-bottom: 1rem !important; }\n  .pl-sm-3,\n  .px-sm-3 {\n    padding-left: 1rem !important; }\n  .p-sm-4 {\n    padding: 1.5rem !important; }\n  .pt-sm-4,\n  .py-sm-4 {\n    padding-top: 1.5rem !important; }\n  .pr-sm-4,\n  .px-sm-4 {\n    padding-right: 1.5rem !important; }\n  .pb-sm-4,\n  .py-sm-4 {\n    padding-bottom: 1.5rem !important; }\n  .pl-sm-4,\n  .px-sm-4 {\n    padding-left: 1.5rem !important; }\n  .p-sm-5 {\n    padding: 3rem !important; }\n  .pt-sm-5,\n  .py-sm-5 {\n    padding-top: 3rem !important; }\n  .pr-sm-5,\n  .px-sm-5 {\n    padding-right: 3rem !important; }\n  .pb-sm-5,\n  .py-sm-5 {\n    padding-bottom: 3rem !important; }\n  .pl-sm-5,\n  .px-sm-5 {\n    padding-left: 3rem !important; }\n  .m-sm-n1 {\n    margin: -0.25rem !important; }\n  .mt-sm-n1,\n  .my-sm-n1 {\n    margin-top: -0.25rem !important; }\n  .mr-sm-n1,\n  .mx-sm-n1 {\n    margin-right: -0.25rem !important; }\n  .mb-sm-n1,\n  .my-sm-n1 {\n    margin-bottom: -0.25rem !important; }\n  .ml-sm-n1,\n  .mx-sm-n1 {\n    margin-left: -0.25rem !important; }\n  .m-sm-n2 {\n    margin: -0.5rem !important; }\n  .mt-sm-n2,\n  .my-sm-n2 {\n    margin-top: -0.5rem !important; }\n  .mr-sm-n2,\n  .mx-sm-n2 {\n    margin-right: -0.5rem !important; }\n  .mb-sm-n2,\n  .my-sm-n2 {\n    margin-bottom: -0.5rem !important; }\n  .ml-sm-n2,\n  .mx-sm-n2 {\n    margin-left: -0.5rem !important; }\n  .m-sm-n3 {\n    margin: -1rem !important; }\n  .mt-sm-n3,\n  .my-sm-n3 {\n    margin-top: -1rem !important; }\n  .mr-sm-n3,\n  .mx-sm-n3 {\n    margin-right: -1rem !important; }\n  .mb-sm-n3,\n  .my-sm-n3 {\n    margin-bottom: -1rem !important; }\n  .ml-sm-n3,\n  .mx-sm-n3 {\n    margin-left: -1rem !important; }\n  .m-sm-n4 {\n    margin: -1.5rem !important; }\n  .mt-sm-n4,\n  .my-sm-n4 {\n    margin-top: -1.5rem !important; }\n  .mr-sm-n4,\n  .mx-sm-n4 {\n    margin-right: -1.5rem !important; }\n  .mb-sm-n4,\n  .my-sm-n4 {\n    margin-bottom: -1.5rem !important; }\n  .ml-sm-n4,\n  .mx-sm-n4 {\n    margin-left: -1.5rem !important; }\n  .m-sm-n5 {\n    margin: -3rem !important; }\n  .mt-sm-n5,\n  .my-sm-n5 {\n    margin-top: -3rem !important; }\n  .mr-sm-n5,\n  .mx-sm-n5 {\n    margin-right: -3rem !important; }\n  .mb-sm-n5,\n  .my-sm-n5 {\n    margin-bottom: -3rem !important; }\n  .ml-sm-n5,\n  .mx-sm-n5 {\n    margin-left: -3rem !important; }\n  .m-sm-auto {\n    margin: auto !important; }\n  .mt-sm-auto,\n  .my-sm-auto {\n    margin-top: auto !important; }\n  .mr-sm-auto,\n  .mx-sm-auto {\n    margin-right: auto !important; }\n  .mb-sm-auto,\n  .my-sm-auto {\n    margin-bottom: auto !important; }\n  .ml-sm-auto,\n  .mx-sm-auto {\n    margin-left: auto !important; } }\n\n@media (min-width: 768px) {\n  .m-md-0 {\n    margin: 0 !important; }\n  .mt-md-0,\n  .my-md-0 {\n    margin-top: 0 !important; }\n  .mr-md-0,\n  .mx-md-0 {\n    margin-right: 0 !important; }\n  .mb-md-0,\n  .my-md-0 {\n    margin-bottom: 0 !important; }\n  .ml-md-0,\n  .mx-md-0 {\n    margin-left: 0 !important; }\n  .m-md-1 {\n    margin: 0.25rem !important; }\n  .mt-md-1,\n  .my-md-1 {\n    margin-top: 0.25rem !important; }\n  .mr-md-1,\n  .mx-md-1 {\n    margin-right: 0.25rem !important; }\n  .mb-md-1,\n  .my-md-1 {\n    margin-bottom: 0.25rem !important; }\n  .ml-md-1,\n  .mx-md-1 {\n    margin-left: 0.25rem !important; }\n  .m-md-2 {\n    margin: 0.5rem !important; }\n  .mt-md-2,\n  .my-md-2 {\n    margin-top: 0.5rem !important; }\n  .mr-md-2,\n  .mx-md-2 {\n    margin-right: 0.5rem !important; }\n  .mb-md-2,\n  .my-md-2 {\n    margin-bottom: 0.5rem !important; }\n  .ml-md-2,\n  .mx-md-2 {\n    margin-left: 0.5rem !important; }\n  .m-md-3 {\n    margin: 1rem !important; }\n  .mt-md-3,\n  .my-md-3 {\n    margin-top: 1rem !important; }\n  .mr-md-3,\n  .mx-md-3 {\n    margin-right: 1rem !important; }\n  .mb-md-3,\n  .my-md-3 {\n    margin-bottom: 1rem !important; }\n  .ml-md-3,\n  .mx-md-3 {\n    margin-left: 1rem !important; }\n  .m-md-4 {\n    margin: 1.5rem !important; }\n  .mt-md-4,\n  .my-md-4 {\n    margin-top: 1.5rem !important; }\n  .mr-md-4,\n  .mx-md-4 {\n    margin-right: 1.5rem !important; }\n  .mb-md-4,\n  .my-md-4 {\n    margin-bottom: 1.5rem !important; }\n  .ml-md-4,\n  .mx-md-4 {\n    margin-left: 1.5rem !important; }\n  .m-md-5 {\n    margin: 3rem !important; }\n  .mt-md-5,\n  .my-md-5 {\n    margin-top: 3rem !important; }\n  .mr-md-5,\n  .mx-md-5 {\n    margin-right: 3rem !important; }\n  .mb-md-5,\n  .my-md-5 {\n    margin-bottom: 3rem !important; }\n  .ml-md-5,\n  .mx-md-5 {\n    margin-left: 3rem !important; }\n  .p-md-0 {\n    padding: 0 !important; }\n  .pt-md-0,\n  .py-md-0 {\n    padding-top: 0 !important; }\n  .pr-md-0,\n  .px-md-0 {\n    padding-right: 0 !important; }\n  .pb-md-0,\n  .py-md-0 {\n    padding-bottom: 0 !important; }\n  .pl-md-0,\n  .px-md-0 {\n    padding-left: 0 !important; }\n  .p-md-1 {\n    padding: 0.25rem !important; }\n  .pt-md-1,\n  .py-md-1 {\n    padding-top: 0.25rem !important; }\n  .pr-md-1,\n  .px-md-1 {\n    padding-right: 0.25rem !important; }\n  .pb-md-1,\n  .py-md-1 {\n    padding-bottom: 0.25rem !important; }\n  .pl-md-1,\n  .px-md-1 {\n    padding-left: 0.25rem !important; }\n  .p-md-2 {\n    padding: 0.5rem !important; }\n  .pt-md-2,\n  .py-md-2 {\n    padding-top: 0.5rem !important; }\n  .pr-md-2,\n  .px-md-2 {\n    padding-right: 0.5rem !important; }\n  .pb-md-2,\n  .py-md-2 {\n    padding-bottom: 0.5rem !important; }\n  .pl-md-2,\n  .px-md-2 {\n    padding-left: 0.5rem !important; }\n  .p-md-3 {\n    padding: 1rem !important; }\n  .pt-md-3,\n  .py-md-3 {\n    padding-top: 1rem !important; }\n  .pr-md-3,\n  .px-md-3 {\n    padding-right: 1rem !important; }\n  .pb-md-3,\n  .py-md-3 {\n    padding-bottom: 1rem !important; }\n  .pl-md-3,\n  .px-md-3 {\n    padding-left: 1rem !important; }\n  .p-md-4 {\n    padding: 1.5rem !important; }\n  .pt-md-4,\n  .py-md-4 {\n    padding-top: 1.5rem !important; }\n  .pr-md-4,\n  .px-md-4 {\n    padding-right: 1.5rem !important; }\n  .pb-md-4,\n  .py-md-4 {\n    padding-bottom: 1.5rem !important; }\n  .pl-md-4,\n  .px-md-4 {\n    padding-left: 1.5rem !important; }\n  .p-md-5 {\n    padding: 3rem !important; }\n  .pt-md-5,\n  .py-md-5 {\n    padding-top: 3rem !important; }\n  .pr-md-5,\n  .px-md-5 {\n    padding-right: 3rem !important; }\n  .pb-md-5,\n  .py-md-5 {\n    padding-bottom: 3rem !important; }\n  .pl-md-5,\n  .px-md-5 {\n    padding-left: 3rem !important; }\n  .m-md-n1 {\n    margin: -0.25rem !important; }\n  .mt-md-n1,\n  .my-md-n1 {\n    margin-top: -0.25rem !important; }\n  .mr-md-n1,\n  .mx-md-n1 {\n    margin-right: -0.25rem !important; }\n  .mb-md-n1,\n  .my-md-n1 {\n    margin-bottom: -0.25rem !important; }\n  .ml-md-n1,\n  .mx-md-n1 {\n    margin-left: -0.25rem !important; }\n  .m-md-n2 {\n    margin: -0.5rem !important; }\n  .mt-md-n2,\n  .my-md-n2 {\n    margin-top: -0.5rem !important; }\n  .mr-md-n2,\n  .mx-md-n2 {\n    margin-right: -0.5rem !important; }\n  .mb-md-n2,\n  .my-md-n2 {\n    margin-bottom: -0.5rem !important; }\n  .ml-md-n2,\n  .mx-md-n2 {\n    margin-left: -0.5rem !important; }\n  .m-md-n3 {\n    margin: -1rem !important; }\n  .mt-md-n3,\n  .my-md-n3 {\n    margin-top: -1rem !important; }\n  .mr-md-n3,\n  .mx-md-n3 {\n    margin-right: -1rem !important; }\n  .mb-md-n3,\n  .my-md-n3 {\n    margin-bottom: -1rem !important; }\n  .ml-md-n3,\n  .mx-md-n3 {\n    margin-left: -1rem !important; }\n  .m-md-n4 {\n    margin: -1.5rem !important; }\n  .mt-md-n4,\n  .my-md-n4 {\n    margin-top: -1.5rem !important; }\n  .mr-md-n4,\n  .mx-md-n4 {\n    margin-right: -1.5rem !important; }\n  .mb-md-n4,\n  .my-md-n4 {\n    margin-bottom: -1.5rem !important; }\n  .ml-md-n4,\n  .mx-md-n4 {\n    margin-left: -1.5rem !important; }\n  .m-md-n5 {\n    margin: -3rem !important; }\n  .mt-md-n5,\n  .my-md-n5 {\n    margin-top: -3rem !important; }\n  .mr-md-n5,\n  .mx-md-n5 {\n    margin-right: -3rem !important; }\n  .mb-md-n5,\n  .my-md-n5 {\n    margin-bottom: -3rem !important; }\n  .ml-md-n5,\n  .mx-md-n5 {\n    margin-left: -3rem !important; }\n  .m-md-auto {\n    margin: auto !important; }\n  .mt-md-auto,\n  .my-md-auto {\n    margin-top: auto !important; }\n  .mr-md-auto,\n  .mx-md-auto {\n    margin-right: auto !important; }\n  .mb-md-auto,\n  .my-md-auto {\n    margin-bottom: auto !important; }\n  .ml-md-auto,\n  .mx-md-auto {\n    margin-left: auto !important; } }\n\n@media (min-width: 992px) {\n  .m-lg-0 {\n    margin: 0 !important; }\n  .mt-lg-0,\n  .my-lg-0 {\n    margin-top: 0 !important; }\n  .mr-lg-0,\n  .mx-lg-0 {\n    margin-right: 0 !important; }\n  .mb-lg-0,\n  .my-lg-0 {\n    margin-bottom: 0 !important; }\n  .ml-lg-0,\n  .mx-lg-0 {\n    margin-left: 0 !important; }\n  .m-lg-1 {\n    margin: 0.25rem !important; }\n  .mt-lg-1,\n  .my-lg-1 {\n    margin-top: 0.25rem !important; }\n  .mr-lg-1,\n  .mx-lg-1 {\n    margin-right: 0.25rem !important; }\n  .mb-lg-1,\n  .my-lg-1 {\n    margin-bottom: 0.25rem !important; }\n  .ml-lg-1,\n  .mx-lg-1 {\n    margin-left: 0.25rem !important; }\n  .m-lg-2 {\n    margin: 0.5rem !important; }\n  .mt-lg-2,\n  .my-lg-2 {\n    margin-top: 0.5rem !important; }\n  .mr-lg-2,\n  .mx-lg-2 {\n    margin-right: 0.5rem !important; }\n  .mb-lg-2,\n  .my-lg-2 {\n    margin-bottom: 0.5rem !important; }\n  .ml-lg-2,\n  .mx-lg-2 {\n    margin-left: 0.5rem !important; }\n  .m-lg-3 {\n    margin: 1rem !important; }\n  .mt-lg-3,\n  .my-lg-3 {\n    margin-top: 1rem !important; }\n  .mr-lg-3,\n  .mx-lg-3 {\n    margin-right: 1rem !important; }\n  .mb-lg-3,\n  .my-lg-3 {\n    margin-bottom: 1rem !important; }\n  .ml-lg-3,\n  .mx-lg-3 {\n    margin-left: 1rem !important; }\n  .m-lg-4 {\n    margin: 1.5rem !important; }\n  .mt-lg-4,\n  .my-lg-4 {\n    margin-top: 1.5rem !important; }\n  .mr-lg-4,\n  .mx-lg-4 {\n    margin-right: 1.5rem !important; }\n  .mb-lg-4,\n  .my-lg-4 {\n    margin-bottom: 1.5rem !important; }\n  .ml-lg-4,\n  .mx-lg-4 {\n    margin-left: 1.5rem !important; }\n  .m-lg-5 {\n    margin: 3rem !important; }\n  .mt-lg-5,\n  .my-lg-5 {\n    margin-top: 3rem !important; }\n  .mr-lg-5,\n  .mx-lg-5 {\n    margin-right: 3rem !important; }\n  .mb-lg-5,\n  .my-lg-5 {\n    margin-bottom: 3rem !important; }\n  .ml-lg-5,\n  .mx-lg-5 {\n    margin-left: 3rem !important; }\n  .p-lg-0 {\n    padding: 0 !important; }\n  .pt-lg-0,\n  .py-lg-0 {\n    padding-top: 0 !important; }\n  .pr-lg-0,\n  .px-lg-0 {\n    padding-right: 0 !important; }\n  .pb-lg-0,\n  .py-lg-0 {\n    padding-bottom: 0 !important; }\n  .pl-lg-0,\n  .px-lg-0 {\n    padding-left: 0 !important; }\n  .p-lg-1 {\n    padding: 0.25rem !important; }\n  .pt-lg-1,\n  .py-lg-1 {\n    padding-top: 0.25rem !important; }\n  .pr-lg-1,\n  .px-lg-1 {\n    padding-right: 0.25rem !important; }\n  .pb-lg-1,\n  .py-lg-1 {\n    padding-bottom: 0.25rem !important; }\n  .pl-lg-1,\n  .px-lg-1 {\n    padding-left: 0.25rem !important; }\n  .p-lg-2 {\n    padding: 0.5rem !important; }\n  .pt-lg-2,\n  .py-lg-2 {\n    padding-top: 0.5rem !important; }\n  .pr-lg-2,\n  .px-lg-2 {\n    padding-right: 0.5rem !important; }\n  .pb-lg-2,\n  .py-lg-2 {\n    padding-bottom: 0.5rem !important; }\n  .pl-lg-2,\n  .px-lg-2 {\n    padding-left: 0.5rem !important; }\n  .p-lg-3 {\n    padding: 1rem !important; }\n  .pt-lg-3,\n  .py-lg-3 {\n    padding-top: 1rem !important; }\n  .pr-lg-3,\n  .px-lg-3 {\n    padding-right: 1rem !important; }\n  .pb-lg-3,\n  .py-lg-3 {\n    padding-bottom: 1rem !important; }\n  .pl-lg-3,\n  .px-lg-3 {\n    padding-left: 1rem !important; }\n  .p-lg-4 {\n    padding: 1.5rem !important; }\n  .pt-lg-4,\n  .py-lg-4 {\n    padding-top: 1.5rem !important; }\n  .pr-lg-4,\n  .px-lg-4 {\n    padding-right: 1.5rem !important; }\n  .pb-lg-4,\n  .py-lg-4 {\n    padding-bottom: 1.5rem !important; }\n  .pl-lg-4,\n  .px-lg-4 {\n    padding-left: 1.5rem !important; }\n  .p-lg-5 {\n    padding: 3rem !important; }\n  .pt-lg-5,\n  .py-lg-5 {\n    padding-top: 3rem !important; }\n  .pr-lg-5,\n  .px-lg-5 {\n    padding-right: 3rem !important; }\n  .pb-lg-5,\n  .py-lg-5 {\n    padding-bottom: 3rem !important; }\n  .pl-lg-5,\n  .px-lg-5 {\n    padding-left: 3rem !important; }\n  .m-lg-n1 {\n    margin: -0.25rem !important; }\n  .mt-lg-n1,\n  .my-lg-n1 {\n    margin-top: -0.25rem !important; }\n  .mr-lg-n1,\n  .mx-lg-n1 {\n    margin-right: -0.25rem !important; }\n  .mb-lg-n1,\n  .my-lg-n1 {\n    margin-bottom: -0.25rem !important; }\n  .ml-lg-n1,\n  .mx-lg-n1 {\n    margin-left: -0.25rem !important; }\n  .m-lg-n2 {\n    margin: -0.5rem !important; }\n  .mt-lg-n2,\n  .my-lg-n2 {\n    margin-top: -0.5rem !important; }\n  .mr-lg-n2,\n  .mx-lg-n2 {\n    margin-right: -0.5rem !important; }\n  .mb-lg-n2,\n  .my-lg-n2 {\n    margin-bottom: -0.5rem !important; }\n  .ml-lg-n2,\n  .mx-lg-n2 {\n    margin-left: -0.5rem !important; }\n  .m-lg-n3 {\n    margin: -1rem !important; }\n  .mt-lg-n3,\n  .my-lg-n3 {\n    margin-top: -1rem !important; }\n  .mr-lg-n3,\n  .mx-lg-n3 {\n    margin-right: -1rem !important; }\n  .mb-lg-n3,\n  .my-lg-n3 {\n    margin-bottom: -1rem !important; }\n  .ml-lg-n3,\n  .mx-lg-n3 {\n    margin-left: -1rem !important; }\n  .m-lg-n4 {\n    margin: -1.5rem !important; }\n  .mt-lg-n4,\n  .my-lg-n4 {\n    margin-top: -1.5rem !important; }\n  .mr-lg-n4,\n  .mx-lg-n4 {\n    margin-right: -1.5rem !important; }\n  .mb-lg-n4,\n  .my-lg-n4 {\n    margin-bottom: -1.5rem !important; }\n  .ml-lg-n4,\n  .mx-lg-n4 {\n    margin-left: -1.5rem !important; }\n  .m-lg-n5 {\n    margin: -3rem !important; }\n  .mt-lg-n5,\n  .my-lg-n5 {\n    margin-top: -3rem !important; }\n  .mr-lg-n5,\n  .mx-lg-n5 {\n    margin-right: -3rem !important; }\n  .mb-lg-n5,\n  .my-lg-n5 {\n    margin-bottom: -3rem !important; }\n  .ml-lg-n5,\n  .mx-lg-n5 {\n    margin-left: -3rem !important; }\n  .m-lg-auto {\n    margin: auto !important; }\n  .mt-lg-auto,\n  .my-lg-auto {\n    margin-top: auto !important; }\n  .mr-lg-auto,\n  .mx-lg-auto {\n    margin-right: auto !important; }\n  .mb-lg-auto,\n  .my-lg-auto {\n    margin-bottom: auto !important; }\n  .ml-lg-auto,\n  .mx-lg-auto {\n    margin-left: auto !important; } }\n\n@media (min-width: 1200px) {\n  .m-xl-0 {\n    margin: 0 !important; }\n  .mt-xl-0,\n  .my-xl-0 {\n    margin-top: 0 !important; }\n  .mr-xl-0,\n  .mx-xl-0 {\n    margin-right: 0 !important; }\n  .mb-xl-0,\n  .my-xl-0 {\n    margin-bottom: 0 !important; }\n  .ml-xl-0,\n  .mx-xl-0 {\n    margin-left: 0 !important; }\n  .m-xl-1 {\n    margin: 0.25rem !important; }\n  .mt-xl-1,\n  .my-xl-1 {\n    margin-top: 0.25rem !important; }\n  .mr-xl-1,\n  .mx-xl-1 {\n    margin-right: 0.25rem !important; }\n  .mb-xl-1,\n  .my-xl-1 {\n    margin-bottom: 0.25rem !important; }\n  .ml-xl-1,\n  .mx-xl-1 {\n    margin-left: 0.25rem !important; }\n  .m-xl-2 {\n    margin: 0.5rem !important; }\n  .mt-xl-2,\n  .my-xl-2 {\n    margin-top: 0.5rem !important; }\n  .mr-xl-2,\n  .mx-xl-2 {\n    margin-right: 0.5rem !important; }\n  .mb-xl-2,\n  .my-xl-2 {\n    margin-bottom: 0.5rem !important; }\n  .ml-xl-2,\n  .mx-xl-2 {\n    margin-left: 0.5rem !important; }\n  .m-xl-3 {\n    margin: 1rem !important; }\n  .mt-xl-3,\n  .my-xl-3 {\n    margin-top: 1rem !important; }\n  .mr-xl-3,\n  .mx-xl-3 {\n    margin-right: 1rem !important; }\n  .mb-xl-3,\n  .my-xl-3 {\n    margin-bottom: 1rem !important; }\n  .ml-xl-3,\n  .mx-xl-3 {\n    margin-left: 1rem !important; }\n  .m-xl-4 {\n    margin: 1.5rem !important; }\n  .mt-xl-4,\n  .my-xl-4 {\n    margin-top: 1.5rem !important; }\n  .mr-xl-4,\n  .mx-xl-4 {\n    margin-right: 1.5rem !important; }\n  .mb-xl-4,\n  .my-xl-4 {\n    margin-bottom: 1.5rem !important; }\n  .ml-xl-4,\n  .mx-xl-4 {\n    margin-left: 1.5rem !important; }\n  .m-xl-5 {\n    margin: 3rem !important; }\n  .mt-xl-5,\n  .my-xl-5 {\n    margin-top: 3rem !important; }\n  .mr-xl-5,\n  .mx-xl-5 {\n    margin-right: 3rem !important; }\n  .mb-xl-5,\n  .my-xl-5 {\n    margin-bottom: 3rem !important; }\n  .ml-xl-5,\n  .mx-xl-5 {\n    margin-left: 3rem !important; }\n  .p-xl-0 {\n    padding: 0 !important; }\n  .pt-xl-0,\n  .py-xl-0 {\n    padding-top: 0 !important; }\n  .pr-xl-0,\n  .px-xl-0 {\n    padding-right: 0 !important; }\n  .pb-xl-0,\n  .py-xl-0 {\n    padding-bottom: 0 !important; }\n  .pl-xl-0,\n  .px-xl-0 {\n    padding-left: 0 !important; }\n  .p-xl-1 {\n    padding: 0.25rem !important; }\n  .pt-xl-1,\n  .py-xl-1 {\n    padding-top: 0.25rem !important; }\n  .pr-xl-1,\n  .px-xl-1 {\n    padding-right: 0.25rem !important; }\n  .pb-xl-1,\n  .py-xl-1 {\n    padding-bottom: 0.25rem !important; }\n  .pl-xl-1,\n  .px-xl-1 {\n    padding-left: 0.25rem !important; }\n  .p-xl-2 {\n    padding: 0.5rem !important; }\n  .pt-xl-2,\n  .py-xl-2 {\n    padding-top: 0.5rem !important; }\n  .pr-xl-2,\n  .px-xl-2 {\n    padding-right: 0.5rem !important; }\n  .pb-xl-2,\n  .py-xl-2 {\n    padding-bottom: 0.5rem !important; }\n  .pl-xl-2,\n  .px-xl-2 {\n    padding-left: 0.5rem !important; }\n  .p-xl-3 {\n    padding: 1rem !important; }\n  .pt-xl-3,\n  .py-xl-3 {\n    padding-top: 1rem !important; }\n  .pr-xl-3,\n  .px-xl-3 {\n    padding-right: 1rem !important; }\n  .pb-xl-3,\n  .py-xl-3 {\n    padding-bottom: 1rem !important; }\n  .pl-xl-3,\n  .px-xl-3 {\n    padding-left: 1rem !important; }\n  .p-xl-4 {\n    padding: 1.5rem !important; }\n  .pt-xl-4,\n  .py-xl-4 {\n    padding-top: 1.5rem !important; }\n  .pr-xl-4,\n  .px-xl-4 {\n    padding-right: 1.5rem !important; }\n  .pb-xl-4,\n  .py-xl-4 {\n    padding-bottom: 1.5rem !important; }\n  .pl-xl-4,\n  .px-xl-4 {\n    padding-left: 1.5rem !important; }\n  .p-xl-5 {\n    padding: 3rem !important; }\n  .pt-xl-5,\n  .py-xl-5 {\n    padding-top: 3rem !important; }\n  .pr-xl-5,\n  .px-xl-5 {\n    padding-right: 3rem !important; }\n  .pb-xl-5,\n  .py-xl-5 {\n    padding-bottom: 3rem !important; }\n  .pl-xl-5,\n  .px-xl-5 {\n    padding-left: 3rem !important; }\n  .m-xl-n1 {\n    margin: -0.25rem !important; }\n  .mt-xl-n1,\n  .my-xl-n1 {\n    margin-top: -0.25rem !important; }\n  .mr-xl-n1,\n  .mx-xl-n1 {\n    margin-right: -0.25rem !important; }\n  .mb-xl-n1,\n  .my-xl-n1 {\n    margin-bottom: -0.25rem !important; }\n  .ml-xl-n1,\n  .mx-xl-n1 {\n    margin-left: -0.25rem !important; }\n  .m-xl-n2 {\n    margin: -0.5rem !important; }\n  .mt-xl-n2,\n  .my-xl-n2 {\n    margin-top: -0.5rem !important; }\n  .mr-xl-n2,\n  .mx-xl-n2 {\n    margin-right: -0.5rem !important; }\n  .mb-xl-n2,\n  .my-xl-n2 {\n    margin-bottom: -0.5rem !important; }\n  .ml-xl-n2,\n  .mx-xl-n2 {\n    margin-left: -0.5rem !important; }\n  .m-xl-n3 {\n    margin: -1rem !important; }\n  .mt-xl-n3,\n  .my-xl-n3 {\n    margin-top: -1rem !important; }\n  .mr-xl-n3,\n  .mx-xl-n3 {\n    margin-right: -1rem !important; }\n  .mb-xl-n3,\n  .my-xl-n3 {\n    margin-bottom: -1rem !important; }\n  .ml-xl-n3,\n  .mx-xl-n3 {\n    margin-left: -1rem !important; }\n  .m-xl-n4 {\n    margin: -1.5rem !important; }\n  .mt-xl-n4,\n  .my-xl-n4 {\n    margin-top: -1.5rem !important; }\n  .mr-xl-n4,\n  .mx-xl-n4 {\n    margin-right: -1.5rem !important; }\n  .mb-xl-n4,\n  .my-xl-n4 {\n    margin-bottom: -1.5rem !important; }\n  .ml-xl-n4,\n  .mx-xl-n4 {\n    margin-left: -1.5rem !important; }\n  .m-xl-n5 {\n    margin: -3rem !important; }\n  .mt-xl-n5,\n  .my-xl-n5 {\n    margin-top: -3rem !important; }\n  .mr-xl-n5,\n  .mx-xl-n5 {\n    margin-right: -3rem !important; }\n  .mb-xl-n5,\n  .my-xl-n5 {\n    margin-bottom: -3rem !important; }\n  .ml-xl-n5,\n  .mx-xl-n5 {\n    margin-left: -3rem !important; }\n  .m-xl-auto {\n    margin: auto !important; }\n  .mt-xl-auto,\n  .my-xl-auto {\n    margin-top: auto !important; }\n  .mr-xl-auto,\n  .mx-xl-auto {\n    margin-right: auto !important; }\n  .mb-xl-auto,\n  .my-xl-auto {\n    margin-bottom: auto !important; }\n  .ml-xl-auto,\n  .mx-xl-auto {\n    margin-left: auto !important; } }\n\n.text-monospace {\n  font-family: SFMono-Regular, Menlo, Monaco, Consolas, \"Liberation Mono\", \"Courier New\", monospace !important; }\n\n.text-justify {\n  text-align: justify !important; }\n\n.text-wrap {\n  white-space: normal !important; }\n\n.text-nowrap {\n  white-space: nowrap !important; }\n\n.text-truncate {\n  overflow: hidden;\n  text-overflow: ellipsis;\n  white-space: nowrap; }\n\n.text-left {\n  text-align: left !important; }\n\n.text-right {\n  text-align: right !important; }\n\n.text-center {\n  text-align: center !important; }\n\n@media (min-width: 576px) {\n  .text-sm-left {\n    text-align: left !important; }\n  .text-sm-right {\n    text-align: right !important; }\n  .text-sm-center {\n    text-align: center !important; } }\n\n@media (min-width: 768px) {\n  .text-md-left {\n    text-align: left !important; }\n  .text-md-right {\n    text-align: right !important; }\n  .text-md-center {\n    text-align: center !important; } }\n\n@media (min-width: 992px) {\n  .text-lg-left {\n    text-align: left !important; }\n  .text-lg-right {\n    text-align: right !important; }\n  .text-lg-center {\n    text-align: center !important; } }\n\n@media (min-width: 1200px) {\n  .text-xl-left {\n    text-align: left !important; }\n  .text-xl-right {\n    text-align: right !important; }\n  .text-xl-center {\n    text-align: center !important; } }\n\n.text-lowercase {\n  text-transform: lowercase !important; }\n\n.text-uppercase {\n  text-transform: uppercase !important; }\n\n.text-capitalize {\n  text-transform: capitalize !important; }\n\n.font-weight-light {\n  font-weight: 300 !important; }\n\n.font-weight-lighter {\n  font-weight: lighter !important; }\n\n.font-weight-normal {\n  font-weight: 400 !important; }\n\n.font-weight-bold {\n  font-weight: 700 !important; }\n\n.font-weight-bolder {\n  font-weight: bolder !important; }\n\n.font-italic {\n  font-style: italic !important; }\n\n.text-white {\n  color: #fff !important; }\n\n.text-primary {\n  color: #007bff !important; }\n\na.text-primary:hover, a.text-primary:focus {\n  color: #0056b3 !important; }\n\n.text-secondary {\n  color: #6c757d !important; }\n\na.text-secondary:hover, a.text-secondary:focus {\n  color: #494f54 !important; }\n\n.text-success {\n  color: #28a745 !important; }\n\na.text-success:hover, a.text-success:focus {\n  color: #19692c !important; }\n\n.text-info {\n  color: #17a2b8 !important; }\n\na.text-info:hover, a.text-info:focus {\n  color: #0f6674 !important; }\n\n.text-warning {\n  color: #ffc107 !important; }\n\na.text-warning:hover, a.text-warning:focus {\n  color: #ba8b00 !important; }\n\n.text-danger {\n  color: #dc3545 !important; }\n\na.text-danger:hover, a.text-danger:focus {\n  color: #a71d2a !important; }\n\n.text-light {\n  color: #f8f9fa !important; }\n\na.text-light:hover, a.text-light:focus {\n  color: #cbd3da !important; }\n\n.text-dark {\n  color: #343a40 !important; }\n\na.text-dark:hover, a.text-dark:focus {\n  color: #121416 !important; }\n\n.text-body {\n  color: #212529 !important; }\n\n.text-muted {\n  color: #6c757d !important; }\n\n.text-black-50 {\n  color: rgba(0, 0, 0, 0.5) !important; }\n\n.text-white-50 {\n  color: rgba(255, 255, 255, 0.5) !important; }\n\n.text-hide {\n  font: 0/0 a;\n  color: transparent;\n  text-shadow: none;\n  background-color: transparent;\n  border: 0; }\n\n.text-decoration-none {\n  text-decoration: none !important; }\n\n.text-break {\n  word-break: break-word !important;\n  overflow-wrap: break-word !important; }\n\n.text-reset {\n  color: inherit !important; }\n\n.bg-primary {\n  background-color: #007bff !important; }\n\na.bg-primary:hover, a.bg-primary:focus,\nbutton.bg-primary:hover,\nbutton.bg-primary:focus {\n  background-color: #0062cc !important; }\n\n.bg-secondary {\n  background-color: #6c757d !important; }\n\na.bg-secondary:hover, a.bg-secondary:focus,\nbutton.bg-secondary:hover,\nbutton.bg-secondary:focus {\n  background-color: #545b62 !important; }\n\n.bg-success {\n  background-color: #28a745 !important; }\n\na.bg-success:hover, a.bg-success:focus,\nbutton.bg-success:hover,\nbutton.bg-success:focus {\n  background-color: #1e7e34 !important; }\n\n.bg-info {\n  background-color: #17a2b8 !important; }\n\na.bg-info:hover, a.bg-info:focus,\nbutton.bg-info:hover,\nbutton.bg-info:focus {\n  background-color: #117a8b !important; }\n\n.bg-warning {\n  background-color: #ffc107 !important; }\n\na.bg-warning:hover, a.bg-warning:focus,\nbutton.bg-warning:hover,\nbutton.bg-warning:focus {\n  background-color: #d39e00 !important; }\n\n.bg-danger {\n  background-color: #dc3545 !important; }\n\na.bg-danger:hover, a.bg-danger:focus,\nbutton.bg-danger:hover,\nbutton.bg-danger:focus {\n  background-color: #bd2130 !important; }\n\n.bg-light {\n  background-color: #f8f9fa !important; }\n\na.bg-light:hover, a.bg-light:focus,\nbutton.bg-light:hover,\nbutton.bg-light:focus {\n  background-color: #dae0e5 !important; }\n\n.bg-dark {\n  background-color: #343a40 !important; }\n\na.bg-dark:hover, a.bg-dark:focus,\nbutton.bg-dark:hover,\nbutton.bg-dark:focus {\n  background-color: #1d2124 !important; }\n\n.bg-white {\n  background-color: #fff !important; }\n\n.bg-transparent {\n  background-color: transparent !important; }\n", ""]);



/***/ }),

/***/ "./src/style/highlightjs.scss":
/*!************************************!*\
  !*** ./src/style/highlightjs.scss ***!
  \************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(/*! ../../node_modules/css-loader/dist/runtime/api.js */ "./node_modules/css-loader/dist/runtime/api.js")(false);
// Imports
exports.i(__webpack_require__(/*! -!../../node_modules/css-loader/dist/cjs.js!highlight.js/styles/default.css */ "./node_modules/css-loader/dist/cjs.js!./node_modules/highlight.js/styles/default.css"), "");

// Module
exports.push([module.i, "\n", ""]);



/***/ }),

/***/ "./src/style/index.ts":
/*!****************************!*\
  !*** ./src/style/index.ts ***!
  \****************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const lit_element_1 = __webpack_require__(/*! lit-element */ "./node_modules/lit-element/lit-element.js");
const bootstrapCss = __webpack_require__(/*! ./bootstrap.scss */ "./src/style/bootstrap.scss");
const highlightjsCss = __webpack_require__(/*! ./highlightjs.scss */ "./src/style/highlightjs.scss");
exports.bootstrap = lit_element_1.css `${lit_element_1.unsafeCSS(bootstrapCss.toString())}`;
exports.highlightJS = lit_element_1.css `${lit_element_1.unsafeCSS(highlightjsCss.toString())}`;


/***/ })

/******/ });
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly8vd2VicGFjay9ib290c3RyYXAiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2hpZ2hsaWdodC5qcy9zdHlsZXMvZGVmYXVsdC5jc3MiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2Nzcy1sb2FkZXIvZGlzdC9ydW50aW1lL2FwaS5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvaGlnaGxpZ2h0LmpzL2xpYi9oaWdobGlnaHQuanMiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2hpZ2hsaWdodC5qcy9saWIvbGFuZ3VhZ2VzL2NzLmpzIiwid2VicGFjazovLy8uL25vZGVfbW9kdWxlcy9oaWdobGlnaHQuanMvbGliL2xhbmd1YWdlcy9qYXZhLmpzIiwid2VicGFjazovLy8uL25vZGVfbW9kdWxlcy9oaWdobGlnaHQuanMvbGliL2xhbmd1YWdlcy9qYXZhc2NyaXB0LmpzIiwid2VicGFjazovLy8uL25vZGVfbW9kdWxlcy9oaWdobGlnaHQuanMvbGliL2xhbmd1YWdlcy9zY2FsYS5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvaGlnaGxpZ2h0LmpzL2xpYi9sYW5ndWFnZXMvdHlwZXNjcmlwdC5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWVsZW1lbnQvbGliL2Nzcy10YWcuanMiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2xpdC1lbGVtZW50L2xpYi9kZWNvcmF0b3JzLmpzIiwid2VicGFjazovLy8uL25vZGVfbW9kdWxlcy9saXQtZWxlbWVudC9saWIvdXBkYXRpbmctZWxlbWVudC5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWVsZW1lbnQvbGl0LWVsZW1lbnQuanMiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2xpdC1odG1sL2xpYi9kZWZhdWx0LXRlbXBsYXRlLXByb2Nlc3Nvci5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL2RpcmVjdGl2ZS5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL2RvbS5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL21vZGlmeS10ZW1wbGF0ZS5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL3BhcnQuanMiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2xpdC1odG1sL2xpYi9wYXJ0cy5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL3JlbmRlci5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL3NoYWR5LXJlbmRlci5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL3RlbXBsYXRlLWZhY3RvcnkuanMiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2xpdC1odG1sL2xpYi90ZW1wbGF0ZS1pbnN0YW5jZS5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL3RlbXBsYXRlLXJlc3VsdC5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL3RlbXBsYXRlLmpzIiwid2VicGFjazovLy8uL25vZGVfbW9kdWxlcy9saXQtaHRtbC9saXQtaHRtbC5qcyIsIndlYnBhY2s6Ly8vLi9zcmMvY29tcG9uZW50cy9tdXRhdGlvbi10ZXN0LXJlcG9ydC1hcHAudHMiLCJ3ZWJwYWNrOi8vLy4vc3JjL2NvbXBvbmVudHMvbXV0YXRpb24tdGVzdC1yZXBvcnQtYnJlYWRjcnVtYi50cyIsIndlYnBhY2s6Ly8vLi9zcmMvY29tcG9uZW50cy9tdXRhdGlvbi10ZXN0LXJlcG9ydC1maWxlLWxlZ2VuZC50cyIsIndlYnBhY2s6Ly8vLi9zcmMvY29tcG9uZW50cy9tdXRhdGlvbi10ZXN0LXJlcG9ydC1maWxlLnRzIiwid2VicGFjazovLy8uL3NyYy9jb21wb25lbnRzL211dGF0aW9uLXRlc3QtcmVwb3J0LW11dGFudC50cyIsIndlYnBhY2s6Ly8vLi9zcmMvY29tcG9uZW50cy9tdXRhdGlvbi10ZXN0LXJlcG9ydC1yZXN1bHQudHMiLCJ3ZWJwYWNrOi8vLy4vc3JjL2NvbXBvbmVudHMvbXV0YXRpb24tdGVzdC1yZXBvcnQtcm91dGVyLnRzIiwid2VicGFjazovLy8uL3NyYy9jb21wb25lbnRzL211dGF0aW9uLXRlc3QtcmVwb3J0LXRvdGFscy50cyIsIndlYnBhY2s6Ly8vLi9zcmMvaGVscGVycy50cyIsIndlYnBhY2s6Ly8vLi9zcmMvaW5kZXgudHMiLCJ3ZWJwYWNrOi8vLy4vc3JjL3N0eWxlL2Jvb3RzdHJhcC5zY3NzIiwid2VicGFjazovLy8uL3NyYy9zdHlsZS9oaWdobGlnaHRqcy5zY3NzIiwid2VicGFjazovLy8uL3NyYy9zdHlsZS9pbmRleC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUE7QUFDQTs7QUFFQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBOztBQUVBO0FBQ0E7O0FBRUE7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7OztBQUdBO0FBQ0E7O0FBRUE7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQSxrREFBMEMsZ0NBQWdDO0FBQzFFO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0EsZ0VBQXdELGtCQUFrQjtBQUMxRTtBQUNBLHlEQUFpRCxjQUFjO0FBQy9EOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxpREFBeUMsaUNBQWlDO0FBQzFFLHdIQUFnSCxtQkFBbUIsRUFBRTtBQUNySTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBLG1DQUEyQiwwQkFBMEIsRUFBRTtBQUN2RCx5Q0FBaUMsZUFBZTtBQUNoRDtBQUNBO0FBQ0E7O0FBRUE7QUFDQSw4REFBc0QsK0RBQStEOztBQUVySDtBQUNBOzs7QUFHQTtBQUNBOzs7Ozs7Ozs7Ozs7QUNsRkEs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QTs7QUFFQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxLQUFLO0FBQ0w7QUFDQTtBQUNBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBOztBQUVBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxXQUFXO0FBQ1g7QUFDQTtBQUNBO0FBQ0EsT0FBTztBQUNQOztBQUVBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBOztBQUVBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQSxLQUFLO0FBQ0w7QUFDQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQSw0Q0FBNEMsdUJBQXVCO0FBQ25FO0FBQ0E7O0FBRUE7QUFDQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBOztBQUVBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLDhGQUE4RjtBQUM5Rix5Q0FBeUM7QUFDekMsZ0ZBQWdGLHNEQUFzRDs7QUFFdEk7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FB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Q3ZDO0FBQ0EsU0FBUyxFQUFFO0FBQ1g7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLHFDQUFxQyxjQUFjO0FBQ25EO0FBQ0E7QUFDQSx3QkFBd0IsY0FBYztBQUN0QztBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ087QUFDUDtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0Esc0M7Ozs7Ozs7Ozs7OztBQzNMQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNPO0FBQ1A7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLEtBQUs7QUFDTDtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ087QUFDUDtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDTztBQUNQO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLFNBQVM7QUFDVDtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLCtEQUErRCxLQUFLO0FBQ3BFO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxhQUFhO0FBQ2I7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EsYUFBYTtBQUNiO0FBQ0E7QUFDQSxTQUFTO0FBQ1Q7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLFNBQVM7QUFDVDtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxvQkFBb0IsWUFBWTtBQUNoQyx3QkFBd0IsSUFBSTtBQUM1QixpQkFBaUIsUUFBUTtBQUN6QjtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLGlCQUFpQixRQUFRO0FBQ3pCO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSw0Qzs7Ozs7Ozs7Ozs7O0FDL2lCQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUMwQztBQUNTO0FBQ1M7QUFDbEI7QUFDTjtBQUM2QztBQUNsQjtBQUM5QjtBQUNqQztBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EsMkNBQTJDLFlBQVk7QUFDdkQ7QUFDQTtBQUNBO0FBQ0E7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Q0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EsUUFBUSwyREFBVztBQUNuQjtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDTztBQUNQO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxlQUFlLGlFQUFXO0FBQzFCO0FBQ0EsaUNBQWlDLGlEQUFRO0FBQ3pDO0FBQ0E7QUFDQSxtQ0FBbUMsaURBQVE7QUFDM0M7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EsNkJBQTZCLGlEQUFRO0FBQ3JDO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDTztBQUNQO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ087QUFDUDtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ087QUFDUDtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxlQUFlLGlFQUFXO0FBQzFCO0FBQ0EsaUNBQWlDLGlEQUFRO0FBQ3pDO0FBQ0E7QUFDQSxtQ0FBbUMsaURBQVE7QUFDM0M7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EsNkJBQTZCLGlEQUFRO0FBQ3JDO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FB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aW9uLXRlc3QtZWxlbWVudHMuanMiLCJzb3VyY2VzQ29udGVudCI6WyIgXHQvLyBUaGUgbW9kdWxlIGNhY2hlXG4gXHR2YXIgaW5zdGFsbGVkTW9kdWxlcyA9IHt9O1xuXG4gXHQvLyBUaGUgcmVxdWlyZSBmdW5jdGlvblxuIFx0ZnVuY3Rpb24gX193ZWJwYWNrX3JlcXVpcmVfXyhtb2R1bGVJZCkge1xuXG4gXHRcdC8vIENoZWNrIGlmIG1vZHVsZSBpcyBpbiBjYWNoZVxuIFx0XHRpZihpbnN0YWxsZWRNb2R1bGVzW21vZHVsZUlkXSkge1xuIFx0XHRcdHJldHVybiBpbnN0YWxsZWRNb2R1bGVzW21vZHVsZUlkXS5leHBvcnRzO1xuIFx0XHR9XG4gXHRcdC8vIENyZWF0ZSBhIG5ldyBtb2R1bGUgKGFuZCBwdXQgaXQgaW50byB0aGUgY2FjaGUpXG4gXHRcdHZhciBtb2R1bGUgPSBpbnN0YWxsZWRNb2R1bGVzW21vZHVsZUlkXSA9IHtcbiBcdFx0XHRpOiBtb2R1bGVJZCxcbiBcdFx0XHRsOiBmYWxzZSxcbiBcdFx0XHRleHBvcnRzOiB7fVxuIFx0XHR9O1xuXG4gXHRcdC8vIEV4ZWN1dGUgdGhlIG1vZHVsZSBmdW5jdGlvblxuIFx0XHRtb2R1bGVzW21vZHVsZUlkXS5jYWxsKG1vZHVsZS5leHBvcnRzLCBtb2R1bGUsIG1vZHVsZS5leHBvcnRzLCBfX3dlYnBhY2tfcmVxdWlyZV9fKTtcblxuIFx0XHQvLyBGbGFnIHRoZSBtb2R1bGUgYXMgbG9hZGVkXG4gXHRcdG1vZHVsZS5sID0gdHJ1ZTtcblxuIFx0XHQvLyBSZXR1cm4gdGhlIGV4cG9ydHMgb2YgdGhlIG1vZHVsZVxuIFx0XHRyZXR1cm4gbW9kdWxlLmV4cG9ydHM7XG4gXHR9XG5cblxuIFx0Ly8gZXhwb3NlIHRoZSBtb2R1bGVzIG9iamVjdCAoX193ZWJwYWNrX21vZHVsZXNfXylcbiBcdF9fd2VicGFja19yZXF1aXJlX18ubSA9IG1vZHVsZXM7XG5cbiBcdC8vIGV4cG9zZSB0aGUgbW9kdWxlIGNhY2hlXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLmMgPSBpbnN0YWxsZWRNb2R1bGVzO1xuXG4gXHQvLyBkZWZpbmUgZ2V0dGVyIGZ1bmN0aW9uIGZvciBoYXJtb255IGV4cG9ydHNcbiBcdF9fd2VicGFja19yZXF1aXJlX18uZCA9IGZ1bmN0aW9uKGV4cG9ydHMsIG5hbWUsIGdldHRlcikge1xuIFx0XHRpZighX193ZWJwYWNrX3JlcXVpcmVfXy5vKGV4cG9ydHMsIG5hbWUpKSB7XG4gXHRcdFx0T2JqZWN0LmRlZmluZVByb3BlcnR5KGV4cG9ydHMsIG5hbWUsIHsgZW51bWVyYWJsZTogdHJ1ZSwgZ2V0OiBnZXR0ZXIgfSk7XG4gXHRcdH1cbiBcdH07XG5cbiBcdC8vIGRlZmluZSBfX2VzTW9kdWxlIG9uIGV4cG9ydHNcbiBcdF9fd2VicGFja19yZXF1aXJlX18uciA9IGZ1bmN0aW9uKGV4cG9ydHMpIHtcbiBcdFx0aWYodHlwZW9mIFN5bWJvbCAhPT0gJ3VuZGVmaW5lZCcgJiYgU3ltYm9sLnRvU3RyaW5nVGFnKSB7XG4gXHRcdFx0T2JqZWN0LmRlZmluZVByb3BlcnR5KGV4cG9ydHMsIFN5bWJvbC50b1N0cmluZ1RhZywgeyB2YWx1ZTogJ01vZHVsZScgfSk7XG4gXHRcdH1cbiBcdFx0T2JqZWN0LmRlZmluZVByb3BlcnR5KGV4cG9ydHMsICdfX2VzTW9kdWxlJywgeyB2YWx1ZTogdHJ1ZSB9KTtcbiBcdH07XG5cbiBcdC8vIGNyZWF0ZSBhIGZha2UgbmFtZXNwYWNlIG9iamVjdFxuIFx0Ly8gbW9kZSAmIDE6IHZhbHVlIGlzIGEgbW9kdWxlIGlkLCByZXF1aXJlIGl0XG4gXHQvLyBtb2RlICYgMjogbWVyZ2UgYWxsIHByb3BlcnRpZXMgb2YgdmFsdWUgaW50byB0aGUgbnNcbiBcdC8vIG1vZGUgJiA0OiByZXR1cm4gdmFsdWUgd2hlbiBhbHJlYWR5IG5zIG9iamVjdFxuIFx0Ly8gbW9kZSAmIDh8MTogYmVoYXZlIGxpa2UgcmVxdWlyZVxuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy50ID0gZnVuY3Rpb24odmFsdWUsIG1vZGUpIHtcbiBcdFx0aWYobW9kZSAmIDEpIHZhbHVlID0gX193ZWJwYWNrX3JlcXVpcmVfXyh2YWx1ZSk7XG4gXHRcdGlmKG1vZGUgJiA4KSByZXR1cm4gdmFsdWU7XG4gXHRcdGlmKChtb2RlICYgNCkgJiYgdHlwZW9mIHZhbHVlID09PSAnb2JqZWN0JyAmJiB2YWx1ZSAmJiB2YWx1ZS5fX2VzTW9kdWxlKSByZXR1cm4gdmFsdWU7XG4gXHRcdHZhciBucyA9IE9iamVjdC5jcmVhdGUobnVsbCk7XG4gXHRcdF9fd2VicGFja19yZXF1aXJlX18ucihucyk7XG4gXHRcdE9iamVjdC5kZWZpbmVQcm9wZXJ0eShucywgJ2RlZmF1bHQnLCB7IGVudW1lcmFibGU6IHRydWUsIHZhbHVlOiB2YWx1ZSB9KTtcbiBcdFx0aWYobW9kZSAmIDIgJiYgdHlwZW9mIHZhbHVlICE9ICdzdHJpbmcnKSBmb3IodmFyIGtleSBpbiB2YWx1ZSkgX193ZWJwYWNrX3JlcXVpcmVfXy5kKG5zLCBrZXksIGZ1bmN0aW9uKGtleSkgeyByZXR1cm4gdmFsdWVba2V5XTsgfS5iaW5kKG51bGwsIGtleSkpO1xuIFx0XHRyZXR1cm4gbnM7XG4gXHR9O1xuXG4gXHQvLyBnZXREZWZhdWx0RXhwb3J0IGZ1bmN0aW9uIGZvciBjb21wYXRpYmlsaXR5IHdpdGggbm9uLWhhcm1vbnkgbW9kdWxlc1xuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5uID0gZnVuY3Rpb24obW9kdWxlKSB7XG4gXHRcdHZhciBnZXR0ZXIgPSBtb2R1bGUgJiYgbW9kdWxlLl9fZXNNb2R1bGUgP1xuIFx0XHRcdGZ1bmN0aW9uIGdldERlZmF1bHQoKSB7IHJldHVybiBtb2R1bGVbJ2RlZmF1bHQnXTsgfSA6XG4gXHRcdFx0ZnVuY3Rpb24gZ2V0TW9kdWxlRXhwb3J0cygpIHsgcmV0dXJuIG1vZHVsZTsgfTtcbiBcdFx0X193ZWJwYWNrX3JlcXVpcmVfXy5kKGdldHRlciwgJ2EnLCBnZXR0ZXIpO1xuIFx0XHRyZXR1cm4gZ2V0dGVyO1xuIFx0fTtcblxuIFx0Ly8gT2JqZWN0LnByb3RvdHlwZS5oYXNPd25Qcm9wZXJ0eS5jYWxsXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLm8gPSBmdW5jdGlvbihvYmplY3QsIHByb3BlcnR5KSB7IHJldHVybiBPYmplY3QucHJvdG90eXBlLmhhc093blByb3BlcnR5LmNhbGwob2JqZWN0LCBwcm9wZXJ0eSk7IH07XG5cbiBcdC8vIF9fd2VicGFja19wdWJsaWNfcGF0aF9fXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLnAgPSBcIlwiO1xuXG5cbiBcdC8vIExvYWQgZW50cnkgbW9kdWxlIGFuZCByZXR1cm4gZXhwb3J0c1xuIFx0cmV0dXJuIF9fd2VicGFja19yZXF1aXJlX18oX193ZWJwYWNrX3JlcXVpcmVfXy5zID0gXCIuL3NyYy9pbmRleC50c1wiKTtcbiIsImV4cG9ydHMgPSBtb2R1bGUuZXhwb3J0cyA9IHJlcXVpcmUoXCIuLi8uLi9jc3MtbG9hZGVyL2Rpc3QvcnVudGltZS9hcGkuanNcIikoZmFsc2UpO1xuLy8gTW9kdWxlXG5leHBvcnRzLnB1c2goW21vZHVsZS5pZCwgXCIvKlxcblxcbk9yaWdpbmFsIGhpZ2hsaWdodC5qcyBzdHlsZSAoYykgSXZhbiBTYWdhbGFldiA8bWFuaWFjQHNvZnR3YXJlbWFuaWFjcy5vcmc+XFxuXFxuKi9cXG5cXG4uaGxqcyB7XFxuICBkaXNwbGF5OiBibG9jaztcXG4gIG92ZXJmbG93LXg6IGF1dG87XFxuICBwYWRkaW5nOiAwLjVlbTtcXG4gIGJhY2tncm91bmQ6ICNGMEYwRjA7XFxufVxcblxcblxcbi8qIEJhc2UgY29sb3I6IHNhdHVyYXRpb24gMDsgKi9cXG5cXG4uaGxqcyxcXG4uaGxqcy1zdWJzdCB7XFxuICBjb2xvcjogIzQ0NDtcXG59XFxuXFxuLmhsanMtY29tbWVudCB7XFxuICBjb2xvcjogIzg4ODg4ODtcXG59XFxuXFxuLmhsanMta2V5d29yZCxcXG4uaGxqcy1hdHRyaWJ1dGUsXFxuLmhsanMtc2VsZWN0b3ItdGFnLFxcbi5obGpzLW1ldGEta2V5d29yZCxcXG4uaGxqcy1kb2N0YWcsXFxuLmhsanMtbmFtZSB7XFxuICBmb250LXdlaWdodDogYm9sZDtcXG59XFxuXFxuXFxuLyogVXNlciBjb2xvcjogaHVlOiAwICovXFxuXFxuLmhsanMtdHlwZSxcXG4uaGxqcy1zdHJpbmcsXFxuLmhsanMtbnVtYmVyLFxcbi5obGpzLXNlbGVjdG9yLWlkLFxcbi5obGpzLXNlbGVjdG9yLWNsYXNzLFxcbi5obGpzLXF1b3RlLFxcbi5obGpzLXRlbXBsYXRlLXRhZyxcXG4uaGxqcy1kZWxldGlvbiB7XFxuICBjb2xvcjogIzg4MDAwMDtcXG59XFxuXFxuLmhsanMtdGl0bGUsXFxuLmhsanMtc2VjdGlvbiB7XFxuICBjb2xvcjogIzg4MDAwMDtcXG4gIGZvbnQtd2VpZ2h0OiBib2xkO1xcbn1cXG5cXG4uaGxqcy1yZWdleHAsXFxuLmhsanMtc3ltYm9sLFxcbi5obGpzLXZhcmlhYmxlLFxcbi5obGpzLXRlbXBsYXRlLXZhcmlhYmxlLFxcbi5obGpzLWxpbmssXFxuLmhsanMtc2VsZWN0b3ItYXR0cixcXG4uaGxqcy1zZWxlY3Rvci1wc2V1ZG8ge1xcbiAgY29sb3I6ICNCQzYwNjA7XFxufVxcblxcblxcbi8qIExhbmd1YWdlIGNvbG9yOiBodWU6IDkwOyAqL1xcblxcbi5obGpzLWxpdGVyYWwge1xcbiAgY29sb3I6ICM3OEE5NjA7XFxufVxcblxcbi5obGpzLWJ1aWx0X2luLFxcbi5obGpzLWJ1bGxldCxcXG4uaGxqcy1jb2RlLFxcbi5obGpzLWFkZGl0aW9uIHtcXG4gIGNvbG9yOiAjMzk3MzAwO1xcbn1cXG5cXG5cXG4vKiBNZXRhIGNvbG9yOiBodWU6IDIwMCAqL1xcblxcbi5obGpzLW1ldGEge1xcbiAgY29sb3I6ICMxZjcxOTk7XFxufVxcblxcbi5obGpzLW1ldGEtc3RyaW5nIHtcXG4gIGNvbG9yOiAjNGQ5OWJmO1xcbn1cXG5cXG5cXG4vKiBNaXNjIGVmZmVjdHMgKi9cXG5cXG4uaGxqcy1lbXBoYXNpcyB7XFxuICBmb250LXN0eWxlOiBpdGFsaWM7XFxufVxcblxcbi5obGpzLXN0cm9uZyB7XFxuICBmb250LXdlaWdodDogYm9sZDtcXG59XFxuXCIsIFwiXCJdKTtcblxuIiwiXCJ1c2Ugc3RyaWN0XCI7XG5cbi8qXG4gIE1JVCBMaWNlbnNlIGh0dHA6Ly93d3cub3BlbnNvdXJjZS5vcmcvbGljZW5zZXMvbWl0LWxpY2Vuc2UucGhwXG4gIEF1dGhvciBUb2JpYXMgS29wcGVycyBAc29rcmFcbiovXG4vLyBjc3MgYmFzZSBjb2RlLCBpbmplY3RlZCBieSB0aGUgY3NzLWxvYWRlclxubW9kdWxlLmV4cG9ydHMgPSBmdW5jdGlvbiAodXNlU291cmNlTWFwKSB7XG4gIHZhciBsaXN0ID0gW107IC8vIHJldHVybiB0aGUgbGlzdCBvZiBtb2R1bGVzIGFzIGNzcyBzdHJpbmdcblxuICBsaXN0LnRvU3RyaW5nID0gZnVuY3Rpb24gdG9TdHJpbmcoKSB7XG4gICAgcmV0dXJuIHRoaXMubWFwKGZ1bmN0aW9uIChpdGVtKSB7XG4gICAgICB2YXIgY29udGVudCA9IGNzc1dpdGhNYXBwaW5nVG9TdHJpbmcoaXRlbSwgdXNlU291cmNlTWFwKTtcblxuICAgICAgaWYgKGl0ZW1bMl0pIHtcbiAgICAgICAgcmV0dXJuICdAbWVkaWEgJyArIGl0ZW1bMl0gKyAneycgKyBjb250ZW50ICsgJ30nO1xuICAgICAgfSBlbHNlIHtcbiAgICAgICAgcmV0dXJuIGNvbnRlbnQ7XG4gICAgICB9XG4gICAgfSkuam9pbignJyk7XG4gIH07IC8vIGltcG9ydCBhIGxpc3Qgb2YgbW9kdWxlcyBpbnRvIHRoZSBsaXN0XG5cblxuICBsaXN0LmkgPSBmdW5jdGlvbiAobW9kdWxlcywgbWVkaWFRdWVyeSkge1xuICAgIGlmICh0eXBlb2YgbW9kdWxlcyA9PT0gJ3N0cmluZycpIHtcbiAgICAgIG1vZHVsZXMgPSBbW251bGwsIG1vZHVsZXMsICcnXV07XG4gICAgfVxuXG4gICAgdmFyIGFscmVhZHlJbXBvcnRlZE1vZHVsZXMgPSB7fTtcblxuICAgIGZvciAodmFyIGkgPSAwOyBpIDwgdGhpcy5sZW5ndGg7IGkrKykge1xuICAgICAgdmFyIGlkID0gdGhpc1tpXVswXTtcblxuICAgICAgaWYgKGlkICE9IG51bGwpIHtcbiAgICAgICAgYWxyZWFkeUltcG9ydGVkTW9kdWxlc1tpZF0gPSB0cnVlO1xuICAgICAgfVxuICAgIH1cblxuICAgIGZvciAoaSA9IDA7IGkgPCBtb2R1bGVzLmxlbmd0aDsgaSsrKSB7XG4gICAgICB2YXIgaXRlbSA9IG1vZHVsZXNbaV07IC8vIHNraXAgYWxyZWFkeSBpbXBvcnRlZCBtb2R1bGVcbiAgICAgIC8vIHRoaXMgaW1wbGVtZW50YXRpb24gaXMgbm90IDEwMCUgcGVyZmVjdCBmb3Igd2VpcmQgbWVkaWEgcXVlcnkgY29tYmluYXRpb25zXG4gICAgICAvLyB3aGVuIGEgbW9kdWxlIGlzIGltcG9ydGVkIG11bHRpcGxlIHRpbWVzIHdpdGggZGlmZmVyZW50IG1lZGlhIHF1ZXJpZXMuXG4gICAgICAvLyBJIGhvcGUgdGhpcyB3aWxsIG5ldmVyIG9jY3VyIChIZXkgdGhpcyB3YXkgd2UgaGF2ZSBzbWFsbGVyIGJ1bmRsZXMpXG5cbiAgICAgIGlmIChpdGVtWzBdID09IG51bGwgfHwgIWFscmVhZHlJbXBvcnRlZE1vZHVsZXNbaXRlbVswXV0pIHtcbiAgICAgICAgaWYgKG1lZGlhUXVlcnkgJiYgIWl0ZW1bMl0pIHtcbiAgICAgICAgICBpdGVtWzJdID0gbWVkaWFRdWVyeTtcbiAgICAgICAgfSBlbHNlIGlmIChtZWRpYVF1ZXJ5KSB7XG4gICAgICAgICAgaXRlbVsyXSA9ICcoJyArIGl0ZW1bMl0gKyAnKSBhbmQgKCcgKyBtZWRpYVF1ZXJ5ICsgJyknO1xuICAgICAgICB9XG5cbiAgICAgICAgbGlzdC5wdXNoKGl0ZW0pO1xuICAgICAgfVxuICAgIH1cbiAgfTtcblxuICByZXR1cm4gbGlzdDtcbn07XG5cbmZ1bmN0aW9uIGNzc1dpdGhNYXBwaW5nVG9TdHJpbmcoaXRlbSwgdXNlU291cmNlTWFwKSB7XG4gIHZhciBjb250ZW50ID0gaXRlbVsxXSB8fCAnJztcbiAgdmFyIGNzc01hcHBpbmcgPSBpdGVtWzNdO1xuXG4gIGlmICghY3NzTWFwcGluZykge1xuICAgIHJldHVybiBjb250ZW50O1xuICB9XG5cbiAgaWYgKHVzZVNvdXJjZU1hcCAmJiB0eXBlb2YgYnRvYSA9PT0gJ2Z1bmN0aW9uJykge1xuICAgIHZhciBzb3VyY2VNYXBwaW5nID0gdG9Db21tZW50KGNzc01hcHBpbmcpO1xuICAgIHZhciBzb3VyY2VVUkxzID0gY3NzTWFwcGluZy5zb3VyY2VzLm1hcChmdW5jdGlvbiAoc291cmNlKSB7XG4gICAgICByZXR1cm4gJy8qIyBzb3VyY2VVUkw9JyArIGNzc01hcHBpbmcuc291cmNlUm9vdCArIHNvdXJjZSArICcgKi8nO1xuICAgIH0pO1xuICAgIHJldHVybiBbY29udGVudF0uY29uY2F0KHNvdXJjZVVSTHMpLmNvbmNhdChbc291cmNlTWFwcGluZ10pLmpvaW4oJ1xcbicpO1xuICB9XG5cbiAgcmV0dXJuIFtjb250ZW50XS5qb2luKCdcXG4nKTtcbn0gLy8gQWRhcHRlZCBmcm9tIGNvbnZlcnQtc291cmNlLW1hcCAoTUlUKVxuXG5cbmZ1bmN0aW9uIHRvQ29tbWVudChzb3VyY2VNYXApIHtcbiAgLy8gZXNsaW50LWRpc2FibGUtbmV4dC1saW5lIG5vLXVuZGVmXG4gIHZhciBiYXNlNjQgPSBidG9hKHVuZXNjYXBlKGVuY29kZVVSSUNvbXBvbmVudChKU09OLnN0cmluZ2lmeShzb3VyY2VNYXApKSkpO1xuICB2YXIgZGF0YSA9ICdzb3VyY2VNYXBwaW5nVVJMPWRhdGE6YXBwbGljYXRpb24vanNvbjtjaGFyc2V0PXV0Zi04O2Jhc2U2NCwnICsgYmFzZTY0O1xuICByZXR1cm4gJy8qIyAnICsgZGF0YSArICcgKi8nO1xufSIsIi8qXG5TeW50YXggaGlnaGxpZ2h0aW5nIHdpdGggbGFuZ3VhZ2UgYXV0b2RldGVjdGlvbi5cbmh0dHBzOi8vaGlnaGxpZ2h0anMub3JnL1xuKi9cblxuKGZ1bmN0aW9uKGZhY3RvcnkpIHtcblxuICAvLyBGaW5kIHRoZSBnbG9iYWwgb2JqZWN0IGZvciBleHBvcnQgdG8gYm90aCB0aGUgYnJvd3NlciBhbmQgd2ViIHdvcmtlcnMuXG4gIHZhciBnbG9iYWxPYmplY3QgPSB0eXBlb2Ygd2luZG93ID09PSAnb2JqZWN0JyAmJiB3aW5kb3cgfHxcbiAgICAgICAgICAgICAgICAgICAgIHR5cGVvZiBzZWxmID09PSAnb2JqZWN0JyAmJiBzZWxmO1xuXG4gIC8vIFNldHVwIGhpZ2hsaWdodC5qcyBmb3IgZGlmZmVyZW50IGVudmlyb25tZW50cy4gRmlyc3QgaXMgTm9kZS5qcyBvclxuICAvLyBDb21tb25KUy5cbiAgaWYodHlwZW9mIGV4cG9ydHMgIT09ICd1bmRlZmluZWQnKSB7XG4gICAgZmFjdG9yeShleHBvcnRzKTtcbiAgfSBlbHNlIGlmKGdsb2JhbE9iamVjdCkge1xuICAgIC8vIEV4cG9ydCBobGpzIGdsb2JhbGx5IGV2ZW4gd2hlbiB1c2luZyBBTUQgZm9yIGNhc2VzIHdoZW4gdGhpcyBzY3JpcHRcbiAgICAvLyBpcyBsb2FkZWQgd2l0aCBvdGhlcnMgdGhhdCBtYXkgc3RpbGwgZXhwZWN0IGEgZ2xvYmFsIGhsanMuXG4gICAgZ2xvYmFsT2JqZWN0LmhsanMgPSBmYWN0b3J5KHt9KTtcblxuICAgIC8vIEZpbmFsbHkgcmVnaXN0ZXIgdGhlIGdsb2JhbCBobGpzIHdpdGggQU1ELlxuICAgIGlmKHR5cGVvZiBkZWZpbmUgPT09ICdmdW5jdGlvbicgJiYgZGVmaW5lLmFtZCkge1xuICAgICAgZGVmaW5lKFtdLCBmdW5jdGlvbigpIHtcbiAgICAgICAgcmV0dXJuIGdsb2JhbE9iamVjdC5obGpzO1xuICAgICAgfSk7XG4gICAgfVxuICB9XG5cbn0oZnVuY3Rpb24oaGxqcykge1xuICAvLyBDb252ZW5pZW5jZSB2YXJpYWJsZXMgZm9yIGJ1aWxkLWluIG9iamVjdHNcbiAgdmFyIEFycmF5UHJvdG8gPSBbXSxcbiAgICAgIG9iamVjdEtleXMgPSBPYmplY3Qua2V5cztcblxuICAvLyBHbG9iYWwgaW50ZXJuYWwgdmFyaWFibGVzIHVzZWQgd2l0aGluIHRoZSBoaWdobGlnaHQuanMgbGlicmFyeS5cbiAgdmFyIGxhbmd1YWdlcyA9IHt9LFxuICAgICAgYWxpYXNlcyAgID0ge307XG5cbiAgLy8gUmVndWxhciBleHByZXNzaW9ucyB1c2VkIHRocm91Z2hvdXQgdGhlIGhpZ2hsaWdodC5qcyBsaWJyYXJ5LlxuICB2YXIgbm9IaWdobGlnaHRSZSAgICA9IC9eKG5vLT9oaWdobGlnaHR8cGxhaW58dGV4dCkkL2ksXG4gICAgICBsYW5ndWFnZVByZWZpeFJlID0gL1xcYmxhbmcoPzp1YWdlKT8tKFtcXHctXSspXFxiL2ksXG4gICAgICBmaXhNYXJrdXBSZSAgICAgID0gLygoXig8W14+XSs+fFxcdHwpK3woPzpcXG4pKSkvZ207XG5cbiAgdmFyIHNwYW5FbmRUYWcgPSAnPC9zcGFuPic7XG5cbiAgLy8gR2xvYmFsIG9wdGlvbnMgdXNlZCB3aGVuIHdpdGhpbiBleHRlcm5hbCBBUElzLiBUaGlzIGlzIG1vZGlmaWVkIHdoZW5cbiAgLy8gY2FsbGluZyB0aGUgYGhsanMuY29uZmlndXJlYCBmdW5jdGlvbi5cbiAgdmFyIG9wdGlvbnMgPSB7XG4gICAgY2xhc3NQcmVmaXg6ICdobGpzLScsXG4gICAgdGFiUmVwbGFjZTogbnVsbCxcbiAgICB1c2VCUjogZmFsc2UsXG4gICAgbGFuZ3VhZ2VzOiB1bmRlZmluZWRcbiAgfTtcblxuXG4gIC8qIFV0aWxpdHkgZnVuY3Rpb25zICovXG5cbiAgZnVuY3Rpb24gZXNjYXBlKHZhbHVlKSB7XG4gICAgcmV0dXJuIHZhbHVlLnJlcGxhY2UoLyYvZywgJyZhbXA7JykucmVwbGFjZSgvPC9nLCAnJmx0OycpLnJlcGxhY2UoLz4vZywgJyZndDsnKTtcbiAgfVxuXG4gIGZ1bmN0aW9uIHRhZyhub2RlKSB7XG4gICAgcmV0dXJuIG5vZGUubm9kZU5hbWUudG9Mb3dlckNhc2UoKTtcbiAgfVxuXG4gIGZ1bmN0aW9uIHRlc3RSZShyZSwgbGV4ZW1lKSB7XG4gICAgdmFyIG1hdGNoID0gcmUgJiYgcmUuZXhlYyhsZXhlbWUpO1xuICAgIHJldHVybiBtYXRjaCAmJiBtYXRjaC5pbmRleCA9PT0gMDtcbiAgfVxuXG4gIGZ1bmN0aW9uIGlzTm90SGlnaGxpZ2h0ZWQobGFuZ3VhZ2UpIHtcbiAgICByZXR1cm4gbm9IaWdobGlnaHRSZS50ZXN0KGxhbmd1YWdlKTtcbiAgfVxuXG4gIGZ1bmN0aW9uIGJsb2NrTGFuZ3VhZ2UoYmxvY2spIHtcbiAgICB2YXIgaSwgbWF0Y2gsIGxlbmd0aCwgX2NsYXNzO1xuICAgIHZhciBjbGFzc2VzID0gYmxvY2suY2xhc3NOYW1lICsgJyAnO1xuXG4gICAgY2xhc3NlcyArPSBibG9jay5wYXJlbnROb2RlID8gYmxvY2sucGFyZW50Tm9kZS5jbGFzc05hbWUgOiAnJztcblxuICAgIC8vIGxhbmd1YWdlLSogdGFrZXMgcHJlY2VkZW5jZSBvdmVyIG5vbi1wcmVmaXhlZCBjbGFzcyBuYW1lcy5cbiAgICBtYXRjaCA9IGxhbmd1YWdlUHJlZml4UmUuZXhlYyhjbGFzc2VzKTtcbiAgICBpZiAobWF0Y2gpIHtcbiAgICAgIHJldHVybiBnZXRMYW5ndWFnZShtYXRjaFsxXSkgPyBtYXRjaFsxXSA6ICduby1oaWdobGlnaHQnO1xuICAgIH1cblxuICAgIGNsYXNzZXMgPSBjbGFzc2VzLnNwbGl0KC9cXHMrLyk7XG5cbiAgICBmb3IgKGkgPSAwLCBsZW5ndGggPSBjbGFzc2VzLmxlbmd0aDsgaSA8IGxlbmd0aDsgaSsrKSB7XG4gICAgICBfY2xhc3MgPSBjbGFzc2VzW2ldO1xuXG4gICAgICBpZiAoaXNOb3RIaWdobGlnaHRlZChfY2xhc3MpIHx8IGdldExhbmd1YWdlKF9jbGFzcykpIHtcbiAgICAgICAgcmV0dXJuIF9jbGFzcztcbiAgICAgIH1cbiAgICB9XG4gIH1cblxuICBmdW5jdGlvbiBpbmhlcml0KHBhcmVudCkgeyAgLy8gaW5oZXJpdChwYXJlbnQsIG92ZXJyaWRlX29iaiwgb3ZlcnJpZGVfb2JqLCAuLi4pXG4gICAgdmFyIGtleTtcbiAgICB2YXIgcmVzdWx0ID0ge307XG4gICAgdmFyIG9iamVjdHMgPSBBcnJheS5wcm90b3R5cGUuc2xpY2UuY2FsbChhcmd1bWVudHMsIDEpO1xuXG4gICAgZm9yIChrZXkgaW4gcGFyZW50KVxuICAgICAgcmVzdWx0W2tleV0gPSBwYXJlbnRba2V5XTtcbiAgICBvYmplY3RzLmZvckVhY2goZnVuY3Rpb24ob2JqKSB7XG4gICAgICBmb3IgKGtleSBpbiBvYmopXG4gICAgICAgIHJlc3VsdFtrZXldID0gb2JqW2tleV07XG4gICAgfSk7XG4gICAgcmV0dXJuIHJlc3VsdDtcbiAgfVxuXG4gIC8qIFN0cmVhbSBtZXJnaW5nICovXG5cbiAgZnVuY3Rpb24gbm9kZVN0cmVhbShub2RlKSB7XG4gICAgdmFyIHJlc3VsdCA9IFtdO1xuICAgIChmdW5jdGlvbiBfbm9kZVN0cmVhbShub2RlLCBvZmZzZXQpIHtcbiAgICAgIGZvciAodmFyIGNoaWxkID0gbm9kZS5maXJzdENoaWxkOyBjaGlsZDsgY2hpbGQgPSBjaGlsZC5uZXh0U2libGluZykge1xuICAgICAgICBpZiAoY2hpbGQubm9kZVR5cGUgPT09IDMpXG4gICAgICAgICAgb2Zmc2V0ICs9IGNoaWxkLm5vZGVWYWx1ZS5sZW5ndGg7XG4gICAgICAgIGVsc2UgaWYgKGNoaWxkLm5vZGVUeXBlID09PSAxKSB7XG4gICAgICAgICAgcmVzdWx0LnB1c2goe1xuICAgICAgICAgICAgZXZlbnQ6ICdzdGFydCcsXG4gICAgICAgICAgICBvZmZzZXQ6IG9mZnNldCxcbiAgICAgICAgICAgIG5vZGU6IGNoaWxkXG4gICAgICAgICAgfSk7XG4gICAgICAgICAgb2Zmc2V0ID0gX25vZGVTdHJlYW0oY2hpbGQsIG9mZnNldCk7XG4gICAgICAgICAgLy8gUHJldmVudCB2b2lkIGVsZW1lbnRzIGZyb20gaGF2aW5nIGFuIGVuZCB0YWcgdGhhdCB3b3VsZCBhY3R1YWxseVxuICAgICAgICAgIC8vIGRvdWJsZSB0aGVtIGluIHRoZSBvdXRwdXQuIFRoZXJlIGFyZSBtb3JlIHZvaWQgZWxlbWVudHMgaW4gSFRNTFxuICAgICAgICAgIC8vIGJ1dCB3ZSBsaXN0IG9ubHkgdGhvc2UgcmVhbGlzdGljYWxseSBleHBlY3RlZCBpbiBjb2RlIGRpc3BsYXkuXG4gICAgICAgICAgaWYgKCF0YWcoY2hpbGQpLm1hdGNoKC9icnxocnxpbWd8aW5wdXQvKSkge1xuICAgICAgICAgICAgcmVzdWx0LnB1c2goe1xuICAgICAgICAgICAgICBldmVudDogJ3N0b3AnLFxuICAgICAgICAgICAgICBvZmZzZXQ6IG9mZnNldCxcbiAgICAgICAgICAgICAgbm9kZTogY2hpbGRcbiAgICAgICAgICAgIH0pO1xuICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgICAgfVxuICAgICAgcmV0dXJuIG9mZnNldDtcbiAgICB9KShub2RlLCAwKTtcbiAgICByZXR1cm4gcmVzdWx0O1xuICB9XG5cbiAgZnVuY3Rpb24gbWVyZ2VTdHJlYW1zKG9yaWdpbmFsLCBoaWdobGlnaHRlZCwgdmFsdWUpIHtcbiAgICB2YXIgcHJvY2Vzc2VkID0gMDtcbiAgICB2YXIgcmVzdWx0ID0gJyc7XG4gICAgdmFyIG5vZGVTdGFjayA9IFtdO1xuXG4gICAgZnVuY3Rpb24gc2VsZWN0U3RyZWFtKCkge1xuICAgICAgaWYgKCFvcmlnaW5hbC5sZW5ndGggfHwgIWhpZ2hsaWdodGVkLmxlbmd0aCkge1xuICAgICAgICByZXR1cm4gb3JpZ2luYWwubGVuZ3RoID8gb3JpZ2luYWwgOiBoaWdobGlnaHRlZDtcbiAgICAgIH1cbiAgICAgIGlmIChvcmlnaW5hbFswXS5vZmZzZXQgIT09IGhpZ2hsaWdodGVkWzBdLm9mZnNldCkge1xuICAgICAgICByZXR1cm4gKG9yaWdpbmFsWzBdLm9mZnNldCA8IGhpZ2hsaWdodGVkWzBdLm9mZnNldCkgPyBvcmlnaW5hbCA6IGhpZ2hsaWdodGVkO1xuICAgICAgfVxuXG4gICAgICAvKlxuICAgICAgVG8gYXZvaWQgc3RhcnRpbmcgdGhlIHN0cmVhbSBqdXN0IGJlZm9yZSBpdCBzaG91bGQgc3RvcCB0aGUgb3JkZXIgaXNcbiAgICAgIGVuc3VyZWQgdGhhdCBvcmlnaW5hbCBhbHdheXMgc3RhcnRzIGZpcnN0IGFuZCBjbG9zZXMgbGFzdDpcblxuICAgICAgaWYgKGV2ZW50MSA9PSAnc3RhcnQnICYmIGV2ZW50MiA9PSAnc3RhcnQnKVxuICAgICAgICByZXR1cm4gb3JpZ2luYWw7XG4gICAgICBpZiAoZXZlbnQxID09ICdzdGFydCcgJiYgZXZlbnQyID09ICdzdG9wJylcbiAgICAgICAgcmV0dXJuIGhpZ2hsaWdodGVkO1xuICAgICAgaWYgKGV2ZW50MSA9PSAnc3RvcCcgJiYgZXZlbnQyID09ICdzdGFydCcpXG4gICAgICAgIHJldHVybiBvcmlnaW5hbDtcbiAgICAgIGlmIChldmVudDEgPT0gJ3N0b3AnICYmIGV2ZW50MiA9PSAnc3RvcCcpXG4gICAgICAgIHJldHVybiBoaWdobGlnaHRlZDtcblxuICAgICAgLi4uIHdoaWNoIGlzIGNvbGxhcHNlZCB0bzpcbiAgICAgICovXG4gICAgICByZXR1cm4gaGlnaGxpZ2h0ZWRbMF0uZXZlbnQgPT09ICdzdGFydCcgPyBvcmlnaW5hbCA6IGhpZ2hsaWdodGVkO1xuICAgIH1cblxuICAgIGZ1bmN0aW9uIG9wZW4obm9kZSkge1xuICAgICAgZnVuY3Rpb24gYXR0cl9zdHIoYSkge3JldHVybiAnICcgKyBhLm5vZGVOYW1lICsgJz1cIicgKyBlc2NhcGUoYS52YWx1ZSkucmVwbGFjZSgnXCInLCAnJnF1b3Q7JykgKyAnXCInO31cbiAgICAgIHJlc3VsdCArPSAnPCcgKyB0YWcobm9kZSkgKyBBcnJheVByb3RvLm1hcC5jYWxsKG5vZGUuYXR0cmlidXRlcywgYXR0cl9zdHIpLmpvaW4oJycpICsgJz4nO1xuICAgIH1cblxuICAgIGZ1bmN0aW9uIGNsb3NlKG5vZGUpIHtcbiAgICAgIHJlc3VsdCArPSAnPC8nICsgdGFnKG5vZGUpICsgJz4nO1xuICAgIH1cblxuICAgIGZ1bmN0aW9uIHJlbmRlcihldmVudCkge1xuICAgICAgKGV2ZW50LmV2ZW50ID09PSAnc3RhcnQnID8gb3BlbiA6IGNsb3NlKShldmVudC5ub2RlKTtcbiAgICB9XG5cbiAgICB3aGlsZSAob3JpZ2luYWwubGVuZ3RoIHx8IGhpZ2hsaWdodGVkLmxlbmd0aCkge1xuICAgICAgdmFyIHN0cmVhbSA9IHNlbGVjdFN0cmVhbSgpO1xuICAgICAgcmVzdWx0ICs9IGVzY2FwZSh2YWx1ZS5zdWJzdHJpbmcocHJvY2Vzc2VkLCBzdHJlYW1bMF0ub2Zmc2V0KSk7XG4gICAgICBwcm9jZXNzZWQgPSBzdHJlYW1bMF0ub2Zmc2V0O1xuICAgICAgaWYgKHN0cmVhbSA9PT0gb3JpZ2luYWwpIHtcbiAgICAgICAgLypcbiAgICAgICAgT24gYW55IG9wZW5pbmcgb3IgY2xvc2luZyB0YWcgb2YgdGhlIG9yaWdpbmFsIG1hcmt1cCB3ZSBmaXJzdCBjbG9zZVxuICAgICAgICB0aGUgZW50aXJlIGhpZ2hsaWdodGVkIG5vZGUgc3RhY2ssIHRoZW4gcmVuZGVyIHRoZSBvcmlnaW5hbCB0YWcgYWxvbmdcbiAgICAgICAgd2l0aCBhbGwgdGhlIGZvbGxvd2luZyBvcmlnaW5hbCB0YWdzIGF0IHRoZSBzYW1lIG9mZnNldCBhbmQgdGhlblxuICAgICAgICByZW9wZW4gYWxsIHRoZSB0YWdzIG9uIHRoZSBoaWdobGlnaHRlZCBzdGFjay5cbiAgICAgICAgKi9cbiAgICAgICAgbm9kZVN0YWNrLnJldmVyc2UoKS5mb3JFYWNoKGNsb3NlKTtcbiAgICAgICAgZG8ge1xuICAgICAgICAgIHJlbmRlcihzdHJlYW0uc3BsaWNlKDAsIDEpWzBdKTtcbiAgICAgICAgICBzdHJlYW0gPSBzZWxlY3RTdHJlYW0oKTtcbiAgICAgICAgfSB3aGlsZSAoc3RyZWFtID09PSBvcmlnaW5hbCAmJiBzdHJlYW0ubGVuZ3RoICYmIHN0cmVhbVswXS5vZmZzZXQgPT09IHByb2Nlc3NlZCk7XG4gICAgICAgIG5vZGVTdGFjay5yZXZlcnNlKCkuZm9yRWFjaChvcGVuKTtcbiAgICAgIH0gZWxzZSB7XG4gICAgICAgIGlmIChzdHJlYW1bMF0uZXZlbnQgPT09ICdzdGFydCcpIHtcbiAgICAgICAgICBub2RlU3RhY2sucHVzaChzdHJlYW1bMF0ubm9kZSk7XG4gICAgICAgIH0gZWxzZSB7XG4gICAgICAgICAgbm9kZVN0YWNrLnBvcCgpO1xuICAgICAgICB9XG4gICAgICAgIHJlbmRlcihzdHJlYW0uc3BsaWNlKDAsIDEpWzBdKTtcbiAgICAgIH1cbiAgICB9XG4gICAgcmV0dXJuIHJlc3VsdCArIGVzY2FwZSh2YWx1ZS5zdWJzdHIocHJvY2Vzc2VkKSk7XG4gIH1cblxuICAvKiBJbml0aWFsaXphdGlvbiAqL1xuXG4gIGZ1bmN0aW9uIGV4cGFuZF9tb2RlKG1vZGUpIHtcbiAgICBpZiAobW9kZS52YXJpYW50cyAmJiAhbW9kZS5jYWNoZWRfdmFyaWFudHMpIHtcbiAgICAgIG1vZGUuY2FjaGVkX3ZhcmlhbnRzID0gbW9kZS52YXJpYW50cy5tYXAoZnVuY3Rpb24odmFyaWFudCkge1xuICAgICAgICByZXR1cm4gaW5oZXJpdChtb2RlLCB7dmFyaWFudHM6IG51bGx9LCB2YXJpYW50KTtcbiAgICAgIH0pO1xuICAgIH1cbiAgICByZXR1cm4gbW9kZS5jYWNoZWRfdmFyaWFudHMgfHwgKG1vZGUuZW5kc1dpdGhQYXJlbnQgJiYgW2luaGVyaXQobW9kZSldKSB8fCBbbW9kZV07XG4gIH1cblxuICBmdW5jdGlvbiBjb21waWxlTGFuZ3VhZ2UobGFuZ3VhZ2UpIHtcblxuICAgIGZ1bmN0aW9uIHJlU3RyKHJlKSB7XG4gICAgICAgIHJldHVybiAocmUgJiYgcmUuc291cmNlKSB8fCByZTtcbiAgICB9XG5cbiAgICBmdW5jdGlvbiBsYW5nUmUodmFsdWUsIGdsb2JhbCkge1xuICAgICAgcmV0dXJuIG5ldyBSZWdFeHAoXG4gICAgICAgIHJlU3RyKHZhbHVlKSxcbiAgICAgICAgJ20nICsgKGxhbmd1YWdlLmNhc2VfaW5zZW5zaXRpdmUgPyAnaScgOiAnJykgKyAoZ2xvYmFsID8gJ2cnIDogJycpXG4gICAgICApO1xuICAgIH1cblxuICAgIGZ1bmN0aW9uIGNvbXBpbGVNb2RlKG1vZGUsIHBhcmVudCkge1xuICAgICAgaWYgKG1vZGUuY29tcGlsZWQpXG4gICAgICAgIHJldHVybjtcbiAgICAgIG1vZGUuY29tcGlsZWQgPSB0cnVlO1xuXG4gICAgICBtb2RlLmtleXdvcmRzID0gbW9kZS5rZXl3b3JkcyB8fCBtb2RlLmJlZ2luS2V5d29yZHM7XG4gICAgICBpZiAobW9kZS5rZXl3b3Jkcykge1xuICAgICAgICB2YXIgY29tcGlsZWRfa2V5d29yZHMgPSB7fTtcblxuICAgICAgICB2YXIgZmxhdHRlbiA9IGZ1bmN0aW9uKGNsYXNzTmFtZSwgc3RyKSB7XG4gICAgICAgICAgaWYgKGxhbmd1YWdlLmNhc2VfaW5zZW5zaXRpdmUpIHtcbiAgICAgICAgICAgIHN0ciA9IHN0ci50b0xvd2VyQ2FzZSgpO1xuICAgICAgICAgIH1cbiAgICAgICAgICBzdHIuc3BsaXQoJyAnKS5mb3JFYWNoKGZ1bmN0aW9uKGt3KSB7XG4gICAgICAgICAgICB2YXIgcGFpciA9IGt3LnNwbGl0KCd8Jyk7XG4gICAgICAgICAgICBjb21waWxlZF9rZXl3b3Jkc1twYWlyWzBdXSA9IFtjbGFzc05hbWUsIHBhaXJbMV0gPyBOdW1iZXIocGFpclsxXSkgOiAxXTtcbiAgICAgICAgICB9KTtcbiAgICAgICAgfTtcblxuICAgICAgICBpZiAodHlwZW9mIG1vZGUua2V5d29yZHMgPT09ICdzdHJpbmcnKSB7IC8vIHN0cmluZ1xuICAgICAgICAgIGZsYXR0ZW4oJ2tleXdvcmQnLCBtb2RlLmtleXdvcmRzKTtcbiAgICAgICAgfSBlbHNlIHtcbiAgICAgICAgICBvYmplY3RLZXlzKG1vZGUua2V5d29yZHMpLmZvckVhY2goZnVuY3Rpb24gKGNsYXNzTmFtZSkge1xuICAgICAgICAgICAgZmxhdHRlbihjbGFzc05hbWUsIG1vZGUua2V5d29yZHNbY2xhc3NOYW1lXSk7XG4gICAgICAgICAgfSk7XG4gICAgICAgIH1cbiAgICAgICAgbW9kZS5rZXl3b3JkcyA9IGNvbXBpbGVkX2tleXdvcmRzO1xuICAgICAgfVxuICAgICAgbW9kZS5sZXhlbWVzUmUgPSBsYW5nUmUobW9kZS5sZXhlbWVzIHx8IC9cXHcrLywgdHJ1ZSk7XG5cbiAgICAgIGlmIChwYXJlbnQpIHtcbiAgICAgICAgaWYgKG1vZGUuYmVnaW5LZXl3b3Jkcykge1xuICAgICAgICAgIG1vZGUuYmVnaW4gPSAnXFxcXGIoJyArIG1vZGUuYmVnaW5LZXl3b3Jkcy5zcGxpdCgnICcpLmpvaW4oJ3wnKSArICcpXFxcXGInO1xuICAgICAgICB9XG4gICAgICAgIGlmICghbW9kZS5iZWdpbilcbiAgICAgICAgICBtb2RlLmJlZ2luID0gL1xcQnxcXGIvO1xuICAgICAgICBtb2RlLmJlZ2luUmUgPSBsYW5nUmUobW9kZS5iZWdpbik7XG4gICAgICAgIGlmIChtb2RlLmVuZFNhbWVBc0JlZ2luKVxuICAgICAgICAgIG1vZGUuZW5kID0gbW9kZS5iZWdpbjtcbiAgICAgICAgaWYgKCFtb2RlLmVuZCAmJiAhbW9kZS5lbmRzV2l0aFBhcmVudClcbiAgICAgICAgICBtb2RlLmVuZCA9IC9cXEJ8XFxiLztcbiAgICAgICAgaWYgKG1vZGUuZW5kKVxuICAgICAgICAgIG1vZGUuZW5kUmUgPSBsYW5nUmUobW9kZS5lbmQpO1xuICAgICAgICBtb2RlLnRlcm1pbmF0b3JfZW5kID0gcmVTdHIobW9kZS5lbmQpIHx8ICcnO1xuICAgICAgICBpZiAobW9kZS5lbmRzV2l0aFBhcmVudCAmJiBwYXJlbnQudGVybWluYXRvcl9lbmQpXG4gICAgICAgICAgbW9kZS50ZXJtaW5hdG9yX2VuZCArPSAobW9kZS5lbmQgPyAnfCcgOiAnJykgKyBwYXJlbnQudGVybWluYXRvcl9lbmQ7XG4gICAgICB9XG4gICAgICBpZiAobW9kZS5pbGxlZ2FsKVxuICAgICAgICBtb2RlLmlsbGVnYWxSZSA9IGxhbmdSZShtb2RlLmlsbGVnYWwpO1xuICAgICAgaWYgKG1vZGUucmVsZXZhbmNlID09IG51bGwpXG4gICAgICAgIG1vZGUucmVsZXZhbmNlID0gMTtcbiAgICAgIGlmICghbW9kZS5jb250YWlucykge1xuICAgICAgICBtb2RlLmNvbnRhaW5zID0gW107XG4gICAgICB9XG4gICAgICBtb2RlLmNvbnRhaW5zID0gQXJyYXkucHJvdG90eXBlLmNvbmNhdC5hcHBseShbXSwgbW9kZS5jb250YWlucy5tYXAoZnVuY3Rpb24oYykge1xuICAgICAgICByZXR1cm4gZXhwYW5kX21vZGUoYyA9PT0gJ3NlbGYnID8gbW9kZSA6IGMpO1xuICAgICAgfSkpO1xuICAgICAgbW9kZS5jb250YWlucy5mb3JFYWNoKGZ1bmN0aW9uKGMpIHtjb21waWxlTW9kZShjLCBtb2RlKTt9KTtcblxuICAgICAgaWYgKG1vZGUuc3RhcnRzKSB7XG4gICAgICAgIGNvbXBpbGVNb2RlKG1vZGUuc3RhcnRzLCBwYXJlbnQpO1xuICAgICAgfVxuXG4gICAgICB2YXIgdGVybWluYXRvcnMgPVxuICAgICAgICBtb2RlLmNvbnRhaW5zLm1hcChmdW5jdGlvbihjKSB7XG4gICAgICAgICAgcmV0dXJuIGMuYmVnaW5LZXl3b3JkcyA/ICdcXFxcLj8oJyArIGMuYmVnaW4gKyAnKVxcXFwuPycgOiBjLmJlZ2luO1xuICAgICAgICB9KVxuICAgICAgICAuY29uY2F0KFttb2RlLnRlcm1pbmF0b3JfZW5kLCBtb2RlLmlsbGVnYWxdKVxuICAgICAgICAubWFwKHJlU3RyKVxuICAgICAgICAuZmlsdGVyKEJvb2xlYW4pO1xuICAgICAgbW9kZS50ZXJtaW5hdG9ycyA9IHRlcm1pbmF0b3JzLmxlbmd0aCA/IGxhbmdSZSh0ZXJtaW5hdG9ycy5qb2luKCd8JyksIHRydWUpIDoge2V4ZWM6IGZ1bmN0aW9uKC8qcyovKSB7cmV0dXJuIG51bGw7fX07XG4gICAgfVxuXG4gICAgY29tcGlsZU1vZGUobGFuZ3VhZ2UpO1xuICB9XG5cbiAgLypcbiAgQ29yZSBoaWdobGlnaHRpbmcgZnVuY3Rpb24uIEFjY2VwdHMgYSBsYW5ndWFnZSBuYW1lLCBvciBhbiBhbGlhcywgYW5kIGFcbiAgc3RyaW5nIHdpdGggdGhlIGNvZGUgdG8gaGlnaGxpZ2h0LiBSZXR1cm5zIGFuIG9iamVjdCB3aXRoIHRoZSBmb2xsb3dpbmdcbiAgcHJvcGVydGllczpcblxuICAtIHJlbGV2YW5jZSAoaW50KVxuICAtIHZhbHVlIChhbiBIVE1MIHN0cmluZyB3aXRoIGhpZ2hsaWdodGluZyBtYXJrdXApXG5cbiAgKi9cbiAgZnVuY3Rpb24gaGlnaGxpZ2h0KG5hbWUsIHZhbHVlLCBpZ25vcmVfaWxsZWdhbHMsIGNvbnRpbnVhdGlvbikge1xuXG4gICAgZnVuY3Rpb24gZXNjYXBlUmUodmFsdWUpIHtcbiAgICAgIHJldHVybiBuZXcgUmVnRXhwKHZhbHVlLnJlcGxhY2UoL1stXFwvXFxcXF4kKis/LigpfFtcXF17fV0vZywgJ1xcXFwkJicpLCAnbScpO1xuICAgIH1cblxuICAgIGZ1bmN0aW9uIHN1Yk1vZGUobGV4ZW1lLCBtb2RlKSB7XG4gICAgICB2YXIgaSwgbGVuZ3RoO1xuXG4gICAgICBmb3IgKGkgPSAwLCBsZW5ndGggPSBtb2RlLmNvbnRhaW5zLmxlbmd0aDsgaSA8IGxlbmd0aDsgaSsrKSB7XG4gICAgICAgIGlmICh0ZXN0UmUobW9kZS5jb250YWluc1tpXS5iZWdpblJlLCBsZXhlbWUpKSB7XG4gICAgICAgICAgaWYgKG1vZGUuY29udGFpbnNbaV0uZW5kU2FtZUFzQmVnaW4pIHtcbiAgICAgICAgICAgIG1vZGUuY29udGFpbnNbaV0uZW5kUmUgPSBlc2NhcGVSZSggbW9kZS5jb250YWluc1tpXS5iZWdpblJlLmV4ZWMobGV4ZW1lKVswXSApO1xuICAgICAgICAgIH1cbiAgICAgICAgICByZXR1cm4gbW9kZS5jb250YWluc1tpXTtcbiAgICAgICAgfVxuICAgICAgfVxuICAgIH1cblxuICAgIGZ1bmN0aW9uIGVuZE9mTW9kZShtb2RlLCBsZXhlbWUpIHtcbiAgICAgIGlmICh0ZXN0UmUobW9kZS5lbmRSZSwgbGV4ZW1lKSkge1xuICAgICAgICB3aGlsZSAobW9kZS5lbmRzUGFyZW50ICYmIG1vZGUucGFyZW50KSB7XG4gICAgICAgICAgbW9kZSA9IG1vZGUucGFyZW50O1xuICAgICAgICB9XG4gICAgICAgIHJldHVybiBtb2RlO1xuICAgICAgfVxuICAgICAgaWYgKG1vZGUuZW5kc1dpdGhQYXJlbnQpIHtcbiAgICAgICAgcmV0dXJuIGVuZE9mTW9kZShtb2RlLnBhcmVudCwgbGV4ZW1lKTtcbiAgICAgIH1cbiAgICB9XG5cbiAgICBmdW5jdGlvbiBpc0lsbGVnYWwobGV4ZW1lLCBtb2RlKSB7XG4gICAgICByZXR1cm4gIWlnbm9yZV9pbGxlZ2FscyAmJiB0ZXN0UmUobW9kZS5pbGxlZ2FsUmUsIGxleGVtZSk7XG4gICAgfVxuXG4gICAgZnVuY3Rpb24ga2V5d29yZE1hdGNoKG1vZGUsIG1hdGNoKSB7XG4gICAgICB2YXIgbWF0Y2hfc3RyID0gbGFuZ3VhZ2UuY2FzZV9pbnNlbnNpdGl2ZSA/IG1hdGNoWzBdLnRvTG93ZXJDYXNlKCkgOiBtYXRjaFswXTtcbiAgICAgIHJldHVybiBtb2RlLmtleXdvcmRzLmhhc093blByb3BlcnR5KG1hdGNoX3N0cikgJiYgbW9kZS5rZXl3b3Jkc1ttYXRjaF9zdHJdO1xuICAgIH1cblxuICAgIGZ1bmN0aW9uIGJ1aWxkU3BhbihjbGFzc25hbWUsIGluc2lkZVNwYW4sIGxlYXZlT3Blbiwgbm9QcmVmaXgpIHtcbiAgICAgIHZhciBjbGFzc1ByZWZpeCA9IG5vUHJlZml4ID8gJycgOiBvcHRpb25zLmNsYXNzUHJlZml4LFxuICAgICAgICAgIG9wZW5TcGFuICAgID0gJzxzcGFuIGNsYXNzPVwiJyArIGNsYXNzUHJlZml4LFxuICAgICAgICAgIGNsb3NlU3BhbiAgID0gbGVhdmVPcGVuID8gJycgOiBzcGFuRW5kVGFnO1xuXG4gICAgICBvcGVuU3BhbiArPSBjbGFzc25hbWUgKyAnXCI+JztcblxuICAgICAgcmV0dXJuIG9wZW5TcGFuICsgaW5zaWRlU3BhbiArIGNsb3NlU3BhbjtcbiAgICB9XG5cbiAgICBmdW5jdGlvbiBwcm9jZXNzS2V5d29yZHMoKSB7XG4gICAgICB2YXIga2V5d29yZF9tYXRjaCwgbGFzdF9pbmRleCwgbWF0Y2gsIHJlc3VsdDtcblxuICAgICAgaWYgKCF0b3Aua2V5d29yZHMpXG4gICAgICAgIHJldHVybiBlc2NhcGUobW9kZV9idWZmZXIpO1xuXG4gICAgICByZXN1bHQgPSAnJztcbiAgICAgIGxhc3RfaW5kZXggPSAwO1xuICAgICAgdG9wLmxleGVtZXNSZS5sYXN0SW5kZXggPSAwO1xuICAgICAgbWF0Y2ggPSB0b3AubGV4ZW1lc1JlLmV4ZWMobW9kZV9idWZmZXIpO1xuXG4gICAgICB3aGlsZSAobWF0Y2gpIHtcbiAgICAgICAgcmVzdWx0ICs9IGVzY2FwZShtb2RlX2J1ZmZlci5zdWJzdHJpbmcobGFzdF9pbmRleCwgbWF0Y2guaW5kZXgpKTtcbiAgICAgICAga2V5d29yZF9tYXRjaCA9IGtleXdvcmRNYXRjaCh0b3AsIG1hdGNoKTtcbiAgICAgICAgaWYgKGtleXdvcmRfbWF0Y2gpIHtcbiAgICAgICAgICByZWxldmFuY2UgKz0ga2V5d29yZF9tYXRjaFsxXTtcbiAgICAgICAgICByZXN1bHQgKz0gYnVpbGRTcGFuKGtleXdvcmRfbWF0Y2hbMF0sIGVzY2FwZShtYXRjaFswXSkpO1xuICAgICAgICB9IGVsc2Uge1xuICAgICAgICAgIHJlc3VsdCArPSBlc2NhcGUobWF0Y2hbMF0pO1xuICAgICAgICB9XG4gICAgICAgIGxhc3RfaW5kZXggPSB0b3AubGV4ZW1lc1JlLmxhc3RJbmRleDtcbiAgICAgICAgbWF0Y2ggPSB0b3AubGV4ZW1lc1JlLmV4ZWMobW9kZV9idWZmZXIpO1xuICAgICAgfVxuICAgICAgcmV0dXJuIHJlc3VsdCArIGVzY2FwZShtb2RlX2J1ZmZlci5zdWJzdHIobGFzdF9pbmRleCkpO1xuICAgIH1cblxuICAgIGZ1bmN0aW9uIHByb2Nlc3NTdWJMYW5ndWFnZSgpIHtcbiAgICAgIHZhciBleHBsaWNpdCA9IHR5cGVvZiB0b3Auc3ViTGFuZ3VhZ2UgPT09ICdzdHJpbmcnO1xuICAgICAgaWYgKGV4cGxpY2l0ICYmICFsYW5ndWFnZXNbdG9wLnN1Ykxhbmd1YWdlXSkge1xuICAgICAgICByZXR1cm4gZXNjYXBlKG1vZGVfYnVmZmVyKTtcbiAgICAgIH1cblxuICAgICAgdmFyIHJlc3VsdCA9IGV4cGxpY2l0ID9cbiAgICAgICAgICAgICAgICAgICBoaWdobGlnaHQodG9wLnN1Ykxhbmd1YWdlLCBtb2RlX2J1ZmZlciwgdHJ1ZSwgY29udGludWF0aW9uc1t0b3Auc3ViTGFuZ3VhZ2VdKSA6XG4gICAgICAgICAgICAgICAgICAgaGlnaGxpZ2h0QXV0byhtb2RlX2J1ZmZlciwgdG9wLnN1Ykxhbmd1YWdlLmxlbmd0aCA/IHRvcC5zdWJMYW5ndWFnZSA6IHVuZGVmaW5lZCk7XG5cbiAgICAgIC8vIENvdW50aW5nIGVtYmVkZGVkIGxhbmd1YWdlIHNjb3JlIHRvd2FyZHMgdGhlIGhvc3QgbGFuZ3VhZ2UgbWF5IGJlIGRpc2FibGVkXG4gICAgICAvLyB3aXRoIHplcm9pbmcgdGhlIGNvbnRhaW5pbmcgbW9kZSByZWxldmFuY2UuIFVzZWNhc2UgaW4gcG9pbnQgaXMgTWFya2Rvd24gdGhhdFxuICAgICAgLy8gYWxsb3dzIFhNTCBldmVyeXdoZXJlIGFuZCBtYWtlcyBldmVyeSBYTUwgc25pcHBldCB0byBoYXZlIGEgbXVjaCBsYXJnZXIgTWFya2Rvd25cbiAgICAgIC8vIHNjb3JlLlxuICAgICAgaWYgKHRvcC5yZWxldmFuY2UgPiAwKSB7XG4gICAgICAgIHJlbGV2YW5jZSArPSByZXN1bHQucmVsZXZhbmNlO1xuICAgICAgfVxuICAgICAgaWYgKGV4cGxpY2l0KSB7XG4gICAgICAgIGNvbnRpbnVhdGlvbnNbdG9wLnN1Ykxhbmd1YWdlXSA9IHJlc3VsdC50b3A7XG4gICAgICB9XG4gICAgICByZXR1cm4gYnVpbGRTcGFuKHJlc3VsdC5sYW5ndWFnZSwgcmVzdWx0LnZhbHVlLCBmYWxzZSwgdHJ1ZSk7XG4gICAgfVxuXG4gICAgZnVuY3Rpb24gcHJvY2Vzc0J1ZmZlcigpIHtcbiAgICAgIHJlc3VsdCArPSAodG9wLnN1Ykxhbmd1YWdlICE9IG51bGwgPyBwcm9jZXNzU3ViTGFuZ3VhZ2UoKSA6IHByb2Nlc3NLZXl3b3JkcygpKTtcbiAgICAgIG1vZGVfYnVmZmVyID0gJyc7XG4gICAgfVxuXG4gICAgZnVuY3Rpb24gc3RhcnROZXdNb2RlKG1vZGUpIHtcbiAgICAgIHJlc3VsdCArPSBtb2RlLmNsYXNzTmFtZT8gYnVpbGRTcGFuKG1vZGUuY2xhc3NOYW1lLCAnJywgdHJ1ZSk6ICcnO1xuICAgICAgdG9wID0gT2JqZWN0LmNyZWF0ZShtb2RlLCB7cGFyZW50OiB7dmFsdWU6IHRvcH19KTtcbiAgICB9XG5cbiAgICBmdW5jdGlvbiBwcm9jZXNzTGV4ZW1lKGJ1ZmZlciwgbGV4ZW1lKSB7XG5cbiAgICAgIG1vZGVfYnVmZmVyICs9IGJ1ZmZlcjtcblxuICAgICAgaWYgKGxleGVtZSA9PSBudWxsKSB7XG4gICAgICAgIHByb2Nlc3NCdWZmZXIoKTtcbiAgICAgICAgcmV0dXJuIDA7XG4gICAgICB9XG5cbiAgICAgIHZhciBuZXdfbW9kZSA9IHN1Yk1vZGUobGV4ZW1lLCB0b3ApO1xuICAgICAgaWYgKG5ld19tb2RlKSB7XG4gICAgICAgIGlmIChuZXdfbW9kZS5za2lwKSB7XG4gICAgICAgICAgbW9kZV9idWZmZXIgKz0gbGV4ZW1lO1xuICAgICAgICB9IGVsc2Uge1xuICAgICAgICAgIGlmIChuZXdfbW9kZS5leGNsdWRlQmVnaW4pIHtcbiAgICAgICAgICAgIG1vZGVfYnVmZmVyICs9IGxleGVtZTtcbiAgICAgICAgICB9XG4gICAgICAgICAgcHJvY2Vzc0J1ZmZlcigpO1xuICAgICAgICAgIGlmICghbmV3X21vZGUucmV0dXJuQmVnaW4gJiYgIW5ld19tb2RlLmV4Y2x1ZGVCZWdpbikge1xuICAgICAgICAgICAgbW9kZV9idWZmZXIgPSBsZXhlbWU7XG4gICAgICAgICAgfVxuICAgICAgICB9XG4gICAgICAgIHN0YXJ0TmV3TW9kZShuZXdfbW9kZSwgbGV4ZW1lKTtcbiAgICAgICAgcmV0dXJuIG5ld19tb2RlLnJldHVybkJlZ2luID8gMCA6IGxleGVtZS5sZW5ndGg7XG4gICAgICB9XG5cbiAgICAgIHZhciBlbmRfbW9kZSA9IGVuZE9mTW9kZSh0b3AsIGxleGVtZSk7XG4gICAgICBpZiAoZW5kX21vZGUpIHtcbiAgICAgICAgdmFyIG9yaWdpbiA9IHRvcDtcbiAgICAgICAgaWYgKG9yaWdpbi5za2lwKSB7XG4gICAgICAgICAgbW9kZV9idWZmZXIgKz0gbGV4ZW1lO1xuICAgICAgICB9IGVsc2Uge1xuICAgICAgICAgIGlmICghKG9yaWdpbi5yZXR1cm5FbmQgfHwgb3JpZ2luLmV4Y2x1ZGVFbmQpKSB7XG4gICAgICAgICAgICBtb2RlX2J1ZmZlciArPSBsZXhlbWU7XG4gICAgICAgICAgfVxuICAgICAgICAgIHByb2Nlc3NCdWZmZXIoKTtcbiAgICAgICAgICBpZiAob3JpZ2luLmV4Y2x1ZGVFbmQpIHtcbiAgICAgICAgICAgIG1vZGVfYnVmZmVyID0gbGV4ZW1lO1xuICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgICAgICBkbyB7XG4gICAgICAgICAgaWYgKHRvcC5jbGFzc05hbWUpIHtcbiAgICAgICAgICAgIHJlc3VsdCArPSBzcGFuRW5kVGFnO1xuICAgICAgICAgIH1cbiAgICAgICAgICBpZiAoIXRvcC5za2lwICYmICF0b3Auc3ViTGFuZ3VhZ2UpIHtcbiAgICAgICAgICAgIHJlbGV2YW5jZSArPSB0b3AucmVsZXZhbmNlO1xuICAgICAgICAgIH1cbiAgICAgICAgICB0b3AgPSB0b3AucGFyZW50O1xuICAgICAgICB9IHdoaWxlICh0b3AgIT09IGVuZF9tb2RlLnBhcmVudCk7XG4gICAgICAgIGlmIChlbmRfbW9kZS5zdGFydHMpIHtcbiAgICAgICAgICBpZiAoZW5kX21vZGUuZW5kU2FtZUFzQmVnaW4pIHtcbiAgICAgICAgICAgIGVuZF9tb2RlLnN0YXJ0cy5lbmRSZSA9IGVuZF9tb2RlLmVuZFJlO1xuICAgICAgICAgIH1cbiAgICAgICAgICBzdGFydE5ld01vZGUoZW5kX21vZGUuc3RhcnRzLCAnJyk7XG4gICAgICAgIH1cbiAgICAgICAgcmV0dXJuIG9yaWdpbi5yZXR1cm5FbmQgPyAwIDogbGV4ZW1lLmxlbmd0aDtcbiAgICAgIH1cblxuICAgICAgaWYgKGlzSWxsZWdhbChsZXhlbWUsIHRvcCkpXG4gICAgICAgIHRocm93IG5ldyBFcnJvcignSWxsZWdhbCBsZXhlbWUgXCInICsgbGV4ZW1lICsgJ1wiIGZvciBtb2RlIFwiJyArICh0b3AuY2xhc3NOYW1lIHx8ICc8dW5uYW1lZD4nKSArICdcIicpO1xuXG4gICAgICAvKlxuICAgICAgUGFyc2VyIHNob3VsZCBub3QgcmVhY2ggdGhpcyBwb2ludCBhcyBhbGwgdHlwZXMgb2YgbGV4ZW1lcyBzaG91bGQgYmUgY2F1Z2h0XG4gICAgICBlYXJsaWVyLCBidXQgaWYgaXQgZG9lcyBkdWUgdG8gc29tZSBidWcgbWFrZSBzdXJlIGl0IGFkdmFuY2VzIGF0IGxlYXN0IG9uZVxuICAgICAgY2hhcmFjdGVyIGZvcndhcmQgdG8gcHJldmVudCBpbmZpbml0ZSBsb29waW5nLlxuICAgICAgKi9cbiAgICAgIG1vZGVfYnVmZmVyICs9IGxleGVtZTtcbiAgICAgIHJldHVybiBsZXhlbWUubGVuZ3RoIHx8IDE7XG4gICAgfVxuXG4gICAgdmFyIGxhbmd1YWdlID0gZ2V0TGFuZ3VhZ2UobmFtZSk7XG4gICAgaWYgKCFsYW5ndWFnZSkge1xuICAgICAgdGhyb3cgbmV3IEVycm9yKCdVbmtub3duIGxhbmd1YWdlOiBcIicgKyBuYW1lICsgJ1wiJyk7XG4gICAgfVxuXG4gICAgY29tcGlsZUxhbmd1YWdlKGxhbmd1YWdlKTtcbiAgICB2YXIgdG9wID0gY29udGludWF0aW9uIHx8IGxhbmd1YWdlO1xuICAgIHZhciBjb250aW51YXRpb25zID0ge307IC8vIGtlZXAgY29udGludWF0aW9ucyBmb3Igc3ViLWxhbmd1YWdlc1xuICAgIHZhciByZXN1bHQgPSAnJywgY3VycmVudDtcbiAgICBmb3IoY3VycmVudCA9IHRvcDsgY3VycmVudCAhPT0gbGFuZ3VhZ2U7IGN1cnJlbnQgPSBjdXJyZW50LnBhcmVudCkge1xuICAgICAgaWYgKGN1cnJlbnQuY2xhc3NOYW1lKSB7XG4gICAgICAgIHJlc3VsdCA9IGJ1aWxkU3BhbihjdXJyZW50LmNsYXNzTmFtZSwgJycsIHRydWUpICsgcmVzdWx0O1xuICAgICAgfVxuICAgIH1cbiAgICB2YXIgbW9kZV9idWZmZXIgPSAnJztcbiAgICB2YXIgcmVsZXZhbmNlID0gMDtcbiAgICB0cnkge1xuICAgICAgdmFyIG1hdGNoLCBjb3VudCwgaW5kZXggPSAwO1xuICAgICAgd2hpbGUgKHRydWUpIHtcbiAgICAgICAgdG9wLnRlcm1pbmF0b3JzLmxhc3RJbmRleCA9IGluZGV4O1xuICAgICAgICBtYXRjaCA9IHRvcC50ZXJtaW5hdG9ycy5leGVjKHZhbHVlKTtcbiAgICAgICAgaWYgKCFtYXRjaClcbiAgICAgICAgICBicmVhaztcbiAgICAgICAgY291bnQgPSBwcm9jZXNzTGV4ZW1lKHZhbHVlLnN1YnN0cmluZyhpbmRleCwgbWF0Y2guaW5kZXgpLCBtYXRjaFswXSk7XG4gICAgICAgIGluZGV4ID0gbWF0Y2guaW5kZXggKyBjb3VudDtcbiAgICAgIH1cbiAgICAgIHByb2Nlc3NMZXhlbWUodmFsdWUuc3Vic3RyKGluZGV4KSk7XG4gICAgICBmb3IoY3VycmVudCA9IHRvcDsgY3VycmVudC5wYXJlbnQ7IGN1cnJlbnQgPSBjdXJyZW50LnBhcmVudCkgeyAvLyBjbG9zZSBkYW5nbGluZyBtb2Rlc1xuICAgICAgICBpZiAoY3VycmVudC5jbGFzc05hbWUpIHtcbiAgICAgICAgICByZXN1bHQgKz0gc3BhbkVuZFRhZztcbiAgICAgICAgfVxuICAgICAgfVxuICAgICAgcmV0dXJuIHtcbiAgICAgICAgcmVsZXZhbmNlOiByZWxldmFuY2UsXG4gICAgICAgIHZhbHVlOiByZXN1bHQsXG4gICAgICAgIGxhbmd1YWdlOiBuYW1lLFxuICAgICAgICB0b3A6IHRvcFxuICAgICAgfTtcbiAgICB9IGNhdGNoIChlKSB7XG4gICAgICBpZiAoZS5tZXNzYWdlICYmIGUubWVzc2FnZS5pbmRleE9mKCdJbGxlZ2FsJykgIT09IC0xKSB7XG4gICAgICAgIHJldHVybiB7XG4gICAgICAgICAgcmVsZXZhbmNlOiAwLFxuICAgICAgICAgIHZhbHVlOiBlc2NhcGUodmFsdWUpXG4gICAgICAgIH07XG4gICAgICB9IGVsc2Uge1xuICAgICAgICB0aHJvdyBlO1xuICAgICAgfVxuICAgIH1cbiAgfVxuXG4gIC8qXG4gIEhpZ2hsaWdodGluZyB3aXRoIGxhbmd1YWdlIGRldGVjdGlvbi4gQWNjZXB0cyBhIHN0cmluZyB3aXRoIHRoZSBjb2RlIHRvXG4gIGhpZ2hsaWdodC4gUmV0dXJucyBhbiBvYmplY3Qgd2l0aCB0aGUgZm9sbG93aW5nIHByb3BlcnRpZXM6XG5cbiAgLSBsYW5ndWFnZSAoZGV0ZWN0ZWQgbGFuZ3VhZ2UpXG4gIC0gcmVsZXZhbmNlIChpbnQpXG4gIC0gdmFsdWUgKGFuIEhUTUwgc3RyaW5nIHdpdGggaGlnaGxpZ2h0aW5nIG1hcmt1cClcbiAgLSBzZWNvbmRfYmVzdCAob2JqZWN0IHdpdGggdGhlIHNhbWUgc3RydWN0dXJlIGZvciBzZWNvbmQtYmVzdCBoZXVyaXN0aWNhbGx5XG4gICAgZGV0ZWN0ZWQgbGFuZ3VhZ2UsIG1heSBiZSBhYnNlbnQpXG5cbiAgKi9cbiAgZnVuY3Rpb24gaGlnaGxpZ2h0QXV0byh0ZXh0LCBsYW5ndWFnZVN1YnNldCkge1xuICAgIGxhbmd1YWdlU3Vic2V0ID0gbGFuZ3VhZ2VTdWJzZXQgfHwgb3B0aW9ucy5sYW5ndWFnZXMgfHwgb2JqZWN0S2V5cyhsYW5ndWFnZXMpO1xuICAgIHZhciByZXN1bHQgPSB7XG4gICAgICByZWxldmFuY2U6IDAsXG4gICAgICB2YWx1ZTogZXNjYXBlKHRleHQpXG4gICAgfTtcbiAgICB2YXIgc2Vjb25kX2Jlc3QgPSByZXN1bHQ7XG4gICAgbGFuZ3VhZ2VTdWJzZXQuZmlsdGVyKGdldExhbmd1YWdlKS5maWx0ZXIoYXV0b0RldGVjdGlvbikuZm9yRWFjaChmdW5jdGlvbihuYW1lKSB7XG4gICAgICB2YXIgY3VycmVudCA9IGhpZ2hsaWdodChuYW1lLCB0ZXh0LCBmYWxzZSk7XG4gICAgICBjdXJyZW50Lmxhbmd1YWdlID0gbmFtZTtcbiAgICAgIGlmIChjdXJyZW50LnJlbGV2YW5jZSA+IHNlY29uZF9iZXN0LnJlbGV2YW5jZSkge1xuICAgICAgICBzZWNvbmRfYmVzdCA9IGN1cnJlbnQ7XG4gICAgICB9XG4gICAgICBpZiAoY3VycmVudC5yZWxldmFuY2UgPiByZXN1bHQucmVsZXZhbmNlKSB7XG4gICAgICAgIHNlY29uZF9iZXN0ID0gcmVzdWx0O1xuICAgICAgICByZXN1bHQgPSBjdXJyZW50O1xuICAgICAgfVxuICAgIH0pO1xuICAgIGlmIChzZWNvbmRfYmVzdC5sYW5ndWFnZSkge1xuICAgICAgcmVzdWx0LnNlY29uZF9iZXN0ID0gc2Vjb25kX2Jlc3Q7XG4gICAgfVxuICAgIHJldHVybiByZXN1bHQ7XG4gIH1cblxuICAvKlxuICBQb3N0LXByb2Nlc3Npbmcgb2YgdGhlIGhpZ2hsaWdodGVkIG1hcmt1cDpcblxuICAtIHJlcGxhY2UgVEFCcyB3aXRoIHNvbWV0aGluZyBtb3JlIHVzZWZ1bFxuICAtIHJlcGxhY2UgcmVhbCBsaW5lLWJyZWFrcyB3aXRoICc8YnI+JyBmb3Igbm9uLXByZSBjb250YWluZXJzXG5cbiAgKi9cbiAgZnVuY3Rpb24gZml4TWFya3VwKHZhbHVlKSB7XG4gICAgcmV0dXJuICEob3B0aW9ucy50YWJSZXBsYWNlIHx8IG9wdGlvbnMudXNlQlIpXG4gICAgICA/IHZhbHVlXG4gICAgICA6IHZhbHVlLnJlcGxhY2UoZml4TWFya3VwUmUsIGZ1bmN0aW9uKG1hdGNoLCBwMSkge1xuICAgICAgICAgIGlmIChvcHRpb25zLnVzZUJSICYmIG1hdGNoID09PSAnXFxuJykge1xuICAgICAgICAgICAgcmV0dXJuICc8YnI+JztcbiAgICAgICAgICB9IGVsc2UgaWYgKG9wdGlvbnMudGFiUmVwbGFjZSkge1xuICAgICAgICAgICAgcmV0dXJuIHAxLnJlcGxhY2UoL1xcdC9nLCBvcHRpb25zLnRhYlJlcGxhY2UpO1xuICAgICAgICAgIH1cbiAgICAgICAgICByZXR1cm4gJyc7XG4gICAgICB9KTtcbiAgfVxuXG4gIGZ1bmN0aW9uIGJ1aWxkQ2xhc3NOYW1lKHByZXZDbGFzc05hbWUsIGN1cnJlbnRMYW5nLCByZXN1bHRMYW5nKSB7XG4gICAgdmFyIGxhbmd1YWdlID0gY3VycmVudExhbmcgPyBhbGlhc2VzW2N1cnJlbnRMYW5nXSA6IHJlc3VsdExhbmcsXG4gICAgICAgIHJlc3VsdCAgID0gW3ByZXZDbGFzc05hbWUudHJpbSgpXTtcblxuICAgIGlmICghcHJldkNsYXNzTmFtZS5tYXRjaCgvXFxiaGxqc1xcYi8pKSB7XG4gICAgICByZXN1bHQucHVzaCgnaGxqcycpO1xuICAgIH1cblxuICAgIGlmIChwcmV2Q2xhc3NOYW1lLmluZGV4T2YobGFuZ3VhZ2UpID09PSAtMSkge1xuICAgICAgcmVzdWx0LnB1c2gobGFuZ3VhZ2UpO1xuICAgIH1cblxuICAgIHJldHVybiByZXN1bHQuam9pbignICcpLnRyaW0oKTtcbiAgfVxuXG4gIC8qXG4gIEFwcGxpZXMgaGlnaGxpZ2h0aW5nIHRvIGEgRE9NIG5vZGUgY29udGFpbmluZyBjb2RlLiBBY2NlcHRzIGEgRE9NIG5vZGUgYW5kXG4gIHR3byBvcHRpb25hbCBwYXJhbWV0ZXJzIGZvciBmaXhNYXJrdXAuXG4gICovXG4gIGZ1bmN0aW9uIGhpZ2hsaWdodEJsb2NrKGJsb2NrKSB7XG4gICAgdmFyIG5vZGUsIG9yaWdpbmFsU3RyZWFtLCByZXN1bHQsIHJlc3VsdE5vZGUsIHRleHQ7XG4gICAgdmFyIGxhbmd1YWdlID0gYmxvY2tMYW5ndWFnZShibG9jayk7XG5cbiAgICBpZiAoaXNOb3RIaWdobGlnaHRlZChsYW5ndWFnZSkpXG4gICAgICAgIHJldHVybjtcblxuICAgIGlmIChvcHRpb25zLnVzZUJSKSB7XG4gICAgICBub2RlID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudE5TKCdodHRwOi8vd3d3LnczLm9yZy8xOTk5L3hodG1sJywgJ2RpdicpO1xuICAgICAgbm9kZS5pbm5lckhUTUwgPSBibG9jay5pbm5lckhUTUwucmVwbGFjZSgvXFxuL2csICcnKS5yZXBsYWNlKC88YnJbIFxcL10qPi9nLCAnXFxuJyk7XG4gICAgfSBlbHNlIHtcbiAgICAgIG5vZGUgPSBibG9jaztcbiAgICB9XG4gICAgdGV4dCA9IG5vZGUudGV4dENvbnRlbnQ7XG4gICAgcmVzdWx0ID0gbGFuZ3VhZ2UgPyBoaWdobGlnaHQobGFuZ3VhZ2UsIHRleHQsIHRydWUpIDogaGlnaGxpZ2h0QXV0byh0ZXh0KTtcblxuICAgIG9yaWdpbmFsU3RyZWFtID0gbm9kZVN0cmVhbShub2RlKTtcbiAgICBpZiAob3JpZ2luYWxTdHJlYW0ubGVuZ3RoKSB7XG4gICAgICByZXN1bHROb2RlID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudE5TKCdodHRwOi8vd3d3LnczLm9yZy8xOTk5L3hodG1sJywgJ2RpdicpO1xuICAgICAgcmVzdWx0Tm9kZS5pbm5lckhUTUwgPSByZXN1bHQudmFsdWU7XG4gICAgICByZXN1bHQudmFsdWUgPSBtZXJnZVN0cmVhbXMob3JpZ2luYWxTdHJlYW0sIG5vZGVTdHJlYW0ocmVzdWx0Tm9kZSksIHRleHQpO1xuICAgIH1cbiAgICByZXN1bHQudmFsdWUgPSBmaXhNYXJrdXAocmVzdWx0LnZhbHVlKTtcblxuICAgIGJsb2NrLmlubmVySFRNTCA9IHJlc3VsdC52YWx1ZTtcbiAgICBibG9jay5jbGFzc05hbWUgPSBidWlsZENsYXNzTmFtZShibG9jay5jbGFzc05hbWUsIGxhbmd1YWdlLCByZXN1bHQubGFuZ3VhZ2UpO1xuICAgIGJsb2NrLnJlc3VsdCA9IHtcbiAgICAgIGxhbmd1YWdlOiByZXN1bHQubGFuZ3VhZ2UsXG4gICAgICByZTogcmVzdWx0LnJlbGV2YW5jZVxuICAgIH07XG4gICAgaWYgKHJlc3VsdC5zZWNvbmRfYmVzdCkge1xuICAgICAgYmxvY2suc2Vjb25kX2Jlc3QgPSB7XG4gICAgICAgIGxhbmd1YWdlOiByZXN1bHQuc2Vjb25kX2Jlc3QubGFuZ3VhZ2UsXG4gICAgICAgIHJlOiByZXN1bHQuc2Vjb25kX2Jlc3QucmVsZXZhbmNlXG4gICAgICB9O1xuICAgIH1cbiAgfVxuXG4gIC8qXG4gIFVwZGF0ZXMgaGlnaGxpZ2h0LmpzIGdsb2JhbCBvcHRpb25zIHdpdGggdmFsdWVzIHBhc3NlZCBpbiB0aGUgZm9ybSBvZiBhbiBvYmplY3QuXG4gICovXG4gIGZ1bmN0aW9uIGNvbmZpZ3VyZSh1c2VyX29wdGlvbnMpIHtcbiAgICBvcHRpb25zID0gaW5oZXJpdChvcHRpb25zLCB1c2VyX29wdGlvbnMpO1xuICB9XG5cbiAgLypcbiAgQXBwbGllcyBoaWdobGlnaHRpbmcgdG8gYWxsIDxwcmU+PGNvZGU+Li48L2NvZGU+PC9wcmU+IGJsb2NrcyBvbiBhIHBhZ2UuXG4gICovXG4gIGZ1bmN0aW9uIGluaXRIaWdobGlnaHRpbmcoKSB7XG4gICAgaWYgKGluaXRIaWdobGlnaHRpbmcuY2FsbGVkKVxuICAgICAgcmV0dXJuO1xuICAgIGluaXRIaWdobGlnaHRpbmcuY2FsbGVkID0gdHJ1ZTtcblxuICAgIHZhciBibG9ja3MgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yQWxsKCdwcmUgY29kZScpO1xuICAgIEFycmF5UHJvdG8uZm9yRWFjaC5jYWxsKGJsb2NrcywgaGlnaGxpZ2h0QmxvY2spO1xuICB9XG5cbiAgLypcbiAgQXR0YWNoZXMgaGlnaGxpZ2h0aW5nIHRvIHRoZSBwYWdlIGxvYWQgZXZlbnQuXG4gICovXG4gIGZ1bmN0aW9uIGluaXRIaWdobGlnaHRpbmdPbkxvYWQoKSB7XG4gICAgYWRkRXZlbnRMaXN0ZW5lcignRE9NQ29udGVudExvYWRlZCcsIGluaXRIaWdobGlnaHRpbmcsIGZhbHNlKTtcbiAgICBhZGRFdmVudExpc3RlbmVyKCdsb2FkJywgaW5pdEhpZ2hsaWdodGluZywgZmFsc2UpO1xuICB9XG5cbiAgZnVuY3Rpb24gcmVnaXN0ZXJMYW5ndWFnZShuYW1lLCBsYW5ndWFnZSkge1xuICAgIHZhciBsYW5nID0gbGFuZ3VhZ2VzW25hbWVdID0gbGFuZ3VhZ2UoaGxqcyk7XG4gICAgaWYgKGxhbmcuYWxpYXNlcykge1xuICAgICAgbGFuZy5hbGlhc2VzLmZvckVhY2goZnVuY3Rpb24oYWxpYXMpIHthbGlhc2VzW2FsaWFzXSA9IG5hbWU7fSk7XG4gICAgfVxuICB9XG5cbiAgZnVuY3Rpb24gbGlzdExhbmd1YWdlcygpIHtcbiAgICByZXR1cm4gb2JqZWN0S2V5cyhsYW5ndWFnZXMpO1xuICB9XG5cbiAgZnVuY3Rpb24gZ2V0TGFuZ3VhZ2UobmFtZSkge1xuICAgIG5hbWUgPSAobmFtZSB8fCAnJykudG9Mb3dlckNhc2UoKTtcbiAgICByZXR1cm4gbGFuZ3VhZ2VzW25hbWVdIHx8IGxhbmd1YWdlc1thbGlhc2VzW25hbWVdXTtcbiAgfVxuXG4gIGZ1bmN0aW9uIGF1dG9EZXRlY3Rpb24obmFtZSkge1xuICAgIHZhciBsYW5nID0gZ2V0TGFuZ3VhZ2UobmFtZSk7XG4gICAgcmV0dXJuIGxhbmcgJiYgIWxhbmcuZGlzYWJsZUF1dG9kZXRlY3Q7XG4gIH1cblxuICAvKiBJbnRlcmZhY2UgZGVmaW5pdGlvbiAqL1xuXG4gIGhsanMuaGlnaGxpZ2h0ID0gaGlnaGxpZ2h0O1xuICBobGpzLmhpZ2hsaWdodEF1dG8gPSBoaWdobGlnaHRBdXRvO1xuICBobGpzLmZpeE1hcmt1cCA9IGZpeE1hcmt1cDtcbiAgaGxqcy5oaWdobGlnaHRCbG9jayA9IGhpZ2hsaWdodEJsb2NrO1xuICBobGpzLmNvbmZpZ3VyZSA9IGNvbmZpZ3VyZTtcbiAgaGxqcy5pbml0SGlnaGxpZ2h0aW5nID0gaW5pdEhpZ2hsaWdodGluZztcbiAgaGxqcy5pbml0SGlnaGxpZ2h0aW5nT25Mb2FkID0gaW5pdEhpZ2hsaWdodGluZ09uTG9hZDtcbiAgaGxqcy5yZWdpc3Rlckxhbmd1YWdlID0gcmVnaXN0ZXJMYW5ndWFnZTtcbiAgaGxqcy5saXN0TGFuZ3VhZ2VzID0gbGlzdExhbmd1YWdlcztcbiAgaGxqcy5nZXRMYW5ndWFnZSA9IGdldExhbmd1YWdlO1xuICBobGpzLmF1dG9EZXRlY3Rpb24gPSBhdXRvRGV0ZWN0aW9uO1xuICBobGpzLmluaGVyaXQgPSBpbmhlcml0O1xuXG4gIC8vIENvbW1vbiByZWdleHBzXG4gIGhsanMuSURFTlRfUkUgPSAnW2EtekEtWl1cXFxcdyonO1xuICBobGpzLlVOREVSU0NPUkVfSURFTlRfUkUgPSAnW2EtekEtWl9dXFxcXHcqJztcbiAgaGxqcy5OVU1CRVJfUkUgPSAnXFxcXGJcXFxcZCsoXFxcXC5cXFxcZCspPyc7XG4gIGhsanMuQ19OVU1CRVJfUkUgPSAnKC0/KShcXFxcYjBbeFhdW2EtZkEtRjAtOV0rfChcXFxcYlxcXFxkKyhcXFxcLlxcXFxkKik/fFxcXFwuXFxcXGQrKShbZUVdWy0rXT9cXFxcZCspPyknOyAvLyAweC4uLiwgMC4uLiwgZGVjaW1hbCwgZmxvYXRcbiAgaGxqcy5CSU5BUllfTlVNQkVSX1JFID0gJ1xcXFxiKDBiWzAxXSspJzsgLy8gMGIuLi5cbiAgaGxqcy5SRV9TVEFSVEVSU19SRSA9ICchfCE9fCE9PXwlfCU9fCZ8JiZ8Jj18XFxcXCp8XFxcXCo9fFxcXFwrfFxcXFwrPXwsfC18LT18Lz18L3w6fDt8PDx8PDw9fDw9fDx8PT09fD09fD18Pj4+PXw+Pj18Pj18Pj4+fD4+fD58XFxcXD98XFxcXFt8XFxcXHt8XFxcXCh8XFxcXF58XFxcXF49fFxcXFx8fFxcXFx8PXxcXFxcfFxcXFx8fH4nO1xuXG4gIC8vIENvbW1vbiBtb2Rlc1xuICBobGpzLkJBQ0tTTEFTSF9FU0NBUEUgPSB7XG4gICAgYmVnaW46ICdcXFxcXFxcXFtcXFxcc1xcXFxTXScsIHJlbGV2YW5jZTogMFxuICB9O1xuICBobGpzLkFQT1NfU1RSSU5HX01PREUgPSB7XG4gICAgY2xhc3NOYW1lOiAnc3RyaW5nJyxcbiAgICBiZWdpbjogJ1xcJycsIGVuZDogJ1xcJycsXG4gICAgaWxsZWdhbDogJ1xcXFxuJyxcbiAgICBjb250YWluczogW2hsanMuQkFDS1NMQVNIX0VTQ0FQRV1cbiAgfTtcbiAgaGxqcy5RVU9URV9TVFJJTkdfTU9ERSA9IHtcbiAgICBjbGFzc05hbWU6ICdzdHJpbmcnLFxuICAgIGJlZ2luOiAnXCInLCBlbmQ6ICdcIicsXG4gICAgaWxsZWdhbDogJ1xcXFxuJyxcbiAgICBjb250YWluczogW2hsanMuQkFDS1NMQVNIX0VTQ0FQRV1cbiAgfTtcbiAgaGxqcy5QSFJBU0FMX1dPUkRTX01PREUgPSB7XG4gICAgYmVnaW46IC9cXGIoYXxhbnx0aGV8YXJlfEknbXxpc24ndHxkb24ndHxkb2Vzbid0fHdvbid0fGJ1dHxqdXN0fHNob3VsZHxwcmV0dHl8c2ltcGx5fGVub3VnaHxnb25uYXxnb2luZ3x3dGZ8c298c3VjaHx3aWxsfHlvdXx5b3VyfHRoZXl8bGlrZXxtb3JlKVxcYi9cbiAgfTtcbiAgaGxqcy5DT01NRU5UID0gZnVuY3Rpb24gKGJlZ2luLCBlbmQsIGluaGVyaXRzKSB7XG4gICAgdmFyIG1vZGUgPSBobGpzLmluaGVyaXQoXG4gICAgICB7XG4gICAgICAgIGNsYXNzTmFtZTogJ2NvbW1lbnQnLFxuICAgICAgICBiZWdpbjogYmVnaW4sIGVuZDogZW5kLFxuICAgICAgICBjb250YWluczogW11cbiAgICAgIH0sXG4gICAgICBpbmhlcml0cyB8fCB7fVxuICAgICk7XG4gICAgbW9kZS5jb250YWlucy5wdXNoKGhsanMuUEhSQVNBTF9XT1JEU19NT0RFKTtcbiAgICBtb2RlLmNvbnRhaW5zLnB1c2goe1xuICAgICAgY2xhc3NOYW1lOiAnZG9jdGFnJyxcbiAgICAgIGJlZ2luOiAnKD86VE9ET3xGSVhNRXxOT1RFfEJVR3xYWFgpOicsXG4gICAgICByZWxldmFuY2U6IDBcbiAgICB9KTtcbiAgICByZXR1cm4gbW9kZTtcbiAgfTtcbiAgaGxqcy5DX0xJTkVfQ09NTUVOVF9NT0RFID0gaGxqcy5DT01NRU5UKCcvLycsICckJyk7XG4gIGhsanMuQ19CTE9DS19DT01NRU5UX01PREUgPSBobGpzLkNPTU1FTlQoJy9cXFxcKicsICdcXFxcKi8nKTtcbiAgaGxqcy5IQVNIX0NPTU1FTlRfTU9ERSA9IGhsanMuQ09NTUVOVCgnIycsICckJyk7XG4gIGhsanMuTlVNQkVSX01PREUgPSB7XG4gICAgY2xhc3NOYW1lOiAnbnVtYmVyJyxcbiAgICBiZWdpbjogaGxqcy5OVU1CRVJfUkUsXG4gICAgcmVsZXZhbmNlOiAwXG4gIH07XG4gIGhsanMuQ19OVU1CRVJfTU9ERSA9IHtcbiAgICBjbGFzc05hbWU6ICdudW1iZXInLFxuICAgIGJlZ2luOiBobGpzLkNfTlVNQkVSX1JFLFxuICAgIHJlbGV2YW5jZTogMFxuICB9O1xuICBobGpzLkJJTkFSWV9OVU1CRVJfTU9ERSA9IHtcbiAgICBjbGFzc05hbWU6ICdudW1iZXInLFxuICAgIGJlZ2luOiBobGpzLkJJTkFSWV9OVU1CRVJfUkUsXG4gICAgcmVsZXZhbmNlOiAwXG4gIH07XG4gIGhsanMuQ1NTX05VTUJFUl9NT0RFID0ge1xuICAgIGNsYXNzTmFtZTogJ251bWJlcicsXG4gICAgYmVnaW46IGhsanMuTlVNQkVSX1JFICsgJygnICtcbiAgICAgICclfGVtfGV4fGNofHJlbScgICtcbiAgICAgICd8dnd8dmh8dm1pbnx2bWF4JyArXG4gICAgICAnfGNtfG1tfGlufHB0fHBjfHB4JyArXG4gICAgICAnfGRlZ3xncmFkfHJhZHx0dXJuJyArXG4gICAgICAnfHN8bXMnICtcbiAgICAgICd8SHp8a0h6JyArXG4gICAgICAnfGRwaXxkcGNtfGRwcHgnICtcbiAgICAgICcpPycsXG4gICAgcmVsZXZhbmNlOiAwXG4gIH07XG4gIGhsanMuUkVHRVhQX01PREUgPSB7XG4gICAgY2xhc3NOYW1lOiAncmVnZXhwJyxcbiAgICBiZWdpbjogL1xcLy8sIGVuZDogL1xcL1tnaW11eV0qLyxcbiAgICBpbGxlZ2FsOiAvXFxuLyxcbiAgICBjb250YWluczogW1xuICAgICAgaGxqcy5CQUNLU0xBU0hfRVNDQVBFLFxuICAgICAge1xuICAgICAgICBiZWdpbjogL1xcWy8sIGVuZDogL1xcXS8sXG4gICAgICAgIHJlbGV2YW5jZTogMCxcbiAgICAgICAgY29udGFpbnM6IFtobGpzLkJBQ0tTTEFTSF9FU0NBUEVdXG4gICAgICB9XG4gICAgXVxuICB9O1xuICBobGpzLlRJVExFX01PREUgPSB7XG4gICAgY2xhc3NOYW1lOiAndGl0bGUnLFxuICAgIGJlZ2luOiBobGpzLklERU5UX1JFLFxuICAgIHJlbGV2YW5jZTogMFxuICB9O1xuICBobGpzLlVOREVSU0NPUkVfVElUTEVfTU9ERSA9IHtcbiAgICBjbGFzc05hbWU6ICd0aXRsZScsXG4gICAgYmVnaW46IGhsanMuVU5ERVJTQ09SRV9JREVOVF9SRSxcbiAgICByZWxldmFuY2U6IDBcbiAgfTtcbiAgaGxqcy5NRVRIT0RfR1VBUkQgPSB7XG4gICAgLy8gZXhjbHVkZXMgbWV0aG9kIG5hbWVzIGZyb20ga2V5d29yZCBwcm9jZXNzaW5nXG4gICAgYmVnaW46ICdcXFxcLlxcXFxzKicgKyBobGpzLlVOREVSU0NPUkVfSURFTlRfUkUsXG4gICAgcmVsZXZhbmNlOiAwXG4gIH07XG5cbiAgcmV0dXJuIGhsanM7XG59KSk7XG4iLCJtb2R1bGUuZXhwb3J0cyA9IGZ1bmN0aW9uKGhsanMpIHtcbiAgdmFyIEtFWVdPUkRTID0ge1xuICAgIGtleXdvcmQ6XG4gICAgICAvLyBOb3JtYWwga2V5d29yZHMuXG4gICAgICAnYWJzdHJhY3QgYXMgYmFzZSBib29sIGJyZWFrIGJ5dGUgY2FzZSBjYXRjaCBjaGFyIGNoZWNrZWQgY29uc3QgY29udGludWUgZGVjaW1hbCAnICtcbiAgICAgICdkZWZhdWx0IGRlbGVnYXRlIGRvIGRvdWJsZSBlbnVtIGV2ZW50IGV4cGxpY2l0IGV4dGVybiBmaW5hbGx5IGZpeGVkIGZsb2F0ICcgK1xuICAgICAgJ2ZvciBmb3JlYWNoIGdvdG8gaWYgaW1wbGljaXQgaW4gaW50IGludGVyZmFjZSBpbnRlcm5hbCBpcyBsb2NrIGxvbmcgbmFtZW9mICcgK1xuICAgICAgJ29iamVjdCBvcGVyYXRvciBvdXQgb3ZlcnJpZGUgcGFyYW1zIHByaXZhdGUgcHJvdGVjdGVkIHB1YmxpYyByZWFkb25seSByZWYgc2J5dGUgJyArXG4gICAgICAnc2VhbGVkIHNob3J0IHNpemVvZiBzdGFja2FsbG9jIHN0YXRpYyBzdHJpbmcgc3RydWN0IHN3aXRjaCB0aGlzIHRyeSB0eXBlb2YgJyArXG4gICAgICAndWludCB1bG9uZyB1bmNoZWNrZWQgdW5zYWZlIHVzaG9ydCB1c2luZyB2aXJ0dWFsIHZvaWQgdm9sYXRpbGUgd2hpbGUgJyArXG4gICAgICAvLyBDb250ZXh0dWFsIGtleXdvcmRzLlxuICAgICAgJ2FkZCBhbGlhcyBhc2NlbmRpbmcgYXN5bmMgYXdhaXQgYnkgZGVzY2VuZGluZyBkeW5hbWljIGVxdWFscyBmcm9tIGdldCBnbG9iYWwgZ3JvdXAgaW50byBqb2luICcgK1xuICAgICAgJ2xldCBvbiBvcmRlcmJ5IHBhcnRpYWwgcmVtb3ZlIHNlbGVjdCBzZXQgdmFsdWUgdmFyIHdoZXJlIHlpZWxkJyxcbiAgICBsaXRlcmFsOlxuICAgICAgJ251bGwgZmFsc2UgdHJ1ZSdcbiAgfTtcbiAgdmFyIE5VTUJFUlMgPSB7XG4gICAgY2xhc3NOYW1lOiAnbnVtYmVyJyxcbiAgICB2YXJpYW50czogW1xuICAgICAgeyBiZWdpbjogJ1xcXFxiKDBiWzAxXFwnXSspJyB9LFxuICAgICAgeyBiZWdpbjogJygtPylcXFxcYihbXFxcXGRcXCddKyhcXFxcLltcXFxcZFxcJ10qKT98XFxcXC5bXFxcXGRcXCddKykodXxVfGx8THx1bHxVTHxmfEZ8YnxCKScgfSxcbiAgICAgIHsgYmVnaW46ICcoLT8pKFxcXFxiMFt4WF1bYS1mQS1GMC05XFwnXSt8KFxcXFxiW1xcXFxkXFwnXSsoXFxcXC5bXFxcXGRcXCddKik/fFxcXFwuW1xcXFxkXFwnXSspKFtlRV1bLStdP1tcXFxcZFxcJ10rKT8pJyB9XG4gICAgXSxcbiAgICByZWxldmFuY2U6IDBcbiAgfTtcbiAgdmFyIFZFUkJBVElNX1NUUklORyA9IHtcbiAgICBjbGFzc05hbWU6ICdzdHJpbmcnLFxuICAgIGJlZ2luOiAnQFwiJywgZW5kOiAnXCInLFxuICAgIGNvbnRhaW5zOiBbe2JlZ2luOiAnXCJcIid9XVxuICB9O1xuICB2YXIgVkVSQkFUSU1fU1RSSU5HX05PX0xGID0gaGxqcy5pbmhlcml0KFZFUkJBVElNX1NUUklORywge2lsbGVnYWw6IC9cXG4vfSk7XG4gIHZhciBTVUJTVCA9IHtcbiAgICBjbGFzc05hbWU6ICdzdWJzdCcsXG4gICAgYmVnaW46ICd7JywgZW5kOiAnfScsXG4gICAga2V5d29yZHM6IEtFWVdPUkRTXG4gIH07XG4gIHZhciBTVUJTVF9OT19MRiA9IGhsanMuaW5oZXJpdChTVUJTVCwge2lsbGVnYWw6IC9cXG4vfSk7XG4gIHZhciBJTlRFUlBPTEFURURfU1RSSU5HID0ge1xuICAgIGNsYXNzTmFtZTogJ3N0cmluZycsXG4gICAgYmVnaW46IC9cXCRcIi8sIGVuZDogJ1wiJyxcbiAgICBpbGxlZ2FsOiAvXFxuLyxcbiAgICBjb250YWluczogW3tiZWdpbjogJ3t7J30sIHtiZWdpbjogJ319J30sIGhsanMuQkFDS1NMQVNIX0VTQ0FQRSwgU1VCU1RfTk9fTEZdXG4gIH07XG4gIHZhciBJTlRFUlBPTEFURURfVkVSQkFUSU1fU1RSSU5HID0ge1xuICAgIGNsYXNzTmFtZTogJ3N0cmluZycsXG4gICAgYmVnaW46IC9cXCRAXCIvLCBlbmQ6ICdcIicsXG4gICAgY29udGFpbnM6IFt7YmVnaW46ICd7eyd9LCB7YmVnaW46ICd9fSd9LCB7YmVnaW46ICdcIlwiJ30sIFNVQlNUXVxuICB9O1xuICB2YXIgSU5URVJQT0xBVEVEX1ZFUkJBVElNX1NUUklOR19OT19MRiA9IGhsanMuaW5oZXJpdChJTlRFUlBPTEFURURfVkVSQkFUSU1fU1RSSU5HLCB7XG4gICAgaWxsZWdhbDogL1xcbi8sXG4gICAgY29udGFpbnM6IFt7YmVnaW46ICd7eyd9LCB7YmVnaW46ICd9fSd9LCB7YmVnaW46ICdcIlwiJ30sIFNVQlNUX05PX0xGXVxuICB9KTtcbiAgU1VCU1QuY29udGFpbnMgPSBbXG4gICAgSU5URVJQT0xBVEVEX1ZFUkJBVElNX1NUUklORyxcbiAgICBJTlRFUlBPTEFURURfU1RSSU5HLFxuICAgIFZFUkJBVElNX1NUUklORyxcbiAgICBobGpzLkFQT1NfU1RSSU5HX01PREUsXG4gICAgaGxqcy5RVU9URV9TVFJJTkdfTU9ERSxcbiAgICBOVU1CRVJTLFxuICAgIGhsanMuQ19CTE9DS19DT01NRU5UX01PREVcbiAgXTtcbiAgU1VCU1RfTk9fTEYuY29udGFpbnMgPSBbXG4gICAgSU5URVJQT0xBVEVEX1ZFUkJBVElNX1NUUklOR19OT19MRixcbiAgICBJTlRFUlBPTEFURURfU1RSSU5HLFxuICAgIFZFUkJBVElNX1NUUklOR19OT19MRixcbiAgICBobGpzLkFQT1NfU1RSSU5HX01PREUsXG4gICAgaGxqcy5RVU9URV9TVFJJTkdfTU9ERSxcbiAgICBOVU1CRVJTLFxuICAgIGhsanMuaW5oZXJpdChobGpzLkNfQkxPQ0tfQ09NTUVOVF9NT0RFLCB7aWxsZWdhbDogL1xcbi99KVxuICBdO1xuICB2YXIgU1RSSU5HID0ge1xuICAgIHZhcmlhbnRzOiBbXG4gICAgICBJTlRFUlBPTEFURURfVkVSQkFUSU1fU1RSSU5HLFxuICAgICAgSU5URVJQT0xBVEVEX1NUUklORyxcbiAgICAgIFZFUkJBVElNX1NUUklORyxcbiAgICAgIGhsanMuQVBPU19TVFJJTkdfTU9ERSxcbiAgICAgIGhsanMuUVVPVEVfU1RSSU5HX01PREVcbiAgICBdXG4gIH07XG5cbiAgdmFyIFRZUEVfSURFTlRfUkUgPSBobGpzLklERU5UX1JFICsgJyg8JyArIGhsanMuSURFTlRfUkUgKyAnKFxcXFxzKixcXFxccyonICsgaGxqcy5JREVOVF9SRSArICcpKj4pPyhcXFxcW1xcXFxdKT8nO1xuXG4gIHJldHVybiB7XG4gICAgYWxpYXNlczogWydjc2hhcnAnLCAnYyMnXSxcbiAgICBrZXl3b3JkczogS0VZV09SRFMsXG4gICAgaWxsZWdhbDogLzo6LyxcbiAgICBjb250YWluczogW1xuICAgICAgaGxqcy5DT01NRU5UKFxuICAgICAgICAnLy8vJyxcbiAgICAgICAgJyQnLFxuICAgICAgICB7XG4gICAgICAgICAgcmV0dXJuQmVnaW46IHRydWUsXG4gICAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICAgIHtcbiAgICAgICAgICAgICAgY2xhc3NOYW1lOiAnZG9jdGFnJyxcbiAgICAgICAgICAgICAgdmFyaWFudHM6IFtcbiAgICAgICAgICAgICAgICB7XG4gICAgICAgICAgICAgICAgICBiZWdpbjogJy8vLycsIHJlbGV2YW5jZTogMFxuICAgICAgICAgICAgICAgIH0sXG4gICAgICAgICAgICAgICAge1xuICAgICAgICAgICAgICAgICAgYmVnaW46ICc8IS0tfC0tPidcbiAgICAgICAgICAgICAgICB9LFxuICAgICAgICAgICAgICAgIHtcbiAgICAgICAgICAgICAgICAgIGJlZ2luOiAnPC8/JywgZW5kOiAnPidcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgIF1cbiAgICAgICAgICAgIH1cbiAgICAgICAgICBdXG4gICAgICAgIH1cbiAgICAgICksXG4gICAgICBobGpzLkNfTElORV9DT01NRU5UX01PREUsXG4gICAgICBobGpzLkNfQkxPQ0tfQ09NTUVOVF9NT0RFLFxuICAgICAge1xuICAgICAgICBjbGFzc05hbWU6ICdtZXRhJyxcbiAgICAgICAgYmVnaW46ICcjJywgZW5kOiAnJCcsXG4gICAgICAgIGtleXdvcmRzOiB7XG4gICAgICAgICAgJ21ldGEta2V5d29yZCc6ICdpZiBlbHNlIGVsaWYgZW5kaWYgZGVmaW5lIHVuZGVmIHdhcm5pbmcgZXJyb3IgbGluZSByZWdpb24gZW5kcmVnaW9uIHByYWdtYSBjaGVja3N1bSdcbiAgICAgICAgfVxuICAgICAgfSxcbiAgICAgIFNUUklORyxcbiAgICAgIE5VTUJFUlMsXG4gICAgICB7XG4gICAgICAgIGJlZ2luS2V5d29yZHM6ICdjbGFzcyBpbnRlcmZhY2UnLCBlbmQ6IC9bezs9XS8sXG4gICAgICAgIGlsbGVnYWw6IC9bXlxcczosXS8sXG4gICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAgaGxqcy5USVRMRV9NT0RFLFxuICAgICAgICAgIGhsanMuQ19MSU5FX0NPTU1FTlRfTU9ERSxcbiAgICAgICAgICBobGpzLkNfQkxPQ0tfQ09NTUVOVF9NT0RFXG4gICAgICAgIF1cbiAgICAgIH0sXG4gICAgICB7XG4gICAgICAgIGJlZ2luS2V5d29yZHM6ICduYW1lc3BhY2UnLCBlbmQ6IC9bezs9XS8sXG4gICAgICAgIGlsbGVnYWw6IC9bXlxcczpdLyxcbiAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICBobGpzLmluaGVyaXQoaGxqcy5USVRMRV9NT0RFLCB7YmVnaW46ICdbYS16QS1aXShcXFxcLj9cXFxcdykqJ30pLFxuICAgICAgICAgIGhsanMuQ19MSU5FX0NPTU1FTlRfTU9ERSxcbiAgICAgICAgICBobGpzLkNfQkxPQ0tfQ09NTUVOVF9NT0RFXG4gICAgICAgIF1cbiAgICAgIH0sXG4gICAgICB7XG4gICAgICAgIC8vIFtBdHRyaWJ1dGVzKFwiXCIpXVxuICAgICAgICBjbGFzc05hbWU6ICdtZXRhJyxcbiAgICAgICAgYmVnaW46ICdeXFxcXHMqXFxcXFsnLCBleGNsdWRlQmVnaW46IHRydWUsIGVuZDogJ1xcXFxdJywgZXhjbHVkZUVuZDogdHJ1ZSxcbiAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICB7Y2xhc3NOYW1lOiAnbWV0YS1zdHJpbmcnLCBiZWdpbjogL1wiLywgZW5kOiAvXCIvfVxuICAgICAgICBdXG4gICAgICB9LFxuICAgICAge1xuICAgICAgICAvLyBFeHByZXNzaW9uIGtleXdvcmRzIHByZXZlbnQgJ2tleXdvcmQgTmFtZSguLi4pJyBmcm9tIGJlaW5nXG4gICAgICAgIC8vIHJlY29nbml6ZWQgYXMgYSBmdW5jdGlvbiBkZWZpbml0aW9uXG4gICAgICAgIGJlZ2luS2V5d29yZHM6ICduZXcgcmV0dXJuIHRocm93IGF3YWl0IGVsc2UnLFxuICAgICAgICByZWxldmFuY2U6IDBcbiAgICAgIH0sXG4gICAgICB7XG4gICAgICAgIGNsYXNzTmFtZTogJ2Z1bmN0aW9uJyxcbiAgICAgICAgYmVnaW46ICcoJyArIFRZUEVfSURFTlRfUkUgKyAnXFxcXHMrKSsnICsgaGxqcy5JREVOVF9SRSArICdcXFxccypcXFxcKCcsIHJldHVybkJlZ2luOiB0cnVlLFxuICAgICAgICBlbmQ6IC9cXHMqW3s7PV0vLCBleGNsdWRlRW5kOiB0cnVlLFxuICAgICAgICBrZXl3b3JkczogS0VZV09SRFMsXG4gICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAge1xuICAgICAgICAgICAgYmVnaW46IGhsanMuSURFTlRfUkUgKyAnXFxcXHMqXFxcXCgnLCByZXR1cm5CZWdpbjogdHJ1ZSxcbiAgICAgICAgICAgIGNvbnRhaW5zOiBbaGxqcy5USVRMRV9NT0RFXSxcbiAgICAgICAgICAgIHJlbGV2YW5jZTogMFxuICAgICAgICAgIH0sXG4gICAgICAgICAge1xuICAgICAgICAgICAgY2xhc3NOYW1lOiAncGFyYW1zJyxcbiAgICAgICAgICAgIGJlZ2luOiAvXFwoLywgZW5kOiAvXFwpLyxcbiAgICAgICAgICAgIGV4Y2x1ZGVCZWdpbjogdHJ1ZSxcbiAgICAgICAgICAgIGV4Y2x1ZGVFbmQ6IHRydWUsXG4gICAgICAgICAgICBrZXl3b3JkczogS0VZV09SRFMsXG4gICAgICAgICAgICByZWxldmFuY2U6IDAsXG4gICAgICAgICAgICBjb250YWluczogW1xuICAgICAgICAgICAgICBTVFJJTkcsXG4gICAgICAgICAgICAgIE5VTUJFUlMsXG4gICAgICAgICAgICAgIGhsanMuQ19CTE9DS19DT01NRU5UX01PREVcbiAgICAgICAgICAgIF1cbiAgICAgICAgICB9LFxuICAgICAgICAgIGhsanMuQ19MSU5FX0NPTU1FTlRfTU9ERSxcbiAgICAgICAgICBobGpzLkNfQkxPQ0tfQ09NTUVOVF9NT0RFXG4gICAgICAgIF1cbiAgICAgIH1cbiAgICBdXG4gIH07XG59OyIsIm1vZHVsZS5leHBvcnRzID0gZnVuY3Rpb24oaGxqcykge1xuICB2YXIgSkFWQV9JREVOVF9SRSA9ICdbXFx1MDBDMC1cXHUwMkI4YS16QS1aXyRdW1xcdTAwQzAtXFx1MDJCOGEtekEtWl8kMC05XSonO1xuICB2YXIgR0VORVJJQ19JREVOVF9SRSA9IEpBVkFfSURFTlRfUkUgKyAnKDwnICsgSkFWQV9JREVOVF9SRSArICcoXFxcXHMqLFxcXFxzKicgKyBKQVZBX0lERU5UX1JFICsgJykqPik/JztcbiAgdmFyIEtFWVdPUkRTID1cbiAgICAnZmFsc2Ugc3luY2hyb25pemVkIGludCBhYnN0cmFjdCBmbG9hdCBwcml2YXRlIGNoYXIgYm9vbGVhbiB2YXIgc3RhdGljIG51bGwgaWYgY29uc3QgJyArXG4gICAgJ2ZvciB0cnVlIHdoaWxlIGxvbmcgc3RyaWN0ZnAgZmluYWxseSBwcm90ZWN0ZWQgaW1wb3J0IG5hdGl2ZSBmaW5hbCB2b2lkICcgK1xuICAgICdlbnVtIGVsc2UgYnJlYWsgdHJhbnNpZW50IGNhdGNoIGluc3RhbmNlb2YgYnl0ZSBzdXBlciB2b2xhdGlsZSBjYXNlIGFzc2VydCBzaG9ydCAnICtcbiAgICAncGFja2FnZSBkZWZhdWx0IGRvdWJsZSBwdWJsaWMgdHJ5IHRoaXMgc3dpdGNoIGNvbnRpbnVlIHRocm93cyBwcm90ZWN0ZWQgcHVibGljIHByaXZhdGUgJyArXG4gICAgJ21vZHVsZSByZXF1aXJlcyBleHBvcnRzIGRvJztcblxuICAvLyBodHRwczovL2RvY3Mub3JhY2xlLmNvbS9qYXZhc2UvNy9kb2NzL3RlY2hub3Rlcy9ndWlkZXMvbGFuZ3VhZ2UvdW5kZXJzY29yZXMtbGl0ZXJhbHMuaHRtbFxuICB2YXIgSkFWQV9OVU1CRVJfUkUgPSAnXFxcXGInICtcbiAgICAnKCcgK1xuICAgICAgJzBbYkJdKFswMV0rWzAxX10rWzAxXSt8WzAxXSspJyArIC8vIDBiLi4uXG4gICAgICAnfCcgK1xuICAgICAgJzBbeFhdKFthLWZBLUYwLTldK1thLWZBLUYwLTlfXStbYS1mQS1GMC05XSt8W2EtZkEtRjAtOV0rKScgKyAvLyAweC4uLlxuICAgICAgJ3wnICtcbiAgICAgICcoJyArXG4gICAgICAgICcoW1xcXFxkXStbXFxcXGRfXStbXFxcXGRdK3xbXFxcXGRdKykoXFxcXC4oW1xcXFxkXStbXFxcXGRfXStbXFxcXGRdK3xbXFxcXGRdKykpPycgK1xuICAgICAgICAnfCcgK1xuICAgICAgICAnXFxcXC4oW1xcXFxkXStbXFxcXGRfXStbXFxcXGRdK3xbXFxcXGRdKyknICtcbiAgICAgICcpJyArXG4gICAgICAnKFtlRV1bLStdP1xcXFxkKyk/JyArIC8vIG9jdGFsLCBkZWNpbWFsLCBmbG9hdFxuICAgICcpJyArXG4gICAgJ1tsTGZGXT8nO1xuICB2YXIgSkFWQV9OVU1CRVJfTU9ERSA9IHtcbiAgICBjbGFzc05hbWU6ICdudW1iZXInLFxuICAgIGJlZ2luOiBKQVZBX05VTUJFUl9SRSxcbiAgICByZWxldmFuY2U6IDBcbiAgfTtcblxuICByZXR1cm4ge1xuICAgIGFsaWFzZXM6IFsnanNwJ10sXG4gICAga2V5d29yZHM6IEtFWVdPUkRTLFxuICAgIGlsbGVnYWw6IC88XFwvfCMvLFxuICAgIGNvbnRhaW5zOiBbXG4gICAgICBobGpzLkNPTU1FTlQoXG4gICAgICAgICcvXFxcXCpcXFxcKicsXG4gICAgICAgICdcXFxcKi8nLFxuICAgICAgICB7XG4gICAgICAgICAgcmVsZXZhbmNlIDogMCxcbiAgICAgICAgICBjb250YWlucyA6IFtcbiAgICAgICAgICAgIHtcbiAgICAgICAgICAgICAgLy8gZWF0IHVwIEAncyBpbiBlbWFpbHMgdG8gcHJldmVudCB0aGVtIHRvIGJlIHJlY29nbml6ZWQgYXMgZG9jdGFnc1xuICAgICAgICAgICAgICBiZWdpbjogL1xcdytALywgcmVsZXZhbmNlOiAwXG4gICAgICAgICAgICB9LFxuICAgICAgICAgICAge1xuICAgICAgICAgICAgICBjbGFzc05hbWUgOiAnZG9jdGFnJyxcbiAgICAgICAgICAgICAgYmVnaW4gOiAnQFtBLVphLXpdKydcbiAgICAgICAgICAgIH1cbiAgICAgICAgICBdXG4gICAgICAgIH1cbiAgICAgICksXG4gICAgICBobGpzLkNfTElORV9DT01NRU5UX01PREUsXG4gICAgICBobGpzLkNfQkxPQ0tfQ09NTUVOVF9NT0RFLFxuICAgICAgaGxqcy5BUE9TX1NUUklOR19NT0RFLFxuICAgICAgaGxqcy5RVU9URV9TVFJJTkdfTU9ERSxcbiAgICAgIHtcbiAgICAgICAgY2xhc3NOYW1lOiAnY2xhc3MnLFxuICAgICAgICBiZWdpbktleXdvcmRzOiAnY2xhc3MgaW50ZXJmYWNlJywgZW5kOiAvW3s7PV0vLCBleGNsdWRlRW5kOiB0cnVlLFxuICAgICAgICBrZXl3b3JkczogJ2NsYXNzIGludGVyZmFjZScsXG4gICAgICAgIGlsbGVnYWw6IC9bOlwiXFxbXFxdXS8sXG4gICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAge2JlZ2luS2V5d29yZHM6ICdleHRlbmRzIGltcGxlbWVudHMnfSxcbiAgICAgICAgICBobGpzLlVOREVSU0NPUkVfVElUTEVfTU9ERVxuICAgICAgICBdXG4gICAgICB9LFxuICAgICAge1xuICAgICAgICAvLyBFeHByZXNzaW9uIGtleXdvcmRzIHByZXZlbnQgJ2tleXdvcmQgTmFtZSguLi4pJyBmcm9tIGJlaW5nXG4gICAgICAgIC8vIHJlY29nbml6ZWQgYXMgYSBmdW5jdGlvbiBkZWZpbml0aW9uXG4gICAgICAgIGJlZ2luS2V5d29yZHM6ICduZXcgdGhyb3cgcmV0dXJuIGVsc2UnLFxuICAgICAgICByZWxldmFuY2U6IDBcbiAgICAgIH0sXG4gICAgICB7XG4gICAgICAgIGNsYXNzTmFtZTogJ2Z1bmN0aW9uJyxcbiAgICAgICAgYmVnaW46ICcoJyArIEdFTkVSSUNfSURFTlRfUkUgKyAnXFxcXHMrKSsnICsgaGxqcy5VTkRFUlNDT1JFX0lERU5UX1JFICsgJ1xcXFxzKlxcXFwoJywgcmV0dXJuQmVnaW46IHRydWUsIGVuZDogL1t7Oz1dLyxcbiAgICAgICAgZXhjbHVkZUVuZDogdHJ1ZSxcbiAgICAgICAga2V5d29yZHM6IEtFWVdPUkRTLFxuICAgICAgICBjb250YWluczogW1xuICAgICAgICAgIHtcbiAgICAgICAgICAgIGJlZ2luOiBobGpzLlVOREVSU0NPUkVfSURFTlRfUkUgKyAnXFxcXHMqXFxcXCgnLCByZXR1cm5CZWdpbjogdHJ1ZSxcbiAgICAgICAgICAgIHJlbGV2YW5jZTogMCxcbiAgICAgICAgICAgIGNvbnRhaW5zOiBbaGxqcy5VTkRFUlNDT1JFX1RJVExFX01PREVdXG4gICAgICAgICAgfSxcbiAgICAgICAgICB7XG4gICAgICAgICAgICBjbGFzc05hbWU6ICdwYXJhbXMnLFxuICAgICAgICAgICAgYmVnaW46IC9cXCgvLCBlbmQ6IC9cXCkvLFxuICAgICAgICAgICAga2V5d29yZHM6IEtFWVdPUkRTLFxuICAgICAgICAgICAgcmVsZXZhbmNlOiAwLFxuICAgICAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICAgICAgaGxqcy5BUE9TX1NUUklOR19NT0RFLFxuICAgICAgICAgICAgICBobGpzLlFVT1RFX1NUUklOR19NT0RFLFxuICAgICAgICAgICAgICBobGpzLkNfTlVNQkVSX01PREUsXG4gICAgICAgICAgICAgIGhsanMuQ19CTE9DS19DT01NRU5UX01PREVcbiAgICAgICAgICAgIF1cbiAgICAgICAgICB9LFxuICAgICAgICAgIGhsanMuQ19MSU5FX0NPTU1FTlRfTU9ERSxcbiAgICAgICAgICBobGpzLkNfQkxPQ0tfQ09NTUVOVF9NT0RFXG4gICAgICAgIF1cbiAgICAgIH0sXG4gICAgICBKQVZBX05VTUJFUl9NT0RFLFxuICAgICAge1xuICAgICAgICBjbGFzc05hbWU6ICdtZXRhJywgYmVnaW46ICdAW0EtWmEtel0rJ1xuICAgICAgfVxuICAgIF1cbiAgfTtcbn07IiwibW9kdWxlLmV4cG9ydHMgPSBmdW5jdGlvbihobGpzKSB7XG4gIHZhciBJREVOVF9SRSA9ICdbQS1aYS16JF9dWzAtOUEtWmEteiRfXSonO1xuICB2YXIgS0VZV09SRFMgPSB7XG4gICAga2V5d29yZDpcbiAgICAgICdpbiBvZiBpZiBmb3Igd2hpbGUgZmluYWxseSB2YXIgbmV3IGZ1bmN0aW9uIGRvIHJldHVybiB2b2lkIGVsc2UgYnJlYWsgY2F0Y2ggJyArXG4gICAgICAnaW5zdGFuY2VvZiB3aXRoIHRocm93IGNhc2UgZGVmYXVsdCB0cnkgdGhpcyBzd2l0Y2ggY29udGludWUgdHlwZW9mIGRlbGV0ZSAnICtcbiAgICAgICdsZXQgeWllbGQgY29uc3QgZXhwb3J0IHN1cGVyIGRlYnVnZ2VyIGFzIGFzeW5jIGF3YWl0IHN0YXRpYyAnICtcbiAgICAgIC8vIEVDTUFTY3JpcHQgNiBtb2R1bGVzIGltcG9ydFxuICAgICAgJ2ltcG9ydCBmcm9tIGFzJ1xuICAgICxcbiAgICBsaXRlcmFsOlxuICAgICAgJ3RydWUgZmFsc2UgbnVsbCB1bmRlZmluZWQgTmFOIEluZmluaXR5JyxcbiAgICBidWlsdF9pbjpcbiAgICAgICdldmFsIGlzRmluaXRlIGlzTmFOIHBhcnNlRmxvYXQgcGFyc2VJbnQgZGVjb2RlVVJJIGRlY29kZVVSSUNvbXBvbmVudCAnICtcbiAgICAgICdlbmNvZGVVUkkgZW5jb2RlVVJJQ29tcG9uZW50IGVzY2FwZSB1bmVzY2FwZSBPYmplY3QgRnVuY3Rpb24gQm9vbGVhbiBFcnJvciAnICtcbiAgICAgICdFdmFsRXJyb3IgSW50ZXJuYWxFcnJvciBSYW5nZUVycm9yIFJlZmVyZW5jZUVycm9yIFN0b3BJdGVyYXRpb24gU3ludGF4RXJyb3IgJyArXG4gICAgICAnVHlwZUVycm9yIFVSSUVycm9yIE51bWJlciBNYXRoIERhdGUgU3RyaW5nIFJlZ0V4cCBBcnJheSBGbG9hdDMyQXJyYXkgJyArXG4gICAgICAnRmxvYXQ2NEFycmF5IEludDE2QXJyYXkgSW50MzJBcnJheSBJbnQ4QXJyYXkgVWludDE2QXJyYXkgVWludDMyQXJyYXkgJyArXG4gICAgICAnVWludDhBcnJheSBVaW50OENsYW1wZWRBcnJheSBBcnJheUJ1ZmZlciBEYXRhVmlldyBKU09OIEludGwgYXJndW1lbnRzIHJlcXVpcmUgJyArXG4gICAgICAnbW9kdWxlIGNvbnNvbGUgd2luZG93IGRvY3VtZW50IFN5bWJvbCBTZXQgTWFwIFdlYWtTZXQgV2Vha01hcCBQcm94eSBSZWZsZWN0ICcgK1xuICAgICAgJ1Byb21pc2UnXG4gIH07XG4gIHZhciBOVU1CRVIgPSB7XG4gICAgY2xhc3NOYW1lOiAnbnVtYmVyJyxcbiAgICB2YXJpYW50czogW1xuICAgICAgeyBiZWdpbjogJ1xcXFxiKDBbYkJdWzAxXSspJyB9LFxuICAgICAgeyBiZWdpbjogJ1xcXFxiKDBbb09dWzAtN10rKScgfSxcbiAgICAgIHsgYmVnaW46IGhsanMuQ19OVU1CRVJfUkUgfVxuICAgIF0sXG4gICAgcmVsZXZhbmNlOiAwXG4gIH07XG4gIHZhciBTVUJTVCA9IHtcbiAgICBjbGFzc05hbWU6ICdzdWJzdCcsXG4gICAgYmVnaW46ICdcXFxcJFxcXFx7JywgZW5kOiAnXFxcXH0nLFxuICAgIGtleXdvcmRzOiBLRVlXT1JEUyxcbiAgICBjb250YWluczogW10gIC8vIGRlZmluZWQgbGF0ZXJcbiAgfTtcbiAgdmFyIFRFTVBMQVRFX1NUUklORyA9IHtcbiAgICBjbGFzc05hbWU6ICdzdHJpbmcnLFxuICAgIGJlZ2luOiAnYCcsIGVuZDogJ2AnLFxuICAgIGNvbnRhaW5zOiBbXG4gICAgICBobGpzLkJBQ0tTTEFTSF9FU0NBUEUsXG4gICAgICBTVUJTVFxuICAgIF1cbiAgfTtcbiAgU1VCU1QuY29udGFpbnMgPSBbXG4gICAgaGxqcy5BUE9TX1NUUklOR19NT0RFLFxuICAgIGhsanMuUVVPVEVfU1RSSU5HX01PREUsXG4gICAgVEVNUExBVEVfU1RSSU5HLFxuICAgIE5VTUJFUixcbiAgICBobGpzLlJFR0VYUF9NT0RFXG4gIF1cbiAgdmFyIFBBUkFNU19DT05UQUlOUyA9IFNVQlNULmNvbnRhaW5zLmNvbmNhdChbXG4gICAgaGxqcy5DX0JMT0NLX0NPTU1FTlRfTU9ERSxcbiAgICBobGpzLkNfTElORV9DT01NRU5UX01PREVcbiAgXSk7XG5cbiAgcmV0dXJuIHtcbiAgICBhbGlhc2VzOiBbJ2pzJywgJ2pzeCddLFxuICAgIGtleXdvcmRzOiBLRVlXT1JEUyxcbiAgICBjb250YWluczogW1xuICAgICAge1xuICAgICAgICBjbGFzc05hbWU6ICdtZXRhJyxcbiAgICAgICAgcmVsZXZhbmNlOiAxMCxcbiAgICAgICAgYmVnaW46IC9eXFxzKlsnXCJddXNlIChzdHJpY3R8YXNtKVsnXCJdL1xuICAgICAgfSxcbiAgICAgIHtcbiAgICAgICAgY2xhc3NOYW1lOiAnbWV0YScsXG4gICAgICAgIGJlZ2luOiAvXiMhLywgZW5kOiAvJC9cbiAgICAgIH0sXG4gICAgICBobGpzLkFQT1NfU1RSSU5HX01PREUsXG4gICAgICBobGpzLlFVT1RFX1NUUklOR19NT0RFLFxuICAgICAgVEVNUExBVEVfU1RSSU5HLFxuICAgICAgaGxqcy5DX0xJTkVfQ09NTUVOVF9NT0RFLFxuICAgICAgaGxqcy5DX0JMT0NLX0NPTU1FTlRfTU9ERSxcbiAgICAgIE5VTUJFUixcbiAgICAgIHsgLy8gb2JqZWN0IGF0dHIgY29udGFpbmVyXG4gICAgICAgIGJlZ2luOiAvW3ssXVxccyovLCByZWxldmFuY2U6IDAsXG4gICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAge1xuICAgICAgICAgICAgYmVnaW46IElERU5UX1JFICsgJ1xcXFxzKjonLCByZXR1cm5CZWdpbjogdHJ1ZSxcbiAgICAgICAgICAgIHJlbGV2YW5jZTogMCxcbiAgICAgICAgICAgIGNvbnRhaW5zOiBbe2NsYXNzTmFtZTogJ2F0dHInLCBiZWdpbjogSURFTlRfUkUsIHJlbGV2YW5jZTogMH1dXG4gICAgICAgICAgfVxuICAgICAgICBdXG4gICAgICB9LFxuICAgICAgeyAvLyBcInZhbHVlXCIgY29udGFpbmVyXG4gICAgICAgIGJlZ2luOiAnKCcgKyBobGpzLlJFX1NUQVJURVJTX1JFICsgJ3xcXFxcYihjYXNlfHJldHVybnx0aHJvdylcXFxcYilcXFxccyonLFxuICAgICAgICBrZXl3b3JkczogJ3JldHVybiB0aHJvdyBjYXNlJyxcbiAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICBobGpzLkNfTElORV9DT01NRU5UX01PREUsXG4gICAgICAgICAgaGxqcy5DX0JMT0NLX0NPTU1FTlRfTU9ERSxcbiAgICAgICAgICBobGpzLlJFR0VYUF9NT0RFLFxuICAgICAgICAgIHtcbiAgICAgICAgICAgIGNsYXNzTmFtZTogJ2Z1bmN0aW9uJyxcbiAgICAgICAgICAgIGJlZ2luOiAnKFxcXFwoLio/XFxcXCl8JyArIElERU5UX1JFICsgJylcXFxccyo9PicsIHJldHVybkJlZ2luOiB0cnVlLFxuICAgICAgICAgICAgZW5kOiAnXFxcXHMqPT4nLFxuICAgICAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICAgICAge1xuICAgICAgICAgICAgICAgIGNsYXNzTmFtZTogJ3BhcmFtcycsXG4gICAgICAgICAgICAgICAgdmFyaWFudHM6IFtcbiAgICAgICAgICAgICAgICAgIHtcbiAgICAgICAgICAgICAgICAgICAgYmVnaW46IElERU5UX1JFXG4gICAgICAgICAgICAgICAgICB9LFxuICAgICAgICAgICAgICAgICAge1xuICAgICAgICAgICAgICAgICAgICBiZWdpbjogL1xcKFxccypcXCkvLFxuICAgICAgICAgICAgICAgICAgfSxcbiAgICAgICAgICAgICAgICAgIHtcbiAgICAgICAgICAgICAgICAgICAgYmVnaW46IC9cXCgvLCBlbmQ6IC9cXCkvLFxuICAgICAgICAgICAgICAgICAgICBleGNsdWRlQmVnaW46IHRydWUsIGV4Y2x1ZGVFbmQ6IHRydWUsXG4gICAgICAgICAgICAgICAgICAgIGtleXdvcmRzOiBLRVlXT1JEUyxcbiAgICAgICAgICAgICAgICAgICAgY29udGFpbnM6IFBBUkFNU19DT05UQUlOU1xuICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIF1cbiAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgXVxuICAgICAgICAgIH0sXG4gICAgICAgICAgeyAvLyBFNFggLyBKU1hcbiAgICAgICAgICAgIGJlZ2luOiAvPC8sIGVuZDogLyhcXC9cXHcrfFxcdytcXC8pPi8sXG4gICAgICAgICAgICBzdWJMYW5ndWFnZTogJ3htbCcsXG4gICAgICAgICAgICBjb250YWluczogW1xuICAgICAgICAgICAgICB7YmVnaW46IC88XFx3K1xccypcXC8+Lywgc2tpcDogdHJ1ZX0sXG4gICAgICAgICAgICAgIHtcbiAgICAgICAgICAgICAgICBiZWdpbjogLzxcXHcrLywgZW5kOiAvKFxcL1xcdyt8XFx3K1xcLyk+Lywgc2tpcDogdHJ1ZSxcbiAgICAgICAgICAgICAgICBjb250YWluczogW1xuICAgICAgICAgICAgICAgICAge2JlZ2luOiAvPFxcdytcXHMqXFwvPi8sIHNraXA6IHRydWV9LFxuICAgICAgICAgICAgICAgICAgJ3NlbGYnXG4gICAgICAgICAgICAgICAgXVxuICAgICAgICAgICAgICB9XG4gICAgICAgICAgICBdXG4gICAgICAgICAgfVxuICAgICAgICBdLFxuICAgICAgICByZWxldmFuY2U6IDBcbiAgICAgIH0sXG4gICAgICB7XG4gICAgICAgIGNsYXNzTmFtZTogJ2Z1bmN0aW9uJyxcbiAgICAgICAgYmVnaW5LZXl3b3JkczogJ2Z1bmN0aW9uJywgZW5kOiAvXFx7LywgZXhjbHVkZUVuZDogdHJ1ZSxcbiAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICBobGpzLmluaGVyaXQoaGxqcy5USVRMRV9NT0RFLCB7YmVnaW46IElERU5UX1JFfSksXG4gICAgICAgICAge1xuICAgICAgICAgICAgY2xhc3NOYW1lOiAncGFyYW1zJyxcbiAgICAgICAgICAgIGJlZ2luOiAvXFwoLywgZW5kOiAvXFwpLyxcbiAgICAgICAgICAgIGV4Y2x1ZGVCZWdpbjogdHJ1ZSxcbiAgICAgICAgICAgIGV4Y2x1ZGVFbmQ6IHRydWUsXG4gICAgICAgICAgICBjb250YWluczogUEFSQU1TX0NPTlRBSU5TXG4gICAgICAgICAgfVxuICAgICAgICBdLFxuICAgICAgICBpbGxlZ2FsOiAvXFxbfCUvXG4gICAgICB9LFxuICAgICAge1xuICAgICAgICBiZWdpbjogL1xcJFsoLl0vIC8vIHJlbGV2YW5jZSBib29zdGVyIGZvciBhIHBhdHRlcm4gY29tbW9uIHRvIEpTIGxpYnM6IGAkKHNvbWV0aGluZylgIGFuZCBgJC5zb21ldGhpbmdgXG4gICAgICB9LFxuICAgICAgaGxqcy5NRVRIT0RfR1VBUkQsXG4gICAgICB7IC8vIEVTNiBjbGFzc1xuICAgICAgICBjbGFzc05hbWU6ICdjbGFzcycsXG4gICAgICAgIGJlZ2luS2V5d29yZHM6ICdjbGFzcycsIGVuZDogL1t7Oz1dLywgZXhjbHVkZUVuZDogdHJ1ZSxcbiAgICAgICAgaWxsZWdhbDogL1s6XCJcXFtcXF1dLyxcbiAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICB7YmVnaW5LZXl3b3JkczogJ2V4dGVuZHMnfSxcbiAgICAgICAgICBobGpzLlVOREVSU0NPUkVfVElUTEVfTU9ERVxuICAgICAgICBdXG4gICAgICB9LFxuICAgICAge1xuICAgICAgICBiZWdpbktleXdvcmRzOiAnY29uc3RydWN0b3IgZ2V0IHNldCcsIGVuZDogL1xcey8sIGV4Y2x1ZGVFbmQ6IHRydWVcbiAgICAgIH1cbiAgICBdLFxuICAgIGlsbGVnYWw6IC8jKD8hISkvXG4gIH07XG59OyIsIm1vZHVsZS5leHBvcnRzID0gZnVuY3Rpb24oaGxqcykge1xuXG4gIHZhciBBTk5PVEFUSU9OID0geyBjbGFzc05hbWU6ICdtZXRhJywgYmVnaW46ICdAW0EtWmEtel0rJyB9O1xuXG4gIC8vIHVzZWQgaW4gc3RyaW5ncyBmb3IgZXNjYXBpbmcvaW50ZXJwb2xhdGlvbi9zdWJzdGl0dXRpb25cbiAgdmFyIFNVQlNUID0ge1xuICAgIGNsYXNzTmFtZTogJ3N1YnN0JyxcbiAgICB2YXJpYW50czogW1xuICAgICAge2JlZ2luOiAnXFxcXCRbQS1aYS16MC05X10rJ30sXG4gICAgICB7YmVnaW46ICdcXFxcJHsnLCBlbmQ6ICd9J31cbiAgICBdXG4gIH07XG5cbiAgdmFyIFNUUklORyA9IHtcbiAgICBjbGFzc05hbWU6ICdzdHJpbmcnLFxuICAgIHZhcmlhbnRzOiBbXG4gICAgICB7XG4gICAgICAgIGJlZ2luOiAnXCInLCBlbmQ6ICdcIicsXG4gICAgICAgIGlsbGVnYWw6ICdcXFxcbicsXG4gICAgICAgIGNvbnRhaW5zOiBbaGxqcy5CQUNLU0xBU0hfRVNDQVBFXVxuICAgICAgfSxcbiAgICAgIHtcbiAgICAgICAgYmVnaW46ICdcIlwiXCInLCBlbmQ6ICdcIlwiXCInLFxuICAgICAgICByZWxldmFuY2U6IDEwXG4gICAgICB9LFxuICAgICAge1xuICAgICAgICBiZWdpbjogJ1thLXpdK1wiJywgZW5kOiAnXCInLFxuICAgICAgICBpbGxlZ2FsOiAnXFxcXG4nLFxuICAgICAgICBjb250YWluczogW2hsanMuQkFDS1NMQVNIX0VTQ0FQRSwgU1VCU1RdXG4gICAgICB9LFxuICAgICAge1xuICAgICAgICBjbGFzc05hbWU6ICdzdHJpbmcnLFxuICAgICAgICBiZWdpbjogJ1thLXpdK1wiXCJcIicsIGVuZDogJ1wiXCJcIicsXG4gICAgICAgIGNvbnRhaW5zOiBbU1VCU1RdLFxuICAgICAgICByZWxldmFuY2U6IDEwXG4gICAgICB9XG4gICAgXVxuXG4gIH07XG5cbiAgdmFyIFNZTUJPTCA9IHtcbiAgICBjbGFzc05hbWU6ICdzeW1ib2wnLFxuICAgIGJlZ2luOiAnXFwnXFxcXHdbXFxcXHdcXFxcZF9dKig/IVxcJyknXG4gIH07XG5cbiAgdmFyIFRZUEUgPSB7XG4gICAgY2xhc3NOYW1lOiAndHlwZScsXG4gICAgYmVnaW46ICdcXFxcYltBLVpdW0EtWmEtejAtOV9dKicsXG4gICAgcmVsZXZhbmNlOiAwXG4gIH07XG5cbiAgdmFyIE5BTUUgPSB7XG4gICAgY2xhc3NOYW1lOiAndGl0bGUnLFxuICAgIGJlZ2luOiAvW14wLTlcXG5cXHQgXCInKCksLmB7fVxcW1xcXTo7XVteXFxuXFx0IFwiJygpLC5ge31cXFtcXF06O10rfFteMC05XFxuXFx0IFwiJygpLC5ge31cXFtcXF06Oz1dLyxcbiAgICByZWxldmFuY2U6IDBcbiAgfTtcblxuICB2YXIgQ0xBU1MgPSB7XG4gICAgY2xhc3NOYW1lOiAnY2xhc3MnLFxuICAgIGJlZ2luS2V5d29yZHM6ICdjbGFzcyBvYmplY3QgdHJhaXQgdHlwZScsXG4gICAgZW5kOiAvWzo9e1xcW1xcbjtdLyxcbiAgICBleGNsdWRlRW5kOiB0cnVlLFxuICAgIGNvbnRhaW5zOiBbXG4gICAgICB7XG4gICAgICAgIGJlZ2luS2V5d29yZHM6ICdleHRlbmRzIHdpdGgnLFxuICAgICAgICByZWxldmFuY2U6IDEwXG4gICAgICB9LFxuICAgICAge1xuICAgICAgICBiZWdpbjogL1xcWy8sXG4gICAgICAgIGVuZDogL1xcXS8sXG4gICAgICAgIGV4Y2x1ZGVCZWdpbjogdHJ1ZSxcbiAgICAgICAgZXhjbHVkZUVuZDogdHJ1ZSxcbiAgICAgICAgcmVsZXZhbmNlOiAwLFxuICAgICAgICBjb250YWluczogW1RZUEVdXG4gICAgICB9LFxuICAgICAge1xuICAgICAgICBjbGFzc05hbWU6ICdwYXJhbXMnLFxuICAgICAgICBiZWdpbjogL1xcKC8sXG4gICAgICAgIGVuZDogL1xcKS8sXG4gICAgICAgIGV4Y2x1ZGVCZWdpbjogdHJ1ZSxcbiAgICAgICAgZXhjbHVkZUVuZDogdHJ1ZSxcbiAgICAgICAgcmVsZXZhbmNlOiAwLFxuICAgICAgICBjb250YWluczogW1RZUEVdXG4gICAgICB9LFxuICAgICAgTkFNRVxuICAgIF1cbiAgfTtcblxuICB2YXIgTUVUSE9EID0ge1xuICAgIGNsYXNzTmFtZTogJ2Z1bmN0aW9uJyxcbiAgICBiZWdpbktleXdvcmRzOiAnZGVmJyxcbiAgICBlbmQ6IC9bOj17XFxbKFxcbjtdLyxcbiAgICBleGNsdWRlRW5kOiB0cnVlLFxuICAgIGNvbnRhaW5zOiBbTkFNRV1cbiAgfTtcblxuICByZXR1cm4ge1xuICAgIGtleXdvcmRzOiB7XG4gICAgICBsaXRlcmFsOiAndHJ1ZSBmYWxzZSBudWxsJyxcbiAgICAgIGtleXdvcmQ6ICd0eXBlIHlpZWxkIGxhenkgb3ZlcnJpZGUgZGVmIHdpdGggdmFsIHZhciBzZWFsZWQgYWJzdHJhY3QgcHJpdmF0ZSB0cmFpdCBvYmplY3QgaWYgZm9yU29tZSBmb3Igd2hpbGUgdGhyb3cgZmluYWxseSBwcm90ZWN0ZWQgZXh0ZW5kcyBpbXBvcnQgZmluYWwgcmV0dXJuIGVsc2UgYnJlYWsgbmV3IGNhdGNoIHN1cGVyIGNsYXNzIGNhc2UgcGFja2FnZSBkZWZhdWx0IHRyeSB0aGlzIG1hdGNoIGNvbnRpbnVlIHRocm93cyBpbXBsaWNpdCdcbiAgICB9LFxuICAgIGNvbnRhaW5zOiBbXG4gICAgICBobGpzLkNfTElORV9DT01NRU5UX01PREUsXG4gICAgICBobGpzLkNfQkxPQ0tfQ09NTUVOVF9NT0RFLFxuICAgICAgU1RSSU5HLFxuICAgICAgU1lNQk9MLFxuICAgICAgVFlQRSxcbiAgICAgIE1FVEhPRCxcbiAgICAgIENMQVNTLFxuICAgICAgaGxqcy5DX05VTUJFUl9NT0RFLFxuICAgICAgQU5OT1RBVElPTlxuICAgIF1cbiAgfTtcbn07IiwibW9kdWxlLmV4cG9ydHMgPSBmdW5jdGlvbihobGpzKSB7XG4gIHZhciBKU19JREVOVF9SRSA9ICdbQS1aYS16JF9dWzAtOUEtWmEteiRfXSonO1xuICB2YXIgS0VZV09SRFMgPSB7XG4gICAga2V5d29yZDpcbiAgICAgICdpbiBpZiBmb3Igd2hpbGUgZmluYWxseSB2YXIgbmV3IGZ1bmN0aW9uIGRvIHJldHVybiB2b2lkIGVsc2UgYnJlYWsgY2F0Y2ggJyArXG4gICAgICAnaW5zdGFuY2VvZiB3aXRoIHRocm93IGNhc2UgZGVmYXVsdCB0cnkgdGhpcyBzd2l0Y2ggY29udGludWUgdHlwZW9mIGRlbGV0ZSAnICtcbiAgICAgICdsZXQgeWllbGQgY29uc3QgY2xhc3MgcHVibGljIHByaXZhdGUgcHJvdGVjdGVkIGdldCBzZXQgc3VwZXIgJyArXG4gICAgICAnc3RhdGljIGltcGxlbWVudHMgZW51bSBleHBvcnQgaW1wb3J0IGRlY2xhcmUgdHlwZSBuYW1lc3BhY2UgYWJzdHJhY3QgJyArXG4gICAgICAnYXMgZnJvbSBleHRlbmRzIGFzeW5jIGF3YWl0JyxcbiAgICBsaXRlcmFsOlxuICAgICAgJ3RydWUgZmFsc2UgbnVsbCB1bmRlZmluZWQgTmFOIEluZmluaXR5JyxcbiAgICBidWlsdF9pbjpcbiAgICAgICdldmFsIGlzRmluaXRlIGlzTmFOIHBhcnNlRmxvYXQgcGFyc2VJbnQgZGVjb2RlVVJJIGRlY29kZVVSSUNvbXBvbmVudCAnICtcbiAgICAgICdlbmNvZGVVUkkgZW5jb2RlVVJJQ29tcG9uZW50IGVzY2FwZSB1bmVzY2FwZSBPYmplY3QgRnVuY3Rpb24gQm9vbGVhbiBFcnJvciAnICtcbiAgICAgICdFdmFsRXJyb3IgSW50ZXJuYWxFcnJvciBSYW5nZUVycm9yIFJlZmVyZW5jZUVycm9yIFN0b3BJdGVyYXRpb24gU3ludGF4RXJyb3IgJyArXG4gICAgICAnVHlwZUVycm9yIFVSSUVycm9yIE51bWJlciBNYXRoIERhdGUgU3RyaW5nIFJlZ0V4cCBBcnJheSBGbG9hdDMyQXJyYXkgJyArXG4gICAgICAnRmxvYXQ2NEFycmF5IEludDE2QXJyYXkgSW50MzJBcnJheSBJbnQ4QXJyYXkgVWludDE2QXJyYXkgVWludDMyQXJyYXkgJyArXG4gICAgICAnVWludDhBcnJheSBVaW50OENsYW1wZWRBcnJheSBBcnJheUJ1ZmZlciBEYXRhVmlldyBKU09OIEludGwgYXJndW1lbnRzIHJlcXVpcmUgJyArXG4gICAgICAnbW9kdWxlIGNvbnNvbGUgd2luZG93IGRvY3VtZW50IGFueSBudW1iZXIgYm9vbGVhbiBzdHJpbmcgdm9pZCBQcm9taXNlJ1xuICB9O1xuXG4gIHZhciBERUNPUkFUT1IgPSB7XG4gICAgY2xhc3NOYW1lOiAnbWV0YScsXG4gICAgYmVnaW46ICdAJyArIEpTX0lERU5UX1JFLFxuICB9O1xuXG4gIHZhciBBUkdTID1cbiAge1xuICAgIGJlZ2luOiAnXFxcXCgnLFxuICAgIGVuZDogL1xcKS8sXG4gICAga2V5d29yZHM6IEtFWVdPUkRTLFxuICAgIGNvbnRhaW5zOiBbXG4gICAgICAnc2VsZicsXG4gICAgICBobGpzLlFVT1RFX1NUUklOR19NT0RFLFxuICAgICAgaGxqcy5BUE9TX1NUUklOR19NT0RFLFxuICAgICAgaGxqcy5OVU1CRVJfTU9ERVxuICAgIF1cbiAgfTtcblxuICB2YXIgUEFSQU1TID0ge1xuICAgIGNsYXNzTmFtZTogJ3BhcmFtcycsXG4gICAgYmVnaW46IC9cXCgvLCBlbmQ6IC9cXCkvLFxuICAgIGV4Y2x1ZGVCZWdpbjogdHJ1ZSxcbiAgICBleGNsdWRlRW5kOiB0cnVlLFxuICAgIGtleXdvcmRzOiBLRVlXT1JEUyxcbiAgICBjb250YWluczogW1xuICAgICAgaGxqcy5DX0xJTkVfQ09NTUVOVF9NT0RFLFxuICAgICAgaGxqcy5DX0JMT0NLX0NPTU1FTlRfTU9ERSxcbiAgICAgIERFQ09SQVRPUixcbiAgICAgIEFSR1NcbiAgICBdXG4gIH07XG5cbiAgcmV0dXJuIHtcbiAgICBhbGlhc2VzOiBbJ3RzJ10sXG4gICAga2V5d29yZHM6IEtFWVdPUkRTLFxuICAgIGNvbnRhaW5zOiBbXG4gICAgICB7XG4gICAgICAgIGNsYXNzTmFtZTogJ21ldGEnLFxuICAgICAgICBiZWdpbjogL15cXHMqWydcIl11c2Ugc3RyaWN0WydcIl0vXG4gICAgICB9LFxuICAgICAgaGxqcy5BUE9TX1NUUklOR19NT0RFLFxuICAgICAgaGxqcy5RVU9URV9TVFJJTkdfTU9ERSxcbiAgICAgIHsgLy8gdGVtcGxhdGUgc3RyaW5nXG4gICAgICAgIGNsYXNzTmFtZTogJ3N0cmluZycsXG4gICAgICAgIGJlZ2luOiAnYCcsIGVuZDogJ2AnLFxuICAgICAgICBjb250YWluczogW1xuICAgICAgICAgIGhsanMuQkFDS1NMQVNIX0VTQ0FQRSxcbiAgICAgICAgICB7XG4gICAgICAgICAgICBjbGFzc05hbWU6ICdzdWJzdCcsXG4gICAgICAgICAgICBiZWdpbjogJ1xcXFwkXFxcXHsnLCBlbmQ6ICdcXFxcfSdcbiAgICAgICAgICB9XG4gICAgICAgIF1cbiAgICAgIH0sXG4gICAgICBobGpzLkNfTElORV9DT01NRU5UX01PREUsXG4gICAgICBobGpzLkNfQkxPQ0tfQ09NTUVOVF9NT0RFLFxuICAgICAge1xuICAgICAgICBjbGFzc05hbWU6ICdudW1iZXInLFxuICAgICAgICB2YXJpYW50czogW1xuICAgICAgICAgIHsgYmVnaW46ICdcXFxcYigwW2JCXVswMV0rKScgfSxcbiAgICAgICAgICB7IGJlZ2luOiAnXFxcXGIoMFtvT11bMC03XSspJyB9LFxuICAgICAgICAgIHsgYmVnaW46IGhsanMuQ19OVU1CRVJfUkUgfVxuICAgICAgICBdLFxuICAgICAgICByZWxldmFuY2U6IDBcbiAgICAgIH0sXG4gICAgICB7IC8vIFwidmFsdWVcIiBjb250YWluZXJcbiAgICAgICAgYmVnaW46ICcoJyArIGhsanMuUkVfU1RBUlRFUlNfUkUgKyAnfFxcXFxiKGNhc2V8cmV0dXJufHRocm93KVxcXFxiKVxcXFxzKicsXG4gICAgICAgIGtleXdvcmRzOiAncmV0dXJuIHRocm93IGNhc2UnLFxuICAgICAgICBjb250YWluczogW1xuICAgICAgICAgIGhsanMuQ19MSU5FX0NPTU1FTlRfTU9ERSxcbiAgICAgICAgICBobGpzLkNfQkxPQ0tfQ09NTUVOVF9NT0RFLFxuICAgICAgICAgIGhsanMuUkVHRVhQX01PREUsXG4gICAgICAgICAge1xuICAgICAgICAgICAgY2xhc3NOYW1lOiAnZnVuY3Rpb24nLFxuICAgICAgICAgICAgYmVnaW46ICcoXFxcXCguKj9cXFxcKXwnICsgaGxqcy5JREVOVF9SRSArICcpXFxcXHMqPT4nLCByZXR1cm5CZWdpbjogdHJ1ZSxcbiAgICAgICAgICAgIGVuZDogJ1xcXFxzKj0+JyxcbiAgICAgICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAgICAgIHtcbiAgICAgICAgICAgICAgICBjbGFzc05hbWU6ICdwYXJhbXMnLFxuICAgICAgICAgICAgICAgIHZhcmlhbnRzOiBbXG4gICAgICAgICAgICAgICAgICB7XG4gICAgICAgICAgICAgICAgICAgIGJlZ2luOiBobGpzLklERU5UX1JFXG4gICAgICAgICAgICAgICAgICB9LFxuICAgICAgICAgICAgICAgICAge1xuICAgICAgICAgICAgICAgICAgICBiZWdpbjogL1xcKFxccypcXCkvLFxuICAgICAgICAgICAgICAgICAgfSxcbiAgICAgICAgICAgICAgICAgIHtcbiAgICAgICAgICAgICAgICAgICAgYmVnaW46IC9cXCgvLCBlbmQ6IC9cXCkvLFxuICAgICAgICAgICAgICAgICAgICBleGNsdWRlQmVnaW46IHRydWUsIGV4Y2x1ZGVFbmQ6IHRydWUsXG4gICAgICAgICAgICAgICAgICAgIGtleXdvcmRzOiBLRVlXT1JEUyxcbiAgICAgICAgICAgICAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICAgICAgICAgICAgICAnc2VsZicsXG4gICAgICAgICAgICAgICAgICAgICAgaGxqcy5DX0xJTkVfQ09NTUVOVF9NT0RFLFxuICAgICAgICAgICAgICAgICAgICAgIGhsanMuQ19CTE9DS19DT01NRU5UX01PREVcbiAgICAgICAgICAgICAgICAgICAgXVxuICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIF1cbiAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgXVxuICAgICAgICAgIH1cbiAgICAgICAgXSxcbiAgICAgICAgcmVsZXZhbmNlOiAwXG4gICAgICB9LFxuICAgICAge1xuICAgICAgICBjbGFzc05hbWU6ICdmdW5jdGlvbicsXG4gICAgICAgIGJlZ2luOiAnZnVuY3Rpb24nLCBlbmQ6IC9bXFx7O10vLCBleGNsdWRlRW5kOiB0cnVlLFxuICAgICAgICBrZXl3b3JkczogS0VZV09SRFMsXG4gICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAgJ3NlbGYnLFxuICAgICAgICAgIGhsanMuaW5oZXJpdChobGpzLlRJVExFX01PREUsIHsgYmVnaW46IEpTX0lERU5UX1JFIH0pLFxuICAgICAgICAgIFBBUkFNU1xuICAgICAgICBdLFxuICAgICAgICBpbGxlZ2FsOiAvJS8sXG4gICAgICAgIHJlbGV2YW5jZTogMCAvLyAoKSA9PiB7fSBpcyBtb3JlIHR5cGljYWwgaW4gVHlwZVNjcmlwdFxuICAgICAgfSxcbiAgICAgIHtcbiAgICAgICAgYmVnaW5LZXl3b3JkczogJ2NvbnN0cnVjdG9yJywgZW5kOiAvXFx7LywgZXhjbHVkZUVuZDogdHJ1ZSxcbiAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICAnc2VsZicsXG4gICAgICAgICAgUEFSQU1TXG4gICAgICAgIF1cbiAgICAgIH0sXG4gICAgICB7IC8vIHByZXZlbnQgcmVmZXJlbmNlcyBsaWtlIG1vZHVsZS5pZCBmcm9tIGJlaW5nIGhpZ2xpZ2h0ZWQgYXMgbW9kdWxlIGRlZmluaXRpb25zXG4gICAgICAgIGJlZ2luOiAvbW9kdWxlXFwuLyxcbiAgICAgICAga2V5d29yZHM6IHsgYnVpbHRfaW46ICdtb2R1bGUnIH0sXG4gICAgICAgIHJlbGV2YW5jZTogMFxuICAgICAgfSxcbiAgICAgIHtcbiAgICAgICAgYmVnaW5LZXl3b3JkczogJ21vZHVsZScsIGVuZDogL1xcey8sIGV4Y2x1ZGVFbmQ6IHRydWVcbiAgICAgIH0sXG4gICAgICB7XG4gICAgICAgIGJlZ2luS2V5d29yZHM6ICdpbnRlcmZhY2UnLCBlbmQ6IC9cXHsvLCBleGNsdWRlRW5kOiB0cnVlLFxuICAgICAgICBrZXl3b3JkczogJ2ludGVyZmFjZSBleHRlbmRzJ1xuICAgICAgfSxcbiAgICAgIHtcbiAgICAgICAgYmVnaW46IC9cXCRbKC5dLyAvLyByZWxldmFuY2UgYm9vc3RlciBmb3IgYSBwYXR0ZXJuIGNvbW1vbiB0byBKUyBsaWJzOiBgJChzb21ldGhpbmcpYCBhbmQgYCQuc29tZXRoaW5nYFxuICAgICAgfSxcbiAgICAgIHtcbiAgICAgICAgYmVnaW46ICdcXFxcLicgKyBobGpzLklERU5UX1JFLCByZWxldmFuY2U6IDAgLy8gaGFjazogcHJldmVudHMgZGV0ZWN0aW9uIG9mIGtleXdvcmRzIGFmdGVyIGRvdHNcbiAgICAgIH0sXG4gICAgICBERUNPUkFUT1IsXG4gICAgICBBUkdTXG4gICAgXVxuICB9O1xufTsiLCIvKipcbkBsaWNlbnNlXG5Db3B5cmlnaHQgKGMpIDIwMTkgVGhlIFBvbHltZXIgUHJvamVjdCBBdXRob3JzLiBBbGwgcmlnaHRzIHJlc2VydmVkLlxuVGhpcyBjb2RlIG1heSBvbmx5IGJlIHVzZWQgdW5kZXIgdGhlIEJTRCBzdHlsZSBsaWNlbnNlIGZvdW5kIGF0XG5odHRwOi8vcG9seW1lci5naXRodWIuaW8vTElDRU5TRS50eHQgVGhlIGNvbXBsZXRlIHNldCBvZiBhdXRob3JzIG1heSBiZSBmb3VuZCBhdFxuaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0FVVEhPUlMudHh0IFRoZSBjb21wbGV0ZSBzZXQgb2YgY29udHJpYnV0b3JzIG1heSBiZVxuZm91bmQgYXQgaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0NPTlRSSUJVVE9SUy50eHQgQ29kZSBkaXN0cmlidXRlZCBieSBHb29nbGUgYXNcbnBhcnQgb2YgdGhlIHBvbHltZXIgcHJvamVjdCBpcyBhbHNvIHN1YmplY3QgdG8gYW4gYWRkaXRpb25hbCBJUCByaWdodHMgZ3JhbnRcbmZvdW5kIGF0IGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9QQVRFTlRTLnR4dFxuKi9cbmV4cG9ydCBjb25zdCBzdXBwb3J0c0Fkb3B0aW5nU3R5bGVTaGVldHMgPSAoJ2Fkb3B0ZWRTdHlsZVNoZWV0cycgaW4gRG9jdW1lbnQucHJvdG90eXBlKSAmJlxuICAgICgncmVwbGFjZScgaW4gQ1NTU3R5bGVTaGVldC5wcm90b3R5cGUpO1xuY29uc3QgY29uc3RydWN0aW9uVG9rZW4gPSBTeW1ib2woKTtcbmV4cG9ydCBjbGFzcyBDU1NSZXN1bHQge1xuICAgIGNvbnN0cnVjdG9yKGNzc1RleHQsIHNhZmVUb2tlbikge1xuICAgICAgICBpZiAoc2FmZVRva2VuICE9PSBjb25zdHJ1Y3Rpb25Ub2tlbikge1xuICAgICAgICAgICAgdGhyb3cgbmV3IEVycm9yKCdDU1NSZXN1bHQgaXMgbm90IGNvbnN0cnVjdGFibGUuIFVzZSBgdW5zYWZlQ1NTYCBvciBgY3NzYCBpbnN0ZWFkLicpO1xuICAgICAgICB9XG4gICAgICAgIHRoaXMuY3NzVGV4dCA9IGNzc1RleHQ7XG4gICAgfVxuICAgIC8vIE5vdGUsIHRoaXMgaXMgYSBnZXR0ZXIgc28gdGhhdCBpdCdzIGxhenkuIEluIHByYWN0aWNlLCB0aGlzIG1lYW5zXG4gICAgLy8gc3R5bGVzaGVldHMgYXJlIG5vdCBjcmVhdGVkIHVudGlsIHRoZSBmaXJzdCBlbGVtZW50IGluc3RhbmNlIGlzIG1hZGUuXG4gICAgZ2V0IHN0eWxlU2hlZXQoKSB7XG4gICAgICAgIGlmICh0aGlzLl9zdHlsZVNoZWV0ID09PSB1bmRlZmluZWQpIHtcbiAgICAgICAgICAgIC8vIE5vdGUsIGlmIGBhZG9wdGVkU3R5bGVTaGVldHNgIGlzIHN1cHBvcnRlZCB0aGVuIHdlIGFzc3VtZSBDU1NTdHlsZVNoZWV0XG4gICAgICAgICAgICAvLyBpcyBjb25zdHJ1Y3RhYmxlLlxuICAgICAgICAgICAgaWYgKHN1cHBvcnRzQWRvcHRpbmdTdHlsZVNoZWV0cykge1xuICAgICAgICAgICAgICAgIHRoaXMuX3N0eWxlU2hlZXQgPSBuZXcgQ1NTU3R5bGVTaGVldCgpO1xuICAgICAgICAgICAgICAgIHRoaXMuX3N0eWxlU2hlZXQucmVwbGFjZVN5bmModGhpcy5jc3NUZXh0KTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgICAgIGVsc2Uge1xuICAgICAgICAgICAgICAgIHRoaXMuX3N0eWxlU2hlZXQgPSBudWxsO1xuICAgICAgICAgICAgfVxuICAgICAgICB9XG4gICAgICAgIHJldHVybiB0aGlzLl9zdHlsZVNoZWV0O1xuICAgIH1cbiAgICB0b1N0cmluZygpIHtcbiAgICAgICAgcmV0dXJuIHRoaXMuY3NzVGV4dDtcbiAgICB9XG59XG4vKipcbiAqIFdyYXAgYSB2YWx1ZSBmb3IgaW50ZXJwb2xhdGlvbiBpbiBhIGNzcyB0YWdnZWQgdGVtcGxhdGUgbGl0ZXJhbC5cbiAqXG4gKiBUaGlzIGlzIHVuc2FmZSBiZWNhdXNlIHVudHJ1c3RlZCBDU1MgdGV4dCBjYW4gYmUgdXNlZCB0byBwaG9uZSBob21lXG4gKiBvciBleGZpbHRyYXRlIGRhdGEgdG8gYW4gYXR0YWNrZXIgY29udHJvbGxlZCBzaXRlLiBUYWtlIGNhcmUgdG8gb25seSB1c2VcbiAqIHRoaXMgd2l0aCB0cnVzdGVkIGlucHV0LlxuICovXG5leHBvcnQgY29uc3QgdW5zYWZlQ1NTID0gKHZhbHVlKSA9PiB7XG4gICAgcmV0dXJuIG5ldyBDU1NSZXN1bHQoU3RyaW5nKHZhbHVlKSwgY29uc3RydWN0aW9uVG9rZW4pO1xufTtcbmNvbnN0IHRleHRGcm9tQ1NTUmVzdWx0ID0gKHZhbHVlKSA9PiB7XG4gICAgaWYgKHZhbHVlIGluc3RhbmNlb2YgQ1NTUmVzdWx0KSB7XG4gICAgICAgIHJldHVybiB2YWx1ZS5jc3NUZXh0O1xuICAgIH1cbiAgICBlbHNlIHtcbiAgICAgICAgdGhyb3cgbmV3IEVycm9yKGBWYWx1ZSBwYXNzZWQgdG8gJ2NzcycgZnVuY3Rpb24gbXVzdCBiZSBhICdjc3MnIGZ1bmN0aW9uIHJlc3VsdDogJHt2YWx1ZX0uIFVzZSAndW5zYWZlQ1NTJyB0byBwYXNzIG5vbi1saXRlcmFsIHZhbHVlcywgYnV0XG4gICAgICAgICAgICB0YWtlIGNhcmUgdG8gZW5zdXJlIHBhZ2Ugc2VjdXJpdHkuYCk7XG4gICAgfVxufTtcbi8qKlxuICogVGVtcGxhdGUgdGFnIHdoaWNoIHdoaWNoIGNhbiBiZSB1c2VkIHdpdGggTGl0RWxlbWVudCdzIGBzdHlsZWAgcHJvcGVydHkgdG9cbiAqIHNldCBlbGVtZW50IHN0eWxlcy4gRm9yIHNlY3VyaXR5IHJlYXNvbnMsIG9ubHkgbGl0ZXJhbCBzdHJpbmcgdmFsdWVzIG1heSBiZVxuICogdXNlZC4gVG8gaW5jb3Jwb3JhdGUgbm9uLWxpdGVyYWwgdmFsdWVzIGB1bnNhZmVDU1NgIG1heSBiZSB1c2VkIGluc2lkZSBhXG4gKiB0ZW1wbGF0ZSBzdHJpbmcgcGFydC5cbiAqL1xuZXhwb3J0IGNvbnN0IGNzcyA9IChzdHJpbmdzLCAuLi52YWx1ZXMpID0+IHtcbiAgICBjb25zdCBjc3NUZXh0ID0gdmFsdWVzLnJlZHVjZSgoYWNjLCB2LCBpZHgpID0+IGFjYyArIHRleHRGcm9tQ1NTUmVzdWx0KHYpICsgc3RyaW5nc1tpZHggKyAxXSwgc3RyaW5nc1swXSk7XG4gICAgcmV0dXJuIG5ldyBDU1NSZXN1bHQoY3NzVGV4dCwgY29uc3RydWN0aW9uVG9rZW4pO1xufTtcbi8vIyBzb3VyY2VNYXBwaW5nVVJMPWNzcy10YWcuanMubWFwIiwiLyoqXG4gKiBAbGljZW5zZVxuICogQ29weXJpZ2h0IChjKSAyMDE3IFRoZSBQb2x5bWVyIFByb2plY3QgQXV0aG9ycy4gQWxsIHJpZ2h0cyByZXNlcnZlZC5cbiAqIFRoaXMgY29kZSBtYXkgb25seSBiZSB1c2VkIHVuZGVyIHRoZSBCU0Qgc3R5bGUgbGljZW5zZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0xJQ0VOU0UudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGF1dGhvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQVVUSE9SUy50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgY29udHJpYnV0b3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0NPTlRSSUJVVE9SUy50eHRcbiAqIENvZGUgZGlzdHJpYnV0ZWQgYnkgR29vZ2xlIGFzIHBhcnQgb2YgdGhlIHBvbHltZXIgcHJvamVjdCBpcyBhbHNvXG4gKiBzdWJqZWN0IHRvIGFuIGFkZGl0aW9uYWwgSVAgcmlnaHRzIGdyYW50IGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vUEFURU5UUy50eHRcbiAqL1xuY29uc3QgbGVnYWN5Q3VzdG9tRWxlbWVudCA9ICh0YWdOYW1lLCBjbGF6eikgPT4ge1xuICAgIHdpbmRvdy5jdXN0b21FbGVtZW50cy5kZWZpbmUodGFnTmFtZSwgY2xhenopO1xuICAgIC8vIENhc3QgYXMgYW55IGJlY2F1c2UgVFMgZG9lc24ndCByZWNvZ25pemUgdGhlIHJldHVybiB0eXBlIGFzIGJlaW5nIGFcbiAgICAvLyBzdWJ0eXBlIG9mIHRoZSBkZWNvcmF0ZWQgY2xhc3Mgd2hlbiBjbGF6eiBpcyB0eXBlZCBhc1xuICAgIC8vIGBDb25zdHJ1Y3RvcjxIVE1MRWxlbWVudD5gIGZvciBzb21lIHJlYXNvbi5cbiAgICAvLyBgQ29uc3RydWN0b3I8SFRNTEVsZW1lbnQ+YCBpcyBoZWxwZnVsIHRvIG1ha2Ugc3VyZSB0aGUgZGVjb3JhdG9yIGlzXG4gICAgLy8gYXBwbGllZCB0byBlbGVtZW50cyBob3dldmVyLlxuICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTpuby1hbnlcbiAgICByZXR1cm4gY2xheno7XG59O1xuY29uc3Qgc3RhbmRhcmRDdXN0b21FbGVtZW50ID0gKHRhZ05hbWUsIGRlc2NyaXB0b3IpID0+IHtcbiAgICBjb25zdCB7IGtpbmQsIGVsZW1lbnRzIH0gPSBkZXNjcmlwdG9yO1xuICAgIHJldHVybiB7XG4gICAgICAgIGtpbmQsXG4gICAgICAgIGVsZW1lbnRzLFxuICAgICAgICAvLyBUaGlzIGNhbGxiYWNrIGlzIGNhbGxlZCBvbmNlIHRoZSBjbGFzcyBpcyBvdGhlcndpc2UgZnVsbHkgZGVmaW5lZFxuICAgICAgICBmaW5pc2hlcihjbGF6eikge1xuICAgICAgICAgICAgd2luZG93LmN1c3RvbUVsZW1lbnRzLmRlZmluZSh0YWdOYW1lLCBjbGF6eik7XG4gICAgICAgIH1cbiAgICB9O1xufTtcbi8qKlxuICogQ2xhc3MgZGVjb3JhdG9yIGZhY3RvcnkgdGhhdCBkZWZpbmVzIHRoZSBkZWNvcmF0ZWQgY2xhc3MgYXMgYSBjdXN0b20gZWxlbWVudC5cbiAqXG4gKiBAcGFyYW0gdGFnTmFtZSB0aGUgbmFtZSBvZiB0aGUgY3VzdG9tIGVsZW1lbnQgdG8gZGVmaW5lXG4gKi9cbmV4cG9ydCBjb25zdCBjdXN0b21FbGVtZW50ID0gKHRhZ05hbWUpID0+IChjbGFzc09yRGVzY3JpcHRvcikgPT4gKHR5cGVvZiBjbGFzc09yRGVzY3JpcHRvciA9PT0gJ2Z1bmN0aW9uJykgP1xuICAgIGxlZ2FjeUN1c3RvbUVsZW1lbnQodGFnTmFtZSwgY2xhc3NPckRlc2NyaXB0b3IpIDpcbiAgICBzdGFuZGFyZEN1c3RvbUVsZW1lbnQodGFnTmFtZSwgY2xhc3NPckRlc2NyaXB0b3IpO1xuY29uc3Qgc3RhbmRhcmRQcm9wZXJ0eSA9IChvcHRpb25zLCBlbGVtZW50KSA9PiB7XG4gICAgLy8gV2hlbiBkZWNvcmF0aW5nIGFuIGFjY2Vzc29yLCBwYXNzIGl0IHRocm91Z2ggYW5kIGFkZCBwcm9wZXJ0eSBtZXRhZGF0YS5cbiAgICAvLyBOb3RlLCB0aGUgYGhhc093blByb3BlcnR5YCBjaGVjayBpbiBgY3JlYXRlUHJvcGVydHlgIGVuc3VyZXMgd2UgZG9uJ3RcbiAgICAvLyBzdG9tcCBvdmVyIHRoZSB1c2VyJ3MgYWNjZXNzb3IuXG4gICAgaWYgKGVsZW1lbnQua2luZCA9PT0gJ21ldGhvZCcgJiYgZWxlbWVudC5kZXNjcmlwdG9yICYmXG4gICAgICAgICEoJ3ZhbHVlJyBpbiBlbGVtZW50LmRlc2NyaXB0b3IpKSB7XG4gICAgICAgIHJldHVybiBPYmplY3QuYXNzaWduKHt9LCBlbGVtZW50LCB7IGZpbmlzaGVyKGNsYXp6KSB7XG4gICAgICAgICAgICAgICAgY2xhenouY3JlYXRlUHJvcGVydHkoZWxlbWVudC5rZXksIG9wdGlvbnMpO1xuICAgICAgICAgICAgfSB9KTtcbiAgICB9XG4gICAgZWxzZSB7XG4gICAgICAgIC8vIGNyZWF0ZVByb3BlcnR5KCkgdGFrZXMgY2FyZSBvZiBkZWZpbmluZyB0aGUgcHJvcGVydHksIGJ1dCB3ZSBzdGlsbFxuICAgICAgICAvLyBtdXN0IHJldHVybiBzb21lIGtpbmQgb2YgZGVzY3JpcHRvciwgc28gcmV0dXJuIGEgZGVzY3JpcHRvciBmb3IgYW5cbiAgICAgICAgLy8gdW51c2VkIHByb3RvdHlwZSBmaWVsZC4gVGhlIGZpbmlzaGVyIGNhbGxzIGNyZWF0ZVByb3BlcnR5KCkuXG4gICAgICAgIHJldHVybiB7XG4gICAgICAgICAgICBraW5kOiAnZmllbGQnLFxuICAgICAgICAgICAga2V5OiBTeW1ib2woKSxcbiAgICAgICAgICAgIHBsYWNlbWVudDogJ293bicsXG4gICAgICAgICAgICBkZXNjcmlwdG9yOiB7fSxcbiAgICAgICAgICAgIC8vIFdoZW4gQGJhYmVsL3BsdWdpbi1wcm9wb3NhbC1kZWNvcmF0b3JzIGltcGxlbWVudHMgaW5pdGlhbGl6ZXJzLFxuICAgICAgICAgICAgLy8gZG8gdGhpcyBpbnN0ZWFkIG9mIHRoZSBpbml0aWFsaXplciBiZWxvdy4gU2VlOlxuICAgICAgICAgICAgLy8gaHR0cHM6Ly9naXRodWIuY29tL2JhYmVsL2JhYmVsL2lzc3Vlcy85MjYwIGV4dHJhczogW1xuICAgICAgICAgICAgLy8gICB7XG4gICAgICAgICAgICAvLyAgICAga2luZDogJ2luaXRpYWxpemVyJyxcbiAgICAgICAgICAgIC8vICAgICBwbGFjZW1lbnQ6ICdvd24nLFxuICAgICAgICAgICAgLy8gICAgIGluaXRpYWxpemVyOiBkZXNjcmlwdG9yLmluaXRpYWxpemVyLFxuICAgICAgICAgICAgLy8gICB9XG4gICAgICAgICAgICAvLyBdLFxuICAgICAgICAgICAgLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOm5vLWFueSBkZWNvcmF0b3JcbiAgICAgICAgICAgIGluaXRpYWxpemVyKCkge1xuICAgICAgICAgICAgICAgIGlmICh0eXBlb2YgZWxlbWVudC5pbml0aWFsaXplciA9PT0gJ2Z1bmN0aW9uJykge1xuICAgICAgICAgICAgICAgICAgICB0aGlzW2VsZW1lbnQua2V5XSA9IGVsZW1lbnQuaW5pdGlhbGl6ZXIuY2FsbCh0aGlzKTtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICB9LFxuICAgICAgICAgICAgZmluaXNoZXIoY2xhenopIHtcbiAgICAgICAgICAgICAgICBjbGF6ei5jcmVhdGVQcm9wZXJ0eShlbGVtZW50LmtleSwgb3B0aW9ucyk7XG4gICAgICAgICAgICB9XG4gICAgICAgIH07XG4gICAgfVxufTtcbmNvbnN0IGxlZ2FjeVByb3BlcnR5ID0gKG9wdGlvbnMsIHByb3RvLCBuYW1lKSA9PiB7XG4gICAgcHJvdG8uY29uc3RydWN0b3JcbiAgICAgICAgLmNyZWF0ZVByb3BlcnR5KG5hbWUsIG9wdGlvbnMpO1xufTtcbi8qKlxuICogQSBwcm9wZXJ0eSBkZWNvcmF0b3Igd2hpY2ggY3JlYXRlcyBhIExpdEVsZW1lbnQgcHJvcGVydHkgd2hpY2ggcmVmbGVjdHMgYVxuICogY29ycmVzcG9uZGluZyBhdHRyaWJ1dGUgdmFsdWUuIEEgYFByb3BlcnR5RGVjbGFyYXRpb25gIG1heSBvcHRpb25hbGx5IGJlXG4gKiBzdXBwbGllZCB0byBjb25maWd1cmUgcHJvcGVydHkgZmVhdHVyZXMuXG4gKlxuICogQEV4cG9ydERlY29yYXRlZEl0ZW1zXG4gKi9cbmV4cG9ydCBmdW5jdGlvbiBwcm9wZXJ0eShvcHRpb25zKSB7XG4gICAgLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOm5vLWFueSBkZWNvcmF0b3JcbiAgICByZXR1cm4gKHByb3RvT3JEZXNjcmlwdG9yLCBuYW1lKSA9PiAobmFtZSAhPT0gdW5kZWZpbmVkKSA/XG4gICAgICAgIGxlZ2FjeVByb3BlcnR5KG9wdGlvbnMsIHByb3RvT3JEZXNjcmlwdG9yLCBuYW1lKSA6XG4gICAgICAgIHN0YW5kYXJkUHJvcGVydHkob3B0aW9ucywgcHJvdG9PckRlc2NyaXB0b3IpO1xufVxuLyoqXG4gKiBBIHByb3BlcnR5IGRlY29yYXRvciB0aGF0IGNvbnZlcnRzIGEgY2xhc3MgcHJvcGVydHkgaW50byBhIGdldHRlciB0aGF0XG4gKiBleGVjdXRlcyBhIHF1ZXJ5U2VsZWN0b3Igb24gdGhlIGVsZW1lbnQncyByZW5kZXJSb290LlxuICovXG5leHBvcnQgY29uc3QgcXVlcnkgPSBfcXVlcnkoKHRhcmdldCwgc2VsZWN0b3IpID0+IHRhcmdldC5xdWVyeVNlbGVjdG9yKHNlbGVjdG9yKSk7XG4vKipcbiAqIEEgcHJvcGVydHkgZGVjb3JhdG9yIHRoYXQgY29udmVydHMgYSBjbGFzcyBwcm9wZXJ0eSBpbnRvIGEgZ2V0dGVyXG4gKiB0aGF0IGV4ZWN1dGVzIGEgcXVlcnlTZWxlY3RvckFsbCBvbiB0aGUgZWxlbWVudCdzIHJlbmRlclJvb3QuXG4gKi9cbmV4cG9ydCBjb25zdCBxdWVyeUFsbCA9IF9xdWVyeSgodGFyZ2V0LCBzZWxlY3RvcikgPT4gdGFyZ2V0LnF1ZXJ5U2VsZWN0b3JBbGwoc2VsZWN0b3IpKTtcbmNvbnN0IGxlZ2FjeVF1ZXJ5ID0gKGRlc2NyaXB0b3IsIHByb3RvLCBuYW1lKSA9PiB7XG4gICAgT2JqZWN0LmRlZmluZVByb3BlcnR5KHByb3RvLCBuYW1lLCBkZXNjcmlwdG9yKTtcbn07XG5jb25zdCBzdGFuZGFyZFF1ZXJ5ID0gKGRlc2NyaXB0b3IsIGVsZW1lbnQpID0+ICh7XG4gICAga2luZDogJ21ldGhvZCcsXG4gICAgcGxhY2VtZW50OiAncHJvdG90eXBlJyxcbiAgICBrZXk6IGVsZW1lbnQua2V5LFxuICAgIGRlc2NyaXB0b3IsXG59KTtcbi8qKlxuICogQmFzZS1pbXBsZW1lbnRhdGlvbiBvZiBgQHF1ZXJ5YCBhbmQgYEBxdWVyeUFsbGAgZGVjb3JhdG9ycy5cbiAqXG4gKiBAcGFyYW0gcXVlcnlGbiBleGVjdHV0ZSBhIGBzZWxlY3RvcmAgKGllLCBxdWVyeVNlbGVjdG9yIG9yIHF1ZXJ5U2VsZWN0b3JBbGwpXG4gKiBhZ2FpbnN0IGB0YXJnZXRgLlxuICogQHN1cHByZXNzIHt2aXNpYmlsaXR5fSBUaGUgZGVzY3JpcHRvciBhY2Nlc3NlcyBhbiBpbnRlcm5hbCBmaWVsZCBvbiB0aGVcbiAqIGVsZW1lbnQuXG4gKi9cbmZ1bmN0aW9uIF9xdWVyeShxdWVyeUZuKSB7XG4gICAgcmV0dXJuIChzZWxlY3RvcikgPT4gKHByb3RvT3JEZXNjcmlwdG9yLCBcbiAgICAvLyB0c2xpbnQ6ZGlzYWJsZS1uZXh0LWxpbmU6bm8tYW55IGRlY29yYXRvclxuICAgIG5hbWUpID0+IHtcbiAgICAgICAgY29uc3QgZGVzY3JpcHRvciA9IHtcbiAgICAgICAgICAgIGdldCgpIHtcbiAgICAgICAgICAgICAgICByZXR1cm4gcXVlcnlGbih0aGlzLnJlbmRlclJvb3QsIHNlbGVjdG9yKTtcbiAgICAgICAgICAgIH0sXG4gICAgICAgICAgICBlbnVtZXJhYmxlOiB0cnVlLFxuICAgICAgICAgICAgY29uZmlndXJhYmxlOiB0cnVlLFxuICAgICAgICB9O1xuICAgICAgICByZXR1cm4gKG5hbWUgIT09IHVuZGVmaW5lZCkgP1xuICAgICAgICAgICAgbGVnYWN5UXVlcnkoZGVzY3JpcHRvciwgcHJvdG9PckRlc2NyaXB0b3IsIG5hbWUpIDpcbiAgICAgICAgICAgIHN0YW5kYXJkUXVlcnkoZGVzY3JpcHRvciwgcHJvdG9PckRlc2NyaXB0b3IpO1xuICAgIH07XG59XG5jb25zdCBzdGFuZGFyZEV2ZW50T3B0aW9ucyA9IChvcHRpb25zLCBlbGVtZW50KSA9PiB7XG4gICAgcmV0dXJuIE9iamVjdC5hc3NpZ24oe30sIGVsZW1lbnQsIHsgZmluaXNoZXIoY2xhenopIHtcbiAgICAgICAgICAgIE9iamVjdC5hc3NpZ24oY2xhenoucHJvdG90eXBlW2VsZW1lbnQua2V5XSwgb3B0aW9ucyk7XG4gICAgICAgIH0gfSk7XG59O1xuY29uc3QgbGVnYWN5RXZlbnRPcHRpb25zID0gXG4vLyB0c2xpbnQ6ZGlzYWJsZS1uZXh0LWxpbmU6bm8tYW55IGxlZ2FjeSBkZWNvcmF0b3JcbihvcHRpb25zLCBwcm90bywgbmFtZSkgPT4ge1xuICAgIE9iamVjdC5hc3NpZ24ocHJvdG9bbmFtZV0sIG9wdGlvbnMpO1xufTtcbi8qKlxuICogQWRkcyBldmVudCBsaXN0ZW5lciBvcHRpb25zIHRvIGEgbWV0aG9kIHVzZWQgYXMgYW4gZXZlbnQgbGlzdGVuZXIgaW4gYVxuICogbGl0LWh0bWwgdGVtcGxhdGUuXG4gKlxuICogQHBhcmFtIG9wdGlvbnMgQW4gb2JqZWN0IHRoYXQgc3BlY2lmaXMgZXZlbnQgbGlzdGVuZXIgb3B0aW9ucyBhcyBhY2NlcHRlZCBieVxuICogYEV2ZW50VGFyZ2V0I2FkZEV2ZW50TGlzdGVuZXJgIGFuZCBgRXZlbnRUYXJnZXQjcmVtb3ZlRXZlbnRMaXN0ZW5lcmAuXG4gKlxuICogQ3VycmVudCBicm93c2VycyBzdXBwb3J0IHRoZSBgY2FwdHVyZWAsIGBwYXNzaXZlYCwgYW5kIGBvbmNlYCBvcHRpb25zLiBTZWU6XG4gKiBodHRwczovL2RldmVsb3Blci5tb3ppbGxhLm9yZy9lbi1VUy9kb2NzL1dlYi9BUEkvRXZlbnRUYXJnZXQvYWRkRXZlbnRMaXN0ZW5lciNQYXJhbWV0ZXJzXG4gKlxuICogQGV4YW1wbGVcbiAqXG4gKiAgICAgY2xhc3MgTXlFbGVtZW50IHtcbiAqXG4gKiAgICAgICBjbGlja2VkID0gZmFsc2U7XG4gKlxuICogICAgICAgcmVuZGVyKCkge1xuICogICAgICAgICByZXR1cm4gaHRtbGA8ZGl2IEBjbGljaz0ke3RoaXMuX29uQ2xpY2t9YD48YnV0dG9uPjwvYnV0dG9uPjwvZGl2PmA7XG4gKiAgICAgICB9XG4gKlxuICogICAgICAgQGV2ZW50T3B0aW9ucyh7Y2FwdHVyZTogdHJ1ZX0pXG4gKiAgICAgICBfb25DbGljayhlKSB7XG4gKiAgICAgICAgIHRoaXMuY2xpY2tlZCA9IHRydWU7XG4gKiAgICAgICB9XG4gKiAgICAgfVxuICovXG5leHBvcnQgY29uc3QgZXZlbnRPcHRpb25zID0gKG9wdGlvbnMpID0+IFxuLy8gUmV0dXJuIHZhbHVlIHR5cGVkIGFzIGFueSB0byBwcmV2ZW50IFR5cGVTY3JpcHQgZnJvbSBjb21wbGFpbmluZyB0aGF0XG4vLyBzdGFuZGFyZCBkZWNvcmF0b3IgZnVuY3Rpb24gc2lnbmF0dXJlIGRvZXMgbm90IG1hdGNoIFR5cGVTY3JpcHQgZGVjb3JhdG9yXG4vLyBzaWduYXR1cmVcbi8vIFRPRE8oa3NjaGFhZik6IHVuY2xlYXIgd2h5IGl0IHdhcyBvbmx5IGZhaWxpbmcgb24gdGhpcyBkZWNvcmF0b3IgYW5kIG5vdFxuLy8gdGhlIG90aGVyc1xuKChwcm90b09yRGVzY3JpcHRvciwgbmFtZSkgPT4gKG5hbWUgIT09IHVuZGVmaW5lZCkgP1xuICAgIGxlZ2FjeUV2ZW50T3B0aW9ucyhvcHRpb25zLCBwcm90b09yRGVzY3JpcHRvciwgbmFtZSkgOlxuICAgIHN0YW5kYXJkRXZlbnRPcHRpb25zKG9wdGlvbnMsIHByb3RvT3JEZXNjcmlwdG9yKSk7XG4vLyMgc291cmNlTWFwcGluZ1VSTD1kZWNvcmF0b3JzLmpzLm1hcCIsIi8qKlxuICogQGxpY2Vuc2VcbiAqIENvcHlyaWdodCAoYykgMjAxNyBUaGUgUG9seW1lciBQcm9qZWN0IEF1dGhvcnMuIEFsbCByaWdodHMgcmVzZXJ2ZWQuXG4gKiBUaGlzIGNvZGUgbWF5IG9ubHkgYmUgdXNlZCB1bmRlciB0aGUgQlNEIHN0eWxlIGxpY2Vuc2UgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9MSUNFTlNFLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBhdXRob3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0FVVEhPUlMudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGNvbnRyaWJ1dG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9DT05UUklCVVRPUlMudHh0XG4gKiBDb2RlIGRpc3RyaWJ1dGVkIGJ5IEdvb2dsZSBhcyBwYXJ0IG9mIHRoZSBwb2x5bWVyIHByb2plY3QgaXMgYWxzb1xuICogc3ViamVjdCB0byBhbiBhZGRpdGlvbmFsIElQIHJpZ2h0cyBncmFudCBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL1BBVEVOVFMudHh0XG4gKi9cbi8qKlxuICogV2hlbiB1c2luZyBDbG9zdXJlIENvbXBpbGVyLCBKU0NvbXBpbGVyX3JlbmFtZVByb3BlcnR5KHByb3BlcnR5LCBvYmplY3QpIGlzXG4gKiByZXBsYWNlZCBhdCBjb21waWxlIHRpbWUgYnkgdGhlIG11bmdlZCBuYW1lIGZvciBvYmplY3RbcHJvcGVydHldLiBXZSBjYW5ub3RcbiAqIGFsaWFzIHRoaXMgZnVuY3Rpb24sIHNvIHdlIGhhdmUgdG8gdXNlIGEgc21hbGwgc2hpbSB0aGF0IGhhcyB0aGUgc2FtZVxuICogYmVoYXZpb3Igd2hlbiBub3QgY29tcGlsaW5nLlxuICovXG53aW5kb3cuSlNDb21waWxlcl9yZW5hbWVQcm9wZXJ0eSA9XG4gICAgKHByb3AsIF9vYmopID0+IHByb3A7XG5leHBvcnQgY29uc3QgZGVmYXVsdENvbnZlcnRlciA9IHtcbiAgICB0b0F0dHJpYnV0ZSh2YWx1ZSwgdHlwZSkge1xuICAgICAgICBzd2l0Y2ggKHR5cGUpIHtcbiAgICAgICAgICAgIGNhc2UgQm9vbGVhbjpcbiAgICAgICAgICAgICAgICByZXR1cm4gdmFsdWUgPyAnJyA6IG51bGw7XG4gICAgICAgICAgICBjYXNlIE9iamVjdDpcbiAgICAgICAgICAgIGNhc2UgQXJyYXk6XG4gICAgICAgICAgICAgICAgLy8gaWYgdGhlIHZhbHVlIGlzIGBudWxsYCBvciBgdW5kZWZpbmVkYCBwYXNzIHRoaXMgdGhyb3VnaFxuICAgICAgICAgICAgICAgIC8vIHRvIGFsbG93IHJlbW92aW5nL25vIGNoYW5nZSBiZWhhdmlvci5cbiAgICAgICAgICAgICAgICByZXR1cm4gdmFsdWUgPT0gbnVsbCA/IHZhbHVlIDogSlNPTi5zdHJpbmdpZnkodmFsdWUpO1xuICAgICAgICB9XG4gICAgICAgIHJldHVybiB2YWx1ZTtcbiAgICB9LFxuICAgIGZyb21BdHRyaWJ1dGUodmFsdWUsIHR5cGUpIHtcbiAgICAgICAgc3dpdGNoICh0eXBlKSB7XG4gICAgICAgICAgICBjYXNlIEJvb2xlYW46XG4gICAgICAgICAgICAgICAgcmV0dXJuIHZhbHVlICE9PSBudWxsO1xuICAgICAgICAgICAgY2FzZSBOdW1iZXI6XG4gICAgICAgICAgICAgICAgcmV0dXJuIHZhbHVlID09PSBudWxsID8gbnVsbCA6IE51bWJlcih2YWx1ZSk7XG4gICAgICAgICAgICBjYXNlIE9iamVjdDpcbiAgICAgICAgICAgIGNhc2UgQXJyYXk6XG4gICAgICAgICAgICAgICAgcmV0dXJuIEpTT04ucGFyc2UodmFsdWUpO1xuICAgICAgICB9XG4gICAgICAgIHJldHVybiB2YWx1ZTtcbiAgICB9XG59O1xuLyoqXG4gKiBDaGFuZ2UgZnVuY3Rpb24gdGhhdCByZXR1cm5zIHRydWUgaWYgYHZhbHVlYCBpcyBkaWZmZXJlbnQgZnJvbSBgb2xkVmFsdWVgLlxuICogVGhpcyBtZXRob2QgaXMgdXNlZCBhcyB0aGUgZGVmYXVsdCBmb3IgYSBwcm9wZXJ0eSdzIGBoYXNDaGFuZ2VkYCBmdW5jdGlvbi5cbiAqL1xuZXhwb3J0IGNvbnN0IG5vdEVxdWFsID0gKHZhbHVlLCBvbGQpID0+IHtcbiAgICAvLyBUaGlzIGVuc3VyZXMgKG9sZD09TmFOLCB2YWx1ZT09TmFOKSBhbHdheXMgcmV0dXJucyBmYWxzZVxuICAgIHJldHVybiBvbGQgIT09IHZhbHVlICYmIChvbGQgPT09IG9sZCB8fCB2YWx1ZSA9PT0gdmFsdWUpO1xufTtcbmNvbnN0IGRlZmF1bHRQcm9wZXJ0eURlY2xhcmF0aW9uID0ge1xuICAgIGF0dHJpYnV0ZTogdHJ1ZSxcbiAgICB0eXBlOiBTdHJpbmcsXG4gICAgY29udmVydGVyOiBkZWZhdWx0Q29udmVydGVyLFxuICAgIHJlZmxlY3Q6IGZhbHNlLFxuICAgIGhhc0NoYW5nZWQ6IG5vdEVxdWFsXG59O1xuY29uc3QgbWljcm90YXNrUHJvbWlzZSA9IFByb21pc2UucmVzb2x2ZSh0cnVlKTtcbmNvbnN0IFNUQVRFX0hBU19VUERBVEVEID0gMTtcbmNvbnN0IFNUQVRFX1VQREFURV9SRVFVRVNURUQgPSAxIDw8IDI7XG5jb25zdCBTVEFURV9JU19SRUZMRUNUSU5HX1RPX0FUVFJJQlVURSA9IDEgPDwgMztcbmNvbnN0IFNUQVRFX0lTX1JFRkxFQ1RJTkdfVE9fUFJPUEVSVFkgPSAxIDw8IDQ7XG5jb25zdCBTVEFURV9IQVNfQ09OTkVDVEVEID0gMSA8PCA1O1xuLyoqXG4gKiBCYXNlIGVsZW1lbnQgY2xhc3Mgd2hpY2ggbWFuYWdlcyBlbGVtZW50IHByb3BlcnRpZXMgYW5kIGF0dHJpYnV0ZXMuIFdoZW5cbiAqIHByb3BlcnRpZXMgY2hhbmdlLCB0aGUgYHVwZGF0ZWAgbWV0aG9kIGlzIGFzeW5jaHJvbm91c2x5IGNhbGxlZC4gVGhpcyBtZXRob2RcbiAqIHNob3VsZCBiZSBzdXBwbGllZCBieSBzdWJjbGFzc2VycyB0byByZW5kZXIgdXBkYXRlcyBhcyBkZXNpcmVkLlxuICovXG5leHBvcnQgY2xhc3MgVXBkYXRpbmdFbGVtZW50IGV4dGVuZHMgSFRNTEVsZW1lbnQge1xuICAgIGNvbnN0cnVjdG9yKCkge1xuICAgICAgICBzdXBlcigpO1xuICAgICAgICB0aGlzLl91cGRhdGVTdGF0ZSA9IDA7XG4gICAgICAgIHRoaXMuX2luc3RhbmNlUHJvcGVydGllcyA9IHVuZGVmaW5lZDtcbiAgICAgICAgdGhpcy5fdXBkYXRlUHJvbWlzZSA9IG1pY3JvdGFza1Byb21pc2U7XG4gICAgICAgIHRoaXMuX2hhc0Nvbm5lY3RlZFJlc29sdmVyID0gdW5kZWZpbmVkO1xuICAgICAgICAvKipcbiAgICAgICAgICogTWFwIHdpdGgga2V5cyBmb3IgYW55IHByb3BlcnRpZXMgdGhhdCBoYXZlIGNoYW5nZWQgc2luY2UgdGhlIGxhc3RcbiAgICAgICAgICogdXBkYXRlIGN5Y2xlIHdpdGggcHJldmlvdXMgdmFsdWVzLlxuICAgICAgICAgKi9cbiAgICAgICAgdGhpcy5fY2hhbmdlZFByb3BlcnRpZXMgPSBuZXcgTWFwKCk7XG4gICAgICAgIC8qKlxuICAgICAgICAgKiBNYXAgd2l0aCBrZXlzIG9mIHByb3BlcnRpZXMgdGhhdCBzaG91bGQgYmUgcmVmbGVjdGVkIHdoZW4gdXBkYXRlZC5cbiAgICAgICAgICovXG4gICAgICAgIHRoaXMuX3JlZmxlY3RpbmdQcm9wZXJ0aWVzID0gdW5kZWZpbmVkO1xuICAgICAgICB0aGlzLmluaXRpYWxpemUoKTtcbiAgICB9XG4gICAgLyoqXG4gICAgICogUmV0dXJucyBhIGxpc3Qgb2YgYXR0cmlidXRlcyBjb3JyZXNwb25kaW5nIHRvIHRoZSByZWdpc3RlcmVkIHByb3BlcnRpZXMuXG4gICAgICogQG5vY29sbGFwc2VcbiAgICAgKi9cbiAgICBzdGF0aWMgZ2V0IG9ic2VydmVkQXR0cmlidXRlcygpIHtcbiAgICAgICAgLy8gbm90ZTogcGlnZ3kgYmFja2luZyBvbiB0aGlzIHRvIGVuc3VyZSB3ZSdyZSBmaW5hbGl6ZWQuXG4gICAgICAgIHRoaXMuZmluYWxpemUoKTtcbiAgICAgICAgY29uc3QgYXR0cmlidXRlcyA9IFtdO1xuICAgICAgICAvLyBVc2UgZm9yRWFjaCBzbyB0aGlzIHdvcmtzIGV2ZW4gaWYgZm9yL29mIGxvb3BzIGFyZSBjb21waWxlZCB0byBmb3IgbG9vcHNcbiAgICAgICAgLy8gZXhwZWN0aW5nIGFycmF5c1xuICAgICAgICB0aGlzLl9jbGFzc1Byb3BlcnRpZXMuZm9yRWFjaCgodiwgcCkgPT4ge1xuICAgICAgICAgICAgY29uc3QgYXR0ciA9IHRoaXMuX2F0dHJpYnV0ZU5hbWVGb3JQcm9wZXJ0eShwLCB2KTtcbiAgICAgICAgICAgIGlmIChhdHRyICE9PSB1bmRlZmluZWQpIHtcbiAgICAgICAgICAgICAgICB0aGlzLl9hdHRyaWJ1dGVUb1Byb3BlcnR5TWFwLnNldChhdHRyLCBwKTtcbiAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzLnB1c2goYXR0cik7XG4gICAgICAgICAgICB9XG4gICAgICAgIH0pO1xuICAgICAgICByZXR1cm4gYXR0cmlidXRlcztcbiAgICB9XG4gICAgLyoqXG4gICAgICogRW5zdXJlcyB0aGUgcHJpdmF0ZSBgX2NsYXNzUHJvcGVydGllc2AgcHJvcGVydHkgbWV0YWRhdGEgaXMgY3JlYXRlZC5cbiAgICAgKiBJbiBhZGRpdGlvbiB0byBgZmluYWxpemVgIHRoaXMgaXMgYWxzbyBjYWxsZWQgaW4gYGNyZWF0ZVByb3BlcnR5YCB0b1xuICAgICAqIGVuc3VyZSB0aGUgYEBwcm9wZXJ0eWAgZGVjb3JhdG9yIGNhbiBhZGQgcHJvcGVydHkgbWV0YWRhdGEuXG4gICAgICovXG4gICAgLyoqIEBub2NvbGxhcHNlICovXG4gICAgc3RhdGljIF9lbnN1cmVDbGFzc1Byb3BlcnRpZXMoKSB7XG4gICAgICAgIC8vIGVuc3VyZSBwcml2YXRlIHN0b3JhZ2UgZm9yIHByb3BlcnR5IGRlY2xhcmF0aW9ucy5cbiAgICAgICAgaWYgKCF0aGlzLmhhc093blByb3BlcnR5KEpTQ29tcGlsZXJfcmVuYW1lUHJvcGVydHkoJ19jbGFzc1Byb3BlcnRpZXMnLCB0aGlzKSkpIHtcbiAgICAgICAgICAgIHRoaXMuX2NsYXNzUHJvcGVydGllcyA9IG5ldyBNYXAoKTtcbiAgICAgICAgICAgIC8vIE5PVEU6IFdvcmthcm91bmQgSUUxMSBub3Qgc3VwcG9ydGluZyBNYXAgY29uc3RydWN0b3IgYXJndW1lbnQuXG4gICAgICAgICAgICBjb25zdCBzdXBlclByb3BlcnRpZXMgPSBPYmplY3QuZ2V0UHJvdG90eXBlT2YodGhpcykuX2NsYXNzUHJvcGVydGllcztcbiAgICAgICAgICAgIGlmIChzdXBlclByb3BlcnRpZXMgIT09IHVuZGVmaW5lZCkge1xuICAgICAgICAgICAgICAgIHN1cGVyUHJvcGVydGllcy5mb3JFYWNoKCh2LCBrKSA9PiB0aGlzLl9jbGFzc1Byb3BlcnRpZXMuc2V0KGssIHYpKTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgIH1cbiAgICAvKipcbiAgICAgKiBDcmVhdGVzIGEgcHJvcGVydHkgYWNjZXNzb3Igb24gdGhlIGVsZW1lbnQgcHJvdG90eXBlIGlmIG9uZSBkb2VzIG5vdCBleGlzdC5cbiAgICAgKiBUaGUgcHJvcGVydHkgc2V0dGVyIGNhbGxzIHRoZSBwcm9wZXJ0eSdzIGBoYXNDaGFuZ2VkYCBwcm9wZXJ0eSBvcHRpb25cbiAgICAgKiBvciB1c2VzIGEgc3RyaWN0IGlkZW50aXR5IGNoZWNrIHRvIGRldGVybWluZSB3aGV0aGVyIG9yIG5vdCB0byByZXF1ZXN0XG4gICAgICogYW4gdXBkYXRlLlxuICAgICAqIEBub2NvbGxhcHNlXG4gICAgICovXG4gICAgc3RhdGljIGNyZWF0ZVByb3BlcnR5KG5hbWUsIG9wdGlvbnMgPSBkZWZhdWx0UHJvcGVydHlEZWNsYXJhdGlvbikge1xuICAgICAgICAvLyBOb3RlLCBzaW5jZSB0aGlzIGNhbiBiZSBjYWxsZWQgYnkgdGhlIGBAcHJvcGVydHlgIGRlY29yYXRvciB3aGljaFxuICAgICAgICAvLyBpcyBjYWxsZWQgYmVmb3JlIGBmaW5hbGl6ZWAsIHdlIGVuc3VyZSBzdG9yYWdlIGV4aXN0cyBmb3IgcHJvcGVydHlcbiAgICAgICAgLy8gbWV0YWRhdGEuXG4gICAgICAgIHRoaXMuX2Vuc3VyZUNsYXNzUHJvcGVydGllcygpO1xuICAgICAgICB0aGlzLl9jbGFzc1Byb3BlcnRpZXMuc2V0KG5hbWUsIG9wdGlvbnMpO1xuICAgICAgICAvLyBEbyBub3QgZ2VuZXJhdGUgYW4gYWNjZXNzb3IgaWYgdGhlIHByb3RvdHlwZSBhbHJlYWR5IGhhcyBvbmUsIHNpbmNlXG4gICAgICAgIC8vIGl0IHdvdWxkIGJlIGxvc3Qgb3RoZXJ3aXNlIGFuZCB0aGF0IHdvdWxkIG5ldmVyIGJlIHRoZSB1c2VyJ3MgaW50ZW50aW9uO1xuICAgICAgICAvLyBJbnN0ZWFkLCB3ZSBleHBlY3QgdXNlcnMgdG8gY2FsbCBgcmVxdWVzdFVwZGF0ZWAgdGhlbXNlbHZlcyBmcm9tXG4gICAgICAgIC8vIHVzZXItZGVmaW5lZCBhY2Nlc3NvcnMuIE5vdGUgdGhhdCBpZiB0aGUgc3VwZXIgaGFzIGFuIGFjY2Vzc29yIHdlIHdpbGxcbiAgICAgICAgLy8gc3RpbGwgb3ZlcndyaXRlIGl0XG4gICAgICAgIGlmIChvcHRpb25zLm5vQWNjZXNzb3IgfHwgdGhpcy5wcm90b3R5cGUuaGFzT3duUHJvcGVydHkobmFtZSkpIHtcbiAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgfVxuICAgICAgICBjb25zdCBrZXkgPSB0eXBlb2YgbmFtZSA9PT0gJ3N5bWJvbCcgPyBTeW1ib2woKSA6IGBfXyR7bmFtZX1gO1xuICAgICAgICBPYmplY3QuZGVmaW5lUHJvcGVydHkodGhpcy5wcm90b3R5cGUsIG5hbWUsIHtcbiAgICAgICAgICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTpuby1hbnkgbm8gc3ltYm9sIGluIGluZGV4XG4gICAgICAgICAgICBnZXQoKSB7XG4gICAgICAgICAgICAgICAgLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOm5vLWFueSBubyBzeW1ib2wgaW4gaW5kZXhcbiAgICAgICAgICAgICAgICByZXR1cm4gdGhpc1trZXldO1xuICAgICAgICAgICAgfSxcbiAgICAgICAgICAgIHNldCh2YWx1ZSkge1xuICAgICAgICAgICAgICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTpuby1hbnkgbm8gc3ltYm9sIGluIGluZGV4XG4gICAgICAgICAgICAgICAgY29uc3Qgb2xkVmFsdWUgPSB0aGlzW25hbWVdO1xuICAgICAgICAgICAgICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTpuby1hbnkgbm8gc3ltYm9sIGluIGluZGV4XG4gICAgICAgICAgICAgICAgdGhpc1trZXldID0gdmFsdWU7XG4gICAgICAgICAgICAgICAgdGhpcy5yZXF1ZXN0VXBkYXRlKG5hbWUsIG9sZFZhbHVlKTtcbiAgICAgICAgICAgIH0sXG4gICAgICAgICAgICBjb25maWd1cmFibGU6IHRydWUsXG4gICAgICAgICAgICBlbnVtZXJhYmxlOiB0cnVlXG4gICAgICAgIH0pO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBDcmVhdGVzIHByb3BlcnR5IGFjY2Vzc29ycyBmb3IgcmVnaXN0ZXJlZCBwcm9wZXJ0aWVzIGFuZCBlbnN1cmVzXG4gICAgICogYW55IHN1cGVyY2xhc3NlcyBhcmUgYWxzbyBmaW5hbGl6ZWQuXG4gICAgICogQG5vY29sbGFwc2VcbiAgICAgKi9cbiAgICBzdGF0aWMgZmluYWxpemUoKSB7XG4gICAgICAgIGlmICh0aGlzLmhhc093blByb3BlcnR5KEpTQ29tcGlsZXJfcmVuYW1lUHJvcGVydHkoJ2ZpbmFsaXplZCcsIHRoaXMpKSAmJlxuICAgICAgICAgICAgdGhpcy5maW5hbGl6ZWQpIHtcbiAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgfVxuICAgICAgICAvLyBmaW5hbGl6ZSBhbnkgc3VwZXJjbGFzc2VzXG4gICAgICAgIGNvbnN0IHN1cGVyQ3RvciA9IE9iamVjdC5nZXRQcm90b3R5cGVPZih0aGlzKTtcbiAgICAgICAgaWYgKHR5cGVvZiBzdXBlckN0b3IuZmluYWxpemUgPT09ICdmdW5jdGlvbicpIHtcbiAgICAgICAgICAgIHN1cGVyQ3Rvci5maW5hbGl6ZSgpO1xuICAgICAgICB9XG4gICAgICAgIHRoaXMuZmluYWxpemVkID0gdHJ1ZTtcbiAgICAgICAgdGhpcy5fZW5zdXJlQ2xhc3NQcm9wZXJ0aWVzKCk7XG4gICAgICAgIC8vIGluaXRpYWxpemUgTWFwIHBvcHVsYXRlZCBpbiBvYnNlcnZlZEF0dHJpYnV0ZXNcbiAgICAgICAgdGhpcy5fYXR0cmlidXRlVG9Qcm9wZXJ0eU1hcCA9IG5ldyBNYXAoKTtcbiAgICAgICAgLy8gbWFrZSBhbnkgcHJvcGVydGllc1xuICAgICAgICAvLyBOb3RlLCBvbmx5IHByb2Nlc3MgXCJvd25cIiBwcm9wZXJ0aWVzIHNpbmNlIHRoaXMgZWxlbWVudCB3aWxsIGluaGVyaXRcbiAgICAgICAgLy8gYW55IHByb3BlcnRpZXMgZGVmaW5lZCBvbiB0aGUgc3VwZXJDbGFzcywgYW5kIGZpbmFsaXphdGlvbiBlbnN1cmVzXG4gICAgICAgIC8vIHRoZSBlbnRpcmUgcHJvdG90eXBlIGNoYWluIGlzIGZpbmFsaXplZC5cbiAgICAgICAgaWYgKHRoaXMuaGFzT3duUHJvcGVydHkoSlNDb21waWxlcl9yZW5hbWVQcm9wZXJ0eSgncHJvcGVydGllcycsIHRoaXMpKSkge1xuICAgICAgICAgICAgY29uc3QgcHJvcHMgPSB0aGlzLnByb3BlcnRpZXM7XG4gICAgICAgICAgICAvLyBzdXBwb3J0IHN5bWJvbHMgaW4gcHJvcGVydGllcyAoSUUxMSBkb2VzIG5vdCBzdXBwb3J0IHRoaXMpXG4gICAgICAgICAgICBjb25zdCBwcm9wS2V5cyA9IFtcbiAgICAgICAgICAgICAgICAuLi5PYmplY3QuZ2V0T3duUHJvcGVydHlOYW1lcyhwcm9wcyksXG4gICAgICAgICAgICAgICAgLi4uKHR5cGVvZiBPYmplY3QuZ2V0T3duUHJvcGVydHlTeW1ib2xzID09PSAnZnVuY3Rpb24nKSA/XG4gICAgICAgICAgICAgICAgICAgIE9iamVjdC5nZXRPd25Qcm9wZXJ0eVN5bWJvbHMocHJvcHMpIDpcbiAgICAgICAgICAgICAgICAgICAgW11cbiAgICAgICAgICAgIF07XG4gICAgICAgICAgICAvLyBUaGlzIGZvci9vZiBpcyBvayBiZWNhdXNlIHByb3BLZXlzIGlzIGFuIGFycmF5XG4gICAgICAgICAgICBmb3IgKGNvbnN0IHAgb2YgcHJvcEtleXMpIHtcbiAgICAgICAgICAgICAgICAvLyBub3RlLCB1c2Ugb2YgYGFueWAgaXMgZHVlIHRvIFR5cGVTcmlwdCBsYWNrIG9mIHN1cHBvcnQgZm9yIHN5bWJvbCBpblxuICAgICAgICAgICAgICAgIC8vIGluZGV4IHR5cGVzXG4gICAgICAgICAgICAgICAgLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOm5vLWFueSBubyBzeW1ib2wgaW4gaW5kZXhcbiAgICAgICAgICAgICAgICB0aGlzLmNyZWF0ZVByb3BlcnR5KHAsIHByb3BzW3BdKTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgIH1cbiAgICAvKipcbiAgICAgKiBSZXR1cm5zIHRoZSBwcm9wZXJ0eSBuYW1lIGZvciB0aGUgZ2l2ZW4gYXR0cmlidXRlIGBuYW1lYC5cbiAgICAgKiBAbm9jb2xsYXBzZVxuICAgICAqL1xuICAgIHN0YXRpYyBfYXR0cmlidXRlTmFtZUZvclByb3BlcnR5KG5hbWUsIG9wdGlvbnMpIHtcbiAgICAgICAgY29uc3QgYXR0cmlidXRlID0gb3B0aW9ucy5hdHRyaWJ1dGU7XG4gICAgICAgIHJldHVybiBhdHRyaWJ1dGUgPT09IGZhbHNlID9cbiAgICAgICAgICAgIHVuZGVmaW5lZCA6XG4gICAgICAgICAgICAodHlwZW9mIGF0dHJpYnV0ZSA9PT0gJ3N0cmluZycgP1xuICAgICAgICAgICAgICAgIGF0dHJpYnV0ZSA6XG4gICAgICAgICAgICAgICAgKHR5cGVvZiBuYW1lID09PSAnc3RyaW5nJyA/IG5hbWUudG9Mb3dlckNhc2UoKSA6IHVuZGVmaW5lZCkpO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBSZXR1cm5zIHRydWUgaWYgYSBwcm9wZXJ0eSBzaG91bGQgcmVxdWVzdCBhbiB1cGRhdGUuXG4gICAgICogQ2FsbGVkIHdoZW4gYSBwcm9wZXJ0eSB2YWx1ZSBpcyBzZXQgYW5kIHVzZXMgdGhlIGBoYXNDaGFuZ2VkYFxuICAgICAqIG9wdGlvbiBmb3IgdGhlIHByb3BlcnR5IGlmIHByZXNlbnQgb3IgYSBzdHJpY3QgaWRlbnRpdHkgY2hlY2suXG4gICAgICogQG5vY29sbGFwc2VcbiAgICAgKi9cbiAgICBzdGF0aWMgX3ZhbHVlSGFzQ2hhbmdlZCh2YWx1ZSwgb2xkLCBoYXNDaGFuZ2VkID0gbm90RXF1YWwpIHtcbiAgICAgICAgcmV0dXJuIGhhc0NoYW5nZWQodmFsdWUsIG9sZCk7XG4gICAgfVxuICAgIC8qKlxuICAgICAqIFJldHVybnMgdGhlIHByb3BlcnR5IHZhbHVlIGZvciB0aGUgZ2l2ZW4gYXR0cmlidXRlIHZhbHVlLlxuICAgICAqIENhbGxlZCB2aWEgdGhlIGBhdHRyaWJ1dGVDaGFuZ2VkQ2FsbGJhY2tgIGFuZCB1c2VzIHRoZSBwcm9wZXJ0eSdzXG4gICAgICogYGNvbnZlcnRlcmAgb3IgYGNvbnZlcnRlci5mcm9tQXR0cmlidXRlYCBwcm9wZXJ0eSBvcHRpb24uXG4gICAgICogQG5vY29sbGFwc2VcbiAgICAgKi9cbiAgICBzdGF0aWMgX3Byb3BlcnR5VmFsdWVGcm9tQXR0cmlidXRlKHZhbHVlLCBvcHRpb25zKSB7XG4gICAgICAgIGNvbnN0IHR5cGUgPSBvcHRpb25zLnR5cGU7XG4gICAgICAgIGNvbnN0IGNvbnZlcnRlciA9IG9wdGlvbnMuY29udmVydGVyIHx8IGRlZmF1bHRDb252ZXJ0ZXI7XG4gICAgICAgIGNvbnN0IGZyb21BdHRyaWJ1dGUgPSAodHlwZW9mIGNvbnZlcnRlciA9PT0gJ2Z1bmN0aW9uJyA/IGNvbnZlcnRlciA6IGNvbnZlcnRlci5mcm9tQXR0cmlidXRlKTtcbiAgICAgICAgcmV0dXJuIGZyb21BdHRyaWJ1dGUgPyBmcm9tQXR0cmlidXRlKHZhbHVlLCB0eXBlKSA6IHZhbHVlO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBSZXR1cm5zIHRoZSBhdHRyaWJ1dGUgdmFsdWUgZm9yIHRoZSBnaXZlbiBwcm9wZXJ0eSB2YWx1ZS4gSWYgdGhpc1xuICAgICAqIHJldHVybnMgdW5kZWZpbmVkLCB0aGUgcHJvcGVydHkgd2lsbCAqbm90KiBiZSByZWZsZWN0ZWQgdG8gYW4gYXR0cmlidXRlLlxuICAgICAqIElmIHRoaXMgcmV0dXJucyBudWxsLCB0aGUgYXR0cmlidXRlIHdpbGwgYmUgcmVtb3ZlZCwgb3RoZXJ3aXNlIHRoZVxuICAgICAqIGF0dHJpYnV0ZSB3aWxsIGJlIHNldCB0byB0aGUgdmFsdWUuXG4gICAgICogVGhpcyB1c2VzIHRoZSBwcm9wZXJ0eSdzIGByZWZsZWN0YCBhbmQgYHR5cGUudG9BdHRyaWJ1dGVgIHByb3BlcnR5IG9wdGlvbnMuXG4gICAgICogQG5vY29sbGFwc2VcbiAgICAgKi9cbiAgICBzdGF0aWMgX3Byb3BlcnR5VmFsdWVUb0F0dHJpYnV0ZSh2YWx1ZSwgb3B0aW9ucykge1xuICAgICAgICBpZiAob3B0aW9ucy5yZWZsZWN0ID09PSB1bmRlZmluZWQpIHtcbiAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgfVxuICAgICAgICBjb25zdCB0eXBlID0gb3B0aW9ucy50eXBlO1xuICAgICAgICBjb25zdCBjb252ZXJ0ZXIgPSBvcHRpb25zLmNvbnZlcnRlcjtcbiAgICAgICAgY29uc3QgdG9BdHRyaWJ1dGUgPSBjb252ZXJ0ZXIgJiYgY29udmVydGVyLnRvQXR0cmlidXRlIHx8XG4gICAgICAgICAgICBkZWZhdWx0Q29udmVydGVyLnRvQXR0cmlidXRlO1xuICAgICAgICByZXR1cm4gdG9BdHRyaWJ1dGUodmFsdWUsIHR5cGUpO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBQZXJmb3JtcyBlbGVtZW50IGluaXRpYWxpemF0aW9uLiBCeSBkZWZhdWx0IGNhcHR1cmVzIGFueSBwcmUtc2V0IHZhbHVlcyBmb3JcbiAgICAgKiByZWdpc3RlcmVkIHByb3BlcnRpZXMuXG4gICAgICovXG4gICAgaW5pdGlhbGl6ZSgpIHtcbiAgICAgICAgdGhpcy5fc2F2ZUluc3RhbmNlUHJvcGVydGllcygpO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBGaXhlcyBhbnkgcHJvcGVydGllcyBzZXQgb24gdGhlIGluc3RhbmNlIGJlZm9yZSB1cGdyYWRlIHRpbWUuXG4gICAgICogT3RoZXJ3aXNlIHRoZXNlIHdvdWxkIHNoYWRvdyB0aGUgYWNjZXNzb3IgYW5kIGJyZWFrIHRoZXNlIHByb3BlcnRpZXMuXG4gICAgICogVGhlIHByb3BlcnRpZXMgYXJlIHN0b3JlZCBpbiBhIE1hcCB3aGljaCBpcyBwbGF5ZWQgYmFjayBhZnRlciB0aGVcbiAgICAgKiBjb25zdHJ1Y3RvciBydW5zLiBOb3RlLCBvbiB2ZXJ5IG9sZCB2ZXJzaW9ucyBvZiBTYWZhcmkgKDw9OSkgb3IgQ2hyb21lXG4gICAgICogKDw9NDEpLCBwcm9wZXJ0aWVzIGNyZWF0ZWQgZm9yIG5hdGl2ZSBwbGF0Zm9ybSBwcm9wZXJ0aWVzIGxpa2UgKGBpZGAgb3JcbiAgICAgKiBgbmFtZWApIG1heSBub3QgaGF2ZSBkZWZhdWx0IHZhbHVlcyBzZXQgaW4gdGhlIGVsZW1lbnQgY29uc3RydWN0b3IuIE9uXG4gICAgICogdGhlc2UgYnJvd3NlcnMgbmF0aXZlIHByb3BlcnRpZXMgYXBwZWFyIG9uIGluc3RhbmNlcyBhbmQgdGhlcmVmb3JlIHRoZWlyXG4gICAgICogZGVmYXVsdCB2YWx1ZSB3aWxsIG92ZXJ3cml0ZSBhbnkgZWxlbWVudCBkZWZhdWx0IChlLmcuIGlmIHRoZSBlbGVtZW50IHNldHNcbiAgICAgKiB0aGlzLmlkID0gJ2lkJyBpbiB0aGUgY29uc3RydWN0b3IsIHRoZSAnaWQnIHdpbGwgYmVjb21lICcnIHNpbmNlIHRoaXMgaXNcbiAgICAgKiB0aGUgbmF0aXZlIHBsYXRmb3JtIGRlZmF1bHQpLlxuICAgICAqL1xuICAgIF9zYXZlSW5zdGFuY2VQcm9wZXJ0aWVzKCkge1xuICAgICAgICAvLyBVc2UgZm9yRWFjaCBzbyB0aGlzIHdvcmtzIGV2ZW4gaWYgZm9yL29mIGxvb3BzIGFyZSBjb21waWxlZCB0byBmb3IgbG9vcHNcbiAgICAgICAgLy8gZXhwZWN0aW5nIGFycmF5c1xuICAgICAgICB0aGlzLmNvbnN0cnVjdG9yXG4gICAgICAgICAgICAuX2NsYXNzUHJvcGVydGllcy5mb3JFYWNoKChfdiwgcCkgPT4ge1xuICAgICAgICAgICAgaWYgKHRoaXMuaGFzT3duUHJvcGVydHkocCkpIHtcbiAgICAgICAgICAgICAgICBjb25zdCB2YWx1ZSA9IHRoaXNbcF07XG4gICAgICAgICAgICAgICAgZGVsZXRlIHRoaXNbcF07XG4gICAgICAgICAgICAgICAgaWYgKCF0aGlzLl9pbnN0YW5jZVByb3BlcnRpZXMpIHtcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5faW5zdGFuY2VQcm9wZXJ0aWVzID0gbmV3IE1hcCgpO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICB0aGlzLl9pbnN0YW5jZVByb3BlcnRpZXMuc2V0KHAsIHZhbHVlKTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfSk7XG4gICAgfVxuICAgIC8qKlxuICAgICAqIEFwcGxpZXMgcHJldmlvdXNseSBzYXZlZCBpbnN0YW5jZSBwcm9wZXJ0aWVzLlxuICAgICAqL1xuICAgIF9hcHBseUluc3RhbmNlUHJvcGVydGllcygpIHtcbiAgICAgICAgLy8gVXNlIGZvckVhY2ggc28gdGhpcyB3b3JrcyBldmVuIGlmIGZvci9vZiBsb29wcyBhcmUgY29tcGlsZWQgdG8gZm9yIGxvb3BzXG4gICAgICAgIC8vIGV4cGVjdGluZyBhcnJheXNcbiAgICAgICAgLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOm5vLWFueVxuICAgICAgICB0aGlzLl9pbnN0YW5jZVByb3BlcnRpZXMuZm9yRWFjaCgodiwgcCkgPT4gdGhpc1twXSA9IHYpO1xuICAgICAgICB0aGlzLl9pbnN0YW5jZVByb3BlcnRpZXMgPSB1bmRlZmluZWQ7XG4gICAgfVxuICAgIGNvbm5lY3RlZENhbGxiYWNrKCkge1xuICAgICAgICB0aGlzLl91cGRhdGVTdGF0ZSA9IHRoaXMuX3VwZGF0ZVN0YXRlIHwgU1RBVEVfSEFTX0NPTk5FQ1RFRDtcbiAgICAgICAgLy8gRW5zdXJlIGNvbm5lY3Rpb24gdHJpZ2dlcnMgYW4gdXBkYXRlLiBVcGRhdGVzIGNhbm5vdCBjb21wbGV0ZSBiZWZvcmVcbiAgICAgICAgLy8gY29ubmVjdGlvbiBhbmQgaWYgb25lIGlzIHBlbmRpbmcgY29ubmVjdGlvbiB0aGUgYF9oYXNDb25uZWN0aW9uUmVzb2x2ZXJgXG4gICAgICAgIC8vIHdpbGwgZXhpc3QuIElmIHNvLCByZXNvbHZlIGl0IHRvIGNvbXBsZXRlIHRoZSB1cGRhdGUsIG90aGVyd2lzZVxuICAgICAgICAvLyByZXF1ZXN0VXBkYXRlLlxuICAgICAgICBpZiAodGhpcy5faGFzQ29ubmVjdGVkUmVzb2x2ZXIpIHtcbiAgICAgICAgICAgIHRoaXMuX2hhc0Nvbm5lY3RlZFJlc29sdmVyKCk7XG4gICAgICAgICAgICB0aGlzLl9oYXNDb25uZWN0ZWRSZXNvbHZlciA9IHVuZGVmaW5lZDtcbiAgICAgICAgfVxuICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgIHRoaXMucmVxdWVzdFVwZGF0ZSgpO1xuICAgICAgICB9XG4gICAgfVxuICAgIC8qKlxuICAgICAqIEFsbG93cyBmb3IgYHN1cGVyLmRpc2Nvbm5lY3RlZENhbGxiYWNrKClgIGluIGV4dGVuc2lvbnMgd2hpbGVcbiAgICAgKiByZXNlcnZpbmcgdGhlIHBvc3NpYmlsaXR5IG9mIG1ha2luZyBub24tYnJlYWtpbmcgZmVhdHVyZSBhZGRpdGlvbnNcbiAgICAgKiB3aGVuIGRpc2Nvbm5lY3RpbmcgYXQgc29tZSBwb2ludCBpbiB0aGUgZnV0dXJlLlxuICAgICAqL1xuICAgIGRpc2Nvbm5lY3RlZENhbGxiYWNrKCkge1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBTeW5jaHJvbml6ZXMgcHJvcGVydHkgdmFsdWVzIHdoZW4gYXR0cmlidXRlcyBjaGFuZ2UuXG4gICAgICovXG4gICAgYXR0cmlidXRlQ2hhbmdlZENhbGxiYWNrKG5hbWUsIG9sZCwgdmFsdWUpIHtcbiAgICAgICAgaWYgKG9sZCAhPT0gdmFsdWUpIHtcbiAgICAgICAgICAgIHRoaXMuX2F0dHJpYnV0ZVRvUHJvcGVydHkobmFtZSwgdmFsdWUpO1xuICAgICAgICB9XG4gICAgfVxuICAgIF9wcm9wZXJ0eVRvQXR0cmlidXRlKG5hbWUsIHZhbHVlLCBvcHRpb25zID0gZGVmYXVsdFByb3BlcnR5RGVjbGFyYXRpb24pIHtcbiAgICAgICAgY29uc3QgY3RvciA9IHRoaXMuY29uc3RydWN0b3I7XG4gICAgICAgIGNvbnN0IGF0dHIgPSBjdG9yLl9hdHRyaWJ1dGVOYW1lRm9yUHJvcGVydHkobmFtZSwgb3B0aW9ucyk7XG4gICAgICAgIGlmIChhdHRyICE9PSB1bmRlZmluZWQpIHtcbiAgICAgICAgICAgIGNvbnN0IGF0dHJWYWx1ZSA9IGN0b3IuX3Byb3BlcnR5VmFsdWVUb0F0dHJpYnV0ZSh2YWx1ZSwgb3B0aW9ucyk7XG4gICAgICAgICAgICAvLyBhbiB1bmRlZmluZWQgdmFsdWUgZG9lcyBub3QgY2hhbmdlIHRoZSBhdHRyaWJ1dGUuXG4gICAgICAgICAgICBpZiAoYXR0clZhbHVlID09PSB1bmRlZmluZWQpIHtcbiAgICAgICAgICAgICAgICByZXR1cm47XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICAvLyBUcmFjayBpZiB0aGUgcHJvcGVydHkgaXMgYmVpbmcgcmVmbGVjdGVkIHRvIGF2b2lkXG4gICAgICAgICAgICAvLyBzZXR0aW5nIHRoZSBwcm9wZXJ0eSBhZ2FpbiB2aWEgYGF0dHJpYnV0ZUNoYW5nZWRDYWxsYmFja2AuIE5vdGU6XG4gICAgICAgICAgICAvLyAxLiB0aGlzIHRha2VzIGFkdmFudGFnZSBvZiB0aGUgZmFjdCB0aGF0IHRoZSBjYWxsYmFjayBpcyBzeW5jaHJvbm91cy5cbiAgICAgICAgICAgIC8vIDIuIHdpbGwgYmVoYXZlIGluY29ycmVjdGx5IGlmIG11bHRpcGxlIGF0dHJpYnV0ZXMgYXJlIGluIHRoZSByZWFjdGlvblxuICAgICAgICAgICAgLy8gc3RhY2sgYXQgdGltZSBvZiBjYWxsaW5nLiBIb3dldmVyLCBzaW5jZSB3ZSBwcm9jZXNzIGF0dHJpYnV0ZXNcbiAgICAgICAgICAgIC8vIGluIGB1cGRhdGVgIHRoaXMgc2hvdWxkIG5vdCBiZSBwb3NzaWJsZSAob3IgYW4gZXh0cmVtZSBjb3JuZXIgY2FzZVxuICAgICAgICAgICAgLy8gdGhhdCB3ZSdkIGxpa2UgdG8gZGlzY292ZXIpLlxuICAgICAgICAgICAgLy8gbWFyayBzdGF0ZSByZWZsZWN0aW5nXG4gICAgICAgICAgICB0aGlzLl91cGRhdGVTdGF0ZSA9IHRoaXMuX3VwZGF0ZVN0YXRlIHwgU1RBVEVfSVNfUkVGTEVDVElOR19UT19BVFRSSUJVVEU7XG4gICAgICAgICAgICBpZiAoYXR0clZhbHVlID09IG51bGwpIHtcbiAgICAgICAgICAgICAgICB0aGlzLnJlbW92ZUF0dHJpYnV0ZShhdHRyKTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgICAgIGVsc2Uge1xuICAgICAgICAgICAgICAgIHRoaXMuc2V0QXR0cmlidXRlKGF0dHIsIGF0dHJWYWx1ZSk7XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICAvLyBtYXJrIHN0YXRlIG5vdCByZWZsZWN0aW5nXG4gICAgICAgICAgICB0aGlzLl91cGRhdGVTdGF0ZSA9IHRoaXMuX3VwZGF0ZVN0YXRlICYgflNUQVRFX0lTX1JFRkxFQ1RJTkdfVE9fQVRUUklCVVRFO1xuICAgICAgICB9XG4gICAgfVxuICAgIF9hdHRyaWJ1dGVUb1Byb3BlcnR5KG5hbWUsIHZhbHVlKSB7XG4gICAgICAgIC8vIFVzZSB0cmFja2luZyBpbmZvIHRvIGF2b2lkIGRlc2VyaWFsaXppbmcgYXR0cmlidXRlIHZhbHVlIGlmIGl0IHdhc1xuICAgICAgICAvLyBqdXN0IHNldCBmcm9tIGEgcHJvcGVydHkgc2V0dGVyLlxuICAgICAgICBpZiAodGhpcy5fdXBkYXRlU3RhdGUgJiBTVEFURV9JU19SRUZMRUNUSU5HX1RPX0FUVFJJQlVURSkge1xuICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICB9XG4gICAgICAgIGNvbnN0IGN0b3IgPSB0aGlzLmNvbnN0cnVjdG9yO1xuICAgICAgICBjb25zdCBwcm9wTmFtZSA9IGN0b3IuX2F0dHJpYnV0ZVRvUHJvcGVydHlNYXAuZ2V0KG5hbWUpO1xuICAgICAgICBpZiAocHJvcE5hbWUgIT09IHVuZGVmaW5lZCkge1xuICAgICAgICAgICAgY29uc3Qgb3B0aW9ucyA9IGN0b3IuX2NsYXNzUHJvcGVydGllcy5nZXQocHJvcE5hbWUpIHx8IGRlZmF1bHRQcm9wZXJ0eURlY2xhcmF0aW9uO1xuICAgICAgICAgICAgLy8gbWFyayBzdGF0ZSByZWZsZWN0aW5nXG4gICAgICAgICAgICB0aGlzLl91cGRhdGVTdGF0ZSA9IHRoaXMuX3VwZGF0ZVN0YXRlIHwgU1RBVEVfSVNfUkVGTEVDVElOR19UT19QUk9QRVJUWTtcbiAgICAgICAgICAgIHRoaXNbcHJvcE5hbWVdID1cbiAgICAgICAgICAgICAgICAvLyB0c2xpbnQ6ZGlzYWJsZS1uZXh0LWxpbmU6bm8tYW55XG4gICAgICAgICAgICAgICAgY3Rvci5fcHJvcGVydHlWYWx1ZUZyb21BdHRyaWJ1dGUodmFsdWUsIG9wdGlvbnMpO1xuICAgICAgICAgICAgLy8gbWFyayBzdGF0ZSBub3QgcmVmbGVjdGluZ1xuICAgICAgICAgICAgdGhpcy5fdXBkYXRlU3RhdGUgPSB0aGlzLl91cGRhdGVTdGF0ZSAmIH5TVEFURV9JU19SRUZMRUNUSU5HX1RPX1BST1BFUlRZO1xuICAgICAgICB9XG4gICAgfVxuICAgIC8qKlxuICAgICAqIFJlcXVlc3RzIGFuIHVwZGF0ZSB3aGljaCBpcyBwcm9jZXNzZWQgYXN5bmNocm9ub3VzbHkuIFRoaXMgc2hvdWxkXG4gICAgICogYmUgY2FsbGVkIHdoZW4gYW4gZWxlbWVudCBzaG91bGQgdXBkYXRlIGJhc2VkIG9uIHNvbWUgc3RhdGUgbm90IHRyaWdnZXJlZFxuICAgICAqIGJ5IHNldHRpbmcgYSBwcm9wZXJ0eS4gSW4gdGhpcyBjYXNlLCBwYXNzIG5vIGFyZ3VtZW50cy4gSXQgc2hvdWxkIGFsc28gYmVcbiAgICAgKiBjYWxsZWQgd2hlbiBtYW51YWxseSBpbXBsZW1lbnRpbmcgYSBwcm9wZXJ0eSBzZXR0ZXIuIEluIHRoaXMgY2FzZSwgcGFzcyB0aGVcbiAgICAgKiBwcm9wZXJ0eSBgbmFtZWAgYW5kIGBvbGRWYWx1ZWAgdG8gZW5zdXJlIHRoYXQgYW55IGNvbmZpZ3VyZWQgcHJvcGVydHlcbiAgICAgKiBvcHRpb25zIGFyZSBob25vcmVkLiBSZXR1cm5zIHRoZSBgdXBkYXRlQ29tcGxldGVgIFByb21pc2Ugd2hpY2ggaXMgcmVzb2x2ZWRcbiAgICAgKiB3aGVuIHRoZSB1cGRhdGUgY29tcGxldGVzLlxuICAgICAqXG4gICAgICogQHBhcmFtIG5hbWUge1Byb3BlcnR5S2V5fSAob3B0aW9uYWwpIG5hbWUgb2YgcmVxdWVzdGluZyBwcm9wZXJ0eVxuICAgICAqIEBwYXJhbSBvbGRWYWx1ZSB7YW55fSAob3B0aW9uYWwpIG9sZCB2YWx1ZSBvZiByZXF1ZXN0aW5nIHByb3BlcnR5XG4gICAgICogQHJldHVybnMge1Byb21pc2V9IEEgUHJvbWlzZSB0aGF0IGlzIHJlc29sdmVkIHdoZW4gdGhlIHVwZGF0ZSBjb21wbGV0ZXMuXG4gICAgICovXG4gICAgcmVxdWVzdFVwZGF0ZShuYW1lLCBvbGRWYWx1ZSkge1xuICAgICAgICBsZXQgc2hvdWxkUmVxdWVzdFVwZGF0ZSA9IHRydWU7XG4gICAgICAgIC8vIGlmIHdlIGhhdmUgYSBwcm9wZXJ0eSBrZXksIHBlcmZvcm0gcHJvcGVydHkgdXBkYXRlIHN0ZXBzLlxuICAgICAgICBpZiAobmFtZSAhPT0gdW5kZWZpbmVkICYmICF0aGlzLl9jaGFuZ2VkUHJvcGVydGllcy5oYXMobmFtZSkpIHtcbiAgICAgICAgICAgIGNvbnN0IGN0b3IgPSB0aGlzLmNvbnN0cnVjdG9yO1xuICAgICAgICAgICAgY29uc3Qgb3B0aW9ucyA9IGN0b3IuX2NsYXNzUHJvcGVydGllcy5nZXQobmFtZSkgfHwgZGVmYXVsdFByb3BlcnR5RGVjbGFyYXRpb247XG4gICAgICAgICAgICBpZiAoY3Rvci5fdmFsdWVIYXNDaGFuZ2VkKHRoaXNbbmFtZV0sIG9sZFZhbHVlLCBvcHRpb25zLmhhc0NoYW5nZWQpKSB7XG4gICAgICAgICAgICAgICAgLy8gdHJhY2sgb2xkIHZhbHVlIHdoZW4gY2hhbmdpbmcuXG4gICAgICAgICAgICAgICAgdGhpcy5fY2hhbmdlZFByb3BlcnRpZXMuc2V0KG5hbWUsIG9sZFZhbHVlKTtcbiAgICAgICAgICAgICAgICAvLyBhZGQgdG8gcmVmbGVjdGluZyBwcm9wZXJ0aWVzIHNldFxuICAgICAgICAgICAgICAgIGlmIChvcHRpb25zLnJlZmxlY3QgPT09IHRydWUgJiZcbiAgICAgICAgICAgICAgICAgICAgISh0aGlzLl91cGRhdGVTdGF0ZSAmIFNUQVRFX0lTX1JFRkxFQ1RJTkdfVE9fUFJPUEVSVFkpKSB7XG4gICAgICAgICAgICAgICAgICAgIGlmICh0aGlzLl9yZWZsZWN0aW5nUHJvcGVydGllcyA9PT0gdW5kZWZpbmVkKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICB0aGlzLl9yZWZsZWN0aW5nUHJvcGVydGllcyA9IG5ldyBNYXAoKTtcbiAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICB0aGlzLl9yZWZsZWN0aW5nUHJvcGVydGllcy5zZXQobmFtZSwgb3B0aW9ucyk7XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIC8vIGFib3J0IHRoZSByZXF1ZXN0IGlmIHRoZSBwcm9wZXJ0eSBzaG91bGQgbm90IGJlIGNvbnNpZGVyZWQgY2hhbmdlZC5cbiAgICAgICAgICAgIH1cbiAgICAgICAgICAgIGVsc2Uge1xuICAgICAgICAgICAgICAgIHNob3VsZFJlcXVlc3RVcGRhdGUgPSBmYWxzZTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgICAgICBpZiAoIXRoaXMuX2hhc1JlcXVlc3RlZFVwZGF0ZSAmJiBzaG91bGRSZXF1ZXN0VXBkYXRlKSB7XG4gICAgICAgICAgICB0aGlzLl9lbnF1ZXVlVXBkYXRlKCk7XG4gICAgICAgIH1cbiAgICAgICAgcmV0dXJuIHRoaXMudXBkYXRlQ29tcGxldGU7XG4gICAgfVxuICAgIC8qKlxuICAgICAqIFNldHMgdXAgdGhlIGVsZW1lbnQgdG8gYXN5bmNocm9ub3VzbHkgdXBkYXRlLlxuICAgICAqL1xuICAgIGFzeW5jIF9lbnF1ZXVlVXBkYXRlKCkge1xuICAgICAgICAvLyBNYXJrIHN0YXRlIHVwZGF0aW5nLi4uXG4gICAgICAgIHRoaXMuX3VwZGF0ZVN0YXRlID0gdGhpcy5fdXBkYXRlU3RhdGUgfCBTVEFURV9VUERBVEVfUkVRVUVTVEVEO1xuICAgICAgICBsZXQgcmVzb2x2ZTtcbiAgICAgICAgY29uc3QgcHJldmlvdXNVcGRhdGVQcm9taXNlID0gdGhpcy5fdXBkYXRlUHJvbWlzZTtcbiAgICAgICAgdGhpcy5fdXBkYXRlUHJvbWlzZSA9IG5ldyBQcm9taXNlKChyZXMpID0+IHJlc29sdmUgPSByZXMpO1xuICAgICAgICAvLyBFbnN1cmUgYW55IHByZXZpb3VzIHVwZGF0ZSBoYXMgcmVzb2x2ZWQgYmVmb3JlIHVwZGF0aW5nLlxuICAgICAgICAvLyBUaGlzIGBhd2FpdGAgYWxzbyBlbnN1cmVzIHRoYXQgcHJvcGVydHkgY2hhbmdlcyBhcmUgYmF0Y2hlZC5cbiAgICAgICAgYXdhaXQgcHJldmlvdXNVcGRhdGVQcm9taXNlO1xuICAgICAgICAvLyBNYWtlIHN1cmUgdGhlIGVsZW1lbnQgaGFzIGNvbm5lY3RlZCBiZWZvcmUgdXBkYXRpbmcuXG4gICAgICAgIGlmICghdGhpcy5faGFzQ29ubmVjdGVkKSB7XG4gICAgICAgICAgICBhd2FpdCBuZXcgUHJvbWlzZSgocmVzKSA9PiB0aGlzLl9oYXNDb25uZWN0ZWRSZXNvbHZlciA9IHJlcyk7XG4gICAgICAgIH1cbiAgICAgICAgLy8gQWxsb3cgYHBlcmZvcm1VcGRhdGVgIHRvIGJlIGFzeW5jaHJvbm91cyB0byBlbmFibGUgc2NoZWR1bGluZyBvZiB1cGRhdGVzLlxuICAgICAgICBjb25zdCByZXN1bHQgPSB0aGlzLnBlcmZvcm1VcGRhdGUoKTtcbiAgICAgICAgLy8gTm90ZSwgdGhpcyBpcyB0byBhdm9pZCBkZWxheWluZyBhbiBhZGRpdGlvbmFsIG1pY3JvdGFzayB1bmxlc3Mgd2UgbmVlZFxuICAgICAgICAvLyB0by5cbiAgICAgICAgaWYgKHJlc3VsdCAhPSBudWxsICYmXG4gICAgICAgICAgICB0eXBlb2YgcmVzdWx0LnRoZW4gPT09ICdmdW5jdGlvbicpIHtcbiAgICAgICAgICAgIGF3YWl0IHJlc3VsdDtcbiAgICAgICAgfVxuICAgICAgICByZXNvbHZlKCF0aGlzLl9oYXNSZXF1ZXN0ZWRVcGRhdGUpO1xuICAgIH1cbiAgICBnZXQgX2hhc0Nvbm5lY3RlZCgpIHtcbiAgICAgICAgcmV0dXJuICh0aGlzLl91cGRhdGVTdGF0ZSAmIFNUQVRFX0hBU19DT05ORUNURUQpO1xuICAgIH1cbiAgICBnZXQgX2hhc1JlcXVlc3RlZFVwZGF0ZSgpIHtcbiAgICAgICAgcmV0dXJuICh0aGlzLl91cGRhdGVTdGF0ZSAmIFNUQVRFX1VQREFURV9SRVFVRVNURUQpO1xuICAgIH1cbiAgICBnZXQgaGFzVXBkYXRlZCgpIHtcbiAgICAgICAgcmV0dXJuICh0aGlzLl91cGRhdGVTdGF0ZSAmIFNUQVRFX0hBU19VUERBVEVEKTtcbiAgICB9XG4gICAgLyoqXG4gICAgICogUGVyZm9ybXMgYW4gZWxlbWVudCB1cGRhdGUuXG4gICAgICpcbiAgICAgKiBZb3UgY2FuIG92ZXJyaWRlIHRoaXMgbWV0aG9kIHRvIGNoYW5nZSB0aGUgdGltaW5nIG9mIHVwZGF0ZXMuIEZvciBpbnN0YW5jZSxcbiAgICAgKiB0byBzY2hlZHVsZSB1cGRhdGVzIHRvIG9jY3VyIGp1c3QgYmVmb3JlIHRoZSBuZXh0IGZyYW1lOlxuICAgICAqXG4gICAgICogYGBgXG4gICAgICogcHJvdGVjdGVkIGFzeW5jIHBlcmZvcm1VcGRhdGUoKTogUHJvbWlzZTx1bmtub3duPiB7XG4gICAgICogICBhd2FpdCBuZXcgUHJvbWlzZSgocmVzb2x2ZSkgPT4gcmVxdWVzdEFuaW1hdGlvbkZyYW1lKCgpID0+IHJlc29sdmUoKSkpO1xuICAgICAqICAgc3VwZXIucGVyZm9ybVVwZGF0ZSgpO1xuICAgICAqIH1cbiAgICAgKiBgYGBcbiAgICAgKi9cbiAgICBwZXJmb3JtVXBkYXRlKCkge1xuICAgICAgICAvLyBNaXhpbiBpbnN0YW5jZSBwcm9wZXJ0aWVzIG9uY2UsIGlmIHRoZXkgZXhpc3QuXG4gICAgICAgIGlmICh0aGlzLl9pbnN0YW5jZVByb3BlcnRpZXMpIHtcbiAgICAgICAgICAgIHRoaXMuX2FwcGx5SW5zdGFuY2VQcm9wZXJ0aWVzKCk7XG4gICAgICAgIH1cbiAgICAgICAgaWYgKHRoaXMuc2hvdWxkVXBkYXRlKHRoaXMuX2NoYW5nZWRQcm9wZXJ0aWVzKSkge1xuICAgICAgICAgICAgY29uc3QgY2hhbmdlZFByb3BlcnRpZXMgPSB0aGlzLl9jaGFuZ2VkUHJvcGVydGllcztcbiAgICAgICAgICAgIHRoaXMudXBkYXRlKGNoYW5nZWRQcm9wZXJ0aWVzKTtcbiAgICAgICAgICAgIHRoaXMuX21hcmtVcGRhdGVkKCk7XG4gICAgICAgICAgICBpZiAoISh0aGlzLl91cGRhdGVTdGF0ZSAmIFNUQVRFX0hBU19VUERBVEVEKSkge1xuICAgICAgICAgICAgICAgIHRoaXMuX3VwZGF0ZVN0YXRlID0gdGhpcy5fdXBkYXRlU3RhdGUgfCBTVEFURV9IQVNfVVBEQVRFRDtcbiAgICAgICAgICAgICAgICB0aGlzLmZpcnN0VXBkYXRlZChjaGFuZ2VkUHJvcGVydGllcyk7XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICB0aGlzLnVwZGF0ZWQoY2hhbmdlZFByb3BlcnRpZXMpO1xuICAgICAgICB9XG4gICAgICAgIGVsc2Uge1xuICAgICAgICAgICAgdGhpcy5fbWFya1VwZGF0ZWQoKTtcbiAgICAgICAgfVxuICAgIH1cbiAgICBfbWFya1VwZGF0ZWQoKSB7XG4gICAgICAgIHRoaXMuX2NoYW5nZWRQcm9wZXJ0aWVzID0gbmV3IE1hcCgpO1xuICAgICAgICB0aGlzLl91cGRhdGVTdGF0ZSA9IHRoaXMuX3VwZGF0ZVN0YXRlICYgflNUQVRFX1VQREFURV9SRVFVRVNURUQ7XG4gICAgfVxuICAgIC8qKlxuICAgICAqIFJldHVybnMgYSBQcm9taXNlIHRoYXQgcmVzb2x2ZXMgd2hlbiB0aGUgZWxlbWVudCBoYXMgY29tcGxldGVkIHVwZGF0aW5nLlxuICAgICAqIFRoZSBQcm9taXNlIHZhbHVlIGlzIGEgYm9vbGVhbiB0aGF0IGlzIGB0cnVlYCBpZiB0aGUgZWxlbWVudCBjb21wbGV0ZWQgdGhlXG4gICAgICogdXBkYXRlIHdpdGhvdXQgdHJpZ2dlcmluZyBhbm90aGVyIHVwZGF0ZS4gVGhlIFByb21pc2UgcmVzdWx0IGlzIGBmYWxzZWAgaWZcbiAgICAgKiBhIHByb3BlcnR5IHdhcyBzZXQgaW5zaWRlIGB1cGRhdGVkKClgLiBUaGlzIGdldHRlciBjYW4gYmUgaW1wbGVtZW50ZWQgdG9cbiAgICAgKiBhd2FpdCBhZGRpdGlvbmFsIHN0YXRlLiBGb3IgZXhhbXBsZSwgaXQgaXMgc29tZXRpbWVzIHVzZWZ1bCB0byBhd2FpdCBhXG4gICAgICogcmVuZGVyZWQgZWxlbWVudCBiZWZvcmUgZnVsZmlsbGluZyB0aGlzIFByb21pc2UuIFRvIGRvIHRoaXMsIGZpcnN0IGF3YWl0XG4gICAgICogYHN1cGVyLnVwZGF0ZUNvbXBsZXRlYCB0aGVuIGFueSBzdWJzZXF1ZW50IHN0YXRlLlxuICAgICAqXG4gICAgICogQHJldHVybnMge1Byb21pc2V9IFRoZSBQcm9taXNlIHJldHVybnMgYSBib29sZWFuIHRoYXQgaW5kaWNhdGVzIGlmIHRoZVxuICAgICAqIHVwZGF0ZSByZXNvbHZlZCB3aXRob3V0IHRyaWdnZXJpbmcgYW5vdGhlciB1cGRhdGUuXG4gICAgICovXG4gICAgZ2V0IHVwZGF0ZUNvbXBsZXRlKCkge1xuICAgICAgICByZXR1cm4gdGhpcy5fdXBkYXRlUHJvbWlzZTtcbiAgICB9XG4gICAgLyoqXG4gICAgICogQ29udHJvbHMgd2hldGhlciBvciBub3QgYHVwZGF0ZWAgc2hvdWxkIGJlIGNhbGxlZCB3aGVuIHRoZSBlbGVtZW50IHJlcXVlc3RzXG4gICAgICogYW4gdXBkYXRlLiBCeSBkZWZhdWx0LCB0aGlzIG1ldGhvZCBhbHdheXMgcmV0dXJucyBgdHJ1ZWAsIGJ1dCB0aGlzIGNhbiBiZVxuICAgICAqIGN1c3RvbWl6ZWQgdG8gY29udHJvbCB3aGVuIHRvIHVwZGF0ZS5cbiAgICAgKlxuICAgICAqICogQHBhcmFtIF9jaGFuZ2VkUHJvcGVydGllcyBNYXAgb2YgY2hhbmdlZCBwcm9wZXJ0aWVzIHdpdGggb2xkIHZhbHVlc1xuICAgICAqL1xuICAgIHNob3VsZFVwZGF0ZShfY2hhbmdlZFByb3BlcnRpZXMpIHtcbiAgICAgICAgcmV0dXJuIHRydWU7XG4gICAgfVxuICAgIC8qKlxuICAgICAqIFVwZGF0ZXMgdGhlIGVsZW1lbnQuIFRoaXMgbWV0aG9kIHJlZmxlY3RzIHByb3BlcnR5IHZhbHVlcyB0byBhdHRyaWJ1dGVzLlxuICAgICAqIEl0IGNhbiBiZSBvdmVycmlkZGVuIHRvIHJlbmRlciBhbmQga2VlcCB1cGRhdGVkIGVsZW1lbnQgRE9NLlxuICAgICAqIFNldHRpbmcgcHJvcGVydGllcyBpbnNpZGUgdGhpcyBtZXRob2Qgd2lsbCAqbm90KiB0cmlnZ2VyXG4gICAgICogYW5vdGhlciB1cGRhdGUuXG4gICAgICpcbiAgICAgKiAqIEBwYXJhbSBfY2hhbmdlZFByb3BlcnRpZXMgTWFwIG9mIGNoYW5nZWQgcHJvcGVydGllcyB3aXRoIG9sZCB2YWx1ZXNcbiAgICAgKi9cbiAgICB1cGRhdGUoX2NoYW5nZWRQcm9wZXJ0aWVzKSB7XG4gICAgICAgIGlmICh0aGlzLl9yZWZsZWN0aW5nUHJvcGVydGllcyAhPT0gdW5kZWZpbmVkICYmXG4gICAgICAgICAgICB0aGlzLl9yZWZsZWN0aW5nUHJvcGVydGllcy5zaXplID4gMCkge1xuICAgICAgICAgICAgLy8gVXNlIGZvckVhY2ggc28gdGhpcyB3b3JrcyBldmVuIGlmIGZvci9vZiBsb29wcyBhcmUgY29tcGlsZWQgdG8gZm9yXG4gICAgICAgICAgICAvLyBsb29wcyBleHBlY3RpbmcgYXJyYXlzXG4gICAgICAgICAgICB0aGlzLl9yZWZsZWN0aW5nUHJvcGVydGllcy5mb3JFYWNoKCh2LCBrKSA9PiB0aGlzLl9wcm9wZXJ0eVRvQXR0cmlidXRlKGssIHRoaXNba10sIHYpKTtcbiAgICAgICAgICAgIHRoaXMuX3JlZmxlY3RpbmdQcm9wZXJ0aWVzID0gdW5kZWZpbmVkO1xuICAgICAgICB9XG4gICAgfVxuICAgIC8qKlxuICAgICAqIEludm9rZWQgd2hlbmV2ZXIgdGhlIGVsZW1lbnQgaXMgdXBkYXRlZC4gSW1wbGVtZW50IHRvIHBlcmZvcm1cbiAgICAgKiBwb3N0LXVwZGF0aW5nIHRhc2tzIHZpYSBET00gQVBJcywgZm9yIGV4YW1wbGUsIGZvY3VzaW5nIGFuIGVsZW1lbnQuXG4gICAgICpcbiAgICAgKiBTZXR0aW5nIHByb3BlcnRpZXMgaW5zaWRlIHRoaXMgbWV0aG9kIHdpbGwgdHJpZ2dlciB0aGUgZWxlbWVudCB0byB1cGRhdGVcbiAgICAgKiBhZ2FpbiBhZnRlciB0aGlzIHVwZGF0ZSBjeWNsZSBjb21wbGV0ZXMuXG4gICAgICpcbiAgICAgKiAqIEBwYXJhbSBfY2hhbmdlZFByb3BlcnRpZXMgTWFwIG9mIGNoYW5nZWQgcHJvcGVydGllcyB3aXRoIG9sZCB2YWx1ZXNcbiAgICAgKi9cbiAgICB1cGRhdGVkKF9jaGFuZ2VkUHJvcGVydGllcykge1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBJbnZva2VkIHdoZW4gdGhlIGVsZW1lbnQgaXMgZmlyc3QgdXBkYXRlZC4gSW1wbGVtZW50IHRvIHBlcmZvcm0gb25lIHRpbWVcbiAgICAgKiB3b3JrIG9uIHRoZSBlbGVtZW50IGFmdGVyIHVwZGF0ZS5cbiAgICAgKlxuICAgICAqIFNldHRpbmcgcHJvcGVydGllcyBpbnNpZGUgdGhpcyBtZXRob2Qgd2lsbCB0cmlnZ2VyIHRoZSBlbGVtZW50IHRvIHVwZGF0ZVxuICAgICAqIGFnYWluIGFmdGVyIHRoaXMgdXBkYXRlIGN5Y2xlIGNvbXBsZXRlcy5cbiAgICAgKlxuICAgICAqICogQHBhcmFtIF9jaGFuZ2VkUHJvcGVydGllcyBNYXAgb2YgY2hhbmdlZCBwcm9wZXJ0aWVzIHdpdGggb2xkIHZhbHVlc1xuICAgICAqL1xuICAgIGZpcnN0VXBkYXRlZChfY2hhbmdlZFByb3BlcnRpZXMpIHtcbiAgICB9XG59XG4vKipcbiAqIE1hcmtzIGNsYXNzIGFzIGhhdmluZyBmaW5pc2hlZCBjcmVhdGluZyBwcm9wZXJ0aWVzLlxuICovXG5VcGRhdGluZ0VsZW1lbnQuZmluYWxpemVkID0gdHJ1ZTtcbi8vIyBzb3VyY2VNYXBwaW5nVVJMPXVwZGF0aW5nLWVsZW1lbnQuanMubWFwIiwiLyoqXG4gKiBAbGljZW5zZVxuICogQ29weXJpZ2h0IChjKSAyMDE3IFRoZSBQb2x5bWVyIFByb2plY3QgQXV0aG9ycy4gQWxsIHJpZ2h0cyByZXNlcnZlZC5cbiAqIFRoaXMgY29kZSBtYXkgb25seSBiZSB1c2VkIHVuZGVyIHRoZSBCU0Qgc3R5bGUgbGljZW5zZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0xJQ0VOU0UudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGF1dGhvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQVVUSE9SUy50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgY29udHJpYnV0b3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0NPTlRSSUJVVE9SUy50eHRcbiAqIENvZGUgZGlzdHJpYnV0ZWQgYnkgR29vZ2xlIGFzIHBhcnQgb2YgdGhlIHBvbHltZXIgcHJvamVjdCBpcyBhbHNvXG4gKiBzdWJqZWN0IHRvIGFuIGFkZGl0aW9uYWwgSVAgcmlnaHRzIGdyYW50IGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vUEFURU5UUy50eHRcbiAqL1xuaW1wb3J0IHsgVGVtcGxhdGVSZXN1bHQgfSBmcm9tICdsaXQtaHRtbCc7XG5pbXBvcnQgeyByZW5kZXIgfSBmcm9tICdsaXQtaHRtbC9saWIvc2hhZHktcmVuZGVyJztcbmltcG9ydCB7IFVwZGF0aW5nRWxlbWVudCB9IGZyb20gJy4vbGliL3VwZGF0aW5nLWVsZW1lbnQuanMnO1xuZXhwb3J0ICogZnJvbSAnLi9saWIvdXBkYXRpbmctZWxlbWVudC5qcyc7XG5leHBvcnQgKiBmcm9tICcuL2xpYi9kZWNvcmF0b3JzLmpzJztcbmV4cG9ydCB7IGh0bWwsIHN2ZywgVGVtcGxhdGVSZXN1bHQsIFNWR1RlbXBsYXRlUmVzdWx0IH0gZnJvbSAnbGl0LWh0bWwvbGl0LWh0bWwnO1xuaW1wb3J0IHsgc3VwcG9ydHNBZG9wdGluZ1N0eWxlU2hlZXRzIH0gZnJvbSAnLi9saWIvY3NzLXRhZy5qcyc7XG5leHBvcnQgKiBmcm9tICcuL2xpYi9jc3MtdGFnLmpzJztcbi8vIElNUE9SVEFOVDogZG8gbm90IGNoYW5nZSB0aGUgcHJvcGVydHkgbmFtZSBvciB0aGUgYXNzaWdubWVudCBleHByZXNzaW9uLlxuLy8gVGhpcyBsaW5lIHdpbGwgYmUgdXNlZCBpbiByZWdleGVzIHRvIHNlYXJjaCBmb3IgTGl0RWxlbWVudCB1c2FnZS5cbi8vIFRPRE8oanVzdGluZmFnbmFuaSk6IGluamVjdCB2ZXJzaW9uIG51bWJlciBhdCBidWlsZCB0aW1lXG4od2luZG93WydsaXRFbGVtZW50VmVyc2lvbnMnXSB8fCAod2luZG93WydsaXRFbGVtZW50VmVyc2lvbnMnXSA9IFtdKSlcbiAgICAucHVzaCgnMi4wLjEnKTtcbi8qKlxuICogTWluaW1hbCBpbXBsZW1lbnRhdGlvbiBvZiBBcnJheS5wcm90b3R5cGUuZmxhdFxuICogQHBhcmFtIGFyciB0aGUgYXJyYXkgdG8gZmxhdHRlblxuICogQHBhcmFtIHJlc3VsdCB0aGUgYWNjdW1sYXRlZCByZXN1bHRcbiAqL1xuZnVuY3Rpb24gYXJyYXlGbGF0KHN0eWxlcywgcmVzdWx0ID0gW10pIHtcbiAgICBmb3IgKGxldCBpID0gMCwgbGVuZ3RoID0gc3R5bGVzLmxlbmd0aDsgaSA8IGxlbmd0aDsgaSsrKSB7XG4gICAgICAgIGNvbnN0IHZhbHVlID0gc3R5bGVzW2ldO1xuICAgICAgICBpZiAoQXJyYXkuaXNBcnJheSh2YWx1ZSkpIHtcbiAgICAgICAgICAgIGFycmF5RmxhdCh2YWx1ZSwgcmVzdWx0KTtcbiAgICAgICAgfVxuICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgIHJlc3VsdC5wdXNoKHZhbHVlKTtcbiAgICAgICAgfVxuICAgIH1cbiAgICByZXR1cm4gcmVzdWx0O1xufVxuLyoqIERlZXBseSBmbGF0dGVucyBzdHlsZXMgYXJyYXkuIFVzZXMgbmF0aXZlIGZsYXQgaWYgYXZhaWxhYmxlLiAqL1xuY29uc3QgZmxhdHRlblN0eWxlcyA9IChzdHlsZXMpID0+IHN0eWxlcy5mbGF0ID8gc3R5bGVzLmZsYXQoSW5maW5pdHkpIDogYXJyYXlGbGF0KHN0eWxlcyk7XG5leHBvcnQgY2xhc3MgTGl0RWxlbWVudCBleHRlbmRzIFVwZGF0aW5nRWxlbWVudCB7XG4gICAgLyoqIEBub2NvbGxhcHNlICovXG4gICAgc3RhdGljIGZpbmFsaXplKCkge1xuICAgICAgICBzdXBlci5maW5hbGl6ZSgpO1xuICAgICAgICAvLyBQcmVwYXJlIHN0eWxpbmcgdGhhdCBpcyBzdGFtcGVkIGF0IGZpcnN0IHJlbmRlciB0aW1lLiBTdHlsaW5nXG4gICAgICAgIC8vIGlzIGJ1aWx0IGZyb20gdXNlciBwcm92aWRlZCBgc3R5bGVzYCBvciBpcyBpbmhlcml0ZWQgZnJvbSB0aGUgc3VwZXJjbGFzcy5cbiAgICAgICAgdGhpcy5fc3R5bGVzID1cbiAgICAgICAgICAgIHRoaXMuaGFzT3duUHJvcGVydHkoSlNDb21waWxlcl9yZW5hbWVQcm9wZXJ0eSgnc3R5bGVzJywgdGhpcykpID9cbiAgICAgICAgICAgICAgICB0aGlzLl9nZXRVbmlxdWVTdHlsZXMoKSA6XG4gICAgICAgICAgICAgICAgdGhpcy5fc3R5bGVzIHx8IFtdO1xuICAgIH1cbiAgICAvKiogQG5vY29sbGFwc2UgKi9cbiAgICBzdGF0aWMgX2dldFVuaXF1ZVN0eWxlcygpIHtcbiAgICAgICAgLy8gVGFrZSBjYXJlIG5vdCB0byBjYWxsIGB0aGlzLnN0eWxlc2AgbXVsdGlwbGUgdGltZXMgc2luY2UgdGhpcyBnZW5lcmF0ZXNcbiAgICAgICAgLy8gbmV3IENTU1Jlc3VsdHMgZWFjaCB0aW1lLlxuICAgICAgICAvLyBUT0RPKHNvcnZlbGwpOiBTaW5jZSB3ZSBkbyBub3QgY2FjaGUgQ1NTUmVzdWx0cyBieSBpbnB1dCwgYW55XG4gICAgICAgIC8vIHNoYXJlZCBzdHlsZXMgd2lsbCBnZW5lcmF0ZSBuZXcgc3R5bGVzaGVldCBvYmplY3RzLCB3aGljaCBpcyB3YXN0ZWZ1bC5cbiAgICAgICAgLy8gVGhpcyBzaG91bGQgYmUgYWRkcmVzc2VkIHdoZW4gYSBicm93c2VyIHNoaXBzIGNvbnN0cnVjdGFibGVcbiAgICAgICAgLy8gc3R5bGVzaGVldHMuXG4gICAgICAgIGNvbnN0IHVzZXJTdHlsZXMgPSB0aGlzLnN0eWxlcztcbiAgICAgICAgY29uc3Qgc3R5bGVzID0gW107XG4gICAgICAgIGlmIChBcnJheS5pc0FycmF5KHVzZXJTdHlsZXMpKSB7XG4gICAgICAgICAgICBjb25zdCBmbGF0U3R5bGVzID0gZmxhdHRlblN0eWxlcyh1c2VyU3R5bGVzKTtcbiAgICAgICAgICAgIC8vIEFzIGEgcGVyZm9ybWFuY2Ugb3B0aW1pemF0aW9uIHRvIGF2b2lkIGR1cGxpY2F0ZWQgc3R5bGluZyB0aGF0IGNhblxuICAgICAgICAgICAgLy8gb2NjdXIgZXNwZWNpYWxseSB3aGVuIGNvbXBvc2luZyB2aWEgc3ViY2xhc3NpbmcsIGRlLWR1cGxpY2F0ZSBzdHlsZXNcbiAgICAgICAgICAgIC8vIHByZXNlcnZpbmcgdGhlIGxhc3QgaXRlbSBpbiB0aGUgbGlzdC4gVGhlIGxhc3QgaXRlbSBpcyBrZXB0IHRvXG4gICAgICAgICAgICAvLyB0cnkgdG8gcHJlc2VydmUgY2FzY2FkZSBvcmRlciB3aXRoIHRoZSBhc3N1bXB0aW9uIHRoYXQgaXQncyBtb3N0XG4gICAgICAgICAgICAvLyBpbXBvcnRhbnQgdGhhdCBsYXN0IGFkZGVkIHN0eWxlcyBvdmVycmlkZSBwcmV2aW91cyBzdHlsZXMuXG4gICAgICAgICAgICBjb25zdCBzdHlsZVNldCA9IGZsYXRTdHlsZXMucmVkdWNlUmlnaHQoKHNldCwgcykgPT4ge1xuICAgICAgICAgICAgICAgIHNldC5hZGQocyk7XG4gICAgICAgICAgICAgICAgLy8gb24gSUUgc2V0LmFkZCBkb2VzIG5vdCByZXR1cm4gdGhlIHNldC5cbiAgICAgICAgICAgICAgICByZXR1cm4gc2V0O1xuICAgICAgICAgICAgfSwgbmV3IFNldCgpKTtcbiAgICAgICAgICAgIC8vIEFycmF5LmZyb20gZG9lcyBub3Qgd29yayBvbiBTZXQgaW4gSUVcbiAgICAgICAgICAgIHN0eWxlU2V0LmZvckVhY2goKHYpID0+IHN0eWxlcy51bnNoaWZ0KHYpKTtcbiAgICAgICAgfVxuICAgICAgICBlbHNlIGlmICh1c2VyU3R5bGVzKSB7XG4gICAgICAgICAgICBzdHlsZXMucHVzaCh1c2VyU3R5bGVzKTtcbiAgICAgICAgfVxuICAgICAgICByZXR1cm4gc3R5bGVzO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBQZXJmb3JtcyBlbGVtZW50IGluaXRpYWxpemF0aW9uLiBCeSBkZWZhdWx0IHRoaXMgY2FsbHMgYGNyZWF0ZVJlbmRlclJvb3RgXG4gICAgICogdG8gY3JlYXRlIHRoZSBlbGVtZW50IGByZW5kZXJSb290YCBub2RlIGFuZCBjYXB0dXJlcyBhbnkgcHJlLXNldCB2YWx1ZXMgZm9yXG4gICAgICogcmVnaXN0ZXJlZCBwcm9wZXJ0aWVzLlxuICAgICAqL1xuICAgIGluaXRpYWxpemUoKSB7XG4gICAgICAgIHN1cGVyLmluaXRpYWxpemUoKTtcbiAgICAgICAgdGhpcy5yZW5kZXJSb290ID0gdGhpcy5jcmVhdGVSZW5kZXJSb290KCk7XG4gICAgICAgIC8vIE5vdGUsIGlmIHJlbmRlclJvb3QgaXMgbm90IGEgc2hhZG93Um9vdCwgc3R5bGVzIHdvdWxkL2NvdWxkIGFwcGx5IHRvIHRoZVxuICAgICAgICAvLyBlbGVtZW50J3MgZ2V0Um9vdE5vZGUoKS4gV2hpbGUgdGhpcyBjb3VsZCBiZSBkb25lLCB3ZSdyZSBjaG9vc2luZyBub3QgdG9cbiAgICAgICAgLy8gc3VwcG9ydCB0aGlzIG5vdyBzaW5jZSBpdCB3b3VsZCByZXF1aXJlIGRpZmZlcmVudCBsb2dpYyBhcm91bmQgZGUtZHVwaW5nLlxuICAgICAgICBpZiAod2luZG93LlNoYWRvd1Jvb3QgJiYgdGhpcy5yZW5kZXJSb290IGluc3RhbmNlb2Ygd2luZG93LlNoYWRvd1Jvb3QpIHtcbiAgICAgICAgICAgIHRoaXMuYWRvcHRTdHlsZXMoKTtcbiAgICAgICAgfVxuICAgIH1cbiAgICAvKipcbiAgICAgKiBSZXR1cm5zIHRoZSBub2RlIGludG8gd2hpY2ggdGhlIGVsZW1lbnQgc2hvdWxkIHJlbmRlciBhbmQgYnkgZGVmYXVsdFxuICAgICAqIGNyZWF0ZXMgYW5kIHJldHVybnMgYW4gb3BlbiBzaGFkb3dSb290LiBJbXBsZW1lbnQgdG8gY3VzdG9taXplIHdoZXJlIHRoZVxuICAgICAqIGVsZW1lbnQncyBET00gaXMgcmVuZGVyZWQuIEZvciBleGFtcGxlLCB0byByZW5kZXIgaW50byB0aGUgZWxlbWVudCdzXG4gICAgICogY2hpbGROb2RlcywgcmV0dXJuIGB0aGlzYC5cbiAgICAgKiBAcmV0dXJucyB7RWxlbWVudHxEb2N1bWVudEZyYWdtZW50fSBSZXR1cm5zIGEgbm9kZSBpbnRvIHdoaWNoIHRvIHJlbmRlci5cbiAgICAgKi9cbiAgICBjcmVhdGVSZW5kZXJSb290KCkge1xuICAgICAgICByZXR1cm4gdGhpcy5hdHRhY2hTaGFkb3coeyBtb2RlOiAnb3BlbicgfSk7XG4gICAgfVxuICAgIC8qKlxuICAgICAqIEFwcGxpZXMgc3R5bGluZyB0byB0aGUgZWxlbWVudCBzaGFkb3dSb290IHVzaW5nIHRoZSBgc3RhdGljIGdldCBzdHlsZXNgXG4gICAgICogcHJvcGVydHkuIFN0eWxpbmcgd2lsbCBhcHBseSB1c2luZyBgc2hhZG93Um9vdC5hZG9wdGVkU3R5bGVTaGVldHNgIHdoZXJlXG4gICAgICogYXZhaWxhYmxlIGFuZCB3aWxsIGZhbGxiYWNrIG90aGVyd2lzZS4gV2hlbiBTaGFkb3cgRE9NIGlzIHBvbHlmaWxsZWQsXG4gICAgICogU2hhZHlDU1Mgc2NvcGVzIHN0eWxlcyBhbmQgYWRkcyB0aGVtIHRvIHRoZSBkb2N1bWVudC4gV2hlbiBTaGFkb3cgRE9NXG4gICAgICogaXMgYXZhaWxhYmxlIGJ1dCBgYWRvcHRlZFN0eWxlU2hlZXRzYCBpcyBub3QsIHN0eWxlcyBhcmUgYXBwZW5kZWQgdG8gdGhlXG4gICAgICogZW5kIG9mIHRoZSBgc2hhZG93Um9vdGAgdG8gW21pbWljIHNwZWNcbiAgICAgKiBiZWhhdmlvcl0oaHR0cHM6Ly93aWNnLmdpdGh1Yi5pby9jb25zdHJ1Y3Qtc3R5bGVzaGVldHMvI3VzaW5nLWNvbnN0cnVjdGVkLXN0eWxlc2hlZXRzKS5cbiAgICAgKi9cbiAgICBhZG9wdFN0eWxlcygpIHtcbiAgICAgICAgY29uc3Qgc3R5bGVzID0gdGhpcy5jb25zdHJ1Y3Rvci5fc3R5bGVzO1xuICAgICAgICBpZiAoc3R5bGVzLmxlbmd0aCA9PT0gMCkge1xuICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICB9XG4gICAgICAgIC8vIFRoZXJlIGFyZSB0aHJlZSBzZXBhcmF0ZSBjYXNlcyBoZXJlIGJhc2VkIG9uIFNoYWRvdyBET00gc3VwcG9ydC5cbiAgICAgICAgLy8gKDEpIHNoYWRvd1Jvb3QgcG9seWZpbGxlZDogdXNlIFNoYWR5Q1NTXG4gICAgICAgIC8vICgyKSBzaGFkb3dSb290LmFkb3B0ZWRTdHlsZVNoZWV0cyBhdmFpbGFibGU6IHVzZSBpdC5cbiAgICAgICAgLy8gKDMpIHNoYWRvd1Jvb3QuYWRvcHRlZFN0eWxlU2hlZXRzIHBvbHlmaWxsZWQ6IGFwcGVuZCBzdHlsZXMgYWZ0ZXJcbiAgICAgICAgLy8gcmVuZGVyaW5nXG4gICAgICAgIGlmICh3aW5kb3cuU2hhZHlDU1MgIT09IHVuZGVmaW5lZCAmJiAhd2luZG93LlNoYWR5Q1NTLm5hdGl2ZVNoYWRvdykge1xuICAgICAgICAgICAgd2luZG93LlNoYWR5Q1NTLlNjb3BpbmdTaGltLnByZXBhcmVBZG9wdGVkQ3NzVGV4dChzdHlsZXMubWFwKChzKSA9PiBzLmNzc1RleHQpLCB0aGlzLmxvY2FsTmFtZSk7XG4gICAgICAgIH1cbiAgICAgICAgZWxzZSBpZiAoc3VwcG9ydHNBZG9wdGluZ1N0eWxlU2hlZXRzKSB7XG4gICAgICAgICAgICB0aGlzLnJlbmRlclJvb3QuYWRvcHRlZFN0eWxlU2hlZXRzID1cbiAgICAgICAgICAgICAgICBzdHlsZXMubWFwKChzKSA9PiBzLnN0eWxlU2hlZXQpO1xuICAgICAgICB9XG4gICAgICAgIGVsc2Uge1xuICAgICAgICAgICAgLy8gVGhpcyBtdXN0IGJlIGRvbmUgYWZ0ZXIgcmVuZGVyaW5nIHNvIHRoZSBhY3R1YWwgc3R5bGUgaW5zZXJ0aW9uIGlzIGRvbmVcbiAgICAgICAgICAgIC8vIGluIGB1cGRhdGVgLlxuICAgICAgICAgICAgdGhpcy5fbmVlZHNTaGltQWRvcHRlZFN0eWxlU2hlZXRzID0gdHJ1ZTtcbiAgICAgICAgfVxuICAgIH1cbiAgICBjb25uZWN0ZWRDYWxsYmFjaygpIHtcbiAgICAgICAgc3VwZXIuY29ubmVjdGVkQ2FsbGJhY2soKTtcbiAgICAgICAgLy8gTm90ZSwgZmlyc3QgdXBkYXRlL3JlbmRlciBoYW5kbGVzIHN0eWxlRWxlbWVudCBzbyB3ZSBvbmx5IGNhbGwgdGhpcyBpZlxuICAgICAgICAvLyBjb25uZWN0ZWQgYWZ0ZXIgZmlyc3QgdXBkYXRlLlxuICAgICAgICBpZiAodGhpcy5oYXNVcGRhdGVkICYmIHdpbmRvdy5TaGFkeUNTUyAhPT0gdW5kZWZpbmVkKSB7XG4gICAgICAgICAgICB3aW5kb3cuU2hhZHlDU1Muc3R5bGVFbGVtZW50KHRoaXMpO1xuICAgICAgICB9XG4gICAgfVxuICAgIC8qKlxuICAgICAqIFVwZGF0ZXMgdGhlIGVsZW1lbnQuIFRoaXMgbWV0aG9kIHJlZmxlY3RzIHByb3BlcnR5IHZhbHVlcyB0byBhdHRyaWJ1dGVzXG4gICAgICogYW5kIGNhbGxzIGByZW5kZXJgIHRvIHJlbmRlciBET00gdmlhIGxpdC1odG1sLiBTZXR0aW5nIHByb3BlcnRpZXMgaW5zaWRlXG4gICAgICogdGhpcyBtZXRob2Qgd2lsbCAqbm90KiB0cmlnZ2VyIGFub3RoZXIgdXBkYXRlLlxuICAgICAqICogQHBhcmFtIF9jaGFuZ2VkUHJvcGVydGllcyBNYXAgb2YgY2hhbmdlZCBwcm9wZXJ0aWVzIHdpdGggb2xkIHZhbHVlc1xuICAgICAqL1xuICAgIHVwZGF0ZShjaGFuZ2VkUHJvcGVydGllcykge1xuICAgICAgICBzdXBlci51cGRhdGUoY2hhbmdlZFByb3BlcnRpZXMpO1xuICAgICAgICBjb25zdCB0ZW1wbGF0ZVJlc3VsdCA9IHRoaXMucmVuZGVyKCk7XG4gICAgICAgIGlmICh0ZW1wbGF0ZVJlc3VsdCBpbnN0YW5jZW9mIFRlbXBsYXRlUmVzdWx0KSB7XG4gICAgICAgICAgICB0aGlzLmNvbnN0cnVjdG9yXG4gICAgICAgICAgICAgICAgLnJlbmRlcih0ZW1wbGF0ZVJlc3VsdCwgdGhpcy5yZW5kZXJSb290LCB7IHNjb3BlTmFtZTogdGhpcy5sb2NhbE5hbWUsIGV2ZW50Q29udGV4dDogdGhpcyB9KTtcbiAgICAgICAgfVxuICAgICAgICAvLyBXaGVuIG5hdGl2ZSBTaGFkb3cgRE9NIGlzIHVzZWQgYnV0IGFkb3B0ZWRTdHlsZXMgYXJlIG5vdCBzdXBwb3J0ZWQsXG4gICAgICAgIC8vIGluc2VydCBzdHlsaW5nIGFmdGVyIHJlbmRlcmluZyB0byBlbnN1cmUgYWRvcHRlZFN0eWxlcyBoYXZlIGhpZ2hlc3RcbiAgICAgICAgLy8gcHJpb3JpdHkuXG4gICAgICAgIGlmICh0aGlzLl9uZWVkc1NoaW1BZG9wdGVkU3R5bGVTaGVldHMpIHtcbiAgICAgICAgICAgIHRoaXMuX25lZWRzU2hpbUFkb3B0ZWRTdHlsZVNoZWV0cyA9IGZhbHNlO1xuICAgICAgICAgICAgdGhpcy5jb25zdHJ1Y3Rvci5fc3R5bGVzLmZvckVhY2goKHMpID0+IHtcbiAgICAgICAgICAgICAgICBjb25zdCBzdHlsZSA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoJ3N0eWxlJyk7XG4gICAgICAgICAgICAgICAgc3R5bGUudGV4dENvbnRlbnQgPSBzLmNzc1RleHQ7XG4gICAgICAgICAgICAgICAgdGhpcy5yZW5kZXJSb290LmFwcGVuZENoaWxkKHN0eWxlKTtcbiAgICAgICAgICAgIH0pO1xuICAgICAgICB9XG4gICAgfVxuICAgIC8qKlxuICAgICAqIEludm9rZWQgb24gZWFjaCB1cGRhdGUgdG8gcGVyZm9ybSByZW5kZXJpbmcgdGFza3MuIFRoaXMgbWV0aG9kIG11c3QgcmV0dXJuXG4gICAgICogYSBsaXQtaHRtbCBUZW1wbGF0ZVJlc3VsdC4gU2V0dGluZyBwcm9wZXJ0aWVzIGluc2lkZSB0aGlzIG1ldGhvZCB3aWxsICpub3QqXG4gICAgICogdHJpZ2dlciB0aGUgZWxlbWVudCB0byB1cGRhdGUuXG4gICAgICovXG4gICAgcmVuZGVyKCkge1xuICAgIH1cbn1cbi8qKlxuICogRW5zdXJlIHRoaXMgY2xhc3MgaXMgbWFya2VkIGFzIGBmaW5hbGl6ZWRgIGFzIGFuIG9wdGltaXphdGlvbiBlbnN1cmluZ1xuICogaXQgd2lsbCBub3QgbmVlZGxlc3NseSB0cnkgdG8gYGZpbmFsaXplYC5cbiAqL1xuTGl0RWxlbWVudC5maW5hbGl6ZWQgPSB0cnVlO1xuLyoqXG4gKiBSZW5kZXIgbWV0aG9kIHVzZWQgdG8gcmVuZGVyIHRoZSBsaXQtaHRtbCBUZW1wbGF0ZVJlc3VsdCB0byB0aGUgZWxlbWVudCdzXG4gKiBET00uXG4gKiBAcGFyYW0ge1RlbXBsYXRlUmVzdWx0fSBUZW1wbGF0ZSB0byByZW5kZXIuXG4gKiBAcGFyYW0ge0VsZW1lbnR8RG9jdW1lbnRGcmFnbWVudH0gTm9kZSBpbnRvIHdoaWNoIHRvIHJlbmRlci5cbiAqIEBwYXJhbSB7U3RyaW5nfSBFbGVtZW50IG5hbWUuXG4gKiBAbm9jb2xsYXBzZVxuICovXG5MaXRFbGVtZW50LnJlbmRlciA9IHJlbmRlcjtcbi8vIyBzb3VyY2VNYXBwaW5nVVJMPWxpdC1lbGVtZW50LmpzLm1hcCIsIi8qKlxuICogQGxpY2Vuc2VcbiAqIENvcHlyaWdodCAoYykgMjAxNyBUaGUgUG9seW1lciBQcm9qZWN0IEF1dGhvcnMuIEFsbCByaWdodHMgcmVzZXJ2ZWQuXG4gKiBUaGlzIGNvZGUgbWF5IG9ubHkgYmUgdXNlZCB1bmRlciB0aGUgQlNEIHN0eWxlIGxpY2Vuc2UgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9MSUNFTlNFLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBhdXRob3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0FVVEhPUlMudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGNvbnRyaWJ1dG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9DT05UUklCVVRPUlMudHh0XG4gKiBDb2RlIGRpc3RyaWJ1dGVkIGJ5IEdvb2dsZSBhcyBwYXJ0IG9mIHRoZSBwb2x5bWVyIHByb2plY3QgaXMgYWxzb1xuICogc3ViamVjdCB0byBhbiBhZGRpdGlvbmFsIElQIHJpZ2h0cyBncmFudCBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL1BBVEVOVFMudHh0XG4gKi9cbmltcG9ydCB7IEF0dHJpYnV0ZUNvbW1pdHRlciwgQm9vbGVhbkF0dHJpYnV0ZVBhcnQsIEV2ZW50UGFydCwgTm9kZVBhcnQsIFByb3BlcnR5Q29tbWl0dGVyIH0gZnJvbSAnLi9wYXJ0cy5qcyc7XG4vKipcbiAqIENyZWF0ZXMgUGFydHMgd2hlbiBhIHRlbXBsYXRlIGlzIGluc3RhbnRpYXRlZC5cbiAqL1xuZXhwb3J0IGNsYXNzIERlZmF1bHRUZW1wbGF0ZVByb2Nlc3NvciB7XG4gICAgLyoqXG4gICAgICogQ3JlYXRlIHBhcnRzIGZvciBhbiBhdHRyaWJ1dGUtcG9zaXRpb24gYmluZGluZywgZ2l2ZW4gdGhlIGV2ZW50LCBhdHRyaWJ1dGVcbiAgICAgKiBuYW1lLCBhbmQgc3RyaW5nIGxpdGVyYWxzLlxuICAgICAqXG4gICAgICogQHBhcmFtIGVsZW1lbnQgVGhlIGVsZW1lbnQgY29udGFpbmluZyB0aGUgYmluZGluZ1xuICAgICAqIEBwYXJhbSBuYW1lICBUaGUgYXR0cmlidXRlIG5hbWVcbiAgICAgKiBAcGFyYW0gc3RyaW5ncyBUaGUgc3RyaW5nIGxpdGVyYWxzLiBUaGVyZSBhcmUgYWx3YXlzIGF0IGxlYXN0IHR3byBzdHJpbmdzLFxuICAgICAqICAgZXZlbnQgZm9yIGZ1bGx5LWNvbnRyb2xsZWQgYmluZGluZ3Mgd2l0aCBhIHNpbmdsZSBleHByZXNzaW9uLlxuICAgICAqL1xuICAgIGhhbmRsZUF0dHJpYnV0ZUV4cHJlc3Npb25zKGVsZW1lbnQsIG5hbWUsIHN0cmluZ3MsIG9wdGlvbnMpIHtcbiAgICAgICAgY29uc3QgcHJlZml4ID0gbmFtZVswXTtcbiAgICAgICAgaWYgKHByZWZpeCA9PT0gJy4nKSB7XG4gICAgICAgICAgICBjb25zdCBjb21pdHRlciA9IG5ldyBQcm9wZXJ0eUNvbW1pdHRlcihlbGVtZW50LCBuYW1lLnNsaWNlKDEpLCBzdHJpbmdzKTtcbiAgICAgICAgICAgIHJldHVybiBjb21pdHRlci5wYXJ0cztcbiAgICAgICAgfVxuICAgICAgICBpZiAocHJlZml4ID09PSAnQCcpIHtcbiAgICAgICAgICAgIHJldHVybiBbbmV3IEV2ZW50UGFydChlbGVtZW50LCBuYW1lLnNsaWNlKDEpLCBvcHRpb25zLmV2ZW50Q29udGV4dCldO1xuICAgICAgICB9XG4gICAgICAgIGlmIChwcmVmaXggPT09ICc/Jykge1xuICAgICAgICAgICAgcmV0dXJuIFtuZXcgQm9vbGVhbkF0dHJpYnV0ZVBhcnQoZWxlbWVudCwgbmFtZS5zbGljZSgxKSwgc3RyaW5ncyldO1xuICAgICAgICB9XG4gICAgICAgIGNvbnN0IGNvbWl0dGVyID0gbmV3IEF0dHJpYnV0ZUNvbW1pdHRlcihlbGVtZW50LCBuYW1lLCBzdHJpbmdzKTtcbiAgICAgICAgcmV0dXJuIGNvbWl0dGVyLnBhcnRzO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBDcmVhdGUgcGFydHMgZm9yIGEgdGV4dC1wb3NpdGlvbiBiaW5kaW5nLlxuICAgICAqIEBwYXJhbSB0ZW1wbGF0ZUZhY3RvcnlcbiAgICAgKi9cbiAgICBoYW5kbGVUZXh0RXhwcmVzc2lvbihvcHRpb25zKSB7XG4gICAgICAgIHJldHVybiBuZXcgTm9kZVBhcnQob3B0aW9ucyk7XG4gICAgfVxufVxuZXhwb3J0IGNvbnN0IGRlZmF1bHRUZW1wbGF0ZVByb2Nlc3NvciA9IG5ldyBEZWZhdWx0VGVtcGxhdGVQcm9jZXNzb3IoKTtcbi8vIyBzb3VyY2VNYXBwaW5nVVJMPWRlZmF1bHQtdGVtcGxhdGUtcHJvY2Vzc29yLmpzLm1hcCIsIi8qKlxuICogQGxpY2Vuc2VcbiAqIENvcHlyaWdodCAoYykgMjAxNyBUaGUgUG9seW1lciBQcm9qZWN0IEF1dGhvcnMuIEFsbCByaWdodHMgcmVzZXJ2ZWQuXG4gKiBUaGlzIGNvZGUgbWF5IG9ubHkgYmUgdXNlZCB1bmRlciB0aGUgQlNEIHN0eWxlIGxpY2Vuc2UgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9MSUNFTlNFLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBhdXRob3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0FVVEhPUlMudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGNvbnRyaWJ1dG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9DT05UUklCVVRPUlMudHh0XG4gKiBDb2RlIGRpc3RyaWJ1dGVkIGJ5IEdvb2dsZSBhcyBwYXJ0IG9mIHRoZSBwb2x5bWVyIHByb2plY3QgaXMgYWxzb1xuICogc3ViamVjdCB0byBhbiBhZGRpdGlvbmFsIElQIHJpZ2h0cyBncmFudCBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL1BBVEVOVFMudHh0XG4gKi9cbmNvbnN0IGRpcmVjdGl2ZXMgPSBuZXcgV2Vha01hcCgpO1xuLyoqXG4gKiBCcmFuZHMgYSBmdW5jdGlvbiBhcyBhIGRpcmVjdGl2ZSBzbyB0aGF0IGxpdC1odG1sIHdpbGwgY2FsbCB0aGUgZnVuY3Rpb25cbiAqIGR1cmluZyB0ZW1wbGF0ZSByZW5kZXJpbmcsIHJhdGhlciB0aGFuIHBhc3NpbmcgYXMgYSB2YWx1ZS5cbiAqXG4gKiBAcGFyYW0gZiBUaGUgZGlyZWN0aXZlIGZhY3RvcnkgZnVuY3Rpb24uIE11c3QgYmUgYSBmdW5jdGlvbiB0aGF0IHJldHVybnMgYVxuICogZnVuY3Rpb24gb2YgdGhlIHNpZ25hdHVyZSBgKHBhcnQ6IFBhcnQpID0+IHZvaWRgLiBUaGUgcmV0dXJuZWQgZnVuY3Rpb24gd2lsbFxuICogYmUgY2FsbGVkIHdpdGggdGhlIHBhcnQgb2JqZWN0XG4gKlxuICogQGV4YW1wbGVcbiAqXG4gKiBgYGBcbiAqIGltcG9ydCB7ZGlyZWN0aXZlLCBodG1sfSBmcm9tICdsaXQtaHRtbCc7XG4gKlxuICogY29uc3QgaW1tdXRhYmxlID0gZGlyZWN0aXZlKCh2KSA9PiAocGFydCkgPT4ge1xuICogICBpZiAocGFydC52YWx1ZSAhPT0gdikge1xuICogICAgIHBhcnQuc2V0VmFsdWUodilcbiAqICAgfVxuICogfSk7XG4gKiBgYGBcbiAqL1xuLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOm5vLWFueVxuZXhwb3J0IGNvbnN0IGRpcmVjdGl2ZSA9IChmKSA9PiAoKC4uLmFyZ3MpID0+IHtcbiAgICBjb25zdCBkID0gZiguLi5hcmdzKTtcbiAgICBkaXJlY3RpdmVzLnNldChkLCB0cnVlKTtcbiAgICByZXR1cm4gZDtcbn0pO1xuZXhwb3J0IGNvbnN0IGlzRGlyZWN0aXZlID0gKG8pID0+IHtcbiAgICByZXR1cm4gdHlwZW9mIG8gPT09ICdmdW5jdGlvbicgJiYgZGlyZWN0aXZlcy5oYXMobyk7XG59O1xuLy8jIHNvdXJjZU1hcHBpbmdVUkw9ZGlyZWN0aXZlLmpzLm1hcCIsIi8qKlxuICogQGxpY2Vuc2VcbiAqIENvcHlyaWdodCAoYykgMjAxNyBUaGUgUG9seW1lciBQcm9qZWN0IEF1dGhvcnMuIEFsbCByaWdodHMgcmVzZXJ2ZWQuXG4gKiBUaGlzIGNvZGUgbWF5IG9ubHkgYmUgdXNlZCB1bmRlciB0aGUgQlNEIHN0eWxlIGxpY2Vuc2UgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9MSUNFTlNFLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBhdXRob3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0FVVEhPUlMudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGNvbnRyaWJ1dG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9DT05UUklCVVRPUlMudHh0XG4gKiBDb2RlIGRpc3RyaWJ1dGVkIGJ5IEdvb2dsZSBhcyBwYXJ0IG9mIHRoZSBwb2x5bWVyIHByb2plY3QgaXMgYWxzb1xuICogc3ViamVjdCB0byBhbiBhZGRpdGlvbmFsIElQIHJpZ2h0cyBncmFudCBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL1BBVEVOVFMudHh0XG4gKi9cbi8qKlxuICogVHJ1ZSBpZiB0aGUgY3VzdG9tIGVsZW1lbnRzIHBvbHlmaWxsIGlzIGluIHVzZS5cbiAqL1xuZXhwb3J0IGNvbnN0IGlzQ0VQb2x5ZmlsbCA9IHdpbmRvdy5jdXN0b21FbGVtZW50cyAhPT0gdW5kZWZpbmVkICYmXG4gICAgd2luZG93LmN1c3RvbUVsZW1lbnRzLnBvbHlmaWxsV3JhcEZsdXNoQ2FsbGJhY2sgIT09XG4gICAgICAgIHVuZGVmaW5lZDtcbi8qKlxuICogUmVwYXJlbnRzIG5vZGVzLCBzdGFydGluZyBmcm9tIGBzdGFydE5vZGVgIChpbmNsdXNpdmUpIHRvIGBlbmROb2RlYFxuICogKGV4Y2x1c2l2ZSksIGludG8gYW5vdGhlciBjb250YWluZXIgKGNvdWxkIGJlIHRoZSBzYW1lIGNvbnRhaW5lciksIGJlZm9yZVxuICogYGJlZm9yZU5vZGVgLiBJZiBgYmVmb3JlTm9kZWAgaXMgbnVsbCwgaXQgYXBwZW5kcyB0aGUgbm9kZXMgdG8gdGhlXG4gKiBjb250YWluZXIuXG4gKi9cbmV4cG9ydCBjb25zdCByZXBhcmVudE5vZGVzID0gKGNvbnRhaW5lciwgc3RhcnQsIGVuZCA9IG51bGwsIGJlZm9yZSA9IG51bGwpID0+IHtcbiAgICBsZXQgbm9kZSA9IHN0YXJ0O1xuICAgIHdoaWxlIChub2RlICE9PSBlbmQpIHtcbiAgICAgICAgY29uc3QgbiA9IG5vZGUubmV4dFNpYmxpbmc7XG4gICAgICAgIGNvbnRhaW5lci5pbnNlcnRCZWZvcmUobm9kZSwgYmVmb3JlKTtcbiAgICAgICAgbm9kZSA9IG47XG4gICAgfVxufTtcbi8qKlxuICogUmVtb3ZlcyBub2Rlcywgc3RhcnRpbmcgZnJvbSBgc3RhcnROb2RlYCAoaW5jbHVzaXZlKSB0byBgZW5kTm9kZWBcbiAqIChleGNsdXNpdmUpLCBmcm9tIGBjb250YWluZXJgLlxuICovXG5leHBvcnQgY29uc3QgcmVtb3ZlTm9kZXMgPSAoY29udGFpbmVyLCBzdGFydE5vZGUsIGVuZE5vZGUgPSBudWxsKSA9PiB7XG4gICAgbGV0IG5vZGUgPSBzdGFydE5vZGU7XG4gICAgd2hpbGUgKG5vZGUgIT09IGVuZE5vZGUpIHtcbiAgICAgICAgY29uc3QgbiA9IG5vZGUubmV4dFNpYmxpbmc7XG4gICAgICAgIGNvbnRhaW5lci5yZW1vdmVDaGlsZChub2RlKTtcbiAgICAgICAgbm9kZSA9IG47XG4gICAgfVxufTtcbi8vIyBzb3VyY2VNYXBwaW5nVVJMPWRvbS5qcy5tYXAiLCIvKipcbiAqIEBsaWNlbnNlXG4gKiBDb3B5cmlnaHQgKGMpIDIwMTcgVGhlIFBvbHltZXIgUHJvamVjdCBBdXRob3JzLiBBbGwgcmlnaHRzIHJlc2VydmVkLlxuICogVGhpcyBjb2RlIG1heSBvbmx5IGJlIHVzZWQgdW5kZXIgdGhlIEJTRCBzdHlsZSBsaWNlbnNlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vTElDRU5TRS50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgYXV0aG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9BVVRIT1JTLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBjb250cmlidXRvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQ09OVFJJQlVUT1JTLnR4dFxuICogQ29kZSBkaXN0cmlidXRlZCBieSBHb29nbGUgYXMgcGFydCBvZiB0aGUgcG9seW1lciBwcm9qZWN0IGlzIGFsc29cbiAqIHN1YmplY3QgdG8gYW4gYWRkaXRpb25hbCBJUCByaWdodHMgZ3JhbnQgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9QQVRFTlRTLnR4dFxuICovXG4vKipcbiAqIEBtb2R1bGUgc2hhZHktcmVuZGVyXG4gKi9cbmltcG9ydCB7IGlzVGVtcGxhdGVQYXJ0QWN0aXZlIH0gZnJvbSAnLi90ZW1wbGF0ZS5qcyc7XG5jb25zdCB3YWxrZXJOb2RlRmlsdGVyID0gMTMzIC8qIE5vZGVGaWx0ZXIuU0hPV197RUxFTUVOVHxDT01NRU5UfFRFWFR9ICovO1xuLyoqXG4gKiBSZW1vdmVzIHRoZSBsaXN0IG9mIG5vZGVzIGZyb20gYSBUZW1wbGF0ZSBzYWZlbHkuIEluIGFkZGl0aW9uIHRvIHJlbW92aW5nXG4gKiBub2RlcyBmcm9tIHRoZSBUZW1wbGF0ZSwgdGhlIFRlbXBsYXRlIHBhcnQgaW5kaWNlcyBhcmUgdXBkYXRlZCB0byBtYXRjaFxuICogdGhlIG11dGF0ZWQgVGVtcGxhdGUgRE9NLlxuICpcbiAqIEFzIHRoZSB0ZW1wbGF0ZSBpcyB3YWxrZWQgdGhlIHJlbW92YWwgc3RhdGUgaXMgdHJhY2tlZCBhbmRcbiAqIHBhcnQgaW5kaWNlcyBhcmUgYWRqdXN0ZWQgYXMgbmVlZGVkLlxuICpcbiAqIGRpdlxuICogICBkaXYjMSAocmVtb3ZlKSA8LS0gc3RhcnQgcmVtb3ZpbmcgKHJlbW92aW5nIG5vZGUgaXMgZGl2IzEpXG4gKiAgICAgZGl2XG4gKiAgICAgICBkaXYjMiAocmVtb3ZlKSAgPC0tIGNvbnRpbnVlIHJlbW92aW5nIChyZW1vdmluZyBub2RlIGlzIHN0aWxsIGRpdiMxKVxuICogICAgICAgICBkaXZcbiAqIGRpdiA8LS0gc3RvcCByZW1vdmluZyBzaW5jZSBwcmV2aW91cyBzaWJsaW5nIGlzIHRoZSByZW1vdmluZyBub2RlIChkaXYjMSxcbiAqIHJlbW92ZWQgNCBub2RlcylcbiAqL1xuZXhwb3J0IGZ1bmN0aW9uIHJlbW92ZU5vZGVzRnJvbVRlbXBsYXRlKHRlbXBsYXRlLCBub2Rlc1RvUmVtb3ZlKSB7XG4gICAgY29uc3QgeyBlbGVtZW50OiB7IGNvbnRlbnQgfSwgcGFydHMgfSA9IHRlbXBsYXRlO1xuICAgIGNvbnN0IHdhbGtlciA9IGRvY3VtZW50LmNyZWF0ZVRyZWVXYWxrZXIoY29udGVudCwgd2Fsa2VyTm9kZUZpbHRlciwgbnVsbCwgZmFsc2UpO1xuICAgIGxldCBwYXJ0SW5kZXggPSBuZXh0QWN0aXZlSW5kZXhJblRlbXBsYXRlUGFydHMocGFydHMpO1xuICAgIGxldCBwYXJ0ID0gcGFydHNbcGFydEluZGV4XTtcbiAgICBsZXQgbm9kZUluZGV4ID0gLTE7XG4gICAgbGV0IHJlbW92ZUNvdW50ID0gMDtcbiAgICBjb25zdCBub2Rlc1RvUmVtb3ZlSW5UZW1wbGF0ZSA9IFtdO1xuICAgIGxldCBjdXJyZW50UmVtb3ZpbmdOb2RlID0gbnVsbDtcbiAgICB3aGlsZSAod2Fsa2VyLm5leHROb2RlKCkpIHtcbiAgICAgICAgbm9kZUluZGV4Kys7XG4gICAgICAgIGNvbnN0IG5vZGUgPSB3YWxrZXIuY3VycmVudE5vZGU7XG4gICAgICAgIC8vIEVuZCByZW1vdmFsIGlmIHN0ZXBwZWQgcGFzdCB0aGUgcmVtb3Zpbmcgbm9kZVxuICAgICAgICBpZiAobm9kZS5wcmV2aW91c1NpYmxpbmcgPT09IGN1cnJlbnRSZW1vdmluZ05vZGUpIHtcbiAgICAgICAgICAgIGN1cnJlbnRSZW1vdmluZ05vZGUgPSBudWxsO1xuICAgICAgICB9XG4gICAgICAgIC8vIEEgbm9kZSB0byByZW1vdmUgd2FzIGZvdW5kIGluIHRoZSB0ZW1wbGF0ZVxuICAgICAgICBpZiAobm9kZXNUb1JlbW92ZS5oYXMobm9kZSkpIHtcbiAgICAgICAgICAgIG5vZGVzVG9SZW1vdmVJblRlbXBsYXRlLnB1c2gobm9kZSk7XG4gICAgICAgICAgICAvLyBUcmFjayBub2RlIHdlJ3JlIHJlbW92aW5nXG4gICAgICAgICAgICBpZiAoY3VycmVudFJlbW92aW5nTm9kZSA9PT0gbnVsbCkge1xuICAgICAgICAgICAgICAgIGN1cnJlbnRSZW1vdmluZ05vZGUgPSBub2RlO1xuICAgICAgICAgICAgfVxuICAgICAgICB9XG4gICAgICAgIC8vIFdoZW4gcmVtb3ZpbmcsIGluY3JlbWVudCBjb3VudCBieSB3aGljaCB0byBhZGp1c3Qgc3Vic2VxdWVudCBwYXJ0IGluZGljZXNcbiAgICAgICAgaWYgKGN1cnJlbnRSZW1vdmluZ05vZGUgIT09IG51bGwpIHtcbiAgICAgICAgICAgIHJlbW92ZUNvdW50Kys7XG4gICAgICAgIH1cbiAgICAgICAgd2hpbGUgKHBhcnQgIT09IHVuZGVmaW5lZCAmJiBwYXJ0LmluZGV4ID09PSBub2RlSW5kZXgpIHtcbiAgICAgICAgICAgIC8vIElmIHBhcnQgaXMgaW4gYSByZW1vdmVkIG5vZGUgZGVhY3RpdmF0ZSBpdCBieSBzZXR0aW5nIGluZGV4IHRvIC0xIG9yXG4gICAgICAgICAgICAvLyBhZGp1c3QgdGhlIGluZGV4IGFzIG5lZWRlZC5cbiAgICAgICAgICAgIHBhcnQuaW5kZXggPSBjdXJyZW50UmVtb3ZpbmdOb2RlICE9PSBudWxsID8gLTEgOiBwYXJ0LmluZGV4IC0gcmVtb3ZlQ291bnQ7XG4gICAgICAgICAgICAvLyBnbyB0byB0aGUgbmV4dCBhY3RpdmUgcGFydC5cbiAgICAgICAgICAgIHBhcnRJbmRleCA9IG5leHRBY3RpdmVJbmRleEluVGVtcGxhdGVQYXJ0cyhwYXJ0cywgcGFydEluZGV4KTtcbiAgICAgICAgICAgIHBhcnQgPSBwYXJ0c1twYXJ0SW5kZXhdO1xuICAgICAgICB9XG4gICAgfVxuICAgIG5vZGVzVG9SZW1vdmVJblRlbXBsYXRlLmZvckVhY2goKG4pID0+IG4ucGFyZW50Tm9kZS5yZW1vdmVDaGlsZChuKSk7XG59XG5jb25zdCBjb3VudE5vZGVzID0gKG5vZGUpID0+IHtcbiAgICBsZXQgY291bnQgPSAobm9kZS5ub2RlVHlwZSA9PT0gMTEgLyogTm9kZS5ET0NVTUVOVF9GUkFHTUVOVF9OT0RFICovKSA/IDAgOiAxO1xuICAgIGNvbnN0IHdhbGtlciA9IGRvY3VtZW50LmNyZWF0ZVRyZWVXYWxrZXIobm9kZSwgd2Fsa2VyTm9kZUZpbHRlciwgbnVsbCwgZmFsc2UpO1xuICAgIHdoaWxlICh3YWxrZXIubmV4dE5vZGUoKSkge1xuICAgICAgICBjb3VudCsrO1xuICAgIH1cbiAgICByZXR1cm4gY291bnQ7XG59O1xuY29uc3QgbmV4dEFjdGl2ZUluZGV4SW5UZW1wbGF0ZVBhcnRzID0gKHBhcnRzLCBzdGFydEluZGV4ID0gLTEpID0+IHtcbiAgICBmb3IgKGxldCBpID0gc3RhcnRJbmRleCArIDE7IGkgPCBwYXJ0cy5sZW5ndGg7IGkrKykge1xuICAgICAgICBjb25zdCBwYXJ0ID0gcGFydHNbaV07XG4gICAgICAgIGlmIChpc1RlbXBsYXRlUGFydEFjdGl2ZShwYXJ0KSkge1xuICAgICAgICAgICAgcmV0dXJuIGk7XG4gICAgICAgIH1cbiAgICB9XG4gICAgcmV0dXJuIC0xO1xufTtcbi8qKlxuICogSW5zZXJ0cyB0aGUgZ2l2ZW4gbm9kZSBpbnRvIHRoZSBUZW1wbGF0ZSwgb3B0aW9uYWxseSBiZWZvcmUgdGhlIGdpdmVuXG4gKiByZWZOb2RlLiBJbiBhZGRpdGlvbiB0byBpbnNlcnRpbmcgdGhlIG5vZGUgaW50byB0aGUgVGVtcGxhdGUsIHRoZSBUZW1wbGF0ZVxuICogcGFydCBpbmRpY2VzIGFyZSB1cGRhdGVkIHRvIG1hdGNoIHRoZSBtdXRhdGVkIFRlbXBsYXRlIERPTS5cbiAqL1xuZXhwb3J0IGZ1bmN0aW9uIGluc2VydE5vZGVJbnRvVGVtcGxhdGUodGVtcGxhdGUsIG5vZGUsIHJlZk5vZGUgPSBudWxsKSB7XG4gICAgY29uc3QgeyBlbGVtZW50OiB7IGNvbnRlbnQgfSwgcGFydHMgfSA9IHRlbXBsYXRlO1xuICAgIC8vIElmIHRoZXJlJ3Mgbm8gcmVmTm9kZSwgdGhlbiBwdXQgbm9kZSBhdCBlbmQgb2YgdGVtcGxhdGUuXG4gICAgLy8gTm8gcGFydCBpbmRpY2VzIG5lZWQgdG8gYmUgc2hpZnRlZCBpbiB0aGlzIGNhc2UuXG4gICAgaWYgKHJlZk5vZGUgPT09IG51bGwgfHwgcmVmTm9kZSA9PT0gdW5kZWZpbmVkKSB7XG4gICAgICAgIGNvbnRlbnQuYXBwZW5kQ2hpbGQobm9kZSk7XG4gICAgICAgIHJldHVybjtcbiAgICB9XG4gICAgY29uc3Qgd2Fsa2VyID0gZG9jdW1lbnQuY3JlYXRlVHJlZVdhbGtlcihjb250ZW50LCB3YWxrZXJOb2RlRmlsdGVyLCBudWxsLCBmYWxzZSk7XG4gICAgbGV0IHBhcnRJbmRleCA9IG5leHRBY3RpdmVJbmRleEluVGVtcGxhdGVQYXJ0cyhwYXJ0cyk7XG4gICAgbGV0IGluc2VydENvdW50ID0gMDtcbiAgICBsZXQgd2Fsa2VySW5kZXggPSAtMTtcbiAgICB3aGlsZSAod2Fsa2VyLm5leHROb2RlKCkpIHtcbiAgICAgICAgd2Fsa2VySW5kZXgrKztcbiAgICAgICAgY29uc3Qgd2Fsa2VyTm9kZSA9IHdhbGtlci5jdXJyZW50Tm9kZTtcbiAgICAgICAgaWYgKHdhbGtlck5vZGUgPT09IHJlZk5vZGUpIHtcbiAgICAgICAgICAgIGluc2VydENvdW50ID0gY291bnROb2Rlcyhub2RlKTtcbiAgICAgICAgICAgIHJlZk5vZGUucGFyZW50Tm9kZS5pbnNlcnRCZWZvcmUobm9kZSwgcmVmTm9kZSk7XG4gICAgICAgIH1cbiAgICAgICAgd2hpbGUgKHBhcnRJbmRleCAhPT0gLTEgJiYgcGFydHNbcGFydEluZGV4XS5pbmRleCA9PT0gd2Fsa2VySW5kZXgpIHtcbiAgICAgICAgICAgIC8vIElmIHdlJ3ZlIGluc2VydGVkIHRoZSBub2RlLCBzaW1wbHkgYWRqdXN0IGFsbCBzdWJzZXF1ZW50IHBhcnRzXG4gICAgICAgICAgICBpZiAoaW5zZXJ0Q291bnQgPiAwKSB7XG4gICAgICAgICAgICAgICAgd2hpbGUgKHBhcnRJbmRleCAhPT0gLTEpIHtcbiAgICAgICAgICAgICAgICAgICAgcGFydHNbcGFydEluZGV4XS5pbmRleCArPSBpbnNlcnRDb3VudDtcbiAgICAgICAgICAgICAgICAgICAgcGFydEluZGV4ID0gbmV4dEFjdGl2ZUluZGV4SW5UZW1wbGF0ZVBhcnRzKHBhcnRzLCBwYXJ0SW5kZXgpO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICByZXR1cm47XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICBwYXJ0SW5kZXggPSBuZXh0QWN0aXZlSW5kZXhJblRlbXBsYXRlUGFydHMocGFydHMsIHBhcnRJbmRleCk7XG4gICAgICAgIH1cbiAgICB9XG59XG4vLyMgc291cmNlTWFwcGluZ1VSTD1tb2RpZnktdGVtcGxhdGUuanMubWFwIiwiLyoqXG4gKiBAbGljZW5zZVxuICogQ29weXJpZ2h0IChjKSAyMDE4IFRoZSBQb2x5bWVyIFByb2plY3QgQXV0aG9ycy4gQWxsIHJpZ2h0cyByZXNlcnZlZC5cbiAqIFRoaXMgY29kZSBtYXkgb25seSBiZSB1c2VkIHVuZGVyIHRoZSBCU0Qgc3R5bGUgbGljZW5zZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0xJQ0VOU0UudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGF1dGhvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQVVUSE9SUy50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgY29udHJpYnV0b3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0NPTlRSSUJVVE9SUy50eHRcbiAqIENvZGUgZGlzdHJpYnV0ZWQgYnkgR29vZ2xlIGFzIHBhcnQgb2YgdGhlIHBvbHltZXIgcHJvamVjdCBpcyBhbHNvXG4gKiBzdWJqZWN0IHRvIGFuIGFkZGl0aW9uYWwgSVAgcmlnaHRzIGdyYW50IGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vUEFURU5UUy50eHRcbiAqL1xuLyoqXG4gKiBBIHNlbnRpbmVsIHZhbHVlIHRoYXQgc2lnbmFscyB0aGF0IGEgdmFsdWUgd2FzIGhhbmRsZWQgYnkgYSBkaXJlY3RpdmUgYW5kXG4gKiBzaG91bGQgbm90IGJlIHdyaXR0ZW4gdG8gdGhlIERPTS5cbiAqL1xuZXhwb3J0IGNvbnN0IG5vQ2hhbmdlID0ge307XG4vKipcbiAqIEEgc2VudGluZWwgdmFsdWUgdGhhdCBzaWduYWxzIGEgTm9kZVBhcnQgdG8gZnVsbHkgY2xlYXIgaXRzIGNvbnRlbnQuXG4gKi9cbmV4cG9ydCBjb25zdCBub3RoaW5nID0ge307XG4vLyMgc291cmNlTWFwcGluZ1VSTD1wYXJ0LmpzLm1hcCIsIi8qKlxuICogQGxpY2Vuc2VcbiAqIENvcHlyaWdodCAoYykgMjAxNyBUaGUgUG9seW1lciBQcm9qZWN0IEF1dGhvcnMuIEFsbCByaWdodHMgcmVzZXJ2ZWQuXG4gKiBUaGlzIGNvZGUgbWF5IG9ubHkgYmUgdXNlZCB1bmRlciB0aGUgQlNEIHN0eWxlIGxpY2Vuc2UgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9MSUNFTlNFLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBhdXRob3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0FVVEhPUlMudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGNvbnRyaWJ1dG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9DT05UUklCVVRPUlMudHh0XG4gKiBDb2RlIGRpc3RyaWJ1dGVkIGJ5IEdvb2dsZSBhcyBwYXJ0IG9mIHRoZSBwb2x5bWVyIHByb2plY3QgaXMgYWxzb1xuICogc3ViamVjdCB0byBhbiBhZGRpdGlvbmFsIElQIHJpZ2h0cyBncmFudCBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL1BBVEVOVFMudHh0XG4gKi9cbi8qKlxuICogQG1vZHVsZSBsaXQtaHRtbFxuICovXG5pbXBvcnQgeyBpc0RpcmVjdGl2ZSB9IGZyb20gJy4vZGlyZWN0aXZlLmpzJztcbmltcG9ydCB7IHJlbW92ZU5vZGVzIH0gZnJvbSAnLi9kb20uanMnO1xuaW1wb3J0IHsgbm9DaGFuZ2UsIG5vdGhpbmcgfSBmcm9tICcuL3BhcnQuanMnO1xuaW1wb3J0IHsgVGVtcGxhdGVJbnN0YW5jZSB9IGZyb20gJy4vdGVtcGxhdGUtaW5zdGFuY2UuanMnO1xuaW1wb3J0IHsgVGVtcGxhdGVSZXN1bHQgfSBmcm9tICcuL3RlbXBsYXRlLXJlc3VsdC5qcyc7XG5pbXBvcnQgeyBjcmVhdGVNYXJrZXIgfSBmcm9tICcuL3RlbXBsYXRlLmpzJztcbmV4cG9ydCBjb25zdCBpc1ByaW1pdGl2ZSA9ICh2YWx1ZSkgPT4ge1xuICAgIHJldHVybiAodmFsdWUgPT09IG51bGwgfHxcbiAgICAgICAgISh0eXBlb2YgdmFsdWUgPT09ICdvYmplY3QnIHx8IHR5cGVvZiB2YWx1ZSA9PT0gJ2Z1bmN0aW9uJykpO1xufTtcbi8qKlxuICogU2V0cyBhdHRyaWJ1dGUgdmFsdWVzIGZvciBBdHRyaWJ1dGVQYXJ0cywgc28gdGhhdCB0aGUgdmFsdWUgaXMgb25seSBzZXQgb25jZVxuICogZXZlbiBpZiB0aGVyZSBhcmUgbXVsdGlwbGUgcGFydHMgZm9yIGFuIGF0dHJpYnV0ZS5cbiAqL1xuZXhwb3J0IGNsYXNzIEF0dHJpYnV0ZUNvbW1pdHRlciB7XG4gICAgY29uc3RydWN0b3IoZWxlbWVudCwgbmFtZSwgc3RyaW5ncykge1xuICAgICAgICB0aGlzLmRpcnR5ID0gdHJ1ZTtcbiAgICAgICAgdGhpcy5lbGVtZW50ID0gZWxlbWVudDtcbiAgICAgICAgdGhpcy5uYW1lID0gbmFtZTtcbiAgICAgICAgdGhpcy5zdHJpbmdzID0gc3RyaW5ncztcbiAgICAgICAgdGhpcy5wYXJ0cyA9IFtdO1xuICAgICAgICBmb3IgKGxldCBpID0gMDsgaSA8IHN0cmluZ3MubGVuZ3RoIC0gMTsgaSsrKSB7XG4gICAgICAgICAgICB0aGlzLnBhcnRzW2ldID0gdGhpcy5fY3JlYXRlUGFydCgpO1xuICAgICAgICB9XG4gICAgfVxuICAgIC8qKlxuICAgICAqIENyZWF0ZXMgYSBzaW5nbGUgcGFydC4gT3ZlcnJpZGUgdGhpcyB0byBjcmVhdGUgYSBkaWZmZXJudCB0eXBlIG9mIHBhcnQuXG4gICAgICovXG4gICAgX2NyZWF0ZVBhcnQoKSB7XG4gICAgICAgIHJldHVybiBuZXcgQXR0cmlidXRlUGFydCh0aGlzKTtcbiAgICB9XG4gICAgX2dldFZhbHVlKCkge1xuICAgICAgICBjb25zdCBzdHJpbmdzID0gdGhpcy5zdHJpbmdzO1xuICAgICAgICBjb25zdCBsID0gc3RyaW5ncy5sZW5ndGggLSAxO1xuICAgICAgICBsZXQgdGV4dCA9ICcnO1xuICAgICAgICBmb3IgKGxldCBpID0gMDsgaSA8IGw7IGkrKykge1xuICAgICAgICAgICAgdGV4dCArPSBzdHJpbmdzW2ldO1xuICAgICAgICAgICAgY29uc3QgcGFydCA9IHRoaXMucGFydHNbaV07XG4gICAgICAgICAgICBpZiAocGFydCAhPT0gdW5kZWZpbmVkKSB7XG4gICAgICAgICAgICAgICAgY29uc3QgdiA9IHBhcnQudmFsdWU7XG4gICAgICAgICAgICAgICAgaWYgKHYgIT0gbnVsbCAmJlxuICAgICAgICAgICAgICAgICAgICAoQXJyYXkuaXNBcnJheSh2KSB8fFxuICAgICAgICAgICAgICAgICAgICAgICAgLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOm5vLWFueVxuICAgICAgICAgICAgICAgICAgICAgICAgdHlwZW9mIHYgIT09ICdzdHJpbmcnICYmIHZbU3ltYm9sLml0ZXJhdG9yXSkpIHtcbiAgICAgICAgICAgICAgICAgICAgZm9yIChjb25zdCB0IG9mIHYpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgIHRleHQgKz0gdHlwZW9mIHQgPT09ICdzdHJpbmcnID8gdCA6IFN0cmluZyh0KTtcbiAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgICAgICAgICAgdGV4dCArPSB0eXBlb2YgdiA9PT0gJ3N0cmluZycgPyB2IDogU3RyaW5nKHYpO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgICAgICB0ZXh0ICs9IHN0cmluZ3NbbF07XG4gICAgICAgIHJldHVybiB0ZXh0O1xuICAgIH1cbiAgICBjb21taXQoKSB7XG4gICAgICAgIGlmICh0aGlzLmRpcnR5KSB7XG4gICAgICAgICAgICB0aGlzLmRpcnR5ID0gZmFsc2U7XG4gICAgICAgICAgICB0aGlzLmVsZW1lbnQuc2V0QXR0cmlidXRlKHRoaXMubmFtZSwgdGhpcy5fZ2V0VmFsdWUoKSk7XG4gICAgICAgIH1cbiAgICB9XG59XG5leHBvcnQgY2xhc3MgQXR0cmlidXRlUGFydCB7XG4gICAgY29uc3RydWN0b3IoY29taXR0ZXIpIHtcbiAgICAgICAgdGhpcy52YWx1ZSA9IHVuZGVmaW5lZDtcbiAgICAgICAgdGhpcy5jb21taXR0ZXIgPSBjb21pdHRlcjtcbiAgICB9XG4gICAgc2V0VmFsdWUodmFsdWUpIHtcbiAgICAgICAgaWYgKHZhbHVlICE9PSBub0NoYW5nZSAmJiAoIWlzUHJpbWl0aXZlKHZhbHVlKSB8fCB2YWx1ZSAhPT0gdGhpcy52YWx1ZSkpIHtcbiAgICAgICAgICAgIHRoaXMudmFsdWUgPSB2YWx1ZTtcbiAgICAgICAgICAgIC8vIElmIHRoZSB2YWx1ZSBpcyBhIG5vdCBhIGRpcmVjdGl2ZSwgZGlydHkgdGhlIGNvbW1pdHRlciBzbyB0aGF0IGl0J2xsXG4gICAgICAgICAgICAvLyBjYWxsIHNldEF0dHJpYnV0ZS4gSWYgdGhlIHZhbHVlIGlzIGEgZGlyZWN0aXZlLCBpdCdsbCBkaXJ0eSB0aGVcbiAgICAgICAgICAgIC8vIGNvbW1pdHRlciBpZiBpdCBjYWxscyBzZXRWYWx1ZSgpLlxuICAgICAgICAgICAgaWYgKCFpc0RpcmVjdGl2ZSh2YWx1ZSkpIHtcbiAgICAgICAgICAgICAgICB0aGlzLmNvbW1pdHRlci5kaXJ0eSA9IHRydWU7XG4gICAgICAgICAgICB9XG4gICAgICAgIH1cbiAgICB9XG4gICAgY29tbWl0KCkge1xuICAgICAgICB3aGlsZSAoaXNEaXJlY3RpdmUodGhpcy52YWx1ZSkpIHtcbiAgICAgICAgICAgIGNvbnN0IGRpcmVjdGl2ZSA9IHRoaXMudmFsdWU7XG4gICAgICAgICAgICB0aGlzLnZhbHVlID0gbm9DaGFuZ2U7XG4gICAgICAgICAgICBkaXJlY3RpdmUodGhpcyk7XG4gICAgICAgIH1cbiAgICAgICAgaWYgKHRoaXMudmFsdWUgPT09IG5vQ2hhbmdlKSB7XG4gICAgICAgICAgICByZXR1cm47XG4gICAgICAgIH1cbiAgICAgICAgdGhpcy5jb21taXR0ZXIuY29tbWl0KCk7XG4gICAgfVxufVxuZXhwb3J0IGNsYXNzIE5vZGVQYXJ0IHtcbiAgICBjb25zdHJ1Y3RvcihvcHRpb25zKSB7XG4gICAgICAgIHRoaXMudmFsdWUgPSB1bmRlZmluZWQ7XG4gICAgICAgIHRoaXMuX3BlbmRpbmdWYWx1ZSA9IHVuZGVmaW5lZDtcbiAgICAgICAgdGhpcy5vcHRpb25zID0gb3B0aW9ucztcbiAgICB9XG4gICAgLyoqXG4gICAgICogSW5zZXJ0cyB0aGlzIHBhcnQgaW50byBhIGNvbnRhaW5lci5cbiAgICAgKlxuICAgICAqIFRoaXMgcGFydCBtdXN0IGJlIGVtcHR5LCBhcyBpdHMgY29udGVudHMgYXJlIG5vdCBhdXRvbWF0aWNhbGx5IG1vdmVkLlxuICAgICAqL1xuICAgIGFwcGVuZEludG8oY29udGFpbmVyKSB7XG4gICAgICAgIHRoaXMuc3RhcnROb2RlID0gY29udGFpbmVyLmFwcGVuZENoaWxkKGNyZWF0ZU1hcmtlcigpKTtcbiAgICAgICAgdGhpcy5lbmROb2RlID0gY29udGFpbmVyLmFwcGVuZENoaWxkKGNyZWF0ZU1hcmtlcigpKTtcbiAgICB9XG4gICAgLyoqXG4gICAgICogSW5zZXJ0cyB0aGlzIHBhcnQgYmV0d2VlbiBgcmVmYCBhbmQgYHJlZmAncyBuZXh0IHNpYmxpbmcuIEJvdGggYHJlZmAgYW5kXG4gICAgICogaXRzIG5leHQgc2libGluZyBtdXN0IGJlIHN0YXRpYywgdW5jaGFuZ2luZyBub2RlcyBzdWNoIGFzIHRob3NlIHRoYXQgYXBwZWFyXG4gICAgICogaW4gYSBsaXRlcmFsIHNlY3Rpb24gb2YgYSB0ZW1wbGF0ZS5cbiAgICAgKlxuICAgICAqIFRoaXMgcGFydCBtdXN0IGJlIGVtcHR5LCBhcyBpdHMgY29udGVudHMgYXJlIG5vdCBhdXRvbWF0aWNhbGx5IG1vdmVkLlxuICAgICAqL1xuICAgIGluc2VydEFmdGVyTm9kZShyZWYpIHtcbiAgICAgICAgdGhpcy5zdGFydE5vZGUgPSByZWY7XG4gICAgICAgIHRoaXMuZW5kTm9kZSA9IHJlZi5uZXh0U2libGluZztcbiAgICB9XG4gICAgLyoqXG4gICAgICogQXBwZW5kcyB0aGlzIHBhcnQgaW50byBhIHBhcmVudCBwYXJ0LlxuICAgICAqXG4gICAgICogVGhpcyBwYXJ0IG11c3QgYmUgZW1wdHksIGFzIGl0cyBjb250ZW50cyBhcmUgbm90IGF1dG9tYXRpY2FsbHkgbW92ZWQuXG4gICAgICovXG4gICAgYXBwZW5kSW50b1BhcnQocGFydCkge1xuICAgICAgICBwYXJ0Ll9pbnNlcnQodGhpcy5zdGFydE5vZGUgPSBjcmVhdGVNYXJrZXIoKSk7XG4gICAgICAgIHBhcnQuX2luc2VydCh0aGlzLmVuZE5vZGUgPSBjcmVhdGVNYXJrZXIoKSk7XG4gICAgfVxuICAgIC8qKlxuICAgICAqIEFwcGVuZHMgdGhpcyBwYXJ0IGFmdGVyIGByZWZgXG4gICAgICpcbiAgICAgKiBUaGlzIHBhcnQgbXVzdCBiZSBlbXB0eSwgYXMgaXRzIGNvbnRlbnRzIGFyZSBub3QgYXV0b21hdGljYWxseSBtb3ZlZC5cbiAgICAgKi9cbiAgICBpbnNlcnRBZnRlclBhcnQocmVmKSB7XG4gICAgICAgIHJlZi5faW5zZXJ0KHRoaXMuc3RhcnROb2RlID0gY3JlYXRlTWFya2VyKCkpO1xuICAgICAgICB0aGlzLmVuZE5vZGUgPSByZWYuZW5kTm9kZTtcbiAgICAgICAgcmVmLmVuZE5vZGUgPSB0aGlzLnN0YXJ0Tm9kZTtcbiAgICB9XG4gICAgc2V0VmFsdWUodmFsdWUpIHtcbiAgICAgICAgdGhpcy5fcGVuZGluZ1ZhbHVlID0gdmFsdWU7XG4gICAgfVxuICAgIGNvbW1pdCgpIHtcbiAgICAgICAgd2hpbGUgKGlzRGlyZWN0aXZlKHRoaXMuX3BlbmRpbmdWYWx1ZSkpIHtcbiAgICAgICAgICAgIGNvbnN0IGRpcmVjdGl2ZSA9IHRoaXMuX3BlbmRpbmdWYWx1ZTtcbiAgICAgICAgICAgIHRoaXMuX3BlbmRpbmdWYWx1ZSA9IG5vQ2hhbmdlO1xuICAgICAgICAgICAgZGlyZWN0aXZlKHRoaXMpO1xuICAgICAgICB9XG4gICAgICAgIGNvbnN0IHZhbHVlID0gdGhpcy5fcGVuZGluZ1ZhbHVlO1xuICAgICAgICBpZiAodmFsdWUgPT09IG5vQ2hhbmdlKSB7XG4gICAgICAgICAgICByZXR1cm47XG4gICAgICAgIH1cbiAgICAgICAgaWYgKGlzUHJpbWl0aXZlKHZhbHVlKSkge1xuICAgICAgICAgICAgaWYgKHZhbHVlICE9PSB0aGlzLnZhbHVlKSB7XG4gICAgICAgICAgICAgICAgdGhpcy5fY29tbWl0VGV4dCh2YWx1ZSk7XG4gICAgICAgICAgICB9XG4gICAgICAgIH1cbiAgICAgICAgZWxzZSBpZiAodmFsdWUgaW5zdGFuY2VvZiBUZW1wbGF0ZVJlc3VsdCkge1xuICAgICAgICAgICAgdGhpcy5fY29tbWl0VGVtcGxhdGVSZXN1bHQodmFsdWUpO1xuICAgICAgICB9XG4gICAgICAgIGVsc2UgaWYgKHZhbHVlIGluc3RhbmNlb2YgTm9kZSkge1xuICAgICAgICAgICAgdGhpcy5fY29tbWl0Tm9kZSh2YWx1ZSk7XG4gICAgICAgIH1cbiAgICAgICAgZWxzZSBpZiAoQXJyYXkuaXNBcnJheSh2YWx1ZSkgfHxcbiAgICAgICAgICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTpuby1hbnlcbiAgICAgICAgICAgIHZhbHVlW1N5bWJvbC5pdGVyYXRvcl0pIHtcbiAgICAgICAgICAgIHRoaXMuX2NvbW1pdEl0ZXJhYmxlKHZhbHVlKTtcbiAgICAgICAgfVxuICAgICAgICBlbHNlIGlmICh2YWx1ZSA9PT0gbm90aGluZykge1xuICAgICAgICAgICAgdGhpcy52YWx1ZSA9IG5vdGhpbmc7XG4gICAgICAgICAgICB0aGlzLmNsZWFyKCk7XG4gICAgICAgIH1cbiAgICAgICAgZWxzZSB7XG4gICAgICAgICAgICAvLyBGYWxsYmFjaywgd2lsbCByZW5kZXIgdGhlIHN0cmluZyByZXByZXNlbnRhdGlvblxuICAgICAgICAgICAgdGhpcy5fY29tbWl0VGV4dCh2YWx1ZSk7XG4gICAgICAgIH1cbiAgICB9XG4gICAgX2luc2VydChub2RlKSB7XG4gICAgICAgIHRoaXMuZW5kTm9kZS5wYXJlbnROb2RlLmluc2VydEJlZm9yZShub2RlLCB0aGlzLmVuZE5vZGUpO1xuICAgIH1cbiAgICBfY29tbWl0Tm9kZSh2YWx1ZSkge1xuICAgICAgICBpZiAodGhpcy52YWx1ZSA9PT0gdmFsdWUpIHtcbiAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgfVxuICAgICAgICB0aGlzLmNsZWFyKCk7XG4gICAgICAgIHRoaXMuX2luc2VydCh2YWx1ZSk7XG4gICAgICAgIHRoaXMudmFsdWUgPSB2YWx1ZTtcbiAgICB9XG4gICAgX2NvbW1pdFRleHQodmFsdWUpIHtcbiAgICAgICAgY29uc3Qgbm9kZSA9IHRoaXMuc3RhcnROb2RlLm5leHRTaWJsaW5nO1xuICAgICAgICB2YWx1ZSA9IHZhbHVlID09IG51bGwgPyAnJyA6IHZhbHVlO1xuICAgICAgICBpZiAobm9kZSA9PT0gdGhpcy5lbmROb2RlLnByZXZpb3VzU2libGluZyAmJlxuICAgICAgICAgICAgbm9kZS5ub2RlVHlwZSA9PT0gMyAvKiBOb2RlLlRFWFRfTk9ERSAqLykge1xuICAgICAgICAgICAgLy8gSWYgd2Ugb25seSBoYXZlIGEgc2luZ2xlIHRleHQgbm9kZSBiZXR3ZWVuIHRoZSBtYXJrZXJzLCB3ZSBjYW4ganVzdFxuICAgICAgICAgICAgLy8gc2V0IGl0cyB2YWx1ZSwgcmF0aGVyIHRoYW4gcmVwbGFjaW5nIGl0LlxuICAgICAgICAgICAgLy8gVE9ETyhqdXN0aW5mYWduYW5pKTogQ2FuIHdlIGp1c3QgY2hlY2sgaWYgdGhpcy52YWx1ZSBpcyBwcmltaXRpdmU/XG4gICAgICAgICAgICBub2RlLmRhdGEgPSB2YWx1ZTtcbiAgICAgICAgfVxuICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgIHRoaXMuX2NvbW1pdE5vZGUoZG9jdW1lbnQuY3JlYXRlVGV4dE5vZGUodHlwZW9mIHZhbHVlID09PSAnc3RyaW5nJyA/IHZhbHVlIDogU3RyaW5nKHZhbHVlKSkpO1xuICAgICAgICB9XG4gICAgICAgIHRoaXMudmFsdWUgPSB2YWx1ZTtcbiAgICB9XG4gICAgX2NvbW1pdFRlbXBsYXRlUmVzdWx0KHZhbHVlKSB7XG4gICAgICAgIGNvbnN0IHRlbXBsYXRlID0gdGhpcy5vcHRpb25zLnRlbXBsYXRlRmFjdG9yeSh2YWx1ZSk7XG4gICAgICAgIGlmICh0aGlzLnZhbHVlIGluc3RhbmNlb2YgVGVtcGxhdGVJbnN0YW5jZSAmJlxuICAgICAgICAgICAgdGhpcy52YWx1ZS50ZW1wbGF0ZSA9PT0gdGVtcGxhdGUpIHtcbiAgICAgICAgICAgIHRoaXMudmFsdWUudXBkYXRlKHZhbHVlLnZhbHVlcyk7XG4gICAgICAgIH1cbiAgICAgICAgZWxzZSB7XG4gICAgICAgICAgICAvLyBNYWtlIHN1cmUgd2UgcHJvcGFnYXRlIHRoZSB0ZW1wbGF0ZSBwcm9jZXNzb3IgZnJvbSB0aGUgVGVtcGxhdGVSZXN1bHRcbiAgICAgICAgICAgIC8vIHNvIHRoYXQgd2UgdXNlIGl0cyBzeW50YXggZXh0ZW5zaW9uLCBldGMuIFRoZSB0ZW1wbGF0ZSBmYWN0b3J5IGNvbWVzXG4gICAgICAgICAgICAvLyBmcm9tIHRoZSByZW5kZXIgZnVuY3Rpb24gb3B0aW9ucyBzbyB0aGF0IGl0IGNhbiBjb250cm9sIHRlbXBsYXRlXG4gICAgICAgICAgICAvLyBjYWNoaW5nIGFuZCBwcmVwcm9jZXNzaW5nLlxuICAgICAgICAgICAgY29uc3QgaW5zdGFuY2UgPSBuZXcgVGVtcGxhdGVJbnN0YW5jZSh0ZW1wbGF0ZSwgdmFsdWUucHJvY2Vzc29yLCB0aGlzLm9wdGlvbnMpO1xuICAgICAgICAgICAgY29uc3QgZnJhZ21lbnQgPSBpbnN0YW5jZS5fY2xvbmUoKTtcbiAgICAgICAgICAgIGluc3RhbmNlLnVwZGF0ZSh2YWx1ZS52YWx1ZXMpO1xuICAgICAgICAgICAgdGhpcy5fY29tbWl0Tm9kZShmcmFnbWVudCk7XG4gICAgICAgICAgICB0aGlzLnZhbHVlID0gaW5zdGFuY2U7XG4gICAgICAgIH1cbiAgICB9XG4gICAgX2NvbW1pdEl0ZXJhYmxlKHZhbHVlKSB7XG4gICAgICAgIC8vIEZvciBhbiBJdGVyYWJsZSwgd2UgY3JlYXRlIGEgbmV3IEluc3RhbmNlUGFydCBwZXIgaXRlbSwgdGhlbiBzZXQgaXRzXG4gICAgICAgIC8vIHZhbHVlIHRvIHRoZSBpdGVtLiBUaGlzIGlzIGEgbGl0dGxlIGJpdCBvZiBvdmVyaGVhZCBmb3IgZXZlcnkgaXRlbSBpblxuICAgICAgICAvLyBhbiBJdGVyYWJsZSwgYnV0IGl0IGxldHMgdXMgcmVjdXJzZSBlYXNpbHkgYW5kIGVmZmljaWVudGx5IHVwZGF0ZSBBcnJheXNcbiAgICAgICAgLy8gb2YgVGVtcGxhdGVSZXN1bHRzIHRoYXQgd2lsbCBiZSBjb21tb25seSByZXR1cm5lZCBmcm9tIGV4cHJlc3Npb25zIGxpa2U6XG4gICAgICAgIC8vIGFycmF5Lm1hcCgoaSkgPT4gaHRtbGAke2l9YCksIGJ5IHJldXNpbmcgZXhpc3RpbmcgVGVtcGxhdGVJbnN0YW5jZXMuXG4gICAgICAgIC8vIElmIF92YWx1ZSBpcyBhbiBhcnJheSwgdGhlbiB0aGUgcHJldmlvdXMgcmVuZGVyIHdhcyBvZiBhblxuICAgICAgICAvLyBpdGVyYWJsZSBhbmQgX3ZhbHVlIHdpbGwgY29udGFpbiB0aGUgTm9kZVBhcnRzIGZyb20gdGhlIHByZXZpb3VzXG4gICAgICAgIC8vIHJlbmRlci4gSWYgX3ZhbHVlIGlzIG5vdCBhbiBhcnJheSwgY2xlYXIgdGhpcyBwYXJ0IGFuZCBtYWtlIGEgbmV3XG4gICAgICAgIC8vIGFycmF5IGZvciBOb2RlUGFydHMuXG4gICAgICAgIGlmICghQXJyYXkuaXNBcnJheSh0aGlzLnZhbHVlKSkge1xuICAgICAgICAgICAgdGhpcy52YWx1ZSA9IFtdO1xuICAgICAgICAgICAgdGhpcy5jbGVhcigpO1xuICAgICAgICB9XG4gICAgICAgIC8vIExldHMgdXMga2VlcCB0cmFjayBvZiBob3cgbWFueSBpdGVtcyB3ZSBzdGFtcGVkIHNvIHdlIGNhbiBjbGVhciBsZWZ0b3ZlclxuICAgICAgICAvLyBpdGVtcyBmcm9tIGEgcHJldmlvdXMgcmVuZGVyXG4gICAgICAgIGNvbnN0IGl0ZW1QYXJ0cyA9IHRoaXMudmFsdWU7XG4gICAgICAgIGxldCBwYXJ0SW5kZXggPSAwO1xuICAgICAgICBsZXQgaXRlbVBhcnQ7XG4gICAgICAgIGZvciAoY29uc3QgaXRlbSBvZiB2YWx1ZSkge1xuICAgICAgICAgICAgLy8gVHJ5IHRvIHJldXNlIGFuIGV4aXN0aW5nIHBhcnRcbiAgICAgICAgICAgIGl0ZW1QYXJ0ID0gaXRlbVBhcnRzW3BhcnRJbmRleF07XG4gICAgICAgICAgICAvLyBJZiBubyBleGlzdGluZyBwYXJ0LCBjcmVhdGUgYSBuZXcgb25lXG4gICAgICAgICAgICBpZiAoaXRlbVBhcnQgPT09IHVuZGVmaW5lZCkge1xuICAgICAgICAgICAgICAgIGl0ZW1QYXJ0ID0gbmV3IE5vZGVQYXJ0KHRoaXMub3B0aW9ucyk7XG4gICAgICAgICAgICAgICAgaXRlbVBhcnRzLnB1c2goaXRlbVBhcnQpO1xuICAgICAgICAgICAgICAgIGlmIChwYXJ0SW5kZXggPT09IDApIHtcbiAgICAgICAgICAgICAgICAgICAgaXRlbVBhcnQuYXBwZW5kSW50b1BhcnQodGhpcyk7XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIGVsc2Uge1xuICAgICAgICAgICAgICAgICAgICBpdGVtUGFydC5pbnNlcnRBZnRlclBhcnQoaXRlbVBhcnRzW3BhcnRJbmRleCAtIDFdKTtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICBpdGVtUGFydC5zZXRWYWx1ZShpdGVtKTtcbiAgICAgICAgICAgIGl0ZW1QYXJ0LmNvbW1pdCgpO1xuICAgICAgICAgICAgcGFydEluZGV4Kys7XG4gICAgICAgIH1cbiAgICAgICAgaWYgKHBhcnRJbmRleCA8IGl0ZW1QYXJ0cy5sZW5ndGgpIHtcbiAgICAgICAgICAgIC8vIFRydW5jYXRlIHRoZSBwYXJ0cyBhcnJheSBzbyBfdmFsdWUgcmVmbGVjdHMgdGhlIGN1cnJlbnQgc3RhdGVcbiAgICAgICAgICAgIGl0ZW1QYXJ0cy5sZW5ndGggPSBwYXJ0SW5kZXg7XG4gICAgICAgICAgICB0aGlzLmNsZWFyKGl0ZW1QYXJ0ICYmIGl0ZW1QYXJ0LmVuZE5vZGUpO1xuICAgICAgICB9XG4gICAgfVxuICAgIGNsZWFyKHN0YXJ0Tm9kZSA9IHRoaXMuc3RhcnROb2RlKSB7XG4gICAgICAgIHJlbW92ZU5vZGVzKHRoaXMuc3RhcnROb2RlLnBhcmVudE5vZGUsIHN0YXJ0Tm9kZS5uZXh0U2libGluZywgdGhpcy5lbmROb2RlKTtcbiAgICB9XG59XG4vKipcbiAqIEltcGxlbWVudHMgYSBib29sZWFuIGF0dHJpYnV0ZSwgcm91Z2hseSBhcyBkZWZpbmVkIGluIHRoZSBIVE1MXG4gKiBzcGVjaWZpY2F0aW9uLlxuICpcbiAqIElmIHRoZSB2YWx1ZSBpcyB0cnV0aHksIHRoZW4gdGhlIGF0dHJpYnV0ZSBpcyBwcmVzZW50IHdpdGggYSB2YWx1ZSBvZlxuICogJycuIElmIHRoZSB2YWx1ZSBpcyBmYWxzZXksIHRoZSBhdHRyaWJ1dGUgaXMgcmVtb3ZlZC5cbiAqL1xuZXhwb3J0IGNsYXNzIEJvb2xlYW5BdHRyaWJ1dGVQYXJ0IHtcbiAgICBjb25zdHJ1Y3RvcihlbGVtZW50LCBuYW1lLCBzdHJpbmdzKSB7XG4gICAgICAgIHRoaXMudmFsdWUgPSB1bmRlZmluZWQ7XG4gICAgICAgIHRoaXMuX3BlbmRpbmdWYWx1ZSA9IHVuZGVmaW5lZDtcbiAgICAgICAgaWYgKHN0cmluZ3MubGVuZ3RoICE9PSAyIHx8IHN0cmluZ3NbMF0gIT09ICcnIHx8IHN0cmluZ3NbMV0gIT09ICcnKSB7XG4gICAgICAgICAgICB0aHJvdyBuZXcgRXJyb3IoJ0Jvb2xlYW4gYXR0cmlidXRlcyBjYW4gb25seSBjb250YWluIGEgc2luZ2xlIGV4cHJlc3Npb24nKTtcbiAgICAgICAgfVxuICAgICAgICB0aGlzLmVsZW1lbnQgPSBlbGVtZW50O1xuICAgICAgICB0aGlzLm5hbWUgPSBuYW1lO1xuICAgICAgICB0aGlzLnN0cmluZ3MgPSBzdHJpbmdzO1xuICAgIH1cbiAgICBzZXRWYWx1ZSh2YWx1ZSkge1xuICAgICAgICB0aGlzLl9wZW5kaW5nVmFsdWUgPSB2YWx1ZTtcbiAgICB9XG4gICAgY29tbWl0KCkge1xuICAgICAgICB3aGlsZSAoaXNEaXJlY3RpdmUodGhpcy5fcGVuZGluZ1ZhbHVlKSkge1xuICAgICAgICAgICAgY29uc3QgZGlyZWN0aXZlID0gdGhpcy5fcGVuZGluZ1ZhbHVlO1xuICAgICAgICAgICAgdGhpcy5fcGVuZGluZ1ZhbHVlID0gbm9DaGFuZ2U7XG4gICAgICAgICAgICBkaXJlY3RpdmUodGhpcyk7XG4gICAgICAgIH1cbiAgICAgICAgaWYgKHRoaXMuX3BlbmRpbmdWYWx1ZSA9PT0gbm9DaGFuZ2UpIHtcbiAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgfVxuICAgICAgICBjb25zdCB2YWx1ZSA9ICEhdGhpcy5fcGVuZGluZ1ZhbHVlO1xuICAgICAgICBpZiAodGhpcy52YWx1ZSAhPT0gdmFsdWUpIHtcbiAgICAgICAgICAgIGlmICh2YWx1ZSkge1xuICAgICAgICAgICAgICAgIHRoaXMuZWxlbWVudC5zZXRBdHRyaWJ1dGUodGhpcy5uYW1lLCAnJyk7XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgICAgICB0aGlzLmVsZW1lbnQucmVtb3ZlQXR0cmlidXRlKHRoaXMubmFtZSk7XG4gICAgICAgICAgICB9XG4gICAgICAgIH1cbiAgICAgICAgdGhpcy52YWx1ZSA9IHZhbHVlO1xuICAgICAgICB0aGlzLl9wZW5kaW5nVmFsdWUgPSBub0NoYW5nZTtcbiAgICB9XG59XG4vKipcbiAqIFNldHMgYXR0cmlidXRlIHZhbHVlcyBmb3IgUHJvcGVydHlQYXJ0cywgc28gdGhhdCB0aGUgdmFsdWUgaXMgb25seSBzZXQgb25jZVxuICogZXZlbiBpZiB0aGVyZSBhcmUgbXVsdGlwbGUgcGFydHMgZm9yIGEgcHJvcGVydHkuXG4gKlxuICogSWYgYW4gZXhwcmVzc2lvbiBjb250cm9scyB0aGUgd2hvbGUgcHJvcGVydHkgdmFsdWUsIHRoZW4gdGhlIHZhbHVlIGlzIHNpbXBseVxuICogYXNzaWduZWQgdG8gdGhlIHByb3BlcnR5IHVuZGVyIGNvbnRyb2wuIElmIHRoZXJlIGFyZSBzdHJpbmcgbGl0ZXJhbHMgb3JcbiAqIG11bHRpcGxlIGV4cHJlc3Npb25zLCB0aGVuIHRoZSBzdHJpbmdzIGFyZSBleHByZXNzaW9ucyBhcmUgaW50ZXJwb2xhdGVkIGludG9cbiAqIGEgc3RyaW5nIGZpcnN0LlxuICovXG5leHBvcnQgY2xhc3MgUHJvcGVydHlDb21taXR0ZXIgZXh0ZW5kcyBBdHRyaWJ1dGVDb21taXR0ZXIge1xuICAgIGNvbnN0cnVjdG9yKGVsZW1lbnQsIG5hbWUsIHN0cmluZ3MpIHtcbiAgICAgICAgc3VwZXIoZWxlbWVudCwgbmFtZSwgc3RyaW5ncyk7XG4gICAgICAgIHRoaXMuc2luZ2xlID1cbiAgICAgICAgICAgIChzdHJpbmdzLmxlbmd0aCA9PT0gMiAmJiBzdHJpbmdzWzBdID09PSAnJyAmJiBzdHJpbmdzWzFdID09PSAnJyk7XG4gICAgfVxuICAgIF9jcmVhdGVQYXJ0KCkge1xuICAgICAgICByZXR1cm4gbmV3IFByb3BlcnR5UGFydCh0aGlzKTtcbiAgICB9XG4gICAgX2dldFZhbHVlKCkge1xuICAgICAgICBpZiAodGhpcy5zaW5nbGUpIHtcbiAgICAgICAgICAgIHJldHVybiB0aGlzLnBhcnRzWzBdLnZhbHVlO1xuICAgICAgICB9XG4gICAgICAgIHJldHVybiBzdXBlci5fZ2V0VmFsdWUoKTtcbiAgICB9XG4gICAgY29tbWl0KCkge1xuICAgICAgICBpZiAodGhpcy5kaXJ0eSkge1xuICAgICAgICAgICAgdGhpcy5kaXJ0eSA9IGZhbHNlO1xuICAgICAgICAgICAgLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOm5vLWFueVxuICAgICAgICAgICAgdGhpcy5lbGVtZW50W3RoaXMubmFtZV0gPSB0aGlzLl9nZXRWYWx1ZSgpO1xuICAgICAgICB9XG4gICAgfVxufVxuZXhwb3J0IGNsYXNzIFByb3BlcnR5UGFydCBleHRlbmRzIEF0dHJpYnV0ZVBhcnQge1xufVxuLy8gRGV0ZWN0IGV2ZW50IGxpc3RlbmVyIG9wdGlvbnMgc3VwcG9ydC4gSWYgdGhlIGBjYXB0dXJlYCBwcm9wZXJ0eSBpcyByZWFkXG4vLyBmcm9tIHRoZSBvcHRpb25zIG9iamVjdCwgdGhlbiBvcHRpb25zIGFyZSBzdXBwb3J0ZWQuIElmIG5vdCwgdGhlbiB0aGUgdGhyaWRcbi8vIGFyZ3VtZW50IHRvIGFkZC9yZW1vdmVFdmVudExpc3RlbmVyIGlzIGludGVycHJldGVkIGFzIHRoZSBib29sZWFuIGNhcHR1cmVcbi8vIHZhbHVlIHNvIHdlIHNob3VsZCBvbmx5IHBhc3MgdGhlIGBjYXB0dXJlYCBwcm9wZXJ0eS5cbmxldCBldmVudE9wdGlvbnNTdXBwb3J0ZWQgPSBmYWxzZTtcbnRyeSB7XG4gICAgY29uc3Qgb3B0aW9ucyA9IHtcbiAgICAgICAgZ2V0IGNhcHR1cmUoKSB7XG4gICAgICAgICAgICBldmVudE9wdGlvbnNTdXBwb3J0ZWQgPSB0cnVlO1xuICAgICAgICAgICAgcmV0dXJuIGZhbHNlO1xuICAgICAgICB9XG4gICAgfTtcbiAgICAvLyB0c2xpbnQ6ZGlzYWJsZS1uZXh0LWxpbmU6bm8tYW55XG4gICAgd2luZG93LmFkZEV2ZW50TGlzdGVuZXIoJ3Rlc3QnLCBvcHRpb25zLCBvcHRpb25zKTtcbiAgICAvLyB0c2xpbnQ6ZGlzYWJsZS1uZXh0LWxpbmU6bm8tYW55XG4gICAgd2luZG93LnJlbW92ZUV2ZW50TGlzdGVuZXIoJ3Rlc3QnLCBvcHRpb25zLCBvcHRpb25zKTtcbn1cbmNhdGNoIChfZSkge1xufVxuZXhwb3J0IGNsYXNzIEV2ZW50UGFydCB7XG4gICAgY29uc3RydWN0b3IoZWxlbWVudCwgZXZlbnROYW1lLCBldmVudENvbnRleHQpIHtcbiAgICAgICAgdGhpcy52YWx1ZSA9IHVuZGVmaW5lZDtcbiAgICAgICAgdGhpcy5fcGVuZGluZ1ZhbHVlID0gdW5kZWZpbmVkO1xuICAgICAgICB0aGlzLmVsZW1lbnQgPSBlbGVtZW50O1xuICAgICAgICB0aGlzLmV2ZW50TmFtZSA9IGV2ZW50TmFtZTtcbiAgICAgICAgdGhpcy5ldmVudENvbnRleHQgPSBldmVudENvbnRleHQ7XG4gICAgICAgIHRoaXMuX2JvdW5kSGFuZGxlRXZlbnQgPSAoZSkgPT4gdGhpcy5oYW5kbGVFdmVudChlKTtcbiAgICB9XG4gICAgc2V0VmFsdWUodmFsdWUpIHtcbiAgICAgICAgdGhpcy5fcGVuZGluZ1ZhbHVlID0gdmFsdWU7XG4gICAgfVxuICAgIGNvbW1pdCgpIHtcbiAgICAgICAgd2hpbGUgKGlzRGlyZWN0aXZlKHRoaXMuX3BlbmRpbmdWYWx1ZSkpIHtcbiAgICAgICAgICAgIGNvbnN0IGRpcmVjdGl2ZSA9IHRoaXMuX3BlbmRpbmdWYWx1ZTtcbiAgICAgICAgICAgIHRoaXMuX3BlbmRpbmdWYWx1ZSA9IG5vQ2hhbmdlO1xuICAgICAgICAgICAgZGlyZWN0aXZlKHRoaXMpO1xuICAgICAgICB9XG4gICAgICAgIGlmICh0aGlzLl9wZW5kaW5nVmFsdWUgPT09IG5vQ2hhbmdlKSB7XG4gICAgICAgICAgICByZXR1cm47XG4gICAgICAgIH1cbiAgICAgICAgY29uc3QgbmV3TGlzdGVuZXIgPSB0aGlzLl9wZW5kaW5nVmFsdWU7XG4gICAgICAgIGNvbnN0IG9sZExpc3RlbmVyID0gdGhpcy52YWx1ZTtcbiAgICAgICAgY29uc3Qgc2hvdWxkUmVtb3ZlTGlzdGVuZXIgPSBuZXdMaXN0ZW5lciA9PSBudWxsIHx8XG4gICAgICAgICAgICBvbGRMaXN0ZW5lciAhPSBudWxsICYmXG4gICAgICAgICAgICAgICAgKG5ld0xpc3RlbmVyLmNhcHR1cmUgIT09IG9sZExpc3RlbmVyLmNhcHR1cmUgfHxcbiAgICAgICAgICAgICAgICAgICAgbmV3TGlzdGVuZXIub25jZSAhPT0gb2xkTGlzdGVuZXIub25jZSB8fFxuICAgICAgICAgICAgICAgICAgICBuZXdMaXN0ZW5lci5wYXNzaXZlICE9PSBvbGRMaXN0ZW5lci5wYXNzaXZlKTtcbiAgICAgICAgY29uc3Qgc2hvdWxkQWRkTGlzdGVuZXIgPSBuZXdMaXN0ZW5lciAhPSBudWxsICYmIChvbGRMaXN0ZW5lciA9PSBudWxsIHx8IHNob3VsZFJlbW92ZUxpc3RlbmVyKTtcbiAgICAgICAgaWYgKHNob3VsZFJlbW92ZUxpc3RlbmVyKSB7XG4gICAgICAgICAgICB0aGlzLmVsZW1lbnQucmVtb3ZlRXZlbnRMaXN0ZW5lcih0aGlzLmV2ZW50TmFtZSwgdGhpcy5fYm91bmRIYW5kbGVFdmVudCwgdGhpcy5fb3B0aW9ucyk7XG4gICAgICAgIH1cbiAgICAgICAgaWYgKHNob3VsZEFkZExpc3RlbmVyKSB7XG4gICAgICAgICAgICB0aGlzLl9vcHRpb25zID0gZ2V0T3B0aW9ucyhuZXdMaXN0ZW5lcik7XG4gICAgICAgICAgICB0aGlzLmVsZW1lbnQuYWRkRXZlbnRMaXN0ZW5lcih0aGlzLmV2ZW50TmFtZSwgdGhpcy5fYm91bmRIYW5kbGVFdmVudCwgdGhpcy5fb3B0aW9ucyk7XG4gICAgICAgIH1cbiAgICAgICAgdGhpcy52YWx1ZSA9IG5ld0xpc3RlbmVyO1xuICAgICAgICB0aGlzLl9wZW5kaW5nVmFsdWUgPSBub0NoYW5nZTtcbiAgICB9XG4gICAgaGFuZGxlRXZlbnQoZXZlbnQpIHtcbiAgICAgICAgaWYgKHR5cGVvZiB0aGlzLnZhbHVlID09PSAnZnVuY3Rpb24nKSB7XG4gICAgICAgICAgICB0aGlzLnZhbHVlLmNhbGwodGhpcy5ldmVudENvbnRleHQgfHwgdGhpcy5lbGVtZW50LCBldmVudCk7XG4gICAgICAgIH1cbiAgICAgICAgZWxzZSB7XG4gICAgICAgICAgICB0aGlzLnZhbHVlLmhhbmRsZUV2ZW50KGV2ZW50KTtcbiAgICAgICAgfVxuICAgIH1cbn1cbi8vIFdlIGNvcHkgb3B0aW9ucyBiZWNhdXNlIG9mIHRoZSBpbmNvbnNpc3RlbnQgYmVoYXZpb3Igb2YgYnJvd3NlcnMgd2hlbiByZWFkaW5nXG4vLyB0aGUgdGhpcmQgYXJndW1lbnQgb2YgYWRkL3JlbW92ZUV2ZW50TGlzdGVuZXIuIElFMTEgZG9lc24ndCBzdXBwb3J0IG9wdGlvbnNcbi8vIGF0IGFsbC4gQ2hyb21lIDQxIG9ubHkgcmVhZHMgYGNhcHR1cmVgIGlmIHRoZSBhcmd1bWVudCBpcyBhbiBvYmplY3QuXG5jb25zdCBnZXRPcHRpb25zID0gKG8pID0+IG8gJiZcbiAgICAoZXZlbnRPcHRpb25zU3VwcG9ydGVkID9cbiAgICAgICAgeyBjYXB0dXJlOiBvLmNhcHR1cmUsIHBhc3NpdmU6IG8ucGFzc2l2ZSwgb25jZTogby5vbmNlIH0gOlxuICAgICAgICBvLmNhcHR1cmUpO1xuLy8jIHNvdXJjZU1hcHBpbmdVUkw9cGFydHMuanMubWFwIiwiLyoqXG4gKiBAbGljZW5zZVxuICogQ29weXJpZ2h0IChjKSAyMDE3IFRoZSBQb2x5bWVyIFByb2plY3QgQXV0aG9ycy4gQWxsIHJpZ2h0cyByZXNlcnZlZC5cbiAqIFRoaXMgY29kZSBtYXkgb25seSBiZSB1c2VkIHVuZGVyIHRoZSBCU0Qgc3R5bGUgbGljZW5zZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0xJQ0VOU0UudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGF1dGhvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQVVUSE9SUy50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgY29udHJpYnV0b3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0NPTlRSSUJVVE9SUy50eHRcbiAqIENvZGUgZGlzdHJpYnV0ZWQgYnkgR29vZ2xlIGFzIHBhcnQgb2YgdGhlIHBvbHltZXIgcHJvamVjdCBpcyBhbHNvXG4gKiBzdWJqZWN0IHRvIGFuIGFkZGl0aW9uYWwgSVAgcmlnaHRzIGdyYW50IGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vUEFURU5UUy50eHRcbiAqL1xuLyoqXG4gKiBAbW9kdWxlIGxpdC1odG1sXG4gKi9cbmltcG9ydCB7IHJlbW92ZU5vZGVzIH0gZnJvbSAnLi9kb20uanMnO1xuaW1wb3J0IHsgTm9kZVBhcnQgfSBmcm9tICcuL3BhcnRzLmpzJztcbmltcG9ydCB7IHRlbXBsYXRlRmFjdG9yeSB9IGZyb20gJy4vdGVtcGxhdGUtZmFjdG9yeS5qcyc7XG5leHBvcnQgY29uc3QgcGFydHMgPSBuZXcgV2Vha01hcCgpO1xuLyoqXG4gKiBSZW5kZXJzIGEgdGVtcGxhdGUgdG8gYSBjb250YWluZXIuXG4gKlxuICogVG8gdXBkYXRlIGEgY29udGFpbmVyIHdpdGggbmV3IHZhbHVlcywgcmVldmFsdWF0ZSB0aGUgdGVtcGxhdGUgbGl0ZXJhbCBhbmRcbiAqIGNhbGwgYHJlbmRlcmAgd2l0aCB0aGUgbmV3IHJlc3VsdC5cbiAqXG4gKiBAcGFyYW0gcmVzdWx0IGEgVGVtcGxhdGVSZXN1bHQgY3JlYXRlZCBieSBldmFsdWF0aW5nIGEgdGVtcGxhdGUgdGFnIGxpa2VcbiAqICAgICBgaHRtbGAgb3IgYHN2Z2AuXG4gKiBAcGFyYW0gY29udGFpbmVyIEEgRE9NIHBhcmVudCB0byByZW5kZXIgdG8uIFRoZSBlbnRpcmUgY29udGVudHMgYXJlIGVpdGhlclxuICogICAgIHJlcGxhY2VkLCBvciBlZmZpY2llbnRseSB1cGRhdGVkIGlmIHRoZSBzYW1lIHJlc3VsdCB0eXBlIHdhcyBwcmV2aW91c1xuICogICAgIHJlbmRlcmVkIHRoZXJlLlxuICogQHBhcmFtIG9wdGlvbnMgUmVuZGVyT3B0aW9ucyBmb3IgdGhlIGVudGlyZSByZW5kZXIgdHJlZSByZW5kZXJlZCB0byB0aGlzXG4gKiAgICAgY29udGFpbmVyLiBSZW5kZXIgb3B0aW9ucyBtdXN0ICpub3QqIGNoYW5nZSBiZXR3ZWVuIHJlbmRlcnMgdG8gdGhlIHNhbWVcbiAqICAgICBjb250YWluZXIsIGFzIHRob3NlIGNoYW5nZXMgd2lsbCBub3QgZWZmZWN0IHByZXZpb3VzbHkgcmVuZGVyZWQgRE9NLlxuICovXG5leHBvcnQgY29uc3QgcmVuZGVyID0gKHJlc3VsdCwgY29udGFpbmVyLCBvcHRpb25zKSA9PiB7XG4gICAgbGV0IHBhcnQgPSBwYXJ0cy5nZXQoY29udGFpbmVyKTtcbiAgICBpZiAocGFydCA9PT0gdW5kZWZpbmVkKSB7XG4gICAgICAgIHJlbW92ZU5vZGVzKGNvbnRhaW5lciwgY29udGFpbmVyLmZpcnN0Q2hpbGQpO1xuICAgICAgICBwYXJ0cy5zZXQoY29udGFpbmVyLCBwYXJ0ID0gbmV3IE5vZGVQYXJ0KE9iamVjdC5hc3NpZ24oeyB0ZW1wbGF0ZUZhY3RvcnkgfSwgb3B0aW9ucykpKTtcbiAgICAgICAgcGFydC5hcHBlbmRJbnRvKGNvbnRhaW5lcik7XG4gICAgfVxuICAgIHBhcnQuc2V0VmFsdWUocmVzdWx0KTtcbiAgICBwYXJ0LmNvbW1pdCgpO1xufTtcbi8vIyBzb3VyY2VNYXBwaW5nVVJMPXJlbmRlci5qcy5tYXAiLCIvKipcbiAqIEBsaWNlbnNlXG4gKiBDb3B5cmlnaHQgKGMpIDIwMTcgVGhlIFBvbHltZXIgUHJvamVjdCBBdXRob3JzLiBBbGwgcmlnaHRzIHJlc2VydmVkLlxuICogVGhpcyBjb2RlIG1heSBvbmx5IGJlIHVzZWQgdW5kZXIgdGhlIEJTRCBzdHlsZSBsaWNlbnNlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vTElDRU5TRS50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgYXV0aG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9BVVRIT1JTLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBjb250cmlidXRvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQ09OVFJJQlVUT1JTLnR4dFxuICogQ29kZSBkaXN0cmlidXRlZCBieSBHb29nbGUgYXMgcGFydCBvZiB0aGUgcG9seW1lciBwcm9qZWN0IGlzIGFsc29cbiAqIHN1YmplY3QgdG8gYW4gYWRkaXRpb25hbCBJUCByaWdodHMgZ3JhbnQgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9QQVRFTlRTLnR4dFxuICovXG4vKipcbiAqIE1vZHVsZSB0byBhZGQgc2hhZHkgRE9NL3NoYWR5IENTUyBwb2x5ZmlsbCBzdXBwb3J0IHRvIGxpdC1odG1sIHRlbXBsYXRlXG4gKiByZW5kZXJpbmcuIFNlZSB0aGUgW1tyZW5kZXJdXSBtZXRob2QgZm9yIGRldGFpbHMuXG4gKlxuICogQG1vZHVsZSBzaGFkeS1yZW5kZXJcbiAqIEBwcmVmZXJyZWRcbiAqL1xuLyoqXG4gKiBEbyBub3QgcmVtb3ZlIHRoaXMgY29tbWVudDsgaXQga2VlcHMgdHlwZWRvYyBmcm9tIG1pc3BsYWNpbmcgdGhlIG1vZHVsZVxuICogZG9jcy5cbiAqL1xuaW1wb3J0IHsgcmVtb3ZlTm9kZXMgfSBmcm9tICcuL2RvbS5qcyc7XG5pbXBvcnQgeyBpbnNlcnROb2RlSW50b1RlbXBsYXRlLCByZW1vdmVOb2Rlc0Zyb21UZW1wbGF0ZSB9IGZyb20gJy4vbW9kaWZ5LXRlbXBsYXRlLmpzJztcbmltcG9ydCB7IHBhcnRzLCByZW5kZXIgYXMgbGl0UmVuZGVyIH0gZnJvbSAnLi9yZW5kZXIuanMnO1xuaW1wb3J0IHsgdGVtcGxhdGVDYWNoZXMgfSBmcm9tICcuL3RlbXBsYXRlLWZhY3RvcnkuanMnO1xuaW1wb3J0IHsgVGVtcGxhdGVJbnN0YW5jZSB9IGZyb20gJy4vdGVtcGxhdGUtaW5zdGFuY2UuanMnO1xuaW1wb3J0IHsgVGVtcGxhdGVSZXN1bHQgfSBmcm9tICcuL3RlbXBsYXRlLXJlc3VsdC5qcyc7XG5pbXBvcnQgeyBtYXJrZXIsIFRlbXBsYXRlIH0gZnJvbSAnLi90ZW1wbGF0ZS5qcyc7XG5leHBvcnQgeyBodG1sLCBzdmcsIFRlbXBsYXRlUmVzdWx0IH0gZnJvbSAnLi4vbGl0LWh0bWwuanMnO1xuLy8gR2V0IGEga2V5IHRvIGxvb2t1cCBpbiBgdGVtcGxhdGVDYWNoZXNgLlxuY29uc3QgZ2V0VGVtcGxhdGVDYWNoZUtleSA9ICh0eXBlLCBzY29wZU5hbWUpID0+IGAke3R5cGV9LS0ke3Njb3BlTmFtZX1gO1xubGV0IGNvbXBhdGlibGVTaGFkeUNTU1ZlcnNpb24gPSB0cnVlO1xuaWYgKHR5cGVvZiB3aW5kb3cuU2hhZHlDU1MgPT09ICd1bmRlZmluZWQnKSB7XG4gICAgY29tcGF0aWJsZVNoYWR5Q1NTVmVyc2lvbiA9IGZhbHNlO1xufVxuZWxzZSBpZiAodHlwZW9mIHdpbmRvdy5TaGFkeUNTUy5wcmVwYXJlVGVtcGxhdGVEb20gPT09ICd1bmRlZmluZWQnKSB7XG4gICAgY29uc29sZS53YXJuKGBJbmNvbXBhdGlibGUgU2hhZHlDU1MgdmVyc2lvbiBkZXRlY3RlZC5gICtcbiAgICAgICAgYFBsZWFzZSB1cGRhdGUgdG8gYXQgbGVhc3QgQHdlYmNvbXBvbmVudHMvd2ViY29tcG9uZW50c2pzQDIuMC4yIGFuZGAgK1xuICAgICAgICBgQHdlYmNvbXBvbmVudHMvc2hhZHljc3NAMS4zLjEuYCk7XG4gICAgY29tcGF0aWJsZVNoYWR5Q1NTVmVyc2lvbiA9IGZhbHNlO1xufVxuLyoqXG4gKiBUZW1wbGF0ZSBmYWN0b3J5IHdoaWNoIHNjb3BlcyB0ZW1wbGF0ZSBET00gdXNpbmcgU2hhZHlDU1MuXG4gKiBAcGFyYW0gc2NvcGVOYW1lIHtzdHJpbmd9XG4gKi9cbmNvbnN0IHNoYWR5VGVtcGxhdGVGYWN0b3J5ID0gKHNjb3BlTmFtZSkgPT4gKHJlc3VsdCkgPT4ge1xuICAgIGNvbnN0IGNhY2hlS2V5ID0gZ2V0VGVtcGxhdGVDYWNoZUtleShyZXN1bHQudHlwZSwgc2NvcGVOYW1lKTtcbiAgICBsZXQgdGVtcGxhdGVDYWNoZSA9IHRlbXBsYXRlQ2FjaGVzLmdldChjYWNoZUtleSk7XG4gICAgaWYgKHRlbXBsYXRlQ2FjaGUgPT09IHVuZGVmaW5lZCkge1xuICAgICAgICB0ZW1wbGF0ZUNhY2hlID0ge1xuICAgICAgICAgICAgc3RyaW5nc0FycmF5OiBuZXcgV2Vha01hcCgpLFxuICAgICAgICAgICAga2V5U3RyaW5nOiBuZXcgTWFwKClcbiAgICAgICAgfTtcbiAgICAgICAgdGVtcGxhdGVDYWNoZXMuc2V0KGNhY2hlS2V5LCB0ZW1wbGF0ZUNhY2hlKTtcbiAgICB9XG4gICAgbGV0IHRlbXBsYXRlID0gdGVtcGxhdGVDYWNoZS5zdHJpbmdzQXJyYXkuZ2V0KHJlc3VsdC5zdHJpbmdzKTtcbiAgICBpZiAodGVtcGxhdGUgIT09IHVuZGVmaW5lZCkge1xuICAgICAgICByZXR1cm4gdGVtcGxhdGU7XG4gICAgfVxuICAgIGNvbnN0IGtleSA9IHJlc3VsdC5zdHJpbmdzLmpvaW4obWFya2VyKTtcbiAgICB0ZW1wbGF0ZSA9IHRlbXBsYXRlQ2FjaGUua2V5U3RyaW5nLmdldChrZXkpO1xuICAgIGlmICh0ZW1wbGF0ZSA9PT0gdW5kZWZpbmVkKSB7XG4gICAgICAgIGNvbnN0IGVsZW1lbnQgPSByZXN1bHQuZ2V0VGVtcGxhdGVFbGVtZW50KCk7XG4gICAgICAgIGlmIChjb21wYXRpYmxlU2hhZHlDU1NWZXJzaW9uKSB7XG4gICAgICAgICAgICB3aW5kb3cuU2hhZHlDU1MucHJlcGFyZVRlbXBsYXRlRG9tKGVsZW1lbnQsIHNjb3BlTmFtZSk7XG4gICAgICAgIH1cbiAgICAgICAgdGVtcGxhdGUgPSBuZXcgVGVtcGxhdGUocmVzdWx0LCBlbGVtZW50KTtcbiAgICAgICAgdGVtcGxhdGVDYWNoZS5rZXlTdHJpbmcuc2V0KGtleSwgdGVtcGxhdGUpO1xuICAgIH1cbiAgICB0ZW1wbGF0ZUNhY2hlLnN0cmluZ3NBcnJheS5zZXQocmVzdWx0LnN0cmluZ3MsIHRlbXBsYXRlKTtcbiAgICByZXR1cm4gdGVtcGxhdGU7XG59O1xuY29uc3QgVEVNUExBVEVfVFlQRVMgPSBbJ2h0bWwnLCAnc3ZnJ107XG4vKipcbiAqIFJlbW92ZXMgYWxsIHN0eWxlIGVsZW1lbnRzIGZyb20gVGVtcGxhdGVzIGZvciB0aGUgZ2l2ZW4gc2NvcGVOYW1lLlxuICovXG5jb25zdCByZW1vdmVTdHlsZXNGcm9tTGl0VGVtcGxhdGVzID0gKHNjb3BlTmFtZSkgPT4ge1xuICAgIFRFTVBMQVRFX1RZUEVTLmZvckVhY2goKHR5cGUpID0+IHtcbiAgICAgICAgY29uc3QgdGVtcGxhdGVzID0gdGVtcGxhdGVDYWNoZXMuZ2V0KGdldFRlbXBsYXRlQ2FjaGVLZXkodHlwZSwgc2NvcGVOYW1lKSk7XG4gICAgICAgIGlmICh0ZW1wbGF0ZXMgIT09IHVuZGVmaW5lZCkge1xuICAgICAgICAgICAgdGVtcGxhdGVzLmtleVN0cmluZy5mb3JFYWNoKCh0ZW1wbGF0ZSkgPT4ge1xuICAgICAgICAgICAgICAgIGNvbnN0IHsgZWxlbWVudDogeyBjb250ZW50IH0gfSA9IHRlbXBsYXRlO1xuICAgICAgICAgICAgICAgIC8vIElFIDExIGRvZXNuJ3Qgc3VwcG9ydCB0aGUgaXRlcmFibGUgcGFyYW0gU2V0IGNvbnN0cnVjdG9yXG4gICAgICAgICAgICAgICAgY29uc3Qgc3R5bGVzID0gbmV3IFNldCgpO1xuICAgICAgICAgICAgICAgIEFycmF5LmZyb20oY29udGVudC5xdWVyeVNlbGVjdG9yQWxsKCdzdHlsZScpKS5mb3JFYWNoKChzKSA9PiB7XG4gICAgICAgICAgICAgICAgICAgIHN0eWxlcy5hZGQocyk7XG4gICAgICAgICAgICAgICAgfSk7XG4gICAgICAgICAgICAgICAgcmVtb3ZlTm9kZXNGcm9tVGVtcGxhdGUodGVtcGxhdGUsIHN0eWxlcyk7XG4gICAgICAgICAgICB9KTtcbiAgICAgICAgfVxuICAgIH0pO1xufTtcbmNvbnN0IHNoYWR5UmVuZGVyU2V0ID0gbmV3IFNldCgpO1xuLyoqXG4gKiBGb3IgdGhlIGdpdmVuIHNjb3BlIG5hbWUsIGVuc3VyZXMgdGhhdCBTaGFkeUNTUyBzdHlsZSBzY29waW5nIGlzIHBlcmZvcm1lZC5cbiAqIFRoaXMgaXMgZG9uZSBqdXN0IG9uY2UgcGVyIHNjb3BlIG5hbWUgc28gdGhlIGZyYWdtZW50IGFuZCB0ZW1wbGF0ZSBjYW5ub3RcbiAqIGJlIG1vZGlmaWVkLlxuICogKDEpIGV4dHJhY3RzIHN0eWxlcyBmcm9tIHRoZSByZW5kZXJlZCBmcmFnbWVudCBhbmQgaGFuZHMgdGhlbSB0byBTaGFkeUNTU1xuICogdG8gYmUgc2NvcGVkIGFuZCBhcHBlbmRlZCB0byB0aGUgZG9jdW1lbnRcbiAqICgyKSByZW1vdmVzIHN0eWxlIGVsZW1lbnRzIGZyb20gYWxsIGxpdC1odG1sIFRlbXBsYXRlcyBmb3IgdGhpcyBzY29wZSBuYW1lLlxuICpcbiAqIE5vdGUsIDxzdHlsZT4gZWxlbWVudHMgY2FuIG9ubHkgYmUgcGxhY2VkIGludG8gdGVtcGxhdGVzIGZvciB0aGVcbiAqIGluaXRpYWwgcmVuZGVyaW5nIG9mIHRoZSBzY29wZS4gSWYgPHN0eWxlPiBlbGVtZW50cyBhcmUgaW5jbHVkZWQgaW4gdGVtcGxhdGVzXG4gKiBkeW5hbWljYWxseSByZW5kZXJlZCB0byB0aGUgc2NvcGUgKGFmdGVyIHRoZSBmaXJzdCBzY29wZSByZW5kZXIpLCB0aGV5IHdpbGxcbiAqIG5vdCBiZSBzY29wZWQgYW5kIHRoZSA8c3R5bGU+IHdpbGwgYmUgbGVmdCBpbiB0aGUgdGVtcGxhdGUgYW5kIHJlbmRlcmVkXG4gKiBvdXRwdXQuXG4gKi9cbmNvbnN0IHByZXBhcmVUZW1wbGF0ZVN0eWxlcyA9IChyZW5kZXJlZERPTSwgdGVtcGxhdGUsIHNjb3BlTmFtZSkgPT4ge1xuICAgIHNoYWR5UmVuZGVyU2V0LmFkZChzY29wZU5hbWUpO1xuICAgIC8vIE1vdmUgc3R5bGVzIG91dCBvZiByZW5kZXJlZCBET00gYW5kIHN0b3JlLlxuICAgIGNvbnN0IHN0eWxlcyA9IHJlbmRlcmVkRE9NLnF1ZXJ5U2VsZWN0b3JBbGwoJ3N0eWxlJyk7XG4gICAgLy8gSWYgdGhlcmUgYXJlIG5vIHN0eWxlcywgc2tpcCB1bm5lY2Vzc2FyeSB3b3JrXG4gICAgaWYgKHN0eWxlcy5sZW5ndGggPT09IDApIHtcbiAgICAgICAgLy8gRW5zdXJlIHByZXBhcmVUZW1wbGF0ZVN0eWxlcyBpcyBjYWxsZWQgdG8gc3VwcG9ydCBhZGRpbmdcbiAgICAgICAgLy8gc3R5bGVzIHZpYSBgcHJlcGFyZUFkb3B0ZWRDc3NUZXh0YCBzaW5jZSB0aGF0IHJlcXVpcmVzIHRoYXRcbiAgICAgICAgLy8gYHByZXBhcmVUZW1wbGF0ZVN0eWxlc2AgaXMgY2FsbGVkLlxuICAgICAgICB3aW5kb3cuU2hhZHlDU1MucHJlcGFyZVRlbXBsYXRlU3R5bGVzKHRlbXBsYXRlLmVsZW1lbnQsIHNjb3BlTmFtZSk7XG4gICAgICAgIHJldHVybjtcbiAgICB9XG4gICAgY29uc3QgY29uZGVuc2VkU3R5bGUgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KCdzdHlsZScpO1xuICAgIC8vIENvbGxlY3Qgc3R5bGVzIGludG8gYSBzaW5nbGUgc3R5bGUuIFRoaXMgaGVscHMgdXMgbWFrZSBzdXJlIFNoYWR5Q1NTXG4gICAgLy8gbWFuaXB1bGF0aW9ucyB3aWxsIG5vdCBwcmV2ZW50IHVzIGZyb20gYmVpbmcgYWJsZSB0byBmaXggdXAgdGVtcGxhdGVcbiAgICAvLyBwYXJ0IGluZGljZXMuXG4gICAgLy8gTk9URTogY29sbGVjdGluZyBzdHlsZXMgaXMgaW5lZmZpY2llbnQgZm9yIGJyb3dzZXJzIGJ1dCBTaGFkeUNTU1xuICAgIC8vIGN1cnJlbnRseSBkb2VzIHRoaXMgYW55d2F5LiBXaGVuIGl0IGRvZXMgbm90LCB0aGlzIHNob3VsZCBiZSBjaGFuZ2VkLlxuICAgIGZvciAobGV0IGkgPSAwOyBpIDwgc3R5bGVzLmxlbmd0aDsgaSsrKSB7XG4gICAgICAgIGNvbnN0IHN0eWxlID0gc3R5bGVzW2ldO1xuICAgICAgICBzdHlsZS5wYXJlbnROb2RlLnJlbW92ZUNoaWxkKHN0eWxlKTtcbiAgICAgICAgY29uZGVuc2VkU3R5bGUudGV4dENvbnRlbnQgKz0gc3R5bGUudGV4dENvbnRlbnQ7XG4gICAgfVxuICAgIC8vIFJlbW92ZSBzdHlsZXMgZnJvbSBuZXN0ZWQgdGVtcGxhdGVzIGluIHRoaXMgc2NvcGUuXG4gICAgcmVtb3ZlU3R5bGVzRnJvbUxpdFRlbXBsYXRlcyhzY29wZU5hbWUpO1xuICAgIC8vIEFuZCB0aGVuIHB1dCB0aGUgY29uZGVuc2VkIHN0eWxlIGludG8gdGhlIFwicm9vdFwiIHRlbXBsYXRlIHBhc3NlZCBpbiBhc1xuICAgIC8vIGB0ZW1wbGF0ZWAuXG4gICAgaW5zZXJ0Tm9kZUludG9UZW1wbGF0ZSh0ZW1wbGF0ZSwgY29uZGVuc2VkU3R5bGUsIHRlbXBsYXRlLmVsZW1lbnQuY29udGVudC5maXJzdENoaWxkKTtcbiAgICAvLyBOb3RlLCBpdCdzIGltcG9ydGFudCB0aGF0IFNoYWR5Q1NTIGdldHMgdGhlIHRlbXBsYXRlIHRoYXQgYGxpdC1odG1sYFxuICAgIC8vIHdpbGwgYWN0dWFsbHkgcmVuZGVyIHNvIHRoYXQgaXQgY2FuIHVwZGF0ZSB0aGUgc3R5bGUgaW5zaWRlIHdoZW5cbiAgICAvLyBuZWVkZWQgKGUuZy4gQGFwcGx5IG5hdGl2ZSBTaGFkb3cgRE9NIGNhc2UpLlxuICAgIHdpbmRvdy5TaGFkeUNTUy5wcmVwYXJlVGVtcGxhdGVTdHlsZXModGVtcGxhdGUuZWxlbWVudCwgc2NvcGVOYW1lKTtcbiAgICBpZiAod2luZG93LlNoYWR5Q1NTLm5hdGl2ZVNoYWRvdykge1xuICAgICAgICAvLyBXaGVuIGluIG5hdGl2ZSBTaGFkb3cgRE9NLCByZS1hZGQgc3R5bGluZyB0byByZW5kZXJlZCBjb250ZW50IHVzaW5nXG4gICAgICAgIC8vIHRoZSBzdHlsZSBTaGFkeUNTUyBwcm9kdWNlZC5cbiAgICAgICAgY29uc3Qgc3R5bGUgPSB0ZW1wbGF0ZS5lbGVtZW50LmNvbnRlbnQucXVlcnlTZWxlY3Rvcignc3R5bGUnKTtcbiAgICAgICAgcmVuZGVyZWRET00uaW5zZXJ0QmVmb3JlKHN0eWxlLmNsb25lTm9kZSh0cnVlKSwgcmVuZGVyZWRET00uZmlyc3RDaGlsZCk7XG4gICAgfVxuICAgIGVsc2Uge1xuICAgICAgICAvLyBXaGVuIG5vdCBpbiBuYXRpdmUgU2hhZG93IERPTSwgYXQgdGhpcyBwb2ludCBTaGFkeUNTUyB3aWxsIGhhdmVcbiAgICAgICAgLy8gcmVtb3ZlZCB0aGUgc3R5bGUgZnJvbSB0aGUgbGl0IHRlbXBsYXRlIGFuZCBwYXJ0cyB3aWxsIGJlIGJyb2tlbiBhcyBhXG4gICAgICAgIC8vIHJlc3VsdC4gVG8gZml4IHRoaXMsIHdlIHB1dCBiYWNrIHRoZSBzdHlsZSBub2RlIFNoYWR5Q1NTIHJlbW92ZWRcbiAgICAgICAgLy8gYW5kIHRoZW4gdGVsbCBsaXQgdG8gcmVtb3ZlIHRoYXQgbm9kZSBmcm9tIHRoZSB0ZW1wbGF0ZS5cbiAgICAgICAgLy8gTk9URSwgU2hhZHlDU1MgY3JlYXRlcyBpdHMgb3duIHN0eWxlIHNvIHdlIGNhbiBzYWZlbHkgYWRkL3JlbW92ZVxuICAgICAgICAvLyBgY29uZGVuc2VkU3R5bGVgIGhlcmUuXG4gICAgICAgIHRlbXBsYXRlLmVsZW1lbnQuY29udGVudC5pbnNlcnRCZWZvcmUoY29uZGVuc2VkU3R5bGUsIHRlbXBsYXRlLmVsZW1lbnQuY29udGVudC5maXJzdENoaWxkKTtcbiAgICAgICAgY29uc3QgcmVtb3ZlcyA9IG5ldyBTZXQoKTtcbiAgICAgICAgcmVtb3Zlcy5hZGQoY29uZGVuc2VkU3R5bGUpO1xuICAgICAgICByZW1vdmVOb2Rlc0Zyb21UZW1wbGF0ZSh0ZW1wbGF0ZSwgcmVtb3Zlcyk7XG4gICAgfVxufTtcbi8qKlxuICogRXh0ZW5zaW9uIHRvIHRoZSBzdGFuZGFyZCBgcmVuZGVyYCBtZXRob2Qgd2hpY2ggc3VwcG9ydHMgcmVuZGVyaW5nXG4gKiB0byBTaGFkb3dSb290cyB3aGVuIHRoZSBTaGFkeURPTSAoaHR0cHM6Ly9naXRodWIuY29tL3dlYmNvbXBvbmVudHMvc2hhZHlkb20pXG4gKiBhbmQgU2hhZHlDU1MgKGh0dHBzOi8vZ2l0aHViLmNvbS93ZWJjb21wb25lbnRzL3NoYWR5Y3NzKSBwb2x5ZmlsbHMgYXJlIHVzZWRcbiAqIG9yIHdoZW4gdGhlIHdlYmNvbXBvbmVudHNqc1xuICogKGh0dHBzOi8vZ2l0aHViLmNvbS93ZWJjb21wb25lbnRzL3dlYmNvbXBvbmVudHNqcykgcG9seWZpbGwgaXMgdXNlZC5cbiAqXG4gKiBBZGRzIGEgYHNjb3BlTmFtZWAgb3B0aW9uIHdoaWNoIGlzIHVzZWQgdG8gc2NvcGUgZWxlbWVudCBET00gYW5kIHN0eWxlc2hlZXRzXG4gKiB3aGVuIG5hdGl2ZSBTaGFkb3dET00gaXMgdW5hdmFpbGFibGUuIFRoZSBgc2NvcGVOYW1lYCB3aWxsIGJlIGFkZGVkIHRvXG4gKiB0aGUgY2xhc3MgYXR0cmlidXRlIG9mIGFsbCByZW5kZXJlZCBET00uIEluIGFkZGl0aW9uLCBhbnkgc3R5bGUgZWxlbWVudHMgd2lsbFxuICogYmUgYXV0b21hdGljYWxseSByZS13cml0dGVuIHdpdGggdGhpcyBgc2NvcGVOYW1lYCBzZWxlY3RvciBhbmQgbW92ZWQgb3V0XG4gKiBvZiB0aGUgcmVuZGVyZWQgRE9NIGFuZCBpbnRvIHRoZSBkb2N1bWVudCBgPGhlYWQ+YC5cbiAqXG4gKiBJdCBpcyBjb21tb24gdG8gdXNlIHRoaXMgcmVuZGVyIG1ldGhvZCBpbiBjb25qdW5jdGlvbiB3aXRoIGEgY3VzdG9tIGVsZW1lbnRcbiAqIHdoaWNoIHJlbmRlcnMgYSBzaGFkb3dSb290LiBXaGVuIHRoaXMgaXMgZG9uZSwgdHlwaWNhbGx5IHRoZSBlbGVtZW50J3NcbiAqIGBsb2NhbE5hbWVgIHNob3VsZCBiZSB1c2VkIGFzIHRoZSBgc2NvcGVOYW1lYC5cbiAqXG4gKiBJbiBhZGRpdGlvbiB0byBET00gc2NvcGluZywgU2hhZHlDU1MgYWxzbyBzdXBwb3J0cyBhIGJhc2ljIHNoaW0gZm9yIGNzc1xuICogY3VzdG9tIHByb3BlcnRpZXMgKG5lZWRlZCBvbmx5IG9uIG9sZGVyIGJyb3dzZXJzIGxpa2UgSUUxMSkgYW5kIGEgc2hpbSBmb3JcbiAqIGEgZGVwcmVjYXRlZCBmZWF0dXJlIGNhbGxlZCBgQGFwcGx5YCB0aGF0IHN1cHBvcnRzIGFwcGx5aW5nIGEgc2V0IG9mIGNzc1xuICogY3VzdG9tIHByb3BlcnRpZXMgdG8gYSBnaXZlbiBsb2NhdGlvbi5cbiAqXG4gKiBVc2FnZSBjb25zaWRlcmF0aW9uczpcbiAqXG4gKiAqIFBhcnQgdmFsdWVzIGluIGA8c3R5bGU+YCBlbGVtZW50cyBhcmUgb25seSBhcHBsaWVkIHRoZSBmaXJzdCB0aW1lIGEgZ2l2ZW5cbiAqIGBzY29wZU5hbWVgIHJlbmRlcnMuIFN1YnNlcXVlbnQgY2hhbmdlcyB0byBwYXJ0cyBpbiBzdHlsZSBlbGVtZW50cyB3aWxsIGhhdmVcbiAqIG5vIGVmZmVjdC4gQmVjYXVzZSBvZiB0aGlzLCBwYXJ0cyBpbiBzdHlsZSBlbGVtZW50cyBzaG91bGQgb25seSBiZSB1c2VkIGZvclxuICogdmFsdWVzIHRoYXQgd2lsbCBuZXZlciBjaGFuZ2UsIGZvciBleGFtcGxlIHBhcnRzIHRoYXQgc2V0IHNjb3BlLXdpZGUgdGhlbWVcbiAqIHZhbHVlcyBvciBwYXJ0cyB3aGljaCByZW5kZXIgc2hhcmVkIHN0eWxlIGVsZW1lbnRzLlxuICpcbiAqICogTm90ZSwgZHVlIHRvIGEgbGltaXRhdGlvbiBvZiB0aGUgU2hhZHlET00gcG9seWZpbGwsIHJlbmRlcmluZyBpbiBhXG4gKiBjdXN0b20gZWxlbWVudCdzIGBjb25zdHJ1Y3RvcmAgaXMgbm90IHN1cHBvcnRlZC4gSW5zdGVhZCByZW5kZXJpbmcgc2hvdWxkXG4gKiBlaXRoZXIgZG9uZSBhc3luY2hyb25vdXNseSwgZm9yIGV4YW1wbGUgYXQgbWljcm90YXNrIHRpbWluZyAoZm9yIGV4YW1wbGVcbiAqIGBQcm9taXNlLnJlc29sdmUoKWApLCBvciBiZSBkZWZlcnJlZCB1bnRpbCB0aGUgZmlyc3QgdGltZSB0aGUgZWxlbWVudCdzXG4gKiBgY29ubmVjdGVkQ2FsbGJhY2tgIHJ1bnMuXG4gKlxuICogVXNhZ2UgY29uc2lkZXJhdGlvbnMgd2hlbiB1c2luZyBzaGltbWVkIGN1c3RvbSBwcm9wZXJ0aWVzIG9yIGBAYXBwbHlgOlxuICpcbiAqICogV2hlbmV2ZXIgYW55IGR5bmFtaWMgY2hhbmdlcyBhcmUgbWFkZSB3aGljaCBhZmZlY3RcbiAqIGNzcyBjdXN0b20gcHJvcGVydGllcywgYFNoYWR5Q1NTLnN0eWxlRWxlbWVudChlbGVtZW50KWAgbXVzdCBiZSBjYWxsZWRcbiAqIHRvIHVwZGF0ZSB0aGUgZWxlbWVudC4gVGhlcmUgYXJlIHR3byBjYXNlcyB3aGVuIHRoaXMgaXMgbmVlZGVkOlxuICogKDEpIHRoZSBlbGVtZW50IGlzIGNvbm5lY3RlZCB0byBhIG5ldyBwYXJlbnQsICgyKSBhIGNsYXNzIGlzIGFkZGVkIHRvIHRoZVxuICogZWxlbWVudCB0aGF0IGNhdXNlcyBpdCB0byBtYXRjaCBkaWZmZXJlbnQgY3VzdG9tIHByb3BlcnRpZXMuXG4gKiBUbyBhZGRyZXNzIHRoZSBmaXJzdCBjYXNlIHdoZW4gcmVuZGVyaW5nIGEgY3VzdG9tIGVsZW1lbnQsIGBzdHlsZUVsZW1lbnRgXG4gKiBzaG91bGQgYmUgY2FsbGVkIGluIHRoZSBlbGVtZW50J3MgYGNvbm5lY3RlZENhbGxiYWNrYC5cbiAqXG4gKiAqIFNoaW1tZWQgY3VzdG9tIHByb3BlcnRpZXMgbWF5IG9ubHkgYmUgZGVmaW5lZCBlaXRoZXIgZm9yIGFuIGVudGlyZVxuICogc2hhZG93Um9vdCAoZm9yIGV4YW1wbGUsIGluIGEgYDpob3N0YCBydWxlKSBvciB2aWEgYSBydWxlIHRoYXQgZGlyZWN0bHlcbiAqIG1hdGNoZXMgYW4gZWxlbWVudCB3aXRoIGEgc2hhZG93Um9vdC4gSW4gb3RoZXIgd29yZHMsIGluc3RlYWQgb2YgZmxvd2luZyBmcm9tXG4gKiBwYXJlbnQgdG8gY2hpbGQgYXMgZG8gbmF0aXZlIGNzcyBjdXN0b20gcHJvcGVydGllcywgc2hpbW1lZCBjdXN0b20gcHJvcGVydGllc1xuICogZmxvdyBvbmx5IGZyb20gc2hhZG93Um9vdHMgdG8gbmVzdGVkIHNoYWRvd1Jvb3RzLlxuICpcbiAqICogV2hlbiB1c2luZyBgQGFwcGx5YCBtaXhpbmcgY3NzIHNob3J0aGFuZCBwcm9wZXJ0eSBuYW1lcyB3aXRoXG4gKiBub24tc2hvcnRoYW5kIG5hbWVzIChmb3IgZXhhbXBsZSBgYm9yZGVyYCBhbmQgYGJvcmRlci13aWR0aGApIGlzIG5vdFxuICogc3VwcG9ydGVkLlxuICovXG5leHBvcnQgY29uc3QgcmVuZGVyID0gKHJlc3VsdCwgY29udGFpbmVyLCBvcHRpb25zKSA9PiB7XG4gICAgY29uc3Qgc2NvcGVOYW1lID0gb3B0aW9ucy5zY29wZU5hbWU7XG4gICAgY29uc3QgaGFzUmVuZGVyZWQgPSBwYXJ0cy5oYXMoY29udGFpbmVyKTtcbiAgICBjb25zdCBuZWVkc1Njb3BpbmcgPSBjb250YWluZXIgaW5zdGFuY2VvZiBTaGFkb3dSb290ICYmXG4gICAgICAgIGNvbXBhdGlibGVTaGFkeUNTU1ZlcnNpb24gJiYgcmVzdWx0IGluc3RhbmNlb2YgVGVtcGxhdGVSZXN1bHQ7XG4gICAgLy8gSGFuZGxlIGZpcnN0IHJlbmRlciB0byBhIHNjb3BlIHNwZWNpYWxseS4uLlxuICAgIGNvbnN0IGZpcnN0U2NvcGVSZW5kZXIgPSBuZWVkc1Njb3BpbmcgJiYgIXNoYWR5UmVuZGVyU2V0LmhhcyhzY29wZU5hbWUpO1xuICAgIC8vIE9uIGZpcnN0IHNjb3BlIHJlbmRlciwgcmVuZGVyIGludG8gYSBmcmFnbWVudDsgdGhpcyBjYW5ub3QgYmUgYSBzaW5nbGVcbiAgICAvLyBmcmFnbWVudCB0aGF0IGlzIHJldXNlZCBzaW5jZSBuZXN0ZWQgcmVuZGVycyBjYW4gb2NjdXIgc3luY2hyb25vdXNseS5cbiAgICBjb25zdCByZW5kZXJDb250YWluZXIgPSBmaXJzdFNjb3BlUmVuZGVyID8gZG9jdW1lbnQuY3JlYXRlRG9jdW1lbnRGcmFnbWVudCgpIDogY29udGFpbmVyO1xuICAgIGxpdFJlbmRlcihyZXN1bHQsIHJlbmRlckNvbnRhaW5lciwgT2JqZWN0LmFzc2lnbih7IHRlbXBsYXRlRmFjdG9yeTogc2hhZHlUZW1wbGF0ZUZhY3Rvcnkoc2NvcGVOYW1lKSB9LCBvcHRpb25zKSk7XG4gICAgLy8gV2hlbiBwZXJmb3JtaW5nIGZpcnN0IHNjb3BlIHJlbmRlcixcbiAgICAvLyAoMSkgV2UndmUgcmVuZGVyZWQgaW50byBhIGZyYWdtZW50IHNvIHRoYXQgdGhlcmUncyBhIGNoYW5jZSB0b1xuICAgIC8vIGBwcmVwYXJlVGVtcGxhdGVTdHlsZXNgIGJlZm9yZSBzdWItZWxlbWVudHMgaGl0IHRoZSBET01cbiAgICAvLyAod2hpY2ggbWlnaHQgY2F1c2UgdGhlbSB0byByZW5kZXIgYmFzZWQgb24gYSBjb21tb24gcGF0dGVybiBvZlxuICAgIC8vIHJlbmRlcmluZyBpbiBhIGN1c3RvbSBlbGVtZW50J3MgYGNvbm5lY3RlZENhbGxiYWNrYCk7XG4gICAgLy8gKDIpIFNjb3BlIHRoZSB0ZW1wbGF0ZSB3aXRoIFNoYWR5Q1NTIG9uZSB0aW1lIG9ubHkgZm9yIHRoaXMgc2NvcGUuXG4gICAgLy8gKDMpIFJlbmRlciB0aGUgZnJhZ21lbnQgaW50byB0aGUgY29udGFpbmVyIGFuZCBtYWtlIHN1cmUgdGhlXG4gICAgLy8gY29udGFpbmVyIGtub3dzIGl0cyBgcGFydGAgaXMgdGhlIG9uZSB3ZSBqdXN0IHJlbmRlcmVkLiBUaGlzIGVuc3VyZXNcbiAgICAvLyBET00gd2lsbCBiZSByZS11c2VkIG9uIHN1YnNlcXVlbnQgcmVuZGVycy5cbiAgICBpZiAoZmlyc3RTY29wZVJlbmRlcikge1xuICAgICAgICBjb25zdCBwYXJ0ID0gcGFydHMuZ2V0KHJlbmRlckNvbnRhaW5lcik7XG4gICAgICAgIHBhcnRzLmRlbGV0ZShyZW5kZXJDb250YWluZXIpO1xuICAgICAgICBpZiAocGFydC52YWx1ZSBpbnN0YW5jZW9mIFRlbXBsYXRlSW5zdGFuY2UpIHtcbiAgICAgICAgICAgIHByZXBhcmVUZW1wbGF0ZVN0eWxlcyhyZW5kZXJDb250YWluZXIsIHBhcnQudmFsdWUudGVtcGxhdGUsIHNjb3BlTmFtZSk7XG4gICAgICAgIH1cbiAgICAgICAgcmVtb3ZlTm9kZXMoY29udGFpbmVyLCBjb250YWluZXIuZmlyc3RDaGlsZCk7XG4gICAgICAgIGNvbnRhaW5lci5hcHBlbmRDaGlsZChyZW5kZXJDb250YWluZXIpO1xuICAgICAgICBwYXJ0cy5zZXQoY29udGFpbmVyLCBwYXJ0KTtcbiAgICB9XG4gICAgLy8gQWZ0ZXIgZWxlbWVudHMgaGF2ZSBoaXQgdGhlIERPTSwgdXBkYXRlIHN0eWxpbmcgaWYgdGhpcyBpcyB0aGVcbiAgICAvLyBpbml0aWFsIHJlbmRlciB0byB0aGlzIGNvbnRhaW5lci5cbiAgICAvLyBUaGlzIGlzIG5lZWRlZCB3aGVuZXZlciBkeW5hbWljIGNoYW5nZXMgYXJlIG1hZGUgc28gaXQgd291bGQgYmVcbiAgICAvLyBzYWZlc3QgdG8gZG8gZXZlcnkgcmVuZGVyOyBob3dldmVyLCB0aGlzIHdvdWxkIHJlZ3Jlc3MgcGVyZm9ybWFuY2VcbiAgICAvLyBzbyB3ZSBsZWF2ZSBpdCB1cCB0byB0aGUgdXNlciB0byBjYWxsIGBTaGFkeUNTU1Muc3R5bGVFbGVtZW50YFxuICAgIC8vIGZvciBkeW5hbWljIGNoYW5nZXMuXG4gICAgaWYgKCFoYXNSZW5kZXJlZCAmJiBuZWVkc1Njb3BpbmcpIHtcbiAgICAgICAgd2luZG93LlNoYWR5Q1NTLnN0eWxlRWxlbWVudChjb250YWluZXIuaG9zdCk7XG4gICAgfVxufTtcbi8vIyBzb3VyY2VNYXBwaW5nVVJMPXNoYWR5LXJlbmRlci5qcy5tYXAiLCIvKipcbiAqIEBsaWNlbnNlXG4gKiBDb3B5cmlnaHQgKGMpIDIwMTcgVGhlIFBvbHltZXIgUHJvamVjdCBBdXRob3JzLiBBbGwgcmlnaHRzIHJlc2VydmVkLlxuICogVGhpcyBjb2RlIG1heSBvbmx5IGJlIHVzZWQgdW5kZXIgdGhlIEJTRCBzdHlsZSBsaWNlbnNlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vTElDRU5TRS50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgYXV0aG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9BVVRIT1JTLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBjb250cmlidXRvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQ09OVFJJQlVUT1JTLnR4dFxuICogQ29kZSBkaXN0cmlidXRlZCBieSBHb29nbGUgYXMgcGFydCBvZiB0aGUgcG9seW1lciBwcm9qZWN0IGlzIGFsc29cbiAqIHN1YmplY3QgdG8gYW4gYWRkaXRpb25hbCBJUCByaWdodHMgZ3JhbnQgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9QQVRFTlRTLnR4dFxuICovXG5pbXBvcnQgeyBtYXJrZXIsIFRlbXBsYXRlIH0gZnJvbSAnLi90ZW1wbGF0ZS5qcyc7XG4vKipcbiAqIFRoZSBkZWZhdWx0IFRlbXBsYXRlRmFjdG9yeSB3aGljaCBjYWNoZXMgVGVtcGxhdGVzIGtleWVkIG9uXG4gKiByZXN1bHQudHlwZSBhbmQgcmVzdWx0LnN0cmluZ3MuXG4gKi9cbmV4cG9ydCBmdW5jdGlvbiB0ZW1wbGF0ZUZhY3RvcnkocmVzdWx0KSB7XG4gICAgbGV0IHRlbXBsYXRlQ2FjaGUgPSB0ZW1wbGF0ZUNhY2hlcy5nZXQocmVzdWx0LnR5cGUpO1xuICAgIGlmICh0ZW1wbGF0ZUNhY2hlID09PSB1bmRlZmluZWQpIHtcbiAgICAgICAgdGVtcGxhdGVDYWNoZSA9IHtcbiAgICAgICAgICAgIHN0cmluZ3NBcnJheTogbmV3IFdlYWtNYXAoKSxcbiAgICAgICAgICAgIGtleVN0cmluZzogbmV3IE1hcCgpXG4gICAgICAgIH07XG4gICAgICAgIHRlbXBsYXRlQ2FjaGVzLnNldChyZXN1bHQudHlwZSwgdGVtcGxhdGVDYWNoZSk7XG4gICAgfVxuICAgIGxldCB0ZW1wbGF0ZSA9IHRlbXBsYXRlQ2FjaGUuc3RyaW5nc0FycmF5LmdldChyZXN1bHQuc3RyaW5ncyk7XG4gICAgaWYgKHRlbXBsYXRlICE9PSB1bmRlZmluZWQpIHtcbiAgICAgICAgcmV0dXJuIHRlbXBsYXRlO1xuICAgIH1cbiAgICAvLyBJZiB0aGUgVGVtcGxhdGVTdHJpbmdzQXJyYXkgaXMgbmV3LCBnZW5lcmF0ZSBhIGtleSBmcm9tIHRoZSBzdHJpbmdzXG4gICAgLy8gVGhpcyBrZXkgaXMgc2hhcmVkIGJldHdlZW4gYWxsIHRlbXBsYXRlcyB3aXRoIGlkZW50aWNhbCBjb250ZW50XG4gICAgY29uc3Qga2V5ID0gcmVzdWx0LnN0cmluZ3Muam9pbihtYXJrZXIpO1xuICAgIC8vIENoZWNrIGlmIHdlIGFscmVhZHkgaGF2ZSBhIFRlbXBsYXRlIGZvciB0aGlzIGtleVxuICAgIHRlbXBsYXRlID0gdGVtcGxhdGVDYWNoZS5rZXlTdHJpbmcuZ2V0KGtleSk7XG4gICAgaWYgKHRlbXBsYXRlID09PSB1bmRlZmluZWQpIHtcbiAgICAgICAgLy8gSWYgd2UgaGF2ZSBub3Qgc2VlbiB0aGlzIGtleSBiZWZvcmUsIGNyZWF0ZSBhIG5ldyBUZW1wbGF0ZVxuICAgICAgICB0ZW1wbGF0ZSA9IG5ldyBUZW1wbGF0ZShyZXN1bHQsIHJlc3VsdC5nZXRUZW1wbGF0ZUVsZW1lbnQoKSk7XG4gICAgICAgIC8vIENhY2hlIHRoZSBUZW1wbGF0ZSBmb3IgdGhpcyBrZXlcbiAgICAgICAgdGVtcGxhdGVDYWNoZS5rZXlTdHJpbmcuc2V0KGtleSwgdGVtcGxhdGUpO1xuICAgIH1cbiAgICAvLyBDYWNoZSBhbGwgZnV0dXJlIHF1ZXJpZXMgZm9yIHRoaXMgVGVtcGxhdGVTdHJpbmdzQXJyYXlcbiAgICB0ZW1wbGF0ZUNhY2hlLnN0cmluZ3NBcnJheS5zZXQocmVzdWx0LnN0cmluZ3MsIHRlbXBsYXRlKTtcbiAgICByZXR1cm4gdGVtcGxhdGU7XG59XG5leHBvcnQgY29uc3QgdGVtcGxhdGVDYWNoZXMgPSBuZXcgTWFwKCk7XG4vLyMgc291cmNlTWFwcGluZ1VSTD10ZW1wbGF0ZS1mYWN0b3J5LmpzLm1hcCIsIi8qKlxuICogQGxpY2Vuc2VcbiAqIENvcHlyaWdodCAoYykgMjAxNyBUaGUgUG9seW1lciBQcm9qZWN0IEF1dGhvcnMuIEFsbCByaWdodHMgcmVzZXJ2ZWQuXG4gKiBUaGlzIGNvZGUgbWF5IG9ubHkgYmUgdXNlZCB1bmRlciB0aGUgQlNEIHN0eWxlIGxpY2Vuc2UgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9MSUNFTlNFLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBhdXRob3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0FVVEhPUlMudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGNvbnRyaWJ1dG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9DT05UUklCVVRPUlMudHh0XG4gKiBDb2RlIGRpc3RyaWJ1dGVkIGJ5IEdvb2dsZSBhcyBwYXJ0IG9mIHRoZSBwb2x5bWVyIHByb2plY3QgaXMgYWxzb1xuICogc3ViamVjdCB0byBhbiBhZGRpdGlvbmFsIElQIHJpZ2h0cyBncmFudCBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL1BBVEVOVFMudHh0XG4gKi9cbi8qKlxuICogQG1vZHVsZSBsaXQtaHRtbFxuICovXG5pbXBvcnQgeyBpc0NFUG9seWZpbGwgfSBmcm9tICcuL2RvbS5qcyc7XG5pbXBvcnQgeyBpc1RlbXBsYXRlUGFydEFjdGl2ZSB9IGZyb20gJy4vdGVtcGxhdGUuanMnO1xuLyoqXG4gKiBBbiBpbnN0YW5jZSBvZiBhIGBUZW1wbGF0ZWAgdGhhdCBjYW4gYmUgYXR0YWNoZWQgdG8gdGhlIERPTSBhbmQgdXBkYXRlZFxuICogd2l0aCBuZXcgdmFsdWVzLlxuICovXG5leHBvcnQgY2xhc3MgVGVtcGxhdGVJbnN0YW5jZSB7XG4gICAgY29uc3RydWN0b3IodGVtcGxhdGUsIHByb2Nlc3Nvciwgb3B0aW9ucykge1xuICAgICAgICB0aGlzLl9wYXJ0cyA9IFtdO1xuICAgICAgICB0aGlzLnRlbXBsYXRlID0gdGVtcGxhdGU7XG4gICAgICAgIHRoaXMucHJvY2Vzc29yID0gcHJvY2Vzc29yO1xuICAgICAgICB0aGlzLm9wdGlvbnMgPSBvcHRpb25zO1xuICAgIH1cbiAgICB1cGRhdGUodmFsdWVzKSB7XG4gICAgICAgIGxldCBpID0gMDtcbiAgICAgICAgZm9yIChjb25zdCBwYXJ0IG9mIHRoaXMuX3BhcnRzKSB7XG4gICAgICAgICAgICBpZiAocGFydCAhPT0gdW5kZWZpbmVkKSB7XG4gICAgICAgICAgICAgICAgcGFydC5zZXRWYWx1ZSh2YWx1ZXNbaV0pO1xuICAgICAgICAgICAgfVxuICAgICAgICAgICAgaSsrO1xuICAgICAgICB9XG4gICAgICAgIGZvciAoY29uc3QgcGFydCBvZiB0aGlzLl9wYXJ0cykge1xuICAgICAgICAgICAgaWYgKHBhcnQgIT09IHVuZGVmaW5lZCkge1xuICAgICAgICAgICAgICAgIHBhcnQuY29tbWl0KCk7XG4gICAgICAgICAgICB9XG4gICAgICAgIH1cbiAgICB9XG4gICAgX2Nsb25lKCkge1xuICAgICAgICAvLyBXaGVuIHVzaW5nIHRoZSBDdXN0b20gRWxlbWVudHMgcG9seWZpbGwsIGNsb25lIHRoZSBub2RlLCByYXRoZXIgdGhhblxuICAgICAgICAvLyBpbXBvcnRpbmcgaXQsIHRvIGtlZXAgdGhlIGZyYWdtZW50IGluIHRoZSB0ZW1wbGF0ZSdzIGRvY3VtZW50LiBUaGlzXG4gICAgICAgIC8vIGxlYXZlcyB0aGUgZnJhZ21lbnQgaW5lcnQgc28gY3VzdG9tIGVsZW1lbnRzIHdvbid0IHVwZ3JhZGUgYW5kXG4gICAgICAgIC8vIHBvdGVudGlhbGx5IG1vZGlmeSB0aGVpciBjb250ZW50cyBieSBjcmVhdGluZyBhIHBvbHlmaWxsZWQgU2hhZG93Um9vdFxuICAgICAgICAvLyB3aGlsZSB3ZSB0cmF2ZXJzZSB0aGUgdHJlZS5cbiAgICAgICAgY29uc3QgZnJhZ21lbnQgPSBpc0NFUG9seWZpbGwgP1xuICAgICAgICAgICAgdGhpcy50ZW1wbGF0ZS5lbGVtZW50LmNvbnRlbnQuY2xvbmVOb2RlKHRydWUpIDpcbiAgICAgICAgICAgIGRvY3VtZW50LmltcG9ydE5vZGUodGhpcy50ZW1wbGF0ZS5lbGVtZW50LmNvbnRlbnQsIHRydWUpO1xuICAgICAgICBjb25zdCBwYXJ0cyA9IHRoaXMudGVtcGxhdGUucGFydHM7XG4gICAgICAgIGxldCBwYXJ0SW5kZXggPSAwO1xuICAgICAgICBsZXQgbm9kZUluZGV4ID0gMDtcbiAgICAgICAgY29uc3QgX3ByZXBhcmVJbnN0YW5jZSA9IChmcmFnbWVudCkgPT4ge1xuICAgICAgICAgICAgLy8gRWRnZSBuZWVkcyBhbGwgNCBwYXJhbWV0ZXJzIHByZXNlbnQ7IElFMTEgbmVlZHMgM3JkIHBhcmFtZXRlciB0byBiZVxuICAgICAgICAgICAgLy8gbnVsbFxuICAgICAgICAgICAgY29uc3Qgd2Fsa2VyID0gZG9jdW1lbnQuY3JlYXRlVHJlZVdhbGtlcihmcmFnbWVudCwgMTMzIC8qIE5vZGVGaWx0ZXIuU0hPV197RUxFTUVOVHxDT01NRU5UfFRFWFR9ICovLCBudWxsLCBmYWxzZSk7XG4gICAgICAgICAgICBsZXQgbm9kZSA9IHdhbGtlci5uZXh0Tm9kZSgpO1xuICAgICAgICAgICAgLy8gTG9vcCB0aHJvdWdoIGFsbCB0aGUgbm9kZXMgYW5kIHBhcnRzIG9mIGEgdGVtcGxhdGVcbiAgICAgICAgICAgIHdoaWxlIChwYXJ0SW5kZXggPCBwYXJ0cy5sZW5ndGggJiYgbm9kZSAhPT0gbnVsbCkge1xuICAgICAgICAgICAgICAgIGNvbnN0IHBhcnQgPSBwYXJ0c1twYXJ0SW5kZXhdO1xuICAgICAgICAgICAgICAgIC8vIENvbnNlY3V0aXZlIFBhcnRzIG1heSBoYXZlIHRoZSBzYW1lIG5vZGUgaW5kZXgsIGluIHRoZSBjYXNlIG9mXG4gICAgICAgICAgICAgICAgLy8gbXVsdGlwbGUgYm91bmQgYXR0cmlidXRlcyBvbiBhbiBlbGVtZW50LiBTbyBlYWNoIGl0ZXJhdGlvbiB3ZSBlaXRoZXJcbiAgICAgICAgICAgICAgICAvLyBpbmNyZW1lbnQgdGhlIG5vZGVJbmRleCwgaWYgd2UgYXJlbid0IG9uIGEgbm9kZSB3aXRoIGEgcGFydCwgb3IgdGhlXG4gICAgICAgICAgICAgICAgLy8gcGFydEluZGV4IGlmIHdlIGFyZS4gQnkgbm90IGluY3JlbWVudGluZyB0aGUgbm9kZUluZGV4IHdoZW4gd2UgZmluZCBhXG4gICAgICAgICAgICAgICAgLy8gcGFydCwgd2UgYWxsb3cgZm9yIHRoZSBuZXh0IHBhcnQgdG8gYmUgYXNzb2NpYXRlZCB3aXRoIHRoZSBjdXJyZW50XG4gICAgICAgICAgICAgICAgLy8gbm9kZSBpZiBuZWNjZXNzYXNyeS5cbiAgICAgICAgICAgICAgICBpZiAoIWlzVGVtcGxhdGVQYXJ0QWN0aXZlKHBhcnQpKSB7XG4gICAgICAgICAgICAgICAgICAgIHRoaXMuX3BhcnRzLnB1c2godW5kZWZpbmVkKTtcbiAgICAgICAgICAgICAgICAgICAgcGFydEluZGV4Kys7XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIGVsc2UgaWYgKG5vZGVJbmRleCA9PT0gcGFydC5pbmRleCkge1xuICAgICAgICAgICAgICAgICAgICBpZiAocGFydC50eXBlID09PSAnbm9kZScpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbnN0IHBhcnQgPSB0aGlzLnByb2Nlc3Nvci5oYW5kbGVUZXh0RXhwcmVzc2lvbih0aGlzLm9wdGlvbnMpO1xuICAgICAgICAgICAgICAgICAgICAgICAgcGFydC5pbnNlcnRBZnRlck5vZGUobm9kZS5wcmV2aW91c1NpYmxpbmcpO1xuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5fcGFydHMucHVzaChwYXJ0KTtcbiAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuX3BhcnRzLnB1c2goLi4udGhpcy5wcm9jZXNzb3IuaGFuZGxlQXR0cmlidXRlRXhwcmVzc2lvbnMobm9kZSwgcGFydC5uYW1lLCBwYXJ0LnN0cmluZ3MsIHRoaXMub3B0aW9ucykpO1xuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgIHBhcnRJbmRleCsrO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgICAgICAgICAgbm9kZUluZGV4Kys7XG4gICAgICAgICAgICAgICAgICAgIGlmIChub2RlLm5vZGVOYW1lID09PSAnVEVNUExBVEUnKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICBfcHJlcGFyZUluc3RhbmNlKG5vZGUuY29udGVudCk7XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgbm9kZSA9IHdhbGtlci5uZXh0Tm9kZSgpO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH1cbiAgICAgICAgfTtcbiAgICAgICAgX3ByZXBhcmVJbnN0YW5jZShmcmFnbWVudCk7XG4gICAgICAgIGlmIChpc0NFUG9seWZpbGwpIHtcbiAgICAgICAgICAgIGRvY3VtZW50LmFkb3B0Tm9kZShmcmFnbWVudCk7XG4gICAgICAgICAgICBjdXN0b21FbGVtZW50cy51cGdyYWRlKGZyYWdtZW50KTtcbiAgICAgICAgfVxuICAgICAgICByZXR1cm4gZnJhZ21lbnQ7XG4gICAgfVxufVxuLy8jIHNvdXJjZU1hcHBpbmdVUkw9dGVtcGxhdGUtaW5zdGFuY2UuanMubWFwIiwiLyoqXG4gKiBAbGljZW5zZVxuICogQ29weXJpZ2h0IChjKSAyMDE3IFRoZSBQb2x5bWVyIFByb2plY3QgQXV0aG9ycy4gQWxsIHJpZ2h0cyByZXNlcnZlZC5cbiAqIFRoaXMgY29kZSBtYXkgb25seSBiZSB1c2VkIHVuZGVyIHRoZSBCU0Qgc3R5bGUgbGljZW5zZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0xJQ0VOU0UudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGF1dGhvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQVVUSE9SUy50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgY29udHJpYnV0b3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0NPTlRSSUJVVE9SUy50eHRcbiAqIENvZGUgZGlzdHJpYnV0ZWQgYnkgR29vZ2xlIGFzIHBhcnQgb2YgdGhlIHBvbHltZXIgcHJvamVjdCBpcyBhbHNvXG4gKiBzdWJqZWN0IHRvIGFuIGFkZGl0aW9uYWwgSVAgcmlnaHRzIGdyYW50IGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vUEFURU5UUy50eHRcbiAqL1xuLyoqXG4gKiBAbW9kdWxlIGxpdC1odG1sXG4gKi9cbmltcG9ydCB7IHJlcGFyZW50Tm9kZXMgfSBmcm9tICcuL2RvbS5qcyc7XG5pbXBvcnQgeyBib3VuZEF0dHJpYnV0ZVN1ZmZpeCwgbGFzdEF0dHJpYnV0ZU5hbWVSZWdleCwgbWFya2VyLCBub2RlTWFya2VyIH0gZnJvbSAnLi90ZW1wbGF0ZS5qcyc7XG4vKipcbiAqIFRoZSByZXR1cm4gdHlwZSBvZiBgaHRtbGAsIHdoaWNoIGhvbGRzIGEgVGVtcGxhdGUgYW5kIHRoZSB2YWx1ZXMgZnJvbVxuICogaW50ZXJwb2xhdGVkIGV4cHJlc3Npb25zLlxuICovXG5leHBvcnQgY2xhc3MgVGVtcGxhdGVSZXN1bHQge1xuICAgIGNvbnN0cnVjdG9yKHN0cmluZ3MsIHZhbHVlcywgdHlwZSwgcHJvY2Vzc29yKSB7XG4gICAgICAgIHRoaXMuc3RyaW5ncyA9IHN0cmluZ3M7XG4gICAgICAgIHRoaXMudmFsdWVzID0gdmFsdWVzO1xuICAgICAgICB0aGlzLnR5cGUgPSB0eXBlO1xuICAgICAgICB0aGlzLnByb2Nlc3NvciA9IHByb2Nlc3NvcjtcbiAgICB9XG4gICAgLyoqXG4gICAgICogUmV0dXJucyBhIHN0cmluZyBvZiBIVE1MIHVzZWQgdG8gY3JlYXRlIGEgYDx0ZW1wbGF0ZT5gIGVsZW1lbnQuXG4gICAgICovXG4gICAgZ2V0SFRNTCgpIHtcbiAgICAgICAgY29uc3QgZW5kSW5kZXggPSB0aGlzLnN0cmluZ3MubGVuZ3RoIC0gMTtcbiAgICAgICAgbGV0IGh0bWwgPSAnJztcbiAgICAgICAgZm9yIChsZXQgaSA9IDA7IGkgPCBlbmRJbmRleDsgaSsrKSB7XG4gICAgICAgICAgICBjb25zdCBzID0gdGhpcy5zdHJpbmdzW2ldO1xuICAgICAgICAgICAgLy8gVGhpcyBleGVjKCkgY2FsbCBkb2VzIHR3byB0aGluZ3M6XG4gICAgICAgICAgICAvLyAxKSBBcHBlbmRzIGEgc3VmZml4IHRvIHRoZSBib3VuZCBhdHRyaWJ1dGUgbmFtZSB0byBvcHQgb3V0IG9mIHNwZWNpYWxcbiAgICAgICAgICAgIC8vIGF0dHJpYnV0ZSB2YWx1ZSBwYXJzaW5nIHRoYXQgSUUxMSBhbmQgRWRnZSBkbywgbGlrZSBmb3Igc3R5bGUgYW5kXG4gICAgICAgICAgICAvLyBtYW55IFNWRyBhdHRyaWJ1dGVzLiBUaGUgVGVtcGxhdGUgY2xhc3MgYWxzbyBhcHBlbmRzIHRoZSBzYW1lIHN1ZmZpeFxuICAgICAgICAgICAgLy8gd2hlbiBsb29raW5nIHVwIGF0dHJpYnV0ZXMgdG8gY3JlYXRlIFBhcnRzLlxuICAgICAgICAgICAgLy8gMikgQWRkcyBhbiB1bnF1b3RlZC1hdHRyaWJ1dGUtc2FmZSBtYXJrZXIgZm9yIHRoZSBmaXJzdCBleHByZXNzaW9uIGluXG4gICAgICAgICAgICAvLyBhbiBhdHRyaWJ1dGUuIFN1YnNlcXVlbnQgYXR0cmlidXRlIGV4cHJlc3Npb25zIHdpbGwgdXNlIG5vZGUgbWFya2VycyxcbiAgICAgICAgICAgIC8vIGFuZCB0aGlzIGlzIHNhZmUgc2luY2UgYXR0cmlidXRlcyB3aXRoIG11bHRpcGxlIGV4cHJlc3Npb25zIGFyZVxuICAgICAgICAgICAgLy8gZ3VhcmFudGVlZCB0byBiZSBxdW90ZWQuXG4gICAgICAgICAgICBjb25zdCBtYXRjaCA9IGxhc3RBdHRyaWJ1dGVOYW1lUmVnZXguZXhlYyhzKTtcbiAgICAgICAgICAgIGlmIChtYXRjaCkge1xuICAgICAgICAgICAgICAgIC8vIFdlJ3JlIHN0YXJ0aW5nIGEgbmV3IGJvdW5kIGF0dHJpYnV0ZS5cbiAgICAgICAgICAgICAgICAvLyBBZGQgdGhlIHNhZmUgYXR0cmlidXRlIHN1ZmZpeCwgYW5kIHVzZSB1bnF1b3RlZC1hdHRyaWJ1dGUtc2FmZVxuICAgICAgICAgICAgICAgIC8vIG1hcmtlci5cbiAgICAgICAgICAgICAgICBodG1sICs9IHMuc3Vic3RyKDAsIG1hdGNoLmluZGV4KSArIG1hdGNoWzFdICsgbWF0Y2hbMl0gK1xuICAgICAgICAgICAgICAgICAgICBib3VuZEF0dHJpYnV0ZVN1ZmZpeCArIG1hdGNoWzNdICsgbWFya2VyO1xuICAgICAgICAgICAgfVxuICAgICAgICAgICAgZWxzZSB7XG4gICAgICAgICAgICAgICAgLy8gV2UncmUgZWl0aGVyIGluIGEgYm91bmQgbm9kZSwgb3IgdHJhaWxpbmcgYm91bmQgYXR0cmlidXRlLlxuICAgICAgICAgICAgICAgIC8vIEVpdGhlciB3YXksIG5vZGVNYXJrZXIgaXMgc2FmZSB0byB1c2UuXG4gICAgICAgICAgICAgICAgaHRtbCArPSBzICsgbm9kZU1hcmtlcjtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgICAgICByZXR1cm4gaHRtbCArIHRoaXMuc3RyaW5nc1tlbmRJbmRleF07XG4gICAgfVxuICAgIGdldFRlbXBsYXRlRWxlbWVudCgpIHtcbiAgICAgICAgY29uc3QgdGVtcGxhdGUgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KCd0ZW1wbGF0ZScpO1xuICAgICAgICB0ZW1wbGF0ZS5pbm5lckhUTUwgPSB0aGlzLmdldEhUTUwoKTtcbiAgICAgICAgcmV0dXJuIHRlbXBsYXRlO1xuICAgIH1cbn1cbi8qKlxuICogQSBUZW1wbGF0ZVJlc3VsdCBmb3IgU1ZHIGZyYWdtZW50cy5cbiAqXG4gKiBUaGlzIGNsYXNzIHdyYXBzIEhUTWwgaW4gYW4gYDxzdmc+YCB0YWcgaW4gb3JkZXIgdG8gcGFyc2UgaXRzIGNvbnRlbnRzIGluIHRoZVxuICogU1ZHIG5hbWVzcGFjZSwgdGhlbiBtb2RpZmllcyB0aGUgdGVtcGxhdGUgdG8gcmVtb3ZlIHRoZSBgPHN2Zz5gIHRhZyBzbyB0aGF0XG4gKiBjbG9uZXMgb25seSBjb250YWluZXIgdGhlIG9yaWdpbmFsIGZyYWdtZW50LlxuICovXG5leHBvcnQgY2xhc3MgU1ZHVGVtcGxhdGVSZXN1bHQgZXh0ZW5kcyBUZW1wbGF0ZVJlc3VsdCB7XG4gICAgZ2V0SFRNTCgpIHtcbiAgICAgICAgcmV0dXJuIGA8c3ZnPiR7c3VwZXIuZ2V0SFRNTCgpfTwvc3ZnPmA7XG4gICAgfVxuICAgIGdldFRlbXBsYXRlRWxlbWVudCgpIHtcbiAgICAgICAgY29uc3QgdGVtcGxhdGUgPSBzdXBlci5nZXRUZW1wbGF0ZUVsZW1lbnQoKTtcbiAgICAgICAgY29uc3QgY29udGVudCA9IHRlbXBsYXRlLmNvbnRlbnQ7XG4gICAgICAgIGNvbnN0IHN2Z0VsZW1lbnQgPSBjb250ZW50LmZpcnN0Q2hpbGQ7XG4gICAgICAgIGNvbnRlbnQucmVtb3ZlQ2hpbGQoc3ZnRWxlbWVudCk7XG4gICAgICAgIHJlcGFyZW50Tm9kZXMoY29udGVudCwgc3ZnRWxlbWVudC5maXJzdENoaWxkKTtcbiAgICAgICAgcmV0dXJuIHRlbXBsYXRlO1xuICAgIH1cbn1cbi8vIyBzb3VyY2VNYXBwaW5nVVJMPXRlbXBsYXRlLXJlc3VsdC5qcy5tYXAiLCIvKipcbiAqIEBsaWNlbnNlXG4gKiBDb3B5cmlnaHQgKGMpIDIwMTcgVGhlIFBvbHltZXIgUHJvamVjdCBBdXRob3JzLiBBbGwgcmlnaHRzIHJlc2VydmVkLlxuICogVGhpcyBjb2RlIG1heSBvbmx5IGJlIHVzZWQgdW5kZXIgdGhlIEJTRCBzdHlsZSBsaWNlbnNlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vTElDRU5TRS50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgYXV0aG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9BVVRIT1JTLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBjb250cmlidXRvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQ09OVFJJQlVUT1JTLnR4dFxuICogQ29kZSBkaXN0cmlidXRlZCBieSBHb29nbGUgYXMgcGFydCBvZiB0aGUgcG9seW1lciBwcm9qZWN0IGlzIGFsc29cbiAqIHN1YmplY3QgdG8gYW4gYWRkaXRpb25hbCBJUCByaWdodHMgZ3JhbnQgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9QQVRFTlRTLnR4dFxuICovXG4vKipcbiAqIEFuIGV4cHJlc3Npb24gbWFya2VyIHdpdGggZW1iZWRkZWQgdW5pcXVlIGtleSB0byBhdm9pZCBjb2xsaXNpb24gd2l0aFxuICogcG9zc2libGUgdGV4dCBpbiB0ZW1wbGF0ZXMuXG4gKi9cbmV4cG9ydCBjb25zdCBtYXJrZXIgPSBge3tsaXQtJHtTdHJpbmcoTWF0aC5yYW5kb20oKSkuc2xpY2UoMil9fX1gO1xuLyoqXG4gKiBBbiBleHByZXNzaW9uIG1hcmtlciB1c2VkIHRleHQtcG9zaXRpb25zLCBtdWx0aS1iaW5kaW5nIGF0dHJpYnV0ZXMsIGFuZFxuICogYXR0cmlidXRlcyB3aXRoIG1hcmt1cC1saWtlIHRleHQgdmFsdWVzLlxuICovXG5leHBvcnQgY29uc3Qgbm9kZU1hcmtlciA9IGA8IS0tJHttYXJrZXJ9LS0+YDtcbmV4cG9ydCBjb25zdCBtYXJrZXJSZWdleCA9IG5ldyBSZWdFeHAoYCR7bWFya2VyfXwke25vZGVNYXJrZXJ9YCk7XG4vKipcbiAqIFN1ZmZpeCBhcHBlbmRlZCB0byBhbGwgYm91bmQgYXR0cmlidXRlIG5hbWVzLlxuICovXG5leHBvcnQgY29uc3QgYm91bmRBdHRyaWJ1dGVTdWZmaXggPSAnJGxpdCQnO1xuLyoqXG4gKiBBbiB1cGRhdGVhYmxlIFRlbXBsYXRlIHRoYXQgdHJhY2tzIHRoZSBsb2NhdGlvbiBvZiBkeW5hbWljIHBhcnRzLlxuICovXG5leHBvcnQgY2xhc3MgVGVtcGxhdGUge1xuICAgIGNvbnN0cnVjdG9yKHJlc3VsdCwgZWxlbWVudCkge1xuICAgICAgICB0aGlzLnBhcnRzID0gW107XG4gICAgICAgIHRoaXMuZWxlbWVudCA9IGVsZW1lbnQ7XG4gICAgICAgIGxldCBpbmRleCA9IC0xO1xuICAgICAgICBsZXQgcGFydEluZGV4ID0gMDtcbiAgICAgICAgY29uc3Qgbm9kZXNUb1JlbW92ZSA9IFtdO1xuICAgICAgICBjb25zdCBfcHJlcGFyZVRlbXBsYXRlID0gKHRlbXBsYXRlKSA9PiB7XG4gICAgICAgICAgICBjb25zdCBjb250ZW50ID0gdGVtcGxhdGUuY29udGVudDtcbiAgICAgICAgICAgIC8vIEVkZ2UgbmVlZHMgYWxsIDQgcGFyYW1ldGVycyBwcmVzZW50OyBJRTExIG5lZWRzIDNyZCBwYXJhbWV0ZXIgdG8gYmVcbiAgICAgICAgICAgIC8vIG51bGxcbiAgICAgICAgICAgIGNvbnN0IHdhbGtlciA9IGRvY3VtZW50LmNyZWF0ZVRyZWVXYWxrZXIoY29udGVudCwgMTMzIC8qIE5vZGVGaWx0ZXIuU0hPV197RUxFTUVOVHxDT01NRU5UfFRFWFR9ICovLCBudWxsLCBmYWxzZSk7XG4gICAgICAgICAgICAvLyBLZWVwcyB0cmFjayBvZiB0aGUgbGFzdCBpbmRleCBhc3NvY2lhdGVkIHdpdGggYSBwYXJ0LiBXZSB0cnkgdG8gZGVsZXRlXG4gICAgICAgICAgICAvLyB1bm5lY2Vzc2FyeSBub2RlcywgYnV0IHdlIG5ldmVyIHdhbnQgdG8gYXNzb2NpYXRlIHR3byBkaWZmZXJlbnQgcGFydHNcbiAgICAgICAgICAgIC8vIHRvIHRoZSBzYW1lIGluZGV4LiBUaGV5IG11c3QgaGF2ZSBhIGNvbnN0YW50IG5vZGUgYmV0d2Vlbi5cbiAgICAgICAgICAgIGxldCBsYXN0UGFydEluZGV4ID0gMDtcbiAgICAgICAgICAgIHdoaWxlICh3YWxrZXIubmV4dE5vZGUoKSkge1xuICAgICAgICAgICAgICAgIGluZGV4Kys7XG4gICAgICAgICAgICAgICAgY29uc3Qgbm9kZSA9IHdhbGtlci5jdXJyZW50Tm9kZTtcbiAgICAgICAgICAgICAgICBpZiAobm9kZS5ub2RlVHlwZSA9PT0gMSAvKiBOb2RlLkVMRU1FTlRfTk9ERSAqLykge1xuICAgICAgICAgICAgICAgICAgICBpZiAobm9kZS5oYXNBdHRyaWJ1dGVzKCkpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbnN0IGF0dHJpYnV0ZXMgPSBub2RlLmF0dHJpYnV0ZXM7XG4gICAgICAgICAgICAgICAgICAgICAgICAvLyBQZXJcbiAgICAgICAgICAgICAgICAgICAgICAgIC8vIGh0dHBzOi8vZGV2ZWxvcGVyLm1vemlsbGEub3JnL2VuLVVTL2RvY3MvV2ViL0FQSS9OYW1lZE5vZGVNYXAsXG4gICAgICAgICAgICAgICAgICAgICAgICAvLyBhdHRyaWJ1dGVzIGFyZSBub3QgZ3VhcmFudGVlZCB0byBiZSByZXR1cm5lZCBpbiBkb2N1bWVudCBvcmRlci5cbiAgICAgICAgICAgICAgICAgICAgICAgIC8vIEluIHBhcnRpY3VsYXIsIEVkZ2UvSUUgY2FuIHJldHVybiB0aGVtIG91dCBvZiBvcmRlciwgc28gd2UgY2Fubm90XG4gICAgICAgICAgICAgICAgICAgICAgICAvLyBhc3N1bWUgYSBjb3JyZXNwb25kYW5jZSBiZXR3ZWVuIHBhcnQgaW5kZXggYW5kIGF0dHJpYnV0ZSBpbmRleC5cbiAgICAgICAgICAgICAgICAgICAgICAgIGxldCBjb3VudCA9IDA7XG4gICAgICAgICAgICAgICAgICAgICAgICBmb3IgKGxldCBpID0gMDsgaSA8IGF0dHJpYnV0ZXMubGVuZ3RoOyBpKyspIHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAoYXR0cmlidXRlc1tpXS52YWx1ZS5pbmRleE9mKG1hcmtlcikgPj0gMCkge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBjb3VudCsrO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgICAgIHdoaWxlIChjb3VudC0tID4gMCkge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIC8vIEdldCB0aGUgdGVtcGxhdGUgbGl0ZXJhbCBzZWN0aW9uIGxlYWRpbmcgdXAgdG8gdGhlIGZpcnN0XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgLy8gZXhwcmVzc2lvbiBpbiB0aGlzIGF0dHJpYnV0ZVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNvbnN0IHN0cmluZ0ZvclBhcnQgPSByZXN1bHQuc3RyaW5nc1twYXJ0SW5kZXhdO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIC8vIEZpbmQgdGhlIGF0dHJpYnV0ZSBuYW1lXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY29uc3QgbmFtZSA9IGxhc3RBdHRyaWJ1dGVOYW1lUmVnZXguZXhlYyhzdHJpbmdGb3JQYXJ0KVsyXTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAvLyBGaW5kIHRoZSBjb3JyZXNwb25kaW5nIGF0dHJpYnV0ZVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIC8vIEFsbCBib3VuZCBhdHRyaWJ1dGVzIGhhdmUgaGFkIGEgc3VmZml4IGFkZGVkIGluXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgLy8gVGVtcGxhdGVSZXN1bHQjZ2V0SFRNTCB0byBvcHQgb3V0IG9mIHNwZWNpYWwgYXR0cmlidXRlXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgLy8gaGFuZGxpbmcuIFRvIGxvb2sgdXAgdGhlIGF0dHJpYnV0ZSB2YWx1ZSB3ZSBhbHNvIG5lZWQgdG8gYWRkXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgLy8gdGhlIHN1ZmZpeC5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjb25zdCBhdHRyaWJ1dGVMb29rdXBOYW1lID0gbmFtZS50b0xvd2VyQ2FzZSgpICsgYm91bmRBdHRyaWJ1dGVTdWZmaXg7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY29uc3QgYXR0cmlidXRlVmFsdWUgPSBub2RlLmdldEF0dHJpYnV0ZShhdHRyaWJ1dGVMb29rdXBOYW1lKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjb25zdCBzdHJpbmdzID0gYXR0cmlidXRlVmFsdWUuc3BsaXQobWFya2VyUmVnZXgpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMucGFydHMucHVzaCh7IHR5cGU6ICdhdHRyaWJ1dGUnLCBpbmRleCwgbmFtZSwgc3RyaW5ncyB9KTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBub2RlLnJlbW92ZUF0dHJpYnV0ZShhdHRyaWJ1dGVMb29rdXBOYW1lKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBwYXJ0SW5kZXggKz0gc3RyaW5ncy5sZW5ndGggLSAxO1xuICAgICAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgIGlmIChub2RlLnRhZ05hbWUgPT09ICdURU1QTEFURScpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgIF9wcmVwYXJlVGVtcGxhdGUobm9kZSk7XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgZWxzZSBpZiAobm9kZS5ub2RlVHlwZSA9PT0gMyAvKiBOb2RlLlRFWFRfTk9ERSAqLykge1xuICAgICAgICAgICAgICAgICAgICBjb25zdCBkYXRhID0gbm9kZS5kYXRhO1xuICAgICAgICAgICAgICAgICAgICBpZiAoZGF0YS5pbmRleE9mKG1hcmtlcikgPj0gMCkge1xuICAgICAgICAgICAgICAgICAgICAgICAgY29uc3QgcGFyZW50ID0gbm9kZS5wYXJlbnROb2RlO1xuICAgICAgICAgICAgICAgICAgICAgICAgY29uc3Qgc3RyaW5ncyA9IGRhdGEuc3BsaXQobWFya2VyUmVnZXgpO1xuICAgICAgICAgICAgICAgICAgICAgICAgY29uc3QgbGFzdEluZGV4ID0gc3RyaW5ncy5sZW5ndGggLSAxO1xuICAgICAgICAgICAgICAgICAgICAgICAgLy8gR2VuZXJhdGUgYSBuZXcgdGV4dCBub2RlIGZvciBlYWNoIGxpdGVyYWwgc2VjdGlvblxuICAgICAgICAgICAgICAgICAgICAgICAgLy8gVGhlc2Ugbm9kZXMgYXJlIGFsc28gdXNlZCBhcyB0aGUgbWFya2VycyBmb3Igbm9kZSBwYXJ0c1xuICAgICAgICAgICAgICAgICAgICAgICAgZm9yIChsZXQgaSA9IDA7IGkgPCBsYXN0SW5kZXg7IGkrKykge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHBhcmVudC5pbnNlcnRCZWZvcmUoKHN0cmluZ3NbaV0gPT09ICcnKSA/IGNyZWF0ZU1hcmtlcigpIDpcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZG9jdW1lbnQuY3JlYXRlVGV4dE5vZGUoc3RyaW5nc1tpXSksIG5vZGUpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMucGFydHMucHVzaCh7IHR5cGU6ICdub2RlJywgaW5kZXg6ICsraW5kZXggfSk7XG4gICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgICAgICAvLyBJZiB0aGVyZSdzIG5vIHRleHQsIHdlIG11c3QgaW5zZXJ0IGEgY29tbWVudCB0byBtYXJrIG91ciBwbGFjZS5cbiAgICAgICAgICAgICAgICAgICAgICAgIC8vIEVsc2UsIHdlIGNhbiB0cnVzdCBpdCB3aWxsIHN0aWNrIGFyb3VuZCBhZnRlciBjbG9uaW5nLlxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHN0cmluZ3NbbGFzdEluZGV4XSA9PT0gJycpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBwYXJlbnQuaW5zZXJ0QmVmb3JlKGNyZWF0ZU1hcmtlcigpLCBub2RlKTtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBub2Rlc1RvUmVtb3ZlLnB1c2gobm9kZSk7XG4gICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBub2RlLmRhdGEgPSBzdHJpbmdzW2xhc3RJbmRleF07XG4gICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgICAgICAvLyBXZSBoYXZlIGEgcGFydCBmb3IgZWFjaCBtYXRjaCBmb3VuZFxuICAgICAgICAgICAgICAgICAgICAgICAgcGFydEluZGV4ICs9IGxhc3RJbmRleDtcbiAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICBlbHNlIGlmIChub2RlLm5vZGVUeXBlID09PSA4IC8qIE5vZGUuQ09NTUVOVF9OT0RFICovKSB7XG4gICAgICAgICAgICAgICAgICAgIGlmIChub2RlLmRhdGEgPT09IG1hcmtlcikge1xuICAgICAgICAgICAgICAgICAgICAgICAgY29uc3QgcGFyZW50ID0gbm9kZS5wYXJlbnROb2RlO1xuICAgICAgICAgICAgICAgICAgICAgICAgLy8gQWRkIGEgbmV3IG1hcmtlciBub2RlIHRvIGJlIHRoZSBzdGFydE5vZGUgb2YgdGhlIFBhcnQgaWYgYW55IG9mXG4gICAgICAgICAgICAgICAgICAgICAgICAvLyB0aGUgZm9sbG93aW5nIGFyZSB0cnVlOlxuICAgICAgICAgICAgICAgICAgICAgICAgLy8gICogV2UgZG9uJ3QgaGF2ZSBhIHByZXZpb3VzU2libGluZ1xuICAgICAgICAgICAgICAgICAgICAgICAgLy8gICogVGhlIHByZXZpb3VzU2libGluZyBpcyBhbHJlYWR5IHRoZSBzdGFydCBvZiBhIHByZXZpb3VzIHBhcnRcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChub2RlLnByZXZpb3VzU2libGluZyA9PT0gbnVsbCB8fCBpbmRleCA9PT0gbGFzdFBhcnRJbmRleCkge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGluZGV4Kys7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcGFyZW50Lmluc2VydEJlZm9yZShjcmVhdGVNYXJrZXIoKSwgbm9kZSk7XG4gICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgICAgICBsYXN0UGFydEluZGV4ID0gaW5kZXg7XG4gICAgICAgICAgICAgICAgICAgICAgICB0aGlzLnBhcnRzLnB1c2goeyB0eXBlOiAnbm9kZScsIGluZGV4IH0pO1xuICAgICAgICAgICAgICAgICAgICAgICAgLy8gSWYgd2UgZG9uJ3QgaGF2ZSBhIG5leHRTaWJsaW5nLCBrZWVwIHRoaXMgbm9kZSBzbyB3ZSBoYXZlIGFuIGVuZC5cbiAgICAgICAgICAgICAgICAgICAgICAgIC8vIEVsc2UsIHdlIGNhbiByZW1vdmUgaXQgdG8gc2F2ZSBmdXR1cmUgY29zdHMuXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAobm9kZS5uZXh0U2libGluZyA9PT0gbnVsbCkge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG5vZGUuZGF0YSA9ICcnO1xuICAgICAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICAgICAgZWxzZSB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbm9kZXNUb1JlbW92ZS5wdXNoKG5vZGUpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGluZGV4LS07XG4gICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgICAgICBwYXJ0SW5kZXgrKztcbiAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgICAgICAgICAgICAgIGxldCBpID0gLTE7XG4gICAgICAgICAgICAgICAgICAgICAgICB3aGlsZSAoKGkgPSBub2RlLmRhdGEuaW5kZXhPZihtYXJrZXIsIGkgKyAxKSkgIT09XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgLTEpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAvLyBDb21tZW50IG5vZGUgaGFzIGEgYmluZGluZyBtYXJrZXIgaW5zaWRlLCBtYWtlIGFuIGluYWN0aXZlIHBhcnRcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAvLyBUaGUgYmluZGluZyB3b24ndCB3b3JrLCBidXQgc3Vic2VxdWVudCBiaW5kaW5ncyB3aWxsXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgLy8gVE9ETyAoanVzdGluZmFnbmFuaSk6IGNvbnNpZGVyIHdoZXRoZXIgaXQncyBldmVuIHdvcnRoIGl0IHRvXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgLy8gbWFrZSBiaW5kaW5ncyBpbiBjb21tZW50cyB3b3JrXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5wYXJ0cy5wdXNoKHsgdHlwZTogJ25vZGUnLCBpbmRleDogLTEgfSk7XG4gICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICB9XG4gICAgICAgIH07XG4gICAgICAgIF9wcmVwYXJlVGVtcGxhdGUoZWxlbWVudCk7XG4gICAgICAgIC8vIFJlbW92ZSB0ZXh0IGJpbmRpbmcgbm9kZXMgYWZ0ZXIgdGhlIHdhbGsgdG8gbm90IGRpc3R1cmIgdGhlIFRyZWVXYWxrZXJcbiAgICAgICAgZm9yIChjb25zdCBuIG9mIG5vZGVzVG9SZW1vdmUpIHtcbiAgICAgICAgICAgIG4ucGFyZW50Tm9kZS5yZW1vdmVDaGlsZChuKTtcbiAgICAgICAgfVxuICAgIH1cbn1cbmV4cG9ydCBjb25zdCBpc1RlbXBsYXRlUGFydEFjdGl2ZSA9IChwYXJ0KSA9PiBwYXJ0LmluZGV4ICE9PSAtMTtcbi8vIEFsbG93cyBgZG9jdW1lbnQuY3JlYXRlQ29tbWVudCgnJylgIHRvIGJlIHJlbmFtZWQgZm9yIGFcbi8vIHNtYWxsIG1hbnVhbCBzaXplLXNhdmluZ3MuXG5leHBvcnQgY29uc3QgY3JlYXRlTWFya2VyID0gKCkgPT4gZG9jdW1lbnQuY3JlYXRlQ29tbWVudCgnJyk7XG4vKipcbiAqIFRoaXMgcmVnZXggZXh0cmFjdHMgdGhlIGF0dHJpYnV0ZSBuYW1lIHByZWNlZGluZyBhbiBhdHRyaWJ1dGUtcG9zaXRpb25cbiAqIGV4cHJlc3Npb24uIEl0IGRvZXMgdGhpcyBieSBtYXRjaGluZyB0aGUgc3ludGF4IGFsbG93ZWQgZm9yIGF0dHJpYnV0ZXNcbiAqIGFnYWluc3QgdGhlIHN0cmluZyBsaXRlcmFsIGRpcmVjdGx5IHByZWNlZGluZyB0aGUgZXhwcmVzc2lvbiwgYXNzdW1pbmcgdGhhdFxuICogdGhlIGV4cHJlc3Npb24gaXMgaW4gYW4gYXR0cmlidXRlLXZhbHVlIHBvc2l0aW9uLlxuICpcbiAqIFNlZSBhdHRyaWJ1dGVzIGluIHRoZSBIVE1MIHNwZWM6XG4gKiBodHRwczovL3d3dy53My5vcmcvVFIvaHRtbDUvc3ludGF4Lmh0bWwjYXR0cmlidXRlcy0wXG4gKlxuICogXCJcXDAtXFx4MUZcXHg3Ri1cXHg5RlwiIGFyZSBVbmljb2RlIGNvbnRyb2wgY2hhcmFjdGVyc1xuICpcbiAqIFwiIFxceDA5XFx4MGFcXHgwY1xceDBkXCIgYXJlIEhUTUwgc3BhY2UgY2hhcmFjdGVyczpcbiAqIGh0dHBzOi8vd3d3LnczLm9yZy9UUi9odG1sNS9pbmZyYXN0cnVjdHVyZS5odG1sI3NwYWNlLWNoYXJhY3RlclxuICpcbiAqIFNvIGFuIGF0dHJpYnV0ZSBpczpcbiAqICAqIFRoZSBuYW1lOiBhbnkgY2hhcmFjdGVyIGV4Y2VwdCBhIGNvbnRyb2wgY2hhcmFjdGVyLCBzcGFjZSBjaGFyYWN0ZXIsICgnKSxcbiAqICAgIChcIiksIFwiPlwiLCBcIj1cIiwgb3IgXCIvXCJcbiAqICAqIEZvbGxvd2VkIGJ5IHplcm8gb3IgbW9yZSBzcGFjZSBjaGFyYWN0ZXJzXG4gKiAgKiBGb2xsb3dlZCBieSBcIj1cIlxuICogICogRm9sbG93ZWQgYnkgemVybyBvciBtb3JlIHNwYWNlIGNoYXJhY3RlcnNcbiAqICAqIEZvbGxvd2VkIGJ5OlxuICogICAgKiBBbnkgY2hhcmFjdGVyIGV4Y2VwdCBzcGFjZSwgKCcpLCAoXCIpLCBcIjxcIiwgXCI+XCIsIFwiPVwiLCAoYCksIG9yXG4gKiAgICAqIChcIikgdGhlbiBhbnkgbm9uLShcIiksIG9yXG4gKiAgICAqICgnKSB0aGVuIGFueSBub24tKCcpXG4gKi9cbmV4cG9ydCBjb25zdCBsYXN0QXR0cmlidXRlTmFtZVJlZ2V4ID0gLyhbIFxceDA5XFx4MGFcXHgwY1xceDBkXSkoW15cXDAtXFx4MUZcXHg3Ri1cXHg5RiBcXHgwOVxceDBhXFx4MGNcXHgwZFwiJz49L10rKShbIFxceDA5XFx4MGFcXHgwY1xceDBkXSo9WyBcXHgwOVxceDBhXFx4MGNcXHgwZF0qKD86W14gXFx4MDlcXHgwYVxceDBjXFx4MGRcIidgPD49XSp8XCJbXlwiXSp8J1teJ10qKSkkLztcbi8vIyBzb3VyY2VNYXBwaW5nVVJMPXRlbXBsYXRlLmpzLm1hcCIsIi8qKlxuICogQGxpY2Vuc2VcbiAqIENvcHlyaWdodCAoYykgMjAxNyBUaGUgUG9seW1lciBQcm9qZWN0IEF1dGhvcnMuIEFsbCByaWdodHMgcmVzZXJ2ZWQuXG4gKiBUaGlzIGNvZGUgbWF5IG9ubHkgYmUgdXNlZCB1bmRlciB0aGUgQlNEIHN0eWxlIGxpY2Vuc2UgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9MSUNFTlNFLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBhdXRob3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0FVVEhPUlMudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGNvbnRyaWJ1dG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9DT05UUklCVVRPUlMudHh0XG4gKiBDb2RlIGRpc3RyaWJ1dGVkIGJ5IEdvb2dsZSBhcyBwYXJ0IG9mIHRoZSBwb2x5bWVyIHByb2plY3QgaXMgYWxzb1xuICogc3ViamVjdCB0byBhbiBhZGRpdGlvbmFsIElQIHJpZ2h0cyBncmFudCBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL1BBVEVOVFMudHh0XG4gKi9cbi8qKlxuICpcbiAqIE1haW4gbGl0LWh0bWwgbW9kdWxlLlxuICpcbiAqIE1haW4gZXhwb3J0czpcbiAqXG4gKiAtICBbW2h0bWxdXVxuICogLSAgW1tzdmddXVxuICogLSAgW1tyZW5kZXJdXVxuICpcbiAqIEBtb2R1bGUgbGl0LWh0bWxcbiAqIEBwcmVmZXJyZWRcbiAqL1xuLyoqXG4gKiBEbyBub3QgcmVtb3ZlIHRoaXMgY29tbWVudDsgaXQga2VlcHMgdHlwZWRvYyBmcm9tIG1pc3BsYWNpbmcgdGhlIG1vZHVsZVxuICogZG9jcy5cbiAqL1xuaW1wb3J0IHsgZGVmYXVsdFRlbXBsYXRlUHJvY2Vzc29yIH0gZnJvbSAnLi9saWIvZGVmYXVsdC10ZW1wbGF0ZS1wcm9jZXNzb3IuanMnO1xuaW1wb3J0IHsgU1ZHVGVtcGxhdGVSZXN1bHQsIFRlbXBsYXRlUmVzdWx0IH0gZnJvbSAnLi9saWIvdGVtcGxhdGUtcmVzdWx0LmpzJztcbmV4cG9ydCB7IERlZmF1bHRUZW1wbGF0ZVByb2Nlc3NvciwgZGVmYXVsdFRlbXBsYXRlUHJvY2Vzc29yIH0gZnJvbSAnLi9saWIvZGVmYXVsdC10ZW1wbGF0ZS1wcm9jZXNzb3IuanMnO1xuZXhwb3J0IHsgZGlyZWN0aXZlLCBpc0RpcmVjdGl2ZSB9IGZyb20gJy4vbGliL2RpcmVjdGl2ZS5qcyc7XG4vLyBUT0RPKGp1c3RpbmZhZ25hbmkpOiByZW1vdmUgbGluZSB3aGVuIHdlIGdldCBOb2RlUGFydCBtb3ZpbmcgbWV0aG9kc1xuZXhwb3J0IHsgcmVtb3ZlTm9kZXMsIHJlcGFyZW50Tm9kZXMgfSBmcm9tICcuL2xpYi9kb20uanMnO1xuZXhwb3J0IHsgbm9DaGFuZ2UsIG5vdGhpbmcgfSBmcm9tICcuL2xpYi9wYXJ0LmpzJztcbmV4cG9ydCB7IEF0dHJpYnV0ZUNvbW1pdHRlciwgQXR0cmlidXRlUGFydCwgQm9vbGVhbkF0dHJpYnV0ZVBhcnQsIEV2ZW50UGFydCwgaXNQcmltaXRpdmUsIE5vZGVQYXJ0LCBQcm9wZXJ0eUNvbW1pdHRlciwgUHJvcGVydHlQYXJ0IH0gZnJvbSAnLi9saWIvcGFydHMuanMnO1xuZXhwb3J0IHsgcGFydHMsIHJlbmRlciB9IGZyb20gJy4vbGliL3JlbmRlci5qcyc7XG5leHBvcnQgeyB0ZW1wbGF0ZUNhY2hlcywgdGVtcGxhdGVGYWN0b3J5IH0gZnJvbSAnLi9saWIvdGVtcGxhdGUtZmFjdG9yeS5qcyc7XG5leHBvcnQgeyBUZW1wbGF0ZUluc3RhbmNlIH0gZnJvbSAnLi9saWIvdGVtcGxhdGUtaW5zdGFuY2UuanMnO1xuZXhwb3J0IHsgU1ZHVGVtcGxhdGVSZXN1bHQsIFRlbXBsYXRlUmVzdWx0IH0gZnJvbSAnLi9saWIvdGVtcGxhdGUtcmVzdWx0LmpzJztcbmV4cG9ydCB7IGNyZWF0ZU1hcmtlciwgaXNUZW1wbGF0ZVBhcnRBY3RpdmUsIFRlbXBsYXRlIH0gZnJvbSAnLi9saWIvdGVtcGxhdGUuanMnO1xuLy8gSU1QT1JUQU5UOiBkbyBub3QgY2hhbmdlIHRoZSBwcm9wZXJ0eSBuYW1lIG9yIHRoZSBhc3NpZ25tZW50IGV4cHJlc3Npb24uXG4vLyBUaGlzIGxpbmUgd2lsbCBiZSB1c2VkIGluIHJlZ2V4ZXMgdG8gc2VhcmNoIGZvciBsaXQtaHRtbCB1c2FnZS5cbi8vIFRPRE8oanVzdGluZmFnbmFuaSk6IGluamVjdCB2ZXJzaW9uIG51bWJlciBhdCBidWlsZCB0aW1lXG4od2luZG93WydsaXRIdG1sVmVyc2lvbnMnXSB8fCAod2luZG93WydsaXRIdG1sVmVyc2lvbnMnXSA9IFtdKSkucHVzaCgnMS4wLjAnKTtcbi8qKlxuICogSW50ZXJwcmV0cyBhIHRlbXBsYXRlIGxpdGVyYWwgYXMgYW4gSFRNTCB0ZW1wbGF0ZSB0aGF0IGNhbiBlZmZpY2llbnRseVxuICogcmVuZGVyIHRvIGFuZCB1cGRhdGUgYSBjb250YWluZXIuXG4gKi9cbmV4cG9ydCBjb25zdCBodG1sID0gKHN0cmluZ3MsIC4uLnZhbHVlcykgPT4gbmV3IFRlbXBsYXRlUmVzdWx0KHN0cmluZ3MsIHZhbHVlcywgJ2h0bWwnLCBkZWZhdWx0VGVtcGxhdGVQcm9jZXNzb3IpO1xuLyoqXG4gKiBJbnRlcnByZXRzIGEgdGVtcGxhdGUgbGl0ZXJhbCBhcyBhbiBTVkcgdGVtcGxhdGUgdGhhdCBjYW4gZWZmaWNpZW50bHlcbiAqIHJlbmRlciB0byBhbmQgdXBkYXRlIGEgY29udGFpbmVyLlxuICovXG5leHBvcnQgY29uc3Qgc3ZnID0gKHN0cmluZ3MsIC4uLnZhbHVlcykgPT4gbmV3IFNWR1RlbXBsYXRlUmVzdWx0KHN0cmluZ3MsIHZhbHVlcywgJ3N2ZycsIGRlZmF1bHRUZW1wbGF0ZVByb2Nlc3Nvcik7XG4vLyMgc291cmNlTWFwcGluZ1VSTD1saXQtaHRtbC5qcy5tYXAiLCJpbXBvcnQgeyBMaXRFbGVtZW50LCBodG1sLCBwcm9wZXJ0eSwgY3VzdG9tRWxlbWVudCwgY3NzLCB1bnNhZmVDU1MgfSBmcm9tICdsaXQtZWxlbWVudCc7XHJcbmltcG9ydCB7IE11dGF0aW9uVGVzdFJlc3VsdCB9IGZyb20gJy4uLy4uL2FwaSc7XHJcbmltcG9ydCB7IGlzRGlyZWN0b3J5UmVzdWx0IH0gZnJvbSAnLi4vaGVscGVycyc7XHJcbmltcG9ydCB7IGJvb3RzdHJhcCB9IGZyb20gJy4uL3N0eWxlJztcclxuXHJcbkBjdXN0b21FbGVtZW50KCdtdXRhdGlvbi10ZXN0LXJlcG9ydC1hcHAnKVxyXG5leHBvcnQgY2xhc3MgTXV0YXRpb25UZXN0UmVwb3J0QXBwQ29tcG9uZW50IGV4dGVuZHMgTGl0RWxlbWVudCB7XHJcbiAgcHJpdmF0ZSByZXBvcnQ6IE11dGF0aW9uVGVzdFJlc3VsdCB8IHVuZGVmaW5lZDtcclxuXHJcbiAgQHByb3BlcnR5KClcclxuICBwdWJsaWMgc3JjOiBzdHJpbmcgfCB1bmRlZmluZWQ7XHJcblxyXG4gIEBwcm9wZXJ0eSgpXHJcbiAgcHVibGljIGVycm9yTWVzc2FnZTogc3RyaW5nIHwgdW5kZWZpbmVkO1xyXG5cclxuICBAcHJvcGVydHkoKVxyXG4gIHB1YmxpYyBjb250ZXh0OiBNdXRhdGlvblRlc3RSZXN1bHQgfCB1bmRlZmluZWQ7XHJcblxyXG4gIEBwcm9wZXJ0eSgpXHJcbiAgcHVibGljIHBhdGg6IFJlYWRvbmx5QXJyYXk8c3RyaW5nPiB8IHVuZGVmaW5lZDtcclxuXHJcbiAgcHVibGljIGNvbm5lY3RlZENhbGxiYWNrKCkge1xyXG4gICAgc3VwZXIuY29ubmVjdGVkQ2FsbGJhY2soKTtcclxuICAgIGlmICh0aGlzLnNyYykge1xyXG4gICAgICB0aGlzLmxvYWREYXRhKHRoaXMuc3JjKVxyXG4gICAgICAgIC5jYXRjaChlcnJvciA9PlxyXG4gICAgICAgICAgdGhpcy5lcnJvck1lc3NhZ2UgPSBlcnJvci50b1N0cmluZygpKTtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgIHRoaXMuZXJyb3JNZXNzYWdlID0gJ1NvdXJjZSBub3Qgc2V0LiBQbGVhc2UgcG9pbnQgdGhlIGBzcmNgIGF0dHJpYnV0ZSB0byB0aGUgbXV0YXRpb24gdGVzdCByZXBvcnQgZGF0YS4nO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSBhc3luYyBsb2FkRGF0YShzcmM6IHN0cmluZykge1xyXG4gICAgY29uc3QgcmVzID0gYXdhaXQgZmV0Y2goc3JjKTtcclxuICAgIHRoaXMucmVwb3J0ID0gYXdhaXQgcmVzLmpzb24oKTtcclxuICAgIHRoaXMudXBkYXRlQ29udGV4dCgpO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSB1cGRhdGVDb250ZXh0KCkge1xyXG4gICAgaWYgKHRoaXMucGF0aCkge1xyXG4gICAgICBjb25zdCBwYXRoUXVldWUgPSB0aGlzLnBhdGguc2xpY2UoKTtcclxuICAgICAgbGV0IG5ld0NvbnRleHQ6IE11dGF0aW9uVGVzdFJlc3VsdCB8IHVuZGVmaW5lZCA9IHRoaXMucmVwb3J0O1xyXG4gICAgICBsZXQgcGF0aFBhcnQ6IHN0cmluZyB8IHVuZGVmaW5lZDtcclxuXHJcbiAgICAgIHdoaWxlIChwYXRoUGFydCA9IHBhdGhRdWV1ZS5zaGlmdCgpKSB7XHJcbiAgICAgICAgaWYgKGlzRGlyZWN0b3J5UmVzdWx0KG5ld0NvbnRleHQpKSB7XHJcbiAgICAgICAgICBuZXdDb250ZXh0ID0gbmV3Q29udGV4dC5jaGlsZFJlc3VsdHMuZmluZChjaGlsZCA9PiBjaGlsZC5uYW1lID09PSBwYXRoUGFydCk7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgIG5ld0NvbnRleHQgPSB1bmRlZmluZWQ7XHJcbiAgICAgICAgICBicmVhaztcclxuICAgICAgICB9XHJcbiAgICAgIH1cclxuICAgICAgdGhpcy5jb250ZXh0ID0gbmV3Q29udGV4dDtcclxuICAgICAgaWYgKCF0aGlzLmNvbnRleHQpIHtcclxuICAgICAgICB0aGlzLmVycm9yTWVzc2FnZSA9IGA0MDQgLSAke3RoaXMucGF0aC5qb2luKCcvJyl9IG5vdCBmb3VuZGA7XHJcbiAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgdGhpcy5lcnJvck1lc3NhZ2UgPSB1bmRlZmluZWQ7XHJcbiAgICAgIH1cclxuICAgIH1cclxuICB9XHJcblxyXG4gIHByaXZhdGUgcmVhZG9ubHkgdXBkYXRlUGF0aCA9IChldmVudDogQ3VzdG9tRXZlbnQpID0+IHtcclxuICAgIHRoaXMucGF0aCA9IGV2ZW50LmRldGFpbDtcclxuICAgIHRoaXMudXBkYXRlQ29udGV4dCgpO1xyXG4gIH1cclxuXHJcbiAgcHVibGljIHN0YXRpYyBzdHlsZXMgPSBbXHJcbiAgICBib290c3RyYXAsXHJcbiAgICBjc3NgXHJcbiAgICA6aG9zdCB7XHJcbiAgICAgIGxpbmUtaGVpZ2h0OiAxLjE1O1xyXG4gICAgICBtYXJnaW46IDA7XHJcbiAgICAgIGZvbnQtZmFtaWx5OiAtYXBwbGUtc3lzdGVtLCBCbGlua01hY1N5c3RlbUZvbnQsIFwiU2Vnb2UgVUlcIiwgUm9ib3RvLCBcIkhlbHZldGljYSBOZXVlXCIsIEFyaWFsLCBcIk5vdG8gU2Fuc1wiLCBzYW5zLXNlcmlmLCBcIkFwcGxlIENvbG9yIEVtb2ppXCIsIFwiU2Vnb2UgVUkgRW1vamlcIiwgXCJTZWdvZSBVSSBTeW1ib2xcIiwgXCJOb3RvIENvbG9yIEVtb2ppXCI7XHJcbiAgICAgIGZvbnQtc2l6ZTogMXJlbTtcclxuICAgICAgZm9udC13ZWlnaHQ6IDQwMDtcclxuICAgICAgbGluZS1oZWlnaHQ6IDEuNTtcclxuICAgICAgY29sb3I6ICMyMTI1Mjk7XHJcbiAgICAgIHRleHQtYWxpZ246IGxlZnQ7XHJcbiAgICAgIGJhY2tncm91bmQtY29sb3I6ICNmZmY7XHJcbiAgICB9XHJcbiAgICBgXHJcbiAgXTtcclxuXHJcbiAgcHVibGljIHJlbmRlcigpIHtcclxuICAgIHJldHVybiBodG1sYFxyXG4gICAgPG11dGF0aW9uLXRlc3QtcmVwb3J0LXJvdXRlciBAcGF0aC1jaGFuZ2VkPVwiJHt0aGlzLnVwZGF0ZVBhdGh9XCI+PC9tdXRhdGlvbi10ZXN0LXJlcG9ydC1yb3V0ZXI+XHJcbiAgICA8ZGl2IGNsYXNzPVwiY29udGFpbmVyXCI+XHJcbiAgICAgIDxkaXYgY2xhc3M9XCJyb3dcIj5cclxuICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLW1kLTEyXCI+XHJcbiAgICAgICAgICAke3RoaXMucmVuZGVyVGl0bGUoKX1cclxuICAgICAgICAgIDxtdXRhdGlvbi10ZXN0LXJlcG9ydC1icmVhZGNydW1iIC5wYXRoPVwiJHt0aGlzLnBhdGh9XCI+PC9tdXRhdGlvbi10ZXN0LXJlcG9ydC1icmVhZGNydW1iPlxyXG4gICAgICAgICAgJHt0aGlzLnJlbmRlckVycm9yTWVzc2FnZSgpfVxyXG4gICAgICAgICAgJHt0aGlzLnJlbmRlck11dGF0aW9uVGVzdFJlcG9ydCgpfVxyXG4gICAgICAgIDwvZGl2PlxyXG4gICAgICA8L2Rpdj5cclxuICAgIDwvZGl2PlxyXG4gICAgYDtcclxuICB9XHJcblxyXG4gIHByaXZhdGUgcmVuZGVyVGl0bGUoKSB7XHJcbiAgICBpZiAodGhpcy5jb250ZXh0KSB7XHJcbiAgICAgIHJldHVybiBodG1sYDxoMSBjbGFzcz1cImRpc3BsYXktMVwiPiR7dGhpcy5jb250ZXh0Lm5hbWV9PC9oMT5gO1xyXG4gICAgfSBlbHNlIHtcclxuICAgICAgcmV0dXJuIHVuZGVmaW5lZDtcclxuICAgIH1cclxuICB9XHJcblxyXG4gIHByaXZhdGUgcmVuZGVyRXJyb3JNZXNzYWdlKCkge1xyXG4gICAgaWYgKHRoaXMuZXJyb3JNZXNzYWdlKSB7XHJcbiAgICAgIHJldHVybiBodG1sYFxyXG4gICAgICA8ZGl2IGNsYXNzPVwiYWxlcnQgYWxlcnQtZGFuZ2VyXCIgcm9sZT1cImFsZXJ0XCI+XHJcbiAgICAgICAgJHt0aGlzLmVycm9yTWVzc2FnZX1cclxuICAgICAgPC9kaXY+XHJcbiAgICAgICAgYDtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgIHJldHVybiBodG1sYGA7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlck11dGF0aW9uVGVzdFJlcG9ydCgpIHtcclxuICAgIGlmICh0aGlzLmNvbnRleHQpIHtcclxuICAgICAgcmV0dXJuIGh0bWxgPG11dGF0aW9uLXRlc3QtcmVwb3J0LXJlc3VsdCAuY3VycmVudFBhdGg9XCIke3RoaXMucGF0aH1cIiAubW9kZWw9XCIke3RoaXMuY29udGV4dH1cIj48L211dGF0aW9uLXRlc3QtcmVwb3J0LXJlc3VsdD5gO1xyXG4gICAgfSBlbHNlIHtcclxuICAgICAgcmV0dXJuICcnO1xyXG4gICAgfVxyXG4gIH1cclxufVxyXG4iLCJpbXBvcnQgeyBMaXRFbGVtZW50LCBodG1sLCBwcm9wZXJ0eSwgY3VzdG9tRWxlbWVudCB9IGZyb20gJ2xpdC1lbGVtZW50JztcclxuaW1wb3J0IHsgYm9vdHN0cmFwIH0gZnJvbSAnLi4vc3R5bGUnO1xyXG5cclxuQGN1c3RvbUVsZW1lbnQoJ211dGF0aW9uLXRlc3QtcmVwb3J0LWJyZWFkY3J1bWInKVxyXG5leHBvcnQgY2xhc3MgTXV0YXRpb25UZXN0UmVwb3J0QnJlYWRjcnVtYkNvbXBvbmVudCBleHRlbmRzIExpdEVsZW1lbnQge1xyXG4gIEBwcm9wZXJ0eSgpXHJcbiAgcHVibGljIHBhdGg6IFJlYWRvbmx5QXJyYXk8c3RyaW5nPiB8IHVuZGVmaW5lZDtcclxuXHJcbiAgcHVibGljIHN0YXRpYyBzdHlsZXMgPSBbYm9vdHN0cmFwXTtcclxuXHJcbiAgcHVibGljIHJlbmRlcigpIHtcclxuICAgIHJldHVybiBodG1sYFxyXG4gICAgICAgIDxvbCBjbGFzcz0nYnJlYWRjcnVtYic+XHJcbiAgICAgICAgICAke3RoaXMucmVuZGVyUm9vdEl0ZW0oKX1cclxuICAgICAgICAgICR7dGhpcy5wYXRoID8gdGhpcy5yZW5kZXJCcmVhZGNydW1iSXRlbXModGhpcy5wYXRoKSA6ICcnfVxyXG4gICAgICAgIDwvb2w+XHJcbiAgICBgO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSByZW5kZXJSb290SXRlbSgpIHtcclxuICAgIGlmICh0aGlzLnBhdGggJiYgdGhpcy5wYXRoLmxlbmd0aCkge1xyXG4gICAgICByZXR1cm4gdGhpcy5yZW5kZXJMaW5rKCdBbGwgZmlsZXMnLCAnIycpO1xyXG4gICAgfSBlbHNlIHtcclxuICAgICAgcmV0dXJuIHRoaXMucmVuZGVyQWN0aXZlSXRlbSgnQWxsIGZpbGVzJyk7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlckJyZWFkY3J1bWJJdGVtcyhwYXRoOiBSZWFkb25seUFycmF5PHN0cmluZz4pIHtcclxuICAgIHJldHVybiBwYXRoLm1hcCgoaXRlbSwgaW5kZXgpID0+IHtcclxuICAgICAgaWYgKGluZGV4ID09PSBwYXRoLmxlbmd0aCAtIDEpIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5yZW5kZXJBY3RpdmVJdGVtKGl0ZW0pO1xyXG4gICAgICB9IGVsc2Uge1xyXG4gICAgICAgIHJldHVybiB0aGlzLnJlbmRlckxpbmsoaXRlbSwgYCMke3BhdGguZmlsdGVyKChfLCBpKSA9PiBpIDw9IGluZGV4KS5qb2luKCcvJyl9YCk7XHJcbiAgICAgIH1cclxuICAgIH0pO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSByZW5kZXJBY3RpdmVJdGVtKHRpdGxlOiBzdHJpbmcpIHtcclxuICAgIHJldHVybiBodG1sYDxsaSBjbGFzcz1cImJyZWFkY3J1bWItaXRlbSBhY3RpdmVcIiBhcmlhLWN1cnJlbnQ9XCJwYWdlXCI+JHt0aXRsZX08L2xpPmA7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlckxpbmsodGl0bGU6IHN0cmluZywgdXJsOiBzdHJpbmcpIHtcclxuICAgIHJldHVybiBodG1sYDxsaSBjbGFzcz1cImJyZWFkY3J1bWItaXRlbVwiPjxhIGhyZWY9XCIke3VybH1cIj4ke3RpdGxlfTwvYT48L2xpPmA7XHJcbiAgfVxyXG59XHJcbiIsImltcG9ydCB7IGN1c3RvbUVsZW1lbnQsIExpdEVsZW1lbnQsIHByb3BlcnR5LCBQcm9wZXJ0eVZhbHVlcywgaHRtbCwgY3NzIH0gZnJvbSAnbGl0LWVsZW1lbnQnO1xyXG5pbXBvcnQgeyBNdXRhbnRSZXN1bHQsIE11dGFudFN0YXR1cyB9IGZyb20gJy4uLy4uL2FwaSc7XHJcbmltcG9ydCB7IGJvb3RzdHJhcCB9IGZyb20gJy4uL3N0eWxlJztcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgTXV0YW50RmlsdGVyIHtcclxuICBzdGF0dXM6IE11dGFudFN0YXR1cztcclxuICBudW1iZXJPZk11dGFudHM6IG51bWJlcjtcclxuICBlbmFibGVkOiBib29sZWFuO1xyXG59XHJcblxyXG5AY3VzdG9tRWxlbWVudCgnbXV0YXRpb24tdGVzdC1yZXBvcnQtZmlsZS1sZWdlbmQnKVxyXG5leHBvcnQgY2xhc3MgTXV0YXRpb25UZXN0UmVwb3J0RmlsZUxlZ2VuZENvbXBvbmVudCBleHRlbmRzIExpdEVsZW1lbnQge1xyXG5cclxuICBAcHJvcGVydHkoKVxyXG4gIHB1YmxpYyBtdXRhbnRzITogUmVhZG9ubHlBcnJheTxNdXRhbnRSZXN1bHQ+O1xyXG5cclxuICBAcHJvcGVydHkoKVxyXG4gIHByaXZhdGUgZ2V0IGNvbGxhcHNlQnV0dG9uVGV4dCgpIHtcclxuICAgIGlmICh0aGlzLmNvbGxhcHNlZCkge1xyXG4gICAgICByZXR1cm4gJ0V4cGFuZCBhbGwnO1xyXG4gICAgfSBlbHNlIHtcclxuICAgICAgcmV0dXJuICdDb2xsYXBzZSBhbGwnO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgQHByb3BlcnR5KClcclxuICBwcml2YXRlIGNvbGxhcHNlZCA9IHRydWU7XHJcblxyXG4gIEBwcm9wZXJ0eSgpXHJcbiAgcHJpdmF0ZSBmaWx0ZXJzOiBNdXRhbnRGaWx0ZXJbXSA9IFtdO1xyXG5cclxuICBwdWJsaWMgdXBkYXRlZChjaGFuZ2VkUHJvcGVydGllczogUHJvcGVydHlWYWx1ZXMpIHtcclxuICAgIGlmIChjaGFuZ2VkUHJvcGVydGllcy5oYXMoJ211dGFudHMnKSkge1xyXG4gICAgICB0aGlzLnVwZGF0ZU1vZGVsKCk7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHVwZGF0ZU1vZGVsKCkge1xyXG4gICAgdGhpcy5maWx0ZXJzID0gW011dGFudFN0YXR1cy5LaWxsZWQsIE11dGFudFN0YXR1cy5TdXJ2aXZlZCwgTXV0YW50U3RhdHVzLk5vQ292ZXJhZ2UsIE11dGFudFN0YXR1cy5UaW1lb3V0LCBNdXRhbnRTdGF0dXMuQ29tcGlsZUVycm9yLCBNdXRhbnRTdGF0dXMuUnVudGltZUVycm9yXVxyXG4gICAgICAubWFwKHN0YXR1cyA9PiAoe1xyXG4gICAgICAgIGVuYWJsZWQ6IFtNdXRhbnRTdGF0dXMuU3Vydml2ZWQsIE11dGFudFN0YXR1cy5Ob0NvdmVyYWdlLCBNdXRhbnRTdGF0dXMuVGltZW91dF0uc29tZShzID0+IHMgPT09IHN0YXR1cyksXHJcbiAgICAgICAgbnVtYmVyT2ZNdXRhbnRzOiB0aGlzLm11dGFudHMuZmlsdGVyKG0gPT4gbS5zdGF0dXMgPT09IHN0YXR1cykubGVuZ3RoLFxyXG4gICAgICAgIHN0YXR1c1xyXG4gICAgICB9KSk7XHJcbiAgICB0aGlzLmRpc3BhdGNoRmlsdGVyc0NoYW5nZWRFdmVudCgpO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSBjaGVja2JveENsaWNrZWQoZmlsdGVyOiBNdXRhbnRGaWx0ZXIpIHtcclxuICAgIGZpbHRlci5lbmFibGVkID0gIWZpbHRlci5lbmFibGVkO1xyXG4gICAgdGhpcy5kaXNwYXRjaEZpbHRlcnNDaGFuZ2VkRXZlbnQoKTtcclxuICB9XHJcblxyXG4gIHByaXZhdGUgZGlzcGF0Y2hGaWx0ZXJzQ2hhbmdlZEV2ZW50KCkge1xyXG4gICAgdGhpcy5kaXNwYXRjaEV2ZW50KG5ldyBDdXN0b21FdmVudCgnZmlsdGVycy1jaGFuZ2VkJywgeyBkZXRhaWw6IHRoaXMuZmlsdGVycyB9KSk7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlYWRvbmx5IHRvZ2dsZU9wZW5BbGwgPSAoKSA9PiB7XHJcbiAgICB0aGlzLmNvbGxhcHNlZCA9ICF0aGlzLmNvbGxhcHNlZDtcclxuICAgIGlmICh0aGlzLmNvbGxhcHNlZCkge1xyXG4gICAgICB0aGlzLmRpc3BhdGNoRXZlbnQobmV3IEN1c3RvbUV2ZW50KCdjb2xsYXBzZS1hbGwnKSk7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICB0aGlzLmRpc3BhdGNoRXZlbnQobmV3IEN1c3RvbUV2ZW50KCdvcGVuLWFsbCcpKTtcclxuICAgIH1cclxuICB9XHJcblxyXG4gIHB1YmxpYyBzdGF0aWMgc3R5bGVzID0gW1xyXG4gICAgY3NzYFxyXG4gICAgICAubGVnZW5ke1xyXG4gICAgICAgIHBvc2l0aW9uOiBzdGlja3k7XHJcbiAgICAgICAgdG9wOiAwO1xyXG4gICAgICAgIGJhY2tncm91bmQ6ICNGRkY7XHJcbiAgICAgIH1cclxuICBgLCBib290c3RyYXBdO1xyXG5cclxuICBwdWJsaWMgcmVuZGVyKCkge1xyXG4gICAgcmV0dXJuIGh0bWxgXHJcbiAgICAgIDxkaXYgY2xhc3M9J3JvdyBsZWdlbmQnPlxyXG4gICAgICAgIDxmb3JtIGNsYXNzPSdjb2wtbWQtMTInIG5vdmFsaWRhdGU9J25vdmFsaWRhdGUnPlxyXG4gICAgICAgICAgJHt0aGlzLmZpbHRlcnMubWFwKGZpbHRlciA9PiBodG1sYFxyXG4gICAgICAgICAgPGRpdiBjbGFzcz1cImZvcm0tY2hlY2sgZm9ybS1jaGVjay1pbmxpbmVcIj5cclxuICAgICAgICAgICAgPGxhYmVsIGNsYXNzPVwiZm9ybS1jaGVjay1sYWJlbFwiPlxyXG4gICAgICAgICAgICAgIDxpbnB1dCBjbGFzcz1cImZvcm0tY2hlY2staW5wdXRcIiB0eXBlPVwiY2hlY2tib3hcIiA/Y2hlY2tlZD1cIiR7ZmlsdGVyLmVuYWJsZWR9XCIgdmFsdWU9XCIke2ZpbHRlci5zdGF0dXN9XCIgQGlucHV0PVwiJHsoKSA9PiB0aGlzLmNoZWNrYm94Q2xpY2tlZChmaWx0ZXIpfVwiPlxyXG4gICAgICAgICAgICAgICR7ZmlsdGVyLnN0YXR1c30gKCR7ZmlsdGVyLm51bWJlck9mTXV0YW50c30pXHJcbiAgICAgICAgICAgIDwvbGFiZWw+XHJcbiAgICAgICAgICA8L2Rpdj5cclxuICAgICAgICAgIGApfVxyXG4gICAgICAgICAgPGJ1dHRvbiBAY2xpY2s9XCIke3RoaXMudG9nZ2xlT3BlbkFsbH1cIiBjbGFzcz1cImJ0biBidG4tc20gYnRuLXNlY29uZGFyeVwiIHJvbGU9XCJsaW5rXCI+JHt0aGlzLmNvbGxhcHNlQnV0dG9uVGV4dH08L2J1dHRvbj5cclxuICAgICAgICA8L2Zvcm0+XHJcbiAgICAgIDwvZGl2PlxyXG4gICAgYDtcclxuICB9XHJcblxyXG59XHJcbiIsImltcG9ydCB7IExpdEVsZW1lbnQsIGh0bWwsIHByb3BlcnR5LCBjdXN0b21FbGVtZW50LCBjc3MsIFByb3BlcnR5VmFsdWVzLCB1bnNhZmVDU1MgfSBmcm9tICdsaXQtZWxlbWVudCc7XHJcbmltcG9ydCB7IEZpbGVSZXN1bHQsIE11dGFudFN0YXR1cywgTXV0YW50UmVzdWx0IH0gZnJvbSAnLi4vLi4vYXBpJztcclxuaW1wb3J0IGhsanMgZnJvbSAnaGlnaGxpZ2h0LmpzL2xpYi9oaWdobGlnaHQnO1xyXG5pbXBvcnQgamF2YXNjcmlwdCBmcm9tICdoaWdobGlnaHQuanMvbGliL2xhbmd1YWdlcy9qYXZhc2NyaXB0JztcclxuaW1wb3J0IHNjYWxhIGZyb20gJ2hpZ2hsaWdodC5qcy9saWIvbGFuZ3VhZ2VzL3NjYWxhJztcclxuaW1wb3J0IGphdmEgZnJvbSAnaGlnaGxpZ2h0LmpzL2xpYi9sYW5ndWFnZXMvamF2YSc7XHJcbmltcG9ydCBjcyBmcm9tICdoaWdobGlnaHQuanMvbGliL2xhbmd1YWdlcy9jcyc7XHJcbmltcG9ydCB0eXBlc2NyaXB0IGZyb20gJ2hpZ2hsaWdodC5qcy9saWIvbGFuZ3VhZ2VzL3R5cGVzY3JpcHQnO1xyXG5pbXBvcnQgeyBnZXRDb250ZXh0Q2xhc3NGb3JTdGF0dXMsIExJTkVfU1RBUlRfSU5ERVgsIENPTFVNTl9TVEFSVF9JTkRFWCwgbGluZXMgYXMgdG9MaW5lcywgZXNjYXBlSHRtbCwgTkVXX0xJTkUsIENBUlJJQUdFX1JFVFVSTiB9IGZyb20gJy4uL2hlbHBlcnMnO1xyXG5pbXBvcnQgeyBNdXRhdGlvblRlc3RSZXBvcnRNdXRhbnRDb21wb25lbnQgfSBmcm9tICcuL211dGF0aW9uLXRlc3QtcmVwb3J0LW11dGFudCc7XHJcbmltcG9ydCB7IE11dGFudEZpbHRlciB9IGZyb20gJy4vbXV0YXRpb24tdGVzdC1yZXBvcnQtZmlsZS1sZWdlbmQnO1xyXG5pbXBvcnQgeyBib290c3RyYXAsIGhpZ2hsaWdodEpTIH0gZnJvbSAnLi4vc3R5bGUnO1xyXG5obGpzLnJlZ2lzdGVyTGFuZ3VhZ2UoJ2phdmFzY3JpcHQnLCBqYXZhc2NyaXB0KTtcclxuaGxqcy5yZWdpc3Rlckxhbmd1YWdlKCd0eXBlc2NyaXB0JywgdHlwZXNjcmlwdCk7XHJcbmhsanMucmVnaXN0ZXJMYW5ndWFnZSgnY3MnLCBjcyk7XHJcbmhsanMucmVnaXN0ZXJMYW5ndWFnZSgnamF2YScsIGphdmEpO1xyXG5obGpzLnJlZ2lzdGVyTGFuZ3VhZ2UoJ3NjYWxhJywgc2NhbGEpO1xyXG5cclxuQGN1c3RvbUVsZW1lbnQoJ211dGF0aW9uLXRlc3QtcmVwb3J0LWZpbGUnKVxyXG5leHBvcnQgY2xhc3MgTXV0YXRpb25UZXN0UmVwb3J0RmlsZUNvbXBvbmVudCBleHRlbmRzIExpdEVsZW1lbnQge1xyXG5cclxuICBAcHJvcGVydHkoKVxyXG4gIHByaXZhdGUgcmVhZG9ubHkgbW9kZWwhOiBGaWxlUmVzdWx0O1xyXG5cclxuICBwdWJsaWMgc3RhdGljIHN0eWxlcyA9IFtcclxuICAgIGhpZ2hsaWdodEpTLFxyXG4gICAgYm9vdHN0cmFwLFxyXG4gICAgY3NzYFxyXG4gICAgLmJnLWRhbmdlci1saWdodCB7XHJcbiAgICAgIGJhY2tncm91bmQtY29sb3I6ICNmMmRlZGU7XHJcbiAgICB9XHJcbiAgICAuYmctc3VjY2Vzcy1saWdodCB7XHJcbiAgICAgICAgYmFja2dyb3VuZC1jb2xvcjogI2RmZjBkODtcclxuICAgIH1cclxuICAgIC5iZy13YXJuaW5nLWxpZ2h0IHtcclxuICAgICAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmNmOGUzO1xyXG4gICAgfVxyXG4gICAgYF07XHJcblxyXG4gIHByaXZhdGUgcmVhZG9ubHkgb3BlbkFsbCA9ICgpID0+IHtcclxuICAgIHRoaXMuZm9yRWFjaE11dGFudENvbXBvbmVudChtdXRhbnRDb21wb25lbnQgPT4gbXV0YW50Q29tcG9uZW50Lm9wZW4gPSB0cnVlKTtcclxuICB9XHJcbiAgcHJpdmF0ZSByZWFkb25seSBjb2xsYXBzZUFsbCA9ICgpID0+IHtcclxuICAgIHRoaXMuZm9yRWFjaE11dGFudENvbXBvbmVudChtdXRhbnRDb21wb25lbnQgPT4gbXV0YW50Q29tcG9uZW50Lm9wZW4gPSBmYWxzZSk7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIGZvckVhY2hNdXRhbnRDb21wb25lbnQoYWN0aW9uOiAobXV0YW50OiBNdXRhdGlvblRlc3RSZXBvcnRNdXRhbnRDb21wb25lbnQpID0+IHZvaWQpIHtcclxuICAgIGZvciAoY29uc3QgbXV0YW50Q29tcG9uZW50IG9mIHRoaXMucm9vdC5xdWVyeVNlbGVjdG9yQWxsKCdtdXRhdGlvbi10ZXN0LXJlcG9ydC1tdXRhbnQnKSkge1xyXG4gICAgICBpZiAobXV0YW50Q29tcG9uZW50IGluc3RhbmNlb2YgTXV0YXRpb25UZXN0UmVwb3J0TXV0YW50Q29tcG9uZW50KSB7XHJcbiAgICAgICAgYWN0aW9uKG11dGFudENvbXBvbmVudCk7XHJcbiAgICAgIH1cclxuICAgIH1cclxuICB9XHJcblxyXG4gIHByaXZhdGUgcmVhZG9ubHkgZmlsdGVyc0NoYW5nZWQgPSAoZXZlbnQ6IEN1c3RvbUV2ZW50PE11dGFudEZpbHRlcltdPikgPT4ge1xyXG4gICAgY29uc3QgZW5hYmxlZE11dGFudFN0YXRlcyA9IGV2ZW50LmRldGFpbFxyXG4gICAgICAuZmlsdGVyKG11dGFudEZpbHRlciA9PiBtdXRhbnRGaWx0ZXIuZW5hYmxlZClcclxuICAgICAgLm1hcChtdXRhbnRGaWx0ZXIgPT4gbXV0YW50RmlsdGVyLnN0YXR1cyk7XHJcbiAgICB0aGlzLmZvckVhY2hNdXRhbnRDb21wb25lbnQobXV0YW50Q29tcG9uZW50ID0+IHtcclxuICAgICAgbXV0YW50Q29tcG9uZW50LnNob3cgPSBlbmFibGVkTXV0YW50U3RhdGVzLnNvbWUoc3RhdGUgPT4gbXV0YW50Q29tcG9uZW50LnN0YXR1cyA9PT0gc3RhdGUpO1xyXG4gICAgfSk7XHJcbiAgfVxyXG5cclxuICBwdWJsaWMgcmVuZGVyKCkge1xyXG4gICAgcmV0dXJuIGh0bWxgXHJcbiAgICAgICAgICA8bXV0YXRpb24tdGVzdC1yZXBvcnQtZmlsZS1sZWdlbmQgQGZpbHRlcnMtY2hhbmdlZD1cIiR7dGhpcy5maWx0ZXJzQ2hhbmdlZH1cIiBAb3Blbi1hbGw9XCIke3RoaXMub3BlbkFsbH1cIiBAY29sbGFwc2UtYWxsPVwiJHt0aGlzLmNvbGxhcHNlQWxsfVwiXHJcbiAgICAgICAgICAgIC5tdXRhbnRzPVwiJHt0aGlzLm1vZGVsLm11dGFudHN9XCI+PC9tdXRhdGlvbi10ZXN0LXJlcG9ydC1maWxlLWxlZ2VuZD5cclxuICAgICAgICAgIDxwcmU+PGNvZGUgY2xhc3M9XCJsYW5nLSR7dGhpcy5tb2RlbC5sYW5ndWFnZX0gaGxqc1wiIC5pbm5lckhUTUw9XCIke3RoaXMucmVuZGVyQ29kZSgpfVwiPjwvY29kZT48L3ByZT5cclxuICAgICAgICBgO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSBnZXQgcm9vdCgpIHtcclxuICAgIHJldHVybiB0aGlzLnNoYWRvd1Jvb3QgfHwgdGhpcztcclxuICB9XHJcblxyXG4gIHByaXZhdGUgcmVuZGVyQ29kZSgpIHtcclxuICAgIGNvbnN0IGNvZGUgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KCdjb2RlJyk7XHJcbiAgICBjb2RlLmNsYXNzTGlzdC5hZGQoYGxhbmctJHt0aGlzLm1vZGVsLmxhbmd1YWdlfWApO1xyXG4gICAgY29kZS5pbm5lckhUTUwgPSB0aGlzLmFubm90YXRlZENvZGUoKTtcclxuICAgIGhsanMuaGlnaGxpZ2h0QmxvY2soY29kZSk7XHJcbiAgICByZXR1cm4gY29kZS5pbm5lckhUTUw7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIGFubm90YXRlZENvZGUoKTogc3RyaW5nIHtcclxuXHJcbiAgICBjb25zdCBsaW5lcyA9IHRvTGluZXModGhpcy5tb2RlbC5zb3VyY2UpO1xyXG4gICAgY29uc3QgY3VycmVudEN1cnNvck11dGFudFN0YXR1c2VzID0ge1xyXG4gICAgICBraWxsZWQ6IDAsXHJcbiAgICAgIG5vQ292ZXJhZ2U6IDAsXHJcbiAgICAgIHN1cnZpdmVkOiAwLFxyXG4gICAgICB0aW1lb3V0OiAwXHJcbiAgICB9O1xyXG5cclxuICAgIGNvbnN0IGFkanVzdEN1cnJlbnRNdXRhbnRSZXN1bHQgPSAodmFsdWVUb0FkZDogbnVtYmVyKSA9PiAobXV0YW50OiBNdXRhbnRSZXN1bHQpID0+IHtcclxuICAgICAgc3dpdGNoIChtdXRhbnQuc3RhdHVzKSB7XHJcbiAgICAgICAgY2FzZSBNdXRhbnRTdGF0dXMuS2lsbGVkOlxyXG4gICAgICAgICAgY3VycmVudEN1cnNvck11dGFudFN0YXR1c2VzLmtpbGxlZCArPSB2YWx1ZVRvQWRkO1xyXG4gICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgY2FzZSBNdXRhbnRTdGF0dXMuU3Vydml2ZWQ6XHJcbiAgICAgICAgICBjdXJyZW50Q3Vyc29yTXV0YW50U3RhdHVzZXMuc3Vydml2ZWQgKz0gdmFsdWVUb0FkZDtcclxuICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgIGNhc2UgTXV0YW50U3RhdHVzLlRpbWVvdXQ6XHJcbiAgICAgICAgICBjdXJyZW50Q3Vyc29yTXV0YW50U3RhdHVzZXMudGltZW91dCArPSB2YWx1ZVRvQWRkO1xyXG4gICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgY2FzZSBNdXRhbnRTdGF0dXMuTm9Db3ZlcmFnZTpcclxuICAgICAgICAgIGN1cnJlbnRDdXJzb3JNdXRhbnRTdGF0dXNlcy5ub0NvdmVyYWdlICs9IHZhbHVlVG9BZGQ7XHJcbiAgICAgICAgICBicmVhaztcclxuICAgICAgfVxyXG4gICAgfTtcclxuXHJcbiAgICBjb25zdCBkZXRlcm1pbmVCYWNrZ3JvdW5kID0gKCkgPT4ge1xyXG4gICAgICBpZiAoY3VycmVudEN1cnNvck11dGFudFN0YXR1c2VzLnN1cnZpdmVkID4gMCkge1xyXG4gICAgICAgIHJldHVybiBnZXRDb250ZXh0Q2xhc3NGb3JTdGF0dXMoTXV0YW50U3RhdHVzLlN1cnZpdmVkKSArICctbGlnaHQnO1xyXG4gICAgICB9IGVsc2UgaWYgKGN1cnJlbnRDdXJzb3JNdXRhbnRTdGF0dXNlcy5ub0NvdmVyYWdlID4gMCkge1xyXG4gICAgICAgIHJldHVybiBnZXRDb250ZXh0Q2xhc3NGb3JTdGF0dXMoTXV0YW50U3RhdHVzLk5vQ292ZXJhZ2UpICsgJy1saWdodCc7XHJcbiAgICAgIH0gZWxzZSBpZiAoY3VycmVudEN1cnNvck11dGFudFN0YXR1c2VzLnRpbWVvdXQgPiAwKSB7XHJcbiAgICAgICAgcmV0dXJuIGdldENvbnRleHRDbGFzc0ZvclN0YXR1cyhNdXRhbnRTdGF0dXMuVGltZW91dCkgKyAnLWxpZ2h0JztcclxuICAgICAgfSBlbHNlIGlmIChjdXJyZW50Q3Vyc29yTXV0YW50U3RhdHVzZXMua2lsbGVkID4gMCkge1xyXG4gICAgICAgIHJldHVybiBnZXRDb250ZXh0Q2xhc3NGb3JTdGF0dXMoTXV0YW50U3RhdHVzLktpbGxlZCkgKyAnLWxpZ2h0JztcclxuICAgICAgfVxyXG4gICAgICByZXR1cm4gbnVsbDtcclxuICAgIH07XHJcblxyXG4gICAgY29uc3QgYW5ub3RhdGVDaGFyYWN0ZXIgPSAoY2hhcjogc3RyaW5nLCBsaW5lOiBudW1iZXIsIGNvbHVtbjogbnVtYmVyKTogc3RyaW5nID0+IHtcclxuICAgICAgY29uc3QgbXV0YW50c1N0YXJ0aW5nID0gdGhpcy5tb2RlbC5tdXRhbnRzLmZpbHRlcihtID0+IG0ubG9jYXRpb24uc3RhcnQubGluZSA9PT0gbGluZSAmJiBtLmxvY2F0aW9uLnN0YXJ0LmNvbHVtbiA9PT0gY29sdW1uKTtcclxuICAgICAgY29uc3QgbXV0YW50c0VuZGluZyA9IHRoaXMubW9kZWwubXV0YW50cy5maWx0ZXIobSA9PiBtLmxvY2F0aW9uLmVuZC5saW5lID09PSBsaW5lICYmIG0ubG9jYXRpb24uZW5kLmNvbHVtbiA9PT0gY29sdW1uKTtcclxuICAgICAgbXV0YW50c1N0YXJ0aW5nLmZvckVhY2goYWRqdXN0Q3VycmVudE11dGFudFJlc3VsdCgxKSk7XHJcbiAgICAgIG11dGFudHNFbmRpbmcuZm9yRWFjaChhZGp1c3RDdXJyZW50TXV0YW50UmVzdWx0KC0xKSk7XHJcbiAgICAgIGNvbnN0IGlzU3RhcnQgPSBsaW5lID09PSBMSU5FX1NUQVJUX0lOREVYICYmIGNvbHVtbiA9PT0gQ09MVU1OX1NUQVJUX0lOREVYO1xyXG4gICAgICBjb25zdCBpc0VuZCA9IGxpbmUgPT09IGxpbmVzLmxlbmd0aCArIExJTkVfU1RBUlRfSU5ERVggLSAxICYmIGNvbHVtbiA9PT0gbGluZXNbbGluZSAtIExJTkVfU1RBUlRfSU5ERVhdLmxlbmd0aCArIENPTFVNTl9TVEFSVF9JTkRFWCAtIDE7XHJcbiAgICAgIGNvbnN0IGJhY2tncm91bmRDb2xvckFubm90YXRpb24gPSBtdXRhbnRzU3RhcnRpbmcubGVuZ3RoIHx8IG11dGFudHNFbmRpbmcubGVuZ3RoIHx8IGlzU3RhcnQgPyBgPHNwYW4gY2xhc3M9XCJiZy0ke2RldGVybWluZUJhY2tncm91bmQoKX1cIj5gIDogJyc7XHJcbiAgICAgIGNvbnN0IGJhY2tncm91bmRDb2xvckVuZEFubm90YXRpb24gPSAoKG11dGFudHNTdGFydGluZy5sZW5ndGggfHwgbXV0YW50c0VuZGluZy5sZW5ndGgpICYmICFpc1N0YXJ0KSB8fCBpc0VuZCA/ICc8L3NwYW4+JyA6ICcnO1xyXG4gICAgICBjb25zdCBtdXRhbnRzQW5ub3RhdGlvbnMgPSBtdXRhbnRzU3RhcnRpbmcubWFwKG0gPT5cclxuICAgICAgICBgPG11dGF0aW9uLXRlc3QtcmVwb3J0LW11dGFudCBtdXRhbnRJZD1cIiR7bS5pZH1cIiBtdXRhdG9yTmFtZT1cIiR7bS5tdXRhdG9yTmFtZX1cIiBzdGF0dXM9XCIke20uc3RhdHVzfVwiPjxzcGFuIHNsb3Q9XCJyZXBsYWNlbWVudFwiPiR7bS5yZXBsYWNlbWVudH08L3NwYW4+PHNwYW4gc2xvdD1cImFjdHVhbFwiPmApO1xyXG4gICAgICBjb25zdCBvcmlnaW5hbENvZGVFbmRBbm5vdGF0aW9ucyA9IG11dGFudHNFbmRpbmcubWFwKCgpID0+IGA8L3NwYW4+PC9tdXRhdGlvbi10ZXN0LXJlcG9ydC1tdXRhbnQ+YCk7XHJcbiAgICAgIHJldHVybiBgJHtiYWNrZ3JvdW5kQ29sb3JFbmRBbm5vdGF0aW9ufSR7b3JpZ2luYWxDb2RlRW5kQW5ub3RhdGlvbnMuam9pbignJyl9JHttdXRhbnRzQW5ub3RhdGlvbnMuam9pbignJyl9JHtiYWNrZ3JvdW5kQ29sb3JBbm5vdGF0aW9ufSR7ZXNjYXBlSHRtbChjaGFyKX1gO1xyXG4gICAgfTtcclxuXHJcbiAgICByZXR1cm4gd2Fsa1N0cmluZyh0aGlzLm1vZGVsLnNvdXJjZSwgYW5ub3RhdGVDaGFyYWN0ZXIpO1xyXG4gIH1cclxufVxyXG5cclxuLyoqXHJcbiAqIFdhbGtzIGEgc3RyaW5nLiBFeGVjdXRlcyBhIGZ1bmN0aW9uIG9uIGVhY2ggY2hhcmFjdGVyIG9mIHRoZSBzdHJpbmcgKGV4Y2VwdCBmb3IgbmV3IGxpbmVzIGFuZCBjYXJyaWFnZSByZXR1cm5zKVxyXG4gKiBAcGFyYW0gc291cmNlIHRoZSBzdHJpbmcgdG8gd2Fsa1xyXG4gKiBAcGFyYW0gZm4gVGhlIGZ1bmN0aW9uIHRvIGV4ZWN1dGUgb24gZWFjaCBjaGFyYWN0ZXIgb2YgdGhlIHN0cmluZ1xyXG4gKi9cclxuZnVuY3Rpb24gd2Fsa1N0cmluZyhzb3VyY2U6IHN0cmluZywgZm46IChjaGFyOiBzdHJpbmcsIGxpbmU6IG51bWJlciwgY29sdW1uOiBudW1iZXIpID0+IHN0cmluZyk6IHN0cmluZyB7XHJcbiAgY29uc3QgcmVzdWx0czogc3RyaW5nW10gPSBbXTtcclxuICBsZXQgY29sdW1uID0gQ09MVU1OX1NUQVJUX0lOREVYO1xyXG4gIGxldCByb3cgPSBMSU5FX1NUQVJUX0lOREVYO1xyXG5cclxuICBmb3IgKGxldCBpID0gMDsgaSA8IHNvdXJjZS5sZW5ndGg7IGkrKykgeyAvLyB0c2xpbnQ6ZGlzYWJsZS1saW5lOnByZWZlci1mb3Itb2ZcclxuICAgIGlmIChjb2x1bW4gPT09IENPTFVNTl9TVEFSVF9JTkRFWCAmJiBzb3VyY2VbaV0gPT09IENBUlJJQUdFX1JFVFVSTikge1xyXG4gICAgICBjb250aW51ZTtcclxuICAgIH1cclxuICAgIGlmIChzb3VyY2VbaV0gPT09IE5FV19MSU5FKSB7XHJcbiAgICAgIHJvdysrO1xyXG4gICAgICBjb2x1bW4gPSBDT0xVTU5fU1RBUlRfSU5ERVg7XHJcbiAgICAgIHJlc3VsdHMucHVzaChORVdfTElORSk7XHJcbiAgICAgIGNvbnRpbnVlO1xyXG4gICAgfVxyXG4gICAgcmVzdWx0cy5wdXNoKGZuKHNvdXJjZVtpXSwgcm93LCBjb2x1bW4rKykpO1xyXG4gIH1cclxuICByZXR1cm4gcmVzdWx0cy5qb2luKCcnKTtcclxufVxyXG4iLCJpbXBvcnQgeyBjdXN0b21FbGVtZW50LCBMaXRFbGVtZW50LCBwcm9wZXJ0eSwgaHRtbCwgY3NzIH0gZnJvbSAnbGl0LWVsZW1lbnQnO1xyXG5pbXBvcnQgeyBNdXRhbnRTdGF0dXMgfSBmcm9tICcuLi8uLi9hcGknO1xyXG5pbXBvcnQgeyBnZXRDb250ZXh0Q2xhc3NGb3JTdGF0dXMgfSBmcm9tICcuLi9oZWxwZXJzJztcclxuaW1wb3J0IHsgYm9vdHN0cmFwIH0gZnJvbSAnLi4vc3R5bGUnO1xyXG5cclxuQGN1c3RvbUVsZW1lbnQoJ211dGF0aW9uLXRlc3QtcmVwb3J0LW11dGFudCcpXHJcbmV4cG9ydCBjbGFzcyBNdXRhdGlvblRlc3RSZXBvcnRNdXRhbnRDb21wb25lbnQgZXh0ZW5kcyBMaXRFbGVtZW50IHtcclxuXHJcbiAgQHByb3BlcnR5KClcclxuICBwdWJsaWMgbXV0YW50SWQhOiBzdHJpbmc7XHJcblxyXG4gIEBwcm9wZXJ0eSgpXHJcbiAgcHVibGljIG11dGF0b3JOYW1lITogc3RyaW5nO1xyXG5cclxuICBAcHJvcGVydHkoKVxyXG4gIHB1YmxpYyBzdGF0dXMhOiBNdXRhbnRTdGF0dXM7XHJcblxyXG4gIEBwcm9wZXJ0eSgpXHJcbiAgcHVibGljIHNob3c6IGJvb2xlYW4gPSB0cnVlO1xyXG5cclxuICBAcHJvcGVydHkoKVxyXG4gIHB1YmxpYyBvcGVuOiBib29sZWFuID0gZmFsc2U7XHJcblxyXG4gIHB1YmxpYyBzdGF0aWMgc3R5bGVzID0gW1xyXG4gICAgYm9vdHN0cmFwLFxyXG4gICAgY3NzYFxyXG4gICAgLmJhZGdlIHtcclxuICAgICAgY3Vyc29yOiBwb2ludGVyO1xyXG4gICAgfVxyXG4gICAgLmRpc2FibGVkLWNvZGUge1xyXG4gICAgICB0ZXh0LWRlY29yYXRpb246IGxpbmUtdGhyb3VnaDtcclxuICAgIH1cclxuICAgIDo6c2xvdHRlZCguaGxqcy1zdHJpbmcpIHtcclxuICAgICAgY29sb3I6ICNmZmY7XHJcbiAgICB9XHJcbiAgYF07XHJcblxyXG4gIHB1YmxpYyByZW5kZXIoKSB7XHJcbiAgICAvLyBUaGlzIHBhcnQgaXMgbmV3bGluZSBzaWduaWZpY2FudCwgYXMgaXQgaXMgcmVuZGVyZWQgaW4gYSA8Y29kZT4gYmxvY2suXHJcbiAgICAvLyBObyB1bm5lY2Vzc2FyeSBuZXcgbGluZXNcclxuICAgIHJldHVybiBodG1sYCR7dGhpcy5yZW5kZXJCdXR0b24oKX0ke3RoaXMucmVuZGVyQ29kZSgpfWA7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlckJ1dHRvbigpIHtcclxuICAgIGlmICh0aGlzLnNob3cpIHtcclxuICAgICAgcmV0dXJuIGh0bWxgPHNwYW4gY2xhc3M9XCJiYWRnZSBiYWRnZS0ke3RoaXMub3BlbiA/ICdpbmZvJyA6IGdldENvbnRleHRDbGFzc0ZvclN0YXR1cyh0aGlzLnN0YXR1cyl9XCIgQGNsaWNrPVwiJHsoKSA9PiB0aGlzLm9wZW4gPSAhdGhpcy5vcGVufVwiXHJcbiAgdGl0bGU9XCIke3RoaXMubXV0YXRvck5hbWV9XCI+JHt0aGlzLm11dGFudElkfTwvYnV0dG9uPmA7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICByZXR1cm4gdW5kZWZpbmVkO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSByZW5kZXJDb2RlKCkge1xyXG4gICAgcmV0dXJuIGh0bWxgJHt0aGlzLnJlbmRlclJlcGxhY2VtZW50KCl9JHt0aGlzLnJlbmRlckFjdHVhbCgpfWA7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlckFjdHVhbCgpIHtcclxuICAgIGNvbnN0IGFjdHVhbENvZGVTbG90ID0gaHRtbGA8c2xvdCBuYW1lPVwiYWN0dWFsXCI+PC9zbG90PmA7XHJcbiAgICByZXR1cm4gaHRtbGA8c3BhbiBjbGFzcz1cIiR7dGhpcy5vcGVuICYmIHRoaXMuc2hvdyA/ICdkaXNhYmxlZC1jb2RlJyA6ICcnfVwiPiR7YWN0dWFsQ29kZVNsb3R9PC9zcGFuPmA7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlclJlcGxhY2VtZW50KCkge1xyXG4gICAgY29uc3QgcmVwbGFjZW1lbnRTbG90ID0gaHRtbGA8c2xvdCBjbGFzcz1cInJlcGxhY2VtZW50XCIgbmFtZT1cInJlcGxhY2VtZW50XCI+PC9zbG90PmA7XHJcbiAgICByZXR1cm4gaHRtbGA8c3BhbiBjbGFzcz1cImJhZGdlIGJhZGdlLWluZm9cIiA/aGlkZGVuPVwiJHshdGhpcy5vcGVuIHx8ICF0aGlzLnNob3d9XCI+JHtyZXBsYWNlbWVudFNsb3R9PC9zcGFuPmA7XHJcbiAgfVxyXG59XHJcbiIsImltcG9ydCB7IExpdEVsZW1lbnQsIGh0bWwsIHByb3BlcnR5LCBjdXN0b21FbGVtZW50IH0gZnJvbSAnbGl0LWVsZW1lbnQnO1xyXG5pbXBvcnQgeyBNdXRhdGlvblRlc3RSZXN1bHQgfSBmcm9tICcuLi8uLi9hcGknO1xyXG5pbXBvcnQgeyBpc0ZpbGVSZXN1bHQgfSBmcm9tICcuLi9oZWxwZXJzJztcclxuaW1wb3J0IHsgYm9vdHN0cmFwIH0gZnJvbSAnLi4vc3R5bGUnO1xyXG5cclxuQGN1c3RvbUVsZW1lbnQoJ211dGF0aW9uLXRlc3QtcmVwb3J0LXJlc3VsdCcpXHJcbmV4cG9ydCBjbGFzcyBNdXRhdGlvblRlc3RSZXBvcnRSZXN1bHRDb21wb25lbnQgZXh0ZW5kcyBMaXRFbGVtZW50IHtcclxuXHJcbiAgQHByb3BlcnR5KClcclxuICBwcml2YXRlIHJlYWRvbmx5IG1vZGVsITogTXV0YXRpb25UZXN0UmVzdWx0O1xyXG5cclxuICBAcHJvcGVydHkoKVxyXG4gIHByaXZhdGUgcmVhZG9ubHkgY3VycmVudFBhdGghOiBzdHJpbmdbXTtcclxuXHJcbiAgcHVibGljIHN0YXRpYyBzdHlsZXMgPSBbYm9vdHN0cmFwXTtcclxuXHJcbiAgcHVibGljIHJlbmRlcigpIHtcclxuICAgIHJldHVybiBodG1sYFxyXG4gICAgPGRpdiBjbGFzcz0ncm93Jz5cclxuICAgICAgPGRpdiBjbGFzcz0ndG90YWxzIGNvbC1zbS0xMSc+XHJcbiAgICAgICAgPG11dGF0aW9uLXRlc3QtcmVwb3J0LXRvdGFscyAuY3VycmVudFBhdGg9XCIke3RoaXMuY3VycmVudFBhdGh9XCIgLm1vZGVsPVwiJHt0aGlzLm1vZGVsfVwiPjwvbXV0YXRpb24tdGVzdC1yZXBvcnQtdG90YWxzPlxyXG4gICAgICA8L2Rpdj5cclxuICAgIDwvZGl2PlxyXG4gICAgJHt0aGlzLnJlbmRlckZpbGVSZXN1bHQoKX1cclxuICAgIGA7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlckZpbGVSZXN1bHQoKSB7XHJcbiAgICBpZiAoaXNGaWxlUmVzdWx0KHRoaXMubW9kZWwpKSB7XHJcbiAgICAgIHJldHVybiBodG1sYFxyXG4gICAgICAgIDxtdXRhdGlvbi10ZXN0LXJlcG9ydC1maWxlIC5tb2RlbD1cIiR7dGhpcy5tb2RlbH1cIj48L211dGF0aW9uLXRlc3QtcmVwb3J0LWZpbGU+XHJcbiAgICAgIGA7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICByZXR1cm4gaHRtbGBgO1xyXG4gICAgfVxyXG4gIH1cclxufVxyXG4iLCJcclxuaW1wb3J0IHsgTGl0RWxlbWVudCwgY3VzdG9tRWxlbWVudCB9IGZyb20gJ2xpdC1lbGVtZW50JztcclxuXHJcbkBjdXN0b21FbGVtZW50KCdtdXRhdGlvbi10ZXN0LXJlcG9ydC1yb3V0ZXInKVxyXG5leHBvcnQgY2xhc3MgTXV0YXRpb25UZXN0UmVwb3J0Um91dGVyQ29tcG9uZW50IGV4dGVuZHMgTGl0RWxlbWVudCB7XHJcblxyXG4gIHB1YmxpYyBjb25uZWN0ZWRDYWxsYmFjaygpIHtcclxuICAgIHN1cGVyLmNvbm5lY3RlZENhbGxiYWNrKCk7XHJcbiAgICB3aW5kb3cuYWRkRXZlbnRMaXN0ZW5lcignaGFzaGNoYW5nZScsIHRoaXMudXBkYXRlUGF0aCk7XHJcbiAgICB0aGlzLnVwZGF0ZVBhdGgoKTtcclxuICB9XHJcblxyXG4gIHByaXZhdGUgcmVhZG9ubHkgdXBkYXRlUGF0aCA9ICgpID0+IHtcclxuICAgIGNvbnN0IHBhdGhBc1N0cmluZyA9IHdpbmRvdy5sb2NhdGlvbi5oYXNoLnN1YnN0cigxKTtcclxuICAgIGNvbnN0IHBhdGggPSBwYXRoQXNTdHJpbmcubGVuZ3RoID8gcGF0aEFzU3RyaW5nLnNwbGl0KCcvJykgOiBbXTtcclxuICAgIHRoaXMuZGlzcGF0Y2hFdmVudChuZXcgQ3VzdG9tRXZlbnQoJ3BhdGgtY2hhbmdlZCcsIHsgZGV0YWlsOiBwYXRoIH0pKTtcclxuICB9XHJcbn1cclxuIiwiaW1wb3J0IHsgTGl0RWxlbWVudCwgaHRtbCwgcHJvcGVydHksIGN1c3RvbUVsZW1lbnQsIGNzcyB9IGZyb20gJ2xpdC1lbGVtZW50JztcclxuaW1wb3J0IHsgTXV0YXRpb25UZXN0UmVzdWx0LCBNdXRhdGlvblJlc3VsdEhlYWx0aCB9IGZyb20gJy4uLy4uL2FwaSc7XHJcbmltcG9ydCB7IGlzRGlyZWN0b3J5UmVzdWx0IH0gZnJvbSAnLi4vaGVscGVycyc7XHJcbmltcG9ydCB7IGJvb3RzdHJhcCB9IGZyb20gJy4uL3N0eWxlJztcclxuXHJcbkBjdXN0b21FbGVtZW50KCdtdXRhdGlvbi10ZXN0LXJlcG9ydC10b3RhbHMnKVxyXG5leHBvcnQgY2xhc3MgTXV0YXRpb25UZXN0UmVwb3J0VG90YWxzQ29tcG9uZW50IGV4dGVuZHMgTGl0RWxlbWVudCB7XHJcbiAgQHByb3BlcnR5KClcclxuICBwdWJsaWMgbW9kZWwhOiBNdXRhdGlvblRlc3RSZXN1bHQ7XHJcblxyXG4gIEBwcm9wZXJ0eSgpXHJcbiAgcHVibGljIGN1cnJlbnRQYXRoITogc3RyaW5nW107XHJcblxyXG4gIEBwcm9wZXJ0eSgpXHJcbiAgcHJpdmF0ZSBnZXQgY2hpbGRSZXN1bHRzKCk6IE11dGF0aW9uVGVzdFJlc3VsdFtdIHtcclxuICAgIGlmIChpc0RpcmVjdG9yeVJlc3VsdCh0aGlzLm1vZGVsKSkge1xyXG4gICAgICByZXR1cm4gdGhpcy5tb2RlbC5jaGlsZFJlc3VsdHM7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICByZXR1cm4gW107XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwdWJsaWMgc3RhdGljIHN0eWxlcyA9IFtib290c3RyYXAsXHJcbiAgICBjc3NgXHJcbiAgICB0aC5yb3RhdGUge1xyXG4gICAgICAvKiBTb21ldGhpbmcgeW91IGNhbiBjb3VudCBvbiAqL1xyXG4gICAgICBoZWlnaHQ6IDUwcHg7XHJcbiAgICAgIHdoaXRlLXNwYWNlOiBub3dyYXA7XHJcbiAgICAgIHBhZGRpbmctYm90dG9tOiAxMHB4O1xyXG4gICAgfVxyXG5cclxuICAgIHRoLnJvdGF0ZSA+IGRpdiB7XHJcbiAgICAgIHRyYW5zZm9ybTpcclxuICAgICAgdHJhbnNsYXRlKDI3cHgsIDBweClcclxuICAgICAgcm90YXRlKDMyNWRlZyk7XHJcbiAgICAgIHdpZHRoOiAzMHB4O1xyXG4gICAgfVxyXG5cclxuICAgIC50YWJsZS1uby10b3A+dGhlYWQ+dHI+dGgge1xyXG4gICAgICBib3JkZXItd2lkdGg6IDA7XHJcbiAgICB9XHJcblxyXG4gICAgLnRhYmxlLW5vLXRvcCB7XHJcbiAgICAgIGJvcmRlci13aWR0aDogMDtcclxuICAgIH1cclxuICBgXTtcclxuXHJcbiAgcHVibGljIHJlbmRlcigpIHtcclxuICAgIHJldHVybiBodG1sYFxyXG4gICAgICAgICAgPHRhYmxlIGNsYXNzPVwidGFibGUgdGFibGUtc20gdGFibGUtaG92ZXIgdGFibGUtYm9yZGVyZWQgdGFibGUtbm8tdG9wXCI+XHJcbiAgICAgICAgICAgICR7dGhpcy5yZW5kZXJIZWFkKCl9XHJcbiAgICAgICAgICAgICR7dGhpcy5yZW5kZXJCb2R5KCl9XHJcbiAgICAgICAgICA8L3RhYmxlPlxyXG4gICAgICBgO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSByZW5kZXJIZWFkKCkge1xyXG4gICAgcmV0dXJuIGh0bWxgPHRoZWFkPlxyXG4gIDx0cj5cclxuICAgIDx0aCBzdHlsZT0nd2lkdGg6IDIwJSc+XHJcbiAgICAgIDxkaXY+PHNwYW4+RmlsZSAvIERpcmVjdG9yeTwvc3Bhbj48L2Rpdj5cclxuICAgIDwvdGg+XHJcbiAgICA8dGggY29sc3Bhbj0nMic+XHJcbiAgICAgIDxkaXY+PHNwYW4+TXV0YXRpb24gc2NvcmU8L3NwYW4+PC9kaXY+XHJcbiAgICA8L3RoPlxyXG4gICAgJHt0aGlzLnJlbmRlclRvdGFsc0NvbHVtbnMoKX1cclxuICA8L3RyPlxyXG48L3RoZWFkPmA7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlckJvZHkoKSB7XHJcbiAgICByZXR1cm4gaHRtbGBcclxuICAgIDx0Ym9keT5cclxuICAgICAgJHt0aGlzLnJlbmRlclJvdyh0aGlzLm1vZGVsLCBmYWxzZSl9XHJcbiAgICAgICR7dGhpcy5jaGlsZFJlc3VsdHMubWFwKGNoaWxkID0+IGh0bWxgJHt0aGlzLnJlbmRlclJvdyhjaGlsZCwgdHJ1ZSl9YCl9XHJcbiAgICA8L3Rib2R5PmA7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlclJvdyhzdWJqZWN0OiBNdXRhdGlvblRlc3RSZXN1bHQsIGh5cGVybGluazogYm9vbGVhbikge1xyXG4gICAgY29uc3QgbXV0YXRpb25TY29yZVJvdW5kZWQgPSBzdWJqZWN0Lm11dGF0aW9uU2NvcmUudG9GaXhlZCgyKTtcclxuICAgIGNvbnN0IGNvbG9yaW5nQ2xhc3MgPSB0aGlzLmRldGVybWluZUNvbG9yaW5nQ2xhc3Moc3ViamVjdCk7XHJcbiAgICBjb25zdCBzdHlsZSA9IGB3aWR0aDogJHttdXRhdGlvblNjb3JlUm91bmRlZH0lYDtcclxuICAgIHJldHVybiBodG1sYFxyXG4gICAgPHRyPlxyXG4gICAgICA8dGQ+JHtoeXBlcmxpbmsgPyBodG1sYDxhIGhyZWY9XCIke3RoaXMubGluayhzdWJqZWN0Lm5hbWUpfVwiPiR7c3ViamVjdC5uYW1lfTwvYT5gIDogaHRtbGA8c3Bhbj4ke3N1YmplY3QubmFtZX08L3NwYW4+YH08L3RkPlxyXG4gICAgICA8dGQ+XHJcbiAgICAgICAgPGRpdiBjbGFzcz1cInByb2dyZXNzXCI+XHJcbiAgICAgICAgICA8ZGl2IGNsYXNzPVwicHJvZ3Jlc3MtYmFyIGJnLSR7Y29sb3JpbmdDbGFzc31cIiByb2xlPVwicHJvZ3Jlc3NiYXJcIiBhcmlhLXZhbHVlbm93PVwiJHttdXRhdGlvblNjb3JlUm91bmRlZH1cIlxyXG4gICAgICAgICAgICBhcmlhLXZhbHVlbWluPVwiMFwiIGFyaWEtdmFsdWVtYXg9XCIxMDBcIiAuc3R5bGU9XCIke3N0eWxlfVwiPlxyXG4gICAgICAgICAgICAke211dGF0aW9uU2NvcmVSb3VuZGVkfSVcclxuICAgICAgICAgIDwvZGl2PlxyXG4gICAgICAgIDwvZGl2PlxyXG4gICAgICA8L3RkPlxyXG4gICAgICA8dGggY2xhc3M9XCJ0ZXh0LWNlbnRlciB0ZXh0LSR7Y29sb3JpbmdDbGFzc31cIj4ke211dGF0aW9uU2NvcmVSb3VuZGVkfTwvdGg+XHJcbiAgICAgICR7T2JqZWN0LmtleXModGhpcy5tb2RlbC50b3RhbHMpLm1hcCh0aXRsZSA9PiBodG1sYDx0ZCBjbGFzcz1cInRleHQtY2VudGVyXCI+JHtzdWJqZWN0LnRvdGFsc1t0aXRsZV19PC90ZD5gKX1cclxuICAgIDwvdHI+XHJcbiAgICBgIDtcclxuICB9XHJcblxyXG4gIHByaXZhdGUgbGluayh0bzogc3RyaW5nKSB7XHJcbiAgICBpZiAodGhpcy5jdXJyZW50UGF0aCAmJiB0aGlzLmN1cnJlbnRQYXRoLmxlbmd0aCkge1xyXG4gICAgICByZXR1cm4gYCMke3RoaXMuY3VycmVudFBhdGguam9pbignLycpfS8ke3RvfWA7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICByZXR1cm4gYCMke3RvfWA7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlclRvdGFsc0NvbHVtbnMoKSB7XHJcbiAgICByZXR1cm4gaHRtbGBcclxuICAgICAgICAke09iamVjdC5rZXlzKHRoaXMubW9kZWwudG90YWxzKS5tYXAodGl0bGUgPT4gaHRtbGA8dGggY2xhc3M9J3JvdGF0ZSB0ZXh0LWNlbnRlcicgc3R5bGU9J3dpZHRoOiA1MHB4Jz5cclxuICAgICAgICAgIDxkaXY+PHNwYW4+JHt0aXRsZX08L3NwYW4+PC9kaXY+XHJcbiAgICAgICAgPC90aD5gKX1cclxuICAgIGA7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIGRldGVybWluZUNvbG9yaW5nQ2xhc3Moc3ViamVjdDogTXV0YXRpb25UZXN0UmVzdWx0KSB7XHJcbiAgICBzd2l0Y2ggKHN1YmplY3QuaGVhbHRoKSB7XHJcbiAgICAgIGNhc2UgTXV0YXRpb25SZXN1bHRIZWFsdGguRGFuZ2VyOlxyXG4gICAgICAgIHJldHVybiAnZGFuZ2VyJztcclxuICAgICAgY2FzZSBNdXRhdGlvblJlc3VsdEhlYWx0aC5Hb29kOlxyXG4gICAgICAgIHJldHVybiAnc3VjY2Vzcyc7XHJcbiAgICAgIGNhc2UgTXV0YXRpb25SZXN1bHRIZWFsdGguV2FybmluZzpcclxuICAgICAgICByZXR1cm4gJ3dhcm5pbmcnO1xyXG4gICAgICBkZWZhdWx0OlxyXG4gICAgICAgIHJldHVybiAnc2Vjb25kYXJ5JztcclxuICAgIH1cclxuICB9XHJcblxyXG59XHJcbiIsImltcG9ydCB7IE11dGF0aW9uVGVzdFJlc3VsdCB9IGZyb20gJy4uL2FwaS9NdXRhdGlvblRlc3RSZXN1bHQnO1xyXG5pbXBvcnQgeyBEaXJlY3RvcnlSZXN1bHQgfSBmcm9tICcuLi9hcGkvRGlyZWN0b3J5UmVzdWx0JztcclxuaW1wb3J0IHsgRmlsZVJlc3VsdCwgTXV0YW50U3RhdHVzIH0gZnJvbSAnLi4vYXBpJztcclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBpc0RpcmVjdG9yeVJlc3VsdChyZXN1bHQ6IE11dGF0aW9uVGVzdFJlc3VsdCB8IHVuZGVmaW5lZCk6IHJlc3VsdCBpcyBEaXJlY3RvcnlSZXN1bHQge1xyXG4gIHJldHVybiBCb29sZWFuKHJlc3VsdCAmJiAocmVzdWx0IGFzIERpcmVjdG9yeVJlc3VsdCkuY2hpbGRSZXN1bHRzKTtcclxufVxyXG5cclxuZXhwb3J0IGZ1bmN0aW9uIGlzRmlsZVJlc3VsdChyZXN1bHQ6IE11dGF0aW9uVGVzdFJlc3VsdCB8IHVuZGVmaW5lZCk6IHJlc3VsdCBpcyBGaWxlUmVzdWx0IHtcclxuICByZXR1cm4gQm9vbGVhbihyZXN1bHQgJiYgKHJlc3VsdCBhcyBGaWxlUmVzdWx0KS5tdXRhbnRzKTtcclxufVxyXG5cclxuZXhwb3J0IGZ1bmN0aW9uIGdldENvbnRleHRDbGFzc0ZvclN0YXR1cyhzdGF0dXM6IE11dGFudFN0YXR1cykge1xyXG4gIHN3aXRjaCAoc3RhdHVzKSB7XHJcbiAgICBjYXNlIE11dGFudFN0YXR1cy5LaWxsZWQ6XHJcbiAgICAgIHJldHVybiAnc3VjY2Vzcyc7XHJcbiAgICBjYXNlIE11dGFudFN0YXR1cy5Ob0NvdmVyYWdlOlxyXG4gICAgY2FzZSBNdXRhbnRTdGF0dXMuU3Vydml2ZWQ6XHJcbiAgICAgIHJldHVybiAnZGFuZ2VyJztcclxuICAgIGNhc2UgTXV0YW50U3RhdHVzLlRpbWVvdXQ6XHJcbiAgICAgIHJldHVybiAnd2FybmluZyc7XHJcbiAgICBjYXNlIE11dGFudFN0YXR1cy5SdW50aW1lRXJyb3I6XHJcbiAgICBjYXNlIE11dGFudFN0YXR1cy5Db21waWxlRXJyb3I6XHJcbiAgICAgIHJldHVybiAnc2Vjb25kYXJ5JztcclxuICB9XHJcbn1cclxuXHJcbmV4cG9ydCBjb25zdCBDT0xVTU5fU1RBUlRfSU5ERVggPSAxO1xyXG5leHBvcnQgY29uc3QgTElORV9TVEFSVF9JTkRFWCA9IDE7XHJcbmV4cG9ydCBjb25zdCBORVdfTElORSA9ICdcXG4nO1xyXG5leHBvcnQgY29uc3QgQ0FSUklBR0VfUkVUVVJOID0gJ1xccic7XHJcbmV4cG9ydCBmdW5jdGlvbiBsaW5lcyhjb250ZW50OiBzdHJpbmcpIHtcclxuICByZXR1cm4gY29udGVudC5zcGxpdChORVdfTElORSkubWFwKGxpbmUgPT4gbGluZS5lbmRzV2l0aChDQVJSSUFHRV9SRVRVUk4pID8gbGluZS5zdWJzdHIoMCwgbGluZS5sZW5ndGggLSAxKSA6IGxpbmUpO1xyXG59XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gZXNjYXBlSHRtbCh1bnNhZmU6IHN0cmluZykge1xyXG4gIHJldHVybiB1bnNhZmVcclxuICAgIC5yZXBsYWNlKC8mL2csICcmYW1wOycpXHJcbiAgICAucmVwbGFjZSgvPC9nLCAnJmx0OycpXHJcbiAgICAucmVwbGFjZSgvPi9nLCAnJmd0OycpXHJcbiAgICAucmVwbGFjZSgvXCIvZywgJyZxdW90OycpXHJcbiAgICAucmVwbGFjZSgvJy9nLCAnJiMwMzk7Jyk7XHJcbn1cclxuIiwiaW1wb3J0ICcuL2NvbXBvbmVudHMvbXV0YXRpb24tdGVzdC1yZXBvcnQtYXBwJztcclxuaW1wb3J0ICcuL2NvbXBvbmVudHMvbXV0YXRpb24tdGVzdC1yZXBvcnQtZmlsZSc7XHJcbmltcG9ydCAnLi9jb21wb25lbnRzL211dGF0aW9uLXRlc3QtcmVwb3J0LXRvdGFscyc7XHJcbmltcG9ydCAnLi9jb21wb25lbnRzL211dGF0aW9uLXRlc3QtcmVwb3J0LXJlc3VsdCc7XHJcbmltcG9ydCAnLi9jb21wb25lbnRzL211dGF0aW9uLXRlc3QtcmVwb3J0LWJyZWFkY3J1bWInO1xyXG5pbXBvcnQgJy4vY29tcG9uZW50cy9tdXRhdGlvbi10ZXN0LXJlcG9ydC1yb3V0ZXInO1xyXG5pbXBvcnQgJy4vY29tcG9uZW50cy9tdXRhdGlvbi10ZXN0LXJlcG9ydC1tdXRhbnQnO1xyXG5pbXBvcnQgJy4vY29tcG9uZW50cy9tdXRhdGlvbi10ZXN0LXJlcG9ydC1maWxlLWxlZ2VuZCc7XHJcbiIsImV4cG9ydHMgPSBtb2R1bGUuZXhwb3J0cyA9IHJlcXVpcmUoXCIuLi8uLi9ub2RlX21vZHVsZXMvY3NzLWxvYWRlci9kaXN0L3J1bnRpbWUvYXBpLmpzXCIpKGZhbHNlKTtcbi8vIE1vZHVsZVxuZXhwb3J0cy5wdXNoKFttb2R1bGUuaWQsIFwiLyohXFxuICogQm9vdHN0cmFwIFJlYm9vdCB2NC4zLjEgKGh0dHBzOi8vZ2V0Ym9vdHN0cmFwLmNvbS8pXFxuICogQ29weXJpZ2h0IDIwMTEtMjAxOSBUaGUgQm9vdHN0cmFwIEF1dGhvcnNcXG4gKiBDb3B5cmlnaHQgMjAxMS0yMDE5IFR3aXR0ZXIsIEluYy5cXG4gKiBMaWNlbnNlZCB1bmRlciBNSVQgKGh0dHBzOi8vZ2l0aHViLmNvbS90d2JzL2Jvb3RzdHJhcC9ibG9iL21hc3Rlci9MSUNFTlNFKVxcbiAqIEZvcmtlZCBmcm9tIE5vcm1hbGl6ZS5jc3MsIGxpY2Vuc2VkIE1JVCAoaHR0cHM6Ly9naXRodWIuY29tL25lY29sYXMvbm9ybWFsaXplLmNzcy9ibG9iL21hc3Rlci9MSUNFTlNFLm1kKVxcbiAqL1xcbiosXFxuKjo6YmVmb3JlLFxcbio6OmFmdGVyIHtcXG4gIGJveC1zaXppbmc6IGJvcmRlci1ib3g7IH1cXG5cXG5odG1sIHtcXG4gIGZvbnQtZmFtaWx5OiBzYW5zLXNlcmlmO1xcbiAgbGluZS1oZWlnaHQ6IDEuMTU7XFxuICAtd2Via2l0LXRleHQtc2l6ZS1hZGp1c3Q6IDEwMCU7XFxuICAtd2Via2l0LXRhcC1oaWdobGlnaHQtY29sb3I6IHJnYmEoMCwgMCwgMCwgMCk7IH1cXG5cXG5hcnRpY2xlLCBhc2lkZSwgZmlnY2FwdGlvbiwgZmlndXJlLCBmb290ZXIsIGhlYWRlciwgaGdyb3VwLCBtYWluLCBuYXYsIHNlY3Rpb24ge1xcbiAgZGlzcGxheTogYmxvY2s7IH1cXG5cXG5ib2R5IHtcXG4gIG1hcmdpbjogMDtcXG4gIGZvbnQtZmFtaWx5OiAtYXBwbGUtc3lzdGVtLCBCbGlua01hY1N5c3RlbUZvbnQsIFxcXCJTZWdvZSBVSVxcXCIsIFJvYm90bywgXFxcIkhlbHZldGljYSBOZXVlXFxcIiwgQXJpYWwsIFxcXCJOb3RvIFNhbnNcXFwiLCBzYW5zLXNlcmlmLCBcXFwiQXBwbGUgQ29sb3IgRW1vamlcXFwiLCBcXFwiU2Vnb2UgVUkgRW1vamlcXFwiLCBcXFwiU2Vnb2UgVUkgU3ltYm9sXFxcIiwgXFxcIk5vdG8gQ29sb3IgRW1vamlcXFwiO1xcbiAgZm9udC1zaXplOiAxcmVtO1xcbiAgZm9udC13ZWlnaHQ6IDQwMDtcXG4gIGxpbmUtaGVpZ2h0OiAxLjU7XFxuICBjb2xvcjogIzIxMjUyOTtcXG4gIHRleHQtYWxpZ246IGxlZnQ7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmZmOyB9XFxuXFxuW3RhYmluZGV4PVxcXCItMVxcXCJdOmZvY3VzIHtcXG4gIG91dGxpbmU6IDAgIWltcG9ydGFudDsgfVxcblxcbmhyIHtcXG4gIGJveC1zaXppbmc6IGNvbnRlbnQtYm94O1xcbiAgaGVpZ2h0OiAwO1xcbiAgb3ZlcmZsb3c6IHZpc2libGU7IH1cXG5cXG5oMSwgaDIsIGgzLCBoNCwgaDUsIGg2IHtcXG4gIG1hcmdpbi10b3A6IDA7XFxuICBtYXJnaW4tYm90dG9tOiAwLjVyZW07IH1cXG5cXG5wIHtcXG4gIG1hcmdpbi10b3A6IDA7XFxuICBtYXJnaW4tYm90dG9tOiAxcmVtOyB9XFxuXFxuYWJiclt0aXRsZV0sXFxuYWJicltkYXRhLW9yaWdpbmFsLXRpdGxlXSB7XFxuICB0ZXh0LWRlY29yYXRpb246IHVuZGVybGluZTtcXG4gIHRleHQtZGVjb3JhdGlvbjogdW5kZXJsaW5lIGRvdHRlZDtcXG4gIGN1cnNvcjogaGVscDtcXG4gIGJvcmRlci1ib3R0b206IDA7XFxuICB0ZXh0LWRlY29yYXRpb24tc2tpcC1pbms6IG5vbmU7IH1cXG5cXG5hZGRyZXNzIHtcXG4gIG1hcmdpbi1ib3R0b206IDFyZW07XFxuICBmb250LXN0eWxlOiBub3JtYWw7XFxuICBsaW5lLWhlaWdodDogaW5oZXJpdDsgfVxcblxcbm9sLFxcbnVsLFxcbmRsIHtcXG4gIG1hcmdpbi10b3A6IDA7XFxuICBtYXJnaW4tYm90dG9tOiAxcmVtOyB9XFxuXFxub2wgb2wsXFxudWwgdWwsXFxub2wgdWwsXFxudWwgb2wge1xcbiAgbWFyZ2luLWJvdHRvbTogMDsgfVxcblxcbmR0IHtcXG4gIGZvbnQtd2VpZ2h0OiA3MDA7IH1cXG5cXG5kZCB7XFxuICBtYXJnaW4tYm90dG9tOiAuNXJlbTtcXG4gIG1hcmdpbi1sZWZ0OiAwOyB9XFxuXFxuYmxvY2txdW90ZSB7XFxuICBtYXJnaW46IDAgMCAxcmVtOyB9XFxuXFxuYixcXG5zdHJvbmcge1xcbiAgZm9udC13ZWlnaHQ6IGJvbGRlcjsgfVxcblxcbnNtYWxsIHtcXG4gIGZvbnQtc2l6ZTogODAlOyB9XFxuXFxuc3ViLFxcbnN1cCB7XFxuICBwb3NpdGlvbjogcmVsYXRpdmU7XFxuICBmb250LXNpemU6IDc1JTtcXG4gIGxpbmUtaGVpZ2h0OiAwO1xcbiAgdmVydGljYWwtYWxpZ246IGJhc2VsaW5lOyB9XFxuXFxuc3ViIHtcXG4gIGJvdHRvbTogLS4yNWVtOyB9XFxuXFxuc3VwIHtcXG4gIHRvcDogLS41ZW07IH1cXG5cXG5hIHtcXG4gIGNvbG9yOiAjMDA3YmZmO1xcbiAgdGV4dC1kZWNvcmF0aW9uOiBub25lO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogdHJhbnNwYXJlbnQ7IH1cXG4gIGE6aG92ZXIge1xcbiAgICBjb2xvcjogIzAwNTZiMztcXG4gICAgdGV4dC1kZWNvcmF0aW9uOiB1bmRlcmxpbmU7IH1cXG5cXG5hOm5vdChbaHJlZl0pOm5vdChbdGFiaW5kZXhdKSB7XFxuICBjb2xvcjogaW5oZXJpdDtcXG4gIHRleHQtZGVjb3JhdGlvbjogbm9uZTsgfVxcbiAgYTpub3QoW2hyZWZdKTpub3QoW3RhYmluZGV4XSk6aG92ZXIsIGE6bm90KFtocmVmXSk6bm90KFt0YWJpbmRleF0pOmZvY3VzIHtcXG4gICAgY29sb3I6IGluaGVyaXQ7XFxuICAgIHRleHQtZGVjb3JhdGlvbjogbm9uZTsgfVxcbiAgYTpub3QoW2hyZWZdKTpub3QoW3RhYmluZGV4XSk6Zm9jdXMge1xcbiAgICBvdXRsaW5lOiAwOyB9XFxuXFxucHJlLFxcbmNvZGUsXFxua2JkLFxcbnNhbXAge1xcbiAgZm9udC1mYW1pbHk6IFNGTW9uby1SZWd1bGFyLCBNZW5sbywgTW9uYWNvLCBDb25zb2xhcywgXFxcIkxpYmVyYXRpb24gTW9ub1xcXCIsIFxcXCJDb3VyaWVyIE5ld1xcXCIsIG1vbm9zcGFjZTtcXG4gIGZvbnQtc2l6ZTogMWVtOyB9XFxuXFxucHJlIHtcXG4gIG1hcmdpbi10b3A6IDA7XFxuICBtYXJnaW4tYm90dG9tOiAxcmVtO1xcbiAgb3ZlcmZsb3c6IGF1dG87IH1cXG5cXG5maWd1cmUge1xcbiAgbWFyZ2luOiAwIDAgMXJlbTsgfVxcblxcbmltZyB7XFxuICB2ZXJ0aWNhbC1hbGlnbjogbWlkZGxlO1xcbiAgYm9yZGVyLXN0eWxlOiBub25lOyB9XFxuXFxuc3ZnIHtcXG4gIG92ZXJmbG93OiBoaWRkZW47XFxuICB2ZXJ0aWNhbC1hbGlnbjogbWlkZGxlOyB9XFxuXFxudGFibGUge1xcbiAgYm9yZGVyLWNvbGxhcHNlOiBjb2xsYXBzZTsgfVxcblxcbmNhcHRpb24ge1xcbiAgcGFkZGluZy10b3A6IDAuNzVyZW07XFxuICBwYWRkaW5nLWJvdHRvbTogMC43NXJlbTtcXG4gIGNvbG9yOiAjNmM3NTdkO1xcbiAgdGV4dC1hbGlnbjogbGVmdDtcXG4gIGNhcHRpb24tc2lkZTogYm90dG9tOyB9XFxuXFxudGgge1xcbiAgdGV4dC1hbGlnbjogaW5oZXJpdDsgfVxcblxcbmxhYmVsIHtcXG4gIGRpc3BsYXk6IGlubGluZS1ibG9jaztcXG4gIG1hcmdpbi1ib3R0b206IDAuNXJlbTsgfVxcblxcbmJ1dHRvbiB7XFxuICBib3JkZXItcmFkaXVzOiAwOyB9XFxuXFxuYnV0dG9uOmZvY3VzIHtcXG4gIG91dGxpbmU6IDFweCBkb3R0ZWQ7XFxuICBvdXRsaW5lOiA1cHggYXV0byAtd2Via2l0LWZvY3VzLXJpbmctY29sb3I7IH1cXG5cXG5pbnB1dCxcXG5idXR0b24sXFxuc2VsZWN0LFxcbm9wdGdyb3VwLFxcbnRleHRhcmVhIHtcXG4gIG1hcmdpbjogMDtcXG4gIGZvbnQtZmFtaWx5OiBpbmhlcml0O1xcbiAgZm9udC1zaXplOiBpbmhlcml0O1xcbiAgbGluZS1oZWlnaHQ6IGluaGVyaXQ7IH1cXG5cXG5idXR0b24sXFxuaW5wdXQge1xcbiAgb3ZlcmZsb3c6IHZpc2libGU7IH1cXG5cXG5idXR0b24sXFxuc2VsZWN0IHtcXG4gIHRleHQtdHJhbnNmb3JtOiBub25lOyB9XFxuXFxuc2VsZWN0IHtcXG4gIHdvcmQtd3JhcDogbm9ybWFsOyB9XFxuXFxuYnV0dG9uLFxcblt0eXBlPVxcXCJidXR0b25cXFwiXSxcXG5bdHlwZT1cXFwicmVzZXRcXFwiXSxcXG5bdHlwZT1cXFwic3VibWl0XFxcIl0ge1xcbiAgLXdlYmtpdC1hcHBlYXJhbmNlOiBidXR0b247IH1cXG5cXG5idXR0b246bm90KDpkaXNhYmxlZCksXFxuW3R5cGU9XFxcImJ1dHRvblxcXCJdOm5vdCg6ZGlzYWJsZWQpLFxcblt0eXBlPVxcXCJyZXNldFxcXCJdOm5vdCg6ZGlzYWJsZWQpLFxcblt0eXBlPVxcXCJzdWJtaXRcXFwiXTpub3QoOmRpc2FibGVkKSB7XFxuICBjdXJzb3I6IHBvaW50ZXI7IH1cXG5cXG5idXR0b246Oi1tb3otZm9jdXMtaW5uZXIsXFxuW3R5cGU9XFxcImJ1dHRvblxcXCJdOjotbW96LWZvY3VzLWlubmVyLFxcblt0eXBlPVxcXCJyZXNldFxcXCJdOjotbW96LWZvY3VzLWlubmVyLFxcblt0eXBlPVxcXCJzdWJtaXRcXFwiXTo6LW1vei1mb2N1cy1pbm5lciB7XFxuICBwYWRkaW5nOiAwO1xcbiAgYm9yZGVyLXN0eWxlOiBub25lOyB9XFxuXFxuaW5wdXRbdHlwZT1cXFwicmFkaW9cXFwiXSxcXG5pbnB1dFt0eXBlPVxcXCJjaGVja2JveFxcXCJdIHtcXG4gIGJveC1zaXppbmc6IGJvcmRlci1ib3g7XFxuICBwYWRkaW5nOiAwOyB9XFxuXFxuaW5wdXRbdHlwZT1cXFwiZGF0ZVxcXCJdLFxcbmlucHV0W3R5cGU9XFxcInRpbWVcXFwiXSxcXG5pbnB1dFt0eXBlPVxcXCJkYXRldGltZS1sb2NhbFxcXCJdLFxcbmlucHV0W3R5cGU9XFxcIm1vbnRoXFxcIl0ge1xcbiAgLXdlYmtpdC1hcHBlYXJhbmNlOiBsaXN0Ym94OyB9XFxuXFxudGV4dGFyZWEge1xcbiAgb3ZlcmZsb3c6IGF1dG87XFxuICByZXNpemU6IHZlcnRpY2FsOyB9XFxuXFxuZmllbGRzZXQge1xcbiAgbWluLXdpZHRoOiAwO1xcbiAgcGFkZGluZzogMDtcXG4gIG1hcmdpbjogMDtcXG4gIGJvcmRlcjogMDsgfVxcblxcbmxlZ2VuZCB7XFxuICBkaXNwbGF5OiBibG9jaztcXG4gIHdpZHRoOiAxMDAlO1xcbiAgbWF4LXdpZHRoOiAxMDAlO1xcbiAgcGFkZGluZzogMDtcXG4gIG1hcmdpbi1ib3R0b206IC41cmVtO1xcbiAgZm9udC1zaXplOiAxLjVyZW07XFxuICBsaW5lLWhlaWdodDogaW5oZXJpdDtcXG4gIGNvbG9yOiBpbmhlcml0O1xcbiAgd2hpdGUtc3BhY2U6IG5vcm1hbDsgfVxcblxcbnByb2dyZXNzIHtcXG4gIHZlcnRpY2FsLWFsaWduOiBiYXNlbGluZTsgfVxcblxcblt0eXBlPVxcXCJudW1iZXJcXFwiXTo6LXdlYmtpdC1pbm5lci1zcGluLWJ1dHRvbixcXG5bdHlwZT1cXFwibnVtYmVyXFxcIl06Oi13ZWJraXQtb3V0ZXItc3Bpbi1idXR0b24ge1xcbiAgaGVpZ2h0OiBhdXRvOyB9XFxuXFxuW3R5cGU9XFxcInNlYXJjaFxcXCJdIHtcXG4gIG91dGxpbmUtb2Zmc2V0OiAtMnB4O1xcbiAgLXdlYmtpdC1hcHBlYXJhbmNlOiBub25lOyB9XFxuXFxuW3R5cGU9XFxcInNlYXJjaFxcXCJdOjotd2Via2l0LXNlYXJjaC1kZWNvcmF0aW9uIHtcXG4gIC13ZWJraXQtYXBwZWFyYW5jZTogbm9uZTsgfVxcblxcbjo6LXdlYmtpdC1maWxlLXVwbG9hZC1idXR0b24ge1xcbiAgZm9udDogaW5oZXJpdDtcXG4gIC13ZWJraXQtYXBwZWFyYW5jZTogYnV0dG9uOyB9XFxuXFxub3V0cHV0IHtcXG4gIGRpc3BsYXk6IGlubGluZS1ibG9jazsgfVxcblxcbnN1bW1hcnkge1xcbiAgZGlzcGxheTogbGlzdC1pdGVtO1xcbiAgY3Vyc29yOiBwb2ludGVyOyB9XFxuXFxudGVtcGxhdGUge1xcbiAgZGlzcGxheTogbm9uZTsgfVxcblxcbltoaWRkZW5dIHtcXG4gIGRpc3BsYXk6IG5vbmUgIWltcG9ydGFudDsgfVxcblxcbi5jb250YWluZXIge1xcbiAgd2lkdGg6IDEwMCU7XFxuICBwYWRkaW5nLXJpZ2h0OiAxNXB4O1xcbiAgcGFkZGluZy1sZWZ0OiAxNXB4O1xcbiAgbWFyZ2luLXJpZ2h0OiBhdXRvO1xcbiAgbWFyZ2luLWxlZnQ6IGF1dG87IH1cXG4gIEBtZWRpYSAobWluLXdpZHRoOiA1NzZweCkge1xcbiAgICAuY29udGFpbmVyIHtcXG4gICAgICBtYXgtd2lkdGg6IDU0MHB4OyB9IH1cXG4gIEBtZWRpYSAobWluLXdpZHRoOiA3NjhweCkge1xcbiAgICAuY29udGFpbmVyIHtcXG4gICAgICBtYXgtd2lkdGg6IDcyMHB4OyB9IH1cXG4gIEBtZWRpYSAobWluLXdpZHRoOiA5OTJweCkge1xcbiAgICAuY29udGFpbmVyIHtcXG4gICAgICBtYXgtd2lkdGg6IDk2MHB4OyB9IH1cXG4gIEBtZWRpYSAobWluLXdpZHRoOiAxMjAwcHgpIHtcXG4gICAgLmNvbnRhaW5lciB7XFxuICAgICAgbWF4LXdpZHRoOiAxMTQwcHg7IH0gfVxcblxcbi5jb250YWluZXItZmx1aWQge1xcbiAgd2lkdGg6IDEwMCU7XFxuICBwYWRkaW5nLXJpZ2h0OiAxNXB4O1xcbiAgcGFkZGluZy1sZWZ0OiAxNXB4O1xcbiAgbWFyZ2luLXJpZ2h0OiBhdXRvO1xcbiAgbWFyZ2luLWxlZnQ6IGF1dG87IH1cXG5cXG4ucm93IHtcXG4gIGRpc3BsYXk6IGZsZXg7XFxuICBmbGV4LXdyYXA6IHdyYXA7XFxuICBtYXJnaW4tcmlnaHQ6IC0xNXB4O1xcbiAgbWFyZ2luLWxlZnQ6IC0xNXB4OyB9XFxuXFxuLm5vLWd1dHRlcnMge1xcbiAgbWFyZ2luLXJpZ2h0OiAwO1xcbiAgbWFyZ2luLWxlZnQ6IDA7IH1cXG4gIC5uby1ndXR0ZXJzID4gLmNvbCxcXG4gIC5uby1ndXR0ZXJzID4gW2NsYXNzKj1cXFwiY29sLVxcXCJdIHtcXG4gICAgcGFkZGluZy1yaWdodDogMDtcXG4gICAgcGFkZGluZy1sZWZ0OiAwOyB9XFxuXFxuLmNvbC0xLCAuY29sLTIsIC5jb2wtMywgLmNvbC00LCAuY29sLTUsIC5jb2wtNiwgLmNvbC03LCAuY29sLTgsIC5jb2wtOSwgLmNvbC0xMCwgLmNvbC0xMSwgLmNvbC0xMiwgLmNvbCxcXG4uY29sLWF1dG8sIC5jb2wtc20tMSwgLmNvbC1zbS0yLCAuY29sLXNtLTMsIC5jb2wtc20tNCwgLmNvbC1zbS01LCAuY29sLXNtLTYsIC5jb2wtc20tNywgLmNvbC1zbS04LCAuY29sLXNtLTksIC5jb2wtc20tMTAsIC5jb2wtc20tMTEsIC5jb2wtc20tMTIsIC5jb2wtc20sXFxuLmNvbC1zbS1hdXRvLCAuY29sLW1kLTEsIC5jb2wtbWQtMiwgLmNvbC1tZC0zLCAuY29sLW1kLTQsIC5jb2wtbWQtNSwgLmNvbC1tZC02LCAuY29sLW1kLTcsIC5jb2wtbWQtOCwgLmNvbC1tZC05LCAuY29sLW1kLTEwLCAuY29sLW1kLTExLCAuY29sLW1kLTEyLCAuY29sLW1kLFxcbi5jb2wtbWQtYXV0bywgLmNvbC1sZy0xLCAuY29sLWxnLTIsIC5jb2wtbGctMywgLmNvbC1sZy00LCAuY29sLWxnLTUsIC5jb2wtbGctNiwgLmNvbC1sZy03LCAuY29sLWxnLTgsIC5jb2wtbGctOSwgLmNvbC1sZy0xMCwgLmNvbC1sZy0xMSwgLmNvbC1sZy0xMiwgLmNvbC1sZyxcXG4uY29sLWxnLWF1dG8sIC5jb2wteGwtMSwgLmNvbC14bC0yLCAuY29sLXhsLTMsIC5jb2wteGwtNCwgLmNvbC14bC01LCAuY29sLXhsLTYsIC5jb2wteGwtNywgLmNvbC14bC04LCAuY29sLXhsLTksIC5jb2wteGwtMTAsIC5jb2wteGwtMTEsIC5jb2wteGwtMTIsIC5jb2wteGwsXFxuLmNvbC14bC1hdXRvIHtcXG4gIHBvc2l0aW9uOiByZWxhdGl2ZTtcXG4gIHdpZHRoOiAxMDAlO1xcbiAgcGFkZGluZy1yaWdodDogMTVweDtcXG4gIHBhZGRpbmctbGVmdDogMTVweDsgfVxcblxcbi5jb2wge1xcbiAgZmxleC1iYXNpczogMDtcXG4gIGZsZXgtZ3JvdzogMTtcXG4gIG1heC13aWR0aDogMTAwJTsgfVxcblxcbi5jb2wtYXV0byB7XFxuICBmbGV4OiAwIDAgYXV0bztcXG4gIHdpZHRoOiBhdXRvO1xcbiAgbWF4LXdpZHRoOiAxMDAlOyB9XFxuXFxuLmNvbC0xIHtcXG4gIGZsZXg6IDAgMCA4LjMzMzMzJTtcXG4gIG1heC13aWR0aDogOC4zMzMzMyU7IH1cXG5cXG4uY29sLTIge1xcbiAgZmxleDogMCAwIDE2LjY2NjY3JTtcXG4gIG1heC13aWR0aDogMTYuNjY2NjclOyB9XFxuXFxuLmNvbC0zIHtcXG4gIGZsZXg6IDAgMCAyNSU7XFxuICBtYXgtd2lkdGg6IDI1JTsgfVxcblxcbi5jb2wtNCB7XFxuICBmbGV4OiAwIDAgMzMuMzMzMzMlO1xcbiAgbWF4LXdpZHRoOiAzMy4zMzMzMyU7IH1cXG5cXG4uY29sLTUge1xcbiAgZmxleDogMCAwIDQxLjY2NjY3JTtcXG4gIG1heC13aWR0aDogNDEuNjY2NjclOyB9XFxuXFxuLmNvbC02IHtcXG4gIGZsZXg6IDAgMCA1MCU7XFxuICBtYXgtd2lkdGg6IDUwJTsgfVxcblxcbi5jb2wtNyB7XFxuICBmbGV4OiAwIDAgNTguMzMzMzMlO1xcbiAgbWF4LXdpZHRoOiA1OC4zMzMzMyU7IH1cXG5cXG4uY29sLTgge1xcbiAgZmxleDogMCAwIDY2LjY2NjY3JTtcXG4gIG1heC13aWR0aDogNjYuNjY2NjclOyB9XFxuXFxuLmNvbC05IHtcXG4gIGZsZXg6IDAgMCA3NSU7XFxuICBtYXgtd2lkdGg6IDc1JTsgfVxcblxcbi5jb2wtMTAge1xcbiAgZmxleDogMCAwIDgzLjMzMzMzJTtcXG4gIG1heC13aWR0aDogODMuMzMzMzMlOyB9XFxuXFxuLmNvbC0xMSB7XFxuICBmbGV4OiAwIDAgOTEuNjY2NjclO1xcbiAgbWF4LXdpZHRoOiA5MS42NjY2NyU7IH1cXG5cXG4uY29sLTEyIHtcXG4gIGZsZXg6IDAgMCAxMDAlO1xcbiAgbWF4LXdpZHRoOiAxMDAlOyB9XFxuXFxuLm9yZGVyLWZpcnN0IHtcXG4gIG9yZGVyOiAtMTsgfVxcblxcbi5vcmRlci1sYXN0IHtcXG4gIG9yZGVyOiAxMzsgfVxcblxcbi5vcmRlci0wIHtcXG4gIG9yZGVyOiAwOyB9XFxuXFxuLm9yZGVyLTEge1xcbiAgb3JkZXI6IDE7IH1cXG5cXG4ub3JkZXItMiB7XFxuICBvcmRlcjogMjsgfVxcblxcbi5vcmRlci0zIHtcXG4gIG9yZGVyOiAzOyB9XFxuXFxuLm9yZGVyLTQge1xcbiAgb3JkZXI6IDQ7IH1cXG5cXG4ub3JkZXItNSB7XFxuICBvcmRlcjogNTsgfVxcblxcbi5vcmRlci02IHtcXG4gIG9yZGVyOiA2OyB9XFxuXFxuLm9yZGVyLTcge1xcbiAgb3JkZXI6IDc7IH1cXG5cXG4ub3JkZXItOCB7XFxuICBvcmRlcjogODsgfVxcblxcbi5vcmRlci05IHtcXG4gIG9yZGVyOiA5OyB9XFxuXFxuLm9yZGVyLTEwIHtcXG4gIG9yZGVyOiAxMDsgfVxcblxcbi5vcmRlci0xMSB7XFxuICBvcmRlcjogMTE7IH1cXG5cXG4ub3JkZXItMTIge1xcbiAgb3JkZXI6IDEyOyB9XFxuXFxuLm9mZnNldC0xIHtcXG4gIG1hcmdpbi1sZWZ0OiA4LjMzMzMzJTsgfVxcblxcbi5vZmZzZXQtMiB7XFxuICBtYXJnaW4tbGVmdDogMTYuNjY2NjclOyB9XFxuXFxuLm9mZnNldC0zIHtcXG4gIG1hcmdpbi1sZWZ0OiAyNSU7IH1cXG5cXG4ub2Zmc2V0LTQge1xcbiAgbWFyZ2luLWxlZnQ6IDMzLjMzMzMzJTsgfVxcblxcbi5vZmZzZXQtNSB7XFxuICBtYXJnaW4tbGVmdDogNDEuNjY2NjclOyB9XFxuXFxuLm9mZnNldC02IHtcXG4gIG1hcmdpbi1sZWZ0OiA1MCU7IH1cXG5cXG4ub2Zmc2V0LTcge1xcbiAgbWFyZ2luLWxlZnQ6IDU4LjMzMzMzJTsgfVxcblxcbi5vZmZzZXQtOCB7XFxuICBtYXJnaW4tbGVmdDogNjYuNjY2NjclOyB9XFxuXFxuLm9mZnNldC05IHtcXG4gIG1hcmdpbi1sZWZ0OiA3NSU7IH1cXG5cXG4ub2Zmc2V0LTEwIHtcXG4gIG1hcmdpbi1sZWZ0OiA4My4zMzMzMyU7IH1cXG5cXG4ub2Zmc2V0LTExIHtcXG4gIG1hcmdpbi1sZWZ0OiA5MS42NjY2NyU7IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogNTc2cHgpIHtcXG4gIC5jb2wtc20ge1xcbiAgICBmbGV4LWJhc2lzOiAwO1xcbiAgICBmbGV4LWdyb3c6IDE7XFxuICAgIG1heC13aWR0aDogMTAwJTsgfVxcbiAgLmNvbC1zbS1hdXRvIHtcXG4gICAgZmxleDogMCAwIGF1dG87XFxuICAgIHdpZHRoOiBhdXRvO1xcbiAgICBtYXgtd2lkdGg6IDEwMCU7IH1cXG4gIC5jb2wtc20tMSB7XFxuICAgIGZsZXg6IDAgMCA4LjMzMzMzJTtcXG4gICAgbWF4LXdpZHRoOiA4LjMzMzMzJTsgfVxcbiAgLmNvbC1zbS0yIHtcXG4gICAgZmxleDogMCAwIDE2LjY2NjY3JTtcXG4gICAgbWF4LXdpZHRoOiAxNi42NjY2NyU7IH1cXG4gIC5jb2wtc20tMyB7XFxuICAgIGZsZXg6IDAgMCAyNSU7XFxuICAgIG1heC13aWR0aDogMjUlOyB9XFxuICAuY29sLXNtLTQge1xcbiAgICBmbGV4OiAwIDAgMzMuMzMzMzMlO1xcbiAgICBtYXgtd2lkdGg6IDMzLjMzMzMzJTsgfVxcbiAgLmNvbC1zbS01IHtcXG4gICAgZmxleDogMCAwIDQxLjY2NjY3JTtcXG4gICAgbWF4LXdpZHRoOiA0MS42NjY2NyU7IH1cXG4gIC5jb2wtc20tNiB7XFxuICAgIGZsZXg6IDAgMCA1MCU7XFxuICAgIG1heC13aWR0aDogNTAlOyB9XFxuICAuY29sLXNtLTcge1xcbiAgICBmbGV4OiAwIDAgNTguMzMzMzMlO1xcbiAgICBtYXgtd2lkdGg6IDU4LjMzMzMzJTsgfVxcbiAgLmNvbC1zbS04IHtcXG4gICAgZmxleDogMCAwIDY2LjY2NjY3JTtcXG4gICAgbWF4LXdpZHRoOiA2Ni42NjY2NyU7IH1cXG4gIC5jb2wtc20tOSB7XFxuICAgIGZsZXg6IDAgMCA3NSU7XFxuICAgIG1heC13aWR0aDogNzUlOyB9XFxuICAuY29sLXNtLTEwIHtcXG4gICAgZmxleDogMCAwIDgzLjMzMzMzJTtcXG4gICAgbWF4LXdpZHRoOiA4My4zMzMzMyU7IH1cXG4gIC5jb2wtc20tMTEge1xcbiAgICBmbGV4OiAwIDAgOTEuNjY2NjclO1xcbiAgICBtYXgtd2lkdGg6IDkxLjY2NjY3JTsgfVxcbiAgLmNvbC1zbS0xMiB7XFxuICAgIGZsZXg6IDAgMCAxMDAlO1xcbiAgICBtYXgtd2lkdGg6IDEwMCU7IH1cXG4gIC5vcmRlci1zbS1maXJzdCB7XFxuICAgIG9yZGVyOiAtMTsgfVxcbiAgLm9yZGVyLXNtLWxhc3Qge1xcbiAgICBvcmRlcjogMTM7IH1cXG4gIC5vcmRlci1zbS0wIHtcXG4gICAgb3JkZXI6IDA7IH1cXG4gIC5vcmRlci1zbS0xIHtcXG4gICAgb3JkZXI6IDE7IH1cXG4gIC5vcmRlci1zbS0yIHtcXG4gICAgb3JkZXI6IDI7IH1cXG4gIC5vcmRlci1zbS0zIHtcXG4gICAgb3JkZXI6IDM7IH1cXG4gIC5vcmRlci1zbS00IHtcXG4gICAgb3JkZXI6IDQ7IH1cXG4gIC5vcmRlci1zbS01IHtcXG4gICAgb3JkZXI6IDU7IH1cXG4gIC5vcmRlci1zbS02IHtcXG4gICAgb3JkZXI6IDY7IH1cXG4gIC5vcmRlci1zbS03IHtcXG4gICAgb3JkZXI6IDc7IH1cXG4gIC5vcmRlci1zbS04IHtcXG4gICAgb3JkZXI6IDg7IH1cXG4gIC5vcmRlci1zbS05IHtcXG4gICAgb3JkZXI6IDk7IH1cXG4gIC5vcmRlci1zbS0xMCB7XFxuICAgIG9yZGVyOiAxMDsgfVxcbiAgLm9yZGVyLXNtLTExIHtcXG4gICAgb3JkZXI6IDExOyB9XFxuICAub3JkZXItc20tMTIge1xcbiAgICBvcmRlcjogMTI7IH1cXG4gIC5vZmZzZXQtc20tMCB7XFxuICAgIG1hcmdpbi1sZWZ0OiAwOyB9XFxuICAub2Zmc2V0LXNtLTEge1xcbiAgICBtYXJnaW4tbGVmdDogOC4zMzMzMyU7IH1cXG4gIC5vZmZzZXQtc20tMiB7XFxuICAgIG1hcmdpbi1sZWZ0OiAxNi42NjY2NyU7IH1cXG4gIC5vZmZzZXQtc20tMyB7XFxuICAgIG1hcmdpbi1sZWZ0OiAyNSU7IH1cXG4gIC5vZmZzZXQtc20tNCB7XFxuICAgIG1hcmdpbi1sZWZ0OiAzMy4zMzMzMyU7IH1cXG4gIC5vZmZzZXQtc20tNSB7XFxuICAgIG1hcmdpbi1sZWZ0OiA0MS42NjY2NyU7IH1cXG4gIC5vZmZzZXQtc20tNiB7XFxuICAgIG1hcmdpbi1sZWZ0OiA1MCU7IH1cXG4gIC5vZmZzZXQtc20tNyB7XFxuICAgIG1hcmdpbi1sZWZ0OiA1OC4zMzMzMyU7IH1cXG4gIC5vZmZzZXQtc20tOCB7XFxuICAgIG1hcmdpbi1sZWZ0OiA2Ni42NjY2NyU7IH1cXG4gIC5vZmZzZXQtc20tOSB7XFxuICAgIG1hcmdpbi1sZWZ0OiA3NSU7IH1cXG4gIC5vZmZzZXQtc20tMTAge1xcbiAgICBtYXJnaW4tbGVmdDogODMuMzMzMzMlOyB9XFxuICAub2Zmc2V0LXNtLTExIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDkxLjY2NjY3JTsgfSB9XFxuXFxuQG1lZGlhIChtaW4td2lkdGg6IDc2OHB4KSB7XFxuICAuY29sLW1kIHtcXG4gICAgZmxleC1iYXNpczogMDtcXG4gICAgZmxleC1ncm93OiAxO1xcbiAgICBtYXgtd2lkdGg6IDEwMCU7IH1cXG4gIC5jb2wtbWQtYXV0byB7XFxuICAgIGZsZXg6IDAgMCBhdXRvO1xcbiAgICB3aWR0aDogYXV0bztcXG4gICAgbWF4LXdpZHRoOiAxMDAlOyB9XFxuICAuY29sLW1kLTEge1xcbiAgICBmbGV4OiAwIDAgOC4zMzMzMyU7XFxuICAgIG1heC13aWR0aDogOC4zMzMzMyU7IH1cXG4gIC5jb2wtbWQtMiB7XFxuICAgIGZsZXg6IDAgMCAxNi42NjY2NyU7XFxuICAgIG1heC13aWR0aDogMTYuNjY2NjclOyB9XFxuICAuY29sLW1kLTMge1xcbiAgICBmbGV4OiAwIDAgMjUlO1xcbiAgICBtYXgtd2lkdGg6IDI1JTsgfVxcbiAgLmNvbC1tZC00IHtcXG4gICAgZmxleDogMCAwIDMzLjMzMzMzJTtcXG4gICAgbWF4LXdpZHRoOiAzMy4zMzMzMyU7IH1cXG4gIC5jb2wtbWQtNSB7XFxuICAgIGZsZXg6IDAgMCA0MS42NjY2NyU7XFxuICAgIG1heC13aWR0aDogNDEuNjY2NjclOyB9XFxuICAuY29sLW1kLTYge1xcbiAgICBmbGV4OiAwIDAgNTAlO1xcbiAgICBtYXgtd2lkdGg6IDUwJTsgfVxcbiAgLmNvbC1tZC03IHtcXG4gICAgZmxleDogMCAwIDU4LjMzMzMzJTtcXG4gICAgbWF4LXdpZHRoOiA1OC4zMzMzMyU7IH1cXG4gIC5jb2wtbWQtOCB7XFxuICAgIGZsZXg6IDAgMCA2Ni42NjY2NyU7XFxuICAgIG1heC13aWR0aDogNjYuNjY2NjclOyB9XFxuICAuY29sLW1kLTkge1xcbiAgICBmbGV4OiAwIDAgNzUlO1xcbiAgICBtYXgtd2lkdGg6IDc1JTsgfVxcbiAgLmNvbC1tZC0xMCB7XFxuICAgIGZsZXg6IDAgMCA4My4zMzMzMyU7XFxuICAgIG1heC13aWR0aDogODMuMzMzMzMlOyB9XFxuICAuY29sLW1kLTExIHtcXG4gICAgZmxleDogMCAwIDkxLjY2NjY3JTtcXG4gICAgbWF4LXdpZHRoOiA5MS42NjY2NyU7IH1cXG4gIC5jb2wtbWQtMTIge1xcbiAgICBmbGV4OiAwIDAgMTAwJTtcXG4gICAgbWF4LXdpZHRoOiAxMDAlOyB9XFxuICAub3JkZXItbWQtZmlyc3Qge1xcbiAgICBvcmRlcjogLTE7IH1cXG4gIC5vcmRlci1tZC1sYXN0IHtcXG4gICAgb3JkZXI6IDEzOyB9XFxuICAub3JkZXItbWQtMCB7XFxuICAgIG9yZGVyOiAwOyB9XFxuICAub3JkZXItbWQtMSB7XFxuICAgIG9yZGVyOiAxOyB9XFxuICAub3JkZXItbWQtMiB7XFxuICAgIG9yZGVyOiAyOyB9XFxuICAub3JkZXItbWQtMyB7XFxuICAgIG9yZGVyOiAzOyB9XFxuICAub3JkZXItbWQtNCB7XFxuICAgIG9yZGVyOiA0OyB9XFxuICAub3JkZXItbWQtNSB7XFxuICAgIG9yZGVyOiA1OyB9XFxuICAub3JkZXItbWQtNiB7XFxuICAgIG9yZGVyOiA2OyB9XFxuICAub3JkZXItbWQtNyB7XFxuICAgIG9yZGVyOiA3OyB9XFxuICAub3JkZXItbWQtOCB7XFxuICAgIG9yZGVyOiA4OyB9XFxuICAub3JkZXItbWQtOSB7XFxuICAgIG9yZGVyOiA5OyB9XFxuICAub3JkZXItbWQtMTAge1xcbiAgICBvcmRlcjogMTA7IH1cXG4gIC5vcmRlci1tZC0xMSB7XFxuICAgIG9yZGVyOiAxMTsgfVxcbiAgLm9yZGVyLW1kLTEyIHtcXG4gICAgb3JkZXI6IDEyOyB9XFxuICAub2Zmc2V0LW1kLTAge1xcbiAgICBtYXJnaW4tbGVmdDogMDsgfVxcbiAgLm9mZnNldC1tZC0xIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDguMzMzMzMlOyB9XFxuICAub2Zmc2V0LW1kLTIge1xcbiAgICBtYXJnaW4tbGVmdDogMTYuNjY2NjclOyB9XFxuICAub2Zmc2V0LW1kLTMge1xcbiAgICBtYXJnaW4tbGVmdDogMjUlOyB9XFxuICAub2Zmc2V0LW1kLTQge1xcbiAgICBtYXJnaW4tbGVmdDogMzMuMzMzMzMlOyB9XFxuICAub2Zmc2V0LW1kLTUge1xcbiAgICBtYXJnaW4tbGVmdDogNDEuNjY2NjclOyB9XFxuICAub2Zmc2V0LW1kLTYge1xcbiAgICBtYXJnaW4tbGVmdDogNTAlOyB9XFxuICAub2Zmc2V0LW1kLTcge1xcbiAgICBtYXJnaW4tbGVmdDogNTguMzMzMzMlOyB9XFxuICAub2Zmc2V0LW1kLTgge1xcbiAgICBtYXJnaW4tbGVmdDogNjYuNjY2NjclOyB9XFxuICAub2Zmc2V0LW1kLTkge1xcbiAgICBtYXJnaW4tbGVmdDogNzUlOyB9XFxuICAub2Zmc2V0LW1kLTEwIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDgzLjMzMzMzJTsgfVxcbiAgLm9mZnNldC1tZC0xMSB7XFxuICAgIG1hcmdpbi1sZWZ0OiA5MS42NjY2NyU7IH0gfVxcblxcbkBtZWRpYSAobWluLXdpZHRoOiA5OTJweCkge1xcbiAgLmNvbC1sZyB7XFxuICAgIGZsZXgtYmFzaXM6IDA7XFxuICAgIGZsZXgtZ3JvdzogMTtcXG4gICAgbWF4LXdpZHRoOiAxMDAlOyB9XFxuICAuY29sLWxnLWF1dG8ge1xcbiAgICBmbGV4OiAwIDAgYXV0bztcXG4gICAgd2lkdGg6IGF1dG87XFxuICAgIG1heC13aWR0aDogMTAwJTsgfVxcbiAgLmNvbC1sZy0xIHtcXG4gICAgZmxleDogMCAwIDguMzMzMzMlO1xcbiAgICBtYXgtd2lkdGg6IDguMzMzMzMlOyB9XFxuICAuY29sLWxnLTIge1xcbiAgICBmbGV4OiAwIDAgMTYuNjY2NjclO1xcbiAgICBtYXgtd2lkdGg6IDE2LjY2NjY3JTsgfVxcbiAgLmNvbC1sZy0zIHtcXG4gICAgZmxleDogMCAwIDI1JTtcXG4gICAgbWF4LXdpZHRoOiAyNSU7IH1cXG4gIC5jb2wtbGctNCB7XFxuICAgIGZsZXg6IDAgMCAzMy4zMzMzMyU7XFxuICAgIG1heC13aWR0aDogMzMuMzMzMzMlOyB9XFxuICAuY29sLWxnLTUge1xcbiAgICBmbGV4OiAwIDAgNDEuNjY2NjclO1xcbiAgICBtYXgtd2lkdGg6IDQxLjY2NjY3JTsgfVxcbiAgLmNvbC1sZy02IHtcXG4gICAgZmxleDogMCAwIDUwJTtcXG4gICAgbWF4LXdpZHRoOiA1MCU7IH1cXG4gIC5jb2wtbGctNyB7XFxuICAgIGZsZXg6IDAgMCA1OC4zMzMzMyU7XFxuICAgIG1heC13aWR0aDogNTguMzMzMzMlOyB9XFxuICAuY29sLWxnLTgge1xcbiAgICBmbGV4OiAwIDAgNjYuNjY2NjclO1xcbiAgICBtYXgtd2lkdGg6IDY2LjY2NjY3JTsgfVxcbiAgLmNvbC1sZy05IHtcXG4gICAgZmxleDogMCAwIDc1JTtcXG4gICAgbWF4LXdpZHRoOiA3NSU7IH1cXG4gIC5jb2wtbGctMTAge1xcbiAgICBmbGV4OiAwIDAgODMuMzMzMzMlO1xcbiAgICBtYXgtd2lkdGg6IDgzLjMzMzMzJTsgfVxcbiAgLmNvbC1sZy0xMSB7XFxuICAgIGZsZXg6IDAgMCA5MS42NjY2NyU7XFxuICAgIG1heC13aWR0aDogOTEuNjY2NjclOyB9XFxuICAuY29sLWxnLTEyIHtcXG4gICAgZmxleDogMCAwIDEwMCU7XFxuICAgIG1heC13aWR0aDogMTAwJTsgfVxcbiAgLm9yZGVyLWxnLWZpcnN0IHtcXG4gICAgb3JkZXI6IC0xOyB9XFxuICAub3JkZXItbGctbGFzdCB7XFxuICAgIG9yZGVyOiAxMzsgfVxcbiAgLm9yZGVyLWxnLTAge1xcbiAgICBvcmRlcjogMDsgfVxcbiAgLm9yZGVyLWxnLTEge1xcbiAgICBvcmRlcjogMTsgfVxcbiAgLm9yZGVyLWxnLTIge1xcbiAgICBvcmRlcjogMjsgfVxcbiAgLm9yZGVyLWxnLTMge1xcbiAgICBvcmRlcjogMzsgfVxcbiAgLm9yZGVyLWxnLTQge1xcbiAgICBvcmRlcjogNDsgfVxcbiAgLm9yZGVyLWxnLTUge1xcbiAgICBvcmRlcjogNTsgfVxcbiAgLm9yZGVyLWxnLTYge1xcbiAgICBvcmRlcjogNjsgfVxcbiAgLm9yZGVyLWxnLTcge1xcbiAgICBvcmRlcjogNzsgfVxcbiAgLm9yZGVyLWxnLTgge1xcbiAgICBvcmRlcjogODsgfVxcbiAgLm9yZGVyLWxnLTkge1xcbiAgICBvcmRlcjogOTsgfVxcbiAgLm9yZGVyLWxnLTEwIHtcXG4gICAgb3JkZXI6IDEwOyB9XFxuICAub3JkZXItbGctMTEge1xcbiAgICBvcmRlcjogMTE7IH1cXG4gIC5vcmRlci1sZy0xMiB7XFxuICAgIG9yZGVyOiAxMjsgfVxcbiAgLm9mZnNldC1sZy0wIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDA7IH1cXG4gIC5vZmZzZXQtbGctMSB7XFxuICAgIG1hcmdpbi1sZWZ0OiA4LjMzMzMzJTsgfVxcbiAgLm9mZnNldC1sZy0yIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDE2LjY2NjY3JTsgfVxcbiAgLm9mZnNldC1sZy0zIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDI1JTsgfVxcbiAgLm9mZnNldC1sZy00IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDMzLjMzMzMzJTsgfVxcbiAgLm9mZnNldC1sZy01IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDQxLjY2NjY3JTsgfVxcbiAgLm9mZnNldC1sZy02IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDUwJTsgfVxcbiAgLm9mZnNldC1sZy03IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDU4LjMzMzMzJTsgfVxcbiAgLm9mZnNldC1sZy04IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDY2LjY2NjY3JTsgfVxcbiAgLm9mZnNldC1sZy05IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDc1JTsgfVxcbiAgLm9mZnNldC1sZy0xMCB7XFxuICAgIG1hcmdpbi1sZWZ0OiA4My4zMzMzMyU7IH1cXG4gIC5vZmZzZXQtbGctMTEge1xcbiAgICBtYXJnaW4tbGVmdDogOTEuNjY2NjclOyB9IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogMTIwMHB4KSB7XFxuICAuY29sLXhsIHtcXG4gICAgZmxleC1iYXNpczogMDtcXG4gICAgZmxleC1ncm93OiAxO1xcbiAgICBtYXgtd2lkdGg6IDEwMCU7IH1cXG4gIC5jb2wteGwtYXV0byB7XFxuICAgIGZsZXg6IDAgMCBhdXRvO1xcbiAgICB3aWR0aDogYXV0bztcXG4gICAgbWF4LXdpZHRoOiAxMDAlOyB9XFxuICAuY29sLXhsLTEge1xcbiAgICBmbGV4OiAwIDAgOC4zMzMzMyU7XFxuICAgIG1heC13aWR0aDogOC4zMzMzMyU7IH1cXG4gIC5jb2wteGwtMiB7XFxuICAgIGZsZXg6IDAgMCAxNi42NjY2NyU7XFxuICAgIG1heC13aWR0aDogMTYuNjY2NjclOyB9XFxuICAuY29sLXhsLTMge1xcbiAgICBmbGV4OiAwIDAgMjUlO1xcbiAgICBtYXgtd2lkdGg6IDI1JTsgfVxcbiAgLmNvbC14bC00IHtcXG4gICAgZmxleDogMCAwIDMzLjMzMzMzJTtcXG4gICAgbWF4LXdpZHRoOiAzMy4zMzMzMyU7IH1cXG4gIC5jb2wteGwtNSB7XFxuICAgIGZsZXg6IDAgMCA0MS42NjY2NyU7XFxuICAgIG1heC13aWR0aDogNDEuNjY2NjclOyB9XFxuICAuY29sLXhsLTYge1xcbiAgICBmbGV4OiAwIDAgNTAlO1xcbiAgICBtYXgtd2lkdGg6IDUwJTsgfVxcbiAgLmNvbC14bC03IHtcXG4gICAgZmxleDogMCAwIDU4LjMzMzMzJTtcXG4gICAgbWF4LXdpZHRoOiA1OC4zMzMzMyU7IH1cXG4gIC5jb2wteGwtOCB7XFxuICAgIGZsZXg6IDAgMCA2Ni42NjY2NyU7XFxuICAgIG1heC13aWR0aDogNjYuNjY2NjclOyB9XFxuICAuY29sLXhsLTkge1xcbiAgICBmbGV4OiAwIDAgNzUlO1xcbiAgICBtYXgtd2lkdGg6IDc1JTsgfVxcbiAgLmNvbC14bC0xMCB7XFxuICAgIGZsZXg6IDAgMCA4My4zMzMzMyU7XFxuICAgIG1heC13aWR0aDogODMuMzMzMzMlOyB9XFxuICAuY29sLXhsLTExIHtcXG4gICAgZmxleDogMCAwIDkxLjY2NjY3JTtcXG4gICAgbWF4LXdpZHRoOiA5MS42NjY2NyU7IH1cXG4gIC5jb2wteGwtMTIge1xcbiAgICBmbGV4OiAwIDAgMTAwJTtcXG4gICAgbWF4LXdpZHRoOiAxMDAlOyB9XFxuICAub3JkZXIteGwtZmlyc3Qge1xcbiAgICBvcmRlcjogLTE7IH1cXG4gIC5vcmRlci14bC1sYXN0IHtcXG4gICAgb3JkZXI6IDEzOyB9XFxuICAub3JkZXIteGwtMCB7XFxuICAgIG9yZGVyOiAwOyB9XFxuICAub3JkZXIteGwtMSB7XFxuICAgIG9yZGVyOiAxOyB9XFxuICAub3JkZXIteGwtMiB7XFxuICAgIG9yZGVyOiAyOyB9XFxuICAub3JkZXIteGwtMyB7XFxuICAgIG9yZGVyOiAzOyB9XFxuICAub3JkZXIteGwtNCB7XFxuICAgIG9yZGVyOiA0OyB9XFxuICAub3JkZXIteGwtNSB7XFxuICAgIG9yZGVyOiA1OyB9XFxuICAub3JkZXIteGwtNiB7XFxuICAgIG9yZGVyOiA2OyB9XFxuICAub3JkZXIteGwtNyB7XFxuICAgIG9yZGVyOiA3OyB9XFxuICAub3JkZXIteGwtOCB7XFxuICAgIG9yZGVyOiA4OyB9XFxuICAub3JkZXIteGwtOSB7XFxuICAgIG9yZGVyOiA5OyB9XFxuICAub3JkZXIteGwtMTAge1xcbiAgICBvcmRlcjogMTA7IH1cXG4gIC5vcmRlci14bC0xMSB7XFxuICAgIG9yZGVyOiAxMTsgfVxcbiAgLm9yZGVyLXhsLTEyIHtcXG4gICAgb3JkZXI6IDEyOyB9XFxuICAub2Zmc2V0LXhsLTAge1xcbiAgICBtYXJnaW4tbGVmdDogMDsgfVxcbiAgLm9mZnNldC14bC0xIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDguMzMzMzMlOyB9XFxuICAub2Zmc2V0LXhsLTIge1xcbiAgICBtYXJnaW4tbGVmdDogMTYuNjY2NjclOyB9XFxuICAub2Zmc2V0LXhsLTMge1xcbiAgICBtYXJnaW4tbGVmdDogMjUlOyB9XFxuICAub2Zmc2V0LXhsLTQge1xcbiAgICBtYXJnaW4tbGVmdDogMzMuMzMzMzMlOyB9XFxuICAub2Zmc2V0LXhsLTUge1xcbiAgICBtYXJnaW4tbGVmdDogNDEuNjY2NjclOyB9XFxuICAub2Zmc2V0LXhsLTYge1xcbiAgICBtYXJnaW4tbGVmdDogNTAlOyB9XFxuICAub2Zmc2V0LXhsLTcge1xcbiAgICBtYXJnaW4tbGVmdDogNTguMzMzMzMlOyB9XFxuICAub2Zmc2V0LXhsLTgge1xcbiAgICBtYXJnaW4tbGVmdDogNjYuNjY2NjclOyB9XFxuICAub2Zmc2V0LXhsLTkge1xcbiAgICBtYXJnaW4tbGVmdDogNzUlOyB9XFxuICAub2Zmc2V0LXhsLTEwIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDgzLjMzMzMzJTsgfVxcbiAgLm9mZnNldC14bC0xMSB7XFxuICAgIG1hcmdpbi1sZWZ0OiA5MS42NjY2NyU7IH0gfVxcblxcbmgxLCBoMiwgaDMsIGg0LCBoNSwgaDYsXFxuLmgxLCAuaDIsIC5oMywgLmg0LCAuaDUsIC5oNiB7XFxuICBtYXJnaW4tYm90dG9tOiAwLjVyZW07XFxuICBmb250LXdlaWdodDogNTAwO1xcbiAgbGluZS1oZWlnaHQ6IDEuMjsgfVxcblxcbmgxLCAuaDEge1xcbiAgZm9udC1zaXplOiAyLjVyZW07IH1cXG5cXG5oMiwgLmgyIHtcXG4gIGZvbnQtc2l6ZTogMnJlbTsgfVxcblxcbmgzLCAuaDMge1xcbiAgZm9udC1zaXplOiAxLjc1cmVtOyB9XFxuXFxuaDQsIC5oNCB7XFxuICBmb250LXNpemU6IDEuNXJlbTsgfVxcblxcbmg1LCAuaDUge1xcbiAgZm9udC1zaXplOiAxLjI1cmVtOyB9XFxuXFxuaDYsIC5oNiB7XFxuICBmb250LXNpemU6IDFyZW07IH1cXG5cXG4ubGVhZCB7XFxuICBmb250LXNpemU6IDEuMjVyZW07XFxuICBmb250LXdlaWdodDogMzAwOyB9XFxuXFxuLmRpc3BsYXktMSB7XFxuICBmb250LXNpemU6IDZyZW07XFxuICBmb250LXdlaWdodDogMzAwO1xcbiAgbGluZS1oZWlnaHQ6IDEuMjsgfVxcblxcbi5kaXNwbGF5LTIge1xcbiAgZm9udC1zaXplOiA1LjVyZW07XFxuICBmb250LXdlaWdodDogMzAwO1xcbiAgbGluZS1oZWlnaHQ6IDEuMjsgfVxcblxcbi5kaXNwbGF5LTMge1xcbiAgZm9udC1zaXplOiA0LjVyZW07XFxuICBmb250LXdlaWdodDogMzAwO1xcbiAgbGluZS1oZWlnaHQ6IDEuMjsgfVxcblxcbi5kaXNwbGF5LTQge1xcbiAgZm9udC1zaXplOiAzLjVyZW07XFxuICBmb250LXdlaWdodDogMzAwO1xcbiAgbGluZS1oZWlnaHQ6IDEuMjsgfVxcblxcbmhyIHtcXG4gIG1hcmdpbi10b3A6IDFyZW07XFxuICBtYXJnaW4tYm90dG9tOiAxcmVtO1xcbiAgYm9yZGVyOiAwO1xcbiAgYm9yZGVyLXRvcDogMXB4IHNvbGlkIHJnYmEoMCwgMCwgMCwgMC4xKTsgfVxcblxcbnNtYWxsLFxcbi5zbWFsbCB7XFxuICBmb250LXNpemU6IDgwJTtcXG4gIGZvbnQtd2VpZ2h0OiA0MDA7IH1cXG5cXG5tYXJrLFxcbi5tYXJrIHtcXG4gIHBhZGRpbmc6IDAuMmVtO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2ZjZjhlMzsgfVxcblxcbi5saXN0LXVuc3R5bGVkIHtcXG4gIHBhZGRpbmctbGVmdDogMDtcXG4gIGxpc3Qtc3R5bGU6IG5vbmU7IH1cXG5cXG4ubGlzdC1pbmxpbmUge1xcbiAgcGFkZGluZy1sZWZ0OiAwO1xcbiAgbGlzdC1zdHlsZTogbm9uZTsgfVxcblxcbi5saXN0LWlubGluZS1pdGVtIHtcXG4gIGRpc3BsYXk6IGlubGluZS1ibG9jazsgfVxcbiAgLmxpc3QtaW5saW5lLWl0ZW06bm90KDpsYXN0LWNoaWxkKSB7XFxuICAgIG1hcmdpbi1yaWdodDogMC41cmVtOyB9XFxuXFxuLmluaXRpYWxpc20ge1xcbiAgZm9udC1zaXplOiA5MCU7XFxuICB0ZXh0LXRyYW5zZm9ybTogdXBwZXJjYXNlOyB9XFxuXFxuLmJsb2NrcXVvdGUge1xcbiAgbWFyZ2luLWJvdHRvbTogMXJlbTtcXG4gIGZvbnQtc2l6ZTogMS4yNXJlbTsgfVxcblxcbi5ibG9ja3F1b3RlLWZvb3RlciB7XFxuICBkaXNwbGF5OiBibG9jaztcXG4gIGZvbnQtc2l6ZTogODAlO1xcbiAgY29sb3I6ICM2Yzc1N2Q7IH1cXG4gIC5ibG9ja3F1b3RlLWZvb3Rlcjo6YmVmb3JlIHtcXG4gICAgY29udGVudDogXFxcIlxcXFwyMDE0XFxcXDAwQTBcXFwiOyB9XFxuXFxuLmJyZWFkY3J1bWIge1xcbiAgZGlzcGxheTogZmxleDtcXG4gIGZsZXgtd3JhcDogd3JhcDtcXG4gIHBhZGRpbmc6IDAuNzVyZW0gMXJlbTtcXG4gIG1hcmdpbi1ib3R0b206IDFyZW07XFxuICBsaXN0LXN0eWxlOiBub25lO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2U5ZWNlZjtcXG4gIGJvcmRlci1yYWRpdXM6IDAuMjVyZW07IH1cXG5cXG4uYnJlYWRjcnVtYi1pdGVtICsgLmJyZWFkY3J1bWItaXRlbSB7XFxuICBwYWRkaW5nLWxlZnQ6IDAuNXJlbTsgfVxcbiAgLmJyZWFkY3J1bWItaXRlbSArIC5icmVhZGNydW1iLWl0ZW06OmJlZm9yZSB7XFxuICAgIGRpc3BsYXk6IGlubGluZS1ibG9jaztcXG4gICAgcGFkZGluZy1yaWdodDogMC41cmVtO1xcbiAgICBjb2xvcjogIzZjNzU3ZDtcXG4gICAgY29udGVudDogXFxcIi9cXFwiOyB9XFxuXFxuLmJyZWFkY3J1bWItaXRlbSArIC5icmVhZGNydW1iLWl0ZW06aG92ZXI6OmJlZm9yZSB7XFxuICB0ZXh0LWRlY29yYXRpb246IHVuZGVybGluZTsgfVxcblxcbi5icmVhZGNydW1iLWl0ZW0gKyAuYnJlYWRjcnVtYi1pdGVtOmhvdmVyOjpiZWZvcmUge1xcbiAgdGV4dC1kZWNvcmF0aW9uOiBub25lOyB9XFxuXFxuLmJyZWFkY3J1bWItaXRlbS5hY3RpdmUge1xcbiAgY29sb3I6ICM2Yzc1N2Q7IH1cXG5cXG4udGFibGUge1xcbiAgd2lkdGg6IDEwMCU7XFxuICBtYXJnaW4tYm90dG9tOiAxcmVtO1xcbiAgY29sb3I6ICMyMTI1Mjk7IH1cXG4gIC50YWJsZSB0aCxcXG4gIC50YWJsZSB0ZCB7XFxuICAgIHBhZGRpbmc6IDAuNzVyZW07XFxuICAgIHZlcnRpY2FsLWFsaWduOiB0b3A7XFxuICAgIGJvcmRlci10b3A6IDFweCBzb2xpZCAjZGVlMmU2OyB9XFxuICAudGFibGUgdGhlYWQgdGgge1xcbiAgICB2ZXJ0aWNhbC1hbGlnbjogYm90dG9tO1xcbiAgICBib3JkZXItYm90dG9tOiAycHggc29saWQgI2RlZTJlNjsgfVxcbiAgLnRhYmxlIHRib2R5ICsgdGJvZHkge1xcbiAgICBib3JkZXItdG9wOiAycHggc29saWQgI2RlZTJlNjsgfVxcblxcbi50YWJsZS1zbSB0aCxcXG4udGFibGUtc20gdGQge1xcbiAgcGFkZGluZzogMC4zcmVtOyB9XFxuXFxuLnRhYmxlLWJvcmRlcmVkIHtcXG4gIGJvcmRlcjogMXB4IHNvbGlkICNkZWUyZTY7IH1cXG4gIC50YWJsZS1ib3JkZXJlZCB0aCxcXG4gIC50YWJsZS1ib3JkZXJlZCB0ZCB7XFxuICAgIGJvcmRlcjogMXB4IHNvbGlkICNkZWUyZTY7IH1cXG4gIC50YWJsZS1ib3JkZXJlZCB0aGVhZCB0aCxcXG4gIC50YWJsZS1ib3JkZXJlZCB0aGVhZCB0ZCB7XFxuICAgIGJvcmRlci1ib3R0b20td2lkdGg6IDJweDsgfVxcblxcbi50YWJsZS1ib3JkZXJsZXNzIHRoLFxcbi50YWJsZS1ib3JkZXJsZXNzIHRkLFxcbi50YWJsZS1ib3JkZXJsZXNzIHRoZWFkIHRoLFxcbi50YWJsZS1ib3JkZXJsZXNzIHRib2R5ICsgdGJvZHkge1xcbiAgYm9yZGVyOiAwOyB9XFxuXFxuLnRhYmxlLXN0cmlwZWQgdGJvZHkgdHI6bnRoLW9mLXR5cGUob2RkKSB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiByZ2JhKDAsIDAsIDAsIDAuMDUpOyB9XFxuXFxuLnRhYmxlLWhvdmVyIHRib2R5IHRyOmhvdmVyIHtcXG4gIGNvbG9yOiAjMjEyNTI5O1xcbiAgYmFja2dyb3VuZC1jb2xvcjogcmdiYSgwLCAwLCAwLCAwLjA3NSk7IH1cXG5cXG4udGFibGUtcHJpbWFyeSxcXG4udGFibGUtcHJpbWFyeSA+IHRoLFxcbi50YWJsZS1wcmltYXJ5ID4gdGQge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2I4ZGFmZjsgfVxcblxcbi50YWJsZS1wcmltYXJ5IHRoLFxcbi50YWJsZS1wcmltYXJ5IHRkLFxcbi50YWJsZS1wcmltYXJ5IHRoZWFkIHRoLFxcbi50YWJsZS1wcmltYXJ5IHRib2R5ICsgdGJvZHkge1xcbiAgYm9yZGVyLWNvbG9yOiAjN2FiYWZmOyB9XFxuXFxuLnRhYmxlLWhvdmVyIC50YWJsZS1wcmltYXJ5OmhvdmVyIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICM5ZmNkZmY7IH1cXG4gIC50YWJsZS1ob3ZlciAudGFibGUtcHJpbWFyeTpob3ZlciA+IHRkLFxcbiAgLnRhYmxlLWhvdmVyIC50YWJsZS1wcmltYXJ5OmhvdmVyID4gdGgge1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjOWZjZGZmOyB9XFxuXFxuLnRhYmxlLXNlY29uZGFyeSxcXG4udGFibGUtc2Vjb25kYXJ5ID4gdGgsXFxuLnRhYmxlLXNlY29uZGFyeSA+IHRkIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNkNmQ4ZGI7IH1cXG5cXG4udGFibGUtc2Vjb25kYXJ5IHRoLFxcbi50YWJsZS1zZWNvbmRhcnkgdGQsXFxuLnRhYmxlLXNlY29uZGFyeSB0aGVhZCB0aCxcXG4udGFibGUtc2Vjb25kYXJ5IHRib2R5ICsgdGJvZHkge1xcbiAgYm9yZGVyLWNvbG9yOiAjYjNiN2JiOyB9XFxuXFxuLnRhYmxlLWhvdmVyIC50YWJsZS1zZWNvbmRhcnk6aG92ZXIge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2M4Y2JjZjsgfVxcbiAgLnRhYmxlLWhvdmVyIC50YWJsZS1zZWNvbmRhcnk6aG92ZXIgPiB0ZCxcXG4gIC50YWJsZS1ob3ZlciAudGFibGUtc2Vjb25kYXJ5OmhvdmVyID4gdGgge1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjYzhjYmNmOyB9XFxuXFxuLnRhYmxlLXN1Y2Nlc3MsXFxuLnRhYmxlLXN1Y2Nlc3MgPiB0aCxcXG4udGFibGUtc3VjY2VzcyA+IHRkIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNjM2U2Y2I7IH1cXG5cXG4udGFibGUtc3VjY2VzcyB0aCxcXG4udGFibGUtc3VjY2VzcyB0ZCxcXG4udGFibGUtc3VjY2VzcyB0aGVhZCB0aCxcXG4udGFibGUtc3VjY2VzcyB0Ym9keSArIHRib2R5IHtcXG4gIGJvcmRlci1jb2xvcjogIzhmZDE5ZTsgfVxcblxcbi50YWJsZS1ob3ZlciAudGFibGUtc3VjY2Vzczpob3ZlciB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjYjFkZmJiOyB9XFxuICAudGFibGUtaG92ZXIgLnRhYmxlLXN1Y2Nlc3M6aG92ZXIgPiB0ZCxcXG4gIC50YWJsZS1ob3ZlciAudGFibGUtc3VjY2Vzczpob3ZlciA+IHRoIHtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2IxZGZiYjsgfVxcblxcbi50YWJsZS1pbmZvLFxcbi50YWJsZS1pbmZvID4gdGgsXFxuLnRhYmxlLWluZm8gPiB0ZCB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjYmVlNWViOyB9XFxuXFxuLnRhYmxlLWluZm8gdGgsXFxuLnRhYmxlLWluZm8gdGQsXFxuLnRhYmxlLWluZm8gdGhlYWQgdGgsXFxuLnRhYmxlLWluZm8gdGJvZHkgKyB0Ym9keSB7XFxuICBib3JkZXItY29sb3I6ICM4NmNmZGE7IH1cXG5cXG4udGFibGUtaG92ZXIgLnRhYmxlLWluZm86aG92ZXIge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2FiZGRlNTsgfVxcbiAgLnRhYmxlLWhvdmVyIC50YWJsZS1pbmZvOmhvdmVyID4gdGQsXFxuICAudGFibGUtaG92ZXIgLnRhYmxlLWluZm86aG92ZXIgPiB0aCB7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNhYmRkZTU7IH1cXG5cXG4udGFibGUtd2FybmluZyxcXG4udGFibGUtd2FybmluZyA+IHRoLFxcbi50YWJsZS13YXJuaW5nID4gdGQge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2ZmZWViYTsgfVxcblxcbi50YWJsZS13YXJuaW5nIHRoLFxcbi50YWJsZS13YXJuaW5nIHRkLFxcbi50YWJsZS13YXJuaW5nIHRoZWFkIHRoLFxcbi50YWJsZS13YXJuaW5nIHRib2R5ICsgdGJvZHkge1xcbiAgYm9yZGVyLWNvbG9yOiAjZmZkZjdlOyB9XFxuXFxuLnRhYmxlLWhvdmVyIC50YWJsZS13YXJuaW5nOmhvdmVyIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNmZmU4YTE7IH1cXG4gIC50YWJsZS1ob3ZlciAudGFibGUtd2FybmluZzpob3ZlciA+IHRkLFxcbiAgLnRhYmxlLWhvdmVyIC50YWJsZS13YXJuaW5nOmhvdmVyID4gdGgge1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmZlOGExOyB9XFxuXFxuLnRhYmxlLWRhbmdlcixcXG4udGFibGUtZGFuZ2VyID4gdGgsXFxuLnRhYmxlLWRhbmdlciA+IHRkIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNmNWM2Y2I7IH1cXG5cXG4udGFibGUtZGFuZ2VyIHRoLFxcbi50YWJsZS1kYW5nZXIgdGQsXFxuLnRhYmxlLWRhbmdlciB0aGVhZCB0aCxcXG4udGFibGUtZGFuZ2VyIHRib2R5ICsgdGJvZHkge1xcbiAgYm9yZGVyLWNvbG9yOiAjZWQ5NjllOyB9XFxuXFxuLnRhYmxlLWhvdmVyIC50YWJsZS1kYW5nZXI6aG92ZXIge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2YxYjBiNzsgfVxcbiAgLnRhYmxlLWhvdmVyIC50YWJsZS1kYW5nZXI6aG92ZXIgPiB0ZCxcXG4gIC50YWJsZS1ob3ZlciAudGFibGUtZGFuZ2VyOmhvdmVyID4gdGgge1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZjFiMGI3OyB9XFxuXFxuLnRhYmxlLWxpZ2h0LFxcbi50YWJsZS1saWdodCA+IHRoLFxcbi50YWJsZS1saWdodCA+IHRkIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNmZGZkZmU7IH1cXG5cXG4udGFibGUtbGlnaHQgdGgsXFxuLnRhYmxlLWxpZ2h0IHRkLFxcbi50YWJsZS1saWdodCB0aGVhZCB0aCxcXG4udGFibGUtbGlnaHQgdGJvZHkgKyB0Ym9keSB7XFxuICBib3JkZXItY29sb3I6ICNmYmZjZmM7IH1cXG5cXG4udGFibGUtaG92ZXIgLnRhYmxlLWxpZ2h0OmhvdmVyIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNlY2VjZjY7IH1cXG4gIC50YWJsZS1ob3ZlciAudGFibGUtbGlnaHQ6aG92ZXIgPiB0ZCxcXG4gIC50YWJsZS1ob3ZlciAudGFibGUtbGlnaHQ6aG92ZXIgPiB0aCB7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNlY2VjZjY7IH1cXG5cXG4udGFibGUtZGFyayxcXG4udGFibGUtZGFyayA+IHRoLFxcbi50YWJsZS1kYXJrID4gdGQge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2M2YzhjYTsgfVxcblxcbi50YWJsZS1kYXJrIHRoLFxcbi50YWJsZS1kYXJrIHRkLFxcbi50YWJsZS1kYXJrIHRoZWFkIHRoLFxcbi50YWJsZS1kYXJrIHRib2R5ICsgdGJvZHkge1xcbiAgYm9yZGVyLWNvbG9yOiAjOTU5OTljOyB9XFxuXFxuLnRhYmxlLWhvdmVyIC50YWJsZS1kYXJrOmhvdmVyIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNiOWJiYmU7IH1cXG4gIC50YWJsZS1ob3ZlciAudGFibGUtZGFyazpob3ZlciA+IHRkLFxcbiAgLnRhYmxlLWhvdmVyIC50YWJsZS1kYXJrOmhvdmVyID4gdGgge1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjYjliYmJlOyB9XFxuXFxuLnRhYmxlLWFjdGl2ZSxcXG4udGFibGUtYWN0aXZlID4gdGgsXFxuLnRhYmxlLWFjdGl2ZSA+IHRkIHtcXG4gIGJhY2tncm91bmQtY29sb3I6IHJnYmEoMCwgMCwgMCwgMC4wNzUpOyB9XFxuXFxuLnRhYmxlLWhvdmVyIC50YWJsZS1hY3RpdmU6aG92ZXIge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogcmdiYSgwLCAwLCAwLCAwLjA3NSk7IH1cXG4gIC50YWJsZS1ob3ZlciAudGFibGUtYWN0aXZlOmhvdmVyID4gdGQsXFxuICAudGFibGUtaG92ZXIgLnRhYmxlLWFjdGl2ZTpob3ZlciA+IHRoIHtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogcmdiYSgwLCAwLCAwLCAwLjA3NSk7IH1cXG5cXG4udGFibGUgLnRoZWFkLWRhcmsgdGgge1xcbiAgY29sb3I6ICNmZmY7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjMzQzYTQwO1xcbiAgYm9yZGVyLWNvbG9yOiAjNDU0ZDU1OyB9XFxuXFxuLnRhYmxlIC50aGVhZC1saWdodCB0aCB7XFxuICBjb2xvcjogIzQ5NTA1NztcXG4gIGJhY2tncm91bmQtY29sb3I6ICNlOWVjZWY7XFxuICBib3JkZXItY29sb3I6ICNkZWUyZTY7IH1cXG5cXG4udGFibGUtZGFyayB7XFxuICBjb2xvcjogI2ZmZjtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMzNDNhNDA7IH1cXG4gIC50YWJsZS1kYXJrIHRoLFxcbiAgLnRhYmxlLWRhcmsgdGQsXFxuICAudGFibGUtZGFyayB0aGVhZCB0aCB7XFxuICAgIGJvcmRlci1jb2xvcjogIzQ1NGQ1NTsgfVxcbiAgLnRhYmxlLWRhcmsudGFibGUtYm9yZGVyZWQge1xcbiAgICBib3JkZXI6IDA7IH1cXG4gIC50YWJsZS1kYXJrLnRhYmxlLXN0cmlwZWQgdGJvZHkgdHI6bnRoLW9mLXR5cGUob2RkKSB7XFxuICAgIGJhY2tncm91bmQtY29sb3I6IHJnYmEoMjU1LCAyNTUsIDI1NSwgMC4wNSk7IH1cXG4gIC50YWJsZS1kYXJrLnRhYmxlLWhvdmVyIHRib2R5IHRyOmhvdmVyIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6IHJnYmEoMjU1LCAyNTUsIDI1NSwgMC4wNzUpOyB9XFxuXFxuQG1lZGlhIChtYXgtd2lkdGg6IDU3NS45OHB4KSB7XFxuICAudGFibGUtcmVzcG9uc2l2ZS1zbSB7XFxuICAgIGRpc3BsYXk6IGJsb2NrO1xcbiAgICB3aWR0aDogMTAwJTtcXG4gICAgb3ZlcmZsb3cteDogYXV0bztcXG4gICAgLXdlYmtpdC1vdmVyZmxvdy1zY3JvbGxpbmc6IHRvdWNoOyB9XFxuICAgIC50YWJsZS1yZXNwb25zaXZlLXNtID4gLnRhYmxlLWJvcmRlcmVkIHtcXG4gICAgICBib3JkZXI6IDA7IH0gfVxcblxcbkBtZWRpYSAobWF4LXdpZHRoOiA3NjcuOThweCkge1xcbiAgLnRhYmxlLXJlc3BvbnNpdmUtbWQge1xcbiAgICBkaXNwbGF5OiBibG9jaztcXG4gICAgd2lkdGg6IDEwMCU7XFxuICAgIG92ZXJmbG93LXg6IGF1dG87XFxuICAgIC13ZWJraXQtb3ZlcmZsb3ctc2Nyb2xsaW5nOiB0b3VjaDsgfVxcbiAgICAudGFibGUtcmVzcG9uc2l2ZS1tZCA+IC50YWJsZS1ib3JkZXJlZCB7XFxuICAgICAgYm9yZGVyOiAwOyB9IH1cXG5cXG5AbWVkaWEgKG1heC13aWR0aDogOTkxLjk4cHgpIHtcXG4gIC50YWJsZS1yZXNwb25zaXZlLWxnIHtcXG4gICAgZGlzcGxheTogYmxvY2s7XFxuICAgIHdpZHRoOiAxMDAlO1xcbiAgICBvdmVyZmxvdy14OiBhdXRvO1xcbiAgICAtd2Via2l0LW92ZXJmbG93LXNjcm9sbGluZzogdG91Y2g7IH1cXG4gICAgLnRhYmxlLXJlc3BvbnNpdmUtbGcgPiAudGFibGUtYm9yZGVyZWQge1xcbiAgICAgIGJvcmRlcjogMDsgfSB9XFxuXFxuQG1lZGlhIChtYXgtd2lkdGg6IDExOTkuOThweCkge1xcbiAgLnRhYmxlLXJlc3BvbnNpdmUteGwge1xcbiAgICBkaXNwbGF5OiBibG9jaztcXG4gICAgd2lkdGg6IDEwMCU7XFxuICAgIG92ZXJmbG93LXg6IGF1dG87XFxuICAgIC13ZWJraXQtb3ZlcmZsb3ctc2Nyb2xsaW5nOiB0b3VjaDsgfVxcbiAgICAudGFibGUtcmVzcG9uc2l2ZS14bCA+IC50YWJsZS1ib3JkZXJlZCB7XFxuICAgICAgYm9yZGVyOiAwOyB9IH1cXG5cXG4udGFibGUtcmVzcG9uc2l2ZSB7XFxuICBkaXNwbGF5OiBibG9jaztcXG4gIHdpZHRoOiAxMDAlO1xcbiAgb3ZlcmZsb3cteDogYXV0bztcXG4gIC13ZWJraXQtb3ZlcmZsb3ctc2Nyb2xsaW5nOiB0b3VjaDsgfVxcbiAgLnRhYmxlLXJlc3BvbnNpdmUgPiAudGFibGUtYm9yZGVyZWQge1xcbiAgICBib3JkZXI6IDA7IH1cXG5cXG4uYmFkZ2Uge1xcbiAgZGlzcGxheTogaW5saW5lLWJsb2NrO1xcbiAgcGFkZGluZzogMC4yNWVtIDAuNGVtO1xcbiAgZm9udC1zaXplOiA3NSU7XFxuICBmb250LXdlaWdodDogNzAwO1xcbiAgbGluZS1oZWlnaHQ6IDE7XFxuICB0ZXh0LWFsaWduOiBjZW50ZXI7XFxuICB3aGl0ZS1zcGFjZTogbm93cmFwO1xcbiAgdmVydGljYWwtYWxpZ246IGJhc2VsaW5lO1xcbiAgYm9yZGVyLXJhZGl1czogMC4yNXJlbTtcXG4gIHRyYW5zaXRpb246IGNvbG9yIDAuMTVzIGVhc2UtaW4tb3V0LCBiYWNrZ3JvdW5kLWNvbG9yIDAuMTVzIGVhc2UtaW4tb3V0LCBib3JkZXItY29sb3IgMC4xNXMgZWFzZS1pbi1vdXQsIGJveC1zaGFkb3cgMC4xNXMgZWFzZS1pbi1vdXQ7IH1cXG4gIEBtZWRpYSAocHJlZmVycy1yZWR1Y2VkLW1vdGlvbjogcmVkdWNlKSB7XFxuICAgIC5iYWRnZSB7XFxuICAgICAgdHJhbnNpdGlvbjogbm9uZTsgfSB9XFxuICBhLmJhZGdlOmhvdmVyLCBhLmJhZGdlOmZvY3VzIHtcXG4gICAgdGV4dC1kZWNvcmF0aW9uOiBub25lOyB9XFxuICAuYmFkZ2U6ZW1wdHkge1xcbiAgICBkaXNwbGF5OiBub25lOyB9XFxuXFxuLmJ0biAuYmFkZ2Uge1xcbiAgcG9zaXRpb246IHJlbGF0aXZlO1xcbiAgdG9wOiAtMXB4OyB9XFxuXFxuLmJhZGdlLXBpbGwge1xcbiAgcGFkZGluZy1yaWdodDogMC42ZW07XFxuICBwYWRkaW5nLWxlZnQ6IDAuNmVtO1xcbiAgYm9yZGVyLXJhZGl1czogMTByZW07IH1cXG5cXG4uYmFkZ2UtcHJpbWFyeSB7XFxuICBjb2xvcjogI2ZmZjtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMwMDdiZmY7IH1cXG4gIGEuYmFkZ2UtcHJpbWFyeTpob3ZlciwgYS5iYWRnZS1wcmltYXJ5OmZvY3VzIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICMwMDYyY2M7IH1cXG4gIGEuYmFkZ2UtcHJpbWFyeTpmb2N1cywgYS5iYWRnZS1wcmltYXJ5LmZvY3VzIHtcXG4gICAgb3V0bGluZTogMDtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMCwgMTIzLCAyNTUsIDAuNSk7IH1cXG5cXG4uYmFkZ2Utc2Vjb25kYXJ5IHtcXG4gIGNvbG9yOiAjZmZmO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogIzZjNzU3ZDsgfVxcbiAgYS5iYWRnZS1zZWNvbmRhcnk6aG92ZXIsIGEuYmFkZ2Utc2Vjb25kYXJ5OmZvY3VzIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICM1NDViNjI7IH1cXG4gIGEuYmFkZ2Utc2Vjb25kYXJ5OmZvY3VzLCBhLmJhZGdlLXNlY29uZGFyeS5mb2N1cyB7XFxuICAgIG91dGxpbmU6IDA7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDEwOCwgMTE3LCAxMjUsIDAuNSk7IH1cXG5cXG4uYmFkZ2Utc3VjY2VzcyB7XFxuICBjb2xvcjogI2ZmZjtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMyOGE3NDU7IH1cXG4gIGEuYmFkZ2Utc3VjY2Vzczpob3ZlciwgYS5iYWRnZS1zdWNjZXNzOmZvY3VzIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICMxZTdlMzQ7IH1cXG4gIGEuYmFkZ2Utc3VjY2Vzczpmb2N1cywgYS5iYWRnZS1zdWNjZXNzLmZvY3VzIHtcXG4gICAgb3V0bGluZTogMDtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoNDAsIDE2NywgNjksIDAuNSk7IH1cXG5cXG4uYmFkZ2UtaW5mbyB7XFxuICBjb2xvcjogI2ZmZjtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMxN2EyYjg7IH1cXG4gIGEuYmFkZ2UtaW5mbzpob3ZlciwgYS5iYWRnZS1pbmZvOmZvY3VzIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICMxMTdhOGI7IH1cXG4gIGEuYmFkZ2UtaW5mbzpmb2N1cywgYS5iYWRnZS1pbmZvLmZvY3VzIHtcXG4gICAgb3V0bGluZTogMDtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjMsIDE2MiwgMTg0LCAwLjUpOyB9XFxuXFxuLmJhZGdlLXdhcm5pbmcge1xcbiAgY29sb3I6ICMyMTI1Mjk7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmZjMTA3OyB9XFxuICBhLmJhZGdlLXdhcm5pbmc6aG92ZXIsIGEuYmFkZ2Utd2FybmluZzpmb2N1cyB7XFxuICAgIGNvbG9yOiAjMjEyNTI5O1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZDM5ZTAwOyB9XFxuICBhLmJhZGdlLXdhcm5pbmc6Zm9jdXMsIGEuYmFkZ2Utd2FybmluZy5mb2N1cyB7XFxuICAgIG91dGxpbmU6IDA7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDI1NSwgMTkzLCA3LCAwLjUpOyB9XFxuXFxuLmJhZGdlLWRhbmdlciB7XFxuICBjb2xvcjogI2ZmZjtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNkYzM1NDU7IH1cXG4gIGEuYmFkZ2UtZGFuZ2VyOmhvdmVyLCBhLmJhZGdlLWRhbmdlcjpmb2N1cyB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjYmQyMTMwOyB9XFxuICBhLmJhZGdlLWRhbmdlcjpmb2N1cywgYS5iYWRnZS1kYW5nZXIuZm9jdXMge1xcbiAgICBvdXRsaW5lOiAwO1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyMjAsIDUzLCA2OSwgMC41KTsgfVxcblxcbi5iYWRnZS1saWdodCB7XFxuICBjb2xvcjogIzIxMjUyOTtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNmOGY5ZmE7IH1cXG4gIGEuYmFkZ2UtbGlnaHQ6aG92ZXIsIGEuYmFkZ2UtbGlnaHQ6Zm9jdXMge1xcbiAgICBjb2xvcjogIzIxMjUyOTtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2RhZTBlNTsgfVxcbiAgYS5iYWRnZS1saWdodDpmb2N1cywgYS5iYWRnZS1saWdodC5mb2N1cyB7XFxuICAgIG91dGxpbmU6IDA7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDI0OCwgMjQ5LCAyNTAsIDAuNSk7IH1cXG5cXG4uYmFkZ2UtZGFyayB7XFxuICBjb2xvcjogI2ZmZjtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMzNDNhNDA7IH1cXG4gIGEuYmFkZ2UtZGFyazpob3ZlciwgYS5iYWRnZS1kYXJrOmZvY3VzIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICMxZDIxMjQ7IH1cXG4gIGEuYmFkZ2UtZGFyazpmb2N1cywgYS5iYWRnZS1kYXJrLmZvY3VzIHtcXG4gICAgb3V0bGluZTogMDtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoNTIsIDU4LCA2NCwgMC41KTsgfVxcblxcbi5idG4ge1xcbiAgZGlzcGxheTogaW5saW5lLWJsb2NrO1xcbiAgZm9udC13ZWlnaHQ6IDQwMDtcXG4gIGNvbG9yOiAjMjEyNTI5O1xcbiAgdGV4dC1hbGlnbjogY2VudGVyO1xcbiAgdmVydGljYWwtYWxpZ246IG1pZGRsZTtcXG4gIHVzZXItc2VsZWN0OiBub25lO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogdHJhbnNwYXJlbnQ7XFxuICBib3JkZXI6IDFweCBzb2xpZCB0cmFuc3BhcmVudDtcXG4gIHBhZGRpbmc6IDAuMzc1cmVtIDAuNzVyZW07XFxuICBmb250LXNpemU6IDFyZW07XFxuICBsaW5lLWhlaWdodDogMS41O1xcbiAgYm9yZGVyLXJhZGl1czogMC4yNXJlbTtcXG4gIHRyYW5zaXRpb246IGNvbG9yIDAuMTVzIGVhc2UtaW4tb3V0LCBiYWNrZ3JvdW5kLWNvbG9yIDAuMTVzIGVhc2UtaW4tb3V0LCBib3JkZXItY29sb3IgMC4xNXMgZWFzZS1pbi1vdXQsIGJveC1zaGFkb3cgMC4xNXMgZWFzZS1pbi1vdXQ7IH1cXG4gIEBtZWRpYSAocHJlZmVycy1yZWR1Y2VkLW1vdGlvbjogcmVkdWNlKSB7XFxuICAgIC5idG4ge1xcbiAgICAgIHRyYW5zaXRpb246IG5vbmU7IH0gfVxcbiAgLmJ0bjpob3ZlciB7XFxuICAgIGNvbG9yOiAjMjEyNTI5O1xcbiAgICB0ZXh0LWRlY29yYXRpb246IG5vbmU7IH1cXG4gIC5idG46Zm9jdXMsIC5idG4uZm9jdXMge1xcbiAgICBvdXRsaW5lOiAwO1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgwLCAxMjMsIDI1NSwgMC4yNSk7IH1cXG4gIC5idG4uZGlzYWJsZWQsIC5idG46ZGlzYWJsZWQge1xcbiAgICBvcGFjaXR5OiAwLjY1OyB9XFxuXFxuYS5idG4uZGlzYWJsZWQsXFxuZmllbGRzZXQ6ZGlzYWJsZWQgYS5idG4ge1xcbiAgcG9pbnRlci1ldmVudHM6IG5vbmU7IH1cXG5cXG4uYnRuLXByaW1hcnkge1xcbiAgY29sb3I6ICNmZmY7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjMDA3YmZmO1xcbiAgYm9yZGVyLWNvbG9yOiAjMDA3YmZmOyB9XFxuICAuYnRuLXByaW1hcnk6aG92ZXIge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzAwNjlkOTtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMDA2MmNjOyB9XFxuICAuYnRuLXByaW1hcnk6Zm9jdXMsIC5idG4tcHJpbWFyeS5mb2N1cyB7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDM4LCAxNDMsIDI1NSwgMC41KTsgfVxcbiAgLmJ0bi1wcmltYXJ5LmRpc2FibGVkLCAuYnRuLXByaW1hcnk6ZGlzYWJsZWQge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzAwN2JmZjtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMDA3YmZmOyB9XFxuICAuYnRuLXByaW1hcnk6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlLCAuYnRuLXByaW1hcnk6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlLFxcbiAgLnNob3cgPiAuYnRuLXByaW1hcnkuZHJvcGRvd24tdG9nZ2xlIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICMwMDYyY2M7XFxuICAgIGJvcmRlci1jb2xvcjogIzAwNWNiZjsgfVxcbiAgICAuYnRuLXByaW1hcnk6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlOmZvY3VzLCAuYnRuLXByaW1hcnk6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlOmZvY3VzLFxcbiAgICAuc2hvdyA+IC5idG4tcHJpbWFyeS5kcm9wZG93bi10b2dnbGU6Zm9jdXMge1xcbiAgICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDM4LCAxNDMsIDI1NSwgMC41KTsgfVxcblxcbi5idG4tc2Vjb25kYXJ5IHtcXG4gIGNvbG9yOiAjZmZmO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogIzZjNzU3ZDtcXG4gIGJvcmRlci1jb2xvcjogIzZjNzU3ZDsgfVxcbiAgLmJ0bi1zZWNvbmRhcnk6aG92ZXIge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzVhNjI2ODtcXG4gICAgYm9yZGVyLWNvbG9yOiAjNTQ1YjYyOyB9XFxuICAuYnRuLXNlY29uZGFyeTpmb2N1cywgLmJ0bi1zZWNvbmRhcnkuZm9jdXMge1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgxMzAsIDEzOCwgMTQ1LCAwLjUpOyB9XFxuICAuYnRuLXNlY29uZGFyeS5kaXNhYmxlZCwgLmJ0bi1zZWNvbmRhcnk6ZGlzYWJsZWQge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzZjNzU3ZDtcXG4gICAgYm9yZGVyLWNvbG9yOiAjNmM3NTdkOyB9XFxuICAuYnRuLXNlY29uZGFyeTpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmUsIC5idG4tc2Vjb25kYXJ5Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZSxcXG4gIC5zaG93ID4gLmJ0bi1zZWNvbmRhcnkuZHJvcGRvd24tdG9nZ2xlIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICM1NDViNjI7XFxuICAgIGJvcmRlci1jb2xvcjogIzRlNTU1YjsgfVxcbiAgICAuYnRuLXNlY29uZGFyeTpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmU6Zm9jdXMsIC5idG4tc2Vjb25kYXJ5Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZTpmb2N1cyxcXG4gICAgLnNob3cgPiAuYnRuLXNlY29uZGFyeS5kcm9wZG93bi10b2dnbGU6Zm9jdXMge1xcbiAgICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDEzMCwgMTM4LCAxNDUsIDAuNSk7IH1cXG5cXG4uYnRuLXN1Y2Nlc3Mge1xcbiAgY29sb3I6ICNmZmY7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjMjhhNzQ1O1xcbiAgYm9yZGVyLWNvbG9yOiAjMjhhNzQ1OyB9XFxuICAuYnRuLXN1Y2Nlc3M6aG92ZXIge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzIxODgzODtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMWU3ZTM0OyB9XFxuICAuYnRuLXN1Y2Nlc3M6Zm9jdXMsIC5idG4tc3VjY2Vzcy5mb2N1cyB7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDcyLCAxODAsIDk3LCAwLjUpOyB9XFxuICAuYnRuLXN1Y2Nlc3MuZGlzYWJsZWQsIC5idG4tc3VjY2VzczpkaXNhYmxlZCB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjMjhhNzQ1O1xcbiAgICBib3JkZXItY29sb3I6ICMyOGE3NDU7IH1cXG4gIC5idG4tc3VjY2Vzczpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmUsIC5idG4tc3VjY2Vzczpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmUsXFxuICAuc2hvdyA+IC5idG4tc3VjY2Vzcy5kcm9wZG93bi10b2dnbGUge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzFlN2UzNDtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMWM3NDMwOyB9XFxuICAgIC5idG4tc3VjY2Vzczpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmU6Zm9jdXMsIC5idG4tc3VjY2Vzczpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmU6Zm9jdXMsXFxuICAgIC5zaG93ID4gLmJ0bi1zdWNjZXNzLmRyb3Bkb3duLXRvZ2dsZTpmb2N1cyB7XFxuICAgICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoNzIsIDE4MCwgOTcsIDAuNSk7IH1cXG5cXG4uYnRuLWluZm8ge1xcbiAgY29sb3I6ICNmZmY7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjMTdhMmI4O1xcbiAgYm9yZGVyLWNvbG9yOiAjMTdhMmI4OyB9XFxuICAuYnRuLWluZm86aG92ZXIge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzEzODQ5NjtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMTE3YThiOyB9XFxuICAuYnRuLWluZm86Zm9jdXMsIC5idG4taW5mby5mb2N1cyB7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDU4LCAxNzYsIDE5NSwgMC41KTsgfVxcbiAgLmJ0bi1pbmZvLmRpc2FibGVkLCAuYnRuLWluZm86ZGlzYWJsZWQge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzE3YTJiODtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMTdhMmI4OyB9XFxuICAuYnRuLWluZm86bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlLCAuYnRuLWluZm86bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlLFxcbiAgLnNob3cgPiAuYnRuLWluZm8uZHJvcGRvd24tdG9nZ2xlIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICMxMTdhOGI7XFxuICAgIGJvcmRlci1jb2xvcjogIzEwNzA3ZjsgfVxcbiAgICAuYnRuLWluZm86bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlOmZvY3VzLCAuYnRuLWluZm86bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlOmZvY3VzLFxcbiAgICAuc2hvdyA+IC5idG4taW5mby5kcm9wZG93bi10b2dnbGU6Zm9jdXMge1xcbiAgICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDU4LCAxNzYsIDE5NSwgMC41KTsgfVxcblxcbi5idG4td2FybmluZyB7XFxuICBjb2xvcjogIzIxMjUyOTtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNmZmMxMDc7XFxuICBib3JkZXItY29sb3I6ICNmZmMxMDc7IH1cXG4gIC5idG4td2FybmluZzpob3ZlciB7XFxuICAgIGNvbG9yOiAjMjEyNTI5O1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZTBhODAwO1xcbiAgICBib3JkZXItY29sb3I6ICNkMzllMDA7IH1cXG4gIC5idG4td2FybmluZzpmb2N1cywgLmJ0bi13YXJuaW5nLmZvY3VzIHtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjIyLCAxNzAsIDEyLCAwLjUpOyB9XFxuICAuYnRuLXdhcm5pbmcuZGlzYWJsZWQsIC5idG4td2FybmluZzpkaXNhYmxlZCB7XFxuICAgIGNvbG9yOiAjMjEyNTI5O1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmZjMTA3O1xcbiAgICBib3JkZXItY29sb3I6ICNmZmMxMDc7IH1cXG4gIC5idG4td2FybmluZzpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmUsIC5idG4td2FybmluZzpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmUsXFxuICAuc2hvdyA+IC5idG4td2FybmluZy5kcm9wZG93bi10b2dnbGUge1xcbiAgICBjb2xvcjogIzIxMjUyOTtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2QzOWUwMDtcXG4gICAgYm9yZGVyLWNvbG9yOiAjYzY5NTAwOyB9XFxuICAgIC5idG4td2FybmluZzpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmU6Zm9jdXMsIC5idG4td2FybmluZzpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmU6Zm9jdXMsXFxuICAgIC5zaG93ID4gLmJ0bi13YXJuaW5nLmRyb3Bkb3duLXRvZ2dsZTpmb2N1cyB7XFxuICAgICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjIyLCAxNzAsIDEyLCAwLjUpOyB9XFxuXFxuLmJ0bi1kYW5nZXIge1xcbiAgY29sb3I6ICNmZmY7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjZGMzNTQ1O1xcbiAgYm9yZGVyLWNvbG9yOiAjZGMzNTQ1OyB9XFxuICAuYnRuLWRhbmdlcjpob3ZlciB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjYzgyMzMzO1xcbiAgICBib3JkZXItY29sb3I6ICNiZDIxMzA7IH1cXG4gIC5idG4tZGFuZ2VyOmZvY3VzLCAuYnRuLWRhbmdlci5mb2N1cyB7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDIyNSwgODMsIDk3LCAwLjUpOyB9XFxuICAuYnRuLWRhbmdlci5kaXNhYmxlZCwgLmJ0bi1kYW5nZXI6ZGlzYWJsZWQge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2RjMzU0NTtcXG4gICAgYm9yZGVyLWNvbG9yOiAjZGMzNTQ1OyB9XFxuICAuYnRuLWRhbmdlcjpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmUsIC5idG4tZGFuZ2VyOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZSxcXG4gIC5zaG93ID4gLmJ0bi1kYW5nZXIuZHJvcGRvd24tdG9nZ2xlIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNiZDIxMzA7XFxuICAgIGJvcmRlci1jb2xvcjogI2IyMWYyZDsgfVxcbiAgICAuYnRuLWRhbmdlcjpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmU6Zm9jdXMsIC5idG4tZGFuZ2VyOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZTpmb2N1cyxcXG4gICAgLnNob3cgPiAuYnRuLWRhbmdlci5kcm9wZG93bi10b2dnbGU6Zm9jdXMge1xcbiAgICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDIyNSwgODMsIDk3LCAwLjUpOyB9XFxuXFxuLmJ0bi1saWdodCB7XFxuICBjb2xvcjogIzIxMjUyOTtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNmOGY5ZmE7XFxuICBib3JkZXItY29sb3I6ICNmOGY5ZmE7IH1cXG4gIC5idG4tbGlnaHQ6aG92ZXIge1xcbiAgICBjb2xvcjogIzIxMjUyOTtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2UyZTZlYTtcXG4gICAgYm9yZGVyLWNvbG9yOiAjZGFlMGU1OyB9XFxuICAuYnRuLWxpZ2h0OmZvY3VzLCAuYnRuLWxpZ2h0LmZvY3VzIHtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjE2LCAyMTcsIDIxOSwgMC41KTsgfVxcbiAgLmJ0bi1saWdodC5kaXNhYmxlZCwgLmJ0bi1saWdodDpkaXNhYmxlZCB7XFxuICAgIGNvbG9yOiAjMjEyNTI5O1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZjhmOWZhO1xcbiAgICBib3JkZXItY29sb3I6ICNmOGY5ZmE7IH1cXG4gIC5idG4tbGlnaHQ6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlLCAuYnRuLWxpZ2h0Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZSxcXG4gIC5zaG93ID4gLmJ0bi1saWdodC5kcm9wZG93bi10b2dnbGUge1xcbiAgICBjb2xvcjogIzIxMjUyOTtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2RhZTBlNTtcXG4gICAgYm9yZGVyLWNvbG9yOiAjZDNkOWRmOyB9XFxuICAgIC5idG4tbGlnaHQ6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlOmZvY3VzLCAuYnRuLWxpZ2h0Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZTpmb2N1cyxcXG4gICAgLnNob3cgPiAuYnRuLWxpZ2h0LmRyb3Bkb3duLXRvZ2dsZTpmb2N1cyB7XFxuICAgICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjE2LCAyMTcsIDIxOSwgMC41KTsgfVxcblxcbi5idG4tZGFyayB7XFxuICBjb2xvcjogI2ZmZjtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMzNDNhNDA7XFxuICBib3JkZXItY29sb3I6ICMzNDNhNDA7IH1cXG4gIC5idG4tZGFyazpob3ZlciB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjMjMyNzJiO1xcbiAgICBib3JkZXItY29sb3I6ICMxZDIxMjQ7IH1cXG4gIC5idG4tZGFyazpmb2N1cywgLmJ0bi1kYXJrLmZvY3VzIHtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoODIsIDg4LCA5MywgMC41KTsgfVxcbiAgLmJ0bi1kYXJrLmRpc2FibGVkLCAuYnRuLWRhcms6ZGlzYWJsZWQge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzM0M2E0MDtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMzQzYTQwOyB9XFxuICAuYnRuLWRhcms6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlLCAuYnRuLWRhcms6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlLFxcbiAgLnNob3cgPiAuYnRuLWRhcmsuZHJvcGRvd24tdG9nZ2xlIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICMxZDIxMjQ7XFxuICAgIGJvcmRlci1jb2xvcjogIzE3MWExZDsgfVxcbiAgICAuYnRuLWRhcms6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlOmZvY3VzLCAuYnRuLWRhcms6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlOmZvY3VzLFxcbiAgICAuc2hvdyA+IC5idG4tZGFyay5kcm9wZG93bi10b2dnbGU6Zm9jdXMge1xcbiAgICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDgyLCA4OCwgOTMsIDAuNSk7IH1cXG5cXG4uYnRuLW91dGxpbmUtcHJpbWFyeSB7XFxuICBjb2xvcjogIzAwN2JmZjtcXG4gIGJvcmRlci1jb2xvcjogIzAwN2JmZjsgfVxcbiAgLmJ0bi1vdXRsaW5lLXByaW1hcnk6aG92ZXIge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzAwN2JmZjtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMDA3YmZmOyB9XFxuICAuYnRuLW91dGxpbmUtcHJpbWFyeTpmb2N1cywgLmJ0bi1vdXRsaW5lLXByaW1hcnkuZm9jdXMge1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgwLCAxMjMsIDI1NSwgMC41KTsgfVxcbiAgLmJ0bi1vdXRsaW5lLXByaW1hcnkuZGlzYWJsZWQsIC5idG4tb3V0bGluZS1wcmltYXJ5OmRpc2FibGVkIHtcXG4gICAgY29sb3I6ICMwMDdiZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6IHRyYW5zcGFyZW50OyB9XFxuICAuYnRuLW91dGxpbmUtcHJpbWFyeTpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmUsIC5idG4tb3V0bGluZS1wcmltYXJ5Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZSxcXG4gIC5zaG93ID4gLmJ0bi1vdXRsaW5lLXByaW1hcnkuZHJvcGRvd24tdG9nZ2xlIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICMwMDdiZmY7XFxuICAgIGJvcmRlci1jb2xvcjogIzAwN2JmZjsgfVxcbiAgICAuYnRuLW91dGxpbmUtcHJpbWFyeTpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmU6Zm9jdXMsIC5idG4tb3V0bGluZS1wcmltYXJ5Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZTpmb2N1cyxcXG4gICAgLnNob3cgPiAuYnRuLW91dGxpbmUtcHJpbWFyeS5kcm9wZG93bi10b2dnbGU6Zm9jdXMge1xcbiAgICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDAsIDEyMywgMjU1LCAwLjUpOyB9XFxuXFxuLmJ0bi1vdXRsaW5lLXNlY29uZGFyeSB7XFxuICBjb2xvcjogIzZjNzU3ZDtcXG4gIGJvcmRlci1jb2xvcjogIzZjNzU3ZDsgfVxcbiAgLmJ0bi1vdXRsaW5lLXNlY29uZGFyeTpob3ZlciB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjNmM3NTdkO1xcbiAgICBib3JkZXItY29sb3I6ICM2Yzc1N2Q7IH1cXG4gIC5idG4tb3V0bGluZS1zZWNvbmRhcnk6Zm9jdXMsIC5idG4tb3V0bGluZS1zZWNvbmRhcnkuZm9jdXMge1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgxMDgsIDExNywgMTI1LCAwLjUpOyB9XFxuICAuYnRuLW91dGxpbmUtc2Vjb25kYXJ5LmRpc2FibGVkLCAuYnRuLW91dGxpbmUtc2Vjb25kYXJ5OmRpc2FibGVkIHtcXG4gICAgY29sb3I6ICM2Yzc1N2Q7XFxuICAgIGJhY2tncm91bmQtY29sb3I6IHRyYW5zcGFyZW50OyB9XFxuICAuYnRuLW91dGxpbmUtc2Vjb25kYXJ5Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZSwgLmJ0bi1vdXRsaW5lLXNlY29uZGFyeTpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmUsXFxuICAuc2hvdyA+IC5idG4tb3V0bGluZS1zZWNvbmRhcnkuZHJvcGRvd24tdG9nZ2xlIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICM2Yzc1N2Q7XFxuICAgIGJvcmRlci1jb2xvcjogIzZjNzU3ZDsgfVxcbiAgICAuYnRuLW91dGxpbmUtc2Vjb25kYXJ5Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZTpmb2N1cywgLmJ0bi1vdXRsaW5lLXNlY29uZGFyeTpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmU6Zm9jdXMsXFxuICAgIC5zaG93ID4gLmJ0bi1vdXRsaW5lLXNlY29uZGFyeS5kcm9wZG93bi10b2dnbGU6Zm9jdXMge1xcbiAgICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDEwOCwgMTE3LCAxMjUsIDAuNSk7IH1cXG5cXG4uYnRuLW91dGxpbmUtc3VjY2VzcyB7XFxuICBjb2xvcjogIzI4YTc0NTtcXG4gIGJvcmRlci1jb2xvcjogIzI4YTc0NTsgfVxcbiAgLmJ0bi1vdXRsaW5lLXN1Y2Nlc3M6aG92ZXIge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzI4YTc0NTtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMjhhNzQ1OyB9XFxuICAuYnRuLW91dGxpbmUtc3VjY2Vzczpmb2N1cywgLmJ0bi1vdXRsaW5lLXN1Y2Nlc3MuZm9jdXMge1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSg0MCwgMTY3LCA2OSwgMC41KTsgfVxcbiAgLmJ0bi1vdXRsaW5lLXN1Y2Nlc3MuZGlzYWJsZWQsIC5idG4tb3V0bGluZS1zdWNjZXNzOmRpc2FibGVkIHtcXG4gICAgY29sb3I6ICMyOGE3NDU7XFxuICAgIGJhY2tncm91bmQtY29sb3I6IHRyYW5zcGFyZW50OyB9XFxuICAuYnRuLW91dGxpbmUtc3VjY2Vzczpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmUsIC5idG4tb3V0bGluZS1zdWNjZXNzOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZSxcXG4gIC5zaG93ID4gLmJ0bi1vdXRsaW5lLXN1Y2Nlc3MuZHJvcGRvd24tdG9nZ2xlIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICMyOGE3NDU7XFxuICAgIGJvcmRlci1jb2xvcjogIzI4YTc0NTsgfVxcbiAgICAuYnRuLW91dGxpbmUtc3VjY2Vzczpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmU6Zm9jdXMsIC5idG4tb3V0bGluZS1zdWNjZXNzOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZTpmb2N1cyxcXG4gICAgLnNob3cgPiAuYnRuLW91dGxpbmUtc3VjY2Vzcy5kcm9wZG93bi10b2dnbGU6Zm9jdXMge1xcbiAgICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDQwLCAxNjcsIDY5LCAwLjUpOyB9XFxuXFxuLmJ0bi1vdXRsaW5lLWluZm8ge1xcbiAgY29sb3I6ICMxN2EyYjg7XFxuICBib3JkZXItY29sb3I6ICMxN2EyYjg7IH1cXG4gIC5idG4tb3V0bGluZS1pbmZvOmhvdmVyIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICMxN2EyYjg7XFxuICAgIGJvcmRlci1jb2xvcjogIzE3YTJiODsgfVxcbiAgLmJ0bi1vdXRsaW5lLWluZm86Zm9jdXMsIC5idG4tb3V0bGluZS1pbmZvLmZvY3VzIHtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjMsIDE2MiwgMTg0LCAwLjUpOyB9XFxuICAuYnRuLW91dGxpbmUtaW5mby5kaXNhYmxlZCwgLmJ0bi1vdXRsaW5lLWluZm86ZGlzYWJsZWQge1xcbiAgICBjb2xvcjogIzE3YTJiODtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogdHJhbnNwYXJlbnQ7IH1cXG4gIC5idG4tb3V0bGluZS1pbmZvOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZSwgLmJ0bi1vdXRsaW5lLWluZm86bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlLFxcbiAgLnNob3cgPiAuYnRuLW91dGxpbmUtaW5mby5kcm9wZG93bi10b2dnbGUge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzE3YTJiODtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMTdhMmI4OyB9XFxuICAgIC5idG4tb3V0bGluZS1pbmZvOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZTpmb2N1cywgLmJ0bi1vdXRsaW5lLWluZm86bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlOmZvY3VzLFxcbiAgICAuc2hvdyA+IC5idG4tb3V0bGluZS1pbmZvLmRyb3Bkb3duLXRvZ2dsZTpmb2N1cyB7XFxuICAgICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjMsIDE2MiwgMTg0LCAwLjUpOyB9XFxuXFxuLmJ0bi1vdXRsaW5lLXdhcm5pbmcge1xcbiAgY29sb3I6ICNmZmMxMDc7XFxuICBib3JkZXItY29sb3I6ICNmZmMxMDc7IH1cXG4gIC5idG4tb3V0bGluZS13YXJuaW5nOmhvdmVyIHtcXG4gICAgY29sb3I6ICMyMTI1Mjk7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNmZmMxMDc7XFxuICAgIGJvcmRlci1jb2xvcjogI2ZmYzEwNzsgfVxcbiAgLmJ0bi1vdXRsaW5lLXdhcm5pbmc6Zm9jdXMsIC5idG4tb3V0bGluZS13YXJuaW5nLmZvY3VzIHtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjU1LCAxOTMsIDcsIDAuNSk7IH1cXG4gIC5idG4tb3V0bGluZS13YXJuaW5nLmRpc2FibGVkLCAuYnRuLW91dGxpbmUtd2FybmluZzpkaXNhYmxlZCB7XFxuICAgIGNvbG9yOiAjZmZjMTA3O1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiB0cmFuc3BhcmVudDsgfVxcbiAgLmJ0bi1vdXRsaW5lLXdhcm5pbmc6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlLCAuYnRuLW91dGxpbmUtd2FybmluZzpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmUsXFxuICAuc2hvdyA+IC5idG4tb3V0bGluZS13YXJuaW5nLmRyb3Bkb3duLXRvZ2dsZSB7XFxuICAgIGNvbG9yOiAjMjEyNTI5O1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmZjMTA3O1xcbiAgICBib3JkZXItY29sb3I6ICNmZmMxMDc7IH1cXG4gICAgLmJ0bi1vdXRsaW5lLXdhcm5pbmc6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlOmZvY3VzLCAuYnRuLW91dGxpbmUtd2FybmluZzpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmU6Zm9jdXMsXFxuICAgIC5zaG93ID4gLmJ0bi1vdXRsaW5lLXdhcm5pbmcuZHJvcGRvd24tdG9nZ2xlOmZvY3VzIHtcXG4gICAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyNTUsIDE5MywgNywgMC41KTsgfVxcblxcbi5idG4tb3V0bGluZS1kYW5nZXIge1xcbiAgY29sb3I6ICNkYzM1NDU7XFxuICBib3JkZXItY29sb3I6ICNkYzM1NDU7IH1cXG4gIC5idG4tb3V0bGluZS1kYW5nZXI6aG92ZXIge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2RjMzU0NTtcXG4gICAgYm9yZGVyLWNvbG9yOiAjZGMzNTQ1OyB9XFxuICAuYnRuLW91dGxpbmUtZGFuZ2VyOmZvY3VzLCAuYnRuLW91dGxpbmUtZGFuZ2VyLmZvY3VzIHtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjIwLCA1MywgNjksIDAuNSk7IH1cXG4gIC5idG4tb3V0bGluZS1kYW5nZXIuZGlzYWJsZWQsIC5idG4tb3V0bGluZS1kYW5nZXI6ZGlzYWJsZWQge1xcbiAgICBjb2xvcjogI2RjMzU0NTtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogdHJhbnNwYXJlbnQ7IH1cXG4gIC5idG4tb3V0bGluZS1kYW5nZXI6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlLCAuYnRuLW91dGxpbmUtZGFuZ2VyOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZSxcXG4gIC5zaG93ID4gLmJ0bi1vdXRsaW5lLWRhbmdlci5kcm9wZG93bi10b2dnbGUge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2RjMzU0NTtcXG4gICAgYm9yZGVyLWNvbG9yOiAjZGMzNTQ1OyB9XFxuICAgIC5idG4tb3V0bGluZS1kYW5nZXI6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlOmZvY3VzLCAuYnRuLW91dGxpbmUtZGFuZ2VyOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZTpmb2N1cyxcXG4gICAgLnNob3cgPiAuYnRuLW91dGxpbmUtZGFuZ2VyLmRyb3Bkb3duLXRvZ2dsZTpmb2N1cyB7XFxuICAgICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjIwLCA1MywgNjksIDAuNSk7IH1cXG5cXG4uYnRuLW91dGxpbmUtbGlnaHQge1xcbiAgY29sb3I6ICNmOGY5ZmE7XFxuICBib3JkZXItY29sb3I6ICNmOGY5ZmE7IH1cXG4gIC5idG4tb3V0bGluZS1saWdodDpob3ZlciB7XFxuICAgIGNvbG9yOiAjMjEyNTI5O1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZjhmOWZhO1xcbiAgICBib3JkZXItY29sb3I6ICNmOGY5ZmE7IH1cXG4gIC5idG4tb3V0bGluZS1saWdodDpmb2N1cywgLmJ0bi1vdXRsaW5lLWxpZ2h0LmZvY3VzIHtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjQ4LCAyNDksIDI1MCwgMC41KTsgfVxcbiAgLmJ0bi1vdXRsaW5lLWxpZ2h0LmRpc2FibGVkLCAuYnRuLW91dGxpbmUtbGlnaHQ6ZGlzYWJsZWQge1xcbiAgICBjb2xvcjogI2Y4ZjlmYTtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogdHJhbnNwYXJlbnQ7IH1cXG4gIC5idG4tb3V0bGluZS1saWdodDpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmUsIC5idG4tb3V0bGluZS1saWdodDpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmUsXFxuICAuc2hvdyA+IC5idG4tb3V0bGluZS1saWdodC5kcm9wZG93bi10b2dnbGUge1xcbiAgICBjb2xvcjogIzIxMjUyOTtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2Y4ZjlmYTtcXG4gICAgYm9yZGVyLWNvbG9yOiAjZjhmOWZhOyB9XFxuICAgIC5idG4tb3V0bGluZS1saWdodDpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmU6Zm9jdXMsIC5idG4tb3V0bGluZS1saWdodDpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmU6Zm9jdXMsXFxuICAgIC5zaG93ID4gLmJ0bi1vdXRsaW5lLWxpZ2h0LmRyb3Bkb3duLXRvZ2dsZTpmb2N1cyB7XFxuICAgICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjQ4LCAyNDksIDI1MCwgMC41KTsgfVxcblxcbi5idG4tb3V0bGluZS1kYXJrIHtcXG4gIGNvbG9yOiAjMzQzYTQwO1xcbiAgYm9yZGVyLWNvbG9yOiAjMzQzYTQwOyB9XFxuICAuYnRuLW91dGxpbmUtZGFyazpob3ZlciB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjMzQzYTQwO1xcbiAgICBib3JkZXItY29sb3I6ICMzNDNhNDA7IH1cXG4gIC5idG4tb3V0bGluZS1kYXJrOmZvY3VzLCAuYnRuLW91dGxpbmUtZGFyay5mb2N1cyB7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDUyLCA1OCwgNjQsIDAuNSk7IH1cXG4gIC5idG4tb3V0bGluZS1kYXJrLmRpc2FibGVkLCAuYnRuLW91dGxpbmUtZGFyazpkaXNhYmxlZCB7XFxuICAgIGNvbG9yOiAjMzQzYTQwO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiB0cmFuc3BhcmVudDsgfVxcbiAgLmJ0bi1vdXRsaW5lLWRhcms6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlLCAuYnRuLW91dGxpbmUtZGFyazpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmUsXFxuICAuc2hvdyA+IC5idG4tb3V0bGluZS1kYXJrLmRyb3Bkb3duLXRvZ2dsZSB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjMzQzYTQwO1xcbiAgICBib3JkZXItY29sb3I6ICMzNDNhNDA7IH1cXG4gICAgLmJ0bi1vdXRsaW5lLWRhcms6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlOmZvY3VzLCAuYnRuLW91dGxpbmUtZGFyazpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmU6Zm9jdXMsXFxuICAgIC5zaG93ID4gLmJ0bi1vdXRsaW5lLWRhcmsuZHJvcGRvd24tdG9nZ2xlOmZvY3VzIHtcXG4gICAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSg1MiwgNTgsIDY0LCAwLjUpOyB9XFxuXFxuLmJ0bi1saW5rIHtcXG4gIGZvbnQtd2VpZ2h0OiA0MDA7XFxuICBjb2xvcjogIzAwN2JmZjtcXG4gIHRleHQtZGVjb3JhdGlvbjogbm9uZTsgfVxcbiAgLmJ0bi1saW5rOmhvdmVyIHtcXG4gICAgY29sb3I6ICMwMDU2YjM7XFxuICAgIHRleHQtZGVjb3JhdGlvbjogdW5kZXJsaW5lOyB9XFxuICAuYnRuLWxpbms6Zm9jdXMsIC5idG4tbGluay5mb2N1cyB7XFxuICAgIHRleHQtZGVjb3JhdGlvbjogdW5kZXJsaW5lO1xcbiAgICBib3gtc2hhZG93OiBub25lOyB9XFxuICAuYnRuLWxpbms6ZGlzYWJsZWQsIC5idG4tbGluay5kaXNhYmxlZCB7XFxuICAgIGNvbG9yOiAjNmM3NTdkO1xcbiAgICBwb2ludGVyLWV2ZW50czogbm9uZTsgfVxcblxcbi5idG4tbGcge1xcbiAgcGFkZGluZzogMC41cmVtIDFyZW07XFxuICBmb250LXNpemU6IDEuMjVyZW07XFxuICBsaW5lLWhlaWdodDogMS41O1xcbiAgYm9yZGVyLXJhZGl1czogMC4zcmVtOyB9XFxuXFxuLmJ0bi1zbSB7XFxuICBwYWRkaW5nOiAwLjI1cmVtIDAuNXJlbTtcXG4gIGZvbnQtc2l6ZTogMC44NzVyZW07XFxuICBsaW5lLWhlaWdodDogMS41O1xcbiAgYm9yZGVyLXJhZGl1czogMC4ycmVtOyB9XFxuXFxuLmJ0bi1ibG9jayB7XFxuICBkaXNwbGF5OiBibG9jaztcXG4gIHdpZHRoOiAxMDAlOyB9XFxuICAuYnRuLWJsb2NrICsgLmJ0bi1ibG9jayB7XFxuICAgIG1hcmdpbi10b3A6IDAuNXJlbTsgfVxcblxcbmlucHV0W3R5cGU9XFxcInN1Ym1pdFxcXCJdLmJ0bi1ibG9jayxcXG5pbnB1dFt0eXBlPVxcXCJyZXNldFxcXCJdLmJ0bi1ibG9jayxcXG5pbnB1dFt0eXBlPVxcXCJidXR0b25cXFwiXS5idG4tYmxvY2sge1xcbiAgd2lkdGg6IDEwMCU7IH1cXG5cXG4uZmFkZSB7XFxuICB0cmFuc2l0aW9uOiBvcGFjaXR5IDAuMTVzIGxpbmVhcjsgfVxcbiAgQG1lZGlhIChwcmVmZXJzLXJlZHVjZWQtbW90aW9uOiByZWR1Y2UpIHtcXG4gICAgLmZhZGUge1xcbiAgICAgIHRyYW5zaXRpb246IG5vbmU7IH0gfVxcbiAgLmZhZGU6bm90KC5zaG93KSB7XFxuICAgIG9wYWNpdHk6IDA7IH1cXG5cXG4uY29sbGFwc2U6bm90KC5zaG93KSB7XFxuICBkaXNwbGF5OiBub25lOyB9XFxuXFxuLmNvbGxhcHNpbmcge1xcbiAgcG9zaXRpb246IHJlbGF0aXZlO1xcbiAgaGVpZ2h0OiAwO1xcbiAgb3ZlcmZsb3c6IGhpZGRlbjtcXG4gIHRyYW5zaXRpb246IGhlaWdodCAwLjM1cyBlYXNlOyB9XFxuICBAbWVkaWEgKHByZWZlcnMtcmVkdWNlZC1tb3Rpb246IHJlZHVjZSkge1xcbiAgICAuY29sbGFwc2luZyB7XFxuICAgICAgdHJhbnNpdGlvbjogbm9uZTsgfSB9XFxuXFxuLmZvcm0tY29udHJvbCB7XFxuICBkaXNwbGF5OiBibG9jaztcXG4gIHdpZHRoOiAxMDAlO1xcbiAgaGVpZ2h0OiBjYWxjKDEuNWVtICsgMC43NXJlbSArIDJweCk7XFxuICBwYWRkaW5nOiAwLjM3NXJlbSAwLjc1cmVtO1xcbiAgZm9udC1zaXplOiAxcmVtO1xcbiAgZm9udC13ZWlnaHQ6IDQwMDtcXG4gIGxpbmUtaGVpZ2h0OiAxLjU7XFxuICBjb2xvcjogIzQ5NTA1NztcXG4gIGJhY2tncm91bmQtY29sb3I6ICNmZmY7XFxuICBiYWNrZ3JvdW5kLWNsaXA6IHBhZGRpbmctYm94O1xcbiAgYm9yZGVyOiAxcHggc29saWQgI2NlZDRkYTtcXG4gIGJvcmRlci1yYWRpdXM6IDAuMjVyZW07XFxuICB0cmFuc2l0aW9uOiBib3JkZXItY29sb3IgMC4xNXMgZWFzZS1pbi1vdXQsIGJveC1zaGFkb3cgMC4xNXMgZWFzZS1pbi1vdXQ7IH1cXG4gIEBtZWRpYSAocHJlZmVycy1yZWR1Y2VkLW1vdGlvbjogcmVkdWNlKSB7XFxuICAgIC5mb3JtLWNvbnRyb2wge1xcbiAgICAgIHRyYW5zaXRpb246IG5vbmU7IH0gfVxcbiAgLmZvcm0tY29udHJvbDo6LW1zLWV4cGFuZCB7XFxuICAgIGJhY2tncm91bmQtY29sb3I6IHRyYW5zcGFyZW50O1xcbiAgICBib3JkZXI6IDA7IH1cXG4gIC5mb3JtLWNvbnRyb2w6Zm9jdXMge1xcbiAgICBjb2xvcjogIzQ5NTA1NztcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2ZmZjtcXG4gICAgYm9yZGVyLWNvbG9yOiAjODBiZGZmO1xcbiAgICBvdXRsaW5lOiAwO1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgwLCAxMjMsIDI1NSwgMC4yNSk7IH1cXG4gIC5mb3JtLWNvbnRyb2w6OnBsYWNlaG9sZGVyIHtcXG4gICAgY29sb3I6ICM2Yzc1N2Q7XFxuICAgIG9wYWNpdHk6IDE7IH1cXG4gIC5mb3JtLWNvbnRyb2w6ZGlzYWJsZWQsIC5mb3JtLWNvbnRyb2xbcmVhZG9ubHldIHtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2U5ZWNlZjtcXG4gICAgb3BhY2l0eTogMTsgfVxcblxcbnNlbGVjdC5mb3JtLWNvbnRyb2w6Zm9jdXM6Oi1tcy12YWx1ZSB7XFxuICBjb2xvcjogIzQ5NTA1NztcXG4gIGJhY2tncm91bmQtY29sb3I6ICNmZmY7IH1cXG5cXG4uZm9ybS1jb250cm9sLWZpbGUsXFxuLmZvcm0tY29udHJvbC1yYW5nZSB7XFxuICBkaXNwbGF5OiBibG9jaztcXG4gIHdpZHRoOiAxMDAlOyB9XFxuXFxuLmNvbC1mb3JtLWxhYmVsIHtcXG4gIHBhZGRpbmctdG9wOiBjYWxjKDAuMzc1cmVtICsgMXB4KTtcXG4gIHBhZGRpbmctYm90dG9tOiBjYWxjKDAuMzc1cmVtICsgMXB4KTtcXG4gIG1hcmdpbi1ib3R0b206IDA7XFxuICBmb250LXNpemU6IGluaGVyaXQ7XFxuICBsaW5lLWhlaWdodDogMS41OyB9XFxuXFxuLmNvbC1mb3JtLWxhYmVsLWxnIHtcXG4gIHBhZGRpbmctdG9wOiBjYWxjKDAuNXJlbSArIDFweCk7XFxuICBwYWRkaW5nLWJvdHRvbTogY2FsYygwLjVyZW0gKyAxcHgpO1xcbiAgZm9udC1zaXplOiAxLjI1cmVtO1xcbiAgbGluZS1oZWlnaHQ6IDEuNTsgfVxcblxcbi5jb2wtZm9ybS1sYWJlbC1zbSB7XFxuICBwYWRkaW5nLXRvcDogY2FsYygwLjI1cmVtICsgMXB4KTtcXG4gIHBhZGRpbmctYm90dG9tOiBjYWxjKDAuMjVyZW0gKyAxcHgpO1xcbiAgZm9udC1zaXplOiAwLjg3NXJlbTtcXG4gIGxpbmUtaGVpZ2h0OiAxLjU7IH1cXG5cXG4uZm9ybS1jb250cm9sLXBsYWludGV4dCB7XFxuICBkaXNwbGF5OiBibG9jaztcXG4gIHdpZHRoOiAxMDAlO1xcbiAgcGFkZGluZy10b3A6IDAuMzc1cmVtO1xcbiAgcGFkZGluZy1ib3R0b206IDAuMzc1cmVtO1xcbiAgbWFyZ2luLWJvdHRvbTogMDtcXG4gIGxpbmUtaGVpZ2h0OiAxLjU7XFxuICBjb2xvcjogIzIxMjUyOTtcXG4gIGJhY2tncm91bmQtY29sb3I6IHRyYW5zcGFyZW50O1xcbiAgYm9yZGVyOiBzb2xpZCB0cmFuc3BhcmVudDtcXG4gIGJvcmRlci13aWR0aDogMXB4IDA7IH1cXG4gIC5mb3JtLWNvbnRyb2wtcGxhaW50ZXh0LmZvcm0tY29udHJvbC1zbSwgLmZvcm0tY29udHJvbC1wbGFpbnRleHQuZm9ybS1jb250cm9sLWxnIHtcXG4gICAgcGFkZGluZy1yaWdodDogMDtcXG4gICAgcGFkZGluZy1sZWZ0OiAwOyB9XFxuXFxuLmZvcm0tY29udHJvbC1zbSB7XFxuICBoZWlnaHQ6IGNhbGMoMS41ZW0gKyAwLjVyZW0gKyAycHgpO1xcbiAgcGFkZGluZzogMC4yNXJlbSAwLjVyZW07XFxuICBmb250LXNpemU6IDAuODc1cmVtO1xcbiAgbGluZS1oZWlnaHQ6IDEuNTtcXG4gIGJvcmRlci1yYWRpdXM6IDAuMnJlbTsgfVxcblxcbi5mb3JtLWNvbnRyb2wtbGcge1xcbiAgaGVpZ2h0OiBjYWxjKDEuNWVtICsgMXJlbSArIDJweCk7XFxuICBwYWRkaW5nOiAwLjVyZW0gMXJlbTtcXG4gIGZvbnQtc2l6ZTogMS4yNXJlbTtcXG4gIGxpbmUtaGVpZ2h0OiAxLjU7XFxuICBib3JkZXItcmFkaXVzOiAwLjNyZW07IH1cXG5cXG5zZWxlY3QuZm9ybS1jb250cm9sW3NpemVdLCBzZWxlY3QuZm9ybS1jb250cm9sW211bHRpcGxlXSB7XFxuICBoZWlnaHQ6IGF1dG87IH1cXG5cXG50ZXh0YXJlYS5mb3JtLWNvbnRyb2wge1xcbiAgaGVpZ2h0OiBhdXRvOyB9XFxuXFxuLmZvcm0tZ3JvdXAge1xcbiAgbWFyZ2luLWJvdHRvbTogMXJlbTsgfVxcblxcbi5mb3JtLXRleHQge1xcbiAgZGlzcGxheTogYmxvY2s7XFxuICBtYXJnaW4tdG9wOiAwLjI1cmVtOyB9XFxuXFxuLmZvcm0tcm93IHtcXG4gIGRpc3BsYXk6IGZsZXg7XFxuICBmbGV4LXdyYXA6IHdyYXA7XFxuICBtYXJnaW4tcmlnaHQ6IC01cHg7XFxuICBtYXJnaW4tbGVmdDogLTVweDsgfVxcbiAgLmZvcm0tcm93ID4gLmNvbCxcXG4gIC5mb3JtLXJvdyA+IFtjbGFzcyo9XFxcImNvbC1cXFwiXSB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDVweDtcXG4gICAgcGFkZGluZy1sZWZ0OiA1cHg7IH1cXG5cXG4uZm9ybS1jaGVjayB7XFxuICBwb3NpdGlvbjogcmVsYXRpdmU7XFxuICBkaXNwbGF5OiBibG9jaztcXG4gIHBhZGRpbmctbGVmdDogMS4yNXJlbTsgfVxcblxcbi5mb3JtLWNoZWNrLWlucHV0IHtcXG4gIHBvc2l0aW9uOiBhYnNvbHV0ZTtcXG4gIG1hcmdpbi10b3A6IDAuM3JlbTtcXG4gIG1hcmdpbi1sZWZ0OiAtMS4yNXJlbTsgfVxcbiAgLmZvcm0tY2hlY2staW5wdXQ6ZGlzYWJsZWQgfiAuZm9ybS1jaGVjay1sYWJlbCB7XFxuICAgIGNvbG9yOiAjNmM3NTdkOyB9XFxuXFxuLmZvcm0tY2hlY2stbGFiZWwge1xcbiAgbWFyZ2luLWJvdHRvbTogMDsgfVxcblxcbi5mb3JtLWNoZWNrLWlubGluZSB7XFxuICBkaXNwbGF5OiBpbmxpbmUtZmxleDtcXG4gIGFsaWduLWl0ZW1zOiBjZW50ZXI7XFxuICBwYWRkaW5nLWxlZnQ6IDA7XFxuICBtYXJnaW4tcmlnaHQ6IDAuNzVyZW07IH1cXG4gIC5mb3JtLWNoZWNrLWlubGluZSAuZm9ybS1jaGVjay1pbnB1dCB7XFxuICAgIHBvc2l0aW9uOiBzdGF0aWM7XFxuICAgIG1hcmdpbi10b3A6IDA7XFxuICAgIG1hcmdpbi1yaWdodDogMC4zMTI1cmVtO1xcbiAgICBtYXJnaW4tbGVmdDogMDsgfVxcblxcbi52YWxpZC1mZWVkYmFjayB7XFxuICBkaXNwbGF5OiBub25lO1xcbiAgd2lkdGg6IDEwMCU7XFxuICBtYXJnaW4tdG9wOiAwLjI1cmVtO1xcbiAgZm9udC1zaXplOiA4MCU7XFxuICBjb2xvcjogIzI4YTc0NTsgfVxcblxcbi52YWxpZC10b29sdGlwIHtcXG4gIHBvc2l0aW9uOiBhYnNvbHV0ZTtcXG4gIHRvcDogMTAwJTtcXG4gIHotaW5kZXg6IDU7XFxuICBkaXNwbGF5OiBub25lO1xcbiAgbWF4LXdpZHRoOiAxMDAlO1xcbiAgcGFkZGluZzogMC4yNXJlbSAwLjVyZW07XFxuICBtYXJnaW4tdG9wOiAuMXJlbTtcXG4gIGZvbnQtc2l6ZTogMC44NzVyZW07XFxuICBsaW5lLWhlaWdodDogMS41O1xcbiAgY29sb3I6ICNmZmY7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiByZ2JhKDQwLCAxNjcsIDY5LCAwLjkpO1xcbiAgYm9yZGVyLXJhZGl1czogMC4yNXJlbTsgfVxcblxcbi53YXMtdmFsaWRhdGVkIC5mb3JtLWNvbnRyb2w6dmFsaWQsIC5mb3JtLWNvbnRyb2wuaXMtdmFsaWQge1xcbiAgYm9yZGVyLWNvbG9yOiAjMjhhNzQ1O1xcbiAgcGFkZGluZy1yaWdodDogY2FsYygxLjVlbSArIDAuNzVyZW0pO1xcbiAgYmFja2dyb3VuZC1pbWFnZTogdXJsKFxcXCJkYXRhOmltYWdlL3N2Zyt4bWwsJTNjc3ZnIHhtbG5zPSdodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2Zycgdmlld0JveD0nMCAwIDggOCclM2UlM2NwYXRoIGZpbGw9JyUyMzI4YTc0NScgZD0nTTIuMyA2LjczTC42IDQuNTNjLS40LTEuMDQuNDYtMS40IDEuMS0uOGwxLjEgMS40IDMuNC0zLjhjLjYtLjYzIDEuNi0uMjcgMS4yLjdsLTQgNC42Yy0uNDMuNS0uOC40LTEuMS4xeicvJTNlJTNjL3N2ZyUzZVxcXCIpO1xcbiAgYmFja2dyb3VuZC1yZXBlYXQ6IG5vLXJlcGVhdDtcXG4gIGJhY2tncm91bmQtcG9zaXRpb246IGNlbnRlciByaWdodCBjYWxjKDAuMzc1ZW0gKyAwLjE4NzVyZW0pO1xcbiAgYmFja2dyb3VuZC1zaXplOiBjYWxjKDAuNzVlbSArIDAuMzc1cmVtKSBjYWxjKDAuNzVlbSArIDAuMzc1cmVtKTsgfVxcbiAgLndhcy12YWxpZGF0ZWQgLmZvcm0tY29udHJvbDp2YWxpZDpmb2N1cywgLmZvcm0tY29udHJvbC5pcy12YWxpZDpmb2N1cyB7XFxuICAgIGJvcmRlci1jb2xvcjogIzI4YTc0NTtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoNDAsIDE2NywgNjksIDAuMjUpOyB9XFxuICAud2FzLXZhbGlkYXRlZCAuZm9ybS1jb250cm9sOnZhbGlkIH4gLnZhbGlkLWZlZWRiYWNrLFxcbiAgLndhcy12YWxpZGF0ZWQgLmZvcm0tY29udHJvbDp2YWxpZCB+IC52YWxpZC10b29sdGlwLCAuZm9ybS1jb250cm9sLmlzLXZhbGlkIH4gLnZhbGlkLWZlZWRiYWNrLFxcbiAgLmZvcm0tY29udHJvbC5pcy12YWxpZCB+IC52YWxpZC10b29sdGlwIHtcXG4gICAgZGlzcGxheTogYmxvY2s7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCB0ZXh0YXJlYS5mb3JtLWNvbnRyb2w6dmFsaWQsIHRleHRhcmVhLmZvcm0tY29udHJvbC5pcy12YWxpZCB7XFxuICBwYWRkaW5nLXJpZ2h0OiBjYWxjKDEuNWVtICsgMC43NXJlbSk7XFxuICBiYWNrZ3JvdW5kLXBvc2l0aW9uOiB0b3AgY2FsYygwLjM3NWVtICsgMC4xODc1cmVtKSByaWdodCBjYWxjKDAuMzc1ZW0gKyAwLjE4NzVyZW0pOyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1zZWxlY3Q6dmFsaWQsIC5jdXN0b20tc2VsZWN0LmlzLXZhbGlkIHtcXG4gIGJvcmRlci1jb2xvcjogIzI4YTc0NTtcXG4gIHBhZGRpbmctcmlnaHQ6IGNhbGMoKDFlbSArIDAuNzVyZW0pICogMyAvIDQgKyAxLjc1cmVtKTtcXG4gIGJhY2tncm91bmQ6IHVybChcXFwiZGF0YTppbWFnZS9zdmcreG1sLCUzY3N2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHZpZXdCb3g9JzAgMCA0IDUnJTNlJTNjcGF0aCBmaWxsPSclMjMzNDNhNDAnIGQ9J00yIDBMMCAyaDR6bTAgNUwwIDNoNHonLyUzZSUzYy9zdmclM2VcXFwiKSBuby1yZXBlYXQgcmlnaHQgMC43NXJlbSBjZW50ZXIvOHB4IDEwcHgsIHVybChcXFwiZGF0YTppbWFnZS9zdmcreG1sLCUzY3N2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHZpZXdCb3g9JzAgMCA4IDgnJTNlJTNjcGF0aCBmaWxsPSclMjMyOGE3NDUnIGQ9J00yLjMgNi43M0wuNiA0LjUzYy0uNC0xLjA0LjQ2LTEuNCAxLjEtLjhsMS4xIDEuNCAzLjQtMy44Yy42LS42MyAxLjYtLjI3IDEuMi43bC00IDQuNmMtLjQzLjUtLjguNC0xLjEuMXonLyUzZSUzYy9zdmclM2VcXFwiKSAjZmZmIG5vLXJlcGVhdCBjZW50ZXIgcmlnaHQgMS43NXJlbS9jYWxjKDAuNzVlbSArIDAuMzc1cmVtKSBjYWxjKDAuNzVlbSArIDAuMzc1cmVtKTsgfVxcbiAgLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1zZWxlY3Q6dmFsaWQ6Zm9jdXMsIC5jdXN0b20tc2VsZWN0LmlzLXZhbGlkOmZvY3VzIHtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMjhhNzQ1O1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSg0MCwgMTY3LCA2OSwgMC4yNSk7IH1cXG4gIC53YXMtdmFsaWRhdGVkIC5jdXN0b20tc2VsZWN0OnZhbGlkIH4gLnZhbGlkLWZlZWRiYWNrLFxcbiAgLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1zZWxlY3Q6dmFsaWQgfiAudmFsaWQtdG9vbHRpcCwgLmN1c3RvbS1zZWxlY3QuaXMtdmFsaWQgfiAudmFsaWQtZmVlZGJhY2ssXFxuICAuY3VzdG9tLXNlbGVjdC5pcy12YWxpZCB+IC52YWxpZC10b29sdGlwIHtcXG4gICAgZGlzcGxheTogYmxvY2s7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuZm9ybS1jb250cm9sLWZpbGU6dmFsaWQgfiAudmFsaWQtZmVlZGJhY2ssXFxuLndhcy12YWxpZGF0ZWQgLmZvcm0tY29udHJvbC1maWxlOnZhbGlkIH4gLnZhbGlkLXRvb2x0aXAsIC5mb3JtLWNvbnRyb2wtZmlsZS5pcy12YWxpZCB+IC52YWxpZC1mZWVkYmFjayxcXG4uZm9ybS1jb250cm9sLWZpbGUuaXMtdmFsaWQgfiAudmFsaWQtdG9vbHRpcCB7XFxuICBkaXNwbGF5OiBibG9jazsgfVxcblxcbi53YXMtdmFsaWRhdGVkIC5mb3JtLWNoZWNrLWlucHV0OnZhbGlkIH4gLmZvcm0tY2hlY2stbGFiZWwsIC5mb3JtLWNoZWNrLWlucHV0LmlzLXZhbGlkIH4gLmZvcm0tY2hlY2stbGFiZWwge1xcbiAgY29sb3I6ICMyOGE3NDU7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuZm9ybS1jaGVjay1pbnB1dDp2YWxpZCB+IC52YWxpZC1mZWVkYmFjayxcXG4ud2FzLXZhbGlkYXRlZCAuZm9ybS1jaGVjay1pbnB1dDp2YWxpZCB+IC52YWxpZC10b29sdGlwLCAuZm9ybS1jaGVjay1pbnB1dC5pcy12YWxpZCB+IC52YWxpZC1mZWVkYmFjayxcXG4uZm9ybS1jaGVjay1pbnB1dC5pcy12YWxpZCB+IC52YWxpZC10b29sdGlwIHtcXG4gIGRpc3BsYXk6IGJsb2NrOyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1jb250cm9sLWlucHV0OnZhbGlkIH4gLmN1c3RvbS1jb250cm9sLWxhYmVsLCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQuaXMtdmFsaWQgfiAuY3VzdG9tLWNvbnRyb2wtbGFiZWwge1xcbiAgY29sb3I6ICMyOGE3NDU7IH1cXG4gIC53YXMtdmFsaWRhdGVkIC5jdXN0b20tY29udHJvbC1pbnB1dDp2YWxpZCB+IC5jdXN0b20tY29udHJvbC1sYWJlbDo6YmVmb3JlLCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQuaXMtdmFsaWQgfiAuY3VzdG9tLWNvbnRyb2wtbGFiZWw6OmJlZm9yZSB7XFxuICAgIGJvcmRlci1jb2xvcjogIzI4YTc0NTsgfVxcblxcbi53YXMtdmFsaWRhdGVkIC5jdXN0b20tY29udHJvbC1pbnB1dDp2YWxpZCB+IC52YWxpZC1mZWVkYmFjayxcXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQ6dmFsaWQgfiAudmFsaWQtdG9vbHRpcCwgLmN1c3RvbS1jb250cm9sLWlucHV0LmlzLXZhbGlkIH4gLnZhbGlkLWZlZWRiYWNrLFxcbi5jdXN0b20tY29udHJvbC1pbnB1dC5pcy12YWxpZCB+IC52YWxpZC10b29sdGlwIHtcXG4gIGRpc3BsYXk6IGJsb2NrOyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1jb250cm9sLWlucHV0OnZhbGlkOmNoZWNrZWQgfiAuY3VzdG9tLWNvbnRyb2wtbGFiZWw6OmJlZm9yZSwgLmN1c3RvbS1jb250cm9sLWlucHV0LmlzLXZhbGlkOmNoZWNrZWQgfiAuY3VzdG9tLWNvbnRyb2wtbGFiZWw6OmJlZm9yZSB7XFxuICBib3JkZXItY29sb3I6ICMzNGNlNTc7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjMzRjZTU3OyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1jb250cm9sLWlucHV0OnZhbGlkOmZvY3VzIH4gLmN1c3RvbS1jb250cm9sLWxhYmVsOjpiZWZvcmUsIC5jdXN0b20tY29udHJvbC1pbnB1dC5pcy12YWxpZDpmb2N1cyB+IC5jdXN0b20tY29udHJvbC1sYWJlbDo6YmVmb3JlIHtcXG4gIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDQwLCAxNjcsIDY5LCAwLjI1KTsgfVxcblxcbi53YXMtdmFsaWRhdGVkIC5jdXN0b20tY29udHJvbC1pbnB1dDp2YWxpZDpmb2N1czpub3QoOmNoZWNrZWQpIH4gLmN1c3RvbS1jb250cm9sLWxhYmVsOjpiZWZvcmUsIC5jdXN0b20tY29udHJvbC1pbnB1dC5pcy12YWxpZDpmb2N1czpub3QoOmNoZWNrZWQpIH4gLmN1c3RvbS1jb250cm9sLWxhYmVsOjpiZWZvcmUge1xcbiAgYm9yZGVyLWNvbG9yOiAjMjhhNzQ1OyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1maWxlLWlucHV0OnZhbGlkIH4gLmN1c3RvbS1maWxlLWxhYmVsLCAuY3VzdG9tLWZpbGUtaW5wdXQuaXMtdmFsaWQgfiAuY3VzdG9tLWZpbGUtbGFiZWwge1xcbiAgYm9yZGVyLWNvbG9yOiAjMjhhNzQ1OyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1maWxlLWlucHV0OnZhbGlkIH4gLnZhbGlkLWZlZWRiYWNrLFxcbi53YXMtdmFsaWRhdGVkIC5jdXN0b20tZmlsZS1pbnB1dDp2YWxpZCB+IC52YWxpZC10b29sdGlwLCAuY3VzdG9tLWZpbGUtaW5wdXQuaXMtdmFsaWQgfiAudmFsaWQtZmVlZGJhY2ssXFxuLmN1c3RvbS1maWxlLWlucHV0LmlzLXZhbGlkIH4gLnZhbGlkLXRvb2x0aXAge1xcbiAgZGlzcGxheTogYmxvY2s7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLWZpbGUtaW5wdXQ6dmFsaWQ6Zm9jdXMgfiAuY3VzdG9tLWZpbGUtbGFiZWwsIC5jdXN0b20tZmlsZS1pbnB1dC5pcy12YWxpZDpmb2N1cyB+IC5jdXN0b20tZmlsZS1sYWJlbCB7XFxuICBib3JkZXItY29sb3I6ICMyOGE3NDU7XFxuICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSg0MCwgMTY3LCA2OSwgMC4yNSk7IH1cXG5cXG4uaW52YWxpZC1mZWVkYmFjayB7XFxuICBkaXNwbGF5OiBub25lO1xcbiAgd2lkdGg6IDEwMCU7XFxuICBtYXJnaW4tdG9wOiAwLjI1cmVtO1xcbiAgZm9udC1zaXplOiA4MCU7XFxuICBjb2xvcjogI2RjMzU0NTsgfVxcblxcbi5pbnZhbGlkLXRvb2x0aXAge1xcbiAgcG9zaXRpb246IGFic29sdXRlO1xcbiAgdG9wOiAxMDAlO1xcbiAgei1pbmRleDogNTtcXG4gIGRpc3BsYXk6IG5vbmU7XFxuICBtYXgtd2lkdGg6IDEwMCU7XFxuICBwYWRkaW5nOiAwLjI1cmVtIDAuNXJlbTtcXG4gIG1hcmdpbi10b3A6IC4xcmVtO1xcbiAgZm9udC1zaXplOiAwLjg3NXJlbTtcXG4gIGxpbmUtaGVpZ2h0OiAxLjU7XFxuICBjb2xvcjogI2ZmZjtcXG4gIGJhY2tncm91bmQtY29sb3I6IHJnYmEoMjIwLCA1MywgNjksIDAuOSk7XFxuICBib3JkZXItcmFkaXVzOiAwLjI1cmVtOyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmZvcm0tY29udHJvbDppbnZhbGlkLCAuZm9ybS1jb250cm9sLmlzLWludmFsaWQge1xcbiAgYm9yZGVyLWNvbG9yOiAjZGMzNTQ1O1xcbiAgcGFkZGluZy1yaWdodDogY2FsYygxLjVlbSArIDAuNzVyZW0pO1xcbiAgYmFja2dyb3VuZC1pbWFnZTogdXJsKFxcXCJkYXRhOmltYWdlL3N2Zyt4bWwsJTNjc3ZnIHhtbG5zPSdodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZycgZmlsbD0nJTIzZGMzNTQ1JyB2aWV3Qm94PSctMiAtMiA3IDcnJTNlJTNjcGF0aCBzdHJva2U9JyUyM2RjMzU0NScgZD0nTTAgMGwzIDNtMC0zTDAgMycvJTNlJTNjY2lyY2xlIHI9Jy41Jy8lM2UlM2NjaXJjbGUgY3g9JzMnIHI9Jy41Jy8lM2UlM2NjaXJjbGUgY3k9JzMnIHI9Jy41Jy8lM2UlM2NjaXJjbGUgY3g9JzMnIGN5PSczJyByPScuNScvJTNlJTNjL3N2ZyUzRVxcXCIpO1xcbiAgYmFja2dyb3VuZC1yZXBlYXQ6IG5vLXJlcGVhdDtcXG4gIGJhY2tncm91bmQtcG9zaXRpb246IGNlbnRlciByaWdodCBjYWxjKDAuMzc1ZW0gKyAwLjE4NzVyZW0pO1xcbiAgYmFja2dyb3VuZC1zaXplOiBjYWxjKDAuNzVlbSArIDAuMzc1cmVtKSBjYWxjKDAuNzVlbSArIDAuMzc1cmVtKTsgfVxcbiAgLndhcy12YWxpZGF0ZWQgLmZvcm0tY29udHJvbDppbnZhbGlkOmZvY3VzLCAuZm9ybS1jb250cm9sLmlzLWludmFsaWQ6Zm9jdXMge1xcbiAgICBib3JkZXItY29sb3I6ICNkYzM1NDU7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDIyMCwgNTMsIDY5LCAwLjI1KTsgfVxcbiAgLndhcy12YWxpZGF0ZWQgLmZvcm0tY29udHJvbDppbnZhbGlkIH4gLmludmFsaWQtZmVlZGJhY2ssXFxuICAud2FzLXZhbGlkYXRlZCAuZm9ybS1jb250cm9sOmludmFsaWQgfiAuaW52YWxpZC10b29sdGlwLCAuZm9ybS1jb250cm9sLmlzLWludmFsaWQgfiAuaW52YWxpZC1mZWVkYmFjayxcXG4gIC5mb3JtLWNvbnRyb2wuaXMtaW52YWxpZCB+IC5pbnZhbGlkLXRvb2x0aXAge1xcbiAgICBkaXNwbGF5OiBibG9jazsgfVxcblxcbi53YXMtdmFsaWRhdGVkIHRleHRhcmVhLmZvcm0tY29udHJvbDppbnZhbGlkLCB0ZXh0YXJlYS5mb3JtLWNvbnRyb2wuaXMtaW52YWxpZCB7XFxuICBwYWRkaW5nLXJpZ2h0OiBjYWxjKDEuNWVtICsgMC43NXJlbSk7XFxuICBiYWNrZ3JvdW5kLXBvc2l0aW9uOiB0b3AgY2FsYygwLjM3NWVtICsgMC4xODc1cmVtKSByaWdodCBjYWxjKDAuMzc1ZW0gKyAwLjE4NzVyZW0pOyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1zZWxlY3Q6aW52YWxpZCwgLmN1c3RvbS1zZWxlY3QuaXMtaW52YWxpZCB7XFxuICBib3JkZXItY29sb3I6ICNkYzM1NDU7XFxuICBwYWRkaW5nLXJpZ2h0OiBjYWxjKCgxZW0gKyAwLjc1cmVtKSAqIDMgLyA0ICsgMS43NXJlbSk7XFxuICBiYWNrZ3JvdW5kOiB1cmwoXFxcImRhdGE6aW1hZ2Uvc3ZnK3htbCwlM2NzdmcgeG1sbnM9J2h0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnJyB2aWV3Qm94PScwIDAgNCA1JyUzZSUzY3BhdGggZmlsbD0nJTIzMzQzYTQwJyBkPSdNMiAwTDAgMmg0em0wIDVMMCAzaDR6Jy8lM2UlM2Mvc3ZnJTNlXFxcIikgbm8tcmVwZWF0IHJpZ2h0IDAuNzVyZW0gY2VudGVyLzhweCAxMHB4LCB1cmwoXFxcImRhdGE6aW1hZ2Uvc3ZnK3htbCwlM2NzdmcgeG1sbnM9J2h0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnJyBmaWxsPSclMjNkYzM1NDUnIHZpZXdCb3g9Jy0yIC0yIDcgNyclM2UlM2NwYXRoIHN0cm9rZT0nJTIzZGMzNTQ1JyBkPSdNMCAwbDMgM20wLTNMMCAzJy8lM2UlM2NjaXJjbGUgcj0nLjUnLyUzZSUzY2NpcmNsZSBjeD0nMycgcj0nLjUnLyUzZSUzY2NpcmNsZSBjeT0nMycgcj0nLjUnLyUzZSUzY2NpcmNsZSBjeD0nMycgY3k9JzMnIHI9Jy41Jy8lM2UlM2Mvc3ZnJTNFXFxcIikgI2ZmZiBuby1yZXBlYXQgY2VudGVyIHJpZ2h0IDEuNzVyZW0vY2FsYygwLjc1ZW0gKyAwLjM3NXJlbSkgY2FsYygwLjc1ZW0gKyAwLjM3NXJlbSk7IH1cXG4gIC53YXMtdmFsaWRhdGVkIC5jdXN0b20tc2VsZWN0OmludmFsaWQ6Zm9jdXMsIC5jdXN0b20tc2VsZWN0LmlzLWludmFsaWQ6Zm9jdXMge1xcbiAgICBib3JkZXItY29sb3I6ICNkYzM1NDU7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDIyMCwgNTMsIDY5LCAwLjI1KTsgfVxcbiAgLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1zZWxlY3Q6aW52YWxpZCB+IC5pbnZhbGlkLWZlZWRiYWNrLFxcbiAgLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1zZWxlY3Q6aW52YWxpZCB+IC5pbnZhbGlkLXRvb2x0aXAsIC5jdXN0b20tc2VsZWN0LmlzLWludmFsaWQgfiAuaW52YWxpZC1mZWVkYmFjayxcXG4gIC5jdXN0b20tc2VsZWN0LmlzLWludmFsaWQgfiAuaW52YWxpZC10b29sdGlwIHtcXG4gICAgZGlzcGxheTogYmxvY2s7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuZm9ybS1jb250cm9sLWZpbGU6aW52YWxpZCB+IC5pbnZhbGlkLWZlZWRiYWNrLFxcbi53YXMtdmFsaWRhdGVkIC5mb3JtLWNvbnRyb2wtZmlsZTppbnZhbGlkIH4gLmludmFsaWQtdG9vbHRpcCwgLmZvcm0tY29udHJvbC1maWxlLmlzLWludmFsaWQgfiAuaW52YWxpZC1mZWVkYmFjayxcXG4uZm9ybS1jb250cm9sLWZpbGUuaXMtaW52YWxpZCB+IC5pbnZhbGlkLXRvb2x0aXAge1xcbiAgZGlzcGxheTogYmxvY2s7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuZm9ybS1jaGVjay1pbnB1dDppbnZhbGlkIH4gLmZvcm0tY2hlY2stbGFiZWwsIC5mb3JtLWNoZWNrLWlucHV0LmlzLWludmFsaWQgfiAuZm9ybS1jaGVjay1sYWJlbCB7XFxuICBjb2xvcjogI2RjMzU0NTsgfVxcblxcbi53YXMtdmFsaWRhdGVkIC5mb3JtLWNoZWNrLWlucHV0OmludmFsaWQgfiAuaW52YWxpZC1mZWVkYmFjayxcXG4ud2FzLXZhbGlkYXRlZCAuZm9ybS1jaGVjay1pbnB1dDppbnZhbGlkIH4gLmludmFsaWQtdG9vbHRpcCwgLmZvcm0tY2hlY2staW5wdXQuaXMtaW52YWxpZCB+IC5pbnZhbGlkLWZlZWRiYWNrLFxcbi5mb3JtLWNoZWNrLWlucHV0LmlzLWludmFsaWQgfiAuaW52YWxpZC10b29sdGlwIHtcXG4gIGRpc3BsYXk6IGJsb2NrOyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1jb250cm9sLWlucHV0OmludmFsaWQgfiAuY3VzdG9tLWNvbnRyb2wtbGFiZWwsIC5jdXN0b20tY29udHJvbC1pbnB1dC5pcy1pbnZhbGlkIH4gLmN1c3RvbS1jb250cm9sLWxhYmVsIHtcXG4gIGNvbG9yOiAjZGMzNTQ1OyB9XFxuICAud2FzLXZhbGlkYXRlZCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQ6aW52YWxpZCB+IC5jdXN0b20tY29udHJvbC1sYWJlbDo6YmVmb3JlLCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQuaXMtaW52YWxpZCB+IC5jdXN0b20tY29udHJvbC1sYWJlbDo6YmVmb3JlIHtcXG4gICAgYm9yZGVyLWNvbG9yOiAjZGMzNTQ1OyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1jb250cm9sLWlucHV0OmludmFsaWQgfiAuaW52YWxpZC1mZWVkYmFjayxcXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQ6aW52YWxpZCB+IC5pbnZhbGlkLXRvb2x0aXAsIC5jdXN0b20tY29udHJvbC1pbnB1dC5pcy1pbnZhbGlkIH4gLmludmFsaWQtZmVlZGJhY2ssXFxuLmN1c3RvbS1jb250cm9sLWlucHV0LmlzLWludmFsaWQgfiAuaW52YWxpZC10b29sdGlwIHtcXG4gIGRpc3BsYXk6IGJsb2NrOyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1jb250cm9sLWlucHV0OmludmFsaWQ6Y2hlY2tlZCB+IC5jdXN0b20tY29udHJvbC1sYWJlbDo6YmVmb3JlLCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQuaXMtaW52YWxpZDpjaGVja2VkIH4gLmN1c3RvbS1jb250cm9sLWxhYmVsOjpiZWZvcmUge1xcbiAgYm9yZGVyLWNvbG9yOiAjZTQ2MDZkO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2U0NjA2ZDsgfVxcblxcbi53YXMtdmFsaWRhdGVkIC5jdXN0b20tY29udHJvbC1pbnB1dDppbnZhbGlkOmZvY3VzIH4gLmN1c3RvbS1jb250cm9sLWxhYmVsOjpiZWZvcmUsIC5jdXN0b20tY29udHJvbC1pbnB1dC5pcy1pbnZhbGlkOmZvY3VzIH4gLmN1c3RvbS1jb250cm9sLWxhYmVsOjpiZWZvcmUge1xcbiAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjIwLCA1MywgNjksIDAuMjUpOyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1jb250cm9sLWlucHV0OmludmFsaWQ6Zm9jdXM6bm90KDpjaGVja2VkKSB+IC5jdXN0b20tY29udHJvbC1sYWJlbDo6YmVmb3JlLCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQuaXMtaW52YWxpZDpmb2N1czpub3QoOmNoZWNrZWQpIH4gLmN1c3RvbS1jb250cm9sLWxhYmVsOjpiZWZvcmUge1xcbiAgYm9yZGVyLWNvbG9yOiAjZGMzNTQ1OyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1maWxlLWlucHV0OmludmFsaWQgfiAuY3VzdG9tLWZpbGUtbGFiZWwsIC5jdXN0b20tZmlsZS1pbnB1dC5pcy1pbnZhbGlkIH4gLmN1c3RvbS1maWxlLWxhYmVsIHtcXG4gIGJvcmRlci1jb2xvcjogI2RjMzU0NTsgfVxcblxcbi53YXMtdmFsaWRhdGVkIC5jdXN0b20tZmlsZS1pbnB1dDppbnZhbGlkIH4gLmludmFsaWQtZmVlZGJhY2ssXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1maWxlLWlucHV0OmludmFsaWQgfiAuaW52YWxpZC10b29sdGlwLCAuY3VzdG9tLWZpbGUtaW5wdXQuaXMtaW52YWxpZCB+IC5pbnZhbGlkLWZlZWRiYWNrLFxcbi5jdXN0b20tZmlsZS1pbnB1dC5pcy1pbnZhbGlkIH4gLmludmFsaWQtdG9vbHRpcCB7XFxuICBkaXNwbGF5OiBibG9jazsgfVxcblxcbi53YXMtdmFsaWRhdGVkIC5jdXN0b20tZmlsZS1pbnB1dDppbnZhbGlkOmZvY3VzIH4gLmN1c3RvbS1maWxlLWxhYmVsLCAuY3VzdG9tLWZpbGUtaW5wdXQuaXMtaW52YWxpZDpmb2N1cyB+IC5jdXN0b20tZmlsZS1sYWJlbCB7XFxuICBib3JkZXItY29sb3I6ICNkYzM1NDU7XFxuICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyMjAsIDUzLCA2OSwgMC4yNSk7IH1cXG5cXG4uZm9ybS1pbmxpbmUge1xcbiAgZGlzcGxheTogZmxleDtcXG4gIGZsZXgtZmxvdzogcm93IHdyYXA7XFxuICBhbGlnbi1pdGVtczogY2VudGVyOyB9XFxuICAuZm9ybS1pbmxpbmUgLmZvcm0tY2hlY2sge1xcbiAgICB3aWR0aDogMTAwJTsgfVxcbiAgQG1lZGlhIChtaW4td2lkdGg6IDU3NnB4KSB7XFxuICAgIC5mb3JtLWlubGluZSBsYWJlbCB7XFxuICAgICAgZGlzcGxheTogZmxleDtcXG4gICAgICBhbGlnbi1pdGVtczogY2VudGVyO1xcbiAgICAgIGp1c3RpZnktY29udGVudDogY2VudGVyO1xcbiAgICAgIG1hcmdpbi1ib3R0b206IDA7IH1cXG4gICAgLmZvcm0taW5saW5lIC5mb3JtLWdyb3VwIHtcXG4gICAgICBkaXNwbGF5OiBmbGV4O1xcbiAgICAgIGZsZXg6IDAgMCBhdXRvO1xcbiAgICAgIGZsZXgtZmxvdzogcm93IHdyYXA7XFxuICAgICAgYWxpZ24taXRlbXM6IGNlbnRlcjtcXG4gICAgICBtYXJnaW4tYm90dG9tOiAwOyB9XFxuICAgIC5mb3JtLWlubGluZSAuZm9ybS1jb250cm9sIHtcXG4gICAgICBkaXNwbGF5OiBpbmxpbmUtYmxvY2s7XFxuICAgICAgd2lkdGg6IGF1dG87XFxuICAgICAgdmVydGljYWwtYWxpZ246IG1pZGRsZTsgfVxcbiAgICAuZm9ybS1pbmxpbmUgLmZvcm0tY29udHJvbC1wbGFpbnRleHQge1xcbiAgICAgIGRpc3BsYXk6IGlubGluZS1ibG9jazsgfVxcbiAgICAuZm9ybS1pbmxpbmUgLmlucHV0LWdyb3VwLFxcbiAgICAuZm9ybS1pbmxpbmUgLmN1c3RvbS1zZWxlY3Qge1xcbiAgICAgIHdpZHRoOiBhdXRvOyB9XFxuICAgIC5mb3JtLWlubGluZSAuZm9ybS1jaGVjayB7XFxuICAgICAgZGlzcGxheTogZmxleDtcXG4gICAgICBhbGlnbi1pdGVtczogY2VudGVyO1xcbiAgICAgIGp1c3RpZnktY29udGVudDogY2VudGVyO1xcbiAgICAgIHdpZHRoOiBhdXRvO1xcbiAgICAgIHBhZGRpbmctbGVmdDogMDsgfVxcbiAgICAuZm9ybS1pbmxpbmUgLmZvcm0tY2hlY2staW5wdXQge1xcbiAgICAgIHBvc2l0aW9uOiByZWxhdGl2ZTtcXG4gICAgICBmbGV4LXNocmluazogMDtcXG4gICAgICBtYXJnaW4tdG9wOiAwO1xcbiAgICAgIG1hcmdpbi1yaWdodDogMC4yNXJlbTtcXG4gICAgICBtYXJnaW4tbGVmdDogMDsgfVxcbiAgICAuZm9ybS1pbmxpbmUgLmN1c3RvbS1jb250cm9sIHtcXG4gICAgICBhbGlnbi1pdGVtczogY2VudGVyO1xcbiAgICAgIGp1c3RpZnktY29udGVudDogY2VudGVyOyB9XFxuICAgIC5mb3JtLWlubGluZSAuY3VzdG9tLWNvbnRyb2wtbGFiZWwge1xcbiAgICAgIG1hcmdpbi1ib3R0b206IDA7IH0gfVxcblxcbkBrZXlmcmFtZXMgcHJvZ3Jlc3MtYmFyLXN0cmlwZXMge1xcbiAgZnJvbSB7XFxuICAgIGJhY2tncm91bmQtcG9zaXRpb246IDFyZW0gMDsgfVxcbiAgdG8ge1xcbiAgICBiYWNrZ3JvdW5kLXBvc2l0aW9uOiAwIDA7IH0gfVxcblxcbi5wcm9ncmVzcyB7XFxuICBkaXNwbGF5OiBmbGV4O1xcbiAgaGVpZ2h0OiAxcmVtO1xcbiAgb3ZlcmZsb3c6IGhpZGRlbjtcXG4gIGZvbnQtc2l6ZTogMC43NXJlbTtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNlOWVjZWY7XFxuICBib3JkZXItcmFkaXVzOiAwLjI1cmVtOyB9XFxuXFxuLnByb2dyZXNzLWJhciB7XFxuICBkaXNwbGF5OiBmbGV4O1xcbiAgZmxleC1kaXJlY3Rpb246IGNvbHVtbjtcXG4gIGp1c3RpZnktY29udGVudDogY2VudGVyO1xcbiAgY29sb3I6ICNmZmY7XFxuICB0ZXh0LWFsaWduOiBjZW50ZXI7XFxuICB3aGl0ZS1zcGFjZTogbm93cmFwO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogIzAwN2JmZjtcXG4gIHRyYW5zaXRpb246IHdpZHRoIDAuNnMgZWFzZTsgfVxcbiAgQG1lZGlhIChwcmVmZXJzLXJlZHVjZWQtbW90aW9uOiByZWR1Y2UpIHtcXG4gICAgLnByb2dyZXNzLWJhciB7XFxuICAgICAgdHJhbnNpdGlvbjogbm9uZTsgfSB9XFxuXFxuLnByb2dyZXNzLWJhci1zdHJpcGVkIHtcXG4gIGJhY2tncm91bmQtaW1hZ2U6IGxpbmVhci1ncmFkaWVudCg0NWRlZywgcmdiYSgyNTUsIDI1NSwgMjU1LCAwLjE1KSAyNSUsIHRyYW5zcGFyZW50IDI1JSwgdHJhbnNwYXJlbnQgNTAlLCByZ2JhKDI1NSwgMjU1LCAyNTUsIDAuMTUpIDUwJSwgcmdiYSgyNTUsIDI1NSwgMjU1LCAwLjE1KSA3NSUsIHRyYW5zcGFyZW50IDc1JSwgdHJhbnNwYXJlbnQpO1xcbiAgYmFja2dyb3VuZC1zaXplOiAxcmVtIDFyZW07IH1cXG5cXG4ucHJvZ3Jlc3MtYmFyLWFuaW1hdGVkIHtcXG4gIGFuaW1hdGlvbjogcHJvZ3Jlc3MtYmFyLXN0cmlwZXMgMXMgbGluZWFyIGluZmluaXRlOyB9XFxuICBAbWVkaWEgKHByZWZlcnMtcmVkdWNlZC1tb3Rpb246IHJlZHVjZSkge1xcbiAgICAucHJvZ3Jlc3MtYmFyLWFuaW1hdGVkIHtcXG4gICAgICBhbmltYXRpb246IG5vbmU7IH0gfVxcblxcbi5kLW5vbmUge1xcbiAgZGlzcGxheTogbm9uZSAhaW1wb3J0YW50OyB9XFxuXFxuLmQtaW5saW5lIHtcXG4gIGRpc3BsYXk6IGlubGluZSAhaW1wb3J0YW50OyB9XFxuXFxuLmQtaW5saW5lLWJsb2NrIHtcXG4gIGRpc3BsYXk6IGlubGluZS1ibG9jayAhaW1wb3J0YW50OyB9XFxuXFxuLmQtYmxvY2sge1xcbiAgZGlzcGxheTogYmxvY2sgIWltcG9ydGFudDsgfVxcblxcbi5kLXRhYmxlIHtcXG4gIGRpc3BsYXk6IHRhYmxlICFpbXBvcnRhbnQ7IH1cXG5cXG4uZC10YWJsZS1yb3cge1xcbiAgZGlzcGxheTogdGFibGUtcm93ICFpbXBvcnRhbnQ7IH1cXG5cXG4uZC10YWJsZS1jZWxsIHtcXG4gIGRpc3BsYXk6IHRhYmxlLWNlbGwgIWltcG9ydGFudDsgfVxcblxcbi5kLWZsZXgge1xcbiAgZGlzcGxheTogZmxleCAhaW1wb3J0YW50OyB9XFxuXFxuLmQtaW5saW5lLWZsZXgge1xcbiAgZGlzcGxheTogaW5saW5lLWZsZXggIWltcG9ydGFudDsgfVxcblxcbkBtZWRpYSAobWluLXdpZHRoOiA1NzZweCkge1xcbiAgLmQtc20tbm9uZSB7XFxuICAgIGRpc3BsYXk6IG5vbmUgIWltcG9ydGFudDsgfVxcbiAgLmQtc20taW5saW5lIHtcXG4gICAgZGlzcGxheTogaW5saW5lICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXNtLWlubGluZS1ibG9jayB7XFxuICAgIGRpc3BsYXk6IGlubGluZS1ibG9jayAhaW1wb3J0YW50OyB9XFxuICAuZC1zbS1ibG9jayB7XFxuICAgIGRpc3BsYXk6IGJsb2NrICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXNtLXRhYmxlIHtcXG4gICAgZGlzcGxheTogdGFibGUgIWltcG9ydGFudDsgfVxcbiAgLmQtc20tdGFibGUtcm93IHtcXG4gICAgZGlzcGxheTogdGFibGUtcm93ICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXNtLXRhYmxlLWNlbGwge1xcbiAgICBkaXNwbGF5OiB0YWJsZS1jZWxsICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXNtLWZsZXgge1xcbiAgICBkaXNwbGF5OiBmbGV4ICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXNtLWlubGluZS1mbGV4IHtcXG4gICAgZGlzcGxheTogaW5saW5lLWZsZXggIWltcG9ydGFudDsgfSB9XFxuXFxuQG1lZGlhIChtaW4td2lkdGg6IDc2OHB4KSB7XFxuICAuZC1tZC1ub25lIHtcXG4gICAgZGlzcGxheTogbm9uZSAhaW1wb3J0YW50OyB9XFxuICAuZC1tZC1pbmxpbmUge1xcbiAgICBkaXNwbGF5OiBpbmxpbmUgIWltcG9ydGFudDsgfVxcbiAgLmQtbWQtaW5saW5lLWJsb2NrIHtcXG4gICAgZGlzcGxheTogaW5saW5lLWJsb2NrICFpbXBvcnRhbnQ7IH1cXG4gIC5kLW1kLWJsb2NrIHtcXG4gICAgZGlzcGxheTogYmxvY2sgIWltcG9ydGFudDsgfVxcbiAgLmQtbWQtdGFibGUge1xcbiAgICBkaXNwbGF5OiB0YWJsZSAhaW1wb3J0YW50OyB9XFxuICAuZC1tZC10YWJsZS1yb3cge1xcbiAgICBkaXNwbGF5OiB0YWJsZS1yb3cgIWltcG9ydGFudDsgfVxcbiAgLmQtbWQtdGFibGUtY2VsbCB7XFxuICAgIGRpc3BsYXk6IHRhYmxlLWNlbGwgIWltcG9ydGFudDsgfVxcbiAgLmQtbWQtZmxleCB7XFxuICAgIGRpc3BsYXk6IGZsZXggIWltcG9ydGFudDsgfVxcbiAgLmQtbWQtaW5saW5lLWZsZXgge1xcbiAgICBkaXNwbGF5OiBpbmxpbmUtZmxleCAhaW1wb3J0YW50OyB9IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogOTkycHgpIHtcXG4gIC5kLWxnLW5vbmUge1xcbiAgICBkaXNwbGF5OiBub25lICFpbXBvcnRhbnQ7IH1cXG4gIC5kLWxnLWlubGluZSB7XFxuICAgIGRpc3BsYXk6IGlubGluZSAhaW1wb3J0YW50OyB9XFxuICAuZC1sZy1pbmxpbmUtYmxvY2sge1xcbiAgICBkaXNwbGF5OiBpbmxpbmUtYmxvY2sgIWltcG9ydGFudDsgfVxcbiAgLmQtbGctYmxvY2sge1xcbiAgICBkaXNwbGF5OiBibG9jayAhaW1wb3J0YW50OyB9XFxuICAuZC1sZy10YWJsZSB7XFxuICAgIGRpc3BsYXk6IHRhYmxlICFpbXBvcnRhbnQ7IH1cXG4gIC5kLWxnLXRhYmxlLXJvdyB7XFxuICAgIGRpc3BsYXk6IHRhYmxlLXJvdyAhaW1wb3J0YW50OyB9XFxuICAuZC1sZy10YWJsZS1jZWxsIHtcXG4gICAgZGlzcGxheTogdGFibGUtY2VsbCAhaW1wb3J0YW50OyB9XFxuICAuZC1sZy1mbGV4IHtcXG4gICAgZGlzcGxheTogZmxleCAhaW1wb3J0YW50OyB9XFxuICAuZC1sZy1pbmxpbmUtZmxleCB7XFxuICAgIGRpc3BsYXk6IGlubGluZS1mbGV4ICFpbXBvcnRhbnQ7IH0gfVxcblxcbkBtZWRpYSAobWluLXdpZHRoOiAxMjAwcHgpIHtcXG4gIC5kLXhsLW5vbmUge1xcbiAgICBkaXNwbGF5OiBub25lICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXhsLWlubGluZSB7XFxuICAgIGRpc3BsYXk6IGlubGluZSAhaW1wb3J0YW50OyB9XFxuICAuZC14bC1pbmxpbmUtYmxvY2sge1xcbiAgICBkaXNwbGF5OiBpbmxpbmUtYmxvY2sgIWltcG9ydGFudDsgfVxcbiAgLmQteGwtYmxvY2sge1xcbiAgICBkaXNwbGF5OiBibG9jayAhaW1wb3J0YW50OyB9XFxuICAuZC14bC10YWJsZSB7XFxuICAgIGRpc3BsYXk6IHRhYmxlICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXhsLXRhYmxlLXJvdyB7XFxuICAgIGRpc3BsYXk6IHRhYmxlLXJvdyAhaW1wb3J0YW50OyB9XFxuICAuZC14bC10YWJsZS1jZWxsIHtcXG4gICAgZGlzcGxheTogdGFibGUtY2VsbCAhaW1wb3J0YW50OyB9XFxuICAuZC14bC1mbGV4IHtcXG4gICAgZGlzcGxheTogZmxleCAhaW1wb3J0YW50OyB9XFxuICAuZC14bC1pbmxpbmUtZmxleCB7XFxuICAgIGRpc3BsYXk6IGlubGluZS1mbGV4ICFpbXBvcnRhbnQ7IH0gfVxcblxcbkBtZWRpYSBwcmludCB7XFxuICAuZC1wcmludC1ub25lIHtcXG4gICAgZGlzcGxheTogbm9uZSAhaW1wb3J0YW50OyB9XFxuICAuZC1wcmludC1pbmxpbmUge1xcbiAgICBkaXNwbGF5OiBpbmxpbmUgIWltcG9ydGFudDsgfVxcbiAgLmQtcHJpbnQtaW5saW5lLWJsb2NrIHtcXG4gICAgZGlzcGxheTogaW5saW5lLWJsb2NrICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXByaW50LWJsb2NrIHtcXG4gICAgZGlzcGxheTogYmxvY2sgIWltcG9ydGFudDsgfVxcbiAgLmQtcHJpbnQtdGFibGUge1xcbiAgICBkaXNwbGF5OiB0YWJsZSAhaW1wb3J0YW50OyB9XFxuICAuZC1wcmludC10YWJsZS1yb3cge1xcbiAgICBkaXNwbGF5OiB0YWJsZS1yb3cgIWltcG9ydGFudDsgfVxcbiAgLmQtcHJpbnQtdGFibGUtY2VsbCB7XFxuICAgIGRpc3BsYXk6IHRhYmxlLWNlbGwgIWltcG9ydGFudDsgfVxcbiAgLmQtcHJpbnQtZmxleCB7XFxuICAgIGRpc3BsYXk6IGZsZXggIWltcG9ydGFudDsgfVxcbiAgLmQtcHJpbnQtaW5saW5lLWZsZXgge1xcbiAgICBkaXNwbGF5OiBpbmxpbmUtZmxleCAhaW1wb3J0YW50OyB9IH1cXG5cXG4uZmxleC1yb3cge1xcbiAgZmxleC1kaXJlY3Rpb246IHJvdyAhaW1wb3J0YW50OyB9XFxuXFxuLmZsZXgtY29sdW1uIHtcXG4gIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW4gIWltcG9ydGFudDsgfVxcblxcbi5mbGV4LXJvdy1yZXZlcnNlIHtcXG4gIGZsZXgtZGlyZWN0aW9uOiByb3ctcmV2ZXJzZSAhaW1wb3J0YW50OyB9XFxuXFxuLmZsZXgtY29sdW1uLXJldmVyc2Uge1xcbiAgZmxleC1kaXJlY3Rpb246IGNvbHVtbi1yZXZlcnNlICFpbXBvcnRhbnQ7IH1cXG5cXG4uZmxleC13cmFwIHtcXG4gIGZsZXgtd3JhcDogd3JhcCAhaW1wb3J0YW50OyB9XFxuXFxuLmZsZXgtbm93cmFwIHtcXG4gIGZsZXgtd3JhcDogbm93cmFwICFpbXBvcnRhbnQ7IH1cXG5cXG4uZmxleC13cmFwLXJldmVyc2Uge1xcbiAgZmxleC13cmFwOiB3cmFwLXJldmVyc2UgIWltcG9ydGFudDsgfVxcblxcbi5mbGV4LWZpbGwge1xcbiAgZmxleDogMSAxIGF1dG8gIWltcG9ydGFudDsgfVxcblxcbi5mbGV4LWdyb3ctMCB7XFxuICBmbGV4LWdyb3c6IDAgIWltcG9ydGFudDsgfVxcblxcbi5mbGV4LWdyb3ctMSB7XFxuICBmbGV4LWdyb3c6IDEgIWltcG9ydGFudDsgfVxcblxcbi5mbGV4LXNocmluay0wIHtcXG4gIGZsZXgtc2hyaW5rOiAwICFpbXBvcnRhbnQ7IH1cXG5cXG4uZmxleC1zaHJpbmstMSB7XFxuICBmbGV4LXNocmluazogMSAhaW1wb3J0YW50OyB9XFxuXFxuLmp1c3RpZnktY29udGVudC1zdGFydCB7XFxuICBqdXN0aWZ5LWNvbnRlbnQ6IGZsZXgtc3RhcnQgIWltcG9ydGFudDsgfVxcblxcbi5qdXN0aWZ5LWNvbnRlbnQtZW5kIHtcXG4gIGp1c3RpZnktY29udGVudDogZmxleC1lbmQgIWltcG9ydGFudDsgfVxcblxcbi5qdXN0aWZ5LWNvbnRlbnQtY2VudGVyIHtcXG4gIGp1c3RpZnktY29udGVudDogY2VudGVyICFpbXBvcnRhbnQ7IH1cXG5cXG4uanVzdGlmeS1jb250ZW50LWJldHdlZW4ge1xcbiAganVzdGlmeS1jb250ZW50OiBzcGFjZS1iZXR3ZWVuICFpbXBvcnRhbnQ7IH1cXG5cXG4uanVzdGlmeS1jb250ZW50LWFyb3VuZCB7XFxuICBqdXN0aWZ5LWNvbnRlbnQ6IHNwYWNlLWFyb3VuZCAhaW1wb3J0YW50OyB9XFxuXFxuLmFsaWduLWl0ZW1zLXN0YXJ0IHtcXG4gIGFsaWduLWl0ZW1zOiBmbGV4LXN0YXJ0ICFpbXBvcnRhbnQ7IH1cXG5cXG4uYWxpZ24taXRlbXMtZW5kIHtcXG4gIGFsaWduLWl0ZW1zOiBmbGV4LWVuZCAhaW1wb3J0YW50OyB9XFxuXFxuLmFsaWduLWl0ZW1zLWNlbnRlciB7XFxuICBhbGlnbi1pdGVtczogY2VudGVyICFpbXBvcnRhbnQ7IH1cXG5cXG4uYWxpZ24taXRlbXMtYmFzZWxpbmUge1xcbiAgYWxpZ24taXRlbXM6IGJhc2VsaW5lICFpbXBvcnRhbnQ7IH1cXG5cXG4uYWxpZ24taXRlbXMtc3RyZXRjaCB7XFxuICBhbGlnbi1pdGVtczogc3RyZXRjaCAhaW1wb3J0YW50OyB9XFxuXFxuLmFsaWduLWNvbnRlbnQtc3RhcnQge1xcbiAgYWxpZ24tY29udGVudDogZmxleC1zdGFydCAhaW1wb3J0YW50OyB9XFxuXFxuLmFsaWduLWNvbnRlbnQtZW5kIHtcXG4gIGFsaWduLWNvbnRlbnQ6IGZsZXgtZW5kICFpbXBvcnRhbnQ7IH1cXG5cXG4uYWxpZ24tY29udGVudC1jZW50ZXIge1xcbiAgYWxpZ24tY29udGVudDogY2VudGVyICFpbXBvcnRhbnQ7IH1cXG5cXG4uYWxpZ24tY29udGVudC1iZXR3ZWVuIHtcXG4gIGFsaWduLWNvbnRlbnQ6IHNwYWNlLWJldHdlZW4gIWltcG9ydGFudDsgfVxcblxcbi5hbGlnbi1jb250ZW50LWFyb3VuZCB7XFxuICBhbGlnbi1jb250ZW50OiBzcGFjZS1hcm91bmQgIWltcG9ydGFudDsgfVxcblxcbi5hbGlnbi1jb250ZW50LXN0cmV0Y2gge1xcbiAgYWxpZ24tY29udGVudDogc3RyZXRjaCAhaW1wb3J0YW50OyB9XFxuXFxuLmFsaWduLXNlbGYtYXV0byB7XFxuICBhbGlnbi1zZWxmOiBhdXRvICFpbXBvcnRhbnQ7IH1cXG5cXG4uYWxpZ24tc2VsZi1zdGFydCB7XFxuICBhbGlnbi1zZWxmOiBmbGV4LXN0YXJ0ICFpbXBvcnRhbnQ7IH1cXG5cXG4uYWxpZ24tc2VsZi1lbmQge1xcbiAgYWxpZ24tc2VsZjogZmxleC1lbmQgIWltcG9ydGFudDsgfVxcblxcbi5hbGlnbi1zZWxmLWNlbnRlciB7XFxuICBhbGlnbi1zZWxmOiBjZW50ZXIgIWltcG9ydGFudDsgfVxcblxcbi5hbGlnbi1zZWxmLWJhc2VsaW5lIHtcXG4gIGFsaWduLXNlbGY6IGJhc2VsaW5lICFpbXBvcnRhbnQ7IH1cXG5cXG4uYWxpZ24tc2VsZi1zdHJldGNoIHtcXG4gIGFsaWduLXNlbGY6IHN0cmV0Y2ggIWltcG9ydGFudDsgfVxcblxcbkBtZWRpYSAobWluLXdpZHRoOiA1NzZweCkge1xcbiAgLmZsZXgtc20tcm93IHtcXG4gICAgZmxleC1kaXJlY3Rpb246IHJvdyAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1zbS1jb2x1bW4ge1xcbiAgICBmbGV4LWRpcmVjdGlvbjogY29sdW1uICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LXNtLXJvdy1yZXZlcnNlIHtcXG4gICAgZmxleC1kaXJlY3Rpb246IHJvdy1yZXZlcnNlICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LXNtLWNvbHVtbi1yZXZlcnNlIHtcXG4gICAgZmxleC1kaXJlY3Rpb246IGNvbHVtbi1yZXZlcnNlICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LXNtLXdyYXAge1xcbiAgICBmbGV4LXdyYXA6IHdyYXAgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtc20tbm93cmFwIHtcXG4gICAgZmxleC13cmFwOiBub3dyYXAgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtc20td3JhcC1yZXZlcnNlIHtcXG4gICAgZmxleC13cmFwOiB3cmFwLXJldmVyc2UgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtc20tZmlsbCB7XFxuICAgIGZsZXg6IDEgMSBhdXRvICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LXNtLWdyb3ctMCB7XFxuICAgIGZsZXgtZ3JvdzogMCAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1zbS1ncm93LTEge1xcbiAgICBmbGV4LWdyb3c6IDEgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtc20tc2hyaW5rLTAge1xcbiAgICBmbGV4LXNocmluazogMCAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1zbS1zaHJpbmstMSB7XFxuICAgIGZsZXgtc2hyaW5rOiAxICFpbXBvcnRhbnQ7IH1cXG4gIC5qdXN0aWZ5LWNvbnRlbnQtc20tc3RhcnQge1xcbiAgICBqdXN0aWZ5LWNvbnRlbnQ6IGZsZXgtc3RhcnQgIWltcG9ydGFudDsgfVxcbiAgLmp1c3RpZnktY29udGVudC1zbS1lbmQge1xcbiAgICBqdXN0aWZ5LWNvbnRlbnQ6IGZsZXgtZW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5qdXN0aWZ5LWNvbnRlbnQtc20tY2VudGVyIHtcXG4gICAganVzdGlmeS1jb250ZW50OiBjZW50ZXIgIWltcG9ydGFudDsgfVxcbiAgLmp1c3RpZnktY29udGVudC1zbS1iZXR3ZWVuIHtcXG4gICAganVzdGlmeS1jb250ZW50OiBzcGFjZS1iZXR3ZWVuICFpbXBvcnRhbnQ7IH1cXG4gIC5qdXN0aWZ5LWNvbnRlbnQtc20tYXJvdW5kIHtcXG4gICAganVzdGlmeS1jb250ZW50OiBzcGFjZS1hcm91bmQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWl0ZW1zLXNtLXN0YXJ0IHtcXG4gICAgYWxpZ24taXRlbXM6IGZsZXgtc3RhcnQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWl0ZW1zLXNtLWVuZCB7XFxuICAgIGFsaWduLWl0ZW1zOiBmbGV4LWVuZCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24taXRlbXMtc20tY2VudGVyIHtcXG4gICAgYWxpZ24taXRlbXM6IGNlbnRlciAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24taXRlbXMtc20tYmFzZWxpbmUge1xcbiAgICBhbGlnbi1pdGVtczogYmFzZWxpbmUgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWl0ZW1zLXNtLXN0cmV0Y2gge1xcbiAgICBhbGlnbi1pdGVtczogc3RyZXRjaCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC1zbS1zdGFydCB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IGZsZXgtc3RhcnQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQtc20tZW5kIHtcXG4gICAgYWxpZ24tY29udGVudDogZmxleC1lbmQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQtc20tY2VudGVyIHtcXG4gICAgYWxpZ24tY29udGVudDogY2VudGVyICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LXNtLWJldHdlZW4ge1xcbiAgICBhbGlnbi1jb250ZW50OiBzcGFjZS1iZXR3ZWVuICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LXNtLWFyb3VuZCB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IHNwYWNlLWFyb3VuZCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC1zbS1zdHJldGNoIHtcXG4gICAgYWxpZ24tY29udGVudDogc3RyZXRjaCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi1zbS1hdXRvIHtcXG4gICAgYWxpZ24tc2VsZjogYXV0byAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi1zbS1zdGFydCB7XFxuICAgIGFsaWduLXNlbGY6IGZsZXgtc3RhcnQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLXNlbGYtc20tZW5kIHtcXG4gICAgYWxpZ24tc2VsZjogZmxleC1lbmQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLXNlbGYtc20tY2VudGVyIHtcXG4gICAgYWxpZ24tc2VsZjogY2VudGVyICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLXNtLWJhc2VsaW5lIHtcXG4gICAgYWxpZ24tc2VsZjogYmFzZWxpbmUgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLXNlbGYtc20tc3RyZXRjaCB7XFxuICAgIGFsaWduLXNlbGY6IHN0cmV0Y2ggIWltcG9ydGFudDsgfSB9XFxuXFxuQG1lZGlhIChtaW4td2lkdGg6IDc2OHB4KSB7XFxuICAuZmxleC1tZC1yb3cge1xcbiAgICBmbGV4LWRpcmVjdGlvbjogcm93ICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LW1kLWNvbHVtbiB7XFxuICAgIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW4gIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtbWQtcm93LXJldmVyc2Uge1xcbiAgICBmbGV4LWRpcmVjdGlvbjogcm93LXJldmVyc2UgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtbWQtY29sdW1uLXJldmVyc2Uge1xcbiAgICBmbGV4LWRpcmVjdGlvbjogY29sdW1uLXJldmVyc2UgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtbWQtd3JhcCB7XFxuICAgIGZsZXgtd3JhcDogd3JhcCAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1tZC1ub3dyYXAge1xcbiAgICBmbGV4LXdyYXA6IG5vd3JhcCAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1tZC13cmFwLXJldmVyc2Uge1xcbiAgICBmbGV4LXdyYXA6IHdyYXAtcmV2ZXJzZSAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1tZC1maWxsIHtcXG4gICAgZmxleDogMSAxIGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtbWQtZ3Jvdy0wIHtcXG4gICAgZmxleC1ncm93OiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LW1kLWdyb3ctMSB7XFxuICAgIGZsZXgtZ3JvdzogMSAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1tZC1zaHJpbmstMCB7XFxuICAgIGZsZXgtc2hyaW5rOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LW1kLXNocmluay0xIHtcXG4gICAgZmxleC1zaHJpbms6IDEgIWltcG9ydGFudDsgfVxcbiAgLmp1c3RpZnktY29udGVudC1tZC1zdGFydCB7XFxuICAgIGp1c3RpZnktY29udGVudDogZmxleC1zdGFydCAhaW1wb3J0YW50OyB9XFxuICAuanVzdGlmeS1jb250ZW50LW1kLWVuZCB7XFxuICAgIGp1c3RpZnktY29udGVudDogZmxleC1lbmQgIWltcG9ydGFudDsgfVxcbiAgLmp1c3RpZnktY29udGVudC1tZC1jZW50ZXIge1xcbiAgICBqdXN0aWZ5LWNvbnRlbnQ6IGNlbnRlciAhaW1wb3J0YW50OyB9XFxuICAuanVzdGlmeS1jb250ZW50LW1kLWJldHdlZW4ge1xcbiAgICBqdXN0aWZ5LWNvbnRlbnQ6IHNwYWNlLWJldHdlZW4gIWltcG9ydGFudDsgfVxcbiAgLmp1c3RpZnktY29udGVudC1tZC1hcm91bmQge1xcbiAgICBqdXN0aWZ5LWNvbnRlbnQ6IHNwYWNlLWFyb3VuZCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24taXRlbXMtbWQtc3RhcnQge1xcbiAgICBhbGlnbi1pdGVtczogZmxleC1zdGFydCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24taXRlbXMtbWQtZW5kIHtcXG4gICAgYWxpZ24taXRlbXM6IGZsZXgtZW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1pdGVtcy1tZC1jZW50ZXIge1xcbiAgICBhbGlnbi1pdGVtczogY2VudGVyICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1pdGVtcy1tZC1iYXNlbGluZSB7XFxuICAgIGFsaWduLWl0ZW1zOiBiYXNlbGluZSAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24taXRlbXMtbWQtc3RyZXRjaCB7XFxuICAgIGFsaWduLWl0ZW1zOiBzdHJldGNoICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LW1kLXN0YXJ0IHtcXG4gICAgYWxpZ24tY29udGVudDogZmxleC1zdGFydCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC1tZC1lbmQge1xcbiAgICBhbGlnbi1jb250ZW50OiBmbGV4LWVuZCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC1tZC1jZW50ZXIge1xcbiAgICBhbGlnbi1jb250ZW50OiBjZW50ZXIgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQtbWQtYmV0d2VlbiB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IHNwYWNlLWJldHdlZW4gIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQtbWQtYXJvdW5kIHtcXG4gICAgYWxpZ24tY29udGVudDogc3BhY2UtYXJvdW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LW1kLXN0cmV0Y2gge1xcbiAgICBhbGlnbi1jb250ZW50OiBzdHJldGNoICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLW1kLWF1dG8ge1xcbiAgICBhbGlnbi1zZWxmOiBhdXRvICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLW1kLXN0YXJ0IHtcXG4gICAgYWxpZ24tc2VsZjogZmxleC1zdGFydCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi1tZC1lbmQge1xcbiAgICBhbGlnbi1zZWxmOiBmbGV4LWVuZCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi1tZC1jZW50ZXIge1xcbiAgICBhbGlnbi1zZWxmOiBjZW50ZXIgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLXNlbGYtbWQtYmFzZWxpbmUge1xcbiAgICBhbGlnbi1zZWxmOiBiYXNlbGluZSAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi1tZC1zdHJldGNoIHtcXG4gICAgYWxpZ24tc2VsZjogc3RyZXRjaCAhaW1wb3J0YW50OyB9IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogOTkycHgpIHtcXG4gIC5mbGV4LWxnLXJvdyB7XFxuICAgIGZsZXgtZGlyZWN0aW9uOiByb3cgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtbGctY29sdW1uIHtcXG4gICAgZmxleC1kaXJlY3Rpb246IGNvbHVtbiAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1sZy1yb3ctcmV2ZXJzZSB7XFxuICAgIGZsZXgtZGlyZWN0aW9uOiByb3ctcmV2ZXJzZSAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1sZy1jb2x1bW4tcmV2ZXJzZSB7XFxuICAgIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW4tcmV2ZXJzZSAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1sZy13cmFwIHtcXG4gICAgZmxleC13cmFwOiB3cmFwICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LWxnLW5vd3JhcCB7XFxuICAgIGZsZXgtd3JhcDogbm93cmFwICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LWxnLXdyYXAtcmV2ZXJzZSB7XFxuICAgIGZsZXgtd3JhcDogd3JhcC1yZXZlcnNlICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LWxnLWZpbGwge1xcbiAgICBmbGV4OiAxIDEgYXV0byAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1sZy1ncm93LTAge1xcbiAgICBmbGV4LWdyb3c6IDAgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtbGctZ3Jvdy0xIHtcXG4gICAgZmxleC1ncm93OiAxICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LWxnLXNocmluay0wIHtcXG4gICAgZmxleC1zaHJpbms6IDAgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtbGctc2hyaW5rLTEge1xcbiAgICBmbGV4LXNocmluazogMSAhaW1wb3J0YW50OyB9XFxuICAuanVzdGlmeS1jb250ZW50LWxnLXN0YXJ0IHtcXG4gICAganVzdGlmeS1jb250ZW50OiBmbGV4LXN0YXJ0ICFpbXBvcnRhbnQ7IH1cXG4gIC5qdXN0aWZ5LWNvbnRlbnQtbGctZW5kIHtcXG4gICAganVzdGlmeS1jb250ZW50OiBmbGV4LWVuZCAhaW1wb3J0YW50OyB9XFxuICAuanVzdGlmeS1jb250ZW50LWxnLWNlbnRlciB7XFxuICAgIGp1c3RpZnktY29udGVudDogY2VudGVyICFpbXBvcnRhbnQ7IH1cXG4gIC5qdXN0aWZ5LWNvbnRlbnQtbGctYmV0d2VlbiB7XFxuICAgIGp1c3RpZnktY29udGVudDogc3BhY2UtYmV0d2VlbiAhaW1wb3J0YW50OyB9XFxuICAuanVzdGlmeS1jb250ZW50LWxnLWFyb3VuZCB7XFxuICAgIGp1c3RpZnktY29udGVudDogc3BhY2UtYXJvdW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1pdGVtcy1sZy1zdGFydCB7XFxuICAgIGFsaWduLWl0ZW1zOiBmbGV4LXN0YXJ0ICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1pdGVtcy1sZy1lbmQge1xcbiAgICBhbGlnbi1pdGVtczogZmxleC1lbmQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWl0ZW1zLWxnLWNlbnRlciB7XFxuICAgIGFsaWduLWl0ZW1zOiBjZW50ZXIgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWl0ZW1zLWxnLWJhc2VsaW5lIHtcXG4gICAgYWxpZ24taXRlbXM6IGJhc2VsaW5lICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1pdGVtcy1sZy1zdHJldGNoIHtcXG4gICAgYWxpZ24taXRlbXM6IHN0cmV0Y2ggIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQtbGctc3RhcnQge1xcbiAgICBhbGlnbi1jb250ZW50OiBmbGV4LXN0YXJ0ICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LWxnLWVuZCB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IGZsZXgtZW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LWxnLWNlbnRlciB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IGNlbnRlciAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC1sZy1iZXR3ZWVuIHtcXG4gICAgYWxpZ24tY29udGVudDogc3BhY2UtYmV0d2VlbiAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC1sZy1hcm91bmQge1xcbiAgICBhbGlnbi1jb250ZW50OiBzcGFjZS1hcm91bmQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQtbGctc3RyZXRjaCB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IHN0cmV0Y2ggIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLXNlbGYtbGctYXV0byB7XFxuICAgIGFsaWduLXNlbGY6IGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLXNlbGYtbGctc3RhcnQge1xcbiAgICBhbGlnbi1zZWxmOiBmbGV4LXN0YXJ0ICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLWxnLWVuZCB7XFxuICAgIGFsaWduLXNlbGY6IGZsZXgtZW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLWxnLWNlbnRlciB7XFxuICAgIGFsaWduLXNlbGY6IGNlbnRlciAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi1sZy1iYXNlbGluZSB7XFxuICAgIGFsaWduLXNlbGY6IGJhc2VsaW5lICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLWxnLXN0cmV0Y2gge1xcbiAgICBhbGlnbi1zZWxmOiBzdHJldGNoICFpbXBvcnRhbnQ7IH0gfVxcblxcbkBtZWRpYSAobWluLXdpZHRoOiAxMjAwcHgpIHtcXG4gIC5mbGV4LXhsLXJvdyB7XFxuICAgIGZsZXgtZGlyZWN0aW9uOiByb3cgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgteGwtY29sdW1uIHtcXG4gICAgZmxleC1kaXJlY3Rpb246IGNvbHVtbiAhaW1wb3J0YW50OyB9XFxuICAuZmxleC14bC1yb3ctcmV2ZXJzZSB7XFxuICAgIGZsZXgtZGlyZWN0aW9uOiByb3ctcmV2ZXJzZSAhaW1wb3J0YW50OyB9XFxuICAuZmxleC14bC1jb2x1bW4tcmV2ZXJzZSB7XFxuICAgIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW4tcmV2ZXJzZSAhaW1wb3J0YW50OyB9XFxuICAuZmxleC14bC13cmFwIHtcXG4gICAgZmxleC13cmFwOiB3cmFwICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LXhsLW5vd3JhcCB7XFxuICAgIGZsZXgtd3JhcDogbm93cmFwICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LXhsLXdyYXAtcmV2ZXJzZSB7XFxuICAgIGZsZXgtd3JhcDogd3JhcC1yZXZlcnNlICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LXhsLWZpbGwge1xcbiAgICBmbGV4OiAxIDEgYXV0byAhaW1wb3J0YW50OyB9XFxuICAuZmxleC14bC1ncm93LTAge1xcbiAgICBmbGV4LWdyb3c6IDAgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgteGwtZ3Jvdy0xIHtcXG4gICAgZmxleC1ncm93OiAxICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LXhsLXNocmluay0wIHtcXG4gICAgZmxleC1zaHJpbms6IDAgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgteGwtc2hyaW5rLTEge1xcbiAgICBmbGV4LXNocmluazogMSAhaW1wb3J0YW50OyB9XFxuICAuanVzdGlmeS1jb250ZW50LXhsLXN0YXJ0IHtcXG4gICAganVzdGlmeS1jb250ZW50OiBmbGV4LXN0YXJ0ICFpbXBvcnRhbnQ7IH1cXG4gIC5qdXN0aWZ5LWNvbnRlbnQteGwtZW5kIHtcXG4gICAganVzdGlmeS1jb250ZW50OiBmbGV4LWVuZCAhaW1wb3J0YW50OyB9XFxuICAuanVzdGlmeS1jb250ZW50LXhsLWNlbnRlciB7XFxuICAgIGp1c3RpZnktY29udGVudDogY2VudGVyICFpbXBvcnRhbnQ7IH1cXG4gIC5qdXN0aWZ5LWNvbnRlbnQteGwtYmV0d2VlbiB7XFxuICAgIGp1c3RpZnktY29udGVudDogc3BhY2UtYmV0d2VlbiAhaW1wb3J0YW50OyB9XFxuICAuanVzdGlmeS1jb250ZW50LXhsLWFyb3VuZCB7XFxuICAgIGp1c3RpZnktY29udGVudDogc3BhY2UtYXJvdW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1pdGVtcy14bC1zdGFydCB7XFxuICAgIGFsaWduLWl0ZW1zOiBmbGV4LXN0YXJ0ICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1pdGVtcy14bC1lbmQge1xcbiAgICBhbGlnbi1pdGVtczogZmxleC1lbmQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWl0ZW1zLXhsLWNlbnRlciB7XFxuICAgIGFsaWduLWl0ZW1zOiBjZW50ZXIgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWl0ZW1zLXhsLWJhc2VsaW5lIHtcXG4gICAgYWxpZ24taXRlbXM6IGJhc2VsaW5lICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1pdGVtcy14bC1zdHJldGNoIHtcXG4gICAgYWxpZ24taXRlbXM6IHN0cmV0Y2ggIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQteGwtc3RhcnQge1xcbiAgICBhbGlnbi1jb250ZW50OiBmbGV4LXN0YXJ0ICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LXhsLWVuZCB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IGZsZXgtZW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LXhsLWNlbnRlciB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IGNlbnRlciAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC14bC1iZXR3ZWVuIHtcXG4gICAgYWxpZ24tY29udGVudDogc3BhY2UtYmV0d2VlbiAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC14bC1hcm91bmQge1xcbiAgICBhbGlnbi1jb250ZW50OiBzcGFjZS1hcm91bmQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQteGwtc3RyZXRjaCB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IHN0cmV0Y2ggIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLXNlbGYteGwtYXV0byB7XFxuICAgIGFsaWduLXNlbGY6IGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLXNlbGYteGwtc3RhcnQge1xcbiAgICBhbGlnbi1zZWxmOiBmbGV4LXN0YXJ0ICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLXhsLWVuZCB7XFxuICAgIGFsaWduLXNlbGY6IGZsZXgtZW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLXhsLWNlbnRlciB7XFxuICAgIGFsaWduLXNlbGY6IGNlbnRlciAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi14bC1iYXNlbGluZSB7XFxuICAgIGFsaWduLXNlbGY6IGJhc2VsaW5lICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLXhsLXN0cmV0Y2gge1xcbiAgICBhbGlnbi1zZWxmOiBzdHJldGNoICFpbXBvcnRhbnQ7IH0gfVxcblxcbi5tLTAge1xcbiAgbWFyZ2luOiAwICFpbXBvcnRhbnQ7IH1cXG5cXG4ubXQtMCxcXG4ubXktMCB7XFxuICBtYXJnaW4tdG9wOiAwICFpbXBvcnRhbnQ7IH1cXG5cXG4ubXItMCxcXG4ubXgtMCB7XFxuICBtYXJnaW4tcmlnaHQ6IDAgIWltcG9ydGFudDsgfVxcblxcbi5tYi0wLFxcbi5teS0wIHtcXG4gIG1hcmdpbi1ib3R0b206IDAgIWltcG9ydGFudDsgfVxcblxcbi5tbC0wLFxcbi5teC0wIHtcXG4gIG1hcmdpbi1sZWZ0OiAwICFpbXBvcnRhbnQ7IH1cXG5cXG4ubS0xIHtcXG4gIG1hcmdpbjogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm10LTEsXFxuLm15LTEge1xcbiAgbWFyZ2luLXRvcDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1yLTEsXFxuLm14LTEge1xcbiAgbWFyZ2luLXJpZ2h0OiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWItMSxcXG4ubXktMSB7XFxuICBtYXJnaW4tYm90dG9tOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWwtMSxcXG4ubXgtMSB7XFxuICBtYXJnaW4tbGVmdDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm0tMiB7XFxuICBtYXJnaW46IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm10LTIsXFxuLm15LTIge1xcbiAgbWFyZ2luLXRvcDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubXItMixcXG4ubXgtMiB7XFxuICBtYXJnaW4tcmlnaHQ6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1iLTIsXFxuLm15LTIge1xcbiAgbWFyZ2luLWJvdHRvbTogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWwtMixcXG4ubXgtMiB7XFxuICBtYXJnaW4tbGVmdDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubS0zIHtcXG4gIG1hcmdpbjogMXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm10LTMsXFxuLm15LTMge1xcbiAgbWFyZ2luLXRvcDogMXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1yLTMsXFxuLm14LTMge1xcbiAgbWFyZ2luLXJpZ2h0OiAxcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWItMyxcXG4ubXktMyB7XFxuICBtYXJnaW4tYm90dG9tOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWwtMyxcXG4ubXgtMyB7XFxuICBtYXJnaW4tbGVmdDogMXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm0tNCB7XFxuICBtYXJnaW46IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm10LTQsXFxuLm15LTQge1xcbiAgbWFyZ2luLXRvcDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubXItNCxcXG4ubXgtNCB7XFxuICBtYXJnaW4tcmlnaHQ6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1iLTQsXFxuLm15LTQge1xcbiAgbWFyZ2luLWJvdHRvbTogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWwtNCxcXG4ubXgtNCB7XFxuICBtYXJnaW4tbGVmdDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubS01IHtcXG4gIG1hcmdpbjogM3JlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm10LTUsXFxuLm15LTUge1xcbiAgbWFyZ2luLXRvcDogM3JlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1yLTUsXFxuLm14LTUge1xcbiAgbWFyZ2luLXJpZ2h0OiAzcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWItNSxcXG4ubXktNSB7XFxuICBtYXJnaW4tYm90dG9tOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWwtNSxcXG4ubXgtNSB7XFxuICBtYXJnaW4tbGVmdDogM3JlbSAhaW1wb3J0YW50OyB9XFxuXFxuLnAtMCB7XFxuICBwYWRkaW5nOiAwICFpbXBvcnRhbnQ7IH1cXG5cXG4ucHQtMCxcXG4ucHktMCB7XFxuICBwYWRkaW5nLXRvcDogMCAhaW1wb3J0YW50OyB9XFxuXFxuLnByLTAsXFxuLnB4LTAge1xcbiAgcGFkZGluZy1yaWdodDogMCAhaW1wb3J0YW50OyB9XFxuXFxuLnBiLTAsXFxuLnB5LTAge1xcbiAgcGFkZGluZy1ib3R0b206IDAgIWltcG9ydGFudDsgfVxcblxcbi5wbC0wLFxcbi5weC0wIHtcXG4gIHBhZGRpbmctbGVmdDogMCAhaW1wb3J0YW50OyB9XFxuXFxuLnAtMSB7XFxuICBwYWRkaW5nOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucHQtMSxcXG4ucHktMSB7XFxuICBwYWRkaW5nLXRvcDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLnByLTEsXFxuLnB4LTEge1xcbiAgcGFkZGluZy1yaWdodDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLnBiLTEsXFxuLnB5LTEge1xcbiAgcGFkZGluZy1ib3R0b206IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wbC0xLFxcbi5weC0xIHtcXG4gIHBhZGRpbmctbGVmdDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLnAtMiB7XFxuICBwYWRkaW5nOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wdC0yLFxcbi5weS0yIHtcXG4gIHBhZGRpbmctdG9wOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wci0yLFxcbi5weC0yIHtcXG4gIHBhZGRpbmctcmlnaHQ6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLnBiLTIsXFxuLnB5LTIge1xcbiAgcGFkZGluZy1ib3R0b206IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLnBsLTIsXFxuLnB4LTIge1xcbiAgcGFkZGluZy1sZWZ0OiAwLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wLTMge1xcbiAgcGFkZGluZzogMXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLnB0LTMsXFxuLnB5LTMge1xcbiAgcGFkZGluZy10b3A6IDFyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wci0zLFxcbi5weC0zIHtcXG4gIHBhZGRpbmctcmlnaHQ6IDFyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wYi0zLFxcbi5weS0zIHtcXG4gIHBhZGRpbmctYm90dG9tOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucGwtMyxcXG4ucHgtMyB7XFxuICBwYWRkaW5nLWxlZnQ6IDFyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wLTQge1xcbiAgcGFkZGluZzogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucHQtNCxcXG4ucHktNCB7XFxuICBwYWRkaW5nLXRvcDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucHItNCxcXG4ucHgtNCB7XFxuICBwYWRkaW5nLXJpZ2h0OiAxLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wYi00LFxcbi5weS00IHtcXG4gIHBhZGRpbmctYm90dG9tOiAxLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wbC00LFxcbi5weC00IHtcXG4gIHBhZGRpbmctbGVmdDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucC01IHtcXG4gIHBhZGRpbmc6IDNyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wdC01LFxcbi5weS01IHtcXG4gIHBhZGRpbmctdG9wOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucHItNSxcXG4ucHgtNSB7XFxuICBwYWRkaW5nLXJpZ2h0OiAzcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucGItNSxcXG4ucHktNSB7XFxuICBwYWRkaW5nLWJvdHRvbTogM3JlbSAhaW1wb3J0YW50OyB9XFxuXFxuLnBsLTUsXFxuLnB4LTUge1xcbiAgcGFkZGluZy1sZWZ0OiAzcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubS1uMSB7XFxuICBtYXJnaW46IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubXQtbjEsXFxuLm15LW4xIHtcXG4gIG1hcmdpbi10b3A6IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubXItbjEsXFxuLm14LW4xIHtcXG4gIG1hcmdpbi1yaWdodDogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tYi1uMSxcXG4ubXktbjEge1xcbiAgbWFyZ2luLWJvdHRvbTogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tbC1uMSxcXG4ubXgtbjEge1xcbiAgbWFyZ2luLWxlZnQ6IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubS1uMiB7XFxuICBtYXJnaW46IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tdC1uMixcXG4ubXktbjIge1xcbiAgbWFyZ2luLXRvcDogLTAuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1yLW4yLFxcbi5teC1uMiB7XFxuICBtYXJnaW4tcmlnaHQ6IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tYi1uMixcXG4ubXktbjIge1xcbiAgbWFyZ2luLWJvdHRvbTogLTAuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1sLW4yLFxcbi5teC1uMiB7XFxuICBtYXJnaW4tbGVmdDogLTAuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm0tbjMge1xcbiAgbWFyZ2luOiAtMXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm10LW4zLFxcbi5teS1uMyB7XFxuICBtYXJnaW4tdG9wOiAtMXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1yLW4zLFxcbi5teC1uMyB7XFxuICBtYXJnaW4tcmlnaHQ6IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWItbjMsXFxuLm15LW4zIHtcXG4gIG1hcmdpbi1ib3R0b206IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWwtbjMsXFxuLm14LW4zIHtcXG4gIG1hcmdpbi1sZWZ0OiAtMXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm0tbjQge1xcbiAgbWFyZ2luOiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubXQtbjQsXFxuLm15LW40IHtcXG4gIG1hcmdpbi10b3A6IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tci1uNCxcXG4ubXgtbjQge1xcbiAgbWFyZ2luLXJpZ2h0OiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWItbjQsXFxuLm15LW40IHtcXG4gIG1hcmdpbi1ib3R0b206IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tbC1uNCxcXG4ubXgtbjQge1xcbiAgbWFyZ2luLWxlZnQ6IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tLW41IHtcXG4gIG1hcmdpbjogLTNyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tdC1uNSxcXG4ubXktbjUge1xcbiAgbWFyZ2luLXRvcDogLTNyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tci1uNSxcXG4ubXgtbjUge1xcbiAgbWFyZ2luLXJpZ2h0OiAtM3JlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1iLW41LFxcbi5teS1uNSB7XFxuICBtYXJnaW4tYm90dG9tOiAtM3JlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1sLW41LFxcbi5teC1uNSB7XFxuICBtYXJnaW4tbGVmdDogLTNyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tLWF1dG8ge1xcbiAgbWFyZ2luOiBhdXRvICFpbXBvcnRhbnQ7IH1cXG5cXG4ubXQtYXV0byxcXG4ubXktYXV0byB7XFxuICBtYXJnaW4tdG9wOiBhdXRvICFpbXBvcnRhbnQ7IH1cXG5cXG4ubXItYXV0byxcXG4ubXgtYXV0byB7XFxuICBtYXJnaW4tcmlnaHQ6IGF1dG8gIWltcG9ydGFudDsgfVxcblxcbi5tYi1hdXRvLFxcbi5teS1hdXRvIHtcXG4gIG1hcmdpbi1ib3R0b206IGF1dG8gIWltcG9ydGFudDsgfVxcblxcbi5tbC1hdXRvLFxcbi5teC1hdXRvIHtcXG4gIG1hcmdpbi1sZWZ0OiBhdXRvICFpbXBvcnRhbnQ7IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogNTc2cHgpIHtcXG4gIC5tLXNtLTAge1xcbiAgICBtYXJnaW46IDAgIWltcG9ydGFudDsgfVxcbiAgLm10LXNtLTAsXFxuICAubXktc20tMCB7XFxuICAgIG1hcmdpbi10b3A6IDAgIWltcG9ydGFudDsgfVxcbiAgLm1yLXNtLTAsXFxuICAubXgtc20tMCB7XFxuICAgIG1hcmdpbi1yaWdodDogMCAhaW1wb3J0YW50OyB9XFxuICAubWItc20tMCxcXG4gIC5teS1zbS0wIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogMCAhaW1wb3J0YW50OyB9XFxuICAubWwtc20tMCxcXG4gIC5teC1zbS0wIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDAgIWltcG9ydGFudDsgfVxcbiAgLm0tc20tMSB7XFxuICAgIG1hcmdpbjogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXQtc20tMSxcXG4gIC5teS1zbS0xIHtcXG4gICAgbWFyZ2luLXRvcDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItc20tMSxcXG4gIC5teC1zbS0xIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1zbS0xLFxcbiAgLm15LXNtLTEge1xcbiAgICBtYXJnaW4tYm90dG9tOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC1zbS0xLFxcbiAgLm14LXNtLTEge1xcbiAgICBtYXJnaW4tbGVmdDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1zbS0yIHtcXG4gICAgbWFyZ2luOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LXNtLTIsXFxuICAubXktc20tMiB7XFxuICAgIG1hcmdpbi10b3A6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItc20tMixcXG4gIC5teC1zbS0yIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLXNtLTIsXFxuICAubXktc20tMiB7XFxuICAgIG1hcmdpbi1ib3R0b206IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtc20tMixcXG4gIC5teC1zbS0yIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1zbS0zIHtcXG4gICAgbWFyZ2luOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1zbS0zLFxcbiAgLm15LXNtLTMge1xcbiAgICBtYXJnaW4tdG9wOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1zbS0zLFxcbiAgLm14LXNtLTMge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLXNtLTMsXFxuICAubXktc20tMyB7XFxuICAgIG1hcmdpbi1ib3R0b206IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXNtLTMsXFxuICAubXgtc20tMyB7XFxuICAgIG1hcmdpbi1sZWZ0OiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLXNtLTQge1xcbiAgICBtYXJnaW46IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXQtc20tNCxcXG4gIC5teS1zbS00IHtcXG4gICAgbWFyZ2luLXRvcDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1zbS00LFxcbiAgLm14LXNtLTQge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWItc20tNCxcXG4gIC5teS1zbS00IHtcXG4gICAgbWFyZ2luLWJvdHRvbTogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC1zbS00LFxcbiAgLm14LXNtLTQge1xcbiAgICBtYXJnaW4tbGVmdDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLXNtLTUge1xcbiAgICBtYXJnaW46IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LXNtLTUsXFxuICAubXktc20tNSB7XFxuICAgIG1hcmdpbi10b3A6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLXNtLTUsXFxuICAubXgtc20tNSB7XFxuICAgIG1hcmdpbi1yaWdodDogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubWItc20tNSxcXG4gIC5teS1zbS01IHtcXG4gICAgbWFyZ2luLWJvdHRvbTogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtc20tNSxcXG4gIC5teC1zbS01IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLnAtc20tMCB7XFxuICAgIHBhZGRpbmc6IDAgIWltcG9ydGFudDsgfVxcbiAgLnB0LXNtLTAsXFxuICAucHktc20tMCB7XFxuICAgIHBhZGRpbmctdG9wOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5wci1zbS0wLFxcbiAgLnB4LXNtLTAge1xcbiAgICBwYWRkaW5nLXJpZ2h0OiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5wYi1zbS0wLFxcbiAgLnB5LXNtLTAge1xcbiAgICBwYWRkaW5nLWJvdHRvbTogMCAhaW1wb3J0YW50OyB9XFxuICAucGwtc20tMCxcXG4gIC5weC1zbS0wIHtcXG4gICAgcGFkZGluZy1sZWZ0OiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5wLXNtLTEge1xcbiAgICBwYWRkaW5nOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1zbS0xLFxcbiAgLnB5LXNtLTEge1xcbiAgICBwYWRkaW5nLXRvcDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHItc20tMSxcXG4gIC5weC1zbS0xIHtcXG4gICAgcGFkZGluZy1yaWdodDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucGItc20tMSxcXG4gIC5weS1zbS0xIHtcXG4gICAgcGFkZGluZy1ib3R0b206IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBsLXNtLTEsXFxuICAucHgtc20tMSB7XFxuICAgIHBhZGRpbmctbGVmdDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucC1zbS0yIHtcXG4gICAgcGFkZGluZzogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1zbS0yLFxcbiAgLnB5LXNtLTIge1xcbiAgICBwYWRkaW5nLXRvcDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wci1zbS0yLFxcbiAgLnB4LXNtLTIge1xcbiAgICBwYWRkaW5nLXJpZ2h0OiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLXNtLTIsXFxuICAucHktc20tMiB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBsLXNtLTIsXFxuICAucHgtc20tMiB7XFxuICAgIHBhZGRpbmctbGVmdDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wLXNtLTMge1xcbiAgICBwYWRkaW5nOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1zbS0zLFxcbiAgLnB5LXNtLTMge1xcbiAgICBwYWRkaW5nLXRvcDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHItc20tMyxcXG4gIC5weC1zbS0zIHtcXG4gICAgcGFkZGluZy1yaWdodDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAucGItc20tMyxcXG4gIC5weS1zbS0zIHtcXG4gICAgcGFkZGluZy1ib3R0b206IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBsLXNtLTMsXFxuICAucHgtc20tMyB7XFxuICAgIHBhZGRpbmctbGVmdDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAucC1zbS00IHtcXG4gICAgcGFkZGluZzogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1zbS00LFxcbiAgLnB5LXNtLTQge1xcbiAgICBwYWRkaW5nLXRvcDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wci1zbS00LFxcbiAgLnB4LXNtLTQge1xcbiAgICBwYWRkaW5nLXJpZ2h0OiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLXNtLTQsXFxuICAucHktc20tNCB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBsLXNtLTQsXFxuICAucHgtc20tNCB7XFxuICAgIHBhZGRpbmctbGVmdDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wLXNtLTUge1xcbiAgICBwYWRkaW5nOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1zbS01LFxcbiAgLnB5LXNtLTUge1xcbiAgICBwYWRkaW5nLXRvcDogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAucHItc20tNSxcXG4gIC5weC1zbS01IHtcXG4gICAgcGFkZGluZy1yaWdodDogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAucGItc20tNSxcXG4gIC5weS1zbS01IHtcXG4gICAgcGFkZGluZy1ib3R0b206IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBsLXNtLTUsXFxuICAucHgtc20tNSB7XFxuICAgIHBhZGRpbmctbGVmdDogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubS1zbS1uMSB7XFxuICAgIG1hcmdpbjogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LXNtLW4xLFxcbiAgLm15LXNtLW4xIHtcXG4gICAgbWFyZ2luLXRvcDogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLXNtLW4xLFxcbiAgLm14LXNtLW4xIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWItc20tbjEsXFxuICAubXktc20tbjEge1xcbiAgICBtYXJnaW4tYm90dG9tOiAtMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtc20tbjEsXFxuICAubXgtc20tbjEge1xcbiAgICBtYXJnaW4tbGVmdDogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tc20tbjIge1xcbiAgICBtYXJnaW46IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LXNtLW4yLFxcbiAgLm15LXNtLW4yIHtcXG4gICAgbWFyZ2luLXRvcDogLTAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItc20tbjIsXFxuICAubXgtc20tbjIge1xcbiAgICBtYXJnaW4tcmlnaHQ6IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLXNtLW4yLFxcbiAgLm15LXNtLW4yIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtc20tbjIsXFxuICAubXgtc20tbjIge1xcbiAgICBtYXJnaW4tbGVmdDogLTAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1zbS1uMyB7XFxuICAgIG1hcmdpbjogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LXNtLW4zLFxcbiAgLm15LXNtLW4zIHtcXG4gICAgbWFyZ2luLXRvcDogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLXNtLW4zLFxcbiAgLm14LXNtLW4zIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWItc20tbjMsXFxuICAubXktc20tbjMge1xcbiAgICBtYXJnaW4tYm90dG9tOiAtMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtc20tbjMsXFxuICAubXgtc20tbjMge1xcbiAgICBtYXJnaW4tbGVmdDogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tc20tbjQge1xcbiAgICBtYXJnaW46IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LXNtLW40LFxcbiAgLm15LXNtLW40IHtcXG4gICAgbWFyZ2luLXRvcDogLTEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItc20tbjQsXFxuICAubXgtc20tbjQge1xcbiAgICBtYXJnaW4tcmlnaHQ6IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLXNtLW40LFxcbiAgLm15LXNtLW40IHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtc20tbjQsXFxuICAubXgtc20tbjQge1xcbiAgICBtYXJnaW4tbGVmdDogLTEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1zbS1uNSB7XFxuICAgIG1hcmdpbjogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LXNtLW41LFxcbiAgLm15LXNtLW41IHtcXG4gICAgbWFyZ2luLXRvcDogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLXNtLW41LFxcbiAgLm14LXNtLW41IHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubWItc20tbjUsXFxuICAubXktc20tbjUge1xcbiAgICBtYXJnaW4tYm90dG9tOiAtM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtc20tbjUsXFxuICAubXgtc20tbjUge1xcbiAgICBtYXJnaW4tbGVmdDogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tc20tYXV0byB7XFxuICAgIG1hcmdpbjogYXV0byAhaW1wb3J0YW50OyB9XFxuICAubXQtc20tYXV0byxcXG4gIC5teS1zbS1hdXRvIHtcXG4gICAgbWFyZ2luLXRvcDogYXV0byAhaW1wb3J0YW50OyB9XFxuICAubXItc20tYXV0byxcXG4gIC5teC1zbS1hdXRvIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiBhdXRvICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1zbS1hdXRvLFxcbiAgLm15LXNtLWF1dG8ge1xcbiAgICBtYXJnaW4tYm90dG9tOiBhdXRvICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC1zbS1hdXRvLFxcbiAgLm14LXNtLWF1dG8ge1xcbiAgICBtYXJnaW4tbGVmdDogYXV0byAhaW1wb3J0YW50OyB9IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogNzY4cHgpIHtcXG4gIC5tLW1kLTAge1xcbiAgICBtYXJnaW46IDAgIWltcG9ydGFudDsgfVxcbiAgLm10LW1kLTAsXFxuICAubXktbWQtMCB7XFxuICAgIG1hcmdpbi10b3A6IDAgIWltcG9ydGFudDsgfVxcbiAgLm1yLW1kLTAsXFxuICAubXgtbWQtMCB7XFxuICAgIG1hcmdpbi1yaWdodDogMCAhaW1wb3J0YW50OyB9XFxuICAubWItbWQtMCxcXG4gIC5teS1tZC0wIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogMCAhaW1wb3J0YW50OyB9XFxuICAubWwtbWQtMCxcXG4gIC5teC1tZC0wIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDAgIWltcG9ydGFudDsgfVxcbiAgLm0tbWQtMSB7XFxuICAgIG1hcmdpbjogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXQtbWQtMSxcXG4gIC5teS1tZC0xIHtcXG4gICAgbWFyZ2luLXRvcDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItbWQtMSxcXG4gIC5teC1tZC0xIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1tZC0xLFxcbiAgLm15LW1kLTEge1xcbiAgICBtYXJnaW4tYm90dG9tOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC1tZC0xLFxcbiAgLm14LW1kLTEge1xcbiAgICBtYXJnaW4tbGVmdDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1tZC0yIHtcXG4gICAgbWFyZ2luOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LW1kLTIsXFxuICAubXktbWQtMiB7XFxuICAgIG1hcmdpbi10b3A6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItbWQtMixcXG4gIC5teC1tZC0yIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLW1kLTIsXFxuICAubXktbWQtMiB7XFxuICAgIG1hcmdpbi1ib3R0b206IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbWQtMixcXG4gIC5teC1tZC0yIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1tZC0zIHtcXG4gICAgbWFyZ2luOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1tZC0zLFxcbiAgLm15LW1kLTMge1xcbiAgICBtYXJnaW4tdG9wOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1tZC0zLFxcbiAgLm14LW1kLTMge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLW1kLTMsXFxuICAubXktbWQtMyB7XFxuICAgIG1hcmdpbi1ib3R0b206IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLW1kLTMsXFxuICAubXgtbWQtMyB7XFxuICAgIG1hcmdpbi1sZWZ0OiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLW1kLTQge1xcbiAgICBtYXJnaW46IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXQtbWQtNCxcXG4gIC5teS1tZC00IHtcXG4gICAgbWFyZ2luLXRvcDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1tZC00LFxcbiAgLm14LW1kLTQge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWItbWQtNCxcXG4gIC5teS1tZC00IHtcXG4gICAgbWFyZ2luLWJvdHRvbTogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC1tZC00LFxcbiAgLm14LW1kLTQge1xcbiAgICBtYXJnaW4tbGVmdDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLW1kLTUge1xcbiAgICBtYXJnaW46IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LW1kLTUsXFxuICAubXktbWQtNSB7XFxuICAgIG1hcmdpbi10b3A6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLW1kLTUsXFxuICAubXgtbWQtNSB7XFxuICAgIG1hcmdpbi1yaWdodDogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubWItbWQtNSxcXG4gIC5teS1tZC01IHtcXG4gICAgbWFyZ2luLWJvdHRvbTogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbWQtNSxcXG4gIC5teC1tZC01IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLnAtbWQtMCB7XFxuICAgIHBhZGRpbmc6IDAgIWltcG9ydGFudDsgfVxcbiAgLnB0LW1kLTAsXFxuICAucHktbWQtMCB7XFxuICAgIHBhZGRpbmctdG9wOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5wci1tZC0wLFxcbiAgLnB4LW1kLTAge1xcbiAgICBwYWRkaW5nLXJpZ2h0OiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5wYi1tZC0wLFxcbiAgLnB5LW1kLTAge1xcbiAgICBwYWRkaW5nLWJvdHRvbTogMCAhaW1wb3J0YW50OyB9XFxuICAucGwtbWQtMCxcXG4gIC5weC1tZC0wIHtcXG4gICAgcGFkZGluZy1sZWZ0OiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5wLW1kLTEge1xcbiAgICBwYWRkaW5nOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1tZC0xLFxcbiAgLnB5LW1kLTEge1xcbiAgICBwYWRkaW5nLXRvcDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHItbWQtMSxcXG4gIC5weC1tZC0xIHtcXG4gICAgcGFkZGluZy1yaWdodDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucGItbWQtMSxcXG4gIC5weS1tZC0xIHtcXG4gICAgcGFkZGluZy1ib3R0b206IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBsLW1kLTEsXFxuICAucHgtbWQtMSB7XFxuICAgIHBhZGRpbmctbGVmdDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucC1tZC0yIHtcXG4gICAgcGFkZGluZzogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1tZC0yLFxcbiAgLnB5LW1kLTIge1xcbiAgICBwYWRkaW5nLXRvcDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wci1tZC0yLFxcbiAgLnB4LW1kLTIge1xcbiAgICBwYWRkaW5nLXJpZ2h0OiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLW1kLTIsXFxuICAucHktbWQtMiB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBsLW1kLTIsXFxuICAucHgtbWQtMiB7XFxuICAgIHBhZGRpbmctbGVmdDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wLW1kLTMge1xcbiAgICBwYWRkaW5nOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1tZC0zLFxcbiAgLnB5LW1kLTMge1xcbiAgICBwYWRkaW5nLXRvcDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHItbWQtMyxcXG4gIC5weC1tZC0zIHtcXG4gICAgcGFkZGluZy1yaWdodDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAucGItbWQtMyxcXG4gIC5weS1tZC0zIHtcXG4gICAgcGFkZGluZy1ib3R0b206IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBsLW1kLTMsXFxuICAucHgtbWQtMyB7XFxuICAgIHBhZGRpbmctbGVmdDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAucC1tZC00IHtcXG4gICAgcGFkZGluZzogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1tZC00LFxcbiAgLnB5LW1kLTQge1xcbiAgICBwYWRkaW5nLXRvcDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wci1tZC00LFxcbiAgLnB4LW1kLTQge1xcbiAgICBwYWRkaW5nLXJpZ2h0OiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLW1kLTQsXFxuICAucHktbWQtNCB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBsLW1kLTQsXFxuICAucHgtbWQtNCB7XFxuICAgIHBhZGRpbmctbGVmdDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wLW1kLTUge1xcbiAgICBwYWRkaW5nOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1tZC01LFxcbiAgLnB5LW1kLTUge1xcbiAgICBwYWRkaW5nLXRvcDogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAucHItbWQtNSxcXG4gIC5weC1tZC01IHtcXG4gICAgcGFkZGluZy1yaWdodDogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAucGItbWQtNSxcXG4gIC5weS1tZC01IHtcXG4gICAgcGFkZGluZy1ib3R0b206IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBsLW1kLTUsXFxuICAucHgtbWQtNSB7XFxuICAgIHBhZGRpbmctbGVmdDogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubS1tZC1uMSB7XFxuICAgIG1hcmdpbjogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LW1kLW4xLFxcbiAgLm15LW1kLW4xIHtcXG4gICAgbWFyZ2luLXRvcDogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLW1kLW4xLFxcbiAgLm14LW1kLW4xIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWItbWQtbjEsXFxuICAubXktbWQtbjEge1xcbiAgICBtYXJnaW4tYm90dG9tOiAtMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbWQtbjEsXFxuICAubXgtbWQtbjEge1xcbiAgICBtYXJnaW4tbGVmdDogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbWQtbjIge1xcbiAgICBtYXJnaW46IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LW1kLW4yLFxcbiAgLm15LW1kLW4yIHtcXG4gICAgbWFyZ2luLXRvcDogLTAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItbWQtbjIsXFxuICAubXgtbWQtbjIge1xcbiAgICBtYXJnaW4tcmlnaHQ6IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLW1kLW4yLFxcbiAgLm15LW1kLW4yIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbWQtbjIsXFxuICAubXgtbWQtbjIge1xcbiAgICBtYXJnaW4tbGVmdDogLTAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1tZC1uMyB7XFxuICAgIG1hcmdpbjogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LW1kLW4zLFxcbiAgLm15LW1kLW4zIHtcXG4gICAgbWFyZ2luLXRvcDogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLW1kLW4zLFxcbiAgLm14LW1kLW4zIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWItbWQtbjMsXFxuICAubXktbWQtbjMge1xcbiAgICBtYXJnaW4tYm90dG9tOiAtMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbWQtbjMsXFxuICAubXgtbWQtbjMge1xcbiAgICBtYXJnaW4tbGVmdDogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbWQtbjQge1xcbiAgICBtYXJnaW46IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LW1kLW40LFxcbiAgLm15LW1kLW40IHtcXG4gICAgbWFyZ2luLXRvcDogLTEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItbWQtbjQsXFxuICAubXgtbWQtbjQge1xcbiAgICBtYXJnaW4tcmlnaHQ6IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLW1kLW40LFxcbiAgLm15LW1kLW40IHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbWQtbjQsXFxuICAubXgtbWQtbjQge1xcbiAgICBtYXJnaW4tbGVmdDogLTEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1tZC1uNSB7XFxuICAgIG1hcmdpbjogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LW1kLW41LFxcbiAgLm15LW1kLW41IHtcXG4gICAgbWFyZ2luLXRvcDogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLW1kLW41LFxcbiAgLm14LW1kLW41IHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubWItbWQtbjUsXFxuICAubXktbWQtbjUge1xcbiAgICBtYXJnaW4tYm90dG9tOiAtM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbWQtbjUsXFxuICAubXgtbWQtbjUge1xcbiAgICBtYXJnaW4tbGVmdDogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbWQtYXV0byB7XFxuICAgIG1hcmdpbjogYXV0byAhaW1wb3J0YW50OyB9XFxuICAubXQtbWQtYXV0byxcXG4gIC5teS1tZC1hdXRvIHtcXG4gICAgbWFyZ2luLXRvcDogYXV0byAhaW1wb3J0YW50OyB9XFxuICAubXItbWQtYXV0byxcXG4gIC5teC1tZC1hdXRvIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiBhdXRvICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1tZC1hdXRvLFxcbiAgLm15LW1kLWF1dG8ge1xcbiAgICBtYXJnaW4tYm90dG9tOiBhdXRvICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC1tZC1hdXRvLFxcbiAgLm14LW1kLWF1dG8ge1xcbiAgICBtYXJnaW4tbGVmdDogYXV0byAhaW1wb3J0YW50OyB9IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogOTkycHgpIHtcXG4gIC5tLWxnLTAge1xcbiAgICBtYXJnaW46IDAgIWltcG9ydGFudDsgfVxcbiAgLm10LWxnLTAsXFxuICAubXktbGctMCB7XFxuICAgIG1hcmdpbi10b3A6IDAgIWltcG9ydGFudDsgfVxcbiAgLm1yLWxnLTAsXFxuICAubXgtbGctMCB7XFxuICAgIG1hcmdpbi1yaWdodDogMCAhaW1wb3J0YW50OyB9XFxuICAubWItbGctMCxcXG4gIC5teS1sZy0wIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogMCAhaW1wb3J0YW50OyB9XFxuICAubWwtbGctMCxcXG4gIC5teC1sZy0wIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDAgIWltcG9ydGFudDsgfVxcbiAgLm0tbGctMSB7XFxuICAgIG1hcmdpbjogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXQtbGctMSxcXG4gIC5teS1sZy0xIHtcXG4gICAgbWFyZ2luLXRvcDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItbGctMSxcXG4gIC5teC1sZy0xIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1sZy0xLFxcbiAgLm15LWxnLTEge1xcbiAgICBtYXJnaW4tYm90dG9tOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC1sZy0xLFxcbiAgLm14LWxnLTEge1xcbiAgICBtYXJnaW4tbGVmdDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1sZy0yIHtcXG4gICAgbWFyZ2luOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LWxnLTIsXFxuICAubXktbGctMiB7XFxuICAgIG1hcmdpbi10b3A6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItbGctMixcXG4gIC5teC1sZy0yIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLWxnLTIsXFxuICAubXktbGctMiB7XFxuICAgIG1hcmdpbi1ib3R0b206IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbGctMixcXG4gIC5teC1sZy0yIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1sZy0zIHtcXG4gICAgbWFyZ2luOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1sZy0zLFxcbiAgLm15LWxnLTMge1xcbiAgICBtYXJnaW4tdG9wOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1sZy0zLFxcbiAgLm14LWxnLTMge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLWxnLTMsXFxuICAubXktbGctMyB7XFxuICAgIG1hcmdpbi1ib3R0b206IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLWxnLTMsXFxuICAubXgtbGctMyB7XFxuICAgIG1hcmdpbi1sZWZ0OiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLWxnLTQge1xcbiAgICBtYXJnaW46IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXQtbGctNCxcXG4gIC5teS1sZy00IHtcXG4gICAgbWFyZ2luLXRvcDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1sZy00LFxcbiAgLm14LWxnLTQge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWItbGctNCxcXG4gIC5teS1sZy00IHtcXG4gICAgbWFyZ2luLWJvdHRvbTogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC1sZy00LFxcbiAgLm14LWxnLTQge1xcbiAgICBtYXJnaW4tbGVmdDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLWxnLTUge1xcbiAgICBtYXJnaW46IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LWxnLTUsXFxuICAubXktbGctNSB7XFxuICAgIG1hcmdpbi10b3A6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLWxnLTUsXFxuICAubXgtbGctNSB7XFxuICAgIG1hcmdpbi1yaWdodDogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubWItbGctNSxcXG4gIC5teS1sZy01IHtcXG4gICAgbWFyZ2luLWJvdHRvbTogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbGctNSxcXG4gIC5teC1sZy01IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLnAtbGctMCB7XFxuICAgIHBhZGRpbmc6IDAgIWltcG9ydGFudDsgfVxcbiAgLnB0LWxnLTAsXFxuICAucHktbGctMCB7XFxuICAgIHBhZGRpbmctdG9wOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5wci1sZy0wLFxcbiAgLnB4LWxnLTAge1xcbiAgICBwYWRkaW5nLXJpZ2h0OiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5wYi1sZy0wLFxcbiAgLnB5LWxnLTAge1xcbiAgICBwYWRkaW5nLWJvdHRvbTogMCAhaW1wb3J0YW50OyB9XFxuICAucGwtbGctMCxcXG4gIC5weC1sZy0wIHtcXG4gICAgcGFkZGluZy1sZWZ0OiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5wLWxnLTEge1xcbiAgICBwYWRkaW5nOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1sZy0xLFxcbiAgLnB5LWxnLTEge1xcbiAgICBwYWRkaW5nLXRvcDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHItbGctMSxcXG4gIC5weC1sZy0xIHtcXG4gICAgcGFkZGluZy1yaWdodDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucGItbGctMSxcXG4gIC5weS1sZy0xIHtcXG4gICAgcGFkZGluZy1ib3R0b206IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBsLWxnLTEsXFxuICAucHgtbGctMSB7XFxuICAgIHBhZGRpbmctbGVmdDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucC1sZy0yIHtcXG4gICAgcGFkZGluZzogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1sZy0yLFxcbiAgLnB5LWxnLTIge1xcbiAgICBwYWRkaW5nLXRvcDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wci1sZy0yLFxcbiAgLnB4LWxnLTIge1xcbiAgICBwYWRkaW5nLXJpZ2h0OiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLWxnLTIsXFxuICAucHktbGctMiB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBsLWxnLTIsXFxuICAucHgtbGctMiB7XFxuICAgIHBhZGRpbmctbGVmdDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wLWxnLTMge1xcbiAgICBwYWRkaW5nOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1sZy0zLFxcbiAgLnB5LWxnLTMge1xcbiAgICBwYWRkaW5nLXRvcDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHItbGctMyxcXG4gIC5weC1sZy0zIHtcXG4gICAgcGFkZGluZy1yaWdodDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAucGItbGctMyxcXG4gIC5weS1sZy0zIHtcXG4gICAgcGFkZGluZy1ib3R0b206IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBsLWxnLTMsXFxuICAucHgtbGctMyB7XFxuICAgIHBhZGRpbmctbGVmdDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAucC1sZy00IHtcXG4gICAgcGFkZGluZzogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1sZy00LFxcbiAgLnB5LWxnLTQge1xcbiAgICBwYWRkaW5nLXRvcDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wci1sZy00LFxcbiAgLnB4LWxnLTQge1xcbiAgICBwYWRkaW5nLXJpZ2h0OiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLWxnLTQsXFxuICAucHktbGctNCB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBsLWxnLTQsXFxuICAucHgtbGctNCB7XFxuICAgIHBhZGRpbmctbGVmdDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wLWxnLTUge1xcbiAgICBwYWRkaW5nOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1sZy01LFxcbiAgLnB5LWxnLTUge1xcbiAgICBwYWRkaW5nLXRvcDogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAucHItbGctNSxcXG4gIC5weC1sZy01IHtcXG4gICAgcGFkZGluZy1yaWdodDogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAucGItbGctNSxcXG4gIC5weS1sZy01IHtcXG4gICAgcGFkZGluZy1ib3R0b206IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBsLWxnLTUsXFxuICAucHgtbGctNSB7XFxuICAgIHBhZGRpbmctbGVmdDogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubS1sZy1uMSB7XFxuICAgIG1hcmdpbjogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LWxnLW4xLFxcbiAgLm15LWxnLW4xIHtcXG4gICAgbWFyZ2luLXRvcDogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLWxnLW4xLFxcbiAgLm14LWxnLW4xIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWItbGctbjEsXFxuICAubXktbGctbjEge1xcbiAgICBtYXJnaW4tYm90dG9tOiAtMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbGctbjEsXFxuICAubXgtbGctbjEge1xcbiAgICBtYXJnaW4tbGVmdDogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbGctbjIge1xcbiAgICBtYXJnaW46IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LWxnLW4yLFxcbiAgLm15LWxnLW4yIHtcXG4gICAgbWFyZ2luLXRvcDogLTAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItbGctbjIsXFxuICAubXgtbGctbjIge1xcbiAgICBtYXJnaW4tcmlnaHQ6IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLWxnLW4yLFxcbiAgLm15LWxnLW4yIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbGctbjIsXFxuICAubXgtbGctbjIge1xcbiAgICBtYXJnaW4tbGVmdDogLTAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1sZy1uMyB7XFxuICAgIG1hcmdpbjogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LWxnLW4zLFxcbiAgLm15LWxnLW4zIHtcXG4gICAgbWFyZ2luLXRvcDogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLWxnLW4zLFxcbiAgLm14LWxnLW4zIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWItbGctbjMsXFxuICAubXktbGctbjMge1xcbiAgICBtYXJnaW4tYm90dG9tOiAtMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbGctbjMsXFxuICAubXgtbGctbjMge1xcbiAgICBtYXJnaW4tbGVmdDogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbGctbjQge1xcbiAgICBtYXJnaW46IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LWxnLW40LFxcbiAgLm15LWxnLW40IHtcXG4gICAgbWFyZ2luLXRvcDogLTEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItbGctbjQsXFxuICAubXgtbGctbjQge1xcbiAgICBtYXJnaW4tcmlnaHQ6IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLWxnLW40LFxcbiAgLm15LWxnLW40IHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbGctbjQsXFxuICAubXgtbGctbjQge1xcbiAgICBtYXJnaW4tbGVmdDogLTEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1sZy1uNSB7XFxuICAgIG1hcmdpbjogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LWxnLW41LFxcbiAgLm15LWxnLW41IHtcXG4gICAgbWFyZ2luLXRvcDogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLWxnLW41LFxcbiAgLm14LWxnLW41IHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubWItbGctbjUsXFxuICAubXktbGctbjUge1xcbiAgICBtYXJnaW4tYm90dG9tOiAtM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbGctbjUsXFxuICAubXgtbGctbjUge1xcbiAgICBtYXJnaW4tbGVmdDogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbGctYXV0byB7XFxuICAgIG1hcmdpbjogYXV0byAhaW1wb3J0YW50OyB9XFxuICAubXQtbGctYXV0byxcXG4gIC5teS1sZy1hdXRvIHtcXG4gICAgbWFyZ2luLXRvcDogYXV0byAhaW1wb3J0YW50OyB9XFxuICAubXItbGctYXV0byxcXG4gIC5teC1sZy1hdXRvIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiBhdXRvICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1sZy1hdXRvLFxcbiAgLm15LWxnLWF1dG8ge1xcbiAgICBtYXJnaW4tYm90dG9tOiBhdXRvICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC1sZy1hdXRvLFxcbiAgLm14LWxnLWF1dG8ge1xcbiAgICBtYXJnaW4tbGVmdDogYXV0byAhaW1wb3J0YW50OyB9IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogMTIwMHB4KSB7XFxuICAubS14bC0wIHtcXG4gICAgbWFyZ2luOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC14bC0wLFxcbiAgLm15LXhsLTAge1xcbiAgICBtYXJnaW4tdG9wOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5tci14bC0wLFxcbiAgLm14LXhsLTAge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDAgIWltcG9ydGFudDsgfVxcbiAgLm1iLXhsLTAsXFxuICAubXkteGwtMCB7XFxuICAgIG1hcmdpbi1ib3R0b206IDAgIWltcG9ydGFudDsgfVxcbiAgLm1sLXhsLTAsXFxuICAubXgteGwtMCB7XFxuICAgIG1hcmdpbi1sZWZ0OiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5tLXhsLTEge1xcbiAgICBtYXJnaW46IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LXhsLTEsXFxuICAubXkteGwtMSB7XFxuICAgIG1hcmdpbi10b3A6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLXhsLTEsXFxuICAubXgteGwtMSB7XFxuICAgIG1hcmdpbi1yaWdodDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWIteGwtMSxcXG4gIC5teS14bC0xIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwteGwtMSxcXG4gIC5teC14bC0xIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0teGwtMiB7XFxuICAgIG1hcmdpbjogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC14bC0yLFxcbiAgLm15LXhsLTIge1xcbiAgICBtYXJnaW4tdG9wOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLXhsLTIsXFxuICAubXgteGwtMiB7XFxuICAgIG1hcmdpbi1yaWdodDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi14bC0yLFxcbiAgLm15LXhsLTIge1xcbiAgICBtYXJnaW4tYm90dG9tOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXhsLTIsXFxuICAubXgteGwtMiB7XFxuICAgIG1hcmdpbi1sZWZ0OiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0teGwtMyB7XFxuICAgIG1hcmdpbjogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXQteGwtMyxcXG4gIC5teS14bC0zIHtcXG4gICAgbWFyZ2luLXRvcDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXIteGwtMyxcXG4gIC5teC14bC0zIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi14bC0zLFxcbiAgLm15LXhsLTMge1xcbiAgICBtYXJnaW4tYm90dG9tOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC14bC0zLFxcbiAgLm14LXhsLTMge1xcbiAgICBtYXJnaW4tbGVmdDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS14bC00IHtcXG4gICAgbWFyZ2luOiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LXhsLTQsXFxuICAubXkteGwtNCB7XFxuICAgIG1hcmdpbi10b3A6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXIteGwtNCxcXG4gIC5teC14bC00IHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLXhsLTQsXFxuICAubXkteGwtNCB7XFxuICAgIG1hcmdpbi1ib3R0b206IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwteGwtNCxcXG4gIC5teC14bC00IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS14bC01IHtcXG4gICAgbWFyZ2luOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC14bC01LFxcbiAgLm15LXhsLTUge1xcbiAgICBtYXJnaW4tdG9wOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci14bC01LFxcbiAgLm14LXhsLTUge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLXhsLTUsXFxuICAubXkteGwtNSB7XFxuICAgIG1hcmdpbi1ib3R0b206IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXhsLTUsXFxuICAubXgteGwtNSB7XFxuICAgIG1hcmdpbi1sZWZ0OiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wLXhsLTAge1xcbiAgICBwYWRkaW5nOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC14bC0wLFxcbiAgLnB5LXhsLTAge1xcbiAgICBwYWRkaW5nLXRvcDogMCAhaW1wb3J0YW50OyB9XFxuICAucHIteGwtMCxcXG4gIC5weC14bC0wIHtcXG4gICAgcGFkZGluZy1yaWdodDogMCAhaW1wb3J0YW50OyB9XFxuICAucGIteGwtMCxcXG4gIC5weS14bC0wIHtcXG4gICAgcGFkZGluZy1ib3R0b206IDAgIWltcG9ydGFudDsgfVxcbiAgLnBsLXhsLTAsXFxuICAucHgteGwtMCB7XFxuICAgIHBhZGRpbmctbGVmdDogMCAhaW1wb3J0YW50OyB9XFxuICAucC14bC0xIHtcXG4gICAgcGFkZGluZzogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQteGwtMSxcXG4gIC5weS14bC0xIHtcXG4gICAgcGFkZGluZy10b3A6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnByLXhsLTEsXFxuICAucHgteGwtMSB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLXhsLTEsXFxuICAucHkteGwtMSB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC14bC0xLFxcbiAgLnB4LXhsLTEge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnAteGwtMiB7XFxuICAgIHBhZGRpbmc6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQteGwtMixcXG4gIC5weS14bC0yIHtcXG4gICAgcGFkZGluZy10b3A6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHIteGwtMixcXG4gIC5weC14bC0yIHtcXG4gICAgcGFkZGluZy1yaWdodDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wYi14bC0yLFxcbiAgLnB5LXhsLTIge1xcbiAgICBwYWRkaW5nLWJvdHRvbTogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC14bC0yLFxcbiAgLnB4LXhsLTIge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucC14bC0zIHtcXG4gICAgcGFkZGluZzogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQteGwtMyxcXG4gIC5weS14bC0zIHtcXG4gICAgcGFkZGluZy10b3A6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnByLXhsLTMsXFxuICAucHgteGwtMyB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLXhsLTMsXFxuICAucHkteGwtMyB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC14bC0zLFxcbiAgLnB4LXhsLTMge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnAteGwtNCB7XFxuICAgIHBhZGRpbmc6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQteGwtNCxcXG4gIC5weS14bC00IHtcXG4gICAgcGFkZGluZy10b3A6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHIteGwtNCxcXG4gIC5weC14bC00IHtcXG4gICAgcGFkZGluZy1yaWdodDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wYi14bC00LFxcbiAgLnB5LXhsLTQge1xcbiAgICBwYWRkaW5nLWJvdHRvbTogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC14bC00LFxcbiAgLnB4LXhsLTQge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucC14bC01IHtcXG4gICAgcGFkZGluZzogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAucHQteGwtNSxcXG4gIC5weS14bC01IHtcXG4gICAgcGFkZGluZy10b3A6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLnByLXhsLTUsXFxuICAucHgteGwtNSB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLXhsLTUsXFxuICAucHkteGwtNSB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC14bC01LFxcbiAgLnB4LXhsLTUge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0teGwtbjEge1xcbiAgICBtYXJnaW46IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC14bC1uMSxcXG4gIC5teS14bC1uMSB7XFxuICAgIG1hcmdpbi10b3A6IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci14bC1uMSxcXG4gIC5teC14bC1uMSB7XFxuICAgIG1hcmdpbi1yaWdodDogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLXhsLW4xLFxcbiAgLm15LXhsLW4xIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXhsLW4xLFxcbiAgLm14LXhsLW4xIHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLXhsLW4yIHtcXG4gICAgbWFyZ2luOiAtMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC14bC1uMixcXG4gIC5teS14bC1uMiB7XFxuICAgIG1hcmdpbi10b3A6IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLXhsLW4yLFxcbiAgLm14LXhsLW4yIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi14bC1uMixcXG4gIC5teS14bC1uMiB7XFxuICAgIG1hcmdpbi1ib3R0b206IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXhsLW4yLFxcbiAgLm14LXhsLW4yIHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0teGwtbjMge1xcbiAgICBtYXJnaW46IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC14bC1uMyxcXG4gIC5teS14bC1uMyB7XFxuICAgIG1hcmdpbi10b3A6IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci14bC1uMyxcXG4gIC5teC14bC1uMyB7XFxuICAgIG1hcmdpbi1yaWdodDogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLXhsLW4zLFxcbiAgLm15LXhsLW4zIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXhsLW4zLFxcbiAgLm14LXhsLW4zIHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLXhsLW40IHtcXG4gICAgbWFyZ2luOiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC14bC1uNCxcXG4gIC5teS14bC1uNCB7XFxuICAgIG1hcmdpbi10b3A6IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLXhsLW40LFxcbiAgLm14LXhsLW40IHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi14bC1uNCxcXG4gIC5teS14bC1uNCB7XFxuICAgIG1hcmdpbi1ib3R0b206IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXhsLW40LFxcbiAgLm14LXhsLW40IHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0teGwtbjUge1xcbiAgICBtYXJnaW46IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC14bC1uNSxcXG4gIC5teS14bC1uNSB7XFxuICAgIG1hcmdpbi10b3A6IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci14bC1uNSxcXG4gIC5teC14bC1uNSB7XFxuICAgIG1hcmdpbi1yaWdodDogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLXhsLW41LFxcbiAgLm15LXhsLW41IHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXhsLW41LFxcbiAgLm14LXhsLW41IHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLXhsLWF1dG8ge1xcbiAgICBtYXJnaW46IGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLm10LXhsLWF1dG8sXFxuICAubXkteGwtYXV0byB7XFxuICAgIG1hcmdpbi10b3A6IGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLm1yLXhsLWF1dG8sXFxuICAubXgteGwtYXV0byB7XFxuICAgIG1hcmdpbi1yaWdodDogYXV0byAhaW1wb3J0YW50OyB9XFxuICAubWIteGwtYXV0byxcXG4gIC5teS14bC1hdXRvIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogYXV0byAhaW1wb3J0YW50OyB9XFxuICAubWwteGwtYXV0byxcXG4gIC5teC14bC1hdXRvIHtcXG4gICAgbWFyZ2luLWxlZnQ6IGF1dG8gIWltcG9ydGFudDsgfSB9XFxuXFxuLnRleHQtbW9ub3NwYWNlIHtcXG4gIGZvbnQtZmFtaWx5OiBTRk1vbm8tUmVndWxhciwgTWVubG8sIE1vbmFjbywgQ29uc29sYXMsIFxcXCJMaWJlcmF0aW9uIE1vbm9cXFwiLCBcXFwiQ291cmllciBOZXdcXFwiLCBtb25vc3BhY2UgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LWp1c3RpZnkge1xcbiAgdGV4dC1hbGlnbjoganVzdGlmeSAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtd3JhcCB7XFxuICB3aGl0ZS1zcGFjZTogbm9ybWFsICFpbXBvcnRhbnQ7IH1cXG5cXG4udGV4dC1ub3dyYXAge1xcbiAgd2hpdGUtc3BhY2U6IG5vd3JhcCAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtdHJ1bmNhdGUge1xcbiAgb3ZlcmZsb3c6IGhpZGRlbjtcXG4gIHRleHQtb3ZlcmZsb3c6IGVsbGlwc2lzO1xcbiAgd2hpdGUtc3BhY2U6IG5vd3JhcDsgfVxcblxcbi50ZXh0LWxlZnQge1xcbiAgdGV4dC1hbGlnbjogbGVmdCAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtcmlnaHQge1xcbiAgdGV4dC1hbGlnbjogcmlnaHQgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LWNlbnRlciB7XFxuICB0ZXh0LWFsaWduOiBjZW50ZXIgIWltcG9ydGFudDsgfVxcblxcbkBtZWRpYSAobWluLXdpZHRoOiA1NzZweCkge1xcbiAgLnRleHQtc20tbGVmdCB7XFxuICAgIHRleHQtYWxpZ246IGxlZnQgIWltcG9ydGFudDsgfVxcbiAgLnRleHQtc20tcmlnaHQge1xcbiAgICB0ZXh0LWFsaWduOiByaWdodCAhaW1wb3J0YW50OyB9XFxuICAudGV4dC1zbS1jZW50ZXIge1xcbiAgICB0ZXh0LWFsaWduOiBjZW50ZXIgIWltcG9ydGFudDsgfSB9XFxuXFxuQG1lZGlhIChtaW4td2lkdGg6IDc2OHB4KSB7XFxuICAudGV4dC1tZC1sZWZ0IHtcXG4gICAgdGV4dC1hbGlnbjogbGVmdCAhaW1wb3J0YW50OyB9XFxuICAudGV4dC1tZC1yaWdodCB7XFxuICAgIHRleHQtYWxpZ246IHJpZ2h0ICFpbXBvcnRhbnQ7IH1cXG4gIC50ZXh0LW1kLWNlbnRlciB7XFxuICAgIHRleHQtYWxpZ246IGNlbnRlciAhaW1wb3J0YW50OyB9IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogOTkycHgpIHtcXG4gIC50ZXh0LWxnLWxlZnQge1xcbiAgICB0ZXh0LWFsaWduOiBsZWZ0ICFpbXBvcnRhbnQ7IH1cXG4gIC50ZXh0LWxnLXJpZ2h0IHtcXG4gICAgdGV4dC1hbGlnbjogcmlnaHQgIWltcG9ydGFudDsgfVxcbiAgLnRleHQtbGctY2VudGVyIHtcXG4gICAgdGV4dC1hbGlnbjogY2VudGVyICFpbXBvcnRhbnQ7IH0gfVxcblxcbkBtZWRpYSAobWluLXdpZHRoOiAxMjAwcHgpIHtcXG4gIC50ZXh0LXhsLWxlZnQge1xcbiAgICB0ZXh0LWFsaWduOiBsZWZ0ICFpbXBvcnRhbnQ7IH1cXG4gIC50ZXh0LXhsLXJpZ2h0IHtcXG4gICAgdGV4dC1hbGlnbjogcmlnaHQgIWltcG9ydGFudDsgfVxcbiAgLnRleHQteGwtY2VudGVyIHtcXG4gICAgdGV4dC1hbGlnbjogY2VudGVyICFpbXBvcnRhbnQ7IH0gfVxcblxcbi50ZXh0LWxvd2VyY2FzZSB7XFxuICB0ZXh0LXRyYW5zZm9ybTogbG93ZXJjYXNlICFpbXBvcnRhbnQ7IH1cXG5cXG4udGV4dC11cHBlcmNhc2Uge1xcbiAgdGV4dC10cmFuc2Zvcm06IHVwcGVyY2FzZSAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtY2FwaXRhbGl6ZSB7XFxuICB0ZXh0LXRyYW5zZm9ybTogY2FwaXRhbGl6ZSAhaW1wb3J0YW50OyB9XFxuXFxuLmZvbnQtd2VpZ2h0LWxpZ2h0IHtcXG4gIGZvbnQtd2VpZ2h0OiAzMDAgIWltcG9ydGFudDsgfVxcblxcbi5mb250LXdlaWdodC1saWdodGVyIHtcXG4gIGZvbnQtd2VpZ2h0OiBsaWdodGVyICFpbXBvcnRhbnQ7IH1cXG5cXG4uZm9udC13ZWlnaHQtbm9ybWFsIHtcXG4gIGZvbnQtd2VpZ2h0OiA0MDAgIWltcG9ydGFudDsgfVxcblxcbi5mb250LXdlaWdodC1ib2xkIHtcXG4gIGZvbnQtd2VpZ2h0OiA3MDAgIWltcG9ydGFudDsgfVxcblxcbi5mb250LXdlaWdodC1ib2xkZXIge1xcbiAgZm9udC13ZWlnaHQ6IGJvbGRlciAhaW1wb3J0YW50OyB9XFxuXFxuLmZvbnQtaXRhbGljIHtcXG4gIGZvbnQtc3R5bGU6IGl0YWxpYyAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtd2hpdGUge1xcbiAgY29sb3I6ICNmZmYgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LXByaW1hcnkge1xcbiAgY29sb3I6ICMwMDdiZmYgIWltcG9ydGFudDsgfVxcblxcbmEudGV4dC1wcmltYXJ5OmhvdmVyLCBhLnRleHQtcHJpbWFyeTpmb2N1cyB7XFxuICBjb2xvcjogIzAwNTZiMyAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtc2Vjb25kYXJ5IHtcXG4gIGNvbG9yOiAjNmM3NTdkICFpbXBvcnRhbnQ7IH1cXG5cXG5hLnRleHQtc2Vjb25kYXJ5OmhvdmVyLCBhLnRleHQtc2Vjb25kYXJ5OmZvY3VzIHtcXG4gIGNvbG9yOiAjNDk0ZjU0ICFpbXBvcnRhbnQ7IH1cXG5cXG4udGV4dC1zdWNjZXNzIHtcXG4gIGNvbG9yOiAjMjhhNzQ1ICFpbXBvcnRhbnQ7IH1cXG5cXG5hLnRleHQtc3VjY2Vzczpob3ZlciwgYS50ZXh0LXN1Y2Nlc3M6Zm9jdXMge1xcbiAgY29sb3I6ICMxOTY5MmMgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LWluZm8ge1xcbiAgY29sb3I6ICMxN2EyYjggIWltcG9ydGFudDsgfVxcblxcbmEudGV4dC1pbmZvOmhvdmVyLCBhLnRleHQtaW5mbzpmb2N1cyB7XFxuICBjb2xvcjogIzBmNjY3NCAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtd2FybmluZyB7XFxuICBjb2xvcjogI2ZmYzEwNyAhaW1wb3J0YW50OyB9XFxuXFxuYS50ZXh0LXdhcm5pbmc6aG92ZXIsIGEudGV4dC13YXJuaW5nOmZvY3VzIHtcXG4gIGNvbG9yOiAjYmE4YjAwICFpbXBvcnRhbnQ7IH1cXG5cXG4udGV4dC1kYW5nZXIge1xcbiAgY29sb3I6ICNkYzM1NDUgIWltcG9ydGFudDsgfVxcblxcbmEudGV4dC1kYW5nZXI6aG92ZXIsIGEudGV4dC1kYW5nZXI6Zm9jdXMge1xcbiAgY29sb3I6ICNhNzFkMmEgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LWxpZ2h0IHtcXG4gIGNvbG9yOiAjZjhmOWZhICFpbXBvcnRhbnQ7IH1cXG5cXG5hLnRleHQtbGlnaHQ6aG92ZXIsIGEudGV4dC1saWdodDpmb2N1cyB7XFxuICBjb2xvcjogI2NiZDNkYSAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtZGFyayB7XFxuICBjb2xvcjogIzM0M2E0MCAhaW1wb3J0YW50OyB9XFxuXFxuYS50ZXh0LWRhcms6aG92ZXIsIGEudGV4dC1kYXJrOmZvY3VzIHtcXG4gIGNvbG9yOiAjMTIxNDE2ICFpbXBvcnRhbnQ7IH1cXG5cXG4udGV4dC1ib2R5IHtcXG4gIGNvbG9yOiAjMjEyNTI5ICFpbXBvcnRhbnQ7IH1cXG5cXG4udGV4dC1tdXRlZCB7XFxuICBjb2xvcjogIzZjNzU3ZCAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtYmxhY2stNTAge1xcbiAgY29sb3I6IHJnYmEoMCwgMCwgMCwgMC41KSAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtd2hpdGUtNTAge1xcbiAgY29sb3I6IHJnYmEoMjU1LCAyNTUsIDI1NSwgMC41KSAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtaGlkZSB7XFxuICBmb250OiAwLzAgYTtcXG4gIGNvbG9yOiB0cmFuc3BhcmVudDtcXG4gIHRleHQtc2hhZG93OiBub25lO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogdHJhbnNwYXJlbnQ7XFxuICBib3JkZXI6IDA7IH1cXG5cXG4udGV4dC1kZWNvcmF0aW9uLW5vbmUge1xcbiAgdGV4dC1kZWNvcmF0aW9uOiBub25lICFpbXBvcnRhbnQ7IH1cXG5cXG4udGV4dC1icmVhayB7XFxuICB3b3JkLWJyZWFrOiBicmVhay13b3JkICFpbXBvcnRhbnQ7XFxuICBvdmVyZmxvdy13cmFwOiBicmVhay13b3JkICFpbXBvcnRhbnQ7IH1cXG5cXG4udGV4dC1yZXNldCB7XFxuICBjb2xvcjogaW5oZXJpdCAhaW1wb3J0YW50OyB9XFxuXFxuLmJnLXByaW1hcnkge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogIzAwN2JmZiAhaW1wb3J0YW50OyB9XFxuXFxuYS5iZy1wcmltYXJ5OmhvdmVyLCBhLmJnLXByaW1hcnk6Zm9jdXMsXFxuYnV0dG9uLmJnLXByaW1hcnk6aG92ZXIsXFxuYnV0dG9uLmJnLXByaW1hcnk6Zm9jdXMge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogIzAwNjJjYyAhaW1wb3J0YW50OyB9XFxuXFxuLmJnLXNlY29uZGFyeSB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjNmM3NTdkICFpbXBvcnRhbnQ7IH1cXG5cXG5hLmJnLXNlY29uZGFyeTpob3ZlciwgYS5iZy1zZWNvbmRhcnk6Zm9jdXMsXFxuYnV0dG9uLmJnLXNlY29uZGFyeTpob3ZlcixcXG5idXR0b24uYmctc2Vjb25kYXJ5OmZvY3VzIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICM1NDViNjIgIWltcG9ydGFudDsgfVxcblxcbi5iZy1zdWNjZXNzIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMyOGE3NDUgIWltcG9ydGFudDsgfVxcblxcbmEuYmctc3VjY2Vzczpob3ZlciwgYS5iZy1zdWNjZXNzOmZvY3VzLFxcbmJ1dHRvbi5iZy1zdWNjZXNzOmhvdmVyLFxcbmJ1dHRvbi5iZy1zdWNjZXNzOmZvY3VzIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMxZTdlMzQgIWltcG9ydGFudDsgfVxcblxcbi5iZy1pbmZvIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMxN2EyYjggIWltcG9ydGFudDsgfVxcblxcbmEuYmctaW5mbzpob3ZlciwgYS5iZy1pbmZvOmZvY3VzLFxcbmJ1dHRvbi5iZy1pbmZvOmhvdmVyLFxcbmJ1dHRvbi5iZy1pbmZvOmZvY3VzIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMxMTdhOGIgIWltcG9ydGFudDsgfVxcblxcbi5iZy13YXJuaW5nIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNmZmMxMDcgIWltcG9ydGFudDsgfVxcblxcbmEuYmctd2FybmluZzpob3ZlciwgYS5iZy13YXJuaW5nOmZvY3VzLFxcbmJ1dHRvbi5iZy13YXJuaW5nOmhvdmVyLFxcbmJ1dHRvbi5iZy13YXJuaW5nOmZvY3VzIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNkMzllMDAgIWltcG9ydGFudDsgfVxcblxcbi5iZy1kYW5nZXIge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2RjMzU0NSAhaW1wb3J0YW50OyB9XFxuXFxuYS5iZy1kYW5nZXI6aG92ZXIsIGEuYmctZGFuZ2VyOmZvY3VzLFxcbmJ1dHRvbi5iZy1kYW5nZXI6aG92ZXIsXFxuYnV0dG9uLmJnLWRhbmdlcjpmb2N1cyB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjYmQyMTMwICFpbXBvcnRhbnQ7IH1cXG5cXG4uYmctbGlnaHQge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2Y4ZjlmYSAhaW1wb3J0YW50OyB9XFxuXFxuYS5iZy1saWdodDpob3ZlciwgYS5iZy1saWdodDpmb2N1cyxcXG5idXR0b24uYmctbGlnaHQ6aG92ZXIsXFxuYnV0dG9uLmJnLWxpZ2h0OmZvY3VzIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNkYWUwZTUgIWltcG9ydGFudDsgfVxcblxcbi5iZy1kYXJrIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMzNDNhNDAgIWltcG9ydGFudDsgfVxcblxcbmEuYmctZGFyazpob3ZlciwgYS5iZy1kYXJrOmZvY3VzLFxcbmJ1dHRvbi5iZy1kYXJrOmhvdmVyLFxcbmJ1dHRvbi5iZy1kYXJrOmZvY3VzIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMxZDIxMjQgIWltcG9ydGFudDsgfVxcblxcbi5iZy13aGl0ZSB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmZmICFpbXBvcnRhbnQ7IH1cXG5cXG4uYmctdHJhbnNwYXJlbnQge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogdHJhbnNwYXJlbnQgIWltcG9ydGFudDsgfVxcblwiLCBcIlwiXSk7XG5cbiIsImV4cG9ydHMgPSBtb2R1bGUuZXhwb3J0cyA9IHJlcXVpcmUoXCIuLi8uLi9ub2RlX21vZHVsZXMvY3NzLWxvYWRlci9kaXN0L3J1bnRpbWUvYXBpLmpzXCIpKGZhbHNlKTtcbi8vIEltcG9ydHNcbmV4cG9ydHMuaShyZXF1aXJlKFwiLSEuLi8uLi9ub2RlX21vZHVsZXMvY3NzLWxvYWRlci9kaXN0L2Nqcy5qcyFoaWdobGlnaHQuanMvc3R5bGVzL2RlZmF1bHQuY3NzXCIpLCBcIlwiKTtcblxuLy8gTW9kdWxlXG5leHBvcnRzLnB1c2goW21vZHVsZS5pZCwgXCJcXG5cIiwgXCJcIl0pO1xuXG4iLCJpbXBvcnQgeyBjc3MsIENTU1Jlc3VsdCwgdW5zYWZlQ1NTIH0gZnJvbSAnbGl0LWVsZW1lbnQnO1xyXG5jb25zdCBib290c3RyYXBDc3MgPSByZXF1aXJlKCcuL2Jvb3RzdHJhcC5zY3NzJyk7XHJcbmNvbnN0IGhpZ2hsaWdodGpzQ3NzID0gcmVxdWlyZSgnLi9oaWdobGlnaHRqcy5zY3NzJyk7XHJcblxyXG5leHBvcnQgY29uc3QgYm9vdHN0cmFwOiBDU1NSZXN1bHQgPSBjc3NgJHt1bnNhZmVDU1MoYm9vdHN0cmFwQ3NzLnRvU3RyaW5nKCkpfWA7XHJcbmV4cG9ydCBjb25zdCBoaWdobGlnaHRKUzogQ1NTUmVzdWx0ID0gY3NzYCR7dW5zYWZlQ1NTKGhpZ2hsaWdodGpzQ3NzLnRvU3RyaW5nKCkpfWA7XHJcbiJdLCJzb3VyY2VSb290IjoiIn0=