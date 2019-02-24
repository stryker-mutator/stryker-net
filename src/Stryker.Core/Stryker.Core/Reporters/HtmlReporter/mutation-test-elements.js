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

/***/ "./node_modules/lit-html/directives/unsafe-html.js":
/*!*********************************************************!*\
  !*** ./node_modules/lit-html/directives/unsafe-html.js ***!
  \*********************************************************/
/*! exports provided: unsafeHTML */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "unsafeHTML", function() { return unsafeHTML; });
/* harmony import */ var _lib_parts_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../lib/parts.js */ "./node_modules/lit-html/lib/parts.js");
/* harmony import */ var _lit_html_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../lit-html.js */ "./node_modules/lit-html/lit-html.js");
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


// For each part, remember the value that was last rendered to the part by the
// unsafeHTML directive, and the DocumentFragment that was last set as a value.
// The DocumentFragment is used as a unique key to check if the last value
// rendered to the part was with unsafeHTML. If not, we'll always re-render the
// value passed to unsafeHTML.
const previousValues = new WeakMap();
/**
 * Renders the result as HTML, rather than text.
 *
 * Note, this is unsafe to use with any user-provided input that hasn't been
 * sanitized or escaped, as it may lead to cross-site-scripting
 * vulnerabilities.
 */
const unsafeHTML = Object(_lit_html_js__WEBPACK_IMPORTED_MODULE_1__["directive"])((value) => (part) => {
    if (!(part instanceof _lit_html_js__WEBPACK_IMPORTED_MODULE_1__["NodePart"])) {
        throw new Error('unsafeHTML can only be used in text bindings');
    }
    const previousValue = previousValues.get(part);
    if (previousValue !== undefined && Object(_lib_parts_js__WEBPACK_IMPORTED_MODULE_0__["isPrimitive"])(value) &&
        value === previousValue.value && part.value === previousValue.fragment) {
        return;
    }
    const template = document.createElement('template');
    template.innerHTML = value; // innerHTML casts to string internally
    const fragment = document.importNode(template.content, true);
    part.setValue(fragment);
    previousValues.set(part, { value, fragment });
});
//# sourceMappingURL=unsafe-html.js.map

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
    get title() {
        if (this.context && this.path) {
            return this.path;
        }
        else {
            return helpers_1.ROOT_NAME;
        }
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
            const report = yield res.json();
            report.files = helpers_1.normalizeFileNames(report.files);
            this.report = report;
            this.updateContext();
        });
    }
    updateContext() {
        if (this.report) {
            if (this.path) {
                this.context = this.report.files[this.path];
                if (!this.context) {
                    this.errorMessage = `404 - ${this.path} not found`;
                }
                else {
                    this.errorMessage = undefined;
                }
            }
            else {
                this.context = undefined;
            }
        }
    }
    render() {
        return lit_element_1.html `
    <mutation-test-report-title .title="${this.title}"></mutation-test-report-title>
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
        if (this.report) {
            return lit_element_1.html `<h1 class="display-4">${this.title}</h1>`;
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
        if (this.context && this.report) {
            return lit_element_1.html `<mutation-test-report-file .name="${this.title}" .thresholds="${this.report.thresholds}" .model="${this.context}"></mutation-test-report-file>`;
        }
        else if (this.report) {
            return lit_element_1.html `<mutation-test-report-result .model="${this.report}"></mutation-test-report-result>`;
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
], MutationTestReportAppComponent.prototype, "report", void 0);
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
__decorate([
    lit_element_1.property()
], MutationTestReportAppComponent.prototype, "title", null);
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
const helpers_1 = __webpack_require__(/*! ../helpers */ "./src/helpers.ts");
let MutationTestReportBreadcrumbComponent = class MutationTestReportBreadcrumbComponent extends lit_element_1.LitElement {
    render() {
        return lit_element_1.html `
        <ol class='breadcrumb'>
          ${this.renderRootItem()}
          ${this.path ? this.renderBreadcrumbItems([this.path]) : ''}
        </ol>
    `;
    }
    renderRootItem() {
        if (this.path && this.path.length) {
            return this.renderLink(helpers_1.ROOT_NAME, '#');
        }
        else {
            return this.renderActiveItem(helpers_1.ROOT_NAME);
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
            .filter(status => this.mutants.some(mutant => mutant.status === status))
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
          <button @click="${this.toggleOpenAll}" class="btn btn-sm btn-secondary" type="button">${this.collapseButtonText}</button>
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
const unsafe_html_1 = __webpack_require__(/*! lit-html/directives/unsafe-html */ "./node_modules/lit-html/directives/unsafe-html.js");
const highlight_1 = __importDefault(__webpack_require__(/*! highlight.js/lib/highlight */ "./node_modules/highlight.js/lib/highlight.js"));
const javascript_1 = __importDefault(__webpack_require__(/*! highlight.js/lib/languages/javascript */ "./node_modules/highlight.js/lib/languages/javascript.js"));
const scala_1 = __importDefault(__webpack_require__(/*! highlight.js/lib/languages/scala */ "./node_modules/highlight.js/lib/languages/scala.js"));
const java_1 = __importDefault(__webpack_require__(/*! highlight.js/lib/languages/java */ "./node_modules/highlight.js/lib/languages/java.js"));
const cs_1 = __importDefault(__webpack_require__(/*! highlight.js/lib/languages/cs */ "./node_modules/highlight.js/lib/languages/cs.js"));
const typescript_1 = __importDefault(__webpack_require__(/*! highlight.js/lib/languages/typescript */ "./node_modules/highlight.js/lib/languages/typescript.js"));
const helpers_1 = __webpack_require__(/*! ../helpers */ "./src/helpers.ts");
const mutation_test_report_mutant_1 = __webpack_require__(/*! ./mutation-test-report-mutant */ "./src/components/mutation-test-report-mutant.ts");
const style_1 = __webpack_require__(/*! ../style */ "./src/style/index.ts");
const ResultTable_1 = __webpack_require__(/*! ../model/ResultTable */ "./src/model/ResultTable.ts");
highlight_1.default.registerLanguage('javascript', javascript_1.default);
highlight_1.default.registerLanguage('typescript', typescript_1.default);
highlight_1.default.registerLanguage('cs', cs_1.default);
highlight_1.default.registerLanguage('java', java_1.default);
highlight_1.default.registerLanguage('scala', scala_1.default);
let MutationTestReportFileComponent = class MutationTestReportFileComponent extends lit_element_1.LitElement {
    constructor() {
        super(...arguments);
        this.enabledMutantStates = [];
        this.openAll = () => {
            this.forEachMutantComponent(mutantComponent => mutantComponent.expand = true);
        };
        this.collapseAll = () => {
            this.forEachMutantComponent(mutantComponent => mutantComponent.expand = false);
        };
        this.filtersChanged = (event) => {
            this.enabledMutantStates = event.detail
                .filter(mutantFilter => mutantFilter.enabled)
                .map(mutantFilter => mutantFilter.status);
            this.updateShownMutants();
        };
    }
    forEachMutantComponent(action, host = this.root) {
        for (const mutantComponent of host.querySelectorAll('mutation-test-report-mutant')) {
            if (mutantComponent instanceof mutation_test_report_mutant_1.MutationTestReportMutantComponent) {
                action(mutantComponent);
            }
        }
    }
    updateShownMutants() {
        this.forEachMutantComponent(mutantComponent => {
            mutantComponent.show = this.enabledMutantStates.some(state => mutantComponent.mutant !== undefined && mutantComponent.mutant.status === state);
        });
    }
    render() {
        return lit_element_1.html `
        <div class="row">
          <div class="totals col-sm-11">
            <mutation-test-report-totals .model="${ResultTable_1.ResultTable.forFile(this.name, this.model, this.thresholds)}"></mutation-test-report-totals>
          </div>
        </div>
        <div class="row">
          <div class="col-md-12">
            <mutation-test-report-file-legend @filters-changed="${this.filtersChanged}" @open-all="${this.openAll}"
              @collapse-all="${this.collapseAll}" .mutants="${this.model.mutants}"></mutation-test-report-file-legend>
            <pre><code class="lang-${this.model.language} hljs">${unsafe_html_1.unsafeHTML(this.renderCode())}</code></pre>
          </div>
        </div>
        `;
    }
    firstUpdated(_changedProperties) {
        super.firstUpdated(_changedProperties);
        const code = this.root.querySelector('code');
        if (code) {
            highlight_1.default.highlightBlock(code);
            this.forEachMutantComponent(mutantComponent => {
                mutantComponent.mutant = this.model.mutants.find(mutant => mutant.id.toString() === mutantComponent.getAttribute('mutant-id'));
            }, code);
        }
    }
    get root() {
        return this.shadowRoot || this;
    }
    /**
     * Walks over the code in this.model.source and adds the
     * `<mutation-test-report-mutant>` elements.
     * It also adds the background color using
     * `<span class="bg-danger-light">` and friends.
     */
    renderCode() {
        const backgroundState = new BackgroundColorCalculator();
        const startedMutants = [];
        const walker = (char, pos) => {
            const currentMutants = this.model.mutants.filter(m => eq(m.location.start, pos));
            const mutantsEnding = startedMutants.filter(m => gte(pos, m.location.end));
            mutantsEnding.forEach(mutant => startedMutants.splice(startedMutants.indexOf(mutant), 1));
            startedMutants.push(...currentMutants);
            const builder = [];
            if (currentMutants.length || mutantsEnding.length) {
                currentMutants.forEach(backgroundState.markMutantStart);
                mutantsEnding.forEach(backgroundState.markMutantEnd);
                // End previous color span
                builder.push('</span>');
                // End mutants
                mutantsEnding.forEach(() => builder.push('</mutation-test-report-mutant>'));
                // Start mutants
                currentMutants.forEach(mutant => builder.push(`<mutation-test-report-mutant mutant-id="${mutant.id}">`));
                // Start new color span
                builder.push(`<span class="bg-${backgroundState.determineBackground()}">`);
            }
            // Append the code character
            builder.push(helpers_1.escapeHtml(char));
            return builder.join('');
        };
        return `<span>${walkString(this.model.source, walker)}</span>`;
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
__decorate([
    lit_element_1.property()
], MutationTestReportFileComponent.prototype, "name", void 0);
__decorate([
    lit_element_1.property()
], MutationTestReportFileComponent.prototype, "thresholds", void 0);
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
    let column = helpers_1.COLUMN_START_INDEX;
    let line = helpers_1.LINE_START_INDEX;
    const builder = [];
    for (const currentChar of source) {
        if (column === helpers_1.COLUMN_START_INDEX && currentChar === helpers_1.CARRIAGE_RETURN) {
            continue;
        }
        if (currentChar === helpers_1.NEW_LINE) {
            line++;
            column = helpers_1.COLUMN_START_INDEX;
            builder.push(helpers_1.NEW_LINE);
            continue;
        }
        builder.push(fn(currentChar, { line, column: column++ }));
    }
    return builder.join('');
}
/**
 * Class to keep track of the states of the
 * mutants that are active at the cursor while walking the code.
 */
class BackgroundColorCalculator {
    constructor() {
        this.killed = 0;
        this.noCoverage = 0;
        this.survived = 0;
        this.timeout = 0;
        this.markMutantStart = (mutant) => {
            this.countMutant(1, mutant.status);
        };
        this.markMutantEnd = (mutant) => {
            this.countMutant(-1, mutant.status);
        };
        this.determineBackground = () => {
            if (this.survived > 0) {
                return helpers_1.getContextClassForStatus("Survived" /* Survived */) + '-light';
            }
            else if (this.noCoverage > 0) {
                return helpers_1.getContextClassForStatus("NoCoverage" /* NoCoverage */) + '-light';
            }
            else if (this.timeout > 0) {
                return helpers_1.getContextClassForStatus("Timeout" /* Timeout */) + '-light';
            }
            else if (this.killed > 0) {
                return helpers_1.getContextClassForStatus("Killed" /* Killed */) + '-light';
            }
            return null;
        };
    }
    countMutant(valueToAdd, status) {
        switch (status) {
            case "Killed" /* Killed */:
                this.killed += valueToAdd;
                break;
            case "Survived" /* Survived */:
                this.survived += valueToAdd;
                break;
            case "Timeout" /* Timeout */:
                this.timeout += valueToAdd;
                break;
            case "NoCoverage" /* NoCoverage */:
                this.noCoverage += valueToAdd;
                break;
        }
    }
}
function gte(a, b) {
    return a.line > b.line || (a.line === b.line && a.column >= b.column);
}
function eq(a, b) {
    return a.line === b.line && a.column === b.column;
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
        this.expand = false;
    }
    render() {
        // This part is newline significant, as it is rendered in a <code> block.
        // No unnecessary new lines
        return lit_element_1.html `${this.renderButton()}${this.renderCode()}`;
    }
    renderButton() {
        if (this.show && this.mutant) {
            return lit_element_1.html `<span class="badge badge-${this.expand ? 'info' : helpers_1.getContextClassForStatus(this.mutant.status)}" @click="${() => this.expand = !this.expand}"
  title="${this.mutant.mutatorName}">${this.mutant.id}</button>`;
        }
        else {
            return undefined;
        }
    }
    renderCode() {
        return lit_element_1.html `${this.renderReplacement()}${this.renderActual()}`;
    }
    renderActual() {
        const actualCodeSlot = lit_element_1.html `<slot></slot>`;
        return lit_element_1.html `<span class="${this.expand && this.show ? 'disabled-code' : ''}">${actualCodeSlot}</span>`;
    }
    renderReplacement() {
        if (this.mutant) {
            return lit_element_1.html `<span class="badge badge-info" ?hidden="${!this.expand || !this.show}">${this.mutant.replacement}</span>`;
        }
        else {
            return undefined;
        }
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
], MutationTestReportMutantComponent.prototype, "mutant", void 0);
__decorate([
    lit_element_1.property()
], MutationTestReportMutantComponent.prototype, "show", void 0);
__decorate([
    lit_element_1.property()
], MutationTestReportMutantComponent.prototype, "expand", void 0);
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
const style_1 = __webpack_require__(/*! ../style */ "./src/style/index.ts");
const ResultTable_1 = __webpack_require__(/*! ../model/ResultTable */ "./src/model/ResultTable.ts");
const helpers_1 = __webpack_require__(/*! ../helpers */ "./src/helpers.ts");
let MutationTestReportResultComponent = class MutationTestReportResultComponent extends lit_element_1.LitElement {
    render() {
        const rows = [
            new ResultTable_1.TableRow(helpers_1.ROOT_NAME, helpers_1.flatMap(Object.keys(this.model.files), key => this.model.files[key].mutants), false),
            ...Object.keys(this.model.files).map(fileName => new ResultTable_1.TableRow(fileName, this.model.files[fileName].mutants, true))
        ];
        return lit_element_1.html `
    <div class='row'>
      <div class='totals col-sm-11'>
        <mutation-test-report-totals .model="${new ResultTable_1.ResultTable(rows, this.model.thresholds)}"></mutation-test-report-totals>
      </div>
    </div>
    `;
    }
};
MutationTestReportResultComponent.styles = [style_1.bootstrap];
__decorate([
    lit_element_1.property()
], MutationTestReportResultComponent.prototype, "model", void 0);
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
            const path = window.location.hash.substr(1);
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

/***/ "./src/components/mutation-test-report-title.ts":
/*!******************************************************!*\
  !*** ./src/components/mutation-test-report-title.ts ***!
  \******************************************************/
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
    set title(value) {
        document.title = value;
    }
};
__decorate([
    lit_element_1.property()
], MutationTestReportRouterComponent.prototype, "title", null);
MutationTestReportRouterComponent = __decorate([
    lit_element_1.customElement('mutation-test-report-title')
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
const style_1 = __webpack_require__(/*! ../style */ "./src/style/index.ts");
let MutationTestReportTotalsComponent = class MutationTestReportTotalsComponent extends lit_element_1.LitElement {
    constructor() {
        super(...arguments);
        this.renderRow = (row) => {
            const mutationScoreRounded = row.mutationScore.toFixed(2);
            const coloringClass = this.determineColoringClass(row.mutationScore);
            const style = `width: ${mutationScoreRounded}%`;
            return lit_element_1.html `
    <tr>
      <td>${row.shouldLink ? lit_element_1.html `<a href="${this.link(row.name)}">${row.name}</a>` : lit_element_1.html `<span>${row.name}</span>`}</td>
      <td>
        <div class="progress">
          <div class="progress-bar bg-${coloringClass}" role="progressbar" aria-valuenow="${mutationScoreRounded}"
            aria-valuemin="0" aria-valuemax="100" .style="${style}">
            ${mutationScoreRounded}%
          </div>
        </div>
      </td>
      <th class="text-center text-${coloringClass}">${mutationScoreRounded}</th>
      <td class="text-center">${row.killed}</td>
      <td class="text-center">${row.survived}</td>
      <td class="text-center">${row.timeout}</td>
      <td class="text-center">${row.noCoverage}</td>
      <td class="text-center">${row.runtimeErrors}</td>
      <td class="text-center">${row.compileErrors}</td>
      <th class="text-center">${row.totalDetected}</th>
      <th class="text-center">${row.totalUndetected}</th>
      <th class="text-center">${row.totalMutants}</th>
    </tr>
    `;
        };
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
    <th style="width: 20%">
      <div><span>File / Directory</span></div>
    </th>
    <th colspan="2">
      <div><span>Mutation score</span></div>
    </th>
    <th class="rotate text-center" style="width: 50px">
      <div><span># Killed</span></div>
    </th>
    <th class="rotate text-center" style="width: 50px">
      <div><span># Survived</span></div>
    </th>
    <th class="rotate text-center" style="width: 50px">
      <div><span># Timeout</span></div>
    </th>
    <th class="rotate text-center" style="width: 50px">
      <div><span># No coverage</span></div>
    </th>
    <th class="rotate text-center" style="width: 50px">
      <div><span># Runtime errors</span></div>
    </th>
    <th class="rotate text-center" style="width: 50px">
      <div><span># Transpile errors</span></div>
    </th>
    <th class="rotate rotate-width-70 text-center" style="width: 70px">
      <div><span>Total detected</span></div>
    </th>
    <th class="rotate rotate-width-70 text-center" style="width: 70px">
      <div><span>Total undetected</span></div>
    </th>
    <th class="rotate rotate-width-70 text-center" style="width: 70px">
      <div><span>Total mutants</span></div>
    </th>
  </tr>
</thead>`;
    }
    renderBody() {
        return lit_element_1.html `
    <tbody>
      ${this.model.rows.map(this.renderRow)}
    </tbody>`;
    }
    link(to) {
        return `#${to}`;
    }
    determineColoringClass(score) {
        if (score < this.model.thresholds.low) {
            return 'danger';
        }
        else if (score < this.model.thresholds.high) {
            return 'warning';
        }
        else {
            return 'success';
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
exports.ROOT_NAME = 'All files';
const SEPARATOR = '/';
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
function flatMap(source, fn) {
    const result = [];
    source.map(fn).forEach(items => result.push(...items));
    return result;
}
exports.flatMap = flatMap;
function normalizeFileNames(files) {
    const fileNames = Object.keys(files);
    const commonBasePath = determineCommonBasePath(fileNames);
    const result = {};
    fileNames.forEach(fileName => {
        result[normalize(fileName.substr(commonBasePath.length))] = files[fileName];
    });
    return result;
}
exports.normalizeFileNames = normalizeFileNames;
function normalize(fileName) {
    return fileName.split(/\/|\\/)
        .filter(pathPart => pathPart)
        .join('/');
}
function determineCommonBasePath(fileNames) {
    const directories = fileNames.map(fileName => fileName.split(/\/|\\/).slice(0, -1));
    if (directories.length) {
        return directories.reduce(filterDirectories).join(SEPARATOR);
    }
    else {
        return '';
    }
}
function filterDirectories(previousDirectories, currentDirectories) {
    for (let i = 0; i < previousDirectories.length; i++) {
        if (previousDirectories[i] !== currentDirectories[i]) {
            return previousDirectories.splice(0, i);
        }
    }
    return previousDirectories;
}


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
__webpack_require__(/*! ./components/mutation-test-report-title */ "./src/components/mutation-test-report-title.ts");
__webpack_require__(/*! ./components/mutation-test-report-router */ "./src/components/mutation-test-report-router.ts");
__webpack_require__(/*! ./components/mutation-test-report-mutant */ "./src/components/mutation-test-report-mutant.ts");
__webpack_require__(/*! ./components/mutation-test-report-file-legend */ "./src/components/mutation-test-report-file-legend.ts");


/***/ }),

/***/ "./src/model/ResultTable.ts":
/*!**********************************!*\
  !*** ./src/model/ResultTable.ts ***!
  \**********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
const DEFAULT_IF_NO_VALID_MUTANTS = 100;
class TableRow {
    constructor(name, results, shouldLink) {
        this.name = name;
        this.shouldLink = shouldLink;
        const count = (mutantResult) => results.filter(mutant => mutant.status === mutantResult).length;
        this.killed = count("Killed" /* Killed */);
        this.timeout = count("Timeout" /* Timeout */);
        this.survived = count("Survived" /* Survived */);
        this.noCoverage = count("NoCoverage" /* NoCoverage */);
        this.runtimeErrors = count("RuntimeError" /* RuntimeError */);
        this.compileErrors = count("CompileError" /* CompileError */);
        this.totalDetected = this.timeout + this.killed;
        this.totalUndetected = this.survived + this.noCoverage;
        this.totalCovered = this.totalDetected + this.survived;
        this.totalValid = this.totalUndetected + this.totalDetected;
        this.totalInvalid = this.runtimeErrors + this.compileErrors;
        this.totalMutants = this.totalValid + this.totalInvalid;
        this.mutationScore = this.totalValid > 0 ? this.totalDetected / this.totalValid * 100 : DEFAULT_IF_NO_VALID_MUTANTS;
        this.mutationScoreBasedOnCoveredCode = this.totalValid > 0 ? this.totalDetected / this.totalCovered * 100 || 0 : DEFAULT_IF_NO_VALID_MUTANTS;
    }
}
exports.TableRow = TableRow;
class ResultTable {
    constructor(rows, thresholds) {
        this.rows = rows;
        this.thresholds = thresholds;
    }
    static forFile(name, result, thresholds) {
        return new ResultTable([
            new TableRow(name, result.mutants, false)
        ], thresholds);
    }
}
exports.ResultTable = ResultTable;


/***/ }),

/***/ "./src/style/bootstrap.scss":
/*!**********************************!*\
  !*** ./src/style/bootstrap.scss ***!
  \**********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(/*! ../../node_modules/css-loader/dist/runtime/api.js */ "./node_modules/css-loader/dist/runtime/api.js")(false);
// Module
exports.push([module.i, "/*!\n * Bootstrap Reboot v4.3.1 (https://getbootstrap.com/)\n * Copyright 2011-2019 The Bootstrap Authors\n * Copyright 2011-2019 Twitter, Inc.\n * Licensed under MIT (https://github.com/twbs/bootstrap/blob/master/LICENSE)\n * Forked from Normalize.css, licensed MIT (https://github.com/necolas/normalize.css/blob/master/LICENSE.md)\n */\n*,\n*::before,\n*::after {\n  box-sizing: border-box; }\n\nhtml {\n  font-family: sans-serif;\n  line-height: 1.15;\n  -webkit-text-size-adjust: 100%;\n  -webkit-tap-highlight-color: rgba(0, 0, 0, 0); }\n\narticle, aside, figcaption, figure, footer, header, hgroup, main, nav, section {\n  display: block; }\n\nbody {\n  margin: 0;\n  font-family: -apple-system, BlinkMacSystemFont, \"Segoe UI\", Roboto, \"Helvetica Neue\", Arial, \"Noto Sans\", sans-serif, \"Apple Color Emoji\", \"Segoe UI Emoji\", \"Segoe UI Symbol\", \"Noto Color Emoji\";\n  font-size: 1rem;\n  font-weight: 400;\n  line-height: 1.5;\n  color: #212529;\n  text-align: left;\n  background-color: #fff; }\n\n[tabindex=\"-1\"]:focus {\n  outline: 0 !important; }\n\nhr {\n  box-sizing: content-box;\n  height: 0;\n  overflow: visible; }\n\nh1, h2, h3, h4, h5, h6 {\n  margin-top: 0;\n  margin-bottom: 0.5rem; }\n\np {\n  margin-top: 0;\n  margin-bottom: 1rem; }\n\nabbr[title],\nabbr[data-original-title] {\n  text-decoration: underline;\n  text-decoration: underline dotted;\n  cursor: help;\n  border-bottom: 0;\n  text-decoration-skip-ink: none; }\n\naddress {\n  margin-bottom: 1rem;\n  font-style: normal;\n  line-height: inherit; }\n\nol,\nul,\ndl {\n  margin-top: 0;\n  margin-bottom: 1rem; }\n\nol ol,\nul ul,\nol ul,\nul ol {\n  margin-bottom: 0; }\n\ndt {\n  font-weight: 700; }\n\ndd {\n  margin-bottom: .5rem;\n  margin-left: 0; }\n\nblockquote {\n  margin: 0 0 1rem; }\n\nb,\nstrong {\n  font-weight: bolder; }\n\nsmall {\n  font-size: 80%; }\n\nsub,\nsup {\n  position: relative;\n  font-size: 75%;\n  line-height: 0;\n  vertical-align: baseline; }\n\nsub {\n  bottom: -.25em; }\n\nsup {\n  top: -.5em; }\n\na {\n  color: #007bff;\n  text-decoration: none;\n  background-color: transparent; }\n  a:hover {\n    color: #0056b3;\n    text-decoration: underline; }\n\na:not([href]):not([tabindex]) {\n  color: inherit;\n  text-decoration: none; }\n  a:not([href]):not([tabindex]):hover, a:not([href]):not([tabindex]):focus {\n    color: inherit;\n    text-decoration: none; }\n  a:not([href]):not([tabindex]):focus {\n    outline: 0; }\n\npre,\ncode,\nkbd,\nsamp {\n  font-family: SFMono-Regular, Menlo, Monaco, Consolas, \"Liberation Mono\", \"Courier New\", monospace;\n  font-size: 1em; }\n\npre {\n  margin-top: 0;\n  margin-bottom: 1rem;\n  overflow: auto; }\n\nfigure {\n  margin: 0 0 1rem; }\n\nimg {\n  vertical-align: middle;\n  border-style: none; }\n\nsvg {\n  overflow: hidden;\n  vertical-align: middle; }\n\ntable {\n  border-collapse: collapse; }\n\ncaption {\n  padding-top: 0.75rem;\n  padding-bottom: 0.75rem;\n  color: #6c757d;\n  text-align: left;\n  caption-side: bottom; }\n\nth {\n  text-align: inherit; }\n\nlabel {\n  display: inline-block;\n  margin-bottom: 0.5rem; }\n\nbutton {\n  border-radius: 0; }\n\nbutton:focus {\n  outline: 1px dotted;\n  outline: 5px auto -webkit-focus-ring-color; }\n\ninput,\nbutton,\nselect,\noptgroup,\ntextarea {\n  margin: 0;\n  font-family: inherit;\n  font-size: inherit;\n  line-height: inherit; }\n\nbutton,\ninput {\n  overflow: visible; }\n\nbutton,\nselect {\n  text-transform: none; }\n\nselect {\n  word-wrap: normal; }\n\nbutton,\n[type=\"button\"],\n[type=\"reset\"],\n[type=\"submit\"] {\n  -webkit-appearance: button; }\n\nbutton:not(:disabled),\n[type=\"button\"]:not(:disabled),\n[type=\"reset\"]:not(:disabled),\n[type=\"submit\"]:not(:disabled) {\n  cursor: pointer; }\n\nbutton::-moz-focus-inner,\n[type=\"button\"]::-moz-focus-inner,\n[type=\"reset\"]::-moz-focus-inner,\n[type=\"submit\"]::-moz-focus-inner {\n  padding: 0;\n  border-style: none; }\n\ninput[type=\"radio\"],\ninput[type=\"checkbox\"] {\n  box-sizing: border-box;\n  padding: 0; }\n\ninput[type=\"date\"],\ninput[type=\"time\"],\ninput[type=\"datetime-local\"],\ninput[type=\"month\"] {\n  -webkit-appearance: listbox; }\n\ntextarea {\n  overflow: auto;\n  resize: vertical; }\n\nfieldset {\n  min-width: 0;\n  padding: 0;\n  margin: 0;\n  border: 0; }\n\nlegend {\n  display: block;\n  width: 100%;\n  max-width: 100%;\n  padding: 0;\n  margin-bottom: .5rem;\n  font-size: 1.5rem;\n  line-height: inherit;\n  color: inherit;\n  white-space: normal; }\n\nprogress {\n  vertical-align: baseline; }\n\n[type=\"number\"]::-webkit-inner-spin-button,\n[type=\"number\"]::-webkit-outer-spin-button {\n  height: auto; }\n\n[type=\"search\"] {\n  outline-offset: -2px;\n  -webkit-appearance: none; }\n\n[type=\"search\"]::-webkit-search-decoration {\n  -webkit-appearance: none; }\n\n::-webkit-file-upload-button {\n  font: inherit;\n  -webkit-appearance: button; }\n\noutput {\n  display: inline-block; }\n\nsummary {\n  display: list-item;\n  cursor: pointer; }\n\ntemplate {\n  display: none; }\n\n[hidden] {\n  display: none !important; }\n\n.container {\n  width: 100%;\n  padding-right: 15px;\n  padding-left: 15px;\n  margin-right: auto;\n  margin-left: auto; }\n  @media (min-width: 576px) {\n    .container {\n      max-width: 540px; } }\n  @media (min-width: 768px) {\n    .container {\n      max-width: 720px; } }\n  @media (min-width: 992px) {\n    .container {\n      max-width: 960px; } }\n  @media (min-width: 1200px) {\n    .container {\n      max-width: 1140px; } }\n\n.container-fluid {\n  width: 100%;\n  padding-right: 15px;\n  padding-left: 15px;\n  margin-right: auto;\n  margin-left: auto; }\n\n.row {\n  display: flex;\n  flex-wrap: wrap;\n  margin-right: -15px;\n  margin-left: -15px; }\n\n.no-gutters {\n  margin-right: 0;\n  margin-left: 0; }\n  .no-gutters > .col,\n  .no-gutters > [class*=\"col-\"] {\n    padding-right: 0;\n    padding-left: 0; }\n\n.col-1, .col-2, .col-3, .col-4, .col-5, .col-6, .col-7, .col-8, .col-9, .col-10, .col-11, .col-12, .col,\n.col-auto, .col-sm-1, .col-sm-2, .col-sm-3, .col-sm-4, .col-sm-5, .col-sm-6, .col-sm-7, .col-sm-8, .col-sm-9, .col-sm-10, .col-sm-11, .col-sm-12, .col-sm,\n.col-sm-auto, .col-md-1, .col-md-2, .col-md-3, .col-md-4, .col-md-5, .col-md-6, .col-md-7, .col-md-8, .col-md-9, .col-md-10, .col-md-11, .col-md-12, .col-md,\n.col-md-auto, .col-lg-1, .col-lg-2, .col-lg-3, .col-lg-4, .col-lg-5, .col-lg-6, .col-lg-7, .col-lg-8, .col-lg-9, .col-lg-10, .col-lg-11, .col-lg-12, .col-lg,\n.col-lg-auto, .col-xl-1, .col-xl-2, .col-xl-3, .col-xl-4, .col-xl-5, .col-xl-6, .col-xl-7, .col-xl-8, .col-xl-9, .col-xl-10, .col-xl-11, .col-xl-12, .col-xl,\n.col-xl-auto {\n  position: relative;\n  width: 100%;\n  padding-right: 15px;\n  padding-left: 15px; }\n\n.col {\n  flex-basis: 0;\n  flex-grow: 1;\n  max-width: 100%; }\n\n.col-auto {\n  flex: 0 0 auto;\n  width: auto;\n  max-width: 100%; }\n\n.col-1 {\n  flex: 0 0 8.33333%;\n  max-width: 8.33333%; }\n\n.col-2 {\n  flex: 0 0 16.66667%;\n  max-width: 16.66667%; }\n\n.col-3 {\n  flex: 0 0 25%;\n  max-width: 25%; }\n\n.col-4 {\n  flex: 0 0 33.33333%;\n  max-width: 33.33333%; }\n\n.col-5 {\n  flex: 0 0 41.66667%;\n  max-width: 41.66667%; }\n\n.col-6 {\n  flex: 0 0 50%;\n  max-width: 50%; }\n\n.col-7 {\n  flex: 0 0 58.33333%;\n  max-width: 58.33333%; }\n\n.col-8 {\n  flex: 0 0 66.66667%;\n  max-width: 66.66667%; }\n\n.col-9 {\n  flex: 0 0 75%;\n  max-width: 75%; }\n\n.col-10 {\n  flex: 0 0 83.33333%;\n  max-width: 83.33333%; }\n\n.col-11 {\n  flex: 0 0 91.66667%;\n  max-width: 91.66667%; }\n\n.col-12 {\n  flex: 0 0 100%;\n  max-width: 100%; }\n\n.order-first {\n  order: -1; }\n\n.order-last {\n  order: 13; }\n\n.order-0 {\n  order: 0; }\n\n.order-1 {\n  order: 1; }\n\n.order-2 {\n  order: 2; }\n\n.order-3 {\n  order: 3; }\n\n.order-4 {\n  order: 4; }\n\n.order-5 {\n  order: 5; }\n\n.order-6 {\n  order: 6; }\n\n.order-7 {\n  order: 7; }\n\n.order-8 {\n  order: 8; }\n\n.order-9 {\n  order: 9; }\n\n.order-10 {\n  order: 10; }\n\n.order-11 {\n  order: 11; }\n\n.order-12 {\n  order: 12; }\n\n.offset-1 {\n  margin-left: 8.33333%; }\n\n.offset-2 {\n  margin-left: 16.66667%; }\n\n.offset-3 {\n  margin-left: 25%; }\n\n.offset-4 {\n  margin-left: 33.33333%; }\n\n.offset-5 {\n  margin-left: 41.66667%; }\n\n.offset-6 {\n  margin-left: 50%; }\n\n.offset-7 {\n  margin-left: 58.33333%; }\n\n.offset-8 {\n  margin-left: 66.66667%; }\n\n.offset-9 {\n  margin-left: 75%; }\n\n.offset-10 {\n  margin-left: 83.33333%; }\n\n.offset-11 {\n  margin-left: 91.66667%; }\n\n@media (min-width: 576px) {\n  .col-sm {\n    flex-basis: 0;\n    flex-grow: 1;\n    max-width: 100%; }\n  .col-sm-auto {\n    flex: 0 0 auto;\n    width: auto;\n    max-width: 100%; }\n  .col-sm-1 {\n    flex: 0 0 8.33333%;\n    max-width: 8.33333%; }\n  .col-sm-2 {\n    flex: 0 0 16.66667%;\n    max-width: 16.66667%; }\n  .col-sm-3 {\n    flex: 0 0 25%;\n    max-width: 25%; }\n  .col-sm-4 {\n    flex: 0 0 33.33333%;\n    max-width: 33.33333%; }\n  .col-sm-5 {\n    flex: 0 0 41.66667%;\n    max-width: 41.66667%; }\n  .col-sm-6 {\n    flex: 0 0 50%;\n    max-width: 50%; }\n  .col-sm-7 {\n    flex: 0 0 58.33333%;\n    max-width: 58.33333%; }\n  .col-sm-8 {\n    flex: 0 0 66.66667%;\n    max-width: 66.66667%; }\n  .col-sm-9 {\n    flex: 0 0 75%;\n    max-width: 75%; }\n  .col-sm-10 {\n    flex: 0 0 83.33333%;\n    max-width: 83.33333%; }\n  .col-sm-11 {\n    flex: 0 0 91.66667%;\n    max-width: 91.66667%; }\n  .col-sm-12 {\n    flex: 0 0 100%;\n    max-width: 100%; }\n  .order-sm-first {\n    order: -1; }\n  .order-sm-last {\n    order: 13; }\n  .order-sm-0 {\n    order: 0; }\n  .order-sm-1 {\n    order: 1; }\n  .order-sm-2 {\n    order: 2; }\n  .order-sm-3 {\n    order: 3; }\n  .order-sm-4 {\n    order: 4; }\n  .order-sm-5 {\n    order: 5; }\n  .order-sm-6 {\n    order: 6; }\n  .order-sm-7 {\n    order: 7; }\n  .order-sm-8 {\n    order: 8; }\n  .order-sm-9 {\n    order: 9; }\n  .order-sm-10 {\n    order: 10; }\n  .order-sm-11 {\n    order: 11; }\n  .order-sm-12 {\n    order: 12; }\n  .offset-sm-0 {\n    margin-left: 0; }\n  .offset-sm-1 {\n    margin-left: 8.33333%; }\n  .offset-sm-2 {\n    margin-left: 16.66667%; }\n  .offset-sm-3 {\n    margin-left: 25%; }\n  .offset-sm-4 {\n    margin-left: 33.33333%; }\n  .offset-sm-5 {\n    margin-left: 41.66667%; }\n  .offset-sm-6 {\n    margin-left: 50%; }\n  .offset-sm-7 {\n    margin-left: 58.33333%; }\n  .offset-sm-8 {\n    margin-left: 66.66667%; }\n  .offset-sm-9 {\n    margin-left: 75%; }\n  .offset-sm-10 {\n    margin-left: 83.33333%; }\n  .offset-sm-11 {\n    margin-left: 91.66667%; } }\n\n@media (min-width: 768px) {\n  .col-md {\n    flex-basis: 0;\n    flex-grow: 1;\n    max-width: 100%; }\n  .col-md-auto {\n    flex: 0 0 auto;\n    width: auto;\n    max-width: 100%; }\n  .col-md-1 {\n    flex: 0 0 8.33333%;\n    max-width: 8.33333%; }\n  .col-md-2 {\n    flex: 0 0 16.66667%;\n    max-width: 16.66667%; }\n  .col-md-3 {\n    flex: 0 0 25%;\n    max-width: 25%; }\n  .col-md-4 {\n    flex: 0 0 33.33333%;\n    max-width: 33.33333%; }\n  .col-md-5 {\n    flex: 0 0 41.66667%;\n    max-width: 41.66667%; }\n  .col-md-6 {\n    flex: 0 0 50%;\n    max-width: 50%; }\n  .col-md-7 {\n    flex: 0 0 58.33333%;\n    max-width: 58.33333%; }\n  .col-md-8 {\n    flex: 0 0 66.66667%;\n    max-width: 66.66667%; }\n  .col-md-9 {\n    flex: 0 0 75%;\n    max-width: 75%; }\n  .col-md-10 {\n    flex: 0 0 83.33333%;\n    max-width: 83.33333%; }\n  .col-md-11 {\n    flex: 0 0 91.66667%;\n    max-width: 91.66667%; }\n  .col-md-12 {\n    flex: 0 0 100%;\n    max-width: 100%; }\n  .order-md-first {\n    order: -1; }\n  .order-md-last {\n    order: 13; }\n  .order-md-0 {\n    order: 0; }\n  .order-md-1 {\n    order: 1; }\n  .order-md-2 {\n    order: 2; }\n  .order-md-3 {\n    order: 3; }\n  .order-md-4 {\n    order: 4; }\n  .order-md-5 {\n    order: 5; }\n  .order-md-6 {\n    order: 6; }\n  .order-md-7 {\n    order: 7; }\n  .order-md-8 {\n    order: 8; }\n  .order-md-9 {\n    order: 9; }\n  .order-md-10 {\n    order: 10; }\n  .order-md-11 {\n    order: 11; }\n  .order-md-12 {\n    order: 12; }\n  .offset-md-0 {\n    margin-left: 0; }\n  .offset-md-1 {\n    margin-left: 8.33333%; }\n  .offset-md-2 {\n    margin-left: 16.66667%; }\n  .offset-md-3 {\n    margin-left: 25%; }\n  .offset-md-4 {\n    margin-left: 33.33333%; }\n  .offset-md-5 {\n    margin-left: 41.66667%; }\n  .offset-md-6 {\n    margin-left: 50%; }\n  .offset-md-7 {\n    margin-left: 58.33333%; }\n  .offset-md-8 {\n    margin-left: 66.66667%; }\n  .offset-md-9 {\n    margin-left: 75%; }\n  .offset-md-10 {\n    margin-left: 83.33333%; }\n  .offset-md-11 {\n    margin-left: 91.66667%; } }\n\n@media (min-width: 992px) {\n  .col-lg {\n    flex-basis: 0;\n    flex-grow: 1;\n    max-width: 100%; }\n  .col-lg-auto {\n    flex: 0 0 auto;\n    width: auto;\n    max-width: 100%; }\n  .col-lg-1 {\n    flex: 0 0 8.33333%;\n    max-width: 8.33333%; }\n  .col-lg-2 {\n    flex: 0 0 16.66667%;\n    max-width: 16.66667%; }\n  .col-lg-3 {\n    flex: 0 0 25%;\n    max-width: 25%; }\n  .col-lg-4 {\n    flex: 0 0 33.33333%;\n    max-width: 33.33333%; }\n  .col-lg-5 {\n    flex: 0 0 41.66667%;\n    max-width: 41.66667%; }\n  .col-lg-6 {\n    flex: 0 0 50%;\n    max-width: 50%; }\n  .col-lg-7 {\n    flex: 0 0 58.33333%;\n    max-width: 58.33333%; }\n  .col-lg-8 {\n    flex: 0 0 66.66667%;\n    max-width: 66.66667%; }\n  .col-lg-9 {\n    flex: 0 0 75%;\n    max-width: 75%; }\n  .col-lg-10 {\n    flex: 0 0 83.33333%;\n    max-width: 83.33333%; }\n  .col-lg-11 {\n    flex: 0 0 91.66667%;\n    max-width: 91.66667%; }\n  .col-lg-12 {\n    flex: 0 0 100%;\n    max-width: 100%; }\n  .order-lg-first {\n    order: -1; }\n  .order-lg-last {\n    order: 13; }\n  .order-lg-0 {\n    order: 0; }\n  .order-lg-1 {\n    order: 1; }\n  .order-lg-2 {\n    order: 2; }\n  .order-lg-3 {\n    order: 3; }\n  .order-lg-4 {\n    order: 4; }\n  .order-lg-5 {\n    order: 5; }\n  .order-lg-6 {\n    order: 6; }\n  .order-lg-7 {\n    order: 7; }\n  .order-lg-8 {\n    order: 8; }\n  .order-lg-9 {\n    order: 9; }\n  .order-lg-10 {\n    order: 10; }\n  .order-lg-11 {\n    order: 11; }\n  .order-lg-12 {\n    order: 12; }\n  .offset-lg-0 {\n    margin-left: 0; }\n  .offset-lg-1 {\n    margin-left: 8.33333%; }\n  .offset-lg-2 {\n    margin-left: 16.66667%; }\n  .offset-lg-3 {\n    margin-left: 25%; }\n  .offset-lg-4 {\n    margin-left: 33.33333%; }\n  .offset-lg-5 {\n    margin-left: 41.66667%; }\n  .offset-lg-6 {\n    margin-left: 50%; }\n  .offset-lg-7 {\n    margin-left: 58.33333%; }\n  .offset-lg-8 {\n    margin-left: 66.66667%; }\n  .offset-lg-9 {\n    margin-left: 75%; }\n  .offset-lg-10 {\n    margin-left: 83.33333%; }\n  .offset-lg-11 {\n    margin-left: 91.66667%; } }\n\n@media (min-width: 1200px) {\n  .col-xl {\n    flex-basis: 0;\n    flex-grow: 1;\n    max-width: 100%; }\n  .col-xl-auto {\n    flex: 0 0 auto;\n    width: auto;\n    max-width: 100%; }\n  .col-xl-1 {\n    flex: 0 0 8.33333%;\n    max-width: 8.33333%; }\n  .col-xl-2 {\n    flex: 0 0 16.66667%;\n    max-width: 16.66667%; }\n  .col-xl-3 {\n    flex: 0 0 25%;\n    max-width: 25%; }\n  .col-xl-4 {\n    flex: 0 0 33.33333%;\n    max-width: 33.33333%; }\n  .col-xl-5 {\n    flex: 0 0 41.66667%;\n    max-width: 41.66667%; }\n  .col-xl-6 {\n    flex: 0 0 50%;\n    max-width: 50%; }\n  .col-xl-7 {\n    flex: 0 0 58.33333%;\n    max-width: 58.33333%; }\n  .col-xl-8 {\n    flex: 0 0 66.66667%;\n    max-width: 66.66667%; }\n  .col-xl-9 {\n    flex: 0 0 75%;\n    max-width: 75%; }\n  .col-xl-10 {\n    flex: 0 0 83.33333%;\n    max-width: 83.33333%; }\n  .col-xl-11 {\n    flex: 0 0 91.66667%;\n    max-width: 91.66667%; }\n  .col-xl-12 {\n    flex: 0 0 100%;\n    max-width: 100%; }\n  .order-xl-first {\n    order: -1; }\n  .order-xl-last {\n    order: 13; }\n  .order-xl-0 {\n    order: 0; }\n  .order-xl-1 {\n    order: 1; }\n  .order-xl-2 {\n    order: 2; }\n  .order-xl-3 {\n    order: 3; }\n  .order-xl-4 {\n    order: 4; }\n  .order-xl-5 {\n    order: 5; }\n  .order-xl-6 {\n    order: 6; }\n  .order-xl-7 {\n    order: 7; }\n  .order-xl-8 {\n    order: 8; }\n  .order-xl-9 {\n    order: 9; }\n  .order-xl-10 {\n    order: 10; }\n  .order-xl-11 {\n    order: 11; }\n  .order-xl-12 {\n    order: 12; }\n  .offset-xl-0 {\n    margin-left: 0; }\n  .offset-xl-1 {\n    margin-left: 8.33333%; }\n  .offset-xl-2 {\n    margin-left: 16.66667%; }\n  .offset-xl-3 {\n    margin-left: 25%; }\n  .offset-xl-4 {\n    margin-left: 33.33333%; }\n  .offset-xl-5 {\n    margin-left: 41.66667%; }\n  .offset-xl-6 {\n    margin-left: 50%; }\n  .offset-xl-7 {\n    margin-left: 58.33333%; }\n  .offset-xl-8 {\n    margin-left: 66.66667%; }\n  .offset-xl-9 {\n    margin-left: 75%; }\n  .offset-xl-10 {\n    margin-left: 83.33333%; }\n  .offset-xl-11 {\n    margin-left: 91.66667%; } }\n\nh1, h2, h3, h4, h5, h6,\n.h1, .h2, .h3, .h4, .h5, .h6 {\n  margin-bottom: 0.5rem;\n  font-weight: 500;\n  line-height: 1.2; }\n\nh1, .h1 {\n  font-size: 2.5rem; }\n\nh2, .h2 {\n  font-size: 2rem; }\n\nh3, .h3 {\n  font-size: 1.75rem; }\n\nh4, .h4 {\n  font-size: 1.5rem; }\n\nh5, .h5 {\n  font-size: 1.25rem; }\n\nh6, .h6 {\n  font-size: 1rem; }\n\n.lead {\n  font-size: 1.25rem;\n  font-weight: 300; }\n\n.display-1 {\n  font-size: 6rem;\n  font-weight: 300;\n  line-height: 1.2; }\n\n.display-2 {\n  font-size: 5.5rem;\n  font-weight: 300;\n  line-height: 1.2; }\n\n.display-3 {\n  font-size: 4.5rem;\n  font-weight: 300;\n  line-height: 1.2; }\n\n.display-4 {\n  font-size: 3.5rem;\n  font-weight: 300;\n  line-height: 1.2; }\n\nhr {\n  margin-top: 1rem;\n  margin-bottom: 1rem;\n  border: 0;\n  border-top: 1px solid rgba(0, 0, 0, 0.1); }\n\nsmall,\n.small {\n  font-size: 80%;\n  font-weight: 400; }\n\nmark,\n.mark {\n  padding: 0.2em;\n  background-color: #fcf8e3; }\n\n.list-unstyled {\n  padding-left: 0;\n  list-style: none; }\n\n.list-inline {\n  padding-left: 0;\n  list-style: none; }\n\n.list-inline-item {\n  display: inline-block; }\n  .list-inline-item:not(:last-child) {\n    margin-right: 0.5rem; }\n\n.initialism {\n  font-size: 90%;\n  text-transform: uppercase; }\n\n.blockquote {\n  margin-bottom: 1rem;\n  font-size: 1.25rem; }\n\n.blockquote-footer {\n  display: block;\n  font-size: 80%;\n  color: #6c757d; }\n  .blockquote-footer::before {\n    content: \"\\2014\\00A0\"; }\n\n.breadcrumb {\n  display: flex;\n  flex-wrap: wrap;\n  padding: 0.75rem 1rem;\n  margin-bottom: 1rem;\n  list-style: none;\n  background-color: #e9ecef;\n  border-radius: 0.25rem; }\n\n.breadcrumb-item + .breadcrumb-item {\n  padding-left: 0.5rem; }\n  .breadcrumb-item + .breadcrumb-item::before {\n    display: inline-block;\n    padding-right: 0.5rem;\n    color: #6c757d;\n    content: \"/\"; }\n\n.breadcrumb-item + .breadcrumb-item:hover::before {\n  text-decoration: underline; }\n\n.breadcrumb-item + .breadcrumb-item:hover::before {\n  text-decoration: none; }\n\n.breadcrumb-item.active {\n  color: #6c757d; }\n\n.table {\n  width: 100%;\n  margin-bottom: 1rem;\n  color: #212529; }\n  .table th,\n  .table td {\n    padding: 0.75rem;\n    vertical-align: top;\n    border-top: 1px solid #dee2e6; }\n  .table thead th {\n    vertical-align: bottom;\n    border-bottom: 2px solid #dee2e6; }\n  .table tbody + tbody {\n    border-top: 2px solid #dee2e6; }\n\n.table-sm th,\n.table-sm td {\n  padding: 0.3rem; }\n\n.table-bordered {\n  border: 1px solid #dee2e6; }\n  .table-bordered th,\n  .table-bordered td {\n    border: 1px solid #dee2e6; }\n  .table-bordered thead th,\n  .table-bordered thead td {\n    border-bottom-width: 2px; }\n\n.table-borderless th,\n.table-borderless td,\n.table-borderless thead th,\n.table-borderless tbody + tbody {\n  border: 0; }\n\n.table-striped tbody tr:nth-of-type(odd) {\n  background-color: rgba(0, 0, 0, 0.05); }\n\n.table-hover tbody tr:hover {\n  color: #212529;\n  background-color: rgba(0, 0, 0, 0.075); }\n\n.table-primary,\n.table-primary > th,\n.table-primary > td {\n  background-color: #b8daff; }\n\n.table-primary th,\n.table-primary td,\n.table-primary thead th,\n.table-primary tbody + tbody {\n  border-color: #7abaff; }\n\n.table-hover .table-primary:hover {\n  background-color: #9fcdff; }\n  .table-hover .table-primary:hover > td,\n  .table-hover .table-primary:hover > th {\n    background-color: #9fcdff; }\n\n.table-secondary,\n.table-secondary > th,\n.table-secondary > td {\n  background-color: #d6d8db; }\n\n.table-secondary th,\n.table-secondary td,\n.table-secondary thead th,\n.table-secondary tbody + tbody {\n  border-color: #b3b7bb; }\n\n.table-hover .table-secondary:hover {\n  background-color: #c8cbcf; }\n  .table-hover .table-secondary:hover > td,\n  .table-hover .table-secondary:hover > th {\n    background-color: #c8cbcf; }\n\n.table-success,\n.table-success > th,\n.table-success > td {\n  background-color: #c3e6cb; }\n\n.table-success th,\n.table-success td,\n.table-success thead th,\n.table-success tbody + tbody {\n  border-color: #8fd19e; }\n\n.table-hover .table-success:hover {\n  background-color: #b1dfbb; }\n  .table-hover .table-success:hover > td,\n  .table-hover .table-success:hover > th {\n    background-color: #b1dfbb; }\n\n.table-info,\n.table-info > th,\n.table-info > td {\n  background-color: #bee5eb; }\n\n.table-info th,\n.table-info td,\n.table-info thead th,\n.table-info tbody + tbody {\n  border-color: #86cfda; }\n\n.table-hover .table-info:hover {\n  background-color: #abdde5; }\n  .table-hover .table-info:hover > td,\n  .table-hover .table-info:hover > th {\n    background-color: #abdde5; }\n\n.table-warning,\n.table-warning > th,\n.table-warning > td {\n  background-color: #ffeeba; }\n\n.table-warning th,\n.table-warning td,\n.table-warning thead th,\n.table-warning tbody + tbody {\n  border-color: #ffdf7e; }\n\n.table-hover .table-warning:hover {\n  background-color: #ffe8a1; }\n  .table-hover .table-warning:hover > td,\n  .table-hover .table-warning:hover > th {\n    background-color: #ffe8a1; }\n\n.table-danger,\n.table-danger > th,\n.table-danger > td {\n  background-color: #f5c6cb; }\n\n.table-danger th,\n.table-danger td,\n.table-danger thead th,\n.table-danger tbody + tbody {\n  border-color: #ed969e; }\n\n.table-hover .table-danger:hover {\n  background-color: #f1b0b7; }\n  .table-hover .table-danger:hover > td,\n  .table-hover .table-danger:hover > th {\n    background-color: #f1b0b7; }\n\n.table-light,\n.table-light > th,\n.table-light > td {\n  background-color: #fdfdfe; }\n\n.table-light th,\n.table-light td,\n.table-light thead th,\n.table-light tbody + tbody {\n  border-color: #fbfcfc; }\n\n.table-hover .table-light:hover {\n  background-color: #ececf6; }\n  .table-hover .table-light:hover > td,\n  .table-hover .table-light:hover > th {\n    background-color: #ececf6; }\n\n.table-dark,\n.table-dark > th,\n.table-dark > td {\n  background-color: #c6c8ca; }\n\n.table-dark th,\n.table-dark td,\n.table-dark thead th,\n.table-dark tbody + tbody {\n  border-color: #95999c; }\n\n.table-hover .table-dark:hover {\n  background-color: #b9bbbe; }\n  .table-hover .table-dark:hover > td,\n  .table-hover .table-dark:hover > th {\n    background-color: #b9bbbe; }\n\n.table-active,\n.table-active > th,\n.table-active > td {\n  background-color: rgba(0, 0, 0, 0.075); }\n\n.table-hover .table-active:hover {\n  background-color: rgba(0, 0, 0, 0.075); }\n  .table-hover .table-active:hover > td,\n  .table-hover .table-active:hover > th {\n    background-color: rgba(0, 0, 0, 0.075); }\n\n.table .thead-dark th {\n  color: #fff;\n  background-color: #343a40;\n  border-color: #454d55; }\n\n.table .thead-light th {\n  color: #495057;\n  background-color: #e9ecef;\n  border-color: #dee2e6; }\n\n.table-dark {\n  color: #fff;\n  background-color: #343a40; }\n  .table-dark th,\n  .table-dark td,\n  .table-dark thead th {\n    border-color: #454d55; }\n  .table-dark.table-bordered {\n    border: 0; }\n  .table-dark.table-striped tbody tr:nth-of-type(odd) {\n    background-color: rgba(255, 255, 255, 0.05); }\n  .table-dark.table-hover tbody tr:hover {\n    color: #fff;\n    background-color: rgba(255, 255, 255, 0.075); }\n\n@media (max-width: 575.98px) {\n  .table-responsive-sm {\n    display: block;\n    width: 100%;\n    overflow-x: auto;\n    -webkit-overflow-scrolling: touch; }\n    .table-responsive-sm > .table-bordered {\n      border: 0; } }\n\n@media (max-width: 767.98px) {\n  .table-responsive-md {\n    display: block;\n    width: 100%;\n    overflow-x: auto;\n    -webkit-overflow-scrolling: touch; }\n    .table-responsive-md > .table-bordered {\n      border: 0; } }\n\n@media (max-width: 991.98px) {\n  .table-responsive-lg {\n    display: block;\n    width: 100%;\n    overflow-x: auto;\n    -webkit-overflow-scrolling: touch; }\n    .table-responsive-lg > .table-bordered {\n      border: 0; } }\n\n@media (max-width: 1199.98px) {\n  .table-responsive-xl {\n    display: block;\n    width: 100%;\n    overflow-x: auto;\n    -webkit-overflow-scrolling: touch; }\n    .table-responsive-xl > .table-bordered {\n      border: 0; } }\n\n.table-responsive {\n  display: block;\n  width: 100%;\n  overflow-x: auto;\n  -webkit-overflow-scrolling: touch; }\n  .table-responsive > .table-bordered {\n    border: 0; }\n\n.badge {\n  display: inline-block;\n  padding: 0.25em 0.4em;\n  font-size: 75%;\n  font-weight: 700;\n  line-height: 1;\n  text-align: center;\n  white-space: nowrap;\n  vertical-align: baseline;\n  border-radius: 0.25rem;\n  transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out, border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out; }\n  @media (prefers-reduced-motion: reduce) {\n    .badge {\n      transition: none; } }\n  a.badge:hover, a.badge:focus {\n    text-decoration: none; }\n  .badge:empty {\n    display: none; }\n\n.btn .badge {\n  position: relative;\n  top: -1px; }\n\n.badge-pill {\n  padding-right: 0.6em;\n  padding-left: 0.6em;\n  border-radius: 10rem; }\n\n.badge-primary {\n  color: #fff;\n  background-color: #007bff; }\n  a.badge-primary:hover, a.badge-primary:focus {\n    color: #fff;\n    background-color: #0062cc; }\n  a.badge-primary:focus, a.badge-primary.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.5); }\n\n.badge-secondary {\n  color: #fff;\n  background-color: #6c757d; }\n  a.badge-secondary:hover, a.badge-secondary:focus {\n    color: #fff;\n    background-color: #545b62; }\n  a.badge-secondary:focus, a.badge-secondary.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(108, 117, 125, 0.5); }\n\n.badge-success {\n  color: #fff;\n  background-color: #28a745; }\n  a.badge-success:hover, a.badge-success:focus {\n    color: #fff;\n    background-color: #1e7e34; }\n  a.badge-success:focus, a.badge-success.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.5); }\n\n.badge-info {\n  color: #fff;\n  background-color: #17a2b8; }\n  a.badge-info:hover, a.badge-info:focus {\n    color: #fff;\n    background-color: #117a8b; }\n  a.badge-info:focus, a.badge-info.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(23, 162, 184, 0.5); }\n\n.badge-warning {\n  color: #212529;\n  background-color: #ffc107; }\n  a.badge-warning:hover, a.badge-warning:focus {\n    color: #212529;\n    background-color: #d39e00; }\n  a.badge-warning:focus, a.badge-warning.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(255, 193, 7, 0.5); }\n\n.badge-danger {\n  color: #fff;\n  background-color: #dc3545; }\n  a.badge-danger:hover, a.badge-danger:focus {\n    color: #fff;\n    background-color: #bd2130; }\n  a.badge-danger:focus, a.badge-danger.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.5); }\n\n.badge-light {\n  color: #212529;\n  background-color: #f8f9fa; }\n  a.badge-light:hover, a.badge-light:focus {\n    color: #212529;\n    background-color: #dae0e5; }\n  a.badge-light:focus, a.badge-light.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(248, 249, 250, 0.5); }\n\n.badge-dark {\n  color: #fff;\n  background-color: #343a40; }\n  a.badge-dark:hover, a.badge-dark:focus {\n    color: #fff;\n    background-color: #1d2124; }\n  a.badge-dark:focus, a.badge-dark.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(52, 58, 64, 0.5); }\n\n.btn {\n  display: inline-block;\n  font-weight: 400;\n  color: #212529;\n  text-align: center;\n  vertical-align: middle;\n  user-select: none;\n  background-color: transparent;\n  border: 1px solid transparent;\n  padding: 0.375rem 0.75rem;\n  font-size: 1rem;\n  line-height: 1.5;\n  border-radius: 0.25rem;\n  transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out, border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out; }\n  @media (prefers-reduced-motion: reduce) {\n    .btn {\n      transition: none; } }\n  .btn:hover {\n    color: #212529;\n    text-decoration: none; }\n  .btn:focus, .btn.focus {\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25); }\n  .btn.disabled, .btn:disabled {\n    opacity: 0.65; }\n\na.btn.disabled,\nfieldset:disabled a.btn {\n  pointer-events: none; }\n\n.btn-primary {\n  color: #fff;\n  background-color: #007bff;\n  border-color: #007bff; }\n  .btn-primary:hover {\n    color: #fff;\n    background-color: #0069d9;\n    border-color: #0062cc; }\n  .btn-primary:focus, .btn-primary.focus {\n    box-shadow: 0 0 0 0.2rem rgba(38, 143, 255, 0.5); }\n  .btn-primary.disabled, .btn-primary:disabled {\n    color: #fff;\n    background-color: #007bff;\n    border-color: #007bff; }\n  .btn-primary:not(:disabled):not(.disabled):active, .btn-primary:not(:disabled):not(.disabled).active,\n  .show > .btn-primary.dropdown-toggle {\n    color: #fff;\n    background-color: #0062cc;\n    border-color: #005cbf; }\n    .btn-primary:not(:disabled):not(.disabled):active:focus, .btn-primary:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-primary.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(38, 143, 255, 0.5); }\n\n.btn-secondary {\n  color: #fff;\n  background-color: #6c757d;\n  border-color: #6c757d; }\n  .btn-secondary:hover {\n    color: #fff;\n    background-color: #5a6268;\n    border-color: #545b62; }\n  .btn-secondary:focus, .btn-secondary.focus {\n    box-shadow: 0 0 0 0.2rem rgba(130, 138, 145, 0.5); }\n  .btn-secondary.disabled, .btn-secondary:disabled {\n    color: #fff;\n    background-color: #6c757d;\n    border-color: #6c757d; }\n  .btn-secondary:not(:disabled):not(.disabled):active, .btn-secondary:not(:disabled):not(.disabled).active,\n  .show > .btn-secondary.dropdown-toggle {\n    color: #fff;\n    background-color: #545b62;\n    border-color: #4e555b; }\n    .btn-secondary:not(:disabled):not(.disabled):active:focus, .btn-secondary:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-secondary.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(130, 138, 145, 0.5); }\n\n.btn-success {\n  color: #fff;\n  background-color: #28a745;\n  border-color: #28a745; }\n  .btn-success:hover {\n    color: #fff;\n    background-color: #218838;\n    border-color: #1e7e34; }\n  .btn-success:focus, .btn-success.focus {\n    box-shadow: 0 0 0 0.2rem rgba(72, 180, 97, 0.5); }\n  .btn-success.disabled, .btn-success:disabled {\n    color: #fff;\n    background-color: #28a745;\n    border-color: #28a745; }\n  .btn-success:not(:disabled):not(.disabled):active, .btn-success:not(:disabled):not(.disabled).active,\n  .show > .btn-success.dropdown-toggle {\n    color: #fff;\n    background-color: #1e7e34;\n    border-color: #1c7430; }\n    .btn-success:not(:disabled):not(.disabled):active:focus, .btn-success:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-success.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(72, 180, 97, 0.5); }\n\n.btn-info {\n  color: #fff;\n  background-color: #17a2b8;\n  border-color: #17a2b8; }\n  .btn-info:hover {\n    color: #fff;\n    background-color: #138496;\n    border-color: #117a8b; }\n  .btn-info:focus, .btn-info.focus {\n    box-shadow: 0 0 0 0.2rem rgba(58, 176, 195, 0.5); }\n  .btn-info.disabled, .btn-info:disabled {\n    color: #fff;\n    background-color: #17a2b8;\n    border-color: #17a2b8; }\n  .btn-info:not(:disabled):not(.disabled):active, .btn-info:not(:disabled):not(.disabled).active,\n  .show > .btn-info.dropdown-toggle {\n    color: #fff;\n    background-color: #117a8b;\n    border-color: #10707f; }\n    .btn-info:not(:disabled):not(.disabled):active:focus, .btn-info:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-info.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(58, 176, 195, 0.5); }\n\n.btn-warning {\n  color: #212529;\n  background-color: #ffc107;\n  border-color: #ffc107; }\n  .btn-warning:hover {\n    color: #212529;\n    background-color: #e0a800;\n    border-color: #d39e00; }\n  .btn-warning:focus, .btn-warning.focus {\n    box-shadow: 0 0 0 0.2rem rgba(222, 170, 12, 0.5); }\n  .btn-warning.disabled, .btn-warning:disabled {\n    color: #212529;\n    background-color: #ffc107;\n    border-color: #ffc107; }\n  .btn-warning:not(:disabled):not(.disabled):active, .btn-warning:not(:disabled):not(.disabled).active,\n  .show > .btn-warning.dropdown-toggle {\n    color: #212529;\n    background-color: #d39e00;\n    border-color: #c69500; }\n    .btn-warning:not(:disabled):not(.disabled):active:focus, .btn-warning:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-warning.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(222, 170, 12, 0.5); }\n\n.btn-danger {\n  color: #fff;\n  background-color: #dc3545;\n  border-color: #dc3545; }\n  .btn-danger:hover {\n    color: #fff;\n    background-color: #c82333;\n    border-color: #bd2130; }\n  .btn-danger:focus, .btn-danger.focus {\n    box-shadow: 0 0 0 0.2rem rgba(225, 83, 97, 0.5); }\n  .btn-danger.disabled, .btn-danger:disabled {\n    color: #fff;\n    background-color: #dc3545;\n    border-color: #dc3545; }\n  .btn-danger:not(:disabled):not(.disabled):active, .btn-danger:not(:disabled):not(.disabled).active,\n  .show > .btn-danger.dropdown-toggle {\n    color: #fff;\n    background-color: #bd2130;\n    border-color: #b21f2d; }\n    .btn-danger:not(:disabled):not(.disabled):active:focus, .btn-danger:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-danger.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(225, 83, 97, 0.5); }\n\n.btn-light {\n  color: #212529;\n  background-color: #f8f9fa;\n  border-color: #f8f9fa; }\n  .btn-light:hover {\n    color: #212529;\n    background-color: #e2e6ea;\n    border-color: #dae0e5; }\n  .btn-light:focus, .btn-light.focus {\n    box-shadow: 0 0 0 0.2rem rgba(216, 217, 219, 0.5); }\n  .btn-light.disabled, .btn-light:disabled {\n    color: #212529;\n    background-color: #f8f9fa;\n    border-color: #f8f9fa; }\n  .btn-light:not(:disabled):not(.disabled):active, .btn-light:not(:disabled):not(.disabled).active,\n  .show > .btn-light.dropdown-toggle {\n    color: #212529;\n    background-color: #dae0e5;\n    border-color: #d3d9df; }\n    .btn-light:not(:disabled):not(.disabled):active:focus, .btn-light:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-light.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(216, 217, 219, 0.5); }\n\n.btn-dark {\n  color: #fff;\n  background-color: #343a40;\n  border-color: #343a40; }\n  .btn-dark:hover {\n    color: #fff;\n    background-color: #23272b;\n    border-color: #1d2124; }\n  .btn-dark:focus, .btn-dark.focus {\n    box-shadow: 0 0 0 0.2rem rgba(82, 88, 93, 0.5); }\n  .btn-dark.disabled, .btn-dark:disabled {\n    color: #fff;\n    background-color: #343a40;\n    border-color: #343a40; }\n  .btn-dark:not(:disabled):not(.disabled):active, .btn-dark:not(:disabled):not(.disabled).active,\n  .show > .btn-dark.dropdown-toggle {\n    color: #fff;\n    background-color: #1d2124;\n    border-color: #171a1d; }\n    .btn-dark:not(:disabled):not(.disabled):active:focus, .btn-dark:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-dark.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(82, 88, 93, 0.5); }\n\n.btn-outline-primary {\n  color: #007bff;\n  border-color: #007bff; }\n  .btn-outline-primary:hover {\n    color: #fff;\n    background-color: #007bff;\n    border-color: #007bff; }\n  .btn-outline-primary:focus, .btn-outline-primary.focus {\n    box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.5); }\n  .btn-outline-primary.disabled, .btn-outline-primary:disabled {\n    color: #007bff;\n    background-color: transparent; }\n  .btn-outline-primary:not(:disabled):not(.disabled):active, .btn-outline-primary:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-primary.dropdown-toggle {\n    color: #fff;\n    background-color: #007bff;\n    border-color: #007bff; }\n    .btn-outline-primary:not(:disabled):not(.disabled):active:focus, .btn-outline-primary:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-primary.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.5); }\n\n.btn-outline-secondary {\n  color: #6c757d;\n  border-color: #6c757d; }\n  .btn-outline-secondary:hover {\n    color: #fff;\n    background-color: #6c757d;\n    border-color: #6c757d; }\n  .btn-outline-secondary:focus, .btn-outline-secondary.focus {\n    box-shadow: 0 0 0 0.2rem rgba(108, 117, 125, 0.5); }\n  .btn-outline-secondary.disabled, .btn-outline-secondary:disabled {\n    color: #6c757d;\n    background-color: transparent; }\n  .btn-outline-secondary:not(:disabled):not(.disabled):active, .btn-outline-secondary:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-secondary.dropdown-toggle {\n    color: #fff;\n    background-color: #6c757d;\n    border-color: #6c757d; }\n    .btn-outline-secondary:not(:disabled):not(.disabled):active:focus, .btn-outline-secondary:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-secondary.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(108, 117, 125, 0.5); }\n\n.btn-outline-success {\n  color: #28a745;\n  border-color: #28a745; }\n  .btn-outline-success:hover {\n    color: #fff;\n    background-color: #28a745;\n    border-color: #28a745; }\n  .btn-outline-success:focus, .btn-outline-success.focus {\n    box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.5); }\n  .btn-outline-success.disabled, .btn-outline-success:disabled {\n    color: #28a745;\n    background-color: transparent; }\n  .btn-outline-success:not(:disabled):not(.disabled):active, .btn-outline-success:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-success.dropdown-toggle {\n    color: #fff;\n    background-color: #28a745;\n    border-color: #28a745; }\n    .btn-outline-success:not(:disabled):not(.disabled):active:focus, .btn-outline-success:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-success.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.5); }\n\n.btn-outline-info {\n  color: #17a2b8;\n  border-color: #17a2b8; }\n  .btn-outline-info:hover {\n    color: #fff;\n    background-color: #17a2b8;\n    border-color: #17a2b8; }\n  .btn-outline-info:focus, .btn-outline-info.focus {\n    box-shadow: 0 0 0 0.2rem rgba(23, 162, 184, 0.5); }\n  .btn-outline-info.disabled, .btn-outline-info:disabled {\n    color: #17a2b8;\n    background-color: transparent; }\n  .btn-outline-info:not(:disabled):not(.disabled):active, .btn-outline-info:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-info.dropdown-toggle {\n    color: #fff;\n    background-color: #17a2b8;\n    border-color: #17a2b8; }\n    .btn-outline-info:not(:disabled):not(.disabled):active:focus, .btn-outline-info:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-info.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(23, 162, 184, 0.5); }\n\n.btn-outline-warning {\n  color: #ffc107;\n  border-color: #ffc107; }\n  .btn-outline-warning:hover {\n    color: #212529;\n    background-color: #ffc107;\n    border-color: #ffc107; }\n  .btn-outline-warning:focus, .btn-outline-warning.focus {\n    box-shadow: 0 0 0 0.2rem rgba(255, 193, 7, 0.5); }\n  .btn-outline-warning.disabled, .btn-outline-warning:disabled {\n    color: #ffc107;\n    background-color: transparent; }\n  .btn-outline-warning:not(:disabled):not(.disabled):active, .btn-outline-warning:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-warning.dropdown-toggle {\n    color: #212529;\n    background-color: #ffc107;\n    border-color: #ffc107; }\n    .btn-outline-warning:not(:disabled):not(.disabled):active:focus, .btn-outline-warning:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-warning.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(255, 193, 7, 0.5); }\n\n.btn-outline-danger {\n  color: #dc3545;\n  border-color: #dc3545; }\n  .btn-outline-danger:hover {\n    color: #fff;\n    background-color: #dc3545;\n    border-color: #dc3545; }\n  .btn-outline-danger:focus, .btn-outline-danger.focus {\n    box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.5); }\n  .btn-outline-danger.disabled, .btn-outline-danger:disabled {\n    color: #dc3545;\n    background-color: transparent; }\n  .btn-outline-danger:not(:disabled):not(.disabled):active, .btn-outline-danger:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-danger.dropdown-toggle {\n    color: #fff;\n    background-color: #dc3545;\n    border-color: #dc3545; }\n    .btn-outline-danger:not(:disabled):not(.disabled):active:focus, .btn-outline-danger:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-danger.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.5); }\n\n.btn-outline-light {\n  color: #f8f9fa;\n  border-color: #f8f9fa; }\n  .btn-outline-light:hover {\n    color: #212529;\n    background-color: #f8f9fa;\n    border-color: #f8f9fa; }\n  .btn-outline-light:focus, .btn-outline-light.focus {\n    box-shadow: 0 0 0 0.2rem rgba(248, 249, 250, 0.5); }\n  .btn-outline-light.disabled, .btn-outline-light:disabled {\n    color: #f8f9fa;\n    background-color: transparent; }\n  .btn-outline-light:not(:disabled):not(.disabled):active, .btn-outline-light:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-light.dropdown-toggle {\n    color: #212529;\n    background-color: #f8f9fa;\n    border-color: #f8f9fa; }\n    .btn-outline-light:not(:disabled):not(.disabled):active:focus, .btn-outline-light:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-light.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(248, 249, 250, 0.5); }\n\n.btn-outline-dark {\n  color: #343a40;\n  border-color: #343a40; }\n  .btn-outline-dark:hover {\n    color: #fff;\n    background-color: #343a40;\n    border-color: #343a40; }\n  .btn-outline-dark:focus, .btn-outline-dark.focus {\n    box-shadow: 0 0 0 0.2rem rgba(52, 58, 64, 0.5); }\n  .btn-outline-dark.disabled, .btn-outline-dark:disabled {\n    color: #343a40;\n    background-color: transparent; }\n  .btn-outline-dark:not(:disabled):not(.disabled):active, .btn-outline-dark:not(:disabled):not(.disabled).active,\n  .show > .btn-outline-dark.dropdown-toggle {\n    color: #fff;\n    background-color: #343a40;\n    border-color: #343a40; }\n    .btn-outline-dark:not(:disabled):not(.disabled):active:focus, .btn-outline-dark:not(:disabled):not(.disabled).active:focus,\n    .show > .btn-outline-dark.dropdown-toggle:focus {\n      box-shadow: 0 0 0 0.2rem rgba(52, 58, 64, 0.5); }\n\n.btn-link {\n  font-weight: 400;\n  color: #007bff;\n  text-decoration: none; }\n  .btn-link:hover {\n    color: #0056b3;\n    text-decoration: underline; }\n  .btn-link:focus, .btn-link.focus {\n    text-decoration: underline;\n    box-shadow: none; }\n  .btn-link:disabled, .btn-link.disabled {\n    color: #6c757d;\n    pointer-events: none; }\n\n.btn-lg {\n  padding: 0.5rem 1rem;\n  font-size: 1.25rem;\n  line-height: 1.5;\n  border-radius: 0.3rem; }\n\n.btn-sm {\n  padding: 0.25rem 0.5rem;\n  font-size: 0.875rem;\n  line-height: 1.5;\n  border-radius: 0.2rem; }\n\n.btn-block {\n  display: block;\n  width: 100%; }\n  .btn-block + .btn-block {\n    margin-top: 0.5rem; }\n\ninput[type=\"submit\"].btn-block,\ninput[type=\"reset\"].btn-block,\ninput[type=\"button\"].btn-block {\n  width: 100%; }\n\n.fade {\n  transition: opacity 0.15s linear; }\n  @media (prefers-reduced-motion: reduce) {\n    .fade {\n      transition: none; } }\n  .fade:not(.show) {\n    opacity: 0; }\n\n.collapse:not(.show) {\n  display: none; }\n\n.collapsing {\n  position: relative;\n  height: 0;\n  overflow: hidden;\n  transition: height 0.35s ease; }\n  @media (prefers-reduced-motion: reduce) {\n    .collapsing {\n      transition: none; } }\n\n.form-control {\n  display: block;\n  width: 100%;\n  height: calc(1.5em + 0.75rem + 2px);\n  padding: 0.375rem 0.75rem;\n  font-size: 1rem;\n  font-weight: 400;\n  line-height: 1.5;\n  color: #495057;\n  background-color: #fff;\n  background-clip: padding-box;\n  border: 1px solid #ced4da;\n  border-radius: 0.25rem;\n  transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out; }\n  @media (prefers-reduced-motion: reduce) {\n    .form-control {\n      transition: none; } }\n  .form-control::-ms-expand {\n    background-color: transparent;\n    border: 0; }\n  .form-control:focus {\n    color: #495057;\n    background-color: #fff;\n    border-color: #80bdff;\n    outline: 0;\n    box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25); }\n  .form-control::placeholder {\n    color: #6c757d;\n    opacity: 1; }\n  .form-control:disabled, .form-control[readonly] {\n    background-color: #e9ecef;\n    opacity: 1; }\n\nselect.form-control:focus::-ms-value {\n  color: #495057;\n  background-color: #fff; }\n\n.form-control-file,\n.form-control-range {\n  display: block;\n  width: 100%; }\n\n.col-form-label {\n  padding-top: calc(0.375rem + 1px);\n  padding-bottom: calc(0.375rem + 1px);\n  margin-bottom: 0;\n  font-size: inherit;\n  line-height: 1.5; }\n\n.col-form-label-lg {\n  padding-top: calc(0.5rem + 1px);\n  padding-bottom: calc(0.5rem + 1px);\n  font-size: 1.25rem;\n  line-height: 1.5; }\n\n.col-form-label-sm {\n  padding-top: calc(0.25rem + 1px);\n  padding-bottom: calc(0.25rem + 1px);\n  font-size: 0.875rem;\n  line-height: 1.5; }\n\n.form-control-plaintext {\n  display: block;\n  width: 100%;\n  padding-top: 0.375rem;\n  padding-bottom: 0.375rem;\n  margin-bottom: 0;\n  line-height: 1.5;\n  color: #212529;\n  background-color: transparent;\n  border: solid transparent;\n  border-width: 1px 0; }\n  .form-control-plaintext.form-control-sm, .form-control-plaintext.form-control-lg {\n    padding-right: 0;\n    padding-left: 0; }\n\n.form-control-sm {\n  height: calc(1.5em + 0.5rem + 2px);\n  padding: 0.25rem 0.5rem;\n  font-size: 0.875rem;\n  line-height: 1.5;\n  border-radius: 0.2rem; }\n\n.form-control-lg {\n  height: calc(1.5em + 1rem + 2px);\n  padding: 0.5rem 1rem;\n  font-size: 1.25rem;\n  line-height: 1.5;\n  border-radius: 0.3rem; }\n\nselect.form-control[size], select.form-control[multiple] {\n  height: auto; }\n\ntextarea.form-control {\n  height: auto; }\n\n.form-group {\n  margin-bottom: 1rem; }\n\n.form-text {\n  display: block;\n  margin-top: 0.25rem; }\n\n.form-row {\n  display: flex;\n  flex-wrap: wrap;\n  margin-right: -5px;\n  margin-left: -5px; }\n  .form-row > .col,\n  .form-row > [class*=\"col-\"] {\n    padding-right: 5px;\n    padding-left: 5px; }\n\n.form-check {\n  position: relative;\n  display: block;\n  padding-left: 1.25rem; }\n\n.form-check-input {\n  position: absolute;\n  margin-top: 0.3rem;\n  margin-left: -1.25rem; }\n  .form-check-input:disabled ~ .form-check-label {\n    color: #6c757d; }\n\n.form-check-label {\n  margin-bottom: 0; }\n\n.form-check-inline {\n  display: inline-flex;\n  align-items: center;\n  padding-left: 0;\n  margin-right: 0.75rem; }\n  .form-check-inline .form-check-input {\n    position: static;\n    margin-top: 0;\n    margin-right: 0.3125rem;\n    margin-left: 0; }\n\n.valid-feedback {\n  display: none;\n  width: 100%;\n  margin-top: 0.25rem;\n  font-size: 80%;\n  color: #28a745; }\n\n.valid-tooltip {\n  position: absolute;\n  top: 100%;\n  z-index: 5;\n  display: none;\n  max-width: 100%;\n  padding: 0.25rem 0.5rem;\n  margin-top: .1rem;\n  font-size: 0.875rem;\n  line-height: 1.5;\n  color: #fff;\n  background-color: rgba(40, 167, 69, 0.9);\n  border-radius: 0.25rem; }\n\n.was-validated .form-control:valid, .form-control.is-valid {\n  border-color: #28a745;\n  padding-right: calc(1.5em + 0.75rem);\n  background-image: url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 8 8'%3e%3cpath fill='%2328a745' d='M2.3 6.73L.6 4.53c-.4-1.04.46-1.4 1.1-.8l1.1 1.4 3.4-3.8c.6-.63 1.6-.27 1.2.7l-4 4.6c-.43.5-.8.4-1.1.1z'/%3e%3c/svg%3e\");\n  background-repeat: no-repeat;\n  background-position: center right calc(0.375em + 0.1875rem);\n  background-size: calc(0.75em + 0.375rem) calc(0.75em + 0.375rem); }\n  .was-validated .form-control:valid:focus, .form-control.is-valid:focus {\n    border-color: #28a745;\n    box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.25); }\n  .was-validated .form-control:valid ~ .valid-feedback,\n  .was-validated .form-control:valid ~ .valid-tooltip, .form-control.is-valid ~ .valid-feedback,\n  .form-control.is-valid ~ .valid-tooltip {\n    display: block; }\n\n.was-validated textarea.form-control:valid, textarea.form-control.is-valid {\n  padding-right: calc(1.5em + 0.75rem);\n  background-position: top calc(0.375em + 0.1875rem) right calc(0.375em + 0.1875rem); }\n\n.was-validated .custom-select:valid, .custom-select.is-valid {\n  border-color: #28a745;\n  padding-right: calc((1em + 0.75rem) * 3 / 4 + 1.75rem);\n  background: url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 4 5'%3e%3cpath fill='%23343a40' d='M2 0L0 2h4zm0 5L0 3h4z'/%3e%3c/svg%3e\") no-repeat right 0.75rem center/8px 10px, url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 8 8'%3e%3cpath fill='%2328a745' d='M2.3 6.73L.6 4.53c-.4-1.04.46-1.4 1.1-.8l1.1 1.4 3.4-3.8c.6-.63 1.6-.27 1.2.7l-4 4.6c-.43.5-.8.4-1.1.1z'/%3e%3c/svg%3e\") #fff no-repeat center right 1.75rem/calc(0.75em + 0.375rem) calc(0.75em + 0.375rem); }\n  .was-validated .custom-select:valid:focus, .custom-select.is-valid:focus {\n    border-color: #28a745;\n    box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.25); }\n  .was-validated .custom-select:valid ~ .valid-feedback,\n  .was-validated .custom-select:valid ~ .valid-tooltip, .custom-select.is-valid ~ .valid-feedback,\n  .custom-select.is-valid ~ .valid-tooltip {\n    display: block; }\n\n.was-validated .form-control-file:valid ~ .valid-feedback,\n.was-validated .form-control-file:valid ~ .valid-tooltip, .form-control-file.is-valid ~ .valid-feedback,\n.form-control-file.is-valid ~ .valid-tooltip {\n  display: block; }\n\n.was-validated .form-check-input:valid ~ .form-check-label, .form-check-input.is-valid ~ .form-check-label {\n  color: #28a745; }\n\n.was-validated .form-check-input:valid ~ .valid-feedback,\n.was-validated .form-check-input:valid ~ .valid-tooltip, .form-check-input.is-valid ~ .valid-feedback,\n.form-check-input.is-valid ~ .valid-tooltip {\n  display: block; }\n\n.was-validated .custom-control-input:valid ~ .custom-control-label, .custom-control-input.is-valid ~ .custom-control-label {\n  color: #28a745; }\n  .was-validated .custom-control-input:valid ~ .custom-control-label::before, .custom-control-input.is-valid ~ .custom-control-label::before {\n    border-color: #28a745; }\n\n.was-validated .custom-control-input:valid ~ .valid-feedback,\n.was-validated .custom-control-input:valid ~ .valid-tooltip, .custom-control-input.is-valid ~ .valid-feedback,\n.custom-control-input.is-valid ~ .valid-tooltip {\n  display: block; }\n\n.was-validated .custom-control-input:valid:checked ~ .custom-control-label::before, .custom-control-input.is-valid:checked ~ .custom-control-label::before {\n  border-color: #34ce57;\n  background-color: #34ce57; }\n\n.was-validated .custom-control-input:valid:focus ~ .custom-control-label::before, .custom-control-input.is-valid:focus ~ .custom-control-label::before {\n  box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.25); }\n\n.was-validated .custom-control-input:valid:focus:not(:checked) ~ .custom-control-label::before, .custom-control-input.is-valid:focus:not(:checked) ~ .custom-control-label::before {\n  border-color: #28a745; }\n\n.was-validated .custom-file-input:valid ~ .custom-file-label, .custom-file-input.is-valid ~ .custom-file-label {\n  border-color: #28a745; }\n\n.was-validated .custom-file-input:valid ~ .valid-feedback,\n.was-validated .custom-file-input:valid ~ .valid-tooltip, .custom-file-input.is-valid ~ .valid-feedback,\n.custom-file-input.is-valid ~ .valid-tooltip {\n  display: block; }\n\n.was-validated .custom-file-input:valid:focus ~ .custom-file-label, .custom-file-input.is-valid:focus ~ .custom-file-label {\n  border-color: #28a745;\n  box-shadow: 0 0 0 0.2rem rgba(40, 167, 69, 0.25); }\n\n.invalid-feedback {\n  display: none;\n  width: 100%;\n  margin-top: 0.25rem;\n  font-size: 80%;\n  color: #dc3545; }\n\n.invalid-tooltip {\n  position: absolute;\n  top: 100%;\n  z-index: 5;\n  display: none;\n  max-width: 100%;\n  padding: 0.25rem 0.5rem;\n  margin-top: .1rem;\n  font-size: 0.875rem;\n  line-height: 1.5;\n  color: #fff;\n  background-color: rgba(220, 53, 69, 0.9);\n  border-radius: 0.25rem; }\n\n.was-validated .form-control:invalid, .form-control.is-invalid {\n  border-color: #dc3545;\n  padding-right: calc(1.5em + 0.75rem);\n  background-image: url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='%23dc3545' viewBox='-2 -2 7 7'%3e%3cpath stroke='%23dc3545' d='M0 0l3 3m0-3L0 3'/%3e%3ccircle r='.5'/%3e%3ccircle cx='3' r='.5'/%3e%3ccircle cy='3' r='.5'/%3e%3ccircle cx='3' cy='3' r='.5'/%3e%3c/svg%3E\");\n  background-repeat: no-repeat;\n  background-position: center right calc(0.375em + 0.1875rem);\n  background-size: calc(0.75em + 0.375rem) calc(0.75em + 0.375rem); }\n  .was-validated .form-control:invalid:focus, .form-control.is-invalid:focus {\n    border-color: #dc3545;\n    box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25); }\n  .was-validated .form-control:invalid ~ .invalid-feedback,\n  .was-validated .form-control:invalid ~ .invalid-tooltip, .form-control.is-invalid ~ .invalid-feedback,\n  .form-control.is-invalid ~ .invalid-tooltip {\n    display: block; }\n\n.was-validated textarea.form-control:invalid, textarea.form-control.is-invalid {\n  padding-right: calc(1.5em + 0.75rem);\n  background-position: top calc(0.375em + 0.1875rem) right calc(0.375em + 0.1875rem); }\n\n.was-validated .custom-select:invalid, .custom-select.is-invalid {\n  border-color: #dc3545;\n  padding-right: calc((1em + 0.75rem) * 3 / 4 + 1.75rem);\n  background: url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 4 5'%3e%3cpath fill='%23343a40' d='M2 0L0 2h4zm0 5L0 3h4z'/%3e%3c/svg%3e\") no-repeat right 0.75rem center/8px 10px, url(\"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='%23dc3545' viewBox='-2 -2 7 7'%3e%3cpath stroke='%23dc3545' d='M0 0l3 3m0-3L0 3'/%3e%3ccircle r='.5'/%3e%3ccircle cx='3' r='.5'/%3e%3ccircle cy='3' r='.5'/%3e%3ccircle cx='3' cy='3' r='.5'/%3e%3c/svg%3E\") #fff no-repeat center right 1.75rem/calc(0.75em + 0.375rem) calc(0.75em + 0.375rem); }\n  .was-validated .custom-select:invalid:focus, .custom-select.is-invalid:focus {\n    border-color: #dc3545;\n    box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25); }\n  .was-validated .custom-select:invalid ~ .invalid-feedback,\n  .was-validated .custom-select:invalid ~ .invalid-tooltip, .custom-select.is-invalid ~ .invalid-feedback,\n  .custom-select.is-invalid ~ .invalid-tooltip {\n    display: block; }\n\n.was-validated .form-control-file:invalid ~ .invalid-feedback,\n.was-validated .form-control-file:invalid ~ .invalid-tooltip, .form-control-file.is-invalid ~ .invalid-feedback,\n.form-control-file.is-invalid ~ .invalid-tooltip {\n  display: block; }\n\n.was-validated .form-check-input:invalid ~ .form-check-label, .form-check-input.is-invalid ~ .form-check-label {\n  color: #dc3545; }\n\n.was-validated .form-check-input:invalid ~ .invalid-feedback,\n.was-validated .form-check-input:invalid ~ .invalid-tooltip, .form-check-input.is-invalid ~ .invalid-feedback,\n.form-check-input.is-invalid ~ .invalid-tooltip {\n  display: block; }\n\n.was-validated .custom-control-input:invalid ~ .custom-control-label, .custom-control-input.is-invalid ~ .custom-control-label {\n  color: #dc3545; }\n  .was-validated .custom-control-input:invalid ~ .custom-control-label::before, .custom-control-input.is-invalid ~ .custom-control-label::before {\n    border-color: #dc3545; }\n\n.was-validated .custom-control-input:invalid ~ .invalid-feedback,\n.was-validated .custom-control-input:invalid ~ .invalid-tooltip, .custom-control-input.is-invalid ~ .invalid-feedback,\n.custom-control-input.is-invalid ~ .invalid-tooltip {\n  display: block; }\n\n.was-validated .custom-control-input:invalid:checked ~ .custom-control-label::before, .custom-control-input.is-invalid:checked ~ .custom-control-label::before {\n  border-color: #e4606d;\n  background-color: #e4606d; }\n\n.was-validated .custom-control-input:invalid:focus ~ .custom-control-label::before, .custom-control-input.is-invalid:focus ~ .custom-control-label::before {\n  box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25); }\n\n.was-validated .custom-control-input:invalid:focus:not(:checked) ~ .custom-control-label::before, .custom-control-input.is-invalid:focus:not(:checked) ~ .custom-control-label::before {\n  border-color: #dc3545; }\n\n.was-validated .custom-file-input:invalid ~ .custom-file-label, .custom-file-input.is-invalid ~ .custom-file-label {\n  border-color: #dc3545; }\n\n.was-validated .custom-file-input:invalid ~ .invalid-feedback,\n.was-validated .custom-file-input:invalid ~ .invalid-tooltip, .custom-file-input.is-invalid ~ .invalid-feedback,\n.custom-file-input.is-invalid ~ .invalid-tooltip {\n  display: block; }\n\n.was-validated .custom-file-input:invalid:focus ~ .custom-file-label, .custom-file-input.is-invalid:focus ~ .custom-file-label {\n  border-color: #dc3545;\n  box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25); }\n\n.form-inline {\n  display: flex;\n  flex-flow: row wrap;\n  align-items: center; }\n  .form-inline .form-check {\n    width: 100%; }\n  @media (min-width: 576px) {\n    .form-inline label {\n      display: flex;\n      align-items: center;\n      justify-content: center;\n      margin-bottom: 0; }\n    .form-inline .form-group {\n      display: flex;\n      flex: 0 0 auto;\n      flex-flow: row wrap;\n      align-items: center;\n      margin-bottom: 0; }\n    .form-inline .form-control {\n      display: inline-block;\n      width: auto;\n      vertical-align: middle; }\n    .form-inline .form-control-plaintext {\n      display: inline-block; }\n    .form-inline .input-group,\n    .form-inline .custom-select {\n      width: auto; }\n    .form-inline .form-check {\n      display: flex;\n      align-items: center;\n      justify-content: center;\n      width: auto;\n      padding-left: 0; }\n    .form-inline .form-check-input {\n      position: relative;\n      flex-shrink: 0;\n      margin-top: 0;\n      margin-right: 0.25rem;\n      margin-left: 0; }\n    .form-inline .custom-control {\n      align-items: center;\n      justify-content: center; }\n    .form-inline .custom-control-label {\n      margin-bottom: 0; } }\n\n.popover {\n  position: absolute;\n  top: 0;\n  left: 0;\n  z-index: 1060;\n  display: block;\n  max-width: 276px;\n  font-family: -apple-system, BlinkMacSystemFont, \"Segoe UI\", Roboto, \"Helvetica Neue\", Arial, \"Noto Sans\", sans-serif, \"Apple Color Emoji\", \"Segoe UI Emoji\", \"Segoe UI Symbol\", \"Noto Color Emoji\";\n  font-style: normal;\n  font-weight: 400;\n  line-height: 1.5;\n  text-align: left;\n  text-align: start;\n  text-decoration: none;\n  text-shadow: none;\n  text-transform: none;\n  letter-spacing: normal;\n  word-break: normal;\n  word-spacing: normal;\n  white-space: normal;\n  line-break: auto;\n  font-size: 0.875rem;\n  word-wrap: break-word;\n  background-color: #fff;\n  background-clip: padding-box;\n  border: 1px solid rgba(0, 0, 0, 0.2);\n  border-radius: 0.3rem; }\n  .popover .arrow {\n    position: absolute;\n    display: block;\n    width: 1rem;\n    height: 0.5rem;\n    margin: 0 0.3rem; }\n    .popover .arrow::before, .popover .arrow::after {\n      position: absolute;\n      display: block;\n      content: \"\";\n      border-color: transparent;\n      border-style: solid; }\n\n.bs-popover-top, .bs-popover-auto[x-placement^=\"top\"] {\n  margin-bottom: 0.5rem; }\n  .bs-popover-top > .arrow, .bs-popover-auto[x-placement^=\"top\"] > .arrow {\n    bottom: calc((0.5rem + 1px) * -1); }\n    .bs-popover-top > .arrow::before, .bs-popover-auto[x-placement^=\"top\"] > .arrow::before {\n      bottom: 0;\n      border-width: 0.5rem 0.5rem 0;\n      border-top-color: rgba(0, 0, 0, 0.25); }\n    .bs-popover-top > .arrow::after, .bs-popover-auto[x-placement^=\"top\"] > .arrow::after {\n      bottom: 1px;\n      border-width: 0.5rem 0.5rem 0;\n      border-top-color: #fff; }\n\n.bs-popover-right, .bs-popover-auto[x-placement^=\"right\"] {\n  margin-left: 0.5rem; }\n  .bs-popover-right > .arrow, .bs-popover-auto[x-placement^=\"right\"] > .arrow {\n    left: calc((0.5rem + 1px) * -1);\n    width: 0.5rem;\n    height: 1rem;\n    margin: 0.3rem 0; }\n    .bs-popover-right > .arrow::before, .bs-popover-auto[x-placement^=\"right\"] > .arrow::before {\n      left: 0;\n      border-width: 0.5rem 0.5rem 0.5rem 0;\n      border-right-color: rgba(0, 0, 0, 0.25); }\n    .bs-popover-right > .arrow::after, .bs-popover-auto[x-placement^=\"right\"] > .arrow::after {\n      left: 1px;\n      border-width: 0.5rem 0.5rem 0.5rem 0;\n      border-right-color: #fff; }\n\n.bs-popover-bottom, .bs-popover-auto[x-placement^=\"bottom\"] {\n  margin-top: 0.5rem; }\n  .bs-popover-bottom > .arrow, .bs-popover-auto[x-placement^=\"bottom\"] > .arrow {\n    top: calc((0.5rem + 1px) * -1); }\n    .bs-popover-bottom > .arrow::before, .bs-popover-auto[x-placement^=\"bottom\"] > .arrow::before {\n      top: 0;\n      border-width: 0 0.5rem 0.5rem 0.5rem;\n      border-bottom-color: rgba(0, 0, 0, 0.25); }\n    .bs-popover-bottom > .arrow::after, .bs-popover-auto[x-placement^=\"bottom\"] > .arrow::after {\n      top: 1px;\n      border-width: 0 0.5rem 0.5rem 0.5rem;\n      border-bottom-color: #fff; }\n  .bs-popover-bottom .popover-header::before, .bs-popover-auto[x-placement^=\"bottom\"] .popover-header::before {\n    position: absolute;\n    top: 0;\n    left: 50%;\n    display: block;\n    width: 1rem;\n    margin-left: -0.5rem;\n    content: \"\";\n    border-bottom: 1px solid #f7f7f7; }\n\n.bs-popover-left, .bs-popover-auto[x-placement^=\"left\"] {\n  margin-right: 0.5rem; }\n  .bs-popover-left > .arrow, .bs-popover-auto[x-placement^=\"left\"] > .arrow {\n    right: calc((0.5rem + 1px) * -1);\n    width: 0.5rem;\n    height: 1rem;\n    margin: 0.3rem 0; }\n    .bs-popover-left > .arrow::before, .bs-popover-auto[x-placement^=\"left\"] > .arrow::before {\n      right: 0;\n      border-width: 0.5rem 0 0.5rem 0.5rem;\n      border-left-color: rgba(0, 0, 0, 0.25); }\n    .bs-popover-left > .arrow::after, .bs-popover-auto[x-placement^=\"left\"] > .arrow::after {\n      right: 1px;\n      border-width: 0.5rem 0 0.5rem 0.5rem;\n      border-left-color: #fff; }\n\n.popover-header {\n  padding: 0.5rem 0.75rem;\n  margin-bottom: 0;\n  font-size: 1rem;\n  background-color: #f7f7f7;\n  border-bottom: 1px solid #ebebeb;\n  border-top-left-radius: calc(0.3rem - 1px);\n  border-top-right-radius: calc(0.3rem - 1px); }\n  .popover-header:empty {\n    display: none; }\n\n.popover-body {\n  padding: 0.5rem 0.75rem;\n  color: #212529; }\n\n@keyframes progress-bar-stripes {\n  from {\n    background-position: 1rem 0; }\n  to {\n    background-position: 0 0; } }\n\n.progress {\n  display: flex;\n  height: 1rem;\n  overflow: hidden;\n  font-size: 0.75rem;\n  background-color: #e9ecef;\n  border-radius: 0.25rem; }\n\n.progress-bar {\n  display: flex;\n  flex-direction: column;\n  justify-content: center;\n  color: #fff;\n  text-align: center;\n  white-space: nowrap;\n  background-color: #007bff;\n  transition: width 0.6s ease; }\n  @media (prefers-reduced-motion: reduce) {\n    .progress-bar {\n      transition: none; } }\n\n.progress-bar-striped {\n  background-image: linear-gradient(45deg, rgba(255, 255, 255, 0.15) 25%, transparent 25%, transparent 50%, rgba(255, 255, 255, 0.15) 50%, rgba(255, 255, 255, 0.15) 75%, transparent 75%, transparent);\n  background-size: 1rem 1rem; }\n\n.progress-bar-animated {\n  animation: progress-bar-stripes 1s linear infinite; }\n  @media (prefers-reduced-motion: reduce) {\n    .progress-bar-animated {\n      animation: none; } }\n\n.d-none {\n  display: none !important; }\n\n.d-inline {\n  display: inline !important; }\n\n.d-inline-block {\n  display: inline-block !important; }\n\n.d-block {\n  display: block !important; }\n\n.d-table {\n  display: table !important; }\n\n.d-table-row {\n  display: table-row !important; }\n\n.d-table-cell {\n  display: table-cell !important; }\n\n.d-flex {\n  display: flex !important; }\n\n.d-inline-flex {\n  display: inline-flex !important; }\n\n@media (min-width: 576px) {\n  .d-sm-none {\n    display: none !important; }\n  .d-sm-inline {\n    display: inline !important; }\n  .d-sm-inline-block {\n    display: inline-block !important; }\n  .d-sm-block {\n    display: block !important; }\n  .d-sm-table {\n    display: table !important; }\n  .d-sm-table-row {\n    display: table-row !important; }\n  .d-sm-table-cell {\n    display: table-cell !important; }\n  .d-sm-flex {\n    display: flex !important; }\n  .d-sm-inline-flex {\n    display: inline-flex !important; } }\n\n@media (min-width: 768px) {\n  .d-md-none {\n    display: none !important; }\n  .d-md-inline {\n    display: inline !important; }\n  .d-md-inline-block {\n    display: inline-block !important; }\n  .d-md-block {\n    display: block !important; }\n  .d-md-table {\n    display: table !important; }\n  .d-md-table-row {\n    display: table-row !important; }\n  .d-md-table-cell {\n    display: table-cell !important; }\n  .d-md-flex {\n    display: flex !important; }\n  .d-md-inline-flex {\n    display: inline-flex !important; } }\n\n@media (min-width: 992px) {\n  .d-lg-none {\n    display: none !important; }\n  .d-lg-inline {\n    display: inline !important; }\n  .d-lg-inline-block {\n    display: inline-block !important; }\n  .d-lg-block {\n    display: block !important; }\n  .d-lg-table {\n    display: table !important; }\n  .d-lg-table-row {\n    display: table-row !important; }\n  .d-lg-table-cell {\n    display: table-cell !important; }\n  .d-lg-flex {\n    display: flex !important; }\n  .d-lg-inline-flex {\n    display: inline-flex !important; } }\n\n@media (min-width: 1200px) {\n  .d-xl-none {\n    display: none !important; }\n  .d-xl-inline {\n    display: inline !important; }\n  .d-xl-inline-block {\n    display: inline-block !important; }\n  .d-xl-block {\n    display: block !important; }\n  .d-xl-table {\n    display: table !important; }\n  .d-xl-table-row {\n    display: table-row !important; }\n  .d-xl-table-cell {\n    display: table-cell !important; }\n  .d-xl-flex {\n    display: flex !important; }\n  .d-xl-inline-flex {\n    display: inline-flex !important; } }\n\n@media print {\n  .d-print-none {\n    display: none !important; }\n  .d-print-inline {\n    display: inline !important; }\n  .d-print-inline-block {\n    display: inline-block !important; }\n  .d-print-block {\n    display: block !important; }\n  .d-print-table {\n    display: table !important; }\n  .d-print-table-row {\n    display: table-row !important; }\n  .d-print-table-cell {\n    display: table-cell !important; }\n  .d-print-flex {\n    display: flex !important; }\n  .d-print-inline-flex {\n    display: inline-flex !important; } }\n\n.flex-row {\n  flex-direction: row !important; }\n\n.flex-column {\n  flex-direction: column !important; }\n\n.flex-row-reverse {\n  flex-direction: row-reverse !important; }\n\n.flex-column-reverse {\n  flex-direction: column-reverse !important; }\n\n.flex-wrap {\n  flex-wrap: wrap !important; }\n\n.flex-nowrap {\n  flex-wrap: nowrap !important; }\n\n.flex-wrap-reverse {\n  flex-wrap: wrap-reverse !important; }\n\n.flex-fill {\n  flex: 1 1 auto !important; }\n\n.flex-grow-0 {\n  flex-grow: 0 !important; }\n\n.flex-grow-1 {\n  flex-grow: 1 !important; }\n\n.flex-shrink-0 {\n  flex-shrink: 0 !important; }\n\n.flex-shrink-1 {\n  flex-shrink: 1 !important; }\n\n.justify-content-start {\n  justify-content: flex-start !important; }\n\n.justify-content-end {\n  justify-content: flex-end !important; }\n\n.justify-content-center {\n  justify-content: center !important; }\n\n.justify-content-between {\n  justify-content: space-between !important; }\n\n.justify-content-around {\n  justify-content: space-around !important; }\n\n.align-items-start {\n  align-items: flex-start !important; }\n\n.align-items-end {\n  align-items: flex-end !important; }\n\n.align-items-center {\n  align-items: center !important; }\n\n.align-items-baseline {\n  align-items: baseline !important; }\n\n.align-items-stretch {\n  align-items: stretch !important; }\n\n.align-content-start {\n  align-content: flex-start !important; }\n\n.align-content-end {\n  align-content: flex-end !important; }\n\n.align-content-center {\n  align-content: center !important; }\n\n.align-content-between {\n  align-content: space-between !important; }\n\n.align-content-around {\n  align-content: space-around !important; }\n\n.align-content-stretch {\n  align-content: stretch !important; }\n\n.align-self-auto {\n  align-self: auto !important; }\n\n.align-self-start {\n  align-self: flex-start !important; }\n\n.align-self-end {\n  align-self: flex-end !important; }\n\n.align-self-center {\n  align-self: center !important; }\n\n.align-self-baseline {\n  align-self: baseline !important; }\n\n.align-self-stretch {\n  align-self: stretch !important; }\n\n@media (min-width: 576px) {\n  .flex-sm-row {\n    flex-direction: row !important; }\n  .flex-sm-column {\n    flex-direction: column !important; }\n  .flex-sm-row-reverse {\n    flex-direction: row-reverse !important; }\n  .flex-sm-column-reverse {\n    flex-direction: column-reverse !important; }\n  .flex-sm-wrap {\n    flex-wrap: wrap !important; }\n  .flex-sm-nowrap {\n    flex-wrap: nowrap !important; }\n  .flex-sm-wrap-reverse {\n    flex-wrap: wrap-reverse !important; }\n  .flex-sm-fill {\n    flex: 1 1 auto !important; }\n  .flex-sm-grow-0 {\n    flex-grow: 0 !important; }\n  .flex-sm-grow-1 {\n    flex-grow: 1 !important; }\n  .flex-sm-shrink-0 {\n    flex-shrink: 0 !important; }\n  .flex-sm-shrink-1 {\n    flex-shrink: 1 !important; }\n  .justify-content-sm-start {\n    justify-content: flex-start !important; }\n  .justify-content-sm-end {\n    justify-content: flex-end !important; }\n  .justify-content-sm-center {\n    justify-content: center !important; }\n  .justify-content-sm-between {\n    justify-content: space-between !important; }\n  .justify-content-sm-around {\n    justify-content: space-around !important; }\n  .align-items-sm-start {\n    align-items: flex-start !important; }\n  .align-items-sm-end {\n    align-items: flex-end !important; }\n  .align-items-sm-center {\n    align-items: center !important; }\n  .align-items-sm-baseline {\n    align-items: baseline !important; }\n  .align-items-sm-stretch {\n    align-items: stretch !important; }\n  .align-content-sm-start {\n    align-content: flex-start !important; }\n  .align-content-sm-end {\n    align-content: flex-end !important; }\n  .align-content-sm-center {\n    align-content: center !important; }\n  .align-content-sm-between {\n    align-content: space-between !important; }\n  .align-content-sm-around {\n    align-content: space-around !important; }\n  .align-content-sm-stretch {\n    align-content: stretch !important; }\n  .align-self-sm-auto {\n    align-self: auto !important; }\n  .align-self-sm-start {\n    align-self: flex-start !important; }\n  .align-self-sm-end {\n    align-self: flex-end !important; }\n  .align-self-sm-center {\n    align-self: center !important; }\n  .align-self-sm-baseline {\n    align-self: baseline !important; }\n  .align-self-sm-stretch {\n    align-self: stretch !important; } }\n\n@media (min-width: 768px) {\n  .flex-md-row {\n    flex-direction: row !important; }\n  .flex-md-column {\n    flex-direction: column !important; }\n  .flex-md-row-reverse {\n    flex-direction: row-reverse !important; }\n  .flex-md-column-reverse {\n    flex-direction: column-reverse !important; }\n  .flex-md-wrap {\n    flex-wrap: wrap !important; }\n  .flex-md-nowrap {\n    flex-wrap: nowrap !important; }\n  .flex-md-wrap-reverse {\n    flex-wrap: wrap-reverse !important; }\n  .flex-md-fill {\n    flex: 1 1 auto !important; }\n  .flex-md-grow-0 {\n    flex-grow: 0 !important; }\n  .flex-md-grow-1 {\n    flex-grow: 1 !important; }\n  .flex-md-shrink-0 {\n    flex-shrink: 0 !important; }\n  .flex-md-shrink-1 {\n    flex-shrink: 1 !important; }\n  .justify-content-md-start {\n    justify-content: flex-start !important; }\n  .justify-content-md-end {\n    justify-content: flex-end !important; }\n  .justify-content-md-center {\n    justify-content: center !important; }\n  .justify-content-md-between {\n    justify-content: space-between !important; }\n  .justify-content-md-around {\n    justify-content: space-around !important; }\n  .align-items-md-start {\n    align-items: flex-start !important; }\n  .align-items-md-end {\n    align-items: flex-end !important; }\n  .align-items-md-center {\n    align-items: center !important; }\n  .align-items-md-baseline {\n    align-items: baseline !important; }\n  .align-items-md-stretch {\n    align-items: stretch !important; }\n  .align-content-md-start {\n    align-content: flex-start !important; }\n  .align-content-md-end {\n    align-content: flex-end !important; }\n  .align-content-md-center {\n    align-content: center !important; }\n  .align-content-md-between {\n    align-content: space-between !important; }\n  .align-content-md-around {\n    align-content: space-around !important; }\n  .align-content-md-stretch {\n    align-content: stretch !important; }\n  .align-self-md-auto {\n    align-self: auto !important; }\n  .align-self-md-start {\n    align-self: flex-start !important; }\n  .align-self-md-end {\n    align-self: flex-end !important; }\n  .align-self-md-center {\n    align-self: center !important; }\n  .align-self-md-baseline {\n    align-self: baseline !important; }\n  .align-self-md-stretch {\n    align-self: stretch !important; } }\n\n@media (min-width: 992px) {\n  .flex-lg-row {\n    flex-direction: row !important; }\n  .flex-lg-column {\n    flex-direction: column !important; }\n  .flex-lg-row-reverse {\n    flex-direction: row-reverse !important; }\n  .flex-lg-column-reverse {\n    flex-direction: column-reverse !important; }\n  .flex-lg-wrap {\n    flex-wrap: wrap !important; }\n  .flex-lg-nowrap {\n    flex-wrap: nowrap !important; }\n  .flex-lg-wrap-reverse {\n    flex-wrap: wrap-reverse !important; }\n  .flex-lg-fill {\n    flex: 1 1 auto !important; }\n  .flex-lg-grow-0 {\n    flex-grow: 0 !important; }\n  .flex-lg-grow-1 {\n    flex-grow: 1 !important; }\n  .flex-lg-shrink-0 {\n    flex-shrink: 0 !important; }\n  .flex-lg-shrink-1 {\n    flex-shrink: 1 !important; }\n  .justify-content-lg-start {\n    justify-content: flex-start !important; }\n  .justify-content-lg-end {\n    justify-content: flex-end !important; }\n  .justify-content-lg-center {\n    justify-content: center !important; }\n  .justify-content-lg-between {\n    justify-content: space-between !important; }\n  .justify-content-lg-around {\n    justify-content: space-around !important; }\n  .align-items-lg-start {\n    align-items: flex-start !important; }\n  .align-items-lg-end {\n    align-items: flex-end !important; }\n  .align-items-lg-center {\n    align-items: center !important; }\n  .align-items-lg-baseline {\n    align-items: baseline !important; }\n  .align-items-lg-stretch {\n    align-items: stretch !important; }\n  .align-content-lg-start {\n    align-content: flex-start !important; }\n  .align-content-lg-end {\n    align-content: flex-end !important; }\n  .align-content-lg-center {\n    align-content: center !important; }\n  .align-content-lg-between {\n    align-content: space-between !important; }\n  .align-content-lg-around {\n    align-content: space-around !important; }\n  .align-content-lg-stretch {\n    align-content: stretch !important; }\n  .align-self-lg-auto {\n    align-self: auto !important; }\n  .align-self-lg-start {\n    align-self: flex-start !important; }\n  .align-self-lg-end {\n    align-self: flex-end !important; }\n  .align-self-lg-center {\n    align-self: center !important; }\n  .align-self-lg-baseline {\n    align-self: baseline !important; }\n  .align-self-lg-stretch {\n    align-self: stretch !important; } }\n\n@media (min-width: 1200px) {\n  .flex-xl-row {\n    flex-direction: row !important; }\n  .flex-xl-column {\n    flex-direction: column !important; }\n  .flex-xl-row-reverse {\n    flex-direction: row-reverse !important; }\n  .flex-xl-column-reverse {\n    flex-direction: column-reverse !important; }\n  .flex-xl-wrap {\n    flex-wrap: wrap !important; }\n  .flex-xl-nowrap {\n    flex-wrap: nowrap !important; }\n  .flex-xl-wrap-reverse {\n    flex-wrap: wrap-reverse !important; }\n  .flex-xl-fill {\n    flex: 1 1 auto !important; }\n  .flex-xl-grow-0 {\n    flex-grow: 0 !important; }\n  .flex-xl-grow-1 {\n    flex-grow: 1 !important; }\n  .flex-xl-shrink-0 {\n    flex-shrink: 0 !important; }\n  .flex-xl-shrink-1 {\n    flex-shrink: 1 !important; }\n  .justify-content-xl-start {\n    justify-content: flex-start !important; }\n  .justify-content-xl-end {\n    justify-content: flex-end !important; }\n  .justify-content-xl-center {\n    justify-content: center !important; }\n  .justify-content-xl-between {\n    justify-content: space-between !important; }\n  .justify-content-xl-around {\n    justify-content: space-around !important; }\n  .align-items-xl-start {\n    align-items: flex-start !important; }\n  .align-items-xl-end {\n    align-items: flex-end !important; }\n  .align-items-xl-center {\n    align-items: center !important; }\n  .align-items-xl-baseline {\n    align-items: baseline !important; }\n  .align-items-xl-stretch {\n    align-items: stretch !important; }\n  .align-content-xl-start {\n    align-content: flex-start !important; }\n  .align-content-xl-end {\n    align-content: flex-end !important; }\n  .align-content-xl-center {\n    align-content: center !important; }\n  .align-content-xl-between {\n    align-content: space-between !important; }\n  .align-content-xl-around {\n    align-content: space-around !important; }\n  .align-content-xl-stretch {\n    align-content: stretch !important; }\n  .align-self-xl-auto {\n    align-self: auto !important; }\n  .align-self-xl-start {\n    align-self: flex-start !important; }\n  .align-self-xl-end {\n    align-self: flex-end !important; }\n  .align-self-xl-center {\n    align-self: center !important; }\n  .align-self-xl-baseline {\n    align-self: baseline !important; }\n  .align-self-xl-stretch {\n    align-self: stretch !important; } }\n\n.m-0 {\n  margin: 0 !important; }\n\n.mt-0,\n.my-0 {\n  margin-top: 0 !important; }\n\n.mr-0,\n.mx-0 {\n  margin-right: 0 !important; }\n\n.mb-0,\n.my-0 {\n  margin-bottom: 0 !important; }\n\n.ml-0,\n.mx-0 {\n  margin-left: 0 !important; }\n\n.m-1 {\n  margin: 0.25rem !important; }\n\n.mt-1,\n.my-1 {\n  margin-top: 0.25rem !important; }\n\n.mr-1,\n.mx-1 {\n  margin-right: 0.25rem !important; }\n\n.mb-1,\n.my-1 {\n  margin-bottom: 0.25rem !important; }\n\n.ml-1,\n.mx-1 {\n  margin-left: 0.25rem !important; }\n\n.m-2 {\n  margin: 0.5rem !important; }\n\n.mt-2,\n.my-2 {\n  margin-top: 0.5rem !important; }\n\n.mr-2,\n.mx-2 {\n  margin-right: 0.5rem !important; }\n\n.mb-2,\n.my-2 {\n  margin-bottom: 0.5rem !important; }\n\n.ml-2,\n.mx-2 {\n  margin-left: 0.5rem !important; }\n\n.m-3 {\n  margin: 1rem !important; }\n\n.mt-3,\n.my-3 {\n  margin-top: 1rem !important; }\n\n.mr-3,\n.mx-3 {\n  margin-right: 1rem !important; }\n\n.mb-3,\n.my-3 {\n  margin-bottom: 1rem !important; }\n\n.ml-3,\n.mx-3 {\n  margin-left: 1rem !important; }\n\n.m-4 {\n  margin: 1.5rem !important; }\n\n.mt-4,\n.my-4 {\n  margin-top: 1.5rem !important; }\n\n.mr-4,\n.mx-4 {\n  margin-right: 1.5rem !important; }\n\n.mb-4,\n.my-4 {\n  margin-bottom: 1.5rem !important; }\n\n.ml-4,\n.mx-4 {\n  margin-left: 1.5rem !important; }\n\n.m-5 {\n  margin: 3rem !important; }\n\n.mt-5,\n.my-5 {\n  margin-top: 3rem !important; }\n\n.mr-5,\n.mx-5 {\n  margin-right: 3rem !important; }\n\n.mb-5,\n.my-5 {\n  margin-bottom: 3rem !important; }\n\n.ml-5,\n.mx-5 {\n  margin-left: 3rem !important; }\n\n.p-0 {\n  padding: 0 !important; }\n\n.pt-0,\n.py-0 {\n  padding-top: 0 !important; }\n\n.pr-0,\n.px-0 {\n  padding-right: 0 !important; }\n\n.pb-0,\n.py-0 {\n  padding-bottom: 0 !important; }\n\n.pl-0,\n.px-0 {\n  padding-left: 0 !important; }\n\n.p-1 {\n  padding: 0.25rem !important; }\n\n.pt-1,\n.py-1 {\n  padding-top: 0.25rem !important; }\n\n.pr-1,\n.px-1 {\n  padding-right: 0.25rem !important; }\n\n.pb-1,\n.py-1 {\n  padding-bottom: 0.25rem !important; }\n\n.pl-1,\n.px-1 {\n  padding-left: 0.25rem !important; }\n\n.p-2 {\n  padding: 0.5rem !important; }\n\n.pt-2,\n.py-2 {\n  padding-top: 0.5rem !important; }\n\n.pr-2,\n.px-2 {\n  padding-right: 0.5rem !important; }\n\n.pb-2,\n.py-2 {\n  padding-bottom: 0.5rem !important; }\n\n.pl-2,\n.px-2 {\n  padding-left: 0.5rem !important; }\n\n.p-3 {\n  padding: 1rem !important; }\n\n.pt-3,\n.py-3 {\n  padding-top: 1rem !important; }\n\n.pr-3,\n.px-3 {\n  padding-right: 1rem !important; }\n\n.pb-3,\n.py-3 {\n  padding-bottom: 1rem !important; }\n\n.pl-3,\n.px-3 {\n  padding-left: 1rem !important; }\n\n.p-4 {\n  padding: 1.5rem !important; }\n\n.pt-4,\n.py-4 {\n  padding-top: 1.5rem !important; }\n\n.pr-4,\n.px-4 {\n  padding-right: 1.5rem !important; }\n\n.pb-4,\n.py-4 {\n  padding-bottom: 1.5rem !important; }\n\n.pl-4,\n.px-4 {\n  padding-left: 1.5rem !important; }\n\n.p-5 {\n  padding: 3rem !important; }\n\n.pt-5,\n.py-5 {\n  padding-top: 3rem !important; }\n\n.pr-5,\n.px-5 {\n  padding-right: 3rem !important; }\n\n.pb-5,\n.py-5 {\n  padding-bottom: 3rem !important; }\n\n.pl-5,\n.px-5 {\n  padding-left: 3rem !important; }\n\n.m-n1 {\n  margin: -0.25rem !important; }\n\n.mt-n1,\n.my-n1 {\n  margin-top: -0.25rem !important; }\n\n.mr-n1,\n.mx-n1 {\n  margin-right: -0.25rem !important; }\n\n.mb-n1,\n.my-n1 {\n  margin-bottom: -0.25rem !important; }\n\n.ml-n1,\n.mx-n1 {\n  margin-left: -0.25rem !important; }\n\n.m-n2 {\n  margin: -0.5rem !important; }\n\n.mt-n2,\n.my-n2 {\n  margin-top: -0.5rem !important; }\n\n.mr-n2,\n.mx-n2 {\n  margin-right: -0.5rem !important; }\n\n.mb-n2,\n.my-n2 {\n  margin-bottom: -0.5rem !important; }\n\n.ml-n2,\n.mx-n2 {\n  margin-left: -0.5rem !important; }\n\n.m-n3 {\n  margin: -1rem !important; }\n\n.mt-n3,\n.my-n3 {\n  margin-top: -1rem !important; }\n\n.mr-n3,\n.mx-n3 {\n  margin-right: -1rem !important; }\n\n.mb-n3,\n.my-n3 {\n  margin-bottom: -1rem !important; }\n\n.ml-n3,\n.mx-n3 {\n  margin-left: -1rem !important; }\n\n.m-n4 {\n  margin: -1.5rem !important; }\n\n.mt-n4,\n.my-n4 {\n  margin-top: -1.5rem !important; }\n\n.mr-n4,\n.mx-n4 {\n  margin-right: -1.5rem !important; }\n\n.mb-n4,\n.my-n4 {\n  margin-bottom: -1.5rem !important; }\n\n.ml-n4,\n.mx-n4 {\n  margin-left: -1.5rem !important; }\n\n.m-n5 {\n  margin: -3rem !important; }\n\n.mt-n5,\n.my-n5 {\n  margin-top: -3rem !important; }\n\n.mr-n5,\n.mx-n5 {\n  margin-right: -3rem !important; }\n\n.mb-n5,\n.my-n5 {\n  margin-bottom: -3rem !important; }\n\n.ml-n5,\n.mx-n5 {\n  margin-left: -3rem !important; }\n\n.m-auto {\n  margin: auto !important; }\n\n.mt-auto,\n.my-auto {\n  margin-top: auto !important; }\n\n.mr-auto,\n.mx-auto {\n  margin-right: auto !important; }\n\n.mb-auto,\n.my-auto {\n  margin-bottom: auto !important; }\n\n.ml-auto,\n.mx-auto {\n  margin-left: auto !important; }\n\n@media (min-width: 576px) {\n  .m-sm-0 {\n    margin: 0 !important; }\n  .mt-sm-0,\n  .my-sm-0 {\n    margin-top: 0 !important; }\n  .mr-sm-0,\n  .mx-sm-0 {\n    margin-right: 0 !important; }\n  .mb-sm-0,\n  .my-sm-0 {\n    margin-bottom: 0 !important; }\n  .ml-sm-0,\n  .mx-sm-0 {\n    margin-left: 0 !important; }\n  .m-sm-1 {\n    margin: 0.25rem !important; }\n  .mt-sm-1,\n  .my-sm-1 {\n    margin-top: 0.25rem !important; }\n  .mr-sm-1,\n  .mx-sm-1 {\n    margin-right: 0.25rem !important; }\n  .mb-sm-1,\n  .my-sm-1 {\n    margin-bottom: 0.25rem !important; }\n  .ml-sm-1,\n  .mx-sm-1 {\n    margin-left: 0.25rem !important; }\n  .m-sm-2 {\n    margin: 0.5rem !important; }\n  .mt-sm-2,\n  .my-sm-2 {\n    margin-top: 0.5rem !important; }\n  .mr-sm-2,\n  .mx-sm-2 {\n    margin-right: 0.5rem !important; }\n  .mb-sm-2,\n  .my-sm-2 {\n    margin-bottom: 0.5rem !important; }\n  .ml-sm-2,\n  .mx-sm-2 {\n    margin-left: 0.5rem !important; }\n  .m-sm-3 {\n    margin: 1rem !important; }\n  .mt-sm-3,\n  .my-sm-3 {\n    margin-top: 1rem !important; }\n  .mr-sm-3,\n  .mx-sm-3 {\n    margin-right: 1rem !important; }\n  .mb-sm-3,\n  .my-sm-3 {\n    margin-bottom: 1rem !important; }\n  .ml-sm-3,\n  .mx-sm-3 {\n    margin-left: 1rem !important; }\n  .m-sm-4 {\n    margin: 1.5rem !important; }\n  .mt-sm-4,\n  .my-sm-4 {\n    margin-top: 1.5rem !important; }\n  .mr-sm-4,\n  .mx-sm-4 {\n    margin-right: 1.5rem !important; }\n  .mb-sm-4,\n  .my-sm-4 {\n    margin-bottom: 1.5rem !important; }\n  .ml-sm-4,\n  .mx-sm-4 {\n    margin-left: 1.5rem !important; }\n  .m-sm-5 {\n    margin: 3rem !important; }\n  .mt-sm-5,\n  .my-sm-5 {\n    margin-top: 3rem !important; }\n  .mr-sm-5,\n  .mx-sm-5 {\n    margin-right: 3rem !important; }\n  .mb-sm-5,\n  .my-sm-5 {\n    margin-bottom: 3rem !important; }\n  .ml-sm-5,\n  .mx-sm-5 {\n    margin-left: 3rem !important; }\n  .p-sm-0 {\n    padding: 0 !important; }\n  .pt-sm-0,\n  .py-sm-0 {\n    padding-top: 0 !important; }\n  .pr-sm-0,\n  .px-sm-0 {\n    padding-right: 0 !important; }\n  .pb-sm-0,\n  .py-sm-0 {\n    padding-bottom: 0 !important; }\n  .pl-sm-0,\n  .px-sm-0 {\n    padding-left: 0 !important; }\n  .p-sm-1 {\n    padding: 0.25rem !important; }\n  .pt-sm-1,\n  .py-sm-1 {\n    padding-top: 0.25rem !important; }\n  .pr-sm-1,\n  .px-sm-1 {\n    padding-right: 0.25rem !important; }\n  .pb-sm-1,\n  .py-sm-1 {\n    padding-bottom: 0.25rem !important; }\n  .pl-sm-1,\n  .px-sm-1 {\n    padding-left: 0.25rem !important; }\n  .p-sm-2 {\n    padding: 0.5rem !important; }\n  .pt-sm-2,\n  .py-sm-2 {\n    padding-top: 0.5rem !important; }\n  .pr-sm-2,\n  .px-sm-2 {\n    padding-right: 0.5rem !important; }\n  .pb-sm-2,\n  .py-sm-2 {\n    padding-bottom: 0.5rem !important; }\n  .pl-sm-2,\n  .px-sm-2 {\n    padding-left: 0.5rem !important; }\n  .p-sm-3 {\n    padding: 1rem !important; }\n  .pt-sm-3,\n  .py-sm-3 {\n    padding-top: 1rem !important; }\n  .pr-sm-3,\n  .px-sm-3 {\n    padding-right: 1rem !important; }\n  .pb-sm-3,\n  .py-sm-3 {\n    padding-bottom: 1rem !important; }\n  .pl-sm-3,\n  .px-sm-3 {\n    padding-left: 1rem !important; }\n  .p-sm-4 {\n    padding: 1.5rem !important; }\n  .pt-sm-4,\n  .py-sm-4 {\n    padding-top: 1.5rem !important; }\n  .pr-sm-4,\n  .px-sm-4 {\n    padding-right: 1.5rem !important; }\n  .pb-sm-4,\n  .py-sm-4 {\n    padding-bottom: 1.5rem !important; }\n  .pl-sm-4,\n  .px-sm-4 {\n    padding-left: 1.5rem !important; }\n  .p-sm-5 {\n    padding: 3rem !important; }\n  .pt-sm-5,\n  .py-sm-5 {\n    padding-top: 3rem !important; }\n  .pr-sm-5,\n  .px-sm-5 {\n    padding-right: 3rem !important; }\n  .pb-sm-5,\n  .py-sm-5 {\n    padding-bottom: 3rem !important; }\n  .pl-sm-5,\n  .px-sm-5 {\n    padding-left: 3rem !important; }\n  .m-sm-n1 {\n    margin: -0.25rem !important; }\n  .mt-sm-n1,\n  .my-sm-n1 {\n    margin-top: -0.25rem !important; }\n  .mr-sm-n1,\n  .mx-sm-n1 {\n    margin-right: -0.25rem !important; }\n  .mb-sm-n1,\n  .my-sm-n1 {\n    margin-bottom: -0.25rem !important; }\n  .ml-sm-n1,\n  .mx-sm-n1 {\n    margin-left: -0.25rem !important; }\n  .m-sm-n2 {\n    margin: -0.5rem !important; }\n  .mt-sm-n2,\n  .my-sm-n2 {\n    margin-top: -0.5rem !important; }\n  .mr-sm-n2,\n  .mx-sm-n2 {\n    margin-right: -0.5rem !important; }\n  .mb-sm-n2,\n  .my-sm-n2 {\n    margin-bottom: -0.5rem !important; }\n  .ml-sm-n2,\n  .mx-sm-n2 {\n    margin-left: -0.5rem !important; }\n  .m-sm-n3 {\n    margin: -1rem !important; }\n  .mt-sm-n3,\n  .my-sm-n3 {\n    margin-top: -1rem !important; }\n  .mr-sm-n3,\n  .mx-sm-n3 {\n    margin-right: -1rem !important; }\n  .mb-sm-n3,\n  .my-sm-n3 {\n    margin-bottom: -1rem !important; }\n  .ml-sm-n3,\n  .mx-sm-n3 {\n    margin-left: -1rem !important; }\n  .m-sm-n4 {\n    margin: -1.5rem !important; }\n  .mt-sm-n4,\n  .my-sm-n4 {\n    margin-top: -1.5rem !important; }\n  .mr-sm-n4,\n  .mx-sm-n4 {\n    margin-right: -1.5rem !important; }\n  .mb-sm-n4,\n  .my-sm-n4 {\n    margin-bottom: -1.5rem !important; }\n  .ml-sm-n4,\n  .mx-sm-n4 {\n    margin-left: -1.5rem !important; }\n  .m-sm-n5 {\n    margin: -3rem !important; }\n  .mt-sm-n5,\n  .my-sm-n5 {\n    margin-top: -3rem !important; }\n  .mr-sm-n5,\n  .mx-sm-n5 {\n    margin-right: -3rem !important; }\n  .mb-sm-n5,\n  .my-sm-n5 {\n    margin-bottom: -3rem !important; }\n  .ml-sm-n5,\n  .mx-sm-n5 {\n    margin-left: -3rem !important; }\n  .m-sm-auto {\n    margin: auto !important; }\n  .mt-sm-auto,\n  .my-sm-auto {\n    margin-top: auto !important; }\n  .mr-sm-auto,\n  .mx-sm-auto {\n    margin-right: auto !important; }\n  .mb-sm-auto,\n  .my-sm-auto {\n    margin-bottom: auto !important; }\n  .ml-sm-auto,\n  .mx-sm-auto {\n    margin-left: auto !important; } }\n\n@media (min-width: 768px) {\n  .m-md-0 {\n    margin: 0 !important; }\n  .mt-md-0,\n  .my-md-0 {\n    margin-top: 0 !important; }\n  .mr-md-0,\n  .mx-md-0 {\n    margin-right: 0 !important; }\n  .mb-md-0,\n  .my-md-0 {\n    margin-bottom: 0 !important; }\n  .ml-md-0,\n  .mx-md-0 {\n    margin-left: 0 !important; }\n  .m-md-1 {\n    margin: 0.25rem !important; }\n  .mt-md-1,\n  .my-md-1 {\n    margin-top: 0.25rem !important; }\n  .mr-md-1,\n  .mx-md-1 {\n    margin-right: 0.25rem !important; }\n  .mb-md-1,\n  .my-md-1 {\n    margin-bottom: 0.25rem !important; }\n  .ml-md-1,\n  .mx-md-1 {\n    margin-left: 0.25rem !important; }\n  .m-md-2 {\n    margin: 0.5rem !important; }\n  .mt-md-2,\n  .my-md-2 {\n    margin-top: 0.5rem !important; }\n  .mr-md-2,\n  .mx-md-2 {\n    margin-right: 0.5rem !important; }\n  .mb-md-2,\n  .my-md-2 {\n    margin-bottom: 0.5rem !important; }\n  .ml-md-2,\n  .mx-md-2 {\n    margin-left: 0.5rem !important; }\n  .m-md-3 {\n    margin: 1rem !important; }\n  .mt-md-3,\n  .my-md-3 {\n    margin-top: 1rem !important; }\n  .mr-md-3,\n  .mx-md-3 {\n    margin-right: 1rem !important; }\n  .mb-md-3,\n  .my-md-3 {\n    margin-bottom: 1rem !important; }\n  .ml-md-3,\n  .mx-md-3 {\n    margin-left: 1rem !important; }\n  .m-md-4 {\n    margin: 1.5rem !important; }\n  .mt-md-4,\n  .my-md-4 {\n    margin-top: 1.5rem !important; }\n  .mr-md-4,\n  .mx-md-4 {\n    margin-right: 1.5rem !important; }\n  .mb-md-4,\n  .my-md-4 {\n    margin-bottom: 1.5rem !important; }\n  .ml-md-4,\n  .mx-md-4 {\n    margin-left: 1.5rem !important; }\n  .m-md-5 {\n    margin: 3rem !important; }\n  .mt-md-5,\n  .my-md-5 {\n    margin-top: 3rem !important; }\n  .mr-md-5,\n  .mx-md-5 {\n    margin-right: 3rem !important; }\n  .mb-md-5,\n  .my-md-5 {\n    margin-bottom: 3rem !important; }\n  .ml-md-5,\n  .mx-md-5 {\n    margin-left: 3rem !important; }\n  .p-md-0 {\n    padding: 0 !important; }\n  .pt-md-0,\n  .py-md-0 {\n    padding-top: 0 !important; }\n  .pr-md-0,\n  .px-md-0 {\n    padding-right: 0 !important; }\n  .pb-md-0,\n  .py-md-0 {\n    padding-bottom: 0 !important; }\n  .pl-md-0,\n  .px-md-0 {\n    padding-left: 0 !important; }\n  .p-md-1 {\n    padding: 0.25rem !important; }\n  .pt-md-1,\n  .py-md-1 {\n    padding-top: 0.25rem !important; }\n  .pr-md-1,\n  .px-md-1 {\n    padding-right: 0.25rem !important; }\n  .pb-md-1,\n  .py-md-1 {\n    padding-bottom: 0.25rem !important; }\n  .pl-md-1,\n  .px-md-1 {\n    padding-left: 0.25rem !important; }\n  .p-md-2 {\n    padding: 0.5rem !important; }\n  .pt-md-2,\n  .py-md-2 {\n    padding-top: 0.5rem !important; }\n  .pr-md-2,\n  .px-md-2 {\n    padding-right: 0.5rem !important; }\n  .pb-md-2,\n  .py-md-2 {\n    padding-bottom: 0.5rem !important; }\n  .pl-md-2,\n  .px-md-2 {\n    padding-left: 0.5rem !important; }\n  .p-md-3 {\n    padding: 1rem !important; }\n  .pt-md-3,\n  .py-md-3 {\n    padding-top: 1rem !important; }\n  .pr-md-3,\n  .px-md-3 {\n    padding-right: 1rem !important; }\n  .pb-md-3,\n  .py-md-3 {\n    padding-bottom: 1rem !important; }\n  .pl-md-3,\n  .px-md-3 {\n    padding-left: 1rem !important; }\n  .p-md-4 {\n    padding: 1.5rem !important; }\n  .pt-md-4,\n  .py-md-4 {\n    padding-top: 1.5rem !important; }\n  .pr-md-4,\n  .px-md-4 {\n    padding-right: 1.5rem !important; }\n  .pb-md-4,\n  .py-md-4 {\n    padding-bottom: 1.5rem !important; }\n  .pl-md-4,\n  .px-md-4 {\n    padding-left: 1.5rem !important; }\n  .p-md-5 {\n    padding: 3rem !important; }\n  .pt-md-5,\n  .py-md-5 {\n    padding-top: 3rem !important; }\n  .pr-md-5,\n  .px-md-5 {\n    padding-right: 3rem !important; }\n  .pb-md-5,\n  .py-md-5 {\n    padding-bottom: 3rem !important; }\n  .pl-md-5,\n  .px-md-5 {\n    padding-left: 3rem !important; }\n  .m-md-n1 {\n    margin: -0.25rem !important; }\n  .mt-md-n1,\n  .my-md-n1 {\n    margin-top: -0.25rem !important; }\n  .mr-md-n1,\n  .mx-md-n1 {\n    margin-right: -0.25rem !important; }\n  .mb-md-n1,\n  .my-md-n1 {\n    margin-bottom: -0.25rem !important; }\n  .ml-md-n1,\n  .mx-md-n1 {\n    margin-left: -0.25rem !important; }\n  .m-md-n2 {\n    margin: -0.5rem !important; }\n  .mt-md-n2,\n  .my-md-n2 {\n    margin-top: -0.5rem !important; }\n  .mr-md-n2,\n  .mx-md-n2 {\n    margin-right: -0.5rem !important; }\n  .mb-md-n2,\n  .my-md-n2 {\n    margin-bottom: -0.5rem !important; }\n  .ml-md-n2,\n  .mx-md-n2 {\n    margin-left: -0.5rem !important; }\n  .m-md-n3 {\n    margin: -1rem !important; }\n  .mt-md-n3,\n  .my-md-n3 {\n    margin-top: -1rem !important; }\n  .mr-md-n3,\n  .mx-md-n3 {\n    margin-right: -1rem !important; }\n  .mb-md-n3,\n  .my-md-n3 {\n    margin-bottom: -1rem !important; }\n  .ml-md-n3,\n  .mx-md-n3 {\n    margin-left: -1rem !important; }\n  .m-md-n4 {\n    margin: -1.5rem !important; }\n  .mt-md-n4,\n  .my-md-n4 {\n    margin-top: -1.5rem !important; }\n  .mr-md-n4,\n  .mx-md-n4 {\n    margin-right: -1.5rem !important; }\n  .mb-md-n4,\n  .my-md-n4 {\n    margin-bottom: -1.5rem !important; }\n  .ml-md-n4,\n  .mx-md-n4 {\n    margin-left: -1.5rem !important; }\n  .m-md-n5 {\n    margin: -3rem !important; }\n  .mt-md-n5,\n  .my-md-n5 {\n    margin-top: -3rem !important; }\n  .mr-md-n5,\n  .mx-md-n5 {\n    margin-right: -3rem !important; }\n  .mb-md-n5,\n  .my-md-n5 {\n    margin-bottom: -3rem !important; }\n  .ml-md-n5,\n  .mx-md-n5 {\n    margin-left: -3rem !important; }\n  .m-md-auto {\n    margin: auto !important; }\n  .mt-md-auto,\n  .my-md-auto {\n    margin-top: auto !important; }\n  .mr-md-auto,\n  .mx-md-auto {\n    margin-right: auto !important; }\n  .mb-md-auto,\n  .my-md-auto {\n    margin-bottom: auto !important; }\n  .ml-md-auto,\n  .mx-md-auto {\n    margin-left: auto !important; } }\n\n@media (min-width: 992px) {\n  .m-lg-0 {\n    margin: 0 !important; }\n  .mt-lg-0,\n  .my-lg-0 {\n    margin-top: 0 !important; }\n  .mr-lg-0,\n  .mx-lg-0 {\n    margin-right: 0 !important; }\n  .mb-lg-0,\n  .my-lg-0 {\n    margin-bottom: 0 !important; }\n  .ml-lg-0,\n  .mx-lg-0 {\n    margin-left: 0 !important; }\n  .m-lg-1 {\n    margin: 0.25rem !important; }\n  .mt-lg-1,\n  .my-lg-1 {\n    margin-top: 0.25rem !important; }\n  .mr-lg-1,\n  .mx-lg-1 {\n    margin-right: 0.25rem !important; }\n  .mb-lg-1,\n  .my-lg-1 {\n    margin-bottom: 0.25rem !important; }\n  .ml-lg-1,\n  .mx-lg-1 {\n    margin-left: 0.25rem !important; }\n  .m-lg-2 {\n    margin: 0.5rem !important; }\n  .mt-lg-2,\n  .my-lg-2 {\n    margin-top: 0.5rem !important; }\n  .mr-lg-2,\n  .mx-lg-2 {\n    margin-right: 0.5rem !important; }\n  .mb-lg-2,\n  .my-lg-2 {\n    margin-bottom: 0.5rem !important; }\n  .ml-lg-2,\n  .mx-lg-2 {\n    margin-left: 0.5rem !important; }\n  .m-lg-3 {\n    margin: 1rem !important; }\n  .mt-lg-3,\n  .my-lg-3 {\n    margin-top: 1rem !important; }\n  .mr-lg-3,\n  .mx-lg-3 {\n    margin-right: 1rem !important; }\n  .mb-lg-3,\n  .my-lg-3 {\n    margin-bottom: 1rem !important; }\n  .ml-lg-3,\n  .mx-lg-3 {\n    margin-left: 1rem !important; }\n  .m-lg-4 {\n    margin: 1.5rem !important; }\n  .mt-lg-4,\n  .my-lg-4 {\n    margin-top: 1.5rem !important; }\n  .mr-lg-4,\n  .mx-lg-4 {\n    margin-right: 1.5rem !important; }\n  .mb-lg-4,\n  .my-lg-4 {\n    margin-bottom: 1.5rem !important; }\n  .ml-lg-4,\n  .mx-lg-4 {\n    margin-left: 1.5rem !important; }\n  .m-lg-5 {\n    margin: 3rem !important; }\n  .mt-lg-5,\n  .my-lg-5 {\n    margin-top: 3rem !important; }\n  .mr-lg-5,\n  .mx-lg-5 {\n    margin-right: 3rem !important; }\n  .mb-lg-5,\n  .my-lg-5 {\n    margin-bottom: 3rem !important; }\n  .ml-lg-5,\n  .mx-lg-5 {\n    margin-left: 3rem !important; }\n  .p-lg-0 {\n    padding: 0 !important; }\n  .pt-lg-0,\n  .py-lg-0 {\n    padding-top: 0 !important; }\n  .pr-lg-0,\n  .px-lg-0 {\n    padding-right: 0 !important; }\n  .pb-lg-0,\n  .py-lg-0 {\n    padding-bottom: 0 !important; }\n  .pl-lg-0,\n  .px-lg-0 {\n    padding-left: 0 !important; }\n  .p-lg-1 {\n    padding: 0.25rem !important; }\n  .pt-lg-1,\n  .py-lg-1 {\n    padding-top: 0.25rem !important; }\n  .pr-lg-1,\n  .px-lg-1 {\n    padding-right: 0.25rem !important; }\n  .pb-lg-1,\n  .py-lg-1 {\n    padding-bottom: 0.25rem !important; }\n  .pl-lg-1,\n  .px-lg-1 {\n    padding-left: 0.25rem !important; }\n  .p-lg-2 {\n    padding: 0.5rem !important; }\n  .pt-lg-2,\n  .py-lg-2 {\n    padding-top: 0.5rem !important; }\n  .pr-lg-2,\n  .px-lg-2 {\n    padding-right: 0.5rem !important; }\n  .pb-lg-2,\n  .py-lg-2 {\n    padding-bottom: 0.5rem !important; }\n  .pl-lg-2,\n  .px-lg-2 {\n    padding-left: 0.5rem !important; }\n  .p-lg-3 {\n    padding: 1rem !important; }\n  .pt-lg-3,\n  .py-lg-3 {\n    padding-top: 1rem !important; }\n  .pr-lg-3,\n  .px-lg-3 {\n    padding-right: 1rem !important; }\n  .pb-lg-3,\n  .py-lg-3 {\n    padding-bottom: 1rem !important; }\n  .pl-lg-3,\n  .px-lg-3 {\n    padding-left: 1rem !important; }\n  .p-lg-4 {\n    padding: 1.5rem !important; }\n  .pt-lg-4,\n  .py-lg-4 {\n    padding-top: 1.5rem !important; }\n  .pr-lg-4,\n  .px-lg-4 {\n    padding-right: 1.5rem !important; }\n  .pb-lg-4,\n  .py-lg-4 {\n    padding-bottom: 1.5rem !important; }\n  .pl-lg-4,\n  .px-lg-4 {\n    padding-left: 1.5rem !important; }\n  .p-lg-5 {\n    padding: 3rem !important; }\n  .pt-lg-5,\n  .py-lg-5 {\n    padding-top: 3rem !important; }\n  .pr-lg-5,\n  .px-lg-5 {\n    padding-right: 3rem !important; }\n  .pb-lg-5,\n  .py-lg-5 {\n    padding-bottom: 3rem !important; }\n  .pl-lg-5,\n  .px-lg-5 {\n    padding-left: 3rem !important; }\n  .m-lg-n1 {\n    margin: -0.25rem !important; }\n  .mt-lg-n1,\n  .my-lg-n1 {\n    margin-top: -0.25rem !important; }\n  .mr-lg-n1,\n  .mx-lg-n1 {\n    margin-right: -0.25rem !important; }\n  .mb-lg-n1,\n  .my-lg-n1 {\n    margin-bottom: -0.25rem !important; }\n  .ml-lg-n1,\n  .mx-lg-n1 {\n    margin-left: -0.25rem !important; }\n  .m-lg-n2 {\n    margin: -0.5rem !important; }\n  .mt-lg-n2,\n  .my-lg-n2 {\n    margin-top: -0.5rem !important; }\n  .mr-lg-n2,\n  .mx-lg-n2 {\n    margin-right: -0.5rem !important; }\n  .mb-lg-n2,\n  .my-lg-n2 {\n    margin-bottom: -0.5rem !important; }\n  .ml-lg-n2,\n  .mx-lg-n2 {\n    margin-left: -0.5rem !important; }\n  .m-lg-n3 {\n    margin: -1rem !important; }\n  .mt-lg-n3,\n  .my-lg-n3 {\n    margin-top: -1rem !important; }\n  .mr-lg-n3,\n  .mx-lg-n3 {\n    margin-right: -1rem !important; }\n  .mb-lg-n3,\n  .my-lg-n3 {\n    margin-bottom: -1rem !important; }\n  .ml-lg-n3,\n  .mx-lg-n3 {\n    margin-left: -1rem !important; }\n  .m-lg-n4 {\n    margin: -1.5rem !important; }\n  .mt-lg-n4,\n  .my-lg-n4 {\n    margin-top: -1.5rem !important; }\n  .mr-lg-n4,\n  .mx-lg-n4 {\n    margin-right: -1.5rem !important; }\n  .mb-lg-n4,\n  .my-lg-n4 {\n    margin-bottom: -1.5rem !important; }\n  .ml-lg-n4,\n  .mx-lg-n4 {\n    margin-left: -1.5rem !important; }\n  .m-lg-n5 {\n    margin: -3rem !important; }\n  .mt-lg-n5,\n  .my-lg-n5 {\n    margin-top: -3rem !important; }\n  .mr-lg-n5,\n  .mx-lg-n5 {\n    margin-right: -3rem !important; }\n  .mb-lg-n5,\n  .my-lg-n5 {\n    margin-bottom: -3rem !important; }\n  .ml-lg-n5,\n  .mx-lg-n5 {\n    margin-left: -3rem !important; }\n  .m-lg-auto {\n    margin: auto !important; }\n  .mt-lg-auto,\n  .my-lg-auto {\n    margin-top: auto !important; }\n  .mr-lg-auto,\n  .mx-lg-auto {\n    margin-right: auto !important; }\n  .mb-lg-auto,\n  .my-lg-auto {\n    margin-bottom: auto !important; }\n  .ml-lg-auto,\n  .mx-lg-auto {\n    margin-left: auto !important; } }\n\n@media (min-width: 1200px) {\n  .m-xl-0 {\n    margin: 0 !important; }\n  .mt-xl-0,\n  .my-xl-0 {\n    margin-top: 0 !important; }\n  .mr-xl-0,\n  .mx-xl-0 {\n    margin-right: 0 !important; }\n  .mb-xl-0,\n  .my-xl-0 {\n    margin-bottom: 0 !important; }\n  .ml-xl-0,\n  .mx-xl-0 {\n    margin-left: 0 !important; }\n  .m-xl-1 {\n    margin: 0.25rem !important; }\n  .mt-xl-1,\n  .my-xl-1 {\n    margin-top: 0.25rem !important; }\n  .mr-xl-1,\n  .mx-xl-1 {\n    margin-right: 0.25rem !important; }\n  .mb-xl-1,\n  .my-xl-1 {\n    margin-bottom: 0.25rem !important; }\n  .ml-xl-1,\n  .mx-xl-1 {\n    margin-left: 0.25rem !important; }\n  .m-xl-2 {\n    margin: 0.5rem !important; }\n  .mt-xl-2,\n  .my-xl-2 {\n    margin-top: 0.5rem !important; }\n  .mr-xl-2,\n  .mx-xl-2 {\n    margin-right: 0.5rem !important; }\n  .mb-xl-2,\n  .my-xl-2 {\n    margin-bottom: 0.5rem !important; }\n  .ml-xl-2,\n  .mx-xl-2 {\n    margin-left: 0.5rem !important; }\n  .m-xl-3 {\n    margin: 1rem !important; }\n  .mt-xl-3,\n  .my-xl-3 {\n    margin-top: 1rem !important; }\n  .mr-xl-3,\n  .mx-xl-3 {\n    margin-right: 1rem !important; }\n  .mb-xl-3,\n  .my-xl-3 {\n    margin-bottom: 1rem !important; }\n  .ml-xl-3,\n  .mx-xl-3 {\n    margin-left: 1rem !important; }\n  .m-xl-4 {\n    margin: 1.5rem !important; }\n  .mt-xl-4,\n  .my-xl-4 {\n    margin-top: 1.5rem !important; }\n  .mr-xl-4,\n  .mx-xl-4 {\n    margin-right: 1.5rem !important; }\n  .mb-xl-4,\n  .my-xl-4 {\n    margin-bottom: 1.5rem !important; }\n  .ml-xl-4,\n  .mx-xl-4 {\n    margin-left: 1.5rem !important; }\n  .m-xl-5 {\n    margin: 3rem !important; }\n  .mt-xl-5,\n  .my-xl-5 {\n    margin-top: 3rem !important; }\n  .mr-xl-5,\n  .mx-xl-5 {\n    margin-right: 3rem !important; }\n  .mb-xl-5,\n  .my-xl-5 {\n    margin-bottom: 3rem !important; }\n  .ml-xl-5,\n  .mx-xl-5 {\n    margin-left: 3rem !important; }\n  .p-xl-0 {\n    padding: 0 !important; }\n  .pt-xl-0,\n  .py-xl-0 {\n    padding-top: 0 !important; }\n  .pr-xl-0,\n  .px-xl-0 {\n    padding-right: 0 !important; }\n  .pb-xl-0,\n  .py-xl-0 {\n    padding-bottom: 0 !important; }\n  .pl-xl-0,\n  .px-xl-0 {\n    padding-left: 0 !important; }\n  .p-xl-1 {\n    padding: 0.25rem !important; }\n  .pt-xl-1,\n  .py-xl-1 {\n    padding-top: 0.25rem !important; }\n  .pr-xl-1,\n  .px-xl-1 {\n    padding-right: 0.25rem !important; }\n  .pb-xl-1,\n  .py-xl-1 {\n    padding-bottom: 0.25rem !important; }\n  .pl-xl-1,\n  .px-xl-1 {\n    padding-left: 0.25rem !important; }\n  .p-xl-2 {\n    padding: 0.5rem !important; }\n  .pt-xl-2,\n  .py-xl-2 {\n    padding-top: 0.5rem !important; }\n  .pr-xl-2,\n  .px-xl-2 {\n    padding-right: 0.5rem !important; }\n  .pb-xl-2,\n  .py-xl-2 {\n    padding-bottom: 0.5rem !important; }\n  .pl-xl-2,\n  .px-xl-2 {\n    padding-left: 0.5rem !important; }\n  .p-xl-3 {\n    padding: 1rem !important; }\n  .pt-xl-3,\n  .py-xl-3 {\n    padding-top: 1rem !important; }\n  .pr-xl-3,\n  .px-xl-3 {\n    padding-right: 1rem !important; }\n  .pb-xl-3,\n  .py-xl-3 {\n    padding-bottom: 1rem !important; }\n  .pl-xl-3,\n  .px-xl-3 {\n    padding-left: 1rem !important; }\n  .p-xl-4 {\n    padding: 1.5rem !important; }\n  .pt-xl-4,\n  .py-xl-4 {\n    padding-top: 1.5rem !important; }\n  .pr-xl-4,\n  .px-xl-4 {\n    padding-right: 1.5rem !important; }\n  .pb-xl-4,\n  .py-xl-4 {\n    padding-bottom: 1.5rem !important; }\n  .pl-xl-4,\n  .px-xl-4 {\n    padding-left: 1.5rem !important; }\n  .p-xl-5 {\n    padding: 3rem !important; }\n  .pt-xl-5,\n  .py-xl-5 {\n    padding-top: 3rem !important; }\n  .pr-xl-5,\n  .px-xl-5 {\n    padding-right: 3rem !important; }\n  .pb-xl-5,\n  .py-xl-5 {\n    padding-bottom: 3rem !important; }\n  .pl-xl-5,\n  .px-xl-5 {\n    padding-left: 3rem !important; }\n  .m-xl-n1 {\n    margin: -0.25rem !important; }\n  .mt-xl-n1,\n  .my-xl-n1 {\n    margin-top: -0.25rem !important; }\n  .mr-xl-n1,\n  .mx-xl-n1 {\n    margin-right: -0.25rem !important; }\n  .mb-xl-n1,\n  .my-xl-n1 {\n    margin-bottom: -0.25rem !important; }\n  .ml-xl-n1,\n  .mx-xl-n1 {\n    margin-left: -0.25rem !important; }\n  .m-xl-n2 {\n    margin: -0.5rem !important; }\n  .mt-xl-n2,\n  .my-xl-n2 {\n    margin-top: -0.5rem !important; }\n  .mr-xl-n2,\n  .mx-xl-n2 {\n    margin-right: -0.5rem !important; }\n  .mb-xl-n2,\n  .my-xl-n2 {\n    margin-bottom: -0.5rem !important; }\n  .ml-xl-n2,\n  .mx-xl-n2 {\n    margin-left: -0.5rem !important; }\n  .m-xl-n3 {\n    margin: -1rem !important; }\n  .mt-xl-n3,\n  .my-xl-n3 {\n    margin-top: -1rem !important; }\n  .mr-xl-n3,\n  .mx-xl-n3 {\n    margin-right: -1rem !important; }\n  .mb-xl-n3,\n  .my-xl-n3 {\n    margin-bottom: -1rem !important; }\n  .ml-xl-n3,\n  .mx-xl-n3 {\n    margin-left: -1rem !important; }\n  .m-xl-n4 {\n    margin: -1.5rem !important; }\n  .mt-xl-n4,\n  .my-xl-n4 {\n    margin-top: -1.5rem !important; }\n  .mr-xl-n4,\n  .mx-xl-n4 {\n    margin-right: -1.5rem !important; }\n  .mb-xl-n4,\n  .my-xl-n4 {\n    margin-bottom: -1.5rem !important; }\n  .ml-xl-n4,\n  .mx-xl-n4 {\n    margin-left: -1.5rem !important; }\n  .m-xl-n5 {\n    margin: -3rem !important; }\n  .mt-xl-n5,\n  .my-xl-n5 {\n    margin-top: -3rem !important; }\n  .mr-xl-n5,\n  .mx-xl-n5 {\n    margin-right: -3rem !important; }\n  .mb-xl-n5,\n  .my-xl-n5 {\n    margin-bottom: -3rem !important; }\n  .ml-xl-n5,\n  .mx-xl-n5 {\n    margin-left: -3rem !important; }\n  .m-xl-auto {\n    margin: auto !important; }\n  .mt-xl-auto,\n  .my-xl-auto {\n    margin-top: auto !important; }\n  .mr-xl-auto,\n  .mx-xl-auto {\n    margin-right: auto !important; }\n  .mb-xl-auto,\n  .my-xl-auto {\n    margin-bottom: auto !important; }\n  .ml-xl-auto,\n  .mx-xl-auto {\n    margin-left: auto !important; } }\n\n.text-monospace {\n  font-family: SFMono-Regular, Menlo, Monaco, Consolas, \"Liberation Mono\", \"Courier New\", monospace !important; }\n\n.text-justify {\n  text-align: justify !important; }\n\n.text-wrap {\n  white-space: normal !important; }\n\n.text-nowrap {\n  white-space: nowrap !important; }\n\n.text-truncate {\n  overflow: hidden;\n  text-overflow: ellipsis;\n  white-space: nowrap; }\n\n.text-left {\n  text-align: left !important; }\n\n.text-right {\n  text-align: right !important; }\n\n.text-center {\n  text-align: center !important; }\n\n@media (min-width: 576px) {\n  .text-sm-left {\n    text-align: left !important; }\n  .text-sm-right {\n    text-align: right !important; }\n  .text-sm-center {\n    text-align: center !important; } }\n\n@media (min-width: 768px) {\n  .text-md-left {\n    text-align: left !important; }\n  .text-md-right {\n    text-align: right !important; }\n  .text-md-center {\n    text-align: center !important; } }\n\n@media (min-width: 992px) {\n  .text-lg-left {\n    text-align: left !important; }\n  .text-lg-right {\n    text-align: right !important; }\n  .text-lg-center {\n    text-align: center !important; } }\n\n@media (min-width: 1200px) {\n  .text-xl-left {\n    text-align: left !important; }\n  .text-xl-right {\n    text-align: right !important; }\n  .text-xl-center {\n    text-align: center !important; } }\n\n.text-lowercase {\n  text-transform: lowercase !important; }\n\n.text-uppercase {\n  text-transform: uppercase !important; }\n\n.text-capitalize {\n  text-transform: capitalize !important; }\n\n.font-weight-light {\n  font-weight: 300 !important; }\n\n.font-weight-lighter {\n  font-weight: lighter !important; }\n\n.font-weight-normal {\n  font-weight: 400 !important; }\n\n.font-weight-bold {\n  font-weight: 700 !important; }\n\n.font-weight-bolder {\n  font-weight: bolder !important; }\n\n.font-italic {\n  font-style: italic !important; }\n\n.text-white {\n  color: #fff !important; }\n\n.text-primary {\n  color: #007bff !important; }\n\na.text-primary:hover, a.text-primary:focus {\n  color: #0056b3 !important; }\n\n.text-secondary {\n  color: #6c757d !important; }\n\na.text-secondary:hover, a.text-secondary:focus {\n  color: #494f54 !important; }\n\n.text-success {\n  color: #28a745 !important; }\n\na.text-success:hover, a.text-success:focus {\n  color: #19692c !important; }\n\n.text-info {\n  color: #17a2b8 !important; }\n\na.text-info:hover, a.text-info:focus {\n  color: #0f6674 !important; }\n\n.text-warning {\n  color: #ffc107 !important; }\n\na.text-warning:hover, a.text-warning:focus {\n  color: #ba8b00 !important; }\n\n.text-danger {\n  color: #dc3545 !important; }\n\na.text-danger:hover, a.text-danger:focus {\n  color: #a71d2a !important; }\n\n.text-light {\n  color: #f8f9fa !important; }\n\na.text-light:hover, a.text-light:focus {\n  color: #cbd3da !important; }\n\n.text-dark {\n  color: #343a40 !important; }\n\na.text-dark:hover, a.text-dark:focus {\n  color: #121416 !important; }\n\n.text-body {\n  color: #212529 !important; }\n\n.text-muted {\n  color: #6c757d !important; }\n\n.text-black-50 {\n  color: rgba(0, 0, 0, 0.5) !important; }\n\n.text-white-50 {\n  color: rgba(255, 255, 255, 0.5) !important; }\n\n.text-hide {\n  font: 0/0 a;\n  color: transparent;\n  text-shadow: none;\n  background-color: transparent;\n  border: 0; }\n\n.text-decoration-none {\n  text-decoration: none !important; }\n\n.text-break {\n  word-break: break-word !important;\n  overflow-wrap: break-word !important; }\n\n.text-reset {\n  color: inherit !important; }\n\n.bg-primary {\n  background-color: #007bff !important; }\n\na.bg-primary:hover, a.bg-primary:focus,\nbutton.bg-primary:hover,\nbutton.bg-primary:focus {\n  background-color: #0062cc !important; }\n\n.bg-secondary {\n  background-color: #6c757d !important; }\n\na.bg-secondary:hover, a.bg-secondary:focus,\nbutton.bg-secondary:hover,\nbutton.bg-secondary:focus {\n  background-color: #545b62 !important; }\n\n.bg-success {\n  background-color: #28a745 !important; }\n\na.bg-success:hover, a.bg-success:focus,\nbutton.bg-success:hover,\nbutton.bg-success:focus {\n  background-color: #1e7e34 !important; }\n\n.bg-info {\n  background-color: #17a2b8 !important; }\n\na.bg-info:hover, a.bg-info:focus,\nbutton.bg-info:hover,\nbutton.bg-info:focus {\n  background-color: #117a8b !important; }\n\n.bg-warning {\n  background-color: #ffc107 !important; }\n\na.bg-warning:hover, a.bg-warning:focus,\nbutton.bg-warning:hover,\nbutton.bg-warning:focus {\n  background-color: #d39e00 !important; }\n\n.bg-danger {\n  background-color: #dc3545 !important; }\n\na.bg-danger:hover, a.bg-danger:focus,\nbutton.bg-danger:hover,\nbutton.bg-danger:focus {\n  background-color: #bd2130 !important; }\n\n.bg-light {\n  background-color: #f8f9fa !important; }\n\na.bg-light:hover, a.bg-light:focus,\nbutton.bg-light:hover,\nbutton.bg-light:focus {\n  background-color: #dae0e5 !important; }\n\n.bg-dark {\n  background-color: #343a40 !important; }\n\na.bg-dark:hover, a.bg-dark:focus,\nbutton.bg-dark:hover,\nbutton.bg-dark:focus {\n  background-color: #1d2124 !important; }\n\n.bg-white {\n  background-color: #fff !important; }\n\n.bg-transparent {\n  background-color: transparent !important; }\n", ""]);



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
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly8vd2VicGFjay9ib290c3RyYXAiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2hpZ2hsaWdodC5qcy9zdHlsZXMvZGVmYXVsdC5jc3MiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2Nzcy1sb2FkZXIvZGlzdC9ydW50aW1lL2FwaS5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvaGlnaGxpZ2h0LmpzL2xpYi9oaWdobGlnaHQuanMiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2hpZ2hsaWdodC5qcy9saWIvbGFuZ3VhZ2VzL2NzLmpzIiwid2VicGFjazovLy8uL25vZGVfbW9kdWxlcy9oaWdobGlnaHQuanMvbGliL2xhbmd1YWdlcy9qYXZhLmpzIiwid2VicGFjazovLy8uL25vZGVfbW9kdWxlcy9oaWdobGlnaHQuanMvbGliL2xhbmd1YWdlcy9qYXZhc2NyaXB0LmpzIiwid2VicGFjazovLy8uL25vZGVfbW9kdWxlcy9oaWdobGlnaHQuanMvbGliL2xhbmd1YWdlcy9zY2FsYS5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvaGlnaGxpZ2h0LmpzL2xpYi9sYW5ndWFnZXMvdHlwZXNjcmlwdC5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWVsZW1lbnQvbGliL2Nzcy10YWcuanMiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2xpdC1lbGVtZW50L2xpYi9kZWNvcmF0b3JzLmpzIiwid2VicGFjazovLy8uL25vZGVfbW9kdWxlcy9saXQtZWxlbWVudC9saWIvdXBkYXRpbmctZWxlbWVudC5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWVsZW1lbnQvbGl0LWVsZW1lbnQuanMiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2xpdC1odG1sL2RpcmVjdGl2ZXMvdW5zYWZlLWh0bWwuanMiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2xpdC1odG1sL2xpYi9kZWZhdWx0LXRlbXBsYXRlLXByb2Nlc3Nvci5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL2RpcmVjdGl2ZS5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL2RvbS5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL21vZGlmeS10ZW1wbGF0ZS5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL3BhcnQuanMiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2xpdC1odG1sL2xpYi9wYXJ0cy5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL3JlbmRlci5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL3NoYWR5LXJlbmRlci5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL3RlbXBsYXRlLWZhY3RvcnkuanMiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2xpdC1odG1sL2xpYi90ZW1wbGF0ZS1pbnN0YW5jZS5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL3RlbXBsYXRlLXJlc3VsdC5qcyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvbGl0LWh0bWwvbGliL3RlbXBsYXRlLmpzIiwid2VicGFjazovLy8uL25vZGVfbW9kdWxlcy9saXQtaHRtbC9saXQtaHRtbC5qcyIsIndlYnBhY2s6Ly8vLi9zcmMvY29tcG9uZW50cy9tdXRhdGlvbi10ZXN0LXJlcG9ydC1hcHAudHMiLCJ3ZWJwYWNrOi8vLy4vc3JjL2NvbXBvbmVudHMvbXV0YXRpb24tdGVzdC1yZXBvcnQtYnJlYWRjcnVtYi50cyIsIndlYnBhY2s6Ly8vLi9zcmMvY29tcG9uZW50cy9tdXRhdGlvbi10ZXN0LXJlcG9ydC1maWxlLWxlZ2VuZC50cyIsIndlYnBhY2s6Ly8vLi9zcmMvY29tcG9uZW50cy9tdXRhdGlvbi10ZXN0LXJlcG9ydC1maWxlLnRzIiwid2VicGFjazovLy8uL3NyYy9jb21wb25lbnRzL211dGF0aW9uLXRlc3QtcmVwb3J0LW11dGFudC50cyIsIndlYnBhY2s6Ly8vLi9zcmMvY29tcG9uZW50cy9tdXRhdGlvbi10ZXN0LXJlcG9ydC1yZXN1bHQudHMiLCJ3ZWJwYWNrOi8vLy4vc3JjL2NvbXBvbmVudHMvbXV0YXRpb24tdGVzdC1yZXBvcnQtcm91dGVyLnRzIiwid2VicGFjazovLy8uL3NyYy9jb21wb25lbnRzL211dGF0aW9uLXRlc3QtcmVwb3J0LXRpdGxlLnRzIiwid2VicGFjazovLy8uL3NyYy9jb21wb25lbnRzL211dGF0aW9uLXRlc3QtcmVwb3J0LXRvdGFscy50cyIsIndlYnBhY2s6Ly8vLi9zcmMvaGVscGVycy50cyIsIndlYnBhY2s6Ly8vLi9zcmMvaW5kZXgudHMiLCJ3ZWJwYWNrOi8vLy4vc3JjL21vZGVsL1Jlc3VsdFRhYmxlLnRzIiwid2VicGFjazovLy8uL3NyYy9zdHlsZS9ib290c3RyYXAuc2NzcyIsIndlYnBhY2s6Ly8vLi9zcmMvc3R5bGUvaGlnaGxpZ2h0anMuc2NzcyIsIndlYnBhY2s6Ly8vLi9zcmMvc3R5bGUvaW5kZXgudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IjtBQUFBO0FBQ0E7O0FBRUE7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBOztBQUVBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBOzs7QUFHQTtBQUNBOztBQUVBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0Esa0RBQTBDLGdDQUFnQztBQUMxRTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBLGdFQUF3RCxrQkFBa0I7QUFDMUU7QUFDQSx5REFBaUQsY0FBYztBQUMvRDs7QUFFQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EsaURBQXlDLGlDQUFpQztBQUMxRSx3SEFBZ0gs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QTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0E7O0FBRUE7O0FBRUE7QUFDQTs7QUFFQTtBQUNBOztBQUVBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EsU0FBUztBQUNUO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBO0FBQ0EsaUNBQWlDLFNBQVMsWUFBWTtBQUN0RDs7QUFFQTs7QUFFQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLFNBQVM7QUFDVDtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxTQUFTO0FBQ1Q7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxTQUFTO0FBQ1Q7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBLDJCQUEyQjtBQUMzQjtBQUNBLHNCQUFzQixzQkFBc0I7QUFDNUM7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLHdCQUF3QixnQkFBZ0IsNEJBQTRCO0FBQ3BFO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EsS0FBSztBQUNMO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxPQUFPO0FBQ1A7QUFDQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EsS0FBSztBQUNMO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTs7QUFFQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EsV0FBVztBQUNYO0FBQ0E7QUFDQTtBQUNBLE9BQU87QUFDUDs7QUFFQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBOztBQUVBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0EsS0FBSztBQUNMO0FBQ0E7QUFDQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBOztBQUVBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0EsNENBQTRDLHVCQUF1QjtBQUNuRTtBQUNBOztBQUVBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQTs7QUFFQTs7QUFFQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FB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Q0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxTQUFTO0FBQ1Q7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSwrREFBK0QsS0FBSztBQUNwRTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EsYUFBYTtBQUNiO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLGFBQWE7QUFDYjtBQUNBO0FBQ0EsU0FBUztBQUNUO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxTQUFTO0FBQ1Q7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0Esb0JBQW9CLFlBQVk7QUFDaEMsd0JBQXdCLElBQUk7QUFDNUIsaUJBQWlCLFFBQVE7QUFDekI7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxpQkFBaUIsUUFBUTtBQUN6QjtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0EsNEM7Ozs7Ozs7Ozs7OztBQy9pQkE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtB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QUFDQTtBQUNPO0FBQ1Asc0Q7Ozs7Ozs7Ozs7OztBQ25EQTtBQUFBO0FBQUE7QUFBQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLFdBQVcsZ0JBQWdCO0FBQzNCO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxJQUFJO0FBQ0o7QUFDQTtBQUNBO0FBQ087QUFDUDtBQUNBO0FBQ0E7QUFDQSxDQUFDO0FBQ007QUFDUDtBQUNBO0FBQ0EscUM7Ozs7Ozs7Ozs7OztBQzNDQTtBQUFBO0FBQUE7QUFBQTtBQUFBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ087QUFDUDtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ087QUFDUDtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ087QUFDUDtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBLCtCOzs7Ozs7Ozs7Ozs7QUM3Q0E7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNxRDtBQUNyRCxpREFBaUQscUJBQXFCO0FBQ3RFO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ087QUFDUCxXQUFXLFdBQVcsVUFBVSxTQUFTO0FBQ3pDO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNB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QWMseUNBQXlDLEVBQUUsb0ZBQW9GLHlDQUF5QyxFQUFFLGlCQUFpQix5Q0FBeUMsRUFBRSxnR0FBZ0cseUNBQXlDLEVBQUUsZ0JBQWdCLHlDQUF5QyxFQUFFLDRGQUE0Rix5Q0FBeUMsRUFBRSxlQUFlLHlDQUF5QyxFQUFFLHdGQUF3Rix5Q0FBeUMsRUFBRSxjQUFjLHlDQUF5QyxFQUFFLG9GQUFvRix5Q0FBeUMsRUFBRSxlQUFlLHNDQUFzQyxFQUFFLHFCQUFxQiw2Q0FBNkMsRUFBRTs7Ozs7Ozs7Ozs7OztBQ0ZyMDdHLDJCQUEyQixtQkFBTyxDQUFDLHdHQUFtRDtBQUN0RjtBQUNBLFVBQVUsbUJBQU8sQ0FBQyx5S0FBNkU7O0FBRS9GO0FBQ0EsY0FBYyxRQUFTOzs7Ozs7Ozs7Ozs7Ozs7O0FDTHZCLDBHQUF3RDtBQUN4RCxNQUFNLFlBQVksR0FBRyxtQkFBTyxDQUFDLG9EQUFrQixDQUFDLENBQUM7QUFDakQsTUFBTSxjQUFjLEdBQUcsbUJBQU8sQ0FBQyx3REFBb0IsQ0FBQyxDQUFDO0FBRXhDLGlCQUFTLEdBQWMsaUJBQUcsSUFBRyx1QkFBUyxDQUFDLFlBQVksQ0FBQyxRQUFRLEVBQUUsQ0FBQyxFQUFFLENBQUM7QUFDbEUsbUJBQVcsR0FBYyxpQkFBRyxJQUFHLHVCQUFTLENBQUMsY0FBYyxDQUFDLFFBQVEsRUFBRSxDQUFDLEVBQUUsQ0FBQyIsImZpbGUiOiJtdXRhdGlvbi10ZXN0LWVsZW1lbnRzLmpzIiwic291cmNlc0NvbnRlbnQiOlsiIFx0Ly8gVGhlIG1vZHVsZSBjYWNoZVxuIFx0dmFyIGluc3RhbGxlZE1vZHVsZXMgPSB7fTtcblxuIFx0Ly8gVGhlIHJlcXVpcmUgZnVuY3Rpb25cbiBcdGZ1bmN0aW9uIF9fd2VicGFja19yZXF1aXJlX18obW9kdWxlSWQpIHtcblxuIFx0XHQvLyBDaGVjayBpZiBtb2R1bGUgaXMgaW4gY2FjaGVcbiBcdFx0aWYoaW5zdGFsbGVkTW9kdWxlc1ttb2R1bGVJZF0pIHtcbiBcdFx0XHRyZXR1cm4gaW5zdGFsbGVkTW9kdWxlc1ttb2R1bGVJZF0uZXhwb3J0cztcbiBcdFx0fVxuIFx0XHQvLyBDcmVhdGUgYSBuZXcgbW9kdWxlIChhbmQgcHV0IGl0IGludG8gdGhlIGNhY2hlKVxuIFx0XHR2YXIgbW9kdWxlID0gaW5zdGFsbGVkTW9kdWxlc1ttb2R1bGVJZF0gPSB7XG4gXHRcdFx0aTogbW9kdWxlSWQsXG4gXHRcdFx0bDogZmFsc2UsXG4gXHRcdFx0ZXhwb3J0czoge31cbiBcdFx0fTtcblxuIFx0XHQvLyBFeGVjdXRlIHRoZSBtb2R1bGUgZnVuY3Rpb25cbiBcdFx0bW9kdWxlc1ttb2R1bGVJZF0uY2FsbChtb2R1bGUuZXhwb3J0cywgbW9kdWxlLCBtb2R1bGUuZXhwb3J0cywgX193ZWJwYWNrX3JlcXVpcmVfXyk7XG5cbiBcdFx0Ly8gRmxhZyB0aGUgbW9kdWxlIGFzIGxvYWRlZFxuIFx0XHRtb2R1bGUubCA9IHRydWU7XG5cbiBcdFx0Ly8gUmV0dXJuIHRoZSBleHBvcnRzIG9mIHRoZSBtb2R1bGVcbiBcdFx0cmV0dXJuIG1vZHVsZS5leHBvcnRzO1xuIFx0fVxuXG5cbiBcdC8vIGV4cG9zZSB0aGUgbW9kdWxlcyBvYmplY3QgKF9fd2VicGFja19tb2R1bGVzX18pXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLm0gPSBtb2R1bGVzO1xuXG4gXHQvLyBleHBvc2UgdGhlIG1vZHVsZSBjYWNoZVxuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5jID0gaW5zdGFsbGVkTW9kdWxlcztcblxuIFx0Ly8gZGVmaW5lIGdldHRlciBmdW5jdGlvbiBmb3IgaGFybW9ueSBleHBvcnRzXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLmQgPSBmdW5jdGlvbihleHBvcnRzLCBuYW1lLCBnZXR0ZXIpIHtcbiBcdFx0aWYoIV9fd2VicGFja19yZXF1aXJlX18ubyhleHBvcnRzLCBuYW1lKSkge1xuIFx0XHRcdE9iamVjdC5kZWZpbmVQcm9wZXJ0eShleHBvcnRzLCBuYW1lLCB7IGVudW1lcmFibGU6IHRydWUsIGdldDogZ2V0dGVyIH0pO1xuIFx0XHR9XG4gXHR9O1xuXG4gXHQvLyBkZWZpbmUgX19lc01vZHVsZSBvbiBleHBvcnRzXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLnIgPSBmdW5jdGlvbihleHBvcnRzKSB7XG4gXHRcdGlmKHR5cGVvZiBTeW1ib2wgIT09ICd1bmRlZmluZWQnICYmIFN5bWJvbC50b1N0cmluZ1RhZykge1xuIFx0XHRcdE9iamVjdC5kZWZpbmVQcm9wZXJ0eShleHBvcnRzLCBTeW1ib2wudG9TdHJpbmdUYWcsIHsgdmFsdWU6ICdNb2R1bGUnIH0pO1xuIFx0XHR9XG4gXHRcdE9iamVjdC5kZWZpbmVQcm9wZXJ0eShleHBvcnRzLCAnX19lc01vZHVsZScsIHsgdmFsdWU6IHRydWUgfSk7XG4gXHR9O1xuXG4gXHQvLyBjcmVhdGUgYSBmYWtlIG5hbWVzcGFjZSBvYmplY3RcbiBcdC8vIG1vZGUgJiAxOiB2YWx1ZSBpcyBhIG1vZHVsZSBpZCwgcmVxdWlyZSBpdFxuIFx0Ly8gbW9kZSAmIDI6IG1lcmdlIGFsbCBwcm9wZXJ0aWVzIG9mIHZhbHVlIGludG8gdGhlIG5zXG4gXHQvLyBtb2RlICYgNDogcmV0dXJuIHZhbHVlIHdoZW4gYWxyZWFkeSBucyBvYmplY3RcbiBcdC8vIG1vZGUgJiA4fDE6IGJlaGF2ZSBsaWtlIHJlcXVpcmVcbiBcdF9fd2VicGFja19yZXF1aXJlX18udCA9IGZ1bmN0aW9uKHZhbHVlLCBtb2RlKSB7XG4gXHRcdGlmKG1vZGUgJiAxKSB2YWx1ZSA9IF9fd2VicGFja19yZXF1aXJlX18odmFsdWUpO1xuIFx0XHRpZihtb2RlICYgOCkgcmV0dXJuIHZhbHVlO1xuIFx0XHRpZigobW9kZSAmIDQpICYmIHR5cGVvZiB2YWx1ZSA9PT0gJ29iamVjdCcgJiYgdmFsdWUgJiYgdmFsdWUuX19lc01vZHVsZSkgcmV0dXJuIHZhbHVlO1xuIFx0XHR2YXIgbnMgPSBPYmplY3QuY3JlYXRlKG51bGwpO1xuIFx0XHRfX3dlYnBhY2tfcmVxdWlyZV9fLnIobnMpO1xuIFx0XHRPYmplY3QuZGVmaW5lUHJvcGVydHkobnMsICdkZWZhdWx0JywgeyBlbnVtZXJhYmxlOiB0cnVlLCB2YWx1ZTogdmFsdWUgfSk7XG4gXHRcdGlmKG1vZGUgJiAyICYmIHR5cGVvZiB2YWx1ZSAhPSAnc3RyaW5nJykgZm9yKHZhciBrZXkgaW4gdmFsdWUpIF9fd2VicGFja19yZXF1aXJlX18uZChucywga2V5LCBmdW5jdGlvbihrZXkpIHsgcmV0dXJuIHZhbHVlW2tleV07IH0uYmluZChudWxsLCBrZXkpKTtcbiBcdFx0cmV0dXJuIG5zO1xuIFx0fTtcblxuIFx0Ly8gZ2V0RGVmYXVsdEV4cG9ydCBmdW5jdGlvbiBmb3IgY29tcGF0aWJpbGl0eSB3aXRoIG5vbi1oYXJtb255IG1vZHVsZXNcbiBcdF9fd2VicGFja19yZXF1aXJlX18ubiA9IGZ1bmN0aW9uKG1vZHVsZSkge1xuIFx0XHR2YXIgZ2V0dGVyID0gbW9kdWxlICYmIG1vZHVsZS5fX2VzTW9kdWxlID9cbiBcdFx0XHRmdW5jdGlvbiBnZXREZWZhdWx0KCkgeyByZXR1cm4gbW9kdWxlWydkZWZhdWx0J107IH0gOlxuIFx0XHRcdGZ1bmN0aW9uIGdldE1vZHVsZUV4cG9ydHMoKSB7IHJldHVybiBtb2R1bGU7IH07XG4gXHRcdF9fd2VicGFja19yZXF1aXJlX18uZChnZXR0ZXIsICdhJywgZ2V0dGVyKTtcbiBcdFx0cmV0dXJuIGdldHRlcjtcbiBcdH07XG5cbiBcdC8vIE9iamVjdC5wcm90b3R5cGUuaGFzT3duUHJvcGVydHkuY2FsbFxuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5vID0gZnVuY3Rpb24ob2JqZWN0LCBwcm9wZXJ0eSkgeyByZXR1cm4gT2JqZWN0LnByb3RvdHlwZS5oYXNPd25Qcm9wZXJ0eS5jYWxsKG9iamVjdCwgcHJvcGVydHkpOyB9O1xuXG4gXHQvLyBfX3dlYnBhY2tfcHVibGljX3BhdGhfX1xuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5wID0gXCJcIjtcblxuXG4gXHQvLyBMb2FkIGVudHJ5IG1vZHVsZSBhbmQgcmV0dXJuIGV4cG9ydHNcbiBcdHJldHVybiBfX3dlYnBhY2tfcmVxdWlyZV9fKF9fd2VicGFja19yZXF1aXJlX18ucyA9IFwiLi9zcmMvaW5kZXgudHNcIik7XG4iLCJleHBvcnRzID0gbW9kdWxlLmV4cG9ydHMgPSByZXF1aXJlKFwiLi4vLi4vY3NzLWxvYWRlci9kaXN0L3J1bnRpbWUvYXBpLmpzXCIpKGZhbHNlKTtcbi8vIE1vZHVsZVxuZXhwb3J0cy5wdXNoKFttb2R1bGUuaWQsIFwiLypcXG5cXG5PcmlnaW5hbCBoaWdobGlnaHQuanMgc3R5bGUgKGMpIEl2YW4gU2FnYWxhZXYgPG1hbmlhY0Bzb2Z0d2FyZW1hbmlhY3Mub3JnPlxcblxcbiovXFxuXFxuLmhsanMge1xcbiAgZGlzcGxheTogYmxvY2s7XFxuICBvdmVyZmxvdy14OiBhdXRvO1xcbiAgcGFkZGluZzogMC41ZW07XFxuICBiYWNrZ3JvdW5kOiAjRjBGMEYwO1xcbn1cXG5cXG5cXG4vKiBCYXNlIGNvbG9yOiBzYXR1cmF0aW9uIDA7ICovXFxuXFxuLmhsanMsXFxuLmhsanMtc3Vic3Qge1xcbiAgY29sb3I6ICM0NDQ7XFxufVxcblxcbi5obGpzLWNvbW1lbnQge1xcbiAgY29sb3I6ICM4ODg4ODg7XFxufVxcblxcbi5obGpzLWtleXdvcmQsXFxuLmhsanMtYXR0cmlidXRlLFxcbi5obGpzLXNlbGVjdG9yLXRhZyxcXG4uaGxqcy1tZXRhLWtleXdvcmQsXFxuLmhsanMtZG9jdGFnLFxcbi5obGpzLW5hbWUge1xcbiAgZm9udC13ZWlnaHQ6IGJvbGQ7XFxufVxcblxcblxcbi8qIFVzZXIgY29sb3I6IGh1ZTogMCAqL1xcblxcbi5obGpzLXR5cGUsXFxuLmhsanMtc3RyaW5nLFxcbi5obGpzLW51bWJlcixcXG4uaGxqcy1zZWxlY3Rvci1pZCxcXG4uaGxqcy1zZWxlY3Rvci1jbGFzcyxcXG4uaGxqcy1xdW90ZSxcXG4uaGxqcy10ZW1wbGF0ZS10YWcsXFxuLmhsanMtZGVsZXRpb24ge1xcbiAgY29sb3I6ICM4ODAwMDA7XFxufVxcblxcbi5obGpzLXRpdGxlLFxcbi5obGpzLXNlY3Rpb24ge1xcbiAgY29sb3I6ICM4ODAwMDA7XFxuICBmb250LXdlaWdodDogYm9sZDtcXG59XFxuXFxuLmhsanMtcmVnZXhwLFxcbi5obGpzLXN5bWJvbCxcXG4uaGxqcy12YXJpYWJsZSxcXG4uaGxqcy10ZW1wbGF0ZS12YXJpYWJsZSxcXG4uaGxqcy1saW5rLFxcbi5obGpzLXNlbGVjdG9yLWF0dHIsXFxuLmhsanMtc2VsZWN0b3ItcHNldWRvIHtcXG4gIGNvbG9yOiAjQkM2MDYwO1xcbn1cXG5cXG5cXG4vKiBMYW5ndWFnZSBjb2xvcjogaHVlOiA5MDsgKi9cXG5cXG4uaGxqcy1saXRlcmFsIHtcXG4gIGNvbG9yOiAjNzhBOTYwO1xcbn1cXG5cXG4uaGxqcy1idWlsdF9pbixcXG4uaGxqcy1idWxsZXQsXFxuLmhsanMtY29kZSxcXG4uaGxqcy1hZGRpdGlvbiB7XFxuICBjb2xvcjogIzM5NzMwMDtcXG59XFxuXFxuXFxuLyogTWV0YSBjb2xvcjogaHVlOiAyMDAgKi9cXG5cXG4uaGxqcy1tZXRhIHtcXG4gIGNvbG9yOiAjMWY3MTk5O1xcbn1cXG5cXG4uaGxqcy1tZXRhLXN0cmluZyB7XFxuICBjb2xvcjogIzRkOTliZjtcXG59XFxuXFxuXFxuLyogTWlzYyBlZmZlY3RzICovXFxuXFxuLmhsanMtZW1waGFzaXMge1xcbiAgZm9udC1zdHlsZTogaXRhbGljO1xcbn1cXG5cXG4uaGxqcy1zdHJvbmcge1xcbiAgZm9udC13ZWlnaHQ6IGJvbGQ7XFxufVxcblwiLCBcIlwiXSk7XG5cbiIsIlwidXNlIHN0cmljdFwiO1xuXG4vKlxuICBNSVQgTGljZW5zZSBodHRwOi8vd3d3Lm9wZW5zb3VyY2Uub3JnL2xpY2Vuc2VzL21pdC1saWNlbnNlLnBocFxuICBBdXRob3IgVG9iaWFzIEtvcHBlcnMgQHNva3JhXG4qL1xuLy8gY3NzIGJhc2UgY29kZSwgaW5qZWN0ZWQgYnkgdGhlIGNzcy1sb2FkZXJcbm1vZHVsZS5leHBvcnRzID0gZnVuY3Rpb24gKHVzZVNvdXJjZU1hcCkge1xuICB2YXIgbGlzdCA9IFtdOyAvLyByZXR1cm4gdGhlIGxpc3Qgb2YgbW9kdWxlcyBhcyBjc3Mgc3RyaW5nXG5cbiAgbGlzdC50b1N0cmluZyA9IGZ1bmN0aW9uIHRvU3RyaW5nKCkge1xuICAgIHJldHVybiB0aGlzLm1hcChmdW5jdGlvbiAoaXRlbSkge1xuICAgICAgdmFyIGNvbnRlbnQgPSBjc3NXaXRoTWFwcGluZ1RvU3RyaW5nKGl0ZW0sIHVzZVNvdXJjZU1hcCk7XG5cbiAgICAgIGlmIChpdGVtWzJdKSB7XG4gICAgICAgIHJldHVybiAnQG1lZGlhICcgKyBpdGVtWzJdICsgJ3snICsgY29udGVudCArICd9JztcbiAgICAgIH0gZWxzZSB7XG4gICAgICAgIHJldHVybiBjb250ZW50O1xuICAgICAgfVxuICAgIH0pLmpvaW4oJycpO1xuICB9OyAvLyBpbXBvcnQgYSBsaXN0IG9mIG1vZHVsZXMgaW50byB0aGUgbGlzdFxuXG5cbiAgbGlzdC5pID0gZnVuY3Rpb24gKG1vZHVsZXMsIG1lZGlhUXVlcnkpIHtcbiAgICBpZiAodHlwZW9mIG1vZHVsZXMgPT09ICdzdHJpbmcnKSB7XG4gICAgICBtb2R1bGVzID0gW1tudWxsLCBtb2R1bGVzLCAnJ11dO1xuICAgIH1cblxuICAgIHZhciBhbHJlYWR5SW1wb3J0ZWRNb2R1bGVzID0ge307XG5cbiAgICBmb3IgKHZhciBpID0gMDsgaSA8IHRoaXMubGVuZ3RoOyBpKyspIHtcbiAgICAgIHZhciBpZCA9IHRoaXNbaV1bMF07XG5cbiAgICAgIGlmIChpZCAhPSBudWxsKSB7XG4gICAgICAgIGFscmVhZHlJbXBvcnRlZE1vZHVsZXNbaWRdID0gdHJ1ZTtcbiAgICAgIH1cbiAgICB9XG5cbiAgICBmb3IgKGkgPSAwOyBpIDwgbW9kdWxlcy5sZW5ndGg7IGkrKykge1xuICAgICAgdmFyIGl0ZW0gPSBtb2R1bGVzW2ldOyAvLyBza2lwIGFscmVhZHkgaW1wb3J0ZWQgbW9kdWxlXG4gICAgICAvLyB0aGlzIGltcGxlbWVudGF0aW9uIGlzIG5vdCAxMDAlIHBlcmZlY3QgZm9yIHdlaXJkIG1lZGlhIHF1ZXJ5IGNvbWJpbmF0aW9uc1xuICAgICAgLy8gd2hlbiBhIG1vZHVsZSBpcyBpbXBvcnRlZCBtdWx0aXBsZSB0aW1lcyB3aXRoIGRpZmZlcmVudCBtZWRpYSBxdWVyaWVzLlxuICAgICAgLy8gSSBob3BlIHRoaXMgd2lsbCBuZXZlciBvY2N1ciAoSGV5IHRoaXMgd2F5IHdlIGhhdmUgc21hbGxlciBidW5kbGVzKVxuXG4gICAgICBpZiAoaXRlbVswXSA9PSBudWxsIHx8ICFhbHJlYWR5SW1wb3J0ZWRNb2R1bGVzW2l0ZW1bMF1dKSB7XG4gICAgICAgIGlmIChtZWRpYVF1ZXJ5ICYmICFpdGVtWzJdKSB7XG4gICAgICAgICAgaXRlbVsyXSA9IG1lZGlhUXVlcnk7XG4gICAgICAgIH0gZWxzZSBpZiAobWVkaWFRdWVyeSkge1xuICAgICAgICAgIGl0ZW1bMl0gPSAnKCcgKyBpdGVtWzJdICsgJykgYW5kICgnICsgbWVkaWFRdWVyeSArICcpJztcbiAgICAgICAgfVxuXG4gICAgICAgIGxpc3QucHVzaChpdGVtKTtcbiAgICAgIH1cbiAgICB9XG4gIH07XG5cbiAgcmV0dXJuIGxpc3Q7XG59O1xuXG5mdW5jdGlvbiBjc3NXaXRoTWFwcGluZ1RvU3RyaW5nKGl0ZW0sIHVzZVNvdXJjZU1hcCkge1xuICB2YXIgY29udGVudCA9IGl0ZW1bMV0gfHwgJyc7XG4gIHZhciBjc3NNYXBwaW5nID0gaXRlbVszXTtcblxuICBpZiAoIWNzc01hcHBpbmcpIHtcbiAgICByZXR1cm4gY29udGVudDtcbiAgfVxuXG4gIGlmICh1c2VTb3VyY2VNYXAgJiYgdHlwZW9mIGJ0b2EgPT09ICdmdW5jdGlvbicpIHtcbiAgICB2YXIgc291cmNlTWFwcGluZyA9IHRvQ29tbWVudChjc3NNYXBwaW5nKTtcbiAgICB2YXIgc291cmNlVVJMcyA9IGNzc01hcHBpbmcuc291cmNlcy5tYXAoZnVuY3Rpb24gKHNvdXJjZSkge1xuICAgICAgcmV0dXJuICcvKiMgc291cmNlVVJMPScgKyBjc3NNYXBwaW5nLnNvdXJjZVJvb3QgKyBzb3VyY2UgKyAnICovJztcbiAgICB9KTtcbiAgICByZXR1cm4gW2NvbnRlbnRdLmNvbmNhdChzb3VyY2VVUkxzKS5jb25jYXQoW3NvdXJjZU1hcHBpbmddKS5qb2luKCdcXG4nKTtcbiAgfVxuXG4gIHJldHVybiBbY29udGVudF0uam9pbignXFxuJyk7XG59IC8vIEFkYXB0ZWQgZnJvbSBjb252ZXJ0LXNvdXJjZS1tYXAgKE1JVClcblxuXG5mdW5jdGlvbiB0b0NvbW1lbnQoc291cmNlTWFwKSB7XG4gIC8vIGVzbGludC1kaXNhYmxlLW5leHQtbGluZSBuby11bmRlZlxuICB2YXIgYmFzZTY0ID0gYnRvYSh1bmVzY2FwZShlbmNvZGVVUklDb21wb25lbnQoSlNPTi5zdHJpbmdpZnkoc291cmNlTWFwKSkpKTtcbiAgdmFyIGRhdGEgPSAnc291cmNlTWFwcGluZ1VSTD1kYXRhOmFwcGxpY2F0aW9uL2pzb247Y2hhcnNldD11dGYtODtiYXNlNjQsJyArIGJhc2U2NDtcbiAgcmV0dXJuICcvKiMgJyArIGRhdGEgKyAnICovJztcbn0iLCIvKlxuU3ludGF4IGhpZ2hsaWdodGluZyB3aXRoIGxhbmd1YWdlIGF1dG9kZXRlY3Rpb24uXG5odHRwczovL2hpZ2hsaWdodGpzLm9yZy9cbiovXG5cbihmdW5jdGlvbihmYWN0b3J5KSB7XG5cbiAgLy8gRmluZCB0aGUgZ2xvYmFsIG9iamVjdCBmb3IgZXhwb3J0IHRvIGJvdGggdGhlIGJyb3dzZXIgYW5kIHdlYiB3b3JrZXJzLlxuICB2YXIgZ2xvYmFsT2JqZWN0ID0gdHlwZW9mIHdpbmRvdyA9PT0gJ29iamVjdCcgJiYgd2luZG93IHx8XG4gICAgICAgICAgICAgICAgICAgICB0eXBlb2Ygc2VsZiA9PT0gJ29iamVjdCcgJiYgc2VsZjtcblxuICAvLyBTZXR1cCBoaWdobGlnaHQuanMgZm9yIGRpZmZlcmVudCBlbnZpcm9ubWVudHMuIEZpcnN0IGlzIE5vZGUuanMgb3JcbiAgLy8gQ29tbW9uSlMuXG4gIGlmKHR5cGVvZiBleHBvcnRzICE9PSAndW5kZWZpbmVkJykge1xuICAgIGZhY3RvcnkoZXhwb3J0cyk7XG4gIH0gZWxzZSBpZihnbG9iYWxPYmplY3QpIHtcbiAgICAvLyBFeHBvcnQgaGxqcyBnbG9iYWxseSBldmVuIHdoZW4gdXNpbmcgQU1EIGZvciBjYXNlcyB3aGVuIHRoaXMgc2NyaXB0XG4gICAgLy8gaXMgbG9hZGVkIHdpdGggb3RoZXJzIHRoYXQgbWF5IHN0aWxsIGV4cGVjdCBhIGdsb2JhbCBobGpzLlxuICAgIGdsb2JhbE9iamVjdC5obGpzID0gZmFjdG9yeSh7fSk7XG5cbiAgICAvLyBGaW5hbGx5IHJlZ2lzdGVyIHRoZSBnbG9iYWwgaGxqcyB3aXRoIEFNRC5cbiAgICBpZih0eXBlb2YgZGVmaW5lID09PSAnZnVuY3Rpb24nICYmIGRlZmluZS5hbWQpIHtcbiAgICAgIGRlZmluZShbXSwgZnVuY3Rpb24oKSB7XG4gICAgICAgIHJldHVybiBnbG9iYWxPYmplY3QuaGxqcztcbiAgICAgIH0pO1xuICAgIH1cbiAgfVxuXG59KGZ1bmN0aW9uKGhsanMpIHtcbiAgLy8gQ29udmVuaWVuY2UgdmFyaWFibGVzIGZvciBidWlsZC1pbiBvYmplY3RzXG4gIHZhciBBcnJheVByb3RvID0gW10sXG4gICAgICBvYmplY3RLZXlzID0gT2JqZWN0LmtleXM7XG5cbiAgLy8gR2xvYmFsIGludGVybmFsIHZhcmlhYmxlcyB1c2VkIHdpdGhpbiB0aGUgaGlnaGxpZ2h0LmpzIGxpYnJhcnkuXG4gIHZhciBsYW5ndWFnZXMgPSB7fSxcbiAgICAgIGFsaWFzZXMgICA9IHt9O1xuXG4gIC8vIFJlZ3VsYXIgZXhwcmVzc2lvbnMgdXNlZCB0aHJvdWdob3V0IHRoZSBoaWdobGlnaHQuanMgbGlicmFyeS5cbiAgdmFyIG5vSGlnaGxpZ2h0UmUgICAgPSAvXihuby0/aGlnaGxpZ2h0fHBsYWlufHRleHQpJC9pLFxuICAgICAgbGFuZ3VhZ2VQcmVmaXhSZSA9IC9cXGJsYW5nKD86dWFnZSk/LShbXFx3LV0rKVxcYi9pLFxuICAgICAgZml4TWFya3VwUmUgICAgICA9IC8oKF4oPFtePl0rPnxcXHR8KSt8KD86XFxuKSkpL2dtO1xuXG4gIHZhciBzcGFuRW5kVGFnID0gJzwvc3Bhbj4nO1xuXG4gIC8vIEdsb2JhbCBvcHRpb25zIHVzZWQgd2hlbiB3aXRoaW4gZXh0ZXJuYWwgQVBJcy4gVGhpcyBpcyBtb2RpZmllZCB3aGVuXG4gIC8vIGNhbGxpbmcgdGhlIGBobGpzLmNvbmZpZ3VyZWAgZnVuY3Rpb24uXG4gIHZhciBvcHRpb25zID0ge1xuICAgIGNsYXNzUHJlZml4OiAnaGxqcy0nLFxuICAgIHRhYlJlcGxhY2U6IG51bGwsXG4gICAgdXNlQlI6IGZhbHNlLFxuICAgIGxhbmd1YWdlczogdW5kZWZpbmVkXG4gIH07XG5cblxuICAvKiBVdGlsaXR5IGZ1bmN0aW9ucyAqL1xuXG4gIGZ1bmN0aW9uIGVzY2FwZSh2YWx1ZSkge1xuICAgIHJldHVybiB2YWx1ZS5yZXBsYWNlKC8mL2csICcmYW1wOycpLnJlcGxhY2UoLzwvZywgJyZsdDsnKS5yZXBsYWNlKC8+L2csICcmZ3Q7Jyk7XG4gIH1cblxuICBmdW5jdGlvbiB0YWcobm9kZSkge1xuICAgIHJldHVybiBub2RlLm5vZGVOYW1lLnRvTG93ZXJDYXNlKCk7XG4gIH1cblxuICBmdW5jdGlvbiB0ZXN0UmUocmUsIGxleGVtZSkge1xuICAgIHZhciBtYXRjaCA9IHJlICYmIHJlLmV4ZWMobGV4ZW1lKTtcbiAgICByZXR1cm4gbWF0Y2ggJiYgbWF0Y2guaW5kZXggPT09IDA7XG4gIH1cblxuICBmdW5jdGlvbiBpc05vdEhpZ2hsaWdodGVkKGxhbmd1YWdlKSB7XG4gICAgcmV0dXJuIG5vSGlnaGxpZ2h0UmUudGVzdChsYW5ndWFnZSk7XG4gIH1cblxuICBmdW5jdGlvbiBibG9ja0xhbmd1YWdlKGJsb2NrKSB7XG4gICAgdmFyIGksIG1hdGNoLCBsZW5ndGgsIF9jbGFzcztcbiAgICB2YXIgY2xhc3NlcyA9IGJsb2NrLmNsYXNzTmFtZSArICcgJztcblxuICAgIGNsYXNzZXMgKz0gYmxvY2sucGFyZW50Tm9kZSA/IGJsb2NrLnBhcmVudE5vZGUuY2xhc3NOYW1lIDogJyc7XG5cbiAgICAvLyBsYW5ndWFnZS0qIHRha2VzIHByZWNlZGVuY2Ugb3ZlciBub24tcHJlZml4ZWQgY2xhc3MgbmFtZXMuXG4gICAgbWF0Y2ggPSBsYW5ndWFnZVByZWZpeFJlLmV4ZWMoY2xhc3Nlcyk7XG4gICAgaWYgKG1hdGNoKSB7XG4gICAgICByZXR1cm4gZ2V0TGFuZ3VhZ2UobWF0Y2hbMV0pID8gbWF0Y2hbMV0gOiAnbm8taGlnaGxpZ2h0JztcbiAgICB9XG5cbiAgICBjbGFzc2VzID0gY2xhc3Nlcy5zcGxpdCgvXFxzKy8pO1xuXG4gICAgZm9yIChpID0gMCwgbGVuZ3RoID0gY2xhc3Nlcy5sZW5ndGg7IGkgPCBsZW5ndGg7IGkrKykge1xuICAgICAgX2NsYXNzID0gY2xhc3Nlc1tpXTtcblxuICAgICAgaWYgKGlzTm90SGlnaGxpZ2h0ZWQoX2NsYXNzKSB8fCBnZXRMYW5ndWFnZShfY2xhc3MpKSB7XG4gICAgICAgIHJldHVybiBfY2xhc3M7XG4gICAgICB9XG4gICAgfVxuICB9XG5cbiAgZnVuY3Rpb24gaW5oZXJpdChwYXJlbnQpIHsgIC8vIGluaGVyaXQocGFyZW50LCBvdmVycmlkZV9vYmosIG92ZXJyaWRlX29iaiwgLi4uKVxuICAgIHZhciBrZXk7XG4gICAgdmFyIHJlc3VsdCA9IHt9O1xuICAgIHZhciBvYmplY3RzID0gQXJyYXkucHJvdG90eXBlLnNsaWNlLmNhbGwoYXJndW1lbnRzLCAxKTtcblxuICAgIGZvciAoa2V5IGluIHBhcmVudClcbiAgICAgIHJlc3VsdFtrZXldID0gcGFyZW50W2tleV07XG4gICAgb2JqZWN0cy5mb3JFYWNoKGZ1bmN0aW9uKG9iaikge1xuICAgICAgZm9yIChrZXkgaW4gb2JqKVxuICAgICAgICByZXN1bHRba2V5XSA9IG9ialtrZXldO1xuICAgIH0pO1xuICAgIHJldHVybiByZXN1bHQ7XG4gIH1cblxuICAvKiBTdHJlYW0gbWVyZ2luZyAqL1xuXG4gIGZ1bmN0aW9uIG5vZGVTdHJlYW0obm9kZSkge1xuICAgIHZhciByZXN1bHQgPSBbXTtcbiAgICAoZnVuY3Rpb24gX25vZGVTdHJlYW0obm9kZSwgb2Zmc2V0KSB7XG4gICAgICBmb3IgKHZhciBjaGlsZCA9IG5vZGUuZmlyc3RDaGlsZDsgY2hpbGQ7IGNoaWxkID0gY2hpbGQubmV4dFNpYmxpbmcpIHtcbiAgICAgICAgaWYgKGNoaWxkLm5vZGVUeXBlID09PSAzKVxuICAgICAgICAgIG9mZnNldCArPSBjaGlsZC5ub2RlVmFsdWUubGVuZ3RoO1xuICAgICAgICBlbHNlIGlmIChjaGlsZC5ub2RlVHlwZSA9PT0gMSkge1xuICAgICAgICAgIHJlc3VsdC5wdXNoKHtcbiAgICAgICAgICAgIGV2ZW50OiAnc3RhcnQnLFxuICAgICAgICAgICAgb2Zmc2V0OiBvZmZzZXQsXG4gICAgICAgICAgICBub2RlOiBjaGlsZFxuICAgICAgICAgIH0pO1xuICAgICAgICAgIG9mZnNldCA9IF9ub2RlU3RyZWFtKGNoaWxkLCBvZmZzZXQpO1xuICAgICAgICAgIC8vIFByZXZlbnQgdm9pZCBlbGVtZW50cyBmcm9tIGhhdmluZyBhbiBlbmQgdGFnIHRoYXQgd291bGQgYWN0dWFsbHlcbiAgICAgICAgICAvLyBkb3VibGUgdGhlbSBpbiB0aGUgb3V0cHV0LiBUaGVyZSBhcmUgbW9yZSB2b2lkIGVsZW1lbnRzIGluIEhUTUxcbiAgICAgICAgICAvLyBidXQgd2UgbGlzdCBvbmx5IHRob3NlIHJlYWxpc3RpY2FsbHkgZXhwZWN0ZWQgaW4gY29kZSBkaXNwbGF5LlxuICAgICAgICAgIGlmICghdGFnKGNoaWxkKS5tYXRjaCgvYnJ8aHJ8aW1nfGlucHV0LykpIHtcbiAgICAgICAgICAgIHJlc3VsdC5wdXNoKHtcbiAgICAgICAgICAgICAgZXZlbnQ6ICdzdG9wJyxcbiAgICAgICAgICAgICAgb2Zmc2V0OiBvZmZzZXQsXG4gICAgICAgICAgICAgIG5vZGU6IGNoaWxkXG4gICAgICAgICAgICB9KTtcbiAgICAgICAgICB9XG4gICAgICAgIH1cbiAgICAgIH1cbiAgICAgIHJldHVybiBvZmZzZXQ7XG4gICAgfSkobm9kZSwgMCk7XG4gICAgcmV0dXJuIHJlc3VsdDtcbiAgfVxuXG4gIGZ1bmN0aW9uIG1lcmdlU3RyZWFtcyhvcmlnaW5hbCwgaGlnaGxpZ2h0ZWQsIHZhbHVlKSB7XG4gICAgdmFyIHByb2Nlc3NlZCA9IDA7XG4gICAgdmFyIHJlc3VsdCA9ICcnO1xuICAgIHZhciBub2RlU3RhY2sgPSBbXTtcblxuICAgIGZ1bmN0aW9uIHNlbGVjdFN0cmVhbSgpIHtcbiAgICAgIGlmICghb3JpZ2luYWwubGVuZ3RoIHx8ICFoaWdobGlnaHRlZC5sZW5ndGgpIHtcbiAgICAgICAgcmV0dXJuIG9yaWdpbmFsLmxlbmd0aCA/IG9yaWdpbmFsIDogaGlnaGxpZ2h0ZWQ7XG4gICAgICB9XG4gICAgICBpZiAob3JpZ2luYWxbMF0ub2Zmc2V0ICE9PSBoaWdobGlnaHRlZFswXS5vZmZzZXQpIHtcbiAgICAgICAgcmV0dXJuIChvcmlnaW5hbFswXS5vZmZzZXQgPCBoaWdobGlnaHRlZFswXS5vZmZzZXQpID8gb3JpZ2luYWwgOiBoaWdobGlnaHRlZDtcbiAgICAgIH1cblxuICAgICAgLypcbiAgICAgIFRvIGF2b2lkIHN0YXJ0aW5nIHRoZSBzdHJlYW0ganVzdCBiZWZvcmUgaXQgc2hvdWxkIHN0b3AgdGhlIG9yZGVyIGlzXG4gICAgICBlbnN1cmVkIHRoYXQgb3JpZ2luYWwgYWx3YXlzIHN0YXJ0cyBmaXJzdCBhbmQgY2xvc2VzIGxhc3Q6XG5cbiAgICAgIGlmIChldmVudDEgPT0gJ3N0YXJ0JyAmJiBldmVudDIgPT0gJ3N0YXJ0JylcbiAgICAgICAgcmV0dXJuIG9yaWdpbmFsO1xuICAgICAgaWYgKGV2ZW50MSA9PSAnc3RhcnQnICYmIGV2ZW50MiA9PSAnc3RvcCcpXG4gICAgICAgIHJldHVybiBoaWdobGlnaHRlZDtcbiAgICAgIGlmIChldmVudDEgPT0gJ3N0b3AnICYmIGV2ZW50MiA9PSAnc3RhcnQnKVxuICAgICAgICByZXR1cm4gb3JpZ2luYWw7XG4gICAgICBpZiAoZXZlbnQxID09ICdzdG9wJyAmJiBldmVudDIgPT0gJ3N0b3AnKVxuICAgICAgICByZXR1cm4gaGlnaGxpZ2h0ZWQ7XG5cbiAgICAgIC4uLiB3aGljaCBpcyBjb2xsYXBzZWQgdG86XG4gICAgICAqL1xuICAgICAgcmV0dXJuIGhpZ2hsaWdodGVkWzBdLmV2ZW50ID09PSAnc3RhcnQnID8gb3JpZ2luYWwgOiBoaWdobGlnaHRlZDtcbiAgICB9XG5cbiAgICBmdW5jdGlvbiBvcGVuKG5vZGUpIHtcbiAgICAgIGZ1bmN0aW9uIGF0dHJfc3RyKGEpIHtyZXR1cm4gJyAnICsgYS5ub2RlTmFtZSArICc9XCInICsgZXNjYXBlKGEudmFsdWUpLnJlcGxhY2UoJ1wiJywgJyZxdW90OycpICsgJ1wiJzt9XG4gICAgICByZXN1bHQgKz0gJzwnICsgdGFnKG5vZGUpICsgQXJyYXlQcm90by5tYXAuY2FsbChub2RlLmF0dHJpYnV0ZXMsIGF0dHJfc3RyKS5qb2luKCcnKSArICc+JztcbiAgICB9XG5cbiAgICBmdW5jdGlvbiBjbG9zZShub2RlKSB7XG4gICAgICByZXN1bHQgKz0gJzwvJyArIHRhZyhub2RlKSArICc+JztcbiAgICB9XG5cbiAgICBmdW5jdGlvbiByZW5kZXIoZXZlbnQpIHtcbiAgICAgIChldmVudC5ldmVudCA9PT0gJ3N0YXJ0JyA/IG9wZW4gOiBjbG9zZSkoZXZlbnQubm9kZSk7XG4gICAgfVxuXG4gICAgd2hpbGUgKG9yaWdpbmFsLmxlbmd0aCB8fCBoaWdobGlnaHRlZC5sZW5ndGgpIHtcbiAgICAgIHZhciBzdHJlYW0gPSBzZWxlY3RTdHJlYW0oKTtcbiAgICAgIHJlc3VsdCArPSBlc2NhcGUodmFsdWUuc3Vic3RyaW5nKHByb2Nlc3NlZCwgc3RyZWFtWzBdLm9mZnNldCkpO1xuICAgICAgcHJvY2Vzc2VkID0gc3RyZWFtWzBdLm9mZnNldDtcbiAgICAgIGlmIChzdHJlYW0gPT09IG9yaWdpbmFsKSB7XG4gICAgICAgIC8qXG4gICAgICAgIE9uIGFueSBvcGVuaW5nIG9yIGNsb3NpbmcgdGFnIG9mIHRoZSBvcmlnaW5hbCBtYXJrdXAgd2UgZmlyc3QgY2xvc2VcbiAgICAgICAgdGhlIGVudGlyZSBoaWdobGlnaHRlZCBub2RlIHN0YWNrLCB0aGVuIHJlbmRlciB0aGUgb3JpZ2luYWwgdGFnIGFsb25nXG4gICAgICAgIHdpdGggYWxsIHRoZSBmb2xsb3dpbmcgb3JpZ2luYWwgdGFncyBhdCB0aGUgc2FtZSBvZmZzZXQgYW5kIHRoZW5cbiAgICAgICAgcmVvcGVuIGFsbCB0aGUgdGFncyBvbiB0aGUgaGlnaGxpZ2h0ZWQgc3RhY2suXG4gICAgICAgICovXG4gICAgICAgIG5vZGVTdGFjay5yZXZlcnNlKCkuZm9yRWFjaChjbG9zZSk7XG4gICAgICAgIGRvIHtcbiAgICAgICAgICByZW5kZXIoc3RyZWFtLnNwbGljZSgwLCAxKVswXSk7XG4gICAgICAgICAgc3RyZWFtID0gc2VsZWN0U3RyZWFtKCk7XG4gICAgICAgIH0gd2hpbGUgKHN0cmVhbSA9PT0gb3JpZ2luYWwgJiYgc3RyZWFtLmxlbmd0aCAmJiBzdHJlYW1bMF0ub2Zmc2V0ID09PSBwcm9jZXNzZWQpO1xuICAgICAgICBub2RlU3RhY2sucmV2ZXJzZSgpLmZvckVhY2gob3Blbik7XG4gICAgICB9IGVsc2Uge1xuICAgICAgICBpZiAoc3RyZWFtWzBdLmV2ZW50ID09PSAnc3RhcnQnKSB7XG4gICAgICAgICAgbm9kZVN0YWNrLnB1c2goc3RyZWFtWzBdLm5vZGUpO1xuICAgICAgICB9IGVsc2Uge1xuICAgICAgICAgIG5vZGVTdGFjay5wb3AoKTtcbiAgICAgICAgfVxuICAgICAgICByZW5kZXIoc3RyZWFtLnNwbGljZSgwLCAxKVswXSk7XG4gICAgICB9XG4gICAgfVxuICAgIHJldHVybiByZXN1bHQgKyBlc2NhcGUodmFsdWUuc3Vic3RyKHByb2Nlc3NlZCkpO1xuICB9XG5cbiAgLyogSW5pdGlhbGl6YXRpb24gKi9cblxuICBmdW5jdGlvbiBleHBhbmRfbW9kZShtb2RlKSB7XG4gICAgaWYgKG1vZGUudmFyaWFudHMgJiYgIW1vZGUuY2FjaGVkX3ZhcmlhbnRzKSB7XG4gICAgICBtb2RlLmNhY2hlZF92YXJpYW50cyA9IG1vZGUudmFyaWFudHMubWFwKGZ1bmN0aW9uKHZhcmlhbnQpIHtcbiAgICAgICAgcmV0dXJuIGluaGVyaXQobW9kZSwge3ZhcmlhbnRzOiBudWxsfSwgdmFyaWFudCk7XG4gICAgICB9KTtcbiAgICB9XG4gICAgcmV0dXJuIG1vZGUuY2FjaGVkX3ZhcmlhbnRzIHx8IChtb2RlLmVuZHNXaXRoUGFyZW50ICYmIFtpbmhlcml0KG1vZGUpXSkgfHwgW21vZGVdO1xuICB9XG5cbiAgZnVuY3Rpb24gY29tcGlsZUxhbmd1YWdlKGxhbmd1YWdlKSB7XG5cbiAgICBmdW5jdGlvbiByZVN0cihyZSkge1xuICAgICAgICByZXR1cm4gKHJlICYmIHJlLnNvdXJjZSkgfHwgcmU7XG4gICAgfVxuXG4gICAgZnVuY3Rpb24gbGFuZ1JlKHZhbHVlLCBnbG9iYWwpIHtcbiAgICAgIHJldHVybiBuZXcgUmVnRXhwKFxuICAgICAgICByZVN0cih2YWx1ZSksXG4gICAgICAgICdtJyArIChsYW5ndWFnZS5jYXNlX2luc2Vuc2l0aXZlID8gJ2knIDogJycpICsgKGdsb2JhbCA/ICdnJyA6ICcnKVxuICAgICAgKTtcbiAgICB9XG5cbiAgICBmdW5jdGlvbiBjb21waWxlTW9kZShtb2RlLCBwYXJlbnQpIHtcbiAgICAgIGlmIChtb2RlLmNvbXBpbGVkKVxuICAgICAgICByZXR1cm47XG4gICAgICBtb2RlLmNvbXBpbGVkID0gdHJ1ZTtcblxuICAgICAgbW9kZS5rZXl3b3JkcyA9IG1vZGUua2V5d29yZHMgfHwgbW9kZS5iZWdpbktleXdvcmRzO1xuICAgICAgaWYgKG1vZGUua2V5d29yZHMpIHtcbiAgICAgICAgdmFyIGNvbXBpbGVkX2tleXdvcmRzID0ge307XG5cbiAgICAgICAgdmFyIGZsYXR0ZW4gPSBmdW5jdGlvbihjbGFzc05hbWUsIHN0cikge1xuICAgICAgICAgIGlmIChsYW5ndWFnZS5jYXNlX2luc2Vuc2l0aXZlKSB7XG4gICAgICAgICAgICBzdHIgPSBzdHIudG9Mb3dlckNhc2UoKTtcbiAgICAgICAgICB9XG4gICAgICAgICAgc3RyLnNwbGl0KCcgJykuZm9yRWFjaChmdW5jdGlvbihrdykge1xuICAgICAgICAgICAgdmFyIHBhaXIgPSBrdy5zcGxpdCgnfCcpO1xuICAgICAgICAgICAgY29tcGlsZWRfa2V5d29yZHNbcGFpclswXV0gPSBbY2xhc3NOYW1lLCBwYWlyWzFdID8gTnVtYmVyKHBhaXJbMV0pIDogMV07XG4gICAgICAgICAgfSk7XG4gICAgICAgIH07XG5cbiAgICAgICAgaWYgKHR5cGVvZiBtb2RlLmtleXdvcmRzID09PSAnc3RyaW5nJykgeyAvLyBzdHJpbmdcbiAgICAgICAgICBmbGF0dGVuKCdrZXl3b3JkJywgbW9kZS5rZXl3b3Jkcyk7XG4gICAgICAgIH0gZWxzZSB7XG4gICAgICAgICAgb2JqZWN0S2V5cyhtb2RlLmtleXdvcmRzKS5mb3JFYWNoKGZ1bmN0aW9uIChjbGFzc05hbWUpIHtcbiAgICAgICAgICAgIGZsYXR0ZW4oY2xhc3NOYW1lLCBtb2RlLmtleXdvcmRzW2NsYXNzTmFtZV0pO1xuICAgICAgICAgIH0pO1xuICAgICAgICB9XG4gICAgICAgIG1vZGUua2V5d29yZHMgPSBjb21waWxlZF9rZXl3b3JkcztcbiAgICAgIH1cbiAgICAgIG1vZGUubGV4ZW1lc1JlID0gbGFuZ1JlKG1vZGUubGV4ZW1lcyB8fCAvXFx3Ky8sIHRydWUpO1xuXG4gICAgICBpZiAocGFyZW50KSB7XG4gICAgICAgIGlmIChtb2RlLmJlZ2luS2V5d29yZHMpIHtcbiAgICAgICAgICBtb2RlLmJlZ2luID0gJ1xcXFxiKCcgKyBtb2RlLmJlZ2luS2V5d29yZHMuc3BsaXQoJyAnKS5qb2luKCd8JykgKyAnKVxcXFxiJztcbiAgICAgICAgfVxuICAgICAgICBpZiAoIW1vZGUuYmVnaW4pXG4gICAgICAgICAgbW9kZS5iZWdpbiA9IC9cXEJ8XFxiLztcbiAgICAgICAgbW9kZS5iZWdpblJlID0gbGFuZ1JlKG1vZGUuYmVnaW4pO1xuICAgICAgICBpZiAobW9kZS5lbmRTYW1lQXNCZWdpbilcbiAgICAgICAgICBtb2RlLmVuZCA9IG1vZGUuYmVnaW47XG4gICAgICAgIGlmICghbW9kZS5lbmQgJiYgIW1vZGUuZW5kc1dpdGhQYXJlbnQpXG4gICAgICAgICAgbW9kZS5lbmQgPSAvXFxCfFxcYi87XG4gICAgICAgIGlmIChtb2RlLmVuZClcbiAgICAgICAgICBtb2RlLmVuZFJlID0gbGFuZ1JlKG1vZGUuZW5kKTtcbiAgICAgICAgbW9kZS50ZXJtaW5hdG9yX2VuZCA9IHJlU3RyKG1vZGUuZW5kKSB8fCAnJztcbiAgICAgICAgaWYgKG1vZGUuZW5kc1dpdGhQYXJlbnQgJiYgcGFyZW50LnRlcm1pbmF0b3JfZW5kKVxuICAgICAgICAgIG1vZGUudGVybWluYXRvcl9lbmQgKz0gKG1vZGUuZW5kID8gJ3wnIDogJycpICsgcGFyZW50LnRlcm1pbmF0b3JfZW5kO1xuICAgICAgfVxuICAgICAgaWYgKG1vZGUuaWxsZWdhbClcbiAgICAgICAgbW9kZS5pbGxlZ2FsUmUgPSBsYW5nUmUobW9kZS5pbGxlZ2FsKTtcbiAgICAgIGlmIChtb2RlLnJlbGV2YW5jZSA9PSBudWxsKVxuICAgICAgICBtb2RlLnJlbGV2YW5jZSA9IDE7XG4gICAgICBpZiAoIW1vZGUuY29udGFpbnMpIHtcbiAgICAgICAgbW9kZS5jb250YWlucyA9IFtdO1xuICAgICAgfVxuICAgICAgbW9kZS5jb250YWlucyA9IEFycmF5LnByb3RvdHlwZS5jb25jYXQuYXBwbHkoW10sIG1vZGUuY29udGFpbnMubWFwKGZ1bmN0aW9uKGMpIHtcbiAgICAgICAgcmV0dXJuIGV4cGFuZF9tb2RlKGMgPT09ICdzZWxmJyA/IG1vZGUgOiBjKTtcbiAgICAgIH0pKTtcbiAgICAgIG1vZGUuY29udGFpbnMuZm9yRWFjaChmdW5jdGlvbihjKSB7Y29tcGlsZU1vZGUoYywgbW9kZSk7fSk7XG5cbiAgICAgIGlmIChtb2RlLnN0YXJ0cykge1xuICAgICAgICBjb21waWxlTW9kZShtb2RlLnN0YXJ0cywgcGFyZW50KTtcbiAgICAgIH1cblxuICAgICAgdmFyIHRlcm1pbmF0b3JzID1cbiAgICAgICAgbW9kZS5jb250YWlucy5tYXAoZnVuY3Rpb24oYykge1xuICAgICAgICAgIHJldHVybiBjLmJlZ2luS2V5d29yZHMgPyAnXFxcXC4/KCcgKyBjLmJlZ2luICsgJylcXFxcLj8nIDogYy5iZWdpbjtcbiAgICAgICAgfSlcbiAgICAgICAgLmNvbmNhdChbbW9kZS50ZXJtaW5hdG9yX2VuZCwgbW9kZS5pbGxlZ2FsXSlcbiAgICAgICAgLm1hcChyZVN0cilcbiAgICAgICAgLmZpbHRlcihCb29sZWFuKTtcbiAgICAgIG1vZGUudGVybWluYXRvcnMgPSB0ZXJtaW5hdG9ycy5sZW5ndGggPyBsYW5nUmUodGVybWluYXRvcnMuam9pbignfCcpLCB0cnVlKSA6IHtleGVjOiBmdW5jdGlvbigvKnMqLykge3JldHVybiBudWxsO319O1xuICAgIH1cblxuICAgIGNvbXBpbGVNb2RlKGxhbmd1YWdlKTtcbiAgfVxuXG4gIC8qXG4gIENvcmUgaGlnaGxpZ2h0aW5nIGZ1bmN0aW9uLiBBY2NlcHRzIGEgbGFuZ3VhZ2UgbmFtZSwgb3IgYW4gYWxpYXMsIGFuZCBhXG4gIHN0cmluZyB3aXRoIHRoZSBjb2RlIHRvIGhpZ2hsaWdodC4gUmV0dXJucyBhbiBvYmplY3Qgd2l0aCB0aGUgZm9sbG93aW5nXG4gIHByb3BlcnRpZXM6XG5cbiAgLSByZWxldmFuY2UgKGludClcbiAgLSB2YWx1ZSAoYW4gSFRNTCBzdHJpbmcgd2l0aCBoaWdobGlnaHRpbmcgbWFya3VwKVxuXG4gICovXG4gIGZ1bmN0aW9uIGhpZ2hsaWdodChuYW1lLCB2YWx1ZSwgaWdub3JlX2lsbGVnYWxzLCBjb250aW51YXRpb24pIHtcblxuICAgIGZ1bmN0aW9uIGVzY2FwZVJlKHZhbHVlKSB7XG4gICAgICByZXR1cm4gbmV3IFJlZ0V4cCh2YWx1ZS5yZXBsYWNlKC9bLVxcL1xcXFxeJCorPy4oKXxbXFxde31dL2csICdcXFxcJCYnKSwgJ20nKTtcbiAgICB9XG5cbiAgICBmdW5jdGlvbiBzdWJNb2RlKGxleGVtZSwgbW9kZSkge1xuICAgICAgdmFyIGksIGxlbmd0aDtcblxuICAgICAgZm9yIChpID0gMCwgbGVuZ3RoID0gbW9kZS5jb250YWlucy5sZW5ndGg7IGkgPCBsZW5ndGg7IGkrKykge1xuICAgICAgICBpZiAodGVzdFJlKG1vZGUuY29udGFpbnNbaV0uYmVnaW5SZSwgbGV4ZW1lKSkge1xuICAgICAgICAgIGlmIChtb2RlLmNvbnRhaW5zW2ldLmVuZFNhbWVBc0JlZ2luKSB7XG4gICAgICAgICAgICBtb2RlLmNvbnRhaW5zW2ldLmVuZFJlID0gZXNjYXBlUmUoIG1vZGUuY29udGFpbnNbaV0uYmVnaW5SZS5leGVjKGxleGVtZSlbMF0gKTtcbiAgICAgICAgICB9XG4gICAgICAgICAgcmV0dXJuIG1vZGUuY29udGFpbnNbaV07XG4gICAgICAgIH1cbiAgICAgIH1cbiAgICB9XG5cbiAgICBmdW5jdGlvbiBlbmRPZk1vZGUobW9kZSwgbGV4ZW1lKSB7XG4gICAgICBpZiAodGVzdFJlKG1vZGUuZW5kUmUsIGxleGVtZSkpIHtcbiAgICAgICAgd2hpbGUgKG1vZGUuZW5kc1BhcmVudCAmJiBtb2RlLnBhcmVudCkge1xuICAgICAgICAgIG1vZGUgPSBtb2RlLnBhcmVudDtcbiAgICAgICAgfVxuICAgICAgICByZXR1cm4gbW9kZTtcbiAgICAgIH1cbiAgICAgIGlmIChtb2RlLmVuZHNXaXRoUGFyZW50KSB7XG4gICAgICAgIHJldHVybiBlbmRPZk1vZGUobW9kZS5wYXJlbnQsIGxleGVtZSk7XG4gICAgICB9XG4gICAgfVxuXG4gICAgZnVuY3Rpb24gaXNJbGxlZ2FsKGxleGVtZSwgbW9kZSkge1xuICAgICAgcmV0dXJuICFpZ25vcmVfaWxsZWdhbHMgJiYgdGVzdFJlKG1vZGUuaWxsZWdhbFJlLCBsZXhlbWUpO1xuICAgIH1cblxuICAgIGZ1bmN0aW9uIGtleXdvcmRNYXRjaChtb2RlLCBtYXRjaCkge1xuICAgICAgdmFyIG1hdGNoX3N0ciA9IGxhbmd1YWdlLmNhc2VfaW5zZW5zaXRpdmUgPyBtYXRjaFswXS50b0xvd2VyQ2FzZSgpIDogbWF0Y2hbMF07XG4gICAgICByZXR1cm4gbW9kZS5rZXl3b3Jkcy5oYXNPd25Qcm9wZXJ0eShtYXRjaF9zdHIpICYmIG1vZGUua2V5d29yZHNbbWF0Y2hfc3RyXTtcbiAgICB9XG5cbiAgICBmdW5jdGlvbiBidWlsZFNwYW4oY2xhc3NuYW1lLCBpbnNpZGVTcGFuLCBsZWF2ZU9wZW4sIG5vUHJlZml4KSB7XG4gICAgICB2YXIgY2xhc3NQcmVmaXggPSBub1ByZWZpeCA/ICcnIDogb3B0aW9ucy5jbGFzc1ByZWZpeCxcbiAgICAgICAgICBvcGVuU3BhbiAgICA9ICc8c3BhbiBjbGFzcz1cIicgKyBjbGFzc1ByZWZpeCxcbiAgICAgICAgICBjbG9zZVNwYW4gICA9IGxlYXZlT3BlbiA/ICcnIDogc3BhbkVuZFRhZztcblxuICAgICAgb3BlblNwYW4gKz0gY2xhc3NuYW1lICsgJ1wiPic7XG5cbiAgICAgIHJldHVybiBvcGVuU3BhbiArIGluc2lkZVNwYW4gKyBjbG9zZVNwYW47XG4gICAgfVxuXG4gICAgZnVuY3Rpb24gcHJvY2Vzc0tleXdvcmRzKCkge1xuICAgICAgdmFyIGtleXdvcmRfbWF0Y2gsIGxhc3RfaW5kZXgsIG1hdGNoLCByZXN1bHQ7XG5cbiAgICAgIGlmICghdG9wLmtleXdvcmRzKVxuICAgICAgICByZXR1cm4gZXNjYXBlKG1vZGVfYnVmZmVyKTtcblxuICAgICAgcmVzdWx0ID0gJyc7XG4gICAgICBsYXN0X2luZGV4ID0gMDtcbiAgICAgIHRvcC5sZXhlbWVzUmUubGFzdEluZGV4ID0gMDtcbiAgICAgIG1hdGNoID0gdG9wLmxleGVtZXNSZS5leGVjKG1vZGVfYnVmZmVyKTtcblxuICAgICAgd2hpbGUgKG1hdGNoKSB7XG4gICAgICAgIHJlc3VsdCArPSBlc2NhcGUobW9kZV9idWZmZXIuc3Vic3RyaW5nKGxhc3RfaW5kZXgsIG1hdGNoLmluZGV4KSk7XG4gICAgICAgIGtleXdvcmRfbWF0Y2ggPSBrZXl3b3JkTWF0Y2godG9wLCBtYXRjaCk7XG4gICAgICAgIGlmIChrZXl3b3JkX21hdGNoKSB7XG4gICAgICAgICAgcmVsZXZhbmNlICs9IGtleXdvcmRfbWF0Y2hbMV07XG4gICAgICAgICAgcmVzdWx0ICs9IGJ1aWxkU3BhbihrZXl3b3JkX21hdGNoWzBdLCBlc2NhcGUobWF0Y2hbMF0pKTtcbiAgICAgICAgfSBlbHNlIHtcbiAgICAgICAgICByZXN1bHQgKz0gZXNjYXBlKG1hdGNoWzBdKTtcbiAgICAgICAgfVxuICAgICAgICBsYXN0X2luZGV4ID0gdG9wLmxleGVtZXNSZS5sYXN0SW5kZXg7XG4gICAgICAgIG1hdGNoID0gdG9wLmxleGVtZXNSZS5leGVjKG1vZGVfYnVmZmVyKTtcbiAgICAgIH1cbiAgICAgIHJldHVybiByZXN1bHQgKyBlc2NhcGUobW9kZV9idWZmZXIuc3Vic3RyKGxhc3RfaW5kZXgpKTtcbiAgICB9XG5cbiAgICBmdW5jdGlvbiBwcm9jZXNzU3ViTGFuZ3VhZ2UoKSB7XG4gICAgICB2YXIgZXhwbGljaXQgPSB0eXBlb2YgdG9wLnN1Ykxhbmd1YWdlID09PSAnc3RyaW5nJztcbiAgICAgIGlmIChleHBsaWNpdCAmJiAhbGFuZ3VhZ2VzW3RvcC5zdWJMYW5ndWFnZV0pIHtcbiAgICAgICAgcmV0dXJuIGVzY2FwZShtb2RlX2J1ZmZlcik7XG4gICAgICB9XG5cbiAgICAgIHZhciByZXN1bHQgPSBleHBsaWNpdCA/XG4gICAgICAgICAgICAgICAgICAgaGlnaGxpZ2h0KHRvcC5zdWJMYW5ndWFnZSwgbW9kZV9idWZmZXIsIHRydWUsIGNvbnRpbnVhdGlvbnNbdG9wLnN1Ykxhbmd1YWdlXSkgOlxuICAgICAgICAgICAgICAgICAgIGhpZ2hsaWdodEF1dG8obW9kZV9idWZmZXIsIHRvcC5zdWJMYW5ndWFnZS5sZW5ndGggPyB0b3Auc3ViTGFuZ3VhZ2UgOiB1bmRlZmluZWQpO1xuXG4gICAgICAvLyBDb3VudGluZyBlbWJlZGRlZCBsYW5ndWFnZSBzY29yZSB0b3dhcmRzIHRoZSBob3N0IGxhbmd1YWdlIG1heSBiZSBkaXNhYmxlZFxuICAgICAgLy8gd2l0aCB6ZXJvaW5nIHRoZSBjb250YWluaW5nIG1vZGUgcmVsZXZhbmNlLiBVc2VjYXNlIGluIHBvaW50IGlzIE1hcmtkb3duIHRoYXRcbiAgICAgIC8vIGFsbG93cyBYTUwgZXZlcnl3aGVyZSBhbmQgbWFrZXMgZXZlcnkgWE1MIHNuaXBwZXQgdG8gaGF2ZSBhIG11Y2ggbGFyZ2VyIE1hcmtkb3duXG4gICAgICAvLyBzY29yZS5cbiAgICAgIGlmICh0b3AucmVsZXZhbmNlID4gMCkge1xuICAgICAgICByZWxldmFuY2UgKz0gcmVzdWx0LnJlbGV2YW5jZTtcbiAgICAgIH1cbiAgICAgIGlmIChleHBsaWNpdCkge1xuICAgICAgICBjb250aW51YXRpb25zW3RvcC5zdWJMYW5ndWFnZV0gPSByZXN1bHQudG9wO1xuICAgICAgfVxuICAgICAgcmV0dXJuIGJ1aWxkU3BhbihyZXN1bHQubGFuZ3VhZ2UsIHJlc3VsdC52YWx1ZSwgZmFsc2UsIHRydWUpO1xuICAgIH1cblxuICAgIGZ1bmN0aW9uIHByb2Nlc3NCdWZmZXIoKSB7XG4gICAgICByZXN1bHQgKz0gKHRvcC5zdWJMYW5ndWFnZSAhPSBudWxsID8gcHJvY2Vzc1N1Ykxhbmd1YWdlKCkgOiBwcm9jZXNzS2V5d29yZHMoKSk7XG4gICAgICBtb2RlX2J1ZmZlciA9ICcnO1xuICAgIH1cblxuICAgIGZ1bmN0aW9uIHN0YXJ0TmV3TW9kZShtb2RlKSB7XG4gICAgICByZXN1bHQgKz0gbW9kZS5jbGFzc05hbWU/IGJ1aWxkU3Bhbihtb2RlLmNsYXNzTmFtZSwgJycsIHRydWUpOiAnJztcbiAgICAgIHRvcCA9IE9iamVjdC5jcmVhdGUobW9kZSwge3BhcmVudDoge3ZhbHVlOiB0b3B9fSk7XG4gICAgfVxuXG4gICAgZnVuY3Rpb24gcHJvY2Vzc0xleGVtZShidWZmZXIsIGxleGVtZSkge1xuXG4gICAgICBtb2RlX2J1ZmZlciArPSBidWZmZXI7XG5cbiAgICAgIGlmIChsZXhlbWUgPT0gbnVsbCkge1xuICAgICAgICBwcm9jZXNzQnVmZmVyKCk7XG4gICAgICAgIHJldHVybiAwO1xuICAgICAgfVxuXG4gICAgICB2YXIgbmV3X21vZGUgPSBzdWJNb2RlKGxleGVtZSwgdG9wKTtcbiAgICAgIGlmIChuZXdfbW9kZSkge1xuICAgICAgICBpZiAobmV3X21vZGUuc2tpcCkge1xuICAgICAgICAgIG1vZGVfYnVmZmVyICs9IGxleGVtZTtcbiAgICAgICAgfSBlbHNlIHtcbiAgICAgICAgICBpZiAobmV3X21vZGUuZXhjbHVkZUJlZ2luKSB7XG4gICAgICAgICAgICBtb2RlX2J1ZmZlciArPSBsZXhlbWU7XG4gICAgICAgICAgfVxuICAgICAgICAgIHByb2Nlc3NCdWZmZXIoKTtcbiAgICAgICAgICBpZiAoIW5ld19tb2RlLnJldHVybkJlZ2luICYmICFuZXdfbW9kZS5leGNsdWRlQmVnaW4pIHtcbiAgICAgICAgICAgIG1vZGVfYnVmZmVyID0gbGV4ZW1lO1xuICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgICAgICBzdGFydE5ld01vZGUobmV3X21vZGUsIGxleGVtZSk7XG4gICAgICAgIHJldHVybiBuZXdfbW9kZS5yZXR1cm5CZWdpbiA/IDAgOiBsZXhlbWUubGVuZ3RoO1xuICAgICAgfVxuXG4gICAgICB2YXIgZW5kX21vZGUgPSBlbmRPZk1vZGUodG9wLCBsZXhlbWUpO1xuICAgICAgaWYgKGVuZF9tb2RlKSB7XG4gICAgICAgIHZhciBvcmlnaW4gPSB0b3A7XG4gICAgICAgIGlmIChvcmlnaW4uc2tpcCkge1xuICAgICAgICAgIG1vZGVfYnVmZmVyICs9IGxleGVtZTtcbiAgICAgICAgfSBlbHNlIHtcbiAgICAgICAgICBpZiAoIShvcmlnaW4ucmV0dXJuRW5kIHx8IG9yaWdpbi5leGNsdWRlRW5kKSkge1xuICAgICAgICAgICAgbW9kZV9idWZmZXIgKz0gbGV4ZW1lO1xuICAgICAgICAgIH1cbiAgICAgICAgICBwcm9jZXNzQnVmZmVyKCk7XG4gICAgICAgICAgaWYgKG9yaWdpbi5leGNsdWRlRW5kKSB7XG4gICAgICAgICAgICBtb2RlX2J1ZmZlciA9IGxleGVtZTtcbiAgICAgICAgICB9XG4gICAgICAgIH1cbiAgICAgICAgZG8ge1xuICAgICAgICAgIGlmICh0b3AuY2xhc3NOYW1lKSB7XG4gICAgICAgICAgICByZXN1bHQgKz0gc3BhbkVuZFRhZztcbiAgICAgICAgICB9XG4gICAgICAgICAgaWYgKCF0b3Auc2tpcCAmJiAhdG9wLnN1Ykxhbmd1YWdlKSB7XG4gICAgICAgICAgICByZWxldmFuY2UgKz0gdG9wLnJlbGV2YW5jZTtcbiAgICAgICAgICB9XG4gICAgICAgICAgdG9wID0gdG9wLnBhcmVudDtcbiAgICAgICAgfSB3aGlsZSAodG9wICE9PSBlbmRfbW9kZS5wYXJlbnQpO1xuICAgICAgICBpZiAoZW5kX21vZGUuc3RhcnRzKSB7XG4gICAgICAgICAgaWYgKGVuZF9tb2RlLmVuZFNhbWVBc0JlZ2luKSB7XG4gICAgICAgICAgICBlbmRfbW9kZS5zdGFydHMuZW5kUmUgPSBlbmRfbW9kZS5lbmRSZTtcbiAgICAgICAgICB9XG4gICAgICAgICAgc3RhcnROZXdNb2RlKGVuZF9tb2RlLnN0YXJ0cywgJycpO1xuICAgICAgICB9XG4gICAgICAgIHJldHVybiBvcmlnaW4ucmV0dXJuRW5kID8gMCA6IGxleGVtZS5sZW5ndGg7XG4gICAgICB9XG5cbiAgICAgIGlmIChpc0lsbGVnYWwobGV4ZW1lLCB0b3ApKVxuICAgICAgICB0aHJvdyBuZXcgRXJyb3IoJ0lsbGVnYWwgbGV4ZW1lIFwiJyArIGxleGVtZSArICdcIiBmb3IgbW9kZSBcIicgKyAodG9wLmNsYXNzTmFtZSB8fCAnPHVubmFtZWQ+JykgKyAnXCInKTtcblxuICAgICAgLypcbiAgICAgIFBhcnNlciBzaG91bGQgbm90IHJlYWNoIHRoaXMgcG9pbnQgYXMgYWxsIHR5cGVzIG9mIGxleGVtZXMgc2hvdWxkIGJlIGNhdWdodFxuICAgICAgZWFybGllciwgYnV0IGlmIGl0IGRvZXMgZHVlIHRvIHNvbWUgYnVnIG1ha2Ugc3VyZSBpdCBhZHZhbmNlcyBhdCBsZWFzdCBvbmVcbiAgICAgIGNoYXJhY3RlciBmb3J3YXJkIHRvIHByZXZlbnQgaW5maW5pdGUgbG9vcGluZy5cbiAgICAgICovXG4gICAgICBtb2RlX2J1ZmZlciArPSBsZXhlbWU7XG4gICAgICByZXR1cm4gbGV4ZW1lLmxlbmd0aCB8fCAxO1xuICAgIH1cblxuICAgIHZhciBsYW5ndWFnZSA9IGdldExhbmd1YWdlKG5hbWUpO1xuICAgIGlmICghbGFuZ3VhZ2UpIHtcbiAgICAgIHRocm93IG5ldyBFcnJvcignVW5rbm93biBsYW5ndWFnZTogXCInICsgbmFtZSArICdcIicpO1xuICAgIH1cblxuICAgIGNvbXBpbGVMYW5ndWFnZShsYW5ndWFnZSk7XG4gICAgdmFyIHRvcCA9IGNvbnRpbnVhdGlvbiB8fCBsYW5ndWFnZTtcbiAgICB2YXIgY29udGludWF0aW9ucyA9IHt9OyAvLyBrZWVwIGNvbnRpbnVhdGlvbnMgZm9yIHN1Yi1sYW5ndWFnZXNcbiAgICB2YXIgcmVzdWx0ID0gJycsIGN1cnJlbnQ7XG4gICAgZm9yKGN1cnJlbnQgPSB0b3A7IGN1cnJlbnQgIT09IGxhbmd1YWdlOyBjdXJyZW50ID0gY3VycmVudC5wYXJlbnQpIHtcbiAgICAgIGlmIChjdXJyZW50LmNsYXNzTmFtZSkge1xuICAgICAgICByZXN1bHQgPSBidWlsZFNwYW4oY3VycmVudC5jbGFzc05hbWUsICcnLCB0cnVlKSArIHJlc3VsdDtcbiAgICAgIH1cbiAgICB9XG4gICAgdmFyIG1vZGVfYnVmZmVyID0gJyc7XG4gICAgdmFyIHJlbGV2YW5jZSA9IDA7XG4gICAgdHJ5IHtcbiAgICAgIHZhciBtYXRjaCwgY291bnQsIGluZGV4ID0gMDtcbiAgICAgIHdoaWxlICh0cnVlKSB7XG4gICAgICAgIHRvcC50ZXJtaW5hdG9ycy5sYXN0SW5kZXggPSBpbmRleDtcbiAgICAgICAgbWF0Y2ggPSB0b3AudGVybWluYXRvcnMuZXhlYyh2YWx1ZSk7XG4gICAgICAgIGlmICghbWF0Y2gpXG4gICAgICAgICAgYnJlYWs7XG4gICAgICAgIGNvdW50ID0gcHJvY2Vzc0xleGVtZSh2YWx1ZS5zdWJzdHJpbmcoaW5kZXgsIG1hdGNoLmluZGV4KSwgbWF0Y2hbMF0pO1xuICAgICAgICBpbmRleCA9IG1hdGNoLmluZGV4ICsgY291bnQ7XG4gICAgICB9XG4gICAgICBwcm9jZXNzTGV4ZW1lKHZhbHVlLnN1YnN0cihpbmRleCkpO1xuICAgICAgZm9yKGN1cnJlbnQgPSB0b3A7IGN1cnJlbnQucGFyZW50OyBjdXJyZW50ID0gY3VycmVudC5wYXJlbnQpIHsgLy8gY2xvc2UgZGFuZ2xpbmcgbW9kZXNcbiAgICAgICAgaWYgKGN1cnJlbnQuY2xhc3NOYW1lKSB7XG4gICAgICAgICAgcmVzdWx0ICs9IHNwYW5FbmRUYWc7XG4gICAgICAgIH1cbiAgICAgIH1cbiAgICAgIHJldHVybiB7XG4gICAgICAgIHJlbGV2YW5jZTogcmVsZXZhbmNlLFxuICAgICAgICB2YWx1ZTogcmVzdWx0LFxuICAgICAgICBsYW5ndWFnZTogbmFtZSxcbiAgICAgICAgdG9wOiB0b3BcbiAgICAgIH07XG4gICAgfSBjYXRjaCAoZSkge1xuICAgICAgaWYgKGUubWVzc2FnZSAmJiBlLm1lc3NhZ2UuaW5kZXhPZignSWxsZWdhbCcpICE9PSAtMSkge1xuICAgICAgICByZXR1cm4ge1xuICAgICAgICAgIHJlbGV2YW5jZTogMCxcbiAgICAgICAgICB2YWx1ZTogZXNjYXBlKHZhbHVlKVxuICAgICAgICB9O1xuICAgICAgfSBlbHNlIHtcbiAgICAgICAgdGhyb3cgZTtcbiAgICAgIH1cbiAgICB9XG4gIH1cblxuICAvKlxuICBIaWdobGlnaHRpbmcgd2l0aCBsYW5ndWFnZSBkZXRlY3Rpb24uIEFjY2VwdHMgYSBzdHJpbmcgd2l0aCB0aGUgY29kZSB0b1xuICBoaWdobGlnaHQuIFJldHVybnMgYW4gb2JqZWN0IHdpdGggdGhlIGZvbGxvd2luZyBwcm9wZXJ0aWVzOlxuXG4gIC0gbGFuZ3VhZ2UgKGRldGVjdGVkIGxhbmd1YWdlKVxuICAtIHJlbGV2YW5jZSAoaW50KVxuICAtIHZhbHVlIChhbiBIVE1MIHN0cmluZyB3aXRoIGhpZ2hsaWdodGluZyBtYXJrdXApXG4gIC0gc2Vjb25kX2Jlc3QgKG9iamVjdCB3aXRoIHRoZSBzYW1lIHN0cnVjdHVyZSBmb3Igc2Vjb25kLWJlc3QgaGV1cmlzdGljYWxseVxuICAgIGRldGVjdGVkIGxhbmd1YWdlLCBtYXkgYmUgYWJzZW50KVxuXG4gICovXG4gIGZ1bmN0aW9uIGhpZ2hsaWdodEF1dG8odGV4dCwgbGFuZ3VhZ2VTdWJzZXQpIHtcbiAgICBsYW5ndWFnZVN1YnNldCA9IGxhbmd1YWdlU3Vic2V0IHx8IG9wdGlvbnMubGFuZ3VhZ2VzIHx8IG9iamVjdEtleXMobGFuZ3VhZ2VzKTtcbiAgICB2YXIgcmVzdWx0ID0ge1xuICAgICAgcmVsZXZhbmNlOiAwLFxuICAgICAgdmFsdWU6IGVzY2FwZSh0ZXh0KVxuICAgIH07XG4gICAgdmFyIHNlY29uZF9iZXN0ID0gcmVzdWx0O1xuICAgIGxhbmd1YWdlU3Vic2V0LmZpbHRlcihnZXRMYW5ndWFnZSkuZmlsdGVyKGF1dG9EZXRlY3Rpb24pLmZvckVhY2goZnVuY3Rpb24obmFtZSkge1xuICAgICAgdmFyIGN1cnJlbnQgPSBoaWdobGlnaHQobmFtZSwgdGV4dCwgZmFsc2UpO1xuICAgICAgY3VycmVudC5sYW5ndWFnZSA9IG5hbWU7XG4gICAgICBpZiAoY3VycmVudC5yZWxldmFuY2UgPiBzZWNvbmRfYmVzdC5yZWxldmFuY2UpIHtcbiAgICAgICAgc2Vjb25kX2Jlc3QgPSBjdXJyZW50O1xuICAgICAgfVxuICAgICAgaWYgKGN1cnJlbnQucmVsZXZhbmNlID4gcmVzdWx0LnJlbGV2YW5jZSkge1xuICAgICAgICBzZWNvbmRfYmVzdCA9IHJlc3VsdDtcbiAgICAgICAgcmVzdWx0ID0gY3VycmVudDtcbiAgICAgIH1cbiAgICB9KTtcbiAgICBpZiAoc2Vjb25kX2Jlc3QubGFuZ3VhZ2UpIHtcbiAgICAgIHJlc3VsdC5zZWNvbmRfYmVzdCA9IHNlY29uZF9iZXN0O1xuICAgIH1cbiAgICByZXR1cm4gcmVzdWx0O1xuICB9XG5cbiAgLypcbiAgUG9zdC1wcm9jZXNzaW5nIG9mIHRoZSBoaWdobGlnaHRlZCBtYXJrdXA6XG5cbiAgLSByZXBsYWNlIFRBQnMgd2l0aCBzb21ldGhpbmcgbW9yZSB1c2VmdWxcbiAgLSByZXBsYWNlIHJlYWwgbGluZS1icmVha3Mgd2l0aCAnPGJyPicgZm9yIG5vbi1wcmUgY29udGFpbmVyc1xuXG4gICovXG4gIGZ1bmN0aW9uIGZpeE1hcmt1cCh2YWx1ZSkge1xuICAgIHJldHVybiAhKG9wdGlvbnMudGFiUmVwbGFjZSB8fCBvcHRpb25zLnVzZUJSKVxuICAgICAgPyB2YWx1ZVxuICAgICAgOiB2YWx1ZS5yZXBsYWNlKGZpeE1hcmt1cFJlLCBmdW5jdGlvbihtYXRjaCwgcDEpIHtcbiAgICAgICAgICBpZiAob3B0aW9ucy51c2VCUiAmJiBtYXRjaCA9PT0gJ1xcbicpIHtcbiAgICAgICAgICAgIHJldHVybiAnPGJyPic7XG4gICAgICAgICAgfSBlbHNlIGlmIChvcHRpb25zLnRhYlJlcGxhY2UpIHtcbiAgICAgICAgICAgIHJldHVybiBwMS5yZXBsYWNlKC9cXHQvZywgb3B0aW9ucy50YWJSZXBsYWNlKTtcbiAgICAgICAgICB9XG4gICAgICAgICAgcmV0dXJuICcnO1xuICAgICAgfSk7XG4gIH1cblxuICBmdW5jdGlvbiBidWlsZENsYXNzTmFtZShwcmV2Q2xhc3NOYW1lLCBjdXJyZW50TGFuZywgcmVzdWx0TGFuZykge1xuICAgIHZhciBsYW5ndWFnZSA9IGN1cnJlbnRMYW5nID8gYWxpYXNlc1tjdXJyZW50TGFuZ10gOiByZXN1bHRMYW5nLFxuICAgICAgICByZXN1bHQgICA9IFtwcmV2Q2xhc3NOYW1lLnRyaW0oKV07XG5cbiAgICBpZiAoIXByZXZDbGFzc05hbWUubWF0Y2goL1xcYmhsanNcXGIvKSkge1xuICAgICAgcmVzdWx0LnB1c2goJ2hsanMnKTtcbiAgICB9XG5cbiAgICBpZiAocHJldkNsYXNzTmFtZS5pbmRleE9mKGxhbmd1YWdlKSA9PT0gLTEpIHtcbiAgICAgIHJlc3VsdC5wdXNoKGxhbmd1YWdlKTtcbiAgICB9XG5cbiAgICByZXR1cm4gcmVzdWx0LmpvaW4oJyAnKS50cmltKCk7XG4gIH1cblxuICAvKlxuICBBcHBsaWVzIGhpZ2hsaWdodGluZyB0byBhIERPTSBub2RlIGNvbnRhaW5pbmcgY29kZS4gQWNjZXB0cyBhIERPTSBub2RlIGFuZFxuICB0d28gb3B0aW9uYWwgcGFyYW1ldGVycyBmb3IgZml4TWFya3VwLlxuICAqL1xuICBmdW5jdGlvbiBoaWdobGlnaHRCbG9jayhibG9jaykge1xuICAgIHZhciBub2RlLCBvcmlnaW5hbFN0cmVhbSwgcmVzdWx0LCByZXN1bHROb2RlLCB0ZXh0O1xuICAgIHZhciBsYW5ndWFnZSA9IGJsb2NrTGFuZ3VhZ2UoYmxvY2spO1xuXG4gICAgaWYgKGlzTm90SGlnaGxpZ2h0ZWQobGFuZ3VhZ2UpKVxuICAgICAgICByZXR1cm47XG5cbiAgICBpZiAob3B0aW9ucy51c2VCUikge1xuICAgICAgbm9kZSA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnROUygnaHR0cDovL3d3dy53My5vcmcvMTk5OS94aHRtbCcsICdkaXYnKTtcbiAgICAgIG5vZGUuaW5uZXJIVE1MID0gYmxvY2suaW5uZXJIVE1MLnJlcGxhY2UoL1xcbi9nLCAnJykucmVwbGFjZSgvPGJyWyBcXC9dKj4vZywgJ1xcbicpO1xuICAgIH0gZWxzZSB7XG4gICAgICBub2RlID0gYmxvY2s7XG4gICAgfVxuICAgIHRleHQgPSBub2RlLnRleHRDb250ZW50O1xuICAgIHJlc3VsdCA9IGxhbmd1YWdlID8gaGlnaGxpZ2h0KGxhbmd1YWdlLCB0ZXh0LCB0cnVlKSA6IGhpZ2hsaWdodEF1dG8odGV4dCk7XG5cbiAgICBvcmlnaW5hbFN0cmVhbSA9IG5vZGVTdHJlYW0obm9kZSk7XG4gICAgaWYgKG9yaWdpbmFsU3RyZWFtLmxlbmd0aCkge1xuICAgICAgcmVzdWx0Tm9kZSA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnROUygnaHR0cDovL3d3dy53My5vcmcvMTk5OS94aHRtbCcsICdkaXYnKTtcbiAgICAgIHJlc3VsdE5vZGUuaW5uZXJIVE1MID0gcmVzdWx0LnZhbHVlO1xuICAgICAgcmVzdWx0LnZhbHVlID0gbWVyZ2VTdHJlYW1zKG9yaWdpbmFsU3RyZWFtLCBub2RlU3RyZWFtKHJlc3VsdE5vZGUpLCB0ZXh0KTtcbiAgICB9XG4gICAgcmVzdWx0LnZhbHVlID0gZml4TWFya3VwKHJlc3VsdC52YWx1ZSk7XG5cbiAgICBibG9jay5pbm5lckhUTUwgPSByZXN1bHQudmFsdWU7XG4gICAgYmxvY2suY2xhc3NOYW1lID0gYnVpbGRDbGFzc05hbWUoYmxvY2suY2xhc3NOYW1lLCBsYW5ndWFnZSwgcmVzdWx0Lmxhbmd1YWdlKTtcbiAgICBibG9jay5yZXN1bHQgPSB7XG4gICAgICBsYW5ndWFnZTogcmVzdWx0Lmxhbmd1YWdlLFxuICAgICAgcmU6IHJlc3VsdC5yZWxldmFuY2VcbiAgICB9O1xuICAgIGlmIChyZXN1bHQuc2Vjb25kX2Jlc3QpIHtcbiAgICAgIGJsb2NrLnNlY29uZF9iZXN0ID0ge1xuICAgICAgICBsYW5ndWFnZTogcmVzdWx0LnNlY29uZF9iZXN0Lmxhbmd1YWdlLFxuICAgICAgICByZTogcmVzdWx0LnNlY29uZF9iZXN0LnJlbGV2YW5jZVxuICAgICAgfTtcbiAgICB9XG4gIH1cblxuICAvKlxuICBVcGRhdGVzIGhpZ2hsaWdodC5qcyBnbG9iYWwgb3B0aW9ucyB3aXRoIHZhbHVlcyBwYXNzZWQgaW4gdGhlIGZvcm0gb2YgYW4gb2JqZWN0LlxuICAqL1xuICBmdW5jdGlvbiBjb25maWd1cmUodXNlcl9vcHRpb25zKSB7XG4gICAgb3B0aW9ucyA9IGluaGVyaXQob3B0aW9ucywgdXNlcl9vcHRpb25zKTtcbiAgfVxuXG4gIC8qXG4gIEFwcGxpZXMgaGlnaGxpZ2h0aW5nIHRvIGFsbCA8cHJlPjxjb2RlPi4uPC9jb2RlPjwvcHJlPiBibG9ja3Mgb24gYSBwYWdlLlxuICAqL1xuICBmdW5jdGlvbiBpbml0SGlnaGxpZ2h0aW5nKCkge1xuICAgIGlmIChpbml0SGlnaGxpZ2h0aW5nLmNhbGxlZClcbiAgICAgIHJldHVybjtcbiAgICBpbml0SGlnaGxpZ2h0aW5nLmNhbGxlZCA9IHRydWU7XG5cbiAgICB2YXIgYmxvY2tzID0gZG9jdW1lbnQucXVlcnlTZWxlY3RvckFsbCgncHJlIGNvZGUnKTtcbiAgICBBcnJheVByb3RvLmZvckVhY2guY2FsbChibG9ja3MsIGhpZ2hsaWdodEJsb2NrKTtcbiAgfVxuXG4gIC8qXG4gIEF0dGFjaGVzIGhpZ2hsaWdodGluZyB0byB0aGUgcGFnZSBsb2FkIGV2ZW50LlxuICAqL1xuICBmdW5jdGlvbiBpbml0SGlnaGxpZ2h0aW5nT25Mb2FkKCkge1xuICAgIGFkZEV2ZW50TGlzdGVuZXIoJ0RPTUNvbnRlbnRMb2FkZWQnLCBpbml0SGlnaGxpZ2h0aW5nLCBmYWxzZSk7XG4gICAgYWRkRXZlbnRMaXN0ZW5lcignbG9hZCcsIGluaXRIaWdobGlnaHRpbmcsIGZhbHNlKTtcbiAgfVxuXG4gIGZ1bmN0aW9uIHJlZ2lzdGVyTGFuZ3VhZ2UobmFtZSwgbGFuZ3VhZ2UpIHtcbiAgICB2YXIgbGFuZyA9IGxhbmd1YWdlc1tuYW1lXSA9IGxhbmd1YWdlKGhsanMpO1xuICAgIGlmIChsYW5nLmFsaWFzZXMpIHtcbiAgICAgIGxhbmcuYWxpYXNlcy5mb3JFYWNoKGZ1bmN0aW9uKGFsaWFzKSB7YWxpYXNlc1thbGlhc10gPSBuYW1lO30pO1xuICAgIH1cbiAgfVxuXG4gIGZ1bmN0aW9uIGxpc3RMYW5ndWFnZXMoKSB7XG4gICAgcmV0dXJuIG9iamVjdEtleXMobGFuZ3VhZ2VzKTtcbiAgfVxuXG4gIGZ1bmN0aW9uIGdldExhbmd1YWdlKG5hbWUpIHtcbiAgICBuYW1lID0gKG5hbWUgfHwgJycpLnRvTG93ZXJDYXNlKCk7XG4gICAgcmV0dXJuIGxhbmd1YWdlc1tuYW1lXSB8fCBsYW5ndWFnZXNbYWxpYXNlc1tuYW1lXV07XG4gIH1cblxuICBmdW5jdGlvbiBhdXRvRGV0ZWN0aW9uKG5hbWUpIHtcbiAgICB2YXIgbGFuZyA9IGdldExhbmd1YWdlKG5hbWUpO1xuICAgIHJldHVybiBsYW5nICYmICFsYW5nLmRpc2FibGVBdXRvZGV0ZWN0O1xuICB9XG5cbiAgLyogSW50ZXJmYWNlIGRlZmluaXRpb24gKi9cblxuICBobGpzLmhpZ2hsaWdodCA9IGhpZ2hsaWdodDtcbiAgaGxqcy5oaWdobGlnaHRBdXRvID0gaGlnaGxpZ2h0QXV0bztcbiAgaGxqcy5maXhNYXJrdXAgPSBmaXhNYXJrdXA7XG4gIGhsanMuaGlnaGxpZ2h0QmxvY2sgPSBoaWdobGlnaHRCbG9jaztcbiAgaGxqcy5jb25maWd1cmUgPSBjb25maWd1cmU7XG4gIGhsanMuaW5pdEhpZ2hsaWdodGluZyA9IGluaXRIaWdobGlnaHRpbmc7XG4gIGhsanMuaW5pdEhpZ2hsaWdodGluZ09uTG9hZCA9IGluaXRIaWdobGlnaHRpbmdPbkxvYWQ7XG4gIGhsanMucmVnaXN0ZXJMYW5ndWFnZSA9IHJlZ2lzdGVyTGFuZ3VhZ2U7XG4gIGhsanMubGlzdExhbmd1YWdlcyA9IGxpc3RMYW5ndWFnZXM7XG4gIGhsanMuZ2V0TGFuZ3VhZ2UgPSBnZXRMYW5ndWFnZTtcbiAgaGxqcy5hdXRvRGV0ZWN0aW9uID0gYXV0b0RldGVjdGlvbjtcbiAgaGxqcy5pbmhlcml0ID0gaW5oZXJpdDtcblxuICAvLyBDb21tb24gcmVnZXhwc1xuICBobGpzLklERU5UX1JFID0gJ1thLXpBLVpdXFxcXHcqJztcbiAgaGxqcy5VTkRFUlNDT1JFX0lERU5UX1JFID0gJ1thLXpBLVpfXVxcXFx3Kic7XG4gIGhsanMuTlVNQkVSX1JFID0gJ1xcXFxiXFxcXGQrKFxcXFwuXFxcXGQrKT8nO1xuICBobGpzLkNfTlVNQkVSX1JFID0gJygtPykoXFxcXGIwW3hYXVthLWZBLUYwLTldK3woXFxcXGJcXFxcZCsoXFxcXC5cXFxcZCopP3xcXFxcLlxcXFxkKykoW2VFXVstK10/XFxcXGQrKT8pJzsgLy8gMHguLi4sIDAuLi4sIGRlY2ltYWwsIGZsb2F0XG4gIGhsanMuQklOQVJZX05VTUJFUl9SRSA9ICdcXFxcYigwYlswMV0rKSc7IC8vIDBiLi4uXG4gIGhsanMuUkVfU1RBUlRFUlNfUkUgPSAnIXwhPXwhPT18JXwlPXwmfCYmfCY9fFxcXFwqfFxcXFwqPXxcXFxcK3xcXFxcKz18LHwtfC09fC89fC98Onw7fDw8fDw8PXw8PXw8fD09PXw9PXw9fD4+Pj18Pj49fD49fD4+Pnw+Pnw+fFxcXFw/fFxcXFxbfFxcXFx7fFxcXFwofFxcXFxefFxcXFxePXxcXFxcfHxcXFxcfD18XFxcXHxcXFxcfHx+JztcblxuICAvLyBDb21tb24gbW9kZXNcbiAgaGxqcy5CQUNLU0xBU0hfRVNDQVBFID0ge1xuICAgIGJlZ2luOiAnXFxcXFxcXFxbXFxcXHNcXFxcU10nLCByZWxldmFuY2U6IDBcbiAgfTtcbiAgaGxqcy5BUE9TX1NUUklOR19NT0RFID0ge1xuICAgIGNsYXNzTmFtZTogJ3N0cmluZycsXG4gICAgYmVnaW46ICdcXCcnLCBlbmQ6ICdcXCcnLFxuICAgIGlsbGVnYWw6ICdcXFxcbicsXG4gICAgY29udGFpbnM6IFtobGpzLkJBQ0tTTEFTSF9FU0NBUEVdXG4gIH07XG4gIGhsanMuUVVPVEVfU1RSSU5HX01PREUgPSB7XG4gICAgY2xhc3NOYW1lOiAnc3RyaW5nJyxcbiAgICBiZWdpbjogJ1wiJywgZW5kOiAnXCInLFxuICAgIGlsbGVnYWw6ICdcXFxcbicsXG4gICAgY29udGFpbnM6IFtobGpzLkJBQ0tTTEFTSF9FU0NBUEVdXG4gIH07XG4gIGhsanMuUEhSQVNBTF9XT1JEU19NT0RFID0ge1xuICAgIGJlZ2luOiAvXFxiKGF8YW58dGhlfGFyZXxJJ218aXNuJ3R8ZG9uJ3R8ZG9lc24ndHx3b24ndHxidXR8anVzdHxzaG91bGR8cHJldHR5fHNpbXBseXxlbm91Z2h8Z29ubmF8Z29pbmd8d3RmfHNvfHN1Y2h8d2lsbHx5b3V8eW91cnx0aGV5fGxpa2V8bW9yZSlcXGIvXG4gIH07XG4gIGhsanMuQ09NTUVOVCA9IGZ1bmN0aW9uIChiZWdpbiwgZW5kLCBpbmhlcml0cykge1xuICAgIHZhciBtb2RlID0gaGxqcy5pbmhlcml0KFxuICAgICAge1xuICAgICAgICBjbGFzc05hbWU6ICdjb21tZW50JyxcbiAgICAgICAgYmVnaW46IGJlZ2luLCBlbmQ6IGVuZCxcbiAgICAgICAgY29udGFpbnM6IFtdXG4gICAgICB9LFxuICAgICAgaW5oZXJpdHMgfHwge31cbiAgICApO1xuICAgIG1vZGUuY29udGFpbnMucHVzaChobGpzLlBIUkFTQUxfV09SRFNfTU9ERSk7XG4gICAgbW9kZS5jb250YWlucy5wdXNoKHtcbiAgICAgIGNsYXNzTmFtZTogJ2RvY3RhZycsXG4gICAgICBiZWdpbjogJyg/OlRPRE98RklYTUV8Tk9URXxCVUd8WFhYKTonLFxuICAgICAgcmVsZXZhbmNlOiAwXG4gICAgfSk7XG4gICAgcmV0dXJuIG1vZGU7XG4gIH07XG4gIGhsanMuQ19MSU5FX0NPTU1FTlRfTU9ERSA9IGhsanMuQ09NTUVOVCgnLy8nLCAnJCcpO1xuICBobGpzLkNfQkxPQ0tfQ09NTUVOVF9NT0RFID0gaGxqcy5DT01NRU5UKCcvXFxcXConLCAnXFxcXCovJyk7XG4gIGhsanMuSEFTSF9DT01NRU5UX01PREUgPSBobGpzLkNPTU1FTlQoJyMnLCAnJCcpO1xuICBobGpzLk5VTUJFUl9NT0RFID0ge1xuICAgIGNsYXNzTmFtZTogJ251bWJlcicsXG4gICAgYmVnaW46IGhsanMuTlVNQkVSX1JFLFxuICAgIHJlbGV2YW5jZTogMFxuICB9O1xuICBobGpzLkNfTlVNQkVSX01PREUgPSB7XG4gICAgY2xhc3NOYW1lOiAnbnVtYmVyJyxcbiAgICBiZWdpbjogaGxqcy5DX05VTUJFUl9SRSxcbiAgICByZWxldmFuY2U6IDBcbiAgfTtcbiAgaGxqcy5CSU5BUllfTlVNQkVSX01PREUgPSB7XG4gICAgY2xhc3NOYW1lOiAnbnVtYmVyJyxcbiAgICBiZWdpbjogaGxqcy5CSU5BUllfTlVNQkVSX1JFLFxuICAgIHJlbGV2YW5jZTogMFxuICB9O1xuICBobGpzLkNTU19OVU1CRVJfTU9ERSA9IHtcbiAgICBjbGFzc05hbWU6ICdudW1iZXInLFxuICAgIGJlZ2luOiBobGpzLk5VTUJFUl9SRSArICcoJyArXG4gICAgICAnJXxlbXxleHxjaHxyZW0nICArXG4gICAgICAnfHZ3fHZofHZtaW58dm1heCcgK1xuICAgICAgJ3xjbXxtbXxpbnxwdHxwY3xweCcgK1xuICAgICAgJ3xkZWd8Z3JhZHxyYWR8dHVybicgK1xuICAgICAgJ3xzfG1zJyArXG4gICAgICAnfEh6fGtIeicgK1xuICAgICAgJ3xkcGl8ZHBjbXxkcHB4JyArXG4gICAgICAnKT8nLFxuICAgIHJlbGV2YW5jZTogMFxuICB9O1xuICBobGpzLlJFR0VYUF9NT0RFID0ge1xuICAgIGNsYXNzTmFtZTogJ3JlZ2V4cCcsXG4gICAgYmVnaW46IC9cXC8vLCBlbmQ6IC9cXC9bZ2ltdXldKi8sXG4gICAgaWxsZWdhbDogL1xcbi8sXG4gICAgY29udGFpbnM6IFtcbiAgICAgIGhsanMuQkFDS1NMQVNIX0VTQ0FQRSxcbiAgICAgIHtcbiAgICAgICAgYmVnaW46IC9cXFsvLCBlbmQ6IC9cXF0vLFxuICAgICAgICByZWxldmFuY2U6IDAsXG4gICAgICAgIGNvbnRhaW5zOiBbaGxqcy5CQUNLU0xBU0hfRVNDQVBFXVxuICAgICAgfVxuICAgIF1cbiAgfTtcbiAgaGxqcy5USVRMRV9NT0RFID0ge1xuICAgIGNsYXNzTmFtZTogJ3RpdGxlJyxcbiAgICBiZWdpbjogaGxqcy5JREVOVF9SRSxcbiAgICByZWxldmFuY2U6IDBcbiAgfTtcbiAgaGxqcy5VTkRFUlNDT1JFX1RJVExFX01PREUgPSB7XG4gICAgY2xhc3NOYW1lOiAndGl0bGUnLFxuICAgIGJlZ2luOiBobGpzLlVOREVSU0NPUkVfSURFTlRfUkUsXG4gICAgcmVsZXZhbmNlOiAwXG4gIH07XG4gIGhsanMuTUVUSE9EX0dVQVJEID0ge1xuICAgIC8vIGV4Y2x1ZGVzIG1ldGhvZCBuYW1lcyBmcm9tIGtleXdvcmQgcHJvY2Vzc2luZ1xuICAgIGJlZ2luOiAnXFxcXC5cXFxccyonICsgaGxqcy5VTkRFUlNDT1JFX0lERU5UX1JFLFxuICAgIHJlbGV2YW5jZTogMFxuICB9O1xuXG4gIHJldHVybiBobGpzO1xufSkpO1xuIiwibW9kdWxlLmV4cG9ydHMgPSBmdW5jdGlvbihobGpzKSB7XG4gIHZhciBLRVlXT1JEUyA9IHtcbiAgICBrZXl3b3JkOlxuICAgICAgLy8gTm9ybWFsIGtleXdvcmRzLlxuICAgICAgJ2Fic3RyYWN0IGFzIGJhc2UgYm9vbCBicmVhayBieXRlIGNhc2UgY2F0Y2ggY2hhciBjaGVja2VkIGNvbnN0IGNvbnRpbnVlIGRlY2ltYWwgJyArXG4gICAgICAnZGVmYXVsdCBkZWxlZ2F0ZSBkbyBkb3VibGUgZW51bSBldmVudCBleHBsaWNpdCBleHRlcm4gZmluYWxseSBmaXhlZCBmbG9hdCAnICtcbiAgICAgICdmb3IgZm9yZWFjaCBnb3RvIGlmIGltcGxpY2l0IGluIGludCBpbnRlcmZhY2UgaW50ZXJuYWwgaXMgbG9jayBsb25nIG5hbWVvZiAnICtcbiAgICAgICdvYmplY3Qgb3BlcmF0b3Igb3V0IG92ZXJyaWRlIHBhcmFtcyBwcml2YXRlIHByb3RlY3RlZCBwdWJsaWMgcmVhZG9ubHkgcmVmIHNieXRlICcgK1xuICAgICAgJ3NlYWxlZCBzaG9ydCBzaXplb2Ygc3RhY2thbGxvYyBzdGF0aWMgc3RyaW5nIHN0cnVjdCBzd2l0Y2ggdGhpcyB0cnkgdHlwZW9mICcgK1xuICAgICAgJ3VpbnQgdWxvbmcgdW5jaGVja2VkIHVuc2FmZSB1c2hvcnQgdXNpbmcgdmlydHVhbCB2b2lkIHZvbGF0aWxlIHdoaWxlICcgK1xuICAgICAgLy8gQ29udGV4dHVhbCBrZXl3b3Jkcy5cbiAgICAgICdhZGQgYWxpYXMgYXNjZW5kaW5nIGFzeW5jIGF3YWl0IGJ5IGRlc2NlbmRpbmcgZHluYW1pYyBlcXVhbHMgZnJvbSBnZXQgZ2xvYmFsIGdyb3VwIGludG8gam9pbiAnICtcbiAgICAgICdsZXQgb24gb3JkZXJieSBwYXJ0aWFsIHJlbW92ZSBzZWxlY3Qgc2V0IHZhbHVlIHZhciB3aGVyZSB5aWVsZCcsXG4gICAgbGl0ZXJhbDpcbiAgICAgICdudWxsIGZhbHNlIHRydWUnXG4gIH07XG4gIHZhciBOVU1CRVJTID0ge1xuICAgIGNsYXNzTmFtZTogJ251bWJlcicsXG4gICAgdmFyaWFudHM6IFtcbiAgICAgIHsgYmVnaW46ICdcXFxcYigwYlswMVxcJ10rKScgfSxcbiAgICAgIHsgYmVnaW46ICcoLT8pXFxcXGIoW1xcXFxkXFwnXSsoXFxcXC5bXFxcXGRcXCddKik/fFxcXFwuW1xcXFxkXFwnXSspKHV8VXxsfEx8dWx8VUx8ZnxGfGJ8QiknIH0sXG4gICAgICB7IGJlZ2luOiAnKC0/KShcXFxcYjBbeFhdW2EtZkEtRjAtOVxcJ10rfChcXFxcYltcXFxcZFxcJ10rKFxcXFwuW1xcXFxkXFwnXSopP3xcXFxcLltcXFxcZFxcJ10rKShbZUVdWy0rXT9bXFxcXGRcXCddKyk/KScgfVxuICAgIF0sXG4gICAgcmVsZXZhbmNlOiAwXG4gIH07XG4gIHZhciBWRVJCQVRJTV9TVFJJTkcgPSB7XG4gICAgY2xhc3NOYW1lOiAnc3RyaW5nJyxcbiAgICBiZWdpbjogJ0BcIicsIGVuZDogJ1wiJyxcbiAgICBjb250YWluczogW3tiZWdpbjogJ1wiXCInfV1cbiAgfTtcbiAgdmFyIFZFUkJBVElNX1NUUklOR19OT19MRiA9IGhsanMuaW5oZXJpdChWRVJCQVRJTV9TVFJJTkcsIHtpbGxlZ2FsOiAvXFxuL30pO1xuICB2YXIgU1VCU1QgPSB7XG4gICAgY2xhc3NOYW1lOiAnc3Vic3QnLFxuICAgIGJlZ2luOiAneycsIGVuZDogJ30nLFxuICAgIGtleXdvcmRzOiBLRVlXT1JEU1xuICB9O1xuICB2YXIgU1VCU1RfTk9fTEYgPSBobGpzLmluaGVyaXQoU1VCU1QsIHtpbGxlZ2FsOiAvXFxuL30pO1xuICB2YXIgSU5URVJQT0xBVEVEX1NUUklORyA9IHtcbiAgICBjbGFzc05hbWU6ICdzdHJpbmcnLFxuICAgIGJlZ2luOiAvXFwkXCIvLCBlbmQ6ICdcIicsXG4gICAgaWxsZWdhbDogL1xcbi8sXG4gICAgY29udGFpbnM6IFt7YmVnaW46ICd7eyd9LCB7YmVnaW46ICd9fSd9LCBobGpzLkJBQ0tTTEFTSF9FU0NBUEUsIFNVQlNUX05PX0xGXVxuICB9O1xuICB2YXIgSU5URVJQT0xBVEVEX1ZFUkJBVElNX1NUUklORyA9IHtcbiAgICBjbGFzc05hbWU6ICdzdHJpbmcnLFxuICAgIGJlZ2luOiAvXFwkQFwiLywgZW5kOiAnXCInLFxuICAgIGNvbnRhaW5zOiBbe2JlZ2luOiAne3snfSwge2JlZ2luOiAnfX0nfSwge2JlZ2luOiAnXCJcIid9LCBTVUJTVF1cbiAgfTtcbiAgdmFyIElOVEVSUE9MQVRFRF9WRVJCQVRJTV9TVFJJTkdfTk9fTEYgPSBobGpzLmluaGVyaXQoSU5URVJQT0xBVEVEX1ZFUkJBVElNX1NUUklORywge1xuICAgIGlsbGVnYWw6IC9cXG4vLFxuICAgIGNvbnRhaW5zOiBbe2JlZ2luOiAne3snfSwge2JlZ2luOiAnfX0nfSwge2JlZ2luOiAnXCJcIid9LCBTVUJTVF9OT19MRl1cbiAgfSk7XG4gIFNVQlNULmNvbnRhaW5zID0gW1xuICAgIElOVEVSUE9MQVRFRF9WRVJCQVRJTV9TVFJJTkcsXG4gICAgSU5URVJQT0xBVEVEX1NUUklORyxcbiAgICBWRVJCQVRJTV9TVFJJTkcsXG4gICAgaGxqcy5BUE9TX1NUUklOR19NT0RFLFxuICAgIGhsanMuUVVPVEVfU1RSSU5HX01PREUsXG4gICAgTlVNQkVSUyxcbiAgICBobGpzLkNfQkxPQ0tfQ09NTUVOVF9NT0RFXG4gIF07XG4gIFNVQlNUX05PX0xGLmNvbnRhaW5zID0gW1xuICAgIElOVEVSUE9MQVRFRF9WRVJCQVRJTV9TVFJJTkdfTk9fTEYsXG4gICAgSU5URVJQT0xBVEVEX1NUUklORyxcbiAgICBWRVJCQVRJTV9TVFJJTkdfTk9fTEYsXG4gICAgaGxqcy5BUE9TX1NUUklOR19NT0RFLFxuICAgIGhsanMuUVVPVEVfU1RSSU5HX01PREUsXG4gICAgTlVNQkVSUyxcbiAgICBobGpzLmluaGVyaXQoaGxqcy5DX0JMT0NLX0NPTU1FTlRfTU9ERSwge2lsbGVnYWw6IC9cXG4vfSlcbiAgXTtcbiAgdmFyIFNUUklORyA9IHtcbiAgICB2YXJpYW50czogW1xuICAgICAgSU5URVJQT0xBVEVEX1ZFUkJBVElNX1NUUklORyxcbiAgICAgIElOVEVSUE9MQVRFRF9TVFJJTkcsXG4gICAgICBWRVJCQVRJTV9TVFJJTkcsXG4gICAgICBobGpzLkFQT1NfU1RSSU5HX01PREUsXG4gICAgICBobGpzLlFVT1RFX1NUUklOR19NT0RFXG4gICAgXVxuICB9O1xuXG4gIHZhciBUWVBFX0lERU5UX1JFID0gaGxqcy5JREVOVF9SRSArICcoPCcgKyBobGpzLklERU5UX1JFICsgJyhcXFxccyosXFxcXHMqJyArIGhsanMuSURFTlRfUkUgKyAnKSo+KT8oXFxcXFtcXFxcXSk/JztcblxuICByZXR1cm4ge1xuICAgIGFsaWFzZXM6IFsnY3NoYXJwJywgJ2MjJ10sXG4gICAga2V5d29yZHM6IEtFWVdPUkRTLFxuICAgIGlsbGVnYWw6IC86Oi8sXG4gICAgY29udGFpbnM6IFtcbiAgICAgIGhsanMuQ09NTUVOVChcbiAgICAgICAgJy8vLycsXG4gICAgICAgICckJyxcbiAgICAgICAge1xuICAgICAgICAgIHJldHVybkJlZ2luOiB0cnVlLFxuICAgICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAgICB7XG4gICAgICAgICAgICAgIGNsYXNzTmFtZTogJ2RvY3RhZycsXG4gICAgICAgICAgICAgIHZhcmlhbnRzOiBbXG4gICAgICAgICAgICAgICAge1xuICAgICAgICAgICAgICAgICAgYmVnaW46ICcvLy8nLCByZWxldmFuY2U6IDBcbiAgICAgICAgICAgICAgICB9LFxuICAgICAgICAgICAgICAgIHtcbiAgICAgICAgICAgICAgICAgIGJlZ2luOiAnPCEtLXwtLT4nXG4gICAgICAgICAgICAgICAgfSxcbiAgICAgICAgICAgICAgICB7XG4gICAgICAgICAgICAgICAgICBiZWdpbjogJzwvPycsIGVuZDogJz4nXG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICBdXG4gICAgICAgICAgICB9XG4gICAgICAgICAgXVxuICAgICAgICB9XG4gICAgICApLFxuICAgICAgaGxqcy5DX0xJTkVfQ09NTUVOVF9NT0RFLFxuICAgICAgaGxqcy5DX0JMT0NLX0NPTU1FTlRfTU9ERSxcbiAgICAgIHtcbiAgICAgICAgY2xhc3NOYW1lOiAnbWV0YScsXG4gICAgICAgIGJlZ2luOiAnIycsIGVuZDogJyQnLFxuICAgICAgICBrZXl3b3Jkczoge1xuICAgICAgICAgICdtZXRhLWtleXdvcmQnOiAnaWYgZWxzZSBlbGlmIGVuZGlmIGRlZmluZSB1bmRlZiB3YXJuaW5nIGVycm9yIGxpbmUgcmVnaW9uIGVuZHJlZ2lvbiBwcmFnbWEgY2hlY2tzdW0nXG4gICAgICAgIH1cbiAgICAgIH0sXG4gICAgICBTVFJJTkcsXG4gICAgICBOVU1CRVJTLFxuICAgICAge1xuICAgICAgICBiZWdpbktleXdvcmRzOiAnY2xhc3MgaW50ZXJmYWNlJywgZW5kOiAvW3s7PV0vLFxuICAgICAgICBpbGxlZ2FsOiAvW15cXHM6LF0vLFxuICAgICAgICBjb250YWluczogW1xuICAgICAgICAgIGhsanMuVElUTEVfTU9ERSxcbiAgICAgICAgICBobGpzLkNfTElORV9DT01NRU5UX01PREUsXG4gICAgICAgICAgaGxqcy5DX0JMT0NLX0NPTU1FTlRfTU9ERVxuICAgICAgICBdXG4gICAgICB9LFxuICAgICAge1xuICAgICAgICBiZWdpbktleXdvcmRzOiAnbmFtZXNwYWNlJywgZW5kOiAvW3s7PV0vLFxuICAgICAgICBpbGxlZ2FsOiAvW15cXHM6XS8sXG4gICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAgaGxqcy5pbmhlcml0KGhsanMuVElUTEVfTU9ERSwge2JlZ2luOiAnW2EtekEtWl0oXFxcXC4/XFxcXHcpKid9KSxcbiAgICAgICAgICBobGpzLkNfTElORV9DT01NRU5UX01PREUsXG4gICAgICAgICAgaGxqcy5DX0JMT0NLX0NPTU1FTlRfTU9ERVxuICAgICAgICBdXG4gICAgICB9LFxuICAgICAge1xuICAgICAgICAvLyBbQXR0cmlidXRlcyhcIlwiKV1cbiAgICAgICAgY2xhc3NOYW1lOiAnbWV0YScsXG4gICAgICAgIGJlZ2luOiAnXlxcXFxzKlxcXFxbJywgZXhjbHVkZUJlZ2luOiB0cnVlLCBlbmQ6ICdcXFxcXScsIGV4Y2x1ZGVFbmQ6IHRydWUsXG4gICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAge2NsYXNzTmFtZTogJ21ldGEtc3RyaW5nJywgYmVnaW46IC9cIi8sIGVuZDogL1wiL31cbiAgICAgICAgXVxuICAgICAgfSxcbiAgICAgIHtcbiAgICAgICAgLy8gRXhwcmVzc2lvbiBrZXl3b3JkcyBwcmV2ZW50ICdrZXl3b3JkIE5hbWUoLi4uKScgZnJvbSBiZWluZ1xuICAgICAgICAvLyByZWNvZ25pemVkIGFzIGEgZnVuY3Rpb24gZGVmaW5pdGlvblxuICAgICAgICBiZWdpbktleXdvcmRzOiAnbmV3IHJldHVybiB0aHJvdyBhd2FpdCBlbHNlJyxcbiAgICAgICAgcmVsZXZhbmNlOiAwXG4gICAgICB9LFxuICAgICAge1xuICAgICAgICBjbGFzc05hbWU6ICdmdW5jdGlvbicsXG4gICAgICAgIGJlZ2luOiAnKCcgKyBUWVBFX0lERU5UX1JFICsgJ1xcXFxzKykrJyArIGhsanMuSURFTlRfUkUgKyAnXFxcXHMqXFxcXCgnLCByZXR1cm5CZWdpbjogdHJ1ZSxcbiAgICAgICAgZW5kOiAvXFxzKlt7Oz1dLywgZXhjbHVkZUVuZDogdHJ1ZSxcbiAgICAgICAga2V5d29yZHM6IEtFWVdPUkRTLFxuICAgICAgICBjb250YWluczogW1xuICAgICAgICAgIHtcbiAgICAgICAgICAgIGJlZ2luOiBobGpzLklERU5UX1JFICsgJ1xcXFxzKlxcXFwoJywgcmV0dXJuQmVnaW46IHRydWUsXG4gICAgICAgICAgICBjb250YWluczogW2hsanMuVElUTEVfTU9ERV0sXG4gICAgICAgICAgICByZWxldmFuY2U6IDBcbiAgICAgICAgICB9LFxuICAgICAgICAgIHtcbiAgICAgICAgICAgIGNsYXNzTmFtZTogJ3BhcmFtcycsXG4gICAgICAgICAgICBiZWdpbjogL1xcKC8sIGVuZDogL1xcKS8sXG4gICAgICAgICAgICBleGNsdWRlQmVnaW46IHRydWUsXG4gICAgICAgICAgICBleGNsdWRlRW5kOiB0cnVlLFxuICAgICAgICAgICAga2V5d29yZHM6IEtFWVdPUkRTLFxuICAgICAgICAgICAgcmVsZXZhbmNlOiAwLFxuICAgICAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICAgICAgU1RSSU5HLFxuICAgICAgICAgICAgICBOVU1CRVJTLFxuICAgICAgICAgICAgICBobGpzLkNfQkxPQ0tfQ09NTUVOVF9NT0RFXG4gICAgICAgICAgICBdXG4gICAgICAgICAgfSxcbiAgICAgICAgICBobGpzLkNfTElORV9DT01NRU5UX01PREUsXG4gICAgICAgICAgaGxqcy5DX0JMT0NLX0NPTU1FTlRfTU9ERVxuICAgICAgICBdXG4gICAgICB9XG4gICAgXVxuICB9O1xufTsiLCJtb2R1bGUuZXhwb3J0cyA9IGZ1bmN0aW9uKGhsanMpIHtcbiAgdmFyIEpBVkFfSURFTlRfUkUgPSAnW1xcdTAwQzAtXFx1MDJCOGEtekEtWl8kXVtcXHUwMEMwLVxcdTAyQjhhLXpBLVpfJDAtOV0qJztcbiAgdmFyIEdFTkVSSUNfSURFTlRfUkUgPSBKQVZBX0lERU5UX1JFICsgJyg8JyArIEpBVkFfSURFTlRfUkUgKyAnKFxcXFxzKixcXFxccyonICsgSkFWQV9JREVOVF9SRSArICcpKj4pPyc7XG4gIHZhciBLRVlXT1JEUyA9XG4gICAgJ2ZhbHNlIHN5bmNocm9uaXplZCBpbnQgYWJzdHJhY3QgZmxvYXQgcHJpdmF0ZSBjaGFyIGJvb2xlYW4gdmFyIHN0YXRpYyBudWxsIGlmIGNvbnN0ICcgK1xuICAgICdmb3IgdHJ1ZSB3aGlsZSBsb25nIHN0cmljdGZwIGZpbmFsbHkgcHJvdGVjdGVkIGltcG9ydCBuYXRpdmUgZmluYWwgdm9pZCAnICtcbiAgICAnZW51bSBlbHNlIGJyZWFrIHRyYW5zaWVudCBjYXRjaCBpbnN0YW5jZW9mIGJ5dGUgc3VwZXIgdm9sYXRpbGUgY2FzZSBhc3NlcnQgc2hvcnQgJyArXG4gICAgJ3BhY2thZ2UgZGVmYXVsdCBkb3VibGUgcHVibGljIHRyeSB0aGlzIHN3aXRjaCBjb250aW51ZSB0aHJvd3MgcHJvdGVjdGVkIHB1YmxpYyBwcml2YXRlICcgK1xuICAgICdtb2R1bGUgcmVxdWlyZXMgZXhwb3J0cyBkbyc7XG5cbiAgLy8gaHR0cHM6Ly9kb2NzLm9yYWNsZS5jb20vamF2YXNlLzcvZG9jcy90ZWNobm90ZXMvZ3VpZGVzL2xhbmd1YWdlL3VuZGVyc2NvcmVzLWxpdGVyYWxzLmh0bWxcbiAgdmFyIEpBVkFfTlVNQkVSX1JFID0gJ1xcXFxiJyArXG4gICAgJygnICtcbiAgICAgICcwW2JCXShbMDFdK1swMV9dK1swMV0rfFswMV0rKScgKyAvLyAwYi4uLlxuICAgICAgJ3wnICtcbiAgICAgICcwW3hYXShbYS1mQS1GMC05XStbYS1mQS1GMC05X10rW2EtZkEtRjAtOV0rfFthLWZBLUYwLTldKyknICsgLy8gMHguLi5cbiAgICAgICd8JyArXG4gICAgICAnKCcgK1xuICAgICAgICAnKFtcXFxcZF0rW1xcXFxkX10rW1xcXFxkXSt8W1xcXFxkXSspKFxcXFwuKFtcXFxcZF0rW1xcXFxkX10rW1xcXFxkXSt8W1xcXFxkXSspKT8nICtcbiAgICAgICAgJ3wnICtcbiAgICAgICAgJ1xcXFwuKFtcXFxcZF0rW1xcXFxkX10rW1xcXFxkXSt8W1xcXFxkXSspJyArXG4gICAgICAnKScgK1xuICAgICAgJyhbZUVdWy0rXT9cXFxcZCspPycgKyAvLyBvY3RhbCwgZGVjaW1hbCwgZmxvYXRcbiAgICAnKScgK1xuICAgICdbbExmRl0/JztcbiAgdmFyIEpBVkFfTlVNQkVSX01PREUgPSB7XG4gICAgY2xhc3NOYW1lOiAnbnVtYmVyJyxcbiAgICBiZWdpbjogSkFWQV9OVU1CRVJfUkUsXG4gICAgcmVsZXZhbmNlOiAwXG4gIH07XG5cbiAgcmV0dXJuIHtcbiAgICBhbGlhc2VzOiBbJ2pzcCddLFxuICAgIGtleXdvcmRzOiBLRVlXT1JEUyxcbiAgICBpbGxlZ2FsOiAvPFxcL3wjLyxcbiAgICBjb250YWluczogW1xuICAgICAgaGxqcy5DT01NRU5UKFxuICAgICAgICAnL1xcXFwqXFxcXConLFxuICAgICAgICAnXFxcXCovJyxcbiAgICAgICAge1xuICAgICAgICAgIHJlbGV2YW5jZSA6IDAsXG4gICAgICAgICAgY29udGFpbnMgOiBbXG4gICAgICAgICAgICB7XG4gICAgICAgICAgICAgIC8vIGVhdCB1cCBAJ3MgaW4gZW1haWxzIHRvIHByZXZlbnQgdGhlbSB0byBiZSByZWNvZ25pemVkIGFzIGRvY3RhZ3NcbiAgICAgICAgICAgICAgYmVnaW46IC9cXHcrQC8sIHJlbGV2YW5jZTogMFxuICAgICAgICAgICAgfSxcbiAgICAgICAgICAgIHtcbiAgICAgICAgICAgICAgY2xhc3NOYW1lIDogJ2RvY3RhZycsXG4gICAgICAgICAgICAgIGJlZ2luIDogJ0BbQS1aYS16XSsnXG4gICAgICAgICAgICB9XG4gICAgICAgICAgXVxuICAgICAgICB9XG4gICAgICApLFxuICAgICAgaGxqcy5DX0xJTkVfQ09NTUVOVF9NT0RFLFxuICAgICAgaGxqcy5DX0JMT0NLX0NPTU1FTlRfTU9ERSxcbiAgICAgIGhsanMuQVBPU19TVFJJTkdfTU9ERSxcbiAgICAgIGhsanMuUVVPVEVfU1RSSU5HX01PREUsXG4gICAgICB7XG4gICAgICAgIGNsYXNzTmFtZTogJ2NsYXNzJyxcbiAgICAgICAgYmVnaW5LZXl3b3JkczogJ2NsYXNzIGludGVyZmFjZScsIGVuZDogL1t7Oz1dLywgZXhjbHVkZUVuZDogdHJ1ZSxcbiAgICAgICAga2V5d29yZHM6ICdjbGFzcyBpbnRlcmZhY2UnLFxuICAgICAgICBpbGxlZ2FsOiAvWzpcIlxcW1xcXV0vLFxuICAgICAgICBjb250YWluczogW1xuICAgICAgICAgIHtiZWdpbktleXdvcmRzOiAnZXh0ZW5kcyBpbXBsZW1lbnRzJ30sXG4gICAgICAgICAgaGxqcy5VTkRFUlNDT1JFX1RJVExFX01PREVcbiAgICAgICAgXVxuICAgICAgfSxcbiAgICAgIHtcbiAgICAgICAgLy8gRXhwcmVzc2lvbiBrZXl3b3JkcyBwcmV2ZW50ICdrZXl3b3JkIE5hbWUoLi4uKScgZnJvbSBiZWluZ1xuICAgICAgICAvLyByZWNvZ25pemVkIGFzIGEgZnVuY3Rpb24gZGVmaW5pdGlvblxuICAgICAgICBiZWdpbktleXdvcmRzOiAnbmV3IHRocm93IHJldHVybiBlbHNlJyxcbiAgICAgICAgcmVsZXZhbmNlOiAwXG4gICAgICB9LFxuICAgICAge1xuICAgICAgICBjbGFzc05hbWU6ICdmdW5jdGlvbicsXG4gICAgICAgIGJlZ2luOiAnKCcgKyBHRU5FUklDX0lERU5UX1JFICsgJ1xcXFxzKykrJyArIGhsanMuVU5ERVJTQ09SRV9JREVOVF9SRSArICdcXFxccypcXFxcKCcsIHJldHVybkJlZ2luOiB0cnVlLCBlbmQ6IC9bezs9XS8sXG4gICAgICAgIGV4Y2x1ZGVFbmQ6IHRydWUsXG4gICAgICAgIGtleXdvcmRzOiBLRVlXT1JEUyxcbiAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICB7XG4gICAgICAgICAgICBiZWdpbjogaGxqcy5VTkRFUlNDT1JFX0lERU5UX1JFICsgJ1xcXFxzKlxcXFwoJywgcmV0dXJuQmVnaW46IHRydWUsXG4gICAgICAgICAgICByZWxldmFuY2U6IDAsXG4gICAgICAgICAgICBjb250YWluczogW2hsanMuVU5ERVJTQ09SRV9USVRMRV9NT0RFXVxuICAgICAgICAgIH0sXG4gICAgICAgICAge1xuICAgICAgICAgICAgY2xhc3NOYW1lOiAncGFyYW1zJyxcbiAgICAgICAgICAgIGJlZ2luOiAvXFwoLywgZW5kOiAvXFwpLyxcbiAgICAgICAgICAgIGtleXdvcmRzOiBLRVlXT1JEUyxcbiAgICAgICAgICAgIHJlbGV2YW5jZTogMCxcbiAgICAgICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAgICAgIGhsanMuQVBPU19TVFJJTkdfTU9ERSxcbiAgICAgICAgICAgICAgaGxqcy5RVU9URV9TVFJJTkdfTU9ERSxcbiAgICAgICAgICAgICAgaGxqcy5DX05VTUJFUl9NT0RFLFxuICAgICAgICAgICAgICBobGpzLkNfQkxPQ0tfQ09NTUVOVF9NT0RFXG4gICAgICAgICAgICBdXG4gICAgICAgICAgfSxcbiAgICAgICAgICBobGpzLkNfTElORV9DT01NRU5UX01PREUsXG4gICAgICAgICAgaGxqcy5DX0JMT0NLX0NPTU1FTlRfTU9ERVxuICAgICAgICBdXG4gICAgICB9LFxuICAgICAgSkFWQV9OVU1CRVJfTU9ERSxcbiAgICAgIHtcbiAgICAgICAgY2xhc3NOYW1lOiAnbWV0YScsIGJlZ2luOiAnQFtBLVphLXpdKydcbiAgICAgIH1cbiAgICBdXG4gIH07XG59OyIsIm1vZHVsZS5leHBvcnRzID0gZnVuY3Rpb24oaGxqcykge1xuICB2YXIgSURFTlRfUkUgPSAnW0EtWmEteiRfXVswLTlBLVphLXokX10qJztcbiAgdmFyIEtFWVdPUkRTID0ge1xuICAgIGtleXdvcmQ6XG4gICAgICAnaW4gb2YgaWYgZm9yIHdoaWxlIGZpbmFsbHkgdmFyIG5ldyBmdW5jdGlvbiBkbyByZXR1cm4gdm9pZCBlbHNlIGJyZWFrIGNhdGNoICcgK1xuICAgICAgJ2luc3RhbmNlb2Ygd2l0aCB0aHJvdyBjYXNlIGRlZmF1bHQgdHJ5IHRoaXMgc3dpdGNoIGNvbnRpbnVlIHR5cGVvZiBkZWxldGUgJyArXG4gICAgICAnbGV0IHlpZWxkIGNvbnN0IGV4cG9ydCBzdXBlciBkZWJ1Z2dlciBhcyBhc3luYyBhd2FpdCBzdGF0aWMgJyArXG4gICAgICAvLyBFQ01BU2NyaXB0IDYgbW9kdWxlcyBpbXBvcnRcbiAgICAgICdpbXBvcnQgZnJvbSBhcydcbiAgICAsXG4gICAgbGl0ZXJhbDpcbiAgICAgICd0cnVlIGZhbHNlIG51bGwgdW5kZWZpbmVkIE5hTiBJbmZpbml0eScsXG4gICAgYnVpbHRfaW46XG4gICAgICAnZXZhbCBpc0Zpbml0ZSBpc05hTiBwYXJzZUZsb2F0IHBhcnNlSW50IGRlY29kZVVSSSBkZWNvZGVVUklDb21wb25lbnQgJyArXG4gICAgICAnZW5jb2RlVVJJIGVuY29kZVVSSUNvbXBvbmVudCBlc2NhcGUgdW5lc2NhcGUgT2JqZWN0IEZ1bmN0aW9uIEJvb2xlYW4gRXJyb3IgJyArXG4gICAgICAnRXZhbEVycm9yIEludGVybmFsRXJyb3IgUmFuZ2VFcnJvciBSZWZlcmVuY2VFcnJvciBTdG9wSXRlcmF0aW9uIFN5bnRheEVycm9yICcgK1xuICAgICAgJ1R5cGVFcnJvciBVUklFcnJvciBOdW1iZXIgTWF0aCBEYXRlIFN0cmluZyBSZWdFeHAgQXJyYXkgRmxvYXQzMkFycmF5ICcgK1xuICAgICAgJ0Zsb2F0NjRBcnJheSBJbnQxNkFycmF5IEludDMyQXJyYXkgSW50OEFycmF5IFVpbnQxNkFycmF5IFVpbnQzMkFycmF5ICcgK1xuICAgICAgJ1VpbnQ4QXJyYXkgVWludDhDbGFtcGVkQXJyYXkgQXJyYXlCdWZmZXIgRGF0YVZpZXcgSlNPTiBJbnRsIGFyZ3VtZW50cyByZXF1aXJlICcgK1xuICAgICAgJ21vZHVsZSBjb25zb2xlIHdpbmRvdyBkb2N1bWVudCBTeW1ib2wgU2V0IE1hcCBXZWFrU2V0IFdlYWtNYXAgUHJveHkgUmVmbGVjdCAnICtcbiAgICAgICdQcm9taXNlJ1xuICB9O1xuICB2YXIgTlVNQkVSID0ge1xuICAgIGNsYXNzTmFtZTogJ251bWJlcicsXG4gICAgdmFyaWFudHM6IFtcbiAgICAgIHsgYmVnaW46ICdcXFxcYigwW2JCXVswMV0rKScgfSxcbiAgICAgIHsgYmVnaW46ICdcXFxcYigwW29PXVswLTddKyknIH0sXG4gICAgICB7IGJlZ2luOiBobGpzLkNfTlVNQkVSX1JFIH1cbiAgICBdLFxuICAgIHJlbGV2YW5jZTogMFxuICB9O1xuICB2YXIgU1VCU1QgPSB7XG4gICAgY2xhc3NOYW1lOiAnc3Vic3QnLFxuICAgIGJlZ2luOiAnXFxcXCRcXFxceycsIGVuZDogJ1xcXFx9JyxcbiAgICBrZXl3b3JkczogS0VZV09SRFMsXG4gICAgY29udGFpbnM6IFtdICAvLyBkZWZpbmVkIGxhdGVyXG4gIH07XG4gIHZhciBURU1QTEFURV9TVFJJTkcgPSB7XG4gICAgY2xhc3NOYW1lOiAnc3RyaW5nJyxcbiAgICBiZWdpbjogJ2AnLCBlbmQ6ICdgJyxcbiAgICBjb250YWluczogW1xuICAgICAgaGxqcy5CQUNLU0xBU0hfRVNDQVBFLFxuICAgICAgU1VCU1RcbiAgICBdXG4gIH07XG4gIFNVQlNULmNvbnRhaW5zID0gW1xuICAgIGhsanMuQVBPU19TVFJJTkdfTU9ERSxcbiAgICBobGpzLlFVT1RFX1NUUklOR19NT0RFLFxuICAgIFRFTVBMQVRFX1NUUklORyxcbiAgICBOVU1CRVIsXG4gICAgaGxqcy5SRUdFWFBfTU9ERVxuICBdXG4gIHZhciBQQVJBTVNfQ09OVEFJTlMgPSBTVUJTVC5jb250YWlucy5jb25jYXQoW1xuICAgIGhsanMuQ19CTE9DS19DT01NRU5UX01PREUsXG4gICAgaGxqcy5DX0xJTkVfQ09NTUVOVF9NT0RFXG4gIF0pO1xuXG4gIHJldHVybiB7XG4gICAgYWxpYXNlczogWydqcycsICdqc3gnXSxcbiAgICBrZXl3b3JkczogS0VZV09SRFMsXG4gICAgY29udGFpbnM6IFtcbiAgICAgIHtcbiAgICAgICAgY2xhc3NOYW1lOiAnbWV0YScsXG4gICAgICAgIHJlbGV2YW5jZTogMTAsXG4gICAgICAgIGJlZ2luOiAvXlxccypbJ1wiXXVzZSAoc3RyaWN0fGFzbSlbJ1wiXS9cbiAgICAgIH0sXG4gICAgICB7XG4gICAgICAgIGNsYXNzTmFtZTogJ21ldGEnLFxuICAgICAgICBiZWdpbjogL14jIS8sIGVuZDogLyQvXG4gICAgICB9LFxuICAgICAgaGxqcy5BUE9TX1NUUklOR19NT0RFLFxuICAgICAgaGxqcy5RVU9URV9TVFJJTkdfTU9ERSxcbiAgICAgIFRFTVBMQVRFX1NUUklORyxcbiAgICAgIGhsanMuQ19MSU5FX0NPTU1FTlRfTU9ERSxcbiAgICAgIGhsanMuQ19CTE9DS19DT01NRU5UX01PREUsXG4gICAgICBOVU1CRVIsXG4gICAgICB7IC8vIG9iamVjdCBhdHRyIGNvbnRhaW5lclxuICAgICAgICBiZWdpbjogL1t7LF1cXHMqLywgcmVsZXZhbmNlOiAwLFxuICAgICAgICBjb250YWluczogW1xuICAgICAgICAgIHtcbiAgICAgICAgICAgIGJlZ2luOiBJREVOVF9SRSArICdcXFxccyo6JywgcmV0dXJuQmVnaW46IHRydWUsXG4gICAgICAgICAgICByZWxldmFuY2U6IDAsXG4gICAgICAgICAgICBjb250YWluczogW3tjbGFzc05hbWU6ICdhdHRyJywgYmVnaW46IElERU5UX1JFLCByZWxldmFuY2U6IDB9XVxuICAgICAgICAgIH1cbiAgICAgICAgXVxuICAgICAgfSxcbiAgICAgIHsgLy8gXCJ2YWx1ZVwiIGNvbnRhaW5lclxuICAgICAgICBiZWdpbjogJygnICsgaGxqcy5SRV9TVEFSVEVSU19SRSArICd8XFxcXGIoY2FzZXxyZXR1cm58dGhyb3cpXFxcXGIpXFxcXHMqJyxcbiAgICAgICAga2V5d29yZHM6ICdyZXR1cm4gdGhyb3cgY2FzZScsXG4gICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAgaGxqcy5DX0xJTkVfQ09NTUVOVF9NT0RFLFxuICAgICAgICAgIGhsanMuQ19CTE9DS19DT01NRU5UX01PREUsXG4gICAgICAgICAgaGxqcy5SRUdFWFBfTU9ERSxcbiAgICAgICAgICB7XG4gICAgICAgICAgICBjbGFzc05hbWU6ICdmdW5jdGlvbicsXG4gICAgICAgICAgICBiZWdpbjogJyhcXFxcKC4qP1xcXFwpfCcgKyBJREVOVF9SRSArICcpXFxcXHMqPT4nLCByZXR1cm5CZWdpbjogdHJ1ZSxcbiAgICAgICAgICAgIGVuZDogJ1xcXFxzKj0+JyxcbiAgICAgICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAgICAgIHtcbiAgICAgICAgICAgICAgICBjbGFzc05hbWU6ICdwYXJhbXMnLFxuICAgICAgICAgICAgICAgIHZhcmlhbnRzOiBbXG4gICAgICAgICAgICAgICAgICB7XG4gICAgICAgICAgICAgICAgICAgIGJlZ2luOiBJREVOVF9SRVxuICAgICAgICAgICAgICAgICAgfSxcbiAgICAgICAgICAgICAgICAgIHtcbiAgICAgICAgICAgICAgICAgICAgYmVnaW46IC9cXChcXHMqXFwpLyxcbiAgICAgICAgICAgICAgICAgIH0sXG4gICAgICAgICAgICAgICAgICB7XG4gICAgICAgICAgICAgICAgICAgIGJlZ2luOiAvXFwoLywgZW5kOiAvXFwpLyxcbiAgICAgICAgICAgICAgICAgICAgZXhjbHVkZUJlZ2luOiB0cnVlLCBleGNsdWRlRW5kOiB0cnVlLFxuICAgICAgICAgICAgICAgICAgICBrZXl3b3JkczogS0VZV09SRFMsXG4gICAgICAgICAgICAgICAgICAgIGNvbnRhaW5zOiBQQVJBTVNfQ09OVEFJTlNcbiAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICBdXG4gICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIF1cbiAgICAgICAgICB9LFxuICAgICAgICAgIHsgLy8gRTRYIC8gSlNYXG4gICAgICAgICAgICBiZWdpbjogLzwvLCBlbmQ6IC8oXFwvXFx3K3xcXHcrXFwvKT4vLFxuICAgICAgICAgICAgc3ViTGFuZ3VhZ2U6ICd4bWwnLFxuICAgICAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICAgICAge2JlZ2luOiAvPFxcdytcXHMqXFwvPi8sIHNraXA6IHRydWV9LFxuICAgICAgICAgICAgICB7XG4gICAgICAgICAgICAgICAgYmVnaW46IC88XFx3Ky8sIGVuZDogLyhcXC9cXHcrfFxcdytcXC8pPi8sIHNraXA6IHRydWUsXG4gICAgICAgICAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICAgICAgICAgIHtiZWdpbjogLzxcXHcrXFxzKlxcLz4vLCBza2lwOiB0cnVlfSxcbiAgICAgICAgICAgICAgICAgICdzZWxmJ1xuICAgICAgICAgICAgICAgIF1cbiAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgXVxuICAgICAgICAgIH1cbiAgICAgICAgXSxcbiAgICAgICAgcmVsZXZhbmNlOiAwXG4gICAgICB9LFxuICAgICAge1xuICAgICAgICBjbGFzc05hbWU6ICdmdW5jdGlvbicsXG4gICAgICAgIGJlZ2luS2V5d29yZHM6ICdmdW5jdGlvbicsIGVuZDogL1xcey8sIGV4Y2x1ZGVFbmQ6IHRydWUsXG4gICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAgaGxqcy5pbmhlcml0KGhsanMuVElUTEVfTU9ERSwge2JlZ2luOiBJREVOVF9SRX0pLFxuICAgICAgICAgIHtcbiAgICAgICAgICAgIGNsYXNzTmFtZTogJ3BhcmFtcycsXG4gICAgICAgICAgICBiZWdpbjogL1xcKC8sIGVuZDogL1xcKS8sXG4gICAgICAgICAgICBleGNsdWRlQmVnaW46IHRydWUsXG4gICAgICAgICAgICBleGNsdWRlRW5kOiB0cnVlLFxuICAgICAgICAgICAgY29udGFpbnM6IFBBUkFNU19DT05UQUlOU1xuICAgICAgICAgIH1cbiAgICAgICAgXSxcbiAgICAgICAgaWxsZWdhbDogL1xcW3wlL1xuICAgICAgfSxcbiAgICAgIHtcbiAgICAgICAgYmVnaW46IC9cXCRbKC5dLyAvLyByZWxldmFuY2UgYm9vc3RlciBmb3IgYSBwYXR0ZXJuIGNvbW1vbiB0byBKUyBsaWJzOiBgJChzb21ldGhpbmcpYCBhbmQgYCQuc29tZXRoaW5nYFxuICAgICAgfSxcbiAgICAgIGhsanMuTUVUSE9EX0dVQVJELFxuICAgICAgeyAvLyBFUzYgY2xhc3NcbiAgICAgICAgY2xhc3NOYW1lOiAnY2xhc3MnLFxuICAgICAgICBiZWdpbktleXdvcmRzOiAnY2xhc3MnLCBlbmQ6IC9bezs9XS8sIGV4Y2x1ZGVFbmQ6IHRydWUsXG4gICAgICAgIGlsbGVnYWw6IC9bOlwiXFxbXFxdXS8sXG4gICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAge2JlZ2luS2V5d29yZHM6ICdleHRlbmRzJ30sXG4gICAgICAgICAgaGxqcy5VTkRFUlNDT1JFX1RJVExFX01PREVcbiAgICAgICAgXVxuICAgICAgfSxcbiAgICAgIHtcbiAgICAgICAgYmVnaW5LZXl3b3JkczogJ2NvbnN0cnVjdG9yIGdldCBzZXQnLCBlbmQ6IC9cXHsvLCBleGNsdWRlRW5kOiB0cnVlXG4gICAgICB9XG4gICAgXSxcbiAgICBpbGxlZ2FsOiAvIyg/ISEpL1xuICB9O1xufTsiLCJtb2R1bGUuZXhwb3J0cyA9IGZ1bmN0aW9uKGhsanMpIHtcblxuICB2YXIgQU5OT1RBVElPTiA9IHsgY2xhc3NOYW1lOiAnbWV0YScsIGJlZ2luOiAnQFtBLVphLXpdKycgfTtcblxuICAvLyB1c2VkIGluIHN0cmluZ3MgZm9yIGVzY2FwaW5nL2ludGVycG9sYXRpb24vc3Vic3RpdHV0aW9uXG4gIHZhciBTVUJTVCA9IHtcbiAgICBjbGFzc05hbWU6ICdzdWJzdCcsXG4gICAgdmFyaWFudHM6IFtcbiAgICAgIHtiZWdpbjogJ1xcXFwkW0EtWmEtejAtOV9dKyd9LFxuICAgICAge2JlZ2luOiAnXFxcXCR7JywgZW5kOiAnfSd9XG4gICAgXVxuICB9O1xuXG4gIHZhciBTVFJJTkcgPSB7XG4gICAgY2xhc3NOYW1lOiAnc3RyaW5nJyxcbiAgICB2YXJpYW50czogW1xuICAgICAge1xuICAgICAgICBiZWdpbjogJ1wiJywgZW5kOiAnXCInLFxuICAgICAgICBpbGxlZ2FsOiAnXFxcXG4nLFxuICAgICAgICBjb250YWluczogW2hsanMuQkFDS1NMQVNIX0VTQ0FQRV1cbiAgICAgIH0sXG4gICAgICB7XG4gICAgICAgIGJlZ2luOiAnXCJcIlwiJywgZW5kOiAnXCJcIlwiJyxcbiAgICAgICAgcmVsZXZhbmNlOiAxMFxuICAgICAgfSxcbiAgICAgIHtcbiAgICAgICAgYmVnaW46ICdbYS16XStcIicsIGVuZDogJ1wiJyxcbiAgICAgICAgaWxsZWdhbDogJ1xcXFxuJyxcbiAgICAgICAgY29udGFpbnM6IFtobGpzLkJBQ0tTTEFTSF9FU0NBUEUsIFNVQlNUXVxuICAgICAgfSxcbiAgICAgIHtcbiAgICAgICAgY2xhc3NOYW1lOiAnc3RyaW5nJyxcbiAgICAgICAgYmVnaW46ICdbYS16XStcIlwiXCInLCBlbmQ6ICdcIlwiXCInLFxuICAgICAgICBjb250YWluczogW1NVQlNUXSxcbiAgICAgICAgcmVsZXZhbmNlOiAxMFxuICAgICAgfVxuICAgIF1cblxuICB9O1xuXG4gIHZhciBTWU1CT0wgPSB7XG4gICAgY2xhc3NOYW1lOiAnc3ltYm9sJyxcbiAgICBiZWdpbjogJ1xcJ1xcXFx3W1xcXFx3XFxcXGRfXSooPyFcXCcpJ1xuICB9O1xuXG4gIHZhciBUWVBFID0ge1xuICAgIGNsYXNzTmFtZTogJ3R5cGUnLFxuICAgIGJlZ2luOiAnXFxcXGJbQS1aXVtBLVphLXowLTlfXSonLFxuICAgIHJlbGV2YW5jZTogMFxuICB9O1xuXG4gIHZhciBOQU1FID0ge1xuICAgIGNsYXNzTmFtZTogJ3RpdGxlJyxcbiAgICBiZWdpbjogL1teMC05XFxuXFx0IFwiJygpLC5ge31cXFtcXF06O11bXlxcblxcdCBcIicoKSwuYHt9XFxbXFxdOjtdK3xbXjAtOVxcblxcdCBcIicoKSwuYHt9XFxbXFxdOjs9XS8sXG4gICAgcmVsZXZhbmNlOiAwXG4gIH07XG5cbiAgdmFyIENMQVNTID0ge1xuICAgIGNsYXNzTmFtZTogJ2NsYXNzJyxcbiAgICBiZWdpbktleXdvcmRzOiAnY2xhc3Mgb2JqZWN0IHRyYWl0IHR5cGUnLFxuICAgIGVuZDogL1s6PXtcXFtcXG47XS8sXG4gICAgZXhjbHVkZUVuZDogdHJ1ZSxcbiAgICBjb250YWluczogW1xuICAgICAge1xuICAgICAgICBiZWdpbktleXdvcmRzOiAnZXh0ZW5kcyB3aXRoJyxcbiAgICAgICAgcmVsZXZhbmNlOiAxMFxuICAgICAgfSxcbiAgICAgIHtcbiAgICAgICAgYmVnaW46IC9cXFsvLFxuICAgICAgICBlbmQ6IC9cXF0vLFxuICAgICAgICBleGNsdWRlQmVnaW46IHRydWUsXG4gICAgICAgIGV4Y2x1ZGVFbmQ6IHRydWUsXG4gICAgICAgIHJlbGV2YW5jZTogMCxcbiAgICAgICAgY29udGFpbnM6IFtUWVBFXVxuICAgICAgfSxcbiAgICAgIHtcbiAgICAgICAgY2xhc3NOYW1lOiAncGFyYW1zJyxcbiAgICAgICAgYmVnaW46IC9cXCgvLFxuICAgICAgICBlbmQ6IC9cXCkvLFxuICAgICAgICBleGNsdWRlQmVnaW46IHRydWUsXG4gICAgICAgIGV4Y2x1ZGVFbmQ6IHRydWUsXG4gICAgICAgIHJlbGV2YW5jZTogMCxcbiAgICAgICAgY29udGFpbnM6IFtUWVBFXVxuICAgICAgfSxcbiAgICAgIE5BTUVcbiAgICBdXG4gIH07XG5cbiAgdmFyIE1FVEhPRCA9IHtcbiAgICBjbGFzc05hbWU6ICdmdW5jdGlvbicsXG4gICAgYmVnaW5LZXl3b3JkczogJ2RlZicsXG4gICAgZW5kOiAvWzo9e1xcWyhcXG47XS8sXG4gICAgZXhjbHVkZUVuZDogdHJ1ZSxcbiAgICBjb250YWluczogW05BTUVdXG4gIH07XG5cbiAgcmV0dXJuIHtcbiAgICBrZXl3b3Jkczoge1xuICAgICAgbGl0ZXJhbDogJ3RydWUgZmFsc2UgbnVsbCcsXG4gICAgICBrZXl3b3JkOiAndHlwZSB5aWVsZCBsYXp5IG92ZXJyaWRlIGRlZiB3aXRoIHZhbCB2YXIgc2VhbGVkIGFic3RyYWN0IHByaXZhdGUgdHJhaXQgb2JqZWN0IGlmIGZvclNvbWUgZm9yIHdoaWxlIHRocm93IGZpbmFsbHkgcHJvdGVjdGVkIGV4dGVuZHMgaW1wb3J0IGZpbmFsIHJldHVybiBlbHNlIGJyZWFrIG5ldyBjYXRjaCBzdXBlciBjbGFzcyBjYXNlIHBhY2thZ2UgZGVmYXVsdCB0cnkgdGhpcyBtYXRjaCBjb250aW51ZSB0aHJvd3MgaW1wbGljaXQnXG4gICAgfSxcbiAgICBjb250YWluczogW1xuICAgICAgaGxqcy5DX0xJTkVfQ09NTUVOVF9NT0RFLFxuICAgICAgaGxqcy5DX0JMT0NLX0NPTU1FTlRfTU9ERSxcbiAgICAgIFNUUklORyxcbiAgICAgIFNZTUJPTCxcbiAgICAgIFRZUEUsXG4gICAgICBNRVRIT0QsXG4gICAgICBDTEFTUyxcbiAgICAgIGhsanMuQ19OVU1CRVJfTU9ERSxcbiAgICAgIEFOTk9UQVRJT05cbiAgICBdXG4gIH07XG59OyIsIm1vZHVsZS5leHBvcnRzID0gZnVuY3Rpb24oaGxqcykge1xuICB2YXIgSlNfSURFTlRfUkUgPSAnW0EtWmEteiRfXVswLTlBLVphLXokX10qJztcbiAgdmFyIEtFWVdPUkRTID0ge1xuICAgIGtleXdvcmQ6XG4gICAgICAnaW4gaWYgZm9yIHdoaWxlIGZpbmFsbHkgdmFyIG5ldyBmdW5jdGlvbiBkbyByZXR1cm4gdm9pZCBlbHNlIGJyZWFrIGNhdGNoICcgK1xuICAgICAgJ2luc3RhbmNlb2Ygd2l0aCB0aHJvdyBjYXNlIGRlZmF1bHQgdHJ5IHRoaXMgc3dpdGNoIGNvbnRpbnVlIHR5cGVvZiBkZWxldGUgJyArXG4gICAgICAnbGV0IHlpZWxkIGNvbnN0IGNsYXNzIHB1YmxpYyBwcml2YXRlIHByb3RlY3RlZCBnZXQgc2V0IHN1cGVyICcgK1xuICAgICAgJ3N0YXRpYyBpbXBsZW1lbnRzIGVudW0gZXhwb3J0IGltcG9ydCBkZWNsYXJlIHR5cGUgbmFtZXNwYWNlIGFic3RyYWN0ICcgK1xuICAgICAgJ2FzIGZyb20gZXh0ZW5kcyBhc3luYyBhd2FpdCcsXG4gICAgbGl0ZXJhbDpcbiAgICAgICd0cnVlIGZhbHNlIG51bGwgdW5kZWZpbmVkIE5hTiBJbmZpbml0eScsXG4gICAgYnVpbHRfaW46XG4gICAgICAnZXZhbCBpc0Zpbml0ZSBpc05hTiBwYXJzZUZsb2F0IHBhcnNlSW50IGRlY29kZVVSSSBkZWNvZGVVUklDb21wb25lbnQgJyArXG4gICAgICAnZW5jb2RlVVJJIGVuY29kZVVSSUNvbXBvbmVudCBlc2NhcGUgdW5lc2NhcGUgT2JqZWN0IEZ1bmN0aW9uIEJvb2xlYW4gRXJyb3IgJyArXG4gICAgICAnRXZhbEVycm9yIEludGVybmFsRXJyb3IgUmFuZ2VFcnJvciBSZWZlcmVuY2VFcnJvciBTdG9wSXRlcmF0aW9uIFN5bnRheEVycm9yICcgK1xuICAgICAgJ1R5cGVFcnJvciBVUklFcnJvciBOdW1iZXIgTWF0aCBEYXRlIFN0cmluZyBSZWdFeHAgQXJyYXkgRmxvYXQzMkFycmF5ICcgK1xuICAgICAgJ0Zsb2F0NjRBcnJheSBJbnQxNkFycmF5IEludDMyQXJyYXkgSW50OEFycmF5IFVpbnQxNkFycmF5IFVpbnQzMkFycmF5ICcgK1xuICAgICAgJ1VpbnQ4QXJyYXkgVWludDhDbGFtcGVkQXJyYXkgQXJyYXlCdWZmZXIgRGF0YVZpZXcgSlNPTiBJbnRsIGFyZ3VtZW50cyByZXF1aXJlICcgK1xuICAgICAgJ21vZHVsZSBjb25zb2xlIHdpbmRvdyBkb2N1bWVudCBhbnkgbnVtYmVyIGJvb2xlYW4gc3RyaW5nIHZvaWQgUHJvbWlzZSdcbiAgfTtcblxuICB2YXIgREVDT1JBVE9SID0ge1xuICAgIGNsYXNzTmFtZTogJ21ldGEnLFxuICAgIGJlZ2luOiAnQCcgKyBKU19JREVOVF9SRSxcbiAgfTtcblxuICB2YXIgQVJHUyA9XG4gIHtcbiAgICBiZWdpbjogJ1xcXFwoJyxcbiAgICBlbmQ6IC9cXCkvLFxuICAgIGtleXdvcmRzOiBLRVlXT1JEUyxcbiAgICBjb250YWluczogW1xuICAgICAgJ3NlbGYnLFxuICAgICAgaGxqcy5RVU9URV9TVFJJTkdfTU9ERSxcbiAgICAgIGhsanMuQVBPU19TVFJJTkdfTU9ERSxcbiAgICAgIGhsanMuTlVNQkVSX01PREVcbiAgICBdXG4gIH07XG5cbiAgdmFyIFBBUkFNUyA9IHtcbiAgICBjbGFzc05hbWU6ICdwYXJhbXMnLFxuICAgIGJlZ2luOiAvXFwoLywgZW5kOiAvXFwpLyxcbiAgICBleGNsdWRlQmVnaW46IHRydWUsXG4gICAgZXhjbHVkZUVuZDogdHJ1ZSxcbiAgICBrZXl3b3JkczogS0VZV09SRFMsXG4gICAgY29udGFpbnM6IFtcbiAgICAgIGhsanMuQ19MSU5FX0NPTU1FTlRfTU9ERSxcbiAgICAgIGhsanMuQ19CTE9DS19DT01NRU5UX01PREUsXG4gICAgICBERUNPUkFUT1IsXG4gICAgICBBUkdTXG4gICAgXVxuICB9O1xuXG4gIHJldHVybiB7XG4gICAgYWxpYXNlczogWyd0cyddLFxuICAgIGtleXdvcmRzOiBLRVlXT1JEUyxcbiAgICBjb250YWluczogW1xuICAgICAge1xuICAgICAgICBjbGFzc05hbWU6ICdtZXRhJyxcbiAgICAgICAgYmVnaW46IC9eXFxzKlsnXCJddXNlIHN0cmljdFsnXCJdL1xuICAgICAgfSxcbiAgICAgIGhsanMuQVBPU19TVFJJTkdfTU9ERSxcbiAgICAgIGhsanMuUVVPVEVfU1RSSU5HX01PREUsXG4gICAgICB7IC8vIHRlbXBsYXRlIHN0cmluZ1xuICAgICAgICBjbGFzc05hbWU6ICdzdHJpbmcnLFxuICAgICAgICBiZWdpbjogJ2AnLCBlbmQ6ICdgJyxcbiAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICBobGpzLkJBQ0tTTEFTSF9FU0NBUEUsXG4gICAgICAgICAge1xuICAgICAgICAgICAgY2xhc3NOYW1lOiAnc3Vic3QnLFxuICAgICAgICAgICAgYmVnaW46ICdcXFxcJFxcXFx7JywgZW5kOiAnXFxcXH0nXG4gICAgICAgICAgfVxuICAgICAgICBdXG4gICAgICB9LFxuICAgICAgaGxqcy5DX0xJTkVfQ09NTUVOVF9NT0RFLFxuICAgICAgaGxqcy5DX0JMT0NLX0NPTU1FTlRfTU9ERSxcbiAgICAgIHtcbiAgICAgICAgY2xhc3NOYW1lOiAnbnVtYmVyJyxcbiAgICAgICAgdmFyaWFudHM6IFtcbiAgICAgICAgICB7IGJlZ2luOiAnXFxcXGIoMFtiQl1bMDFdKyknIH0sXG4gICAgICAgICAgeyBiZWdpbjogJ1xcXFxiKDBbb09dWzAtN10rKScgfSxcbiAgICAgICAgICB7IGJlZ2luOiBobGpzLkNfTlVNQkVSX1JFIH1cbiAgICAgICAgXSxcbiAgICAgICAgcmVsZXZhbmNlOiAwXG4gICAgICB9LFxuICAgICAgeyAvLyBcInZhbHVlXCIgY29udGFpbmVyXG4gICAgICAgIGJlZ2luOiAnKCcgKyBobGpzLlJFX1NUQVJURVJTX1JFICsgJ3xcXFxcYihjYXNlfHJldHVybnx0aHJvdylcXFxcYilcXFxccyonLFxuICAgICAgICBrZXl3b3JkczogJ3JldHVybiB0aHJvdyBjYXNlJyxcbiAgICAgICAgY29udGFpbnM6IFtcbiAgICAgICAgICBobGpzLkNfTElORV9DT01NRU5UX01PREUsXG4gICAgICAgICAgaGxqcy5DX0JMT0NLX0NPTU1FTlRfTU9ERSxcbiAgICAgICAgICBobGpzLlJFR0VYUF9NT0RFLFxuICAgICAgICAgIHtcbiAgICAgICAgICAgIGNsYXNzTmFtZTogJ2Z1bmN0aW9uJyxcbiAgICAgICAgICAgIGJlZ2luOiAnKFxcXFwoLio/XFxcXCl8JyArIGhsanMuSURFTlRfUkUgKyAnKVxcXFxzKj0+JywgcmV0dXJuQmVnaW46IHRydWUsXG4gICAgICAgICAgICBlbmQ6ICdcXFxccyo9PicsXG4gICAgICAgICAgICBjb250YWluczogW1xuICAgICAgICAgICAgICB7XG4gICAgICAgICAgICAgICAgY2xhc3NOYW1lOiAncGFyYW1zJyxcbiAgICAgICAgICAgICAgICB2YXJpYW50czogW1xuICAgICAgICAgICAgICAgICAge1xuICAgICAgICAgICAgICAgICAgICBiZWdpbjogaGxqcy5JREVOVF9SRVxuICAgICAgICAgICAgICAgICAgfSxcbiAgICAgICAgICAgICAgICAgIHtcbiAgICAgICAgICAgICAgICAgICAgYmVnaW46IC9cXChcXHMqXFwpLyxcbiAgICAgICAgICAgICAgICAgIH0sXG4gICAgICAgICAgICAgICAgICB7XG4gICAgICAgICAgICAgICAgICAgIGJlZ2luOiAvXFwoLywgZW5kOiAvXFwpLyxcbiAgICAgICAgICAgICAgICAgICAgZXhjbHVkZUJlZ2luOiB0cnVlLCBleGNsdWRlRW5kOiB0cnVlLFxuICAgICAgICAgICAgICAgICAgICBrZXl3b3JkczogS0VZV09SRFMsXG4gICAgICAgICAgICAgICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAgICAgICAgICAgICAgJ3NlbGYnLFxuICAgICAgICAgICAgICAgICAgICAgIGhsanMuQ19MSU5FX0NPTU1FTlRfTU9ERSxcbiAgICAgICAgICAgICAgICAgICAgICBobGpzLkNfQkxPQ0tfQ09NTUVOVF9NT0RFXG4gICAgICAgICAgICAgICAgICAgIF1cbiAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICBdXG4gICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIF1cbiAgICAgICAgICB9XG4gICAgICAgIF0sXG4gICAgICAgIHJlbGV2YW5jZTogMFxuICAgICAgfSxcbiAgICAgIHtcbiAgICAgICAgY2xhc3NOYW1lOiAnZnVuY3Rpb24nLFxuICAgICAgICBiZWdpbjogJ2Z1bmN0aW9uJywgZW5kOiAvW1xceztdLywgZXhjbHVkZUVuZDogdHJ1ZSxcbiAgICAgICAga2V5d29yZHM6IEtFWVdPUkRTLFxuICAgICAgICBjb250YWluczogW1xuICAgICAgICAgICdzZWxmJyxcbiAgICAgICAgICBobGpzLmluaGVyaXQoaGxqcy5USVRMRV9NT0RFLCB7IGJlZ2luOiBKU19JREVOVF9SRSB9KSxcbiAgICAgICAgICBQQVJBTVNcbiAgICAgICAgXSxcbiAgICAgICAgaWxsZWdhbDogLyUvLFxuICAgICAgICByZWxldmFuY2U6IDAgLy8gKCkgPT4ge30gaXMgbW9yZSB0eXBpY2FsIGluIFR5cGVTY3JpcHRcbiAgICAgIH0sXG4gICAgICB7XG4gICAgICAgIGJlZ2luS2V5d29yZHM6ICdjb25zdHJ1Y3RvcicsIGVuZDogL1xcey8sIGV4Y2x1ZGVFbmQ6IHRydWUsXG4gICAgICAgIGNvbnRhaW5zOiBbXG4gICAgICAgICAgJ3NlbGYnLFxuICAgICAgICAgIFBBUkFNU1xuICAgICAgICBdXG4gICAgICB9LFxuICAgICAgeyAvLyBwcmV2ZW50IHJlZmVyZW5jZXMgbGlrZSBtb2R1bGUuaWQgZnJvbSBiZWluZyBoaWdsaWdodGVkIGFzIG1vZHVsZSBkZWZpbml0aW9uc1xuICAgICAgICBiZWdpbjogL21vZHVsZVxcLi8sXG4gICAgICAgIGtleXdvcmRzOiB7IGJ1aWx0X2luOiAnbW9kdWxlJyB9LFxuICAgICAgICByZWxldmFuY2U6IDBcbiAgICAgIH0sXG4gICAgICB7XG4gICAgICAgIGJlZ2luS2V5d29yZHM6ICdtb2R1bGUnLCBlbmQ6IC9cXHsvLCBleGNsdWRlRW5kOiB0cnVlXG4gICAgICB9LFxuICAgICAge1xuICAgICAgICBiZWdpbktleXdvcmRzOiAnaW50ZXJmYWNlJywgZW5kOiAvXFx7LywgZXhjbHVkZUVuZDogdHJ1ZSxcbiAgICAgICAga2V5d29yZHM6ICdpbnRlcmZhY2UgZXh0ZW5kcydcbiAgICAgIH0sXG4gICAgICB7XG4gICAgICAgIGJlZ2luOiAvXFwkWyguXS8gLy8gcmVsZXZhbmNlIGJvb3N0ZXIgZm9yIGEgcGF0dGVybiBjb21tb24gdG8gSlMgbGliczogYCQoc29tZXRoaW5nKWAgYW5kIGAkLnNvbWV0aGluZ2BcbiAgICAgIH0sXG4gICAgICB7XG4gICAgICAgIGJlZ2luOiAnXFxcXC4nICsgaGxqcy5JREVOVF9SRSwgcmVsZXZhbmNlOiAwIC8vIGhhY2s6IHByZXZlbnRzIGRldGVjdGlvbiBvZiBrZXl3b3JkcyBhZnRlciBkb3RzXG4gICAgICB9LFxuICAgICAgREVDT1JBVE9SLFxuICAgICAgQVJHU1xuICAgIF1cbiAgfTtcbn07IiwiLyoqXG5AbGljZW5zZVxuQ29weXJpZ2h0IChjKSAyMDE5IFRoZSBQb2x5bWVyIFByb2plY3QgQXV0aG9ycy4gQWxsIHJpZ2h0cyByZXNlcnZlZC5cblRoaXMgY29kZSBtYXkgb25seSBiZSB1c2VkIHVuZGVyIHRoZSBCU0Qgc3R5bGUgbGljZW5zZSBmb3VuZCBhdFxuaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0xJQ0VOU0UudHh0IFRoZSBjb21wbGV0ZSBzZXQgb2YgYXV0aG9ycyBtYXkgYmUgZm91bmQgYXRcbmh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9BVVRIT1JTLnR4dCBUaGUgY29tcGxldGUgc2V0IG9mIGNvbnRyaWJ1dG9ycyBtYXkgYmVcbmZvdW5kIGF0IGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9DT05UUklCVVRPUlMudHh0IENvZGUgZGlzdHJpYnV0ZWQgYnkgR29vZ2xlIGFzXG5wYXJ0IG9mIHRoZSBwb2x5bWVyIHByb2plY3QgaXMgYWxzbyBzdWJqZWN0IHRvIGFuIGFkZGl0aW9uYWwgSVAgcmlnaHRzIGdyYW50XG5mb3VuZCBhdCBodHRwOi8vcG9seW1lci5naXRodWIuaW8vUEFURU5UUy50eHRcbiovXG5leHBvcnQgY29uc3Qgc3VwcG9ydHNBZG9wdGluZ1N0eWxlU2hlZXRzID0gKCdhZG9wdGVkU3R5bGVTaGVldHMnIGluIERvY3VtZW50LnByb3RvdHlwZSkgJiZcbiAgICAoJ3JlcGxhY2UnIGluIENTU1N0eWxlU2hlZXQucHJvdG90eXBlKTtcbmNvbnN0IGNvbnN0cnVjdGlvblRva2VuID0gU3ltYm9sKCk7XG5leHBvcnQgY2xhc3MgQ1NTUmVzdWx0IHtcbiAgICBjb25zdHJ1Y3Rvcihjc3NUZXh0LCBzYWZlVG9rZW4pIHtcbiAgICAgICAgaWYgKHNhZmVUb2tlbiAhPT0gY29uc3RydWN0aW9uVG9rZW4pIHtcbiAgICAgICAgICAgIHRocm93IG5ldyBFcnJvcignQ1NTUmVzdWx0IGlzIG5vdCBjb25zdHJ1Y3RhYmxlLiBVc2UgYHVuc2FmZUNTU2Agb3IgYGNzc2AgaW5zdGVhZC4nKTtcbiAgICAgICAgfVxuICAgICAgICB0aGlzLmNzc1RleHQgPSBjc3NUZXh0O1xuICAgIH1cbiAgICAvLyBOb3RlLCB0aGlzIGlzIGEgZ2V0dGVyIHNvIHRoYXQgaXQncyBsYXp5LiBJbiBwcmFjdGljZSwgdGhpcyBtZWFuc1xuICAgIC8vIHN0eWxlc2hlZXRzIGFyZSBub3QgY3JlYXRlZCB1bnRpbCB0aGUgZmlyc3QgZWxlbWVudCBpbnN0YW5jZSBpcyBtYWRlLlxuICAgIGdldCBzdHlsZVNoZWV0KCkge1xuICAgICAgICBpZiAodGhpcy5fc3R5bGVTaGVldCA9PT0gdW5kZWZpbmVkKSB7XG4gICAgICAgICAgICAvLyBOb3RlLCBpZiBgYWRvcHRlZFN0eWxlU2hlZXRzYCBpcyBzdXBwb3J0ZWQgdGhlbiB3ZSBhc3N1bWUgQ1NTU3R5bGVTaGVldFxuICAgICAgICAgICAgLy8gaXMgY29uc3RydWN0YWJsZS5cbiAgICAgICAgICAgIGlmIChzdXBwb3J0c0Fkb3B0aW5nU3R5bGVTaGVldHMpIHtcbiAgICAgICAgICAgICAgICB0aGlzLl9zdHlsZVNoZWV0ID0gbmV3IENTU1N0eWxlU2hlZXQoKTtcbiAgICAgICAgICAgICAgICB0aGlzLl9zdHlsZVNoZWV0LnJlcGxhY2VTeW5jKHRoaXMuY3NzVGV4dCk7XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgICAgICB0aGlzLl9zdHlsZVNoZWV0ID0gbnVsbDtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgICAgICByZXR1cm4gdGhpcy5fc3R5bGVTaGVldDtcbiAgICB9XG4gICAgdG9TdHJpbmcoKSB7XG4gICAgICAgIHJldHVybiB0aGlzLmNzc1RleHQ7XG4gICAgfVxufVxuLyoqXG4gKiBXcmFwIGEgdmFsdWUgZm9yIGludGVycG9sYXRpb24gaW4gYSBjc3MgdGFnZ2VkIHRlbXBsYXRlIGxpdGVyYWwuXG4gKlxuICogVGhpcyBpcyB1bnNhZmUgYmVjYXVzZSB1bnRydXN0ZWQgQ1NTIHRleHQgY2FuIGJlIHVzZWQgdG8gcGhvbmUgaG9tZVxuICogb3IgZXhmaWx0cmF0ZSBkYXRhIHRvIGFuIGF0dGFja2VyIGNvbnRyb2xsZWQgc2l0ZS4gVGFrZSBjYXJlIHRvIG9ubHkgdXNlXG4gKiB0aGlzIHdpdGggdHJ1c3RlZCBpbnB1dC5cbiAqL1xuZXhwb3J0IGNvbnN0IHVuc2FmZUNTUyA9ICh2YWx1ZSkgPT4ge1xuICAgIHJldHVybiBuZXcgQ1NTUmVzdWx0KFN0cmluZyh2YWx1ZSksIGNvbnN0cnVjdGlvblRva2VuKTtcbn07XG5jb25zdCB0ZXh0RnJvbUNTU1Jlc3VsdCA9ICh2YWx1ZSkgPT4ge1xuICAgIGlmICh2YWx1ZSBpbnN0YW5jZW9mIENTU1Jlc3VsdCkge1xuICAgICAgICByZXR1cm4gdmFsdWUuY3NzVGV4dDtcbiAgICB9XG4gICAgZWxzZSB7XG4gICAgICAgIHRocm93IG5ldyBFcnJvcihgVmFsdWUgcGFzc2VkIHRvICdjc3MnIGZ1bmN0aW9uIG11c3QgYmUgYSAnY3NzJyBmdW5jdGlvbiByZXN1bHQ6ICR7dmFsdWV9LiBVc2UgJ3Vuc2FmZUNTUycgdG8gcGFzcyBub24tbGl0ZXJhbCB2YWx1ZXMsIGJ1dFxuICAgICAgICAgICAgdGFrZSBjYXJlIHRvIGVuc3VyZSBwYWdlIHNlY3VyaXR5LmApO1xuICAgIH1cbn07XG4vKipcbiAqIFRlbXBsYXRlIHRhZyB3aGljaCB3aGljaCBjYW4gYmUgdXNlZCB3aXRoIExpdEVsZW1lbnQncyBgc3R5bGVgIHByb3BlcnR5IHRvXG4gKiBzZXQgZWxlbWVudCBzdHlsZXMuIEZvciBzZWN1cml0eSByZWFzb25zLCBvbmx5IGxpdGVyYWwgc3RyaW5nIHZhbHVlcyBtYXkgYmVcbiAqIHVzZWQuIFRvIGluY29ycG9yYXRlIG5vbi1saXRlcmFsIHZhbHVlcyBgdW5zYWZlQ1NTYCBtYXkgYmUgdXNlZCBpbnNpZGUgYVxuICogdGVtcGxhdGUgc3RyaW5nIHBhcnQuXG4gKi9cbmV4cG9ydCBjb25zdCBjc3MgPSAoc3RyaW5ncywgLi4udmFsdWVzKSA9PiB7XG4gICAgY29uc3QgY3NzVGV4dCA9IHZhbHVlcy5yZWR1Y2UoKGFjYywgdiwgaWR4KSA9PiBhY2MgKyB0ZXh0RnJvbUNTU1Jlc3VsdCh2KSArIHN0cmluZ3NbaWR4ICsgMV0sIHN0cmluZ3NbMF0pO1xuICAgIHJldHVybiBuZXcgQ1NTUmVzdWx0KGNzc1RleHQsIGNvbnN0cnVjdGlvblRva2VuKTtcbn07XG4vLyMgc291cmNlTWFwcGluZ1VSTD1jc3MtdGFnLmpzLm1hcCIsIi8qKlxuICogQGxpY2Vuc2VcbiAqIENvcHlyaWdodCAoYykgMjAxNyBUaGUgUG9seW1lciBQcm9qZWN0IEF1dGhvcnMuIEFsbCByaWdodHMgcmVzZXJ2ZWQuXG4gKiBUaGlzIGNvZGUgbWF5IG9ubHkgYmUgdXNlZCB1bmRlciB0aGUgQlNEIHN0eWxlIGxpY2Vuc2UgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9MSUNFTlNFLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBhdXRob3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0FVVEhPUlMudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGNvbnRyaWJ1dG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9DT05UUklCVVRPUlMudHh0XG4gKiBDb2RlIGRpc3RyaWJ1dGVkIGJ5IEdvb2dsZSBhcyBwYXJ0IG9mIHRoZSBwb2x5bWVyIHByb2plY3QgaXMgYWxzb1xuICogc3ViamVjdCB0byBhbiBhZGRpdGlvbmFsIElQIHJpZ2h0cyBncmFudCBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL1BBVEVOVFMudHh0XG4gKi9cbmNvbnN0IGxlZ2FjeUN1c3RvbUVsZW1lbnQgPSAodGFnTmFtZSwgY2xhenopID0+IHtcbiAgICB3aW5kb3cuY3VzdG9tRWxlbWVudHMuZGVmaW5lKHRhZ05hbWUsIGNsYXp6KTtcbiAgICAvLyBDYXN0IGFzIGFueSBiZWNhdXNlIFRTIGRvZXNuJ3QgcmVjb2duaXplIHRoZSByZXR1cm4gdHlwZSBhcyBiZWluZyBhXG4gICAgLy8gc3VidHlwZSBvZiB0aGUgZGVjb3JhdGVkIGNsYXNzIHdoZW4gY2xhenogaXMgdHlwZWQgYXNcbiAgICAvLyBgQ29uc3RydWN0b3I8SFRNTEVsZW1lbnQ+YCBmb3Igc29tZSByZWFzb24uXG4gICAgLy8gYENvbnN0cnVjdG9yPEhUTUxFbGVtZW50PmAgaXMgaGVscGZ1bCB0byBtYWtlIHN1cmUgdGhlIGRlY29yYXRvciBpc1xuICAgIC8vIGFwcGxpZWQgdG8gZWxlbWVudHMgaG93ZXZlci5cbiAgICAvLyB0c2xpbnQ6ZGlzYWJsZS1uZXh0LWxpbmU6bm8tYW55XG4gICAgcmV0dXJuIGNsYXp6O1xufTtcbmNvbnN0IHN0YW5kYXJkQ3VzdG9tRWxlbWVudCA9ICh0YWdOYW1lLCBkZXNjcmlwdG9yKSA9PiB7XG4gICAgY29uc3QgeyBraW5kLCBlbGVtZW50cyB9ID0gZGVzY3JpcHRvcjtcbiAgICByZXR1cm4ge1xuICAgICAgICBraW5kLFxuICAgICAgICBlbGVtZW50cyxcbiAgICAgICAgLy8gVGhpcyBjYWxsYmFjayBpcyBjYWxsZWQgb25jZSB0aGUgY2xhc3MgaXMgb3RoZXJ3aXNlIGZ1bGx5IGRlZmluZWRcbiAgICAgICAgZmluaXNoZXIoY2xhenopIHtcbiAgICAgICAgICAgIHdpbmRvdy5jdXN0b21FbGVtZW50cy5kZWZpbmUodGFnTmFtZSwgY2xhenopO1xuICAgICAgICB9XG4gICAgfTtcbn07XG4vKipcbiAqIENsYXNzIGRlY29yYXRvciBmYWN0b3J5IHRoYXQgZGVmaW5lcyB0aGUgZGVjb3JhdGVkIGNsYXNzIGFzIGEgY3VzdG9tIGVsZW1lbnQuXG4gKlxuICogQHBhcmFtIHRhZ05hbWUgdGhlIG5hbWUgb2YgdGhlIGN1c3RvbSBlbGVtZW50IHRvIGRlZmluZVxuICovXG5leHBvcnQgY29uc3QgY3VzdG9tRWxlbWVudCA9ICh0YWdOYW1lKSA9PiAoY2xhc3NPckRlc2NyaXB0b3IpID0+ICh0eXBlb2YgY2xhc3NPckRlc2NyaXB0b3IgPT09ICdmdW5jdGlvbicpID9cbiAgICBsZWdhY3lDdXN0b21FbGVtZW50KHRhZ05hbWUsIGNsYXNzT3JEZXNjcmlwdG9yKSA6XG4gICAgc3RhbmRhcmRDdXN0b21FbGVtZW50KHRhZ05hbWUsIGNsYXNzT3JEZXNjcmlwdG9yKTtcbmNvbnN0IHN0YW5kYXJkUHJvcGVydHkgPSAob3B0aW9ucywgZWxlbWVudCkgPT4ge1xuICAgIC8vIFdoZW4gZGVjb3JhdGluZyBhbiBhY2Nlc3NvciwgcGFzcyBpdCB0aHJvdWdoIGFuZCBhZGQgcHJvcGVydHkgbWV0YWRhdGEuXG4gICAgLy8gTm90ZSwgdGhlIGBoYXNPd25Qcm9wZXJ0eWAgY2hlY2sgaW4gYGNyZWF0ZVByb3BlcnR5YCBlbnN1cmVzIHdlIGRvbid0XG4gICAgLy8gc3RvbXAgb3ZlciB0aGUgdXNlcidzIGFjY2Vzc29yLlxuICAgIGlmIChlbGVtZW50LmtpbmQgPT09ICdtZXRob2QnICYmIGVsZW1lbnQuZGVzY3JpcHRvciAmJlxuICAgICAgICAhKCd2YWx1ZScgaW4gZWxlbWVudC5kZXNjcmlwdG9yKSkge1xuICAgICAgICByZXR1cm4gT2JqZWN0LmFzc2lnbih7fSwgZWxlbWVudCwgeyBmaW5pc2hlcihjbGF6eikge1xuICAgICAgICAgICAgICAgIGNsYXp6LmNyZWF0ZVByb3BlcnR5KGVsZW1lbnQua2V5LCBvcHRpb25zKTtcbiAgICAgICAgICAgIH0gfSk7XG4gICAgfVxuICAgIGVsc2Uge1xuICAgICAgICAvLyBjcmVhdGVQcm9wZXJ0eSgpIHRha2VzIGNhcmUgb2YgZGVmaW5pbmcgdGhlIHByb3BlcnR5LCBidXQgd2Ugc3RpbGxcbiAgICAgICAgLy8gbXVzdCByZXR1cm4gc29tZSBraW5kIG9mIGRlc2NyaXB0b3IsIHNvIHJldHVybiBhIGRlc2NyaXB0b3IgZm9yIGFuXG4gICAgICAgIC8vIHVudXNlZCBwcm90b3R5cGUgZmllbGQuIFRoZSBmaW5pc2hlciBjYWxscyBjcmVhdGVQcm9wZXJ0eSgpLlxuICAgICAgICByZXR1cm4ge1xuICAgICAgICAgICAga2luZDogJ2ZpZWxkJyxcbiAgICAgICAgICAgIGtleTogU3ltYm9sKCksXG4gICAgICAgICAgICBwbGFjZW1lbnQ6ICdvd24nLFxuICAgICAgICAgICAgZGVzY3JpcHRvcjoge30sXG4gICAgICAgICAgICAvLyBXaGVuIEBiYWJlbC9wbHVnaW4tcHJvcG9zYWwtZGVjb3JhdG9ycyBpbXBsZW1lbnRzIGluaXRpYWxpemVycyxcbiAgICAgICAgICAgIC8vIGRvIHRoaXMgaW5zdGVhZCBvZiB0aGUgaW5pdGlhbGl6ZXIgYmVsb3cuIFNlZTpcbiAgICAgICAgICAgIC8vIGh0dHBzOi8vZ2l0aHViLmNvbS9iYWJlbC9iYWJlbC9pc3N1ZXMvOTI2MCBleHRyYXM6IFtcbiAgICAgICAgICAgIC8vICAge1xuICAgICAgICAgICAgLy8gICAgIGtpbmQ6ICdpbml0aWFsaXplcicsXG4gICAgICAgICAgICAvLyAgICAgcGxhY2VtZW50OiAnb3duJyxcbiAgICAgICAgICAgIC8vICAgICBpbml0aWFsaXplcjogZGVzY3JpcHRvci5pbml0aWFsaXplcixcbiAgICAgICAgICAgIC8vICAgfVxuICAgICAgICAgICAgLy8gXSxcbiAgICAgICAgICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTpuby1hbnkgZGVjb3JhdG9yXG4gICAgICAgICAgICBpbml0aWFsaXplcigpIHtcbiAgICAgICAgICAgICAgICBpZiAodHlwZW9mIGVsZW1lbnQuaW5pdGlhbGl6ZXIgPT09ICdmdW5jdGlvbicpIHtcbiAgICAgICAgICAgICAgICAgICAgdGhpc1tlbGVtZW50LmtleV0gPSBlbGVtZW50LmluaXRpYWxpemVyLmNhbGwodGhpcyk7XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgfSxcbiAgICAgICAgICAgIGZpbmlzaGVyKGNsYXp6KSB7XG4gICAgICAgICAgICAgICAgY2xhenouY3JlYXRlUHJvcGVydHkoZWxlbWVudC5rZXksIG9wdGlvbnMpO1xuICAgICAgICAgICAgfVxuICAgICAgICB9O1xuICAgIH1cbn07XG5jb25zdCBsZWdhY3lQcm9wZXJ0eSA9IChvcHRpb25zLCBwcm90bywgbmFtZSkgPT4ge1xuICAgIHByb3RvLmNvbnN0cnVjdG9yXG4gICAgICAgIC5jcmVhdGVQcm9wZXJ0eShuYW1lLCBvcHRpb25zKTtcbn07XG4vKipcbiAqIEEgcHJvcGVydHkgZGVjb3JhdG9yIHdoaWNoIGNyZWF0ZXMgYSBMaXRFbGVtZW50IHByb3BlcnR5IHdoaWNoIHJlZmxlY3RzIGFcbiAqIGNvcnJlc3BvbmRpbmcgYXR0cmlidXRlIHZhbHVlLiBBIGBQcm9wZXJ0eURlY2xhcmF0aW9uYCBtYXkgb3B0aW9uYWxseSBiZVxuICogc3VwcGxpZWQgdG8gY29uZmlndXJlIHByb3BlcnR5IGZlYXR1cmVzLlxuICpcbiAqIEBFeHBvcnREZWNvcmF0ZWRJdGVtc1xuICovXG5leHBvcnQgZnVuY3Rpb24gcHJvcGVydHkob3B0aW9ucykge1xuICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTpuby1hbnkgZGVjb3JhdG9yXG4gICAgcmV0dXJuIChwcm90b09yRGVzY3JpcHRvciwgbmFtZSkgPT4gKG5hbWUgIT09IHVuZGVmaW5lZCkgP1xuICAgICAgICBsZWdhY3lQcm9wZXJ0eShvcHRpb25zLCBwcm90b09yRGVzY3JpcHRvciwgbmFtZSkgOlxuICAgICAgICBzdGFuZGFyZFByb3BlcnR5KG9wdGlvbnMsIHByb3RvT3JEZXNjcmlwdG9yKTtcbn1cbi8qKlxuICogQSBwcm9wZXJ0eSBkZWNvcmF0b3IgdGhhdCBjb252ZXJ0cyBhIGNsYXNzIHByb3BlcnR5IGludG8gYSBnZXR0ZXIgdGhhdFxuICogZXhlY3V0ZXMgYSBxdWVyeVNlbGVjdG9yIG9uIHRoZSBlbGVtZW50J3MgcmVuZGVyUm9vdC5cbiAqL1xuZXhwb3J0IGNvbnN0IHF1ZXJ5ID0gX3F1ZXJ5KCh0YXJnZXQsIHNlbGVjdG9yKSA9PiB0YXJnZXQucXVlcnlTZWxlY3RvcihzZWxlY3RvcikpO1xuLyoqXG4gKiBBIHByb3BlcnR5IGRlY29yYXRvciB0aGF0IGNvbnZlcnRzIGEgY2xhc3MgcHJvcGVydHkgaW50byBhIGdldHRlclxuICogdGhhdCBleGVjdXRlcyBhIHF1ZXJ5U2VsZWN0b3JBbGwgb24gdGhlIGVsZW1lbnQncyByZW5kZXJSb290LlxuICovXG5leHBvcnQgY29uc3QgcXVlcnlBbGwgPSBfcXVlcnkoKHRhcmdldCwgc2VsZWN0b3IpID0+IHRhcmdldC5xdWVyeVNlbGVjdG9yQWxsKHNlbGVjdG9yKSk7XG5jb25zdCBsZWdhY3lRdWVyeSA9IChkZXNjcmlwdG9yLCBwcm90bywgbmFtZSkgPT4ge1xuICAgIE9iamVjdC5kZWZpbmVQcm9wZXJ0eShwcm90bywgbmFtZSwgZGVzY3JpcHRvcik7XG59O1xuY29uc3Qgc3RhbmRhcmRRdWVyeSA9IChkZXNjcmlwdG9yLCBlbGVtZW50KSA9PiAoe1xuICAgIGtpbmQ6ICdtZXRob2QnLFxuICAgIHBsYWNlbWVudDogJ3Byb3RvdHlwZScsXG4gICAga2V5OiBlbGVtZW50LmtleSxcbiAgICBkZXNjcmlwdG9yLFxufSk7XG4vKipcbiAqIEJhc2UtaW1wbGVtZW50YXRpb24gb2YgYEBxdWVyeWAgYW5kIGBAcXVlcnlBbGxgIGRlY29yYXRvcnMuXG4gKlxuICogQHBhcmFtIHF1ZXJ5Rm4gZXhlY3R1dGUgYSBgc2VsZWN0b3JgIChpZSwgcXVlcnlTZWxlY3RvciBvciBxdWVyeVNlbGVjdG9yQWxsKVxuICogYWdhaW5zdCBgdGFyZ2V0YC5cbiAqIEBzdXBwcmVzcyB7dmlzaWJpbGl0eX0gVGhlIGRlc2NyaXB0b3IgYWNjZXNzZXMgYW4gaW50ZXJuYWwgZmllbGQgb24gdGhlXG4gKiBlbGVtZW50LlxuICovXG5mdW5jdGlvbiBfcXVlcnkocXVlcnlGbikge1xuICAgIHJldHVybiAoc2VsZWN0b3IpID0+IChwcm90b09yRGVzY3JpcHRvciwgXG4gICAgLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOm5vLWFueSBkZWNvcmF0b3JcbiAgICBuYW1lKSA9PiB7XG4gICAgICAgIGNvbnN0IGRlc2NyaXB0b3IgPSB7XG4gICAgICAgICAgICBnZXQoKSB7XG4gICAgICAgICAgICAgICAgcmV0dXJuIHF1ZXJ5Rm4odGhpcy5yZW5kZXJSb290LCBzZWxlY3Rvcik7XG4gICAgICAgICAgICB9LFxuICAgICAgICAgICAgZW51bWVyYWJsZTogdHJ1ZSxcbiAgICAgICAgICAgIGNvbmZpZ3VyYWJsZTogdHJ1ZSxcbiAgICAgICAgfTtcbiAgICAgICAgcmV0dXJuIChuYW1lICE9PSB1bmRlZmluZWQpID9cbiAgICAgICAgICAgIGxlZ2FjeVF1ZXJ5KGRlc2NyaXB0b3IsIHByb3RvT3JEZXNjcmlwdG9yLCBuYW1lKSA6XG4gICAgICAgICAgICBzdGFuZGFyZFF1ZXJ5KGRlc2NyaXB0b3IsIHByb3RvT3JEZXNjcmlwdG9yKTtcbiAgICB9O1xufVxuY29uc3Qgc3RhbmRhcmRFdmVudE9wdGlvbnMgPSAob3B0aW9ucywgZWxlbWVudCkgPT4ge1xuICAgIHJldHVybiBPYmplY3QuYXNzaWduKHt9LCBlbGVtZW50LCB7IGZpbmlzaGVyKGNsYXp6KSB7XG4gICAgICAgICAgICBPYmplY3QuYXNzaWduKGNsYXp6LnByb3RvdHlwZVtlbGVtZW50LmtleV0sIG9wdGlvbnMpO1xuICAgICAgICB9IH0pO1xufTtcbmNvbnN0IGxlZ2FjeUV2ZW50T3B0aW9ucyA9IFxuLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOm5vLWFueSBsZWdhY3kgZGVjb3JhdG9yXG4ob3B0aW9ucywgcHJvdG8sIG5hbWUpID0+IHtcbiAgICBPYmplY3QuYXNzaWduKHByb3RvW25hbWVdLCBvcHRpb25zKTtcbn07XG4vKipcbiAqIEFkZHMgZXZlbnQgbGlzdGVuZXIgb3B0aW9ucyB0byBhIG1ldGhvZCB1c2VkIGFzIGFuIGV2ZW50IGxpc3RlbmVyIGluIGFcbiAqIGxpdC1odG1sIHRlbXBsYXRlLlxuICpcbiAqIEBwYXJhbSBvcHRpb25zIEFuIG9iamVjdCB0aGF0IHNwZWNpZmlzIGV2ZW50IGxpc3RlbmVyIG9wdGlvbnMgYXMgYWNjZXB0ZWQgYnlcbiAqIGBFdmVudFRhcmdldCNhZGRFdmVudExpc3RlbmVyYCBhbmQgYEV2ZW50VGFyZ2V0I3JlbW92ZUV2ZW50TGlzdGVuZXJgLlxuICpcbiAqIEN1cnJlbnQgYnJvd3NlcnMgc3VwcG9ydCB0aGUgYGNhcHR1cmVgLCBgcGFzc2l2ZWAsIGFuZCBgb25jZWAgb3B0aW9ucy4gU2VlOlxuICogaHR0cHM6Ly9kZXZlbG9wZXIubW96aWxsYS5vcmcvZW4tVVMvZG9jcy9XZWIvQVBJL0V2ZW50VGFyZ2V0L2FkZEV2ZW50TGlzdGVuZXIjUGFyYW1ldGVyc1xuICpcbiAqIEBleGFtcGxlXG4gKlxuICogICAgIGNsYXNzIE15RWxlbWVudCB7XG4gKlxuICogICAgICAgY2xpY2tlZCA9IGZhbHNlO1xuICpcbiAqICAgICAgIHJlbmRlcigpIHtcbiAqICAgICAgICAgcmV0dXJuIGh0bWxgPGRpdiBAY2xpY2s9JHt0aGlzLl9vbkNsaWNrfWA+PGJ1dHRvbj48L2J1dHRvbj48L2Rpdj5gO1xuICogICAgICAgfVxuICpcbiAqICAgICAgIEBldmVudE9wdGlvbnMoe2NhcHR1cmU6IHRydWV9KVxuICogICAgICAgX29uQ2xpY2soZSkge1xuICogICAgICAgICB0aGlzLmNsaWNrZWQgPSB0cnVlO1xuICogICAgICAgfVxuICogICAgIH1cbiAqL1xuZXhwb3J0IGNvbnN0IGV2ZW50T3B0aW9ucyA9IChvcHRpb25zKSA9PiBcbi8vIFJldHVybiB2YWx1ZSB0eXBlZCBhcyBhbnkgdG8gcHJldmVudCBUeXBlU2NyaXB0IGZyb20gY29tcGxhaW5pbmcgdGhhdFxuLy8gc3RhbmRhcmQgZGVjb3JhdG9yIGZ1bmN0aW9uIHNpZ25hdHVyZSBkb2VzIG5vdCBtYXRjaCBUeXBlU2NyaXB0IGRlY29yYXRvclxuLy8gc2lnbmF0dXJlXG4vLyBUT0RPKGtzY2hhYWYpOiB1bmNsZWFyIHdoeSBpdCB3YXMgb25seSBmYWlsaW5nIG9uIHRoaXMgZGVjb3JhdG9yIGFuZCBub3Rcbi8vIHRoZSBvdGhlcnNcbigocHJvdG9PckRlc2NyaXB0b3IsIG5hbWUpID0+IChuYW1lICE9PSB1bmRlZmluZWQpID9cbiAgICBsZWdhY3lFdmVudE9wdGlvbnMob3B0aW9ucywgcHJvdG9PckRlc2NyaXB0b3IsIG5hbWUpIDpcbiAgICBzdGFuZGFyZEV2ZW50T3B0aW9ucyhvcHRpb25zLCBwcm90b09yRGVzY3JpcHRvcikpO1xuLy8jIHNvdXJjZU1hcHBpbmdVUkw9ZGVjb3JhdG9ycy5qcy5tYXAiLCIvKipcbiAqIEBsaWNlbnNlXG4gKiBDb3B5cmlnaHQgKGMpIDIwMTcgVGhlIFBvbHltZXIgUHJvamVjdCBBdXRob3JzLiBBbGwgcmlnaHRzIHJlc2VydmVkLlxuICogVGhpcyBjb2RlIG1heSBvbmx5IGJlIHVzZWQgdW5kZXIgdGhlIEJTRCBzdHlsZSBsaWNlbnNlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vTElDRU5TRS50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgYXV0aG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9BVVRIT1JTLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBjb250cmlidXRvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQ09OVFJJQlVUT1JTLnR4dFxuICogQ29kZSBkaXN0cmlidXRlZCBieSBHb29nbGUgYXMgcGFydCBvZiB0aGUgcG9seW1lciBwcm9qZWN0IGlzIGFsc29cbiAqIHN1YmplY3QgdG8gYW4gYWRkaXRpb25hbCBJUCByaWdodHMgZ3JhbnQgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9QQVRFTlRTLnR4dFxuICovXG4vKipcbiAqIFdoZW4gdXNpbmcgQ2xvc3VyZSBDb21waWxlciwgSlNDb21waWxlcl9yZW5hbWVQcm9wZXJ0eShwcm9wZXJ0eSwgb2JqZWN0KSBpc1xuICogcmVwbGFjZWQgYXQgY29tcGlsZSB0aW1lIGJ5IHRoZSBtdW5nZWQgbmFtZSBmb3Igb2JqZWN0W3Byb3BlcnR5XS4gV2UgY2Fubm90XG4gKiBhbGlhcyB0aGlzIGZ1bmN0aW9uLCBzbyB3ZSBoYXZlIHRvIHVzZSBhIHNtYWxsIHNoaW0gdGhhdCBoYXMgdGhlIHNhbWVcbiAqIGJlaGF2aW9yIHdoZW4gbm90IGNvbXBpbGluZy5cbiAqL1xud2luZG93LkpTQ29tcGlsZXJfcmVuYW1lUHJvcGVydHkgPVxuICAgIChwcm9wLCBfb2JqKSA9PiBwcm9wO1xuZXhwb3J0IGNvbnN0IGRlZmF1bHRDb252ZXJ0ZXIgPSB7XG4gICAgdG9BdHRyaWJ1dGUodmFsdWUsIHR5cGUpIHtcbiAgICAgICAgc3dpdGNoICh0eXBlKSB7XG4gICAgICAgICAgICBjYXNlIEJvb2xlYW46XG4gICAgICAgICAgICAgICAgcmV0dXJuIHZhbHVlID8gJycgOiBudWxsO1xuICAgICAgICAgICAgY2FzZSBPYmplY3Q6XG4gICAgICAgICAgICBjYXNlIEFycmF5OlxuICAgICAgICAgICAgICAgIC8vIGlmIHRoZSB2YWx1ZSBpcyBgbnVsbGAgb3IgYHVuZGVmaW5lZGAgcGFzcyB0aGlzIHRocm91Z2hcbiAgICAgICAgICAgICAgICAvLyB0byBhbGxvdyByZW1vdmluZy9ubyBjaGFuZ2UgYmVoYXZpb3IuXG4gICAgICAgICAgICAgICAgcmV0dXJuIHZhbHVlID09IG51bGwgPyB2YWx1ZSA6IEpTT04uc3RyaW5naWZ5KHZhbHVlKTtcbiAgICAgICAgfVxuICAgICAgICByZXR1cm4gdmFsdWU7XG4gICAgfSxcbiAgICBmcm9tQXR0cmlidXRlKHZhbHVlLCB0eXBlKSB7XG4gICAgICAgIHN3aXRjaCAodHlwZSkge1xuICAgICAgICAgICAgY2FzZSBCb29sZWFuOlxuICAgICAgICAgICAgICAgIHJldHVybiB2YWx1ZSAhPT0gbnVsbDtcbiAgICAgICAgICAgIGNhc2UgTnVtYmVyOlxuICAgICAgICAgICAgICAgIHJldHVybiB2YWx1ZSA9PT0gbnVsbCA/IG51bGwgOiBOdW1iZXIodmFsdWUpO1xuICAgICAgICAgICAgY2FzZSBPYmplY3Q6XG4gICAgICAgICAgICBjYXNlIEFycmF5OlxuICAgICAgICAgICAgICAgIHJldHVybiBKU09OLnBhcnNlKHZhbHVlKTtcbiAgICAgICAgfVxuICAgICAgICByZXR1cm4gdmFsdWU7XG4gICAgfVxufTtcbi8qKlxuICogQ2hhbmdlIGZ1bmN0aW9uIHRoYXQgcmV0dXJucyB0cnVlIGlmIGB2YWx1ZWAgaXMgZGlmZmVyZW50IGZyb20gYG9sZFZhbHVlYC5cbiAqIFRoaXMgbWV0aG9kIGlzIHVzZWQgYXMgdGhlIGRlZmF1bHQgZm9yIGEgcHJvcGVydHkncyBgaGFzQ2hhbmdlZGAgZnVuY3Rpb24uXG4gKi9cbmV4cG9ydCBjb25zdCBub3RFcXVhbCA9ICh2YWx1ZSwgb2xkKSA9PiB7XG4gICAgLy8gVGhpcyBlbnN1cmVzIChvbGQ9PU5hTiwgdmFsdWU9PU5hTikgYWx3YXlzIHJldHVybnMgZmFsc2VcbiAgICByZXR1cm4gb2xkICE9PSB2YWx1ZSAmJiAob2xkID09PSBvbGQgfHwgdmFsdWUgPT09IHZhbHVlKTtcbn07XG5jb25zdCBkZWZhdWx0UHJvcGVydHlEZWNsYXJhdGlvbiA9IHtcbiAgICBhdHRyaWJ1dGU6IHRydWUsXG4gICAgdHlwZTogU3RyaW5nLFxuICAgIGNvbnZlcnRlcjogZGVmYXVsdENvbnZlcnRlcixcbiAgICByZWZsZWN0OiBmYWxzZSxcbiAgICBoYXNDaGFuZ2VkOiBub3RFcXVhbFxufTtcbmNvbnN0IG1pY3JvdGFza1Byb21pc2UgPSBQcm9taXNlLnJlc29sdmUodHJ1ZSk7XG5jb25zdCBTVEFURV9IQVNfVVBEQVRFRCA9IDE7XG5jb25zdCBTVEFURV9VUERBVEVfUkVRVUVTVEVEID0gMSA8PCAyO1xuY29uc3QgU1RBVEVfSVNfUkVGTEVDVElOR19UT19BVFRSSUJVVEUgPSAxIDw8IDM7XG5jb25zdCBTVEFURV9JU19SRUZMRUNUSU5HX1RPX1BST1BFUlRZID0gMSA8PCA0O1xuY29uc3QgU1RBVEVfSEFTX0NPTk5FQ1RFRCA9IDEgPDwgNTtcbi8qKlxuICogQmFzZSBlbGVtZW50IGNsYXNzIHdoaWNoIG1hbmFnZXMgZWxlbWVudCBwcm9wZXJ0aWVzIGFuZCBhdHRyaWJ1dGVzLiBXaGVuXG4gKiBwcm9wZXJ0aWVzIGNoYW5nZSwgdGhlIGB1cGRhdGVgIG1ldGhvZCBpcyBhc3luY2hyb25vdXNseSBjYWxsZWQuIFRoaXMgbWV0aG9kXG4gKiBzaG91bGQgYmUgc3VwcGxpZWQgYnkgc3ViY2xhc3NlcnMgdG8gcmVuZGVyIHVwZGF0ZXMgYXMgZGVzaXJlZC5cbiAqL1xuZXhwb3J0IGNsYXNzIFVwZGF0aW5nRWxlbWVudCBleHRlbmRzIEhUTUxFbGVtZW50IHtcbiAgICBjb25zdHJ1Y3RvcigpIHtcbiAgICAgICAgc3VwZXIoKTtcbiAgICAgICAgdGhpcy5fdXBkYXRlU3RhdGUgPSAwO1xuICAgICAgICB0aGlzLl9pbnN0YW5jZVByb3BlcnRpZXMgPSB1bmRlZmluZWQ7XG4gICAgICAgIHRoaXMuX3VwZGF0ZVByb21pc2UgPSBtaWNyb3Rhc2tQcm9taXNlO1xuICAgICAgICB0aGlzLl9oYXNDb25uZWN0ZWRSZXNvbHZlciA9IHVuZGVmaW5lZDtcbiAgICAgICAgLyoqXG4gICAgICAgICAqIE1hcCB3aXRoIGtleXMgZm9yIGFueSBwcm9wZXJ0aWVzIHRoYXQgaGF2ZSBjaGFuZ2VkIHNpbmNlIHRoZSBsYXN0XG4gICAgICAgICAqIHVwZGF0ZSBjeWNsZSB3aXRoIHByZXZpb3VzIHZhbHVlcy5cbiAgICAgICAgICovXG4gICAgICAgIHRoaXMuX2NoYW5nZWRQcm9wZXJ0aWVzID0gbmV3IE1hcCgpO1xuICAgICAgICAvKipcbiAgICAgICAgICogTWFwIHdpdGgga2V5cyBvZiBwcm9wZXJ0aWVzIHRoYXQgc2hvdWxkIGJlIHJlZmxlY3RlZCB3aGVuIHVwZGF0ZWQuXG4gICAgICAgICAqL1xuICAgICAgICB0aGlzLl9yZWZsZWN0aW5nUHJvcGVydGllcyA9IHVuZGVmaW5lZDtcbiAgICAgICAgdGhpcy5pbml0aWFsaXplKCk7XG4gICAgfVxuICAgIC8qKlxuICAgICAqIFJldHVybnMgYSBsaXN0IG9mIGF0dHJpYnV0ZXMgY29ycmVzcG9uZGluZyB0byB0aGUgcmVnaXN0ZXJlZCBwcm9wZXJ0aWVzLlxuICAgICAqIEBub2NvbGxhcHNlXG4gICAgICovXG4gICAgc3RhdGljIGdldCBvYnNlcnZlZEF0dHJpYnV0ZXMoKSB7XG4gICAgICAgIC8vIG5vdGU6IHBpZ2d5IGJhY2tpbmcgb24gdGhpcyB0byBlbnN1cmUgd2UncmUgZmluYWxpemVkLlxuICAgICAgICB0aGlzLmZpbmFsaXplKCk7XG4gICAgICAgIGNvbnN0IGF0dHJpYnV0ZXMgPSBbXTtcbiAgICAgICAgLy8gVXNlIGZvckVhY2ggc28gdGhpcyB3b3JrcyBldmVuIGlmIGZvci9vZiBsb29wcyBhcmUgY29tcGlsZWQgdG8gZm9yIGxvb3BzXG4gICAgICAgIC8vIGV4cGVjdGluZyBhcnJheXNcbiAgICAgICAgdGhpcy5fY2xhc3NQcm9wZXJ0aWVzLmZvckVhY2goKHYsIHApID0+IHtcbiAgICAgICAgICAgIGNvbnN0IGF0dHIgPSB0aGlzLl9hdHRyaWJ1dGVOYW1lRm9yUHJvcGVydHkocCwgdik7XG4gICAgICAgICAgICBpZiAoYXR0ciAhPT0gdW5kZWZpbmVkKSB7XG4gICAgICAgICAgICAgICAgdGhpcy5fYXR0cmlidXRlVG9Qcm9wZXJ0eU1hcC5zZXQoYXR0ciwgcCk7XG4gICAgICAgICAgICAgICAgYXR0cmlidXRlcy5wdXNoKGF0dHIpO1xuICAgICAgICAgICAgfVxuICAgICAgICB9KTtcbiAgICAgICAgcmV0dXJuIGF0dHJpYnV0ZXM7XG4gICAgfVxuICAgIC8qKlxuICAgICAqIEVuc3VyZXMgdGhlIHByaXZhdGUgYF9jbGFzc1Byb3BlcnRpZXNgIHByb3BlcnR5IG1ldGFkYXRhIGlzIGNyZWF0ZWQuXG4gICAgICogSW4gYWRkaXRpb24gdG8gYGZpbmFsaXplYCB0aGlzIGlzIGFsc28gY2FsbGVkIGluIGBjcmVhdGVQcm9wZXJ0eWAgdG9cbiAgICAgKiBlbnN1cmUgdGhlIGBAcHJvcGVydHlgIGRlY29yYXRvciBjYW4gYWRkIHByb3BlcnR5IG1ldGFkYXRhLlxuICAgICAqL1xuICAgIC8qKiBAbm9jb2xsYXBzZSAqL1xuICAgIHN0YXRpYyBfZW5zdXJlQ2xhc3NQcm9wZXJ0aWVzKCkge1xuICAgICAgICAvLyBlbnN1cmUgcHJpdmF0ZSBzdG9yYWdlIGZvciBwcm9wZXJ0eSBkZWNsYXJhdGlvbnMuXG4gICAgICAgIGlmICghdGhpcy5oYXNPd25Qcm9wZXJ0eShKU0NvbXBpbGVyX3JlbmFtZVByb3BlcnR5KCdfY2xhc3NQcm9wZXJ0aWVzJywgdGhpcykpKSB7XG4gICAgICAgICAgICB0aGlzLl9jbGFzc1Byb3BlcnRpZXMgPSBuZXcgTWFwKCk7XG4gICAgICAgICAgICAvLyBOT1RFOiBXb3JrYXJvdW5kIElFMTEgbm90IHN1cHBvcnRpbmcgTWFwIGNvbnN0cnVjdG9yIGFyZ3VtZW50LlxuICAgICAgICAgICAgY29uc3Qgc3VwZXJQcm9wZXJ0aWVzID0gT2JqZWN0LmdldFByb3RvdHlwZU9mKHRoaXMpLl9jbGFzc1Byb3BlcnRpZXM7XG4gICAgICAgICAgICBpZiAoc3VwZXJQcm9wZXJ0aWVzICE9PSB1bmRlZmluZWQpIHtcbiAgICAgICAgICAgICAgICBzdXBlclByb3BlcnRpZXMuZm9yRWFjaCgodiwgaykgPT4gdGhpcy5fY2xhc3NQcm9wZXJ0aWVzLnNldChrLCB2KSk7XG4gICAgICAgICAgICB9XG4gICAgICAgIH1cbiAgICB9XG4gICAgLyoqXG4gICAgICogQ3JlYXRlcyBhIHByb3BlcnR5IGFjY2Vzc29yIG9uIHRoZSBlbGVtZW50IHByb3RvdHlwZSBpZiBvbmUgZG9lcyBub3QgZXhpc3QuXG4gICAgICogVGhlIHByb3BlcnR5IHNldHRlciBjYWxscyB0aGUgcHJvcGVydHkncyBgaGFzQ2hhbmdlZGAgcHJvcGVydHkgb3B0aW9uXG4gICAgICogb3IgdXNlcyBhIHN0cmljdCBpZGVudGl0eSBjaGVjayB0byBkZXRlcm1pbmUgd2hldGhlciBvciBub3QgdG8gcmVxdWVzdFxuICAgICAqIGFuIHVwZGF0ZS5cbiAgICAgKiBAbm9jb2xsYXBzZVxuICAgICAqL1xuICAgIHN0YXRpYyBjcmVhdGVQcm9wZXJ0eShuYW1lLCBvcHRpb25zID0gZGVmYXVsdFByb3BlcnR5RGVjbGFyYXRpb24pIHtcbiAgICAgICAgLy8gTm90ZSwgc2luY2UgdGhpcyBjYW4gYmUgY2FsbGVkIGJ5IHRoZSBgQHByb3BlcnR5YCBkZWNvcmF0b3Igd2hpY2hcbiAgICAgICAgLy8gaXMgY2FsbGVkIGJlZm9yZSBgZmluYWxpemVgLCB3ZSBlbnN1cmUgc3RvcmFnZSBleGlzdHMgZm9yIHByb3BlcnR5XG4gICAgICAgIC8vIG1ldGFkYXRhLlxuICAgICAgICB0aGlzLl9lbnN1cmVDbGFzc1Byb3BlcnRpZXMoKTtcbiAgICAgICAgdGhpcy5fY2xhc3NQcm9wZXJ0aWVzLnNldChuYW1lLCBvcHRpb25zKTtcbiAgICAgICAgLy8gRG8gbm90IGdlbmVyYXRlIGFuIGFjY2Vzc29yIGlmIHRoZSBwcm90b3R5cGUgYWxyZWFkeSBoYXMgb25lLCBzaW5jZVxuICAgICAgICAvLyBpdCB3b3VsZCBiZSBsb3N0IG90aGVyd2lzZSBhbmQgdGhhdCB3b3VsZCBuZXZlciBiZSB0aGUgdXNlcidzIGludGVudGlvbjtcbiAgICAgICAgLy8gSW5zdGVhZCwgd2UgZXhwZWN0IHVzZXJzIHRvIGNhbGwgYHJlcXVlc3RVcGRhdGVgIHRoZW1zZWx2ZXMgZnJvbVxuICAgICAgICAvLyB1c2VyLWRlZmluZWQgYWNjZXNzb3JzLiBOb3RlIHRoYXQgaWYgdGhlIHN1cGVyIGhhcyBhbiBhY2Nlc3NvciB3ZSB3aWxsXG4gICAgICAgIC8vIHN0aWxsIG92ZXJ3cml0ZSBpdFxuICAgICAgICBpZiAob3B0aW9ucy5ub0FjY2Vzc29yIHx8IHRoaXMucHJvdG90eXBlLmhhc093blByb3BlcnR5KG5hbWUpKSB7XG4gICAgICAgICAgICByZXR1cm47XG4gICAgICAgIH1cbiAgICAgICAgY29uc3Qga2V5ID0gdHlwZW9mIG5hbWUgPT09ICdzeW1ib2wnID8gU3ltYm9sKCkgOiBgX18ke25hbWV9YDtcbiAgICAgICAgT2JqZWN0LmRlZmluZVByb3BlcnR5KHRoaXMucHJvdG90eXBlLCBuYW1lLCB7XG4gICAgICAgICAgICAvLyB0c2xpbnQ6ZGlzYWJsZS1uZXh0LWxpbmU6bm8tYW55IG5vIHN5bWJvbCBpbiBpbmRleFxuICAgICAgICAgICAgZ2V0KCkge1xuICAgICAgICAgICAgICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTpuby1hbnkgbm8gc3ltYm9sIGluIGluZGV4XG4gICAgICAgICAgICAgICAgcmV0dXJuIHRoaXNba2V5XTtcbiAgICAgICAgICAgIH0sXG4gICAgICAgICAgICBzZXQodmFsdWUpIHtcbiAgICAgICAgICAgICAgICAvLyB0c2xpbnQ6ZGlzYWJsZS1uZXh0LWxpbmU6bm8tYW55IG5vIHN5bWJvbCBpbiBpbmRleFxuICAgICAgICAgICAgICAgIGNvbnN0IG9sZFZhbHVlID0gdGhpc1tuYW1lXTtcbiAgICAgICAgICAgICAgICAvLyB0c2xpbnQ6ZGlzYWJsZS1uZXh0LWxpbmU6bm8tYW55IG5vIHN5bWJvbCBpbiBpbmRleFxuICAgICAgICAgICAgICAgIHRoaXNba2V5XSA9IHZhbHVlO1xuICAgICAgICAgICAgICAgIHRoaXMucmVxdWVzdFVwZGF0ZShuYW1lLCBvbGRWYWx1ZSk7XG4gICAgICAgICAgICB9LFxuICAgICAgICAgICAgY29uZmlndXJhYmxlOiB0cnVlLFxuICAgICAgICAgICAgZW51bWVyYWJsZTogdHJ1ZVxuICAgICAgICB9KTtcbiAgICB9XG4gICAgLyoqXG4gICAgICogQ3JlYXRlcyBwcm9wZXJ0eSBhY2Nlc3NvcnMgZm9yIHJlZ2lzdGVyZWQgcHJvcGVydGllcyBhbmQgZW5zdXJlc1xuICAgICAqIGFueSBzdXBlcmNsYXNzZXMgYXJlIGFsc28gZmluYWxpemVkLlxuICAgICAqIEBub2NvbGxhcHNlXG4gICAgICovXG4gICAgc3RhdGljIGZpbmFsaXplKCkge1xuICAgICAgICBpZiAodGhpcy5oYXNPd25Qcm9wZXJ0eShKU0NvbXBpbGVyX3JlbmFtZVByb3BlcnR5KCdmaW5hbGl6ZWQnLCB0aGlzKSkgJiZcbiAgICAgICAgICAgIHRoaXMuZmluYWxpemVkKSB7XG4gICAgICAgICAgICByZXR1cm47XG4gICAgICAgIH1cbiAgICAgICAgLy8gZmluYWxpemUgYW55IHN1cGVyY2xhc3Nlc1xuICAgICAgICBjb25zdCBzdXBlckN0b3IgPSBPYmplY3QuZ2V0UHJvdG90eXBlT2YodGhpcyk7XG4gICAgICAgIGlmICh0eXBlb2Ygc3VwZXJDdG9yLmZpbmFsaXplID09PSAnZnVuY3Rpb24nKSB7XG4gICAgICAgICAgICBzdXBlckN0b3IuZmluYWxpemUoKTtcbiAgICAgICAgfVxuICAgICAgICB0aGlzLmZpbmFsaXplZCA9IHRydWU7XG4gICAgICAgIHRoaXMuX2Vuc3VyZUNsYXNzUHJvcGVydGllcygpO1xuICAgICAgICAvLyBpbml0aWFsaXplIE1hcCBwb3B1bGF0ZWQgaW4gb2JzZXJ2ZWRBdHRyaWJ1dGVzXG4gICAgICAgIHRoaXMuX2F0dHJpYnV0ZVRvUHJvcGVydHlNYXAgPSBuZXcgTWFwKCk7XG4gICAgICAgIC8vIG1ha2UgYW55IHByb3BlcnRpZXNcbiAgICAgICAgLy8gTm90ZSwgb25seSBwcm9jZXNzIFwib3duXCIgcHJvcGVydGllcyBzaW5jZSB0aGlzIGVsZW1lbnQgd2lsbCBpbmhlcml0XG4gICAgICAgIC8vIGFueSBwcm9wZXJ0aWVzIGRlZmluZWQgb24gdGhlIHN1cGVyQ2xhc3MsIGFuZCBmaW5hbGl6YXRpb24gZW5zdXJlc1xuICAgICAgICAvLyB0aGUgZW50aXJlIHByb3RvdHlwZSBjaGFpbiBpcyBmaW5hbGl6ZWQuXG4gICAgICAgIGlmICh0aGlzLmhhc093blByb3BlcnR5KEpTQ29tcGlsZXJfcmVuYW1lUHJvcGVydHkoJ3Byb3BlcnRpZXMnLCB0aGlzKSkpIHtcbiAgICAgICAgICAgIGNvbnN0IHByb3BzID0gdGhpcy5wcm9wZXJ0aWVzO1xuICAgICAgICAgICAgLy8gc3VwcG9ydCBzeW1ib2xzIGluIHByb3BlcnRpZXMgKElFMTEgZG9lcyBub3Qgc3VwcG9ydCB0aGlzKVxuICAgICAgICAgICAgY29uc3QgcHJvcEtleXMgPSBbXG4gICAgICAgICAgICAgICAgLi4uT2JqZWN0LmdldE93blByb3BlcnR5TmFtZXMocHJvcHMpLFxuICAgICAgICAgICAgICAgIC4uLih0eXBlb2YgT2JqZWN0LmdldE93blByb3BlcnR5U3ltYm9scyA9PT0gJ2Z1bmN0aW9uJykgP1xuICAgICAgICAgICAgICAgICAgICBPYmplY3QuZ2V0T3duUHJvcGVydHlTeW1ib2xzKHByb3BzKSA6XG4gICAgICAgICAgICAgICAgICAgIFtdXG4gICAgICAgICAgICBdO1xuICAgICAgICAgICAgLy8gVGhpcyBmb3Ivb2YgaXMgb2sgYmVjYXVzZSBwcm9wS2V5cyBpcyBhbiBhcnJheVxuICAgICAgICAgICAgZm9yIChjb25zdCBwIG9mIHByb3BLZXlzKSB7XG4gICAgICAgICAgICAgICAgLy8gbm90ZSwgdXNlIG9mIGBhbnlgIGlzIGR1ZSB0byBUeXBlU3JpcHQgbGFjayBvZiBzdXBwb3J0IGZvciBzeW1ib2wgaW5cbiAgICAgICAgICAgICAgICAvLyBpbmRleCB0eXBlc1xuICAgICAgICAgICAgICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTpuby1hbnkgbm8gc3ltYm9sIGluIGluZGV4XG4gICAgICAgICAgICAgICAgdGhpcy5jcmVhdGVQcm9wZXJ0eShwLCBwcm9wc1twXSk7XG4gICAgICAgICAgICB9XG4gICAgICAgIH1cbiAgICB9XG4gICAgLyoqXG4gICAgICogUmV0dXJucyB0aGUgcHJvcGVydHkgbmFtZSBmb3IgdGhlIGdpdmVuIGF0dHJpYnV0ZSBgbmFtZWAuXG4gICAgICogQG5vY29sbGFwc2VcbiAgICAgKi9cbiAgICBzdGF0aWMgX2F0dHJpYnV0ZU5hbWVGb3JQcm9wZXJ0eShuYW1lLCBvcHRpb25zKSB7XG4gICAgICAgIGNvbnN0IGF0dHJpYnV0ZSA9IG9wdGlvbnMuYXR0cmlidXRlO1xuICAgICAgICByZXR1cm4gYXR0cmlidXRlID09PSBmYWxzZSA/XG4gICAgICAgICAgICB1bmRlZmluZWQgOlxuICAgICAgICAgICAgKHR5cGVvZiBhdHRyaWJ1dGUgPT09ICdzdHJpbmcnID9cbiAgICAgICAgICAgICAgICBhdHRyaWJ1dGUgOlxuICAgICAgICAgICAgICAgICh0eXBlb2YgbmFtZSA9PT0gJ3N0cmluZycgPyBuYW1lLnRvTG93ZXJDYXNlKCkgOiB1bmRlZmluZWQpKTtcbiAgICB9XG4gICAgLyoqXG4gICAgICogUmV0dXJucyB0cnVlIGlmIGEgcHJvcGVydHkgc2hvdWxkIHJlcXVlc3QgYW4gdXBkYXRlLlxuICAgICAqIENhbGxlZCB3aGVuIGEgcHJvcGVydHkgdmFsdWUgaXMgc2V0IGFuZCB1c2VzIHRoZSBgaGFzQ2hhbmdlZGBcbiAgICAgKiBvcHRpb24gZm9yIHRoZSBwcm9wZXJ0eSBpZiBwcmVzZW50IG9yIGEgc3RyaWN0IGlkZW50aXR5IGNoZWNrLlxuICAgICAqIEBub2NvbGxhcHNlXG4gICAgICovXG4gICAgc3RhdGljIF92YWx1ZUhhc0NoYW5nZWQodmFsdWUsIG9sZCwgaGFzQ2hhbmdlZCA9IG5vdEVxdWFsKSB7XG4gICAgICAgIHJldHVybiBoYXNDaGFuZ2VkKHZhbHVlLCBvbGQpO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBSZXR1cm5zIHRoZSBwcm9wZXJ0eSB2YWx1ZSBmb3IgdGhlIGdpdmVuIGF0dHJpYnV0ZSB2YWx1ZS5cbiAgICAgKiBDYWxsZWQgdmlhIHRoZSBgYXR0cmlidXRlQ2hhbmdlZENhbGxiYWNrYCBhbmQgdXNlcyB0aGUgcHJvcGVydHknc1xuICAgICAqIGBjb252ZXJ0ZXJgIG9yIGBjb252ZXJ0ZXIuZnJvbUF0dHJpYnV0ZWAgcHJvcGVydHkgb3B0aW9uLlxuICAgICAqIEBub2NvbGxhcHNlXG4gICAgICovXG4gICAgc3RhdGljIF9wcm9wZXJ0eVZhbHVlRnJvbUF0dHJpYnV0ZSh2YWx1ZSwgb3B0aW9ucykge1xuICAgICAgICBjb25zdCB0eXBlID0gb3B0aW9ucy50eXBlO1xuICAgICAgICBjb25zdCBjb252ZXJ0ZXIgPSBvcHRpb25zLmNvbnZlcnRlciB8fCBkZWZhdWx0Q29udmVydGVyO1xuICAgICAgICBjb25zdCBmcm9tQXR0cmlidXRlID0gKHR5cGVvZiBjb252ZXJ0ZXIgPT09ICdmdW5jdGlvbicgPyBjb252ZXJ0ZXIgOiBjb252ZXJ0ZXIuZnJvbUF0dHJpYnV0ZSk7XG4gICAgICAgIHJldHVybiBmcm9tQXR0cmlidXRlID8gZnJvbUF0dHJpYnV0ZSh2YWx1ZSwgdHlwZSkgOiB2YWx1ZTtcbiAgICB9XG4gICAgLyoqXG4gICAgICogUmV0dXJucyB0aGUgYXR0cmlidXRlIHZhbHVlIGZvciB0aGUgZ2l2ZW4gcHJvcGVydHkgdmFsdWUuIElmIHRoaXNcbiAgICAgKiByZXR1cm5zIHVuZGVmaW5lZCwgdGhlIHByb3BlcnR5IHdpbGwgKm5vdCogYmUgcmVmbGVjdGVkIHRvIGFuIGF0dHJpYnV0ZS5cbiAgICAgKiBJZiB0aGlzIHJldHVybnMgbnVsbCwgdGhlIGF0dHJpYnV0ZSB3aWxsIGJlIHJlbW92ZWQsIG90aGVyd2lzZSB0aGVcbiAgICAgKiBhdHRyaWJ1dGUgd2lsbCBiZSBzZXQgdG8gdGhlIHZhbHVlLlxuICAgICAqIFRoaXMgdXNlcyB0aGUgcHJvcGVydHkncyBgcmVmbGVjdGAgYW5kIGB0eXBlLnRvQXR0cmlidXRlYCBwcm9wZXJ0eSBvcHRpb25zLlxuICAgICAqIEBub2NvbGxhcHNlXG4gICAgICovXG4gICAgc3RhdGljIF9wcm9wZXJ0eVZhbHVlVG9BdHRyaWJ1dGUodmFsdWUsIG9wdGlvbnMpIHtcbiAgICAgICAgaWYgKG9wdGlvbnMucmVmbGVjdCA9PT0gdW5kZWZpbmVkKSB7XG4gICAgICAgICAgICByZXR1cm47XG4gICAgICAgIH1cbiAgICAgICAgY29uc3QgdHlwZSA9IG9wdGlvbnMudHlwZTtcbiAgICAgICAgY29uc3QgY29udmVydGVyID0gb3B0aW9ucy5jb252ZXJ0ZXI7XG4gICAgICAgIGNvbnN0IHRvQXR0cmlidXRlID0gY29udmVydGVyICYmIGNvbnZlcnRlci50b0F0dHJpYnV0ZSB8fFxuICAgICAgICAgICAgZGVmYXVsdENvbnZlcnRlci50b0F0dHJpYnV0ZTtcbiAgICAgICAgcmV0dXJuIHRvQXR0cmlidXRlKHZhbHVlLCB0eXBlKTtcbiAgICB9XG4gICAgLyoqXG4gICAgICogUGVyZm9ybXMgZWxlbWVudCBpbml0aWFsaXphdGlvbi4gQnkgZGVmYXVsdCBjYXB0dXJlcyBhbnkgcHJlLXNldCB2YWx1ZXMgZm9yXG4gICAgICogcmVnaXN0ZXJlZCBwcm9wZXJ0aWVzLlxuICAgICAqL1xuICAgIGluaXRpYWxpemUoKSB7XG4gICAgICAgIHRoaXMuX3NhdmVJbnN0YW5jZVByb3BlcnRpZXMoKTtcbiAgICB9XG4gICAgLyoqXG4gICAgICogRml4ZXMgYW55IHByb3BlcnRpZXMgc2V0IG9uIHRoZSBpbnN0YW5jZSBiZWZvcmUgdXBncmFkZSB0aW1lLlxuICAgICAqIE90aGVyd2lzZSB0aGVzZSB3b3VsZCBzaGFkb3cgdGhlIGFjY2Vzc29yIGFuZCBicmVhayB0aGVzZSBwcm9wZXJ0aWVzLlxuICAgICAqIFRoZSBwcm9wZXJ0aWVzIGFyZSBzdG9yZWQgaW4gYSBNYXAgd2hpY2ggaXMgcGxheWVkIGJhY2sgYWZ0ZXIgdGhlXG4gICAgICogY29uc3RydWN0b3IgcnVucy4gTm90ZSwgb24gdmVyeSBvbGQgdmVyc2lvbnMgb2YgU2FmYXJpICg8PTkpIG9yIENocm9tZVxuICAgICAqICg8PTQxKSwgcHJvcGVydGllcyBjcmVhdGVkIGZvciBuYXRpdmUgcGxhdGZvcm0gcHJvcGVydGllcyBsaWtlIChgaWRgIG9yXG4gICAgICogYG5hbWVgKSBtYXkgbm90IGhhdmUgZGVmYXVsdCB2YWx1ZXMgc2V0IGluIHRoZSBlbGVtZW50IGNvbnN0cnVjdG9yLiBPblxuICAgICAqIHRoZXNlIGJyb3dzZXJzIG5hdGl2ZSBwcm9wZXJ0aWVzIGFwcGVhciBvbiBpbnN0YW5jZXMgYW5kIHRoZXJlZm9yZSB0aGVpclxuICAgICAqIGRlZmF1bHQgdmFsdWUgd2lsbCBvdmVyd3JpdGUgYW55IGVsZW1lbnQgZGVmYXVsdCAoZS5nLiBpZiB0aGUgZWxlbWVudCBzZXRzXG4gICAgICogdGhpcy5pZCA9ICdpZCcgaW4gdGhlIGNvbnN0cnVjdG9yLCB0aGUgJ2lkJyB3aWxsIGJlY29tZSAnJyBzaW5jZSB0aGlzIGlzXG4gICAgICogdGhlIG5hdGl2ZSBwbGF0Zm9ybSBkZWZhdWx0KS5cbiAgICAgKi9cbiAgICBfc2F2ZUluc3RhbmNlUHJvcGVydGllcygpIHtcbiAgICAgICAgLy8gVXNlIGZvckVhY2ggc28gdGhpcyB3b3JrcyBldmVuIGlmIGZvci9vZiBsb29wcyBhcmUgY29tcGlsZWQgdG8gZm9yIGxvb3BzXG4gICAgICAgIC8vIGV4cGVjdGluZyBhcnJheXNcbiAgICAgICAgdGhpcy5jb25zdHJ1Y3RvclxuICAgICAgICAgICAgLl9jbGFzc1Byb3BlcnRpZXMuZm9yRWFjaCgoX3YsIHApID0+IHtcbiAgICAgICAgICAgIGlmICh0aGlzLmhhc093blByb3BlcnR5KHApKSB7XG4gICAgICAgICAgICAgICAgY29uc3QgdmFsdWUgPSB0aGlzW3BdO1xuICAgICAgICAgICAgICAgIGRlbGV0ZSB0aGlzW3BdO1xuICAgICAgICAgICAgICAgIGlmICghdGhpcy5faW5zdGFuY2VQcm9wZXJ0aWVzKSB7XG4gICAgICAgICAgICAgICAgICAgIHRoaXMuX2luc3RhbmNlUHJvcGVydGllcyA9IG5ldyBNYXAoKTtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgdGhpcy5faW5zdGFuY2VQcm9wZXJ0aWVzLnNldChwLCB2YWx1ZSk7XG4gICAgICAgICAgICB9XG4gICAgICAgIH0pO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBBcHBsaWVzIHByZXZpb3VzbHkgc2F2ZWQgaW5zdGFuY2UgcHJvcGVydGllcy5cbiAgICAgKi9cbiAgICBfYXBwbHlJbnN0YW5jZVByb3BlcnRpZXMoKSB7XG4gICAgICAgIC8vIFVzZSBmb3JFYWNoIHNvIHRoaXMgd29ya3MgZXZlbiBpZiBmb3Ivb2YgbG9vcHMgYXJlIGNvbXBpbGVkIHRvIGZvciBsb29wc1xuICAgICAgICAvLyBleHBlY3RpbmcgYXJyYXlzXG4gICAgICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTpuby1hbnlcbiAgICAgICAgdGhpcy5faW5zdGFuY2VQcm9wZXJ0aWVzLmZvckVhY2goKHYsIHApID0+IHRoaXNbcF0gPSB2KTtcbiAgICAgICAgdGhpcy5faW5zdGFuY2VQcm9wZXJ0aWVzID0gdW5kZWZpbmVkO1xuICAgIH1cbiAgICBjb25uZWN0ZWRDYWxsYmFjaygpIHtcbiAgICAgICAgdGhpcy5fdXBkYXRlU3RhdGUgPSB0aGlzLl91cGRhdGVTdGF0ZSB8IFNUQVRFX0hBU19DT05ORUNURUQ7XG4gICAgICAgIC8vIEVuc3VyZSBjb25uZWN0aW9uIHRyaWdnZXJzIGFuIHVwZGF0ZS4gVXBkYXRlcyBjYW5ub3QgY29tcGxldGUgYmVmb3JlXG4gICAgICAgIC8vIGNvbm5lY3Rpb24gYW5kIGlmIG9uZSBpcyBwZW5kaW5nIGNvbm5lY3Rpb24gdGhlIGBfaGFzQ29ubmVjdGlvblJlc29sdmVyYFxuICAgICAgICAvLyB3aWxsIGV4aXN0LiBJZiBzbywgcmVzb2x2ZSBpdCB0byBjb21wbGV0ZSB0aGUgdXBkYXRlLCBvdGhlcndpc2VcbiAgICAgICAgLy8gcmVxdWVzdFVwZGF0ZS5cbiAgICAgICAgaWYgKHRoaXMuX2hhc0Nvbm5lY3RlZFJlc29sdmVyKSB7XG4gICAgICAgICAgICB0aGlzLl9oYXNDb25uZWN0ZWRSZXNvbHZlcigpO1xuICAgICAgICAgICAgdGhpcy5faGFzQ29ubmVjdGVkUmVzb2x2ZXIgPSB1bmRlZmluZWQ7XG4gICAgICAgIH1cbiAgICAgICAgZWxzZSB7XG4gICAgICAgICAgICB0aGlzLnJlcXVlc3RVcGRhdGUoKTtcbiAgICAgICAgfVxuICAgIH1cbiAgICAvKipcbiAgICAgKiBBbGxvd3MgZm9yIGBzdXBlci5kaXNjb25uZWN0ZWRDYWxsYmFjaygpYCBpbiBleHRlbnNpb25zIHdoaWxlXG4gICAgICogcmVzZXJ2aW5nIHRoZSBwb3NzaWJpbGl0eSBvZiBtYWtpbmcgbm9uLWJyZWFraW5nIGZlYXR1cmUgYWRkaXRpb25zXG4gICAgICogd2hlbiBkaXNjb25uZWN0aW5nIGF0IHNvbWUgcG9pbnQgaW4gdGhlIGZ1dHVyZS5cbiAgICAgKi9cbiAgICBkaXNjb25uZWN0ZWRDYWxsYmFjaygpIHtcbiAgICB9XG4gICAgLyoqXG4gICAgICogU3luY2hyb25pemVzIHByb3BlcnR5IHZhbHVlcyB3aGVuIGF0dHJpYnV0ZXMgY2hhbmdlLlxuICAgICAqL1xuICAgIGF0dHJpYnV0ZUNoYW5nZWRDYWxsYmFjayhuYW1lLCBvbGQsIHZhbHVlKSB7XG4gICAgICAgIGlmIChvbGQgIT09IHZhbHVlKSB7XG4gICAgICAgICAgICB0aGlzLl9hdHRyaWJ1dGVUb1Byb3BlcnR5KG5hbWUsIHZhbHVlKTtcbiAgICAgICAgfVxuICAgIH1cbiAgICBfcHJvcGVydHlUb0F0dHJpYnV0ZShuYW1lLCB2YWx1ZSwgb3B0aW9ucyA9IGRlZmF1bHRQcm9wZXJ0eURlY2xhcmF0aW9uKSB7XG4gICAgICAgIGNvbnN0IGN0b3IgPSB0aGlzLmNvbnN0cnVjdG9yO1xuICAgICAgICBjb25zdCBhdHRyID0gY3Rvci5fYXR0cmlidXRlTmFtZUZvclByb3BlcnR5KG5hbWUsIG9wdGlvbnMpO1xuICAgICAgICBpZiAoYXR0ciAhPT0gdW5kZWZpbmVkKSB7XG4gICAgICAgICAgICBjb25zdCBhdHRyVmFsdWUgPSBjdG9yLl9wcm9wZXJ0eVZhbHVlVG9BdHRyaWJ1dGUodmFsdWUsIG9wdGlvbnMpO1xuICAgICAgICAgICAgLy8gYW4gdW5kZWZpbmVkIHZhbHVlIGRvZXMgbm90IGNoYW5nZSB0aGUgYXR0cmlidXRlLlxuICAgICAgICAgICAgaWYgKGF0dHJWYWx1ZSA9PT0gdW5kZWZpbmVkKSB7XG4gICAgICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICAgICAgfVxuICAgICAgICAgICAgLy8gVHJhY2sgaWYgdGhlIHByb3BlcnR5IGlzIGJlaW5nIHJlZmxlY3RlZCB0byBhdm9pZFxuICAgICAgICAgICAgLy8gc2V0dGluZyB0aGUgcHJvcGVydHkgYWdhaW4gdmlhIGBhdHRyaWJ1dGVDaGFuZ2VkQ2FsbGJhY2tgLiBOb3RlOlxuICAgICAgICAgICAgLy8gMS4gdGhpcyB0YWtlcyBhZHZhbnRhZ2Ugb2YgdGhlIGZhY3QgdGhhdCB0aGUgY2FsbGJhY2sgaXMgc3luY2hyb25vdXMuXG4gICAgICAgICAgICAvLyAyLiB3aWxsIGJlaGF2ZSBpbmNvcnJlY3RseSBpZiBtdWx0aXBsZSBhdHRyaWJ1dGVzIGFyZSBpbiB0aGUgcmVhY3Rpb25cbiAgICAgICAgICAgIC8vIHN0YWNrIGF0IHRpbWUgb2YgY2FsbGluZy4gSG93ZXZlciwgc2luY2Ugd2UgcHJvY2VzcyBhdHRyaWJ1dGVzXG4gICAgICAgICAgICAvLyBpbiBgdXBkYXRlYCB0aGlzIHNob3VsZCBub3QgYmUgcG9zc2libGUgKG9yIGFuIGV4dHJlbWUgY29ybmVyIGNhc2VcbiAgICAgICAgICAgIC8vIHRoYXQgd2UnZCBsaWtlIHRvIGRpc2NvdmVyKS5cbiAgICAgICAgICAgIC8vIG1hcmsgc3RhdGUgcmVmbGVjdGluZ1xuICAgICAgICAgICAgdGhpcy5fdXBkYXRlU3RhdGUgPSB0aGlzLl91cGRhdGVTdGF0ZSB8IFNUQVRFX0lTX1JFRkxFQ1RJTkdfVE9fQVRUUklCVVRFO1xuICAgICAgICAgICAgaWYgKGF0dHJWYWx1ZSA9PSBudWxsKSB7XG4gICAgICAgICAgICAgICAgdGhpcy5yZW1vdmVBdHRyaWJ1dGUoYXR0cik7XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgICAgICB0aGlzLnNldEF0dHJpYnV0ZShhdHRyLCBhdHRyVmFsdWUpO1xuICAgICAgICAgICAgfVxuICAgICAgICAgICAgLy8gbWFyayBzdGF0ZSBub3QgcmVmbGVjdGluZ1xuICAgICAgICAgICAgdGhpcy5fdXBkYXRlU3RhdGUgPSB0aGlzLl91cGRhdGVTdGF0ZSAmIH5TVEFURV9JU19SRUZMRUNUSU5HX1RPX0FUVFJJQlVURTtcbiAgICAgICAgfVxuICAgIH1cbiAgICBfYXR0cmlidXRlVG9Qcm9wZXJ0eShuYW1lLCB2YWx1ZSkge1xuICAgICAgICAvLyBVc2UgdHJhY2tpbmcgaW5mbyB0byBhdm9pZCBkZXNlcmlhbGl6aW5nIGF0dHJpYnV0ZSB2YWx1ZSBpZiBpdCB3YXNcbiAgICAgICAgLy8ganVzdCBzZXQgZnJvbSBhIHByb3BlcnR5IHNldHRlci5cbiAgICAgICAgaWYgKHRoaXMuX3VwZGF0ZVN0YXRlICYgU1RBVEVfSVNfUkVGTEVDVElOR19UT19BVFRSSUJVVEUpIHtcbiAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgfVxuICAgICAgICBjb25zdCBjdG9yID0gdGhpcy5jb25zdHJ1Y3RvcjtcbiAgICAgICAgY29uc3QgcHJvcE5hbWUgPSBjdG9yLl9hdHRyaWJ1dGVUb1Byb3BlcnR5TWFwLmdldChuYW1lKTtcbiAgICAgICAgaWYgKHByb3BOYW1lICE9PSB1bmRlZmluZWQpIHtcbiAgICAgICAgICAgIGNvbnN0IG9wdGlvbnMgPSBjdG9yLl9jbGFzc1Byb3BlcnRpZXMuZ2V0KHByb3BOYW1lKSB8fCBkZWZhdWx0UHJvcGVydHlEZWNsYXJhdGlvbjtcbiAgICAgICAgICAgIC8vIG1hcmsgc3RhdGUgcmVmbGVjdGluZ1xuICAgICAgICAgICAgdGhpcy5fdXBkYXRlU3RhdGUgPSB0aGlzLl91cGRhdGVTdGF0ZSB8IFNUQVRFX0lTX1JFRkxFQ1RJTkdfVE9fUFJPUEVSVFk7XG4gICAgICAgICAgICB0aGlzW3Byb3BOYW1lXSA9XG4gICAgICAgICAgICAgICAgLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOm5vLWFueVxuICAgICAgICAgICAgICAgIGN0b3IuX3Byb3BlcnR5VmFsdWVGcm9tQXR0cmlidXRlKHZhbHVlLCBvcHRpb25zKTtcbiAgICAgICAgICAgIC8vIG1hcmsgc3RhdGUgbm90IHJlZmxlY3RpbmdcbiAgICAgICAgICAgIHRoaXMuX3VwZGF0ZVN0YXRlID0gdGhpcy5fdXBkYXRlU3RhdGUgJiB+U1RBVEVfSVNfUkVGTEVDVElOR19UT19QUk9QRVJUWTtcbiAgICAgICAgfVxuICAgIH1cbiAgICAvKipcbiAgICAgKiBSZXF1ZXN0cyBhbiB1cGRhdGUgd2hpY2ggaXMgcHJvY2Vzc2VkIGFzeW5jaHJvbm91c2x5LiBUaGlzIHNob3VsZFxuICAgICAqIGJlIGNhbGxlZCB3aGVuIGFuIGVsZW1lbnQgc2hvdWxkIHVwZGF0ZSBiYXNlZCBvbiBzb21lIHN0YXRlIG5vdCB0cmlnZ2VyZWRcbiAgICAgKiBieSBzZXR0aW5nIGEgcHJvcGVydHkuIEluIHRoaXMgY2FzZSwgcGFzcyBubyBhcmd1bWVudHMuIEl0IHNob3VsZCBhbHNvIGJlXG4gICAgICogY2FsbGVkIHdoZW4gbWFudWFsbHkgaW1wbGVtZW50aW5nIGEgcHJvcGVydHkgc2V0dGVyLiBJbiB0aGlzIGNhc2UsIHBhc3MgdGhlXG4gICAgICogcHJvcGVydHkgYG5hbWVgIGFuZCBgb2xkVmFsdWVgIHRvIGVuc3VyZSB0aGF0IGFueSBjb25maWd1cmVkIHByb3BlcnR5XG4gICAgICogb3B0aW9ucyBhcmUgaG9ub3JlZC4gUmV0dXJucyB0aGUgYHVwZGF0ZUNvbXBsZXRlYCBQcm9taXNlIHdoaWNoIGlzIHJlc29sdmVkXG4gICAgICogd2hlbiB0aGUgdXBkYXRlIGNvbXBsZXRlcy5cbiAgICAgKlxuICAgICAqIEBwYXJhbSBuYW1lIHtQcm9wZXJ0eUtleX0gKG9wdGlvbmFsKSBuYW1lIG9mIHJlcXVlc3RpbmcgcHJvcGVydHlcbiAgICAgKiBAcGFyYW0gb2xkVmFsdWUge2FueX0gKG9wdGlvbmFsKSBvbGQgdmFsdWUgb2YgcmVxdWVzdGluZyBwcm9wZXJ0eVxuICAgICAqIEByZXR1cm5zIHtQcm9taXNlfSBBIFByb21pc2UgdGhhdCBpcyByZXNvbHZlZCB3aGVuIHRoZSB1cGRhdGUgY29tcGxldGVzLlxuICAgICAqL1xuICAgIHJlcXVlc3RVcGRhdGUobmFtZSwgb2xkVmFsdWUpIHtcbiAgICAgICAgbGV0IHNob3VsZFJlcXVlc3RVcGRhdGUgPSB0cnVlO1xuICAgICAgICAvLyBpZiB3ZSBoYXZlIGEgcHJvcGVydHkga2V5LCBwZXJmb3JtIHByb3BlcnR5IHVwZGF0ZSBzdGVwcy5cbiAgICAgICAgaWYgKG5hbWUgIT09IHVuZGVmaW5lZCAmJiAhdGhpcy5fY2hhbmdlZFByb3BlcnRpZXMuaGFzKG5hbWUpKSB7XG4gICAgICAgICAgICBjb25zdCBjdG9yID0gdGhpcy5jb25zdHJ1Y3RvcjtcbiAgICAgICAgICAgIGNvbnN0IG9wdGlvbnMgPSBjdG9yLl9jbGFzc1Byb3BlcnRpZXMuZ2V0KG5hbWUpIHx8IGRlZmF1bHRQcm9wZXJ0eURlY2xhcmF0aW9uO1xuICAgICAgICAgICAgaWYgKGN0b3IuX3ZhbHVlSGFzQ2hhbmdlZCh0aGlzW25hbWVdLCBvbGRWYWx1ZSwgb3B0aW9ucy5oYXNDaGFuZ2VkKSkge1xuICAgICAgICAgICAgICAgIC8vIHRyYWNrIG9sZCB2YWx1ZSB3aGVuIGNoYW5naW5nLlxuICAgICAgICAgICAgICAgIHRoaXMuX2NoYW5nZWRQcm9wZXJ0aWVzLnNldChuYW1lLCBvbGRWYWx1ZSk7XG4gICAgICAgICAgICAgICAgLy8gYWRkIHRvIHJlZmxlY3RpbmcgcHJvcGVydGllcyBzZXRcbiAgICAgICAgICAgICAgICBpZiAob3B0aW9ucy5yZWZsZWN0ID09PSB0cnVlICYmXG4gICAgICAgICAgICAgICAgICAgICEodGhpcy5fdXBkYXRlU3RhdGUgJiBTVEFURV9JU19SRUZMRUNUSU5HX1RPX1BST1BFUlRZKSkge1xuICAgICAgICAgICAgICAgICAgICBpZiAodGhpcy5fcmVmbGVjdGluZ1Byb3BlcnRpZXMgPT09IHVuZGVmaW5lZCkge1xuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5fcmVmbGVjdGluZ1Byb3BlcnRpZXMgPSBuZXcgTWFwKCk7XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgdGhpcy5fcmVmbGVjdGluZ1Byb3BlcnRpZXMuc2V0KG5hbWUsIG9wdGlvbnMpO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAvLyBhYm9ydCB0aGUgcmVxdWVzdCBpZiB0aGUgcHJvcGVydHkgc2hvdWxkIG5vdCBiZSBjb25zaWRlcmVkIGNoYW5nZWQuXG4gICAgICAgICAgICB9XG4gICAgICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgICAgICBzaG91bGRSZXF1ZXN0VXBkYXRlID0gZmFsc2U7XG4gICAgICAgICAgICB9XG4gICAgICAgIH1cbiAgICAgICAgaWYgKCF0aGlzLl9oYXNSZXF1ZXN0ZWRVcGRhdGUgJiYgc2hvdWxkUmVxdWVzdFVwZGF0ZSkge1xuICAgICAgICAgICAgdGhpcy5fZW5xdWV1ZVVwZGF0ZSgpO1xuICAgICAgICB9XG4gICAgICAgIHJldHVybiB0aGlzLnVwZGF0ZUNvbXBsZXRlO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBTZXRzIHVwIHRoZSBlbGVtZW50IHRvIGFzeW5jaHJvbm91c2x5IHVwZGF0ZS5cbiAgICAgKi9cbiAgICBhc3luYyBfZW5xdWV1ZVVwZGF0ZSgpIHtcbiAgICAgICAgLy8gTWFyayBzdGF0ZSB1cGRhdGluZy4uLlxuICAgICAgICB0aGlzLl91cGRhdGVTdGF0ZSA9IHRoaXMuX3VwZGF0ZVN0YXRlIHwgU1RBVEVfVVBEQVRFX1JFUVVFU1RFRDtcbiAgICAgICAgbGV0IHJlc29sdmU7XG4gICAgICAgIGNvbnN0IHByZXZpb3VzVXBkYXRlUHJvbWlzZSA9IHRoaXMuX3VwZGF0ZVByb21pc2U7XG4gICAgICAgIHRoaXMuX3VwZGF0ZVByb21pc2UgPSBuZXcgUHJvbWlzZSgocmVzKSA9PiByZXNvbHZlID0gcmVzKTtcbiAgICAgICAgLy8gRW5zdXJlIGFueSBwcmV2aW91cyB1cGRhdGUgaGFzIHJlc29sdmVkIGJlZm9yZSB1cGRhdGluZy5cbiAgICAgICAgLy8gVGhpcyBgYXdhaXRgIGFsc28gZW5zdXJlcyB0aGF0IHByb3BlcnR5IGNoYW5nZXMgYXJlIGJhdGNoZWQuXG4gICAgICAgIGF3YWl0IHByZXZpb3VzVXBkYXRlUHJvbWlzZTtcbiAgICAgICAgLy8gTWFrZSBzdXJlIHRoZSBlbGVtZW50IGhhcyBjb25uZWN0ZWQgYmVmb3JlIHVwZGF0aW5nLlxuICAgICAgICBpZiAoIXRoaXMuX2hhc0Nvbm5lY3RlZCkge1xuICAgICAgICAgICAgYXdhaXQgbmV3IFByb21pc2UoKHJlcykgPT4gdGhpcy5faGFzQ29ubmVjdGVkUmVzb2x2ZXIgPSByZXMpO1xuICAgICAgICB9XG4gICAgICAgIC8vIEFsbG93IGBwZXJmb3JtVXBkYXRlYCB0byBiZSBhc3luY2hyb25vdXMgdG8gZW5hYmxlIHNjaGVkdWxpbmcgb2YgdXBkYXRlcy5cbiAgICAgICAgY29uc3QgcmVzdWx0ID0gdGhpcy5wZXJmb3JtVXBkYXRlKCk7XG4gICAgICAgIC8vIE5vdGUsIHRoaXMgaXMgdG8gYXZvaWQgZGVsYXlpbmcgYW4gYWRkaXRpb25hbCBtaWNyb3Rhc2sgdW5sZXNzIHdlIG5lZWRcbiAgICAgICAgLy8gdG8uXG4gICAgICAgIGlmIChyZXN1bHQgIT0gbnVsbCAmJlxuICAgICAgICAgICAgdHlwZW9mIHJlc3VsdC50aGVuID09PSAnZnVuY3Rpb24nKSB7XG4gICAgICAgICAgICBhd2FpdCByZXN1bHQ7XG4gICAgICAgIH1cbiAgICAgICAgcmVzb2x2ZSghdGhpcy5faGFzUmVxdWVzdGVkVXBkYXRlKTtcbiAgICB9XG4gICAgZ2V0IF9oYXNDb25uZWN0ZWQoKSB7XG4gICAgICAgIHJldHVybiAodGhpcy5fdXBkYXRlU3RhdGUgJiBTVEFURV9IQVNfQ09OTkVDVEVEKTtcbiAgICB9XG4gICAgZ2V0IF9oYXNSZXF1ZXN0ZWRVcGRhdGUoKSB7XG4gICAgICAgIHJldHVybiAodGhpcy5fdXBkYXRlU3RhdGUgJiBTVEFURV9VUERBVEVfUkVRVUVTVEVEKTtcbiAgICB9XG4gICAgZ2V0IGhhc1VwZGF0ZWQoKSB7XG4gICAgICAgIHJldHVybiAodGhpcy5fdXBkYXRlU3RhdGUgJiBTVEFURV9IQVNfVVBEQVRFRCk7XG4gICAgfVxuICAgIC8qKlxuICAgICAqIFBlcmZvcm1zIGFuIGVsZW1lbnQgdXBkYXRlLlxuICAgICAqXG4gICAgICogWW91IGNhbiBvdmVycmlkZSB0aGlzIG1ldGhvZCB0byBjaGFuZ2UgdGhlIHRpbWluZyBvZiB1cGRhdGVzLiBGb3IgaW5zdGFuY2UsXG4gICAgICogdG8gc2NoZWR1bGUgdXBkYXRlcyB0byBvY2N1ciBqdXN0IGJlZm9yZSB0aGUgbmV4dCBmcmFtZTpcbiAgICAgKlxuICAgICAqIGBgYFxuICAgICAqIHByb3RlY3RlZCBhc3luYyBwZXJmb3JtVXBkYXRlKCk6IFByb21pc2U8dW5rbm93bj4ge1xuICAgICAqICAgYXdhaXQgbmV3IFByb21pc2UoKHJlc29sdmUpID0+IHJlcXVlc3RBbmltYXRpb25GcmFtZSgoKSA9PiByZXNvbHZlKCkpKTtcbiAgICAgKiAgIHN1cGVyLnBlcmZvcm1VcGRhdGUoKTtcbiAgICAgKiB9XG4gICAgICogYGBgXG4gICAgICovXG4gICAgcGVyZm9ybVVwZGF0ZSgpIHtcbiAgICAgICAgLy8gTWl4aW4gaW5zdGFuY2UgcHJvcGVydGllcyBvbmNlLCBpZiB0aGV5IGV4aXN0LlxuICAgICAgICBpZiAodGhpcy5faW5zdGFuY2VQcm9wZXJ0aWVzKSB7XG4gICAgICAgICAgICB0aGlzLl9hcHBseUluc3RhbmNlUHJvcGVydGllcygpO1xuICAgICAgICB9XG4gICAgICAgIGlmICh0aGlzLnNob3VsZFVwZGF0ZSh0aGlzLl9jaGFuZ2VkUHJvcGVydGllcykpIHtcbiAgICAgICAgICAgIGNvbnN0IGNoYW5nZWRQcm9wZXJ0aWVzID0gdGhpcy5fY2hhbmdlZFByb3BlcnRpZXM7XG4gICAgICAgICAgICB0aGlzLnVwZGF0ZShjaGFuZ2VkUHJvcGVydGllcyk7XG4gICAgICAgICAgICB0aGlzLl9tYXJrVXBkYXRlZCgpO1xuICAgICAgICAgICAgaWYgKCEodGhpcy5fdXBkYXRlU3RhdGUgJiBTVEFURV9IQVNfVVBEQVRFRCkpIHtcbiAgICAgICAgICAgICAgICB0aGlzLl91cGRhdGVTdGF0ZSA9IHRoaXMuX3VwZGF0ZVN0YXRlIHwgU1RBVEVfSEFTX1VQREFURUQ7XG4gICAgICAgICAgICAgICAgdGhpcy5maXJzdFVwZGF0ZWQoY2hhbmdlZFByb3BlcnRpZXMpO1xuICAgICAgICAgICAgfVxuICAgICAgICAgICAgdGhpcy51cGRhdGVkKGNoYW5nZWRQcm9wZXJ0aWVzKTtcbiAgICAgICAgfVxuICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgIHRoaXMuX21hcmtVcGRhdGVkKCk7XG4gICAgICAgIH1cbiAgICB9XG4gICAgX21hcmtVcGRhdGVkKCkge1xuICAgICAgICB0aGlzLl9jaGFuZ2VkUHJvcGVydGllcyA9IG5ldyBNYXAoKTtcbiAgICAgICAgdGhpcy5fdXBkYXRlU3RhdGUgPSB0aGlzLl91cGRhdGVTdGF0ZSAmIH5TVEFURV9VUERBVEVfUkVRVUVTVEVEO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBSZXR1cm5zIGEgUHJvbWlzZSB0aGF0IHJlc29sdmVzIHdoZW4gdGhlIGVsZW1lbnQgaGFzIGNvbXBsZXRlZCB1cGRhdGluZy5cbiAgICAgKiBUaGUgUHJvbWlzZSB2YWx1ZSBpcyBhIGJvb2xlYW4gdGhhdCBpcyBgdHJ1ZWAgaWYgdGhlIGVsZW1lbnQgY29tcGxldGVkIHRoZVxuICAgICAqIHVwZGF0ZSB3aXRob3V0IHRyaWdnZXJpbmcgYW5vdGhlciB1cGRhdGUuIFRoZSBQcm9taXNlIHJlc3VsdCBpcyBgZmFsc2VgIGlmXG4gICAgICogYSBwcm9wZXJ0eSB3YXMgc2V0IGluc2lkZSBgdXBkYXRlZCgpYC4gVGhpcyBnZXR0ZXIgY2FuIGJlIGltcGxlbWVudGVkIHRvXG4gICAgICogYXdhaXQgYWRkaXRpb25hbCBzdGF0ZS4gRm9yIGV4YW1wbGUsIGl0IGlzIHNvbWV0aW1lcyB1c2VmdWwgdG8gYXdhaXQgYVxuICAgICAqIHJlbmRlcmVkIGVsZW1lbnQgYmVmb3JlIGZ1bGZpbGxpbmcgdGhpcyBQcm9taXNlLiBUbyBkbyB0aGlzLCBmaXJzdCBhd2FpdFxuICAgICAqIGBzdXBlci51cGRhdGVDb21wbGV0ZWAgdGhlbiBhbnkgc3Vic2VxdWVudCBzdGF0ZS5cbiAgICAgKlxuICAgICAqIEByZXR1cm5zIHtQcm9taXNlfSBUaGUgUHJvbWlzZSByZXR1cm5zIGEgYm9vbGVhbiB0aGF0IGluZGljYXRlcyBpZiB0aGVcbiAgICAgKiB1cGRhdGUgcmVzb2x2ZWQgd2l0aG91dCB0cmlnZ2VyaW5nIGFub3RoZXIgdXBkYXRlLlxuICAgICAqL1xuICAgIGdldCB1cGRhdGVDb21wbGV0ZSgpIHtcbiAgICAgICAgcmV0dXJuIHRoaXMuX3VwZGF0ZVByb21pc2U7XG4gICAgfVxuICAgIC8qKlxuICAgICAqIENvbnRyb2xzIHdoZXRoZXIgb3Igbm90IGB1cGRhdGVgIHNob3VsZCBiZSBjYWxsZWQgd2hlbiB0aGUgZWxlbWVudCByZXF1ZXN0c1xuICAgICAqIGFuIHVwZGF0ZS4gQnkgZGVmYXVsdCwgdGhpcyBtZXRob2QgYWx3YXlzIHJldHVybnMgYHRydWVgLCBidXQgdGhpcyBjYW4gYmVcbiAgICAgKiBjdXN0b21pemVkIHRvIGNvbnRyb2wgd2hlbiB0byB1cGRhdGUuXG4gICAgICpcbiAgICAgKiAqIEBwYXJhbSBfY2hhbmdlZFByb3BlcnRpZXMgTWFwIG9mIGNoYW5nZWQgcHJvcGVydGllcyB3aXRoIG9sZCB2YWx1ZXNcbiAgICAgKi9cbiAgICBzaG91bGRVcGRhdGUoX2NoYW5nZWRQcm9wZXJ0aWVzKSB7XG4gICAgICAgIHJldHVybiB0cnVlO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBVcGRhdGVzIHRoZSBlbGVtZW50LiBUaGlzIG1ldGhvZCByZWZsZWN0cyBwcm9wZXJ0eSB2YWx1ZXMgdG8gYXR0cmlidXRlcy5cbiAgICAgKiBJdCBjYW4gYmUgb3ZlcnJpZGRlbiB0byByZW5kZXIgYW5kIGtlZXAgdXBkYXRlZCBlbGVtZW50IERPTS5cbiAgICAgKiBTZXR0aW5nIHByb3BlcnRpZXMgaW5zaWRlIHRoaXMgbWV0aG9kIHdpbGwgKm5vdCogdHJpZ2dlclxuICAgICAqIGFub3RoZXIgdXBkYXRlLlxuICAgICAqXG4gICAgICogKiBAcGFyYW0gX2NoYW5nZWRQcm9wZXJ0aWVzIE1hcCBvZiBjaGFuZ2VkIHByb3BlcnRpZXMgd2l0aCBvbGQgdmFsdWVzXG4gICAgICovXG4gICAgdXBkYXRlKF9jaGFuZ2VkUHJvcGVydGllcykge1xuICAgICAgICBpZiAodGhpcy5fcmVmbGVjdGluZ1Byb3BlcnRpZXMgIT09IHVuZGVmaW5lZCAmJlxuICAgICAgICAgICAgdGhpcy5fcmVmbGVjdGluZ1Byb3BlcnRpZXMuc2l6ZSA+IDApIHtcbiAgICAgICAgICAgIC8vIFVzZSBmb3JFYWNoIHNvIHRoaXMgd29ya3MgZXZlbiBpZiBmb3Ivb2YgbG9vcHMgYXJlIGNvbXBpbGVkIHRvIGZvclxuICAgICAgICAgICAgLy8gbG9vcHMgZXhwZWN0aW5nIGFycmF5c1xuICAgICAgICAgICAgdGhpcy5fcmVmbGVjdGluZ1Byb3BlcnRpZXMuZm9yRWFjaCgodiwgaykgPT4gdGhpcy5fcHJvcGVydHlUb0F0dHJpYnV0ZShrLCB0aGlzW2tdLCB2KSk7XG4gICAgICAgICAgICB0aGlzLl9yZWZsZWN0aW5nUHJvcGVydGllcyA9IHVuZGVmaW5lZDtcbiAgICAgICAgfVxuICAgIH1cbiAgICAvKipcbiAgICAgKiBJbnZva2VkIHdoZW5ldmVyIHRoZSBlbGVtZW50IGlzIHVwZGF0ZWQuIEltcGxlbWVudCB0byBwZXJmb3JtXG4gICAgICogcG9zdC11cGRhdGluZyB0YXNrcyB2aWEgRE9NIEFQSXMsIGZvciBleGFtcGxlLCBmb2N1c2luZyBhbiBlbGVtZW50LlxuICAgICAqXG4gICAgICogU2V0dGluZyBwcm9wZXJ0aWVzIGluc2lkZSB0aGlzIG1ldGhvZCB3aWxsIHRyaWdnZXIgdGhlIGVsZW1lbnQgdG8gdXBkYXRlXG4gICAgICogYWdhaW4gYWZ0ZXIgdGhpcyB1cGRhdGUgY3ljbGUgY29tcGxldGVzLlxuICAgICAqXG4gICAgICogKiBAcGFyYW0gX2NoYW5nZWRQcm9wZXJ0aWVzIE1hcCBvZiBjaGFuZ2VkIHByb3BlcnRpZXMgd2l0aCBvbGQgdmFsdWVzXG4gICAgICovXG4gICAgdXBkYXRlZChfY2hhbmdlZFByb3BlcnRpZXMpIHtcbiAgICB9XG4gICAgLyoqXG4gICAgICogSW52b2tlZCB3aGVuIHRoZSBlbGVtZW50IGlzIGZpcnN0IHVwZGF0ZWQuIEltcGxlbWVudCB0byBwZXJmb3JtIG9uZSB0aW1lXG4gICAgICogd29yayBvbiB0aGUgZWxlbWVudCBhZnRlciB1cGRhdGUuXG4gICAgICpcbiAgICAgKiBTZXR0aW5nIHByb3BlcnRpZXMgaW5zaWRlIHRoaXMgbWV0aG9kIHdpbGwgdHJpZ2dlciB0aGUgZWxlbWVudCB0byB1cGRhdGVcbiAgICAgKiBhZ2FpbiBhZnRlciB0aGlzIHVwZGF0ZSBjeWNsZSBjb21wbGV0ZXMuXG4gICAgICpcbiAgICAgKiAqIEBwYXJhbSBfY2hhbmdlZFByb3BlcnRpZXMgTWFwIG9mIGNoYW5nZWQgcHJvcGVydGllcyB3aXRoIG9sZCB2YWx1ZXNcbiAgICAgKi9cbiAgICBmaXJzdFVwZGF0ZWQoX2NoYW5nZWRQcm9wZXJ0aWVzKSB7XG4gICAgfVxufVxuLyoqXG4gKiBNYXJrcyBjbGFzcyBhcyBoYXZpbmcgZmluaXNoZWQgY3JlYXRpbmcgcHJvcGVydGllcy5cbiAqL1xuVXBkYXRpbmdFbGVtZW50LmZpbmFsaXplZCA9IHRydWU7XG4vLyMgc291cmNlTWFwcGluZ1VSTD11cGRhdGluZy1lbGVtZW50LmpzLm1hcCIsIi8qKlxuICogQGxpY2Vuc2VcbiAqIENvcHlyaWdodCAoYykgMjAxNyBUaGUgUG9seW1lciBQcm9qZWN0IEF1dGhvcnMuIEFsbCByaWdodHMgcmVzZXJ2ZWQuXG4gKiBUaGlzIGNvZGUgbWF5IG9ubHkgYmUgdXNlZCB1bmRlciB0aGUgQlNEIHN0eWxlIGxpY2Vuc2UgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9MSUNFTlNFLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBhdXRob3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0FVVEhPUlMudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGNvbnRyaWJ1dG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9DT05UUklCVVRPUlMudHh0XG4gKiBDb2RlIGRpc3RyaWJ1dGVkIGJ5IEdvb2dsZSBhcyBwYXJ0IG9mIHRoZSBwb2x5bWVyIHByb2plY3QgaXMgYWxzb1xuICogc3ViamVjdCB0byBhbiBhZGRpdGlvbmFsIElQIHJpZ2h0cyBncmFudCBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL1BBVEVOVFMudHh0XG4gKi9cbmltcG9ydCB7IFRlbXBsYXRlUmVzdWx0IH0gZnJvbSAnbGl0LWh0bWwnO1xuaW1wb3J0IHsgcmVuZGVyIH0gZnJvbSAnbGl0LWh0bWwvbGliL3NoYWR5LXJlbmRlcic7XG5pbXBvcnQgeyBVcGRhdGluZ0VsZW1lbnQgfSBmcm9tICcuL2xpYi91cGRhdGluZy1lbGVtZW50LmpzJztcbmV4cG9ydCAqIGZyb20gJy4vbGliL3VwZGF0aW5nLWVsZW1lbnQuanMnO1xuZXhwb3J0ICogZnJvbSAnLi9saWIvZGVjb3JhdG9ycy5qcyc7XG5leHBvcnQgeyBodG1sLCBzdmcsIFRlbXBsYXRlUmVzdWx0LCBTVkdUZW1wbGF0ZVJlc3VsdCB9IGZyb20gJ2xpdC1odG1sL2xpdC1odG1sJztcbmltcG9ydCB7IHN1cHBvcnRzQWRvcHRpbmdTdHlsZVNoZWV0cyB9IGZyb20gJy4vbGliL2Nzcy10YWcuanMnO1xuZXhwb3J0ICogZnJvbSAnLi9saWIvY3NzLXRhZy5qcyc7XG4vLyBJTVBPUlRBTlQ6IGRvIG5vdCBjaGFuZ2UgdGhlIHByb3BlcnR5IG5hbWUgb3IgdGhlIGFzc2lnbm1lbnQgZXhwcmVzc2lvbi5cbi8vIFRoaXMgbGluZSB3aWxsIGJlIHVzZWQgaW4gcmVnZXhlcyB0byBzZWFyY2ggZm9yIExpdEVsZW1lbnQgdXNhZ2UuXG4vLyBUT0RPKGp1c3RpbmZhZ25hbmkpOiBpbmplY3QgdmVyc2lvbiBudW1iZXIgYXQgYnVpbGQgdGltZVxuKHdpbmRvd1snbGl0RWxlbWVudFZlcnNpb25zJ10gfHwgKHdpbmRvd1snbGl0RWxlbWVudFZlcnNpb25zJ10gPSBbXSkpXG4gICAgLnB1c2goJzIuMC4xJyk7XG4vKipcbiAqIE1pbmltYWwgaW1wbGVtZW50YXRpb24gb2YgQXJyYXkucHJvdG90eXBlLmZsYXRcbiAqIEBwYXJhbSBhcnIgdGhlIGFycmF5IHRvIGZsYXR0ZW5cbiAqIEBwYXJhbSByZXN1bHQgdGhlIGFjY3VtbGF0ZWQgcmVzdWx0XG4gKi9cbmZ1bmN0aW9uIGFycmF5RmxhdChzdHlsZXMsIHJlc3VsdCA9IFtdKSB7XG4gICAgZm9yIChsZXQgaSA9IDAsIGxlbmd0aCA9IHN0eWxlcy5sZW5ndGg7IGkgPCBsZW5ndGg7IGkrKykge1xuICAgICAgICBjb25zdCB2YWx1ZSA9IHN0eWxlc1tpXTtcbiAgICAgICAgaWYgKEFycmF5LmlzQXJyYXkodmFsdWUpKSB7XG4gICAgICAgICAgICBhcnJheUZsYXQodmFsdWUsIHJlc3VsdCk7XG4gICAgICAgIH1cbiAgICAgICAgZWxzZSB7XG4gICAgICAgICAgICByZXN1bHQucHVzaCh2YWx1ZSk7XG4gICAgICAgIH1cbiAgICB9XG4gICAgcmV0dXJuIHJlc3VsdDtcbn1cbi8qKiBEZWVwbHkgZmxhdHRlbnMgc3R5bGVzIGFycmF5LiBVc2VzIG5hdGl2ZSBmbGF0IGlmIGF2YWlsYWJsZS4gKi9cbmNvbnN0IGZsYXR0ZW5TdHlsZXMgPSAoc3R5bGVzKSA9PiBzdHlsZXMuZmxhdCA/IHN0eWxlcy5mbGF0KEluZmluaXR5KSA6IGFycmF5RmxhdChzdHlsZXMpO1xuZXhwb3J0IGNsYXNzIExpdEVsZW1lbnQgZXh0ZW5kcyBVcGRhdGluZ0VsZW1lbnQge1xuICAgIC8qKiBAbm9jb2xsYXBzZSAqL1xuICAgIHN0YXRpYyBmaW5hbGl6ZSgpIHtcbiAgICAgICAgc3VwZXIuZmluYWxpemUoKTtcbiAgICAgICAgLy8gUHJlcGFyZSBzdHlsaW5nIHRoYXQgaXMgc3RhbXBlZCBhdCBmaXJzdCByZW5kZXIgdGltZS4gU3R5bGluZ1xuICAgICAgICAvLyBpcyBidWlsdCBmcm9tIHVzZXIgcHJvdmlkZWQgYHN0eWxlc2Agb3IgaXMgaW5oZXJpdGVkIGZyb20gdGhlIHN1cGVyY2xhc3MuXG4gICAgICAgIHRoaXMuX3N0eWxlcyA9XG4gICAgICAgICAgICB0aGlzLmhhc093blByb3BlcnR5KEpTQ29tcGlsZXJfcmVuYW1lUHJvcGVydHkoJ3N0eWxlcycsIHRoaXMpKSA/XG4gICAgICAgICAgICAgICAgdGhpcy5fZ2V0VW5pcXVlU3R5bGVzKCkgOlxuICAgICAgICAgICAgICAgIHRoaXMuX3N0eWxlcyB8fCBbXTtcbiAgICB9XG4gICAgLyoqIEBub2NvbGxhcHNlICovXG4gICAgc3RhdGljIF9nZXRVbmlxdWVTdHlsZXMoKSB7XG4gICAgICAgIC8vIFRha2UgY2FyZSBub3QgdG8gY2FsbCBgdGhpcy5zdHlsZXNgIG11bHRpcGxlIHRpbWVzIHNpbmNlIHRoaXMgZ2VuZXJhdGVzXG4gICAgICAgIC8vIG5ldyBDU1NSZXN1bHRzIGVhY2ggdGltZS5cbiAgICAgICAgLy8gVE9ETyhzb3J2ZWxsKTogU2luY2Ugd2UgZG8gbm90IGNhY2hlIENTU1Jlc3VsdHMgYnkgaW5wdXQsIGFueVxuICAgICAgICAvLyBzaGFyZWQgc3R5bGVzIHdpbGwgZ2VuZXJhdGUgbmV3IHN0eWxlc2hlZXQgb2JqZWN0cywgd2hpY2ggaXMgd2FzdGVmdWwuXG4gICAgICAgIC8vIFRoaXMgc2hvdWxkIGJlIGFkZHJlc3NlZCB3aGVuIGEgYnJvd3NlciBzaGlwcyBjb25zdHJ1Y3RhYmxlXG4gICAgICAgIC8vIHN0eWxlc2hlZXRzLlxuICAgICAgICBjb25zdCB1c2VyU3R5bGVzID0gdGhpcy5zdHlsZXM7XG4gICAgICAgIGNvbnN0IHN0eWxlcyA9IFtdO1xuICAgICAgICBpZiAoQXJyYXkuaXNBcnJheSh1c2VyU3R5bGVzKSkge1xuICAgICAgICAgICAgY29uc3QgZmxhdFN0eWxlcyA9IGZsYXR0ZW5TdHlsZXModXNlclN0eWxlcyk7XG4gICAgICAgICAgICAvLyBBcyBhIHBlcmZvcm1hbmNlIG9wdGltaXphdGlvbiB0byBhdm9pZCBkdXBsaWNhdGVkIHN0eWxpbmcgdGhhdCBjYW5cbiAgICAgICAgICAgIC8vIG9jY3VyIGVzcGVjaWFsbHkgd2hlbiBjb21wb3NpbmcgdmlhIHN1YmNsYXNzaW5nLCBkZS1kdXBsaWNhdGUgc3R5bGVzXG4gICAgICAgICAgICAvLyBwcmVzZXJ2aW5nIHRoZSBsYXN0IGl0ZW0gaW4gdGhlIGxpc3QuIFRoZSBsYXN0IGl0ZW0gaXMga2VwdCB0b1xuICAgICAgICAgICAgLy8gdHJ5IHRvIHByZXNlcnZlIGNhc2NhZGUgb3JkZXIgd2l0aCB0aGUgYXNzdW1wdGlvbiB0aGF0IGl0J3MgbW9zdFxuICAgICAgICAgICAgLy8gaW1wb3J0YW50IHRoYXQgbGFzdCBhZGRlZCBzdHlsZXMgb3ZlcnJpZGUgcHJldmlvdXMgc3R5bGVzLlxuICAgICAgICAgICAgY29uc3Qgc3R5bGVTZXQgPSBmbGF0U3R5bGVzLnJlZHVjZVJpZ2h0KChzZXQsIHMpID0+IHtcbiAgICAgICAgICAgICAgICBzZXQuYWRkKHMpO1xuICAgICAgICAgICAgICAgIC8vIG9uIElFIHNldC5hZGQgZG9lcyBub3QgcmV0dXJuIHRoZSBzZXQuXG4gICAgICAgICAgICAgICAgcmV0dXJuIHNldDtcbiAgICAgICAgICAgIH0sIG5ldyBTZXQoKSk7XG4gICAgICAgICAgICAvLyBBcnJheS5mcm9tIGRvZXMgbm90IHdvcmsgb24gU2V0IGluIElFXG4gICAgICAgICAgICBzdHlsZVNldC5mb3JFYWNoKCh2KSA9PiBzdHlsZXMudW5zaGlmdCh2KSk7XG4gICAgICAgIH1cbiAgICAgICAgZWxzZSBpZiAodXNlclN0eWxlcykge1xuICAgICAgICAgICAgc3R5bGVzLnB1c2godXNlclN0eWxlcyk7XG4gICAgICAgIH1cbiAgICAgICAgcmV0dXJuIHN0eWxlcztcbiAgICB9XG4gICAgLyoqXG4gICAgICogUGVyZm9ybXMgZWxlbWVudCBpbml0aWFsaXphdGlvbi4gQnkgZGVmYXVsdCB0aGlzIGNhbGxzIGBjcmVhdGVSZW5kZXJSb290YFxuICAgICAqIHRvIGNyZWF0ZSB0aGUgZWxlbWVudCBgcmVuZGVyUm9vdGAgbm9kZSBhbmQgY2FwdHVyZXMgYW55IHByZS1zZXQgdmFsdWVzIGZvclxuICAgICAqIHJlZ2lzdGVyZWQgcHJvcGVydGllcy5cbiAgICAgKi9cbiAgICBpbml0aWFsaXplKCkge1xuICAgICAgICBzdXBlci5pbml0aWFsaXplKCk7XG4gICAgICAgIHRoaXMucmVuZGVyUm9vdCA9IHRoaXMuY3JlYXRlUmVuZGVyUm9vdCgpO1xuICAgICAgICAvLyBOb3RlLCBpZiByZW5kZXJSb290IGlzIG5vdCBhIHNoYWRvd1Jvb3QsIHN0eWxlcyB3b3VsZC9jb3VsZCBhcHBseSB0byB0aGVcbiAgICAgICAgLy8gZWxlbWVudCdzIGdldFJvb3ROb2RlKCkuIFdoaWxlIHRoaXMgY291bGQgYmUgZG9uZSwgd2UncmUgY2hvb3Npbmcgbm90IHRvXG4gICAgICAgIC8vIHN1cHBvcnQgdGhpcyBub3cgc2luY2UgaXQgd291bGQgcmVxdWlyZSBkaWZmZXJlbnQgbG9naWMgYXJvdW5kIGRlLWR1cGluZy5cbiAgICAgICAgaWYgKHdpbmRvdy5TaGFkb3dSb290ICYmIHRoaXMucmVuZGVyUm9vdCBpbnN0YW5jZW9mIHdpbmRvdy5TaGFkb3dSb290KSB7XG4gICAgICAgICAgICB0aGlzLmFkb3B0U3R5bGVzKCk7XG4gICAgICAgIH1cbiAgICB9XG4gICAgLyoqXG4gICAgICogUmV0dXJucyB0aGUgbm9kZSBpbnRvIHdoaWNoIHRoZSBlbGVtZW50IHNob3VsZCByZW5kZXIgYW5kIGJ5IGRlZmF1bHRcbiAgICAgKiBjcmVhdGVzIGFuZCByZXR1cm5zIGFuIG9wZW4gc2hhZG93Um9vdC4gSW1wbGVtZW50IHRvIGN1c3RvbWl6ZSB3aGVyZSB0aGVcbiAgICAgKiBlbGVtZW50J3MgRE9NIGlzIHJlbmRlcmVkLiBGb3IgZXhhbXBsZSwgdG8gcmVuZGVyIGludG8gdGhlIGVsZW1lbnQnc1xuICAgICAqIGNoaWxkTm9kZXMsIHJldHVybiBgdGhpc2AuXG4gICAgICogQHJldHVybnMge0VsZW1lbnR8RG9jdW1lbnRGcmFnbWVudH0gUmV0dXJucyBhIG5vZGUgaW50byB3aGljaCB0byByZW5kZXIuXG4gICAgICovXG4gICAgY3JlYXRlUmVuZGVyUm9vdCgpIHtcbiAgICAgICAgcmV0dXJuIHRoaXMuYXR0YWNoU2hhZG93KHsgbW9kZTogJ29wZW4nIH0pO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBBcHBsaWVzIHN0eWxpbmcgdG8gdGhlIGVsZW1lbnQgc2hhZG93Um9vdCB1c2luZyB0aGUgYHN0YXRpYyBnZXQgc3R5bGVzYFxuICAgICAqIHByb3BlcnR5LiBTdHlsaW5nIHdpbGwgYXBwbHkgdXNpbmcgYHNoYWRvd1Jvb3QuYWRvcHRlZFN0eWxlU2hlZXRzYCB3aGVyZVxuICAgICAqIGF2YWlsYWJsZSBhbmQgd2lsbCBmYWxsYmFjayBvdGhlcndpc2UuIFdoZW4gU2hhZG93IERPTSBpcyBwb2x5ZmlsbGVkLFxuICAgICAqIFNoYWR5Q1NTIHNjb3BlcyBzdHlsZXMgYW5kIGFkZHMgdGhlbSB0byB0aGUgZG9jdW1lbnQuIFdoZW4gU2hhZG93IERPTVxuICAgICAqIGlzIGF2YWlsYWJsZSBidXQgYGFkb3B0ZWRTdHlsZVNoZWV0c2AgaXMgbm90LCBzdHlsZXMgYXJlIGFwcGVuZGVkIHRvIHRoZVxuICAgICAqIGVuZCBvZiB0aGUgYHNoYWRvd1Jvb3RgIHRvIFttaW1pYyBzcGVjXG4gICAgICogYmVoYXZpb3JdKGh0dHBzOi8vd2ljZy5naXRodWIuaW8vY29uc3RydWN0LXN0eWxlc2hlZXRzLyN1c2luZy1jb25zdHJ1Y3RlZC1zdHlsZXNoZWV0cykuXG4gICAgICovXG4gICAgYWRvcHRTdHlsZXMoKSB7XG4gICAgICAgIGNvbnN0IHN0eWxlcyA9IHRoaXMuY29uc3RydWN0b3IuX3N0eWxlcztcbiAgICAgICAgaWYgKHN0eWxlcy5sZW5ndGggPT09IDApIHtcbiAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgfVxuICAgICAgICAvLyBUaGVyZSBhcmUgdGhyZWUgc2VwYXJhdGUgY2FzZXMgaGVyZSBiYXNlZCBvbiBTaGFkb3cgRE9NIHN1cHBvcnQuXG4gICAgICAgIC8vICgxKSBzaGFkb3dSb290IHBvbHlmaWxsZWQ6IHVzZSBTaGFkeUNTU1xuICAgICAgICAvLyAoMikgc2hhZG93Um9vdC5hZG9wdGVkU3R5bGVTaGVldHMgYXZhaWxhYmxlOiB1c2UgaXQuXG4gICAgICAgIC8vICgzKSBzaGFkb3dSb290LmFkb3B0ZWRTdHlsZVNoZWV0cyBwb2x5ZmlsbGVkOiBhcHBlbmQgc3R5bGVzIGFmdGVyXG4gICAgICAgIC8vIHJlbmRlcmluZ1xuICAgICAgICBpZiAod2luZG93LlNoYWR5Q1NTICE9PSB1bmRlZmluZWQgJiYgIXdpbmRvdy5TaGFkeUNTUy5uYXRpdmVTaGFkb3cpIHtcbiAgICAgICAgICAgIHdpbmRvdy5TaGFkeUNTUy5TY29waW5nU2hpbS5wcmVwYXJlQWRvcHRlZENzc1RleHQoc3R5bGVzLm1hcCgocykgPT4gcy5jc3NUZXh0KSwgdGhpcy5sb2NhbE5hbWUpO1xuICAgICAgICB9XG4gICAgICAgIGVsc2UgaWYgKHN1cHBvcnRzQWRvcHRpbmdTdHlsZVNoZWV0cykge1xuICAgICAgICAgICAgdGhpcy5yZW5kZXJSb290LmFkb3B0ZWRTdHlsZVNoZWV0cyA9XG4gICAgICAgICAgICAgICAgc3R5bGVzLm1hcCgocykgPT4gcy5zdHlsZVNoZWV0KTtcbiAgICAgICAgfVxuICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgIC8vIFRoaXMgbXVzdCBiZSBkb25lIGFmdGVyIHJlbmRlcmluZyBzbyB0aGUgYWN0dWFsIHN0eWxlIGluc2VydGlvbiBpcyBkb25lXG4gICAgICAgICAgICAvLyBpbiBgdXBkYXRlYC5cbiAgICAgICAgICAgIHRoaXMuX25lZWRzU2hpbUFkb3B0ZWRTdHlsZVNoZWV0cyA9IHRydWU7XG4gICAgICAgIH1cbiAgICB9XG4gICAgY29ubmVjdGVkQ2FsbGJhY2soKSB7XG4gICAgICAgIHN1cGVyLmNvbm5lY3RlZENhbGxiYWNrKCk7XG4gICAgICAgIC8vIE5vdGUsIGZpcnN0IHVwZGF0ZS9yZW5kZXIgaGFuZGxlcyBzdHlsZUVsZW1lbnQgc28gd2Ugb25seSBjYWxsIHRoaXMgaWZcbiAgICAgICAgLy8gY29ubmVjdGVkIGFmdGVyIGZpcnN0IHVwZGF0ZS5cbiAgICAgICAgaWYgKHRoaXMuaGFzVXBkYXRlZCAmJiB3aW5kb3cuU2hhZHlDU1MgIT09IHVuZGVmaW5lZCkge1xuICAgICAgICAgICAgd2luZG93LlNoYWR5Q1NTLnN0eWxlRWxlbWVudCh0aGlzKTtcbiAgICAgICAgfVxuICAgIH1cbiAgICAvKipcbiAgICAgKiBVcGRhdGVzIHRoZSBlbGVtZW50LiBUaGlzIG1ldGhvZCByZWZsZWN0cyBwcm9wZXJ0eSB2YWx1ZXMgdG8gYXR0cmlidXRlc1xuICAgICAqIGFuZCBjYWxscyBgcmVuZGVyYCB0byByZW5kZXIgRE9NIHZpYSBsaXQtaHRtbC4gU2V0dGluZyBwcm9wZXJ0aWVzIGluc2lkZVxuICAgICAqIHRoaXMgbWV0aG9kIHdpbGwgKm5vdCogdHJpZ2dlciBhbm90aGVyIHVwZGF0ZS5cbiAgICAgKiAqIEBwYXJhbSBfY2hhbmdlZFByb3BlcnRpZXMgTWFwIG9mIGNoYW5nZWQgcHJvcGVydGllcyB3aXRoIG9sZCB2YWx1ZXNcbiAgICAgKi9cbiAgICB1cGRhdGUoY2hhbmdlZFByb3BlcnRpZXMpIHtcbiAgICAgICAgc3VwZXIudXBkYXRlKGNoYW5nZWRQcm9wZXJ0aWVzKTtcbiAgICAgICAgY29uc3QgdGVtcGxhdGVSZXN1bHQgPSB0aGlzLnJlbmRlcigpO1xuICAgICAgICBpZiAodGVtcGxhdGVSZXN1bHQgaW5zdGFuY2VvZiBUZW1wbGF0ZVJlc3VsdCkge1xuICAgICAgICAgICAgdGhpcy5jb25zdHJ1Y3RvclxuICAgICAgICAgICAgICAgIC5yZW5kZXIodGVtcGxhdGVSZXN1bHQsIHRoaXMucmVuZGVyUm9vdCwgeyBzY29wZU5hbWU6IHRoaXMubG9jYWxOYW1lLCBldmVudENvbnRleHQ6IHRoaXMgfSk7XG4gICAgICAgIH1cbiAgICAgICAgLy8gV2hlbiBuYXRpdmUgU2hhZG93IERPTSBpcyB1c2VkIGJ1dCBhZG9wdGVkU3R5bGVzIGFyZSBub3Qgc3VwcG9ydGVkLFxuICAgICAgICAvLyBpbnNlcnQgc3R5bGluZyBhZnRlciByZW5kZXJpbmcgdG8gZW5zdXJlIGFkb3B0ZWRTdHlsZXMgaGF2ZSBoaWdoZXN0XG4gICAgICAgIC8vIHByaW9yaXR5LlxuICAgICAgICBpZiAodGhpcy5fbmVlZHNTaGltQWRvcHRlZFN0eWxlU2hlZXRzKSB7XG4gICAgICAgICAgICB0aGlzLl9uZWVkc1NoaW1BZG9wdGVkU3R5bGVTaGVldHMgPSBmYWxzZTtcbiAgICAgICAgICAgIHRoaXMuY29uc3RydWN0b3IuX3N0eWxlcy5mb3JFYWNoKChzKSA9PiB7XG4gICAgICAgICAgICAgICAgY29uc3Qgc3R5bGUgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KCdzdHlsZScpO1xuICAgICAgICAgICAgICAgIHN0eWxlLnRleHRDb250ZW50ID0gcy5jc3NUZXh0O1xuICAgICAgICAgICAgICAgIHRoaXMucmVuZGVyUm9vdC5hcHBlbmRDaGlsZChzdHlsZSk7XG4gICAgICAgICAgICB9KTtcbiAgICAgICAgfVxuICAgIH1cbiAgICAvKipcbiAgICAgKiBJbnZva2VkIG9uIGVhY2ggdXBkYXRlIHRvIHBlcmZvcm0gcmVuZGVyaW5nIHRhc2tzLiBUaGlzIG1ldGhvZCBtdXN0IHJldHVyblxuICAgICAqIGEgbGl0LWh0bWwgVGVtcGxhdGVSZXN1bHQuIFNldHRpbmcgcHJvcGVydGllcyBpbnNpZGUgdGhpcyBtZXRob2Qgd2lsbCAqbm90KlxuICAgICAqIHRyaWdnZXIgdGhlIGVsZW1lbnQgdG8gdXBkYXRlLlxuICAgICAqL1xuICAgIHJlbmRlcigpIHtcbiAgICB9XG59XG4vKipcbiAqIEVuc3VyZSB0aGlzIGNsYXNzIGlzIG1hcmtlZCBhcyBgZmluYWxpemVkYCBhcyBhbiBvcHRpbWl6YXRpb24gZW5zdXJpbmdcbiAqIGl0IHdpbGwgbm90IG5lZWRsZXNzbHkgdHJ5IHRvIGBmaW5hbGl6ZWAuXG4gKi9cbkxpdEVsZW1lbnQuZmluYWxpemVkID0gdHJ1ZTtcbi8qKlxuICogUmVuZGVyIG1ldGhvZCB1c2VkIHRvIHJlbmRlciB0aGUgbGl0LWh0bWwgVGVtcGxhdGVSZXN1bHQgdG8gdGhlIGVsZW1lbnQnc1xuICogRE9NLlxuICogQHBhcmFtIHtUZW1wbGF0ZVJlc3VsdH0gVGVtcGxhdGUgdG8gcmVuZGVyLlxuICogQHBhcmFtIHtFbGVtZW50fERvY3VtZW50RnJhZ21lbnR9IE5vZGUgaW50byB3aGljaCB0byByZW5kZXIuXG4gKiBAcGFyYW0ge1N0cmluZ30gRWxlbWVudCBuYW1lLlxuICogQG5vY29sbGFwc2VcbiAqL1xuTGl0RWxlbWVudC5yZW5kZXIgPSByZW5kZXI7XG4vLyMgc291cmNlTWFwcGluZ1VSTD1saXQtZWxlbWVudC5qcy5tYXAiLCIvKipcbiAqIEBsaWNlbnNlXG4gKiBDb3B5cmlnaHQgKGMpIDIwMTcgVGhlIFBvbHltZXIgUHJvamVjdCBBdXRob3JzLiBBbGwgcmlnaHRzIHJlc2VydmVkLlxuICogVGhpcyBjb2RlIG1heSBvbmx5IGJlIHVzZWQgdW5kZXIgdGhlIEJTRCBzdHlsZSBsaWNlbnNlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vTElDRU5TRS50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgYXV0aG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9BVVRIT1JTLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBjb250cmlidXRvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQ09OVFJJQlVUT1JTLnR4dFxuICogQ29kZSBkaXN0cmlidXRlZCBieSBHb29nbGUgYXMgcGFydCBvZiB0aGUgcG9seW1lciBwcm9qZWN0IGlzIGFsc29cbiAqIHN1YmplY3QgdG8gYW4gYWRkaXRpb25hbCBJUCByaWdodHMgZ3JhbnQgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9QQVRFTlRTLnR4dFxuICovXG5pbXBvcnQgeyBpc1ByaW1pdGl2ZSB9IGZyb20gJy4uL2xpYi9wYXJ0cy5qcyc7XG5pbXBvcnQgeyBkaXJlY3RpdmUsIE5vZGVQYXJ0IH0gZnJvbSAnLi4vbGl0LWh0bWwuanMnO1xuLy8gRm9yIGVhY2ggcGFydCwgcmVtZW1iZXIgdGhlIHZhbHVlIHRoYXQgd2FzIGxhc3QgcmVuZGVyZWQgdG8gdGhlIHBhcnQgYnkgdGhlXG4vLyB1bnNhZmVIVE1MIGRpcmVjdGl2ZSwgYW5kIHRoZSBEb2N1bWVudEZyYWdtZW50IHRoYXQgd2FzIGxhc3Qgc2V0IGFzIGEgdmFsdWUuXG4vLyBUaGUgRG9jdW1lbnRGcmFnbWVudCBpcyB1c2VkIGFzIGEgdW5pcXVlIGtleSB0byBjaGVjayBpZiB0aGUgbGFzdCB2YWx1ZVxuLy8gcmVuZGVyZWQgdG8gdGhlIHBhcnQgd2FzIHdpdGggdW5zYWZlSFRNTC4gSWYgbm90LCB3ZSdsbCBhbHdheXMgcmUtcmVuZGVyIHRoZVxuLy8gdmFsdWUgcGFzc2VkIHRvIHVuc2FmZUhUTUwuXG5jb25zdCBwcmV2aW91c1ZhbHVlcyA9IG5ldyBXZWFrTWFwKCk7XG4vKipcbiAqIFJlbmRlcnMgdGhlIHJlc3VsdCBhcyBIVE1MLCByYXRoZXIgdGhhbiB0ZXh0LlxuICpcbiAqIE5vdGUsIHRoaXMgaXMgdW5zYWZlIHRvIHVzZSB3aXRoIGFueSB1c2VyLXByb3ZpZGVkIGlucHV0IHRoYXQgaGFzbid0IGJlZW5cbiAqIHNhbml0aXplZCBvciBlc2NhcGVkLCBhcyBpdCBtYXkgbGVhZCB0byBjcm9zcy1zaXRlLXNjcmlwdGluZ1xuICogdnVsbmVyYWJpbGl0aWVzLlxuICovXG5leHBvcnQgY29uc3QgdW5zYWZlSFRNTCA9IGRpcmVjdGl2ZSgodmFsdWUpID0+IChwYXJ0KSA9PiB7XG4gICAgaWYgKCEocGFydCBpbnN0YW5jZW9mIE5vZGVQYXJ0KSkge1xuICAgICAgICB0aHJvdyBuZXcgRXJyb3IoJ3Vuc2FmZUhUTUwgY2FuIG9ubHkgYmUgdXNlZCBpbiB0ZXh0IGJpbmRpbmdzJyk7XG4gICAgfVxuICAgIGNvbnN0IHByZXZpb3VzVmFsdWUgPSBwcmV2aW91c1ZhbHVlcy5nZXQocGFydCk7XG4gICAgaWYgKHByZXZpb3VzVmFsdWUgIT09IHVuZGVmaW5lZCAmJiBpc1ByaW1pdGl2ZSh2YWx1ZSkgJiZcbiAgICAgICAgdmFsdWUgPT09IHByZXZpb3VzVmFsdWUudmFsdWUgJiYgcGFydC52YWx1ZSA9PT0gcHJldmlvdXNWYWx1ZS5mcmFnbWVudCkge1xuICAgICAgICByZXR1cm47XG4gICAgfVxuICAgIGNvbnN0IHRlbXBsYXRlID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgndGVtcGxhdGUnKTtcbiAgICB0ZW1wbGF0ZS5pbm5lckhUTUwgPSB2YWx1ZTsgLy8gaW5uZXJIVE1MIGNhc3RzIHRvIHN0cmluZyBpbnRlcm5hbGx5XG4gICAgY29uc3QgZnJhZ21lbnQgPSBkb2N1bWVudC5pbXBvcnROb2RlKHRlbXBsYXRlLmNvbnRlbnQsIHRydWUpO1xuICAgIHBhcnQuc2V0VmFsdWUoZnJhZ21lbnQpO1xuICAgIHByZXZpb3VzVmFsdWVzLnNldChwYXJ0LCB7IHZhbHVlLCBmcmFnbWVudCB9KTtcbn0pO1xuLy8jIHNvdXJjZU1hcHBpbmdVUkw9dW5zYWZlLWh0bWwuanMubWFwIiwiLyoqXG4gKiBAbGljZW5zZVxuICogQ29weXJpZ2h0IChjKSAyMDE3IFRoZSBQb2x5bWVyIFByb2plY3QgQXV0aG9ycy4gQWxsIHJpZ2h0cyByZXNlcnZlZC5cbiAqIFRoaXMgY29kZSBtYXkgb25seSBiZSB1c2VkIHVuZGVyIHRoZSBCU0Qgc3R5bGUgbGljZW5zZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0xJQ0VOU0UudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGF1dGhvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQVVUSE9SUy50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgY29udHJpYnV0b3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0NPTlRSSUJVVE9SUy50eHRcbiAqIENvZGUgZGlzdHJpYnV0ZWQgYnkgR29vZ2xlIGFzIHBhcnQgb2YgdGhlIHBvbHltZXIgcHJvamVjdCBpcyBhbHNvXG4gKiBzdWJqZWN0IHRvIGFuIGFkZGl0aW9uYWwgSVAgcmlnaHRzIGdyYW50IGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vUEFURU5UUy50eHRcbiAqL1xuaW1wb3J0IHsgQXR0cmlidXRlQ29tbWl0dGVyLCBCb29sZWFuQXR0cmlidXRlUGFydCwgRXZlbnRQYXJ0LCBOb2RlUGFydCwgUHJvcGVydHlDb21taXR0ZXIgfSBmcm9tICcuL3BhcnRzLmpzJztcbi8qKlxuICogQ3JlYXRlcyBQYXJ0cyB3aGVuIGEgdGVtcGxhdGUgaXMgaW5zdGFudGlhdGVkLlxuICovXG5leHBvcnQgY2xhc3MgRGVmYXVsdFRlbXBsYXRlUHJvY2Vzc29yIHtcbiAgICAvKipcbiAgICAgKiBDcmVhdGUgcGFydHMgZm9yIGFuIGF0dHJpYnV0ZS1wb3NpdGlvbiBiaW5kaW5nLCBnaXZlbiB0aGUgZXZlbnQsIGF0dHJpYnV0ZVxuICAgICAqIG5hbWUsIGFuZCBzdHJpbmcgbGl0ZXJhbHMuXG4gICAgICpcbiAgICAgKiBAcGFyYW0gZWxlbWVudCBUaGUgZWxlbWVudCBjb250YWluaW5nIHRoZSBiaW5kaW5nXG4gICAgICogQHBhcmFtIG5hbWUgIFRoZSBhdHRyaWJ1dGUgbmFtZVxuICAgICAqIEBwYXJhbSBzdHJpbmdzIFRoZSBzdHJpbmcgbGl0ZXJhbHMuIFRoZXJlIGFyZSBhbHdheXMgYXQgbGVhc3QgdHdvIHN0cmluZ3MsXG4gICAgICogICBldmVudCBmb3IgZnVsbHktY29udHJvbGxlZCBiaW5kaW5ncyB3aXRoIGEgc2luZ2xlIGV4cHJlc3Npb24uXG4gICAgICovXG4gICAgaGFuZGxlQXR0cmlidXRlRXhwcmVzc2lvbnMoZWxlbWVudCwgbmFtZSwgc3RyaW5ncywgb3B0aW9ucykge1xuICAgICAgICBjb25zdCBwcmVmaXggPSBuYW1lWzBdO1xuICAgICAgICBpZiAocHJlZml4ID09PSAnLicpIHtcbiAgICAgICAgICAgIGNvbnN0IGNvbWl0dGVyID0gbmV3IFByb3BlcnR5Q29tbWl0dGVyKGVsZW1lbnQsIG5hbWUuc2xpY2UoMSksIHN0cmluZ3MpO1xuICAgICAgICAgICAgcmV0dXJuIGNvbWl0dGVyLnBhcnRzO1xuICAgICAgICB9XG4gICAgICAgIGlmIChwcmVmaXggPT09ICdAJykge1xuICAgICAgICAgICAgcmV0dXJuIFtuZXcgRXZlbnRQYXJ0KGVsZW1lbnQsIG5hbWUuc2xpY2UoMSksIG9wdGlvbnMuZXZlbnRDb250ZXh0KV07XG4gICAgICAgIH1cbiAgICAgICAgaWYgKHByZWZpeCA9PT0gJz8nKSB7XG4gICAgICAgICAgICByZXR1cm4gW25ldyBCb29sZWFuQXR0cmlidXRlUGFydChlbGVtZW50LCBuYW1lLnNsaWNlKDEpLCBzdHJpbmdzKV07XG4gICAgICAgIH1cbiAgICAgICAgY29uc3QgY29taXR0ZXIgPSBuZXcgQXR0cmlidXRlQ29tbWl0dGVyKGVsZW1lbnQsIG5hbWUsIHN0cmluZ3MpO1xuICAgICAgICByZXR1cm4gY29taXR0ZXIucGFydHM7XG4gICAgfVxuICAgIC8qKlxuICAgICAqIENyZWF0ZSBwYXJ0cyBmb3IgYSB0ZXh0LXBvc2l0aW9uIGJpbmRpbmcuXG4gICAgICogQHBhcmFtIHRlbXBsYXRlRmFjdG9yeVxuICAgICAqL1xuICAgIGhhbmRsZVRleHRFeHByZXNzaW9uKG9wdGlvbnMpIHtcbiAgICAgICAgcmV0dXJuIG5ldyBOb2RlUGFydChvcHRpb25zKTtcbiAgICB9XG59XG5leHBvcnQgY29uc3QgZGVmYXVsdFRlbXBsYXRlUHJvY2Vzc29yID0gbmV3IERlZmF1bHRUZW1wbGF0ZVByb2Nlc3NvcigpO1xuLy8jIHNvdXJjZU1hcHBpbmdVUkw9ZGVmYXVsdC10ZW1wbGF0ZS1wcm9jZXNzb3IuanMubWFwIiwiLyoqXG4gKiBAbGljZW5zZVxuICogQ29weXJpZ2h0IChjKSAyMDE3IFRoZSBQb2x5bWVyIFByb2plY3QgQXV0aG9ycy4gQWxsIHJpZ2h0cyByZXNlcnZlZC5cbiAqIFRoaXMgY29kZSBtYXkgb25seSBiZSB1c2VkIHVuZGVyIHRoZSBCU0Qgc3R5bGUgbGljZW5zZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0xJQ0VOU0UudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGF1dGhvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQVVUSE9SUy50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgY29udHJpYnV0b3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0NPTlRSSUJVVE9SUy50eHRcbiAqIENvZGUgZGlzdHJpYnV0ZWQgYnkgR29vZ2xlIGFzIHBhcnQgb2YgdGhlIHBvbHltZXIgcHJvamVjdCBpcyBhbHNvXG4gKiBzdWJqZWN0IHRvIGFuIGFkZGl0aW9uYWwgSVAgcmlnaHRzIGdyYW50IGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vUEFURU5UUy50eHRcbiAqL1xuY29uc3QgZGlyZWN0aXZlcyA9IG5ldyBXZWFrTWFwKCk7XG4vKipcbiAqIEJyYW5kcyBhIGZ1bmN0aW9uIGFzIGEgZGlyZWN0aXZlIHNvIHRoYXQgbGl0LWh0bWwgd2lsbCBjYWxsIHRoZSBmdW5jdGlvblxuICogZHVyaW5nIHRlbXBsYXRlIHJlbmRlcmluZywgcmF0aGVyIHRoYW4gcGFzc2luZyBhcyBhIHZhbHVlLlxuICpcbiAqIEBwYXJhbSBmIFRoZSBkaXJlY3RpdmUgZmFjdG9yeSBmdW5jdGlvbi4gTXVzdCBiZSBhIGZ1bmN0aW9uIHRoYXQgcmV0dXJucyBhXG4gKiBmdW5jdGlvbiBvZiB0aGUgc2lnbmF0dXJlIGAocGFydDogUGFydCkgPT4gdm9pZGAuIFRoZSByZXR1cm5lZCBmdW5jdGlvbiB3aWxsXG4gKiBiZSBjYWxsZWQgd2l0aCB0aGUgcGFydCBvYmplY3RcbiAqXG4gKiBAZXhhbXBsZVxuICpcbiAqIGBgYFxuICogaW1wb3J0IHtkaXJlY3RpdmUsIGh0bWx9IGZyb20gJ2xpdC1odG1sJztcbiAqXG4gKiBjb25zdCBpbW11dGFibGUgPSBkaXJlY3RpdmUoKHYpID0+IChwYXJ0KSA9PiB7XG4gKiAgIGlmIChwYXJ0LnZhbHVlICE9PSB2KSB7XG4gKiAgICAgcGFydC5zZXRWYWx1ZSh2KVxuICogICB9XG4gKiB9KTtcbiAqIGBgYFxuICovXG4vLyB0c2xpbnQ6ZGlzYWJsZS1uZXh0LWxpbmU6bm8tYW55XG5leHBvcnQgY29uc3QgZGlyZWN0aXZlID0gKGYpID0+ICgoLi4uYXJncykgPT4ge1xuICAgIGNvbnN0IGQgPSBmKC4uLmFyZ3MpO1xuICAgIGRpcmVjdGl2ZXMuc2V0KGQsIHRydWUpO1xuICAgIHJldHVybiBkO1xufSk7XG5leHBvcnQgY29uc3QgaXNEaXJlY3RpdmUgPSAobykgPT4ge1xuICAgIHJldHVybiB0eXBlb2YgbyA9PT0gJ2Z1bmN0aW9uJyAmJiBkaXJlY3RpdmVzLmhhcyhvKTtcbn07XG4vLyMgc291cmNlTWFwcGluZ1VSTD1kaXJlY3RpdmUuanMubWFwIiwiLyoqXG4gKiBAbGljZW5zZVxuICogQ29weXJpZ2h0IChjKSAyMDE3IFRoZSBQb2x5bWVyIFByb2plY3QgQXV0aG9ycy4gQWxsIHJpZ2h0cyByZXNlcnZlZC5cbiAqIFRoaXMgY29kZSBtYXkgb25seSBiZSB1c2VkIHVuZGVyIHRoZSBCU0Qgc3R5bGUgbGljZW5zZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0xJQ0VOU0UudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGF1dGhvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQVVUSE9SUy50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgY29udHJpYnV0b3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0NPTlRSSUJVVE9SUy50eHRcbiAqIENvZGUgZGlzdHJpYnV0ZWQgYnkgR29vZ2xlIGFzIHBhcnQgb2YgdGhlIHBvbHltZXIgcHJvamVjdCBpcyBhbHNvXG4gKiBzdWJqZWN0IHRvIGFuIGFkZGl0aW9uYWwgSVAgcmlnaHRzIGdyYW50IGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vUEFURU5UUy50eHRcbiAqL1xuLyoqXG4gKiBUcnVlIGlmIHRoZSBjdXN0b20gZWxlbWVudHMgcG9seWZpbGwgaXMgaW4gdXNlLlxuICovXG5leHBvcnQgY29uc3QgaXNDRVBvbHlmaWxsID0gd2luZG93LmN1c3RvbUVsZW1lbnRzICE9PSB1bmRlZmluZWQgJiZcbiAgICB3aW5kb3cuY3VzdG9tRWxlbWVudHMucG9seWZpbGxXcmFwRmx1c2hDYWxsYmFjayAhPT1cbiAgICAgICAgdW5kZWZpbmVkO1xuLyoqXG4gKiBSZXBhcmVudHMgbm9kZXMsIHN0YXJ0aW5nIGZyb20gYHN0YXJ0Tm9kZWAgKGluY2x1c2l2ZSkgdG8gYGVuZE5vZGVgXG4gKiAoZXhjbHVzaXZlKSwgaW50byBhbm90aGVyIGNvbnRhaW5lciAoY291bGQgYmUgdGhlIHNhbWUgY29udGFpbmVyKSwgYmVmb3JlXG4gKiBgYmVmb3JlTm9kZWAuIElmIGBiZWZvcmVOb2RlYCBpcyBudWxsLCBpdCBhcHBlbmRzIHRoZSBub2RlcyB0byB0aGVcbiAqIGNvbnRhaW5lci5cbiAqL1xuZXhwb3J0IGNvbnN0IHJlcGFyZW50Tm9kZXMgPSAoY29udGFpbmVyLCBzdGFydCwgZW5kID0gbnVsbCwgYmVmb3JlID0gbnVsbCkgPT4ge1xuICAgIGxldCBub2RlID0gc3RhcnQ7XG4gICAgd2hpbGUgKG5vZGUgIT09IGVuZCkge1xuICAgICAgICBjb25zdCBuID0gbm9kZS5uZXh0U2libGluZztcbiAgICAgICAgY29udGFpbmVyLmluc2VydEJlZm9yZShub2RlLCBiZWZvcmUpO1xuICAgICAgICBub2RlID0gbjtcbiAgICB9XG59O1xuLyoqXG4gKiBSZW1vdmVzIG5vZGVzLCBzdGFydGluZyBmcm9tIGBzdGFydE5vZGVgIChpbmNsdXNpdmUpIHRvIGBlbmROb2RlYFxuICogKGV4Y2x1c2l2ZSksIGZyb20gYGNvbnRhaW5lcmAuXG4gKi9cbmV4cG9ydCBjb25zdCByZW1vdmVOb2RlcyA9IChjb250YWluZXIsIHN0YXJ0Tm9kZSwgZW5kTm9kZSA9IG51bGwpID0+IHtcbiAgICBsZXQgbm9kZSA9IHN0YXJ0Tm9kZTtcbiAgICB3aGlsZSAobm9kZSAhPT0gZW5kTm9kZSkge1xuICAgICAgICBjb25zdCBuID0gbm9kZS5uZXh0U2libGluZztcbiAgICAgICAgY29udGFpbmVyLnJlbW92ZUNoaWxkKG5vZGUpO1xuICAgICAgICBub2RlID0gbjtcbiAgICB9XG59O1xuLy8jIHNvdXJjZU1hcHBpbmdVUkw9ZG9tLmpzLm1hcCIsIi8qKlxuICogQGxpY2Vuc2VcbiAqIENvcHlyaWdodCAoYykgMjAxNyBUaGUgUG9seW1lciBQcm9qZWN0IEF1dGhvcnMuIEFsbCByaWdodHMgcmVzZXJ2ZWQuXG4gKiBUaGlzIGNvZGUgbWF5IG9ubHkgYmUgdXNlZCB1bmRlciB0aGUgQlNEIHN0eWxlIGxpY2Vuc2UgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9MSUNFTlNFLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBhdXRob3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0FVVEhPUlMudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGNvbnRyaWJ1dG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9DT05UUklCVVRPUlMudHh0XG4gKiBDb2RlIGRpc3RyaWJ1dGVkIGJ5IEdvb2dsZSBhcyBwYXJ0IG9mIHRoZSBwb2x5bWVyIHByb2plY3QgaXMgYWxzb1xuICogc3ViamVjdCB0byBhbiBhZGRpdGlvbmFsIElQIHJpZ2h0cyBncmFudCBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL1BBVEVOVFMudHh0XG4gKi9cbi8qKlxuICogQG1vZHVsZSBzaGFkeS1yZW5kZXJcbiAqL1xuaW1wb3J0IHsgaXNUZW1wbGF0ZVBhcnRBY3RpdmUgfSBmcm9tICcuL3RlbXBsYXRlLmpzJztcbmNvbnN0IHdhbGtlck5vZGVGaWx0ZXIgPSAxMzMgLyogTm9kZUZpbHRlci5TSE9XX3tFTEVNRU5UfENPTU1FTlR8VEVYVH0gKi87XG4vKipcbiAqIFJlbW92ZXMgdGhlIGxpc3Qgb2Ygbm9kZXMgZnJvbSBhIFRlbXBsYXRlIHNhZmVseS4gSW4gYWRkaXRpb24gdG8gcmVtb3ZpbmdcbiAqIG5vZGVzIGZyb20gdGhlIFRlbXBsYXRlLCB0aGUgVGVtcGxhdGUgcGFydCBpbmRpY2VzIGFyZSB1cGRhdGVkIHRvIG1hdGNoXG4gKiB0aGUgbXV0YXRlZCBUZW1wbGF0ZSBET00uXG4gKlxuICogQXMgdGhlIHRlbXBsYXRlIGlzIHdhbGtlZCB0aGUgcmVtb3ZhbCBzdGF0ZSBpcyB0cmFja2VkIGFuZFxuICogcGFydCBpbmRpY2VzIGFyZSBhZGp1c3RlZCBhcyBuZWVkZWQuXG4gKlxuICogZGl2XG4gKiAgIGRpdiMxIChyZW1vdmUpIDwtLSBzdGFydCByZW1vdmluZyAocmVtb3Zpbmcgbm9kZSBpcyBkaXYjMSlcbiAqICAgICBkaXZcbiAqICAgICAgIGRpdiMyIChyZW1vdmUpICA8LS0gY29udGludWUgcmVtb3ZpbmcgKHJlbW92aW5nIG5vZGUgaXMgc3RpbGwgZGl2IzEpXG4gKiAgICAgICAgIGRpdlxuICogZGl2IDwtLSBzdG9wIHJlbW92aW5nIHNpbmNlIHByZXZpb3VzIHNpYmxpbmcgaXMgdGhlIHJlbW92aW5nIG5vZGUgKGRpdiMxLFxuICogcmVtb3ZlZCA0IG5vZGVzKVxuICovXG5leHBvcnQgZnVuY3Rpb24gcmVtb3ZlTm9kZXNGcm9tVGVtcGxhdGUodGVtcGxhdGUsIG5vZGVzVG9SZW1vdmUpIHtcbiAgICBjb25zdCB7IGVsZW1lbnQ6IHsgY29udGVudCB9LCBwYXJ0cyB9ID0gdGVtcGxhdGU7XG4gICAgY29uc3Qgd2Fsa2VyID0gZG9jdW1lbnQuY3JlYXRlVHJlZVdhbGtlcihjb250ZW50LCB3YWxrZXJOb2RlRmlsdGVyLCBudWxsLCBmYWxzZSk7XG4gICAgbGV0IHBhcnRJbmRleCA9IG5leHRBY3RpdmVJbmRleEluVGVtcGxhdGVQYXJ0cyhwYXJ0cyk7XG4gICAgbGV0IHBhcnQgPSBwYXJ0c1twYXJ0SW5kZXhdO1xuICAgIGxldCBub2RlSW5kZXggPSAtMTtcbiAgICBsZXQgcmVtb3ZlQ291bnQgPSAwO1xuICAgIGNvbnN0IG5vZGVzVG9SZW1vdmVJblRlbXBsYXRlID0gW107XG4gICAgbGV0IGN1cnJlbnRSZW1vdmluZ05vZGUgPSBudWxsO1xuICAgIHdoaWxlICh3YWxrZXIubmV4dE5vZGUoKSkge1xuICAgICAgICBub2RlSW5kZXgrKztcbiAgICAgICAgY29uc3Qgbm9kZSA9IHdhbGtlci5jdXJyZW50Tm9kZTtcbiAgICAgICAgLy8gRW5kIHJlbW92YWwgaWYgc3RlcHBlZCBwYXN0IHRoZSByZW1vdmluZyBub2RlXG4gICAgICAgIGlmIChub2RlLnByZXZpb3VzU2libGluZyA9PT0gY3VycmVudFJlbW92aW5nTm9kZSkge1xuICAgICAgICAgICAgY3VycmVudFJlbW92aW5nTm9kZSA9IG51bGw7XG4gICAgICAgIH1cbiAgICAgICAgLy8gQSBub2RlIHRvIHJlbW92ZSB3YXMgZm91bmQgaW4gdGhlIHRlbXBsYXRlXG4gICAgICAgIGlmIChub2Rlc1RvUmVtb3ZlLmhhcyhub2RlKSkge1xuICAgICAgICAgICAgbm9kZXNUb1JlbW92ZUluVGVtcGxhdGUucHVzaChub2RlKTtcbiAgICAgICAgICAgIC8vIFRyYWNrIG5vZGUgd2UncmUgcmVtb3ZpbmdcbiAgICAgICAgICAgIGlmIChjdXJyZW50UmVtb3ZpbmdOb2RlID09PSBudWxsKSB7XG4gICAgICAgICAgICAgICAgY3VycmVudFJlbW92aW5nTm9kZSA9IG5vZGU7XG4gICAgICAgICAgICB9XG4gICAgICAgIH1cbiAgICAgICAgLy8gV2hlbiByZW1vdmluZywgaW5jcmVtZW50IGNvdW50IGJ5IHdoaWNoIHRvIGFkanVzdCBzdWJzZXF1ZW50IHBhcnQgaW5kaWNlc1xuICAgICAgICBpZiAoY3VycmVudFJlbW92aW5nTm9kZSAhPT0gbnVsbCkge1xuICAgICAgICAgICAgcmVtb3ZlQ291bnQrKztcbiAgICAgICAgfVxuICAgICAgICB3aGlsZSAocGFydCAhPT0gdW5kZWZpbmVkICYmIHBhcnQuaW5kZXggPT09IG5vZGVJbmRleCkge1xuICAgICAgICAgICAgLy8gSWYgcGFydCBpcyBpbiBhIHJlbW92ZWQgbm9kZSBkZWFjdGl2YXRlIGl0IGJ5IHNldHRpbmcgaW5kZXggdG8gLTEgb3JcbiAgICAgICAgICAgIC8vIGFkanVzdCB0aGUgaW5kZXggYXMgbmVlZGVkLlxuICAgICAgICAgICAgcGFydC5pbmRleCA9IGN1cnJlbnRSZW1vdmluZ05vZGUgIT09IG51bGwgPyAtMSA6IHBhcnQuaW5kZXggLSByZW1vdmVDb3VudDtcbiAgICAgICAgICAgIC8vIGdvIHRvIHRoZSBuZXh0IGFjdGl2ZSBwYXJ0LlxuICAgICAgICAgICAgcGFydEluZGV4ID0gbmV4dEFjdGl2ZUluZGV4SW5UZW1wbGF0ZVBhcnRzKHBhcnRzLCBwYXJ0SW5kZXgpO1xuICAgICAgICAgICAgcGFydCA9IHBhcnRzW3BhcnRJbmRleF07XG4gICAgICAgIH1cbiAgICB9XG4gICAgbm9kZXNUb1JlbW92ZUluVGVtcGxhdGUuZm9yRWFjaCgobikgPT4gbi5wYXJlbnROb2RlLnJlbW92ZUNoaWxkKG4pKTtcbn1cbmNvbnN0IGNvdW50Tm9kZXMgPSAobm9kZSkgPT4ge1xuICAgIGxldCBjb3VudCA9IChub2RlLm5vZGVUeXBlID09PSAxMSAvKiBOb2RlLkRPQ1VNRU5UX0ZSQUdNRU5UX05PREUgKi8pID8gMCA6IDE7XG4gICAgY29uc3Qgd2Fsa2VyID0gZG9jdW1lbnQuY3JlYXRlVHJlZVdhbGtlcihub2RlLCB3YWxrZXJOb2RlRmlsdGVyLCBudWxsLCBmYWxzZSk7XG4gICAgd2hpbGUgKHdhbGtlci5uZXh0Tm9kZSgpKSB7XG4gICAgICAgIGNvdW50Kys7XG4gICAgfVxuICAgIHJldHVybiBjb3VudDtcbn07XG5jb25zdCBuZXh0QWN0aXZlSW5kZXhJblRlbXBsYXRlUGFydHMgPSAocGFydHMsIHN0YXJ0SW5kZXggPSAtMSkgPT4ge1xuICAgIGZvciAobGV0IGkgPSBzdGFydEluZGV4ICsgMTsgaSA8IHBhcnRzLmxlbmd0aDsgaSsrKSB7XG4gICAgICAgIGNvbnN0IHBhcnQgPSBwYXJ0c1tpXTtcbiAgICAgICAgaWYgKGlzVGVtcGxhdGVQYXJ0QWN0aXZlKHBhcnQpKSB7XG4gICAgICAgICAgICByZXR1cm4gaTtcbiAgICAgICAgfVxuICAgIH1cbiAgICByZXR1cm4gLTE7XG59O1xuLyoqXG4gKiBJbnNlcnRzIHRoZSBnaXZlbiBub2RlIGludG8gdGhlIFRlbXBsYXRlLCBvcHRpb25hbGx5IGJlZm9yZSB0aGUgZ2l2ZW5cbiAqIHJlZk5vZGUuIEluIGFkZGl0aW9uIHRvIGluc2VydGluZyB0aGUgbm9kZSBpbnRvIHRoZSBUZW1wbGF0ZSwgdGhlIFRlbXBsYXRlXG4gKiBwYXJ0IGluZGljZXMgYXJlIHVwZGF0ZWQgdG8gbWF0Y2ggdGhlIG11dGF0ZWQgVGVtcGxhdGUgRE9NLlxuICovXG5leHBvcnQgZnVuY3Rpb24gaW5zZXJ0Tm9kZUludG9UZW1wbGF0ZSh0ZW1wbGF0ZSwgbm9kZSwgcmVmTm9kZSA9IG51bGwpIHtcbiAgICBjb25zdCB7IGVsZW1lbnQ6IHsgY29udGVudCB9LCBwYXJ0cyB9ID0gdGVtcGxhdGU7XG4gICAgLy8gSWYgdGhlcmUncyBubyByZWZOb2RlLCB0aGVuIHB1dCBub2RlIGF0IGVuZCBvZiB0ZW1wbGF0ZS5cbiAgICAvLyBObyBwYXJ0IGluZGljZXMgbmVlZCB0byBiZSBzaGlmdGVkIGluIHRoaXMgY2FzZS5cbiAgICBpZiAocmVmTm9kZSA9PT0gbnVsbCB8fCByZWZOb2RlID09PSB1bmRlZmluZWQpIHtcbiAgICAgICAgY29udGVudC5hcHBlbmRDaGlsZChub2RlKTtcbiAgICAgICAgcmV0dXJuO1xuICAgIH1cbiAgICBjb25zdCB3YWxrZXIgPSBkb2N1bWVudC5jcmVhdGVUcmVlV2Fsa2VyKGNvbnRlbnQsIHdhbGtlck5vZGVGaWx0ZXIsIG51bGwsIGZhbHNlKTtcbiAgICBsZXQgcGFydEluZGV4ID0gbmV4dEFjdGl2ZUluZGV4SW5UZW1wbGF0ZVBhcnRzKHBhcnRzKTtcbiAgICBsZXQgaW5zZXJ0Q291bnQgPSAwO1xuICAgIGxldCB3YWxrZXJJbmRleCA9IC0xO1xuICAgIHdoaWxlICh3YWxrZXIubmV4dE5vZGUoKSkge1xuICAgICAgICB3YWxrZXJJbmRleCsrO1xuICAgICAgICBjb25zdCB3YWxrZXJOb2RlID0gd2Fsa2VyLmN1cnJlbnROb2RlO1xuICAgICAgICBpZiAod2Fsa2VyTm9kZSA9PT0gcmVmTm9kZSkge1xuICAgICAgICAgICAgaW5zZXJ0Q291bnQgPSBjb3VudE5vZGVzKG5vZGUpO1xuICAgICAgICAgICAgcmVmTm9kZS5wYXJlbnROb2RlLmluc2VydEJlZm9yZShub2RlLCByZWZOb2RlKTtcbiAgICAgICAgfVxuICAgICAgICB3aGlsZSAocGFydEluZGV4ICE9PSAtMSAmJiBwYXJ0c1twYXJ0SW5kZXhdLmluZGV4ID09PSB3YWxrZXJJbmRleCkge1xuICAgICAgICAgICAgLy8gSWYgd2UndmUgaW5zZXJ0ZWQgdGhlIG5vZGUsIHNpbXBseSBhZGp1c3QgYWxsIHN1YnNlcXVlbnQgcGFydHNcbiAgICAgICAgICAgIGlmIChpbnNlcnRDb3VudCA+IDApIHtcbiAgICAgICAgICAgICAgICB3aGlsZSAocGFydEluZGV4ICE9PSAtMSkge1xuICAgICAgICAgICAgICAgICAgICBwYXJ0c1twYXJ0SW5kZXhdLmluZGV4ICs9IGluc2VydENvdW50O1xuICAgICAgICAgICAgICAgICAgICBwYXJ0SW5kZXggPSBuZXh0QWN0aXZlSW5kZXhJblRlbXBsYXRlUGFydHMocGFydHMsIHBhcnRJbmRleCk7XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgICAgIH1cbiAgICAgICAgICAgIHBhcnRJbmRleCA9IG5leHRBY3RpdmVJbmRleEluVGVtcGxhdGVQYXJ0cyhwYXJ0cywgcGFydEluZGV4KTtcbiAgICAgICAgfVxuICAgIH1cbn1cbi8vIyBzb3VyY2VNYXBwaW5nVVJMPW1vZGlmeS10ZW1wbGF0ZS5qcy5tYXAiLCIvKipcbiAqIEBsaWNlbnNlXG4gKiBDb3B5cmlnaHQgKGMpIDIwMTggVGhlIFBvbHltZXIgUHJvamVjdCBBdXRob3JzLiBBbGwgcmlnaHRzIHJlc2VydmVkLlxuICogVGhpcyBjb2RlIG1heSBvbmx5IGJlIHVzZWQgdW5kZXIgdGhlIEJTRCBzdHlsZSBsaWNlbnNlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vTElDRU5TRS50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgYXV0aG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9BVVRIT1JTLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBjb250cmlidXRvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQ09OVFJJQlVUT1JTLnR4dFxuICogQ29kZSBkaXN0cmlidXRlZCBieSBHb29nbGUgYXMgcGFydCBvZiB0aGUgcG9seW1lciBwcm9qZWN0IGlzIGFsc29cbiAqIHN1YmplY3QgdG8gYW4gYWRkaXRpb25hbCBJUCByaWdodHMgZ3JhbnQgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9QQVRFTlRTLnR4dFxuICovXG4vKipcbiAqIEEgc2VudGluZWwgdmFsdWUgdGhhdCBzaWduYWxzIHRoYXQgYSB2YWx1ZSB3YXMgaGFuZGxlZCBieSBhIGRpcmVjdGl2ZSBhbmRcbiAqIHNob3VsZCBub3QgYmUgd3JpdHRlbiB0byB0aGUgRE9NLlxuICovXG5leHBvcnQgY29uc3Qgbm9DaGFuZ2UgPSB7fTtcbi8qKlxuICogQSBzZW50aW5lbCB2YWx1ZSB0aGF0IHNpZ25hbHMgYSBOb2RlUGFydCB0byBmdWxseSBjbGVhciBpdHMgY29udGVudC5cbiAqL1xuZXhwb3J0IGNvbnN0IG5vdGhpbmcgPSB7fTtcbi8vIyBzb3VyY2VNYXBwaW5nVVJMPXBhcnQuanMubWFwIiwiLyoqXG4gKiBAbGljZW5zZVxuICogQ29weXJpZ2h0IChjKSAyMDE3IFRoZSBQb2x5bWVyIFByb2plY3QgQXV0aG9ycy4gQWxsIHJpZ2h0cyByZXNlcnZlZC5cbiAqIFRoaXMgY29kZSBtYXkgb25seSBiZSB1c2VkIHVuZGVyIHRoZSBCU0Qgc3R5bGUgbGljZW5zZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0xJQ0VOU0UudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGF1dGhvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQVVUSE9SUy50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgY29udHJpYnV0b3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0NPTlRSSUJVVE9SUy50eHRcbiAqIENvZGUgZGlzdHJpYnV0ZWQgYnkgR29vZ2xlIGFzIHBhcnQgb2YgdGhlIHBvbHltZXIgcHJvamVjdCBpcyBhbHNvXG4gKiBzdWJqZWN0IHRvIGFuIGFkZGl0aW9uYWwgSVAgcmlnaHRzIGdyYW50IGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vUEFURU5UUy50eHRcbiAqL1xuLyoqXG4gKiBAbW9kdWxlIGxpdC1odG1sXG4gKi9cbmltcG9ydCB7IGlzRGlyZWN0aXZlIH0gZnJvbSAnLi9kaXJlY3RpdmUuanMnO1xuaW1wb3J0IHsgcmVtb3ZlTm9kZXMgfSBmcm9tICcuL2RvbS5qcyc7XG5pbXBvcnQgeyBub0NoYW5nZSwgbm90aGluZyB9IGZyb20gJy4vcGFydC5qcyc7XG5pbXBvcnQgeyBUZW1wbGF0ZUluc3RhbmNlIH0gZnJvbSAnLi90ZW1wbGF0ZS1pbnN0YW5jZS5qcyc7XG5pbXBvcnQgeyBUZW1wbGF0ZVJlc3VsdCB9IGZyb20gJy4vdGVtcGxhdGUtcmVzdWx0LmpzJztcbmltcG9ydCB7IGNyZWF0ZU1hcmtlciB9IGZyb20gJy4vdGVtcGxhdGUuanMnO1xuZXhwb3J0IGNvbnN0IGlzUHJpbWl0aXZlID0gKHZhbHVlKSA9PiB7XG4gICAgcmV0dXJuICh2YWx1ZSA9PT0gbnVsbCB8fFxuICAgICAgICAhKHR5cGVvZiB2YWx1ZSA9PT0gJ29iamVjdCcgfHwgdHlwZW9mIHZhbHVlID09PSAnZnVuY3Rpb24nKSk7XG59O1xuLyoqXG4gKiBTZXRzIGF0dHJpYnV0ZSB2YWx1ZXMgZm9yIEF0dHJpYnV0ZVBhcnRzLCBzbyB0aGF0IHRoZSB2YWx1ZSBpcyBvbmx5IHNldCBvbmNlXG4gKiBldmVuIGlmIHRoZXJlIGFyZSBtdWx0aXBsZSBwYXJ0cyBmb3IgYW4gYXR0cmlidXRlLlxuICovXG5leHBvcnQgY2xhc3MgQXR0cmlidXRlQ29tbWl0dGVyIHtcbiAgICBjb25zdHJ1Y3RvcihlbGVtZW50LCBuYW1lLCBzdHJpbmdzKSB7XG4gICAgICAgIHRoaXMuZGlydHkgPSB0cnVlO1xuICAgICAgICB0aGlzLmVsZW1lbnQgPSBlbGVtZW50O1xuICAgICAgICB0aGlzLm5hbWUgPSBuYW1lO1xuICAgICAgICB0aGlzLnN0cmluZ3MgPSBzdHJpbmdzO1xuICAgICAgICB0aGlzLnBhcnRzID0gW107XG4gICAgICAgIGZvciAobGV0IGkgPSAwOyBpIDwgc3RyaW5ncy5sZW5ndGggLSAxOyBpKyspIHtcbiAgICAgICAgICAgIHRoaXMucGFydHNbaV0gPSB0aGlzLl9jcmVhdGVQYXJ0KCk7XG4gICAgICAgIH1cbiAgICB9XG4gICAgLyoqXG4gICAgICogQ3JlYXRlcyBhIHNpbmdsZSBwYXJ0LiBPdmVycmlkZSB0aGlzIHRvIGNyZWF0ZSBhIGRpZmZlcm50IHR5cGUgb2YgcGFydC5cbiAgICAgKi9cbiAgICBfY3JlYXRlUGFydCgpIHtcbiAgICAgICAgcmV0dXJuIG5ldyBBdHRyaWJ1dGVQYXJ0KHRoaXMpO1xuICAgIH1cbiAgICBfZ2V0VmFsdWUoKSB7XG4gICAgICAgIGNvbnN0IHN0cmluZ3MgPSB0aGlzLnN0cmluZ3M7XG4gICAgICAgIGNvbnN0IGwgPSBzdHJpbmdzLmxlbmd0aCAtIDE7XG4gICAgICAgIGxldCB0ZXh0ID0gJyc7XG4gICAgICAgIGZvciAobGV0IGkgPSAwOyBpIDwgbDsgaSsrKSB7XG4gICAgICAgICAgICB0ZXh0ICs9IHN0cmluZ3NbaV07XG4gICAgICAgICAgICBjb25zdCBwYXJ0ID0gdGhpcy5wYXJ0c1tpXTtcbiAgICAgICAgICAgIGlmIChwYXJ0ICE9PSB1bmRlZmluZWQpIHtcbiAgICAgICAgICAgICAgICBjb25zdCB2ID0gcGFydC52YWx1ZTtcbiAgICAgICAgICAgICAgICBpZiAodiAhPSBudWxsICYmXG4gICAgICAgICAgICAgICAgICAgIChBcnJheS5pc0FycmF5KHYpIHx8XG4gICAgICAgICAgICAgICAgICAgICAgICAvLyB0c2xpbnQ6ZGlzYWJsZS1uZXh0LWxpbmU6bm8tYW55XG4gICAgICAgICAgICAgICAgICAgICAgICB0eXBlb2YgdiAhPT0gJ3N0cmluZycgJiYgdltTeW1ib2wuaXRlcmF0b3JdKSkge1xuICAgICAgICAgICAgICAgICAgICBmb3IgKGNvbnN0IHQgb2Ygdikge1xuICAgICAgICAgICAgICAgICAgICAgICAgdGV4dCArPSB0eXBlb2YgdCA9PT0gJ3N0cmluZycgPyB0IDogU3RyaW5nKHQpO1xuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIGVsc2Uge1xuICAgICAgICAgICAgICAgICAgICB0ZXh0ICs9IHR5cGVvZiB2ID09PSAnc3RyaW5nJyA/IHYgOiBTdHJpbmcodik7XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgfVxuICAgICAgICB9XG4gICAgICAgIHRleHQgKz0gc3RyaW5nc1tsXTtcbiAgICAgICAgcmV0dXJuIHRleHQ7XG4gICAgfVxuICAgIGNvbW1pdCgpIHtcbiAgICAgICAgaWYgKHRoaXMuZGlydHkpIHtcbiAgICAgICAgICAgIHRoaXMuZGlydHkgPSBmYWxzZTtcbiAgICAgICAgICAgIHRoaXMuZWxlbWVudC5zZXRBdHRyaWJ1dGUodGhpcy5uYW1lLCB0aGlzLl9nZXRWYWx1ZSgpKTtcbiAgICAgICAgfVxuICAgIH1cbn1cbmV4cG9ydCBjbGFzcyBBdHRyaWJ1dGVQYXJ0IHtcbiAgICBjb25zdHJ1Y3Rvcihjb21pdHRlcikge1xuICAgICAgICB0aGlzLnZhbHVlID0gdW5kZWZpbmVkO1xuICAgICAgICB0aGlzLmNvbW1pdHRlciA9IGNvbWl0dGVyO1xuICAgIH1cbiAgICBzZXRWYWx1ZSh2YWx1ZSkge1xuICAgICAgICBpZiAodmFsdWUgIT09IG5vQ2hhbmdlICYmICghaXNQcmltaXRpdmUodmFsdWUpIHx8IHZhbHVlICE9PSB0aGlzLnZhbHVlKSkge1xuICAgICAgICAgICAgdGhpcy52YWx1ZSA9IHZhbHVlO1xuICAgICAgICAgICAgLy8gSWYgdGhlIHZhbHVlIGlzIGEgbm90IGEgZGlyZWN0aXZlLCBkaXJ0eSB0aGUgY29tbWl0dGVyIHNvIHRoYXQgaXQnbGxcbiAgICAgICAgICAgIC8vIGNhbGwgc2V0QXR0cmlidXRlLiBJZiB0aGUgdmFsdWUgaXMgYSBkaXJlY3RpdmUsIGl0J2xsIGRpcnR5IHRoZVxuICAgICAgICAgICAgLy8gY29tbWl0dGVyIGlmIGl0IGNhbGxzIHNldFZhbHVlKCkuXG4gICAgICAgICAgICBpZiAoIWlzRGlyZWN0aXZlKHZhbHVlKSkge1xuICAgICAgICAgICAgICAgIHRoaXMuY29tbWl0dGVyLmRpcnR5ID0gdHJ1ZTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgIH1cbiAgICBjb21taXQoKSB7XG4gICAgICAgIHdoaWxlIChpc0RpcmVjdGl2ZSh0aGlzLnZhbHVlKSkge1xuICAgICAgICAgICAgY29uc3QgZGlyZWN0aXZlID0gdGhpcy52YWx1ZTtcbiAgICAgICAgICAgIHRoaXMudmFsdWUgPSBub0NoYW5nZTtcbiAgICAgICAgICAgIGRpcmVjdGl2ZSh0aGlzKTtcbiAgICAgICAgfVxuICAgICAgICBpZiAodGhpcy52YWx1ZSA9PT0gbm9DaGFuZ2UpIHtcbiAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgfVxuICAgICAgICB0aGlzLmNvbW1pdHRlci5jb21taXQoKTtcbiAgICB9XG59XG5leHBvcnQgY2xhc3MgTm9kZVBhcnQge1xuICAgIGNvbnN0cnVjdG9yKG9wdGlvbnMpIHtcbiAgICAgICAgdGhpcy52YWx1ZSA9IHVuZGVmaW5lZDtcbiAgICAgICAgdGhpcy5fcGVuZGluZ1ZhbHVlID0gdW5kZWZpbmVkO1xuICAgICAgICB0aGlzLm9wdGlvbnMgPSBvcHRpb25zO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBJbnNlcnRzIHRoaXMgcGFydCBpbnRvIGEgY29udGFpbmVyLlxuICAgICAqXG4gICAgICogVGhpcyBwYXJ0IG11c3QgYmUgZW1wdHksIGFzIGl0cyBjb250ZW50cyBhcmUgbm90IGF1dG9tYXRpY2FsbHkgbW92ZWQuXG4gICAgICovXG4gICAgYXBwZW5kSW50byhjb250YWluZXIpIHtcbiAgICAgICAgdGhpcy5zdGFydE5vZGUgPSBjb250YWluZXIuYXBwZW5kQ2hpbGQoY3JlYXRlTWFya2VyKCkpO1xuICAgICAgICB0aGlzLmVuZE5vZGUgPSBjb250YWluZXIuYXBwZW5kQ2hpbGQoY3JlYXRlTWFya2VyKCkpO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBJbnNlcnRzIHRoaXMgcGFydCBiZXR3ZWVuIGByZWZgIGFuZCBgcmVmYCdzIG5leHQgc2libGluZy4gQm90aCBgcmVmYCBhbmRcbiAgICAgKiBpdHMgbmV4dCBzaWJsaW5nIG11c3QgYmUgc3RhdGljLCB1bmNoYW5naW5nIG5vZGVzIHN1Y2ggYXMgdGhvc2UgdGhhdCBhcHBlYXJcbiAgICAgKiBpbiBhIGxpdGVyYWwgc2VjdGlvbiBvZiBhIHRlbXBsYXRlLlxuICAgICAqXG4gICAgICogVGhpcyBwYXJ0IG11c3QgYmUgZW1wdHksIGFzIGl0cyBjb250ZW50cyBhcmUgbm90IGF1dG9tYXRpY2FsbHkgbW92ZWQuXG4gICAgICovXG4gICAgaW5zZXJ0QWZ0ZXJOb2RlKHJlZikge1xuICAgICAgICB0aGlzLnN0YXJ0Tm9kZSA9IHJlZjtcbiAgICAgICAgdGhpcy5lbmROb2RlID0gcmVmLm5leHRTaWJsaW5nO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBBcHBlbmRzIHRoaXMgcGFydCBpbnRvIGEgcGFyZW50IHBhcnQuXG4gICAgICpcbiAgICAgKiBUaGlzIHBhcnQgbXVzdCBiZSBlbXB0eSwgYXMgaXRzIGNvbnRlbnRzIGFyZSBub3QgYXV0b21hdGljYWxseSBtb3ZlZC5cbiAgICAgKi9cbiAgICBhcHBlbmRJbnRvUGFydChwYXJ0KSB7XG4gICAgICAgIHBhcnQuX2luc2VydCh0aGlzLnN0YXJ0Tm9kZSA9IGNyZWF0ZU1hcmtlcigpKTtcbiAgICAgICAgcGFydC5faW5zZXJ0KHRoaXMuZW5kTm9kZSA9IGNyZWF0ZU1hcmtlcigpKTtcbiAgICB9XG4gICAgLyoqXG4gICAgICogQXBwZW5kcyB0aGlzIHBhcnQgYWZ0ZXIgYHJlZmBcbiAgICAgKlxuICAgICAqIFRoaXMgcGFydCBtdXN0IGJlIGVtcHR5LCBhcyBpdHMgY29udGVudHMgYXJlIG5vdCBhdXRvbWF0aWNhbGx5IG1vdmVkLlxuICAgICAqL1xuICAgIGluc2VydEFmdGVyUGFydChyZWYpIHtcbiAgICAgICAgcmVmLl9pbnNlcnQodGhpcy5zdGFydE5vZGUgPSBjcmVhdGVNYXJrZXIoKSk7XG4gICAgICAgIHRoaXMuZW5kTm9kZSA9IHJlZi5lbmROb2RlO1xuICAgICAgICByZWYuZW5kTm9kZSA9IHRoaXMuc3RhcnROb2RlO1xuICAgIH1cbiAgICBzZXRWYWx1ZSh2YWx1ZSkge1xuICAgICAgICB0aGlzLl9wZW5kaW5nVmFsdWUgPSB2YWx1ZTtcbiAgICB9XG4gICAgY29tbWl0KCkge1xuICAgICAgICB3aGlsZSAoaXNEaXJlY3RpdmUodGhpcy5fcGVuZGluZ1ZhbHVlKSkge1xuICAgICAgICAgICAgY29uc3QgZGlyZWN0aXZlID0gdGhpcy5fcGVuZGluZ1ZhbHVlO1xuICAgICAgICAgICAgdGhpcy5fcGVuZGluZ1ZhbHVlID0gbm9DaGFuZ2U7XG4gICAgICAgICAgICBkaXJlY3RpdmUodGhpcyk7XG4gICAgICAgIH1cbiAgICAgICAgY29uc3QgdmFsdWUgPSB0aGlzLl9wZW5kaW5nVmFsdWU7XG4gICAgICAgIGlmICh2YWx1ZSA9PT0gbm9DaGFuZ2UpIHtcbiAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgfVxuICAgICAgICBpZiAoaXNQcmltaXRpdmUodmFsdWUpKSB7XG4gICAgICAgICAgICBpZiAodmFsdWUgIT09IHRoaXMudmFsdWUpIHtcbiAgICAgICAgICAgICAgICB0aGlzLl9jb21taXRUZXh0KHZhbHVlKTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgICAgICBlbHNlIGlmICh2YWx1ZSBpbnN0YW5jZW9mIFRlbXBsYXRlUmVzdWx0KSB7XG4gICAgICAgICAgICB0aGlzLl9jb21taXRUZW1wbGF0ZVJlc3VsdCh2YWx1ZSk7XG4gICAgICAgIH1cbiAgICAgICAgZWxzZSBpZiAodmFsdWUgaW5zdGFuY2VvZiBOb2RlKSB7XG4gICAgICAgICAgICB0aGlzLl9jb21taXROb2RlKHZhbHVlKTtcbiAgICAgICAgfVxuICAgICAgICBlbHNlIGlmIChBcnJheS5pc0FycmF5KHZhbHVlKSB8fFxuICAgICAgICAgICAgLy8gdHNsaW50OmRpc2FibGUtbmV4dC1saW5lOm5vLWFueVxuICAgICAgICAgICAgdmFsdWVbU3ltYm9sLml0ZXJhdG9yXSkge1xuICAgICAgICAgICAgdGhpcy5fY29tbWl0SXRlcmFibGUodmFsdWUpO1xuICAgICAgICB9XG4gICAgICAgIGVsc2UgaWYgKHZhbHVlID09PSBub3RoaW5nKSB7XG4gICAgICAgICAgICB0aGlzLnZhbHVlID0gbm90aGluZztcbiAgICAgICAgICAgIHRoaXMuY2xlYXIoKTtcbiAgICAgICAgfVxuICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgIC8vIEZhbGxiYWNrLCB3aWxsIHJlbmRlciB0aGUgc3RyaW5nIHJlcHJlc2VudGF0aW9uXG4gICAgICAgICAgICB0aGlzLl9jb21taXRUZXh0KHZhbHVlKTtcbiAgICAgICAgfVxuICAgIH1cbiAgICBfaW5zZXJ0KG5vZGUpIHtcbiAgICAgICAgdGhpcy5lbmROb2RlLnBhcmVudE5vZGUuaW5zZXJ0QmVmb3JlKG5vZGUsIHRoaXMuZW5kTm9kZSk7XG4gICAgfVxuICAgIF9jb21taXROb2RlKHZhbHVlKSB7XG4gICAgICAgIGlmICh0aGlzLnZhbHVlID09PSB2YWx1ZSkge1xuICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICB9XG4gICAgICAgIHRoaXMuY2xlYXIoKTtcbiAgICAgICAgdGhpcy5faW5zZXJ0KHZhbHVlKTtcbiAgICAgICAgdGhpcy52YWx1ZSA9IHZhbHVlO1xuICAgIH1cbiAgICBfY29tbWl0VGV4dCh2YWx1ZSkge1xuICAgICAgICBjb25zdCBub2RlID0gdGhpcy5zdGFydE5vZGUubmV4dFNpYmxpbmc7XG4gICAgICAgIHZhbHVlID0gdmFsdWUgPT0gbnVsbCA/ICcnIDogdmFsdWU7XG4gICAgICAgIGlmIChub2RlID09PSB0aGlzLmVuZE5vZGUucHJldmlvdXNTaWJsaW5nICYmXG4gICAgICAgICAgICBub2RlLm5vZGVUeXBlID09PSAzIC8qIE5vZGUuVEVYVF9OT0RFICovKSB7XG4gICAgICAgICAgICAvLyBJZiB3ZSBvbmx5IGhhdmUgYSBzaW5nbGUgdGV4dCBub2RlIGJldHdlZW4gdGhlIG1hcmtlcnMsIHdlIGNhbiBqdXN0XG4gICAgICAgICAgICAvLyBzZXQgaXRzIHZhbHVlLCByYXRoZXIgdGhhbiByZXBsYWNpbmcgaXQuXG4gICAgICAgICAgICAvLyBUT0RPKGp1c3RpbmZhZ25hbmkpOiBDYW4gd2UganVzdCBjaGVjayBpZiB0aGlzLnZhbHVlIGlzIHByaW1pdGl2ZT9cbiAgICAgICAgICAgIG5vZGUuZGF0YSA9IHZhbHVlO1xuICAgICAgICB9XG4gICAgICAgIGVsc2Uge1xuICAgICAgICAgICAgdGhpcy5fY29tbWl0Tm9kZShkb2N1bWVudC5jcmVhdGVUZXh0Tm9kZSh0eXBlb2YgdmFsdWUgPT09ICdzdHJpbmcnID8gdmFsdWUgOiBTdHJpbmcodmFsdWUpKSk7XG4gICAgICAgIH1cbiAgICAgICAgdGhpcy52YWx1ZSA9IHZhbHVlO1xuICAgIH1cbiAgICBfY29tbWl0VGVtcGxhdGVSZXN1bHQodmFsdWUpIHtcbiAgICAgICAgY29uc3QgdGVtcGxhdGUgPSB0aGlzLm9wdGlvbnMudGVtcGxhdGVGYWN0b3J5KHZhbHVlKTtcbiAgICAgICAgaWYgKHRoaXMudmFsdWUgaW5zdGFuY2VvZiBUZW1wbGF0ZUluc3RhbmNlICYmXG4gICAgICAgICAgICB0aGlzLnZhbHVlLnRlbXBsYXRlID09PSB0ZW1wbGF0ZSkge1xuICAgICAgICAgICAgdGhpcy52YWx1ZS51cGRhdGUodmFsdWUudmFsdWVzKTtcbiAgICAgICAgfVxuICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgIC8vIE1ha2Ugc3VyZSB3ZSBwcm9wYWdhdGUgdGhlIHRlbXBsYXRlIHByb2Nlc3NvciBmcm9tIHRoZSBUZW1wbGF0ZVJlc3VsdFxuICAgICAgICAgICAgLy8gc28gdGhhdCB3ZSB1c2UgaXRzIHN5bnRheCBleHRlbnNpb24sIGV0Yy4gVGhlIHRlbXBsYXRlIGZhY3RvcnkgY29tZXNcbiAgICAgICAgICAgIC8vIGZyb20gdGhlIHJlbmRlciBmdW5jdGlvbiBvcHRpb25zIHNvIHRoYXQgaXQgY2FuIGNvbnRyb2wgdGVtcGxhdGVcbiAgICAgICAgICAgIC8vIGNhY2hpbmcgYW5kIHByZXByb2Nlc3NpbmcuXG4gICAgICAgICAgICBjb25zdCBpbnN0YW5jZSA9IG5ldyBUZW1wbGF0ZUluc3RhbmNlKHRlbXBsYXRlLCB2YWx1ZS5wcm9jZXNzb3IsIHRoaXMub3B0aW9ucyk7XG4gICAgICAgICAgICBjb25zdCBmcmFnbWVudCA9IGluc3RhbmNlLl9jbG9uZSgpO1xuICAgICAgICAgICAgaW5zdGFuY2UudXBkYXRlKHZhbHVlLnZhbHVlcyk7XG4gICAgICAgICAgICB0aGlzLl9jb21taXROb2RlKGZyYWdtZW50KTtcbiAgICAgICAgICAgIHRoaXMudmFsdWUgPSBpbnN0YW5jZTtcbiAgICAgICAgfVxuICAgIH1cbiAgICBfY29tbWl0SXRlcmFibGUodmFsdWUpIHtcbiAgICAgICAgLy8gRm9yIGFuIEl0ZXJhYmxlLCB3ZSBjcmVhdGUgYSBuZXcgSW5zdGFuY2VQYXJ0IHBlciBpdGVtLCB0aGVuIHNldCBpdHNcbiAgICAgICAgLy8gdmFsdWUgdG8gdGhlIGl0ZW0uIFRoaXMgaXMgYSBsaXR0bGUgYml0IG9mIG92ZXJoZWFkIGZvciBldmVyeSBpdGVtIGluXG4gICAgICAgIC8vIGFuIEl0ZXJhYmxlLCBidXQgaXQgbGV0cyB1cyByZWN1cnNlIGVhc2lseSBhbmQgZWZmaWNpZW50bHkgdXBkYXRlIEFycmF5c1xuICAgICAgICAvLyBvZiBUZW1wbGF0ZVJlc3VsdHMgdGhhdCB3aWxsIGJlIGNvbW1vbmx5IHJldHVybmVkIGZyb20gZXhwcmVzc2lvbnMgbGlrZTpcbiAgICAgICAgLy8gYXJyYXkubWFwKChpKSA9PiBodG1sYCR7aX1gKSwgYnkgcmV1c2luZyBleGlzdGluZyBUZW1wbGF0ZUluc3RhbmNlcy5cbiAgICAgICAgLy8gSWYgX3ZhbHVlIGlzIGFuIGFycmF5LCB0aGVuIHRoZSBwcmV2aW91cyByZW5kZXIgd2FzIG9mIGFuXG4gICAgICAgIC8vIGl0ZXJhYmxlIGFuZCBfdmFsdWUgd2lsbCBjb250YWluIHRoZSBOb2RlUGFydHMgZnJvbSB0aGUgcHJldmlvdXNcbiAgICAgICAgLy8gcmVuZGVyLiBJZiBfdmFsdWUgaXMgbm90IGFuIGFycmF5LCBjbGVhciB0aGlzIHBhcnQgYW5kIG1ha2UgYSBuZXdcbiAgICAgICAgLy8gYXJyYXkgZm9yIE5vZGVQYXJ0cy5cbiAgICAgICAgaWYgKCFBcnJheS5pc0FycmF5KHRoaXMudmFsdWUpKSB7XG4gICAgICAgICAgICB0aGlzLnZhbHVlID0gW107XG4gICAgICAgICAgICB0aGlzLmNsZWFyKCk7XG4gICAgICAgIH1cbiAgICAgICAgLy8gTGV0cyB1cyBrZWVwIHRyYWNrIG9mIGhvdyBtYW55IGl0ZW1zIHdlIHN0YW1wZWQgc28gd2UgY2FuIGNsZWFyIGxlZnRvdmVyXG4gICAgICAgIC8vIGl0ZW1zIGZyb20gYSBwcmV2aW91cyByZW5kZXJcbiAgICAgICAgY29uc3QgaXRlbVBhcnRzID0gdGhpcy52YWx1ZTtcbiAgICAgICAgbGV0IHBhcnRJbmRleCA9IDA7XG4gICAgICAgIGxldCBpdGVtUGFydDtcbiAgICAgICAgZm9yIChjb25zdCBpdGVtIG9mIHZhbHVlKSB7XG4gICAgICAgICAgICAvLyBUcnkgdG8gcmV1c2UgYW4gZXhpc3RpbmcgcGFydFxuICAgICAgICAgICAgaXRlbVBhcnQgPSBpdGVtUGFydHNbcGFydEluZGV4XTtcbiAgICAgICAgICAgIC8vIElmIG5vIGV4aXN0aW5nIHBhcnQsIGNyZWF0ZSBhIG5ldyBvbmVcbiAgICAgICAgICAgIGlmIChpdGVtUGFydCA9PT0gdW5kZWZpbmVkKSB7XG4gICAgICAgICAgICAgICAgaXRlbVBhcnQgPSBuZXcgTm9kZVBhcnQodGhpcy5vcHRpb25zKTtcbiAgICAgICAgICAgICAgICBpdGVtUGFydHMucHVzaChpdGVtUGFydCk7XG4gICAgICAgICAgICAgICAgaWYgKHBhcnRJbmRleCA9PT0gMCkge1xuICAgICAgICAgICAgICAgICAgICBpdGVtUGFydC5hcHBlbmRJbnRvUGFydCh0aGlzKTtcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgZWxzZSB7XG4gICAgICAgICAgICAgICAgICAgIGl0ZW1QYXJ0Lmluc2VydEFmdGVyUGFydChpdGVtUGFydHNbcGFydEluZGV4IC0gMV0pO1xuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH1cbiAgICAgICAgICAgIGl0ZW1QYXJ0LnNldFZhbHVlKGl0ZW0pO1xuICAgICAgICAgICAgaXRlbVBhcnQuY29tbWl0KCk7XG4gICAgICAgICAgICBwYXJ0SW5kZXgrKztcbiAgICAgICAgfVxuICAgICAgICBpZiAocGFydEluZGV4IDwgaXRlbVBhcnRzLmxlbmd0aCkge1xuICAgICAgICAgICAgLy8gVHJ1bmNhdGUgdGhlIHBhcnRzIGFycmF5IHNvIF92YWx1ZSByZWZsZWN0cyB0aGUgY3VycmVudCBzdGF0ZVxuICAgICAgICAgICAgaXRlbVBhcnRzLmxlbmd0aCA9IHBhcnRJbmRleDtcbiAgICAgICAgICAgIHRoaXMuY2xlYXIoaXRlbVBhcnQgJiYgaXRlbVBhcnQuZW5kTm9kZSk7XG4gICAgICAgIH1cbiAgICB9XG4gICAgY2xlYXIoc3RhcnROb2RlID0gdGhpcy5zdGFydE5vZGUpIHtcbiAgICAgICAgcmVtb3ZlTm9kZXModGhpcy5zdGFydE5vZGUucGFyZW50Tm9kZSwgc3RhcnROb2RlLm5leHRTaWJsaW5nLCB0aGlzLmVuZE5vZGUpO1xuICAgIH1cbn1cbi8qKlxuICogSW1wbGVtZW50cyBhIGJvb2xlYW4gYXR0cmlidXRlLCByb3VnaGx5IGFzIGRlZmluZWQgaW4gdGhlIEhUTUxcbiAqIHNwZWNpZmljYXRpb24uXG4gKlxuICogSWYgdGhlIHZhbHVlIGlzIHRydXRoeSwgdGhlbiB0aGUgYXR0cmlidXRlIGlzIHByZXNlbnQgd2l0aCBhIHZhbHVlIG9mXG4gKiAnJy4gSWYgdGhlIHZhbHVlIGlzIGZhbHNleSwgdGhlIGF0dHJpYnV0ZSBpcyByZW1vdmVkLlxuICovXG5leHBvcnQgY2xhc3MgQm9vbGVhbkF0dHJpYnV0ZVBhcnQge1xuICAgIGNvbnN0cnVjdG9yKGVsZW1lbnQsIG5hbWUsIHN0cmluZ3MpIHtcbiAgICAgICAgdGhpcy52YWx1ZSA9IHVuZGVmaW5lZDtcbiAgICAgICAgdGhpcy5fcGVuZGluZ1ZhbHVlID0gdW5kZWZpbmVkO1xuICAgICAgICBpZiAoc3RyaW5ncy5sZW5ndGggIT09IDIgfHwgc3RyaW5nc1swXSAhPT0gJycgfHwgc3RyaW5nc1sxXSAhPT0gJycpIHtcbiAgICAgICAgICAgIHRocm93IG5ldyBFcnJvcignQm9vbGVhbiBhdHRyaWJ1dGVzIGNhbiBvbmx5IGNvbnRhaW4gYSBzaW5nbGUgZXhwcmVzc2lvbicpO1xuICAgICAgICB9XG4gICAgICAgIHRoaXMuZWxlbWVudCA9IGVsZW1lbnQ7XG4gICAgICAgIHRoaXMubmFtZSA9IG5hbWU7XG4gICAgICAgIHRoaXMuc3RyaW5ncyA9IHN0cmluZ3M7XG4gICAgfVxuICAgIHNldFZhbHVlKHZhbHVlKSB7XG4gICAgICAgIHRoaXMuX3BlbmRpbmdWYWx1ZSA9IHZhbHVlO1xuICAgIH1cbiAgICBjb21taXQoKSB7XG4gICAgICAgIHdoaWxlIChpc0RpcmVjdGl2ZSh0aGlzLl9wZW5kaW5nVmFsdWUpKSB7XG4gICAgICAgICAgICBjb25zdCBkaXJlY3RpdmUgPSB0aGlzLl9wZW5kaW5nVmFsdWU7XG4gICAgICAgICAgICB0aGlzLl9wZW5kaW5nVmFsdWUgPSBub0NoYW5nZTtcbiAgICAgICAgICAgIGRpcmVjdGl2ZSh0aGlzKTtcbiAgICAgICAgfVxuICAgICAgICBpZiAodGhpcy5fcGVuZGluZ1ZhbHVlID09PSBub0NoYW5nZSkge1xuICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICB9XG4gICAgICAgIGNvbnN0IHZhbHVlID0gISF0aGlzLl9wZW5kaW5nVmFsdWU7XG4gICAgICAgIGlmICh0aGlzLnZhbHVlICE9PSB2YWx1ZSkge1xuICAgICAgICAgICAgaWYgKHZhbHVlKSB7XG4gICAgICAgICAgICAgICAgdGhpcy5lbGVtZW50LnNldEF0dHJpYnV0ZSh0aGlzLm5hbWUsICcnKTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgICAgIGVsc2Uge1xuICAgICAgICAgICAgICAgIHRoaXMuZWxlbWVudC5yZW1vdmVBdHRyaWJ1dGUodGhpcy5uYW1lKTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgICAgICB0aGlzLnZhbHVlID0gdmFsdWU7XG4gICAgICAgIHRoaXMuX3BlbmRpbmdWYWx1ZSA9IG5vQ2hhbmdlO1xuICAgIH1cbn1cbi8qKlxuICogU2V0cyBhdHRyaWJ1dGUgdmFsdWVzIGZvciBQcm9wZXJ0eVBhcnRzLCBzbyB0aGF0IHRoZSB2YWx1ZSBpcyBvbmx5IHNldCBvbmNlXG4gKiBldmVuIGlmIHRoZXJlIGFyZSBtdWx0aXBsZSBwYXJ0cyBmb3IgYSBwcm9wZXJ0eS5cbiAqXG4gKiBJZiBhbiBleHByZXNzaW9uIGNvbnRyb2xzIHRoZSB3aG9sZSBwcm9wZXJ0eSB2YWx1ZSwgdGhlbiB0aGUgdmFsdWUgaXMgc2ltcGx5XG4gKiBhc3NpZ25lZCB0byB0aGUgcHJvcGVydHkgdW5kZXIgY29udHJvbC4gSWYgdGhlcmUgYXJlIHN0cmluZyBsaXRlcmFscyBvclxuICogbXVsdGlwbGUgZXhwcmVzc2lvbnMsIHRoZW4gdGhlIHN0cmluZ3MgYXJlIGV4cHJlc3Npb25zIGFyZSBpbnRlcnBvbGF0ZWQgaW50b1xuICogYSBzdHJpbmcgZmlyc3QuXG4gKi9cbmV4cG9ydCBjbGFzcyBQcm9wZXJ0eUNvbW1pdHRlciBleHRlbmRzIEF0dHJpYnV0ZUNvbW1pdHRlciB7XG4gICAgY29uc3RydWN0b3IoZWxlbWVudCwgbmFtZSwgc3RyaW5ncykge1xuICAgICAgICBzdXBlcihlbGVtZW50LCBuYW1lLCBzdHJpbmdzKTtcbiAgICAgICAgdGhpcy5zaW5nbGUgPVxuICAgICAgICAgICAgKHN0cmluZ3MubGVuZ3RoID09PSAyICYmIHN0cmluZ3NbMF0gPT09ICcnICYmIHN0cmluZ3NbMV0gPT09ICcnKTtcbiAgICB9XG4gICAgX2NyZWF0ZVBhcnQoKSB7XG4gICAgICAgIHJldHVybiBuZXcgUHJvcGVydHlQYXJ0KHRoaXMpO1xuICAgIH1cbiAgICBfZ2V0VmFsdWUoKSB7XG4gICAgICAgIGlmICh0aGlzLnNpbmdsZSkge1xuICAgICAgICAgICAgcmV0dXJuIHRoaXMucGFydHNbMF0udmFsdWU7XG4gICAgICAgIH1cbiAgICAgICAgcmV0dXJuIHN1cGVyLl9nZXRWYWx1ZSgpO1xuICAgIH1cbiAgICBjb21taXQoKSB7XG4gICAgICAgIGlmICh0aGlzLmRpcnR5KSB7XG4gICAgICAgICAgICB0aGlzLmRpcnR5ID0gZmFsc2U7XG4gICAgICAgICAgICAvLyB0c2xpbnQ6ZGlzYWJsZS1uZXh0LWxpbmU6bm8tYW55XG4gICAgICAgICAgICB0aGlzLmVsZW1lbnRbdGhpcy5uYW1lXSA9IHRoaXMuX2dldFZhbHVlKCk7XG4gICAgICAgIH1cbiAgICB9XG59XG5leHBvcnQgY2xhc3MgUHJvcGVydHlQYXJ0IGV4dGVuZHMgQXR0cmlidXRlUGFydCB7XG59XG4vLyBEZXRlY3QgZXZlbnQgbGlzdGVuZXIgb3B0aW9ucyBzdXBwb3J0LiBJZiB0aGUgYGNhcHR1cmVgIHByb3BlcnR5IGlzIHJlYWRcbi8vIGZyb20gdGhlIG9wdGlvbnMgb2JqZWN0LCB0aGVuIG9wdGlvbnMgYXJlIHN1cHBvcnRlZC4gSWYgbm90LCB0aGVuIHRoZSB0aHJpZFxuLy8gYXJndW1lbnQgdG8gYWRkL3JlbW92ZUV2ZW50TGlzdGVuZXIgaXMgaW50ZXJwcmV0ZWQgYXMgdGhlIGJvb2xlYW4gY2FwdHVyZVxuLy8gdmFsdWUgc28gd2Ugc2hvdWxkIG9ubHkgcGFzcyB0aGUgYGNhcHR1cmVgIHByb3BlcnR5LlxubGV0IGV2ZW50T3B0aW9uc1N1cHBvcnRlZCA9IGZhbHNlO1xudHJ5IHtcbiAgICBjb25zdCBvcHRpb25zID0ge1xuICAgICAgICBnZXQgY2FwdHVyZSgpIHtcbiAgICAgICAgICAgIGV2ZW50T3B0aW9uc1N1cHBvcnRlZCA9IHRydWU7XG4gICAgICAgICAgICByZXR1cm4gZmFsc2U7XG4gICAgICAgIH1cbiAgICB9O1xuICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTpuby1hbnlcbiAgICB3aW5kb3cuYWRkRXZlbnRMaXN0ZW5lcigndGVzdCcsIG9wdGlvbnMsIG9wdGlvbnMpO1xuICAgIC8vIHRzbGludDpkaXNhYmxlLW5leHQtbGluZTpuby1hbnlcbiAgICB3aW5kb3cucmVtb3ZlRXZlbnRMaXN0ZW5lcigndGVzdCcsIG9wdGlvbnMsIG9wdGlvbnMpO1xufVxuY2F0Y2ggKF9lKSB7XG59XG5leHBvcnQgY2xhc3MgRXZlbnRQYXJ0IHtcbiAgICBjb25zdHJ1Y3RvcihlbGVtZW50LCBldmVudE5hbWUsIGV2ZW50Q29udGV4dCkge1xuICAgICAgICB0aGlzLnZhbHVlID0gdW5kZWZpbmVkO1xuICAgICAgICB0aGlzLl9wZW5kaW5nVmFsdWUgPSB1bmRlZmluZWQ7XG4gICAgICAgIHRoaXMuZWxlbWVudCA9IGVsZW1lbnQ7XG4gICAgICAgIHRoaXMuZXZlbnROYW1lID0gZXZlbnROYW1lO1xuICAgICAgICB0aGlzLmV2ZW50Q29udGV4dCA9IGV2ZW50Q29udGV4dDtcbiAgICAgICAgdGhpcy5fYm91bmRIYW5kbGVFdmVudCA9IChlKSA9PiB0aGlzLmhhbmRsZUV2ZW50KGUpO1xuICAgIH1cbiAgICBzZXRWYWx1ZSh2YWx1ZSkge1xuICAgICAgICB0aGlzLl9wZW5kaW5nVmFsdWUgPSB2YWx1ZTtcbiAgICB9XG4gICAgY29tbWl0KCkge1xuICAgICAgICB3aGlsZSAoaXNEaXJlY3RpdmUodGhpcy5fcGVuZGluZ1ZhbHVlKSkge1xuICAgICAgICAgICAgY29uc3QgZGlyZWN0aXZlID0gdGhpcy5fcGVuZGluZ1ZhbHVlO1xuICAgICAgICAgICAgdGhpcy5fcGVuZGluZ1ZhbHVlID0gbm9DaGFuZ2U7XG4gICAgICAgICAgICBkaXJlY3RpdmUodGhpcyk7XG4gICAgICAgIH1cbiAgICAgICAgaWYgKHRoaXMuX3BlbmRpbmdWYWx1ZSA9PT0gbm9DaGFuZ2UpIHtcbiAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgfVxuICAgICAgICBjb25zdCBuZXdMaXN0ZW5lciA9IHRoaXMuX3BlbmRpbmdWYWx1ZTtcbiAgICAgICAgY29uc3Qgb2xkTGlzdGVuZXIgPSB0aGlzLnZhbHVlO1xuICAgICAgICBjb25zdCBzaG91bGRSZW1vdmVMaXN0ZW5lciA9IG5ld0xpc3RlbmVyID09IG51bGwgfHxcbiAgICAgICAgICAgIG9sZExpc3RlbmVyICE9IG51bGwgJiZcbiAgICAgICAgICAgICAgICAobmV3TGlzdGVuZXIuY2FwdHVyZSAhPT0gb2xkTGlzdGVuZXIuY2FwdHVyZSB8fFxuICAgICAgICAgICAgICAgICAgICBuZXdMaXN0ZW5lci5vbmNlICE9PSBvbGRMaXN0ZW5lci5vbmNlIHx8XG4gICAgICAgICAgICAgICAgICAgIG5ld0xpc3RlbmVyLnBhc3NpdmUgIT09IG9sZExpc3RlbmVyLnBhc3NpdmUpO1xuICAgICAgICBjb25zdCBzaG91bGRBZGRMaXN0ZW5lciA9IG5ld0xpc3RlbmVyICE9IG51bGwgJiYgKG9sZExpc3RlbmVyID09IG51bGwgfHwgc2hvdWxkUmVtb3ZlTGlzdGVuZXIpO1xuICAgICAgICBpZiAoc2hvdWxkUmVtb3ZlTGlzdGVuZXIpIHtcbiAgICAgICAgICAgIHRoaXMuZWxlbWVudC5yZW1vdmVFdmVudExpc3RlbmVyKHRoaXMuZXZlbnROYW1lLCB0aGlzLl9ib3VuZEhhbmRsZUV2ZW50LCB0aGlzLl9vcHRpb25zKTtcbiAgICAgICAgfVxuICAgICAgICBpZiAoc2hvdWxkQWRkTGlzdGVuZXIpIHtcbiAgICAgICAgICAgIHRoaXMuX29wdGlvbnMgPSBnZXRPcHRpb25zKG5ld0xpc3RlbmVyKTtcbiAgICAgICAgICAgIHRoaXMuZWxlbWVudC5hZGRFdmVudExpc3RlbmVyKHRoaXMuZXZlbnROYW1lLCB0aGlzLl9ib3VuZEhhbmRsZUV2ZW50LCB0aGlzLl9vcHRpb25zKTtcbiAgICAgICAgfVxuICAgICAgICB0aGlzLnZhbHVlID0gbmV3TGlzdGVuZXI7XG4gICAgICAgIHRoaXMuX3BlbmRpbmdWYWx1ZSA9IG5vQ2hhbmdlO1xuICAgIH1cbiAgICBoYW5kbGVFdmVudChldmVudCkge1xuICAgICAgICBpZiAodHlwZW9mIHRoaXMudmFsdWUgPT09ICdmdW5jdGlvbicpIHtcbiAgICAgICAgICAgIHRoaXMudmFsdWUuY2FsbCh0aGlzLmV2ZW50Q29udGV4dCB8fCB0aGlzLmVsZW1lbnQsIGV2ZW50KTtcbiAgICAgICAgfVxuICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgIHRoaXMudmFsdWUuaGFuZGxlRXZlbnQoZXZlbnQpO1xuICAgICAgICB9XG4gICAgfVxufVxuLy8gV2UgY29weSBvcHRpb25zIGJlY2F1c2Ugb2YgdGhlIGluY29uc2lzdGVudCBiZWhhdmlvciBvZiBicm93c2VycyB3aGVuIHJlYWRpbmdcbi8vIHRoZSB0aGlyZCBhcmd1bWVudCBvZiBhZGQvcmVtb3ZlRXZlbnRMaXN0ZW5lci4gSUUxMSBkb2Vzbid0IHN1cHBvcnQgb3B0aW9uc1xuLy8gYXQgYWxsLiBDaHJvbWUgNDEgb25seSByZWFkcyBgY2FwdHVyZWAgaWYgdGhlIGFyZ3VtZW50IGlzIGFuIG9iamVjdC5cbmNvbnN0IGdldE9wdGlvbnMgPSAobykgPT4gbyAmJlxuICAgIChldmVudE9wdGlvbnNTdXBwb3J0ZWQgP1xuICAgICAgICB7IGNhcHR1cmU6IG8uY2FwdHVyZSwgcGFzc2l2ZTogby5wYXNzaXZlLCBvbmNlOiBvLm9uY2UgfSA6XG4gICAgICAgIG8uY2FwdHVyZSk7XG4vLyMgc291cmNlTWFwcGluZ1VSTD1wYXJ0cy5qcy5tYXAiLCIvKipcbiAqIEBsaWNlbnNlXG4gKiBDb3B5cmlnaHQgKGMpIDIwMTcgVGhlIFBvbHltZXIgUHJvamVjdCBBdXRob3JzLiBBbGwgcmlnaHRzIHJlc2VydmVkLlxuICogVGhpcyBjb2RlIG1heSBvbmx5IGJlIHVzZWQgdW5kZXIgdGhlIEJTRCBzdHlsZSBsaWNlbnNlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vTElDRU5TRS50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgYXV0aG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9BVVRIT1JTLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBjb250cmlidXRvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQ09OVFJJQlVUT1JTLnR4dFxuICogQ29kZSBkaXN0cmlidXRlZCBieSBHb29nbGUgYXMgcGFydCBvZiB0aGUgcG9seW1lciBwcm9qZWN0IGlzIGFsc29cbiAqIHN1YmplY3QgdG8gYW4gYWRkaXRpb25hbCBJUCByaWdodHMgZ3JhbnQgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9QQVRFTlRTLnR4dFxuICovXG4vKipcbiAqIEBtb2R1bGUgbGl0LWh0bWxcbiAqL1xuaW1wb3J0IHsgcmVtb3ZlTm9kZXMgfSBmcm9tICcuL2RvbS5qcyc7XG5pbXBvcnQgeyBOb2RlUGFydCB9IGZyb20gJy4vcGFydHMuanMnO1xuaW1wb3J0IHsgdGVtcGxhdGVGYWN0b3J5IH0gZnJvbSAnLi90ZW1wbGF0ZS1mYWN0b3J5LmpzJztcbmV4cG9ydCBjb25zdCBwYXJ0cyA9IG5ldyBXZWFrTWFwKCk7XG4vKipcbiAqIFJlbmRlcnMgYSB0ZW1wbGF0ZSB0byBhIGNvbnRhaW5lci5cbiAqXG4gKiBUbyB1cGRhdGUgYSBjb250YWluZXIgd2l0aCBuZXcgdmFsdWVzLCByZWV2YWx1YXRlIHRoZSB0ZW1wbGF0ZSBsaXRlcmFsIGFuZFxuICogY2FsbCBgcmVuZGVyYCB3aXRoIHRoZSBuZXcgcmVzdWx0LlxuICpcbiAqIEBwYXJhbSByZXN1bHQgYSBUZW1wbGF0ZVJlc3VsdCBjcmVhdGVkIGJ5IGV2YWx1YXRpbmcgYSB0ZW1wbGF0ZSB0YWcgbGlrZVxuICogICAgIGBodG1sYCBvciBgc3ZnYC5cbiAqIEBwYXJhbSBjb250YWluZXIgQSBET00gcGFyZW50IHRvIHJlbmRlciB0by4gVGhlIGVudGlyZSBjb250ZW50cyBhcmUgZWl0aGVyXG4gKiAgICAgcmVwbGFjZWQsIG9yIGVmZmljaWVudGx5IHVwZGF0ZWQgaWYgdGhlIHNhbWUgcmVzdWx0IHR5cGUgd2FzIHByZXZpb3VzXG4gKiAgICAgcmVuZGVyZWQgdGhlcmUuXG4gKiBAcGFyYW0gb3B0aW9ucyBSZW5kZXJPcHRpb25zIGZvciB0aGUgZW50aXJlIHJlbmRlciB0cmVlIHJlbmRlcmVkIHRvIHRoaXNcbiAqICAgICBjb250YWluZXIuIFJlbmRlciBvcHRpb25zIG11c3QgKm5vdCogY2hhbmdlIGJldHdlZW4gcmVuZGVycyB0byB0aGUgc2FtZVxuICogICAgIGNvbnRhaW5lciwgYXMgdGhvc2UgY2hhbmdlcyB3aWxsIG5vdCBlZmZlY3QgcHJldmlvdXNseSByZW5kZXJlZCBET00uXG4gKi9cbmV4cG9ydCBjb25zdCByZW5kZXIgPSAocmVzdWx0LCBjb250YWluZXIsIG9wdGlvbnMpID0+IHtcbiAgICBsZXQgcGFydCA9IHBhcnRzLmdldChjb250YWluZXIpO1xuICAgIGlmIChwYXJ0ID09PSB1bmRlZmluZWQpIHtcbiAgICAgICAgcmVtb3ZlTm9kZXMoY29udGFpbmVyLCBjb250YWluZXIuZmlyc3RDaGlsZCk7XG4gICAgICAgIHBhcnRzLnNldChjb250YWluZXIsIHBhcnQgPSBuZXcgTm9kZVBhcnQoT2JqZWN0LmFzc2lnbih7IHRlbXBsYXRlRmFjdG9yeSB9LCBvcHRpb25zKSkpO1xuICAgICAgICBwYXJ0LmFwcGVuZEludG8oY29udGFpbmVyKTtcbiAgICB9XG4gICAgcGFydC5zZXRWYWx1ZShyZXN1bHQpO1xuICAgIHBhcnQuY29tbWl0KCk7XG59O1xuLy8jIHNvdXJjZU1hcHBpbmdVUkw9cmVuZGVyLmpzLm1hcCIsIi8qKlxuICogQGxpY2Vuc2VcbiAqIENvcHlyaWdodCAoYykgMjAxNyBUaGUgUG9seW1lciBQcm9qZWN0IEF1dGhvcnMuIEFsbCByaWdodHMgcmVzZXJ2ZWQuXG4gKiBUaGlzIGNvZGUgbWF5IG9ubHkgYmUgdXNlZCB1bmRlciB0aGUgQlNEIHN0eWxlIGxpY2Vuc2UgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9MSUNFTlNFLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBhdXRob3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0FVVEhPUlMudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGNvbnRyaWJ1dG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9DT05UUklCVVRPUlMudHh0XG4gKiBDb2RlIGRpc3RyaWJ1dGVkIGJ5IEdvb2dsZSBhcyBwYXJ0IG9mIHRoZSBwb2x5bWVyIHByb2plY3QgaXMgYWxzb1xuICogc3ViamVjdCB0byBhbiBhZGRpdGlvbmFsIElQIHJpZ2h0cyBncmFudCBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL1BBVEVOVFMudHh0XG4gKi9cbi8qKlxuICogTW9kdWxlIHRvIGFkZCBzaGFkeSBET00vc2hhZHkgQ1NTIHBvbHlmaWxsIHN1cHBvcnQgdG8gbGl0LWh0bWwgdGVtcGxhdGVcbiAqIHJlbmRlcmluZy4gU2VlIHRoZSBbW3JlbmRlcl1dIG1ldGhvZCBmb3IgZGV0YWlscy5cbiAqXG4gKiBAbW9kdWxlIHNoYWR5LXJlbmRlclxuICogQHByZWZlcnJlZFxuICovXG4vKipcbiAqIERvIG5vdCByZW1vdmUgdGhpcyBjb21tZW50OyBpdCBrZWVwcyB0eXBlZG9jIGZyb20gbWlzcGxhY2luZyB0aGUgbW9kdWxlXG4gKiBkb2NzLlxuICovXG5pbXBvcnQgeyByZW1vdmVOb2RlcyB9IGZyb20gJy4vZG9tLmpzJztcbmltcG9ydCB7IGluc2VydE5vZGVJbnRvVGVtcGxhdGUsIHJlbW92ZU5vZGVzRnJvbVRlbXBsYXRlIH0gZnJvbSAnLi9tb2RpZnktdGVtcGxhdGUuanMnO1xuaW1wb3J0IHsgcGFydHMsIHJlbmRlciBhcyBsaXRSZW5kZXIgfSBmcm9tICcuL3JlbmRlci5qcyc7XG5pbXBvcnQgeyB0ZW1wbGF0ZUNhY2hlcyB9IGZyb20gJy4vdGVtcGxhdGUtZmFjdG9yeS5qcyc7XG5pbXBvcnQgeyBUZW1wbGF0ZUluc3RhbmNlIH0gZnJvbSAnLi90ZW1wbGF0ZS1pbnN0YW5jZS5qcyc7XG5pbXBvcnQgeyBUZW1wbGF0ZVJlc3VsdCB9IGZyb20gJy4vdGVtcGxhdGUtcmVzdWx0LmpzJztcbmltcG9ydCB7IG1hcmtlciwgVGVtcGxhdGUgfSBmcm9tICcuL3RlbXBsYXRlLmpzJztcbmV4cG9ydCB7IGh0bWwsIHN2ZywgVGVtcGxhdGVSZXN1bHQgfSBmcm9tICcuLi9saXQtaHRtbC5qcyc7XG4vLyBHZXQgYSBrZXkgdG8gbG9va3VwIGluIGB0ZW1wbGF0ZUNhY2hlc2AuXG5jb25zdCBnZXRUZW1wbGF0ZUNhY2hlS2V5ID0gKHR5cGUsIHNjb3BlTmFtZSkgPT4gYCR7dHlwZX0tLSR7c2NvcGVOYW1lfWA7XG5sZXQgY29tcGF0aWJsZVNoYWR5Q1NTVmVyc2lvbiA9IHRydWU7XG5pZiAodHlwZW9mIHdpbmRvdy5TaGFkeUNTUyA9PT0gJ3VuZGVmaW5lZCcpIHtcbiAgICBjb21wYXRpYmxlU2hhZHlDU1NWZXJzaW9uID0gZmFsc2U7XG59XG5lbHNlIGlmICh0eXBlb2Ygd2luZG93LlNoYWR5Q1NTLnByZXBhcmVUZW1wbGF0ZURvbSA9PT0gJ3VuZGVmaW5lZCcpIHtcbiAgICBjb25zb2xlLndhcm4oYEluY29tcGF0aWJsZSBTaGFkeUNTUyB2ZXJzaW9uIGRldGVjdGVkLmAgK1xuICAgICAgICBgUGxlYXNlIHVwZGF0ZSB0byBhdCBsZWFzdCBAd2ViY29tcG9uZW50cy93ZWJjb21wb25lbnRzanNAMi4wLjIgYW5kYCArXG4gICAgICAgIGBAd2ViY29tcG9uZW50cy9zaGFkeWNzc0AxLjMuMS5gKTtcbiAgICBjb21wYXRpYmxlU2hhZHlDU1NWZXJzaW9uID0gZmFsc2U7XG59XG4vKipcbiAqIFRlbXBsYXRlIGZhY3Rvcnkgd2hpY2ggc2NvcGVzIHRlbXBsYXRlIERPTSB1c2luZyBTaGFkeUNTUy5cbiAqIEBwYXJhbSBzY29wZU5hbWUge3N0cmluZ31cbiAqL1xuY29uc3Qgc2hhZHlUZW1wbGF0ZUZhY3RvcnkgPSAoc2NvcGVOYW1lKSA9PiAocmVzdWx0KSA9PiB7XG4gICAgY29uc3QgY2FjaGVLZXkgPSBnZXRUZW1wbGF0ZUNhY2hlS2V5KHJlc3VsdC50eXBlLCBzY29wZU5hbWUpO1xuICAgIGxldCB0ZW1wbGF0ZUNhY2hlID0gdGVtcGxhdGVDYWNoZXMuZ2V0KGNhY2hlS2V5KTtcbiAgICBpZiAodGVtcGxhdGVDYWNoZSA9PT0gdW5kZWZpbmVkKSB7XG4gICAgICAgIHRlbXBsYXRlQ2FjaGUgPSB7XG4gICAgICAgICAgICBzdHJpbmdzQXJyYXk6IG5ldyBXZWFrTWFwKCksXG4gICAgICAgICAgICBrZXlTdHJpbmc6IG5ldyBNYXAoKVxuICAgICAgICB9O1xuICAgICAgICB0ZW1wbGF0ZUNhY2hlcy5zZXQoY2FjaGVLZXksIHRlbXBsYXRlQ2FjaGUpO1xuICAgIH1cbiAgICBsZXQgdGVtcGxhdGUgPSB0ZW1wbGF0ZUNhY2hlLnN0cmluZ3NBcnJheS5nZXQocmVzdWx0LnN0cmluZ3MpO1xuICAgIGlmICh0ZW1wbGF0ZSAhPT0gdW5kZWZpbmVkKSB7XG4gICAgICAgIHJldHVybiB0ZW1wbGF0ZTtcbiAgICB9XG4gICAgY29uc3Qga2V5ID0gcmVzdWx0LnN0cmluZ3Muam9pbihtYXJrZXIpO1xuICAgIHRlbXBsYXRlID0gdGVtcGxhdGVDYWNoZS5rZXlTdHJpbmcuZ2V0KGtleSk7XG4gICAgaWYgKHRlbXBsYXRlID09PSB1bmRlZmluZWQpIHtcbiAgICAgICAgY29uc3QgZWxlbWVudCA9IHJlc3VsdC5nZXRUZW1wbGF0ZUVsZW1lbnQoKTtcbiAgICAgICAgaWYgKGNvbXBhdGlibGVTaGFkeUNTU1ZlcnNpb24pIHtcbiAgICAgICAgICAgIHdpbmRvdy5TaGFkeUNTUy5wcmVwYXJlVGVtcGxhdGVEb20oZWxlbWVudCwgc2NvcGVOYW1lKTtcbiAgICAgICAgfVxuICAgICAgICB0ZW1wbGF0ZSA9IG5ldyBUZW1wbGF0ZShyZXN1bHQsIGVsZW1lbnQpO1xuICAgICAgICB0ZW1wbGF0ZUNhY2hlLmtleVN0cmluZy5zZXQoa2V5LCB0ZW1wbGF0ZSk7XG4gICAgfVxuICAgIHRlbXBsYXRlQ2FjaGUuc3RyaW5nc0FycmF5LnNldChyZXN1bHQuc3RyaW5ncywgdGVtcGxhdGUpO1xuICAgIHJldHVybiB0ZW1wbGF0ZTtcbn07XG5jb25zdCBURU1QTEFURV9UWVBFUyA9IFsnaHRtbCcsICdzdmcnXTtcbi8qKlxuICogUmVtb3ZlcyBhbGwgc3R5bGUgZWxlbWVudHMgZnJvbSBUZW1wbGF0ZXMgZm9yIHRoZSBnaXZlbiBzY29wZU5hbWUuXG4gKi9cbmNvbnN0IHJlbW92ZVN0eWxlc0Zyb21MaXRUZW1wbGF0ZXMgPSAoc2NvcGVOYW1lKSA9PiB7XG4gICAgVEVNUExBVEVfVFlQRVMuZm9yRWFjaCgodHlwZSkgPT4ge1xuICAgICAgICBjb25zdCB0ZW1wbGF0ZXMgPSB0ZW1wbGF0ZUNhY2hlcy5nZXQoZ2V0VGVtcGxhdGVDYWNoZUtleSh0eXBlLCBzY29wZU5hbWUpKTtcbiAgICAgICAgaWYgKHRlbXBsYXRlcyAhPT0gdW5kZWZpbmVkKSB7XG4gICAgICAgICAgICB0ZW1wbGF0ZXMua2V5U3RyaW5nLmZvckVhY2goKHRlbXBsYXRlKSA9PiB7XG4gICAgICAgICAgICAgICAgY29uc3QgeyBlbGVtZW50OiB7IGNvbnRlbnQgfSB9ID0gdGVtcGxhdGU7XG4gICAgICAgICAgICAgICAgLy8gSUUgMTEgZG9lc24ndCBzdXBwb3J0IHRoZSBpdGVyYWJsZSBwYXJhbSBTZXQgY29uc3RydWN0b3JcbiAgICAgICAgICAgICAgICBjb25zdCBzdHlsZXMgPSBuZXcgU2V0KCk7XG4gICAgICAgICAgICAgICAgQXJyYXkuZnJvbShjb250ZW50LnF1ZXJ5U2VsZWN0b3JBbGwoJ3N0eWxlJykpLmZvckVhY2goKHMpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgc3R5bGVzLmFkZChzKTtcbiAgICAgICAgICAgICAgICB9KTtcbiAgICAgICAgICAgICAgICByZW1vdmVOb2Rlc0Zyb21UZW1wbGF0ZSh0ZW1wbGF0ZSwgc3R5bGVzKTtcbiAgICAgICAgICAgIH0pO1xuICAgICAgICB9XG4gICAgfSk7XG59O1xuY29uc3Qgc2hhZHlSZW5kZXJTZXQgPSBuZXcgU2V0KCk7XG4vKipcbiAqIEZvciB0aGUgZ2l2ZW4gc2NvcGUgbmFtZSwgZW5zdXJlcyB0aGF0IFNoYWR5Q1NTIHN0eWxlIHNjb3BpbmcgaXMgcGVyZm9ybWVkLlxuICogVGhpcyBpcyBkb25lIGp1c3Qgb25jZSBwZXIgc2NvcGUgbmFtZSBzbyB0aGUgZnJhZ21lbnQgYW5kIHRlbXBsYXRlIGNhbm5vdFxuICogYmUgbW9kaWZpZWQuXG4gKiAoMSkgZXh0cmFjdHMgc3R5bGVzIGZyb20gdGhlIHJlbmRlcmVkIGZyYWdtZW50IGFuZCBoYW5kcyB0aGVtIHRvIFNoYWR5Q1NTXG4gKiB0byBiZSBzY29wZWQgYW5kIGFwcGVuZGVkIHRvIHRoZSBkb2N1bWVudFxuICogKDIpIHJlbW92ZXMgc3R5bGUgZWxlbWVudHMgZnJvbSBhbGwgbGl0LWh0bWwgVGVtcGxhdGVzIGZvciB0aGlzIHNjb3BlIG5hbWUuXG4gKlxuICogTm90ZSwgPHN0eWxlPiBlbGVtZW50cyBjYW4gb25seSBiZSBwbGFjZWQgaW50byB0ZW1wbGF0ZXMgZm9yIHRoZVxuICogaW5pdGlhbCByZW5kZXJpbmcgb2YgdGhlIHNjb3BlLiBJZiA8c3R5bGU+IGVsZW1lbnRzIGFyZSBpbmNsdWRlZCBpbiB0ZW1wbGF0ZXNcbiAqIGR5bmFtaWNhbGx5IHJlbmRlcmVkIHRvIHRoZSBzY29wZSAoYWZ0ZXIgdGhlIGZpcnN0IHNjb3BlIHJlbmRlciksIHRoZXkgd2lsbFxuICogbm90IGJlIHNjb3BlZCBhbmQgdGhlIDxzdHlsZT4gd2lsbCBiZSBsZWZ0IGluIHRoZSB0ZW1wbGF0ZSBhbmQgcmVuZGVyZWRcbiAqIG91dHB1dC5cbiAqL1xuY29uc3QgcHJlcGFyZVRlbXBsYXRlU3R5bGVzID0gKHJlbmRlcmVkRE9NLCB0ZW1wbGF0ZSwgc2NvcGVOYW1lKSA9PiB7XG4gICAgc2hhZHlSZW5kZXJTZXQuYWRkKHNjb3BlTmFtZSk7XG4gICAgLy8gTW92ZSBzdHlsZXMgb3V0IG9mIHJlbmRlcmVkIERPTSBhbmQgc3RvcmUuXG4gICAgY29uc3Qgc3R5bGVzID0gcmVuZGVyZWRET00ucXVlcnlTZWxlY3RvckFsbCgnc3R5bGUnKTtcbiAgICAvLyBJZiB0aGVyZSBhcmUgbm8gc3R5bGVzLCBza2lwIHVubmVjZXNzYXJ5IHdvcmtcbiAgICBpZiAoc3R5bGVzLmxlbmd0aCA9PT0gMCkge1xuICAgICAgICAvLyBFbnN1cmUgcHJlcGFyZVRlbXBsYXRlU3R5bGVzIGlzIGNhbGxlZCB0byBzdXBwb3J0IGFkZGluZ1xuICAgICAgICAvLyBzdHlsZXMgdmlhIGBwcmVwYXJlQWRvcHRlZENzc1RleHRgIHNpbmNlIHRoYXQgcmVxdWlyZXMgdGhhdFxuICAgICAgICAvLyBgcHJlcGFyZVRlbXBsYXRlU3R5bGVzYCBpcyBjYWxsZWQuXG4gICAgICAgIHdpbmRvdy5TaGFkeUNTUy5wcmVwYXJlVGVtcGxhdGVTdHlsZXModGVtcGxhdGUuZWxlbWVudCwgc2NvcGVOYW1lKTtcbiAgICAgICAgcmV0dXJuO1xuICAgIH1cbiAgICBjb25zdCBjb25kZW5zZWRTdHlsZSA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoJ3N0eWxlJyk7XG4gICAgLy8gQ29sbGVjdCBzdHlsZXMgaW50byBhIHNpbmdsZSBzdHlsZS4gVGhpcyBoZWxwcyB1cyBtYWtlIHN1cmUgU2hhZHlDU1NcbiAgICAvLyBtYW5pcHVsYXRpb25zIHdpbGwgbm90IHByZXZlbnQgdXMgZnJvbSBiZWluZyBhYmxlIHRvIGZpeCB1cCB0ZW1wbGF0ZVxuICAgIC8vIHBhcnQgaW5kaWNlcy5cbiAgICAvLyBOT1RFOiBjb2xsZWN0aW5nIHN0eWxlcyBpcyBpbmVmZmljaWVudCBmb3IgYnJvd3NlcnMgYnV0IFNoYWR5Q1NTXG4gICAgLy8gY3VycmVudGx5IGRvZXMgdGhpcyBhbnl3YXkuIFdoZW4gaXQgZG9lcyBub3QsIHRoaXMgc2hvdWxkIGJlIGNoYW5nZWQuXG4gICAgZm9yIChsZXQgaSA9IDA7IGkgPCBzdHlsZXMubGVuZ3RoOyBpKyspIHtcbiAgICAgICAgY29uc3Qgc3R5bGUgPSBzdHlsZXNbaV07XG4gICAgICAgIHN0eWxlLnBhcmVudE5vZGUucmVtb3ZlQ2hpbGQoc3R5bGUpO1xuICAgICAgICBjb25kZW5zZWRTdHlsZS50ZXh0Q29udGVudCArPSBzdHlsZS50ZXh0Q29udGVudDtcbiAgICB9XG4gICAgLy8gUmVtb3ZlIHN0eWxlcyBmcm9tIG5lc3RlZCB0ZW1wbGF0ZXMgaW4gdGhpcyBzY29wZS5cbiAgICByZW1vdmVTdHlsZXNGcm9tTGl0VGVtcGxhdGVzKHNjb3BlTmFtZSk7XG4gICAgLy8gQW5kIHRoZW4gcHV0IHRoZSBjb25kZW5zZWQgc3R5bGUgaW50byB0aGUgXCJyb290XCIgdGVtcGxhdGUgcGFzc2VkIGluIGFzXG4gICAgLy8gYHRlbXBsYXRlYC5cbiAgICBpbnNlcnROb2RlSW50b1RlbXBsYXRlKHRlbXBsYXRlLCBjb25kZW5zZWRTdHlsZSwgdGVtcGxhdGUuZWxlbWVudC5jb250ZW50LmZpcnN0Q2hpbGQpO1xuICAgIC8vIE5vdGUsIGl0J3MgaW1wb3J0YW50IHRoYXQgU2hhZHlDU1MgZ2V0cyB0aGUgdGVtcGxhdGUgdGhhdCBgbGl0LWh0bWxgXG4gICAgLy8gd2lsbCBhY3R1YWxseSByZW5kZXIgc28gdGhhdCBpdCBjYW4gdXBkYXRlIHRoZSBzdHlsZSBpbnNpZGUgd2hlblxuICAgIC8vIG5lZWRlZCAoZS5nLiBAYXBwbHkgbmF0aXZlIFNoYWRvdyBET00gY2FzZSkuXG4gICAgd2luZG93LlNoYWR5Q1NTLnByZXBhcmVUZW1wbGF0ZVN0eWxlcyh0ZW1wbGF0ZS5lbGVtZW50LCBzY29wZU5hbWUpO1xuICAgIGlmICh3aW5kb3cuU2hhZHlDU1MubmF0aXZlU2hhZG93KSB7XG4gICAgICAgIC8vIFdoZW4gaW4gbmF0aXZlIFNoYWRvdyBET00sIHJlLWFkZCBzdHlsaW5nIHRvIHJlbmRlcmVkIGNvbnRlbnQgdXNpbmdcbiAgICAgICAgLy8gdGhlIHN0eWxlIFNoYWR5Q1NTIHByb2R1Y2VkLlxuICAgICAgICBjb25zdCBzdHlsZSA9IHRlbXBsYXRlLmVsZW1lbnQuY29udGVudC5xdWVyeVNlbGVjdG9yKCdzdHlsZScpO1xuICAgICAgICByZW5kZXJlZERPTS5pbnNlcnRCZWZvcmUoc3R5bGUuY2xvbmVOb2RlKHRydWUpLCByZW5kZXJlZERPTS5maXJzdENoaWxkKTtcbiAgICB9XG4gICAgZWxzZSB7XG4gICAgICAgIC8vIFdoZW4gbm90IGluIG5hdGl2ZSBTaGFkb3cgRE9NLCBhdCB0aGlzIHBvaW50IFNoYWR5Q1NTIHdpbGwgaGF2ZVxuICAgICAgICAvLyByZW1vdmVkIHRoZSBzdHlsZSBmcm9tIHRoZSBsaXQgdGVtcGxhdGUgYW5kIHBhcnRzIHdpbGwgYmUgYnJva2VuIGFzIGFcbiAgICAgICAgLy8gcmVzdWx0LiBUbyBmaXggdGhpcywgd2UgcHV0IGJhY2sgdGhlIHN0eWxlIG5vZGUgU2hhZHlDU1MgcmVtb3ZlZFxuICAgICAgICAvLyBhbmQgdGhlbiB0ZWxsIGxpdCB0byByZW1vdmUgdGhhdCBub2RlIGZyb20gdGhlIHRlbXBsYXRlLlxuICAgICAgICAvLyBOT1RFLCBTaGFkeUNTUyBjcmVhdGVzIGl0cyBvd24gc3R5bGUgc28gd2UgY2FuIHNhZmVseSBhZGQvcmVtb3ZlXG4gICAgICAgIC8vIGBjb25kZW5zZWRTdHlsZWAgaGVyZS5cbiAgICAgICAgdGVtcGxhdGUuZWxlbWVudC5jb250ZW50Lmluc2VydEJlZm9yZShjb25kZW5zZWRTdHlsZSwgdGVtcGxhdGUuZWxlbWVudC5jb250ZW50LmZpcnN0Q2hpbGQpO1xuICAgICAgICBjb25zdCByZW1vdmVzID0gbmV3IFNldCgpO1xuICAgICAgICByZW1vdmVzLmFkZChjb25kZW5zZWRTdHlsZSk7XG4gICAgICAgIHJlbW92ZU5vZGVzRnJvbVRlbXBsYXRlKHRlbXBsYXRlLCByZW1vdmVzKTtcbiAgICB9XG59O1xuLyoqXG4gKiBFeHRlbnNpb24gdG8gdGhlIHN0YW5kYXJkIGByZW5kZXJgIG1ldGhvZCB3aGljaCBzdXBwb3J0cyByZW5kZXJpbmdcbiAqIHRvIFNoYWRvd1Jvb3RzIHdoZW4gdGhlIFNoYWR5RE9NIChodHRwczovL2dpdGh1Yi5jb20vd2ViY29tcG9uZW50cy9zaGFkeWRvbSlcbiAqIGFuZCBTaGFkeUNTUyAoaHR0cHM6Ly9naXRodWIuY29tL3dlYmNvbXBvbmVudHMvc2hhZHljc3MpIHBvbHlmaWxscyBhcmUgdXNlZFxuICogb3Igd2hlbiB0aGUgd2ViY29tcG9uZW50c2pzXG4gKiAoaHR0cHM6Ly9naXRodWIuY29tL3dlYmNvbXBvbmVudHMvd2ViY29tcG9uZW50c2pzKSBwb2x5ZmlsbCBpcyB1c2VkLlxuICpcbiAqIEFkZHMgYSBgc2NvcGVOYW1lYCBvcHRpb24gd2hpY2ggaXMgdXNlZCB0byBzY29wZSBlbGVtZW50IERPTSBhbmQgc3R5bGVzaGVldHNcbiAqIHdoZW4gbmF0aXZlIFNoYWRvd0RPTSBpcyB1bmF2YWlsYWJsZS4gVGhlIGBzY29wZU5hbWVgIHdpbGwgYmUgYWRkZWQgdG9cbiAqIHRoZSBjbGFzcyBhdHRyaWJ1dGUgb2YgYWxsIHJlbmRlcmVkIERPTS4gSW4gYWRkaXRpb24sIGFueSBzdHlsZSBlbGVtZW50cyB3aWxsXG4gKiBiZSBhdXRvbWF0aWNhbGx5IHJlLXdyaXR0ZW4gd2l0aCB0aGlzIGBzY29wZU5hbWVgIHNlbGVjdG9yIGFuZCBtb3ZlZCBvdXRcbiAqIG9mIHRoZSByZW5kZXJlZCBET00gYW5kIGludG8gdGhlIGRvY3VtZW50IGA8aGVhZD5gLlxuICpcbiAqIEl0IGlzIGNvbW1vbiB0byB1c2UgdGhpcyByZW5kZXIgbWV0aG9kIGluIGNvbmp1bmN0aW9uIHdpdGggYSBjdXN0b20gZWxlbWVudFxuICogd2hpY2ggcmVuZGVycyBhIHNoYWRvd1Jvb3QuIFdoZW4gdGhpcyBpcyBkb25lLCB0eXBpY2FsbHkgdGhlIGVsZW1lbnQnc1xuICogYGxvY2FsTmFtZWAgc2hvdWxkIGJlIHVzZWQgYXMgdGhlIGBzY29wZU5hbWVgLlxuICpcbiAqIEluIGFkZGl0aW9uIHRvIERPTSBzY29waW5nLCBTaGFkeUNTUyBhbHNvIHN1cHBvcnRzIGEgYmFzaWMgc2hpbSBmb3IgY3NzXG4gKiBjdXN0b20gcHJvcGVydGllcyAobmVlZGVkIG9ubHkgb24gb2xkZXIgYnJvd3NlcnMgbGlrZSBJRTExKSBhbmQgYSBzaGltIGZvclxuICogYSBkZXByZWNhdGVkIGZlYXR1cmUgY2FsbGVkIGBAYXBwbHlgIHRoYXQgc3VwcG9ydHMgYXBwbHlpbmcgYSBzZXQgb2YgY3NzXG4gKiBjdXN0b20gcHJvcGVydGllcyB0byBhIGdpdmVuIGxvY2F0aW9uLlxuICpcbiAqIFVzYWdlIGNvbnNpZGVyYXRpb25zOlxuICpcbiAqICogUGFydCB2YWx1ZXMgaW4gYDxzdHlsZT5gIGVsZW1lbnRzIGFyZSBvbmx5IGFwcGxpZWQgdGhlIGZpcnN0IHRpbWUgYSBnaXZlblxuICogYHNjb3BlTmFtZWAgcmVuZGVycy4gU3Vic2VxdWVudCBjaGFuZ2VzIHRvIHBhcnRzIGluIHN0eWxlIGVsZW1lbnRzIHdpbGwgaGF2ZVxuICogbm8gZWZmZWN0LiBCZWNhdXNlIG9mIHRoaXMsIHBhcnRzIGluIHN0eWxlIGVsZW1lbnRzIHNob3VsZCBvbmx5IGJlIHVzZWQgZm9yXG4gKiB2YWx1ZXMgdGhhdCB3aWxsIG5ldmVyIGNoYW5nZSwgZm9yIGV4YW1wbGUgcGFydHMgdGhhdCBzZXQgc2NvcGUtd2lkZSB0aGVtZVxuICogdmFsdWVzIG9yIHBhcnRzIHdoaWNoIHJlbmRlciBzaGFyZWQgc3R5bGUgZWxlbWVudHMuXG4gKlxuICogKiBOb3RlLCBkdWUgdG8gYSBsaW1pdGF0aW9uIG9mIHRoZSBTaGFkeURPTSBwb2x5ZmlsbCwgcmVuZGVyaW5nIGluIGFcbiAqIGN1c3RvbSBlbGVtZW50J3MgYGNvbnN0cnVjdG9yYCBpcyBub3Qgc3VwcG9ydGVkLiBJbnN0ZWFkIHJlbmRlcmluZyBzaG91bGRcbiAqIGVpdGhlciBkb25lIGFzeW5jaHJvbm91c2x5LCBmb3IgZXhhbXBsZSBhdCBtaWNyb3Rhc2sgdGltaW5nIChmb3IgZXhhbXBsZVxuICogYFByb21pc2UucmVzb2x2ZSgpYCksIG9yIGJlIGRlZmVycmVkIHVudGlsIHRoZSBmaXJzdCB0aW1lIHRoZSBlbGVtZW50J3NcbiAqIGBjb25uZWN0ZWRDYWxsYmFja2AgcnVucy5cbiAqXG4gKiBVc2FnZSBjb25zaWRlcmF0aW9ucyB3aGVuIHVzaW5nIHNoaW1tZWQgY3VzdG9tIHByb3BlcnRpZXMgb3IgYEBhcHBseWA6XG4gKlxuICogKiBXaGVuZXZlciBhbnkgZHluYW1pYyBjaGFuZ2VzIGFyZSBtYWRlIHdoaWNoIGFmZmVjdFxuICogY3NzIGN1c3RvbSBwcm9wZXJ0aWVzLCBgU2hhZHlDU1Muc3R5bGVFbGVtZW50KGVsZW1lbnQpYCBtdXN0IGJlIGNhbGxlZFxuICogdG8gdXBkYXRlIHRoZSBlbGVtZW50LiBUaGVyZSBhcmUgdHdvIGNhc2VzIHdoZW4gdGhpcyBpcyBuZWVkZWQ6XG4gKiAoMSkgdGhlIGVsZW1lbnQgaXMgY29ubmVjdGVkIHRvIGEgbmV3IHBhcmVudCwgKDIpIGEgY2xhc3MgaXMgYWRkZWQgdG8gdGhlXG4gKiBlbGVtZW50IHRoYXQgY2F1c2VzIGl0IHRvIG1hdGNoIGRpZmZlcmVudCBjdXN0b20gcHJvcGVydGllcy5cbiAqIFRvIGFkZHJlc3MgdGhlIGZpcnN0IGNhc2Ugd2hlbiByZW5kZXJpbmcgYSBjdXN0b20gZWxlbWVudCwgYHN0eWxlRWxlbWVudGBcbiAqIHNob3VsZCBiZSBjYWxsZWQgaW4gdGhlIGVsZW1lbnQncyBgY29ubmVjdGVkQ2FsbGJhY2tgLlxuICpcbiAqICogU2hpbW1lZCBjdXN0b20gcHJvcGVydGllcyBtYXkgb25seSBiZSBkZWZpbmVkIGVpdGhlciBmb3IgYW4gZW50aXJlXG4gKiBzaGFkb3dSb290IChmb3IgZXhhbXBsZSwgaW4gYSBgOmhvc3RgIHJ1bGUpIG9yIHZpYSBhIHJ1bGUgdGhhdCBkaXJlY3RseVxuICogbWF0Y2hlcyBhbiBlbGVtZW50IHdpdGggYSBzaGFkb3dSb290LiBJbiBvdGhlciB3b3JkcywgaW5zdGVhZCBvZiBmbG93aW5nIGZyb21cbiAqIHBhcmVudCB0byBjaGlsZCBhcyBkbyBuYXRpdmUgY3NzIGN1c3RvbSBwcm9wZXJ0aWVzLCBzaGltbWVkIGN1c3RvbSBwcm9wZXJ0aWVzXG4gKiBmbG93IG9ubHkgZnJvbSBzaGFkb3dSb290cyB0byBuZXN0ZWQgc2hhZG93Um9vdHMuXG4gKlxuICogKiBXaGVuIHVzaW5nIGBAYXBwbHlgIG1peGluZyBjc3Mgc2hvcnRoYW5kIHByb3BlcnR5IG5hbWVzIHdpdGhcbiAqIG5vbi1zaG9ydGhhbmQgbmFtZXMgKGZvciBleGFtcGxlIGBib3JkZXJgIGFuZCBgYm9yZGVyLXdpZHRoYCkgaXMgbm90XG4gKiBzdXBwb3J0ZWQuXG4gKi9cbmV4cG9ydCBjb25zdCByZW5kZXIgPSAocmVzdWx0LCBjb250YWluZXIsIG9wdGlvbnMpID0+IHtcbiAgICBjb25zdCBzY29wZU5hbWUgPSBvcHRpb25zLnNjb3BlTmFtZTtcbiAgICBjb25zdCBoYXNSZW5kZXJlZCA9IHBhcnRzLmhhcyhjb250YWluZXIpO1xuICAgIGNvbnN0IG5lZWRzU2NvcGluZyA9IGNvbnRhaW5lciBpbnN0YW5jZW9mIFNoYWRvd1Jvb3QgJiZcbiAgICAgICAgY29tcGF0aWJsZVNoYWR5Q1NTVmVyc2lvbiAmJiByZXN1bHQgaW5zdGFuY2VvZiBUZW1wbGF0ZVJlc3VsdDtcbiAgICAvLyBIYW5kbGUgZmlyc3QgcmVuZGVyIHRvIGEgc2NvcGUgc3BlY2lhbGx5Li4uXG4gICAgY29uc3QgZmlyc3RTY29wZVJlbmRlciA9IG5lZWRzU2NvcGluZyAmJiAhc2hhZHlSZW5kZXJTZXQuaGFzKHNjb3BlTmFtZSk7XG4gICAgLy8gT24gZmlyc3Qgc2NvcGUgcmVuZGVyLCByZW5kZXIgaW50byBhIGZyYWdtZW50OyB0aGlzIGNhbm5vdCBiZSBhIHNpbmdsZVxuICAgIC8vIGZyYWdtZW50IHRoYXQgaXMgcmV1c2VkIHNpbmNlIG5lc3RlZCByZW5kZXJzIGNhbiBvY2N1ciBzeW5jaHJvbm91c2x5LlxuICAgIGNvbnN0IHJlbmRlckNvbnRhaW5lciA9IGZpcnN0U2NvcGVSZW5kZXIgPyBkb2N1bWVudC5jcmVhdGVEb2N1bWVudEZyYWdtZW50KCkgOiBjb250YWluZXI7XG4gICAgbGl0UmVuZGVyKHJlc3VsdCwgcmVuZGVyQ29udGFpbmVyLCBPYmplY3QuYXNzaWduKHsgdGVtcGxhdGVGYWN0b3J5OiBzaGFkeVRlbXBsYXRlRmFjdG9yeShzY29wZU5hbWUpIH0sIG9wdGlvbnMpKTtcbiAgICAvLyBXaGVuIHBlcmZvcm1pbmcgZmlyc3Qgc2NvcGUgcmVuZGVyLFxuICAgIC8vICgxKSBXZSd2ZSByZW5kZXJlZCBpbnRvIGEgZnJhZ21lbnQgc28gdGhhdCB0aGVyZSdzIGEgY2hhbmNlIHRvXG4gICAgLy8gYHByZXBhcmVUZW1wbGF0ZVN0eWxlc2AgYmVmb3JlIHN1Yi1lbGVtZW50cyBoaXQgdGhlIERPTVxuICAgIC8vICh3aGljaCBtaWdodCBjYXVzZSB0aGVtIHRvIHJlbmRlciBiYXNlZCBvbiBhIGNvbW1vbiBwYXR0ZXJuIG9mXG4gICAgLy8gcmVuZGVyaW5nIGluIGEgY3VzdG9tIGVsZW1lbnQncyBgY29ubmVjdGVkQ2FsbGJhY2tgKTtcbiAgICAvLyAoMikgU2NvcGUgdGhlIHRlbXBsYXRlIHdpdGggU2hhZHlDU1Mgb25lIHRpbWUgb25seSBmb3IgdGhpcyBzY29wZS5cbiAgICAvLyAoMykgUmVuZGVyIHRoZSBmcmFnbWVudCBpbnRvIHRoZSBjb250YWluZXIgYW5kIG1ha2Ugc3VyZSB0aGVcbiAgICAvLyBjb250YWluZXIga25vd3MgaXRzIGBwYXJ0YCBpcyB0aGUgb25lIHdlIGp1c3QgcmVuZGVyZWQuIFRoaXMgZW5zdXJlc1xuICAgIC8vIERPTSB3aWxsIGJlIHJlLXVzZWQgb24gc3Vic2VxdWVudCByZW5kZXJzLlxuICAgIGlmIChmaXJzdFNjb3BlUmVuZGVyKSB7XG4gICAgICAgIGNvbnN0IHBhcnQgPSBwYXJ0cy5nZXQocmVuZGVyQ29udGFpbmVyKTtcbiAgICAgICAgcGFydHMuZGVsZXRlKHJlbmRlckNvbnRhaW5lcik7XG4gICAgICAgIGlmIChwYXJ0LnZhbHVlIGluc3RhbmNlb2YgVGVtcGxhdGVJbnN0YW5jZSkge1xuICAgICAgICAgICAgcHJlcGFyZVRlbXBsYXRlU3R5bGVzKHJlbmRlckNvbnRhaW5lciwgcGFydC52YWx1ZS50ZW1wbGF0ZSwgc2NvcGVOYW1lKTtcbiAgICAgICAgfVxuICAgICAgICByZW1vdmVOb2Rlcyhjb250YWluZXIsIGNvbnRhaW5lci5maXJzdENoaWxkKTtcbiAgICAgICAgY29udGFpbmVyLmFwcGVuZENoaWxkKHJlbmRlckNvbnRhaW5lcik7XG4gICAgICAgIHBhcnRzLnNldChjb250YWluZXIsIHBhcnQpO1xuICAgIH1cbiAgICAvLyBBZnRlciBlbGVtZW50cyBoYXZlIGhpdCB0aGUgRE9NLCB1cGRhdGUgc3R5bGluZyBpZiB0aGlzIGlzIHRoZVxuICAgIC8vIGluaXRpYWwgcmVuZGVyIHRvIHRoaXMgY29udGFpbmVyLlxuICAgIC8vIFRoaXMgaXMgbmVlZGVkIHdoZW5ldmVyIGR5bmFtaWMgY2hhbmdlcyBhcmUgbWFkZSBzbyBpdCB3b3VsZCBiZVxuICAgIC8vIHNhZmVzdCB0byBkbyBldmVyeSByZW5kZXI7IGhvd2V2ZXIsIHRoaXMgd291bGQgcmVncmVzcyBwZXJmb3JtYW5jZVxuICAgIC8vIHNvIHdlIGxlYXZlIGl0IHVwIHRvIHRoZSB1c2VyIHRvIGNhbGwgYFNoYWR5Q1NTUy5zdHlsZUVsZW1lbnRgXG4gICAgLy8gZm9yIGR5bmFtaWMgY2hhbmdlcy5cbiAgICBpZiAoIWhhc1JlbmRlcmVkICYmIG5lZWRzU2NvcGluZykge1xuICAgICAgICB3aW5kb3cuU2hhZHlDU1Muc3R5bGVFbGVtZW50KGNvbnRhaW5lci5ob3N0KTtcbiAgICB9XG59O1xuLy8jIHNvdXJjZU1hcHBpbmdVUkw9c2hhZHktcmVuZGVyLmpzLm1hcCIsIi8qKlxuICogQGxpY2Vuc2VcbiAqIENvcHlyaWdodCAoYykgMjAxNyBUaGUgUG9seW1lciBQcm9qZWN0IEF1dGhvcnMuIEFsbCByaWdodHMgcmVzZXJ2ZWQuXG4gKiBUaGlzIGNvZGUgbWF5IG9ubHkgYmUgdXNlZCB1bmRlciB0aGUgQlNEIHN0eWxlIGxpY2Vuc2UgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9MSUNFTlNFLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBhdXRob3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0FVVEhPUlMudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGNvbnRyaWJ1dG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9DT05UUklCVVRPUlMudHh0XG4gKiBDb2RlIGRpc3RyaWJ1dGVkIGJ5IEdvb2dsZSBhcyBwYXJ0IG9mIHRoZSBwb2x5bWVyIHByb2plY3QgaXMgYWxzb1xuICogc3ViamVjdCB0byBhbiBhZGRpdGlvbmFsIElQIHJpZ2h0cyBncmFudCBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL1BBVEVOVFMudHh0XG4gKi9cbmltcG9ydCB7IG1hcmtlciwgVGVtcGxhdGUgfSBmcm9tICcuL3RlbXBsYXRlLmpzJztcbi8qKlxuICogVGhlIGRlZmF1bHQgVGVtcGxhdGVGYWN0b3J5IHdoaWNoIGNhY2hlcyBUZW1wbGF0ZXMga2V5ZWQgb25cbiAqIHJlc3VsdC50eXBlIGFuZCByZXN1bHQuc3RyaW5ncy5cbiAqL1xuZXhwb3J0IGZ1bmN0aW9uIHRlbXBsYXRlRmFjdG9yeShyZXN1bHQpIHtcbiAgICBsZXQgdGVtcGxhdGVDYWNoZSA9IHRlbXBsYXRlQ2FjaGVzLmdldChyZXN1bHQudHlwZSk7XG4gICAgaWYgKHRlbXBsYXRlQ2FjaGUgPT09IHVuZGVmaW5lZCkge1xuICAgICAgICB0ZW1wbGF0ZUNhY2hlID0ge1xuICAgICAgICAgICAgc3RyaW5nc0FycmF5OiBuZXcgV2Vha01hcCgpLFxuICAgICAgICAgICAga2V5U3RyaW5nOiBuZXcgTWFwKClcbiAgICAgICAgfTtcbiAgICAgICAgdGVtcGxhdGVDYWNoZXMuc2V0KHJlc3VsdC50eXBlLCB0ZW1wbGF0ZUNhY2hlKTtcbiAgICB9XG4gICAgbGV0IHRlbXBsYXRlID0gdGVtcGxhdGVDYWNoZS5zdHJpbmdzQXJyYXkuZ2V0KHJlc3VsdC5zdHJpbmdzKTtcbiAgICBpZiAodGVtcGxhdGUgIT09IHVuZGVmaW5lZCkge1xuICAgICAgICByZXR1cm4gdGVtcGxhdGU7XG4gICAgfVxuICAgIC8vIElmIHRoZSBUZW1wbGF0ZVN0cmluZ3NBcnJheSBpcyBuZXcsIGdlbmVyYXRlIGEga2V5IGZyb20gdGhlIHN0cmluZ3NcbiAgICAvLyBUaGlzIGtleSBpcyBzaGFyZWQgYmV0d2VlbiBhbGwgdGVtcGxhdGVzIHdpdGggaWRlbnRpY2FsIGNvbnRlbnRcbiAgICBjb25zdCBrZXkgPSByZXN1bHQuc3RyaW5ncy5qb2luKG1hcmtlcik7XG4gICAgLy8gQ2hlY2sgaWYgd2UgYWxyZWFkeSBoYXZlIGEgVGVtcGxhdGUgZm9yIHRoaXMga2V5XG4gICAgdGVtcGxhdGUgPSB0ZW1wbGF0ZUNhY2hlLmtleVN0cmluZy5nZXQoa2V5KTtcbiAgICBpZiAodGVtcGxhdGUgPT09IHVuZGVmaW5lZCkge1xuICAgICAgICAvLyBJZiB3ZSBoYXZlIG5vdCBzZWVuIHRoaXMga2V5IGJlZm9yZSwgY3JlYXRlIGEgbmV3IFRlbXBsYXRlXG4gICAgICAgIHRlbXBsYXRlID0gbmV3IFRlbXBsYXRlKHJlc3VsdCwgcmVzdWx0LmdldFRlbXBsYXRlRWxlbWVudCgpKTtcbiAgICAgICAgLy8gQ2FjaGUgdGhlIFRlbXBsYXRlIGZvciB0aGlzIGtleVxuICAgICAgICB0ZW1wbGF0ZUNhY2hlLmtleVN0cmluZy5zZXQoa2V5LCB0ZW1wbGF0ZSk7XG4gICAgfVxuICAgIC8vIENhY2hlIGFsbCBmdXR1cmUgcXVlcmllcyBmb3IgdGhpcyBUZW1wbGF0ZVN0cmluZ3NBcnJheVxuICAgIHRlbXBsYXRlQ2FjaGUuc3RyaW5nc0FycmF5LnNldChyZXN1bHQuc3RyaW5ncywgdGVtcGxhdGUpO1xuICAgIHJldHVybiB0ZW1wbGF0ZTtcbn1cbmV4cG9ydCBjb25zdCB0ZW1wbGF0ZUNhY2hlcyA9IG5ldyBNYXAoKTtcbi8vIyBzb3VyY2VNYXBwaW5nVVJMPXRlbXBsYXRlLWZhY3RvcnkuanMubWFwIiwiLyoqXG4gKiBAbGljZW5zZVxuICogQ29weXJpZ2h0IChjKSAyMDE3IFRoZSBQb2x5bWVyIFByb2plY3QgQXV0aG9ycy4gQWxsIHJpZ2h0cyByZXNlcnZlZC5cbiAqIFRoaXMgY29kZSBtYXkgb25seSBiZSB1c2VkIHVuZGVyIHRoZSBCU0Qgc3R5bGUgbGljZW5zZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0xJQ0VOU0UudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGF1dGhvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQVVUSE9SUy50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgY29udHJpYnV0b3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0NPTlRSSUJVVE9SUy50eHRcbiAqIENvZGUgZGlzdHJpYnV0ZWQgYnkgR29vZ2xlIGFzIHBhcnQgb2YgdGhlIHBvbHltZXIgcHJvamVjdCBpcyBhbHNvXG4gKiBzdWJqZWN0IHRvIGFuIGFkZGl0aW9uYWwgSVAgcmlnaHRzIGdyYW50IGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vUEFURU5UUy50eHRcbiAqL1xuLyoqXG4gKiBAbW9kdWxlIGxpdC1odG1sXG4gKi9cbmltcG9ydCB7IGlzQ0VQb2x5ZmlsbCB9IGZyb20gJy4vZG9tLmpzJztcbmltcG9ydCB7IGlzVGVtcGxhdGVQYXJ0QWN0aXZlIH0gZnJvbSAnLi90ZW1wbGF0ZS5qcyc7XG4vKipcbiAqIEFuIGluc3RhbmNlIG9mIGEgYFRlbXBsYXRlYCB0aGF0IGNhbiBiZSBhdHRhY2hlZCB0byB0aGUgRE9NIGFuZCB1cGRhdGVkXG4gKiB3aXRoIG5ldyB2YWx1ZXMuXG4gKi9cbmV4cG9ydCBjbGFzcyBUZW1wbGF0ZUluc3RhbmNlIHtcbiAgICBjb25zdHJ1Y3Rvcih0ZW1wbGF0ZSwgcHJvY2Vzc29yLCBvcHRpb25zKSB7XG4gICAgICAgIHRoaXMuX3BhcnRzID0gW107XG4gICAgICAgIHRoaXMudGVtcGxhdGUgPSB0ZW1wbGF0ZTtcbiAgICAgICAgdGhpcy5wcm9jZXNzb3IgPSBwcm9jZXNzb3I7XG4gICAgICAgIHRoaXMub3B0aW9ucyA9IG9wdGlvbnM7XG4gICAgfVxuICAgIHVwZGF0ZSh2YWx1ZXMpIHtcbiAgICAgICAgbGV0IGkgPSAwO1xuICAgICAgICBmb3IgKGNvbnN0IHBhcnQgb2YgdGhpcy5fcGFydHMpIHtcbiAgICAgICAgICAgIGlmIChwYXJ0ICE9PSB1bmRlZmluZWQpIHtcbiAgICAgICAgICAgICAgICBwYXJ0LnNldFZhbHVlKHZhbHVlc1tpXSk7XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICBpKys7XG4gICAgICAgIH1cbiAgICAgICAgZm9yIChjb25zdCBwYXJ0IG9mIHRoaXMuX3BhcnRzKSB7XG4gICAgICAgICAgICBpZiAocGFydCAhPT0gdW5kZWZpbmVkKSB7XG4gICAgICAgICAgICAgICAgcGFydC5jb21taXQoKTtcbiAgICAgICAgICAgIH1cbiAgICAgICAgfVxuICAgIH1cbiAgICBfY2xvbmUoKSB7XG4gICAgICAgIC8vIFdoZW4gdXNpbmcgdGhlIEN1c3RvbSBFbGVtZW50cyBwb2x5ZmlsbCwgY2xvbmUgdGhlIG5vZGUsIHJhdGhlciB0aGFuXG4gICAgICAgIC8vIGltcG9ydGluZyBpdCwgdG8ga2VlcCB0aGUgZnJhZ21lbnQgaW4gdGhlIHRlbXBsYXRlJ3MgZG9jdW1lbnQuIFRoaXNcbiAgICAgICAgLy8gbGVhdmVzIHRoZSBmcmFnbWVudCBpbmVydCBzbyBjdXN0b20gZWxlbWVudHMgd29uJ3QgdXBncmFkZSBhbmRcbiAgICAgICAgLy8gcG90ZW50aWFsbHkgbW9kaWZ5IHRoZWlyIGNvbnRlbnRzIGJ5IGNyZWF0aW5nIGEgcG9seWZpbGxlZCBTaGFkb3dSb290XG4gICAgICAgIC8vIHdoaWxlIHdlIHRyYXZlcnNlIHRoZSB0cmVlLlxuICAgICAgICBjb25zdCBmcmFnbWVudCA9IGlzQ0VQb2x5ZmlsbCA/XG4gICAgICAgICAgICB0aGlzLnRlbXBsYXRlLmVsZW1lbnQuY29udGVudC5jbG9uZU5vZGUodHJ1ZSkgOlxuICAgICAgICAgICAgZG9jdW1lbnQuaW1wb3J0Tm9kZSh0aGlzLnRlbXBsYXRlLmVsZW1lbnQuY29udGVudCwgdHJ1ZSk7XG4gICAgICAgIGNvbnN0IHBhcnRzID0gdGhpcy50ZW1wbGF0ZS5wYXJ0cztcbiAgICAgICAgbGV0IHBhcnRJbmRleCA9IDA7XG4gICAgICAgIGxldCBub2RlSW5kZXggPSAwO1xuICAgICAgICBjb25zdCBfcHJlcGFyZUluc3RhbmNlID0gKGZyYWdtZW50KSA9PiB7XG4gICAgICAgICAgICAvLyBFZGdlIG5lZWRzIGFsbCA0IHBhcmFtZXRlcnMgcHJlc2VudDsgSUUxMSBuZWVkcyAzcmQgcGFyYW1ldGVyIHRvIGJlXG4gICAgICAgICAgICAvLyBudWxsXG4gICAgICAgICAgICBjb25zdCB3YWxrZXIgPSBkb2N1bWVudC5jcmVhdGVUcmVlV2Fsa2VyKGZyYWdtZW50LCAxMzMgLyogTm9kZUZpbHRlci5TSE9XX3tFTEVNRU5UfENPTU1FTlR8VEVYVH0gKi8sIG51bGwsIGZhbHNlKTtcbiAgICAgICAgICAgIGxldCBub2RlID0gd2Fsa2VyLm5leHROb2RlKCk7XG4gICAgICAgICAgICAvLyBMb29wIHRocm91Z2ggYWxsIHRoZSBub2RlcyBhbmQgcGFydHMgb2YgYSB0ZW1wbGF0ZVxuICAgICAgICAgICAgd2hpbGUgKHBhcnRJbmRleCA8IHBhcnRzLmxlbmd0aCAmJiBub2RlICE9PSBudWxsKSB7XG4gICAgICAgICAgICAgICAgY29uc3QgcGFydCA9IHBhcnRzW3BhcnRJbmRleF07XG4gICAgICAgICAgICAgICAgLy8gQ29uc2VjdXRpdmUgUGFydHMgbWF5IGhhdmUgdGhlIHNhbWUgbm9kZSBpbmRleCwgaW4gdGhlIGNhc2Ugb2ZcbiAgICAgICAgICAgICAgICAvLyBtdWx0aXBsZSBib3VuZCBhdHRyaWJ1dGVzIG9uIGFuIGVsZW1lbnQuIFNvIGVhY2ggaXRlcmF0aW9uIHdlIGVpdGhlclxuICAgICAgICAgICAgICAgIC8vIGluY3JlbWVudCB0aGUgbm9kZUluZGV4LCBpZiB3ZSBhcmVuJ3Qgb24gYSBub2RlIHdpdGggYSBwYXJ0LCBvciB0aGVcbiAgICAgICAgICAgICAgICAvLyBwYXJ0SW5kZXggaWYgd2UgYXJlLiBCeSBub3QgaW5jcmVtZW50aW5nIHRoZSBub2RlSW5kZXggd2hlbiB3ZSBmaW5kIGFcbiAgICAgICAgICAgICAgICAvLyBwYXJ0LCB3ZSBhbGxvdyBmb3IgdGhlIG5leHQgcGFydCB0byBiZSBhc3NvY2lhdGVkIHdpdGggdGhlIGN1cnJlbnRcbiAgICAgICAgICAgICAgICAvLyBub2RlIGlmIG5lY2Nlc3Nhc3J5LlxuICAgICAgICAgICAgICAgIGlmICghaXNUZW1wbGF0ZVBhcnRBY3RpdmUocGFydCkpIHtcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5fcGFydHMucHVzaCh1bmRlZmluZWQpO1xuICAgICAgICAgICAgICAgICAgICBwYXJ0SW5kZXgrKztcbiAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgZWxzZSBpZiAobm9kZUluZGV4ID09PSBwYXJ0LmluZGV4KSB7XG4gICAgICAgICAgICAgICAgICAgIGlmIChwYXJ0LnR5cGUgPT09ICdub2RlJykge1xuICAgICAgICAgICAgICAgICAgICAgICAgY29uc3QgcGFydCA9IHRoaXMucHJvY2Vzc29yLmhhbmRsZVRleHRFeHByZXNzaW9uKHRoaXMub3B0aW9ucyk7XG4gICAgICAgICAgICAgICAgICAgICAgICBwYXJ0Lmluc2VydEFmdGVyTm9kZShub2RlLnByZXZpb3VzU2libGluZyk7XG4gICAgICAgICAgICAgICAgICAgICAgICB0aGlzLl9wYXJ0cy5wdXNoKHBhcnQpO1xuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgIGVsc2Uge1xuICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5fcGFydHMucHVzaCguLi50aGlzLnByb2Nlc3Nvci5oYW5kbGVBdHRyaWJ1dGVFeHByZXNzaW9ucyhub2RlLCBwYXJ0Lm5hbWUsIHBhcnQuc3RyaW5ncywgdGhpcy5vcHRpb25zKSk7XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgcGFydEluZGV4Kys7XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIGVsc2Uge1xuICAgICAgICAgICAgICAgICAgICBub2RlSW5kZXgrKztcbiAgICAgICAgICAgICAgICAgICAgaWYgKG5vZGUubm9kZU5hbWUgPT09ICdURU1QTEFURScpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgIF9wcmVwYXJlSW5zdGFuY2Uobm9kZS5jb250ZW50KTtcbiAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICBub2RlID0gd2Fsa2VyLm5leHROb2RlKCk7XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgfVxuICAgICAgICB9O1xuICAgICAgICBfcHJlcGFyZUluc3RhbmNlKGZyYWdtZW50KTtcbiAgICAgICAgaWYgKGlzQ0VQb2x5ZmlsbCkge1xuICAgICAgICAgICAgZG9jdW1lbnQuYWRvcHROb2RlKGZyYWdtZW50KTtcbiAgICAgICAgICAgIGN1c3RvbUVsZW1lbnRzLnVwZ3JhZGUoZnJhZ21lbnQpO1xuICAgICAgICB9XG4gICAgICAgIHJldHVybiBmcmFnbWVudDtcbiAgICB9XG59XG4vLyMgc291cmNlTWFwcGluZ1VSTD10ZW1wbGF0ZS1pbnN0YW5jZS5qcy5tYXAiLCIvKipcbiAqIEBsaWNlbnNlXG4gKiBDb3B5cmlnaHQgKGMpIDIwMTcgVGhlIFBvbHltZXIgUHJvamVjdCBBdXRob3JzLiBBbGwgcmlnaHRzIHJlc2VydmVkLlxuICogVGhpcyBjb2RlIG1heSBvbmx5IGJlIHVzZWQgdW5kZXIgdGhlIEJTRCBzdHlsZSBsaWNlbnNlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vTElDRU5TRS50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgYXV0aG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9BVVRIT1JTLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBjb250cmlidXRvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQ09OVFJJQlVUT1JTLnR4dFxuICogQ29kZSBkaXN0cmlidXRlZCBieSBHb29nbGUgYXMgcGFydCBvZiB0aGUgcG9seW1lciBwcm9qZWN0IGlzIGFsc29cbiAqIHN1YmplY3QgdG8gYW4gYWRkaXRpb25hbCBJUCByaWdodHMgZ3JhbnQgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9QQVRFTlRTLnR4dFxuICovXG4vKipcbiAqIEBtb2R1bGUgbGl0LWh0bWxcbiAqL1xuaW1wb3J0IHsgcmVwYXJlbnROb2RlcyB9IGZyb20gJy4vZG9tLmpzJztcbmltcG9ydCB7IGJvdW5kQXR0cmlidXRlU3VmZml4LCBsYXN0QXR0cmlidXRlTmFtZVJlZ2V4LCBtYXJrZXIsIG5vZGVNYXJrZXIgfSBmcm9tICcuL3RlbXBsYXRlLmpzJztcbi8qKlxuICogVGhlIHJldHVybiB0eXBlIG9mIGBodG1sYCwgd2hpY2ggaG9sZHMgYSBUZW1wbGF0ZSBhbmQgdGhlIHZhbHVlcyBmcm9tXG4gKiBpbnRlcnBvbGF0ZWQgZXhwcmVzc2lvbnMuXG4gKi9cbmV4cG9ydCBjbGFzcyBUZW1wbGF0ZVJlc3VsdCB7XG4gICAgY29uc3RydWN0b3Ioc3RyaW5ncywgdmFsdWVzLCB0eXBlLCBwcm9jZXNzb3IpIHtcbiAgICAgICAgdGhpcy5zdHJpbmdzID0gc3RyaW5ncztcbiAgICAgICAgdGhpcy52YWx1ZXMgPSB2YWx1ZXM7XG4gICAgICAgIHRoaXMudHlwZSA9IHR5cGU7XG4gICAgICAgIHRoaXMucHJvY2Vzc29yID0gcHJvY2Vzc29yO1xuICAgIH1cbiAgICAvKipcbiAgICAgKiBSZXR1cm5zIGEgc3RyaW5nIG9mIEhUTUwgdXNlZCB0byBjcmVhdGUgYSBgPHRlbXBsYXRlPmAgZWxlbWVudC5cbiAgICAgKi9cbiAgICBnZXRIVE1MKCkge1xuICAgICAgICBjb25zdCBlbmRJbmRleCA9IHRoaXMuc3RyaW5ncy5sZW5ndGggLSAxO1xuICAgICAgICBsZXQgaHRtbCA9ICcnO1xuICAgICAgICBmb3IgKGxldCBpID0gMDsgaSA8IGVuZEluZGV4OyBpKyspIHtcbiAgICAgICAgICAgIGNvbnN0IHMgPSB0aGlzLnN0cmluZ3NbaV07XG4gICAgICAgICAgICAvLyBUaGlzIGV4ZWMoKSBjYWxsIGRvZXMgdHdvIHRoaW5nczpcbiAgICAgICAgICAgIC8vIDEpIEFwcGVuZHMgYSBzdWZmaXggdG8gdGhlIGJvdW5kIGF0dHJpYnV0ZSBuYW1lIHRvIG9wdCBvdXQgb2Ygc3BlY2lhbFxuICAgICAgICAgICAgLy8gYXR0cmlidXRlIHZhbHVlIHBhcnNpbmcgdGhhdCBJRTExIGFuZCBFZGdlIGRvLCBsaWtlIGZvciBzdHlsZSBhbmRcbiAgICAgICAgICAgIC8vIG1hbnkgU1ZHIGF0dHJpYnV0ZXMuIFRoZSBUZW1wbGF0ZSBjbGFzcyBhbHNvIGFwcGVuZHMgdGhlIHNhbWUgc3VmZml4XG4gICAgICAgICAgICAvLyB3aGVuIGxvb2tpbmcgdXAgYXR0cmlidXRlcyB0byBjcmVhdGUgUGFydHMuXG4gICAgICAgICAgICAvLyAyKSBBZGRzIGFuIHVucXVvdGVkLWF0dHJpYnV0ZS1zYWZlIG1hcmtlciBmb3IgdGhlIGZpcnN0IGV4cHJlc3Npb24gaW5cbiAgICAgICAgICAgIC8vIGFuIGF0dHJpYnV0ZS4gU3Vic2VxdWVudCBhdHRyaWJ1dGUgZXhwcmVzc2lvbnMgd2lsbCB1c2Ugbm9kZSBtYXJrZXJzLFxuICAgICAgICAgICAgLy8gYW5kIHRoaXMgaXMgc2FmZSBzaW5jZSBhdHRyaWJ1dGVzIHdpdGggbXVsdGlwbGUgZXhwcmVzc2lvbnMgYXJlXG4gICAgICAgICAgICAvLyBndWFyYW50ZWVkIHRvIGJlIHF1b3RlZC5cbiAgICAgICAgICAgIGNvbnN0IG1hdGNoID0gbGFzdEF0dHJpYnV0ZU5hbWVSZWdleC5leGVjKHMpO1xuICAgICAgICAgICAgaWYgKG1hdGNoKSB7XG4gICAgICAgICAgICAgICAgLy8gV2UncmUgc3RhcnRpbmcgYSBuZXcgYm91bmQgYXR0cmlidXRlLlxuICAgICAgICAgICAgICAgIC8vIEFkZCB0aGUgc2FmZSBhdHRyaWJ1dGUgc3VmZml4LCBhbmQgdXNlIHVucXVvdGVkLWF0dHJpYnV0ZS1zYWZlXG4gICAgICAgICAgICAgICAgLy8gbWFya2VyLlxuICAgICAgICAgICAgICAgIGh0bWwgKz0gcy5zdWJzdHIoMCwgbWF0Y2guaW5kZXgpICsgbWF0Y2hbMV0gKyBtYXRjaFsyXSArXG4gICAgICAgICAgICAgICAgICAgIGJvdW5kQXR0cmlidXRlU3VmZml4ICsgbWF0Y2hbM10gKyBtYXJrZXI7XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgICAgICAvLyBXZSdyZSBlaXRoZXIgaW4gYSBib3VuZCBub2RlLCBvciB0cmFpbGluZyBib3VuZCBhdHRyaWJ1dGUuXG4gICAgICAgICAgICAgICAgLy8gRWl0aGVyIHdheSwgbm9kZU1hcmtlciBpcyBzYWZlIHRvIHVzZS5cbiAgICAgICAgICAgICAgICBodG1sICs9IHMgKyBub2RlTWFya2VyO1xuICAgICAgICAgICAgfVxuICAgICAgICB9XG4gICAgICAgIHJldHVybiBodG1sICsgdGhpcy5zdHJpbmdzW2VuZEluZGV4XTtcbiAgICB9XG4gICAgZ2V0VGVtcGxhdGVFbGVtZW50KCkge1xuICAgICAgICBjb25zdCB0ZW1wbGF0ZSA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoJ3RlbXBsYXRlJyk7XG4gICAgICAgIHRlbXBsYXRlLmlubmVySFRNTCA9IHRoaXMuZ2V0SFRNTCgpO1xuICAgICAgICByZXR1cm4gdGVtcGxhdGU7XG4gICAgfVxufVxuLyoqXG4gKiBBIFRlbXBsYXRlUmVzdWx0IGZvciBTVkcgZnJhZ21lbnRzLlxuICpcbiAqIFRoaXMgY2xhc3Mgd3JhcHMgSFRNbCBpbiBhbiBgPHN2Zz5gIHRhZyBpbiBvcmRlciB0byBwYXJzZSBpdHMgY29udGVudHMgaW4gdGhlXG4gKiBTVkcgbmFtZXNwYWNlLCB0aGVuIG1vZGlmaWVzIHRoZSB0ZW1wbGF0ZSB0byByZW1vdmUgdGhlIGA8c3ZnPmAgdGFnIHNvIHRoYXRcbiAqIGNsb25lcyBvbmx5IGNvbnRhaW5lciB0aGUgb3JpZ2luYWwgZnJhZ21lbnQuXG4gKi9cbmV4cG9ydCBjbGFzcyBTVkdUZW1wbGF0ZVJlc3VsdCBleHRlbmRzIFRlbXBsYXRlUmVzdWx0IHtcbiAgICBnZXRIVE1MKCkge1xuICAgICAgICByZXR1cm4gYDxzdmc+JHtzdXBlci5nZXRIVE1MKCl9PC9zdmc+YDtcbiAgICB9XG4gICAgZ2V0VGVtcGxhdGVFbGVtZW50KCkge1xuICAgICAgICBjb25zdCB0ZW1wbGF0ZSA9IHN1cGVyLmdldFRlbXBsYXRlRWxlbWVudCgpO1xuICAgICAgICBjb25zdCBjb250ZW50ID0gdGVtcGxhdGUuY29udGVudDtcbiAgICAgICAgY29uc3Qgc3ZnRWxlbWVudCA9IGNvbnRlbnQuZmlyc3RDaGlsZDtcbiAgICAgICAgY29udGVudC5yZW1vdmVDaGlsZChzdmdFbGVtZW50KTtcbiAgICAgICAgcmVwYXJlbnROb2Rlcyhjb250ZW50LCBzdmdFbGVtZW50LmZpcnN0Q2hpbGQpO1xuICAgICAgICByZXR1cm4gdGVtcGxhdGU7XG4gICAgfVxufVxuLy8jIHNvdXJjZU1hcHBpbmdVUkw9dGVtcGxhdGUtcmVzdWx0LmpzLm1hcCIsIi8qKlxuICogQGxpY2Vuc2VcbiAqIENvcHlyaWdodCAoYykgMjAxNyBUaGUgUG9seW1lciBQcm9qZWN0IEF1dGhvcnMuIEFsbCByaWdodHMgcmVzZXJ2ZWQuXG4gKiBUaGlzIGNvZGUgbWF5IG9ubHkgYmUgdXNlZCB1bmRlciB0aGUgQlNEIHN0eWxlIGxpY2Vuc2UgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9MSUNFTlNFLnR4dFxuICogVGhlIGNvbXBsZXRlIHNldCBvZiBhdXRob3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0FVVEhPUlMudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGNvbnRyaWJ1dG9ycyBtYXkgYmUgZm91bmQgYXRcbiAqIGh0dHA6Ly9wb2x5bWVyLmdpdGh1Yi5pby9DT05UUklCVVRPUlMudHh0XG4gKiBDb2RlIGRpc3RyaWJ1dGVkIGJ5IEdvb2dsZSBhcyBwYXJ0IG9mIHRoZSBwb2x5bWVyIHByb2plY3QgaXMgYWxzb1xuICogc3ViamVjdCB0byBhbiBhZGRpdGlvbmFsIElQIHJpZ2h0cyBncmFudCBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL1BBVEVOVFMudHh0XG4gKi9cbi8qKlxuICogQW4gZXhwcmVzc2lvbiBtYXJrZXIgd2l0aCBlbWJlZGRlZCB1bmlxdWUga2V5IHRvIGF2b2lkIGNvbGxpc2lvbiB3aXRoXG4gKiBwb3NzaWJsZSB0ZXh0IGluIHRlbXBsYXRlcy5cbiAqL1xuZXhwb3J0IGNvbnN0IG1hcmtlciA9IGB7e2xpdC0ke1N0cmluZyhNYXRoLnJhbmRvbSgpKS5zbGljZSgyKX19fWA7XG4vKipcbiAqIEFuIGV4cHJlc3Npb24gbWFya2VyIHVzZWQgdGV4dC1wb3NpdGlvbnMsIG11bHRpLWJpbmRpbmcgYXR0cmlidXRlcywgYW5kXG4gKiBhdHRyaWJ1dGVzIHdpdGggbWFya3VwLWxpa2UgdGV4dCB2YWx1ZXMuXG4gKi9cbmV4cG9ydCBjb25zdCBub2RlTWFya2VyID0gYDwhLS0ke21hcmtlcn0tLT5gO1xuZXhwb3J0IGNvbnN0IG1hcmtlclJlZ2V4ID0gbmV3IFJlZ0V4cChgJHttYXJrZXJ9fCR7bm9kZU1hcmtlcn1gKTtcbi8qKlxuICogU3VmZml4IGFwcGVuZGVkIHRvIGFsbCBib3VuZCBhdHRyaWJ1dGUgbmFtZXMuXG4gKi9cbmV4cG9ydCBjb25zdCBib3VuZEF0dHJpYnV0ZVN1ZmZpeCA9ICckbGl0JCc7XG4vKipcbiAqIEFuIHVwZGF0ZWFibGUgVGVtcGxhdGUgdGhhdCB0cmFja3MgdGhlIGxvY2F0aW9uIG9mIGR5bmFtaWMgcGFydHMuXG4gKi9cbmV4cG9ydCBjbGFzcyBUZW1wbGF0ZSB7XG4gICAgY29uc3RydWN0b3IocmVzdWx0LCBlbGVtZW50KSB7XG4gICAgICAgIHRoaXMucGFydHMgPSBbXTtcbiAgICAgICAgdGhpcy5lbGVtZW50ID0gZWxlbWVudDtcbiAgICAgICAgbGV0IGluZGV4ID0gLTE7XG4gICAgICAgIGxldCBwYXJ0SW5kZXggPSAwO1xuICAgICAgICBjb25zdCBub2Rlc1RvUmVtb3ZlID0gW107XG4gICAgICAgIGNvbnN0IF9wcmVwYXJlVGVtcGxhdGUgPSAodGVtcGxhdGUpID0+IHtcbiAgICAgICAgICAgIGNvbnN0IGNvbnRlbnQgPSB0ZW1wbGF0ZS5jb250ZW50O1xuICAgICAgICAgICAgLy8gRWRnZSBuZWVkcyBhbGwgNCBwYXJhbWV0ZXJzIHByZXNlbnQ7IElFMTEgbmVlZHMgM3JkIHBhcmFtZXRlciB0byBiZVxuICAgICAgICAgICAgLy8gbnVsbFxuICAgICAgICAgICAgY29uc3Qgd2Fsa2VyID0gZG9jdW1lbnQuY3JlYXRlVHJlZVdhbGtlcihjb250ZW50LCAxMzMgLyogTm9kZUZpbHRlci5TSE9XX3tFTEVNRU5UfENPTU1FTlR8VEVYVH0gKi8sIG51bGwsIGZhbHNlKTtcbiAgICAgICAgICAgIC8vIEtlZXBzIHRyYWNrIG9mIHRoZSBsYXN0IGluZGV4IGFzc29jaWF0ZWQgd2l0aCBhIHBhcnQuIFdlIHRyeSB0byBkZWxldGVcbiAgICAgICAgICAgIC8vIHVubmVjZXNzYXJ5IG5vZGVzLCBidXQgd2UgbmV2ZXIgd2FudCB0byBhc3NvY2lhdGUgdHdvIGRpZmZlcmVudCBwYXJ0c1xuICAgICAgICAgICAgLy8gdG8gdGhlIHNhbWUgaW5kZXguIFRoZXkgbXVzdCBoYXZlIGEgY29uc3RhbnQgbm9kZSBiZXR3ZWVuLlxuICAgICAgICAgICAgbGV0IGxhc3RQYXJ0SW5kZXggPSAwO1xuICAgICAgICAgICAgd2hpbGUgKHdhbGtlci5uZXh0Tm9kZSgpKSB7XG4gICAgICAgICAgICAgICAgaW5kZXgrKztcbiAgICAgICAgICAgICAgICBjb25zdCBub2RlID0gd2Fsa2VyLmN1cnJlbnROb2RlO1xuICAgICAgICAgICAgICAgIGlmIChub2RlLm5vZGVUeXBlID09PSAxIC8qIE5vZGUuRUxFTUVOVF9OT0RFICovKSB7XG4gICAgICAgICAgICAgICAgICAgIGlmIChub2RlLmhhc0F0dHJpYnV0ZXMoKSkge1xuICAgICAgICAgICAgICAgICAgICAgICAgY29uc3QgYXR0cmlidXRlcyA9IG5vZGUuYXR0cmlidXRlcztcbiAgICAgICAgICAgICAgICAgICAgICAgIC8vIFBlclxuICAgICAgICAgICAgICAgICAgICAgICAgLy8gaHR0cHM6Ly9kZXZlbG9wZXIubW96aWxsYS5vcmcvZW4tVVMvZG9jcy9XZWIvQVBJL05hbWVkTm9kZU1hcCxcbiAgICAgICAgICAgICAgICAgICAgICAgIC8vIGF0dHJpYnV0ZXMgYXJlIG5vdCBndWFyYW50ZWVkIHRvIGJlIHJldHVybmVkIGluIGRvY3VtZW50IG9yZGVyLlxuICAgICAgICAgICAgICAgICAgICAgICAgLy8gSW4gcGFydGljdWxhciwgRWRnZS9JRSBjYW4gcmV0dXJuIHRoZW0gb3V0IG9mIG9yZGVyLCBzbyB3ZSBjYW5ub3RcbiAgICAgICAgICAgICAgICAgICAgICAgIC8vIGFzc3VtZSBhIGNvcnJlc3BvbmRhbmNlIGJldHdlZW4gcGFydCBpbmRleCBhbmQgYXR0cmlidXRlIGluZGV4LlxuICAgICAgICAgICAgICAgICAgICAgICAgbGV0IGNvdW50ID0gMDtcbiAgICAgICAgICAgICAgICAgICAgICAgIGZvciAobGV0IGkgPSAwOyBpIDwgYXR0cmlidXRlcy5sZW5ndGg7IGkrKykge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmIChhdHRyaWJ1dGVzW2ldLnZhbHVlLmluZGV4T2YobWFya2VyKSA+PSAwKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNvdW50Kys7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgICAgICAgd2hpbGUgKGNvdW50LS0gPiAwKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgLy8gR2V0IHRoZSB0ZW1wbGF0ZSBsaXRlcmFsIHNlY3Rpb24gbGVhZGluZyB1cCB0byB0aGUgZmlyc3RcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAvLyBleHByZXNzaW9uIGluIHRoaXMgYXR0cmlidXRlXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY29uc3Qgc3RyaW5nRm9yUGFydCA9IHJlc3VsdC5zdHJpbmdzW3BhcnRJbmRleF07XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgLy8gRmluZCB0aGUgYXR0cmlidXRlIG5hbWVcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjb25zdCBuYW1lID0gbGFzdEF0dHJpYnV0ZU5hbWVSZWdleC5leGVjKHN0cmluZ0ZvclBhcnQpWzJdO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIC8vIEZpbmQgdGhlIGNvcnJlc3BvbmRpbmcgYXR0cmlidXRlXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgLy8gQWxsIGJvdW5kIGF0dHJpYnV0ZXMgaGF2ZSBoYWQgYSBzdWZmaXggYWRkZWQgaW5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAvLyBUZW1wbGF0ZVJlc3VsdCNnZXRIVE1MIHRvIG9wdCBvdXQgb2Ygc3BlY2lhbCBhdHRyaWJ1dGVcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAvLyBoYW5kbGluZy4gVG8gbG9vayB1cCB0aGUgYXR0cmlidXRlIHZhbHVlIHdlIGFsc28gbmVlZCB0byBhZGRcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAvLyB0aGUgc3VmZml4LlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNvbnN0IGF0dHJpYnV0ZUxvb2t1cE5hbWUgPSBuYW1lLnRvTG93ZXJDYXNlKCkgKyBib3VuZEF0dHJpYnV0ZVN1ZmZpeDtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjb25zdCBhdHRyaWJ1dGVWYWx1ZSA9IG5vZGUuZ2V0QXR0cmlidXRlKGF0dHJpYnV0ZUxvb2t1cE5hbWUpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNvbnN0IHN0cmluZ3MgPSBhdHRyaWJ1dGVWYWx1ZS5zcGxpdChtYXJrZXJSZWdleCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5wYXJ0cy5wdXNoKHsgdHlwZTogJ2F0dHJpYnV0ZScsIGluZGV4LCBuYW1lLCBzdHJpbmdzIH0pO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG5vZGUucmVtb3ZlQXR0cmlidXRlKGF0dHJpYnV0ZUxvb2t1cE5hbWUpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHBhcnRJbmRleCArPSBzdHJpbmdzLmxlbmd0aCAtIDE7XG4gICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgaWYgKG5vZGUudGFnTmFtZSA9PT0gJ1RFTVBMQVRFJykge1xuICAgICAgICAgICAgICAgICAgICAgICAgX3ByZXBhcmVUZW1wbGF0ZShub2RlKTtcbiAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICBlbHNlIGlmIChub2RlLm5vZGVUeXBlID09PSAzIC8qIE5vZGUuVEVYVF9OT0RFICovKSB7XG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IGRhdGEgPSBub2RlLmRhdGE7XG4gICAgICAgICAgICAgICAgICAgIGlmIChkYXRhLmluZGV4T2YobWFya2VyKSA+PSAwKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICBjb25zdCBwYXJlbnQgPSBub2RlLnBhcmVudE5vZGU7XG4gICAgICAgICAgICAgICAgICAgICAgICBjb25zdCBzdHJpbmdzID0gZGF0YS5zcGxpdChtYXJrZXJSZWdleCk7XG4gICAgICAgICAgICAgICAgICAgICAgICBjb25zdCBsYXN0SW5kZXggPSBzdHJpbmdzLmxlbmd0aCAtIDE7XG4gICAgICAgICAgICAgICAgICAgICAgICAvLyBHZW5lcmF0ZSBhIG5ldyB0ZXh0IG5vZGUgZm9yIGVhY2ggbGl0ZXJhbCBzZWN0aW9uXG4gICAgICAgICAgICAgICAgICAgICAgICAvLyBUaGVzZSBub2RlcyBhcmUgYWxzbyB1c2VkIGFzIHRoZSBtYXJrZXJzIGZvciBub2RlIHBhcnRzXG4gICAgICAgICAgICAgICAgICAgICAgICBmb3IgKGxldCBpID0gMDsgaSA8IGxhc3RJbmRleDsgaSsrKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcGFyZW50Lmluc2VydEJlZm9yZSgoc3RyaW5nc1tpXSA9PT0gJycpID8gY3JlYXRlTWFya2VyKCkgOlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBkb2N1bWVudC5jcmVhdGVUZXh0Tm9kZShzdHJpbmdzW2ldKSwgbm9kZSk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5wYXJ0cy5wdXNoKHsgdHlwZTogJ25vZGUnLCBpbmRleDogKytpbmRleCB9KTtcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgICAgIC8vIElmIHRoZXJlJ3Mgbm8gdGV4dCwgd2UgbXVzdCBpbnNlcnQgYSBjb21tZW50IHRvIG1hcmsgb3VyIHBsYWNlLlxuICAgICAgICAgICAgICAgICAgICAgICAgLy8gRWxzZSwgd2UgY2FuIHRydXN0IGl0IHdpbGwgc3RpY2sgYXJvdW5kIGFmdGVyIGNsb25pbmcuXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAoc3RyaW5nc1tsYXN0SW5kZXhdID09PSAnJykge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHBhcmVudC5pbnNlcnRCZWZvcmUoY3JlYXRlTWFya2VyKCksIG5vZGUpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG5vZGVzVG9SZW1vdmUucHVzaChub2RlKTtcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgICAgIGVsc2Uge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG5vZGUuZGF0YSA9IHN0cmluZ3NbbGFzdEluZGV4XTtcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgICAgIC8vIFdlIGhhdmUgYSBwYXJ0IGZvciBlYWNoIG1hdGNoIGZvdW5kXG4gICAgICAgICAgICAgICAgICAgICAgICBwYXJ0SW5kZXggKz0gbGFzdEluZGV4O1xuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIGVsc2UgaWYgKG5vZGUubm9kZVR5cGUgPT09IDggLyogTm9kZS5DT01NRU5UX05PREUgKi8pIHtcbiAgICAgICAgICAgICAgICAgICAgaWYgKG5vZGUuZGF0YSA9PT0gbWFya2VyKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICBjb25zdCBwYXJlbnQgPSBub2RlLnBhcmVudE5vZGU7XG4gICAgICAgICAgICAgICAgICAgICAgICAvLyBBZGQgYSBuZXcgbWFya2VyIG5vZGUgdG8gYmUgdGhlIHN0YXJ0Tm9kZSBvZiB0aGUgUGFydCBpZiBhbnkgb2ZcbiAgICAgICAgICAgICAgICAgICAgICAgIC8vIHRoZSBmb2xsb3dpbmcgYXJlIHRydWU6XG4gICAgICAgICAgICAgICAgICAgICAgICAvLyAgKiBXZSBkb24ndCBoYXZlIGEgcHJldmlvdXNTaWJsaW5nXG4gICAgICAgICAgICAgICAgICAgICAgICAvLyAgKiBUaGUgcHJldmlvdXNTaWJsaW5nIGlzIGFscmVhZHkgdGhlIHN0YXJ0IG9mIGEgcHJldmlvdXMgcGFydFxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKG5vZGUucHJldmlvdXNTaWJsaW5nID09PSBudWxsIHx8IGluZGV4ID09PSBsYXN0UGFydEluZGV4KSB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaW5kZXgrKztcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBwYXJlbnQuaW5zZXJ0QmVmb3JlKGNyZWF0ZU1hcmtlcigpLCBub2RlKTtcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgICAgIGxhc3RQYXJ0SW5kZXggPSBpbmRleDtcbiAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMucGFydHMucHVzaCh7IHR5cGU6ICdub2RlJywgaW5kZXggfSk7XG4gICAgICAgICAgICAgICAgICAgICAgICAvLyBJZiB3ZSBkb24ndCBoYXZlIGEgbmV4dFNpYmxpbmcsIGtlZXAgdGhpcyBub2RlIHNvIHdlIGhhdmUgYW4gZW5kLlxuICAgICAgICAgICAgICAgICAgICAgICAgLy8gRWxzZSwgd2UgY2FuIHJlbW92ZSBpdCB0byBzYXZlIGZ1dHVyZSBjb3N0cy5cbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChub2RlLm5leHRTaWJsaW5nID09PSBudWxsKSB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbm9kZS5kYXRhID0gJyc7XG4gICAgICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgICAgICBlbHNlIHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBub2Rlc1RvUmVtb3ZlLnB1c2gobm9kZSk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaW5kZXgtLTtcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgICAgIHBhcnRJbmRleCsrO1xuICAgICAgICAgICAgICAgICAgICB9XG4gICAgICAgICAgICAgICAgICAgIGVsc2Uge1xuICAgICAgICAgICAgICAgICAgICAgICAgbGV0IGkgPSAtMTtcbiAgICAgICAgICAgICAgICAgICAgICAgIHdoaWxlICgoaSA9IG5vZGUuZGF0YS5pbmRleE9mKG1hcmtlciwgaSArIDEpKSAhPT1cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAtMSkge1xuICAgICAgICAgICAgICAgICAgICAgICAgICAgIC8vIENvbW1lbnQgbm9kZSBoYXMgYSBiaW5kaW5nIG1hcmtlciBpbnNpZGUsIG1ha2UgYW4gaW5hY3RpdmUgcGFydFxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIC8vIFRoZSBiaW5kaW5nIHdvbid0IHdvcmssIGJ1dCBzdWJzZXF1ZW50IGJpbmRpbmdzIHdpbGxcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAvLyBUT0RPIChqdXN0aW5mYWduYW5pKTogY29uc2lkZXIgd2hldGhlciBpdCdzIGV2ZW4gd29ydGggaXQgdG9cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAvLyBtYWtlIGJpbmRpbmdzIGluIGNvbW1lbnRzIHdvcmtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLnBhcnRzLnB1c2goeyB0eXBlOiAnbm9kZScsIGluZGV4OiAtMSB9KTtcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgIH1cbiAgICAgICAgfTtcbiAgICAgICAgX3ByZXBhcmVUZW1wbGF0ZShlbGVtZW50KTtcbiAgICAgICAgLy8gUmVtb3ZlIHRleHQgYmluZGluZyBub2RlcyBhZnRlciB0aGUgd2FsayB0byBub3QgZGlzdHVyYiB0aGUgVHJlZVdhbGtlclxuICAgICAgICBmb3IgKGNvbnN0IG4gb2Ygbm9kZXNUb1JlbW92ZSkge1xuICAgICAgICAgICAgbi5wYXJlbnROb2RlLnJlbW92ZUNoaWxkKG4pO1xuICAgICAgICB9XG4gICAgfVxufVxuZXhwb3J0IGNvbnN0IGlzVGVtcGxhdGVQYXJ0QWN0aXZlID0gKHBhcnQpID0+IHBhcnQuaW5kZXggIT09IC0xO1xuLy8gQWxsb3dzIGBkb2N1bWVudC5jcmVhdGVDb21tZW50KCcnKWAgdG8gYmUgcmVuYW1lZCBmb3IgYVxuLy8gc21hbGwgbWFudWFsIHNpemUtc2F2aW5ncy5cbmV4cG9ydCBjb25zdCBjcmVhdGVNYXJrZXIgPSAoKSA9PiBkb2N1bWVudC5jcmVhdGVDb21tZW50KCcnKTtcbi8qKlxuICogVGhpcyByZWdleCBleHRyYWN0cyB0aGUgYXR0cmlidXRlIG5hbWUgcHJlY2VkaW5nIGFuIGF0dHJpYnV0ZS1wb3NpdGlvblxuICogZXhwcmVzc2lvbi4gSXQgZG9lcyB0aGlzIGJ5IG1hdGNoaW5nIHRoZSBzeW50YXggYWxsb3dlZCBmb3IgYXR0cmlidXRlc1xuICogYWdhaW5zdCB0aGUgc3RyaW5nIGxpdGVyYWwgZGlyZWN0bHkgcHJlY2VkaW5nIHRoZSBleHByZXNzaW9uLCBhc3N1bWluZyB0aGF0XG4gKiB0aGUgZXhwcmVzc2lvbiBpcyBpbiBhbiBhdHRyaWJ1dGUtdmFsdWUgcG9zaXRpb24uXG4gKlxuICogU2VlIGF0dHJpYnV0ZXMgaW4gdGhlIEhUTUwgc3BlYzpcbiAqIGh0dHBzOi8vd3d3LnczLm9yZy9UUi9odG1sNS9zeW50YXguaHRtbCNhdHRyaWJ1dGVzLTBcbiAqXG4gKiBcIlxcMC1cXHgxRlxceDdGLVxceDlGXCIgYXJlIFVuaWNvZGUgY29udHJvbCBjaGFyYWN0ZXJzXG4gKlxuICogXCIgXFx4MDlcXHgwYVxceDBjXFx4MGRcIiBhcmUgSFRNTCBzcGFjZSBjaGFyYWN0ZXJzOlxuICogaHR0cHM6Ly93d3cudzMub3JnL1RSL2h0bWw1L2luZnJhc3RydWN0dXJlLmh0bWwjc3BhY2UtY2hhcmFjdGVyXG4gKlxuICogU28gYW4gYXR0cmlidXRlIGlzOlxuICogICogVGhlIG5hbWU6IGFueSBjaGFyYWN0ZXIgZXhjZXB0IGEgY29udHJvbCBjaGFyYWN0ZXIsIHNwYWNlIGNoYXJhY3RlciwgKCcpLFxuICogICAgKFwiKSwgXCI+XCIsIFwiPVwiLCBvciBcIi9cIlxuICogICogRm9sbG93ZWQgYnkgemVybyBvciBtb3JlIHNwYWNlIGNoYXJhY3RlcnNcbiAqICAqIEZvbGxvd2VkIGJ5IFwiPVwiXG4gKiAgKiBGb2xsb3dlZCBieSB6ZXJvIG9yIG1vcmUgc3BhY2UgY2hhcmFjdGVyc1xuICogICogRm9sbG93ZWQgYnk6XG4gKiAgICAqIEFueSBjaGFyYWN0ZXIgZXhjZXB0IHNwYWNlLCAoJyksIChcIiksIFwiPFwiLCBcIj5cIiwgXCI9XCIsIChgKSwgb3JcbiAqICAgICogKFwiKSB0aGVuIGFueSBub24tKFwiKSwgb3JcbiAqICAgICogKCcpIHRoZW4gYW55IG5vbi0oJylcbiAqL1xuZXhwb3J0IGNvbnN0IGxhc3RBdHRyaWJ1dGVOYW1lUmVnZXggPSAvKFsgXFx4MDlcXHgwYVxceDBjXFx4MGRdKShbXlxcMC1cXHgxRlxceDdGLVxceDlGIFxceDA5XFx4MGFcXHgwY1xceDBkXCInPj0vXSspKFsgXFx4MDlcXHgwYVxceDBjXFx4MGRdKj1bIFxceDA5XFx4MGFcXHgwY1xceDBkXSooPzpbXiBcXHgwOVxceDBhXFx4MGNcXHgwZFwiJ2A8Pj1dKnxcIlteXCJdKnwnW14nXSopKSQvO1xuLy8jIHNvdXJjZU1hcHBpbmdVUkw9dGVtcGxhdGUuanMubWFwIiwiLyoqXG4gKiBAbGljZW5zZVxuICogQ29weXJpZ2h0IChjKSAyMDE3IFRoZSBQb2x5bWVyIFByb2plY3QgQXV0aG9ycy4gQWxsIHJpZ2h0cyByZXNlcnZlZC5cbiAqIFRoaXMgY29kZSBtYXkgb25seSBiZSB1c2VkIHVuZGVyIHRoZSBCU0Qgc3R5bGUgbGljZW5zZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0xJQ0VOU0UudHh0XG4gKiBUaGUgY29tcGxldGUgc2V0IG9mIGF1dGhvcnMgbWF5IGJlIGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vQVVUSE9SUy50eHRcbiAqIFRoZSBjb21wbGV0ZSBzZXQgb2YgY29udHJpYnV0b3JzIG1heSBiZSBmb3VuZCBhdFxuICogaHR0cDovL3BvbHltZXIuZ2l0aHViLmlvL0NPTlRSSUJVVE9SUy50eHRcbiAqIENvZGUgZGlzdHJpYnV0ZWQgYnkgR29vZ2xlIGFzIHBhcnQgb2YgdGhlIHBvbHltZXIgcHJvamVjdCBpcyBhbHNvXG4gKiBzdWJqZWN0IHRvIGFuIGFkZGl0aW9uYWwgSVAgcmlnaHRzIGdyYW50IGZvdW5kIGF0XG4gKiBodHRwOi8vcG9seW1lci5naXRodWIuaW8vUEFURU5UUy50eHRcbiAqL1xuLyoqXG4gKlxuICogTWFpbiBsaXQtaHRtbCBtb2R1bGUuXG4gKlxuICogTWFpbiBleHBvcnRzOlxuICpcbiAqIC0gIFtbaHRtbF1dXG4gKiAtICBbW3N2Z11dXG4gKiAtICBbW3JlbmRlcl1dXG4gKlxuICogQG1vZHVsZSBsaXQtaHRtbFxuICogQHByZWZlcnJlZFxuICovXG4vKipcbiAqIERvIG5vdCByZW1vdmUgdGhpcyBjb21tZW50OyBpdCBrZWVwcyB0eXBlZG9jIGZyb20gbWlzcGxhY2luZyB0aGUgbW9kdWxlXG4gKiBkb2NzLlxuICovXG5pbXBvcnQgeyBkZWZhdWx0VGVtcGxhdGVQcm9jZXNzb3IgfSBmcm9tICcuL2xpYi9kZWZhdWx0LXRlbXBsYXRlLXByb2Nlc3Nvci5qcyc7XG5pbXBvcnQgeyBTVkdUZW1wbGF0ZVJlc3VsdCwgVGVtcGxhdGVSZXN1bHQgfSBmcm9tICcuL2xpYi90ZW1wbGF0ZS1yZXN1bHQuanMnO1xuZXhwb3J0IHsgRGVmYXVsdFRlbXBsYXRlUHJvY2Vzc29yLCBkZWZhdWx0VGVtcGxhdGVQcm9jZXNzb3IgfSBmcm9tICcuL2xpYi9kZWZhdWx0LXRlbXBsYXRlLXByb2Nlc3Nvci5qcyc7XG5leHBvcnQgeyBkaXJlY3RpdmUsIGlzRGlyZWN0aXZlIH0gZnJvbSAnLi9saWIvZGlyZWN0aXZlLmpzJztcbi8vIFRPRE8oanVzdGluZmFnbmFuaSk6IHJlbW92ZSBsaW5lIHdoZW4gd2UgZ2V0IE5vZGVQYXJ0IG1vdmluZyBtZXRob2RzXG5leHBvcnQgeyByZW1vdmVOb2RlcywgcmVwYXJlbnROb2RlcyB9IGZyb20gJy4vbGliL2RvbS5qcyc7XG5leHBvcnQgeyBub0NoYW5nZSwgbm90aGluZyB9IGZyb20gJy4vbGliL3BhcnQuanMnO1xuZXhwb3J0IHsgQXR0cmlidXRlQ29tbWl0dGVyLCBBdHRyaWJ1dGVQYXJ0LCBCb29sZWFuQXR0cmlidXRlUGFydCwgRXZlbnRQYXJ0LCBpc1ByaW1pdGl2ZSwgTm9kZVBhcnQsIFByb3BlcnR5Q29tbWl0dGVyLCBQcm9wZXJ0eVBhcnQgfSBmcm9tICcuL2xpYi9wYXJ0cy5qcyc7XG5leHBvcnQgeyBwYXJ0cywgcmVuZGVyIH0gZnJvbSAnLi9saWIvcmVuZGVyLmpzJztcbmV4cG9ydCB7IHRlbXBsYXRlQ2FjaGVzLCB0ZW1wbGF0ZUZhY3RvcnkgfSBmcm9tICcuL2xpYi90ZW1wbGF0ZS1mYWN0b3J5LmpzJztcbmV4cG9ydCB7IFRlbXBsYXRlSW5zdGFuY2UgfSBmcm9tICcuL2xpYi90ZW1wbGF0ZS1pbnN0YW5jZS5qcyc7XG5leHBvcnQgeyBTVkdUZW1wbGF0ZVJlc3VsdCwgVGVtcGxhdGVSZXN1bHQgfSBmcm9tICcuL2xpYi90ZW1wbGF0ZS1yZXN1bHQuanMnO1xuZXhwb3J0IHsgY3JlYXRlTWFya2VyLCBpc1RlbXBsYXRlUGFydEFjdGl2ZSwgVGVtcGxhdGUgfSBmcm9tICcuL2xpYi90ZW1wbGF0ZS5qcyc7XG4vLyBJTVBPUlRBTlQ6IGRvIG5vdCBjaGFuZ2UgdGhlIHByb3BlcnR5IG5hbWUgb3IgdGhlIGFzc2lnbm1lbnQgZXhwcmVzc2lvbi5cbi8vIFRoaXMgbGluZSB3aWxsIGJlIHVzZWQgaW4gcmVnZXhlcyB0byBzZWFyY2ggZm9yIGxpdC1odG1sIHVzYWdlLlxuLy8gVE9ETyhqdXN0aW5mYWduYW5pKTogaW5qZWN0IHZlcnNpb24gbnVtYmVyIGF0IGJ1aWxkIHRpbWVcbih3aW5kb3dbJ2xpdEh0bWxWZXJzaW9ucyddIHx8ICh3aW5kb3dbJ2xpdEh0bWxWZXJzaW9ucyddID0gW10pKS5wdXNoKCcxLjAuMCcpO1xuLyoqXG4gKiBJbnRlcnByZXRzIGEgdGVtcGxhdGUgbGl0ZXJhbCBhcyBhbiBIVE1MIHRlbXBsYXRlIHRoYXQgY2FuIGVmZmljaWVudGx5XG4gKiByZW5kZXIgdG8gYW5kIHVwZGF0ZSBhIGNvbnRhaW5lci5cbiAqL1xuZXhwb3J0IGNvbnN0IGh0bWwgPSAoc3RyaW5ncywgLi4udmFsdWVzKSA9PiBuZXcgVGVtcGxhdGVSZXN1bHQoc3RyaW5ncywgdmFsdWVzLCAnaHRtbCcsIGRlZmF1bHRUZW1wbGF0ZVByb2Nlc3Nvcik7XG4vKipcbiAqIEludGVycHJldHMgYSB0ZW1wbGF0ZSBsaXRlcmFsIGFzIGFuIFNWRyB0ZW1wbGF0ZSB0aGF0IGNhbiBlZmZpY2llbnRseVxuICogcmVuZGVyIHRvIGFuZCB1cGRhdGUgYSBjb250YWluZXIuXG4gKi9cbmV4cG9ydCBjb25zdCBzdmcgPSAoc3RyaW5ncywgLi4udmFsdWVzKSA9PiBuZXcgU1ZHVGVtcGxhdGVSZXN1bHQoc3RyaW5ncywgdmFsdWVzLCAnc3ZnJywgZGVmYXVsdFRlbXBsYXRlUHJvY2Vzc29yKTtcbi8vIyBzb3VyY2VNYXBwaW5nVVJMPWxpdC1odG1sLmpzLm1hcCIsImltcG9ydCB7IExpdEVsZW1lbnQsIGh0bWwsIHByb3BlcnR5LCBjdXN0b21FbGVtZW50LCBjc3MsIHVuc2FmZUNTUyB9IGZyb20gJ2xpdC1lbGVtZW50JztcclxuaW1wb3J0IHsgTXV0YXRpb25UZXN0UmVzdWx0LCBGaWxlUmVzdWx0IH0gZnJvbSAnLi4vLi4vYXBpJztcclxuaW1wb3J0IHsgUk9PVF9OQU1FLCBub3JtYWxpemVGaWxlTmFtZXMgfSBmcm9tICcuLi9oZWxwZXJzJztcclxuaW1wb3J0IHsgYm9vdHN0cmFwIH0gZnJvbSAnLi4vc3R5bGUnO1xyXG5cclxuQGN1c3RvbUVsZW1lbnQoJ211dGF0aW9uLXRlc3QtcmVwb3J0LWFwcCcpXHJcbmV4cG9ydCBjbGFzcyBNdXRhdGlvblRlc3RSZXBvcnRBcHBDb21wb25lbnQgZXh0ZW5kcyBMaXRFbGVtZW50IHtcclxuICBAcHJvcGVydHkoKVxyXG4gIHByaXZhdGUgcmVwb3J0OiBNdXRhdGlvblRlc3RSZXN1bHQgfCB1bmRlZmluZWQ7XHJcblxyXG4gIEBwcm9wZXJ0eSgpXHJcbiAgcHVibGljIHNyYzogc3RyaW5nIHwgdW5kZWZpbmVkO1xyXG5cclxuICBAcHJvcGVydHkoKVxyXG4gIHB1YmxpYyBlcnJvck1lc3NhZ2U6IHN0cmluZyB8IHVuZGVmaW5lZDtcclxuXHJcbiAgQHByb3BlcnR5KClcclxuICBwdWJsaWMgY29udGV4dDogRmlsZVJlc3VsdCB8IHVuZGVmaW5lZDtcclxuXHJcbiAgQHByb3BlcnR5KClcclxuICBwdWJsaWMgcGF0aDogc3RyaW5nIHwgdW5kZWZpbmVkO1xyXG5cclxuICBAcHJvcGVydHkoKVxyXG4gIHB1YmxpYyBnZXQgdGl0bGUoKTogc3RyaW5nIHtcclxuICAgIGlmICh0aGlzLmNvbnRleHQgJiYgdGhpcy5wYXRoKSB7XHJcbiAgICAgIHJldHVybiB0aGlzLnBhdGg7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICByZXR1cm4gUk9PVF9OQU1FO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgcHVibGljIGNvbm5lY3RlZENhbGxiYWNrKCkge1xyXG4gICAgc3VwZXIuY29ubmVjdGVkQ2FsbGJhY2soKTtcclxuICAgIGlmICh0aGlzLnNyYykge1xyXG4gICAgICB0aGlzLmxvYWREYXRhKHRoaXMuc3JjKVxyXG4gICAgICAgIC5jYXRjaChlcnJvciA9PlxyXG4gICAgICAgICAgdGhpcy5lcnJvck1lc3NhZ2UgPSBlcnJvci50b1N0cmluZygpKTtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgIHRoaXMuZXJyb3JNZXNzYWdlID0gJ1NvdXJjZSBub3Qgc2V0LiBQbGVhc2UgcG9pbnQgdGhlIGBzcmNgIGF0dHJpYnV0ZSB0byB0aGUgbXV0YXRpb24gdGVzdCByZXBvcnQgZGF0YS4nO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSBhc3luYyBsb2FkRGF0YShzcmM6IHN0cmluZykge1xyXG4gICAgY29uc3QgcmVzID0gYXdhaXQgZmV0Y2goc3JjKTtcclxuICAgIGNvbnN0IHJlcG9ydDogTXV0YXRpb25UZXN0UmVzdWx0ID0gYXdhaXQgcmVzLmpzb24oKTtcclxuICAgIHJlcG9ydC5maWxlcyA9IG5vcm1hbGl6ZUZpbGVOYW1lcyhyZXBvcnQuZmlsZXMpO1xyXG4gICAgdGhpcy5yZXBvcnQgPSByZXBvcnQ7XHJcbiAgICB0aGlzLnVwZGF0ZUNvbnRleHQoKTtcclxuICB9XHJcblxyXG4gIHByaXZhdGUgdXBkYXRlQ29udGV4dCgpIHtcclxuICAgIGlmICh0aGlzLnJlcG9ydCkge1xyXG4gICAgICBpZiAodGhpcy5wYXRoKSB7XHJcbiAgICAgICAgdGhpcy5jb250ZXh0ID0gdGhpcy5yZXBvcnQuZmlsZXNbdGhpcy5wYXRoXTtcclxuICAgICAgICBpZiAoIXRoaXMuY29udGV4dCkge1xyXG4gICAgICAgICAgdGhpcy5lcnJvck1lc3NhZ2UgPSBgNDA0IC0gJHt0aGlzLnBhdGh9IG5vdCBmb3VuZGA7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgIHRoaXMuZXJyb3JNZXNzYWdlID0gdW5kZWZpbmVkO1xyXG4gICAgICAgIH1cclxuICAgICAgfSBlbHNlIHtcclxuICAgICAgICB0aGlzLmNvbnRleHQgPSB1bmRlZmluZWQ7XHJcbiAgICAgIH1cclxuICAgIH1cclxuICB9XHJcblxyXG4gIHByaXZhdGUgcmVhZG9ubHkgdXBkYXRlUGF0aCA9IChldmVudDogQ3VzdG9tRXZlbnQpID0+IHtcclxuICAgIHRoaXMucGF0aCA9IGV2ZW50LmRldGFpbDtcclxuICAgIHRoaXMudXBkYXRlQ29udGV4dCgpO1xyXG4gIH1cclxuXHJcbiAgcHVibGljIHN0YXRpYyBzdHlsZXMgPSBbXHJcbiAgICBib290c3RyYXAsXHJcbiAgICBjc3NgXHJcbiAgICA6aG9zdCB7XHJcbiAgICAgIGxpbmUtaGVpZ2h0OiAxLjE1O1xyXG4gICAgICBtYXJnaW46IDA7XHJcbiAgICAgIGZvbnQtZmFtaWx5OiAtYXBwbGUtc3lzdGVtLCBCbGlua01hY1N5c3RlbUZvbnQsIFwiU2Vnb2UgVUlcIiwgUm9ib3RvLCBcIkhlbHZldGljYSBOZXVlXCIsIEFyaWFsLCBcIk5vdG8gU2Fuc1wiLCBzYW5zLXNlcmlmLCBcIkFwcGxlIENvbG9yIEVtb2ppXCIsIFwiU2Vnb2UgVUkgRW1vamlcIiwgXCJTZWdvZSBVSSBTeW1ib2xcIiwgXCJOb3RvIENvbG9yIEVtb2ppXCI7XHJcbiAgICAgIGZvbnQtc2l6ZTogMXJlbTtcclxuICAgICAgZm9udC13ZWlnaHQ6IDQwMDtcclxuICAgICAgbGluZS1oZWlnaHQ6IDEuNTtcclxuICAgICAgY29sb3I6ICMyMTI1Mjk7XHJcbiAgICAgIHRleHQtYWxpZ246IGxlZnQ7XHJcbiAgICAgIGJhY2tncm91bmQtY29sb3I6ICNmZmY7XHJcbiAgICB9XHJcbiAgICBgXHJcbiAgXTtcclxuXHJcbiAgcHVibGljIHJlbmRlcigpIHtcclxuICAgIHJldHVybiBodG1sYFxyXG4gICAgPG11dGF0aW9uLXRlc3QtcmVwb3J0LXRpdGxlIC50aXRsZT1cIiR7dGhpcy50aXRsZX1cIj48L211dGF0aW9uLXRlc3QtcmVwb3J0LXRpdGxlPlxyXG4gICAgPG11dGF0aW9uLXRlc3QtcmVwb3J0LXJvdXRlciBAcGF0aC1jaGFuZ2VkPVwiJHt0aGlzLnVwZGF0ZVBhdGh9XCI+PC9tdXRhdGlvbi10ZXN0LXJlcG9ydC1yb3V0ZXI+XHJcbiAgICA8ZGl2IGNsYXNzPVwiY29udGFpbmVyXCI+XHJcbiAgICAgIDxkaXYgY2xhc3M9XCJyb3dcIj5cclxuICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLW1kLTEyXCI+XHJcbiAgICAgICAgICAke3RoaXMucmVuZGVyVGl0bGUoKX1cclxuICAgICAgICAgIDxtdXRhdGlvbi10ZXN0LXJlcG9ydC1icmVhZGNydW1iIC5wYXRoPVwiJHt0aGlzLnBhdGh9XCI+PC9tdXRhdGlvbi10ZXN0LXJlcG9ydC1icmVhZGNydW1iPlxyXG4gICAgICAgICAgJHt0aGlzLnJlbmRlckVycm9yTWVzc2FnZSgpfVxyXG4gICAgICAgICAgJHt0aGlzLnJlbmRlck11dGF0aW9uVGVzdFJlcG9ydCgpfVxyXG4gICAgICAgIDwvZGl2PlxyXG4gICAgICA8L2Rpdj5cclxuICAgIDwvZGl2PlxyXG4gICAgYDtcclxuICB9XHJcblxyXG4gIHByaXZhdGUgcmVuZGVyVGl0bGUoKSB7XHJcbiAgICBpZiAodGhpcy5yZXBvcnQpIHtcclxuICAgICAgcmV0dXJuIGh0bWxgPGgxIGNsYXNzPVwiZGlzcGxheS00XCI+JHt0aGlzLnRpdGxlfTwvaDE+YDtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgIHJldHVybiB1bmRlZmluZWQ7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlckVycm9yTWVzc2FnZSgpIHtcclxuICAgIGlmICh0aGlzLmVycm9yTWVzc2FnZSkge1xyXG4gICAgICByZXR1cm4gaHRtbGBcclxuICAgICAgPGRpdiBjbGFzcz1cImFsZXJ0IGFsZXJ0LWRhbmdlclwiIHJvbGU9XCJhbGVydFwiPlxyXG4gICAgICAgICR7dGhpcy5lcnJvck1lc3NhZ2V9XHJcbiAgICAgIDwvZGl2PlxyXG4gICAgICAgIGA7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICByZXR1cm4gaHRtbGBgO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSByZW5kZXJNdXRhdGlvblRlc3RSZXBvcnQoKSB7XHJcbiAgICBpZiAodGhpcy5jb250ZXh0ICYmIHRoaXMucmVwb3J0KSB7XHJcbiAgICAgIHJldHVybiBodG1sYDxtdXRhdGlvbi10ZXN0LXJlcG9ydC1maWxlIC5uYW1lPVwiJHt0aGlzLnRpdGxlfVwiIC50aHJlc2hvbGRzPVwiJHt0aGlzLnJlcG9ydC50aHJlc2hvbGRzfVwiIC5tb2RlbD1cIiR7dGhpcy5jb250ZXh0fVwiPjwvbXV0YXRpb24tdGVzdC1yZXBvcnQtZmlsZT5gO1xyXG4gICAgfSBlbHNlIGlmICh0aGlzLnJlcG9ydCkge1xyXG4gICAgICByZXR1cm4gaHRtbGA8bXV0YXRpb24tdGVzdC1yZXBvcnQtcmVzdWx0IC5tb2RlbD1cIiR7dGhpcy5yZXBvcnR9XCI+PC9tdXRhdGlvbi10ZXN0LXJlcG9ydC1yZXN1bHQ+YDtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgIHJldHVybiAnJztcclxuICAgIH1cclxuICB9XHJcbn1cclxuIiwiaW1wb3J0IHsgTGl0RWxlbWVudCwgaHRtbCwgcHJvcGVydHksIGN1c3RvbUVsZW1lbnQgfSBmcm9tICdsaXQtZWxlbWVudCc7XHJcbmltcG9ydCB7IGJvb3RzdHJhcCB9IGZyb20gJy4uL3N0eWxlJztcclxuaW1wb3J0IHsgUk9PVF9OQU1FIH0gZnJvbSAnLi4vaGVscGVycyc7XHJcblxyXG5AY3VzdG9tRWxlbWVudCgnbXV0YXRpb24tdGVzdC1yZXBvcnQtYnJlYWRjcnVtYicpXHJcbmV4cG9ydCBjbGFzcyBNdXRhdGlvblRlc3RSZXBvcnRCcmVhZGNydW1iQ29tcG9uZW50IGV4dGVuZHMgTGl0RWxlbWVudCB7XHJcbiAgQHByb3BlcnR5KClcclxuICBwdWJsaWMgcGF0aDogc3RyaW5nIHwgdW5kZWZpbmVkO1xyXG5cclxuICBwdWJsaWMgc3RhdGljIHN0eWxlcyA9IFtib290c3RyYXBdO1xyXG5cclxuICBwdWJsaWMgcmVuZGVyKCkge1xyXG4gICAgcmV0dXJuIGh0bWxgXHJcbiAgICAgICAgPG9sIGNsYXNzPSdicmVhZGNydW1iJz5cclxuICAgICAgICAgICR7dGhpcy5yZW5kZXJSb290SXRlbSgpfVxyXG4gICAgICAgICAgJHt0aGlzLnBhdGggPyB0aGlzLnJlbmRlckJyZWFkY3J1bWJJdGVtcyhbdGhpcy5wYXRoXSkgOiAnJ31cclxuICAgICAgICA8L29sPlxyXG4gICAgYDtcclxuICB9XHJcblxyXG4gIHByaXZhdGUgcmVuZGVyUm9vdEl0ZW0oKSB7XHJcbiAgICBpZiAodGhpcy5wYXRoICYmIHRoaXMucGF0aC5sZW5ndGgpIHtcclxuICAgICAgcmV0dXJuIHRoaXMucmVuZGVyTGluayhST09UX05BTUUsICcjJyk7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICByZXR1cm4gdGhpcy5yZW5kZXJBY3RpdmVJdGVtKFJPT1RfTkFNRSk7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlckJyZWFkY3J1bWJJdGVtcyhwYXRoOiBSZWFkb25seUFycmF5PHN0cmluZz4pIHtcclxuICAgIHJldHVybiBwYXRoLm1hcCgoaXRlbSwgaW5kZXgpID0+IHtcclxuICAgICAgaWYgKGluZGV4ID09PSBwYXRoLmxlbmd0aCAtIDEpIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5yZW5kZXJBY3RpdmVJdGVtKGl0ZW0pO1xyXG4gICAgICB9IGVsc2Uge1xyXG4gICAgICAgIHJldHVybiB0aGlzLnJlbmRlckxpbmsoaXRlbSwgYCMke3BhdGguZmlsdGVyKChfLCBpKSA9PiBpIDw9IGluZGV4KS5qb2luKCcvJyl9YCk7XHJcbiAgICAgIH1cclxuICAgIH0pO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSByZW5kZXJBY3RpdmVJdGVtKHRpdGxlOiBzdHJpbmcpIHtcclxuICAgIHJldHVybiBodG1sYDxsaSBjbGFzcz1cImJyZWFkY3J1bWItaXRlbSBhY3RpdmVcIiBhcmlhLWN1cnJlbnQ9XCJwYWdlXCI+JHt0aXRsZX08L2xpPmA7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlckxpbmsodGl0bGU6IHN0cmluZywgdXJsOiBzdHJpbmcpIHtcclxuICAgIHJldHVybiBodG1sYDxsaSBjbGFzcz1cImJyZWFkY3J1bWItaXRlbVwiPjxhIGhyZWY9XCIke3VybH1cIj4ke3RpdGxlfTwvYT48L2xpPmA7XHJcbiAgfVxyXG59XHJcbiIsImltcG9ydCB7IGN1c3RvbUVsZW1lbnQsIExpdEVsZW1lbnQsIHByb3BlcnR5LCBQcm9wZXJ0eVZhbHVlcywgaHRtbCwgY3NzIH0gZnJvbSAnbGl0LWVsZW1lbnQnO1xyXG5pbXBvcnQgeyBNdXRhbnRSZXN1bHQsIE11dGFudFN0YXR1cyB9IGZyb20gJy4uLy4uL2FwaSc7XHJcbmltcG9ydCB7IGJvb3RzdHJhcCB9IGZyb20gJy4uL3N0eWxlJztcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgTXV0YW50RmlsdGVyIHtcclxuICBzdGF0dXM6IE11dGFudFN0YXR1cztcclxuICBudW1iZXJPZk11dGFudHM6IG51bWJlcjtcclxuICBlbmFibGVkOiBib29sZWFuO1xyXG59XHJcblxyXG5AY3VzdG9tRWxlbWVudCgnbXV0YXRpb24tdGVzdC1yZXBvcnQtZmlsZS1sZWdlbmQnKVxyXG5leHBvcnQgY2xhc3MgTXV0YXRpb25UZXN0UmVwb3J0RmlsZUxlZ2VuZENvbXBvbmVudCBleHRlbmRzIExpdEVsZW1lbnQge1xyXG5cclxuICBAcHJvcGVydHkoKVxyXG4gIHB1YmxpYyBtdXRhbnRzITogUmVhZG9ubHlBcnJheTxNdXRhbnRSZXN1bHQ+O1xyXG5cclxuICBAcHJvcGVydHkoKVxyXG4gIHByaXZhdGUgZ2V0IGNvbGxhcHNlQnV0dG9uVGV4dCgpIHtcclxuICAgIGlmICh0aGlzLmNvbGxhcHNlZCkge1xyXG4gICAgICByZXR1cm4gJ0V4cGFuZCBhbGwnO1xyXG4gICAgfSBlbHNlIHtcclxuICAgICAgcmV0dXJuICdDb2xsYXBzZSBhbGwnO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgQHByb3BlcnR5KClcclxuICBwcml2YXRlIGNvbGxhcHNlZCA9IHRydWU7XHJcblxyXG4gIEBwcm9wZXJ0eSgpXHJcbiAgcHJpdmF0ZSBmaWx0ZXJzOiBNdXRhbnRGaWx0ZXJbXSA9IFtdO1xyXG5cclxuICBwdWJsaWMgdXBkYXRlZChjaGFuZ2VkUHJvcGVydGllczogUHJvcGVydHlWYWx1ZXMpIHtcclxuICAgIGlmIChjaGFuZ2VkUHJvcGVydGllcy5oYXMoJ211dGFudHMnKSkge1xyXG4gICAgICB0aGlzLnVwZGF0ZU1vZGVsKCk7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHVwZGF0ZU1vZGVsKCkge1xyXG4gICAgdGhpcy5maWx0ZXJzID0gW011dGFudFN0YXR1cy5LaWxsZWQsIE11dGFudFN0YXR1cy5TdXJ2aXZlZCwgTXV0YW50U3RhdHVzLk5vQ292ZXJhZ2UsIE11dGFudFN0YXR1cy5UaW1lb3V0LCBNdXRhbnRTdGF0dXMuQ29tcGlsZUVycm9yLCBNdXRhbnRTdGF0dXMuUnVudGltZUVycm9yXVxyXG4gICAgICAuZmlsdGVyKHN0YXR1cyA9PiB0aGlzLm11dGFudHMuc29tZShtdXRhbnQgPT4gbXV0YW50LnN0YXR1cyA9PT0gc3RhdHVzKSlcclxuICAgICAgLm1hcChzdGF0dXMgPT4gKHtcclxuICAgICAgICBlbmFibGVkOiBbTXV0YW50U3RhdHVzLlN1cnZpdmVkLCBNdXRhbnRTdGF0dXMuTm9Db3ZlcmFnZSwgTXV0YW50U3RhdHVzLlRpbWVvdXRdLnNvbWUocyA9PiBzID09PSBzdGF0dXMpLFxyXG4gICAgICAgIG51bWJlck9mTXV0YW50czogdGhpcy5tdXRhbnRzLmZpbHRlcihtID0+IG0uc3RhdHVzID09PSBzdGF0dXMpLmxlbmd0aCxcclxuICAgICAgICBzdGF0dXNcclxuICAgICAgfSkpO1xyXG4gICAgdGhpcy5kaXNwYXRjaEZpbHRlcnNDaGFuZ2VkRXZlbnQoKTtcclxuICB9XHJcblxyXG4gIHByaXZhdGUgY2hlY2tib3hDbGlja2VkKGZpbHRlcjogTXV0YW50RmlsdGVyKSB7XHJcbiAgICBmaWx0ZXIuZW5hYmxlZCA9ICFmaWx0ZXIuZW5hYmxlZDtcclxuICAgIHRoaXMuZGlzcGF0Y2hGaWx0ZXJzQ2hhbmdlZEV2ZW50KCk7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIGRpc3BhdGNoRmlsdGVyc0NoYW5nZWRFdmVudCgpIHtcclxuICAgIHRoaXMuZGlzcGF0Y2hFdmVudChuZXcgQ3VzdG9tRXZlbnQoJ2ZpbHRlcnMtY2hhbmdlZCcsIHsgZGV0YWlsOiB0aGlzLmZpbHRlcnMgfSkpO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSByZWFkb25seSB0b2dnbGVPcGVuQWxsID0gKCkgPT4ge1xyXG4gICAgdGhpcy5jb2xsYXBzZWQgPSAhdGhpcy5jb2xsYXBzZWQ7XHJcbiAgICBpZiAodGhpcy5jb2xsYXBzZWQpIHtcclxuICAgICAgdGhpcy5kaXNwYXRjaEV2ZW50KG5ldyBDdXN0b21FdmVudCgnY29sbGFwc2UtYWxsJykpO1xyXG4gICAgfSBlbHNlIHtcclxuICAgICAgdGhpcy5kaXNwYXRjaEV2ZW50KG5ldyBDdXN0b21FdmVudCgnb3Blbi1hbGwnKSk7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwdWJsaWMgc3RhdGljIHN0eWxlcyA9IFtcclxuICAgIGNzc2BcclxuICAgICAgLmxlZ2VuZHtcclxuICAgICAgICBwb3NpdGlvbjogc3RpY2t5O1xyXG4gICAgICAgIHRvcDogMDtcclxuICAgICAgICBiYWNrZ3JvdW5kOiAjRkZGO1xyXG4gICAgICB9XHJcbiAgYCwgYm9vdHN0cmFwXTtcclxuXHJcbiAgcHVibGljIHJlbmRlcigpIHtcclxuICAgIHJldHVybiBodG1sYFxyXG4gICAgICA8ZGl2IGNsYXNzPSdyb3cgbGVnZW5kJz5cclxuICAgICAgICA8Zm9ybSBjbGFzcz0nY29sLW1kLTEyJyBub3ZhbGlkYXRlPSdub3ZhbGlkYXRlJz5cclxuICAgICAgICAgICR7dGhpcy5maWx0ZXJzLm1hcChmaWx0ZXIgPT4gaHRtbGBcclxuICAgICAgICAgIDxkaXYgY2xhc3M9XCJmb3JtLWNoZWNrIGZvcm0tY2hlY2staW5saW5lXCI+XHJcbiAgICAgICAgICAgIDxsYWJlbCBjbGFzcz1cImZvcm0tY2hlY2stbGFiZWxcIj5cclxuICAgICAgICAgICAgICA8aW5wdXQgY2xhc3M9XCJmb3JtLWNoZWNrLWlucHV0XCIgdHlwZT1cImNoZWNrYm94XCIgP2NoZWNrZWQ9XCIke2ZpbHRlci5lbmFibGVkfVwiIHZhbHVlPVwiJHtmaWx0ZXIuc3RhdHVzfVwiIEBpbnB1dD1cIiR7KCkgPT4gdGhpcy5jaGVja2JveENsaWNrZWQoZmlsdGVyKX1cIj5cclxuICAgICAgICAgICAgICAke2ZpbHRlci5zdGF0dXN9ICgke2ZpbHRlci5udW1iZXJPZk11dGFudHN9KVxyXG4gICAgICAgICAgICA8L2xhYmVsPlxyXG4gICAgICAgICAgPC9kaXY+XHJcbiAgICAgICAgICBgKX1cclxuICAgICAgICAgIDxidXR0b24gQGNsaWNrPVwiJHt0aGlzLnRvZ2dsZU9wZW5BbGx9XCIgY2xhc3M9XCJidG4gYnRuLXNtIGJ0bi1zZWNvbmRhcnlcIiB0eXBlPVwiYnV0dG9uXCI+JHt0aGlzLmNvbGxhcHNlQnV0dG9uVGV4dH08L2J1dHRvbj5cclxuICAgICAgICA8L2Zvcm0+XHJcbiAgICAgIDwvZGl2PlxyXG4gICAgYDtcclxuICB9XHJcblxyXG59XHJcbiIsImltcG9ydCB7IExpdEVsZW1lbnQsIGh0bWwsIHByb3BlcnR5LCBjdXN0b21FbGVtZW50LCBjc3MsIFByb3BlcnR5VmFsdWVzIH0gZnJvbSAnbGl0LWVsZW1lbnQnO1xyXG5pbXBvcnQgeyB1bnNhZmVIVE1MIH0gZnJvbSAnbGl0LWh0bWwvZGlyZWN0aXZlcy91bnNhZmUtaHRtbCc7XHJcbmltcG9ydCB7IEZpbGVSZXN1bHQsIE11dGFudFN0YXR1cywgTXV0YW50UmVzdWx0IH0gZnJvbSAnLi4vLi4vYXBpJztcclxuaW1wb3J0IGhsanMgZnJvbSAnaGlnaGxpZ2h0LmpzL2xpYi9oaWdobGlnaHQnO1xyXG5pbXBvcnQgamF2YXNjcmlwdCBmcm9tICdoaWdobGlnaHQuanMvbGliL2xhbmd1YWdlcy9qYXZhc2NyaXB0JztcclxuaW1wb3J0IHNjYWxhIGZyb20gJ2hpZ2hsaWdodC5qcy9saWIvbGFuZ3VhZ2VzL3NjYWxhJztcclxuaW1wb3J0IGphdmEgZnJvbSAnaGlnaGxpZ2h0LmpzL2xpYi9sYW5ndWFnZXMvamF2YSc7XHJcbmltcG9ydCBjcyBmcm9tICdoaWdobGlnaHQuanMvbGliL2xhbmd1YWdlcy9jcyc7XHJcbmltcG9ydCB0eXBlc2NyaXB0IGZyb20gJ2hpZ2hsaWdodC5qcy9saWIvbGFuZ3VhZ2VzL3R5cGVzY3JpcHQnO1xyXG5pbXBvcnQgeyBnZXRDb250ZXh0Q2xhc3NGb3JTdGF0dXMsIExJTkVfU1RBUlRfSU5ERVgsIENPTFVNTl9TVEFSVF9JTkRFWCwgZXNjYXBlSHRtbCwgTkVXX0xJTkUsIENBUlJJQUdFX1JFVFVSTiB9IGZyb20gJy4uL2hlbHBlcnMnO1xyXG5pbXBvcnQgeyBNdXRhdGlvblRlc3RSZXBvcnRNdXRhbnRDb21wb25lbnQgfSBmcm9tICcuL211dGF0aW9uLXRlc3QtcmVwb3J0LW11dGFudCc7XHJcbmltcG9ydCB7IE11dGFudEZpbHRlciB9IGZyb20gJy4vbXV0YXRpb24tdGVzdC1yZXBvcnQtZmlsZS1sZWdlbmQnO1xyXG5pbXBvcnQgeyBib290c3RyYXAsIGhpZ2hsaWdodEpTIH0gZnJvbSAnLi4vc3R5bGUnO1xyXG5pbXBvcnQgeyBSZXN1bHRUYWJsZSB9IGZyb20gJy4uL21vZGVsL1Jlc3VsdFRhYmxlJztcclxuaW1wb3J0IHsgVGhyZXNob2xkcywgUG9zaXRpb24gfSBmcm9tICcuLi8uLi9hcGknO1xyXG5cclxuaGxqcy5yZWdpc3Rlckxhbmd1YWdlKCdqYXZhc2NyaXB0JywgamF2YXNjcmlwdCk7XHJcbmhsanMucmVnaXN0ZXJMYW5ndWFnZSgndHlwZXNjcmlwdCcsIHR5cGVzY3JpcHQpO1xyXG5obGpzLnJlZ2lzdGVyTGFuZ3VhZ2UoJ2NzJywgY3MpO1xyXG5obGpzLnJlZ2lzdGVyTGFuZ3VhZ2UoJ2phdmEnLCBqYXZhKTtcclxuaGxqcy5yZWdpc3Rlckxhbmd1YWdlKCdzY2FsYScsIHNjYWxhKTtcclxuXHJcbkBjdXN0b21FbGVtZW50KCdtdXRhdGlvbi10ZXN0LXJlcG9ydC1maWxlJylcclxuZXhwb3J0IGNsYXNzIE11dGF0aW9uVGVzdFJlcG9ydEZpbGVDb21wb25lbnQgZXh0ZW5kcyBMaXRFbGVtZW50IHtcclxuXHJcbiAgQHByb3BlcnR5KClcclxuICBwcml2YXRlIHJlYWRvbmx5IG1vZGVsITogRmlsZVJlc3VsdDtcclxuXHJcbiAgQHByb3BlcnR5KClcclxuICBwcml2YXRlIHJlYWRvbmx5IG5hbWUhOiBzdHJpbmc7XHJcblxyXG4gIEBwcm9wZXJ0eSgpXHJcbiAgcHJpdmF0ZSByZWFkb25seSB0aHJlc2hvbGRzITogVGhyZXNob2xkcztcclxuXHJcbiAgcHJpdmF0ZSBlbmFibGVkTXV0YW50U3RhdGVzOiBNdXRhbnRTdGF0dXNbXSA9IFtdO1xyXG5cclxuICBwdWJsaWMgc3RhdGljIHN0eWxlcyA9IFtcclxuICAgIGhpZ2hsaWdodEpTLFxyXG4gICAgYm9vdHN0cmFwLFxyXG4gICAgY3NzYFxyXG4gICAgLmJnLWRhbmdlci1saWdodCB7XHJcbiAgICAgIGJhY2tncm91bmQtY29sb3I6ICNmMmRlZGU7XHJcbiAgICB9XHJcbiAgICAuYmctc3VjY2Vzcy1saWdodCB7XHJcbiAgICAgICAgYmFja2dyb3VuZC1jb2xvcjogI2RmZjBkODtcclxuICAgIH1cclxuICAgIC5iZy13YXJuaW5nLWxpZ2h0IHtcclxuICAgICAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmNmOGUzO1xyXG4gICAgfVxyXG4gICAgYF07XHJcblxyXG4gIHByaXZhdGUgcmVhZG9ubHkgb3BlbkFsbCA9ICgpID0+IHtcclxuICAgIHRoaXMuZm9yRWFjaE11dGFudENvbXBvbmVudChtdXRhbnRDb21wb25lbnQgPT4gbXV0YW50Q29tcG9uZW50LmV4cGFuZCA9IHRydWUpO1xyXG4gIH1cclxuICBwcml2YXRlIHJlYWRvbmx5IGNvbGxhcHNlQWxsID0gKCkgPT4ge1xyXG4gICAgdGhpcy5mb3JFYWNoTXV0YW50Q29tcG9uZW50KG11dGFudENvbXBvbmVudCA9PiBtdXRhbnRDb21wb25lbnQuZXhwYW5kID0gZmFsc2UpO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSBmb3JFYWNoTXV0YW50Q29tcG9uZW50KGFjdGlvbjogKG11dGFudDogTXV0YXRpb25UZXN0UmVwb3J0TXV0YW50Q29tcG9uZW50KSA9PiB2b2lkLCBob3N0ID0gdGhpcy5yb290KSB7XHJcbiAgICBmb3IgKGNvbnN0IG11dGFudENvbXBvbmVudCBvZiBob3N0LnF1ZXJ5U2VsZWN0b3JBbGwoJ211dGF0aW9uLXRlc3QtcmVwb3J0LW11dGFudCcpKSB7XHJcbiAgICAgIGlmIChtdXRhbnRDb21wb25lbnQgaW5zdGFuY2VvZiBNdXRhdGlvblRlc3RSZXBvcnRNdXRhbnRDb21wb25lbnQpIHtcclxuICAgICAgICBhY3Rpb24obXV0YW50Q29tcG9uZW50KTtcclxuICAgICAgfVxyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSByZWFkb25seSBmaWx0ZXJzQ2hhbmdlZCA9IChldmVudDogQ3VzdG9tRXZlbnQ8TXV0YW50RmlsdGVyW10+KSA9PiB7XHJcbiAgICB0aGlzLmVuYWJsZWRNdXRhbnRTdGF0ZXMgPSBldmVudC5kZXRhaWxcclxuICAgICAgLmZpbHRlcihtdXRhbnRGaWx0ZXIgPT4gbXV0YW50RmlsdGVyLmVuYWJsZWQpXHJcbiAgICAgIC5tYXAobXV0YW50RmlsdGVyID0+IG11dGFudEZpbHRlci5zdGF0dXMpO1xyXG4gICAgdGhpcy51cGRhdGVTaG93bk11dGFudHMoKTtcclxuICB9XHJcblxyXG4gIHByaXZhdGUgdXBkYXRlU2hvd25NdXRhbnRzKCkge1xyXG4gICAgdGhpcy5mb3JFYWNoTXV0YW50Q29tcG9uZW50KG11dGFudENvbXBvbmVudCA9PiB7XHJcbiAgICAgIG11dGFudENvbXBvbmVudC5zaG93ID0gdGhpcy5lbmFibGVkTXV0YW50U3RhdGVzLnNvbWUoc3RhdGUgPT4gbXV0YW50Q29tcG9uZW50Lm11dGFudCAhPT0gdW5kZWZpbmVkICYmIG11dGFudENvbXBvbmVudC5tdXRhbnQuc3RhdHVzID09PSBzdGF0ZSk7XHJcbiAgICB9KTtcclxuICB9XHJcblxyXG4gIHB1YmxpYyByZW5kZXIoKSB7XHJcbiAgICByZXR1cm4gaHRtbGBcclxuICAgICAgICA8ZGl2IGNsYXNzPVwicm93XCI+XHJcbiAgICAgICAgICA8ZGl2IGNsYXNzPVwidG90YWxzIGNvbC1zbS0xMVwiPlxyXG4gICAgICAgICAgICA8bXV0YXRpb24tdGVzdC1yZXBvcnQtdG90YWxzIC5tb2RlbD1cIiR7UmVzdWx0VGFibGUuZm9yRmlsZSh0aGlzLm5hbWUsIHRoaXMubW9kZWwsIHRoaXMudGhyZXNob2xkcyl9XCI+PC9tdXRhdGlvbi10ZXN0LXJlcG9ydC10b3RhbHM+XHJcbiAgICAgICAgICA8L2Rpdj5cclxuICAgICAgICA8L2Rpdj5cclxuICAgICAgICA8ZGl2IGNsYXNzPVwicm93XCI+XHJcbiAgICAgICAgICA8ZGl2IGNsYXNzPVwiY29sLW1kLTEyXCI+XHJcbiAgICAgICAgICAgIDxtdXRhdGlvbi10ZXN0LXJlcG9ydC1maWxlLWxlZ2VuZCBAZmlsdGVycy1jaGFuZ2VkPVwiJHt0aGlzLmZpbHRlcnNDaGFuZ2VkfVwiIEBvcGVuLWFsbD1cIiR7dGhpcy5vcGVuQWxsfVwiXHJcbiAgICAgICAgICAgICAgQGNvbGxhcHNlLWFsbD1cIiR7dGhpcy5jb2xsYXBzZUFsbH1cIiAubXV0YW50cz1cIiR7dGhpcy5tb2RlbC5tdXRhbnRzfVwiPjwvbXV0YXRpb24tdGVzdC1yZXBvcnQtZmlsZS1sZWdlbmQ+XHJcbiAgICAgICAgICAgIDxwcmU+PGNvZGUgY2xhc3M9XCJsYW5nLSR7dGhpcy5tb2RlbC5sYW5ndWFnZX0gaGxqc1wiPiR7dW5zYWZlSFRNTCh0aGlzLnJlbmRlckNvZGUoKSl9PC9jb2RlPjwvcHJlPlxyXG4gICAgICAgICAgPC9kaXY+XHJcbiAgICAgICAgPC9kaXY+XHJcbiAgICAgICAgYDtcclxuICB9XHJcblxyXG4gIHB1YmxpYyBmaXJzdFVwZGF0ZWQoX2NoYW5nZWRQcm9wZXJ0aWVzOiBQcm9wZXJ0eVZhbHVlcykge1xyXG4gICAgc3VwZXIuZmlyc3RVcGRhdGVkKF9jaGFuZ2VkUHJvcGVydGllcyk7XHJcbiAgICBjb25zdCBjb2RlID0gdGhpcy5yb290LnF1ZXJ5U2VsZWN0b3IoJ2NvZGUnKTtcclxuICAgIGlmIChjb2RlKSB7XHJcbiAgICAgIGhsanMuaGlnaGxpZ2h0QmxvY2soY29kZSk7XHJcbiAgICAgIHRoaXMuZm9yRWFjaE11dGFudENvbXBvbmVudChtdXRhbnRDb21wb25lbnQgPT4ge1xyXG4gICAgICAgIG11dGFudENvbXBvbmVudC5tdXRhbnQgPSB0aGlzLm1vZGVsLm11dGFudHMuZmluZChtdXRhbnQgPT4gbXV0YW50LmlkLnRvU3RyaW5nKCkgPT09IG11dGFudENvbXBvbmVudC5nZXRBdHRyaWJ1dGUoJ211dGFudC1pZCcpKTtcclxuICAgICAgfSwgY29kZSk7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIGdldCByb290KCk6IFBhcmVudE5vZGUge1xyXG4gICAgcmV0dXJuIHRoaXMuc2hhZG93Um9vdCB8fCB0aGlzO1xyXG4gIH1cclxuXHJcbiAgLyoqXHJcbiAgICogV2Fsa3Mgb3ZlciB0aGUgY29kZSBpbiB0aGlzLm1vZGVsLnNvdXJjZSBhbmQgYWRkcyB0aGVcclxuICAgKiBgPG11dGF0aW9uLXRlc3QtcmVwb3J0LW11dGFudD5gIGVsZW1lbnRzLlxyXG4gICAqIEl0IGFsc28gYWRkcyB0aGUgYmFja2dyb3VuZCBjb2xvciB1c2luZ1xyXG4gICAqIGA8c3BhbiBjbGFzcz1cImJnLWRhbmdlci1saWdodFwiPmAgYW5kIGZyaWVuZHMuXHJcbiAgICovXHJcbiAgcHJpdmF0ZSByZW5kZXJDb2RlKCk6IHN0cmluZyB7XHJcbiAgICBjb25zdCBiYWNrZ3JvdW5kU3RhdGUgPSBuZXcgQmFja2dyb3VuZENvbG9yQ2FsY3VsYXRvcigpO1xyXG4gICAgY29uc3Qgc3RhcnRlZE11dGFudHM6IE11dGFudFJlc3VsdFtdID0gW107XHJcbiAgICBjb25zdCB3YWxrZXIgPSAoY2hhcjogc3RyaW5nLCBwb3M6IFBvc2l0aW9uKTogc3RyaW5nID0+IHtcclxuICAgICAgY29uc3QgY3VycmVudE11dGFudHMgPSB0aGlzLm1vZGVsLm11dGFudHMuZmlsdGVyKG0gPT4gZXEobS5sb2NhdGlvbi5zdGFydCwgcG9zKSk7XHJcbiAgICAgIGNvbnN0IG11dGFudHNFbmRpbmcgPSBzdGFydGVkTXV0YW50cy5maWx0ZXIobSA9PiBndGUocG9zLCBtLmxvY2F0aW9uLmVuZCkpO1xyXG4gICAgICBtdXRhbnRzRW5kaW5nLmZvckVhY2gobXV0YW50ID0+IHN0YXJ0ZWRNdXRhbnRzLnNwbGljZShzdGFydGVkTXV0YW50cy5pbmRleE9mKG11dGFudCksIDEpKTtcclxuICAgICAgc3RhcnRlZE11dGFudHMucHVzaCguLi5jdXJyZW50TXV0YW50cyk7XHJcbiAgICAgIGNvbnN0IGJ1aWxkZXI6IHN0cmluZ1tdID0gW107XHJcbiAgICAgIGlmIChjdXJyZW50TXV0YW50cy5sZW5ndGggfHwgbXV0YW50c0VuZGluZy5sZW5ndGgpIHtcclxuICAgICAgICBjdXJyZW50TXV0YW50cy5mb3JFYWNoKGJhY2tncm91bmRTdGF0ZS5tYXJrTXV0YW50U3RhcnQpO1xyXG4gICAgICAgIG11dGFudHNFbmRpbmcuZm9yRWFjaChiYWNrZ3JvdW5kU3RhdGUubWFya011dGFudEVuZCk7XHJcblxyXG4gICAgICAgIC8vIEVuZCBwcmV2aW91cyBjb2xvciBzcGFuXHJcbiAgICAgICAgYnVpbGRlci5wdXNoKCc8L3NwYW4+Jyk7XHJcblxyXG4gICAgICAgIC8vIEVuZCBtdXRhbnRzXHJcbiAgICAgICAgbXV0YW50c0VuZGluZy5mb3JFYWNoKCgpID0+IGJ1aWxkZXIucHVzaCgnPC9tdXRhdGlvbi10ZXN0LXJlcG9ydC1tdXRhbnQ+JykpO1xyXG5cclxuICAgICAgICAvLyBTdGFydCBtdXRhbnRzXHJcbiAgICAgICAgY3VycmVudE11dGFudHMuZm9yRWFjaChtdXRhbnQgPT4gYnVpbGRlci5wdXNoKGA8bXV0YXRpb24tdGVzdC1yZXBvcnQtbXV0YW50IG11dGFudC1pZD1cIiR7bXV0YW50LmlkfVwiPmApKTtcclxuXHJcbiAgICAgICAgLy8gU3RhcnQgbmV3IGNvbG9yIHNwYW5cclxuICAgICAgICBidWlsZGVyLnB1c2goYDxzcGFuIGNsYXNzPVwiYmctJHtiYWNrZ3JvdW5kU3RhdGUuZGV0ZXJtaW5lQmFja2dyb3VuZCgpfVwiPmApO1xyXG4gICAgICB9XHJcblxyXG4gICAgICAvLyBBcHBlbmQgdGhlIGNvZGUgY2hhcmFjdGVyXHJcbiAgICAgIGJ1aWxkZXIucHVzaChlc2NhcGVIdG1sKGNoYXIpKTtcclxuICAgICAgcmV0dXJuIGJ1aWxkZXIuam9pbignJyk7XHJcbiAgICB9O1xyXG4gICAgcmV0dXJuIGA8c3Bhbj4ke3dhbGtTdHJpbmcodGhpcy5tb2RlbC5zb3VyY2UsIHdhbGtlcil9PC9zcGFuPmA7XHJcbiAgfVxyXG59XHJcblxyXG4vKipcclxuICogV2Fsa3MgYSBzdHJpbmcuIEV4ZWN1dGVzIGEgZnVuY3Rpb24gb24gZWFjaCBjaGFyYWN0ZXIgb2YgdGhlIHN0cmluZyAoZXhjZXB0IGZvciBuZXcgbGluZXMgYW5kIGNhcnJpYWdlIHJldHVybnMpXHJcbiAqIEBwYXJhbSBzb3VyY2UgdGhlIHN0cmluZyB0byB3YWxrXHJcbiAqIEBwYXJhbSBmbiBUaGUgZnVuY3Rpb24gdG8gZXhlY3V0ZSBvbiBlYWNoIGNoYXJhY3RlciBvZiB0aGUgc3RyaW5nXHJcbiAqL1xyXG5mdW5jdGlvbiB3YWxrU3RyaW5nKHNvdXJjZTogc3RyaW5nLCBmbjogKGNoYXI6IHN0cmluZywgcG9zaXRpb246IFBvc2l0aW9uKSA9PiBzdHJpbmcpOiBzdHJpbmcge1xyXG4gIGxldCBjb2x1bW4gPSBDT0xVTU5fU1RBUlRfSU5ERVg7XHJcbiAgbGV0IGxpbmUgPSBMSU5FX1NUQVJUX0lOREVYO1xyXG4gIGNvbnN0IGJ1aWxkZXI6IHN0cmluZ1tdID0gW107XHJcblxyXG4gIGZvciAoY29uc3QgY3VycmVudENoYXIgb2Ygc291cmNlKSB7XHJcbiAgICBpZiAoY29sdW1uID09PSBDT0xVTU5fU1RBUlRfSU5ERVggJiYgY3VycmVudENoYXIgPT09IENBUlJJQUdFX1JFVFVSTikge1xyXG4gICAgICBjb250aW51ZTtcclxuICAgIH1cclxuICAgIGlmIChjdXJyZW50Q2hhciA9PT0gTkVXX0xJTkUpIHtcclxuICAgICAgbGluZSsrO1xyXG4gICAgICBjb2x1bW4gPSBDT0xVTU5fU1RBUlRfSU5ERVg7XHJcbiAgICAgIGJ1aWxkZXIucHVzaChORVdfTElORSk7XHJcbiAgICAgIGNvbnRpbnVlO1xyXG4gICAgfVxyXG4gICAgYnVpbGRlci5wdXNoKGZuKGN1cnJlbnRDaGFyLCB7IGxpbmUsIGNvbHVtbjogY29sdW1uKysgfSkpO1xyXG4gIH1cclxuICByZXR1cm4gYnVpbGRlci5qb2luKCcnKTtcclxufVxyXG5cclxuLyoqXHJcbiAqIENsYXNzIHRvIGtlZXAgdHJhY2sgb2YgdGhlIHN0YXRlcyBvZiB0aGVcclxuICogbXV0YW50cyB0aGF0IGFyZSBhY3RpdmUgYXQgdGhlIGN1cnNvciB3aGlsZSB3YWxraW5nIHRoZSBjb2RlLlxyXG4gKi9cclxuY2xhc3MgQmFja2dyb3VuZENvbG9yQ2FsY3VsYXRvciB7XHJcbiAgcHJpdmF0ZSBraWxsZWQgPSAwO1xyXG4gIHByaXZhdGUgbm9Db3ZlcmFnZSA9IDA7XHJcbiAgcHJpdmF0ZSBzdXJ2aXZlZCA9IDA7XHJcbiAgcHJpdmF0ZSB0aW1lb3V0ID0gMDtcclxuXHJcbiAgcHVibGljIHJlYWRvbmx5IG1hcmtNdXRhbnRTdGFydCA9IChtdXRhbnQ6IE11dGFudFJlc3VsdCkgPT4ge1xyXG4gICAgdGhpcy5jb3VudE11dGFudCgxLCBtdXRhbnQuc3RhdHVzKTtcclxuICB9XHJcblxyXG4gIHB1YmxpYyByZWFkb25seSBtYXJrTXV0YW50RW5kID0gKG11dGFudDogTXV0YW50UmVzdWx0KSA9PiB7XHJcbiAgICB0aGlzLmNvdW50TXV0YW50KC0xLCBtdXRhbnQuc3RhdHVzKTtcclxuICB9XHJcblxyXG4gIHByaXZhdGUgY291bnRNdXRhbnQodmFsdWVUb0FkZDogbnVtYmVyLCBzdGF0dXM6IE11dGFudFN0YXR1cykge1xyXG4gICAgc3dpdGNoIChzdGF0dXMpIHtcclxuICAgICAgY2FzZSBNdXRhbnRTdGF0dXMuS2lsbGVkOlxyXG4gICAgICAgIHRoaXMua2lsbGVkICs9IHZhbHVlVG9BZGQ7XHJcbiAgICAgICAgYnJlYWs7XHJcbiAgICAgIGNhc2UgTXV0YW50U3RhdHVzLlN1cnZpdmVkOlxyXG4gICAgICAgIHRoaXMuc3Vydml2ZWQgKz0gdmFsdWVUb0FkZDtcclxuICAgICAgICBicmVhaztcclxuICAgICAgY2FzZSBNdXRhbnRTdGF0dXMuVGltZW91dDpcclxuICAgICAgICB0aGlzLnRpbWVvdXQgKz0gdmFsdWVUb0FkZDtcclxuICAgICAgICBicmVhaztcclxuICAgICAgY2FzZSBNdXRhbnRTdGF0dXMuTm9Db3ZlcmFnZTpcclxuICAgICAgICB0aGlzLm5vQ292ZXJhZ2UgKz0gdmFsdWVUb0FkZDtcclxuICAgICAgICBicmVhaztcclxuICAgIH1cclxuICB9XHJcblxyXG4gIHB1YmxpYyBkZXRlcm1pbmVCYWNrZ3JvdW5kID0gKCkgPT4ge1xyXG4gICAgaWYgKHRoaXMuc3Vydml2ZWQgPiAwKSB7XHJcbiAgICAgIHJldHVybiBnZXRDb250ZXh0Q2xhc3NGb3JTdGF0dXMoTXV0YW50U3RhdHVzLlN1cnZpdmVkKSArICctbGlnaHQnO1xyXG4gICAgfSBlbHNlIGlmICh0aGlzLm5vQ292ZXJhZ2UgPiAwKSB7XHJcbiAgICAgIHJldHVybiBnZXRDb250ZXh0Q2xhc3NGb3JTdGF0dXMoTXV0YW50U3RhdHVzLk5vQ292ZXJhZ2UpICsgJy1saWdodCc7XHJcbiAgICB9IGVsc2UgaWYgKHRoaXMudGltZW91dCA+IDApIHtcclxuICAgICAgcmV0dXJuIGdldENvbnRleHRDbGFzc0ZvclN0YXR1cyhNdXRhbnRTdGF0dXMuVGltZW91dCkgKyAnLWxpZ2h0JztcclxuICAgIH0gZWxzZSBpZiAodGhpcy5raWxsZWQgPiAwKSB7XHJcbiAgICAgIHJldHVybiBnZXRDb250ZXh0Q2xhc3NGb3JTdGF0dXMoTXV0YW50U3RhdHVzLktpbGxlZCkgKyAnLWxpZ2h0JztcclxuICAgIH1cclxuICAgIHJldHVybiBudWxsO1xyXG4gIH1cclxufVxyXG5cclxuZnVuY3Rpb24gZ3RlKGE6IFBvc2l0aW9uLCBiOiBQb3NpdGlvbikge1xyXG4gIHJldHVybiBhLmxpbmUgPiBiLmxpbmUgfHwgKGEubGluZSA9PT0gYi5saW5lICYmIGEuY29sdW1uID49IGIuY29sdW1uKTtcclxufVxyXG5cclxuZnVuY3Rpb24gZXEoYTogUG9zaXRpb24sIGI6IFBvc2l0aW9uKSB7XHJcbiAgcmV0dXJuIGEubGluZSA9PT0gYi5saW5lICYmIGEuY29sdW1uID09PSBiLmNvbHVtbjtcclxufVxyXG4iLCJpbXBvcnQgeyBjdXN0b21FbGVtZW50LCBMaXRFbGVtZW50LCBwcm9wZXJ0eSwgaHRtbCwgY3NzIH0gZnJvbSAnbGl0LWVsZW1lbnQnO1xyXG5pbXBvcnQgeyBNdXRhbnRSZXN1bHQgfSBmcm9tICcuLi8uLi9hcGknO1xyXG5pbXBvcnQgeyBnZXRDb250ZXh0Q2xhc3NGb3JTdGF0dXMgfSBmcm9tICcuLi9oZWxwZXJzJztcclxuaW1wb3J0IHsgYm9vdHN0cmFwIH0gZnJvbSAnLi4vc3R5bGUnO1xyXG5cclxuQGN1c3RvbUVsZW1lbnQoJ211dGF0aW9uLXRlc3QtcmVwb3J0LW11dGFudCcpXHJcbmV4cG9ydCBjbGFzcyBNdXRhdGlvblRlc3RSZXBvcnRNdXRhbnRDb21wb25lbnQgZXh0ZW5kcyBMaXRFbGVtZW50IHtcclxuXHJcbiAgQHByb3BlcnR5KClcclxuICBwdWJsaWMgbXV0YW50OiBNdXRhbnRSZXN1bHQgfCB1bmRlZmluZWQ7XHJcblxyXG4gIEBwcm9wZXJ0eSgpXHJcbiAgcHVibGljIHNob3c6IGJvb2xlYW4gPSB0cnVlO1xyXG5cclxuICBAcHJvcGVydHkoKVxyXG4gIHB1YmxpYyBleHBhbmQ6IGJvb2xlYW4gPSBmYWxzZTtcclxuXHJcbiAgcHVibGljIHN0YXRpYyBzdHlsZXMgPSBbXHJcbiAgICBib290c3RyYXAsXHJcbiAgICBjc3NgXHJcbiAgICAuYmFkZ2Uge1xyXG4gICAgICBjdXJzb3I6IHBvaW50ZXI7XHJcbiAgICB9XHJcbiAgICAuZGlzYWJsZWQtY29kZSB7XHJcbiAgICAgIHRleHQtZGVjb3JhdGlvbjogbGluZS10aHJvdWdoO1xyXG4gICAgfVxyXG4gICAgOjpzbG90dGVkKC5obGpzLXN0cmluZykge1xyXG4gICAgICBjb2xvcjogI2ZmZjtcclxuICAgIH1cclxuICBgXTtcclxuXHJcbiAgcHVibGljIHJlbmRlcigpIHtcclxuICAgIC8vIFRoaXMgcGFydCBpcyBuZXdsaW5lIHNpZ25pZmljYW50LCBhcyBpdCBpcyByZW5kZXJlZCBpbiBhIDxjb2RlPiBibG9jay5cclxuICAgIC8vIE5vIHVubmVjZXNzYXJ5IG5ldyBsaW5lc1xyXG4gICAgcmV0dXJuIGh0bWxgJHt0aGlzLnJlbmRlckJ1dHRvbigpfSR7dGhpcy5yZW5kZXJDb2RlKCl9YDtcclxuICB9XHJcblxyXG4gIHByaXZhdGUgcmVuZGVyQnV0dG9uKCkge1xyXG4gICAgaWYgKHRoaXMuc2hvdyAmJiB0aGlzLm11dGFudCkge1xyXG4gICAgICByZXR1cm4gaHRtbGA8c3BhbiBjbGFzcz1cImJhZGdlIGJhZGdlLSR7dGhpcy5leHBhbmQgPyAnaW5mbycgOiBnZXRDb250ZXh0Q2xhc3NGb3JTdGF0dXModGhpcy5tdXRhbnQuc3RhdHVzKX1cIiBAY2xpY2s9XCIkeygpID0+IHRoaXMuZXhwYW5kID0gIXRoaXMuZXhwYW5kfVwiXHJcbiAgdGl0bGU9XCIke3RoaXMubXV0YW50Lm11dGF0b3JOYW1lfVwiPiR7dGhpcy5tdXRhbnQuaWR9PC9idXR0b24+YDtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgIHJldHVybiB1bmRlZmluZWQ7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlckNvZGUoKSB7XHJcbiAgICByZXR1cm4gaHRtbGAke3RoaXMucmVuZGVyUmVwbGFjZW1lbnQoKX0ke3RoaXMucmVuZGVyQWN0dWFsKCl9YDtcclxuICB9XHJcblxyXG4gIHByaXZhdGUgcmVuZGVyQWN0dWFsKCkge1xyXG4gICAgY29uc3QgYWN0dWFsQ29kZVNsb3QgPSBodG1sYDxzbG90Pjwvc2xvdD5gO1xyXG4gICAgcmV0dXJuIGh0bWxgPHNwYW4gY2xhc3M9XCIke3RoaXMuZXhwYW5kICYmIHRoaXMuc2hvdyA/ICdkaXNhYmxlZC1jb2RlJyA6ICcnfVwiPiR7YWN0dWFsQ29kZVNsb3R9PC9zcGFuPmA7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlclJlcGxhY2VtZW50KCkge1xyXG4gICAgaWYgKHRoaXMubXV0YW50KSB7XHJcbiAgICAgIHJldHVybiBodG1sYDxzcGFuIGNsYXNzPVwiYmFkZ2UgYmFkZ2UtaW5mb1wiID9oaWRkZW49XCIkeyF0aGlzLmV4cGFuZCB8fCAhdGhpcy5zaG93fVwiPiR7dGhpcy5tdXRhbnQucmVwbGFjZW1lbnR9PC9zcGFuPmA7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICByZXR1cm4gdW5kZWZpbmVkO1xyXG4gICAgfVxyXG4gIH1cclxufVxyXG4iLCJpbXBvcnQgeyBMaXRFbGVtZW50LCBodG1sLCBwcm9wZXJ0eSwgY3VzdG9tRWxlbWVudCB9IGZyb20gJ2xpdC1lbGVtZW50JztcclxuaW1wb3J0IHsgTXV0YXRpb25UZXN0UmVzdWx0IH0gZnJvbSAnLi4vLi4vYXBpJztcclxuaW1wb3J0IHsgYm9vdHN0cmFwIH0gZnJvbSAnLi4vc3R5bGUnO1xyXG5pbXBvcnQgeyBSZXN1bHRUYWJsZSwgVGFibGVSb3cgfSBmcm9tICcuLi9tb2RlbC9SZXN1bHRUYWJsZSc7XHJcbmltcG9ydCB7IFJPT1RfTkFNRSwgZmxhdE1hcCB9IGZyb20gJy4uL2hlbHBlcnMnO1xyXG5cclxuQGN1c3RvbUVsZW1lbnQoJ211dGF0aW9uLXRlc3QtcmVwb3J0LXJlc3VsdCcpXHJcbmV4cG9ydCBjbGFzcyBNdXRhdGlvblRlc3RSZXBvcnRSZXN1bHRDb21wb25lbnQgZXh0ZW5kcyBMaXRFbGVtZW50IHtcclxuXHJcbiAgQHByb3BlcnR5KClcclxuICBwcml2YXRlIHJlYWRvbmx5IG1vZGVsITogTXV0YXRpb25UZXN0UmVzdWx0O1xyXG5cclxuICBwdWJsaWMgc3RhdGljIHN0eWxlcyA9IFtib290c3RyYXBdO1xyXG5cclxuICBwdWJsaWMgcmVuZGVyKCkge1xyXG4gICAgY29uc3Qgcm93cyA9IFtcclxuICAgICAgbmV3IFRhYmxlUm93KFJPT1RfTkFNRSwgZmxhdE1hcChPYmplY3Qua2V5cyh0aGlzLm1vZGVsLmZpbGVzKSwga2V5ID0+IHRoaXMubW9kZWwuZmlsZXNba2V5XS5tdXRhbnRzKSwgZmFsc2UpLFxyXG4gICAgICAuLi5PYmplY3Qua2V5cyh0aGlzLm1vZGVsLmZpbGVzKS5tYXAoZmlsZU5hbWUgPT4gbmV3IFRhYmxlUm93KGZpbGVOYW1lLCB0aGlzLm1vZGVsLmZpbGVzW2ZpbGVOYW1lXS5tdXRhbnRzLCB0cnVlKSlcclxuICAgIF07XHJcbiAgICByZXR1cm4gaHRtbGBcclxuICAgIDxkaXYgY2xhc3M9J3Jvdyc+XHJcbiAgICAgIDxkaXYgY2xhc3M9J3RvdGFscyBjb2wtc20tMTEnPlxyXG4gICAgICAgIDxtdXRhdGlvbi10ZXN0LXJlcG9ydC10b3RhbHMgLm1vZGVsPVwiJHtuZXcgUmVzdWx0VGFibGUocm93cywgdGhpcy5tb2RlbC50aHJlc2hvbGRzKX1cIj48L211dGF0aW9uLXRlc3QtcmVwb3J0LXRvdGFscz5cclxuICAgICAgPC9kaXY+XHJcbiAgICA8L2Rpdj5cclxuICAgIGA7XHJcbiAgfVxyXG59XHJcbiIsIlxyXG5pbXBvcnQgeyBMaXRFbGVtZW50LCBjdXN0b21FbGVtZW50IH0gZnJvbSAnbGl0LWVsZW1lbnQnO1xyXG5cclxuQGN1c3RvbUVsZW1lbnQoJ211dGF0aW9uLXRlc3QtcmVwb3J0LXJvdXRlcicpXHJcbmV4cG9ydCBjbGFzcyBNdXRhdGlvblRlc3RSZXBvcnRSb3V0ZXJDb21wb25lbnQgZXh0ZW5kcyBMaXRFbGVtZW50IHtcclxuXHJcbiAgcHVibGljIGNvbm5lY3RlZENhbGxiYWNrKCkge1xyXG4gICAgc3VwZXIuY29ubmVjdGVkQ2FsbGJhY2soKTtcclxuICAgIHdpbmRvdy5hZGRFdmVudExpc3RlbmVyKCdoYXNoY2hhbmdlJywgdGhpcy51cGRhdGVQYXRoKTtcclxuICAgIHRoaXMudXBkYXRlUGF0aCgpO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSByZWFkb25seSB1cGRhdGVQYXRoID0gKCkgPT4ge1xyXG4gICAgY29uc3QgcGF0aCA9IHdpbmRvdy5sb2NhdGlvbi5oYXNoLnN1YnN0cigxKTtcclxuICAgIHRoaXMuZGlzcGF0Y2hFdmVudChuZXcgQ3VzdG9tRXZlbnQoJ3BhdGgtY2hhbmdlZCcsIHsgZGV0YWlsOiBwYXRoIH0pKTtcclxuICB9XHJcbn1cclxuIiwiXHJcbmltcG9ydCB7IExpdEVsZW1lbnQsIGN1c3RvbUVsZW1lbnQsIHByb3BlcnR5IH0gZnJvbSAnbGl0LWVsZW1lbnQnO1xyXG5cclxuQGN1c3RvbUVsZW1lbnQoJ211dGF0aW9uLXRlc3QtcmVwb3J0LXRpdGxlJylcclxuZXhwb3J0IGNsYXNzIE11dGF0aW9uVGVzdFJlcG9ydFJvdXRlckNvbXBvbmVudCBleHRlbmRzIExpdEVsZW1lbnQge1xyXG4gIEBwcm9wZXJ0eSgpXHJcbiAgcHVibGljIHNldCB0aXRsZSh2YWx1ZTogc3RyaW5nKSB7XHJcbiAgICBkb2N1bWVudC50aXRsZSA9IHZhbHVlO1xyXG4gIH1cclxufVxyXG4iLCJpbXBvcnQgeyBMaXRFbGVtZW50LCBodG1sLCBwcm9wZXJ0eSwgY3VzdG9tRWxlbWVudCwgY3NzIH0gZnJvbSAnbGl0LWVsZW1lbnQnO1xyXG5pbXBvcnQgeyBib290c3RyYXAgfSBmcm9tICcuLi9zdHlsZSc7XHJcbmltcG9ydCB7IFJlc3VsdFRhYmxlLCBUYWJsZVJvdyB9IGZyb20gJy4uL21vZGVsL1Jlc3VsdFRhYmxlJztcclxuXHJcbkBjdXN0b21FbGVtZW50KCdtdXRhdGlvbi10ZXN0LXJlcG9ydC10b3RhbHMnKVxyXG5leHBvcnQgY2xhc3MgTXV0YXRpb25UZXN0UmVwb3J0VG90YWxzQ29tcG9uZW50IGV4dGVuZHMgTGl0RWxlbWVudCB7XHJcblxyXG4gIEBwcm9wZXJ0eSgpXHJcbiAgcHVibGljIG1vZGVsITogUmVzdWx0VGFibGU7XHJcblxyXG4gIHB1YmxpYyBzdGF0aWMgc3R5bGVzID0gW2Jvb3RzdHJhcCxcclxuICAgIGNzc2BcclxuICAgIHRoLnJvdGF0ZSB7XHJcbiAgICAgIC8qIFNvbWV0aGluZyB5b3UgY2FuIGNvdW50IG9uICovXHJcbiAgICAgIGhlaWdodDogNTBweDtcclxuICAgICAgd2hpdGUtc3BhY2U6IG5vd3JhcDtcclxuICAgICAgcGFkZGluZy1ib3R0b206IDEwcHg7XHJcbiAgICB9XHJcblxyXG4gICAgdGgucm90YXRlID4gZGl2IHtcclxuICAgICAgdHJhbnNmb3JtOlxyXG4gICAgICB0cmFuc2xhdGUoMjdweCwgMHB4KVxyXG4gICAgICByb3RhdGUoMzI1ZGVnKTtcclxuICAgICAgd2lkdGg6IDMwcHg7XHJcbiAgICB9XHJcblxyXG4gICAgLnRhYmxlLW5vLXRvcD50aGVhZD50cj50aCB7XHJcbiAgICAgIGJvcmRlci13aWR0aDogMDtcclxuICAgIH1cclxuXHJcbiAgICAudGFibGUtbm8tdG9wIHtcclxuICAgICAgYm9yZGVyLXdpZHRoOiAwO1xyXG4gICAgfVxyXG4gIGBdO1xyXG5cclxuICBwdWJsaWMgcmVuZGVyKCkge1xyXG4gICAgcmV0dXJuIGh0bWxgXHJcbiAgICAgICAgICA8dGFibGUgY2xhc3M9XCJ0YWJsZSB0YWJsZS1zbSB0YWJsZS1ob3ZlciB0YWJsZS1ib3JkZXJlZCB0YWJsZS1uby10b3BcIj5cclxuICAgICAgICAgICAgJHt0aGlzLnJlbmRlckhlYWQoKX1cclxuICAgICAgICAgICAgJHt0aGlzLnJlbmRlckJvZHkoKX1cclxuICAgICAgICAgIDwvdGFibGU+XHJcbiAgICAgIGA7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIHJlbmRlckhlYWQoKSB7XHJcbiAgICByZXR1cm4gaHRtbGA8dGhlYWQ+XHJcbiAgPHRyPlxyXG4gICAgPHRoIHN0eWxlPVwid2lkdGg6IDIwJVwiPlxyXG4gICAgICA8ZGl2PjxzcGFuPkZpbGUgLyBEaXJlY3Rvcnk8L3NwYW4+PC9kaXY+XHJcbiAgICA8L3RoPlxyXG4gICAgPHRoIGNvbHNwYW49XCIyXCI+XHJcbiAgICAgIDxkaXY+PHNwYW4+TXV0YXRpb24gc2NvcmU8L3NwYW4+PC9kaXY+XHJcbiAgICA8L3RoPlxyXG4gICAgPHRoIGNsYXNzPVwicm90YXRlIHRleHQtY2VudGVyXCIgc3R5bGU9XCJ3aWR0aDogNTBweFwiPlxyXG4gICAgICA8ZGl2PjxzcGFuPiMgS2lsbGVkPC9zcGFuPjwvZGl2PlxyXG4gICAgPC90aD5cclxuICAgIDx0aCBjbGFzcz1cInJvdGF0ZSB0ZXh0LWNlbnRlclwiIHN0eWxlPVwid2lkdGg6IDUwcHhcIj5cclxuICAgICAgPGRpdj48c3Bhbj4jIFN1cnZpdmVkPC9zcGFuPjwvZGl2PlxyXG4gICAgPC90aD5cclxuICAgIDx0aCBjbGFzcz1cInJvdGF0ZSB0ZXh0LWNlbnRlclwiIHN0eWxlPVwid2lkdGg6IDUwcHhcIj5cclxuICAgICAgPGRpdj48c3Bhbj4jIFRpbWVvdXQ8L3NwYW4+PC9kaXY+XHJcbiAgICA8L3RoPlxyXG4gICAgPHRoIGNsYXNzPVwicm90YXRlIHRleHQtY2VudGVyXCIgc3R5bGU9XCJ3aWR0aDogNTBweFwiPlxyXG4gICAgICA8ZGl2PjxzcGFuPiMgTm8gY292ZXJhZ2U8L3NwYW4+PC9kaXY+XHJcbiAgICA8L3RoPlxyXG4gICAgPHRoIGNsYXNzPVwicm90YXRlIHRleHQtY2VudGVyXCIgc3R5bGU9XCJ3aWR0aDogNTBweFwiPlxyXG4gICAgICA8ZGl2PjxzcGFuPiMgUnVudGltZSBlcnJvcnM8L3NwYW4+PC9kaXY+XHJcbiAgICA8L3RoPlxyXG4gICAgPHRoIGNsYXNzPVwicm90YXRlIHRleHQtY2VudGVyXCIgc3R5bGU9XCJ3aWR0aDogNTBweFwiPlxyXG4gICAgICA8ZGl2PjxzcGFuPiMgVHJhbnNwaWxlIGVycm9yczwvc3Bhbj48L2Rpdj5cclxuICAgIDwvdGg+XHJcbiAgICA8dGggY2xhc3M9XCJyb3RhdGUgcm90YXRlLXdpZHRoLTcwIHRleHQtY2VudGVyXCIgc3R5bGU9XCJ3aWR0aDogNzBweFwiPlxyXG4gICAgICA8ZGl2PjxzcGFuPlRvdGFsIGRldGVjdGVkPC9zcGFuPjwvZGl2PlxyXG4gICAgPC90aD5cclxuICAgIDx0aCBjbGFzcz1cInJvdGF0ZSByb3RhdGUtd2lkdGgtNzAgdGV4dC1jZW50ZXJcIiBzdHlsZT1cIndpZHRoOiA3MHB4XCI+XHJcbiAgICAgIDxkaXY+PHNwYW4+VG90YWwgdW5kZXRlY3RlZDwvc3Bhbj48L2Rpdj5cclxuICAgIDwvdGg+XHJcbiAgICA8dGggY2xhc3M9XCJyb3RhdGUgcm90YXRlLXdpZHRoLTcwIHRleHQtY2VudGVyXCIgc3R5bGU9XCJ3aWR0aDogNzBweFwiPlxyXG4gICAgICA8ZGl2PjxzcGFuPlRvdGFsIG11dGFudHM8L3NwYW4+PC9kaXY+XHJcbiAgICA8L3RoPlxyXG4gIDwvdHI+XHJcbjwvdGhlYWQ+YDtcclxuICB9XHJcblxyXG4gIHByaXZhdGUgcmVuZGVyQm9keSgpIHtcclxuICAgIHJldHVybiBodG1sYFxyXG4gICAgPHRib2R5PlxyXG4gICAgICAke3RoaXMubW9kZWwucm93cy5tYXAodGhpcy5yZW5kZXJSb3cpfVxyXG4gICAgPC90Ym9keT5gO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSByZWFkb25seSByZW5kZXJSb3cgPSAocm93OiBUYWJsZVJvdykgPT4ge1xyXG4gICAgY29uc3QgbXV0YXRpb25TY29yZVJvdW5kZWQgPSByb3cubXV0YXRpb25TY29yZS50b0ZpeGVkKDIpO1xyXG4gICAgY29uc3QgY29sb3JpbmdDbGFzcyA9IHRoaXMuZGV0ZXJtaW5lQ29sb3JpbmdDbGFzcyhyb3cubXV0YXRpb25TY29yZSk7XHJcbiAgICBjb25zdCBzdHlsZSA9IGB3aWR0aDogJHttdXRhdGlvblNjb3JlUm91bmRlZH0lYDtcclxuICAgIHJldHVybiBodG1sYFxyXG4gICAgPHRyPlxyXG4gICAgICA8dGQ+JHtyb3cuc2hvdWxkTGluayA/IGh0bWxgPGEgaHJlZj1cIiR7dGhpcy5saW5rKHJvdy5uYW1lKX1cIj4ke3Jvdy5uYW1lfTwvYT5gIDogaHRtbGA8c3Bhbj4ke3Jvdy5uYW1lfTwvc3Bhbj5gfTwvdGQ+XHJcbiAgICAgIDx0ZD5cclxuICAgICAgICA8ZGl2IGNsYXNzPVwicHJvZ3Jlc3NcIj5cclxuICAgICAgICAgIDxkaXYgY2xhc3M9XCJwcm9ncmVzcy1iYXIgYmctJHtjb2xvcmluZ0NsYXNzfVwiIHJvbGU9XCJwcm9ncmVzc2JhclwiIGFyaWEtdmFsdWVub3c9XCIke211dGF0aW9uU2NvcmVSb3VuZGVkfVwiXHJcbiAgICAgICAgICAgIGFyaWEtdmFsdWVtaW49XCIwXCIgYXJpYS12YWx1ZW1heD1cIjEwMFwiIC5zdHlsZT1cIiR7c3R5bGV9XCI+XHJcbiAgICAgICAgICAgICR7bXV0YXRpb25TY29yZVJvdW5kZWR9JVxyXG4gICAgICAgICAgPC9kaXY+XHJcbiAgICAgICAgPC9kaXY+XHJcbiAgICAgIDwvdGQ+XHJcbiAgICAgIDx0aCBjbGFzcz1cInRleHQtY2VudGVyIHRleHQtJHtjb2xvcmluZ0NsYXNzfVwiPiR7bXV0YXRpb25TY29yZVJvdW5kZWR9PC90aD5cclxuICAgICAgPHRkIGNsYXNzPVwidGV4dC1jZW50ZXJcIj4ke3Jvdy5raWxsZWR9PC90ZD5cclxuICAgICAgPHRkIGNsYXNzPVwidGV4dC1jZW50ZXJcIj4ke3Jvdy5zdXJ2aXZlZH08L3RkPlxyXG4gICAgICA8dGQgY2xhc3M9XCJ0ZXh0LWNlbnRlclwiPiR7cm93LnRpbWVvdXR9PC90ZD5cclxuICAgICAgPHRkIGNsYXNzPVwidGV4dC1jZW50ZXJcIj4ke3Jvdy5ub0NvdmVyYWdlfTwvdGQ+XHJcbiAgICAgIDx0ZCBjbGFzcz1cInRleHQtY2VudGVyXCI+JHtyb3cucnVudGltZUVycm9yc308L3RkPlxyXG4gICAgICA8dGQgY2xhc3M9XCJ0ZXh0LWNlbnRlclwiPiR7cm93LmNvbXBpbGVFcnJvcnN9PC90ZD5cclxuICAgICAgPHRoIGNsYXNzPVwidGV4dC1jZW50ZXJcIj4ke3Jvdy50b3RhbERldGVjdGVkfTwvdGg+XHJcbiAgICAgIDx0aCBjbGFzcz1cInRleHQtY2VudGVyXCI+JHtyb3cudG90YWxVbmRldGVjdGVkfTwvdGg+XHJcbiAgICAgIDx0aCBjbGFzcz1cInRleHQtY2VudGVyXCI+JHtyb3cudG90YWxNdXRhbnRzfTwvdGg+XHJcbiAgICA8L3RyPlxyXG4gICAgYCA7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIGxpbmsodG86IHN0cmluZykge1xyXG4gICAgcmV0dXJuIGAjJHt0b31gO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSBkZXRlcm1pbmVDb2xvcmluZ0NsYXNzKHNjb3JlOiBudW1iZXIpIHtcclxuICAgIGlmIChzY29yZSA8IHRoaXMubW9kZWwudGhyZXNob2xkcy5sb3cpIHtcclxuICAgICAgcmV0dXJuICdkYW5nZXInO1xyXG4gICAgfSBlbHNlIGlmIChzY29yZSA8IHRoaXMubW9kZWwudGhyZXNob2xkcy5oaWdoKSB7XHJcbiAgICAgIHJldHVybiAnd2FybmluZyc7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICByZXR1cm4gJ3N1Y2Nlc3MnO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbn1cclxuIiwiaW1wb3J0IHsgTXV0YXRpb25UZXN0UmVzdWx0IH0gZnJvbSAnLi4vYXBpL011dGF0aW9uVGVzdFJlc3VsdCc7XHJcbmltcG9ydCB7IEZpbGVSZXN1bHQsIE11dGFudFN0YXR1cywgRmlsZVJlc3VsdERpY3Rpb25hcnkgfSBmcm9tICcuLi9hcGknO1xyXG5cclxuZXhwb3J0IGNvbnN0IFJPT1RfTkFNRSA9ICdBbGwgZmlsZXMnO1xyXG5jb25zdCBTRVBBUkFUT1IgPSAnLyc7XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gZ2V0Q29udGV4dENsYXNzRm9yU3RhdHVzKHN0YXR1czogTXV0YW50U3RhdHVzKSB7XHJcbiAgc3dpdGNoIChzdGF0dXMpIHtcclxuICAgIGNhc2UgTXV0YW50U3RhdHVzLktpbGxlZDpcclxuICAgICAgcmV0dXJuICdzdWNjZXNzJztcclxuICAgIGNhc2UgTXV0YW50U3RhdHVzLk5vQ292ZXJhZ2U6XHJcbiAgICBjYXNlIE11dGFudFN0YXR1cy5TdXJ2aXZlZDpcclxuICAgICAgcmV0dXJuICdkYW5nZXInO1xyXG4gICAgY2FzZSBNdXRhbnRTdGF0dXMuVGltZW91dDpcclxuICAgICAgcmV0dXJuICd3YXJuaW5nJztcclxuICAgIGNhc2UgTXV0YW50U3RhdHVzLlJ1bnRpbWVFcnJvcjpcclxuICAgIGNhc2UgTXV0YW50U3RhdHVzLkNvbXBpbGVFcnJvcjpcclxuICAgICAgcmV0dXJuICdzZWNvbmRhcnknO1xyXG4gIH1cclxufVxyXG5cclxuZXhwb3J0IGNvbnN0IENPTFVNTl9TVEFSVF9JTkRFWCA9IDE7XHJcbmV4cG9ydCBjb25zdCBMSU5FX1NUQVJUX0lOREVYID0gMTtcclxuZXhwb3J0IGNvbnN0IE5FV19MSU5FID0gJ1xcbic7XHJcbmV4cG9ydCBjb25zdCBDQVJSSUFHRV9SRVRVUk4gPSAnXFxyJztcclxuZXhwb3J0IGZ1bmN0aW9uIGxpbmVzKGNvbnRlbnQ6IHN0cmluZykge1xyXG4gIHJldHVybiBjb250ZW50LnNwbGl0KE5FV19MSU5FKS5tYXAobGluZSA9PiBsaW5lLmVuZHNXaXRoKENBUlJJQUdFX1JFVFVSTikgPyBsaW5lLnN1YnN0cigwLCBsaW5lLmxlbmd0aCAtIDEpIDogbGluZSk7XHJcbn1cclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBlc2NhcGVIdG1sKHVuc2FmZTogc3RyaW5nKSB7XHJcbiAgcmV0dXJuIHVuc2FmZVxyXG4gICAgLnJlcGxhY2UoLyYvZywgJyZhbXA7JylcclxuICAgIC5yZXBsYWNlKC88L2csICcmbHQ7JylcclxuICAgIC5yZXBsYWNlKC8+L2csICcmZ3Q7JylcclxuICAgIC5yZXBsYWNlKC9cIi9nLCAnJnF1b3Q7JylcclxuICAgIC5yZXBsYWNlKC8nL2csICcmIzAzOTsnKTtcclxufVxyXG5cclxuZXhwb3J0IGZ1bmN0aW9uIGZsYXRNYXA8VCwgUj4oc291cmNlOiBUW10sIGZuOiAoaW5wdXQ6IFQpID0+IFJbXSkge1xyXG4gIGNvbnN0IHJlc3VsdDogUltdID0gW107XHJcbiAgc291cmNlLm1hcChmbikuZm9yRWFjaChpdGVtcyA9PiByZXN1bHQucHVzaCguLi5pdGVtcykpO1xyXG4gIHJldHVybiByZXN1bHQ7XHJcbn1cclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBub3JtYWxpemVGaWxlTmFtZXMoZmlsZXM6IEZpbGVSZXN1bHREaWN0aW9uYXJ5KTogRmlsZVJlc3VsdERpY3Rpb25hcnkge1xyXG4gIGNvbnN0IGZpbGVOYW1lcyA9IE9iamVjdC5rZXlzKGZpbGVzKTtcclxuICBjb25zdCBjb21tb25CYXNlUGF0aCA9IGRldGVybWluZUNvbW1vbkJhc2VQYXRoKGZpbGVOYW1lcyk7XHJcbiAgY29uc3QgcmVzdWx0OiBGaWxlUmVzdWx0RGljdGlvbmFyeSA9IHt9O1xyXG4gIGZpbGVOYW1lcy5mb3JFYWNoKGZpbGVOYW1lID0+IHtcclxuICAgIHJlc3VsdFtub3JtYWxpemUoZmlsZU5hbWUuc3Vic3RyKGNvbW1vbkJhc2VQYXRoLmxlbmd0aCkpXSA9IGZpbGVzW2ZpbGVOYW1lXTtcclxuICB9KTtcclxuICByZXR1cm4gcmVzdWx0O1xyXG59XHJcblxyXG5mdW5jdGlvbiBub3JtYWxpemUoZmlsZU5hbWU6IHN0cmluZykge1xyXG4gIHJldHVybiBmaWxlTmFtZS5zcGxpdCgvXFwvfFxcXFwvKVxyXG4gICAgLmZpbHRlcihwYXRoUGFydCA9PiBwYXRoUGFydClcclxuICAgIC5qb2luKCcvJyk7XHJcbn1cclxuXHJcbmZ1bmN0aW9uIGRldGVybWluZUNvbW1vbkJhc2VQYXRoKGZpbGVOYW1lczogc3RyaW5nW10pIHtcclxuICBjb25zdCBkaXJlY3RvcmllcyA9IGZpbGVOYW1lcy5tYXAoZmlsZU5hbWUgPT4gZmlsZU5hbWUuc3BsaXQoL1xcL3xcXFxcLykuc2xpY2UoMCwgLTEpKTtcclxuXHJcbiAgaWYgKGRpcmVjdG9yaWVzLmxlbmd0aCkge1xyXG4gICAgcmV0dXJuIGRpcmVjdG9yaWVzLnJlZHVjZShmaWx0ZXJEaXJlY3Rvcmllcykuam9pbihTRVBBUkFUT1IpO1xyXG4gIH0gZWxzZSB7XHJcbiAgICByZXR1cm4gJyc7XHJcbiAgfVxyXG59XHJcblxyXG5mdW5jdGlvbiBmaWx0ZXJEaXJlY3RvcmllcyhwcmV2aW91c0RpcmVjdG9yaWVzOiBzdHJpbmdbXSwgY3VycmVudERpcmVjdG9yaWVzOiBzdHJpbmdbXSkge1xyXG4gIGZvciAobGV0IGkgPSAwOyBpIDwgcHJldmlvdXNEaXJlY3Rvcmllcy5sZW5ndGg7IGkrKykge1xyXG4gICAgaWYgKHByZXZpb3VzRGlyZWN0b3JpZXNbaV0gIT09IGN1cnJlbnREaXJlY3Rvcmllc1tpXSkge1xyXG4gICAgICByZXR1cm4gcHJldmlvdXNEaXJlY3Rvcmllcy5zcGxpY2UoMCwgaSk7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICByZXR1cm4gcHJldmlvdXNEaXJlY3RvcmllcztcclxufVxyXG4iLCJpbXBvcnQgJy4vY29tcG9uZW50cy9tdXRhdGlvbi10ZXN0LXJlcG9ydC1hcHAnO1xyXG5pbXBvcnQgJy4vY29tcG9uZW50cy9tdXRhdGlvbi10ZXN0LXJlcG9ydC1maWxlJztcclxuaW1wb3J0ICcuL2NvbXBvbmVudHMvbXV0YXRpb24tdGVzdC1yZXBvcnQtdG90YWxzJztcclxuaW1wb3J0ICcuL2NvbXBvbmVudHMvbXV0YXRpb24tdGVzdC1yZXBvcnQtcmVzdWx0JztcclxuaW1wb3J0ICcuL2NvbXBvbmVudHMvbXV0YXRpb24tdGVzdC1yZXBvcnQtYnJlYWRjcnVtYic7XHJcbmltcG9ydCAnLi9jb21wb25lbnRzL211dGF0aW9uLXRlc3QtcmVwb3J0LXRpdGxlJztcclxuaW1wb3J0ICcuL2NvbXBvbmVudHMvbXV0YXRpb24tdGVzdC1yZXBvcnQtcm91dGVyJztcclxuaW1wb3J0ICcuL2NvbXBvbmVudHMvbXV0YXRpb24tdGVzdC1yZXBvcnQtbXV0YW50JztcclxuaW1wb3J0ICcuL2NvbXBvbmVudHMvbXV0YXRpb24tdGVzdC1yZXBvcnQtZmlsZS1sZWdlbmQnO1xyXG4iLCJpbXBvcnQgeyBNdXRhbnRSZXN1bHQsIEZpbGVSZXN1bHREaWN0aW9uYXJ5LCBNdXRhbnRTdGF0dXMsIEZpbGVSZXN1bHQgfSBmcm9tICcuLi8uLi9hcGknO1xyXG5pbXBvcnQgeyBmbGF0TWFwLCBST09UX05BTUUgfSBmcm9tICcuLi9oZWxwZXJzJztcclxuaW1wb3J0IHsgVGhyZXNob2xkcyB9IGZyb20gJy4uLy4uL2FwaS9UaHJlc2hvbGRzJztcclxuXHJcbmNvbnN0IERFRkFVTFRfSUZfTk9fVkFMSURfTVVUQU5UUyA9IDEwMDtcclxuXHJcbmV4cG9ydCBjbGFzcyBUYWJsZVJvdyB7XHJcblxyXG4gIHB1YmxpYyBraWxsZWQ6IG51bWJlcjtcclxuICBwdWJsaWMgdGltZW91dDogbnVtYmVyO1xyXG4gIHB1YmxpYyBzdXJ2aXZlZDogbnVtYmVyO1xyXG4gIHB1YmxpYyBub0NvdmVyYWdlOiBudW1iZXI7XHJcbiAgcHVibGljIHJ1bnRpbWVFcnJvcnM6IG51bWJlcjtcclxuICBwdWJsaWMgY29tcGlsZUVycm9yczogbnVtYmVyO1xyXG4gIHB1YmxpYyB0b3RhbERldGVjdGVkOiBudW1iZXI7XHJcbiAgcHVibGljIHRvdGFsVW5kZXRlY3RlZDogbnVtYmVyO1xyXG4gIHB1YmxpYyB0b3RhbENvdmVyZWQ6IG51bWJlcjtcclxuICBwdWJsaWMgdG90YWxWYWxpZDogbnVtYmVyO1xyXG4gIHB1YmxpYyB0b3RhbEludmFsaWQ6IG51bWJlcjtcclxuICBwdWJsaWMgdG90YWxNdXRhbnRzOiBudW1iZXI7XHJcbiAgcHVibGljIG11dGF0aW9uU2NvcmU6IG51bWJlcjtcclxuICBwdWJsaWMgbXV0YXRpb25TY29yZUJhc2VkT25Db3ZlcmVkQ29kZTogbnVtYmVyO1xyXG5cclxuICBjb25zdHJ1Y3RvcihwdWJsaWMgbmFtZTogc3RyaW5nLCByZXN1bHRzOiBNdXRhbnRSZXN1bHRbXSwgcHVibGljIHNob3VsZExpbms6IGJvb2xlYW4pIHtcclxuICAgIGNvbnN0IGNvdW50ID0gKG11dGFudFJlc3VsdDogTXV0YW50U3RhdHVzKSA9PiByZXN1bHRzLmZpbHRlcihtdXRhbnQgPT4gbXV0YW50LnN0YXR1cyA9PT0gbXV0YW50UmVzdWx0KS5sZW5ndGg7XHJcbiAgICB0aGlzLmtpbGxlZCA9IGNvdW50KE11dGFudFN0YXR1cy5LaWxsZWQpO1xyXG4gICAgdGhpcy50aW1lb3V0ID0gY291bnQoTXV0YW50U3RhdHVzLlRpbWVvdXQpO1xyXG4gICAgdGhpcy5zdXJ2aXZlZCA9IGNvdW50KE11dGFudFN0YXR1cy5TdXJ2aXZlZCk7XHJcbiAgICB0aGlzLm5vQ292ZXJhZ2UgPSBjb3VudChNdXRhbnRTdGF0dXMuTm9Db3ZlcmFnZSk7XHJcbiAgICB0aGlzLnJ1bnRpbWVFcnJvcnMgPSBjb3VudChNdXRhbnRTdGF0dXMuUnVudGltZUVycm9yKTtcclxuICAgIHRoaXMuY29tcGlsZUVycm9ycyA9IGNvdW50KE11dGFudFN0YXR1cy5Db21waWxlRXJyb3IpO1xyXG5cclxuICAgIHRoaXMudG90YWxEZXRlY3RlZCA9IHRoaXMudGltZW91dCArIHRoaXMua2lsbGVkO1xyXG4gICAgdGhpcy50b3RhbFVuZGV0ZWN0ZWQgPSB0aGlzLnN1cnZpdmVkICsgdGhpcy5ub0NvdmVyYWdlO1xyXG4gICAgdGhpcy50b3RhbENvdmVyZWQgPSB0aGlzLnRvdGFsRGV0ZWN0ZWQgKyB0aGlzLnN1cnZpdmVkO1xyXG4gICAgdGhpcy50b3RhbFZhbGlkID0gdGhpcy50b3RhbFVuZGV0ZWN0ZWQgKyB0aGlzLnRvdGFsRGV0ZWN0ZWQ7XHJcbiAgICB0aGlzLnRvdGFsSW52YWxpZCA9IHRoaXMucnVudGltZUVycm9ycyArIHRoaXMuY29tcGlsZUVycm9ycztcclxuICAgIHRoaXMudG90YWxNdXRhbnRzID0gdGhpcy50b3RhbFZhbGlkICsgdGhpcy50b3RhbEludmFsaWQ7XHJcbiAgICB0aGlzLm11dGF0aW9uU2NvcmUgPSB0aGlzLnRvdGFsVmFsaWQgPiAwID8gdGhpcy50b3RhbERldGVjdGVkIC8gdGhpcy50b3RhbFZhbGlkICogMTAwIDogREVGQVVMVF9JRl9OT19WQUxJRF9NVVRBTlRTO1xyXG4gICAgdGhpcy5tdXRhdGlvblNjb3JlQmFzZWRPbkNvdmVyZWRDb2RlID0gdGhpcy50b3RhbFZhbGlkID4gMCA/IHRoaXMudG90YWxEZXRlY3RlZCAvIHRoaXMudG90YWxDb3ZlcmVkICogMTAwIHx8IDAgOiBERUZBVUxUX0lGX05PX1ZBTElEX01VVEFOVFM7XHJcbiAgfVxyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgUmVzdWx0VGFibGUge1xyXG5cclxuICBjb25zdHJ1Y3RvcihwdWJsaWMgcm93czogVGFibGVSb3dbXSwgcHVibGljIHRocmVzaG9sZHM6IFRocmVzaG9sZHMpIHt9XHJcblxyXG4gIHB1YmxpYyBzdGF0aWMgZm9yRmlsZShuYW1lOiBzdHJpbmcsIHJlc3VsdDogRmlsZVJlc3VsdCwgdGhyZXNob2xkczogVGhyZXNob2xkcykge1xyXG4gICAgcmV0dXJuIG5ldyBSZXN1bHRUYWJsZShbXHJcbiAgICAgIG5ldyBUYWJsZVJvdyhuYW1lLCByZXN1bHQubXV0YW50cywgZmFsc2UpXHJcbiAgICBdLCB0aHJlc2hvbGRzKTtcclxuICB9XHJcbn1cclxuIiwiZXhwb3J0cyA9IG1vZHVsZS5leHBvcnRzID0gcmVxdWlyZShcIi4uLy4uL25vZGVfbW9kdWxlcy9jc3MtbG9hZGVyL2Rpc3QvcnVudGltZS9hcGkuanNcIikoZmFsc2UpO1xuLy8gTW9kdWxlXG5leHBvcnRzLnB1c2goW21vZHVsZS5pZCwgXCIvKiFcXG4gKiBCb290c3RyYXAgUmVib290IHY0LjMuMSAoaHR0cHM6Ly9nZXRib290c3RyYXAuY29tLylcXG4gKiBDb3B5cmlnaHQgMjAxMS0yMDE5IFRoZSBCb290c3RyYXAgQXV0aG9yc1xcbiAqIENvcHlyaWdodCAyMDExLTIwMTkgVHdpdHRlciwgSW5jLlxcbiAqIExpY2Vuc2VkIHVuZGVyIE1JVCAoaHR0cHM6Ly9naXRodWIuY29tL3R3YnMvYm9vdHN0cmFwL2Jsb2IvbWFzdGVyL0xJQ0VOU0UpXFxuICogRm9ya2VkIGZyb20gTm9ybWFsaXplLmNzcywgbGljZW5zZWQgTUlUIChodHRwczovL2dpdGh1Yi5jb20vbmVjb2xhcy9ub3JtYWxpemUuY3NzL2Jsb2IvbWFzdGVyL0xJQ0VOU0UubWQpXFxuICovXFxuKixcXG4qOjpiZWZvcmUsXFxuKjo6YWZ0ZXIge1xcbiAgYm94LXNpemluZzogYm9yZGVyLWJveDsgfVxcblxcbmh0bWwge1xcbiAgZm9udC1mYW1pbHk6IHNhbnMtc2VyaWY7XFxuICBsaW5lLWhlaWdodDogMS4xNTtcXG4gIC13ZWJraXQtdGV4dC1zaXplLWFkanVzdDogMTAwJTtcXG4gIC13ZWJraXQtdGFwLWhpZ2hsaWdodC1jb2xvcjogcmdiYSgwLCAwLCAwLCAwKTsgfVxcblxcbmFydGljbGUsIGFzaWRlLCBmaWdjYXB0aW9uLCBmaWd1cmUsIGZvb3RlciwgaGVhZGVyLCBoZ3JvdXAsIG1haW4sIG5hdiwgc2VjdGlvbiB7XFxuICBkaXNwbGF5OiBibG9jazsgfVxcblxcbmJvZHkge1xcbiAgbWFyZ2luOiAwO1xcbiAgZm9udC1mYW1pbHk6IC1hcHBsZS1zeXN0ZW0sIEJsaW5rTWFjU3lzdGVtRm9udCwgXFxcIlNlZ29lIFVJXFxcIiwgUm9ib3RvLCBcXFwiSGVsdmV0aWNhIE5ldWVcXFwiLCBBcmlhbCwgXFxcIk5vdG8gU2Fuc1xcXCIsIHNhbnMtc2VyaWYsIFxcXCJBcHBsZSBDb2xvciBFbW9qaVxcXCIsIFxcXCJTZWdvZSBVSSBFbW9qaVxcXCIsIFxcXCJTZWdvZSBVSSBTeW1ib2xcXFwiLCBcXFwiTm90byBDb2xvciBFbW9qaVxcXCI7XFxuICBmb250LXNpemU6IDFyZW07XFxuICBmb250LXdlaWdodDogNDAwO1xcbiAgbGluZS1oZWlnaHQ6IDEuNTtcXG4gIGNvbG9yOiAjMjEyNTI5O1xcbiAgdGV4dC1hbGlnbjogbGVmdDtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNmZmY7IH1cXG5cXG5bdGFiaW5kZXg9XFxcIi0xXFxcIl06Zm9jdXMge1xcbiAgb3V0bGluZTogMCAhaW1wb3J0YW50OyB9XFxuXFxuaHIge1xcbiAgYm94LXNpemluZzogY29udGVudC1ib3g7XFxuICBoZWlnaHQ6IDA7XFxuICBvdmVyZmxvdzogdmlzaWJsZTsgfVxcblxcbmgxLCBoMiwgaDMsIGg0LCBoNSwgaDYge1xcbiAgbWFyZ2luLXRvcDogMDtcXG4gIG1hcmdpbi1ib3R0b206IDAuNXJlbTsgfVxcblxcbnAge1xcbiAgbWFyZ2luLXRvcDogMDtcXG4gIG1hcmdpbi1ib3R0b206IDFyZW07IH1cXG5cXG5hYmJyW3RpdGxlXSxcXG5hYmJyW2RhdGEtb3JpZ2luYWwtdGl0bGVdIHtcXG4gIHRleHQtZGVjb3JhdGlvbjogdW5kZXJsaW5lO1xcbiAgdGV4dC1kZWNvcmF0aW9uOiB1bmRlcmxpbmUgZG90dGVkO1xcbiAgY3Vyc29yOiBoZWxwO1xcbiAgYm9yZGVyLWJvdHRvbTogMDtcXG4gIHRleHQtZGVjb3JhdGlvbi1za2lwLWluazogbm9uZTsgfVxcblxcbmFkZHJlc3Mge1xcbiAgbWFyZ2luLWJvdHRvbTogMXJlbTtcXG4gIGZvbnQtc3R5bGU6IG5vcm1hbDtcXG4gIGxpbmUtaGVpZ2h0OiBpbmhlcml0OyB9XFxuXFxub2wsXFxudWwsXFxuZGwge1xcbiAgbWFyZ2luLXRvcDogMDtcXG4gIG1hcmdpbi1ib3R0b206IDFyZW07IH1cXG5cXG5vbCBvbCxcXG51bCB1bCxcXG5vbCB1bCxcXG51bCBvbCB7XFxuICBtYXJnaW4tYm90dG9tOiAwOyB9XFxuXFxuZHQge1xcbiAgZm9udC13ZWlnaHQ6IDcwMDsgfVxcblxcbmRkIHtcXG4gIG1hcmdpbi1ib3R0b206IC41cmVtO1xcbiAgbWFyZ2luLWxlZnQ6IDA7IH1cXG5cXG5ibG9ja3F1b3RlIHtcXG4gIG1hcmdpbjogMCAwIDFyZW07IH1cXG5cXG5iLFxcbnN0cm9uZyB7XFxuICBmb250LXdlaWdodDogYm9sZGVyOyB9XFxuXFxuc21hbGwge1xcbiAgZm9udC1zaXplOiA4MCU7IH1cXG5cXG5zdWIsXFxuc3VwIHtcXG4gIHBvc2l0aW9uOiByZWxhdGl2ZTtcXG4gIGZvbnQtc2l6ZTogNzUlO1xcbiAgbGluZS1oZWlnaHQ6IDA7XFxuICB2ZXJ0aWNhbC1hbGlnbjogYmFzZWxpbmU7IH1cXG5cXG5zdWIge1xcbiAgYm90dG9tOiAtLjI1ZW07IH1cXG5cXG5zdXAge1xcbiAgdG9wOiAtLjVlbTsgfVxcblxcbmEge1xcbiAgY29sb3I6ICMwMDdiZmY7XFxuICB0ZXh0LWRlY29yYXRpb246IG5vbmU7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiB0cmFuc3BhcmVudDsgfVxcbiAgYTpob3ZlciB7XFxuICAgIGNvbG9yOiAjMDA1NmIzO1xcbiAgICB0ZXh0LWRlY29yYXRpb246IHVuZGVybGluZTsgfVxcblxcbmE6bm90KFtocmVmXSk6bm90KFt0YWJpbmRleF0pIHtcXG4gIGNvbG9yOiBpbmhlcml0O1xcbiAgdGV4dC1kZWNvcmF0aW9uOiBub25lOyB9XFxuICBhOm5vdChbaHJlZl0pOm5vdChbdGFiaW5kZXhdKTpob3ZlciwgYTpub3QoW2hyZWZdKTpub3QoW3RhYmluZGV4XSk6Zm9jdXMge1xcbiAgICBjb2xvcjogaW5oZXJpdDtcXG4gICAgdGV4dC1kZWNvcmF0aW9uOiBub25lOyB9XFxuICBhOm5vdChbaHJlZl0pOm5vdChbdGFiaW5kZXhdKTpmb2N1cyB7XFxuICAgIG91dGxpbmU6IDA7IH1cXG5cXG5wcmUsXFxuY29kZSxcXG5rYmQsXFxuc2FtcCB7XFxuICBmb250LWZhbWlseTogU0ZNb25vLVJlZ3VsYXIsIE1lbmxvLCBNb25hY28sIENvbnNvbGFzLCBcXFwiTGliZXJhdGlvbiBNb25vXFxcIiwgXFxcIkNvdXJpZXIgTmV3XFxcIiwgbW9ub3NwYWNlO1xcbiAgZm9udC1zaXplOiAxZW07IH1cXG5cXG5wcmUge1xcbiAgbWFyZ2luLXRvcDogMDtcXG4gIG1hcmdpbi1ib3R0b206IDFyZW07XFxuICBvdmVyZmxvdzogYXV0bzsgfVxcblxcbmZpZ3VyZSB7XFxuICBtYXJnaW46IDAgMCAxcmVtOyB9XFxuXFxuaW1nIHtcXG4gIHZlcnRpY2FsLWFsaWduOiBtaWRkbGU7XFxuICBib3JkZXItc3R5bGU6IG5vbmU7IH1cXG5cXG5zdmcge1xcbiAgb3ZlcmZsb3c6IGhpZGRlbjtcXG4gIHZlcnRpY2FsLWFsaWduOiBtaWRkbGU7IH1cXG5cXG50YWJsZSB7XFxuICBib3JkZXItY29sbGFwc2U6IGNvbGxhcHNlOyB9XFxuXFxuY2FwdGlvbiB7XFxuICBwYWRkaW5nLXRvcDogMC43NXJlbTtcXG4gIHBhZGRpbmctYm90dG9tOiAwLjc1cmVtO1xcbiAgY29sb3I6ICM2Yzc1N2Q7XFxuICB0ZXh0LWFsaWduOiBsZWZ0O1xcbiAgY2FwdGlvbi1zaWRlOiBib3R0b207IH1cXG5cXG50aCB7XFxuICB0ZXh0LWFsaWduOiBpbmhlcml0OyB9XFxuXFxubGFiZWwge1xcbiAgZGlzcGxheTogaW5saW5lLWJsb2NrO1xcbiAgbWFyZ2luLWJvdHRvbTogMC41cmVtOyB9XFxuXFxuYnV0dG9uIHtcXG4gIGJvcmRlci1yYWRpdXM6IDA7IH1cXG5cXG5idXR0b246Zm9jdXMge1xcbiAgb3V0bGluZTogMXB4IGRvdHRlZDtcXG4gIG91dGxpbmU6IDVweCBhdXRvIC13ZWJraXQtZm9jdXMtcmluZy1jb2xvcjsgfVxcblxcbmlucHV0LFxcbmJ1dHRvbixcXG5zZWxlY3QsXFxub3B0Z3JvdXAsXFxudGV4dGFyZWEge1xcbiAgbWFyZ2luOiAwO1xcbiAgZm9udC1mYW1pbHk6IGluaGVyaXQ7XFxuICBmb250LXNpemU6IGluaGVyaXQ7XFxuICBsaW5lLWhlaWdodDogaW5oZXJpdDsgfVxcblxcbmJ1dHRvbixcXG5pbnB1dCB7XFxuICBvdmVyZmxvdzogdmlzaWJsZTsgfVxcblxcbmJ1dHRvbixcXG5zZWxlY3Qge1xcbiAgdGV4dC10cmFuc2Zvcm06IG5vbmU7IH1cXG5cXG5zZWxlY3Qge1xcbiAgd29yZC13cmFwOiBub3JtYWw7IH1cXG5cXG5idXR0b24sXFxuW3R5cGU9XFxcImJ1dHRvblxcXCJdLFxcblt0eXBlPVxcXCJyZXNldFxcXCJdLFxcblt0eXBlPVxcXCJzdWJtaXRcXFwiXSB7XFxuICAtd2Via2l0LWFwcGVhcmFuY2U6IGJ1dHRvbjsgfVxcblxcbmJ1dHRvbjpub3QoOmRpc2FibGVkKSxcXG5bdHlwZT1cXFwiYnV0dG9uXFxcIl06bm90KDpkaXNhYmxlZCksXFxuW3R5cGU9XFxcInJlc2V0XFxcIl06bm90KDpkaXNhYmxlZCksXFxuW3R5cGU9XFxcInN1Ym1pdFxcXCJdOm5vdCg6ZGlzYWJsZWQpIHtcXG4gIGN1cnNvcjogcG9pbnRlcjsgfVxcblxcbmJ1dHRvbjo6LW1vei1mb2N1cy1pbm5lcixcXG5bdHlwZT1cXFwiYnV0dG9uXFxcIl06Oi1tb3otZm9jdXMtaW5uZXIsXFxuW3R5cGU9XFxcInJlc2V0XFxcIl06Oi1tb3otZm9jdXMtaW5uZXIsXFxuW3R5cGU9XFxcInN1Ym1pdFxcXCJdOjotbW96LWZvY3VzLWlubmVyIHtcXG4gIHBhZGRpbmc6IDA7XFxuICBib3JkZXItc3R5bGU6IG5vbmU7IH1cXG5cXG5pbnB1dFt0eXBlPVxcXCJyYWRpb1xcXCJdLFxcbmlucHV0W3R5cGU9XFxcImNoZWNrYm94XFxcIl0ge1xcbiAgYm94LXNpemluZzogYm9yZGVyLWJveDtcXG4gIHBhZGRpbmc6IDA7IH1cXG5cXG5pbnB1dFt0eXBlPVxcXCJkYXRlXFxcIl0sXFxuaW5wdXRbdHlwZT1cXFwidGltZVxcXCJdLFxcbmlucHV0W3R5cGU9XFxcImRhdGV0aW1lLWxvY2FsXFxcIl0sXFxuaW5wdXRbdHlwZT1cXFwibW9udGhcXFwiXSB7XFxuICAtd2Via2l0LWFwcGVhcmFuY2U6IGxpc3Rib3g7IH1cXG5cXG50ZXh0YXJlYSB7XFxuICBvdmVyZmxvdzogYXV0bztcXG4gIHJlc2l6ZTogdmVydGljYWw7IH1cXG5cXG5maWVsZHNldCB7XFxuICBtaW4td2lkdGg6IDA7XFxuICBwYWRkaW5nOiAwO1xcbiAgbWFyZ2luOiAwO1xcbiAgYm9yZGVyOiAwOyB9XFxuXFxubGVnZW5kIHtcXG4gIGRpc3BsYXk6IGJsb2NrO1xcbiAgd2lkdGg6IDEwMCU7XFxuICBtYXgtd2lkdGg6IDEwMCU7XFxuICBwYWRkaW5nOiAwO1xcbiAgbWFyZ2luLWJvdHRvbTogLjVyZW07XFxuICBmb250LXNpemU6IDEuNXJlbTtcXG4gIGxpbmUtaGVpZ2h0OiBpbmhlcml0O1xcbiAgY29sb3I6IGluaGVyaXQ7XFxuICB3aGl0ZS1zcGFjZTogbm9ybWFsOyB9XFxuXFxucHJvZ3Jlc3Mge1xcbiAgdmVydGljYWwtYWxpZ246IGJhc2VsaW5lOyB9XFxuXFxuW3R5cGU9XFxcIm51bWJlclxcXCJdOjotd2Via2l0LWlubmVyLXNwaW4tYnV0dG9uLFxcblt0eXBlPVxcXCJudW1iZXJcXFwiXTo6LXdlYmtpdC1vdXRlci1zcGluLWJ1dHRvbiB7XFxuICBoZWlnaHQ6IGF1dG87IH1cXG5cXG5bdHlwZT1cXFwic2VhcmNoXFxcIl0ge1xcbiAgb3V0bGluZS1vZmZzZXQ6IC0ycHg7XFxuICAtd2Via2l0LWFwcGVhcmFuY2U6IG5vbmU7IH1cXG5cXG5bdHlwZT1cXFwic2VhcmNoXFxcIl06Oi13ZWJraXQtc2VhcmNoLWRlY29yYXRpb24ge1xcbiAgLXdlYmtpdC1hcHBlYXJhbmNlOiBub25lOyB9XFxuXFxuOjotd2Via2l0LWZpbGUtdXBsb2FkLWJ1dHRvbiB7XFxuICBmb250OiBpbmhlcml0O1xcbiAgLXdlYmtpdC1hcHBlYXJhbmNlOiBidXR0b247IH1cXG5cXG5vdXRwdXQge1xcbiAgZGlzcGxheTogaW5saW5lLWJsb2NrOyB9XFxuXFxuc3VtbWFyeSB7XFxuICBkaXNwbGF5OiBsaXN0LWl0ZW07XFxuICBjdXJzb3I6IHBvaW50ZXI7IH1cXG5cXG50ZW1wbGF0ZSB7XFxuICBkaXNwbGF5OiBub25lOyB9XFxuXFxuW2hpZGRlbl0ge1xcbiAgZGlzcGxheTogbm9uZSAhaW1wb3J0YW50OyB9XFxuXFxuLmNvbnRhaW5lciB7XFxuICB3aWR0aDogMTAwJTtcXG4gIHBhZGRpbmctcmlnaHQ6IDE1cHg7XFxuICBwYWRkaW5nLWxlZnQ6IDE1cHg7XFxuICBtYXJnaW4tcmlnaHQ6IGF1dG87XFxuICBtYXJnaW4tbGVmdDogYXV0bzsgfVxcbiAgQG1lZGlhIChtaW4td2lkdGg6IDU3NnB4KSB7XFxuICAgIC5jb250YWluZXIge1xcbiAgICAgIG1heC13aWR0aDogNTQwcHg7IH0gfVxcbiAgQG1lZGlhIChtaW4td2lkdGg6IDc2OHB4KSB7XFxuICAgIC5jb250YWluZXIge1xcbiAgICAgIG1heC13aWR0aDogNzIwcHg7IH0gfVxcbiAgQG1lZGlhIChtaW4td2lkdGg6IDk5MnB4KSB7XFxuICAgIC5jb250YWluZXIge1xcbiAgICAgIG1heC13aWR0aDogOTYwcHg7IH0gfVxcbiAgQG1lZGlhIChtaW4td2lkdGg6IDEyMDBweCkge1xcbiAgICAuY29udGFpbmVyIHtcXG4gICAgICBtYXgtd2lkdGg6IDExNDBweDsgfSB9XFxuXFxuLmNvbnRhaW5lci1mbHVpZCB7XFxuICB3aWR0aDogMTAwJTtcXG4gIHBhZGRpbmctcmlnaHQ6IDE1cHg7XFxuICBwYWRkaW5nLWxlZnQ6IDE1cHg7XFxuICBtYXJnaW4tcmlnaHQ6IGF1dG87XFxuICBtYXJnaW4tbGVmdDogYXV0bzsgfVxcblxcbi5yb3cge1xcbiAgZGlzcGxheTogZmxleDtcXG4gIGZsZXgtd3JhcDogd3JhcDtcXG4gIG1hcmdpbi1yaWdodDogLTE1cHg7XFxuICBtYXJnaW4tbGVmdDogLTE1cHg7IH1cXG5cXG4ubm8tZ3V0dGVycyB7XFxuICBtYXJnaW4tcmlnaHQ6IDA7XFxuICBtYXJnaW4tbGVmdDogMDsgfVxcbiAgLm5vLWd1dHRlcnMgPiAuY29sLFxcbiAgLm5vLWd1dHRlcnMgPiBbY2xhc3MqPVxcXCJjb2wtXFxcIl0ge1xcbiAgICBwYWRkaW5nLXJpZ2h0OiAwO1xcbiAgICBwYWRkaW5nLWxlZnQ6IDA7IH1cXG5cXG4uY29sLTEsIC5jb2wtMiwgLmNvbC0zLCAuY29sLTQsIC5jb2wtNSwgLmNvbC02LCAuY29sLTcsIC5jb2wtOCwgLmNvbC05LCAuY29sLTEwLCAuY29sLTExLCAuY29sLTEyLCAuY29sLFxcbi5jb2wtYXV0bywgLmNvbC1zbS0xLCAuY29sLXNtLTIsIC5jb2wtc20tMywgLmNvbC1zbS00LCAuY29sLXNtLTUsIC5jb2wtc20tNiwgLmNvbC1zbS03LCAuY29sLXNtLTgsIC5jb2wtc20tOSwgLmNvbC1zbS0xMCwgLmNvbC1zbS0xMSwgLmNvbC1zbS0xMiwgLmNvbC1zbSxcXG4uY29sLXNtLWF1dG8sIC5jb2wtbWQtMSwgLmNvbC1tZC0yLCAuY29sLW1kLTMsIC5jb2wtbWQtNCwgLmNvbC1tZC01LCAuY29sLW1kLTYsIC5jb2wtbWQtNywgLmNvbC1tZC04LCAuY29sLW1kLTksIC5jb2wtbWQtMTAsIC5jb2wtbWQtMTEsIC5jb2wtbWQtMTIsIC5jb2wtbWQsXFxuLmNvbC1tZC1hdXRvLCAuY29sLWxnLTEsIC5jb2wtbGctMiwgLmNvbC1sZy0zLCAuY29sLWxnLTQsIC5jb2wtbGctNSwgLmNvbC1sZy02LCAuY29sLWxnLTcsIC5jb2wtbGctOCwgLmNvbC1sZy05LCAuY29sLWxnLTEwLCAuY29sLWxnLTExLCAuY29sLWxnLTEyLCAuY29sLWxnLFxcbi5jb2wtbGctYXV0bywgLmNvbC14bC0xLCAuY29sLXhsLTIsIC5jb2wteGwtMywgLmNvbC14bC00LCAuY29sLXhsLTUsIC5jb2wteGwtNiwgLmNvbC14bC03LCAuY29sLXhsLTgsIC5jb2wteGwtOSwgLmNvbC14bC0xMCwgLmNvbC14bC0xMSwgLmNvbC14bC0xMiwgLmNvbC14bCxcXG4uY29sLXhsLWF1dG8ge1xcbiAgcG9zaXRpb246IHJlbGF0aXZlO1xcbiAgd2lkdGg6IDEwMCU7XFxuICBwYWRkaW5nLXJpZ2h0OiAxNXB4O1xcbiAgcGFkZGluZy1sZWZ0OiAxNXB4OyB9XFxuXFxuLmNvbCB7XFxuICBmbGV4LWJhc2lzOiAwO1xcbiAgZmxleC1ncm93OiAxO1xcbiAgbWF4LXdpZHRoOiAxMDAlOyB9XFxuXFxuLmNvbC1hdXRvIHtcXG4gIGZsZXg6IDAgMCBhdXRvO1xcbiAgd2lkdGg6IGF1dG87XFxuICBtYXgtd2lkdGg6IDEwMCU7IH1cXG5cXG4uY29sLTEge1xcbiAgZmxleDogMCAwIDguMzMzMzMlO1xcbiAgbWF4LXdpZHRoOiA4LjMzMzMzJTsgfVxcblxcbi5jb2wtMiB7XFxuICBmbGV4OiAwIDAgMTYuNjY2NjclO1xcbiAgbWF4LXdpZHRoOiAxNi42NjY2NyU7IH1cXG5cXG4uY29sLTMge1xcbiAgZmxleDogMCAwIDI1JTtcXG4gIG1heC13aWR0aDogMjUlOyB9XFxuXFxuLmNvbC00IHtcXG4gIGZsZXg6IDAgMCAzMy4zMzMzMyU7XFxuICBtYXgtd2lkdGg6IDMzLjMzMzMzJTsgfVxcblxcbi5jb2wtNSB7XFxuICBmbGV4OiAwIDAgNDEuNjY2NjclO1xcbiAgbWF4LXdpZHRoOiA0MS42NjY2NyU7IH1cXG5cXG4uY29sLTYge1xcbiAgZmxleDogMCAwIDUwJTtcXG4gIG1heC13aWR0aDogNTAlOyB9XFxuXFxuLmNvbC03IHtcXG4gIGZsZXg6IDAgMCA1OC4zMzMzMyU7XFxuICBtYXgtd2lkdGg6IDU4LjMzMzMzJTsgfVxcblxcbi5jb2wtOCB7XFxuICBmbGV4OiAwIDAgNjYuNjY2NjclO1xcbiAgbWF4LXdpZHRoOiA2Ni42NjY2NyU7IH1cXG5cXG4uY29sLTkge1xcbiAgZmxleDogMCAwIDc1JTtcXG4gIG1heC13aWR0aDogNzUlOyB9XFxuXFxuLmNvbC0xMCB7XFxuICBmbGV4OiAwIDAgODMuMzMzMzMlO1xcbiAgbWF4LXdpZHRoOiA4My4zMzMzMyU7IH1cXG5cXG4uY29sLTExIHtcXG4gIGZsZXg6IDAgMCA5MS42NjY2NyU7XFxuICBtYXgtd2lkdGg6IDkxLjY2NjY3JTsgfVxcblxcbi5jb2wtMTIge1xcbiAgZmxleDogMCAwIDEwMCU7XFxuICBtYXgtd2lkdGg6IDEwMCU7IH1cXG5cXG4ub3JkZXItZmlyc3Qge1xcbiAgb3JkZXI6IC0xOyB9XFxuXFxuLm9yZGVyLWxhc3Qge1xcbiAgb3JkZXI6IDEzOyB9XFxuXFxuLm9yZGVyLTAge1xcbiAgb3JkZXI6IDA7IH1cXG5cXG4ub3JkZXItMSB7XFxuICBvcmRlcjogMTsgfVxcblxcbi5vcmRlci0yIHtcXG4gIG9yZGVyOiAyOyB9XFxuXFxuLm9yZGVyLTMge1xcbiAgb3JkZXI6IDM7IH1cXG5cXG4ub3JkZXItNCB7XFxuICBvcmRlcjogNDsgfVxcblxcbi5vcmRlci01IHtcXG4gIG9yZGVyOiA1OyB9XFxuXFxuLm9yZGVyLTYge1xcbiAgb3JkZXI6IDY7IH1cXG5cXG4ub3JkZXItNyB7XFxuICBvcmRlcjogNzsgfVxcblxcbi5vcmRlci04IHtcXG4gIG9yZGVyOiA4OyB9XFxuXFxuLm9yZGVyLTkge1xcbiAgb3JkZXI6IDk7IH1cXG5cXG4ub3JkZXItMTAge1xcbiAgb3JkZXI6IDEwOyB9XFxuXFxuLm9yZGVyLTExIHtcXG4gIG9yZGVyOiAxMTsgfVxcblxcbi5vcmRlci0xMiB7XFxuICBvcmRlcjogMTI7IH1cXG5cXG4ub2Zmc2V0LTEge1xcbiAgbWFyZ2luLWxlZnQ6IDguMzMzMzMlOyB9XFxuXFxuLm9mZnNldC0yIHtcXG4gIG1hcmdpbi1sZWZ0OiAxNi42NjY2NyU7IH1cXG5cXG4ub2Zmc2V0LTMge1xcbiAgbWFyZ2luLWxlZnQ6IDI1JTsgfVxcblxcbi5vZmZzZXQtNCB7XFxuICBtYXJnaW4tbGVmdDogMzMuMzMzMzMlOyB9XFxuXFxuLm9mZnNldC01IHtcXG4gIG1hcmdpbi1sZWZ0OiA0MS42NjY2NyU7IH1cXG5cXG4ub2Zmc2V0LTYge1xcbiAgbWFyZ2luLWxlZnQ6IDUwJTsgfVxcblxcbi5vZmZzZXQtNyB7XFxuICBtYXJnaW4tbGVmdDogNTguMzMzMzMlOyB9XFxuXFxuLm9mZnNldC04IHtcXG4gIG1hcmdpbi1sZWZ0OiA2Ni42NjY2NyU7IH1cXG5cXG4ub2Zmc2V0LTkge1xcbiAgbWFyZ2luLWxlZnQ6IDc1JTsgfVxcblxcbi5vZmZzZXQtMTAge1xcbiAgbWFyZ2luLWxlZnQ6IDgzLjMzMzMzJTsgfVxcblxcbi5vZmZzZXQtMTEge1xcbiAgbWFyZ2luLWxlZnQ6IDkxLjY2NjY3JTsgfVxcblxcbkBtZWRpYSAobWluLXdpZHRoOiA1NzZweCkge1xcbiAgLmNvbC1zbSB7XFxuICAgIGZsZXgtYmFzaXM6IDA7XFxuICAgIGZsZXgtZ3JvdzogMTtcXG4gICAgbWF4LXdpZHRoOiAxMDAlOyB9XFxuICAuY29sLXNtLWF1dG8ge1xcbiAgICBmbGV4OiAwIDAgYXV0bztcXG4gICAgd2lkdGg6IGF1dG87XFxuICAgIG1heC13aWR0aDogMTAwJTsgfVxcbiAgLmNvbC1zbS0xIHtcXG4gICAgZmxleDogMCAwIDguMzMzMzMlO1xcbiAgICBtYXgtd2lkdGg6IDguMzMzMzMlOyB9XFxuICAuY29sLXNtLTIge1xcbiAgICBmbGV4OiAwIDAgMTYuNjY2NjclO1xcbiAgICBtYXgtd2lkdGg6IDE2LjY2NjY3JTsgfVxcbiAgLmNvbC1zbS0zIHtcXG4gICAgZmxleDogMCAwIDI1JTtcXG4gICAgbWF4LXdpZHRoOiAyNSU7IH1cXG4gIC5jb2wtc20tNCB7XFxuICAgIGZsZXg6IDAgMCAzMy4zMzMzMyU7XFxuICAgIG1heC13aWR0aDogMzMuMzMzMzMlOyB9XFxuICAuY29sLXNtLTUge1xcbiAgICBmbGV4OiAwIDAgNDEuNjY2NjclO1xcbiAgICBtYXgtd2lkdGg6IDQxLjY2NjY3JTsgfVxcbiAgLmNvbC1zbS02IHtcXG4gICAgZmxleDogMCAwIDUwJTtcXG4gICAgbWF4LXdpZHRoOiA1MCU7IH1cXG4gIC5jb2wtc20tNyB7XFxuICAgIGZsZXg6IDAgMCA1OC4zMzMzMyU7XFxuICAgIG1heC13aWR0aDogNTguMzMzMzMlOyB9XFxuICAuY29sLXNtLTgge1xcbiAgICBmbGV4OiAwIDAgNjYuNjY2NjclO1xcbiAgICBtYXgtd2lkdGg6IDY2LjY2NjY3JTsgfVxcbiAgLmNvbC1zbS05IHtcXG4gICAgZmxleDogMCAwIDc1JTtcXG4gICAgbWF4LXdpZHRoOiA3NSU7IH1cXG4gIC5jb2wtc20tMTAge1xcbiAgICBmbGV4OiAwIDAgODMuMzMzMzMlO1xcbiAgICBtYXgtd2lkdGg6IDgzLjMzMzMzJTsgfVxcbiAgLmNvbC1zbS0xMSB7XFxuICAgIGZsZXg6IDAgMCA5MS42NjY2NyU7XFxuICAgIG1heC13aWR0aDogOTEuNjY2NjclOyB9XFxuICAuY29sLXNtLTEyIHtcXG4gICAgZmxleDogMCAwIDEwMCU7XFxuICAgIG1heC13aWR0aDogMTAwJTsgfVxcbiAgLm9yZGVyLXNtLWZpcnN0IHtcXG4gICAgb3JkZXI6IC0xOyB9XFxuICAub3JkZXItc20tbGFzdCB7XFxuICAgIG9yZGVyOiAxMzsgfVxcbiAgLm9yZGVyLXNtLTAge1xcbiAgICBvcmRlcjogMDsgfVxcbiAgLm9yZGVyLXNtLTEge1xcbiAgICBvcmRlcjogMTsgfVxcbiAgLm9yZGVyLXNtLTIge1xcbiAgICBvcmRlcjogMjsgfVxcbiAgLm9yZGVyLXNtLTMge1xcbiAgICBvcmRlcjogMzsgfVxcbiAgLm9yZGVyLXNtLTQge1xcbiAgICBvcmRlcjogNDsgfVxcbiAgLm9yZGVyLXNtLTUge1xcbiAgICBvcmRlcjogNTsgfVxcbiAgLm9yZGVyLXNtLTYge1xcbiAgICBvcmRlcjogNjsgfVxcbiAgLm9yZGVyLXNtLTcge1xcbiAgICBvcmRlcjogNzsgfVxcbiAgLm9yZGVyLXNtLTgge1xcbiAgICBvcmRlcjogODsgfVxcbiAgLm9yZGVyLXNtLTkge1xcbiAgICBvcmRlcjogOTsgfVxcbiAgLm9yZGVyLXNtLTEwIHtcXG4gICAgb3JkZXI6IDEwOyB9XFxuICAub3JkZXItc20tMTEge1xcbiAgICBvcmRlcjogMTE7IH1cXG4gIC5vcmRlci1zbS0xMiB7XFxuICAgIG9yZGVyOiAxMjsgfVxcbiAgLm9mZnNldC1zbS0wIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDA7IH1cXG4gIC5vZmZzZXQtc20tMSB7XFxuICAgIG1hcmdpbi1sZWZ0OiA4LjMzMzMzJTsgfVxcbiAgLm9mZnNldC1zbS0yIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDE2LjY2NjY3JTsgfVxcbiAgLm9mZnNldC1zbS0zIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDI1JTsgfVxcbiAgLm9mZnNldC1zbS00IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDMzLjMzMzMzJTsgfVxcbiAgLm9mZnNldC1zbS01IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDQxLjY2NjY3JTsgfVxcbiAgLm9mZnNldC1zbS02IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDUwJTsgfVxcbiAgLm9mZnNldC1zbS03IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDU4LjMzMzMzJTsgfVxcbiAgLm9mZnNldC1zbS04IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDY2LjY2NjY3JTsgfVxcbiAgLm9mZnNldC1zbS05IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDc1JTsgfVxcbiAgLm9mZnNldC1zbS0xMCB7XFxuICAgIG1hcmdpbi1sZWZ0OiA4My4zMzMzMyU7IH1cXG4gIC5vZmZzZXQtc20tMTEge1xcbiAgICBtYXJnaW4tbGVmdDogOTEuNjY2NjclOyB9IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogNzY4cHgpIHtcXG4gIC5jb2wtbWQge1xcbiAgICBmbGV4LWJhc2lzOiAwO1xcbiAgICBmbGV4LWdyb3c6IDE7XFxuICAgIG1heC13aWR0aDogMTAwJTsgfVxcbiAgLmNvbC1tZC1hdXRvIHtcXG4gICAgZmxleDogMCAwIGF1dG87XFxuICAgIHdpZHRoOiBhdXRvO1xcbiAgICBtYXgtd2lkdGg6IDEwMCU7IH1cXG4gIC5jb2wtbWQtMSB7XFxuICAgIGZsZXg6IDAgMCA4LjMzMzMzJTtcXG4gICAgbWF4LXdpZHRoOiA4LjMzMzMzJTsgfVxcbiAgLmNvbC1tZC0yIHtcXG4gICAgZmxleDogMCAwIDE2LjY2NjY3JTtcXG4gICAgbWF4LXdpZHRoOiAxNi42NjY2NyU7IH1cXG4gIC5jb2wtbWQtMyB7XFxuICAgIGZsZXg6IDAgMCAyNSU7XFxuICAgIG1heC13aWR0aDogMjUlOyB9XFxuICAuY29sLW1kLTQge1xcbiAgICBmbGV4OiAwIDAgMzMuMzMzMzMlO1xcbiAgICBtYXgtd2lkdGg6IDMzLjMzMzMzJTsgfVxcbiAgLmNvbC1tZC01IHtcXG4gICAgZmxleDogMCAwIDQxLjY2NjY3JTtcXG4gICAgbWF4LXdpZHRoOiA0MS42NjY2NyU7IH1cXG4gIC5jb2wtbWQtNiB7XFxuICAgIGZsZXg6IDAgMCA1MCU7XFxuICAgIG1heC13aWR0aDogNTAlOyB9XFxuICAuY29sLW1kLTcge1xcbiAgICBmbGV4OiAwIDAgNTguMzMzMzMlO1xcbiAgICBtYXgtd2lkdGg6IDU4LjMzMzMzJTsgfVxcbiAgLmNvbC1tZC04IHtcXG4gICAgZmxleDogMCAwIDY2LjY2NjY3JTtcXG4gICAgbWF4LXdpZHRoOiA2Ni42NjY2NyU7IH1cXG4gIC5jb2wtbWQtOSB7XFxuICAgIGZsZXg6IDAgMCA3NSU7XFxuICAgIG1heC13aWR0aDogNzUlOyB9XFxuICAuY29sLW1kLTEwIHtcXG4gICAgZmxleDogMCAwIDgzLjMzMzMzJTtcXG4gICAgbWF4LXdpZHRoOiA4My4zMzMzMyU7IH1cXG4gIC5jb2wtbWQtMTEge1xcbiAgICBmbGV4OiAwIDAgOTEuNjY2NjclO1xcbiAgICBtYXgtd2lkdGg6IDkxLjY2NjY3JTsgfVxcbiAgLmNvbC1tZC0xMiB7XFxuICAgIGZsZXg6IDAgMCAxMDAlO1xcbiAgICBtYXgtd2lkdGg6IDEwMCU7IH1cXG4gIC5vcmRlci1tZC1maXJzdCB7XFxuICAgIG9yZGVyOiAtMTsgfVxcbiAgLm9yZGVyLW1kLWxhc3Qge1xcbiAgICBvcmRlcjogMTM7IH1cXG4gIC5vcmRlci1tZC0wIHtcXG4gICAgb3JkZXI6IDA7IH1cXG4gIC5vcmRlci1tZC0xIHtcXG4gICAgb3JkZXI6IDE7IH1cXG4gIC5vcmRlci1tZC0yIHtcXG4gICAgb3JkZXI6IDI7IH1cXG4gIC5vcmRlci1tZC0zIHtcXG4gICAgb3JkZXI6IDM7IH1cXG4gIC5vcmRlci1tZC00IHtcXG4gICAgb3JkZXI6IDQ7IH1cXG4gIC5vcmRlci1tZC01IHtcXG4gICAgb3JkZXI6IDU7IH1cXG4gIC5vcmRlci1tZC02IHtcXG4gICAgb3JkZXI6IDY7IH1cXG4gIC5vcmRlci1tZC03IHtcXG4gICAgb3JkZXI6IDc7IH1cXG4gIC5vcmRlci1tZC04IHtcXG4gICAgb3JkZXI6IDg7IH1cXG4gIC5vcmRlci1tZC05IHtcXG4gICAgb3JkZXI6IDk7IH1cXG4gIC5vcmRlci1tZC0xMCB7XFxuICAgIG9yZGVyOiAxMDsgfVxcbiAgLm9yZGVyLW1kLTExIHtcXG4gICAgb3JkZXI6IDExOyB9XFxuICAub3JkZXItbWQtMTIge1xcbiAgICBvcmRlcjogMTI7IH1cXG4gIC5vZmZzZXQtbWQtMCB7XFxuICAgIG1hcmdpbi1sZWZ0OiAwOyB9XFxuICAub2Zmc2V0LW1kLTEge1xcbiAgICBtYXJnaW4tbGVmdDogOC4zMzMzMyU7IH1cXG4gIC5vZmZzZXQtbWQtMiB7XFxuICAgIG1hcmdpbi1sZWZ0OiAxNi42NjY2NyU7IH1cXG4gIC5vZmZzZXQtbWQtMyB7XFxuICAgIG1hcmdpbi1sZWZ0OiAyNSU7IH1cXG4gIC5vZmZzZXQtbWQtNCB7XFxuICAgIG1hcmdpbi1sZWZ0OiAzMy4zMzMzMyU7IH1cXG4gIC5vZmZzZXQtbWQtNSB7XFxuICAgIG1hcmdpbi1sZWZ0OiA0MS42NjY2NyU7IH1cXG4gIC5vZmZzZXQtbWQtNiB7XFxuICAgIG1hcmdpbi1sZWZ0OiA1MCU7IH1cXG4gIC5vZmZzZXQtbWQtNyB7XFxuICAgIG1hcmdpbi1sZWZ0OiA1OC4zMzMzMyU7IH1cXG4gIC5vZmZzZXQtbWQtOCB7XFxuICAgIG1hcmdpbi1sZWZ0OiA2Ni42NjY2NyU7IH1cXG4gIC5vZmZzZXQtbWQtOSB7XFxuICAgIG1hcmdpbi1sZWZ0OiA3NSU7IH1cXG4gIC5vZmZzZXQtbWQtMTAge1xcbiAgICBtYXJnaW4tbGVmdDogODMuMzMzMzMlOyB9XFxuICAub2Zmc2V0LW1kLTExIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDkxLjY2NjY3JTsgfSB9XFxuXFxuQG1lZGlhIChtaW4td2lkdGg6IDk5MnB4KSB7XFxuICAuY29sLWxnIHtcXG4gICAgZmxleC1iYXNpczogMDtcXG4gICAgZmxleC1ncm93OiAxO1xcbiAgICBtYXgtd2lkdGg6IDEwMCU7IH1cXG4gIC5jb2wtbGctYXV0byB7XFxuICAgIGZsZXg6IDAgMCBhdXRvO1xcbiAgICB3aWR0aDogYXV0bztcXG4gICAgbWF4LXdpZHRoOiAxMDAlOyB9XFxuICAuY29sLWxnLTEge1xcbiAgICBmbGV4OiAwIDAgOC4zMzMzMyU7XFxuICAgIG1heC13aWR0aDogOC4zMzMzMyU7IH1cXG4gIC5jb2wtbGctMiB7XFxuICAgIGZsZXg6IDAgMCAxNi42NjY2NyU7XFxuICAgIG1heC13aWR0aDogMTYuNjY2NjclOyB9XFxuICAuY29sLWxnLTMge1xcbiAgICBmbGV4OiAwIDAgMjUlO1xcbiAgICBtYXgtd2lkdGg6IDI1JTsgfVxcbiAgLmNvbC1sZy00IHtcXG4gICAgZmxleDogMCAwIDMzLjMzMzMzJTtcXG4gICAgbWF4LXdpZHRoOiAzMy4zMzMzMyU7IH1cXG4gIC5jb2wtbGctNSB7XFxuICAgIGZsZXg6IDAgMCA0MS42NjY2NyU7XFxuICAgIG1heC13aWR0aDogNDEuNjY2NjclOyB9XFxuICAuY29sLWxnLTYge1xcbiAgICBmbGV4OiAwIDAgNTAlO1xcbiAgICBtYXgtd2lkdGg6IDUwJTsgfVxcbiAgLmNvbC1sZy03IHtcXG4gICAgZmxleDogMCAwIDU4LjMzMzMzJTtcXG4gICAgbWF4LXdpZHRoOiA1OC4zMzMzMyU7IH1cXG4gIC5jb2wtbGctOCB7XFxuICAgIGZsZXg6IDAgMCA2Ni42NjY2NyU7XFxuICAgIG1heC13aWR0aDogNjYuNjY2NjclOyB9XFxuICAuY29sLWxnLTkge1xcbiAgICBmbGV4OiAwIDAgNzUlO1xcbiAgICBtYXgtd2lkdGg6IDc1JTsgfVxcbiAgLmNvbC1sZy0xMCB7XFxuICAgIGZsZXg6IDAgMCA4My4zMzMzMyU7XFxuICAgIG1heC13aWR0aDogODMuMzMzMzMlOyB9XFxuICAuY29sLWxnLTExIHtcXG4gICAgZmxleDogMCAwIDkxLjY2NjY3JTtcXG4gICAgbWF4LXdpZHRoOiA5MS42NjY2NyU7IH1cXG4gIC5jb2wtbGctMTIge1xcbiAgICBmbGV4OiAwIDAgMTAwJTtcXG4gICAgbWF4LXdpZHRoOiAxMDAlOyB9XFxuICAub3JkZXItbGctZmlyc3Qge1xcbiAgICBvcmRlcjogLTE7IH1cXG4gIC5vcmRlci1sZy1sYXN0IHtcXG4gICAgb3JkZXI6IDEzOyB9XFxuICAub3JkZXItbGctMCB7XFxuICAgIG9yZGVyOiAwOyB9XFxuICAub3JkZXItbGctMSB7XFxuICAgIG9yZGVyOiAxOyB9XFxuICAub3JkZXItbGctMiB7XFxuICAgIG9yZGVyOiAyOyB9XFxuICAub3JkZXItbGctMyB7XFxuICAgIG9yZGVyOiAzOyB9XFxuICAub3JkZXItbGctNCB7XFxuICAgIG9yZGVyOiA0OyB9XFxuICAub3JkZXItbGctNSB7XFxuICAgIG9yZGVyOiA1OyB9XFxuICAub3JkZXItbGctNiB7XFxuICAgIG9yZGVyOiA2OyB9XFxuICAub3JkZXItbGctNyB7XFxuICAgIG9yZGVyOiA3OyB9XFxuICAub3JkZXItbGctOCB7XFxuICAgIG9yZGVyOiA4OyB9XFxuICAub3JkZXItbGctOSB7XFxuICAgIG9yZGVyOiA5OyB9XFxuICAub3JkZXItbGctMTAge1xcbiAgICBvcmRlcjogMTA7IH1cXG4gIC5vcmRlci1sZy0xMSB7XFxuICAgIG9yZGVyOiAxMTsgfVxcbiAgLm9yZGVyLWxnLTEyIHtcXG4gICAgb3JkZXI6IDEyOyB9XFxuICAub2Zmc2V0LWxnLTAge1xcbiAgICBtYXJnaW4tbGVmdDogMDsgfVxcbiAgLm9mZnNldC1sZy0xIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDguMzMzMzMlOyB9XFxuICAub2Zmc2V0LWxnLTIge1xcbiAgICBtYXJnaW4tbGVmdDogMTYuNjY2NjclOyB9XFxuICAub2Zmc2V0LWxnLTMge1xcbiAgICBtYXJnaW4tbGVmdDogMjUlOyB9XFxuICAub2Zmc2V0LWxnLTQge1xcbiAgICBtYXJnaW4tbGVmdDogMzMuMzMzMzMlOyB9XFxuICAub2Zmc2V0LWxnLTUge1xcbiAgICBtYXJnaW4tbGVmdDogNDEuNjY2NjclOyB9XFxuICAub2Zmc2V0LWxnLTYge1xcbiAgICBtYXJnaW4tbGVmdDogNTAlOyB9XFxuICAub2Zmc2V0LWxnLTcge1xcbiAgICBtYXJnaW4tbGVmdDogNTguMzMzMzMlOyB9XFxuICAub2Zmc2V0LWxnLTgge1xcbiAgICBtYXJnaW4tbGVmdDogNjYuNjY2NjclOyB9XFxuICAub2Zmc2V0LWxnLTkge1xcbiAgICBtYXJnaW4tbGVmdDogNzUlOyB9XFxuICAub2Zmc2V0LWxnLTEwIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDgzLjMzMzMzJTsgfVxcbiAgLm9mZnNldC1sZy0xMSB7XFxuICAgIG1hcmdpbi1sZWZ0OiA5MS42NjY2NyU7IH0gfVxcblxcbkBtZWRpYSAobWluLXdpZHRoOiAxMjAwcHgpIHtcXG4gIC5jb2wteGwge1xcbiAgICBmbGV4LWJhc2lzOiAwO1xcbiAgICBmbGV4LWdyb3c6IDE7XFxuICAgIG1heC13aWR0aDogMTAwJTsgfVxcbiAgLmNvbC14bC1hdXRvIHtcXG4gICAgZmxleDogMCAwIGF1dG87XFxuICAgIHdpZHRoOiBhdXRvO1xcbiAgICBtYXgtd2lkdGg6IDEwMCU7IH1cXG4gIC5jb2wteGwtMSB7XFxuICAgIGZsZXg6IDAgMCA4LjMzMzMzJTtcXG4gICAgbWF4LXdpZHRoOiA4LjMzMzMzJTsgfVxcbiAgLmNvbC14bC0yIHtcXG4gICAgZmxleDogMCAwIDE2LjY2NjY3JTtcXG4gICAgbWF4LXdpZHRoOiAxNi42NjY2NyU7IH1cXG4gIC5jb2wteGwtMyB7XFxuICAgIGZsZXg6IDAgMCAyNSU7XFxuICAgIG1heC13aWR0aDogMjUlOyB9XFxuICAuY29sLXhsLTQge1xcbiAgICBmbGV4OiAwIDAgMzMuMzMzMzMlO1xcbiAgICBtYXgtd2lkdGg6IDMzLjMzMzMzJTsgfVxcbiAgLmNvbC14bC01IHtcXG4gICAgZmxleDogMCAwIDQxLjY2NjY3JTtcXG4gICAgbWF4LXdpZHRoOiA0MS42NjY2NyU7IH1cXG4gIC5jb2wteGwtNiB7XFxuICAgIGZsZXg6IDAgMCA1MCU7XFxuICAgIG1heC13aWR0aDogNTAlOyB9XFxuICAuY29sLXhsLTcge1xcbiAgICBmbGV4OiAwIDAgNTguMzMzMzMlO1xcbiAgICBtYXgtd2lkdGg6IDU4LjMzMzMzJTsgfVxcbiAgLmNvbC14bC04IHtcXG4gICAgZmxleDogMCAwIDY2LjY2NjY3JTtcXG4gICAgbWF4LXdpZHRoOiA2Ni42NjY2NyU7IH1cXG4gIC5jb2wteGwtOSB7XFxuICAgIGZsZXg6IDAgMCA3NSU7XFxuICAgIG1heC13aWR0aDogNzUlOyB9XFxuICAuY29sLXhsLTEwIHtcXG4gICAgZmxleDogMCAwIDgzLjMzMzMzJTtcXG4gICAgbWF4LXdpZHRoOiA4My4zMzMzMyU7IH1cXG4gIC5jb2wteGwtMTEge1xcbiAgICBmbGV4OiAwIDAgOTEuNjY2NjclO1xcbiAgICBtYXgtd2lkdGg6IDkxLjY2NjY3JTsgfVxcbiAgLmNvbC14bC0xMiB7XFxuICAgIGZsZXg6IDAgMCAxMDAlO1xcbiAgICBtYXgtd2lkdGg6IDEwMCU7IH1cXG4gIC5vcmRlci14bC1maXJzdCB7XFxuICAgIG9yZGVyOiAtMTsgfVxcbiAgLm9yZGVyLXhsLWxhc3Qge1xcbiAgICBvcmRlcjogMTM7IH1cXG4gIC5vcmRlci14bC0wIHtcXG4gICAgb3JkZXI6IDA7IH1cXG4gIC5vcmRlci14bC0xIHtcXG4gICAgb3JkZXI6IDE7IH1cXG4gIC5vcmRlci14bC0yIHtcXG4gICAgb3JkZXI6IDI7IH1cXG4gIC5vcmRlci14bC0zIHtcXG4gICAgb3JkZXI6IDM7IH1cXG4gIC5vcmRlci14bC00IHtcXG4gICAgb3JkZXI6IDQ7IH1cXG4gIC5vcmRlci14bC01IHtcXG4gICAgb3JkZXI6IDU7IH1cXG4gIC5vcmRlci14bC02IHtcXG4gICAgb3JkZXI6IDY7IH1cXG4gIC5vcmRlci14bC03IHtcXG4gICAgb3JkZXI6IDc7IH1cXG4gIC5vcmRlci14bC04IHtcXG4gICAgb3JkZXI6IDg7IH1cXG4gIC5vcmRlci14bC05IHtcXG4gICAgb3JkZXI6IDk7IH1cXG4gIC5vcmRlci14bC0xMCB7XFxuICAgIG9yZGVyOiAxMDsgfVxcbiAgLm9yZGVyLXhsLTExIHtcXG4gICAgb3JkZXI6IDExOyB9XFxuICAub3JkZXIteGwtMTIge1xcbiAgICBvcmRlcjogMTI7IH1cXG4gIC5vZmZzZXQteGwtMCB7XFxuICAgIG1hcmdpbi1sZWZ0OiAwOyB9XFxuICAub2Zmc2V0LXhsLTEge1xcbiAgICBtYXJnaW4tbGVmdDogOC4zMzMzMyU7IH1cXG4gIC5vZmZzZXQteGwtMiB7XFxuICAgIG1hcmdpbi1sZWZ0OiAxNi42NjY2NyU7IH1cXG4gIC5vZmZzZXQteGwtMyB7XFxuICAgIG1hcmdpbi1sZWZ0OiAyNSU7IH1cXG4gIC5vZmZzZXQteGwtNCB7XFxuICAgIG1hcmdpbi1sZWZ0OiAzMy4zMzMzMyU7IH1cXG4gIC5vZmZzZXQteGwtNSB7XFxuICAgIG1hcmdpbi1sZWZ0OiA0MS42NjY2NyU7IH1cXG4gIC5vZmZzZXQteGwtNiB7XFxuICAgIG1hcmdpbi1sZWZ0OiA1MCU7IH1cXG4gIC5vZmZzZXQteGwtNyB7XFxuICAgIG1hcmdpbi1sZWZ0OiA1OC4zMzMzMyU7IH1cXG4gIC5vZmZzZXQteGwtOCB7XFxuICAgIG1hcmdpbi1sZWZ0OiA2Ni42NjY2NyU7IH1cXG4gIC5vZmZzZXQteGwtOSB7XFxuICAgIG1hcmdpbi1sZWZ0OiA3NSU7IH1cXG4gIC5vZmZzZXQteGwtMTAge1xcbiAgICBtYXJnaW4tbGVmdDogODMuMzMzMzMlOyB9XFxuICAub2Zmc2V0LXhsLTExIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDkxLjY2NjY3JTsgfSB9XFxuXFxuaDEsIGgyLCBoMywgaDQsIGg1LCBoNixcXG4uaDEsIC5oMiwgLmgzLCAuaDQsIC5oNSwgLmg2IHtcXG4gIG1hcmdpbi1ib3R0b206IDAuNXJlbTtcXG4gIGZvbnQtd2VpZ2h0OiA1MDA7XFxuICBsaW5lLWhlaWdodDogMS4yOyB9XFxuXFxuaDEsIC5oMSB7XFxuICBmb250LXNpemU6IDIuNXJlbTsgfVxcblxcbmgyLCAuaDIge1xcbiAgZm9udC1zaXplOiAycmVtOyB9XFxuXFxuaDMsIC5oMyB7XFxuICBmb250LXNpemU6IDEuNzVyZW07IH1cXG5cXG5oNCwgLmg0IHtcXG4gIGZvbnQtc2l6ZTogMS41cmVtOyB9XFxuXFxuaDUsIC5oNSB7XFxuICBmb250LXNpemU6IDEuMjVyZW07IH1cXG5cXG5oNiwgLmg2IHtcXG4gIGZvbnQtc2l6ZTogMXJlbTsgfVxcblxcbi5sZWFkIHtcXG4gIGZvbnQtc2l6ZTogMS4yNXJlbTtcXG4gIGZvbnQtd2VpZ2h0OiAzMDA7IH1cXG5cXG4uZGlzcGxheS0xIHtcXG4gIGZvbnQtc2l6ZTogNnJlbTtcXG4gIGZvbnQtd2VpZ2h0OiAzMDA7XFxuICBsaW5lLWhlaWdodDogMS4yOyB9XFxuXFxuLmRpc3BsYXktMiB7XFxuICBmb250LXNpemU6IDUuNXJlbTtcXG4gIGZvbnQtd2VpZ2h0OiAzMDA7XFxuICBsaW5lLWhlaWdodDogMS4yOyB9XFxuXFxuLmRpc3BsYXktMyB7XFxuICBmb250LXNpemU6IDQuNXJlbTtcXG4gIGZvbnQtd2VpZ2h0OiAzMDA7XFxuICBsaW5lLWhlaWdodDogMS4yOyB9XFxuXFxuLmRpc3BsYXktNCB7XFxuICBmb250LXNpemU6IDMuNXJlbTtcXG4gIGZvbnQtd2VpZ2h0OiAzMDA7XFxuICBsaW5lLWhlaWdodDogMS4yOyB9XFxuXFxuaHIge1xcbiAgbWFyZ2luLXRvcDogMXJlbTtcXG4gIG1hcmdpbi1ib3R0b206IDFyZW07XFxuICBib3JkZXI6IDA7XFxuICBib3JkZXItdG9wOiAxcHggc29saWQgcmdiYSgwLCAwLCAwLCAwLjEpOyB9XFxuXFxuc21hbGwsXFxuLnNtYWxsIHtcXG4gIGZvbnQtc2l6ZTogODAlO1xcbiAgZm9udC13ZWlnaHQ6IDQwMDsgfVxcblxcbm1hcmssXFxuLm1hcmsge1xcbiAgcGFkZGluZzogMC4yZW07XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmNmOGUzOyB9XFxuXFxuLmxpc3QtdW5zdHlsZWQge1xcbiAgcGFkZGluZy1sZWZ0OiAwO1xcbiAgbGlzdC1zdHlsZTogbm9uZTsgfVxcblxcbi5saXN0LWlubGluZSB7XFxuICBwYWRkaW5nLWxlZnQ6IDA7XFxuICBsaXN0LXN0eWxlOiBub25lOyB9XFxuXFxuLmxpc3QtaW5saW5lLWl0ZW0ge1xcbiAgZGlzcGxheTogaW5saW5lLWJsb2NrOyB9XFxuICAubGlzdC1pbmxpbmUtaXRlbTpub3QoOmxhc3QtY2hpbGQpIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAwLjVyZW07IH1cXG5cXG4uaW5pdGlhbGlzbSB7XFxuICBmb250LXNpemU6IDkwJTtcXG4gIHRleHQtdHJhbnNmb3JtOiB1cHBlcmNhc2U7IH1cXG5cXG4uYmxvY2txdW90ZSB7XFxuICBtYXJnaW4tYm90dG9tOiAxcmVtO1xcbiAgZm9udC1zaXplOiAxLjI1cmVtOyB9XFxuXFxuLmJsb2NrcXVvdGUtZm9vdGVyIHtcXG4gIGRpc3BsYXk6IGJsb2NrO1xcbiAgZm9udC1zaXplOiA4MCU7XFxuICBjb2xvcjogIzZjNzU3ZDsgfVxcbiAgLmJsb2NrcXVvdGUtZm9vdGVyOjpiZWZvcmUge1xcbiAgICBjb250ZW50OiBcXFwiXFxcXDIwMTRcXFxcMDBBMFxcXCI7IH1cXG5cXG4uYnJlYWRjcnVtYiB7XFxuICBkaXNwbGF5OiBmbGV4O1xcbiAgZmxleC13cmFwOiB3cmFwO1xcbiAgcGFkZGluZzogMC43NXJlbSAxcmVtO1xcbiAgbWFyZ2luLWJvdHRvbTogMXJlbTtcXG4gIGxpc3Qtc3R5bGU6IG5vbmU7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjZTllY2VmO1xcbiAgYm9yZGVyLXJhZGl1czogMC4yNXJlbTsgfVxcblxcbi5icmVhZGNydW1iLWl0ZW0gKyAuYnJlYWRjcnVtYi1pdGVtIHtcXG4gIHBhZGRpbmctbGVmdDogMC41cmVtOyB9XFxuICAuYnJlYWRjcnVtYi1pdGVtICsgLmJyZWFkY3J1bWItaXRlbTo6YmVmb3JlIHtcXG4gICAgZGlzcGxheTogaW5saW5lLWJsb2NrO1xcbiAgICBwYWRkaW5nLXJpZ2h0OiAwLjVyZW07XFxuICAgIGNvbG9yOiAjNmM3NTdkO1xcbiAgICBjb250ZW50OiBcXFwiL1xcXCI7IH1cXG5cXG4uYnJlYWRjcnVtYi1pdGVtICsgLmJyZWFkY3J1bWItaXRlbTpob3Zlcjo6YmVmb3JlIHtcXG4gIHRleHQtZGVjb3JhdGlvbjogdW5kZXJsaW5lOyB9XFxuXFxuLmJyZWFkY3J1bWItaXRlbSArIC5icmVhZGNydW1iLWl0ZW06aG92ZXI6OmJlZm9yZSB7XFxuICB0ZXh0LWRlY29yYXRpb246IG5vbmU7IH1cXG5cXG4uYnJlYWRjcnVtYi1pdGVtLmFjdGl2ZSB7XFxuICBjb2xvcjogIzZjNzU3ZDsgfVxcblxcbi50YWJsZSB7XFxuICB3aWR0aDogMTAwJTtcXG4gIG1hcmdpbi1ib3R0b206IDFyZW07XFxuICBjb2xvcjogIzIxMjUyOTsgfVxcbiAgLnRhYmxlIHRoLFxcbiAgLnRhYmxlIHRkIHtcXG4gICAgcGFkZGluZzogMC43NXJlbTtcXG4gICAgdmVydGljYWwtYWxpZ246IHRvcDtcXG4gICAgYm9yZGVyLXRvcDogMXB4IHNvbGlkICNkZWUyZTY7IH1cXG4gIC50YWJsZSB0aGVhZCB0aCB7XFxuICAgIHZlcnRpY2FsLWFsaWduOiBib3R0b207XFxuICAgIGJvcmRlci1ib3R0b206IDJweCBzb2xpZCAjZGVlMmU2OyB9XFxuICAudGFibGUgdGJvZHkgKyB0Ym9keSB7XFxuICAgIGJvcmRlci10b3A6IDJweCBzb2xpZCAjZGVlMmU2OyB9XFxuXFxuLnRhYmxlLXNtIHRoLFxcbi50YWJsZS1zbSB0ZCB7XFxuICBwYWRkaW5nOiAwLjNyZW07IH1cXG5cXG4udGFibGUtYm9yZGVyZWQge1xcbiAgYm9yZGVyOiAxcHggc29saWQgI2RlZTJlNjsgfVxcbiAgLnRhYmxlLWJvcmRlcmVkIHRoLFxcbiAgLnRhYmxlLWJvcmRlcmVkIHRkIHtcXG4gICAgYm9yZGVyOiAxcHggc29saWQgI2RlZTJlNjsgfVxcbiAgLnRhYmxlLWJvcmRlcmVkIHRoZWFkIHRoLFxcbiAgLnRhYmxlLWJvcmRlcmVkIHRoZWFkIHRkIHtcXG4gICAgYm9yZGVyLWJvdHRvbS13aWR0aDogMnB4OyB9XFxuXFxuLnRhYmxlLWJvcmRlcmxlc3MgdGgsXFxuLnRhYmxlLWJvcmRlcmxlc3MgdGQsXFxuLnRhYmxlLWJvcmRlcmxlc3MgdGhlYWQgdGgsXFxuLnRhYmxlLWJvcmRlcmxlc3MgdGJvZHkgKyB0Ym9keSB7XFxuICBib3JkZXI6IDA7IH1cXG5cXG4udGFibGUtc3RyaXBlZCB0Ym9keSB0cjpudGgtb2YtdHlwZShvZGQpIHtcXG4gIGJhY2tncm91bmQtY29sb3I6IHJnYmEoMCwgMCwgMCwgMC4wNSk7IH1cXG5cXG4udGFibGUtaG92ZXIgdGJvZHkgdHI6aG92ZXIge1xcbiAgY29sb3I6ICMyMTI1Mjk7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiByZ2JhKDAsIDAsIDAsIDAuMDc1KTsgfVxcblxcbi50YWJsZS1wcmltYXJ5LFxcbi50YWJsZS1wcmltYXJ5ID4gdGgsXFxuLnRhYmxlLXByaW1hcnkgPiB0ZCB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjYjhkYWZmOyB9XFxuXFxuLnRhYmxlLXByaW1hcnkgdGgsXFxuLnRhYmxlLXByaW1hcnkgdGQsXFxuLnRhYmxlLXByaW1hcnkgdGhlYWQgdGgsXFxuLnRhYmxlLXByaW1hcnkgdGJvZHkgKyB0Ym9keSB7XFxuICBib3JkZXItY29sb3I6ICM3YWJhZmY7IH1cXG5cXG4udGFibGUtaG92ZXIgLnRhYmxlLXByaW1hcnk6aG92ZXIge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogIzlmY2RmZjsgfVxcbiAgLnRhYmxlLWhvdmVyIC50YWJsZS1wcmltYXJ5OmhvdmVyID4gdGQsXFxuICAudGFibGUtaG92ZXIgLnRhYmxlLXByaW1hcnk6aG92ZXIgPiB0aCB7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICM5ZmNkZmY7IH1cXG5cXG4udGFibGUtc2Vjb25kYXJ5LFxcbi50YWJsZS1zZWNvbmRhcnkgPiB0aCxcXG4udGFibGUtc2Vjb25kYXJ5ID4gdGQge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2Q2ZDhkYjsgfVxcblxcbi50YWJsZS1zZWNvbmRhcnkgdGgsXFxuLnRhYmxlLXNlY29uZGFyeSB0ZCxcXG4udGFibGUtc2Vjb25kYXJ5IHRoZWFkIHRoLFxcbi50YWJsZS1zZWNvbmRhcnkgdGJvZHkgKyB0Ym9keSB7XFxuICBib3JkZXItY29sb3I6ICNiM2I3YmI7IH1cXG5cXG4udGFibGUtaG92ZXIgLnRhYmxlLXNlY29uZGFyeTpob3ZlciB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjYzhjYmNmOyB9XFxuICAudGFibGUtaG92ZXIgLnRhYmxlLXNlY29uZGFyeTpob3ZlciA+IHRkLFxcbiAgLnRhYmxlLWhvdmVyIC50YWJsZS1zZWNvbmRhcnk6aG92ZXIgPiB0aCB7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNjOGNiY2Y7IH1cXG5cXG4udGFibGUtc3VjY2VzcyxcXG4udGFibGUtc3VjY2VzcyA+IHRoLFxcbi50YWJsZS1zdWNjZXNzID4gdGQge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2MzZTZjYjsgfVxcblxcbi50YWJsZS1zdWNjZXNzIHRoLFxcbi50YWJsZS1zdWNjZXNzIHRkLFxcbi50YWJsZS1zdWNjZXNzIHRoZWFkIHRoLFxcbi50YWJsZS1zdWNjZXNzIHRib2R5ICsgdGJvZHkge1xcbiAgYm9yZGVyLWNvbG9yOiAjOGZkMTllOyB9XFxuXFxuLnRhYmxlLWhvdmVyIC50YWJsZS1zdWNjZXNzOmhvdmVyIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNiMWRmYmI7IH1cXG4gIC50YWJsZS1ob3ZlciAudGFibGUtc3VjY2Vzczpob3ZlciA+IHRkLFxcbiAgLnRhYmxlLWhvdmVyIC50YWJsZS1zdWNjZXNzOmhvdmVyID4gdGgge1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjYjFkZmJiOyB9XFxuXFxuLnRhYmxlLWluZm8sXFxuLnRhYmxlLWluZm8gPiB0aCxcXG4udGFibGUtaW5mbyA+IHRkIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNiZWU1ZWI7IH1cXG5cXG4udGFibGUtaW5mbyB0aCxcXG4udGFibGUtaW5mbyB0ZCxcXG4udGFibGUtaW5mbyB0aGVhZCB0aCxcXG4udGFibGUtaW5mbyB0Ym9keSArIHRib2R5IHtcXG4gIGJvcmRlci1jb2xvcjogIzg2Y2ZkYTsgfVxcblxcbi50YWJsZS1ob3ZlciAudGFibGUtaW5mbzpob3ZlciB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjYWJkZGU1OyB9XFxuICAudGFibGUtaG92ZXIgLnRhYmxlLWluZm86aG92ZXIgPiB0ZCxcXG4gIC50YWJsZS1ob3ZlciAudGFibGUtaW5mbzpob3ZlciA+IHRoIHtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2FiZGRlNTsgfVxcblxcbi50YWJsZS13YXJuaW5nLFxcbi50YWJsZS13YXJuaW5nID4gdGgsXFxuLnRhYmxlLXdhcm5pbmcgPiB0ZCB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmZlZWJhOyB9XFxuXFxuLnRhYmxlLXdhcm5pbmcgdGgsXFxuLnRhYmxlLXdhcm5pbmcgdGQsXFxuLnRhYmxlLXdhcm5pbmcgdGhlYWQgdGgsXFxuLnRhYmxlLXdhcm5pbmcgdGJvZHkgKyB0Ym9keSB7XFxuICBib3JkZXItY29sb3I6ICNmZmRmN2U7IH1cXG5cXG4udGFibGUtaG92ZXIgLnRhYmxlLXdhcm5pbmc6aG92ZXIge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2ZmZThhMTsgfVxcbiAgLnRhYmxlLWhvdmVyIC50YWJsZS13YXJuaW5nOmhvdmVyID4gdGQsXFxuICAudGFibGUtaG92ZXIgLnRhYmxlLXdhcm5pbmc6aG92ZXIgPiB0aCB7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNmZmU4YTE7IH1cXG5cXG4udGFibGUtZGFuZ2VyLFxcbi50YWJsZS1kYW5nZXIgPiB0aCxcXG4udGFibGUtZGFuZ2VyID4gdGQge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2Y1YzZjYjsgfVxcblxcbi50YWJsZS1kYW5nZXIgdGgsXFxuLnRhYmxlLWRhbmdlciB0ZCxcXG4udGFibGUtZGFuZ2VyIHRoZWFkIHRoLFxcbi50YWJsZS1kYW5nZXIgdGJvZHkgKyB0Ym9keSB7XFxuICBib3JkZXItY29sb3I6ICNlZDk2OWU7IH1cXG5cXG4udGFibGUtaG92ZXIgLnRhYmxlLWRhbmdlcjpob3ZlciB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjZjFiMGI3OyB9XFxuICAudGFibGUtaG92ZXIgLnRhYmxlLWRhbmdlcjpob3ZlciA+IHRkLFxcbiAgLnRhYmxlLWhvdmVyIC50YWJsZS1kYW5nZXI6aG92ZXIgPiB0aCB7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNmMWIwYjc7IH1cXG5cXG4udGFibGUtbGlnaHQsXFxuLnRhYmxlLWxpZ2h0ID4gdGgsXFxuLnRhYmxlLWxpZ2h0ID4gdGQge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2ZkZmRmZTsgfVxcblxcbi50YWJsZS1saWdodCB0aCxcXG4udGFibGUtbGlnaHQgdGQsXFxuLnRhYmxlLWxpZ2h0IHRoZWFkIHRoLFxcbi50YWJsZS1saWdodCB0Ym9keSArIHRib2R5IHtcXG4gIGJvcmRlci1jb2xvcjogI2ZiZmNmYzsgfVxcblxcbi50YWJsZS1ob3ZlciAudGFibGUtbGlnaHQ6aG92ZXIge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2VjZWNmNjsgfVxcbiAgLnRhYmxlLWhvdmVyIC50YWJsZS1saWdodDpob3ZlciA+IHRkLFxcbiAgLnRhYmxlLWhvdmVyIC50YWJsZS1saWdodDpob3ZlciA+IHRoIHtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2VjZWNmNjsgfVxcblxcbi50YWJsZS1kYXJrLFxcbi50YWJsZS1kYXJrID4gdGgsXFxuLnRhYmxlLWRhcmsgPiB0ZCB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjYzZjOGNhOyB9XFxuXFxuLnRhYmxlLWRhcmsgdGgsXFxuLnRhYmxlLWRhcmsgdGQsXFxuLnRhYmxlLWRhcmsgdGhlYWQgdGgsXFxuLnRhYmxlLWRhcmsgdGJvZHkgKyB0Ym9keSB7XFxuICBib3JkZXItY29sb3I6ICM5NTk5OWM7IH1cXG5cXG4udGFibGUtaG92ZXIgLnRhYmxlLWRhcms6aG92ZXIge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2I5YmJiZTsgfVxcbiAgLnRhYmxlLWhvdmVyIC50YWJsZS1kYXJrOmhvdmVyID4gdGQsXFxuICAudGFibGUtaG92ZXIgLnRhYmxlLWRhcms6aG92ZXIgPiB0aCB7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNiOWJiYmU7IH1cXG5cXG4udGFibGUtYWN0aXZlLFxcbi50YWJsZS1hY3RpdmUgPiB0aCxcXG4udGFibGUtYWN0aXZlID4gdGQge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogcmdiYSgwLCAwLCAwLCAwLjA3NSk7IH1cXG5cXG4udGFibGUtaG92ZXIgLnRhYmxlLWFjdGl2ZTpob3ZlciB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiByZ2JhKDAsIDAsIDAsIDAuMDc1KTsgfVxcbiAgLnRhYmxlLWhvdmVyIC50YWJsZS1hY3RpdmU6aG92ZXIgPiB0ZCxcXG4gIC50YWJsZS1ob3ZlciAudGFibGUtYWN0aXZlOmhvdmVyID4gdGgge1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiByZ2JhKDAsIDAsIDAsIDAuMDc1KTsgfVxcblxcbi50YWJsZSAudGhlYWQtZGFyayB0aCB7XFxuICBjb2xvcjogI2ZmZjtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMzNDNhNDA7XFxuICBib3JkZXItY29sb3I6ICM0NTRkNTU7IH1cXG5cXG4udGFibGUgLnRoZWFkLWxpZ2h0IHRoIHtcXG4gIGNvbG9yOiAjNDk1MDU3O1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2U5ZWNlZjtcXG4gIGJvcmRlci1jb2xvcjogI2RlZTJlNjsgfVxcblxcbi50YWJsZS1kYXJrIHtcXG4gIGNvbG9yOiAjZmZmO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogIzM0M2E0MDsgfVxcbiAgLnRhYmxlLWRhcmsgdGgsXFxuICAudGFibGUtZGFyayB0ZCxcXG4gIC50YWJsZS1kYXJrIHRoZWFkIHRoIHtcXG4gICAgYm9yZGVyLWNvbG9yOiAjNDU0ZDU1OyB9XFxuICAudGFibGUtZGFyay50YWJsZS1ib3JkZXJlZCB7XFxuICAgIGJvcmRlcjogMDsgfVxcbiAgLnRhYmxlLWRhcmsudGFibGUtc3RyaXBlZCB0Ym9keSB0cjpudGgtb2YtdHlwZShvZGQpIHtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogcmdiYSgyNTUsIDI1NSwgMjU1LCAwLjA1KTsgfVxcbiAgLnRhYmxlLWRhcmsudGFibGUtaG92ZXIgdGJvZHkgdHI6aG92ZXIge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogcmdiYSgyNTUsIDI1NSwgMjU1LCAwLjA3NSk7IH1cXG5cXG5AbWVkaWEgKG1heC13aWR0aDogNTc1Ljk4cHgpIHtcXG4gIC50YWJsZS1yZXNwb25zaXZlLXNtIHtcXG4gICAgZGlzcGxheTogYmxvY2s7XFxuICAgIHdpZHRoOiAxMDAlO1xcbiAgICBvdmVyZmxvdy14OiBhdXRvO1xcbiAgICAtd2Via2l0LW92ZXJmbG93LXNjcm9sbGluZzogdG91Y2g7IH1cXG4gICAgLnRhYmxlLXJlc3BvbnNpdmUtc20gPiAudGFibGUtYm9yZGVyZWQge1xcbiAgICAgIGJvcmRlcjogMDsgfSB9XFxuXFxuQG1lZGlhIChtYXgtd2lkdGg6IDc2Ny45OHB4KSB7XFxuICAudGFibGUtcmVzcG9uc2l2ZS1tZCB7XFxuICAgIGRpc3BsYXk6IGJsb2NrO1xcbiAgICB3aWR0aDogMTAwJTtcXG4gICAgb3ZlcmZsb3cteDogYXV0bztcXG4gICAgLXdlYmtpdC1vdmVyZmxvdy1zY3JvbGxpbmc6IHRvdWNoOyB9XFxuICAgIC50YWJsZS1yZXNwb25zaXZlLW1kID4gLnRhYmxlLWJvcmRlcmVkIHtcXG4gICAgICBib3JkZXI6IDA7IH0gfVxcblxcbkBtZWRpYSAobWF4LXdpZHRoOiA5OTEuOThweCkge1xcbiAgLnRhYmxlLXJlc3BvbnNpdmUtbGcge1xcbiAgICBkaXNwbGF5OiBibG9jaztcXG4gICAgd2lkdGg6IDEwMCU7XFxuICAgIG92ZXJmbG93LXg6IGF1dG87XFxuICAgIC13ZWJraXQtb3ZlcmZsb3ctc2Nyb2xsaW5nOiB0b3VjaDsgfVxcbiAgICAudGFibGUtcmVzcG9uc2l2ZS1sZyA+IC50YWJsZS1ib3JkZXJlZCB7XFxuICAgICAgYm9yZGVyOiAwOyB9IH1cXG5cXG5AbWVkaWEgKG1heC13aWR0aDogMTE5OS45OHB4KSB7XFxuICAudGFibGUtcmVzcG9uc2l2ZS14bCB7XFxuICAgIGRpc3BsYXk6IGJsb2NrO1xcbiAgICB3aWR0aDogMTAwJTtcXG4gICAgb3ZlcmZsb3cteDogYXV0bztcXG4gICAgLXdlYmtpdC1vdmVyZmxvdy1zY3JvbGxpbmc6IHRvdWNoOyB9XFxuICAgIC50YWJsZS1yZXNwb25zaXZlLXhsID4gLnRhYmxlLWJvcmRlcmVkIHtcXG4gICAgICBib3JkZXI6IDA7IH0gfVxcblxcbi50YWJsZS1yZXNwb25zaXZlIHtcXG4gIGRpc3BsYXk6IGJsb2NrO1xcbiAgd2lkdGg6IDEwMCU7XFxuICBvdmVyZmxvdy14OiBhdXRvO1xcbiAgLXdlYmtpdC1vdmVyZmxvdy1zY3JvbGxpbmc6IHRvdWNoOyB9XFxuICAudGFibGUtcmVzcG9uc2l2ZSA+IC50YWJsZS1ib3JkZXJlZCB7XFxuICAgIGJvcmRlcjogMDsgfVxcblxcbi5iYWRnZSB7XFxuICBkaXNwbGF5OiBpbmxpbmUtYmxvY2s7XFxuICBwYWRkaW5nOiAwLjI1ZW0gMC40ZW07XFxuICBmb250LXNpemU6IDc1JTtcXG4gIGZvbnQtd2VpZ2h0OiA3MDA7XFxuICBsaW5lLWhlaWdodDogMTtcXG4gIHRleHQtYWxpZ246IGNlbnRlcjtcXG4gIHdoaXRlLXNwYWNlOiBub3dyYXA7XFxuICB2ZXJ0aWNhbC1hbGlnbjogYmFzZWxpbmU7XFxuICBib3JkZXItcmFkaXVzOiAwLjI1cmVtO1xcbiAgdHJhbnNpdGlvbjogY29sb3IgMC4xNXMgZWFzZS1pbi1vdXQsIGJhY2tncm91bmQtY29sb3IgMC4xNXMgZWFzZS1pbi1vdXQsIGJvcmRlci1jb2xvciAwLjE1cyBlYXNlLWluLW91dCwgYm94LXNoYWRvdyAwLjE1cyBlYXNlLWluLW91dDsgfVxcbiAgQG1lZGlhIChwcmVmZXJzLXJlZHVjZWQtbW90aW9uOiByZWR1Y2UpIHtcXG4gICAgLmJhZGdlIHtcXG4gICAgICB0cmFuc2l0aW9uOiBub25lOyB9IH1cXG4gIGEuYmFkZ2U6aG92ZXIsIGEuYmFkZ2U6Zm9jdXMge1xcbiAgICB0ZXh0LWRlY29yYXRpb246IG5vbmU7IH1cXG4gIC5iYWRnZTplbXB0eSB7XFxuICAgIGRpc3BsYXk6IG5vbmU7IH1cXG5cXG4uYnRuIC5iYWRnZSB7XFxuICBwb3NpdGlvbjogcmVsYXRpdmU7XFxuICB0b3A6IC0xcHg7IH1cXG5cXG4uYmFkZ2UtcGlsbCB7XFxuICBwYWRkaW5nLXJpZ2h0OiAwLjZlbTtcXG4gIHBhZGRpbmctbGVmdDogMC42ZW07XFxuICBib3JkZXItcmFkaXVzOiAxMHJlbTsgfVxcblxcbi5iYWRnZS1wcmltYXJ5IHtcXG4gIGNvbG9yOiAjZmZmO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogIzAwN2JmZjsgfVxcbiAgYS5iYWRnZS1wcmltYXJ5OmhvdmVyLCBhLmJhZGdlLXByaW1hcnk6Zm9jdXMge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzAwNjJjYzsgfVxcbiAgYS5iYWRnZS1wcmltYXJ5OmZvY3VzLCBhLmJhZGdlLXByaW1hcnkuZm9jdXMge1xcbiAgICBvdXRsaW5lOiAwO1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgwLCAxMjMsIDI1NSwgMC41KTsgfVxcblxcbi5iYWRnZS1zZWNvbmRhcnkge1xcbiAgY29sb3I6ICNmZmY7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjNmM3NTdkOyB9XFxuICBhLmJhZGdlLXNlY29uZGFyeTpob3ZlciwgYS5iYWRnZS1zZWNvbmRhcnk6Zm9jdXMge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzU0NWI2MjsgfVxcbiAgYS5iYWRnZS1zZWNvbmRhcnk6Zm9jdXMsIGEuYmFkZ2Utc2Vjb25kYXJ5LmZvY3VzIHtcXG4gICAgb3V0bGluZTogMDtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMTA4LCAxMTcsIDEyNSwgMC41KTsgfVxcblxcbi5iYWRnZS1zdWNjZXNzIHtcXG4gIGNvbG9yOiAjZmZmO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogIzI4YTc0NTsgfVxcbiAgYS5iYWRnZS1zdWNjZXNzOmhvdmVyLCBhLmJhZGdlLXN1Y2Nlc3M6Zm9jdXMge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzFlN2UzNDsgfVxcbiAgYS5iYWRnZS1zdWNjZXNzOmZvY3VzLCBhLmJhZGdlLXN1Y2Nlc3MuZm9jdXMge1xcbiAgICBvdXRsaW5lOiAwO1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSg0MCwgMTY3LCA2OSwgMC41KTsgfVxcblxcbi5iYWRnZS1pbmZvIHtcXG4gIGNvbG9yOiAjZmZmO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogIzE3YTJiODsgfVxcbiAgYS5iYWRnZS1pbmZvOmhvdmVyLCBhLmJhZGdlLWluZm86Zm9jdXMge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzExN2E4YjsgfVxcbiAgYS5iYWRnZS1pbmZvOmZvY3VzLCBhLmJhZGdlLWluZm8uZm9jdXMge1xcbiAgICBvdXRsaW5lOiAwO1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyMywgMTYyLCAxODQsIDAuNSk7IH1cXG5cXG4uYmFkZ2Utd2FybmluZyB7XFxuICBjb2xvcjogIzIxMjUyOTtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNmZmMxMDc7IH1cXG4gIGEuYmFkZ2Utd2FybmluZzpob3ZlciwgYS5iYWRnZS13YXJuaW5nOmZvY3VzIHtcXG4gICAgY29sb3I6ICMyMTI1Mjk7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNkMzllMDA7IH1cXG4gIGEuYmFkZ2Utd2FybmluZzpmb2N1cywgYS5iYWRnZS13YXJuaW5nLmZvY3VzIHtcXG4gICAgb3V0bGluZTogMDtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjU1LCAxOTMsIDcsIDAuNSk7IH1cXG5cXG4uYmFkZ2UtZGFuZ2VyIHtcXG4gIGNvbG9yOiAjZmZmO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2RjMzU0NTsgfVxcbiAgYS5iYWRnZS1kYW5nZXI6aG92ZXIsIGEuYmFkZ2UtZGFuZ2VyOmZvY3VzIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNiZDIxMzA7IH1cXG4gIGEuYmFkZ2UtZGFuZ2VyOmZvY3VzLCBhLmJhZGdlLWRhbmdlci5mb2N1cyB7XFxuICAgIG91dGxpbmU6IDA7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDIyMCwgNTMsIDY5LCAwLjUpOyB9XFxuXFxuLmJhZGdlLWxpZ2h0IHtcXG4gIGNvbG9yOiAjMjEyNTI5O1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2Y4ZjlmYTsgfVxcbiAgYS5iYWRnZS1saWdodDpob3ZlciwgYS5iYWRnZS1saWdodDpmb2N1cyB7XFxuICAgIGNvbG9yOiAjMjEyNTI5O1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZGFlMGU1OyB9XFxuICBhLmJhZGdlLWxpZ2h0OmZvY3VzLCBhLmJhZGdlLWxpZ2h0LmZvY3VzIHtcXG4gICAgb3V0bGluZTogMDtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjQ4LCAyNDksIDI1MCwgMC41KTsgfVxcblxcbi5iYWRnZS1kYXJrIHtcXG4gIGNvbG9yOiAjZmZmO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogIzM0M2E0MDsgfVxcbiAgYS5iYWRnZS1kYXJrOmhvdmVyLCBhLmJhZGdlLWRhcms6Zm9jdXMge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzFkMjEyNDsgfVxcbiAgYS5iYWRnZS1kYXJrOmZvY3VzLCBhLmJhZGdlLWRhcmsuZm9jdXMge1xcbiAgICBvdXRsaW5lOiAwO1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSg1MiwgNTgsIDY0LCAwLjUpOyB9XFxuXFxuLmJ0biB7XFxuICBkaXNwbGF5OiBpbmxpbmUtYmxvY2s7XFxuICBmb250LXdlaWdodDogNDAwO1xcbiAgY29sb3I6ICMyMTI1Mjk7XFxuICB0ZXh0LWFsaWduOiBjZW50ZXI7XFxuICB2ZXJ0aWNhbC1hbGlnbjogbWlkZGxlO1xcbiAgdXNlci1zZWxlY3Q6IG5vbmU7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiB0cmFuc3BhcmVudDtcXG4gIGJvcmRlcjogMXB4IHNvbGlkIHRyYW5zcGFyZW50O1xcbiAgcGFkZGluZzogMC4zNzVyZW0gMC43NXJlbTtcXG4gIGZvbnQtc2l6ZTogMXJlbTtcXG4gIGxpbmUtaGVpZ2h0OiAxLjU7XFxuICBib3JkZXItcmFkaXVzOiAwLjI1cmVtO1xcbiAgdHJhbnNpdGlvbjogY29sb3IgMC4xNXMgZWFzZS1pbi1vdXQsIGJhY2tncm91bmQtY29sb3IgMC4xNXMgZWFzZS1pbi1vdXQsIGJvcmRlci1jb2xvciAwLjE1cyBlYXNlLWluLW91dCwgYm94LXNoYWRvdyAwLjE1cyBlYXNlLWluLW91dDsgfVxcbiAgQG1lZGlhIChwcmVmZXJzLXJlZHVjZWQtbW90aW9uOiByZWR1Y2UpIHtcXG4gICAgLmJ0biB7XFxuICAgICAgdHJhbnNpdGlvbjogbm9uZTsgfSB9XFxuICAuYnRuOmhvdmVyIHtcXG4gICAgY29sb3I6ICMyMTI1Mjk7XFxuICAgIHRleHQtZGVjb3JhdGlvbjogbm9uZTsgfVxcbiAgLmJ0bjpmb2N1cywgLmJ0bi5mb2N1cyB7XFxuICAgIG91dGxpbmU6IDA7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDAsIDEyMywgMjU1LCAwLjI1KTsgfVxcbiAgLmJ0bi5kaXNhYmxlZCwgLmJ0bjpkaXNhYmxlZCB7XFxuICAgIG9wYWNpdHk6IDAuNjU7IH1cXG5cXG5hLmJ0bi5kaXNhYmxlZCxcXG5maWVsZHNldDpkaXNhYmxlZCBhLmJ0biB7XFxuICBwb2ludGVyLWV2ZW50czogbm9uZTsgfVxcblxcbi5idG4tcHJpbWFyeSB7XFxuICBjb2xvcjogI2ZmZjtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMwMDdiZmY7XFxuICBib3JkZXItY29sb3I6ICMwMDdiZmY7IH1cXG4gIC5idG4tcHJpbWFyeTpob3ZlciB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjMDA2OWQ5O1xcbiAgICBib3JkZXItY29sb3I6ICMwMDYyY2M7IH1cXG4gIC5idG4tcHJpbWFyeTpmb2N1cywgLmJ0bi1wcmltYXJ5LmZvY3VzIHtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMzgsIDE0MywgMjU1LCAwLjUpOyB9XFxuICAuYnRuLXByaW1hcnkuZGlzYWJsZWQsIC5idG4tcHJpbWFyeTpkaXNhYmxlZCB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjMDA3YmZmO1xcbiAgICBib3JkZXItY29sb3I6ICMwMDdiZmY7IH1cXG4gIC5idG4tcHJpbWFyeTpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmUsIC5idG4tcHJpbWFyeTpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmUsXFxuICAuc2hvdyA+IC5idG4tcHJpbWFyeS5kcm9wZG93bi10b2dnbGUge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzAwNjJjYztcXG4gICAgYm9yZGVyLWNvbG9yOiAjMDA1Y2JmOyB9XFxuICAgIC5idG4tcHJpbWFyeTpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmU6Zm9jdXMsIC5idG4tcHJpbWFyeTpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmU6Zm9jdXMsXFxuICAgIC5zaG93ID4gLmJ0bi1wcmltYXJ5LmRyb3Bkb3duLXRvZ2dsZTpmb2N1cyB7XFxuICAgICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMzgsIDE0MywgMjU1LCAwLjUpOyB9XFxuXFxuLmJ0bi1zZWNvbmRhcnkge1xcbiAgY29sb3I6ICNmZmY7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjNmM3NTdkO1xcbiAgYm9yZGVyLWNvbG9yOiAjNmM3NTdkOyB9XFxuICAuYnRuLXNlY29uZGFyeTpob3ZlciB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjNWE2MjY4O1xcbiAgICBib3JkZXItY29sb3I6ICM1NDViNjI7IH1cXG4gIC5idG4tc2Vjb25kYXJ5OmZvY3VzLCAuYnRuLXNlY29uZGFyeS5mb2N1cyB7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDEzMCwgMTM4LCAxNDUsIDAuNSk7IH1cXG4gIC5idG4tc2Vjb25kYXJ5LmRpc2FibGVkLCAuYnRuLXNlY29uZGFyeTpkaXNhYmxlZCB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjNmM3NTdkO1xcbiAgICBib3JkZXItY29sb3I6ICM2Yzc1N2Q7IH1cXG4gIC5idG4tc2Vjb25kYXJ5Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZSwgLmJ0bi1zZWNvbmRhcnk6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlLFxcbiAgLnNob3cgPiAuYnRuLXNlY29uZGFyeS5kcm9wZG93bi10b2dnbGUge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzU0NWI2MjtcXG4gICAgYm9yZGVyLWNvbG9yOiAjNGU1NTViOyB9XFxuICAgIC5idG4tc2Vjb25kYXJ5Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZTpmb2N1cywgLmJ0bi1zZWNvbmRhcnk6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlOmZvY3VzLFxcbiAgICAuc2hvdyA+IC5idG4tc2Vjb25kYXJ5LmRyb3Bkb3duLXRvZ2dsZTpmb2N1cyB7XFxuICAgICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMTMwLCAxMzgsIDE0NSwgMC41KTsgfVxcblxcbi5idG4tc3VjY2VzcyB7XFxuICBjb2xvcjogI2ZmZjtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMyOGE3NDU7XFxuICBib3JkZXItY29sb3I6ICMyOGE3NDU7IH1cXG4gIC5idG4tc3VjY2Vzczpob3ZlciB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjMjE4ODM4O1xcbiAgICBib3JkZXItY29sb3I6ICMxZTdlMzQ7IH1cXG4gIC5idG4tc3VjY2Vzczpmb2N1cywgLmJ0bi1zdWNjZXNzLmZvY3VzIHtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoNzIsIDE4MCwgOTcsIDAuNSk7IH1cXG4gIC5idG4tc3VjY2Vzcy5kaXNhYmxlZCwgLmJ0bi1zdWNjZXNzOmRpc2FibGVkIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICMyOGE3NDU7XFxuICAgIGJvcmRlci1jb2xvcjogIzI4YTc0NTsgfVxcbiAgLmJ0bi1zdWNjZXNzOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZSwgLmJ0bi1zdWNjZXNzOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZSxcXG4gIC5zaG93ID4gLmJ0bi1zdWNjZXNzLmRyb3Bkb3duLXRvZ2dsZSB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjMWU3ZTM0O1xcbiAgICBib3JkZXItY29sb3I6ICMxYzc0MzA7IH1cXG4gICAgLmJ0bi1zdWNjZXNzOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZTpmb2N1cywgLmJ0bi1zdWNjZXNzOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZTpmb2N1cyxcXG4gICAgLnNob3cgPiAuYnRuLXN1Y2Nlc3MuZHJvcGRvd24tdG9nZ2xlOmZvY3VzIHtcXG4gICAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSg3MiwgMTgwLCA5NywgMC41KTsgfVxcblxcbi5idG4taW5mbyB7XFxuICBjb2xvcjogI2ZmZjtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMxN2EyYjg7XFxuICBib3JkZXItY29sb3I6ICMxN2EyYjg7IH1cXG4gIC5idG4taW5mbzpob3ZlciB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjMTM4NDk2O1xcbiAgICBib3JkZXItY29sb3I6ICMxMTdhOGI7IH1cXG4gIC5idG4taW5mbzpmb2N1cywgLmJ0bi1pbmZvLmZvY3VzIHtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoNTgsIDE3NiwgMTk1LCAwLjUpOyB9XFxuICAuYnRuLWluZm8uZGlzYWJsZWQsIC5idG4taW5mbzpkaXNhYmxlZCB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjMTdhMmI4O1xcbiAgICBib3JkZXItY29sb3I6ICMxN2EyYjg7IH1cXG4gIC5idG4taW5mbzpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmUsIC5idG4taW5mbzpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmUsXFxuICAuc2hvdyA+IC5idG4taW5mby5kcm9wZG93bi10b2dnbGUge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzExN2E4YjtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMTA3MDdmOyB9XFxuICAgIC5idG4taW5mbzpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmU6Zm9jdXMsIC5idG4taW5mbzpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmU6Zm9jdXMsXFxuICAgIC5zaG93ID4gLmJ0bi1pbmZvLmRyb3Bkb3duLXRvZ2dsZTpmb2N1cyB7XFxuICAgICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoNTgsIDE3NiwgMTk1LCAwLjUpOyB9XFxuXFxuLmJ0bi13YXJuaW5nIHtcXG4gIGNvbG9yOiAjMjEyNTI5O1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2ZmYzEwNztcXG4gIGJvcmRlci1jb2xvcjogI2ZmYzEwNzsgfVxcbiAgLmJ0bi13YXJuaW5nOmhvdmVyIHtcXG4gICAgY29sb3I6ICMyMTI1Mjk7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNlMGE4MDA7XFxuICAgIGJvcmRlci1jb2xvcjogI2QzOWUwMDsgfVxcbiAgLmJ0bi13YXJuaW5nOmZvY3VzLCAuYnRuLXdhcm5pbmcuZm9jdXMge1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyMjIsIDE3MCwgMTIsIDAuNSk7IH1cXG4gIC5idG4td2FybmluZy5kaXNhYmxlZCwgLmJ0bi13YXJuaW5nOmRpc2FibGVkIHtcXG4gICAgY29sb3I6ICMyMTI1Mjk7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNmZmMxMDc7XFxuICAgIGJvcmRlci1jb2xvcjogI2ZmYzEwNzsgfVxcbiAgLmJ0bi13YXJuaW5nOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZSwgLmJ0bi13YXJuaW5nOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZSxcXG4gIC5zaG93ID4gLmJ0bi13YXJuaW5nLmRyb3Bkb3duLXRvZ2dsZSB7XFxuICAgIGNvbG9yOiAjMjEyNTI5O1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZDM5ZTAwO1xcbiAgICBib3JkZXItY29sb3I6ICNjNjk1MDA7IH1cXG4gICAgLmJ0bi13YXJuaW5nOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZTpmb2N1cywgLmJ0bi13YXJuaW5nOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZTpmb2N1cyxcXG4gICAgLnNob3cgPiAuYnRuLXdhcm5pbmcuZHJvcGRvd24tdG9nZ2xlOmZvY3VzIHtcXG4gICAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyMjIsIDE3MCwgMTIsIDAuNSk7IH1cXG5cXG4uYnRuLWRhbmdlciB7XFxuICBjb2xvcjogI2ZmZjtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNkYzM1NDU7XFxuICBib3JkZXItY29sb3I6ICNkYzM1NDU7IH1cXG4gIC5idG4tZGFuZ2VyOmhvdmVyIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNjODIzMzM7XFxuICAgIGJvcmRlci1jb2xvcjogI2JkMjEzMDsgfVxcbiAgLmJ0bi1kYW5nZXI6Zm9jdXMsIC5idG4tZGFuZ2VyLmZvY3VzIHtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjI1LCA4MywgOTcsIDAuNSk7IH1cXG4gIC5idG4tZGFuZ2VyLmRpc2FibGVkLCAuYnRuLWRhbmdlcjpkaXNhYmxlZCB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZGMzNTQ1O1xcbiAgICBib3JkZXItY29sb3I6ICNkYzM1NDU7IH1cXG4gIC5idG4tZGFuZ2VyOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZSwgLmJ0bi1kYW5nZXI6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlLFxcbiAgLnNob3cgPiAuYnRuLWRhbmdlci5kcm9wZG93bi10b2dnbGUge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2JkMjEzMDtcXG4gICAgYm9yZGVyLWNvbG9yOiAjYjIxZjJkOyB9XFxuICAgIC5idG4tZGFuZ2VyOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZTpmb2N1cywgLmJ0bi1kYW5nZXI6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlOmZvY3VzLFxcbiAgICAuc2hvdyA+IC5idG4tZGFuZ2VyLmRyb3Bkb3duLXRvZ2dsZTpmb2N1cyB7XFxuICAgICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjI1LCA4MywgOTcsIDAuNSk7IH1cXG5cXG4uYnRuLWxpZ2h0IHtcXG4gIGNvbG9yOiAjMjEyNTI5O1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2Y4ZjlmYTtcXG4gIGJvcmRlci1jb2xvcjogI2Y4ZjlmYTsgfVxcbiAgLmJ0bi1saWdodDpob3ZlciB7XFxuICAgIGNvbG9yOiAjMjEyNTI5O1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZTJlNmVhO1xcbiAgICBib3JkZXItY29sb3I6ICNkYWUwZTU7IH1cXG4gIC5idG4tbGlnaHQ6Zm9jdXMsIC5idG4tbGlnaHQuZm9jdXMge1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyMTYsIDIxNywgMjE5LCAwLjUpOyB9XFxuICAuYnRuLWxpZ2h0LmRpc2FibGVkLCAuYnRuLWxpZ2h0OmRpc2FibGVkIHtcXG4gICAgY29sb3I6ICMyMTI1Mjk7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNmOGY5ZmE7XFxuICAgIGJvcmRlci1jb2xvcjogI2Y4ZjlmYTsgfVxcbiAgLmJ0bi1saWdodDpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmUsIC5idG4tbGlnaHQ6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlLFxcbiAgLnNob3cgPiAuYnRuLWxpZ2h0LmRyb3Bkb3duLXRvZ2dsZSB7XFxuICAgIGNvbG9yOiAjMjEyNTI5O1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZGFlMGU1O1xcbiAgICBib3JkZXItY29sb3I6ICNkM2Q5ZGY7IH1cXG4gICAgLmJ0bi1saWdodDpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmU6Zm9jdXMsIC5idG4tbGlnaHQ6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlOmZvY3VzLFxcbiAgICAuc2hvdyA+IC5idG4tbGlnaHQuZHJvcGRvd24tdG9nZ2xlOmZvY3VzIHtcXG4gICAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyMTYsIDIxNywgMjE5LCAwLjUpOyB9XFxuXFxuLmJ0bi1kYXJrIHtcXG4gIGNvbG9yOiAjZmZmO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogIzM0M2E0MDtcXG4gIGJvcmRlci1jb2xvcjogIzM0M2E0MDsgfVxcbiAgLmJ0bi1kYXJrOmhvdmVyIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICMyMzI3MmI7XFxuICAgIGJvcmRlci1jb2xvcjogIzFkMjEyNDsgfVxcbiAgLmJ0bi1kYXJrOmZvY3VzLCAuYnRuLWRhcmsuZm9jdXMge1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSg4MiwgODgsIDkzLCAwLjUpOyB9XFxuICAuYnRuLWRhcmsuZGlzYWJsZWQsIC5idG4tZGFyazpkaXNhYmxlZCB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjMzQzYTQwO1xcbiAgICBib3JkZXItY29sb3I6ICMzNDNhNDA7IH1cXG4gIC5idG4tZGFyazpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmUsIC5idG4tZGFyazpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmUsXFxuICAuc2hvdyA+IC5idG4tZGFyay5kcm9wZG93bi10b2dnbGUge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzFkMjEyNDtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMTcxYTFkOyB9XFxuICAgIC5idG4tZGFyazpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmU6Zm9jdXMsIC5idG4tZGFyazpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmU6Zm9jdXMsXFxuICAgIC5zaG93ID4gLmJ0bi1kYXJrLmRyb3Bkb3duLXRvZ2dsZTpmb2N1cyB7XFxuICAgICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoODIsIDg4LCA5MywgMC41KTsgfVxcblxcbi5idG4tb3V0bGluZS1wcmltYXJ5IHtcXG4gIGNvbG9yOiAjMDA3YmZmO1xcbiAgYm9yZGVyLWNvbG9yOiAjMDA3YmZmOyB9XFxuICAuYnRuLW91dGxpbmUtcHJpbWFyeTpob3ZlciB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjMDA3YmZmO1xcbiAgICBib3JkZXItY29sb3I6ICMwMDdiZmY7IH1cXG4gIC5idG4tb3V0bGluZS1wcmltYXJ5OmZvY3VzLCAuYnRuLW91dGxpbmUtcHJpbWFyeS5mb2N1cyB7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDAsIDEyMywgMjU1LCAwLjUpOyB9XFxuICAuYnRuLW91dGxpbmUtcHJpbWFyeS5kaXNhYmxlZCwgLmJ0bi1vdXRsaW5lLXByaW1hcnk6ZGlzYWJsZWQge1xcbiAgICBjb2xvcjogIzAwN2JmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogdHJhbnNwYXJlbnQ7IH1cXG4gIC5idG4tb3V0bGluZS1wcmltYXJ5Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZSwgLmJ0bi1vdXRsaW5lLXByaW1hcnk6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlLFxcbiAgLnNob3cgPiAuYnRuLW91dGxpbmUtcHJpbWFyeS5kcm9wZG93bi10b2dnbGUge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzAwN2JmZjtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMDA3YmZmOyB9XFxuICAgIC5idG4tb3V0bGluZS1wcmltYXJ5Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZTpmb2N1cywgLmJ0bi1vdXRsaW5lLXByaW1hcnk6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlOmZvY3VzLFxcbiAgICAuc2hvdyA+IC5idG4tb3V0bGluZS1wcmltYXJ5LmRyb3Bkb3duLXRvZ2dsZTpmb2N1cyB7XFxuICAgICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMCwgMTIzLCAyNTUsIDAuNSk7IH1cXG5cXG4uYnRuLW91dGxpbmUtc2Vjb25kYXJ5IHtcXG4gIGNvbG9yOiAjNmM3NTdkO1xcbiAgYm9yZGVyLWNvbG9yOiAjNmM3NTdkOyB9XFxuICAuYnRuLW91dGxpbmUtc2Vjb25kYXJ5OmhvdmVyIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICM2Yzc1N2Q7XFxuICAgIGJvcmRlci1jb2xvcjogIzZjNzU3ZDsgfVxcbiAgLmJ0bi1vdXRsaW5lLXNlY29uZGFyeTpmb2N1cywgLmJ0bi1vdXRsaW5lLXNlY29uZGFyeS5mb2N1cyB7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDEwOCwgMTE3LCAxMjUsIDAuNSk7IH1cXG4gIC5idG4tb3V0bGluZS1zZWNvbmRhcnkuZGlzYWJsZWQsIC5idG4tb3V0bGluZS1zZWNvbmRhcnk6ZGlzYWJsZWQge1xcbiAgICBjb2xvcjogIzZjNzU3ZDtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogdHJhbnNwYXJlbnQ7IH1cXG4gIC5idG4tb3V0bGluZS1zZWNvbmRhcnk6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlLCAuYnRuLW91dGxpbmUtc2Vjb25kYXJ5Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZSxcXG4gIC5zaG93ID4gLmJ0bi1vdXRsaW5lLXNlY29uZGFyeS5kcm9wZG93bi10b2dnbGUge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzZjNzU3ZDtcXG4gICAgYm9yZGVyLWNvbG9yOiAjNmM3NTdkOyB9XFxuICAgIC5idG4tb3V0bGluZS1zZWNvbmRhcnk6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlOmZvY3VzLCAuYnRuLW91dGxpbmUtc2Vjb25kYXJ5Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZTpmb2N1cyxcXG4gICAgLnNob3cgPiAuYnRuLW91dGxpbmUtc2Vjb25kYXJ5LmRyb3Bkb3duLXRvZ2dsZTpmb2N1cyB7XFxuICAgICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMTA4LCAxMTcsIDEyNSwgMC41KTsgfVxcblxcbi5idG4tb3V0bGluZS1zdWNjZXNzIHtcXG4gIGNvbG9yOiAjMjhhNzQ1O1xcbiAgYm9yZGVyLWNvbG9yOiAjMjhhNzQ1OyB9XFxuICAuYnRuLW91dGxpbmUtc3VjY2Vzczpob3ZlciB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjMjhhNzQ1O1xcbiAgICBib3JkZXItY29sb3I6ICMyOGE3NDU7IH1cXG4gIC5idG4tb3V0bGluZS1zdWNjZXNzOmZvY3VzLCAuYnRuLW91dGxpbmUtc3VjY2Vzcy5mb2N1cyB7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDQwLCAxNjcsIDY5LCAwLjUpOyB9XFxuICAuYnRuLW91dGxpbmUtc3VjY2Vzcy5kaXNhYmxlZCwgLmJ0bi1vdXRsaW5lLXN1Y2Nlc3M6ZGlzYWJsZWQge1xcbiAgICBjb2xvcjogIzI4YTc0NTtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogdHJhbnNwYXJlbnQ7IH1cXG4gIC5idG4tb3V0bGluZS1zdWNjZXNzOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZSwgLmJ0bi1vdXRsaW5lLXN1Y2Nlc3M6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlLFxcbiAgLnNob3cgPiAuYnRuLW91dGxpbmUtc3VjY2Vzcy5kcm9wZG93bi10b2dnbGUge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzI4YTc0NTtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMjhhNzQ1OyB9XFxuICAgIC5idG4tb3V0bGluZS1zdWNjZXNzOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZTpmb2N1cywgLmJ0bi1vdXRsaW5lLXN1Y2Nlc3M6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlOmZvY3VzLFxcbiAgICAuc2hvdyA+IC5idG4tb3V0bGluZS1zdWNjZXNzLmRyb3Bkb3duLXRvZ2dsZTpmb2N1cyB7XFxuICAgICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoNDAsIDE2NywgNjksIDAuNSk7IH1cXG5cXG4uYnRuLW91dGxpbmUtaW5mbyB7XFxuICBjb2xvcjogIzE3YTJiODtcXG4gIGJvcmRlci1jb2xvcjogIzE3YTJiODsgfVxcbiAgLmJ0bi1vdXRsaW5lLWluZm86aG92ZXIge1xcbiAgICBjb2xvcjogI2ZmZjtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogIzE3YTJiODtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMTdhMmI4OyB9XFxuICAuYnRuLW91dGxpbmUtaW5mbzpmb2N1cywgLmJ0bi1vdXRsaW5lLWluZm8uZm9jdXMge1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyMywgMTYyLCAxODQsIDAuNSk7IH1cXG4gIC5idG4tb3V0bGluZS1pbmZvLmRpc2FibGVkLCAuYnRuLW91dGxpbmUtaW5mbzpkaXNhYmxlZCB7XFxuICAgIGNvbG9yOiAjMTdhMmI4O1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiB0cmFuc3BhcmVudDsgfVxcbiAgLmJ0bi1vdXRsaW5lLWluZm86bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlLCAuYnRuLW91dGxpbmUtaW5mbzpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmUsXFxuICAuc2hvdyA+IC5idG4tb3V0bGluZS1pbmZvLmRyb3Bkb3duLXRvZ2dsZSB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjMTdhMmI4O1xcbiAgICBib3JkZXItY29sb3I6ICMxN2EyYjg7IH1cXG4gICAgLmJ0bi1vdXRsaW5lLWluZm86bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCk6YWN0aXZlOmZvY3VzLCAuYnRuLW91dGxpbmUtaW5mbzpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKS5hY3RpdmU6Zm9jdXMsXFxuICAgIC5zaG93ID4gLmJ0bi1vdXRsaW5lLWluZm8uZHJvcGRvd24tdG9nZ2xlOmZvY3VzIHtcXG4gICAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyMywgMTYyLCAxODQsIDAuNSk7IH1cXG5cXG4uYnRuLW91dGxpbmUtd2FybmluZyB7XFxuICBjb2xvcjogI2ZmYzEwNztcXG4gIGJvcmRlci1jb2xvcjogI2ZmYzEwNzsgfVxcbiAgLmJ0bi1vdXRsaW5lLXdhcm5pbmc6aG92ZXIge1xcbiAgICBjb2xvcjogIzIxMjUyOTtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogI2ZmYzEwNztcXG4gICAgYm9yZGVyLWNvbG9yOiAjZmZjMTA3OyB9XFxuICAuYnRuLW91dGxpbmUtd2FybmluZzpmb2N1cywgLmJ0bi1vdXRsaW5lLXdhcm5pbmcuZm9jdXMge1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyNTUsIDE5MywgNywgMC41KTsgfVxcbiAgLmJ0bi1vdXRsaW5lLXdhcm5pbmcuZGlzYWJsZWQsIC5idG4tb3V0bGluZS13YXJuaW5nOmRpc2FibGVkIHtcXG4gICAgY29sb3I6ICNmZmMxMDc7XFxuICAgIGJhY2tncm91bmQtY29sb3I6IHRyYW5zcGFyZW50OyB9XFxuICAuYnRuLW91dGxpbmUtd2FybmluZzpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmUsIC5idG4tb3V0bGluZS13YXJuaW5nOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZSxcXG4gIC5zaG93ID4gLmJ0bi1vdXRsaW5lLXdhcm5pbmcuZHJvcGRvd24tdG9nZ2xlIHtcXG4gICAgY29sb3I6ICMyMTI1Mjk7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNmZmMxMDc7XFxuICAgIGJvcmRlci1jb2xvcjogI2ZmYzEwNzsgfVxcbiAgICAuYnRuLW91dGxpbmUtd2FybmluZzpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmU6Zm9jdXMsIC5idG4tb3V0bGluZS13YXJuaW5nOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZTpmb2N1cyxcXG4gICAgLnNob3cgPiAuYnRuLW91dGxpbmUtd2FybmluZy5kcm9wZG93bi10b2dnbGU6Zm9jdXMge1xcbiAgICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDI1NSwgMTkzLCA3LCAwLjUpOyB9XFxuXFxuLmJ0bi1vdXRsaW5lLWRhbmdlciB7XFxuICBjb2xvcjogI2RjMzU0NTtcXG4gIGJvcmRlci1jb2xvcjogI2RjMzU0NTsgfVxcbiAgLmJ0bi1vdXRsaW5lLWRhbmdlcjpob3ZlciB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZGMzNTQ1O1xcbiAgICBib3JkZXItY29sb3I6ICNkYzM1NDU7IH1cXG4gIC5idG4tb3V0bGluZS1kYW5nZXI6Zm9jdXMsIC5idG4tb3V0bGluZS1kYW5nZXIuZm9jdXMge1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyMjAsIDUzLCA2OSwgMC41KTsgfVxcbiAgLmJ0bi1vdXRsaW5lLWRhbmdlci5kaXNhYmxlZCwgLmJ0bi1vdXRsaW5lLWRhbmdlcjpkaXNhYmxlZCB7XFxuICAgIGNvbG9yOiAjZGMzNTQ1O1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiB0cmFuc3BhcmVudDsgfVxcbiAgLmJ0bi1vdXRsaW5lLWRhbmdlcjpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmUsIC5idG4tb3V0bGluZS1kYW5nZXI6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlLFxcbiAgLnNob3cgPiAuYnRuLW91dGxpbmUtZGFuZ2VyLmRyb3Bkb3duLXRvZ2dsZSB7XFxuICAgIGNvbG9yOiAjZmZmO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZGMzNTQ1O1xcbiAgICBib3JkZXItY29sb3I6ICNkYzM1NDU7IH1cXG4gICAgLmJ0bi1vdXRsaW5lLWRhbmdlcjpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmU6Zm9jdXMsIC5idG4tb3V0bGluZS1kYW5nZXI6bm90KDpkaXNhYmxlZCk6bm90KC5kaXNhYmxlZCkuYWN0aXZlOmZvY3VzLFxcbiAgICAuc2hvdyA+IC5idG4tb3V0bGluZS1kYW5nZXIuZHJvcGRvd24tdG9nZ2xlOmZvY3VzIHtcXG4gICAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyMjAsIDUzLCA2OSwgMC41KTsgfVxcblxcbi5idG4tb3V0bGluZS1saWdodCB7XFxuICBjb2xvcjogI2Y4ZjlmYTtcXG4gIGJvcmRlci1jb2xvcjogI2Y4ZjlmYTsgfVxcbiAgLmJ0bi1vdXRsaW5lLWxpZ2h0OmhvdmVyIHtcXG4gICAgY29sb3I6ICMyMTI1Mjk7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICNmOGY5ZmE7XFxuICAgIGJvcmRlci1jb2xvcjogI2Y4ZjlmYTsgfVxcbiAgLmJ0bi1vdXRsaW5lLWxpZ2h0OmZvY3VzLCAuYnRuLW91dGxpbmUtbGlnaHQuZm9jdXMge1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyNDgsIDI0OSwgMjUwLCAwLjUpOyB9XFxuICAuYnRuLW91dGxpbmUtbGlnaHQuZGlzYWJsZWQsIC5idG4tb3V0bGluZS1saWdodDpkaXNhYmxlZCB7XFxuICAgIGNvbG9yOiAjZjhmOWZhO1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiB0cmFuc3BhcmVudDsgfVxcbiAgLmJ0bi1vdXRsaW5lLWxpZ2h0Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZSwgLmJ0bi1vdXRsaW5lLWxpZ2h0Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZSxcXG4gIC5zaG93ID4gLmJ0bi1vdXRsaW5lLWxpZ2h0LmRyb3Bkb3duLXRvZ2dsZSB7XFxuICAgIGNvbG9yOiAjMjEyNTI5O1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZjhmOWZhO1xcbiAgICBib3JkZXItY29sb3I6ICNmOGY5ZmE7IH1cXG4gICAgLmJ0bi1vdXRsaW5lLWxpZ2h0Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpOmFjdGl2ZTpmb2N1cywgLmJ0bi1vdXRsaW5lLWxpZ2h0Om5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZTpmb2N1cyxcXG4gICAgLnNob3cgPiAuYnRuLW91dGxpbmUtbGlnaHQuZHJvcGRvd24tdG9nZ2xlOmZvY3VzIHtcXG4gICAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyNDgsIDI0OSwgMjUwLCAwLjUpOyB9XFxuXFxuLmJ0bi1vdXRsaW5lLWRhcmsge1xcbiAgY29sb3I6ICMzNDNhNDA7XFxuICBib3JkZXItY29sb3I6ICMzNDNhNDA7IH1cXG4gIC5idG4tb3V0bGluZS1kYXJrOmhvdmVyIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICMzNDNhNDA7XFxuICAgIGJvcmRlci1jb2xvcjogIzM0M2E0MDsgfVxcbiAgLmJ0bi1vdXRsaW5lLWRhcms6Zm9jdXMsIC5idG4tb3V0bGluZS1kYXJrLmZvY3VzIHtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoNTIsIDU4LCA2NCwgMC41KTsgfVxcbiAgLmJ0bi1vdXRsaW5lLWRhcmsuZGlzYWJsZWQsIC5idG4tb3V0bGluZS1kYXJrOmRpc2FibGVkIHtcXG4gICAgY29sb3I6ICMzNDNhNDA7XFxuICAgIGJhY2tncm91bmQtY29sb3I6IHRyYW5zcGFyZW50OyB9XFxuICAuYnRuLW91dGxpbmUtZGFyazpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmUsIC5idG4tb3V0bGluZS1kYXJrOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZSxcXG4gIC5zaG93ID4gLmJ0bi1vdXRsaW5lLWRhcmsuZHJvcGRvd24tdG9nZ2xlIHtcXG4gICAgY29sb3I6ICNmZmY7XFxuICAgIGJhY2tncm91bmQtY29sb3I6ICMzNDNhNDA7XFxuICAgIGJvcmRlci1jb2xvcjogIzM0M2E0MDsgfVxcbiAgICAuYnRuLW91dGxpbmUtZGFyazpub3QoOmRpc2FibGVkKTpub3QoLmRpc2FibGVkKTphY3RpdmU6Zm9jdXMsIC5idG4tb3V0bGluZS1kYXJrOm5vdCg6ZGlzYWJsZWQpOm5vdCguZGlzYWJsZWQpLmFjdGl2ZTpmb2N1cyxcXG4gICAgLnNob3cgPiAuYnRuLW91dGxpbmUtZGFyay5kcm9wZG93bi10b2dnbGU6Zm9jdXMge1xcbiAgICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDUyLCA1OCwgNjQsIDAuNSk7IH1cXG5cXG4uYnRuLWxpbmsge1xcbiAgZm9udC13ZWlnaHQ6IDQwMDtcXG4gIGNvbG9yOiAjMDA3YmZmO1xcbiAgdGV4dC1kZWNvcmF0aW9uOiBub25lOyB9XFxuICAuYnRuLWxpbms6aG92ZXIge1xcbiAgICBjb2xvcjogIzAwNTZiMztcXG4gICAgdGV4dC1kZWNvcmF0aW9uOiB1bmRlcmxpbmU7IH1cXG4gIC5idG4tbGluazpmb2N1cywgLmJ0bi1saW5rLmZvY3VzIHtcXG4gICAgdGV4dC1kZWNvcmF0aW9uOiB1bmRlcmxpbmU7XFxuICAgIGJveC1zaGFkb3c6IG5vbmU7IH1cXG4gIC5idG4tbGluazpkaXNhYmxlZCwgLmJ0bi1saW5rLmRpc2FibGVkIHtcXG4gICAgY29sb3I6ICM2Yzc1N2Q7XFxuICAgIHBvaW50ZXItZXZlbnRzOiBub25lOyB9XFxuXFxuLmJ0bi1sZyB7XFxuICBwYWRkaW5nOiAwLjVyZW0gMXJlbTtcXG4gIGZvbnQtc2l6ZTogMS4yNXJlbTtcXG4gIGxpbmUtaGVpZ2h0OiAxLjU7XFxuICBib3JkZXItcmFkaXVzOiAwLjNyZW07IH1cXG5cXG4uYnRuLXNtIHtcXG4gIHBhZGRpbmc6IDAuMjVyZW0gMC41cmVtO1xcbiAgZm9udC1zaXplOiAwLjg3NXJlbTtcXG4gIGxpbmUtaGVpZ2h0OiAxLjU7XFxuICBib3JkZXItcmFkaXVzOiAwLjJyZW07IH1cXG5cXG4uYnRuLWJsb2NrIHtcXG4gIGRpc3BsYXk6IGJsb2NrO1xcbiAgd2lkdGg6IDEwMCU7IH1cXG4gIC5idG4tYmxvY2sgKyAuYnRuLWJsb2NrIHtcXG4gICAgbWFyZ2luLXRvcDogMC41cmVtOyB9XFxuXFxuaW5wdXRbdHlwZT1cXFwic3VibWl0XFxcIl0uYnRuLWJsb2NrLFxcbmlucHV0W3R5cGU9XFxcInJlc2V0XFxcIl0uYnRuLWJsb2NrLFxcbmlucHV0W3R5cGU9XFxcImJ1dHRvblxcXCJdLmJ0bi1ibG9jayB7XFxuICB3aWR0aDogMTAwJTsgfVxcblxcbi5mYWRlIHtcXG4gIHRyYW5zaXRpb246IG9wYWNpdHkgMC4xNXMgbGluZWFyOyB9XFxuICBAbWVkaWEgKHByZWZlcnMtcmVkdWNlZC1tb3Rpb246IHJlZHVjZSkge1xcbiAgICAuZmFkZSB7XFxuICAgICAgdHJhbnNpdGlvbjogbm9uZTsgfSB9XFxuICAuZmFkZTpub3QoLnNob3cpIHtcXG4gICAgb3BhY2l0eTogMDsgfVxcblxcbi5jb2xsYXBzZTpub3QoLnNob3cpIHtcXG4gIGRpc3BsYXk6IG5vbmU7IH1cXG5cXG4uY29sbGFwc2luZyB7XFxuICBwb3NpdGlvbjogcmVsYXRpdmU7XFxuICBoZWlnaHQ6IDA7XFxuICBvdmVyZmxvdzogaGlkZGVuO1xcbiAgdHJhbnNpdGlvbjogaGVpZ2h0IDAuMzVzIGVhc2U7IH1cXG4gIEBtZWRpYSAocHJlZmVycy1yZWR1Y2VkLW1vdGlvbjogcmVkdWNlKSB7XFxuICAgIC5jb2xsYXBzaW5nIHtcXG4gICAgICB0cmFuc2l0aW9uOiBub25lOyB9IH1cXG5cXG4uZm9ybS1jb250cm9sIHtcXG4gIGRpc3BsYXk6IGJsb2NrO1xcbiAgd2lkdGg6IDEwMCU7XFxuICBoZWlnaHQ6IGNhbGMoMS41ZW0gKyAwLjc1cmVtICsgMnB4KTtcXG4gIHBhZGRpbmc6IDAuMzc1cmVtIDAuNzVyZW07XFxuICBmb250LXNpemU6IDFyZW07XFxuICBmb250LXdlaWdodDogNDAwO1xcbiAgbGluZS1oZWlnaHQ6IDEuNTtcXG4gIGNvbG9yOiAjNDk1MDU3O1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2ZmZjtcXG4gIGJhY2tncm91bmQtY2xpcDogcGFkZGluZy1ib3g7XFxuICBib3JkZXI6IDFweCBzb2xpZCAjY2VkNGRhO1xcbiAgYm9yZGVyLXJhZGl1czogMC4yNXJlbTtcXG4gIHRyYW5zaXRpb246IGJvcmRlci1jb2xvciAwLjE1cyBlYXNlLWluLW91dCwgYm94LXNoYWRvdyAwLjE1cyBlYXNlLWluLW91dDsgfVxcbiAgQG1lZGlhIChwcmVmZXJzLXJlZHVjZWQtbW90aW9uOiByZWR1Y2UpIHtcXG4gICAgLmZvcm0tY29udHJvbCB7XFxuICAgICAgdHJhbnNpdGlvbjogbm9uZTsgfSB9XFxuICAuZm9ybS1jb250cm9sOjotbXMtZXhwYW5kIHtcXG4gICAgYmFja2dyb3VuZC1jb2xvcjogdHJhbnNwYXJlbnQ7XFxuICAgIGJvcmRlcjogMDsgfVxcbiAgLmZvcm0tY29udHJvbDpmb2N1cyB7XFxuICAgIGNvbG9yOiAjNDk1MDU3O1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmZmO1xcbiAgICBib3JkZXItY29sb3I6ICM4MGJkZmY7XFxuICAgIG91dGxpbmU6IDA7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDAsIDEyMywgMjU1LCAwLjI1KTsgfVxcbiAgLmZvcm0tY29udHJvbDo6cGxhY2Vob2xkZXIge1xcbiAgICBjb2xvcjogIzZjNzU3ZDtcXG4gICAgb3BhY2l0eTogMTsgfVxcbiAgLmZvcm0tY29udHJvbDpkaXNhYmxlZCwgLmZvcm0tY29udHJvbFtyZWFkb25seV0ge1xcbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiAjZTllY2VmO1xcbiAgICBvcGFjaXR5OiAxOyB9XFxuXFxuc2VsZWN0LmZvcm0tY29udHJvbDpmb2N1czo6LW1zLXZhbHVlIHtcXG4gIGNvbG9yOiAjNDk1MDU3O1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2ZmZjsgfVxcblxcbi5mb3JtLWNvbnRyb2wtZmlsZSxcXG4uZm9ybS1jb250cm9sLXJhbmdlIHtcXG4gIGRpc3BsYXk6IGJsb2NrO1xcbiAgd2lkdGg6IDEwMCU7IH1cXG5cXG4uY29sLWZvcm0tbGFiZWwge1xcbiAgcGFkZGluZy10b3A6IGNhbGMoMC4zNzVyZW0gKyAxcHgpO1xcbiAgcGFkZGluZy1ib3R0b206IGNhbGMoMC4zNzVyZW0gKyAxcHgpO1xcbiAgbWFyZ2luLWJvdHRvbTogMDtcXG4gIGZvbnQtc2l6ZTogaW5oZXJpdDtcXG4gIGxpbmUtaGVpZ2h0OiAxLjU7IH1cXG5cXG4uY29sLWZvcm0tbGFiZWwtbGcge1xcbiAgcGFkZGluZy10b3A6IGNhbGMoMC41cmVtICsgMXB4KTtcXG4gIHBhZGRpbmctYm90dG9tOiBjYWxjKDAuNXJlbSArIDFweCk7XFxuICBmb250LXNpemU6IDEuMjVyZW07XFxuICBsaW5lLWhlaWdodDogMS41OyB9XFxuXFxuLmNvbC1mb3JtLWxhYmVsLXNtIHtcXG4gIHBhZGRpbmctdG9wOiBjYWxjKDAuMjVyZW0gKyAxcHgpO1xcbiAgcGFkZGluZy1ib3R0b206IGNhbGMoMC4yNXJlbSArIDFweCk7XFxuICBmb250LXNpemU6IDAuODc1cmVtO1xcbiAgbGluZS1oZWlnaHQ6IDEuNTsgfVxcblxcbi5mb3JtLWNvbnRyb2wtcGxhaW50ZXh0IHtcXG4gIGRpc3BsYXk6IGJsb2NrO1xcbiAgd2lkdGg6IDEwMCU7XFxuICBwYWRkaW5nLXRvcDogMC4zNzVyZW07XFxuICBwYWRkaW5nLWJvdHRvbTogMC4zNzVyZW07XFxuICBtYXJnaW4tYm90dG9tOiAwO1xcbiAgbGluZS1oZWlnaHQ6IDEuNTtcXG4gIGNvbG9yOiAjMjEyNTI5O1xcbiAgYmFja2dyb3VuZC1jb2xvcjogdHJhbnNwYXJlbnQ7XFxuICBib3JkZXI6IHNvbGlkIHRyYW5zcGFyZW50O1xcbiAgYm9yZGVyLXdpZHRoOiAxcHggMDsgfVxcbiAgLmZvcm0tY29udHJvbC1wbGFpbnRleHQuZm9ybS1jb250cm9sLXNtLCAuZm9ybS1jb250cm9sLXBsYWludGV4dC5mb3JtLWNvbnRyb2wtbGcge1xcbiAgICBwYWRkaW5nLXJpZ2h0OiAwO1xcbiAgICBwYWRkaW5nLWxlZnQ6IDA7IH1cXG5cXG4uZm9ybS1jb250cm9sLXNtIHtcXG4gIGhlaWdodDogY2FsYygxLjVlbSArIDAuNXJlbSArIDJweCk7XFxuICBwYWRkaW5nOiAwLjI1cmVtIDAuNXJlbTtcXG4gIGZvbnQtc2l6ZTogMC44NzVyZW07XFxuICBsaW5lLWhlaWdodDogMS41O1xcbiAgYm9yZGVyLXJhZGl1czogMC4ycmVtOyB9XFxuXFxuLmZvcm0tY29udHJvbC1sZyB7XFxuICBoZWlnaHQ6IGNhbGMoMS41ZW0gKyAxcmVtICsgMnB4KTtcXG4gIHBhZGRpbmc6IDAuNXJlbSAxcmVtO1xcbiAgZm9udC1zaXplOiAxLjI1cmVtO1xcbiAgbGluZS1oZWlnaHQ6IDEuNTtcXG4gIGJvcmRlci1yYWRpdXM6IDAuM3JlbTsgfVxcblxcbnNlbGVjdC5mb3JtLWNvbnRyb2xbc2l6ZV0sIHNlbGVjdC5mb3JtLWNvbnRyb2xbbXVsdGlwbGVdIHtcXG4gIGhlaWdodDogYXV0bzsgfVxcblxcbnRleHRhcmVhLmZvcm0tY29udHJvbCB7XFxuICBoZWlnaHQ6IGF1dG87IH1cXG5cXG4uZm9ybS1ncm91cCB7XFxuICBtYXJnaW4tYm90dG9tOiAxcmVtOyB9XFxuXFxuLmZvcm0tdGV4dCB7XFxuICBkaXNwbGF5OiBibG9jaztcXG4gIG1hcmdpbi10b3A6IDAuMjVyZW07IH1cXG5cXG4uZm9ybS1yb3cge1xcbiAgZGlzcGxheTogZmxleDtcXG4gIGZsZXgtd3JhcDogd3JhcDtcXG4gIG1hcmdpbi1yaWdodDogLTVweDtcXG4gIG1hcmdpbi1sZWZ0OiAtNXB4OyB9XFxuICAuZm9ybS1yb3cgPiAuY29sLFxcbiAgLmZvcm0tcm93ID4gW2NsYXNzKj1cXFwiY29sLVxcXCJdIHtcXG4gICAgcGFkZGluZy1yaWdodDogNXB4O1xcbiAgICBwYWRkaW5nLWxlZnQ6IDVweDsgfVxcblxcbi5mb3JtLWNoZWNrIHtcXG4gIHBvc2l0aW9uOiByZWxhdGl2ZTtcXG4gIGRpc3BsYXk6IGJsb2NrO1xcbiAgcGFkZGluZy1sZWZ0OiAxLjI1cmVtOyB9XFxuXFxuLmZvcm0tY2hlY2staW5wdXQge1xcbiAgcG9zaXRpb246IGFic29sdXRlO1xcbiAgbWFyZ2luLXRvcDogMC4zcmVtO1xcbiAgbWFyZ2luLWxlZnQ6IC0xLjI1cmVtOyB9XFxuICAuZm9ybS1jaGVjay1pbnB1dDpkaXNhYmxlZCB+IC5mb3JtLWNoZWNrLWxhYmVsIHtcXG4gICAgY29sb3I6ICM2Yzc1N2Q7IH1cXG5cXG4uZm9ybS1jaGVjay1sYWJlbCB7XFxuICBtYXJnaW4tYm90dG9tOiAwOyB9XFxuXFxuLmZvcm0tY2hlY2staW5saW5lIHtcXG4gIGRpc3BsYXk6IGlubGluZS1mbGV4O1xcbiAgYWxpZ24taXRlbXM6IGNlbnRlcjtcXG4gIHBhZGRpbmctbGVmdDogMDtcXG4gIG1hcmdpbi1yaWdodDogMC43NXJlbTsgfVxcbiAgLmZvcm0tY2hlY2staW5saW5lIC5mb3JtLWNoZWNrLWlucHV0IHtcXG4gICAgcG9zaXRpb246IHN0YXRpYztcXG4gICAgbWFyZ2luLXRvcDogMDtcXG4gICAgbWFyZ2luLXJpZ2h0OiAwLjMxMjVyZW07XFxuICAgIG1hcmdpbi1sZWZ0OiAwOyB9XFxuXFxuLnZhbGlkLWZlZWRiYWNrIHtcXG4gIGRpc3BsYXk6IG5vbmU7XFxuICB3aWR0aDogMTAwJTtcXG4gIG1hcmdpbi10b3A6IDAuMjVyZW07XFxuICBmb250LXNpemU6IDgwJTtcXG4gIGNvbG9yOiAjMjhhNzQ1OyB9XFxuXFxuLnZhbGlkLXRvb2x0aXAge1xcbiAgcG9zaXRpb246IGFic29sdXRlO1xcbiAgdG9wOiAxMDAlO1xcbiAgei1pbmRleDogNTtcXG4gIGRpc3BsYXk6IG5vbmU7XFxuICBtYXgtd2lkdGg6IDEwMCU7XFxuICBwYWRkaW5nOiAwLjI1cmVtIDAuNXJlbTtcXG4gIG1hcmdpbi10b3A6IC4xcmVtO1xcbiAgZm9udC1zaXplOiAwLjg3NXJlbTtcXG4gIGxpbmUtaGVpZ2h0OiAxLjU7XFxuICBjb2xvcjogI2ZmZjtcXG4gIGJhY2tncm91bmQtY29sb3I6IHJnYmEoNDAsIDE2NywgNjksIDAuOSk7XFxuICBib3JkZXItcmFkaXVzOiAwLjI1cmVtOyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmZvcm0tY29udHJvbDp2YWxpZCwgLmZvcm0tY29udHJvbC5pcy12YWxpZCB7XFxuICBib3JkZXItY29sb3I6ICMyOGE3NDU7XFxuICBwYWRkaW5nLXJpZ2h0OiBjYWxjKDEuNWVtICsgMC43NXJlbSk7XFxuICBiYWNrZ3JvdW5kLWltYWdlOiB1cmwoXFxcImRhdGE6aW1hZ2Uvc3ZnK3htbCwlM2NzdmcgeG1sbnM9J2h0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnJyB2aWV3Qm94PScwIDAgOCA4JyUzZSUzY3BhdGggZmlsbD0nJTIzMjhhNzQ1JyBkPSdNMi4zIDYuNzNMLjYgNC41M2MtLjQtMS4wNC40Ni0xLjQgMS4xLS44bDEuMSAxLjQgMy40LTMuOGMuNi0uNjMgMS42LS4yNyAxLjIuN2wtNCA0LjZjLS40My41LS44LjQtMS4xLjF6Jy8lM2UlM2Mvc3ZnJTNlXFxcIik7XFxuICBiYWNrZ3JvdW5kLXJlcGVhdDogbm8tcmVwZWF0O1xcbiAgYmFja2dyb3VuZC1wb3NpdGlvbjogY2VudGVyIHJpZ2h0IGNhbGMoMC4zNzVlbSArIDAuMTg3NXJlbSk7XFxuICBiYWNrZ3JvdW5kLXNpemU6IGNhbGMoMC43NWVtICsgMC4zNzVyZW0pIGNhbGMoMC43NWVtICsgMC4zNzVyZW0pOyB9XFxuICAud2FzLXZhbGlkYXRlZCAuZm9ybS1jb250cm9sOnZhbGlkOmZvY3VzLCAuZm9ybS1jb250cm9sLmlzLXZhbGlkOmZvY3VzIHtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMjhhNzQ1O1xcbiAgICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSg0MCwgMTY3LCA2OSwgMC4yNSk7IH1cXG4gIC53YXMtdmFsaWRhdGVkIC5mb3JtLWNvbnRyb2w6dmFsaWQgfiAudmFsaWQtZmVlZGJhY2ssXFxuICAud2FzLXZhbGlkYXRlZCAuZm9ybS1jb250cm9sOnZhbGlkIH4gLnZhbGlkLXRvb2x0aXAsIC5mb3JtLWNvbnRyb2wuaXMtdmFsaWQgfiAudmFsaWQtZmVlZGJhY2ssXFxuICAuZm9ybS1jb250cm9sLmlzLXZhbGlkIH4gLnZhbGlkLXRvb2x0aXAge1xcbiAgICBkaXNwbGF5OiBibG9jazsgfVxcblxcbi53YXMtdmFsaWRhdGVkIHRleHRhcmVhLmZvcm0tY29udHJvbDp2YWxpZCwgdGV4dGFyZWEuZm9ybS1jb250cm9sLmlzLXZhbGlkIHtcXG4gIHBhZGRpbmctcmlnaHQ6IGNhbGMoMS41ZW0gKyAwLjc1cmVtKTtcXG4gIGJhY2tncm91bmQtcG9zaXRpb246IHRvcCBjYWxjKDAuMzc1ZW0gKyAwLjE4NzVyZW0pIHJpZ2h0IGNhbGMoMC4zNzVlbSArIDAuMTg3NXJlbSk7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLXNlbGVjdDp2YWxpZCwgLmN1c3RvbS1zZWxlY3QuaXMtdmFsaWQge1xcbiAgYm9yZGVyLWNvbG9yOiAjMjhhNzQ1O1xcbiAgcGFkZGluZy1yaWdodDogY2FsYygoMWVtICsgMC43NXJlbSkgKiAzIC8gNCArIDEuNzVyZW0pO1xcbiAgYmFja2dyb3VuZDogdXJsKFxcXCJkYXRhOmltYWdlL3N2Zyt4bWwsJTNjc3ZnIHhtbG5zPSdodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2Zycgdmlld0JveD0nMCAwIDQgNSclM2UlM2NwYXRoIGZpbGw9JyUyMzM0M2E0MCcgZD0nTTIgMEwwIDJoNHptMCA1TDAgM2g0eicvJTNlJTNjL3N2ZyUzZVxcXCIpIG5vLXJlcGVhdCByaWdodCAwLjc1cmVtIGNlbnRlci84cHggMTBweCwgdXJsKFxcXCJkYXRhOmltYWdlL3N2Zyt4bWwsJTNjc3ZnIHhtbG5zPSdodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2Zycgdmlld0JveD0nMCAwIDggOCclM2UlM2NwYXRoIGZpbGw9JyUyMzI4YTc0NScgZD0nTTIuMyA2LjczTC42IDQuNTNjLS40LTEuMDQuNDYtMS40IDEuMS0uOGwxLjEgMS40IDMuNC0zLjhjLjYtLjYzIDEuNi0uMjcgMS4yLjdsLTQgNC42Yy0uNDMuNS0uOC40LTEuMS4xeicvJTNlJTNjL3N2ZyUzZVxcXCIpICNmZmYgbm8tcmVwZWF0IGNlbnRlciByaWdodCAxLjc1cmVtL2NhbGMoMC43NWVtICsgMC4zNzVyZW0pIGNhbGMoMC43NWVtICsgMC4zNzVyZW0pOyB9XFxuICAud2FzLXZhbGlkYXRlZCAuY3VzdG9tLXNlbGVjdDp2YWxpZDpmb2N1cywgLmN1c3RvbS1zZWxlY3QuaXMtdmFsaWQ6Zm9jdXMge1xcbiAgICBib3JkZXItY29sb3I6ICMyOGE3NDU7XFxuICAgIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDQwLCAxNjcsIDY5LCAwLjI1KTsgfVxcbiAgLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1zZWxlY3Q6dmFsaWQgfiAudmFsaWQtZmVlZGJhY2ssXFxuICAud2FzLXZhbGlkYXRlZCAuY3VzdG9tLXNlbGVjdDp2YWxpZCB+IC52YWxpZC10b29sdGlwLCAuY3VzdG9tLXNlbGVjdC5pcy12YWxpZCB+IC52YWxpZC1mZWVkYmFjayxcXG4gIC5jdXN0b20tc2VsZWN0LmlzLXZhbGlkIH4gLnZhbGlkLXRvb2x0aXAge1xcbiAgICBkaXNwbGF5OiBibG9jazsgfVxcblxcbi53YXMtdmFsaWRhdGVkIC5mb3JtLWNvbnRyb2wtZmlsZTp2YWxpZCB+IC52YWxpZC1mZWVkYmFjayxcXG4ud2FzLXZhbGlkYXRlZCAuZm9ybS1jb250cm9sLWZpbGU6dmFsaWQgfiAudmFsaWQtdG9vbHRpcCwgLmZvcm0tY29udHJvbC1maWxlLmlzLXZhbGlkIH4gLnZhbGlkLWZlZWRiYWNrLFxcbi5mb3JtLWNvbnRyb2wtZmlsZS5pcy12YWxpZCB+IC52YWxpZC10b29sdGlwIHtcXG4gIGRpc3BsYXk6IGJsb2NrOyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmZvcm0tY2hlY2staW5wdXQ6dmFsaWQgfiAuZm9ybS1jaGVjay1sYWJlbCwgLmZvcm0tY2hlY2staW5wdXQuaXMtdmFsaWQgfiAuZm9ybS1jaGVjay1sYWJlbCB7XFxuICBjb2xvcjogIzI4YTc0NTsgfVxcblxcbi53YXMtdmFsaWRhdGVkIC5mb3JtLWNoZWNrLWlucHV0OnZhbGlkIH4gLnZhbGlkLWZlZWRiYWNrLFxcbi53YXMtdmFsaWRhdGVkIC5mb3JtLWNoZWNrLWlucHV0OnZhbGlkIH4gLnZhbGlkLXRvb2x0aXAsIC5mb3JtLWNoZWNrLWlucHV0LmlzLXZhbGlkIH4gLnZhbGlkLWZlZWRiYWNrLFxcbi5mb3JtLWNoZWNrLWlucHV0LmlzLXZhbGlkIH4gLnZhbGlkLXRvb2x0aXAge1xcbiAgZGlzcGxheTogYmxvY2s7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQ6dmFsaWQgfiAuY3VzdG9tLWNvbnRyb2wtbGFiZWwsIC5jdXN0b20tY29udHJvbC1pbnB1dC5pcy12YWxpZCB+IC5jdXN0b20tY29udHJvbC1sYWJlbCB7XFxuICBjb2xvcjogIzI4YTc0NTsgfVxcbiAgLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1jb250cm9sLWlucHV0OnZhbGlkIH4gLmN1c3RvbS1jb250cm9sLWxhYmVsOjpiZWZvcmUsIC5jdXN0b20tY29udHJvbC1pbnB1dC5pcy12YWxpZCB+IC5jdXN0b20tY29udHJvbC1sYWJlbDo6YmVmb3JlIHtcXG4gICAgYm9yZGVyLWNvbG9yOiAjMjhhNzQ1OyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1jb250cm9sLWlucHV0OnZhbGlkIH4gLnZhbGlkLWZlZWRiYWNrLFxcbi53YXMtdmFsaWRhdGVkIC5jdXN0b20tY29udHJvbC1pbnB1dDp2YWxpZCB+IC52YWxpZC10b29sdGlwLCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQuaXMtdmFsaWQgfiAudmFsaWQtZmVlZGJhY2ssXFxuLmN1c3RvbS1jb250cm9sLWlucHV0LmlzLXZhbGlkIH4gLnZhbGlkLXRvb2x0aXAge1xcbiAgZGlzcGxheTogYmxvY2s7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQ6dmFsaWQ6Y2hlY2tlZCB+IC5jdXN0b20tY29udHJvbC1sYWJlbDo6YmVmb3JlLCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQuaXMtdmFsaWQ6Y2hlY2tlZCB+IC5jdXN0b20tY29udHJvbC1sYWJlbDo6YmVmb3JlIHtcXG4gIGJvcmRlci1jb2xvcjogIzM0Y2U1NztcXG4gIGJhY2tncm91bmQtY29sb3I6ICMzNGNlNTc7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQ6dmFsaWQ6Zm9jdXMgfiAuY3VzdG9tLWNvbnRyb2wtbGFiZWw6OmJlZm9yZSwgLmN1c3RvbS1jb250cm9sLWlucHV0LmlzLXZhbGlkOmZvY3VzIH4gLmN1c3RvbS1jb250cm9sLWxhYmVsOjpiZWZvcmUge1xcbiAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoNDAsIDE2NywgNjksIDAuMjUpOyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1jb250cm9sLWlucHV0OnZhbGlkOmZvY3VzOm5vdCg6Y2hlY2tlZCkgfiAuY3VzdG9tLWNvbnRyb2wtbGFiZWw6OmJlZm9yZSwgLmN1c3RvbS1jb250cm9sLWlucHV0LmlzLXZhbGlkOmZvY3VzOm5vdCg6Y2hlY2tlZCkgfiAuY3VzdG9tLWNvbnRyb2wtbGFiZWw6OmJlZm9yZSB7XFxuICBib3JkZXItY29sb3I6ICMyOGE3NDU7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLWZpbGUtaW5wdXQ6dmFsaWQgfiAuY3VzdG9tLWZpbGUtbGFiZWwsIC5jdXN0b20tZmlsZS1pbnB1dC5pcy12YWxpZCB+IC5jdXN0b20tZmlsZS1sYWJlbCB7XFxuICBib3JkZXItY29sb3I6ICMyOGE3NDU7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLWZpbGUtaW5wdXQ6dmFsaWQgfiAudmFsaWQtZmVlZGJhY2ssXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1maWxlLWlucHV0OnZhbGlkIH4gLnZhbGlkLXRvb2x0aXAsIC5jdXN0b20tZmlsZS1pbnB1dC5pcy12YWxpZCB+IC52YWxpZC1mZWVkYmFjayxcXG4uY3VzdG9tLWZpbGUtaW5wdXQuaXMtdmFsaWQgfiAudmFsaWQtdG9vbHRpcCB7XFxuICBkaXNwbGF5OiBibG9jazsgfVxcblxcbi53YXMtdmFsaWRhdGVkIC5jdXN0b20tZmlsZS1pbnB1dDp2YWxpZDpmb2N1cyB+IC5jdXN0b20tZmlsZS1sYWJlbCwgLmN1c3RvbS1maWxlLWlucHV0LmlzLXZhbGlkOmZvY3VzIH4gLmN1c3RvbS1maWxlLWxhYmVsIHtcXG4gIGJvcmRlci1jb2xvcjogIzI4YTc0NTtcXG4gIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDQwLCAxNjcsIDY5LCAwLjI1KTsgfVxcblxcbi5pbnZhbGlkLWZlZWRiYWNrIHtcXG4gIGRpc3BsYXk6IG5vbmU7XFxuICB3aWR0aDogMTAwJTtcXG4gIG1hcmdpbi10b3A6IDAuMjVyZW07XFxuICBmb250LXNpemU6IDgwJTtcXG4gIGNvbG9yOiAjZGMzNTQ1OyB9XFxuXFxuLmludmFsaWQtdG9vbHRpcCB7XFxuICBwb3NpdGlvbjogYWJzb2x1dGU7XFxuICB0b3A6IDEwMCU7XFxuICB6LWluZGV4OiA1O1xcbiAgZGlzcGxheTogbm9uZTtcXG4gIG1heC13aWR0aDogMTAwJTtcXG4gIHBhZGRpbmc6IDAuMjVyZW0gMC41cmVtO1xcbiAgbWFyZ2luLXRvcDogLjFyZW07XFxuICBmb250LXNpemU6IDAuODc1cmVtO1xcbiAgbGluZS1oZWlnaHQ6IDEuNTtcXG4gIGNvbG9yOiAjZmZmO1xcbiAgYmFja2dyb3VuZC1jb2xvcjogcmdiYSgyMjAsIDUzLCA2OSwgMC45KTtcXG4gIGJvcmRlci1yYWRpdXM6IDAuMjVyZW07IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuZm9ybS1jb250cm9sOmludmFsaWQsIC5mb3JtLWNvbnRyb2wuaXMtaW52YWxpZCB7XFxuICBib3JkZXItY29sb3I6ICNkYzM1NDU7XFxuICBwYWRkaW5nLXJpZ2h0OiBjYWxjKDEuNWVtICsgMC43NXJlbSk7XFxuICBiYWNrZ3JvdW5kLWltYWdlOiB1cmwoXFxcImRhdGE6aW1hZ2Uvc3ZnK3htbCwlM2NzdmcgeG1sbnM9J2h0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnJyBmaWxsPSclMjNkYzM1NDUnIHZpZXdCb3g9Jy0yIC0yIDcgNyclM2UlM2NwYXRoIHN0cm9rZT0nJTIzZGMzNTQ1JyBkPSdNMCAwbDMgM20wLTNMMCAzJy8lM2UlM2NjaXJjbGUgcj0nLjUnLyUzZSUzY2NpcmNsZSBjeD0nMycgcj0nLjUnLyUzZSUzY2NpcmNsZSBjeT0nMycgcj0nLjUnLyUzZSUzY2NpcmNsZSBjeD0nMycgY3k9JzMnIHI9Jy41Jy8lM2UlM2Mvc3ZnJTNFXFxcIik7XFxuICBiYWNrZ3JvdW5kLXJlcGVhdDogbm8tcmVwZWF0O1xcbiAgYmFja2dyb3VuZC1wb3NpdGlvbjogY2VudGVyIHJpZ2h0IGNhbGMoMC4zNzVlbSArIDAuMTg3NXJlbSk7XFxuICBiYWNrZ3JvdW5kLXNpemU6IGNhbGMoMC43NWVtICsgMC4zNzVyZW0pIGNhbGMoMC43NWVtICsgMC4zNzVyZW0pOyB9XFxuICAud2FzLXZhbGlkYXRlZCAuZm9ybS1jb250cm9sOmludmFsaWQ6Zm9jdXMsIC5mb3JtLWNvbnRyb2wuaXMtaW52YWxpZDpmb2N1cyB7XFxuICAgIGJvcmRlci1jb2xvcjogI2RjMzU0NTtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjIwLCA1MywgNjksIDAuMjUpOyB9XFxuICAud2FzLXZhbGlkYXRlZCAuZm9ybS1jb250cm9sOmludmFsaWQgfiAuaW52YWxpZC1mZWVkYmFjayxcXG4gIC53YXMtdmFsaWRhdGVkIC5mb3JtLWNvbnRyb2w6aW52YWxpZCB+IC5pbnZhbGlkLXRvb2x0aXAsIC5mb3JtLWNvbnRyb2wuaXMtaW52YWxpZCB+IC5pbnZhbGlkLWZlZWRiYWNrLFxcbiAgLmZvcm0tY29udHJvbC5pcy1pbnZhbGlkIH4gLmludmFsaWQtdG9vbHRpcCB7XFxuICAgIGRpc3BsYXk6IGJsb2NrOyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgdGV4dGFyZWEuZm9ybS1jb250cm9sOmludmFsaWQsIHRleHRhcmVhLmZvcm0tY29udHJvbC5pcy1pbnZhbGlkIHtcXG4gIHBhZGRpbmctcmlnaHQ6IGNhbGMoMS41ZW0gKyAwLjc1cmVtKTtcXG4gIGJhY2tncm91bmQtcG9zaXRpb246IHRvcCBjYWxjKDAuMzc1ZW0gKyAwLjE4NzVyZW0pIHJpZ2h0IGNhbGMoMC4zNzVlbSArIDAuMTg3NXJlbSk7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLXNlbGVjdDppbnZhbGlkLCAuY3VzdG9tLXNlbGVjdC5pcy1pbnZhbGlkIHtcXG4gIGJvcmRlci1jb2xvcjogI2RjMzU0NTtcXG4gIHBhZGRpbmctcmlnaHQ6IGNhbGMoKDFlbSArIDAuNzVyZW0pICogMyAvIDQgKyAxLjc1cmVtKTtcXG4gIGJhY2tncm91bmQ6IHVybChcXFwiZGF0YTppbWFnZS9zdmcreG1sLCUzY3N2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIHZpZXdCb3g9JzAgMCA0IDUnJTNlJTNjcGF0aCBmaWxsPSclMjMzNDNhNDAnIGQ9J00yIDBMMCAyaDR6bTAgNUwwIDNoNHonLyUzZSUzYy9zdmclM2VcXFwiKSBuby1yZXBlYXQgcmlnaHQgMC43NXJlbSBjZW50ZXIvOHB4IDEwcHgsIHVybChcXFwiZGF0YTppbWFnZS9zdmcreG1sLCUzY3N2ZyB4bWxucz0naHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnIGZpbGw9JyUyM2RjMzU0NScgdmlld0JveD0nLTIgLTIgNyA3JyUzZSUzY3BhdGggc3Ryb2tlPSclMjNkYzM1NDUnIGQ9J00wIDBsMyAzbTAtM0wwIDMnLyUzZSUzY2NpcmNsZSByPScuNScvJTNlJTNjY2lyY2xlIGN4PSczJyByPScuNScvJTNlJTNjY2lyY2xlIGN5PSczJyByPScuNScvJTNlJTNjY2lyY2xlIGN4PSczJyBjeT0nMycgcj0nLjUnLyUzZSUzYy9zdmclM0VcXFwiKSAjZmZmIG5vLXJlcGVhdCBjZW50ZXIgcmlnaHQgMS43NXJlbS9jYWxjKDAuNzVlbSArIDAuMzc1cmVtKSBjYWxjKDAuNzVlbSArIDAuMzc1cmVtKTsgfVxcbiAgLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1zZWxlY3Q6aW52YWxpZDpmb2N1cywgLmN1c3RvbS1zZWxlY3QuaXMtaW52YWxpZDpmb2N1cyB7XFxuICAgIGJvcmRlci1jb2xvcjogI2RjMzU0NTtcXG4gICAgYm94LXNoYWRvdzogMCAwIDAgMC4ycmVtIHJnYmEoMjIwLCA1MywgNjksIDAuMjUpOyB9XFxuICAud2FzLXZhbGlkYXRlZCAuY3VzdG9tLXNlbGVjdDppbnZhbGlkIH4gLmludmFsaWQtZmVlZGJhY2ssXFxuICAud2FzLXZhbGlkYXRlZCAuY3VzdG9tLXNlbGVjdDppbnZhbGlkIH4gLmludmFsaWQtdG9vbHRpcCwgLmN1c3RvbS1zZWxlY3QuaXMtaW52YWxpZCB+IC5pbnZhbGlkLWZlZWRiYWNrLFxcbiAgLmN1c3RvbS1zZWxlY3QuaXMtaW52YWxpZCB+IC5pbnZhbGlkLXRvb2x0aXAge1xcbiAgICBkaXNwbGF5OiBibG9jazsgfVxcblxcbi53YXMtdmFsaWRhdGVkIC5mb3JtLWNvbnRyb2wtZmlsZTppbnZhbGlkIH4gLmludmFsaWQtZmVlZGJhY2ssXFxuLndhcy12YWxpZGF0ZWQgLmZvcm0tY29udHJvbC1maWxlOmludmFsaWQgfiAuaW52YWxpZC10b29sdGlwLCAuZm9ybS1jb250cm9sLWZpbGUuaXMtaW52YWxpZCB+IC5pbnZhbGlkLWZlZWRiYWNrLFxcbi5mb3JtLWNvbnRyb2wtZmlsZS5pcy1pbnZhbGlkIH4gLmludmFsaWQtdG9vbHRpcCB7XFxuICBkaXNwbGF5OiBibG9jazsgfVxcblxcbi53YXMtdmFsaWRhdGVkIC5mb3JtLWNoZWNrLWlucHV0OmludmFsaWQgfiAuZm9ybS1jaGVjay1sYWJlbCwgLmZvcm0tY2hlY2staW5wdXQuaXMtaW52YWxpZCB+IC5mb3JtLWNoZWNrLWxhYmVsIHtcXG4gIGNvbG9yOiAjZGMzNTQ1OyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmZvcm0tY2hlY2staW5wdXQ6aW52YWxpZCB+IC5pbnZhbGlkLWZlZWRiYWNrLFxcbi53YXMtdmFsaWRhdGVkIC5mb3JtLWNoZWNrLWlucHV0OmludmFsaWQgfiAuaW52YWxpZC10b29sdGlwLCAuZm9ybS1jaGVjay1pbnB1dC5pcy1pbnZhbGlkIH4gLmludmFsaWQtZmVlZGJhY2ssXFxuLmZvcm0tY2hlY2staW5wdXQuaXMtaW52YWxpZCB+IC5pbnZhbGlkLXRvb2x0aXAge1xcbiAgZGlzcGxheTogYmxvY2s7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQ6aW52YWxpZCB+IC5jdXN0b20tY29udHJvbC1sYWJlbCwgLmN1c3RvbS1jb250cm9sLWlucHV0LmlzLWludmFsaWQgfiAuY3VzdG9tLWNvbnRyb2wtbGFiZWwge1xcbiAgY29sb3I6ICNkYzM1NDU7IH1cXG4gIC53YXMtdmFsaWRhdGVkIC5jdXN0b20tY29udHJvbC1pbnB1dDppbnZhbGlkIH4gLmN1c3RvbS1jb250cm9sLWxhYmVsOjpiZWZvcmUsIC5jdXN0b20tY29udHJvbC1pbnB1dC5pcy1pbnZhbGlkIH4gLmN1c3RvbS1jb250cm9sLWxhYmVsOjpiZWZvcmUge1xcbiAgICBib3JkZXItY29sb3I6ICNkYzM1NDU7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQ6aW52YWxpZCB+IC5pbnZhbGlkLWZlZWRiYWNrLFxcbi53YXMtdmFsaWRhdGVkIC5jdXN0b20tY29udHJvbC1pbnB1dDppbnZhbGlkIH4gLmludmFsaWQtdG9vbHRpcCwgLmN1c3RvbS1jb250cm9sLWlucHV0LmlzLWludmFsaWQgfiAuaW52YWxpZC1mZWVkYmFjayxcXG4uY3VzdG9tLWNvbnRyb2wtaW5wdXQuaXMtaW52YWxpZCB+IC5pbnZhbGlkLXRvb2x0aXAge1xcbiAgZGlzcGxheTogYmxvY2s7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQ6aW52YWxpZDpjaGVja2VkIH4gLmN1c3RvbS1jb250cm9sLWxhYmVsOjpiZWZvcmUsIC5jdXN0b20tY29udHJvbC1pbnB1dC5pcy1pbnZhbGlkOmNoZWNrZWQgfiAuY3VzdG9tLWNvbnRyb2wtbGFiZWw6OmJlZm9yZSB7XFxuICBib3JkZXItY29sb3I6ICNlNDYwNmQ7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjZTQ2MDZkOyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1jb250cm9sLWlucHV0OmludmFsaWQ6Zm9jdXMgfiAuY3VzdG9tLWNvbnRyb2wtbGFiZWw6OmJlZm9yZSwgLmN1c3RvbS1jb250cm9sLWlucHV0LmlzLWludmFsaWQ6Zm9jdXMgfiAuY3VzdG9tLWNvbnRyb2wtbGFiZWw6OmJlZm9yZSB7XFxuICBib3gtc2hhZG93OiAwIDAgMCAwLjJyZW0gcmdiYSgyMjAsIDUzLCA2OSwgMC4yNSk7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLWNvbnRyb2wtaW5wdXQ6aW52YWxpZDpmb2N1czpub3QoOmNoZWNrZWQpIH4gLmN1c3RvbS1jb250cm9sLWxhYmVsOjpiZWZvcmUsIC5jdXN0b20tY29udHJvbC1pbnB1dC5pcy1pbnZhbGlkOmZvY3VzOm5vdCg6Y2hlY2tlZCkgfiAuY3VzdG9tLWNvbnRyb2wtbGFiZWw6OmJlZm9yZSB7XFxuICBib3JkZXItY29sb3I6ICNkYzM1NDU7IH1cXG5cXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLWZpbGUtaW5wdXQ6aW52YWxpZCB+IC5jdXN0b20tZmlsZS1sYWJlbCwgLmN1c3RvbS1maWxlLWlucHV0LmlzLWludmFsaWQgfiAuY3VzdG9tLWZpbGUtbGFiZWwge1xcbiAgYm9yZGVyLWNvbG9yOiAjZGMzNTQ1OyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1maWxlLWlucHV0OmludmFsaWQgfiAuaW52YWxpZC1mZWVkYmFjayxcXG4ud2FzLXZhbGlkYXRlZCAuY3VzdG9tLWZpbGUtaW5wdXQ6aW52YWxpZCB+IC5pbnZhbGlkLXRvb2x0aXAsIC5jdXN0b20tZmlsZS1pbnB1dC5pcy1pbnZhbGlkIH4gLmludmFsaWQtZmVlZGJhY2ssXFxuLmN1c3RvbS1maWxlLWlucHV0LmlzLWludmFsaWQgfiAuaW52YWxpZC10b29sdGlwIHtcXG4gIGRpc3BsYXk6IGJsb2NrOyB9XFxuXFxuLndhcy12YWxpZGF0ZWQgLmN1c3RvbS1maWxlLWlucHV0OmludmFsaWQ6Zm9jdXMgfiAuY3VzdG9tLWZpbGUtbGFiZWwsIC5jdXN0b20tZmlsZS1pbnB1dC5pcy1pbnZhbGlkOmZvY3VzIH4gLmN1c3RvbS1maWxlLWxhYmVsIHtcXG4gIGJvcmRlci1jb2xvcjogI2RjMzU0NTtcXG4gIGJveC1zaGFkb3c6IDAgMCAwIDAuMnJlbSByZ2JhKDIyMCwgNTMsIDY5LCAwLjI1KTsgfVxcblxcbi5mb3JtLWlubGluZSB7XFxuICBkaXNwbGF5OiBmbGV4O1xcbiAgZmxleC1mbG93OiByb3cgd3JhcDtcXG4gIGFsaWduLWl0ZW1zOiBjZW50ZXI7IH1cXG4gIC5mb3JtLWlubGluZSAuZm9ybS1jaGVjayB7XFxuICAgIHdpZHRoOiAxMDAlOyB9XFxuICBAbWVkaWEgKG1pbi13aWR0aDogNTc2cHgpIHtcXG4gICAgLmZvcm0taW5saW5lIGxhYmVsIHtcXG4gICAgICBkaXNwbGF5OiBmbGV4O1xcbiAgICAgIGFsaWduLWl0ZW1zOiBjZW50ZXI7XFxuICAgICAganVzdGlmeS1jb250ZW50OiBjZW50ZXI7XFxuICAgICAgbWFyZ2luLWJvdHRvbTogMDsgfVxcbiAgICAuZm9ybS1pbmxpbmUgLmZvcm0tZ3JvdXAge1xcbiAgICAgIGRpc3BsYXk6IGZsZXg7XFxuICAgICAgZmxleDogMCAwIGF1dG87XFxuICAgICAgZmxleC1mbG93OiByb3cgd3JhcDtcXG4gICAgICBhbGlnbi1pdGVtczogY2VudGVyO1xcbiAgICAgIG1hcmdpbi1ib3R0b206IDA7IH1cXG4gICAgLmZvcm0taW5saW5lIC5mb3JtLWNvbnRyb2wge1xcbiAgICAgIGRpc3BsYXk6IGlubGluZS1ibG9jaztcXG4gICAgICB3aWR0aDogYXV0bztcXG4gICAgICB2ZXJ0aWNhbC1hbGlnbjogbWlkZGxlOyB9XFxuICAgIC5mb3JtLWlubGluZSAuZm9ybS1jb250cm9sLXBsYWludGV4dCB7XFxuICAgICAgZGlzcGxheTogaW5saW5lLWJsb2NrOyB9XFxuICAgIC5mb3JtLWlubGluZSAuaW5wdXQtZ3JvdXAsXFxuICAgIC5mb3JtLWlubGluZSAuY3VzdG9tLXNlbGVjdCB7XFxuICAgICAgd2lkdGg6IGF1dG87IH1cXG4gICAgLmZvcm0taW5saW5lIC5mb3JtLWNoZWNrIHtcXG4gICAgICBkaXNwbGF5OiBmbGV4O1xcbiAgICAgIGFsaWduLWl0ZW1zOiBjZW50ZXI7XFxuICAgICAganVzdGlmeS1jb250ZW50OiBjZW50ZXI7XFxuICAgICAgd2lkdGg6IGF1dG87XFxuICAgICAgcGFkZGluZy1sZWZ0OiAwOyB9XFxuICAgIC5mb3JtLWlubGluZSAuZm9ybS1jaGVjay1pbnB1dCB7XFxuICAgICAgcG9zaXRpb246IHJlbGF0aXZlO1xcbiAgICAgIGZsZXgtc2hyaW5rOiAwO1xcbiAgICAgIG1hcmdpbi10b3A6IDA7XFxuICAgICAgbWFyZ2luLXJpZ2h0OiAwLjI1cmVtO1xcbiAgICAgIG1hcmdpbi1sZWZ0OiAwOyB9XFxuICAgIC5mb3JtLWlubGluZSAuY3VzdG9tLWNvbnRyb2wge1xcbiAgICAgIGFsaWduLWl0ZW1zOiBjZW50ZXI7XFxuICAgICAganVzdGlmeS1jb250ZW50OiBjZW50ZXI7IH1cXG4gICAgLmZvcm0taW5saW5lIC5jdXN0b20tY29udHJvbC1sYWJlbCB7XFxuICAgICAgbWFyZ2luLWJvdHRvbTogMDsgfSB9XFxuXFxuLnBvcG92ZXIge1xcbiAgcG9zaXRpb246IGFic29sdXRlO1xcbiAgdG9wOiAwO1xcbiAgbGVmdDogMDtcXG4gIHotaW5kZXg6IDEwNjA7XFxuICBkaXNwbGF5OiBibG9jaztcXG4gIG1heC13aWR0aDogMjc2cHg7XFxuICBmb250LWZhbWlseTogLWFwcGxlLXN5c3RlbSwgQmxpbmtNYWNTeXN0ZW1Gb250LCBcXFwiU2Vnb2UgVUlcXFwiLCBSb2JvdG8sIFxcXCJIZWx2ZXRpY2EgTmV1ZVxcXCIsIEFyaWFsLCBcXFwiTm90byBTYW5zXFxcIiwgc2Fucy1zZXJpZiwgXFxcIkFwcGxlIENvbG9yIEVtb2ppXFxcIiwgXFxcIlNlZ29lIFVJIEVtb2ppXFxcIiwgXFxcIlNlZ29lIFVJIFN5bWJvbFxcXCIsIFxcXCJOb3RvIENvbG9yIEVtb2ppXFxcIjtcXG4gIGZvbnQtc3R5bGU6IG5vcm1hbDtcXG4gIGZvbnQtd2VpZ2h0OiA0MDA7XFxuICBsaW5lLWhlaWdodDogMS41O1xcbiAgdGV4dC1hbGlnbjogbGVmdDtcXG4gIHRleHQtYWxpZ246IHN0YXJ0O1xcbiAgdGV4dC1kZWNvcmF0aW9uOiBub25lO1xcbiAgdGV4dC1zaGFkb3c6IG5vbmU7XFxuICB0ZXh0LXRyYW5zZm9ybTogbm9uZTtcXG4gIGxldHRlci1zcGFjaW5nOiBub3JtYWw7XFxuICB3b3JkLWJyZWFrOiBub3JtYWw7XFxuICB3b3JkLXNwYWNpbmc6IG5vcm1hbDtcXG4gIHdoaXRlLXNwYWNlOiBub3JtYWw7XFxuICBsaW5lLWJyZWFrOiBhdXRvO1xcbiAgZm9udC1zaXplOiAwLjg3NXJlbTtcXG4gIHdvcmQtd3JhcDogYnJlYWstd29yZDtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNmZmY7XFxuICBiYWNrZ3JvdW5kLWNsaXA6IHBhZGRpbmctYm94O1xcbiAgYm9yZGVyOiAxcHggc29saWQgcmdiYSgwLCAwLCAwLCAwLjIpO1xcbiAgYm9yZGVyLXJhZGl1czogMC4zcmVtOyB9XFxuICAucG9wb3ZlciAuYXJyb3cge1xcbiAgICBwb3NpdGlvbjogYWJzb2x1dGU7XFxuICAgIGRpc3BsYXk6IGJsb2NrO1xcbiAgICB3aWR0aDogMXJlbTtcXG4gICAgaGVpZ2h0OiAwLjVyZW07XFxuICAgIG1hcmdpbjogMCAwLjNyZW07IH1cXG4gICAgLnBvcG92ZXIgLmFycm93OjpiZWZvcmUsIC5wb3BvdmVyIC5hcnJvdzo6YWZ0ZXIge1xcbiAgICAgIHBvc2l0aW9uOiBhYnNvbHV0ZTtcXG4gICAgICBkaXNwbGF5OiBibG9jaztcXG4gICAgICBjb250ZW50OiBcXFwiXFxcIjtcXG4gICAgICBib3JkZXItY29sb3I6IHRyYW5zcGFyZW50O1xcbiAgICAgIGJvcmRlci1zdHlsZTogc29saWQ7IH1cXG5cXG4uYnMtcG9wb3Zlci10b3AsIC5icy1wb3BvdmVyLWF1dG9beC1wbGFjZW1lbnRePVxcXCJ0b3BcXFwiXSB7XFxuICBtYXJnaW4tYm90dG9tOiAwLjVyZW07IH1cXG4gIC5icy1wb3BvdmVyLXRvcCA+IC5hcnJvdywgLmJzLXBvcG92ZXItYXV0b1t4LXBsYWNlbWVudF49XFxcInRvcFxcXCJdID4gLmFycm93IHtcXG4gICAgYm90dG9tOiBjYWxjKCgwLjVyZW0gKyAxcHgpICogLTEpOyB9XFxuICAgIC5icy1wb3BvdmVyLXRvcCA+IC5hcnJvdzo6YmVmb3JlLCAuYnMtcG9wb3Zlci1hdXRvW3gtcGxhY2VtZW50Xj1cXFwidG9wXFxcIl0gPiAuYXJyb3c6OmJlZm9yZSB7XFxuICAgICAgYm90dG9tOiAwO1xcbiAgICAgIGJvcmRlci13aWR0aDogMC41cmVtIDAuNXJlbSAwO1xcbiAgICAgIGJvcmRlci10b3AtY29sb3I6IHJnYmEoMCwgMCwgMCwgMC4yNSk7IH1cXG4gICAgLmJzLXBvcG92ZXItdG9wID4gLmFycm93OjphZnRlciwgLmJzLXBvcG92ZXItYXV0b1t4LXBsYWNlbWVudF49XFxcInRvcFxcXCJdID4gLmFycm93OjphZnRlciB7XFxuICAgICAgYm90dG9tOiAxcHg7XFxuICAgICAgYm9yZGVyLXdpZHRoOiAwLjVyZW0gMC41cmVtIDA7XFxuICAgICAgYm9yZGVyLXRvcC1jb2xvcjogI2ZmZjsgfVxcblxcbi5icy1wb3BvdmVyLXJpZ2h0LCAuYnMtcG9wb3Zlci1hdXRvW3gtcGxhY2VtZW50Xj1cXFwicmlnaHRcXFwiXSB7XFxuICBtYXJnaW4tbGVmdDogMC41cmVtOyB9XFxuICAuYnMtcG9wb3Zlci1yaWdodCA+IC5hcnJvdywgLmJzLXBvcG92ZXItYXV0b1t4LXBsYWNlbWVudF49XFxcInJpZ2h0XFxcIl0gPiAuYXJyb3cge1xcbiAgICBsZWZ0OiBjYWxjKCgwLjVyZW0gKyAxcHgpICogLTEpO1xcbiAgICB3aWR0aDogMC41cmVtO1xcbiAgICBoZWlnaHQ6IDFyZW07XFxuICAgIG1hcmdpbjogMC4zcmVtIDA7IH1cXG4gICAgLmJzLXBvcG92ZXItcmlnaHQgPiAuYXJyb3c6OmJlZm9yZSwgLmJzLXBvcG92ZXItYXV0b1t4LXBsYWNlbWVudF49XFxcInJpZ2h0XFxcIl0gPiAuYXJyb3c6OmJlZm9yZSB7XFxuICAgICAgbGVmdDogMDtcXG4gICAgICBib3JkZXItd2lkdGg6IDAuNXJlbSAwLjVyZW0gMC41cmVtIDA7XFxuICAgICAgYm9yZGVyLXJpZ2h0LWNvbG9yOiByZ2JhKDAsIDAsIDAsIDAuMjUpOyB9XFxuICAgIC5icy1wb3BvdmVyLXJpZ2h0ID4gLmFycm93OjphZnRlciwgLmJzLXBvcG92ZXItYXV0b1t4LXBsYWNlbWVudF49XFxcInJpZ2h0XFxcIl0gPiAuYXJyb3c6OmFmdGVyIHtcXG4gICAgICBsZWZ0OiAxcHg7XFxuICAgICAgYm9yZGVyLXdpZHRoOiAwLjVyZW0gMC41cmVtIDAuNXJlbSAwO1xcbiAgICAgIGJvcmRlci1yaWdodC1jb2xvcjogI2ZmZjsgfVxcblxcbi5icy1wb3BvdmVyLWJvdHRvbSwgLmJzLXBvcG92ZXItYXV0b1t4LXBsYWNlbWVudF49XFxcImJvdHRvbVxcXCJdIHtcXG4gIG1hcmdpbi10b3A6IDAuNXJlbTsgfVxcbiAgLmJzLXBvcG92ZXItYm90dG9tID4gLmFycm93LCAuYnMtcG9wb3Zlci1hdXRvW3gtcGxhY2VtZW50Xj1cXFwiYm90dG9tXFxcIl0gPiAuYXJyb3cge1xcbiAgICB0b3A6IGNhbGMoKDAuNXJlbSArIDFweCkgKiAtMSk7IH1cXG4gICAgLmJzLXBvcG92ZXItYm90dG9tID4gLmFycm93OjpiZWZvcmUsIC5icy1wb3BvdmVyLWF1dG9beC1wbGFjZW1lbnRePVxcXCJib3R0b21cXFwiXSA+IC5hcnJvdzo6YmVmb3JlIHtcXG4gICAgICB0b3A6IDA7XFxuICAgICAgYm9yZGVyLXdpZHRoOiAwIDAuNXJlbSAwLjVyZW0gMC41cmVtO1xcbiAgICAgIGJvcmRlci1ib3R0b20tY29sb3I6IHJnYmEoMCwgMCwgMCwgMC4yNSk7IH1cXG4gICAgLmJzLXBvcG92ZXItYm90dG9tID4gLmFycm93OjphZnRlciwgLmJzLXBvcG92ZXItYXV0b1t4LXBsYWNlbWVudF49XFxcImJvdHRvbVxcXCJdID4gLmFycm93OjphZnRlciB7XFxuICAgICAgdG9wOiAxcHg7XFxuICAgICAgYm9yZGVyLXdpZHRoOiAwIDAuNXJlbSAwLjVyZW0gMC41cmVtO1xcbiAgICAgIGJvcmRlci1ib3R0b20tY29sb3I6ICNmZmY7IH1cXG4gIC5icy1wb3BvdmVyLWJvdHRvbSAucG9wb3Zlci1oZWFkZXI6OmJlZm9yZSwgLmJzLXBvcG92ZXItYXV0b1t4LXBsYWNlbWVudF49XFxcImJvdHRvbVxcXCJdIC5wb3BvdmVyLWhlYWRlcjo6YmVmb3JlIHtcXG4gICAgcG9zaXRpb246IGFic29sdXRlO1xcbiAgICB0b3A6IDA7XFxuICAgIGxlZnQ6IDUwJTtcXG4gICAgZGlzcGxheTogYmxvY2s7XFxuICAgIHdpZHRoOiAxcmVtO1xcbiAgICBtYXJnaW4tbGVmdDogLTAuNXJlbTtcXG4gICAgY29udGVudDogXFxcIlxcXCI7XFxuICAgIGJvcmRlci1ib3R0b206IDFweCBzb2xpZCAjZjdmN2Y3OyB9XFxuXFxuLmJzLXBvcG92ZXItbGVmdCwgLmJzLXBvcG92ZXItYXV0b1t4LXBsYWNlbWVudF49XFxcImxlZnRcXFwiXSB7XFxuICBtYXJnaW4tcmlnaHQ6IDAuNXJlbTsgfVxcbiAgLmJzLXBvcG92ZXItbGVmdCA+IC5hcnJvdywgLmJzLXBvcG92ZXItYXV0b1t4LXBsYWNlbWVudF49XFxcImxlZnRcXFwiXSA+IC5hcnJvdyB7XFxuICAgIHJpZ2h0OiBjYWxjKCgwLjVyZW0gKyAxcHgpICogLTEpO1xcbiAgICB3aWR0aDogMC41cmVtO1xcbiAgICBoZWlnaHQ6IDFyZW07XFxuICAgIG1hcmdpbjogMC4zcmVtIDA7IH1cXG4gICAgLmJzLXBvcG92ZXItbGVmdCA+IC5hcnJvdzo6YmVmb3JlLCAuYnMtcG9wb3Zlci1hdXRvW3gtcGxhY2VtZW50Xj1cXFwibGVmdFxcXCJdID4gLmFycm93OjpiZWZvcmUge1xcbiAgICAgIHJpZ2h0OiAwO1xcbiAgICAgIGJvcmRlci13aWR0aDogMC41cmVtIDAgMC41cmVtIDAuNXJlbTtcXG4gICAgICBib3JkZXItbGVmdC1jb2xvcjogcmdiYSgwLCAwLCAwLCAwLjI1KTsgfVxcbiAgICAuYnMtcG9wb3Zlci1sZWZ0ID4gLmFycm93OjphZnRlciwgLmJzLXBvcG92ZXItYXV0b1t4LXBsYWNlbWVudF49XFxcImxlZnRcXFwiXSA+IC5hcnJvdzo6YWZ0ZXIge1xcbiAgICAgIHJpZ2h0OiAxcHg7XFxuICAgICAgYm9yZGVyLXdpZHRoOiAwLjVyZW0gMCAwLjVyZW0gMC41cmVtO1xcbiAgICAgIGJvcmRlci1sZWZ0LWNvbG9yOiAjZmZmOyB9XFxuXFxuLnBvcG92ZXItaGVhZGVyIHtcXG4gIHBhZGRpbmc6IDAuNXJlbSAwLjc1cmVtO1xcbiAgbWFyZ2luLWJvdHRvbTogMDtcXG4gIGZvbnQtc2l6ZTogMXJlbTtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNmN2Y3Zjc7XFxuICBib3JkZXItYm90dG9tOiAxcHggc29saWQgI2ViZWJlYjtcXG4gIGJvcmRlci10b3AtbGVmdC1yYWRpdXM6IGNhbGMoMC4zcmVtIC0gMXB4KTtcXG4gIGJvcmRlci10b3AtcmlnaHQtcmFkaXVzOiBjYWxjKDAuM3JlbSAtIDFweCk7IH1cXG4gIC5wb3BvdmVyLWhlYWRlcjplbXB0eSB7XFxuICAgIGRpc3BsYXk6IG5vbmU7IH1cXG5cXG4ucG9wb3Zlci1ib2R5IHtcXG4gIHBhZGRpbmc6IDAuNXJlbSAwLjc1cmVtO1xcbiAgY29sb3I6ICMyMTI1Mjk7IH1cXG5cXG5Aa2V5ZnJhbWVzIHByb2dyZXNzLWJhci1zdHJpcGVzIHtcXG4gIGZyb20ge1xcbiAgICBiYWNrZ3JvdW5kLXBvc2l0aW9uOiAxcmVtIDA7IH1cXG4gIHRvIHtcXG4gICAgYmFja2dyb3VuZC1wb3NpdGlvbjogMCAwOyB9IH1cXG5cXG4ucHJvZ3Jlc3Mge1xcbiAgZGlzcGxheTogZmxleDtcXG4gIGhlaWdodDogMXJlbTtcXG4gIG92ZXJmbG93OiBoaWRkZW47XFxuICBmb250LXNpemU6IDAuNzVyZW07XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjZTllY2VmO1xcbiAgYm9yZGVyLXJhZGl1czogMC4yNXJlbTsgfVxcblxcbi5wcm9ncmVzcy1iYXIge1xcbiAgZGlzcGxheTogZmxleDtcXG4gIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW47XFxuICBqdXN0aWZ5LWNvbnRlbnQ6IGNlbnRlcjtcXG4gIGNvbG9yOiAjZmZmO1xcbiAgdGV4dC1hbGlnbjogY2VudGVyO1xcbiAgd2hpdGUtc3BhY2U6IG5vd3JhcDtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMwMDdiZmY7XFxuICB0cmFuc2l0aW9uOiB3aWR0aCAwLjZzIGVhc2U7IH1cXG4gIEBtZWRpYSAocHJlZmVycy1yZWR1Y2VkLW1vdGlvbjogcmVkdWNlKSB7XFxuICAgIC5wcm9ncmVzcy1iYXIge1xcbiAgICAgIHRyYW5zaXRpb246IG5vbmU7IH0gfVxcblxcbi5wcm9ncmVzcy1iYXItc3RyaXBlZCB7XFxuICBiYWNrZ3JvdW5kLWltYWdlOiBsaW5lYXItZ3JhZGllbnQoNDVkZWcsIHJnYmEoMjU1LCAyNTUsIDI1NSwgMC4xNSkgMjUlLCB0cmFuc3BhcmVudCAyNSUsIHRyYW5zcGFyZW50IDUwJSwgcmdiYSgyNTUsIDI1NSwgMjU1LCAwLjE1KSA1MCUsIHJnYmEoMjU1LCAyNTUsIDI1NSwgMC4xNSkgNzUlLCB0cmFuc3BhcmVudCA3NSUsIHRyYW5zcGFyZW50KTtcXG4gIGJhY2tncm91bmQtc2l6ZTogMXJlbSAxcmVtOyB9XFxuXFxuLnByb2dyZXNzLWJhci1hbmltYXRlZCB7XFxuICBhbmltYXRpb246IHByb2dyZXNzLWJhci1zdHJpcGVzIDFzIGxpbmVhciBpbmZpbml0ZTsgfVxcbiAgQG1lZGlhIChwcmVmZXJzLXJlZHVjZWQtbW90aW9uOiByZWR1Y2UpIHtcXG4gICAgLnByb2dyZXNzLWJhci1hbmltYXRlZCB7XFxuICAgICAgYW5pbWF0aW9uOiBub25lOyB9IH1cXG5cXG4uZC1ub25lIHtcXG4gIGRpc3BsYXk6IG5vbmUgIWltcG9ydGFudDsgfVxcblxcbi5kLWlubGluZSB7XFxuICBkaXNwbGF5OiBpbmxpbmUgIWltcG9ydGFudDsgfVxcblxcbi5kLWlubGluZS1ibG9jayB7XFxuICBkaXNwbGF5OiBpbmxpbmUtYmxvY2sgIWltcG9ydGFudDsgfVxcblxcbi5kLWJsb2NrIHtcXG4gIGRpc3BsYXk6IGJsb2NrICFpbXBvcnRhbnQ7IH1cXG5cXG4uZC10YWJsZSB7XFxuICBkaXNwbGF5OiB0YWJsZSAhaW1wb3J0YW50OyB9XFxuXFxuLmQtdGFibGUtcm93IHtcXG4gIGRpc3BsYXk6IHRhYmxlLXJvdyAhaW1wb3J0YW50OyB9XFxuXFxuLmQtdGFibGUtY2VsbCB7XFxuICBkaXNwbGF5OiB0YWJsZS1jZWxsICFpbXBvcnRhbnQ7IH1cXG5cXG4uZC1mbGV4IHtcXG4gIGRpc3BsYXk6IGZsZXggIWltcG9ydGFudDsgfVxcblxcbi5kLWlubGluZS1mbGV4IHtcXG4gIGRpc3BsYXk6IGlubGluZS1mbGV4ICFpbXBvcnRhbnQ7IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogNTc2cHgpIHtcXG4gIC5kLXNtLW5vbmUge1xcbiAgICBkaXNwbGF5OiBub25lICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXNtLWlubGluZSB7XFxuICAgIGRpc3BsYXk6IGlubGluZSAhaW1wb3J0YW50OyB9XFxuICAuZC1zbS1pbmxpbmUtYmxvY2sge1xcbiAgICBkaXNwbGF5OiBpbmxpbmUtYmxvY2sgIWltcG9ydGFudDsgfVxcbiAgLmQtc20tYmxvY2sge1xcbiAgICBkaXNwbGF5OiBibG9jayAhaW1wb3J0YW50OyB9XFxuICAuZC1zbS10YWJsZSB7XFxuICAgIGRpc3BsYXk6IHRhYmxlICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXNtLXRhYmxlLXJvdyB7XFxuICAgIGRpc3BsYXk6IHRhYmxlLXJvdyAhaW1wb3J0YW50OyB9XFxuICAuZC1zbS10YWJsZS1jZWxsIHtcXG4gICAgZGlzcGxheTogdGFibGUtY2VsbCAhaW1wb3J0YW50OyB9XFxuICAuZC1zbS1mbGV4IHtcXG4gICAgZGlzcGxheTogZmxleCAhaW1wb3J0YW50OyB9XFxuICAuZC1zbS1pbmxpbmUtZmxleCB7XFxuICAgIGRpc3BsYXk6IGlubGluZS1mbGV4ICFpbXBvcnRhbnQ7IH0gfVxcblxcbkBtZWRpYSAobWluLXdpZHRoOiA3NjhweCkge1xcbiAgLmQtbWQtbm9uZSB7XFxuICAgIGRpc3BsYXk6IG5vbmUgIWltcG9ydGFudDsgfVxcbiAgLmQtbWQtaW5saW5lIHtcXG4gICAgZGlzcGxheTogaW5saW5lICFpbXBvcnRhbnQ7IH1cXG4gIC5kLW1kLWlubGluZS1ibG9jayB7XFxuICAgIGRpc3BsYXk6IGlubGluZS1ibG9jayAhaW1wb3J0YW50OyB9XFxuICAuZC1tZC1ibG9jayB7XFxuICAgIGRpc3BsYXk6IGJsb2NrICFpbXBvcnRhbnQ7IH1cXG4gIC5kLW1kLXRhYmxlIHtcXG4gICAgZGlzcGxheTogdGFibGUgIWltcG9ydGFudDsgfVxcbiAgLmQtbWQtdGFibGUtcm93IHtcXG4gICAgZGlzcGxheTogdGFibGUtcm93ICFpbXBvcnRhbnQ7IH1cXG4gIC5kLW1kLXRhYmxlLWNlbGwge1xcbiAgICBkaXNwbGF5OiB0YWJsZS1jZWxsICFpbXBvcnRhbnQ7IH1cXG4gIC5kLW1kLWZsZXgge1xcbiAgICBkaXNwbGF5OiBmbGV4ICFpbXBvcnRhbnQ7IH1cXG4gIC5kLW1kLWlubGluZS1mbGV4IHtcXG4gICAgZGlzcGxheTogaW5saW5lLWZsZXggIWltcG9ydGFudDsgfSB9XFxuXFxuQG1lZGlhIChtaW4td2lkdGg6IDk5MnB4KSB7XFxuICAuZC1sZy1ub25lIHtcXG4gICAgZGlzcGxheTogbm9uZSAhaW1wb3J0YW50OyB9XFxuICAuZC1sZy1pbmxpbmUge1xcbiAgICBkaXNwbGF5OiBpbmxpbmUgIWltcG9ydGFudDsgfVxcbiAgLmQtbGctaW5saW5lLWJsb2NrIHtcXG4gICAgZGlzcGxheTogaW5saW5lLWJsb2NrICFpbXBvcnRhbnQ7IH1cXG4gIC5kLWxnLWJsb2NrIHtcXG4gICAgZGlzcGxheTogYmxvY2sgIWltcG9ydGFudDsgfVxcbiAgLmQtbGctdGFibGUge1xcbiAgICBkaXNwbGF5OiB0YWJsZSAhaW1wb3J0YW50OyB9XFxuICAuZC1sZy10YWJsZS1yb3cge1xcbiAgICBkaXNwbGF5OiB0YWJsZS1yb3cgIWltcG9ydGFudDsgfVxcbiAgLmQtbGctdGFibGUtY2VsbCB7XFxuICAgIGRpc3BsYXk6IHRhYmxlLWNlbGwgIWltcG9ydGFudDsgfVxcbiAgLmQtbGctZmxleCB7XFxuICAgIGRpc3BsYXk6IGZsZXggIWltcG9ydGFudDsgfVxcbiAgLmQtbGctaW5saW5lLWZsZXgge1xcbiAgICBkaXNwbGF5OiBpbmxpbmUtZmxleCAhaW1wb3J0YW50OyB9IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogMTIwMHB4KSB7XFxuICAuZC14bC1ub25lIHtcXG4gICAgZGlzcGxheTogbm9uZSAhaW1wb3J0YW50OyB9XFxuICAuZC14bC1pbmxpbmUge1xcbiAgICBkaXNwbGF5OiBpbmxpbmUgIWltcG9ydGFudDsgfVxcbiAgLmQteGwtaW5saW5lLWJsb2NrIHtcXG4gICAgZGlzcGxheTogaW5saW5lLWJsb2NrICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXhsLWJsb2NrIHtcXG4gICAgZGlzcGxheTogYmxvY2sgIWltcG9ydGFudDsgfVxcbiAgLmQteGwtdGFibGUge1xcbiAgICBkaXNwbGF5OiB0YWJsZSAhaW1wb3J0YW50OyB9XFxuICAuZC14bC10YWJsZS1yb3cge1xcbiAgICBkaXNwbGF5OiB0YWJsZS1yb3cgIWltcG9ydGFudDsgfVxcbiAgLmQteGwtdGFibGUtY2VsbCB7XFxuICAgIGRpc3BsYXk6IHRhYmxlLWNlbGwgIWltcG9ydGFudDsgfVxcbiAgLmQteGwtZmxleCB7XFxuICAgIGRpc3BsYXk6IGZsZXggIWltcG9ydGFudDsgfVxcbiAgLmQteGwtaW5saW5lLWZsZXgge1xcbiAgICBkaXNwbGF5OiBpbmxpbmUtZmxleCAhaW1wb3J0YW50OyB9IH1cXG5cXG5AbWVkaWEgcHJpbnQge1xcbiAgLmQtcHJpbnQtbm9uZSB7XFxuICAgIGRpc3BsYXk6IG5vbmUgIWltcG9ydGFudDsgfVxcbiAgLmQtcHJpbnQtaW5saW5lIHtcXG4gICAgZGlzcGxheTogaW5saW5lICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXByaW50LWlubGluZS1ibG9jayB7XFxuICAgIGRpc3BsYXk6IGlubGluZS1ibG9jayAhaW1wb3J0YW50OyB9XFxuICAuZC1wcmludC1ibG9jayB7XFxuICAgIGRpc3BsYXk6IGJsb2NrICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXByaW50LXRhYmxlIHtcXG4gICAgZGlzcGxheTogdGFibGUgIWltcG9ydGFudDsgfVxcbiAgLmQtcHJpbnQtdGFibGUtcm93IHtcXG4gICAgZGlzcGxheTogdGFibGUtcm93ICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXByaW50LXRhYmxlLWNlbGwge1xcbiAgICBkaXNwbGF5OiB0YWJsZS1jZWxsICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXByaW50LWZsZXgge1xcbiAgICBkaXNwbGF5OiBmbGV4ICFpbXBvcnRhbnQ7IH1cXG4gIC5kLXByaW50LWlubGluZS1mbGV4IHtcXG4gICAgZGlzcGxheTogaW5saW5lLWZsZXggIWltcG9ydGFudDsgfSB9XFxuXFxuLmZsZXgtcm93IHtcXG4gIGZsZXgtZGlyZWN0aW9uOiByb3cgIWltcG9ydGFudDsgfVxcblxcbi5mbGV4LWNvbHVtbiB7XFxuICBmbGV4LWRpcmVjdGlvbjogY29sdW1uICFpbXBvcnRhbnQ7IH1cXG5cXG4uZmxleC1yb3ctcmV2ZXJzZSB7XFxuICBmbGV4LWRpcmVjdGlvbjogcm93LXJldmVyc2UgIWltcG9ydGFudDsgfVxcblxcbi5mbGV4LWNvbHVtbi1yZXZlcnNlIHtcXG4gIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW4tcmV2ZXJzZSAhaW1wb3J0YW50OyB9XFxuXFxuLmZsZXgtd3JhcCB7XFxuICBmbGV4LXdyYXA6IHdyYXAgIWltcG9ydGFudDsgfVxcblxcbi5mbGV4LW5vd3JhcCB7XFxuICBmbGV4LXdyYXA6IG5vd3JhcCAhaW1wb3J0YW50OyB9XFxuXFxuLmZsZXgtd3JhcC1yZXZlcnNlIHtcXG4gIGZsZXgtd3JhcDogd3JhcC1yZXZlcnNlICFpbXBvcnRhbnQ7IH1cXG5cXG4uZmxleC1maWxsIHtcXG4gIGZsZXg6IDEgMSBhdXRvICFpbXBvcnRhbnQ7IH1cXG5cXG4uZmxleC1ncm93LTAge1xcbiAgZmxleC1ncm93OiAwICFpbXBvcnRhbnQ7IH1cXG5cXG4uZmxleC1ncm93LTEge1xcbiAgZmxleC1ncm93OiAxICFpbXBvcnRhbnQ7IH1cXG5cXG4uZmxleC1zaHJpbmstMCB7XFxuICBmbGV4LXNocmluazogMCAhaW1wb3J0YW50OyB9XFxuXFxuLmZsZXgtc2hyaW5rLTEge1xcbiAgZmxleC1zaHJpbms6IDEgIWltcG9ydGFudDsgfVxcblxcbi5qdXN0aWZ5LWNvbnRlbnQtc3RhcnQge1xcbiAganVzdGlmeS1jb250ZW50OiBmbGV4LXN0YXJ0ICFpbXBvcnRhbnQ7IH1cXG5cXG4uanVzdGlmeS1jb250ZW50LWVuZCB7XFxuICBqdXN0aWZ5LWNvbnRlbnQ6IGZsZXgtZW5kICFpbXBvcnRhbnQ7IH1cXG5cXG4uanVzdGlmeS1jb250ZW50LWNlbnRlciB7XFxuICBqdXN0aWZ5LWNvbnRlbnQ6IGNlbnRlciAhaW1wb3J0YW50OyB9XFxuXFxuLmp1c3RpZnktY29udGVudC1iZXR3ZWVuIHtcXG4gIGp1c3RpZnktY29udGVudDogc3BhY2UtYmV0d2VlbiAhaW1wb3J0YW50OyB9XFxuXFxuLmp1c3RpZnktY29udGVudC1hcm91bmQge1xcbiAganVzdGlmeS1jb250ZW50OiBzcGFjZS1hcm91bmQgIWltcG9ydGFudDsgfVxcblxcbi5hbGlnbi1pdGVtcy1zdGFydCB7XFxuICBhbGlnbi1pdGVtczogZmxleC1zdGFydCAhaW1wb3J0YW50OyB9XFxuXFxuLmFsaWduLWl0ZW1zLWVuZCB7XFxuICBhbGlnbi1pdGVtczogZmxleC1lbmQgIWltcG9ydGFudDsgfVxcblxcbi5hbGlnbi1pdGVtcy1jZW50ZXIge1xcbiAgYWxpZ24taXRlbXM6IGNlbnRlciAhaW1wb3J0YW50OyB9XFxuXFxuLmFsaWduLWl0ZW1zLWJhc2VsaW5lIHtcXG4gIGFsaWduLWl0ZW1zOiBiYXNlbGluZSAhaW1wb3J0YW50OyB9XFxuXFxuLmFsaWduLWl0ZW1zLXN0cmV0Y2gge1xcbiAgYWxpZ24taXRlbXM6IHN0cmV0Y2ggIWltcG9ydGFudDsgfVxcblxcbi5hbGlnbi1jb250ZW50LXN0YXJ0IHtcXG4gIGFsaWduLWNvbnRlbnQ6IGZsZXgtc3RhcnQgIWltcG9ydGFudDsgfVxcblxcbi5hbGlnbi1jb250ZW50LWVuZCB7XFxuICBhbGlnbi1jb250ZW50OiBmbGV4LWVuZCAhaW1wb3J0YW50OyB9XFxuXFxuLmFsaWduLWNvbnRlbnQtY2VudGVyIHtcXG4gIGFsaWduLWNvbnRlbnQ6IGNlbnRlciAhaW1wb3J0YW50OyB9XFxuXFxuLmFsaWduLWNvbnRlbnQtYmV0d2VlbiB7XFxuICBhbGlnbi1jb250ZW50OiBzcGFjZS1iZXR3ZWVuICFpbXBvcnRhbnQ7IH1cXG5cXG4uYWxpZ24tY29udGVudC1hcm91bmQge1xcbiAgYWxpZ24tY29udGVudDogc3BhY2UtYXJvdW5kICFpbXBvcnRhbnQ7IH1cXG5cXG4uYWxpZ24tY29udGVudC1zdHJldGNoIHtcXG4gIGFsaWduLWNvbnRlbnQ6IHN0cmV0Y2ggIWltcG9ydGFudDsgfVxcblxcbi5hbGlnbi1zZWxmLWF1dG8ge1xcbiAgYWxpZ24tc2VsZjogYXV0byAhaW1wb3J0YW50OyB9XFxuXFxuLmFsaWduLXNlbGYtc3RhcnQge1xcbiAgYWxpZ24tc2VsZjogZmxleC1zdGFydCAhaW1wb3J0YW50OyB9XFxuXFxuLmFsaWduLXNlbGYtZW5kIHtcXG4gIGFsaWduLXNlbGY6IGZsZXgtZW5kICFpbXBvcnRhbnQ7IH1cXG5cXG4uYWxpZ24tc2VsZi1jZW50ZXIge1xcbiAgYWxpZ24tc2VsZjogY2VudGVyICFpbXBvcnRhbnQ7IH1cXG5cXG4uYWxpZ24tc2VsZi1iYXNlbGluZSB7XFxuICBhbGlnbi1zZWxmOiBiYXNlbGluZSAhaW1wb3J0YW50OyB9XFxuXFxuLmFsaWduLXNlbGYtc3RyZXRjaCB7XFxuICBhbGlnbi1zZWxmOiBzdHJldGNoICFpbXBvcnRhbnQ7IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogNTc2cHgpIHtcXG4gIC5mbGV4LXNtLXJvdyB7XFxuICAgIGZsZXgtZGlyZWN0aW9uOiByb3cgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtc20tY29sdW1uIHtcXG4gICAgZmxleC1kaXJlY3Rpb246IGNvbHVtbiAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1zbS1yb3ctcmV2ZXJzZSB7XFxuICAgIGZsZXgtZGlyZWN0aW9uOiByb3ctcmV2ZXJzZSAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1zbS1jb2x1bW4tcmV2ZXJzZSB7XFxuICAgIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW4tcmV2ZXJzZSAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1zbS13cmFwIHtcXG4gICAgZmxleC13cmFwOiB3cmFwICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LXNtLW5vd3JhcCB7XFxuICAgIGZsZXgtd3JhcDogbm93cmFwICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LXNtLXdyYXAtcmV2ZXJzZSB7XFxuICAgIGZsZXgtd3JhcDogd3JhcC1yZXZlcnNlICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LXNtLWZpbGwge1xcbiAgICBmbGV4OiAxIDEgYXV0byAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1zbS1ncm93LTAge1xcbiAgICBmbGV4LWdyb3c6IDAgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtc20tZ3Jvdy0xIHtcXG4gICAgZmxleC1ncm93OiAxICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LXNtLXNocmluay0wIHtcXG4gICAgZmxleC1zaHJpbms6IDAgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtc20tc2hyaW5rLTEge1xcbiAgICBmbGV4LXNocmluazogMSAhaW1wb3J0YW50OyB9XFxuICAuanVzdGlmeS1jb250ZW50LXNtLXN0YXJ0IHtcXG4gICAganVzdGlmeS1jb250ZW50OiBmbGV4LXN0YXJ0ICFpbXBvcnRhbnQ7IH1cXG4gIC5qdXN0aWZ5LWNvbnRlbnQtc20tZW5kIHtcXG4gICAganVzdGlmeS1jb250ZW50OiBmbGV4LWVuZCAhaW1wb3J0YW50OyB9XFxuICAuanVzdGlmeS1jb250ZW50LXNtLWNlbnRlciB7XFxuICAgIGp1c3RpZnktY29udGVudDogY2VudGVyICFpbXBvcnRhbnQ7IH1cXG4gIC5qdXN0aWZ5LWNvbnRlbnQtc20tYmV0d2VlbiB7XFxuICAgIGp1c3RpZnktY29udGVudDogc3BhY2UtYmV0d2VlbiAhaW1wb3J0YW50OyB9XFxuICAuanVzdGlmeS1jb250ZW50LXNtLWFyb3VuZCB7XFxuICAgIGp1c3RpZnktY29udGVudDogc3BhY2UtYXJvdW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1pdGVtcy1zbS1zdGFydCB7XFxuICAgIGFsaWduLWl0ZW1zOiBmbGV4LXN0YXJ0ICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1pdGVtcy1zbS1lbmQge1xcbiAgICBhbGlnbi1pdGVtczogZmxleC1lbmQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWl0ZW1zLXNtLWNlbnRlciB7XFxuICAgIGFsaWduLWl0ZW1zOiBjZW50ZXIgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWl0ZW1zLXNtLWJhc2VsaW5lIHtcXG4gICAgYWxpZ24taXRlbXM6IGJhc2VsaW5lICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1pdGVtcy1zbS1zdHJldGNoIHtcXG4gICAgYWxpZ24taXRlbXM6IHN0cmV0Y2ggIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQtc20tc3RhcnQge1xcbiAgICBhbGlnbi1jb250ZW50OiBmbGV4LXN0YXJ0ICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LXNtLWVuZCB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IGZsZXgtZW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LXNtLWNlbnRlciB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IGNlbnRlciAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC1zbS1iZXR3ZWVuIHtcXG4gICAgYWxpZ24tY29udGVudDogc3BhY2UtYmV0d2VlbiAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC1zbS1hcm91bmQge1xcbiAgICBhbGlnbi1jb250ZW50OiBzcGFjZS1hcm91bmQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQtc20tc3RyZXRjaCB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IHN0cmV0Y2ggIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLXNlbGYtc20tYXV0byB7XFxuICAgIGFsaWduLXNlbGY6IGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLXNlbGYtc20tc3RhcnQge1xcbiAgICBhbGlnbi1zZWxmOiBmbGV4LXN0YXJ0ICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLXNtLWVuZCB7XFxuICAgIGFsaWduLXNlbGY6IGZsZXgtZW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLXNtLWNlbnRlciB7XFxuICAgIGFsaWduLXNlbGY6IGNlbnRlciAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi1zbS1iYXNlbGluZSB7XFxuICAgIGFsaWduLXNlbGY6IGJhc2VsaW5lICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLXNtLXN0cmV0Y2gge1xcbiAgICBhbGlnbi1zZWxmOiBzdHJldGNoICFpbXBvcnRhbnQ7IH0gfVxcblxcbkBtZWRpYSAobWluLXdpZHRoOiA3NjhweCkge1xcbiAgLmZsZXgtbWQtcm93IHtcXG4gICAgZmxleC1kaXJlY3Rpb246IHJvdyAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1tZC1jb2x1bW4ge1xcbiAgICBmbGV4LWRpcmVjdGlvbjogY29sdW1uICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LW1kLXJvdy1yZXZlcnNlIHtcXG4gICAgZmxleC1kaXJlY3Rpb246IHJvdy1yZXZlcnNlICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LW1kLWNvbHVtbi1yZXZlcnNlIHtcXG4gICAgZmxleC1kaXJlY3Rpb246IGNvbHVtbi1yZXZlcnNlICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LW1kLXdyYXAge1xcbiAgICBmbGV4LXdyYXA6IHdyYXAgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtbWQtbm93cmFwIHtcXG4gICAgZmxleC13cmFwOiBub3dyYXAgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtbWQtd3JhcC1yZXZlcnNlIHtcXG4gICAgZmxleC13cmFwOiB3cmFwLXJldmVyc2UgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtbWQtZmlsbCB7XFxuICAgIGZsZXg6IDEgMSBhdXRvICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LW1kLWdyb3ctMCB7XFxuICAgIGZsZXgtZ3JvdzogMCAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1tZC1ncm93LTEge1xcbiAgICBmbGV4LWdyb3c6IDEgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtbWQtc2hyaW5rLTAge1xcbiAgICBmbGV4LXNocmluazogMCAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1tZC1zaHJpbmstMSB7XFxuICAgIGZsZXgtc2hyaW5rOiAxICFpbXBvcnRhbnQ7IH1cXG4gIC5qdXN0aWZ5LWNvbnRlbnQtbWQtc3RhcnQge1xcbiAgICBqdXN0aWZ5LWNvbnRlbnQ6IGZsZXgtc3RhcnQgIWltcG9ydGFudDsgfVxcbiAgLmp1c3RpZnktY29udGVudC1tZC1lbmQge1xcbiAgICBqdXN0aWZ5LWNvbnRlbnQ6IGZsZXgtZW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5qdXN0aWZ5LWNvbnRlbnQtbWQtY2VudGVyIHtcXG4gICAganVzdGlmeS1jb250ZW50OiBjZW50ZXIgIWltcG9ydGFudDsgfVxcbiAgLmp1c3RpZnktY29udGVudC1tZC1iZXR3ZWVuIHtcXG4gICAganVzdGlmeS1jb250ZW50OiBzcGFjZS1iZXR3ZWVuICFpbXBvcnRhbnQ7IH1cXG4gIC5qdXN0aWZ5LWNvbnRlbnQtbWQtYXJvdW5kIHtcXG4gICAganVzdGlmeS1jb250ZW50OiBzcGFjZS1hcm91bmQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWl0ZW1zLW1kLXN0YXJ0IHtcXG4gICAgYWxpZ24taXRlbXM6IGZsZXgtc3RhcnQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWl0ZW1zLW1kLWVuZCB7XFxuICAgIGFsaWduLWl0ZW1zOiBmbGV4LWVuZCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24taXRlbXMtbWQtY2VudGVyIHtcXG4gICAgYWxpZ24taXRlbXM6IGNlbnRlciAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24taXRlbXMtbWQtYmFzZWxpbmUge1xcbiAgICBhbGlnbi1pdGVtczogYmFzZWxpbmUgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWl0ZW1zLW1kLXN0cmV0Y2gge1xcbiAgICBhbGlnbi1pdGVtczogc3RyZXRjaCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC1tZC1zdGFydCB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IGZsZXgtc3RhcnQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQtbWQtZW5kIHtcXG4gICAgYWxpZ24tY29udGVudDogZmxleC1lbmQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQtbWQtY2VudGVyIHtcXG4gICAgYWxpZ24tY29udGVudDogY2VudGVyICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LW1kLWJldHdlZW4ge1xcbiAgICBhbGlnbi1jb250ZW50OiBzcGFjZS1iZXR3ZWVuICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LW1kLWFyb3VuZCB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IHNwYWNlLWFyb3VuZCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC1tZC1zdHJldGNoIHtcXG4gICAgYWxpZ24tY29udGVudDogc3RyZXRjaCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi1tZC1hdXRvIHtcXG4gICAgYWxpZ24tc2VsZjogYXV0byAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi1tZC1zdGFydCB7XFxuICAgIGFsaWduLXNlbGY6IGZsZXgtc3RhcnQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLXNlbGYtbWQtZW5kIHtcXG4gICAgYWxpZ24tc2VsZjogZmxleC1lbmQgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLXNlbGYtbWQtY2VudGVyIHtcXG4gICAgYWxpZ24tc2VsZjogY2VudGVyICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLW1kLWJhc2VsaW5lIHtcXG4gICAgYWxpZ24tc2VsZjogYmFzZWxpbmUgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLXNlbGYtbWQtc3RyZXRjaCB7XFxuICAgIGFsaWduLXNlbGY6IHN0cmV0Y2ggIWltcG9ydGFudDsgfSB9XFxuXFxuQG1lZGlhIChtaW4td2lkdGg6IDk5MnB4KSB7XFxuICAuZmxleC1sZy1yb3cge1xcbiAgICBmbGV4LWRpcmVjdGlvbjogcm93ICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LWxnLWNvbHVtbiB7XFxuICAgIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW4gIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtbGctcm93LXJldmVyc2Uge1xcbiAgICBmbGV4LWRpcmVjdGlvbjogcm93LXJldmVyc2UgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtbGctY29sdW1uLXJldmVyc2Uge1xcbiAgICBmbGV4LWRpcmVjdGlvbjogY29sdW1uLXJldmVyc2UgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtbGctd3JhcCB7XFxuICAgIGZsZXgtd3JhcDogd3JhcCAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1sZy1ub3dyYXAge1xcbiAgICBmbGV4LXdyYXA6IG5vd3JhcCAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1sZy13cmFwLXJldmVyc2Uge1xcbiAgICBmbGV4LXdyYXA6IHdyYXAtcmV2ZXJzZSAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1sZy1maWxsIHtcXG4gICAgZmxleDogMSAxIGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLmZsZXgtbGctZ3Jvdy0wIHtcXG4gICAgZmxleC1ncm93OiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LWxnLWdyb3ctMSB7XFxuICAgIGZsZXgtZ3JvdzogMSAhaW1wb3J0YW50OyB9XFxuICAuZmxleC1sZy1zaHJpbmstMCB7XFxuICAgIGZsZXgtc2hyaW5rOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LWxnLXNocmluay0xIHtcXG4gICAgZmxleC1zaHJpbms6IDEgIWltcG9ydGFudDsgfVxcbiAgLmp1c3RpZnktY29udGVudC1sZy1zdGFydCB7XFxuICAgIGp1c3RpZnktY29udGVudDogZmxleC1zdGFydCAhaW1wb3J0YW50OyB9XFxuICAuanVzdGlmeS1jb250ZW50LWxnLWVuZCB7XFxuICAgIGp1c3RpZnktY29udGVudDogZmxleC1lbmQgIWltcG9ydGFudDsgfVxcbiAgLmp1c3RpZnktY29udGVudC1sZy1jZW50ZXIge1xcbiAgICBqdXN0aWZ5LWNvbnRlbnQ6IGNlbnRlciAhaW1wb3J0YW50OyB9XFxuICAuanVzdGlmeS1jb250ZW50LWxnLWJldHdlZW4ge1xcbiAgICBqdXN0aWZ5LWNvbnRlbnQ6IHNwYWNlLWJldHdlZW4gIWltcG9ydGFudDsgfVxcbiAgLmp1c3RpZnktY29udGVudC1sZy1hcm91bmQge1xcbiAgICBqdXN0aWZ5LWNvbnRlbnQ6IHNwYWNlLWFyb3VuZCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24taXRlbXMtbGctc3RhcnQge1xcbiAgICBhbGlnbi1pdGVtczogZmxleC1zdGFydCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24taXRlbXMtbGctZW5kIHtcXG4gICAgYWxpZ24taXRlbXM6IGZsZXgtZW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1pdGVtcy1sZy1jZW50ZXIge1xcbiAgICBhbGlnbi1pdGVtczogY2VudGVyICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1pdGVtcy1sZy1iYXNlbGluZSB7XFxuICAgIGFsaWduLWl0ZW1zOiBiYXNlbGluZSAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24taXRlbXMtbGctc3RyZXRjaCB7XFxuICAgIGFsaWduLWl0ZW1zOiBzdHJldGNoICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LWxnLXN0YXJ0IHtcXG4gICAgYWxpZ24tY29udGVudDogZmxleC1zdGFydCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC1sZy1lbmQge1xcbiAgICBhbGlnbi1jb250ZW50OiBmbGV4LWVuZCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC1sZy1jZW50ZXIge1xcbiAgICBhbGlnbi1jb250ZW50OiBjZW50ZXIgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQtbGctYmV0d2VlbiB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IHNwYWNlLWJldHdlZW4gIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQtbGctYXJvdW5kIHtcXG4gICAgYWxpZ24tY29udGVudDogc3BhY2UtYXJvdW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LWxnLXN0cmV0Y2gge1xcbiAgICBhbGlnbi1jb250ZW50OiBzdHJldGNoICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLWxnLWF1dG8ge1xcbiAgICBhbGlnbi1zZWxmOiBhdXRvICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLWxnLXN0YXJ0IHtcXG4gICAgYWxpZ24tc2VsZjogZmxleC1zdGFydCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi1sZy1lbmQge1xcbiAgICBhbGlnbi1zZWxmOiBmbGV4LWVuZCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi1sZy1jZW50ZXIge1xcbiAgICBhbGlnbi1zZWxmOiBjZW50ZXIgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLXNlbGYtbGctYmFzZWxpbmUge1xcbiAgICBhbGlnbi1zZWxmOiBiYXNlbGluZSAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi1sZy1zdHJldGNoIHtcXG4gICAgYWxpZ24tc2VsZjogc3RyZXRjaCAhaW1wb3J0YW50OyB9IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogMTIwMHB4KSB7XFxuICAuZmxleC14bC1yb3cge1xcbiAgICBmbGV4LWRpcmVjdGlvbjogcm93ICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LXhsLWNvbHVtbiB7XFxuICAgIGZsZXgtZGlyZWN0aW9uOiBjb2x1bW4gIWltcG9ydGFudDsgfVxcbiAgLmZsZXgteGwtcm93LXJldmVyc2Uge1xcbiAgICBmbGV4LWRpcmVjdGlvbjogcm93LXJldmVyc2UgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgteGwtY29sdW1uLXJldmVyc2Uge1xcbiAgICBmbGV4LWRpcmVjdGlvbjogY29sdW1uLXJldmVyc2UgIWltcG9ydGFudDsgfVxcbiAgLmZsZXgteGwtd3JhcCB7XFxuICAgIGZsZXgtd3JhcDogd3JhcCAhaW1wb3J0YW50OyB9XFxuICAuZmxleC14bC1ub3dyYXAge1xcbiAgICBmbGV4LXdyYXA6IG5vd3JhcCAhaW1wb3J0YW50OyB9XFxuICAuZmxleC14bC13cmFwLXJldmVyc2Uge1xcbiAgICBmbGV4LXdyYXA6IHdyYXAtcmV2ZXJzZSAhaW1wb3J0YW50OyB9XFxuICAuZmxleC14bC1maWxsIHtcXG4gICAgZmxleDogMSAxIGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLmZsZXgteGwtZ3Jvdy0wIHtcXG4gICAgZmxleC1ncm93OiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LXhsLWdyb3ctMSB7XFxuICAgIGZsZXgtZ3JvdzogMSAhaW1wb3J0YW50OyB9XFxuICAuZmxleC14bC1zaHJpbmstMCB7XFxuICAgIGZsZXgtc2hyaW5rOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5mbGV4LXhsLXNocmluay0xIHtcXG4gICAgZmxleC1zaHJpbms6IDEgIWltcG9ydGFudDsgfVxcbiAgLmp1c3RpZnktY29udGVudC14bC1zdGFydCB7XFxuICAgIGp1c3RpZnktY29udGVudDogZmxleC1zdGFydCAhaW1wb3J0YW50OyB9XFxuICAuanVzdGlmeS1jb250ZW50LXhsLWVuZCB7XFxuICAgIGp1c3RpZnktY29udGVudDogZmxleC1lbmQgIWltcG9ydGFudDsgfVxcbiAgLmp1c3RpZnktY29udGVudC14bC1jZW50ZXIge1xcbiAgICBqdXN0aWZ5LWNvbnRlbnQ6IGNlbnRlciAhaW1wb3J0YW50OyB9XFxuICAuanVzdGlmeS1jb250ZW50LXhsLWJldHdlZW4ge1xcbiAgICBqdXN0aWZ5LWNvbnRlbnQ6IHNwYWNlLWJldHdlZW4gIWltcG9ydGFudDsgfVxcbiAgLmp1c3RpZnktY29udGVudC14bC1hcm91bmQge1xcbiAgICBqdXN0aWZ5LWNvbnRlbnQ6IHNwYWNlLWFyb3VuZCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24taXRlbXMteGwtc3RhcnQge1xcbiAgICBhbGlnbi1pdGVtczogZmxleC1zdGFydCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24taXRlbXMteGwtZW5kIHtcXG4gICAgYWxpZ24taXRlbXM6IGZsZXgtZW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1pdGVtcy14bC1jZW50ZXIge1xcbiAgICBhbGlnbi1pdGVtczogY2VudGVyICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1pdGVtcy14bC1iYXNlbGluZSB7XFxuICAgIGFsaWduLWl0ZW1zOiBiYXNlbGluZSAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24taXRlbXMteGwtc3RyZXRjaCB7XFxuICAgIGFsaWduLWl0ZW1zOiBzdHJldGNoICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LXhsLXN0YXJ0IHtcXG4gICAgYWxpZ24tY29udGVudDogZmxleC1zdGFydCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC14bC1lbmQge1xcbiAgICBhbGlnbi1jb250ZW50OiBmbGV4LWVuZCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tY29udGVudC14bC1jZW50ZXIge1xcbiAgICBhbGlnbi1jb250ZW50OiBjZW50ZXIgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQteGwtYmV0d2VlbiB7XFxuICAgIGFsaWduLWNvbnRlbnQ6IHNwYWNlLWJldHdlZW4gIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLWNvbnRlbnQteGwtYXJvdW5kIHtcXG4gICAgYWxpZ24tY29udGVudDogc3BhY2UtYXJvdW5kICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1jb250ZW50LXhsLXN0cmV0Y2gge1xcbiAgICBhbGlnbi1jb250ZW50OiBzdHJldGNoICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLXhsLWF1dG8ge1xcbiAgICBhbGlnbi1zZWxmOiBhdXRvICFpbXBvcnRhbnQ7IH1cXG4gIC5hbGlnbi1zZWxmLXhsLXN0YXJ0IHtcXG4gICAgYWxpZ24tc2VsZjogZmxleC1zdGFydCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi14bC1lbmQge1xcbiAgICBhbGlnbi1zZWxmOiBmbGV4LWVuZCAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi14bC1jZW50ZXIge1xcbiAgICBhbGlnbi1zZWxmOiBjZW50ZXIgIWltcG9ydGFudDsgfVxcbiAgLmFsaWduLXNlbGYteGwtYmFzZWxpbmUge1xcbiAgICBhbGlnbi1zZWxmOiBiYXNlbGluZSAhaW1wb3J0YW50OyB9XFxuICAuYWxpZ24tc2VsZi14bC1zdHJldGNoIHtcXG4gICAgYWxpZ24tc2VsZjogc3RyZXRjaCAhaW1wb3J0YW50OyB9IH1cXG5cXG4ubS0wIHtcXG4gIG1hcmdpbjogMCAhaW1wb3J0YW50OyB9XFxuXFxuLm10LTAsXFxuLm15LTAge1xcbiAgbWFyZ2luLXRvcDogMCAhaW1wb3J0YW50OyB9XFxuXFxuLm1yLTAsXFxuLm14LTAge1xcbiAgbWFyZ2luLXJpZ2h0OiAwICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWItMCxcXG4ubXktMCB7XFxuICBtYXJnaW4tYm90dG9tOiAwICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWwtMCxcXG4ubXgtMCB7XFxuICBtYXJnaW4tbGVmdDogMCAhaW1wb3J0YW50OyB9XFxuXFxuLm0tMSB7XFxuICBtYXJnaW46IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tdC0xLFxcbi5teS0xIHtcXG4gIG1hcmdpbi10b3A6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tci0xLFxcbi5teC0xIHtcXG4gIG1hcmdpbi1yaWdodDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1iLTEsXFxuLm15LTEge1xcbiAgbWFyZ2luLWJvdHRvbTogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1sLTEsXFxuLm14LTEge1xcbiAgbWFyZ2luLWxlZnQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tLTIge1xcbiAgbWFyZ2luOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tdC0yLFxcbi5teS0yIHtcXG4gIG1hcmdpbi10b3A6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1yLTIsXFxuLm14LTIge1xcbiAgbWFyZ2luLXJpZ2h0OiAwLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tYi0yLFxcbi5teS0yIHtcXG4gIG1hcmdpbi1ib3R0b206IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1sLTIsXFxuLm14LTIge1xcbiAgbWFyZ2luLWxlZnQ6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm0tMyB7XFxuICBtYXJnaW46IDFyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tdC0zLFxcbi5teS0zIHtcXG4gIG1hcmdpbi10b3A6IDFyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tci0zLFxcbi5teC0zIHtcXG4gIG1hcmdpbi1yaWdodDogMXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1iLTMsXFxuLm15LTMge1xcbiAgbWFyZ2luLWJvdHRvbTogMXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1sLTMsXFxuLm14LTMge1xcbiAgbWFyZ2luLWxlZnQ6IDFyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tLTQge1xcbiAgbWFyZ2luOiAxLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tdC00LFxcbi5teS00IHtcXG4gIG1hcmdpbi10b3A6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1yLTQsXFxuLm14LTQge1xcbiAgbWFyZ2luLXJpZ2h0OiAxLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tYi00LFxcbi5teS00IHtcXG4gIG1hcmdpbi1ib3R0b206IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1sLTQsXFxuLm14LTQge1xcbiAgbWFyZ2luLWxlZnQ6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm0tNSB7XFxuICBtYXJnaW46IDNyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tdC01LFxcbi5teS01IHtcXG4gIG1hcmdpbi10b3A6IDNyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tci01LFxcbi5teC01IHtcXG4gIG1hcmdpbi1yaWdodDogM3JlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1iLTUsXFxuLm15LTUge1xcbiAgbWFyZ2luLWJvdHRvbTogM3JlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1sLTUsXFxuLm14LTUge1xcbiAgbWFyZ2luLWxlZnQ6IDNyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wLTAge1xcbiAgcGFkZGluZzogMCAhaW1wb3J0YW50OyB9XFxuXFxuLnB0LTAsXFxuLnB5LTAge1xcbiAgcGFkZGluZy10b3A6IDAgIWltcG9ydGFudDsgfVxcblxcbi5wci0wLFxcbi5weC0wIHtcXG4gIHBhZGRpbmctcmlnaHQ6IDAgIWltcG9ydGFudDsgfVxcblxcbi5wYi0wLFxcbi5weS0wIHtcXG4gIHBhZGRpbmctYm90dG9tOiAwICFpbXBvcnRhbnQ7IH1cXG5cXG4ucGwtMCxcXG4ucHgtMCB7XFxuICBwYWRkaW5nLWxlZnQ6IDAgIWltcG9ydGFudDsgfVxcblxcbi5wLTEge1xcbiAgcGFkZGluZzogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLnB0LTEsXFxuLnB5LTEge1xcbiAgcGFkZGluZy10b3A6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wci0xLFxcbi5weC0xIHtcXG4gIHBhZGRpbmctcmlnaHQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wYi0xLFxcbi5weS0xIHtcXG4gIHBhZGRpbmctYm90dG9tOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucGwtMSxcXG4ucHgtMSB7XFxuICBwYWRkaW5nLWxlZnQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wLTIge1xcbiAgcGFkZGluZzogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucHQtMixcXG4ucHktMiB7XFxuICBwYWRkaW5nLXRvcDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucHItMixcXG4ucHgtMiB7XFxuICBwYWRkaW5nLXJpZ2h0OiAwLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wYi0yLFxcbi5weS0yIHtcXG4gIHBhZGRpbmctYm90dG9tOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wbC0yLFxcbi5weC0yIHtcXG4gIHBhZGRpbmctbGVmdDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucC0zIHtcXG4gIHBhZGRpbmc6IDFyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wdC0zLFxcbi5weS0zIHtcXG4gIHBhZGRpbmctdG9wOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucHItMyxcXG4ucHgtMyB7XFxuICBwYWRkaW5nLXJpZ2h0OiAxcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucGItMyxcXG4ucHktMyB7XFxuICBwYWRkaW5nLWJvdHRvbTogMXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLnBsLTMsXFxuLnB4LTMge1xcbiAgcGFkZGluZy1sZWZ0OiAxcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucC00IHtcXG4gIHBhZGRpbmc6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLnB0LTQsXFxuLnB5LTQge1xcbiAgcGFkZGluZy10b3A6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLnByLTQsXFxuLnB4LTQge1xcbiAgcGFkZGluZy1yaWdodDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucGItNCxcXG4ucHktNCB7XFxuICBwYWRkaW5nLWJvdHRvbTogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucGwtNCxcXG4ucHgtNCB7XFxuICBwYWRkaW5nLWxlZnQ6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLnAtNSB7XFxuICBwYWRkaW5nOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ucHQtNSxcXG4ucHktNSB7XFxuICBwYWRkaW5nLXRvcDogM3JlbSAhaW1wb3J0YW50OyB9XFxuXFxuLnByLTUsXFxuLnB4LTUge1xcbiAgcGFkZGluZy1yaWdodDogM3JlbSAhaW1wb3J0YW50OyB9XFxuXFxuLnBiLTUsXFxuLnB5LTUge1xcbiAgcGFkZGluZy1ib3R0b206IDNyZW0gIWltcG9ydGFudDsgfVxcblxcbi5wbC01LFxcbi5weC01IHtcXG4gIHBhZGRpbmctbGVmdDogM3JlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm0tbjEge1xcbiAgbWFyZ2luOiAtMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm10LW4xLFxcbi5teS1uMSB7XFxuICBtYXJnaW4tdG9wOiAtMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1yLW4xLFxcbi5teC1uMSB7XFxuICBtYXJnaW4tcmlnaHQ6IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWItbjEsXFxuLm15LW4xIHtcXG4gIG1hcmdpbi1ib3R0b206IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWwtbjEsXFxuLm14LW4xIHtcXG4gIG1hcmdpbi1sZWZ0OiAtMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm0tbjIge1xcbiAgbWFyZ2luOiAtMC41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubXQtbjIsXFxuLm15LW4yIHtcXG4gIG1hcmdpbi10b3A6IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tci1uMixcXG4ubXgtbjIge1xcbiAgbWFyZ2luLXJpZ2h0OiAtMC41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWItbjIsXFxuLm15LW4yIHtcXG4gIG1hcmdpbi1ib3R0b206IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tbC1uMixcXG4ubXgtbjIge1xcbiAgbWFyZ2luLWxlZnQ6IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tLW4zIHtcXG4gIG1hcmdpbjogLTFyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tdC1uMyxcXG4ubXktbjMge1xcbiAgbWFyZ2luLXRvcDogLTFyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tci1uMyxcXG4ubXgtbjMge1xcbiAgbWFyZ2luLXJpZ2h0OiAtMXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1iLW4zLFxcbi5teS1uMyB7XFxuICBtYXJnaW4tYm90dG9tOiAtMXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1sLW4zLFxcbi5teC1uMyB7XFxuICBtYXJnaW4tbGVmdDogLTFyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tLW40IHtcXG4gIG1hcmdpbjogLTEuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm10LW40LFxcbi5teS1uNCB7XFxuICBtYXJnaW4tdG9wOiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubXItbjQsXFxuLm14LW40IHtcXG4gIG1hcmdpbi1yaWdodDogLTEuNXJlbSAhaW1wb3J0YW50OyB9XFxuXFxuLm1iLW40LFxcbi5teS1uNCB7XFxuICBtYXJnaW4tYm90dG9tOiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWwtbjQsXFxuLm14LW40IHtcXG4gIG1hcmdpbi1sZWZ0OiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubS1uNSB7XFxuICBtYXJnaW46IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubXQtbjUsXFxuLm15LW41IHtcXG4gIG1hcmdpbi10b3A6IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubXItbjUsXFxuLm14LW41IHtcXG4gIG1hcmdpbi1yaWdodDogLTNyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tYi1uNSxcXG4ubXktbjUge1xcbiAgbWFyZ2luLWJvdHRvbTogLTNyZW0gIWltcG9ydGFudDsgfVxcblxcbi5tbC1uNSxcXG4ubXgtbjUge1xcbiAgbWFyZ2luLWxlZnQ6IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG5cXG4ubS1hdXRvIHtcXG4gIG1hcmdpbjogYXV0byAhaW1wb3J0YW50OyB9XFxuXFxuLm10LWF1dG8sXFxuLm15LWF1dG8ge1xcbiAgbWFyZ2luLXRvcDogYXV0byAhaW1wb3J0YW50OyB9XFxuXFxuLm1yLWF1dG8sXFxuLm14LWF1dG8ge1xcbiAgbWFyZ2luLXJpZ2h0OiBhdXRvICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWItYXV0byxcXG4ubXktYXV0byB7XFxuICBtYXJnaW4tYm90dG9tOiBhdXRvICFpbXBvcnRhbnQ7IH1cXG5cXG4ubWwtYXV0byxcXG4ubXgtYXV0byB7XFxuICBtYXJnaW4tbGVmdDogYXV0byAhaW1wb3J0YW50OyB9XFxuXFxuQG1lZGlhIChtaW4td2lkdGg6IDU3NnB4KSB7XFxuICAubS1zbS0wIHtcXG4gICAgbWFyZ2luOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1zbS0wLFxcbiAgLm15LXNtLTAge1xcbiAgICBtYXJnaW4tdG9wOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1zbS0wLFxcbiAgLm14LXNtLTAge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDAgIWltcG9ydGFudDsgfVxcbiAgLm1iLXNtLTAsXFxuICAubXktc20tMCB7XFxuICAgIG1hcmdpbi1ib3R0b206IDAgIWltcG9ydGFudDsgfVxcbiAgLm1sLXNtLTAsXFxuICAubXgtc20tMCB7XFxuICAgIG1hcmdpbi1sZWZ0OiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5tLXNtLTEge1xcbiAgICBtYXJnaW46IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LXNtLTEsXFxuICAubXktc20tMSB7XFxuICAgIG1hcmdpbi10b3A6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLXNtLTEsXFxuICAubXgtc20tMSB7XFxuICAgIG1hcmdpbi1yaWdodDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWItc20tMSxcXG4gIC5teS1zbS0xIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtc20tMSxcXG4gIC5teC1zbS0xIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tc20tMiB7XFxuICAgIG1hcmdpbjogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1zbS0yLFxcbiAgLm15LXNtLTIge1xcbiAgICBtYXJnaW4tdG9wOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLXNtLTIsXFxuICAubXgtc20tMiB7XFxuICAgIG1hcmdpbi1yaWdodDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1zbS0yLFxcbiAgLm15LXNtLTIge1xcbiAgICBtYXJnaW4tYm90dG9tOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXNtLTIsXFxuICAubXgtc20tMiB7XFxuICAgIG1hcmdpbi1sZWZ0OiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tc20tMyB7XFxuICAgIG1hcmdpbjogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXQtc20tMyxcXG4gIC5teS1zbS0zIHtcXG4gICAgbWFyZ2luLXRvcDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItc20tMyxcXG4gIC5teC1zbS0zIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1zbS0zLFxcbiAgLm15LXNtLTMge1xcbiAgICBtYXJnaW4tYm90dG9tOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC1zbS0zLFxcbiAgLm14LXNtLTMge1xcbiAgICBtYXJnaW4tbGVmdDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1zbS00IHtcXG4gICAgbWFyZ2luOiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LXNtLTQsXFxuICAubXktc20tNCB7XFxuICAgIG1hcmdpbi10b3A6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItc20tNCxcXG4gIC5teC1zbS00IHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLXNtLTQsXFxuICAubXktc20tNCB7XFxuICAgIG1hcmdpbi1ib3R0b206IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtc20tNCxcXG4gIC5teC1zbS00IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1zbS01IHtcXG4gICAgbWFyZ2luOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1zbS01LFxcbiAgLm15LXNtLTUge1xcbiAgICBtYXJnaW4tdG9wOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1zbS01LFxcbiAgLm14LXNtLTUge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLXNtLTUsXFxuICAubXktc20tNSB7XFxuICAgIG1hcmdpbi1ib3R0b206IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXNtLTUsXFxuICAubXgtc20tNSB7XFxuICAgIG1hcmdpbi1sZWZ0OiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wLXNtLTAge1xcbiAgICBwYWRkaW5nOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1zbS0wLFxcbiAgLnB5LXNtLTAge1xcbiAgICBwYWRkaW5nLXRvcDogMCAhaW1wb3J0YW50OyB9XFxuICAucHItc20tMCxcXG4gIC5weC1zbS0wIHtcXG4gICAgcGFkZGluZy1yaWdodDogMCAhaW1wb3J0YW50OyB9XFxuICAucGItc20tMCxcXG4gIC5weS1zbS0wIHtcXG4gICAgcGFkZGluZy1ib3R0b206IDAgIWltcG9ydGFudDsgfVxcbiAgLnBsLXNtLTAsXFxuICAucHgtc20tMCB7XFxuICAgIHBhZGRpbmctbGVmdDogMCAhaW1wb3J0YW50OyB9XFxuICAucC1zbS0xIHtcXG4gICAgcGFkZGluZzogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQtc20tMSxcXG4gIC5weS1zbS0xIHtcXG4gICAgcGFkZGluZy10b3A6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnByLXNtLTEsXFxuICAucHgtc20tMSB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLXNtLTEsXFxuICAucHktc20tMSB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC1zbS0xLFxcbiAgLnB4LXNtLTEge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnAtc20tMiB7XFxuICAgIHBhZGRpbmc6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQtc20tMixcXG4gIC5weS1zbS0yIHtcXG4gICAgcGFkZGluZy10b3A6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHItc20tMixcXG4gIC5weC1zbS0yIHtcXG4gICAgcGFkZGluZy1yaWdodDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wYi1zbS0yLFxcbiAgLnB5LXNtLTIge1xcbiAgICBwYWRkaW5nLWJvdHRvbTogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC1zbS0yLFxcbiAgLnB4LXNtLTIge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucC1zbS0zIHtcXG4gICAgcGFkZGluZzogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQtc20tMyxcXG4gIC5weS1zbS0zIHtcXG4gICAgcGFkZGluZy10b3A6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnByLXNtLTMsXFxuICAucHgtc20tMyB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLXNtLTMsXFxuICAucHktc20tMyB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC1zbS0zLFxcbiAgLnB4LXNtLTMge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnAtc20tNCB7XFxuICAgIHBhZGRpbmc6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQtc20tNCxcXG4gIC5weS1zbS00IHtcXG4gICAgcGFkZGluZy10b3A6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHItc20tNCxcXG4gIC5weC1zbS00IHtcXG4gICAgcGFkZGluZy1yaWdodDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wYi1zbS00LFxcbiAgLnB5LXNtLTQge1xcbiAgICBwYWRkaW5nLWJvdHRvbTogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC1zbS00LFxcbiAgLnB4LXNtLTQge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucC1zbS01IHtcXG4gICAgcGFkZGluZzogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAucHQtc20tNSxcXG4gIC5weS1zbS01IHtcXG4gICAgcGFkZGluZy10b3A6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLnByLXNtLTUsXFxuICAucHgtc20tNSB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLXNtLTUsXFxuICAucHktc20tNSB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC1zbS01LFxcbiAgLnB4LXNtLTUge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tc20tbjEge1xcbiAgICBtYXJnaW46IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1zbS1uMSxcXG4gIC5teS1zbS1uMSB7XFxuICAgIG1hcmdpbi10b3A6IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1zbS1uMSxcXG4gIC5teC1zbS1uMSB7XFxuICAgIG1hcmdpbi1yaWdodDogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLXNtLW4xLFxcbiAgLm15LXNtLW4xIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXNtLW4xLFxcbiAgLm14LXNtLW4xIHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLXNtLW4yIHtcXG4gICAgbWFyZ2luOiAtMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1zbS1uMixcXG4gIC5teS1zbS1uMiB7XFxuICAgIG1hcmdpbi10b3A6IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLXNtLW4yLFxcbiAgLm14LXNtLW4yIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1zbS1uMixcXG4gIC5teS1zbS1uMiB7XFxuICAgIG1hcmdpbi1ib3R0b206IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXNtLW4yLFxcbiAgLm14LXNtLW4yIHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tc20tbjMge1xcbiAgICBtYXJnaW46IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1zbS1uMyxcXG4gIC5teS1zbS1uMyB7XFxuICAgIG1hcmdpbi10b3A6IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1zbS1uMyxcXG4gIC5teC1zbS1uMyB7XFxuICAgIG1hcmdpbi1yaWdodDogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLXNtLW4zLFxcbiAgLm15LXNtLW4zIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXNtLW4zLFxcbiAgLm14LXNtLW4zIHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLXNtLW40IHtcXG4gICAgbWFyZ2luOiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1zbS1uNCxcXG4gIC5teS1zbS1uNCB7XFxuICAgIG1hcmdpbi10b3A6IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLXNtLW40LFxcbiAgLm14LXNtLW40IHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1zbS1uNCxcXG4gIC5teS1zbS1uNCB7XFxuICAgIG1hcmdpbi1ib3R0b206IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXNtLW40LFxcbiAgLm14LXNtLW40IHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tc20tbjUge1xcbiAgICBtYXJnaW46IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1zbS1uNSxcXG4gIC5teS1zbS1uNSB7XFxuICAgIG1hcmdpbi10b3A6IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1zbS1uNSxcXG4gIC5teC1zbS1uNSB7XFxuICAgIG1hcmdpbi1yaWdodDogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLXNtLW41LFxcbiAgLm15LXNtLW41IHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXNtLW41LFxcbiAgLm14LXNtLW41IHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLXNtLWF1dG8ge1xcbiAgICBtYXJnaW46IGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLm10LXNtLWF1dG8sXFxuICAubXktc20tYXV0byB7XFxuICAgIG1hcmdpbi10b3A6IGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLm1yLXNtLWF1dG8sXFxuICAubXgtc20tYXV0byB7XFxuICAgIG1hcmdpbi1yaWdodDogYXV0byAhaW1wb3J0YW50OyB9XFxuICAubWItc20tYXV0byxcXG4gIC5teS1zbS1hdXRvIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogYXV0byAhaW1wb3J0YW50OyB9XFxuICAubWwtc20tYXV0byxcXG4gIC5teC1zbS1hdXRvIHtcXG4gICAgbWFyZ2luLWxlZnQ6IGF1dG8gIWltcG9ydGFudDsgfSB9XFxuXFxuQG1lZGlhIChtaW4td2lkdGg6IDc2OHB4KSB7XFxuICAubS1tZC0wIHtcXG4gICAgbWFyZ2luOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1tZC0wLFxcbiAgLm15LW1kLTAge1xcbiAgICBtYXJnaW4tdG9wOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1tZC0wLFxcbiAgLm14LW1kLTAge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDAgIWltcG9ydGFudDsgfVxcbiAgLm1iLW1kLTAsXFxuICAubXktbWQtMCB7XFxuICAgIG1hcmdpbi1ib3R0b206IDAgIWltcG9ydGFudDsgfVxcbiAgLm1sLW1kLTAsXFxuICAubXgtbWQtMCB7XFxuICAgIG1hcmdpbi1sZWZ0OiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5tLW1kLTEge1xcbiAgICBtYXJnaW46IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LW1kLTEsXFxuICAubXktbWQtMSB7XFxuICAgIG1hcmdpbi10b3A6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLW1kLTEsXFxuICAubXgtbWQtMSB7XFxuICAgIG1hcmdpbi1yaWdodDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWItbWQtMSxcXG4gIC5teS1tZC0xIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbWQtMSxcXG4gIC5teC1tZC0xIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbWQtMiB7XFxuICAgIG1hcmdpbjogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1tZC0yLFxcbiAgLm15LW1kLTIge1xcbiAgICBtYXJnaW4tdG9wOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLW1kLTIsXFxuICAubXgtbWQtMiB7XFxuICAgIG1hcmdpbi1yaWdodDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1tZC0yLFxcbiAgLm15LW1kLTIge1xcbiAgICBtYXJnaW4tYm90dG9tOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLW1kLTIsXFxuICAubXgtbWQtMiB7XFxuICAgIG1hcmdpbi1sZWZ0OiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbWQtMyB7XFxuICAgIG1hcmdpbjogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXQtbWQtMyxcXG4gIC5teS1tZC0zIHtcXG4gICAgbWFyZ2luLXRvcDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItbWQtMyxcXG4gIC5teC1tZC0zIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1tZC0zLFxcbiAgLm15LW1kLTMge1xcbiAgICBtYXJnaW4tYm90dG9tOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC1tZC0zLFxcbiAgLm14LW1kLTMge1xcbiAgICBtYXJnaW4tbGVmdDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1tZC00IHtcXG4gICAgbWFyZ2luOiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LW1kLTQsXFxuICAubXktbWQtNCB7XFxuICAgIG1hcmdpbi10b3A6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItbWQtNCxcXG4gIC5teC1tZC00IHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLW1kLTQsXFxuICAubXktbWQtNCB7XFxuICAgIG1hcmdpbi1ib3R0b206IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbWQtNCxcXG4gIC5teC1tZC00IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1tZC01IHtcXG4gICAgbWFyZ2luOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1tZC01LFxcbiAgLm15LW1kLTUge1xcbiAgICBtYXJnaW4tdG9wOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1tZC01LFxcbiAgLm14LW1kLTUge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLW1kLTUsXFxuICAubXktbWQtNSB7XFxuICAgIG1hcmdpbi1ib3R0b206IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLW1kLTUsXFxuICAubXgtbWQtNSB7XFxuICAgIG1hcmdpbi1sZWZ0OiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wLW1kLTAge1xcbiAgICBwYWRkaW5nOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1tZC0wLFxcbiAgLnB5LW1kLTAge1xcbiAgICBwYWRkaW5nLXRvcDogMCAhaW1wb3J0YW50OyB9XFxuICAucHItbWQtMCxcXG4gIC5weC1tZC0wIHtcXG4gICAgcGFkZGluZy1yaWdodDogMCAhaW1wb3J0YW50OyB9XFxuICAucGItbWQtMCxcXG4gIC5weS1tZC0wIHtcXG4gICAgcGFkZGluZy1ib3R0b206IDAgIWltcG9ydGFudDsgfVxcbiAgLnBsLW1kLTAsXFxuICAucHgtbWQtMCB7XFxuICAgIHBhZGRpbmctbGVmdDogMCAhaW1wb3J0YW50OyB9XFxuICAucC1tZC0xIHtcXG4gICAgcGFkZGluZzogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQtbWQtMSxcXG4gIC5weS1tZC0xIHtcXG4gICAgcGFkZGluZy10b3A6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnByLW1kLTEsXFxuICAucHgtbWQtMSB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLW1kLTEsXFxuICAucHktbWQtMSB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC1tZC0xLFxcbiAgLnB4LW1kLTEge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnAtbWQtMiB7XFxuICAgIHBhZGRpbmc6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQtbWQtMixcXG4gIC5weS1tZC0yIHtcXG4gICAgcGFkZGluZy10b3A6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHItbWQtMixcXG4gIC5weC1tZC0yIHtcXG4gICAgcGFkZGluZy1yaWdodDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wYi1tZC0yLFxcbiAgLnB5LW1kLTIge1xcbiAgICBwYWRkaW5nLWJvdHRvbTogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC1tZC0yLFxcbiAgLnB4LW1kLTIge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucC1tZC0zIHtcXG4gICAgcGFkZGluZzogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQtbWQtMyxcXG4gIC5weS1tZC0zIHtcXG4gICAgcGFkZGluZy10b3A6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnByLW1kLTMsXFxuICAucHgtbWQtMyB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLW1kLTMsXFxuICAucHktbWQtMyB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC1tZC0zLFxcbiAgLnB4LW1kLTMge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnAtbWQtNCB7XFxuICAgIHBhZGRpbmc6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQtbWQtNCxcXG4gIC5weS1tZC00IHtcXG4gICAgcGFkZGluZy10b3A6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHItbWQtNCxcXG4gIC5weC1tZC00IHtcXG4gICAgcGFkZGluZy1yaWdodDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wYi1tZC00LFxcbiAgLnB5LW1kLTQge1xcbiAgICBwYWRkaW5nLWJvdHRvbTogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC1tZC00LFxcbiAgLnB4LW1kLTQge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucC1tZC01IHtcXG4gICAgcGFkZGluZzogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAucHQtbWQtNSxcXG4gIC5weS1tZC01IHtcXG4gICAgcGFkZGluZy10b3A6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLnByLW1kLTUsXFxuICAucHgtbWQtNSB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLW1kLTUsXFxuICAucHktbWQtNSB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC1tZC01LFxcbiAgLnB4LW1kLTUge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbWQtbjEge1xcbiAgICBtYXJnaW46IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1tZC1uMSxcXG4gIC5teS1tZC1uMSB7XFxuICAgIG1hcmdpbi10b3A6IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1tZC1uMSxcXG4gIC5teC1tZC1uMSB7XFxuICAgIG1hcmdpbi1yaWdodDogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLW1kLW4xLFxcbiAgLm15LW1kLW4xIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLW1kLW4xLFxcbiAgLm14LW1kLW4xIHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLW1kLW4yIHtcXG4gICAgbWFyZ2luOiAtMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1tZC1uMixcXG4gIC5teS1tZC1uMiB7XFxuICAgIG1hcmdpbi10b3A6IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLW1kLW4yLFxcbiAgLm14LW1kLW4yIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1tZC1uMixcXG4gIC5teS1tZC1uMiB7XFxuICAgIG1hcmdpbi1ib3R0b206IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLW1kLW4yLFxcbiAgLm14LW1kLW4yIHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbWQtbjMge1xcbiAgICBtYXJnaW46IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1tZC1uMyxcXG4gIC5teS1tZC1uMyB7XFxuICAgIG1hcmdpbi10b3A6IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1tZC1uMyxcXG4gIC5teC1tZC1uMyB7XFxuICAgIG1hcmdpbi1yaWdodDogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLW1kLW4zLFxcbiAgLm15LW1kLW4zIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLW1kLW4zLFxcbiAgLm14LW1kLW4zIHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLW1kLW40IHtcXG4gICAgbWFyZ2luOiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1tZC1uNCxcXG4gIC5teS1tZC1uNCB7XFxuICAgIG1hcmdpbi10b3A6IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLW1kLW40LFxcbiAgLm14LW1kLW40IHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1tZC1uNCxcXG4gIC5teS1tZC1uNCB7XFxuICAgIG1hcmdpbi1ib3R0b206IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLW1kLW40LFxcbiAgLm14LW1kLW40IHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbWQtbjUge1xcbiAgICBtYXJnaW46IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1tZC1uNSxcXG4gIC5teS1tZC1uNSB7XFxuICAgIG1hcmdpbi10b3A6IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1tZC1uNSxcXG4gIC5teC1tZC1uNSB7XFxuICAgIG1hcmdpbi1yaWdodDogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLW1kLW41LFxcbiAgLm15LW1kLW41IHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLW1kLW41LFxcbiAgLm14LW1kLW41IHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLW1kLWF1dG8ge1xcbiAgICBtYXJnaW46IGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLm10LW1kLWF1dG8sXFxuICAubXktbWQtYXV0byB7XFxuICAgIG1hcmdpbi10b3A6IGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLm1yLW1kLWF1dG8sXFxuICAubXgtbWQtYXV0byB7XFxuICAgIG1hcmdpbi1yaWdodDogYXV0byAhaW1wb3J0YW50OyB9XFxuICAubWItbWQtYXV0byxcXG4gIC5teS1tZC1hdXRvIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogYXV0byAhaW1wb3J0YW50OyB9XFxuICAubWwtbWQtYXV0byxcXG4gIC5teC1tZC1hdXRvIHtcXG4gICAgbWFyZ2luLWxlZnQ6IGF1dG8gIWltcG9ydGFudDsgfSB9XFxuXFxuQG1lZGlhIChtaW4td2lkdGg6IDk5MnB4KSB7XFxuICAubS1sZy0wIHtcXG4gICAgbWFyZ2luOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1sZy0wLFxcbiAgLm15LWxnLTAge1xcbiAgICBtYXJnaW4tdG9wOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1sZy0wLFxcbiAgLm14LWxnLTAge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDAgIWltcG9ydGFudDsgfVxcbiAgLm1iLWxnLTAsXFxuICAubXktbGctMCB7XFxuICAgIG1hcmdpbi1ib3R0b206IDAgIWltcG9ydGFudDsgfVxcbiAgLm1sLWxnLTAsXFxuICAubXgtbGctMCB7XFxuICAgIG1hcmdpbi1sZWZ0OiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5tLWxnLTEge1xcbiAgICBtYXJnaW46IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LWxnLTEsXFxuICAubXktbGctMSB7XFxuICAgIG1hcmdpbi10b3A6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLWxnLTEsXFxuICAubXgtbGctMSB7XFxuICAgIG1hcmdpbi1yaWdodDogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWItbGctMSxcXG4gIC5teS1sZy0xIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbGctMSxcXG4gIC5teC1sZy0xIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbGctMiB7XFxuICAgIG1hcmdpbjogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1sZy0yLFxcbiAgLm15LWxnLTIge1xcbiAgICBtYXJnaW4tdG9wOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLWxnLTIsXFxuICAubXgtbGctMiB7XFxuICAgIG1hcmdpbi1yaWdodDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1sZy0yLFxcbiAgLm15LWxnLTIge1xcbiAgICBtYXJnaW4tYm90dG9tOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLWxnLTIsXFxuICAubXgtbGctMiB7XFxuICAgIG1hcmdpbi1sZWZ0OiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbGctMyB7XFxuICAgIG1hcmdpbjogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXQtbGctMyxcXG4gIC5teS1sZy0zIHtcXG4gICAgbWFyZ2luLXRvcDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItbGctMyxcXG4gIC5teC1sZy0zIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1sZy0zLFxcbiAgLm15LWxnLTMge1xcbiAgICBtYXJnaW4tYm90dG9tOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC1sZy0zLFxcbiAgLm14LWxnLTMge1xcbiAgICBtYXJnaW4tbGVmdDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1sZy00IHtcXG4gICAgbWFyZ2luOiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LWxnLTQsXFxuICAubXktbGctNCB7XFxuICAgIG1hcmdpbi10b3A6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXItbGctNCxcXG4gIC5teC1sZy00IHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLWxnLTQsXFxuICAubXktbGctNCB7XFxuICAgIG1hcmdpbi1ib3R0b206IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwtbGctNCxcXG4gIC5teC1sZy00IHtcXG4gICAgbWFyZ2luLWxlZnQ6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS1sZy01IHtcXG4gICAgbWFyZ2luOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1sZy01LFxcbiAgLm15LWxnLTUge1xcbiAgICBtYXJnaW4tdG9wOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1sZy01LFxcbiAgLm14LWxnLTUge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLWxnLTUsXFxuICAubXktbGctNSB7XFxuICAgIG1hcmdpbi1ib3R0b206IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLWxnLTUsXFxuICAubXgtbGctNSB7XFxuICAgIG1hcmdpbi1sZWZ0OiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wLWxnLTAge1xcbiAgICBwYWRkaW5nOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5wdC1sZy0wLFxcbiAgLnB5LWxnLTAge1xcbiAgICBwYWRkaW5nLXRvcDogMCAhaW1wb3J0YW50OyB9XFxuICAucHItbGctMCxcXG4gIC5weC1sZy0wIHtcXG4gICAgcGFkZGluZy1yaWdodDogMCAhaW1wb3J0YW50OyB9XFxuICAucGItbGctMCxcXG4gIC5weS1sZy0wIHtcXG4gICAgcGFkZGluZy1ib3R0b206IDAgIWltcG9ydGFudDsgfVxcbiAgLnBsLWxnLTAsXFxuICAucHgtbGctMCB7XFxuICAgIHBhZGRpbmctbGVmdDogMCAhaW1wb3J0YW50OyB9XFxuICAucC1sZy0xIHtcXG4gICAgcGFkZGluZzogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQtbGctMSxcXG4gIC5weS1sZy0xIHtcXG4gICAgcGFkZGluZy10b3A6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnByLWxnLTEsXFxuICAucHgtbGctMSB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLWxnLTEsXFxuICAucHktbGctMSB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC1sZy0xLFxcbiAgLnB4LWxnLTEge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnAtbGctMiB7XFxuICAgIHBhZGRpbmc6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQtbGctMixcXG4gIC5weS1sZy0yIHtcXG4gICAgcGFkZGluZy10b3A6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHItbGctMixcXG4gIC5weC1sZy0yIHtcXG4gICAgcGFkZGluZy1yaWdodDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wYi1sZy0yLFxcbiAgLnB5LWxnLTIge1xcbiAgICBwYWRkaW5nLWJvdHRvbTogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC1sZy0yLFxcbiAgLnB4LWxnLTIge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucC1sZy0zIHtcXG4gICAgcGFkZGluZzogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQtbGctMyxcXG4gIC5weS1sZy0zIHtcXG4gICAgcGFkZGluZy10b3A6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnByLWxnLTMsXFxuICAucHgtbGctMyB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLWxnLTMsXFxuICAucHktbGctMyB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC1sZy0zLFxcbiAgLnB4LWxnLTMge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnAtbGctNCB7XFxuICAgIHBhZGRpbmc6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHQtbGctNCxcXG4gIC5weS1sZy00IHtcXG4gICAgcGFkZGluZy10b3A6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucHItbGctNCxcXG4gIC5weC1sZy00IHtcXG4gICAgcGFkZGluZy1yaWdodDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wYi1sZy00LFxcbiAgLnB5LWxnLTQge1xcbiAgICBwYWRkaW5nLWJvdHRvbTogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC1sZy00LFxcbiAgLnB4LWxnLTQge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucC1sZy01IHtcXG4gICAgcGFkZGluZzogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAucHQtbGctNSxcXG4gIC5weS1sZy01IHtcXG4gICAgcGFkZGluZy10b3A6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLnByLWxnLTUsXFxuICAucHgtbGctNSB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLnBiLWxnLTUsXFxuICAucHktbGctNSB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC1sZy01LFxcbiAgLnB4LWxnLTUge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbGctbjEge1xcbiAgICBtYXJnaW46IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1sZy1uMSxcXG4gIC5teS1sZy1uMSB7XFxuICAgIG1hcmdpbi10b3A6IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1sZy1uMSxcXG4gIC5teC1sZy1uMSB7XFxuICAgIG1hcmdpbi1yaWdodDogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLWxnLW4xLFxcbiAgLm15LWxnLW4xIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLWxnLW4xLFxcbiAgLm14LWxnLW4xIHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLWxnLW4yIHtcXG4gICAgbWFyZ2luOiAtMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1sZy1uMixcXG4gIC5teS1sZy1uMiB7XFxuICAgIG1hcmdpbi10b3A6IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLWxnLW4yLFxcbiAgLm14LWxnLW4yIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1sZy1uMixcXG4gIC5teS1sZy1uMiB7XFxuICAgIG1hcmdpbi1ib3R0b206IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLWxnLW4yLFxcbiAgLm14LWxnLW4yIHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0wLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbGctbjMge1xcbiAgICBtYXJnaW46IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1sZy1uMyxcXG4gIC5teS1sZy1uMyB7XFxuICAgIG1hcmdpbi10b3A6IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1sZy1uMyxcXG4gIC5teC1sZy1uMyB7XFxuICAgIG1hcmdpbi1yaWdodDogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLWxnLW4zLFxcbiAgLm15LWxnLW4zIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLWxnLW4zLFxcbiAgLm14LWxnLW4zIHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLWxnLW40IHtcXG4gICAgbWFyZ2luOiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1sZy1uNCxcXG4gIC5teS1sZy1uNCB7XFxuICAgIG1hcmdpbi10b3A6IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLWxnLW40LFxcbiAgLm14LWxnLW40IHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi1sZy1uNCxcXG4gIC5teS1sZy1uNCB7XFxuICAgIG1hcmdpbi1ib3R0b206IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLWxnLW40LFxcbiAgLm14LWxnLW40IHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0xLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0tbGctbjUge1xcbiAgICBtYXJnaW46IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC1sZy1uNSxcXG4gIC5teS1sZy1uNSB7XFxuICAgIG1hcmdpbi10b3A6IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci1sZy1uNSxcXG4gIC5teC1sZy1uNSB7XFxuICAgIG1hcmdpbi1yaWdodDogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLWxnLW41LFxcbiAgLm15LWxnLW41IHtcXG4gICAgbWFyZ2luLWJvdHRvbTogLTNyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLWxnLW41LFxcbiAgLm14LWxnLW41IHtcXG4gICAgbWFyZ2luLWxlZnQ6IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLWxnLWF1dG8ge1xcbiAgICBtYXJnaW46IGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLm10LWxnLWF1dG8sXFxuICAubXktbGctYXV0byB7XFxuICAgIG1hcmdpbi10b3A6IGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLm1yLWxnLWF1dG8sXFxuICAubXgtbGctYXV0byB7XFxuICAgIG1hcmdpbi1yaWdodDogYXV0byAhaW1wb3J0YW50OyB9XFxuICAubWItbGctYXV0byxcXG4gIC5teS1sZy1hdXRvIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogYXV0byAhaW1wb3J0YW50OyB9XFxuICAubWwtbGctYXV0byxcXG4gIC5teC1sZy1hdXRvIHtcXG4gICAgbWFyZ2luLWxlZnQ6IGF1dG8gIWltcG9ydGFudDsgfSB9XFxuXFxuQG1lZGlhIChtaW4td2lkdGg6IDEyMDBweCkge1xcbiAgLm0teGwtMCB7XFxuICAgIG1hcmdpbjogMCAhaW1wb3J0YW50OyB9XFxuICAubXQteGwtMCxcXG4gIC5teS14bC0wIHtcXG4gICAgbWFyZ2luLXRvcDogMCAhaW1wb3J0YW50OyB9XFxuICAubXIteGwtMCxcXG4gIC5teC14bC0wIHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi14bC0wLFxcbiAgLm15LXhsLTAge1xcbiAgICBtYXJnaW4tYm90dG9tOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC14bC0wLFxcbiAgLm14LXhsLTAge1xcbiAgICBtYXJnaW4tbGVmdDogMCAhaW1wb3J0YW50OyB9XFxuICAubS14bC0xIHtcXG4gICAgbWFyZ2luOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC14bC0xLFxcbiAgLm15LXhsLTEge1xcbiAgICBtYXJnaW4tdG9wOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci14bC0xLFxcbiAgLm14LXhsLTEge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1iLXhsLTEsXFxuICAubXkteGwtMSB7XFxuICAgIG1hcmdpbi1ib3R0b206IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXhsLTEsXFxuICAubXgteGwtMSB7XFxuICAgIG1hcmdpbi1sZWZ0OiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLXhsLTIge1xcbiAgICBtYXJnaW46IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXQteGwtMixcXG4gIC5teS14bC0yIHtcXG4gICAgbWFyZ2luLXRvcDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci14bC0yLFxcbiAgLm14LXhsLTIge1xcbiAgICBtYXJnaW4tcmlnaHQ6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWIteGwtMixcXG4gIC5teS14bC0yIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC14bC0yLFxcbiAgLm14LXhsLTIge1xcbiAgICBtYXJnaW4tbGVmdDogMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLXhsLTMge1xcbiAgICBtYXJnaW46IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm10LXhsLTMsXFxuICAubXkteGwtMyB7XFxuICAgIG1hcmdpbi10b3A6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLXhsLTMsXFxuICAubXgteGwtMyB7XFxuICAgIG1hcmdpbi1yaWdodDogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWIteGwtMyxcXG4gIC5teS14bC0zIHtcXG4gICAgbWFyZ2luLWJvdHRvbTogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWwteGwtMyxcXG4gIC5teC14bC0zIHtcXG4gICAgbWFyZ2luLWxlZnQ6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0teGwtNCB7XFxuICAgIG1hcmdpbjogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC14bC00LFxcbiAgLm15LXhsLTQge1xcbiAgICBtYXJnaW4tdG9wOiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1yLXhsLTQsXFxuICAubXgteGwtNCB7XFxuICAgIG1hcmdpbi1yaWdodDogMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi14bC00LFxcbiAgLm15LXhsLTQge1xcbiAgICBtYXJnaW4tYm90dG9tOiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm1sLXhsLTQsXFxuICAubXgteGwtNCB7XFxuICAgIG1hcmdpbi1sZWZ0OiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLm0teGwtNSB7XFxuICAgIG1hcmdpbjogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubXQteGwtNSxcXG4gIC5teS14bC01IHtcXG4gICAgbWFyZ2luLXRvcDogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubXIteGwtNSxcXG4gIC5teC14bC01IHtcXG4gICAgbWFyZ2luLXJpZ2h0OiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi14bC01LFxcbiAgLm15LXhsLTUge1xcbiAgICBtYXJnaW4tYm90dG9tOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC14bC01LFxcbiAgLm14LXhsLTUge1xcbiAgICBtYXJnaW4tbGVmdDogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAucC14bC0wIHtcXG4gICAgcGFkZGluZzogMCAhaW1wb3J0YW50OyB9XFxuICAucHQteGwtMCxcXG4gIC5weS14bC0wIHtcXG4gICAgcGFkZGluZy10b3A6IDAgIWltcG9ydGFudDsgfVxcbiAgLnByLXhsLTAsXFxuICAucHgteGwtMCB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDAgIWltcG9ydGFudDsgfVxcbiAgLnBiLXhsLTAsXFxuICAucHkteGwtMCB7XFxuICAgIHBhZGRpbmctYm90dG9tOiAwICFpbXBvcnRhbnQ7IH1cXG4gIC5wbC14bC0wLFxcbiAgLnB4LXhsLTAge1xcbiAgICBwYWRkaW5nLWxlZnQ6IDAgIWltcG9ydGFudDsgfVxcbiAgLnAteGwtMSB7XFxuICAgIHBhZGRpbmc6IDAuMjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnB0LXhsLTEsXFxuICAucHkteGwtMSB7XFxuICAgIHBhZGRpbmctdG9wOiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wci14bC0xLFxcbiAgLnB4LXhsLTEge1xcbiAgICBwYWRkaW5nLXJpZ2h0OiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wYi14bC0xLFxcbiAgLnB5LXhsLTEge1xcbiAgICBwYWRkaW5nLWJvdHRvbTogMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucGwteGwtMSxcXG4gIC5weC14bC0xIHtcXG4gICAgcGFkZGluZy1sZWZ0OiAwLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wLXhsLTIge1xcbiAgICBwYWRkaW5nOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnB0LXhsLTIsXFxuICAucHkteGwtMiB7XFxuICAgIHBhZGRpbmctdG9wOiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnByLXhsLTIsXFxuICAucHgteGwtMiB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucGIteGwtMixcXG4gIC5weS14bC0yIHtcXG4gICAgcGFkZGluZy1ib3R0b206IDAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucGwteGwtMixcXG4gIC5weC14bC0yIHtcXG4gICAgcGFkZGluZy1sZWZ0OiAwLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnAteGwtMyB7XFxuICAgIHBhZGRpbmc6IDFyZW0gIWltcG9ydGFudDsgfVxcbiAgLnB0LXhsLTMsXFxuICAucHkteGwtMyB7XFxuICAgIHBhZGRpbmctdG9wOiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wci14bC0zLFxcbiAgLnB4LXhsLTMge1xcbiAgICBwYWRkaW5nLXJpZ2h0OiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wYi14bC0zLFxcbiAgLnB5LXhsLTMge1xcbiAgICBwYWRkaW5nLWJvdHRvbTogMXJlbSAhaW1wb3J0YW50OyB9XFxuICAucGwteGwtMyxcXG4gIC5weC14bC0zIHtcXG4gICAgcGFkZGluZy1sZWZ0OiAxcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wLXhsLTQge1xcbiAgICBwYWRkaW5nOiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnB0LXhsLTQsXFxuICAucHkteGwtNCB7XFxuICAgIHBhZGRpbmctdG9wOiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnByLXhsLTQsXFxuICAucHgteGwtNCB7XFxuICAgIHBhZGRpbmctcmlnaHQ6IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucGIteGwtNCxcXG4gIC5weS14bC00IHtcXG4gICAgcGFkZGluZy1ib3R0b206IDEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAucGwteGwtNCxcXG4gIC5weC14bC00IHtcXG4gICAgcGFkZGluZy1sZWZ0OiAxLjVyZW0gIWltcG9ydGFudDsgfVxcbiAgLnAteGwtNSB7XFxuICAgIHBhZGRpbmc6IDNyZW0gIWltcG9ydGFudDsgfVxcbiAgLnB0LXhsLTUsXFxuICAucHkteGwtNSB7XFxuICAgIHBhZGRpbmctdG9wOiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wci14bC01LFxcbiAgLnB4LXhsLTUge1xcbiAgICBwYWRkaW5nLXJpZ2h0OiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5wYi14bC01LFxcbiAgLnB5LXhsLTUge1xcbiAgICBwYWRkaW5nLWJvdHRvbTogM3JlbSAhaW1wb3J0YW50OyB9XFxuICAucGwteGwtNSxcXG4gIC5weC14bC01IHtcXG4gICAgcGFkZGluZy1sZWZ0OiAzcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLXhsLW4xIHtcXG4gICAgbWFyZ2luOiAtMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXQteGwtbjEsXFxuICAubXkteGwtbjEge1xcbiAgICBtYXJnaW4tdG9wOiAtMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXIteGwtbjEsXFxuICAubXgteGwtbjEge1xcbiAgICBtYXJnaW4tcmlnaHQ6IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi14bC1uMSxcXG4gIC5teS14bC1uMSB7XFxuICAgIG1hcmdpbi1ib3R0b206IC0wLjI1cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC14bC1uMSxcXG4gIC5teC14bC1uMSB7XFxuICAgIG1hcmdpbi1sZWZ0OiAtMC4yNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS14bC1uMiB7XFxuICAgIG1hcmdpbjogLTAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXQteGwtbjIsXFxuICAubXkteGwtbjIge1xcbiAgICBtYXJnaW4tdG9wOiAtMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci14bC1uMixcXG4gIC5teC14bC1uMiB7XFxuICAgIG1hcmdpbi1yaWdodDogLTAuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWIteGwtbjIsXFxuICAubXkteGwtbjIge1xcbiAgICBtYXJnaW4tYm90dG9tOiAtMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC14bC1uMixcXG4gIC5teC14bC1uMiB7XFxuICAgIG1hcmdpbi1sZWZ0OiAtMC41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLXhsLW4zIHtcXG4gICAgbWFyZ2luOiAtMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXQteGwtbjMsXFxuICAubXkteGwtbjMge1xcbiAgICBtYXJnaW4tdG9wOiAtMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXIteGwtbjMsXFxuICAubXgteGwtbjMge1xcbiAgICBtYXJnaW4tcmlnaHQ6IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi14bC1uMyxcXG4gIC5teS14bC1uMyB7XFxuICAgIG1hcmdpbi1ib3R0b206IC0xcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC14bC1uMyxcXG4gIC5teC14bC1uMyB7XFxuICAgIG1hcmdpbi1sZWZ0OiAtMXJlbSAhaW1wb3J0YW50OyB9XFxuICAubS14bC1uNCB7XFxuICAgIG1hcmdpbjogLTEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubXQteGwtbjQsXFxuICAubXkteGwtbjQge1xcbiAgICBtYXJnaW4tdG9wOiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tci14bC1uNCxcXG4gIC5teC14bC1uNCB7XFxuICAgIG1hcmdpbi1yaWdodDogLTEuNXJlbSAhaW1wb3J0YW50OyB9XFxuICAubWIteGwtbjQsXFxuICAubXkteGwtbjQge1xcbiAgICBtYXJnaW4tYm90dG9tOiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC14bC1uNCxcXG4gIC5teC14bC1uNCB7XFxuICAgIG1hcmdpbi1sZWZ0OiAtMS41cmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tLXhsLW41IHtcXG4gICAgbWFyZ2luOiAtM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubXQteGwtbjUsXFxuICAubXkteGwtbjUge1xcbiAgICBtYXJnaW4tdG9wOiAtM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubXIteGwtbjUsXFxuICAubXgteGwtbjUge1xcbiAgICBtYXJnaW4tcmlnaHQ6IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tYi14bC1uNSxcXG4gIC5teS14bC1uNSB7XFxuICAgIG1hcmdpbi1ib3R0b206IC0zcmVtICFpbXBvcnRhbnQ7IH1cXG4gIC5tbC14bC1uNSxcXG4gIC5teC14bC1uNSB7XFxuICAgIG1hcmdpbi1sZWZ0OiAtM3JlbSAhaW1wb3J0YW50OyB9XFxuICAubS14bC1hdXRvIHtcXG4gICAgbWFyZ2luOiBhdXRvICFpbXBvcnRhbnQ7IH1cXG4gIC5tdC14bC1hdXRvLFxcbiAgLm15LXhsLWF1dG8ge1xcbiAgICBtYXJnaW4tdG9wOiBhdXRvICFpbXBvcnRhbnQ7IH1cXG4gIC5tci14bC1hdXRvLFxcbiAgLm14LXhsLWF1dG8ge1xcbiAgICBtYXJnaW4tcmlnaHQ6IGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLm1iLXhsLWF1dG8sXFxuICAubXkteGwtYXV0byB7XFxuICAgIG1hcmdpbi1ib3R0b206IGF1dG8gIWltcG9ydGFudDsgfVxcbiAgLm1sLXhsLWF1dG8sXFxuICAubXgteGwtYXV0byB7XFxuICAgIG1hcmdpbi1sZWZ0OiBhdXRvICFpbXBvcnRhbnQ7IH0gfVxcblxcbi50ZXh0LW1vbm9zcGFjZSB7XFxuICBmb250LWZhbWlseTogU0ZNb25vLVJlZ3VsYXIsIE1lbmxvLCBNb25hY28sIENvbnNvbGFzLCBcXFwiTGliZXJhdGlvbiBNb25vXFxcIiwgXFxcIkNvdXJpZXIgTmV3XFxcIiwgbW9ub3NwYWNlICFpbXBvcnRhbnQ7IH1cXG5cXG4udGV4dC1qdXN0aWZ5IHtcXG4gIHRleHQtYWxpZ246IGp1c3RpZnkgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LXdyYXAge1xcbiAgd2hpdGUtc3BhY2U6IG5vcm1hbCAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtbm93cmFwIHtcXG4gIHdoaXRlLXNwYWNlOiBub3dyYXAgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LXRydW5jYXRlIHtcXG4gIG92ZXJmbG93OiBoaWRkZW47XFxuICB0ZXh0LW92ZXJmbG93OiBlbGxpcHNpcztcXG4gIHdoaXRlLXNwYWNlOiBub3dyYXA7IH1cXG5cXG4udGV4dC1sZWZ0IHtcXG4gIHRleHQtYWxpZ246IGxlZnQgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LXJpZ2h0IHtcXG4gIHRleHQtYWxpZ246IHJpZ2h0ICFpbXBvcnRhbnQ7IH1cXG5cXG4udGV4dC1jZW50ZXIge1xcbiAgdGV4dC1hbGlnbjogY2VudGVyICFpbXBvcnRhbnQ7IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogNTc2cHgpIHtcXG4gIC50ZXh0LXNtLWxlZnQge1xcbiAgICB0ZXh0LWFsaWduOiBsZWZ0ICFpbXBvcnRhbnQ7IH1cXG4gIC50ZXh0LXNtLXJpZ2h0IHtcXG4gICAgdGV4dC1hbGlnbjogcmlnaHQgIWltcG9ydGFudDsgfVxcbiAgLnRleHQtc20tY2VudGVyIHtcXG4gICAgdGV4dC1hbGlnbjogY2VudGVyICFpbXBvcnRhbnQ7IH0gfVxcblxcbkBtZWRpYSAobWluLXdpZHRoOiA3NjhweCkge1xcbiAgLnRleHQtbWQtbGVmdCB7XFxuICAgIHRleHQtYWxpZ246IGxlZnQgIWltcG9ydGFudDsgfVxcbiAgLnRleHQtbWQtcmlnaHQge1xcbiAgICB0ZXh0LWFsaWduOiByaWdodCAhaW1wb3J0YW50OyB9XFxuICAudGV4dC1tZC1jZW50ZXIge1xcbiAgICB0ZXh0LWFsaWduOiBjZW50ZXIgIWltcG9ydGFudDsgfSB9XFxuXFxuQG1lZGlhIChtaW4td2lkdGg6IDk5MnB4KSB7XFxuICAudGV4dC1sZy1sZWZ0IHtcXG4gICAgdGV4dC1hbGlnbjogbGVmdCAhaW1wb3J0YW50OyB9XFxuICAudGV4dC1sZy1yaWdodCB7XFxuICAgIHRleHQtYWxpZ246IHJpZ2h0ICFpbXBvcnRhbnQ7IH1cXG4gIC50ZXh0LWxnLWNlbnRlciB7XFxuICAgIHRleHQtYWxpZ246IGNlbnRlciAhaW1wb3J0YW50OyB9IH1cXG5cXG5AbWVkaWEgKG1pbi13aWR0aDogMTIwMHB4KSB7XFxuICAudGV4dC14bC1sZWZ0IHtcXG4gICAgdGV4dC1hbGlnbjogbGVmdCAhaW1wb3J0YW50OyB9XFxuICAudGV4dC14bC1yaWdodCB7XFxuICAgIHRleHQtYWxpZ246IHJpZ2h0ICFpbXBvcnRhbnQ7IH1cXG4gIC50ZXh0LXhsLWNlbnRlciB7XFxuICAgIHRleHQtYWxpZ246IGNlbnRlciAhaW1wb3J0YW50OyB9IH1cXG5cXG4udGV4dC1sb3dlcmNhc2Uge1xcbiAgdGV4dC10cmFuc2Zvcm06IGxvd2VyY2FzZSAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtdXBwZXJjYXNlIHtcXG4gIHRleHQtdHJhbnNmb3JtOiB1cHBlcmNhc2UgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LWNhcGl0YWxpemUge1xcbiAgdGV4dC10cmFuc2Zvcm06IGNhcGl0YWxpemUgIWltcG9ydGFudDsgfVxcblxcbi5mb250LXdlaWdodC1saWdodCB7XFxuICBmb250LXdlaWdodDogMzAwICFpbXBvcnRhbnQ7IH1cXG5cXG4uZm9udC13ZWlnaHQtbGlnaHRlciB7XFxuICBmb250LXdlaWdodDogbGlnaHRlciAhaW1wb3J0YW50OyB9XFxuXFxuLmZvbnQtd2VpZ2h0LW5vcm1hbCB7XFxuICBmb250LXdlaWdodDogNDAwICFpbXBvcnRhbnQ7IH1cXG5cXG4uZm9udC13ZWlnaHQtYm9sZCB7XFxuICBmb250LXdlaWdodDogNzAwICFpbXBvcnRhbnQ7IH1cXG5cXG4uZm9udC13ZWlnaHQtYm9sZGVyIHtcXG4gIGZvbnQtd2VpZ2h0OiBib2xkZXIgIWltcG9ydGFudDsgfVxcblxcbi5mb250LWl0YWxpYyB7XFxuICBmb250LXN0eWxlOiBpdGFsaWMgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LXdoaXRlIHtcXG4gIGNvbG9yOiAjZmZmICFpbXBvcnRhbnQ7IH1cXG5cXG4udGV4dC1wcmltYXJ5IHtcXG4gIGNvbG9yOiAjMDA3YmZmICFpbXBvcnRhbnQ7IH1cXG5cXG5hLnRleHQtcHJpbWFyeTpob3ZlciwgYS50ZXh0LXByaW1hcnk6Zm9jdXMge1xcbiAgY29sb3I6ICMwMDU2YjMgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LXNlY29uZGFyeSB7XFxuICBjb2xvcjogIzZjNzU3ZCAhaW1wb3J0YW50OyB9XFxuXFxuYS50ZXh0LXNlY29uZGFyeTpob3ZlciwgYS50ZXh0LXNlY29uZGFyeTpmb2N1cyB7XFxuICBjb2xvcjogIzQ5NGY1NCAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtc3VjY2VzcyB7XFxuICBjb2xvcjogIzI4YTc0NSAhaW1wb3J0YW50OyB9XFxuXFxuYS50ZXh0LXN1Y2Nlc3M6aG92ZXIsIGEudGV4dC1zdWNjZXNzOmZvY3VzIHtcXG4gIGNvbG9yOiAjMTk2OTJjICFpbXBvcnRhbnQ7IH1cXG5cXG4udGV4dC1pbmZvIHtcXG4gIGNvbG9yOiAjMTdhMmI4ICFpbXBvcnRhbnQ7IH1cXG5cXG5hLnRleHQtaW5mbzpob3ZlciwgYS50ZXh0LWluZm86Zm9jdXMge1xcbiAgY29sb3I6ICMwZjY2NzQgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LXdhcm5pbmcge1xcbiAgY29sb3I6ICNmZmMxMDcgIWltcG9ydGFudDsgfVxcblxcbmEudGV4dC13YXJuaW5nOmhvdmVyLCBhLnRleHQtd2FybmluZzpmb2N1cyB7XFxuICBjb2xvcjogI2JhOGIwMCAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtZGFuZ2VyIHtcXG4gIGNvbG9yOiAjZGMzNTQ1ICFpbXBvcnRhbnQ7IH1cXG5cXG5hLnRleHQtZGFuZ2VyOmhvdmVyLCBhLnRleHQtZGFuZ2VyOmZvY3VzIHtcXG4gIGNvbG9yOiAjYTcxZDJhICFpbXBvcnRhbnQ7IH1cXG5cXG4udGV4dC1saWdodCB7XFxuICBjb2xvcjogI2Y4ZjlmYSAhaW1wb3J0YW50OyB9XFxuXFxuYS50ZXh0LWxpZ2h0OmhvdmVyLCBhLnRleHQtbGlnaHQ6Zm9jdXMge1xcbiAgY29sb3I6ICNjYmQzZGEgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LWRhcmsge1xcbiAgY29sb3I6ICMzNDNhNDAgIWltcG9ydGFudDsgfVxcblxcbmEudGV4dC1kYXJrOmhvdmVyLCBhLnRleHQtZGFyazpmb2N1cyB7XFxuICBjb2xvcjogIzEyMTQxNiAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtYm9keSB7XFxuICBjb2xvcjogIzIxMjUyOSAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtbXV0ZWQge1xcbiAgY29sb3I6ICM2Yzc1N2QgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LWJsYWNrLTUwIHtcXG4gIGNvbG9yOiByZ2JhKDAsIDAsIDAsIDAuNSkgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LXdoaXRlLTUwIHtcXG4gIGNvbG9yOiByZ2JhKDI1NSwgMjU1LCAyNTUsIDAuNSkgIWltcG9ydGFudDsgfVxcblxcbi50ZXh0LWhpZGUge1xcbiAgZm9udDogMC8wIGE7XFxuICBjb2xvcjogdHJhbnNwYXJlbnQ7XFxuICB0ZXh0LXNoYWRvdzogbm9uZTtcXG4gIGJhY2tncm91bmQtY29sb3I6IHRyYW5zcGFyZW50O1xcbiAgYm9yZGVyOiAwOyB9XFxuXFxuLnRleHQtZGVjb3JhdGlvbi1ub25lIHtcXG4gIHRleHQtZGVjb3JhdGlvbjogbm9uZSAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtYnJlYWsge1xcbiAgd29yZC1icmVhazogYnJlYWstd29yZCAhaW1wb3J0YW50O1xcbiAgb3ZlcmZsb3ctd3JhcDogYnJlYWstd29yZCAhaW1wb3J0YW50OyB9XFxuXFxuLnRleHQtcmVzZXQge1xcbiAgY29sb3I6IGluaGVyaXQgIWltcG9ydGFudDsgfVxcblxcbi5iZy1wcmltYXJ5IHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMwMDdiZmYgIWltcG9ydGFudDsgfVxcblxcbmEuYmctcHJpbWFyeTpob3ZlciwgYS5iZy1wcmltYXJ5OmZvY3VzLFxcbmJ1dHRvbi5iZy1wcmltYXJ5OmhvdmVyLFxcbmJ1dHRvbi5iZy1wcmltYXJ5OmZvY3VzIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICMwMDYyY2MgIWltcG9ydGFudDsgfVxcblxcbi5iZy1zZWNvbmRhcnkge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogIzZjNzU3ZCAhaW1wb3J0YW50OyB9XFxuXFxuYS5iZy1zZWNvbmRhcnk6aG92ZXIsIGEuYmctc2Vjb25kYXJ5OmZvY3VzLFxcbmJ1dHRvbi5iZy1zZWNvbmRhcnk6aG92ZXIsXFxuYnV0dG9uLmJnLXNlY29uZGFyeTpmb2N1cyB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjNTQ1YjYyICFpbXBvcnRhbnQ7IH1cXG5cXG4uYmctc3VjY2VzcyB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjMjhhNzQ1ICFpbXBvcnRhbnQ7IH1cXG5cXG5hLmJnLXN1Y2Nlc3M6aG92ZXIsIGEuYmctc3VjY2Vzczpmb2N1cyxcXG5idXR0b24uYmctc3VjY2Vzczpob3ZlcixcXG5idXR0b24uYmctc3VjY2Vzczpmb2N1cyB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjMWU3ZTM0ICFpbXBvcnRhbnQ7IH1cXG5cXG4uYmctaW5mbyB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjMTdhMmI4ICFpbXBvcnRhbnQ7IH1cXG5cXG5hLmJnLWluZm86aG92ZXIsIGEuYmctaW5mbzpmb2N1cyxcXG5idXR0b24uYmctaW5mbzpob3ZlcixcXG5idXR0b24uYmctaW5mbzpmb2N1cyB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjMTE3YThiICFpbXBvcnRhbnQ7IH1cXG5cXG4uYmctd2FybmluZyB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjZmZjMTA3ICFpbXBvcnRhbnQ7IH1cXG5cXG5hLmJnLXdhcm5pbmc6aG92ZXIsIGEuYmctd2FybmluZzpmb2N1cyxcXG5idXR0b24uYmctd2FybmluZzpob3ZlcixcXG5idXR0b24uYmctd2FybmluZzpmb2N1cyB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjZDM5ZTAwICFpbXBvcnRhbnQ7IH1cXG5cXG4uYmctZGFuZ2VyIHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNkYzM1NDUgIWltcG9ydGFudDsgfVxcblxcbmEuYmctZGFuZ2VyOmhvdmVyLCBhLmJnLWRhbmdlcjpmb2N1cyxcXG5idXR0b24uYmctZGFuZ2VyOmhvdmVyLFxcbmJ1dHRvbi5iZy1kYW5nZXI6Zm9jdXMge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2JkMjEzMCAhaW1wb3J0YW50OyB9XFxuXFxuLmJnLWxpZ2h0IHtcXG4gIGJhY2tncm91bmQtY29sb3I6ICNmOGY5ZmEgIWltcG9ydGFudDsgfVxcblxcbmEuYmctbGlnaHQ6aG92ZXIsIGEuYmctbGlnaHQ6Zm9jdXMsXFxuYnV0dG9uLmJnLWxpZ2h0OmhvdmVyLFxcbmJ1dHRvbi5iZy1saWdodDpmb2N1cyB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjZGFlMGU1ICFpbXBvcnRhbnQ7IH1cXG5cXG4uYmctZGFyayB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjMzQzYTQwICFpbXBvcnRhbnQ7IH1cXG5cXG5hLmJnLWRhcms6aG92ZXIsIGEuYmctZGFyazpmb2N1cyxcXG5idXR0b24uYmctZGFyazpob3ZlcixcXG5idXR0b24uYmctZGFyazpmb2N1cyB7XFxuICBiYWNrZ3JvdW5kLWNvbG9yOiAjMWQyMTI0ICFpbXBvcnRhbnQ7IH1cXG5cXG4uYmctd2hpdGUge1xcbiAgYmFja2dyb3VuZC1jb2xvcjogI2ZmZiAhaW1wb3J0YW50OyB9XFxuXFxuLmJnLXRyYW5zcGFyZW50IHtcXG4gIGJhY2tncm91bmQtY29sb3I6IHRyYW5zcGFyZW50ICFpbXBvcnRhbnQ7IH1cXG5cIiwgXCJcIl0pO1xuXG4iLCJleHBvcnRzID0gbW9kdWxlLmV4cG9ydHMgPSByZXF1aXJlKFwiLi4vLi4vbm9kZV9tb2R1bGVzL2Nzcy1sb2FkZXIvZGlzdC9ydW50aW1lL2FwaS5qc1wiKShmYWxzZSk7XG4vLyBJbXBvcnRzXG5leHBvcnRzLmkocmVxdWlyZShcIi0hLi4vLi4vbm9kZV9tb2R1bGVzL2Nzcy1sb2FkZXIvZGlzdC9janMuanMhaGlnaGxpZ2h0LmpzL3N0eWxlcy9kZWZhdWx0LmNzc1wiKSwgXCJcIik7XG5cbi8vIE1vZHVsZVxuZXhwb3J0cy5wdXNoKFttb2R1bGUuaWQsIFwiXFxuXCIsIFwiXCJdKTtcblxuIiwiaW1wb3J0IHsgY3NzLCBDU1NSZXN1bHQsIHVuc2FmZUNTUyB9IGZyb20gJ2xpdC1lbGVtZW50JztcclxuY29uc3QgYm9vdHN0cmFwQ3NzID0gcmVxdWlyZSgnLi9ib290c3RyYXAuc2NzcycpO1xyXG5jb25zdCBoaWdobGlnaHRqc0NzcyA9IHJlcXVpcmUoJy4vaGlnaGxpZ2h0anMuc2NzcycpO1xyXG5cclxuZXhwb3J0IGNvbnN0IGJvb3RzdHJhcDogQ1NTUmVzdWx0ID0gY3NzYCR7dW5zYWZlQ1NTKGJvb3RzdHJhcENzcy50b1N0cmluZygpKX1gO1xyXG5leHBvcnQgY29uc3QgaGlnaGxpZ2h0SlM6IENTU1Jlc3VsdCA9IGNzc2Ake3Vuc2FmZUNTUyhoaWdobGlnaHRqc0Nzcy50b1N0cmluZygpKX1gO1xyXG4iXSwic291cmNlUm9vdCI6IiJ9