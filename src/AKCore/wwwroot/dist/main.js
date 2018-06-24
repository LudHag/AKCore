/******/ (function(modules) { // webpackBootstrap
/******/ 	function hotDisposeChunk(chunkId) {
/******/ 		delete installedChunks[chunkId];
/******/ 	}
/******/ 	var parentHotUpdateCallback = window["webpackHotUpdate"];
/******/ 	window["webpackHotUpdate"] = 
/******/ 	function webpackHotUpdateCallback(chunkId, moreModules) { // eslint-disable-line no-unused-vars
/******/ 		hotAddUpdateChunk(chunkId, moreModules);
/******/ 		if(parentHotUpdateCallback) parentHotUpdateCallback(chunkId, moreModules);
/******/ 	} ;
/******/ 	
/******/ 	function hotDownloadUpdateChunk(chunkId) { // eslint-disable-line no-unused-vars
/******/ 		var head = document.getElementsByTagName("head")[0];
/******/ 		var script = document.createElement("script");
/******/ 		script.type = "text/javascript";
/******/ 		script.charset = "utf-8";
/******/ 		script.src = __webpack_require__.p + "hot/hot-update.js";
/******/ 		;
/******/ 		head.appendChild(script);
/******/ 	}
/******/ 	
/******/ 	function hotDownloadManifest(requestTimeout) { // eslint-disable-line no-unused-vars
/******/ 		requestTimeout = requestTimeout || 10000;
/******/ 		return new Promise(function(resolve, reject) {
/******/ 			if(typeof XMLHttpRequest === "undefined")
/******/ 				return reject(new Error("No browser support"));
/******/ 			try {
/******/ 				var request = new XMLHttpRequest();
/******/ 				var requestPath = __webpack_require__.p + "hot/hot-update.json";
/******/ 				request.open("GET", requestPath, true);
/******/ 				request.timeout = requestTimeout;
/******/ 				request.send(null);
/******/ 			} catch(err) {
/******/ 				return reject(err);
/******/ 			}
/******/ 			request.onreadystatechange = function() {
/******/ 				if(request.readyState !== 4) return;
/******/ 				if(request.status === 0) {
/******/ 					// timeout
/******/ 					reject(new Error("Manifest request to " + requestPath + " timed out."));
/******/ 				} else if(request.status === 404) {
/******/ 					// no update available
/******/ 					resolve();
/******/ 				} else if(request.status !== 200 && request.status !== 304) {
/******/ 					// other failure
/******/ 					reject(new Error("Manifest request to " + requestPath + " failed."));
/******/ 				} else {
/******/ 					// success
/******/ 					try {
/******/ 						var update = JSON.parse(request.responseText);
/******/ 					} catch(e) {
/******/ 						reject(e);
/******/ 						return;
/******/ 					}
/******/ 					resolve(update);
/******/ 				}
/******/ 			};
/******/ 		});
/******/ 	}
/******/
/******/ 	
/******/ 	
/******/ 	var hotApplyOnUpdate = true;
/******/ 	var hotCurrentHash = "85a3e7cb021bdde312fc"; // eslint-disable-line no-unused-vars
/******/ 	var hotRequestTimeout = 10000;
/******/ 	var hotCurrentModuleData = {};
/******/ 	var hotCurrentChildModule; // eslint-disable-line no-unused-vars
/******/ 	var hotCurrentParents = []; // eslint-disable-line no-unused-vars
/******/ 	var hotCurrentParentsTemp = []; // eslint-disable-line no-unused-vars
/******/ 	
/******/ 	function hotCreateRequire(moduleId) { // eslint-disable-line no-unused-vars
/******/ 		var me = installedModules[moduleId];
/******/ 		if(!me) return __webpack_require__;
/******/ 		var fn = function(request) {
/******/ 			if(me.hot.active) {
/******/ 				if(installedModules[request]) {
/******/ 					if(installedModules[request].parents.indexOf(moduleId) < 0)
/******/ 						installedModules[request].parents.push(moduleId);
/******/ 				} else {
/******/ 					hotCurrentParents = [moduleId];
/******/ 					hotCurrentChildModule = request;
/******/ 				}
/******/ 				if(me.children.indexOf(request) < 0)
/******/ 					me.children.push(request);
/******/ 			} else {
/******/ 				console.warn("[HMR] unexpected require(" + request + ") from disposed module " + moduleId);
/******/ 				hotCurrentParents = [];
/******/ 			}
/******/ 			return __webpack_require__(request);
/******/ 		};
/******/ 		var ObjectFactory = function ObjectFactory(name) {
/******/ 			return {
/******/ 				configurable: true,
/******/ 				enumerable: true,
/******/ 				get: function() {
/******/ 					return __webpack_require__[name];
/******/ 				},
/******/ 				set: function(value) {
/******/ 					__webpack_require__[name] = value;
/******/ 				}
/******/ 			};
/******/ 		};
/******/ 		for(var name in __webpack_require__) {
/******/ 			if(Object.prototype.hasOwnProperty.call(__webpack_require__, name) && name !== "e") {
/******/ 				Object.defineProperty(fn, name, ObjectFactory(name));
/******/ 			}
/******/ 		}
/******/ 		fn.e = function(chunkId) {
/******/ 			if(hotStatus === "ready")
/******/ 				hotSetStatus("prepare");
/******/ 			hotChunksLoading++;
/******/ 			return __webpack_require__.e(chunkId).then(finishChunkLoading, function(err) {
/******/ 				finishChunkLoading();
/******/ 				throw err;
/******/ 			});
/******/ 	
/******/ 			function finishChunkLoading() {
/******/ 				hotChunksLoading--;
/******/ 				if(hotStatus === "prepare") {
/******/ 					if(!hotWaitingFilesMap[chunkId]) {
/******/ 						hotEnsureUpdateChunk(chunkId);
/******/ 					}
/******/ 					if(hotChunksLoading === 0 && hotWaitingFiles === 0) {
/******/ 						hotUpdateDownloaded();
/******/ 					}
/******/ 				}
/******/ 			}
/******/ 		};
/******/ 		return fn;
/******/ 	}
/******/ 	
/******/ 	function hotCreateModule(moduleId) { // eslint-disable-line no-unused-vars
/******/ 		var hot = {
/******/ 			// private stuff
/******/ 			_acceptedDependencies: {},
/******/ 			_declinedDependencies: {},
/******/ 			_selfAccepted: false,
/******/ 			_selfDeclined: false,
/******/ 			_disposeHandlers: [],
/******/ 			_main: hotCurrentChildModule !== moduleId,
/******/ 	
/******/ 			// Module API
/******/ 			active: true,
/******/ 			accept: function(dep, callback) {
/******/ 				if(typeof dep === "undefined")
/******/ 					hot._selfAccepted = true;
/******/ 				else if(typeof dep === "function")
/******/ 					hot._selfAccepted = dep;
/******/ 				else if(typeof dep === "object")
/******/ 					for(var i = 0; i < dep.length; i++)
/******/ 						hot._acceptedDependencies[dep[i]] = callback || function() {};
/******/ 				else
/******/ 					hot._acceptedDependencies[dep] = callback || function() {};
/******/ 			},
/******/ 			decline: function(dep) {
/******/ 				if(typeof dep === "undefined")
/******/ 					hot._selfDeclined = true;
/******/ 				else if(typeof dep === "object")
/******/ 					for(var i = 0; i < dep.length; i++)
/******/ 						hot._declinedDependencies[dep[i]] = true;
/******/ 				else
/******/ 					hot._declinedDependencies[dep] = true;
/******/ 			},
/******/ 			dispose: function(callback) {
/******/ 				hot._disposeHandlers.push(callback);
/******/ 			},
/******/ 			addDisposeHandler: function(callback) {
/******/ 				hot._disposeHandlers.push(callback);
/******/ 			},
/******/ 			removeDisposeHandler: function(callback) {
/******/ 				var idx = hot._disposeHandlers.indexOf(callback);
/******/ 				if(idx >= 0) hot._disposeHandlers.splice(idx, 1);
/******/ 			},
/******/ 	
/******/ 			// Management API
/******/ 			check: hotCheck,
/******/ 			apply: hotApply,
/******/ 			status: function(l) {
/******/ 				if(!l) return hotStatus;
/******/ 				hotStatusHandlers.push(l);
/******/ 			},
/******/ 			addStatusHandler: function(l) {
/******/ 				hotStatusHandlers.push(l);
/******/ 			},
/******/ 			removeStatusHandler: function(l) {
/******/ 				var idx = hotStatusHandlers.indexOf(l);
/******/ 				if(idx >= 0) hotStatusHandlers.splice(idx, 1);
/******/ 			},
/******/ 	
/******/ 			//inherit from previous dispose call
/******/ 			data: hotCurrentModuleData[moduleId]
/******/ 		};
/******/ 		hotCurrentChildModule = undefined;
/******/ 		return hot;
/******/ 	}
/******/ 	
/******/ 	var hotStatusHandlers = [];
/******/ 	var hotStatus = "idle";
/******/ 	
/******/ 	function hotSetStatus(newStatus) {
/******/ 		hotStatus = newStatus;
/******/ 		for(var i = 0; i < hotStatusHandlers.length; i++)
/******/ 			hotStatusHandlers[i].call(null, newStatus);
/******/ 	}
/******/ 	
/******/ 	// while downloading
/******/ 	var hotWaitingFiles = 0;
/******/ 	var hotChunksLoading = 0;
/******/ 	var hotWaitingFilesMap = {};
/******/ 	var hotRequestedFilesMap = {};
/******/ 	var hotAvailableFilesMap = {};
/******/ 	var hotDeferred;
/******/ 	
/******/ 	// The update info
/******/ 	var hotUpdate, hotUpdateNewHash;
/******/ 	
/******/ 	function toModuleId(id) {
/******/ 		var isNumber = (+id) + "" === id;
/******/ 		return isNumber ? +id : id;
/******/ 	}
/******/ 	
/******/ 	function hotCheck(apply) {
/******/ 		if(hotStatus !== "idle") throw new Error("check() is only allowed in idle status");
/******/ 		hotApplyOnUpdate = apply;
/******/ 		hotSetStatus("check");
/******/ 		return hotDownloadManifest(hotRequestTimeout).then(function(update) {
/******/ 			if(!update) {
/******/ 				hotSetStatus("idle");
/******/ 				return null;
/******/ 			}
/******/ 			hotRequestedFilesMap = {};
/******/ 			hotWaitingFilesMap = {};
/******/ 			hotAvailableFilesMap = update.c;
/******/ 			hotUpdateNewHash = update.h;
/******/ 	
/******/ 			hotSetStatus("prepare");
/******/ 			var promise = new Promise(function(resolve, reject) {
/******/ 				hotDeferred = {
/******/ 					resolve: resolve,
/******/ 					reject: reject
/******/ 				};
/******/ 			});
/******/ 			hotUpdate = {};
/******/ 			var chunkId = 0;
/******/ 			{ // eslint-disable-line no-lone-blocks
/******/ 				/*globals chunkId */
/******/ 				hotEnsureUpdateChunk(chunkId);
/******/ 			}
/******/ 			if(hotStatus === "prepare" && hotChunksLoading === 0 && hotWaitingFiles === 0) {
/******/ 				hotUpdateDownloaded();
/******/ 			}
/******/ 			return promise;
/******/ 		});
/******/ 	}
/******/ 	
/******/ 	function hotAddUpdateChunk(chunkId, moreModules) { // eslint-disable-line no-unused-vars
/******/ 		if(!hotAvailableFilesMap[chunkId] || !hotRequestedFilesMap[chunkId])
/******/ 			return;
/******/ 		hotRequestedFilesMap[chunkId] = false;
/******/ 		for(var moduleId in moreModules) {
/******/ 			if(Object.prototype.hasOwnProperty.call(moreModules, moduleId)) {
/******/ 				hotUpdate[moduleId] = moreModules[moduleId];
/******/ 			}
/******/ 		}
/******/ 		if(--hotWaitingFiles === 0 && hotChunksLoading === 0) {
/******/ 			hotUpdateDownloaded();
/******/ 		}
/******/ 	}
/******/ 	
/******/ 	function hotEnsureUpdateChunk(chunkId) {
/******/ 		if(!hotAvailableFilesMap[chunkId]) {
/******/ 			hotWaitingFilesMap[chunkId] = true;
/******/ 		} else {
/******/ 			hotRequestedFilesMap[chunkId] = true;
/******/ 			hotWaitingFiles++;
/******/ 			hotDownloadUpdateChunk(chunkId);
/******/ 		}
/******/ 	}
/******/ 	
/******/ 	function hotUpdateDownloaded() {
/******/ 		hotSetStatus("ready");
/******/ 		var deferred = hotDeferred;
/******/ 		hotDeferred = null;
/******/ 		if(!deferred) return;
/******/ 		if(hotApplyOnUpdate) {
/******/ 			// Wrap deferred object in Promise to mark it as a well-handled Promise to
/******/ 			// avoid triggering uncaught exception warning in Chrome.
/******/ 			// See https://bugs.chromium.org/p/chromium/issues/detail?id=465666
/******/ 			Promise.resolve().then(function() {
/******/ 				return hotApply(hotApplyOnUpdate);
/******/ 			}).then(
/******/ 				function(result) {
/******/ 					deferred.resolve(result);
/******/ 				},
/******/ 				function(err) {
/******/ 					deferred.reject(err);
/******/ 				}
/******/ 			);
/******/ 		} else {
/******/ 			var outdatedModules = [];
/******/ 			for(var id in hotUpdate) {
/******/ 				if(Object.prototype.hasOwnProperty.call(hotUpdate, id)) {
/******/ 					outdatedModules.push(toModuleId(id));
/******/ 				}
/******/ 			}
/******/ 			deferred.resolve(outdatedModules);
/******/ 		}
/******/ 	}
/******/ 	
/******/ 	function hotApply(options) {
/******/ 		if(hotStatus !== "ready") throw new Error("apply() is only allowed in ready status");
/******/ 		options = options || {};
/******/ 	
/******/ 		var cb;
/******/ 		var i;
/******/ 		var j;
/******/ 		var module;
/******/ 		var moduleId;
/******/ 	
/******/ 		function getAffectedStuff(updateModuleId) {
/******/ 			var outdatedModules = [updateModuleId];
/******/ 			var outdatedDependencies = {};
/******/ 	
/******/ 			var queue = outdatedModules.slice().map(function(id) {
/******/ 				return {
/******/ 					chain: [id],
/******/ 					id: id
/******/ 				};
/******/ 			});
/******/ 			while(queue.length > 0) {
/******/ 				var queueItem = queue.pop();
/******/ 				var moduleId = queueItem.id;
/******/ 				var chain = queueItem.chain;
/******/ 				module = installedModules[moduleId];
/******/ 				if(!module || module.hot._selfAccepted)
/******/ 					continue;
/******/ 				if(module.hot._selfDeclined) {
/******/ 					return {
/******/ 						type: "self-declined",
/******/ 						chain: chain,
/******/ 						moduleId: moduleId
/******/ 					};
/******/ 				}
/******/ 				if(module.hot._main) {
/******/ 					return {
/******/ 						type: "unaccepted",
/******/ 						chain: chain,
/******/ 						moduleId: moduleId
/******/ 					};
/******/ 				}
/******/ 				for(var i = 0; i < module.parents.length; i++) {
/******/ 					var parentId = module.parents[i];
/******/ 					var parent = installedModules[parentId];
/******/ 					if(!parent) continue;
/******/ 					if(parent.hot._declinedDependencies[moduleId]) {
/******/ 						return {
/******/ 							type: "declined",
/******/ 							chain: chain.concat([parentId]),
/******/ 							moduleId: moduleId,
/******/ 							parentId: parentId
/******/ 						};
/******/ 					}
/******/ 					if(outdatedModules.indexOf(parentId) >= 0) continue;
/******/ 					if(parent.hot._acceptedDependencies[moduleId]) {
/******/ 						if(!outdatedDependencies[parentId])
/******/ 							outdatedDependencies[parentId] = [];
/******/ 						addAllToSet(outdatedDependencies[parentId], [moduleId]);
/******/ 						continue;
/******/ 					}
/******/ 					delete outdatedDependencies[parentId];
/******/ 					outdatedModules.push(parentId);
/******/ 					queue.push({
/******/ 						chain: chain.concat([parentId]),
/******/ 						id: parentId
/******/ 					});
/******/ 				}
/******/ 			}
/******/ 	
/******/ 			return {
/******/ 				type: "accepted",
/******/ 				moduleId: updateModuleId,
/******/ 				outdatedModules: outdatedModules,
/******/ 				outdatedDependencies: outdatedDependencies
/******/ 			};
/******/ 		}
/******/ 	
/******/ 		function addAllToSet(a, b) {
/******/ 			for(var i = 0; i < b.length; i++) {
/******/ 				var item = b[i];
/******/ 				if(a.indexOf(item) < 0)
/******/ 					a.push(item);
/******/ 			}
/******/ 		}
/******/ 	
/******/ 		// at begin all updates modules are outdated
/******/ 		// the "outdated" status can propagate to parents if they don't accept the children
/******/ 		var outdatedDependencies = {};
/******/ 		var outdatedModules = [];
/******/ 		var appliedUpdate = {};
/******/ 	
/******/ 		var warnUnexpectedRequire = function warnUnexpectedRequire() {
/******/ 			console.warn("[HMR] unexpected require(" + result.moduleId + ") to disposed module");
/******/ 		};
/******/ 	
/******/ 		for(var id in hotUpdate) {
/******/ 			if(Object.prototype.hasOwnProperty.call(hotUpdate, id)) {
/******/ 				moduleId = toModuleId(id);
/******/ 				var result;
/******/ 				if(hotUpdate[id]) {
/******/ 					result = getAffectedStuff(moduleId);
/******/ 				} else {
/******/ 					result = {
/******/ 						type: "disposed",
/******/ 						moduleId: id
/******/ 					};
/******/ 				}
/******/ 				var abortError = false;
/******/ 				var doApply = false;
/******/ 				var doDispose = false;
/******/ 				var chainInfo = "";
/******/ 				if(result.chain) {
/******/ 					chainInfo = "\nUpdate propagation: " + result.chain.join(" -> ");
/******/ 				}
/******/ 				switch(result.type) {
/******/ 					case "self-declined":
/******/ 						if(options.onDeclined)
/******/ 							options.onDeclined(result);
/******/ 						if(!options.ignoreDeclined)
/******/ 							abortError = new Error("Aborted because of self decline: " + result.moduleId + chainInfo);
/******/ 						break;
/******/ 					case "declined":
/******/ 						if(options.onDeclined)
/******/ 							options.onDeclined(result);
/******/ 						if(!options.ignoreDeclined)
/******/ 							abortError = new Error("Aborted because of declined dependency: " + result.moduleId + " in " + result.parentId + chainInfo);
/******/ 						break;
/******/ 					case "unaccepted":
/******/ 						if(options.onUnaccepted)
/******/ 							options.onUnaccepted(result);
/******/ 						if(!options.ignoreUnaccepted)
/******/ 							abortError = new Error("Aborted because " + moduleId + " is not accepted" + chainInfo);
/******/ 						break;
/******/ 					case "accepted":
/******/ 						if(options.onAccepted)
/******/ 							options.onAccepted(result);
/******/ 						doApply = true;
/******/ 						break;
/******/ 					case "disposed":
/******/ 						if(options.onDisposed)
/******/ 							options.onDisposed(result);
/******/ 						doDispose = true;
/******/ 						break;
/******/ 					default:
/******/ 						throw new Error("Unexception type " + result.type);
/******/ 				}
/******/ 				if(abortError) {
/******/ 					hotSetStatus("abort");
/******/ 					return Promise.reject(abortError);
/******/ 				}
/******/ 				if(doApply) {
/******/ 					appliedUpdate[moduleId] = hotUpdate[moduleId];
/******/ 					addAllToSet(outdatedModules, result.outdatedModules);
/******/ 					for(moduleId in result.outdatedDependencies) {
/******/ 						if(Object.prototype.hasOwnProperty.call(result.outdatedDependencies, moduleId)) {
/******/ 							if(!outdatedDependencies[moduleId])
/******/ 								outdatedDependencies[moduleId] = [];
/******/ 							addAllToSet(outdatedDependencies[moduleId], result.outdatedDependencies[moduleId]);
/******/ 						}
/******/ 					}
/******/ 				}
/******/ 				if(doDispose) {
/******/ 					addAllToSet(outdatedModules, [result.moduleId]);
/******/ 					appliedUpdate[moduleId] = warnUnexpectedRequire;
/******/ 				}
/******/ 			}
/******/ 		}
/******/ 	
/******/ 		// Store self accepted outdated modules to require them later by the module system
/******/ 		var outdatedSelfAcceptedModules = [];
/******/ 		for(i = 0; i < outdatedModules.length; i++) {
/******/ 			moduleId = outdatedModules[i];
/******/ 			if(installedModules[moduleId] && installedModules[moduleId].hot._selfAccepted)
/******/ 				outdatedSelfAcceptedModules.push({
/******/ 					module: moduleId,
/******/ 					errorHandler: installedModules[moduleId].hot._selfAccepted
/******/ 				});
/******/ 		}
/******/ 	
/******/ 		// Now in "dispose" phase
/******/ 		hotSetStatus("dispose");
/******/ 		Object.keys(hotAvailableFilesMap).forEach(function(chunkId) {
/******/ 			if(hotAvailableFilesMap[chunkId] === false) {
/******/ 				hotDisposeChunk(chunkId);
/******/ 			}
/******/ 		});
/******/ 	
/******/ 		var idx;
/******/ 		var queue = outdatedModules.slice();
/******/ 		while(queue.length > 0) {
/******/ 			moduleId = queue.pop();
/******/ 			module = installedModules[moduleId];
/******/ 			if(!module) continue;
/******/ 	
/******/ 			var data = {};
/******/ 	
/******/ 			// Call dispose handlers
/******/ 			var disposeHandlers = module.hot._disposeHandlers;
/******/ 			for(j = 0; j < disposeHandlers.length; j++) {
/******/ 				cb = disposeHandlers[j];
/******/ 				cb(data);
/******/ 			}
/******/ 			hotCurrentModuleData[moduleId] = data;
/******/ 	
/******/ 			// disable module (this disables requires from this module)
/******/ 			module.hot.active = false;
/******/ 	
/******/ 			// remove module from cache
/******/ 			delete installedModules[moduleId];
/******/ 	
/******/ 			// when disposing there is no need to call dispose handler
/******/ 			delete outdatedDependencies[moduleId];
/******/ 	
/******/ 			// remove "parents" references from all children
/******/ 			for(j = 0; j < module.children.length; j++) {
/******/ 				var child = installedModules[module.children[j]];
/******/ 				if(!child) continue;
/******/ 				idx = child.parents.indexOf(moduleId);
/******/ 				if(idx >= 0) {
/******/ 					child.parents.splice(idx, 1);
/******/ 				}
/******/ 			}
/******/ 		}
/******/ 	
/******/ 		// remove outdated dependency from module children
/******/ 		var dependency;
/******/ 		var moduleOutdatedDependencies;
/******/ 		for(moduleId in outdatedDependencies) {
/******/ 			if(Object.prototype.hasOwnProperty.call(outdatedDependencies, moduleId)) {
/******/ 				module = installedModules[moduleId];
/******/ 				if(module) {
/******/ 					moduleOutdatedDependencies = outdatedDependencies[moduleId];
/******/ 					for(j = 0; j < moduleOutdatedDependencies.length; j++) {
/******/ 						dependency = moduleOutdatedDependencies[j];
/******/ 						idx = module.children.indexOf(dependency);
/******/ 						if(idx >= 0) module.children.splice(idx, 1);
/******/ 					}
/******/ 				}
/******/ 			}
/******/ 		}
/******/ 	
/******/ 		// Not in "apply" phase
/******/ 		hotSetStatus("apply");
/******/ 	
/******/ 		hotCurrentHash = hotUpdateNewHash;
/******/ 	
/******/ 		// insert new code
/******/ 		for(moduleId in appliedUpdate) {
/******/ 			if(Object.prototype.hasOwnProperty.call(appliedUpdate, moduleId)) {
/******/ 				modules[moduleId] = appliedUpdate[moduleId];
/******/ 			}
/******/ 		}
/******/ 	
/******/ 		// call accept handlers
/******/ 		var error = null;
/******/ 		for(moduleId in outdatedDependencies) {
/******/ 			if(Object.prototype.hasOwnProperty.call(outdatedDependencies, moduleId)) {
/******/ 				module = installedModules[moduleId];
/******/ 				if(module) {
/******/ 					moduleOutdatedDependencies = outdatedDependencies[moduleId];
/******/ 					var callbacks = [];
/******/ 					for(i = 0; i < moduleOutdatedDependencies.length; i++) {
/******/ 						dependency = moduleOutdatedDependencies[i];
/******/ 						cb = module.hot._acceptedDependencies[dependency];
/******/ 						if(cb) {
/******/ 							if(callbacks.indexOf(cb) >= 0) continue;
/******/ 							callbacks.push(cb);
/******/ 						}
/******/ 					}
/******/ 					for(i = 0; i < callbacks.length; i++) {
/******/ 						cb = callbacks[i];
/******/ 						try {
/******/ 							cb(moduleOutdatedDependencies);
/******/ 						} catch(err) {
/******/ 							if(options.onErrored) {
/******/ 								options.onErrored({
/******/ 									type: "accept-errored",
/******/ 									moduleId: moduleId,
/******/ 									dependencyId: moduleOutdatedDependencies[i],
/******/ 									error: err
/******/ 								});
/******/ 							}
/******/ 							if(!options.ignoreErrored) {
/******/ 								if(!error)
/******/ 									error = err;
/******/ 							}
/******/ 						}
/******/ 					}
/******/ 				}
/******/ 			}
/******/ 		}
/******/ 	
/******/ 		// Load self accepted modules
/******/ 		for(i = 0; i < outdatedSelfAcceptedModules.length; i++) {
/******/ 			var item = outdatedSelfAcceptedModules[i];
/******/ 			moduleId = item.module;
/******/ 			hotCurrentParents = [moduleId];
/******/ 			try {
/******/ 				__webpack_require__(moduleId);
/******/ 			} catch(err) {
/******/ 				if(typeof item.errorHandler === "function") {
/******/ 					try {
/******/ 						item.errorHandler(err);
/******/ 					} catch(err2) {
/******/ 						if(options.onErrored) {
/******/ 							options.onErrored({
/******/ 								type: "self-accept-error-handler-errored",
/******/ 								moduleId: moduleId,
/******/ 								error: err2,
/******/ 								orginalError: err, // TODO remove in webpack 4
/******/ 								originalError: err
/******/ 							});
/******/ 						}
/******/ 						if(!options.ignoreErrored) {
/******/ 							if(!error)
/******/ 								error = err2;
/******/ 						}
/******/ 						if(!error)
/******/ 							error = err;
/******/ 					}
/******/ 				} else {
/******/ 					if(options.onErrored) {
/******/ 						options.onErrored({
/******/ 							type: "self-accept-errored",
/******/ 							moduleId: moduleId,
/******/ 							error: err
/******/ 						});
/******/ 					}
/******/ 					if(!options.ignoreErrored) {
/******/ 						if(!error)
/******/ 							error = err;
/******/ 					}
/******/ 				}
/******/ 			}
/******/ 		}
/******/ 	
/******/ 		// handle errors in accept handlers and self accepted module load
/******/ 		if(error) {
/******/ 			hotSetStatus("fail");
/******/ 			return Promise.reject(error);
/******/ 		}
/******/ 	
/******/ 		hotSetStatus("idle");
/******/ 		return new Promise(function(resolve) {
/******/ 			resolve(outdatedModules);
/******/ 		});
/******/ 	}
/******/
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
/******/ 			exports: {},
/******/ 			hot: hotCreateModule(moduleId),
/******/ 			parents: (hotCurrentParentsTemp = hotCurrentParents, hotCurrentParents = [], hotCurrentParentsTemp),
/******/ 			children: []
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, hotCreateRequire(moduleId));
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
/******/ 			Object.defineProperty(exports, name, {
/******/ 				configurable: false,
/******/ 				enumerable: true,
/******/ 				get: getter
/******/ 			});
/******/ 		}
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
/******/ 	__webpack_require__.p = "/dist/";
/******/
/******/ 	// __webpack_hash__
/******/ 	__webpack_require__.h = function() { return hotCurrentHash; };
/******/
/******/ 	// Load entry module and return exports
/******/ 	return hotCreateRequire(1)(__webpack_require__.s = 1);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, exports) {

var ENTITIES = [['Aacute', [193]], ['aacute', [225]], ['Abreve', [258]], ['abreve', [259]], ['ac', [8766]], ['acd', [8767]], ['acE', [8766, 819]], ['Acirc', [194]], ['acirc', [226]], ['acute', [180]], ['Acy', [1040]], ['acy', [1072]], ['AElig', [198]], ['aelig', [230]], ['af', [8289]], ['Afr', [120068]], ['afr', [120094]], ['Agrave', [192]], ['agrave', [224]], ['alefsym', [8501]], ['aleph', [8501]], ['Alpha', [913]], ['alpha', [945]], ['Amacr', [256]], ['amacr', [257]], ['amalg', [10815]], ['amp', [38]], ['AMP', [38]], ['andand', [10837]], ['And', [10835]], ['and', [8743]], ['andd', [10844]], ['andslope', [10840]], ['andv', [10842]], ['ang', [8736]], ['ange', [10660]], ['angle', [8736]], ['angmsdaa', [10664]], ['angmsdab', [10665]], ['angmsdac', [10666]], ['angmsdad', [10667]], ['angmsdae', [10668]], ['angmsdaf', [10669]], ['angmsdag', [10670]], ['angmsdah', [10671]], ['angmsd', [8737]], ['angrt', [8735]], ['angrtvb', [8894]], ['angrtvbd', [10653]], ['angsph', [8738]], ['angst', [197]], ['angzarr', [9084]], ['Aogon', [260]], ['aogon', [261]], ['Aopf', [120120]], ['aopf', [120146]], ['apacir', [10863]], ['ap', [8776]], ['apE', [10864]], ['ape', [8778]], ['apid', [8779]], ['apos', [39]], ['ApplyFunction', [8289]], ['approx', [8776]], ['approxeq', [8778]], ['Aring', [197]], ['aring', [229]], ['Ascr', [119964]], ['ascr', [119990]], ['Assign', [8788]], ['ast', [42]], ['asymp', [8776]], ['asympeq', [8781]], ['Atilde', [195]], ['atilde', [227]], ['Auml', [196]], ['auml', [228]], ['awconint', [8755]], ['awint', [10769]], ['backcong', [8780]], ['backepsilon', [1014]], ['backprime', [8245]], ['backsim', [8765]], ['backsimeq', [8909]], ['Backslash', [8726]], ['Barv', [10983]], ['barvee', [8893]], ['barwed', [8965]], ['Barwed', [8966]], ['barwedge', [8965]], ['bbrk', [9141]], ['bbrktbrk', [9142]], ['bcong', [8780]], ['Bcy', [1041]], ['bcy', [1073]], ['bdquo', [8222]], ['becaus', [8757]], ['because', [8757]], ['Because', [8757]], ['bemptyv', [10672]], ['bepsi', [1014]], ['bernou', [8492]], ['Bernoullis', [8492]], ['Beta', [914]], ['beta', [946]], ['beth', [8502]], ['between', [8812]], ['Bfr', [120069]], ['bfr', [120095]], ['bigcap', [8898]], ['bigcirc', [9711]], ['bigcup', [8899]], ['bigodot', [10752]], ['bigoplus', [10753]], ['bigotimes', [10754]], ['bigsqcup', [10758]], ['bigstar', [9733]], ['bigtriangledown', [9661]], ['bigtriangleup', [9651]], ['biguplus', [10756]], ['bigvee', [8897]], ['bigwedge', [8896]], ['bkarow', [10509]], ['blacklozenge', [10731]], ['blacksquare', [9642]], ['blacktriangle', [9652]], ['blacktriangledown', [9662]], ['blacktriangleleft', [9666]], ['blacktriangleright', [9656]], ['blank', [9251]], ['blk12', [9618]], ['blk14', [9617]], ['blk34', [9619]], ['block', [9608]], ['bne', [61, 8421]], ['bnequiv', [8801, 8421]], ['bNot', [10989]], ['bnot', [8976]], ['Bopf', [120121]], ['bopf', [120147]], ['bot', [8869]], ['bottom', [8869]], ['bowtie', [8904]], ['boxbox', [10697]], ['boxdl', [9488]], ['boxdL', [9557]], ['boxDl', [9558]], ['boxDL', [9559]], ['boxdr', [9484]], ['boxdR', [9554]], ['boxDr', [9555]], ['boxDR', [9556]], ['boxh', [9472]], ['boxH', [9552]], ['boxhd', [9516]], ['boxHd', [9572]], ['boxhD', [9573]], ['boxHD', [9574]], ['boxhu', [9524]], ['boxHu', [9575]], ['boxhU', [9576]], ['boxHU', [9577]], ['boxminus', [8863]], ['boxplus', [8862]], ['boxtimes', [8864]], ['boxul', [9496]], ['boxuL', [9563]], ['boxUl', [9564]], ['boxUL', [9565]], ['boxur', [9492]], ['boxuR', [9560]], ['boxUr', [9561]], ['boxUR', [9562]], ['boxv', [9474]], ['boxV', [9553]], ['boxvh', [9532]], ['boxvH', [9578]], ['boxVh', [9579]], ['boxVH', [9580]], ['boxvl', [9508]], ['boxvL', [9569]], ['boxVl', [9570]], ['boxVL', [9571]], ['boxvr', [9500]], ['boxvR', [9566]], ['boxVr', [9567]], ['boxVR', [9568]], ['bprime', [8245]], ['breve', [728]], ['Breve', [728]], ['brvbar', [166]], ['bscr', [119991]], ['Bscr', [8492]], ['bsemi', [8271]], ['bsim', [8765]], ['bsime', [8909]], ['bsolb', [10693]], ['bsol', [92]], ['bsolhsub', [10184]], ['bull', [8226]], ['bullet', [8226]], ['bump', [8782]], ['bumpE', [10926]], ['bumpe', [8783]], ['Bumpeq', [8782]], ['bumpeq', [8783]], ['Cacute', [262]], ['cacute', [263]], ['capand', [10820]], ['capbrcup', [10825]], ['capcap', [10827]], ['cap', [8745]], ['Cap', [8914]], ['capcup', [10823]], ['capdot', [10816]], ['CapitalDifferentialD', [8517]], ['caps', [8745, 65024]], ['caret', [8257]], ['caron', [711]], ['Cayleys', [8493]], ['ccaps', [10829]], ['Ccaron', [268]], ['ccaron', [269]], ['Ccedil', [199]], ['ccedil', [231]], ['Ccirc', [264]], ['ccirc', [265]], ['Cconint', [8752]], ['ccups', [10828]], ['ccupssm', [10832]], ['Cdot', [266]], ['cdot', [267]], ['cedil', [184]], ['Cedilla', [184]], ['cemptyv', [10674]], ['cent', [162]], ['centerdot', [183]], ['CenterDot', [183]], ['cfr', [120096]], ['Cfr', [8493]], ['CHcy', [1063]], ['chcy', [1095]], ['check', [10003]], ['checkmark', [10003]], ['Chi', [935]], ['chi', [967]], ['circ', [710]], ['circeq', [8791]], ['circlearrowleft', [8634]], ['circlearrowright', [8635]], ['circledast', [8859]], ['circledcirc', [8858]], ['circleddash', [8861]], ['CircleDot', [8857]], ['circledR', [174]], ['circledS', [9416]], ['CircleMinus', [8854]], ['CirclePlus', [8853]], ['CircleTimes', [8855]], ['cir', [9675]], ['cirE', [10691]], ['cire', [8791]], ['cirfnint', [10768]], ['cirmid', [10991]], ['cirscir', [10690]], ['ClockwiseContourIntegral', [8754]], ['clubs', [9827]], ['clubsuit', [9827]], ['colon', [58]], ['Colon', [8759]], ['Colone', [10868]], ['colone', [8788]], ['coloneq', [8788]], ['comma', [44]], ['commat', [64]], ['comp', [8705]], ['compfn', [8728]], ['complement', [8705]], ['complexes', [8450]], ['cong', [8773]], ['congdot', [10861]], ['Congruent', [8801]], ['conint', [8750]], ['Conint', [8751]], ['ContourIntegral', [8750]], ['copf', [120148]], ['Copf', [8450]], ['coprod', [8720]], ['Coproduct', [8720]], ['copy', [169]], ['COPY', [169]], ['copysr', [8471]], ['CounterClockwiseContourIntegral', [8755]], ['crarr', [8629]], ['cross', [10007]], ['Cross', [10799]], ['Cscr', [119966]], ['cscr', [119992]], ['csub', [10959]], ['csube', [10961]], ['csup', [10960]], ['csupe', [10962]], ['ctdot', [8943]], ['cudarrl', [10552]], ['cudarrr', [10549]], ['cuepr', [8926]], ['cuesc', [8927]], ['cularr', [8630]], ['cularrp', [10557]], ['cupbrcap', [10824]], ['cupcap', [10822]], ['CupCap', [8781]], ['cup', [8746]], ['Cup', [8915]], ['cupcup', [10826]], ['cupdot', [8845]], ['cupor', [10821]], ['cups', [8746, 65024]], ['curarr', [8631]], ['curarrm', [10556]], ['curlyeqprec', [8926]], ['curlyeqsucc', [8927]], ['curlyvee', [8910]], ['curlywedge', [8911]], ['curren', [164]], ['curvearrowleft', [8630]], ['curvearrowright', [8631]], ['cuvee', [8910]], ['cuwed', [8911]], ['cwconint', [8754]], ['cwint', [8753]], ['cylcty', [9005]], ['dagger', [8224]], ['Dagger', [8225]], ['daleth', [8504]], ['darr', [8595]], ['Darr', [8609]], ['dArr', [8659]], ['dash', [8208]], ['Dashv', [10980]], ['dashv', [8867]], ['dbkarow', [10511]], ['dblac', [733]], ['Dcaron', [270]], ['dcaron', [271]], ['Dcy', [1044]], ['dcy', [1076]], ['ddagger', [8225]], ['ddarr', [8650]], ['DD', [8517]], ['dd', [8518]], ['DDotrahd', [10513]], ['ddotseq', [10871]], ['deg', [176]], ['Del', [8711]], ['Delta', [916]], ['delta', [948]], ['demptyv', [10673]], ['dfisht', [10623]], ['Dfr', [120071]], ['dfr', [120097]], ['dHar', [10597]], ['dharl', [8643]], ['dharr', [8642]], ['DiacriticalAcute', [180]], ['DiacriticalDot', [729]], ['DiacriticalDoubleAcute', [733]], ['DiacriticalGrave', [96]], ['DiacriticalTilde', [732]], ['diam', [8900]], ['diamond', [8900]], ['Diamond', [8900]], ['diamondsuit', [9830]], ['diams', [9830]], ['die', [168]], ['DifferentialD', [8518]], ['digamma', [989]], ['disin', [8946]], ['div', [247]], ['divide', [247]], ['divideontimes', [8903]], ['divonx', [8903]], ['DJcy', [1026]], ['djcy', [1106]], ['dlcorn', [8990]], ['dlcrop', [8973]], ['dollar', [36]], ['Dopf', [120123]], ['dopf', [120149]], ['Dot', [168]], ['dot', [729]], ['DotDot', [8412]], ['doteq', [8784]], ['doteqdot', [8785]], ['DotEqual', [8784]], ['dotminus', [8760]], ['dotplus', [8724]], ['dotsquare', [8865]], ['doublebarwedge', [8966]], ['DoubleContourIntegral', [8751]], ['DoubleDot', [168]], ['DoubleDownArrow', [8659]], ['DoubleLeftArrow', [8656]], ['DoubleLeftRightArrow', [8660]], ['DoubleLeftTee', [10980]], ['DoubleLongLeftArrow', [10232]], ['DoubleLongLeftRightArrow', [10234]], ['DoubleLongRightArrow', [10233]], ['DoubleRightArrow', [8658]], ['DoubleRightTee', [8872]], ['DoubleUpArrow', [8657]], ['DoubleUpDownArrow', [8661]], ['DoubleVerticalBar', [8741]], ['DownArrowBar', [10515]], ['downarrow', [8595]], ['DownArrow', [8595]], ['Downarrow', [8659]], ['DownArrowUpArrow', [8693]], ['DownBreve', [785]], ['downdownarrows', [8650]], ['downharpoonleft', [8643]], ['downharpoonright', [8642]], ['DownLeftRightVector', [10576]], ['DownLeftTeeVector', [10590]], ['DownLeftVectorBar', [10582]], ['DownLeftVector', [8637]], ['DownRightTeeVector', [10591]], ['DownRightVectorBar', [10583]], ['DownRightVector', [8641]], ['DownTeeArrow', [8615]], ['DownTee', [8868]], ['drbkarow', [10512]], ['drcorn', [8991]], ['drcrop', [8972]], ['Dscr', [119967]], ['dscr', [119993]], ['DScy', [1029]], ['dscy', [1109]], ['dsol', [10742]], ['Dstrok', [272]], ['dstrok', [273]], ['dtdot', [8945]], ['dtri', [9663]], ['dtrif', [9662]], ['duarr', [8693]], ['duhar', [10607]], ['dwangle', [10662]], ['DZcy', [1039]], ['dzcy', [1119]], ['dzigrarr', [10239]], ['Eacute', [201]], ['eacute', [233]], ['easter', [10862]], ['Ecaron', [282]], ['ecaron', [283]], ['Ecirc', [202]], ['ecirc', [234]], ['ecir', [8790]], ['ecolon', [8789]], ['Ecy', [1069]], ['ecy', [1101]], ['eDDot', [10871]], ['Edot', [278]], ['edot', [279]], ['eDot', [8785]], ['ee', [8519]], ['efDot', [8786]], ['Efr', [120072]], ['efr', [120098]], ['eg', [10906]], ['Egrave', [200]], ['egrave', [232]], ['egs', [10902]], ['egsdot', [10904]], ['el', [10905]], ['Element', [8712]], ['elinters', [9191]], ['ell', [8467]], ['els', [10901]], ['elsdot', [10903]], ['Emacr', [274]], ['emacr', [275]], ['empty', [8709]], ['emptyset', [8709]], ['EmptySmallSquare', [9723]], ['emptyv', [8709]], ['EmptyVerySmallSquare', [9643]], ['emsp13', [8196]], ['emsp14', [8197]], ['emsp', [8195]], ['ENG', [330]], ['eng', [331]], ['ensp', [8194]], ['Eogon', [280]], ['eogon', [281]], ['Eopf', [120124]], ['eopf', [120150]], ['epar', [8917]], ['eparsl', [10723]], ['eplus', [10865]], ['epsi', [949]], ['Epsilon', [917]], ['epsilon', [949]], ['epsiv', [1013]], ['eqcirc', [8790]], ['eqcolon', [8789]], ['eqsim', [8770]], ['eqslantgtr', [10902]], ['eqslantless', [10901]], ['Equal', [10869]], ['equals', [61]], ['EqualTilde', [8770]], ['equest', [8799]], ['Equilibrium', [8652]], ['equiv', [8801]], ['equivDD', [10872]], ['eqvparsl', [10725]], ['erarr', [10609]], ['erDot', [8787]], ['escr', [8495]], ['Escr', [8496]], ['esdot', [8784]], ['Esim', [10867]], ['esim', [8770]], ['Eta', [919]], ['eta', [951]], ['ETH', [208]], ['eth', [240]], ['Euml', [203]], ['euml', [235]], ['euro', [8364]], ['excl', [33]], ['exist', [8707]], ['Exists', [8707]], ['expectation', [8496]], ['exponentiale', [8519]], ['ExponentialE', [8519]], ['fallingdotseq', [8786]], ['Fcy', [1060]], ['fcy', [1092]], ['female', [9792]], ['ffilig', [64259]], ['fflig', [64256]], ['ffllig', [64260]], ['Ffr', [120073]], ['ffr', [120099]], ['filig', [64257]], ['FilledSmallSquare', [9724]], ['FilledVerySmallSquare', [9642]], ['fjlig', [102, 106]], ['flat', [9837]], ['fllig', [64258]], ['fltns', [9649]], ['fnof', [402]], ['Fopf', [120125]], ['fopf', [120151]], ['forall', [8704]], ['ForAll', [8704]], ['fork', [8916]], ['forkv', [10969]], ['Fouriertrf', [8497]], ['fpartint', [10765]], ['frac12', [189]], ['frac13', [8531]], ['frac14', [188]], ['frac15', [8533]], ['frac16', [8537]], ['frac18', [8539]], ['frac23', [8532]], ['frac25', [8534]], ['frac34', [190]], ['frac35', [8535]], ['frac38', [8540]], ['frac45', [8536]], ['frac56', [8538]], ['frac58', [8541]], ['frac78', [8542]], ['frasl', [8260]], ['frown', [8994]], ['fscr', [119995]], ['Fscr', [8497]], ['gacute', [501]], ['Gamma', [915]], ['gamma', [947]], ['Gammad', [988]], ['gammad', [989]], ['gap', [10886]], ['Gbreve', [286]], ['gbreve', [287]], ['Gcedil', [290]], ['Gcirc', [284]], ['gcirc', [285]], ['Gcy', [1043]], ['gcy', [1075]], ['Gdot', [288]], ['gdot', [289]], ['ge', [8805]], ['gE', [8807]], ['gEl', [10892]], ['gel', [8923]], ['geq', [8805]], ['geqq', [8807]], ['geqslant', [10878]], ['gescc', [10921]], ['ges', [10878]], ['gesdot', [10880]], ['gesdoto', [10882]], ['gesdotol', [10884]], ['gesl', [8923, 65024]], ['gesles', [10900]], ['Gfr', [120074]], ['gfr', [120100]], ['gg', [8811]], ['Gg', [8921]], ['ggg', [8921]], ['gimel', [8503]], ['GJcy', [1027]], ['gjcy', [1107]], ['gla', [10917]], ['gl', [8823]], ['glE', [10898]], ['glj', [10916]], ['gnap', [10890]], ['gnapprox', [10890]], ['gne', [10888]], ['gnE', [8809]], ['gneq', [10888]], ['gneqq', [8809]], ['gnsim', [8935]], ['Gopf', [120126]], ['gopf', [120152]], ['grave', [96]], ['GreaterEqual', [8805]], ['GreaterEqualLess', [8923]], ['GreaterFullEqual', [8807]], ['GreaterGreater', [10914]], ['GreaterLess', [8823]], ['GreaterSlantEqual', [10878]], ['GreaterTilde', [8819]], ['Gscr', [119970]], ['gscr', [8458]], ['gsim', [8819]], ['gsime', [10894]], ['gsiml', [10896]], ['gtcc', [10919]], ['gtcir', [10874]], ['gt', [62]], ['GT', [62]], ['Gt', [8811]], ['gtdot', [8919]], ['gtlPar', [10645]], ['gtquest', [10876]], ['gtrapprox', [10886]], ['gtrarr', [10616]], ['gtrdot', [8919]], ['gtreqless', [8923]], ['gtreqqless', [10892]], ['gtrless', [8823]], ['gtrsim', [8819]], ['gvertneqq', [8809, 65024]], ['gvnE', [8809, 65024]], ['Hacek', [711]], ['hairsp', [8202]], ['half', [189]], ['hamilt', [8459]], ['HARDcy', [1066]], ['hardcy', [1098]], ['harrcir', [10568]], ['harr', [8596]], ['hArr', [8660]], ['harrw', [8621]], ['Hat', [94]], ['hbar', [8463]], ['Hcirc', [292]], ['hcirc', [293]], ['hearts', [9829]], ['heartsuit', [9829]], ['hellip', [8230]], ['hercon', [8889]], ['hfr', [120101]], ['Hfr', [8460]], ['HilbertSpace', [8459]], ['hksearow', [10533]], ['hkswarow', [10534]], ['hoarr', [8703]], ['homtht', [8763]], ['hookleftarrow', [8617]], ['hookrightarrow', [8618]], ['hopf', [120153]], ['Hopf', [8461]], ['horbar', [8213]], ['HorizontalLine', [9472]], ['hscr', [119997]], ['Hscr', [8459]], ['hslash', [8463]], ['Hstrok', [294]], ['hstrok', [295]], ['HumpDownHump', [8782]], ['HumpEqual', [8783]], ['hybull', [8259]], ['hyphen', [8208]], ['Iacute', [205]], ['iacute', [237]], ['ic', [8291]], ['Icirc', [206]], ['icirc', [238]], ['Icy', [1048]], ['icy', [1080]], ['Idot', [304]], ['IEcy', [1045]], ['iecy', [1077]], ['iexcl', [161]], ['iff', [8660]], ['ifr', [120102]], ['Ifr', [8465]], ['Igrave', [204]], ['igrave', [236]], ['ii', [8520]], ['iiiint', [10764]], ['iiint', [8749]], ['iinfin', [10716]], ['iiota', [8489]], ['IJlig', [306]], ['ijlig', [307]], ['Imacr', [298]], ['imacr', [299]], ['image', [8465]], ['ImaginaryI', [8520]], ['imagline', [8464]], ['imagpart', [8465]], ['imath', [305]], ['Im', [8465]], ['imof', [8887]], ['imped', [437]], ['Implies', [8658]], ['incare', [8453]], ['in', [8712]], ['infin', [8734]], ['infintie', [10717]], ['inodot', [305]], ['intcal', [8890]], ['int', [8747]], ['Int', [8748]], ['integers', [8484]], ['Integral', [8747]], ['intercal', [8890]], ['Intersection', [8898]], ['intlarhk', [10775]], ['intprod', [10812]], ['InvisibleComma', [8291]], ['InvisibleTimes', [8290]], ['IOcy', [1025]], ['iocy', [1105]], ['Iogon', [302]], ['iogon', [303]], ['Iopf', [120128]], ['iopf', [120154]], ['Iota', [921]], ['iota', [953]], ['iprod', [10812]], ['iquest', [191]], ['iscr', [119998]], ['Iscr', [8464]], ['isin', [8712]], ['isindot', [8949]], ['isinE', [8953]], ['isins', [8948]], ['isinsv', [8947]], ['isinv', [8712]], ['it', [8290]], ['Itilde', [296]], ['itilde', [297]], ['Iukcy', [1030]], ['iukcy', [1110]], ['Iuml', [207]], ['iuml', [239]], ['Jcirc', [308]], ['jcirc', [309]], ['Jcy', [1049]], ['jcy', [1081]], ['Jfr', [120077]], ['jfr', [120103]], ['jmath', [567]], ['Jopf', [120129]], ['jopf', [120155]], ['Jscr', [119973]], ['jscr', [119999]], ['Jsercy', [1032]], ['jsercy', [1112]], ['Jukcy', [1028]], ['jukcy', [1108]], ['Kappa', [922]], ['kappa', [954]], ['kappav', [1008]], ['Kcedil', [310]], ['kcedil', [311]], ['Kcy', [1050]], ['kcy', [1082]], ['Kfr', [120078]], ['kfr', [120104]], ['kgreen', [312]], ['KHcy', [1061]], ['khcy', [1093]], ['KJcy', [1036]], ['kjcy', [1116]], ['Kopf', [120130]], ['kopf', [120156]], ['Kscr', [119974]], ['kscr', [120000]], ['lAarr', [8666]], ['Lacute', [313]], ['lacute', [314]], ['laemptyv', [10676]], ['lagran', [8466]], ['Lambda', [923]], ['lambda', [955]], ['lang', [10216]], ['Lang', [10218]], ['langd', [10641]], ['langle', [10216]], ['lap', [10885]], ['Laplacetrf', [8466]], ['laquo', [171]], ['larrb', [8676]], ['larrbfs', [10527]], ['larr', [8592]], ['Larr', [8606]], ['lArr', [8656]], ['larrfs', [10525]], ['larrhk', [8617]], ['larrlp', [8619]], ['larrpl', [10553]], ['larrsim', [10611]], ['larrtl', [8610]], ['latail', [10521]], ['lAtail', [10523]], ['lat', [10923]], ['late', [10925]], ['lates', [10925, 65024]], ['lbarr', [10508]], ['lBarr', [10510]], ['lbbrk', [10098]], ['lbrace', [123]], ['lbrack', [91]], ['lbrke', [10635]], ['lbrksld', [10639]], ['lbrkslu', [10637]], ['Lcaron', [317]], ['lcaron', [318]], ['Lcedil', [315]], ['lcedil', [316]], ['lceil', [8968]], ['lcub', [123]], ['Lcy', [1051]], ['lcy', [1083]], ['ldca', [10550]], ['ldquo', [8220]], ['ldquor', [8222]], ['ldrdhar', [10599]], ['ldrushar', [10571]], ['ldsh', [8626]], ['le', [8804]], ['lE', [8806]], ['LeftAngleBracket', [10216]], ['LeftArrowBar', [8676]], ['leftarrow', [8592]], ['LeftArrow', [8592]], ['Leftarrow', [8656]], ['LeftArrowRightArrow', [8646]], ['leftarrowtail', [8610]], ['LeftCeiling', [8968]], ['LeftDoubleBracket', [10214]], ['LeftDownTeeVector', [10593]], ['LeftDownVectorBar', [10585]], ['LeftDownVector', [8643]], ['LeftFloor', [8970]], ['leftharpoondown', [8637]], ['leftharpoonup', [8636]], ['leftleftarrows', [8647]], ['leftrightarrow', [8596]], ['LeftRightArrow', [8596]], ['Leftrightarrow', [8660]], ['leftrightarrows', [8646]], ['leftrightharpoons', [8651]], ['leftrightsquigarrow', [8621]], ['LeftRightVector', [10574]], ['LeftTeeArrow', [8612]], ['LeftTee', [8867]], ['LeftTeeVector', [10586]], ['leftthreetimes', [8907]], ['LeftTriangleBar', [10703]], ['LeftTriangle', [8882]], ['LeftTriangleEqual', [8884]], ['LeftUpDownVector', [10577]], ['LeftUpTeeVector', [10592]], ['LeftUpVectorBar', [10584]], ['LeftUpVector', [8639]], ['LeftVectorBar', [10578]], ['LeftVector', [8636]], ['lEg', [10891]], ['leg', [8922]], ['leq', [8804]], ['leqq', [8806]], ['leqslant', [10877]], ['lescc', [10920]], ['les', [10877]], ['lesdot', [10879]], ['lesdoto', [10881]], ['lesdotor', [10883]], ['lesg', [8922, 65024]], ['lesges', [10899]], ['lessapprox', [10885]], ['lessdot', [8918]], ['lesseqgtr', [8922]], ['lesseqqgtr', [10891]], ['LessEqualGreater', [8922]], ['LessFullEqual', [8806]], ['LessGreater', [8822]], ['lessgtr', [8822]], ['LessLess', [10913]], ['lesssim', [8818]], ['LessSlantEqual', [10877]], ['LessTilde', [8818]], ['lfisht', [10620]], ['lfloor', [8970]], ['Lfr', [120079]], ['lfr', [120105]], ['lg', [8822]], ['lgE', [10897]], ['lHar', [10594]], ['lhard', [8637]], ['lharu', [8636]], ['lharul', [10602]], ['lhblk', [9604]], ['LJcy', [1033]], ['ljcy', [1113]], ['llarr', [8647]], ['ll', [8810]], ['Ll', [8920]], ['llcorner', [8990]], ['Lleftarrow', [8666]], ['llhard', [10603]], ['lltri', [9722]], ['Lmidot', [319]], ['lmidot', [320]], ['lmoustache', [9136]], ['lmoust', [9136]], ['lnap', [10889]], ['lnapprox', [10889]], ['lne', [10887]], ['lnE', [8808]], ['lneq', [10887]], ['lneqq', [8808]], ['lnsim', [8934]], ['loang', [10220]], ['loarr', [8701]], ['lobrk', [10214]], ['longleftarrow', [10229]], ['LongLeftArrow', [10229]], ['Longleftarrow', [10232]], ['longleftrightarrow', [10231]], ['LongLeftRightArrow', [10231]], ['Longleftrightarrow', [10234]], ['longmapsto', [10236]], ['longrightarrow', [10230]], ['LongRightArrow', [10230]], ['Longrightarrow', [10233]], ['looparrowleft', [8619]], ['looparrowright', [8620]], ['lopar', [10629]], ['Lopf', [120131]], ['lopf', [120157]], ['loplus', [10797]], ['lotimes', [10804]], ['lowast', [8727]], ['lowbar', [95]], ['LowerLeftArrow', [8601]], ['LowerRightArrow', [8600]], ['loz', [9674]], ['lozenge', [9674]], ['lozf', [10731]], ['lpar', [40]], ['lparlt', [10643]], ['lrarr', [8646]], ['lrcorner', [8991]], ['lrhar', [8651]], ['lrhard', [10605]], ['lrm', [8206]], ['lrtri', [8895]], ['lsaquo', [8249]], ['lscr', [120001]], ['Lscr', [8466]], ['lsh', [8624]], ['Lsh', [8624]], ['lsim', [8818]], ['lsime', [10893]], ['lsimg', [10895]], ['lsqb', [91]], ['lsquo', [8216]], ['lsquor', [8218]], ['Lstrok', [321]], ['lstrok', [322]], ['ltcc', [10918]], ['ltcir', [10873]], ['lt', [60]], ['LT', [60]], ['Lt', [8810]], ['ltdot', [8918]], ['lthree', [8907]], ['ltimes', [8905]], ['ltlarr', [10614]], ['ltquest', [10875]], ['ltri', [9667]], ['ltrie', [8884]], ['ltrif', [9666]], ['ltrPar', [10646]], ['lurdshar', [10570]], ['luruhar', [10598]], ['lvertneqq', [8808, 65024]], ['lvnE', [8808, 65024]], ['macr', [175]], ['male', [9794]], ['malt', [10016]], ['maltese', [10016]], ['Map', [10501]], ['map', [8614]], ['mapsto', [8614]], ['mapstodown', [8615]], ['mapstoleft', [8612]], ['mapstoup', [8613]], ['marker', [9646]], ['mcomma', [10793]], ['Mcy', [1052]], ['mcy', [1084]], ['mdash', [8212]], ['mDDot', [8762]], ['measuredangle', [8737]], ['MediumSpace', [8287]], ['Mellintrf', [8499]], ['Mfr', [120080]], ['mfr', [120106]], ['mho', [8487]], ['micro', [181]], ['midast', [42]], ['midcir', [10992]], ['mid', [8739]], ['middot', [183]], ['minusb', [8863]], ['minus', [8722]], ['minusd', [8760]], ['minusdu', [10794]], ['MinusPlus', [8723]], ['mlcp', [10971]], ['mldr', [8230]], ['mnplus', [8723]], ['models', [8871]], ['Mopf', [120132]], ['mopf', [120158]], ['mp', [8723]], ['mscr', [120002]], ['Mscr', [8499]], ['mstpos', [8766]], ['Mu', [924]], ['mu', [956]], ['multimap', [8888]], ['mumap', [8888]], ['nabla', [8711]], ['Nacute', [323]], ['nacute', [324]], ['nang', [8736, 8402]], ['nap', [8777]], ['napE', [10864, 824]], ['napid', [8779, 824]], ['napos', [329]], ['napprox', [8777]], ['natural', [9838]], ['naturals', [8469]], ['natur', [9838]], ['nbsp', [160]], ['nbump', [8782, 824]], ['nbumpe', [8783, 824]], ['ncap', [10819]], ['Ncaron', [327]], ['ncaron', [328]], ['Ncedil', [325]], ['ncedil', [326]], ['ncong', [8775]], ['ncongdot', [10861, 824]], ['ncup', [10818]], ['Ncy', [1053]], ['ncy', [1085]], ['ndash', [8211]], ['nearhk', [10532]], ['nearr', [8599]], ['neArr', [8663]], ['nearrow', [8599]], ['ne', [8800]], ['nedot', [8784, 824]], ['NegativeMediumSpace', [8203]], ['NegativeThickSpace', [8203]], ['NegativeThinSpace', [8203]], ['NegativeVeryThinSpace', [8203]], ['nequiv', [8802]], ['nesear', [10536]], ['nesim', [8770, 824]], ['NestedGreaterGreater', [8811]], ['NestedLessLess', [8810]], ['nexist', [8708]], ['nexists', [8708]], ['Nfr', [120081]], ['nfr', [120107]], ['ngE', [8807, 824]], ['nge', [8817]], ['ngeq', [8817]], ['ngeqq', [8807, 824]], ['ngeqslant', [10878, 824]], ['nges', [10878, 824]], ['nGg', [8921, 824]], ['ngsim', [8821]], ['nGt', [8811, 8402]], ['ngt', [8815]], ['ngtr', [8815]], ['nGtv', [8811, 824]], ['nharr', [8622]], ['nhArr', [8654]], ['nhpar', [10994]], ['ni', [8715]], ['nis', [8956]], ['nisd', [8954]], ['niv', [8715]], ['NJcy', [1034]], ['njcy', [1114]], ['nlarr', [8602]], ['nlArr', [8653]], ['nldr', [8229]], ['nlE', [8806, 824]], ['nle', [8816]], ['nleftarrow', [8602]], ['nLeftarrow', [8653]], ['nleftrightarrow', [8622]], ['nLeftrightarrow', [8654]], ['nleq', [8816]], ['nleqq', [8806, 824]], ['nleqslant', [10877, 824]], ['nles', [10877, 824]], ['nless', [8814]], ['nLl', [8920, 824]], ['nlsim', [8820]], ['nLt', [8810, 8402]], ['nlt', [8814]], ['nltri', [8938]], ['nltrie', [8940]], ['nLtv', [8810, 824]], ['nmid', [8740]], ['NoBreak', [8288]], ['NonBreakingSpace', [160]], ['nopf', [120159]], ['Nopf', [8469]], ['Not', [10988]], ['not', [172]], ['NotCongruent', [8802]], ['NotCupCap', [8813]], ['NotDoubleVerticalBar', [8742]], ['NotElement', [8713]], ['NotEqual', [8800]], ['NotEqualTilde', [8770, 824]], ['NotExists', [8708]], ['NotGreater', [8815]], ['NotGreaterEqual', [8817]], ['NotGreaterFullEqual', [8807, 824]], ['NotGreaterGreater', [8811, 824]], ['NotGreaterLess', [8825]], ['NotGreaterSlantEqual', [10878, 824]], ['NotGreaterTilde', [8821]], ['NotHumpDownHump', [8782, 824]], ['NotHumpEqual', [8783, 824]], ['notin', [8713]], ['notindot', [8949, 824]], ['notinE', [8953, 824]], ['notinva', [8713]], ['notinvb', [8951]], ['notinvc', [8950]], ['NotLeftTriangleBar', [10703, 824]], ['NotLeftTriangle', [8938]], ['NotLeftTriangleEqual', [8940]], ['NotLess', [8814]], ['NotLessEqual', [8816]], ['NotLessGreater', [8824]], ['NotLessLess', [8810, 824]], ['NotLessSlantEqual', [10877, 824]], ['NotLessTilde', [8820]], ['NotNestedGreaterGreater', [10914, 824]], ['NotNestedLessLess', [10913, 824]], ['notni', [8716]], ['notniva', [8716]], ['notnivb', [8958]], ['notnivc', [8957]], ['NotPrecedes', [8832]], ['NotPrecedesEqual', [10927, 824]], ['NotPrecedesSlantEqual', [8928]], ['NotReverseElement', [8716]], ['NotRightTriangleBar', [10704, 824]], ['NotRightTriangle', [8939]], ['NotRightTriangleEqual', [8941]], ['NotSquareSubset', [8847, 824]], ['NotSquareSubsetEqual', [8930]], ['NotSquareSuperset', [8848, 824]], ['NotSquareSupersetEqual', [8931]], ['NotSubset', [8834, 8402]], ['NotSubsetEqual', [8840]], ['NotSucceeds', [8833]], ['NotSucceedsEqual', [10928, 824]], ['NotSucceedsSlantEqual', [8929]], ['NotSucceedsTilde', [8831, 824]], ['NotSuperset', [8835, 8402]], ['NotSupersetEqual', [8841]], ['NotTilde', [8769]], ['NotTildeEqual', [8772]], ['NotTildeFullEqual', [8775]], ['NotTildeTilde', [8777]], ['NotVerticalBar', [8740]], ['nparallel', [8742]], ['npar', [8742]], ['nparsl', [11005, 8421]], ['npart', [8706, 824]], ['npolint', [10772]], ['npr', [8832]], ['nprcue', [8928]], ['nprec', [8832]], ['npreceq', [10927, 824]], ['npre', [10927, 824]], ['nrarrc', [10547, 824]], ['nrarr', [8603]], ['nrArr', [8655]], ['nrarrw', [8605, 824]], ['nrightarrow', [8603]], ['nRightarrow', [8655]], ['nrtri', [8939]], ['nrtrie', [8941]], ['nsc', [8833]], ['nsccue', [8929]], ['nsce', [10928, 824]], ['Nscr', [119977]], ['nscr', [120003]], ['nshortmid', [8740]], ['nshortparallel', [8742]], ['nsim', [8769]], ['nsime', [8772]], ['nsimeq', [8772]], ['nsmid', [8740]], ['nspar', [8742]], ['nsqsube', [8930]], ['nsqsupe', [8931]], ['nsub', [8836]], ['nsubE', [10949, 824]], ['nsube', [8840]], ['nsubset', [8834, 8402]], ['nsubseteq', [8840]], ['nsubseteqq', [10949, 824]], ['nsucc', [8833]], ['nsucceq', [10928, 824]], ['nsup', [8837]], ['nsupE', [10950, 824]], ['nsupe', [8841]], ['nsupset', [8835, 8402]], ['nsupseteq', [8841]], ['nsupseteqq', [10950, 824]], ['ntgl', [8825]], ['Ntilde', [209]], ['ntilde', [241]], ['ntlg', [8824]], ['ntriangleleft', [8938]], ['ntrianglelefteq', [8940]], ['ntriangleright', [8939]], ['ntrianglerighteq', [8941]], ['Nu', [925]], ['nu', [957]], ['num', [35]], ['numero', [8470]], ['numsp', [8199]], ['nvap', [8781, 8402]], ['nvdash', [8876]], ['nvDash', [8877]], ['nVdash', [8878]], ['nVDash', [8879]], ['nvge', [8805, 8402]], ['nvgt', [62, 8402]], ['nvHarr', [10500]], ['nvinfin', [10718]], ['nvlArr', [10498]], ['nvle', [8804, 8402]], ['nvlt', [60, 8402]], ['nvltrie', [8884, 8402]], ['nvrArr', [10499]], ['nvrtrie', [8885, 8402]], ['nvsim', [8764, 8402]], ['nwarhk', [10531]], ['nwarr', [8598]], ['nwArr', [8662]], ['nwarrow', [8598]], ['nwnear', [10535]], ['Oacute', [211]], ['oacute', [243]], ['oast', [8859]], ['Ocirc', [212]], ['ocirc', [244]], ['ocir', [8858]], ['Ocy', [1054]], ['ocy', [1086]], ['odash', [8861]], ['Odblac', [336]], ['odblac', [337]], ['odiv', [10808]], ['odot', [8857]], ['odsold', [10684]], ['OElig', [338]], ['oelig', [339]], ['ofcir', [10687]], ['Ofr', [120082]], ['ofr', [120108]], ['ogon', [731]], ['Ograve', [210]], ['ograve', [242]], ['ogt', [10689]], ['ohbar', [10677]], ['ohm', [937]], ['oint', [8750]], ['olarr', [8634]], ['olcir', [10686]], ['olcross', [10683]], ['oline', [8254]], ['olt', [10688]], ['Omacr', [332]], ['omacr', [333]], ['Omega', [937]], ['omega', [969]], ['Omicron', [927]], ['omicron', [959]], ['omid', [10678]], ['ominus', [8854]], ['Oopf', [120134]], ['oopf', [120160]], ['opar', [10679]], ['OpenCurlyDoubleQuote', [8220]], ['OpenCurlyQuote', [8216]], ['operp', [10681]], ['oplus', [8853]], ['orarr', [8635]], ['Or', [10836]], ['or', [8744]], ['ord', [10845]], ['order', [8500]], ['orderof', [8500]], ['ordf', [170]], ['ordm', [186]], ['origof', [8886]], ['oror', [10838]], ['orslope', [10839]], ['orv', [10843]], ['oS', [9416]], ['Oscr', [119978]], ['oscr', [8500]], ['Oslash', [216]], ['oslash', [248]], ['osol', [8856]], ['Otilde', [213]], ['otilde', [245]], ['otimesas', [10806]], ['Otimes', [10807]], ['otimes', [8855]], ['Ouml', [214]], ['ouml', [246]], ['ovbar', [9021]], ['OverBar', [8254]], ['OverBrace', [9182]], ['OverBracket', [9140]], ['OverParenthesis', [9180]], ['para', [182]], ['parallel', [8741]], ['par', [8741]], ['parsim', [10995]], ['parsl', [11005]], ['part', [8706]], ['PartialD', [8706]], ['Pcy', [1055]], ['pcy', [1087]], ['percnt', [37]], ['period', [46]], ['permil', [8240]], ['perp', [8869]], ['pertenk', [8241]], ['Pfr', [120083]], ['pfr', [120109]], ['Phi', [934]], ['phi', [966]], ['phiv', [981]], ['phmmat', [8499]], ['phone', [9742]], ['Pi', [928]], ['pi', [960]], ['pitchfork', [8916]], ['piv', [982]], ['planck', [8463]], ['planckh', [8462]], ['plankv', [8463]], ['plusacir', [10787]], ['plusb', [8862]], ['pluscir', [10786]], ['plus', [43]], ['plusdo', [8724]], ['plusdu', [10789]], ['pluse', [10866]], ['PlusMinus', [177]], ['plusmn', [177]], ['plussim', [10790]], ['plustwo', [10791]], ['pm', [177]], ['Poincareplane', [8460]], ['pointint', [10773]], ['popf', [120161]], ['Popf', [8473]], ['pound', [163]], ['prap', [10935]], ['Pr', [10939]], ['pr', [8826]], ['prcue', [8828]], ['precapprox', [10935]], ['prec', [8826]], ['preccurlyeq', [8828]], ['Precedes', [8826]], ['PrecedesEqual', [10927]], ['PrecedesSlantEqual', [8828]], ['PrecedesTilde', [8830]], ['preceq', [10927]], ['precnapprox', [10937]], ['precneqq', [10933]], ['precnsim', [8936]], ['pre', [10927]], ['prE', [10931]], ['precsim', [8830]], ['prime', [8242]], ['Prime', [8243]], ['primes', [8473]], ['prnap', [10937]], ['prnE', [10933]], ['prnsim', [8936]], ['prod', [8719]], ['Product', [8719]], ['profalar', [9006]], ['profline', [8978]], ['profsurf', [8979]], ['prop', [8733]], ['Proportional', [8733]], ['Proportion', [8759]], ['propto', [8733]], ['prsim', [8830]], ['prurel', [8880]], ['Pscr', [119979]], ['pscr', [120005]], ['Psi', [936]], ['psi', [968]], ['puncsp', [8200]], ['Qfr', [120084]], ['qfr', [120110]], ['qint', [10764]], ['qopf', [120162]], ['Qopf', [8474]], ['qprime', [8279]], ['Qscr', [119980]], ['qscr', [120006]], ['quaternions', [8461]], ['quatint', [10774]], ['quest', [63]], ['questeq', [8799]], ['quot', [34]], ['QUOT', [34]], ['rAarr', [8667]], ['race', [8765, 817]], ['Racute', [340]], ['racute', [341]], ['radic', [8730]], ['raemptyv', [10675]], ['rang', [10217]], ['Rang', [10219]], ['rangd', [10642]], ['range', [10661]], ['rangle', [10217]], ['raquo', [187]], ['rarrap', [10613]], ['rarrb', [8677]], ['rarrbfs', [10528]], ['rarrc', [10547]], ['rarr', [8594]], ['Rarr', [8608]], ['rArr', [8658]], ['rarrfs', [10526]], ['rarrhk', [8618]], ['rarrlp', [8620]], ['rarrpl', [10565]], ['rarrsim', [10612]], ['Rarrtl', [10518]], ['rarrtl', [8611]], ['rarrw', [8605]], ['ratail', [10522]], ['rAtail', [10524]], ['ratio', [8758]], ['rationals', [8474]], ['rbarr', [10509]], ['rBarr', [10511]], ['RBarr', [10512]], ['rbbrk', [10099]], ['rbrace', [125]], ['rbrack', [93]], ['rbrke', [10636]], ['rbrksld', [10638]], ['rbrkslu', [10640]], ['Rcaron', [344]], ['rcaron', [345]], ['Rcedil', [342]], ['rcedil', [343]], ['rceil', [8969]], ['rcub', [125]], ['Rcy', [1056]], ['rcy', [1088]], ['rdca', [10551]], ['rdldhar', [10601]], ['rdquo', [8221]], ['rdquor', [8221]], ['CloseCurlyDoubleQuote', [8221]], ['rdsh', [8627]], ['real', [8476]], ['realine', [8475]], ['realpart', [8476]], ['reals', [8477]], ['Re', [8476]], ['rect', [9645]], ['reg', [174]], ['REG', [174]], ['ReverseElement', [8715]], ['ReverseEquilibrium', [8651]], ['ReverseUpEquilibrium', [10607]], ['rfisht', [10621]], ['rfloor', [8971]], ['rfr', [120111]], ['Rfr', [8476]], ['rHar', [10596]], ['rhard', [8641]], ['rharu', [8640]], ['rharul', [10604]], ['Rho', [929]], ['rho', [961]], ['rhov', [1009]], ['RightAngleBracket', [10217]], ['RightArrowBar', [8677]], ['rightarrow', [8594]], ['RightArrow', [8594]], ['Rightarrow', [8658]], ['RightArrowLeftArrow', [8644]], ['rightarrowtail', [8611]], ['RightCeiling', [8969]], ['RightDoubleBracket', [10215]], ['RightDownTeeVector', [10589]], ['RightDownVectorBar', [10581]], ['RightDownVector', [8642]], ['RightFloor', [8971]], ['rightharpoondown', [8641]], ['rightharpoonup', [8640]], ['rightleftarrows', [8644]], ['rightleftharpoons', [8652]], ['rightrightarrows', [8649]], ['rightsquigarrow', [8605]], ['RightTeeArrow', [8614]], ['RightTee', [8866]], ['RightTeeVector', [10587]], ['rightthreetimes', [8908]], ['RightTriangleBar', [10704]], ['RightTriangle', [8883]], ['RightTriangleEqual', [8885]], ['RightUpDownVector', [10575]], ['RightUpTeeVector', [10588]], ['RightUpVectorBar', [10580]], ['RightUpVector', [8638]], ['RightVectorBar', [10579]], ['RightVector', [8640]], ['ring', [730]], ['risingdotseq', [8787]], ['rlarr', [8644]], ['rlhar', [8652]], ['rlm', [8207]], ['rmoustache', [9137]], ['rmoust', [9137]], ['rnmid', [10990]], ['roang', [10221]], ['roarr', [8702]], ['robrk', [10215]], ['ropar', [10630]], ['ropf', [120163]], ['Ropf', [8477]], ['roplus', [10798]], ['rotimes', [10805]], ['RoundImplies', [10608]], ['rpar', [41]], ['rpargt', [10644]], ['rppolint', [10770]], ['rrarr', [8649]], ['Rrightarrow', [8667]], ['rsaquo', [8250]], ['rscr', [120007]], ['Rscr', [8475]], ['rsh', [8625]], ['Rsh', [8625]], ['rsqb', [93]], ['rsquo', [8217]], ['rsquor', [8217]], ['CloseCurlyQuote', [8217]], ['rthree', [8908]], ['rtimes', [8906]], ['rtri', [9657]], ['rtrie', [8885]], ['rtrif', [9656]], ['rtriltri', [10702]], ['RuleDelayed', [10740]], ['ruluhar', [10600]], ['rx', [8478]], ['Sacute', [346]], ['sacute', [347]], ['sbquo', [8218]], ['scap', [10936]], ['Scaron', [352]], ['scaron', [353]], ['Sc', [10940]], ['sc', [8827]], ['sccue', [8829]], ['sce', [10928]], ['scE', [10932]], ['Scedil', [350]], ['scedil', [351]], ['Scirc', [348]], ['scirc', [349]], ['scnap', [10938]], ['scnE', [10934]], ['scnsim', [8937]], ['scpolint', [10771]], ['scsim', [8831]], ['Scy', [1057]], ['scy', [1089]], ['sdotb', [8865]], ['sdot', [8901]], ['sdote', [10854]], ['searhk', [10533]], ['searr', [8600]], ['seArr', [8664]], ['searrow', [8600]], ['sect', [167]], ['semi', [59]], ['seswar', [10537]], ['setminus', [8726]], ['setmn', [8726]], ['sext', [10038]], ['Sfr', [120086]], ['sfr', [120112]], ['sfrown', [8994]], ['sharp', [9839]], ['SHCHcy', [1065]], ['shchcy', [1097]], ['SHcy', [1064]], ['shcy', [1096]], ['ShortDownArrow', [8595]], ['ShortLeftArrow', [8592]], ['shortmid', [8739]], ['shortparallel', [8741]], ['ShortRightArrow', [8594]], ['ShortUpArrow', [8593]], ['shy', [173]], ['Sigma', [931]], ['sigma', [963]], ['sigmaf', [962]], ['sigmav', [962]], ['sim', [8764]], ['simdot', [10858]], ['sime', [8771]], ['simeq', [8771]], ['simg', [10910]], ['simgE', [10912]], ['siml', [10909]], ['simlE', [10911]], ['simne', [8774]], ['simplus', [10788]], ['simrarr', [10610]], ['slarr', [8592]], ['SmallCircle', [8728]], ['smallsetminus', [8726]], ['smashp', [10803]], ['smeparsl', [10724]], ['smid', [8739]], ['smile', [8995]], ['smt', [10922]], ['smte', [10924]], ['smtes', [10924, 65024]], ['SOFTcy', [1068]], ['softcy', [1100]], ['solbar', [9023]], ['solb', [10692]], ['sol', [47]], ['Sopf', [120138]], ['sopf', [120164]], ['spades', [9824]], ['spadesuit', [9824]], ['spar', [8741]], ['sqcap', [8851]], ['sqcaps', [8851, 65024]], ['sqcup', [8852]], ['sqcups', [8852, 65024]], ['Sqrt', [8730]], ['sqsub', [8847]], ['sqsube', [8849]], ['sqsubset', [8847]], ['sqsubseteq', [8849]], ['sqsup', [8848]], ['sqsupe', [8850]], ['sqsupset', [8848]], ['sqsupseteq', [8850]], ['square', [9633]], ['Square', [9633]], ['SquareIntersection', [8851]], ['SquareSubset', [8847]], ['SquareSubsetEqual', [8849]], ['SquareSuperset', [8848]], ['SquareSupersetEqual', [8850]], ['SquareUnion', [8852]], ['squarf', [9642]], ['squ', [9633]], ['squf', [9642]], ['srarr', [8594]], ['Sscr', [119982]], ['sscr', [120008]], ['ssetmn', [8726]], ['ssmile', [8995]], ['sstarf', [8902]], ['Star', [8902]], ['star', [9734]], ['starf', [9733]], ['straightepsilon', [1013]], ['straightphi', [981]], ['strns', [175]], ['sub', [8834]], ['Sub', [8912]], ['subdot', [10941]], ['subE', [10949]], ['sube', [8838]], ['subedot', [10947]], ['submult', [10945]], ['subnE', [10955]], ['subne', [8842]], ['subplus', [10943]], ['subrarr', [10617]], ['subset', [8834]], ['Subset', [8912]], ['subseteq', [8838]], ['subseteqq', [10949]], ['SubsetEqual', [8838]], ['subsetneq', [8842]], ['subsetneqq', [10955]], ['subsim', [10951]], ['subsub', [10965]], ['subsup', [10963]], ['succapprox', [10936]], ['succ', [8827]], ['succcurlyeq', [8829]], ['Succeeds', [8827]], ['SucceedsEqual', [10928]], ['SucceedsSlantEqual', [8829]], ['SucceedsTilde', [8831]], ['succeq', [10928]], ['succnapprox', [10938]], ['succneqq', [10934]], ['succnsim', [8937]], ['succsim', [8831]], ['SuchThat', [8715]], ['sum', [8721]], ['Sum', [8721]], ['sung', [9834]], ['sup1', [185]], ['sup2', [178]], ['sup3', [179]], ['sup', [8835]], ['Sup', [8913]], ['supdot', [10942]], ['supdsub', [10968]], ['supE', [10950]], ['supe', [8839]], ['supedot', [10948]], ['Superset', [8835]], ['SupersetEqual', [8839]], ['suphsol', [10185]], ['suphsub', [10967]], ['suplarr', [10619]], ['supmult', [10946]], ['supnE', [10956]], ['supne', [8843]], ['supplus', [10944]], ['supset', [8835]], ['Supset', [8913]], ['supseteq', [8839]], ['supseteqq', [10950]], ['supsetneq', [8843]], ['supsetneqq', [10956]], ['supsim', [10952]], ['supsub', [10964]], ['supsup', [10966]], ['swarhk', [10534]], ['swarr', [8601]], ['swArr', [8665]], ['swarrow', [8601]], ['swnwar', [10538]], ['szlig', [223]], ['Tab', [9]], ['target', [8982]], ['Tau', [932]], ['tau', [964]], ['tbrk', [9140]], ['Tcaron', [356]], ['tcaron', [357]], ['Tcedil', [354]], ['tcedil', [355]], ['Tcy', [1058]], ['tcy', [1090]], ['tdot', [8411]], ['telrec', [8981]], ['Tfr', [120087]], ['tfr', [120113]], ['there4', [8756]], ['therefore', [8756]], ['Therefore', [8756]], ['Theta', [920]], ['theta', [952]], ['thetasym', [977]], ['thetav', [977]], ['thickapprox', [8776]], ['thicksim', [8764]], ['ThickSpace', [8287, 8202]], ['ThinSpace', [8201]], ['thinsp', [8201]], ['thkap', [8776]], ['thksim', [8764]], ['THORN', [222]], ['thorn', [254]], ['tilde', [732]], ['Tilde', [8764]], ['TildeEqual', [8771]], ['TildeFullEqual', [8773]], ['TildeTilde', [8776]], ['timesbar', [10801]], ['timesb', [8864]], ['times', [215]], ['timesd', [10800]], ['tint', [8749]], ['toea', [10536]], ['topbot', [9014]], ['topcir', [10993]], ['top', [8868]], ['Topf', [120139]], ['topf', [120165]], ['topfork', [10970]], ['tosa', [10537]], ['tprime', [8244]], ['trade', [8482]], ['TRADE', [8482]], ['triangle', [9653]], ['triangledown', [9663]], ['triangleleft', [9667]], ['trianglelefteq', [8884]], ['triangleq', [8796]], ['triangleright', [9657]], ['trianglerighteq', [8885]], ['tridot', [9708]], ['trie', [8796]], ['triminus', [10810]], ['TripleDot', [8411]], ['triplus', [10809]], ['trisb', [10701]], ['tritime', [10811]], ['trpezium', [9186]], ['Tscr', [119983]], ['tscr', [120009]], ['TScy', [1062]], ['tscy', [1094]], ['TSHcy', [1035]], ['tshcy', [1115]], ['Tstrok', [358]], ['tstrok', [359]], ['twixt', [8812]], ['twoheadleftarrow', [8606]], ['twoheadrightarrow', [8608]], ['Uacute', [218]], ['uacute', [250]], ['uarr', [8593]], ['Uarr', [8607]], ['uArr', [8657]], ['Uarrocir', [10569]], ['Ubrcy', [1038]], ['ubrcy', [1118]], ['Ubreve', [364]], ['ubreve', [365]], ['Ucirc', [219]], ['ucirc', [251]], ['Ucy', [1059]], ['ucy', [1091]], ['udarr', [8645]], ['Udblac', [368]], ['udblac', [369]], ['udhar', [10606]], ['ufisht', [10622]], ['Ufr', [120088]], ['ufr', [120114]], ['Ugrave', [217]], ['ugrave', [249]], ['uHar', [10595]], ['uharl', [8639]], ['uharr', [8638]], ['uhblk', [9600]], ['ulcorn', [8988]], ['ulcorner', [8988]], ['ulcrop', [8975]], ['ultri', [9720]], ['Umacr', [362]], ['umacr', [363]], ['uml', [168]], ['UnderBar', [95]], ['UnderBrace', [9183]], ['UnderBracket', [9141]], ['UnderParenthesis', [9181]], ['Union', [8899]], ['UnionPlus', [8846]], ['Uogon', [370]], ['uogon', [371]], ['Uopf', [120140]], ['uopf', [120166]], ['UpArrowBar', [10514]], ['uparrow', [8593]], ['UpArrow', [8593]], ['Uparrow', [8657]], ['UpArrowDownArrow', [8645]], ['updownarrow', [8597]], ['UpDownArrow', [8597]], ['Updownarrow', [8661]], ['UpEquilibrium', [10606]], ['upharpoonleft', [8639]], ['upharpoonright', [8638]], ['uplus', [8846]], ['UpperLeftArrow', [8598]], ['UpperRightArrow', [8599]], ['upsi', [965]], ['Upsi', [978]], ['upsih', [978]], ['Upsilon', [933]], ['upsilon', [965]], ['UpTeeArrow', [8613]], ['UpTee', [8869]], ['upuparrows', [8648]], ['urcorn', [8989]], ['urcorner', [8989]], ['urcrop', [8974]], ['Uring', [366]], ['uring', [367]], ['urtri', [9721]], ['Uscr', [119984]], ['uscr', [120010]], ['utdot', [8944]], ['Utilde', [360]], ['utilde', [361]], ['utri', [9653]], ['utrif', [9652]], ['uuarr', [8648]], ['Uuml', [220]], ['uuml', [252]], ['uwangle', [10663]], ['vangrt', [10652]], ['varepsilon', [1013]], ['varkappa', [1008]], ['varnothing', [8709]], ['varphi', [981]], ['varpi', [982]], ['varpropto', [8733]], ['varr', [8597]], ['vArr', [8661]], ['varrho', [1009]], ['varsigma', [962]], ['varsubsetneq', [8842, 65024]], ['varsubsetneqq', [10955, 65024]], ['varsupsetneq', [8843, 65024]], ['varsupsetneqq', [10956, 65024]], ['vartheta', [977]], ['vartriangleleft', [8882]], ['vartriangleright', [8883]], ['vBar', [10984]], ['Vbar', [10987]], ['vBarv', [10985]], ['Vcy', [1042]], ['vcy', [1074]], ['vdash', [8866]], ['vDash', [8872]], ['Vdash', [8873]], ['VDash', [8875]], ['Vdashl', [10982]], ['veebar', [8891]], ['vee', [8744]], ['Vee', [8897]], ['veeeq', [8794]], ['vellip', [8942]], ['verbar', [124]], ['Verbar', [8214]], ['vert', [124]], ['Vert', [8214]], ['VerticalBar', [8739]], ['VerticalLine', [124]], ['VerticalSeparator', [10072]], ['VerticalTilde', [8768]], ['VeryThinSpace', [8202]], ['Vfr', [120089]], ['vfr', [120115]], ['vltri', [8882]], ['vnsub', [8834, 8402]], ['vnsup', [8835, 8402]], ['Vopf', [120141]], ['vopf', [120167]], ['vprop', [8733]], ['vrtri', [8883]], ['Vscr', [119985]], ['vscr', [120011]], ['vsubnE', [10955, 65024]], ['vsubne', [8842, 65024]], ['vsupnE', [10956, 65024]], ['vsupne', [8843, 65024]], ['Vvdash', [8874]], ['vzigzag', [10650]], ['Wcirc', [372]], ['wcirc', [373]], ['wedbar', [10847]], ['wedge', [8743]], ['Wedge', [8896]], ['wedgeq', [8793]], ['weierp', [8472]], ['Wfr', [120090]], ['wfr', [120116]], ['Wopf', [120142]], ['wopf', [120168]], ['wp', [8472]], ['wr', [8768]], ['wreath', [8768]], ['Wscr', [119986]], ['wscr', [120012]], ['xcap', [8898]], ['xcirc', [9711]], ['xcup', [8899]], ['xdtri', [9661]], ['Xfr', [120091]], ['xfr', [120117]], ['xharr', [10231]], ['xhArr', [10234]], ['Xi', [926]], ['xi', [958]], ['xlarr', [10229]], ['xlArr', [10232]], ['xmap', [10236]], ['xnis', [8955]], ['xodot', [10752]], ['Xopf', [120143]], ['xopf', [120169]], ['xoplus', [10753]], ['xotime', [10754]], ['xrarr', [10230]], ['xrArr', [10233]], ['Xscr', [119987]], ['xscr', [120013]], ['xsqcup', [10758]], ['xuplus', [10756]], ['xutri', [9651]], ['xvee', [8897]], ['xwedge', [8896]], ['Yacute', [221]], ['yacute', [253]], ['YAcy', [1071]], ['yacy', [1103]], ['Ycirc', [374]], ['ycirc', [375]], ['Ycy', [1067]], ['ycy', [1099]], ['yen', [165]], ['Yfr', [120092]], ['yfr', [120118]], ['YIcy', [1031]], ['yicy', [1111]], ['Yopf', [120144]], ['yopf', [120170]], ['Yscr', [119988]], ['yscr', [120014]], ['YUcy', [1070]], ['yucy', [1102]], ['yuml', [255]], ['Yuml', [376]], ['Zacute', [377]], ['zacute', [378]], ['Zcaron', [381]], ['zcaron', [382]], ['Zcy', [1047]], ['zcy', [1079]], ['Zdot', [379]], ['zdot', [380]], ['zeetrf', [8488]], ['ZeroWidthSpace', [8203]], ['Zeta', [918]], ['zeta', [950]], ['zfr', [120119]], ['Zfr', [8488]], ['ZHcy', [1046]], ['zhcy', [1078]], ['zigrarr', [8669]], ['zopf', [120171]], ['Zopf', [8484]], ['Zscr', [119989]], ['zscr', [120015]], ['zwj', [8205]], ['zwnj', [8204]]];

var alphaIndex = {};
var charIndex = {};

createIndexes(alphaIndex, charIndex);

/**
 * @constructor
 */
function Html5Entities() {}

/**
 * @param {String} str
 * @returns {String}
 */
Html5Entities.prototype.decode = function(str) {
    if (!str || !str.length) {
        return '';
    }
    return str.replace(/&(#?[\w\d]+);?/g, function(s, entity) {
        var chr;
        if (entity.charAt(0) === "#") {
            var code = entity.charAt(1) === 'x' ?
                parseInt(entity.substr(2).toLowerCase(), 16) :
                parseInt(entity.substr(1));

            if (!(isNaN(code) || code < -32768 || code > 65535)) {
                chr = String.fromCharCode(code);
            }
        } else {
            chr = alphaIndex[entity];
        }
        return chr || s;
    });
};

/**
 * @param {String} str
 * @returns {String}
 */
 Html5Entities.decode = function(str) {
    return new Html5Entities().decode(str);
 };

/**
 * @param {String} str
 * @returns {String}
 */
Html5Entities.prototype.encode = function(str) {
    if (!str || !str.length) {
        return '';
    }
    var strLength = str.length;
    var result = '';
    var i = 0;
    while (i < strLength) {
        var charInfo = charIndex[str.charCodeAt(i)];
        if (charInfo) {
            var alpha = charInfo[str.charCodeAt(i + 1)];
            if (alpha) {
                i++;
            } else {
                alpha = charInfo[''];
            }
            if (alpha) {
                result += "&" + alpha + ";";
                i++;
                continue;
            }
        }
        result += str.charAt(i);
        i++;
    }
    return result;
};

/**
 * @param {String} str
 * @returns {String}
 */
 Html5Entities.encode = function(str) {
    return new Html5Entities().encode(str);
 };

/**
 * @param {String} str
 * @returns {String}
 */
Html5Entities.prototype.encodeNonUTF = function(str) {
    if (!str || !str.length) {
        return '';
    }
    var strLength = str.length;
    var result = '';
    var i = 0;
    while (i < strLength) {
        var c = str.charCodeAt(i);
        var charInfo = charIndex[c];
        if (charInfo) {
            var alpha = charInfo[str.charCodeAt(i + 1)];
            if (alpha) {
                i++;
            } else {
                alpha = charInfo[''];
            }
            if (alpha) {
                result += "&" + alpha + ";";
                i++;
                continue;
            }
        }
        if (c < 32 || c > 126) {
            result += '&#' + c + ';';
        } else {
            result += str.charAt(i);
        }
        i++;
    }
    return result;
};

/**
 * @param {String} str
 * @returns {String}
 */
 Html5Entities.encodeNonUTF = function(str) {
    return new Html5Entities().encodeNonUTF(str);
 };

/**
 * @param {String} str
 * @returns {String}
 */
Html5Entities.prototype.encodeNonASCII = function(str) {
    if (!str || !str.length) {
        return '';
    }
    var strLength = str.length;
    var result = '';
    var i = 0;
    while (i < strLength) {
        var c = str.charCodeAt(i);
        if (c <= 255) {
            result += str[i++];
            continue;
        }
        result += '&#' + c + ';';
        i++
    }
    return result;
};

/**
 * @param {String} str
 * @returns {String}
 */
 Html5Entities.encodeNonASCII = function(str) {
    return new Html5Entities().encodeNonASCII(str);
 };

/**
 * @param {Object} alphaIndex Passed by reference.
 * @param {Object} charIndex Passed by reference.
 */
function createIndexes(alphaIndex, charIndex) {
    var i = ENTITIES.length;
    var _results = [];
    while (i--) {
        var e = ENTITIES[i];
        var alpha = e[0];
        var chars = e[1];
        var chr = chars[0];
        var addChar = (chr < 32 || chr > 126) || chr === 62 || chr === 60 || chr === 38 || chr === 34 || chr === 39;
        var charInfo;
        if (addChar) {
            charInfo = charIndex[chr] = charIndex[chr] || {};
        }
        if (chars[1]) {
            var chr2 = chars[1];
            alphaIndex[alpha] = String.fromCharCode(chr) + String.fromCharCode(chr2);
            _results.push(addChar && (charInfo[chr2] = alpha));
        } else {
            alphaIndex[alpha] = String.fromCharCode(chr);
            _results.push(addChar && (charInfo[''] = alpha));
        }
    }
}

module.exports = Html5Entities;


/***/ }),
/* 1 */
/***/ (function(module, exports, __webpack_require__) {

__webpack_require__(2);
module.exports = __webpack_require__(15);


/***/ }),
/* 2 */
/***/ (function(module, exports, __webpack_require__) {

/* WEBPACK VAR INJECTION */(function(__resourceQuery, module) {/*eslint-env browser*/
/*global __resourceQuery __webpack_public_path__*/

var options = {
  path: "/__webpack_hmr",
  timeout: 20 * 1000,
  overlay: true,
  reload: false,
  log: true,
  warn: true,
  name: '',
  autoConnect: true,
  overlayStyles: {},
  overlayWarnings: false,
  ansiColors: {}
};
if (true) {
  var querystring = __webpack_require__(4);
  var overrides = querystring.parse(__resourceQuery.slice(1));
  setOverrides(overrides);
}

if (typeof window === 'undefined') {
  // do nothing
} else if (typeof window.EventSource === 'undefined') {
  console.warn(
    "webpack-hot-middleware's client requires EventSource to work. " +
    "You should include a polyfill if you want to support this browser: " +
    "https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events#Tools"
  );
} else {
  if (options.autoConnect) {
    connect();
  }
}

/* istanbul ignore next */
function setOptionsAndConnect(overrides) {
  setOverrides(overrides);
  connect();
}

function setOverrides(overrides) {
  if (overrides.autoConnect) options.autoConnect = overrides.autoConnect == 'true';
  if (overrides.path) options.path = overrides.path;
  if (overrides.timeout) options.timeout = overrides.timeout;
  if (overrides.overlay) options.overlay = overrides.overlay !== 'false';
  if (overrides.reload) options.reload = overrides.reload !== 'false';
  if (overrides.noInfo && overrides.noInfo !== 'false') {
    options.log = false;
  }
  if (overrides.name) {
    options.name = overrides.name;
  }
  if (overrides.quiet && overrides.quiet !== 'false') {
    options.log = false;
    options.warn = false;
  }

  if (overrides.dynamicPublicPath) {
    options.path = __webpack_require__.p + options.path;
  }

  if (overrides.ansiColors) options.ansiColors = JSON.parse(overrides.ansiColors);
  if (overrides.overlayStyles) options.overlayStyles = JSON.parse(overrides.overlayStyles);

  if (overrides.overlayWarnings) {
    options.overlayWarnings = overrides.overlayWarnings == 'true';
  }
}

function EventSourceWrapper() {
  var source;
  var lastActivity = new Date();
  var listeners = [];

  init();
  var timer = setInterval(function() {
    if ((new Date() - lastActivity) > options.timeout) {
      handleDisconnect();
    }
  }, options.timeout / 2);

  function init() {
    source = new window.EventSource(options.path);
    source.onopen = handleOnline;
    source.onerror = handleDisconnect;
    source.onmessage = handleMessage;
  }

  function handleOnline() {
    if (options.log) console.log("[HMR] connected");
    lastActivity = new Date();
  }

  function handleMessage(event) {
    lastActivity = new Date();
    for (var i = 0; i < listeners.length; i++) {
      listeners[i](event);
    }
  }

  function handleDisconnect() {
    clearInterval(timer);
    source.close();
    setTimeout(init, options.timeout);
  }

  return {
    addMessageListener: function(fn) {
      listeners.push(fn);
    }
  };
}

function getEventSourceWrapper() {
  if (!window.__whmEventSourceWrapper) {
    window.__whmEventSourceWrapper = {};
  }
  if (!window.__whmEventSourceWrapper[options.path]) {
    // cache the wrapper for other entries loaded on
    // the same page with the same options.path
    window.__whmEventSourceWrapper[options.path] = EventSourceWrapper();
  }
  return window.__whmEventSourceWrapper[options.path];
}

function connect() {
  getEventSourceWrapper().addMessageListener(handleMessage);

  function handleMessage(event) {
    if (event.data == "\uD83D\uDC93") {
      return;
    }
    try {
      processMessage(JSON.parse(event.data));
    } catch (ex) {
      if (options.warn) {
        console.warn("Invalid HMR message: " + event.data + "\n" + ex);
      }
    }
  }
}

// the reporter needs to be a singleton on the page
// in case the client is being used by multiple bundles
// we only want to report once.
// all the errors will go to all clients
var singletonKey = '__webpack_hot_middleware_reporter__';
var reporter;
if (typeof window !== 'undefined') {
  if (!window[singletonKey]) {
    window[singletonKey] = createReporter();
  }
  reporter = window[singletonKey];
}

function createReporter() {
  var strip = __webpack_require__(7);

  var overlay;
  if (typeof document !== 'undefined' && options.overlay) {
    overlay = __webpack_require__(9)({
      ansiColors: options.ansiColors,
      overlayStyles: options.overlayStyles
    });
  }

  var styles = {
    errors: "color: #ff0000;",
    warnings: "color: #999933;"
  };
  var previousProblems = null;
  function log(type, obj) {
    var newProblems = obj[type].map(function(msg) { return strip(msg); }).join('\n');
    if (previousProblems == newProblems) {
      return;
    } else {
      previousProblems = newProblems;
    }

    var style = styles[type];
    var name = obj.name ? "'" + obj.name + "' " : "";
    var title = "[HMR] bundle " + name + "has " + obj[type].length + " " + type;
    // NOTE: console.warn or console.error will print the stack trace
    // which isn't helpful here, so using console.log to escape it.
    if (console.group && console.groupEnd) {
      console.group("%c" + title, style);
      console.log("%c" + newProblems, style);
      console.groupEnd();
    } else {
      console.log(
        "%c" + title + "\n\t%c" + newProblems.replace(/\n/g, "\n\t"),
        style + "font-weight: bold;",
        style + "font-weight: normal;"
      );
    }
  }

  return {
    cleanProblemsCache: function () {
      previousProblems = null;
    },
    problems: function(type, obj) {
      if (options.warn) {
        log(type, obj);
      }
      if (overlay) {
        if (options.overlayWarnings || type === 'errors') {
          overlay.showProblems(type, obj[type]);
          return false;
        }
        overlay.clear();
      }
      return true;
    },
    success: function() {
      if (overlay) overlay.clear();
    },
    useCustomOverlay: function(customOverlay) {
      overlay = customOverlay;
    }
  };
}

var processUpdate = __webpack_require__(14);

var customHandler;
var subscribeAllHandler;
function processMessage(obj) {
  switch(obj.action) {
    case "building":
      if (options.log) {
        console.log(
          "[HMR] bundle " + (obj.name ? "'" + obj.name + "' " : "") +
          "rebuilding"
        );
      }
      break;
    case "built":
      if (options.log) {
        console.log(
          "[HMR] bundle " + (obj.name ? "'" + obj.name + "' " : "") +
          "rebuilt in " + obj.time + "ms"
        );
      }
      // fall through
    case "sync":
      if (obj.name && options.name && obj.name !== options.name) {
        return;
      }
      var applyUpdate = true;
      if (obj.errors.length > 0) {
        if (reporter) reporter.problems('errors', obj);
        applyUpdate = false;
      } else if (obj.warnings.length > 0) {
        if (reporter) {
          var overlayShown = reporter.problems('warnings', obj);
          applyUpdate = overlayShown;
        }
      } else {
        if (reporter) {
          reporter.cleanProblemsCache();
          reporter.success();
        }
      }
      if (applyUpdate) {
        processUpdate(obj.hash, obj.modules, options);
      }
      break;
    default:
      if (customHandler) {
        customHandler(obj);
      }
  }

  if (subscribeAllHandler) {
    subscribeAllHandler(obj);
  }
}

if (module) {
  module.exports = {
    subscribeAll: function subscribeAll(handler) {
      subscribeAllHandler = handler;
    },
    subscribe: function subscribe(handler) {
      customHandler = handler;
    },
    useCustomOverlay: function useCustomOverlay(customOverlay) {
      if (reporter) reporter.useCustomOverlay(customOverlay);
    },
    setOptionsAndConnect: setOptionsAndConnect
  };
}

/* WEBPACK VAR INJECTION */}.call(exports, "?path=__webpack_hmr&dynamicPublicPath=true", __webpack_require__(3)(module)))

/***/ }),
/* 3 */
/***/ (function(module, exports) {

module.exports = function(module) {
	if(!module.webpackPolyfill) {
		module.deprecate = function() {};
		module.paths = [];
		// module.parent = undefined by default
		if(!module.children) module.children = [];
		Object.defineProperty(module, "loaded", {
			enumerable: true,
			get: function() {
				return module.l;
			}
		});
		Object.defineProperty(module, "id", {
			enumerable: true,
			get: function() {
				return module.i;
			}
		});
		module.webpackPolyfill = 1;
	}
	return module;
};


/***/ }),
/* 4 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


exports.decode = exports.parse = __webpack_require__(5);
exports.encode = exports.stringify = __webpack_require__(6);


/***/ }),
/* 5 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
// Copyright Joyent, Inc. and other Node contributors.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.



// If obj.hasOwnProperty has been overridden, then calling
// obj.hasOwnProperty(prop) will break.
// See: https://github.com/joyent/node/issues/1707
function hasOwnProperty(obj, prop) {
  return Object.prototype.hasOwnProperty.call(obj, prop);
}

module.exports = function(qs, sep, eq, options) {
  sep = sep || '&';
  eq = eq || '=';
  var obj = {};

  if (typeof qs !== 'string' || qs.length === 0) {
    return obj;
  }

  var regexp = /\+/g;
  qs = qs.split(sep);

  var maxKeys = 1000;
  if (options && typeof options.maxKeys === 'number') {
    maxKeys = options.maxKeys;
  }

  var len = qs.length;
  // maxKeys <= 0 means that we should not limit keys count
  if (maxKeys > 0 && len > maxKeys) {
    len = maxKeys;
  }

  for (var i = 0; i < len; ++i) {
    var x = qs[i].replace(regexp, '%20'),
        idx = x.indexOf(eq),
        kstr, vstr, k, v;

    if (idx >= 0) {
      kstr = x.substr(0, idx);
      vstr = x.substr(idx + 1);
    } else {
      kstr = x;
      vstr = '';
    }

    k = decodeURIComponent(kstr);
    v = decodeURIComponent(vstr);

    if (!hasOwnProperty(obj, k)) {
      obj[k] = v;
    } else if (isArray(obj[k])) {
      obj[k].push(v);
    } else {
      obj[k] = [obj[k], v];
    }
  }

  return obj;
};

var isArray = Array.isArray || function (xs) {
  return Object.prototype.toString.call(xs) === '[object Array]';
};


/***/ }),
/* 6 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
// Copyright Joyent, Inc. and other Node contributors.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.



var stringifyPrimitive = function(v) {
  switch (typeof v) {
    case 'string':
      return v;

    case 'boolean':
      return v ? 'true' : 'false';

    case 'number':
      return isFinite(v) ? v : '';

    default:
      return '';
  }
};

module.exports = function(obj, sep, eq, name) {
  sep = sep || '&';
  eq = eq || '=';
  if (obj === null) {
    obj = undefined;
  }

  if (typeof obj === 'object') {
    return map(objectKeys(obj), function(k) {
      var ks = encodeURIComponent(stringifyPrimitive(k)) + eq;
      if (isArray(obj[k])) {
        return map(obj[k], function(v) {
          return ks + encodeURIComponent(stringifyPrimitive(v));
        }).join(sep);
      } else {
        return ks + encodeURIComponent(stringifyPrimitive(obj[k]));
      }
    }).join(sep);

  }

  if (!name) return '';
  return encodeURIComponent(stringifyPrimitive(name)) + eq +
         encodeURIComponent(stringifyPrimitive(obj));
};

var isArray = Array.isArray || function (xs) {
  return Object.prototype.toString.call(xs) === '[object Array]';
};

function map (xs, f) {
  if (xs.map) return xs.map(f);
  var res = [];
  for (var i = 0; i < xs.length; i++) {
    res.push(f(xs[i], i));
  }
  return res;
}

var objectKeys = Object.keys || function (obj) {
  var res = [];
  for (var key in obj) {
    if (Object.prototype.hasOwnProperty.call(obj, key)) res.push(key);
  }
  return res;
};


/***/ }),
/* 7 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var ansiRegex = __webpack_require__(8)();

module.exports = function (str) {
	return typeof str === 'string' ? str.replace(ansiRegex, '') : str;
};


/***/ }),
/* 8 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

module.exports = function () {
	return /[\u001b\u009b][[()#;?]*(?:[0-9]{1,4}(?:;[0-9]{0,4})*)?[0-9A-PRZcf-nqry=><]/g;
};


/***/ }),
/* 9 */
/***/ (function(module, exports, __webpack_require__) {

/*eslint-env browser*/

var clientOverlay = document.createElement('div');
clientOverlay.id = 'webpack-hot-middleware-clientOverlay';
var styles = {
  background: 'rgba(0,0,0,0.85)',
  color: '#E8E8E8',
  lineHeight: '1.2',
  whiteSpace: 'pre',
  fontFamily: 'Menlo, Consolas, monospace',
  fontSize: '13px',
  position: 'fixed',
  zIndex: 9999,
  padding: '10px',
  left: 0,
  right: 0,
  top: 0,
  bottom: 0,
  overflow: 'auto',
  dir: 'ltr',
  textAlign: 'left'
};

var ansiHTML = __webpack_require__(10);
var colors = {
  reset: ['transparent', 'transparent'],
  black: '181818',
  red: 'E36049',
  green: 'B3CB74',
  yellow: 'FFD080',
  blue: '7CAFC2',
  magenta: '7FACCA',
  cyan: 'C3C2EF',
  lightgrey: 'EBE7E3',
  darkgrey: '6D7891'
};

var Entities = __webpack_require__(11).AllHtmlEntities;
var entities = new Entities();

function showProblems(type, lines) {
  clientOverlay.innerHTML = '';
  lines.forEach(function(msg) {
    msg = ansiHTML(entities.encode(msg));
    var div = document.createElement('div');
    div.style.marginBottom = '26px';
    div.innerHTML = problemType(type) + ' in ' + msg;
    clientOverlay.appendChild(div);
  });
  if (document.body) {
    document.body.appendChild(clientOverlay);
  }
}

function clear() {
  if (document.body && clientOverlay.parentNode) {
    document.body.removeChild(clientOverlay);
  }
}

function problemType (type) {
  var problemColors = {
    errors: colors.red,
    warnings: colors.yellow
  };
  var color = problemColors[type] || colors.red;
  return (
    '<span style="background-color:#' + color + '; color:#fff; padding:2px 4px; border-radius: 2px">' +
      type.slice(0, -1).toUpperCase() +
    '</span>'
  );
}

module.exports = function(options) {
  for (var color in options.overlayColors) {
    if (color in colors) {
      colors[color] = options.overlayColors[color];
    }
    ansiHTML.setColors(colors);
  }

  for (var style in options.overlayStyles) {
    styles[style] = options.overlayStyles[style];
  }

  for (var key in styles) {
    clientOverlay.style[key] = styles[key];
  }

  return {
    showProblems: showProblems,
    clear: clear
  }
};

module.exports.clear = clear;
module.exports.showProblems = showProblems;


/***/ }),
/* 10 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


module.exports = ansiHTML

// Reference to https://github.com/sindresorhus/ansi-regex
var _regANSI = /(?:(?:\u001b\[)|\u009b)(?:(?:[0-9]{1,3})?(?:(?:;[0-9]{0,3})*)?[A-M|f-m])|\u001b[A-M]/

var _defColors = {
  reset: ['fff', '000'], // [FOREGROUD_COLOR, BACKGROUND_COLOR]
  black: '000',
  red: 'ff0000',
  green: '209805',
  yellow: 'e8bf03',
  blue: '0000ff',
  magenta: 'ff00ff',
  cyan: '00ffee',
  lightgrey: 'f0f0f0',
  darkgrey: '888'
}
var _styles = {
  30: 'black',
  31: 'red',
  32: 'green',
  33: 'yellow',
  34: 'blue',
  35: 'magenta',
  36: 'cyan',
  37: 'lightgrey'
}
var _openTags = {
  '1': 'font-weight:bold', // bold
  '2': 'opacity:0.5', // dim
  '3': '<i>', // italic
  '4': '<u>', // underscore
  '8': 'display:none', // hidden
  '9': '<del>' // delete
}
var _closeTags = {
  '23': '</i>', // reset italic
  '24': '</u>', // reset underscore
  '29': '</del>' // reset delete
}

;[0, 21, 22, 27, 28, 39, 49].forEach(function (n) {
  _closeTags[n] = '</span>'
})

/**
 * Converts text with ANSI color codes to HTML markup.
 * @param {String} text
 * @returns {*}
 */
function ansiHTML (text) {
  // Returns the text if the string has no ANSI escape code.
  if (!_regANSI.test(text)) {
    return text
  }

  // Cache opened sequence.
  var ansiCodes = []
  // Replace with markup.
  var ret = text.replace(/\033\[(\d+)*m/g, function (match, seq) {
    var ot = _openTags[seq]
    if (ot) {
      // If current sequence has been opened, close it.
      if (!!~ansiCodes.indexOf(seq)) { // eslint-disable-line no-extra-boolean-cast
        ansiCodes.pop()
        return '</span>'
      }
      // Open tag.
      ansiCodes.push(seq)
      return ot[0] === '<' ? ot : '<span style="' + ot + ';">'
    }

    var ct = _closeTags[seq]
    if (ct) {
      // Pop sequence
      ansiCodes.pop()
      return ct
    }
    return ''
  })

  // Make sure tags are closed.
  var l = ansiCodes.length
  ;(l > 0) && (ret += Array(l + 1).join('</span>'))

  return ret
}

/**
 * Customize colors.
 * @param {Object} colors reference to _defColors
 */
ansiHTML.setColors = function (colors) {
  if (typeof colors !== 'object') {
    throw new Error('`colors` parameter must be an Object.')
  }

  var _finalColors = {}
  for (var key in _defColors) {
    var hex = colors.hasOwnProperty(key) ? colors[key] : null
    if (!hex) {
      _finalColors[key] = _defColors[key]
      continue
    }
    if ('reset' === key) {
      if (typeof hex === 'string') {
        hex = [hex]
      }
      if (!Array.isArray(hex) || hex.length === 0 || hex.some(function (h) {
        return typeof h !== 'string'
      })) {
        throw new Error('The value of `' + key + '` property must be an Array and each item could only be a hex string, e.g.: FF0000')
      }
      var defHexColor = _defColors[key]
      if (!hex[0]) {
        hex[0] = defHexColor[0]
      }
      if (hex.length === 1 || !hex[1]) {
        hex = [hex[0]]
        hex.push(defHexColor[1])
      }

      hex = hex.slice(0, 2)
    } else if (typeof hex !== 'string') {
      throw new Error('The value of `' + key + '` property must be a hex string, e.g.: FF0000')
    }
    _finalColors[key] = hex
  }
  _setTags(_finalColors)
}

/**
 * Reset colors.
 */
ansiHTML.reset = function () {
  _setTags(_defColors)
}

/**
 * Expose tags, including open and close.
 * @type {Object}
 */
ansiHTML.tags = {}

if (Object.defineProperty) {
  Object.defineProperty(ansiHTML.tags, 'open', {
    get: function () { return _openTags }
  })
  Object.defineProperty(ansiHTML.tags, 'close', {
    get: function () { return _closeTags }
  })
} else {
  ansiHTML.tags.open = _openTags
  ansiHTML.tags.close = _closeTags
}

function _setTags (colors) {
  // reset all
  _openTags['0'] = 'font-weight:normal;opacity:1;color:#' + colors.reset[0] + ';background:#' + colors.reset[1]
  // inverse
  _openTags['7'] = 'color:#' + colors.reset[1] + ';background:#' + colors.reset[0]
  // dark grey
  _openTags['90'] = 'color:#' + colors.darkgrey

  for (var code in _styles) {
    var color = _styles[code]
    var oriColor = colors[color] || '000'
    _openTags[code] = 'color:#' + oriColor
    code = parseInt(code)
    _openTags[(code + 10).toString()] = 'background:#' + oriColor
  }
}

ansiHTML.reset()


/***/ }),
/* 11 */
/***/ (function(module, exports, __webpack_require__) {

module.exports = {
  XmlEntities: __webpack_require__(12),
  Html4Entities: __webpack_require__(13),
  Html5Entities: __webpack_require__(0),
  AllHtmlEntities: __webpack_require__(0)
};


/***/ }),
/* 12 */
/***/ (function(module, exports) {

var ALPHA_INDEX = {
    '&lt': '<',
    '&gt': '>',
    '&quot': '"',
    '&apos': '\'',
    '&amp': '&',
    '&lt;': '<',
    '&gt;': '>',
    '&quot;': '"',
    '&apos;': '\'',
    '&amp;': '&'
};

var CHAR_INDEX = {
    60: 'lt',
    62: 'gt',
    34: 'quot',
    39: 'apos',
    38: 'amp'
};

var CHAR_S_INDEX = {
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    '\'': '&apos;',
    '&': '&amp;'
};

/**
 * @constructor
 */
function XmlEntities() {}

/**
 * @param {String} str
 * @returns {String}
 */
XmlEntities.prototype.encode = function(str) {
    if (!str || !str.length) {
        return '';
    }
    return str.replace(/<|>|"|'|&/g, function(s) {
        return CHAR_S_INDEX[s];
    });
};

/**
 * @param {String} str
 * @returns {String}
 */
 XmlEntities.encode = function(str) {
    return new XmlEntities().encode(str);
 };

/**
 * @param {String} str
 * @returns {String}
 */
XmlEntities.prototype.decode = function(str) {
    if (!str || !str.length) {
        return '';
    }
    return str.replace(/&#?[0-9a-zA-Z]+;?/g, function(s) {
        if (s.charAt(1) === '#') {
            var code = s.charAt(2).toLowerCase() === 'x' ?
                parseInt(s.substr(3), 16) :
                parseInt(s.substr(2));

            if (isNaN(code) || code < -32768 || code > 65535) {
                return '';
            }
            return String.fromCharCode(code);
        }
        return ALPHA_INDEX[s] || s;
    });
};

/**
 * @param {String} str
 * @returns {String}
 */
 XmlEntities.decode = function(str) {
    return new XmlEntities().decode(str);
 };

/**
 * @param {String} str
 * @returns {String}
 */
XmlEntities.prototype.encodeNonUTF = function(str) {
    if (!str || !str.length) {
        return '';
    }
    var strLength = str.length;
    var result = '';
    var i = 0;
    while (i < strLength) {
        var c = str.charCodeAt(i);
        var alpha = CHAR_INDEX[c];
        if (alpha) {
            result += "&" + alpha + ";";
            i++;
            continue;
        }
        if (c < 32 || c > 126) {
            result += '&#' + c + ';';
        } else {
            result += str.charAt(i);
        }
        i++;
    }
    return result;
};

/**
 * @param {String} str
 * @returns {String}
 */
 XmlEntities.encodeNonUTF = function(str) {
    return new XmlEntities().encodeNonUTF(str);
 };

/**
 * @param {String} str
 * @returns {String}
 */
XmlEntities.prototype.encodeNonASCII = function(str) {
    if (!str || !str.length) {
        return '';
    }
    var strLenght = str.length;
    var result = '';
    var i = 0;
    while (i < strLenght) {
        var c = str.charCodeAt(i);
        if (c <= 255) {
            result += str[i++];
            continue;
        }
        result += '&#' + c + ';';
        i++;
    }
    return result;
};

/**
 * @param {String} str
 * @returns {String}
 */
 XmlEntities.encodeNonASCII = function(str) {
    return new XmlEntities().encodeNonASCII(str);
 };

module.exports = XmlEntities;


/***/ }),
/* 13 */
/***/ (function(module, exports) {

var HTML_ALPHA = ['apos', 'nbsp', 'iexcl', 'cent', 'pound', 'curren', 'yen', 'brvbar', 'sect', 'uml', 'copy', 'ordf', 'laquo', 'not', 'shy', 'reg', 'macr', 'deg', 'plusmn', 'sup2', 'sup3', 'acute', 'micro', 'para', 'middot', 'cedil', 'sup1', 'ordm', 'raquo', 'frac14', 'frac12', 'frac34', 'iquest', 'Agrave', 'Aacute', 'Acirc', 'Atilde', 'Auml', 'Aring', 'Aelig', 'Ccedil', 'Egrave', 'Eacute', 'Ecirc', 'Euml', 'Igrave', 'Iacute', 'Icirc', 'Iuml', 'ETH', 'Ntilde', 'Ograve', 'Oacute', 'Ocirc', 'Otilde', 'Ouml', 'times', 'Oslash', 'Ugrave', 'Uacute', 'Ucirc', 'Uuml', 'Yacute', 'THORN', 'szlig', 'agrave', 'aacute', 'acirc', 'atilde', 'auml', 'aring', 'aelig', 'ccedil', 'egrave', 'eacute', 'ecirc', 'euml', 'igrave', 'iacute', 'icirc', 'iuml', 'eth', 'ntilde', 'ograve', 'oacute', 'ocirc', 'otilde', 'ouml', 'divide', 'oslash', 'ugrave', 'uacute', 'ucirc', 'uuml', 'yacute', 'thorn', 'yuml', 'quot', 'amp', 'lt', 'gt', 'OElig', 'oelig', 'Scaron', 'scaron', 'Yuml', 'circ', 'tilde', 'ensp', 'emsp', 'thinsp', 'zwnj', 'zwj', 'lrm', 'rlm', 'ndash', 'mdash', 'lsquo', 'rsquo', 'sbquo', 'ldquo', 'rdquo', 'bdquo', 'dagger', 'Dagger', 'permil', 'lsaquo', 'rsaquo', 'euro', 'fnof', 'Alpha', 'Beta', 'Gamma', 'Delta', 'Epsilon', 'Zeta', 'Eta', 'Theta', 'Iota', 'Kappa', 'Lambda', 'Mu', 'Nu', 'Xi', 'Omicron', 'Pi', 'Rho', 'Sigma', 'Tau', 'Upsilon', 'Phi', 'Chi', 'Psi', 'Omega', 'alpha', 'beta', 'gamma', 'delta', 'epsilon', 'zeta', 'eta', 'theta', 'iota', 'kappa', 'lambda', 'mu', 'nu', 'xi', 'omicron', 'pi', 'rho', 'sigmaf', 'sigma', 'tau', 'upsilon', 'phi', 'chi', 'psi', 'omega', 'thetasym', 'upsih', 'piv', 'bull', 'hellip', 'prime', 'Prime', 'oline', 'frasl', 'weierp', 'image', 'real', 'trade', 'alefsym', 'larr', 'uarr', 'rarr', 'darr', 'harr', 'crarr', 'lArr', 'uArr', 'rArr', 'dArr', 'hArr', 'forall', 'part', 'exist', 'empty', 'nabla', 'isin', 'notin', 'ni', 'prod', 'sum', 'minus', 'lowast', 'radic', 'prop', 'infin', 'ang', 'and', 'or', 'cap', 'cup', 'int', 'there4', 'sim', 'cong', 'asymp', 'ne', 'equiv', 'le', 'ge', 'sub', 'sup', 'nsub', 'sube', 'supe', 'oplus', 'otimes', 'perp', 'sdot', 'lceil', 'rceil', 'lfloor', 'rfloor', 'lang', 'rang', 'loz', 'spades', 'clubs', 'hearts', 'diams'];
var HTML_CODES = [39, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 34, 38, 60, 62, 338, 339, 352, 353, 376, 710, 732, 8194, 8195, 8201, 8204, 8205, 8206, 8207, 8211, 8212, 8216, 8217, 8218, 8220, 8221, 8222, 8224, 8225, 8240, 8249, 8250, 8364, 402, 913, 914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929, 931, 932, 933, 934, 935, 936, 937, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 977, 978, 982, 8226, 8230, 8242, 8243, 8254, 8260, 8472, 8465, 8476, 8482, 8501, 8592, 8593, 8594, 8595, 8596, 8629, 8656, 8657, 8658, 8659, 8660, 8704, 8706, 8707, 8709, 8711, 8712, 8713, 8715, 8719, 8721, 8722, 8727, 8730, 8733, 8734, 8736, 8743, 8744, 8745, 8746, 8747, 8756, 8764, 8773, 8776, 8800, 8801, 8804, 8805, 8834, 8835, 8836, 8838, 8839, 8853, 8855, 8869, 8901, 8968, 8969, 8970, 8971, 9001, 9002, 9674, 9824, 9827, 9829, 9830];

var alphaIndex = {};
var numIndex = {};

var i = 0;
var length = HTML_ALPHA.length;
while (i < length) {
    var a = HTML_ALPHA[i];
    var c = HTML_CODES[i];
    alphaIndex[a] = String.fromCharCode(c);
    numIndex[c] = a;
    i++;
}

/**
 * @constructor
 */
function Html4Entities() {}

/**
 * @param {String} str
 * @returns {String}
 */
Html4Entities.prototype.decode = function(str) {
    if (!str || !str.length) {
        return '';
    }
    return str.replace(/&(#?[\w\d]+);?/g, function(s, entity) {
        var chr;
        if (entity.charAt(0) === "#") {
            var code = entity.charAt(1).toLowerCase() === 'x' ?
                parseInt(entity.substr(2), 16) :
                parseInt(entity.substr(1));

            if (!(isNaN(code) || code < -32768 || code > 65535)) {
                chr = String.fromCharCode(code);
            }
        } else {
            chr = alphaIndex[entity];
        }
        return chr || s;
    });
};

/**
 * @param {String} str
 * @returns {String}
 */
Html4Entities.decode = function(str) {
    return new Html4Entities().decode(str);
};

/**
 * @param {String} str
 * @returns {String}
 */
Html4Entities.prototype.encode = function(str) {
    if (!str || !str.length) {
        return '';
    }
    var strLength = str.length;
    var result = '';
    var i = 0;
    while (i < strLength) {
        var alpha = numIndex[str.charCodeAt(i)];
        result += alpha ? "&" + alpha + ";" : str.charAt(i);
        i++;
    }
    return result;
};

/**
 * @param {String} str
 * @returns {String}
 */
Html4Entities.encode = function(str) {
    return new Html4Entities().encode(str);
};

/**
 * @param {String} str
 * @returns {String}
 */
Html4Entities.prototype.encodeNonUTF = function(str) {
    if (!str || !str.length) {
        return '';
    }
    var strLength = str.length;
    var result = '';
    var i = 0;
    while (i < strLength) {
        var cc = str.charCodeAt(i);
        var alpha = numIndex[cc];
        if (alpha) {
            result += "&" + alpha + ";";
        } else if (cc < 32 || cc > 126) {
            result += "&#" + cc + ";";
        } else {
            result += str.charAt(i);
        }
        i++;
    }
    return result;
};

/**
 * @param {String} str
 * @returns {String}
 */
Html4Entities.encodeNonUTF = function(str) {
    return new Html4Entities().encodeNonUTF(str);
};

/**
 * @param {String} str
 * @returns {String}
 */
Html4Entities.prototype.encodeNonASCII = function(str) {
    if (!str || !str.length) {
        return '';
    }
    var strLength = str.length;
    var result = '';
    var i = 0;
    while (i < strLength) {
        var c = str.charCodeAt(i);
        if (c <= 255) {
            result += str[i++];
            continue;
        }
        result += '&#' + c + ';';
        i++;
    }
    return result;
};

/**
 * @param {String} str
 * @returns {String}
 */
Html4Entities.encodeNonASCII = function(str) {
    return new Html4Entities().encodeNonASCII(str);
};

module.exports = Html4Entities;


/***/ }),
/* 14 */
/***/ (function(module, exports, __webpack_require__) {

/**
 * Based heavily on https://github.com/webpack/webpack/blob/
 *  c0afdf9c6abc1dd70707c594e473802a566f7b6e/hot/only-dev-server.js
 * Original copyright Tobias Koppers @sokra (MIT license)
 */

/* global window __webpack_hash__ */

if (false) {
  throw new Error("[HMR] Hot Module Replacement is disabled.");
}

var hmrDocsUrl = "https://webpack.js.org/concepts/hot-module-replacement/"; // eslint-disable-line max-len

var lastHash;
var failureStatuses = { abort: 1, fail: 1 };
var applyOptions = { 				
  ignoreUnaccepted: true,
  ignoreDeclined: true,
  ignoreErrored: true,
  onUnaccepted: function(data) {
    console.warn("Ignored an update to unaccepted module " + data.chain.join(" -> "));
  },
  onDeclined: function(data) {
    console.warn("Ignored an update to declined module " + data.chain.join(" -> "));
  },
  onErrored: function(data) {
    console.error(data.error);
    console.warn("Ignored an error while updating module " + data.moduleId + " (" + data.type + ")");
  } 
}

function upToDate(hash) {
  if (hash) lastHash = hash;
  return lastHash == __webpack_require__.h();
}

module.exports = function(hash, moduleMap, options) {
  var reload = options.reload;
  if (!upToDate(hash) && module.hot.status() == "idle") {
    if (options.log) console.log("[HMR] Checking for updates on the server...");
    check();
  }

  function check() {
    var cb = function(err, updatedModules) {
      if (err) return handleError(err);

      if(!updatedModules) {
        if (options.warn) {
          console.warn("[HMR] Cannot find update (Full reload needed)");
          console.warn("[HMR] (Probably because of restarting the server)");
        }
        performReload();
        return null;
      }

      var applyCallback = function(applyErr, renewedModules) {
        if (applyErr) return handleError(applyErr);

        if (!upToDate()) check();

        logUpdates(updatedModules, renewedModules);
      };

      var applyResult = module.hot.apply(applyOptions, applyCallback);
      // webpack 2 promise
      if (applyResult && applyResult.then) {
        // HotModuleReplacement.runtime.js refers to the result as `outdatedModules`
        applyResult.then(function(outdatedModules) {
          applyCallback(null, outdatedModules);
        });
        applyResult.catch(applyCallback);
      }

    };

    var result = module.hot.check(false, cb);
    // webpack 2 promise
    if (result && result.then) {
        result.then(function(updatedModules) {
            cb(null, updatedModules);
        });
        result.catch(cb);
    }
  }

  function logUpdates(updatedModules, renewedModules) {
    var unacceptedModules = updatedModules.filter(function(moduleId) {
      return renewedModules && renewedModules.indexOf(moduleId) < 0;
    });

    if(unacceptedModules.length > 0) {
      if (options.warn) {
        console.warn(
          "[HMR] The following modules couldn't be hot updated: " +
          "(Full reload needed)\n" +
          "This is usually because the modules which have changed " +
          "(and their parents) do not know how to hot reload themselves. " +
          "See " + hmrDocsUrl + " for more details."
        );
        unacceptedModules.forEach(function(moduleId) {
          console.warn("[HMR]  - " + moduleMap[moduleId]);
        });
      }
      performReload();
      return;
    }

    if (options.log) {
      if(!renewedModules || renewedModules.length === 0) {
        console.log("[HMR] Nothing hot updated.");
      } else {
        console.log("[HMR] Updated modules:");
        renewedModules.forEach(function(moduleId) {
          console.log("[HMR]  - " + moduleMap[moduleId]);
        });
      }

      if (upToDate()) {
        console.log("[HMR] App is up to date.");
      }
    }
  }

  function handleError(err) {
    if (module.hot.status() in failureStatuses) {
      if (options.warn) {
        console.warn("[HMR] Cannot check for update (Full reload needed)");
        console.warn("[HMR] " + err.stack || err.message);
      }
      performReload();
      return;
    }
    if (options.warn) {
      console.warn("[HMR] Update check failed: " + err.stack || err.message);
    }
  }

  function performReload() {
    if (reload) {
      if (options.warn) console.warn("[HMR] Reloading page");
      window.location.reload();
    }
  }
};


/***/ }),
/* 15 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


__webpack_require__(16);

__webpack_require__(17);

__webpack_require__(19);

__webpack_require__(20);

__webpack_require__(22);

__webpack_require__(23);

__webpack_require__(24);

__webpack_require__(25);

__webpack_require__(26);

__webpack_require__(27);

__webpack_require__(28);

__webpack_require__(29);

__webpack_require__(30);

__webpack_require__(31);

__webpack_require__(32);

__webpack_require__(33);

__webpack_require__(34);

__webpack_require__(35);

/***/ }),
/* 16 */
/***/ (function(module, exports) {

// removed by extract-text-webpack-plugin

/***/ }),
/* 17 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
var __WEBPACK_AMD_DEFINE_FACTORY__, __WEBPACK_AMD_DEFINE_ARRAY__, __WEBPACK_AMD_DEFINE_RESULT__;

var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

/*! jQuery UI - v1.12.0 - 2016-08-27
* http://jqueryui.com
* Includes: widget.js, data.js, keycode.js, scroll-parent.js, widgets/sortable.js, widgets/datepicker.js, widgets/mouse.js
* Copyright jQuery Foundation and other contributors; Licensed MIT */

(function (factory) {
	if (true) {

		// AMD. Register as an anonymous module.
		!(__WEBPACK_AMD_DEFINE_ARRAY__ = [__webpack_require__(18)], __WEBPACK_AMD_DEFINE_FACTORY__ = (factory),
				__WEBPACK_AMD_DEFINE_RESULT__ = (typeof __WEBPACK_AMD_DEFINE_FACTORY__ === 'function' ?
				(__WEBPACK_AMD_DEFINE_FACTORY__.apply(exports, __WEBPACK_AMD_DEFINE_ARRAY__)) : __WEBPACK_AMD_DEFINE_FACTORY__),
				__WEBPACK_AMD_DEFINE_RESULT__ !== undefined && (module.exports = __WEBPACK_AMD_DEFINE_RESULT__));
	} else {

		// Browser globals
		factory(jQuery);
	}
})(function ($) {

	$.ui = $.ui || {};

	var version = $.ui.version = "1.12.0";

	/*!
  * jQuery UI Widget 1.12.0
  * http://jqueryui.com
  *
  * Copyright jQuery Foundation and other contributors
  * Released under the MIT license.
  * http://jquery.org/license
  */

	//>>label: Widget
	//>>group: Core
	//>>description: Provides a factory for creating stateful widgets with a common API.
	//>>docs: http://api.jqueryui.com/jQuery.widget/
	//>>demos: http://jqueryui.com/widget/


	var widgetUuid = 0;
	var widgetSlice = Array.prototype.slice;

	$.cleanData = function (orig) {
		return function (elems) {
			var events, elem, i;
			for (i = 0; (elem = elems[i]) != null; i++) {
				try {

					// Only trigger remove when necessary to save time
					events = $._data(elem, "events");
					if (events && events.remove) {
						$(elem).triggerHandler("remove");
					}

					// Http://bugs.jquery.com/ticket/8235
				} catch (e) {}
			}
			orig(elems);
		};
	}($.cleanData);

	$.widget = function (name, base, prototype) {
		var existingConstructor, constructor, basePrototype;

		// ProxiedPrototype allows the provided prototype to remain unmodified
		// so that it can be used as a mixin for multiple widgets (#8876)
		var proxiedPrototype = {};

		var namespace = name.split(".")[0];
		name = name.split(".")[1];
		var fullName = namespace + "-" + name;

		if (!prototype) {
			prototype = base;
			base = $.Widget;
		}

		if ($.isArray(prototype)) {
			prototype = $.extend.apply(null, [{}].concat(prototype));
		}

		// Create selector for plugin
		$.expr[":"][fullName.toLowerCase()] = function (elem) {
			return !!$.data(elem, fullName);
		};

		$[namespace] = $[namespace] || {};
		existingConstructor = $[namespace][name];
		constructor = $[namespace][name] = function (options, element) {

			// Allow instantiation without "new" keyword
			if (!this._createWidget) {
				return new constructor(options, element);
			}

			// Allow instantiation without initializing for simple inheritance
			// must use "new" keyword (the code above always passes args)
			if (arguments.length) {
				this._createWidget(options, element);
			}
		};

		// Extend with the existing constructor to carry over any static properties
		$.extend(constructor, existingConstructor, {
			version: prototype.version,

			// Copy the object used to create the prototype in case we need to
			// redefine the widget later
			_proto: $.extend({}, prototype),

			// Track widgets that inherit from this widget in case this widget is
			// redefined after a widget inherits from it
			_childConstructors: []
		});

		basePrototype = new base();

		// We need to make the options hash a property directly on the new instance
		// otherwise we'll modify the options hash on the prototype that we're
		// inheriting from
		basePrototype.options = $.widget.extend({}, basePrototype.options);
		$.each(prototype, function (prop, value) {
			if (!$.isFunction(value)) {
				proxiedPrototype[prop] = value;
				return;
			}
			proxiedPrototype[prop] = function () {
				function _super() {
					return base.prototype[prop].apply(this, arguments);
				}

				function _superApply(args) {
					return base.prototype[prop].apply(this, args);
				}

				return function () {
					var __super = this._super;
					var __superApply = this._superApply;
					var returnValue;

					this._super = _super;
					this._superApply = _superApply;

					returnValue = value.apply(this, arguments);

					this._super = __super;
					this._superApply = __superApply;

					return returnValue;
				};
			}();
		});
		constructor.prototype = $.widget.extend(basePrototype, {

			// TODO: remove support for widgetEventPrefix
			// always use the name + a colon as the prefix, e.g., draggable:start
			// don't prefix for widgets that aren't DOM-based
			widgetEventPrefix: existingConstructor ? basePrototype.widgetEventPrefix || name : name
		}, proxiedPrototype, {
			constructor: constructor,
			namespace: namespace,
			widgetName: name,
			widgetFullName: fullName
		});

		// If this widget is being redefined then we need to find all widgets that
		// are inheriting from it and redefine all of them so that they inherit from
		// the new version of this widget. We're essentially trying to replace one
		// level in the prototype chain.
		if (existingConstructor) {
			$.each(existingConstructor._childConstructors, function (i, child) {
				var childPrototype = child.prototype;

				// Redefine the child widget using the same prototype that was
				// originally used, but inherit from the new version of the base
				$.widget(childPrototype.namespace + "." + childPrototype.widgetName, constructor, child._proto);
			});

			// Remove the list of existing child constructors from the old constructor
			// so the old child constructors can be garbage collected
			delete existingConstructor._childConstructors;
		} else {
			base._childConstructors.push(constructor);
		}

		$.widget.bridge(name, constructor);

		return constructor;
	};

	$.widget.extend = function (target) {
		var input = widgetSlice.call(arguments, 1);
		var inputIndex = 0;
		var inputLength = input.length;
		var key;
		var value;

		for (; inputIndex < inputLength; inputIndex++) {
			for (key in input[inputIndex]) {
				value = input[inputIndex][key];
				if (input[inputIndex].hasOwnProperty(key) && value !== undefined) {

					// Clone objects
					if ($.isPlainObject(value)) {
						target[key] = $.isPlainObject(target[key]) ? $.widget.extend({}, target[key], value) :

						// Don't extend strings, arrays, etc. with objects
						$.widget.extend({}, value);

						// Copy everything else by reference
					} else {
						target[key] = value;
					}
				}
			}
		}
		return target;
	};

	$.widget.bridge = function (name, object) {
		var fullName = object.prototype.widgetFullName || name;
		$.fn[name] = function (options) {
			var isMethodCall = typeof options === "string";
			var args = widgetSlice.call(arguments, 1);
			var returnValue = this;

			if (isMethodCall) {
				this.each(function () {
					var methodValue;
					var instance = $.data(this, fullName);

					if (options === "instance") {
						returnValue = instance;
						return false;
					}

					if (!instance) {
						return $.error("cannot call methods on " + name + " prior to initialization; " + "attempted to call method '" + options + "'");
					}

					if (!$.isFunction(instance[options]) || options.charAt(0) === "_") {
						return $.error("no such method '" + options + "' for " + name + " widget instance");
					}

					methodValue = instance[options].apply(instance, args);

					if (methodValue !== instance && methodValue !== undefined) {
						returnValue = methodValue && methodValue.jquery ? returnValue.pushStack(methodValue.get()) : methodValue;
						return false;
					}
				});
			} else {

				// Allow multiple hashes to be passed on init
				if (args.length) {
					options = $.widget.extend.apply(null, [options].concat(args));
				}

				this.each(function () {
					var instance = $.data(this, fullName);
					if (instance) {
						instance.option(options || {});
						if (instance._init) {
							instance._init();
						}
					} else {
						$.data(this, fullName, new object(options, this));
					}
				});
			}

			return returnValue;
		};
	};

	$.Widget = function () /* options, element */{};
	$.Widget._childConstructors = [];

	$.Widget.prototype = {
		widgetName: "widget",
		widgetEventPrefix: "",
		defaultElement: "<div>",

		options: {
			classes: {},
			disabled: false,

			// Callbacks
			create: null
		},

		_createWidget: function _createWidget(options, element) {
			element = $(element || this.defaultElement || this)[0];
			this.element = $(element);
			this.uuid = widgetUuid++;
			this.eventNamespace = "." + this.widgetName + this.uuid;

			this.bindings = $();
			this.hoverable = $();
			this.focusable = $();
			this.classesElementLookup = {};

			if (element !== this) {
				$.data(element, this.widgetFullName, this);
				this._on(true, this.element, {
					remove: function remove(event) {
						if (event.target === element) {
							this.destroy();
						}
					}
				});
				this.document = $(element.style ?

				// Element within the document
				element.ownerDocument :

				// Element is window or document
				element.document || element);
				this.window = $(this.document[0].defaultView || this.document[0].parentWindow);
			}

			this.options = $.widget.extend({}, this.options, this._getCreateOptions(), options);

			this._create();

			if (this.options.disabled) {
				this._setOptionDisabled(this.options.disabled);
			}

			this._trigger("create", null, this._getCreateEventData());
			this._init();
		},

		_getCreateOptions: function _getCreateOptions() {
			return {};
		},

		_getCreateEventData: $.noop,

		_create: $.noop,

		_init: $.noop,

		destroy: function destroy() {
			var that = this;

			this._destroy();
			$.each(this.classesElementLookup, function (key, value) {
				that._removeClass(value, key);
			});

			// We can probably remove the unbind calls in 2.0
			// all event bindings should go through this._on()
			this.element.off(this.eventNamespace).removeData(this.widgetFullName);
			this.widget().off(this.eventNamespace).removeAttr("aria-disabled");

			// Clean up events and states
			this.bindings.off(this.eventNamespace);
		},

		_destroy: $.noop,

		widget: function widget() {
			return this.element;
		},

		option: function option(key, value) {
			var options = key;
			var parts;
			var curOption;
			var i;

			if (arguments.length === 0) {

				// Don't return a reference to the internal hash
				return $.widget.extend({}, this.options);
			}

			if (typeof key === "string") {

				// Handle nested keys, e.g., "foo.bar" => { foo: { bar: ___ } }
				options = {};
				parts = key.split(".");
				key = parts.shift();
				if (parts.length) {
					curOption = options[key] = $.widget.extend({}, this.options[key]);
					for (i = 0; i < parts.length - 1; i++) {
						curOption[parts[i]] = curOption[parts[i]] || {};
						curOption = curOption[parts[i]];
					}
					key = parts.pop();
					if (arguments.length === 1) {
						return curOption[key] === undefined ? null : curOption[key];
					}
					curOption[key] = value;
				} else {
					if (arguments.length === 1) {
						return this.options[key] === undefined ? null : this.options[key];
					}
					options[key] = value;
				}
			}

			this._setOptions(options);

			return this;
		},

		_setOptions: function _setOptions(options) {
			var key;

			for (key in options) {
				this._setOption(key, options[key]);
			}

			return this;
		},

		_setOption: function _setOption(key, value) {
			if (key === "classes") {
				this._setOptionClasses(value);
			}

			this.options[key] = value;

			if (key === "disabled") {
				this._setOptionDisabled(value);
			}

			return this;
		},

		_setOptionClasses: function _setOptionClasses(value) {
			var classKey, elements, currentElements;

			for (classKey in value) {
				currentElements = this.classesElementLookup[classKey];
				if (value[classKey] === this.options.classes[classKey] || !currentElements || !currentElements.length) {
					continue;
				}

				// We are doing this to create a new jQuery object because the _removeClass() call
				// on the next line is going to destroy the reference to the current elements being
				// tracked. We need to save a copy of this collection so that we can add the new classes
				// below.
				elements = $(currentElements.get());
				this._removeClass(currentElements, classKey);

				// We don't use _addClass() here, because that uses this.options.classes
				// for generating the string of classes. We want to use the value passed in from
				// _setOption(), this is the new value of the classes option which was passed to
				// _setOption(). We pass this value directly to _classes().
				elements.addClass(this._classes({
					element: elements,
					keys: classKey,
					classes: value,
					add: true
				}));
			}
		},

		_setOptionDisabled: function _setOptionDisabled(value) {
			this._toggleClass(this.widget(), this.widgetFullName + "-disabled", null, !!value);

			// If the widget is becoming disabled, then nothing is interactive
			if (value) {
				this._removeClass(this.hoverable, null, "ui-state-hover");
				this._removeClass(this.focusable, null, "ui-state-focus");
			}
		},

		enable: function enable() {
			return this._setOptions({ disabled: false });
		},

		disable: function disable() {
			return this._setOptions({ disabled: true });
		},

		_classes: function _classes(options) {
			var full = [];
			var that = this;

			options = $.extend({
				element: this.element,
				classes: this.options.classes || {}
			}, options);

			function processClassString(classes, checkOption) {
				var current, i;
				for (i = 0; i < classes.length; i++) {
					current = that.classesElementLookup[classes[i]] || $();
					if (options.add) {
						current = $($.unique(current.get().concat(options.element.get())));
					} else {
						current = $(current.not(options.element).get());
					}
					that.classesElementLookup[classes[i]] = current;
					full.push(classes[i]);
					if (checkOption && options.classes[classes[i]]) {
						full.push(options.classes[classes[i]]);
					}
				}
			}

			if (options.keys) {
				processClassString(options.keys.match(/\S+/g) || [], true);
			}
			if (options.extra) {
				processClassString(options.extra.match(/\S+/g) || []);
			}

			return full.join(" ");
		},

		_removeClass: function _removeClass(element, keys, extra) {
			return this._toggleClass(element, keys, extra, false);
		},

		_addClass: function _addClass(element, keys, extra) {
			return this._toggleClass(element, keys, extra, true);
		},

		_toggleClass: function _toggleClass(element, keys, extra, add) {
			add = typeof add === "boolean" ? add : extra;
			var shift = typeof element === "string" || element === null,
			    options = {
				extra: shift ? keys : extra,
				keys: shift ? element : keys,
				element: shift ? this.element : element,
				add: add
			};
			options.element.toggleClass(this._classes(options), add);
			return this;
		},

		_on: function _on(suppressDisabledCheck, element, handlers) {
			var delegateElement;
			var instance = this;

			// No suppressDisabledCheck flag, shuffle arguments
			if (typeof suppressDisabledCheck !== "boolean") {
				handlers = element;
				element = suppressDisabledCheck;
				suppressDisabledCheck = false;
			}

			// No element argument, shuffle and use this.element
			if (!handlers) {
				handlers = element;
				element = this.element;
				delegateElement = this.widget();
			} else {
				element = delegateElement = $(element);
				this.bindings = this.bindings.add(element);
			}

			$.each(handlers, function (event, handler) {
				function handlerProxy() {

					// Allow widgets to customize the disabled handling
					// - disabled as an array instead of boolean
					// - disabled class as method for disabling individual parts
					if (!suppressDisabledCheck && (instance.options.disabled === true || $(this).hasClass("ui-state-disabled"))) {
						return;
					}
					return (typeof handler === "string" ? instance[handler] : handler).apply(instance, arguments);
				}

				// Copy the guid so direct unbinding works
				if (typeof handler !== "string") {
					handlerProxy.guid = handler.guid = handler.guid || handlerProxy.guid || $.guid++;
				}

				var match = event.match(/^([\w:-]*)\s*(.*)$/);
				var eventName = match[1] + instance.eventNamespace;
				var selector = match[2];

				if (selector) {
					delegateElement.on(eventName, selector, handlerProxy);
				} else {
					element.on(eventName, handlerProxy);
				}
			});
		},

		_off: function _off(element, eventName) {
			eventName = (eventName || "").split(" ").join(this.eventNamespace + " ") + this.eventNamespace;
			element.off(eventName).off(eventName);

			// Clear the stack to avoid memory leaks (#10056)
			this.bindings = $(this.bindings.not(element).get());
			this.focusable = $(this.focusable.not(element).get());
			this.hoverable = $(this.hoverable.not(element).get());
		},

		_delay: function _delay(handler, delay) {
			function handlerProxy() {
				return (typeof handler === "string" ? instance[handler] : handler).apply(instance, arguments);
			}
			var instance = this;
			return setTimeout(handlerProxy, delay || 0);
		},

		_hoverable: function _hoverable(element) {
			this.hoverable = this.hoverable.add(element);
			this._on(element, {
				mouseenter: function mouseenter(event) {
					this._addClass($(event.currentTarget), null, "ui-state-hover");
				},
				mouseleave: function mouseleave(event) {
					this._removeClass($(event.currentTarget), null, "ui-state-hover");
				}
			});
		},

		_focusable: function _focusable(element) {
			this.focusable = this.focusable.add(element);
			this._on(element, {
				focusin: function focusin(event) {
					this._addClass($(event.currentTarget), null, "ui-state-focus");
				},
				focusout: function focusout(event) {
					this._removeClass($(event.currentTarget), null, "ui-state-focus");
				}
			});
		},

		_trigger: function _trigger(type, event, data) {
			var prop, orig;
			var callback = this.options[type];

			data = data || {};
			event = $.Event(event);
			event.type = (type === this.widgetEventPrefix ? type : this.widgetEventPrefix + type).toLowerCase();

			// The original event may come from any element
			// so we need to reset the target on the new event
			event.target = this.element[0];

			// Copy original event properties over to the new event
			orig = event.originalEvent;
			if (orig) {
				for (prop in orig) {
					if (!(prop in event)) {
						event[prop] = orig[prop];
					}
				}
			}

			this.element.trigger(event, data);
			return !($.isFunction(callback) && callback.apply(this.element[0], [event].concat(data)) === false || event.isDefaultPrevented());
		}
	};

	$.each({ show: "fadeIn", hide: "fadeOut" }, function (method, defaultEffect) {
		$.Widget.prototype["_" + method] = function (element, options, callback) {
			if (typeof options === "string") {
				options = { effect: options };
			}

			var hasOptions;
			var effectName = !options ? method : options === true || typeof options === "number" ? defaultEffect : options.effect || defaultEffect;

			options = options || {};
			if (typeof options === "number") {
				options = { duration: options };
			}

			hasOptions = !$.isEmptyObject(options);
			options.complete = callback;

			if (options.delay) {
				element.delay(options.delay);
			}

			if (hasOptions && $.effects && $.effects.effect[effectName]) {
				element[method](options);
			} else if (effectName !== method && element[effectName]) {
				element[effectName](options.duration, options.easing, callback);
			} else {
				element.queue(function (next) {
					$(this)[method]();
					if (callback) {
						callback.call(element[0]);
					}
					next();
				});
			}
		};
	});

	var widget = $.widget;

	/*!
  * jQuery UI :data 1.12.0
  * http://jqueryui.com
  *
  * Copyright jQuery Foundation and other contributors
  * Released under the MIT license.
  * http://jquery.org/license
  */

	//>>label: :data Selector
	//>>group: Core
	//>>description: Selects elements which have data stored under the specified key.
	//>>docs: http://api.jqueryui.com/data-selector/


	var data = $.extend($.expr[":"], {
		data: $.expr.createPseudo ? $.expr.createPseudo(function (dataName) {
			return function (elem) {
				return !!$.data(elem, dataName);
			};
		}) :

		// Support: jQuery <1.8
		function (elem, i, match) {
			return !!$.data(elem, match[3]);
		}
	});

	/*!
  * jQuery UI Keycode 1.12.0
  * http://jqueryui.com
  *
  * Copyright jQuery Foundation and other contributors
  * Released under the MIT license.
  * http://jquery.org/license
  */

	//>>label: Keycode
	//>>group: Core
	//>>description: Provide keycodes as keynames
	//>>docs: http://api.jqueryui.com/jQuery.ui.keyCode/


	var keycode = $.ui.keyCode = {
		BACKSPACE: 8,
		COMMA: 188,
		DELETE: 46,
		DOWN: 40,
		END: 35,
		ENTER: 13,
		ESCAPE: 27,
		HOME: 36,
		LEFT: 37,
		PAGE_DOWN: 34,
		PAGE_UP: 33,
		PERIOD: 190,
		RIGHT: 39,
		SPACE: 32,
		TAB: 9,
		UP: 38
	};

	/*!
  * jQuery UI Scroll Parent 1.12.0
  * http://jqueryui.com
  *
  * Copyright jQuery Foundation and other contributors
  * Released under the MIT license.
  * http://jquery.org/license
  */

	//>>label: scrollParent
	//>>group: Core
	//>>description: Get the closest ancestor element that is scrollable.
	//>>docs: http://api.jqueryui.com/scrollParent/


	var scrollParent = $.fn.scrollParent = function (includeHidden) {
		var position = this.css("position"),
		    excludeStaticParent = position === "absolute",
		    overflowRegex = includeHidden ? /(auto|scroll|hidden)/ : /(auto|scroll)/,
		    scrollParent = this.parents().filter(function () {
			var parent = $(this);
			if (excludeStaticParent && parent.css("position") === "static") {
				return false;
			}
			return overflowRegex.test(parent.css("overflow") + parent.css("overflow-y") + parent.css("overflow-x"));
		}).eq(0);

		return position === "fixed" || !scrollParent.length ? $(this[0].ownerDocument || document) : scrollParent;
	};

	// This file is deprecated
	var ie = $.ui.ie = !!/msie [\w.]+/.exec(navigator.userAgent.toLowerCase());

	/*!
  * jQuery UI Mouse 1.12.0
  * http://jqueryui.com
  *
  * Copyright jQuery Foundation and other contributors
  * Released under the MIT license.
  * http://jquery.org/license
  */

	//>>label: Mouse
	//>>group: Widgets
	//>>description: Abstracts mouse-based interactions to assist in creating certain widgets.
	//>>docs: http://api.jqueryui.com/mouse/


	var mouseHandled = false;
	$(document).on("mouseup", function () {
		mouseHandled = false;
	});

	var widgetsMouse = $.widget("ui.mouse", {
		version: "1.12.0",
		options: {
			cancel: "input, textarea, button, select, option",
			distance: 1,
			delay: 0
		},
		_mouseInit: function _mouseInit() {
			var that = this;

			this.element.on("mousedown." + this.widgetName, function (event) {
				return that._mouseDown(event);
			}).on("click." + this.widgetName, function (event) {
				if (true === $.data(event.target, that.widgetName + ".preventClickEvent")) {
					$.removeData(event.target, that.widgetName + ".preventClickEvent");
					event.stopImmediatePropagation();
					return false;
				}
			});

			this.started = false;
		},

		// TODO: make sure destroying one instance of mouse doesn't mess with
		// other instances of mouse
		_mouseDestroy: function _mouseDestroy() {
			this.element.off("." + this.widgetName);
			if (this._mouseMoveDelegate) {
				this.document.off("mousemove." + this.widgetName, this._mouseMoveDelegate).off("mouseup." + this.widgetName, this._mouseUpDelegate);
			}
		},

		_mouseDown: function _mouseDown(event) {

			// don't let more than one widget handle mouseStart
			if (mouseHandled) {
				return;
			}

			this._mouseMoved = false;

			// We may have missed mouseup (out of window)
			this._mouseStarted && this._mouseUp(event);

			this._mouseDownEvent = event;

			var that = this,
			    btnIsLeft = event.which === 1,


			// event.target.nodeName works around a bug in IE 8 with
			// disabled inputs (#7620)
			elIsCancel = typeof this.options.cancel === "string" && event.target.nodeName ? $(event.target).closest(this.options.cancel).length : false;
			if (!btnIsLeft || elIsCancel || !this._mouseCapture(event)) {
				return true;
			}

			this.mouseDelayMet = !this.options.delay;
			if (!this.mouseDelayMet) {
				this._mouseDelayTimer = setTimeout(function () {
					that.mouseDelayMet = true;
				}, this.options.delay);
			}

			if (this._mouseDistanceMet(event) && this._mouseDelayMet(event)) {
				this._mouseStarted = this._mouseStart(event) !== false;
				if (!this._mouseStarted) {
					event.preventDefault();
					return true;
				}
			}

			// Click event may never have fired (Gecko & Opera)
			if (true === $.data(event.target, this.widgetName + ".preventClickEvent")) {
				$.removeData(event.target, this.widgetName + ".preventClickEvent");
			}

			// These delegates are required to keep context
			this._mouseMoveDelegate = function (event) {
				return that._mouseMove(event);
			};
			this._mouseUpDelegate = function (event) {
				return that._mouseUp(event);
			};

			this.document.on("mousemove." + this.widgetName, this._mouseMoveDelegate).on("mouseup." + this.widgetName, this._mouseUpDelegate);

			event.preventDefault();

			mouseHandled = true;
			return true;
		},

		_mouseMove: function _mouseMove(event) {

			// Only check for mouseups outside the document if you've moved inside the document
			// at least once. This prevents the firing of mouseup in the case of IE<9, which will
			// fire a mousemove event if content is placed under the cursor. See #7778
			// Support: IE <9
			if (this._mouseMoved) {

				// IE mouseup check - mouseup happened when mouse was out of window
				if ($.ui.ie && (!document.documentMode || document.documentMode < 9) && !event.button) {
					return this._mouseUp(event);

					// Iframe mouseup check - mouseup occurred in another document
				} else if (!event.which) {

					// Support: Safari <=8 - 9
					// Safari sets which to 0 if you press any of the following keys
					// during a drag (#14461)
					if (event.originalEvent.altKey || event.originalEvent.ctrlKey || event.originalEvent.metaKey || event.originalEvent.shiftKey) {
						this.ignoreMissingWhich = true;
					} else if (!this.ignoreMissingWhich) {
						return this._mouseUp(event);
					}
				}
			}

			if (event.which || event.button) {
				this._mouseMoved = true;
			}

			if (this._mouseStarted) {
				this._mouseDrag(event);
				return event.preventDefault();
			}

			if (this._mouseDistanceMet(event) && this._mouseDelayMet(event)) {
				this._mouseStarted = this._mouseStart(this._mouseDownEvent, event) !== false;
				this._mouseStarted ? this._mouseDrag(event) : this._mouseUp(event);
			}

			return !this._mouseStarted;
		},

		_mouseUp: function _mouseUp(event) {
			this.document.off("mousemove." + this.widgetName, this._mouseMoveDelegate).off("mouseup." + this.widgetName, this._mouseUpDelegate);

			if (this._mouseStarted) {
				this._mouseStarted = false;

				if (event.target === this._mouseDownEvent.target) {
					$.data(event.target, this.widgetName + ".preventClickEvent", true);
				}

				this._mouseStop(event);
			}

			if (this._mouseDelayTimer) {
				clearTimeout(this._mouseDelayTimer);
				delete this._mouseDelayTimer;
			}

			this.ignoreMissingWhich = false;
			mouseHandled = false;
			event.preventDefault();
		},

		_mouseDistanceMet: function _mouseDistanceMet(event) {
			return Math.max(Math.abs(this._mouseDownEvent.pageX - event.pageX), Math.abs(this._mouseDownEvent.pageY - event.pageY)) >= this.options.distance;
		},

		_mouseDelayMet: function _mouseDelayMet() /* event */{
			return this.mouseDelayMet;
		},

		// These are placeholder methods, to be overriden by extending plugin
		_mouseStart: function _mouseStart() /* event */{},
		_mouseDrag: function _mouseDrag() /* event */{},
		_mouseStop: function _mouseStop() /* event */{},
		_mouseCapture: function _mouseCapture() /* event */{
			return true;
		}
	});

	/*!
  * jQuery UI Sortable 1.12.0
  * http://jqueryui.com
  *
  * Copyright jQuery Foundation and other contributors
  * Released under the MIT license.
  * http://jquery.org/license
  */

	//>>label: Sortable
	//>>group: Interactions
	//>>description: Enables items in a list to be sorted using the mouse.
	//>>docs: http://api.jqueryui.com/sortable/
	//>>demos: http://jqueryui.com/sortable/
	//>>css.structure: ../../themes/base/sortable.css


	var widgetsSortable = $.widget("ui.sortable", $.ui.mouse, {
		version: "1.12.0",
		widgetEventPrefix: "sort",
		ready: false,
		options: {
			appendTo: "parent",
			axis: false,
			connectWith: false,
			containment: false,
			cursor: "auto",
			cursorAt: false,
			dropOnEmpty: true,
			forcePlaceholderSize: false,
			forceHelperSize: false,
			grid: false,
			handle: false,
			helper: "original",
			items: "> *",
			opacity: false,
			placeholder: false,
			revert: false,
			scroll: true,
			scrollSensitivity: 20,
			scrollSpeed: 20,
			scope: "default",
			tolerance: "intersect",
			zIndex: 1000,

			// Callbacks
			activate: null,
			beforeStop: null,
			change: null,
			deactivate: null,
			out: null,
			over: null,
			receive: null,
			remove: null,
			sort: null,
			start: null,
			stop: null,
			update: null
		},

		_isOverAxis: function _isOverAxis(x, reference, size) {
			return x >= reference && x < reference + size;
		},

		_isFloating: function _isFloating(item) {
			return (/left|right/.test(item.css("float")) || /inline|table-cell/.test(item.css("display"))
			);
		},

		_create: function _create() {
			this.containerCache = {};
			this._addClass("ui-sortable");

			//Get the items
			this.refresh();

			//Let's determine the parent's offset
			this.offset = this.element.offset();

			//Initialize mouse events for interaction
			this._mouseInit();

			this._setHandleClassName();

			//We're ready to go
			this.ready = true;
		},

		_setOption: function _setOption(key, value) {
			this._super(key, value);

			if (key === "handle") {
				this._setHandleClassName();
			}
		},

		_setHandleClassName: function _setHandleClassName() {
			var that = this;
			this._removeClass(this.element.find(".ui-sortable-handle"), "ui-sortable-handle");
			$.each(this.items, function () {
				that._addClass(this.instance.options.handle ? this.item.find(this.instance.options.handle) : this.item, "ui-sortable-handle");
			});
		},

		_destroy: function _destroy() {
			this._mouseDestroy();

			for (var i = this.items.length - 1; i >= 0; i--) {
				this.items[i].item.removeData(this.widgetName + "-item");
			}

			return this;
		},

		_mouseCapture: function _mouseCapture(event, overrideHandle) {
			var currentItem = null,
			    validHandle = false,
			    that = this;

			if (this.reverting) {
				return false;
			}

			if (this.options.disabled || this.options.type === "static") {
				return false;
			}

			//We have to refresh the items data once first
			this._refreshItems(event);

			//Find out if the clicked node (or one of its parents) is a actual item in this.items
			$(event.target).parents().each(function () {
				if ($.data(this, that.widgetName + "-item") === that) {
					currentItem = $(this);
					return false;
				}
			});
			if ($.data(event.target, that.widgetName + "-item") === that) {
				currentItem = $(event.target);
			}

			if (!currentItem) {
				return false;
			}
			if (this.options.handle && !overrideHandle) {
				$(this.options.handle, currentItem).find("*").addBack().each(function () {
					if (this === event.target) {
						validHandle = true;
					}
				});
				if (!validHandle) {
					return false;
				}
			}

			this.currentItem = currentItem;
			this._removeCurrentsFromItems();
			return true;
		},

		_mouseStart: function _mouseStart(event, overrideHandle, noActivation) {

			var i,
			    body,
			    o = this.options;

			this.currentContainer = this;

			//We only need to call refreshPositions, because the refreshItems call has been moved to
			// mouseCapture
			this.refreshPositions();

			//Create and append the visible helper
			this.helper = this._createHelper(event);

			//Cache the helper size
			this._cacheHelperProportions();

			/*
    * - Position generation -
    * This block generates everything position related - it's the core of draggables.
    */

			//Cache the margins of the original element
			this._cacheMargins();

			//Get the next scrolling parent
			this.scrollParent = this.helper.scrollParent();

			//The element's absolute position on the page minus margins
			this.offset = this.currentItem.offset();
			this.offset = {
				top: this.offset.top - this.margins.top,
				left: this.offset.left - this.margins.left
			};

			$.extend(this.offset, {
				click: { //Where the click happened, relative to the element
					left: event.pageX - this.offset.left,
					top: event.pageY - this.offset.top
				},
				parent: this._getParentOffset(),

				// This is a relative to absolute position minus the actual position calculation -
				// only used for relative positioned helper
				relative: this._getRelativeOffset()
			});

			// Only after we got the offset, we can change the helper's position to absolute
			// TODO: Still need to figure out a way to make relative sorting possible
			this.helper.css("position", "absolute");
			this.cssPosition = this.helper.css("position");

			//Generate the original position
			this.originalPosition = this._generatePosition(event);
			this.originalPageX = event.pageX;
			this.originalPageY = event.pageY;

			//Adjust the mouse offset relative to the helper if "cursorAt" is supplied
			o.cursorAt && this._adjustOffsetFromHelper(o.cursorAt);

			//Cache the former DOM position
			this.domPosition = {
				prev: this.currentItem.prev()[0],
				parent: this.currentItem.parent()[0]
			};

			// If the helper is not the original, hide the original so it's not playing any role during
			// the drag, won't cause anything bad this way
			if (this.helper[0] !== this.currentItem[0]) {
				this.currentItem.hide();
			}

			//Create the placeholder
			this._createPlaceholder();

			//Set a containment if given in the options
			if (o.containment) {
				this._setContainment();
			}

			if (o.cursor && o.cursor !== "auto") {
				// cursor option
				body = this.document.find("body");

				// Support: IE
				this.storedCursor = body.css("cursor");
				body.css("cursor", o.cursor);

				this.storedStylesheet = $("<style>*{ cursor: " + o.cursor + " !important; }</style>").appendTo(body);
			}

			if (o.opacity) {
				// opacity option
				if (this.helper.css("opacity")) {
					this._storedOpacity = this.helper.css("opacity");
				}
				this.helper.css("opacity", o.opacity);
			}

			if (o.zIndex) {
				// zIndex option
				if (this.helper.css("zIndex")) {
					this._storedZIndex = this.helper.css("zIndex");
				}
				this.helper.css("zIndex", o.zIndex);
			}

			//Prepare scrolling
			if (this.scrollParent[0] !== this.document[0] && this.scrollParent[0].tagName !== "HTML") {
				this.overflowOffset = this.scrollParent.offset();
			}

			//Call callbacks
			this._trigger("start", event, this._uiHash());

			//Recache the helper size
			if (!this._preserveHelperProportions) {
				this._cacheHelperProportions();
			}

			//Post "activate" events to possible containers
			if (!noActivation) {
				for (i = this.containers.length - 1; i >= 0; i--) {
					this.containers[i]._trigger("activate", event, this._uiHash(this));
				}
			}

			//Prepare possible droppables
			if ($.ui.ddmanager) {
				$.ui.ddmanager.current = this;
			}

			if ($.ui.ddmanager && !o.dropBehaviour) {
				$.ui.ddmanager.prepareOffsets(this, event);
			}

			this.dragging = true;

			this._addClass(this.helper, "ui-sortable-helper");

			// Execute the drag once - this causes the helper not to be visiblebefore getting its
			// correct position
			this._mouseDrag(event);
			return true;
		},

		_mouseDrag: function _mouseDrag(event) {
			var i,
			    item,
			    itemElement,
			    intersection,
			    o = this.options,
			    scrolled = false;

			//Compute the helpers position
			this.position = this._generatePosition(event);
			this.positionAbs = this._convertPositionTo("absolute");

			if (!this.lastPositionAbs) {
				this.lastPositionAbs = this.positionAbs;
			}

			//Do scrolling
			if (this.options.scroll) {
				if (this.scrollParent[0] !== this.document[0] && this.scrollParent[0].tagName !== "HTML") {

					if (this.overflowOffset.top + this.scrollParent[0].offsetHeight - event.pageY < o.scrollSensitivity) {
						this.scrollParent[0].scrollTop = scrolled = this.scrollParent[0].scrollTop + o.scrollSpeed;
					} else if (event.pageY - this.overflowOffset.top < o.scrollSensitivity) {
						this.scrollParent[0].scrollTop = scrolled = this.scrollParent[0].scrollTop - o.scrollSpeed;
					}

					if (this.overflowOffset.left + this.scrollParent[0].offsetWidth - event.pageX < o.scrollSensitivity) {
						this.scrollParent[0].scrollLeft = scrolled = this.scrollParent[0].scrollLeft + o.scrollSpeed;
					} else if (event.pageX - this.overflowOffset.left < o.scrollSensitivity) {
						this.scrollParent[0].scrollLeft = scrolled = this.scrollParent[0].scrollLeft - o.scrollSpeed;
					}
				} else {

					if (event.pageY - this.document.scrollTop() < o.scrollSensitivity) {
						scrolled = this.document.scrollTop(this.document.scrollTop() - o.scrollSpeed);
					} else if (this.window.height() - (event.pageY - this.document.scrollTop()) < o.scrollSensitivity) {
						scrolled = this.document.scrollTop(this.document.scrollTop() + o.scrollSpeed);
					}

					if (event.pageX - this.document.scrollLeft() < o.scrollSensitivity) {
						scrolled = this.document.scrollLeft(this.document.scrollLeft() - o.scrollSpeed);
					} else if (this.window.width() - (event.pageX - this.document.scrollLeft()) < o.scrollSensitivity) {
						scrolled = this.document.scrollLeft(this.document.scrollLeft() + o.scrollSpeed);
					}
				}

				if (scrolled !== false && $.ui.ddmanager && !o.dropBehaviour) {
					$.ui.ddmanager.prepareOffsets(this, event);
				}
			}

			//Regenerate the absolute position used for position checks
			this.positionAbs = this._convertPositionTo("absolute");

			//Set the helper position
			if (!this.options.axis || this.options.axis !== "y") {
				this.helper[0].style.left = this.position.left + "px";
			}
			if (!this.options.axis || this.options.axis !== "x") {
				this.helper[0].style.top = this.position.top + "px";
			}

			//Rearrange
			for (i = this.items.length - 1; i >= 0; i--) {

				//Cache variables and intersection, continue if no intersection
				item = this.items[i];
				itemElement = item.item[0];
				intersection = this._intersectsWithPointer(item);
				if (!intersection) {
					continue;
				}

				// Only put the placeholder inside the current Container, skip all
				// items from other containers. This works because when moving
				// an item from one container to another the
				// currentContainer is switched before the placeholder is moved.
				//
				// Without this, moving items in "sub-sortables" can cause
				// the placeholder to jitter between the outer and inner container.
				if (item.instance !== this.currentContainer) {
					continue;
				}

				// Cannot intersect with itself
				// no useless actions that have been done before
				// no action if the item moved is the parent of the item checked
				if (itemElement !== this.currentItem[0] && this.placeholder[intersection === 1 ? "next" : "prev"]()[0] !== itemElement && !$.contains(this.placeholder[0], itemElement) && (this.options.type === "semi-dynamic" ? !$.contains(this.element[0], itemElement) : true)) {

					this.direction = intersection === 1 ? "down" : "up";

					if (this.options.tolerance === "pointer" || this._intersectsWithSides(item)) {
						this._rearrange(event, item);
					} else {
						break;
					}

					this._trigger("change", event, this._uiHash());
					break;
				}
			}

			//Post events to containers
			this._contactContainers(event);

			//Interconnect with droppables
			if ($.ui.ddmanager) {
				$.ui.ddmanager.drag(this, event);
			}

			//Call callbacks
			this._trigger("sort", event, this._uiHash());

			this.lastPositionAbs = this.positionAbs;
			return false;
		},

		_mouseStop: function _mouseStop(event, noPropagation) {

			if (!event) {
				return;
			}

			//If we are using droppables, inform the manager about the drop
			if ($.ui.ddmanager && !this.options.dropBehaviour) {
				$.ui.ddmanager.drop(this, event);
			}

			if (this.options.revert) {
				var that = this,
				    cur = this.placeholder.offset(),
				    axis = this.options.axis,
				    animation = {};

				if (!axis || axis === "x") {
					animation.left = cur.left - this.offset.parent.left - this.margins.left + (this.offsetParent[0] === this.document[0].body ? 0 : this.offsetParent[0].scrollLeft);
				}
				if (!axis || axis === "y") {
					animation.top = cur.top - this.offset.parent.top - this.margins.top + (this.offsetParent[0] === this.document[0].body ? 0 : this.offsetParent[0].scrollTop);
				}
				this.reverting = true;
				$(this.helper).animate(animation, parseInt(this.options.revert, 10) || 500, function () {
					that._clear(event);
				});
			} else {
				this._clear(event, noPropagation);
			}

			return false;
		},

		cancel: function cancel() {

			if (this.dragging) {

				this._mouseUp({ target: null });

				if (this.options.helper === "original") {
					this.currentItem.css(this._storedCSS);
					this._removeClass(this.currentItem, "ui-sortable-helper");
				} else {
					this.currentItem.show();
				}

				//Post deactivating events to containers
				for (var i = this.containers.length - 1; i >= 0; i--) {
					this.containers[i]._trigger("deactivate", null, this._uiHash(this));
					if (this.containers[i].containerCache.over) {
						this.containers[i]._trigger("out", null, this._uiHash(this));
						this.containers[i].containerCache.over = 0;
					}
				}
			}

			if (this.placeholder) {

				//$(this.placeholder[0]).remove(); would have been the jQuery way - unfortunately,
				// it unbinds ALL events from the original node!
				if (this.placeholder[0].parentNode) {
					this.placeholder[0].parentNode.removeChild(this.placeholder[0]);
				}
				if (this.options.helper !== "original" && this.helper && this.helper[0].parentNode) {
					this.helper.remove();
				}

				$.extend(this, {
					helper: null,
					dragging: false,
					reverting: false,
					_noFinalSort: null
				});

				if (this.domPosition.prev) {
					$(this.domPosition.prev).after(this.currentItem);
				} else {
					$(this.domPosition.parent).prepend(this.currentItem);
				}
			}

			return this;
		},

		serialize: function serialize(o) {

			var items = this._getItemsAsjQuery(o && o.connected),
			    str = [];
			o = o || {};

			$(items).each(function () {
				var res = ($(o.item || this).attr(o.attribute || "id") || "").match(o.expression || /(.+)[\-=_](.+)/);
				if (res) {
					str.push((o.key || res[1] + "[]") + "=" + (o.key && o.expression ? res[1] : res[2]));
				}
			});

			if (!str.length && o.key) {
				str.push(o.key + "=");
			}

			return str.join("&");
		},

		toArray: function toArray(o) {

			var items = this._getItemsAsjQuery(o && o.connected),
			    ret = [];

			o = o || {};

			items.each(function () {
				ret.push($(o.item || this).attr(o.attribute || "id") || "");
			});
			return ret;
		},

		/* Be careful with the following core functions */
		_intersectsWith: function _intersectsWith(item) {

			var x1 = this.positionAbs.left,
			    x2 = x1 + this.helperProportions.width,
			    y1 = this.positionAbs.top,
			    y2 = y1 + this.helperProportions.height,
			    l = item.left,
			    r = l + item.width,
			    t = item.top,
			    b = t + item.height,
			    dyClick = this.offset.click.top,
			    dxClick = this.offset.click.left,
			    isOverElementHeight = this.options.axis === "x" || y1 + dyClick > t && y1 + dyClick < b,
			    isOverElementWidth = this.options.axis === "y" || x1 + dxClick > l && x1 + dxClick < r,
			    isOverElement = isOverElementHeight && isOverElementWidth;

			if (this.options.tolerance === "pointer" || this.options.forcePointerForContainers || this.options.tolerance !== "pointer" && this.helperProportions[this.floating ? "width" : "height"] > item[this.floating ? "width" : "height"]) {
				return isOverElement;
			} else {

				return l < x1 + this.helperProportions.width / 2 && // Right Half
				x2 - this.helperProportions.width / 2 < r && // Left Half
				t < y1 + this.helperProportions.height / 2 && // Bottom Half
				y2 - this.helperProportions.height / 2 < b; // Top Half
			}
		},

		_intersectsWithPointer: function _intersectsWithPointer(item) {
			var verticalDirection,
			    horizontalDirection,
			    isOverElementHeight = this.options.axis === "x" || this._isOverAxis(this.positionAbs.top + this.offset.click.top, item.top, item.height),
			    isOverElementWidth = this.options.axis === "y" || this._isOverAxis(this.positionAbs.left + this.offset.click.left, item.left, item.width),
			    isOverElement = isOverElementHeight && isOverElementWidth;

			if (!isOverElement) {
				return false;
			}

			verticalDirection = this._getDragVerticalDirection();
			horizontalDirection = this._getDragHorizontalDirection();

			return this.floating ? horizontalDirection === "right" || verticalDirection === "down" ? 2 : 1 : verticalDirection && (verticalDirection === "down" ? 2 : 1);
		},

		_intersectsWithSides: function _intersectsWithSides(item) {

			var isOverBottomHalf = this._isOverAxis(this.positionAbs.top + this.offset.click.top, item.top + item.height / 2, item.height),
			    isOverRightHalf = this._isOverAxis(this.positionAbs.left + this.offset.click.left, item.left + item.width / 2, item.width),
			    verticalDirection = this._getDragVerticalDirection(),
			    horizontalDirection = this._getDragHorizontalDirection();

			if (this.floating && horizontalDirection) {
				return horizontalDirection === "right" && isOverRightHalf || horizontalDirection === "left" && !isOverRightHalf;
			} else {
				return verticalDirection && (verticalDirection === "down" && isOverBottomHalf || verticalDirection === "up" && !isOverBottomHalf);
			}
		},

		_getDragVerticalDirection: function _getDragVerticalDirection() {
			var delta = this.positionAbs.top - this.lastPositionAbs.top;
			return delta !== 0 && (delta > 0 ? "down" : "up");
		},

		_getDragHorizontalDirection: function _getDragHorizontalDirection() {
			var delta = this.positionAbs.left - this.lastPositionAbs.left;
			return delta !== 0 && (delta > 0 ? "right" : "left");
		},

		refresh: function refresh(event) {
			this._refreshItems(event);
			this._setHandleClassName();
			this.refreshPositions();
			return this;
		},

		_connectWith: function _connectWith() {
			var options = this.options;
			return options.connectWith.constructor === String ? [options.connectWith] : options.connectWith;
		},

		_getItemsAsjQuery: function _getItemsAsjQuery(connected) {

			var i,
			    j,
			    cur,
			    inst,
			    items = [],
			    queries = [],
			    connectWith = this._connectWith();

			if (connectWith && connected) {
				for (i = connectWith.length - 1; i >= 0; i--) {
					cur = $(connectWith[i], this.document[0]);
					for (j = cur.length - 1; j >= 0; j--) {
						inst = $.data(cur[j], this.widgetFullName);
						if (inst && inst !== this && !inst.options.disabled) {
							queries.push([$.isFunction(inst.options.items) ? inst.options.items.call(inst.element) : $(inst.options.items, inst.element).not(".ui-sortable-helper").not(".ui-sortable-placeholder"), inst]);
						}
					}
				}
			}

			queries.push([$.isFunction(this.options.items) ? this.options.items.call(this.element, null, { options: this.options, item: this.currentItem }) : $(this.options.items, this.element).not(".ui-sortable-helper").not(".ui-sortable-placeholder"), this]);

			function addItems() {
				items.push(this);
			}
			for (i = queries.length - 1; i >= 0; i--) {
				queries[i][0].each(addItems);
			}

			return $(items);
		},

		_removeCurrentsFromItems: function _removeCurrentsFromItems() {

			var list = this.currentItem.find(":data(" + this.widgetName + "-item)");

			this.items = $.grep(this.items, function (item) {
				for (var j = 0; j < list.length; j++) {
					if (list[j] === item.item[0]) {
						return false;
					}
				}
				return true;
			});
		},

		_refreshItems: function _refreshItems(event) {

			this.items = [];
			this.containers = [this];

			var i,
			    j,
			    cur,
			    inst,
			    targetData,
			    _queries,
			    item,
			    queriesLength,
			    items = this.items,
			    queries = [[$.isFunction(this.options.items) ? this.options.items.call(this.element[0], event, { item: this.currentItem }) : $(this.options.items, this.element), this]],
			    connectWith = this._connectWith();

			//Shouldn't be run the first time through due to massive slow-down
			if (connectWith && this.ready) {
				for (i = connectWith.length - 1; i >= 0; i--) {
					cur = $(connectWith[i], this.document[0]);
					for (j = cur.length - 1; j >= 0; j--) {
						inst = $.data(cur[j], this.widgetFullName);
						if (inst && inst !== this && !inst.options.disabled) {
							queries.push([$.isFunction(inst.options.items) ? inst.options.items.call(inst.element[0], event, { item: this.currentItem }) : $(inst.options.items, inst.element), inst]);
							this.containers.push(inst);
						}
					}
				}
			}

			for (i = queries.length - 1; i >= 0; i--) {
				targetData = queries[i][1];
				_queries = queries[i][0];

				for (j = 0, queriesLength = _queries.length; j < queriesLength; j++) {
					item = $(_queries[j]);

					// Data for target checking (mouse manager)
					item.data(this.widgetName + "-item", targetData);

					items.push({
						item: item,
						instance: targetData,
						width: 0, height: 0,
						left: 0, top: 0
					});
				}
			}
		},

		refreshPositions: function refreshPositions(fast) {

			// Determine whether items are being displayed horizontally
			this.floating = this.items.length ? this.options.axis === "x" || this._isFloating(this.items[0].item) : false;

			//This has to be redone because due to the item being moved out/into the offsetParent,
			// the offsetParent's position will change
			if (this.offsetParent && this.helper) {
				this.offset.parent = this._getParentOffset();
			}

			var i, item, t, p;

			for (i = this.items.length - 1; i >= 0; i--) {
				item = this.items[i];

				//We ignore calculating positions of all connected containers when we're not over them
				if (item.instance !== this.currentContainer && this.currentContainer && item.item[0] !== this.currentItem[0]) {
					continue;
				}

				t = this.options.toleranceElement ? $(this.options.toleranceElement, item.item) : item.item;

				if (!fast) {
					item.width = t.outerWidth();
					item.height = t.outerHeight();
				}

				p = t.offset();
				item.left = p.left;
				item.top = p.top;
			}

			if (this.options.custom && this.options.custom.refreshContainers) {
				this.options.custom.refreshContainers.call(this);
			} else {
				for (i = this.containers.length - 1; i >= 0; i--) {
					p = this.containers[i].element.offset();
					this.containers[i].containerCache.left = p.left;
					this.containers[i].containerCache.top = p.top;
					this.containers[i].containerCache.width = this.containers[i].element.outerWidth();
					this.containers[i].containerCache.height = this.containers[i].element.outerHeight();
				}
			}

			return this;
		},

		_createPlaceholder: function _createPlaceholder(that) {
			that = that || this;
			var className,
			    o = that.options;

			if (!o.placeholder || o.placeholder.constructor === String) {
				className = o.placeholder;
				o.placeholder = {
					element: function element() {

						var nodeName = that.currentItem[0].nodeName.toLowerCase(),
						    element = $("<" + nodeName + ">", that.document[0]);

						that._addClass(element, "ui-sortable-placeholder", className || that.currentItem[0].className)._removeClass(element, "ui-sortable-helper");

						if (nodeName === "tbody") {
							that._createTrPlaceholder(that.currentItem.find("tr").eq(0), $("<tr>", that.document[0]).appendTo(element));
						} else if (nodeName === "tr") {
							that._createTrPlaceholder(that.currentItem, element);
						} else if (nodeName === "img") {
							element.attr("src", that.currentItem.attr("src"));
						}

						if (!className) {
							element.css("visibility", "hidden");
						}

						return element;
					},
					update: function update(container, p) {

						// 1. If a className is set as 'placeholder option, we don't force sizes -
						// the class is responsible for that
						// 2. The option 'forcePlaceholderSize can be enabled to force it even if a
						// class name is specified
						if (className && !o.forcePlaceholderSize) {
							return;
						}

						//If the element doesn't have a actual height by itself (without styles coming
						// from a stylesheet), it receives the inline height from the dragged item
						if (!p.height()) {
							p.height(that.currentItem.innerHeight() - parseInt(that.currentItem.css("paddingTop") || 0, 10) - parseInt(that.currentItem.css("paddingBottom") || 0, 10));
						}
						if (!p.width()) {
							p.width(that.currentItem.innerWidth() - parseInt(that.currentItem.css("paddingLeft") || 0, 10) - parseInt(that.currentItem.css("paddingRight") || 0, 10));
						}
					}
				};
			}

			//Create the placeholder
			that.placeholder = $(o.placeholder.element.call(that.element, that.currentItem));

			//Append it after the actual current item
			that.currentItem.after(that.placeholder);

			//Update the size of the placeholder (TODO: Logic to fuzzy, see line 316/317)
			o.placeholder.update(that, that.placeholder);
		},

		_createTrPlaceholder: function _createTrPlaceholder(sourceTr, targetTr) {
			var that = this;

			sourceTr.children().each(function () {
				$("<td>&#160;</td>", that.document[0]).attr("colspan", $(this).attr("colspan") || 1).appendTo(targetTr);
			});
		},

		_contactContainers: function _contactContainers(event) {
			var i,
			    j,
			    dist,
			    itemWithLeastDistance,
			    posProperty,
			    sizeProperty,
			    cur,
			    nearBottom,
			    floating,
			    axis,
			    innermostContainer = null,
			    innermostIndex = null;

			// Get innermost container that intersects with item
			for (i = this.containers.length - 1; i >= 0; i--) {

				// Never consider a container that's located within the item itself
				if ($.contains(this.currentItem[0], this.containers[i].element[0])) {
					continue;
				}

				if (this._intersectsWith(this.containers[i].containerCache)) {

					// If we've already found a container and it's more "inner" than this, then continue
					if (innermostContainer && $.contains(this.containers[i].element[0], innermostContainer.element[0])) {
						continue;
					}

					innermostContainer = this.containers[i];
					innermostIndex = i;
				} else {

					// container doesn't intersect. trigger "out" event if necessary
					if (this.containers[i].containerCache.over) {
						this.containers[i]._trigger("out", event, this._uiHash(this));
						this.containers[i].containerCache.over = 0;
					}
				}
			}

			// If no intersecting containers found, return
			if (!innermostContainer) {
				return;
			}

			// Move the item into the container if it's not there already
			if (this.containers.length === 1) {
				if (!this.containers[innermostIndex].containerCache.over) {
					this.containers[innermostIndex]._trigger("over", event, this._uiHash(this));
					this.containers[innermostIndex].containerCache.over = 1;
				}
			} else {

				// When entering a new container, we will find the item with the least distance and
				// append our item near it
				dist = 10000;
				itemWithLeastDistance = null;
				floating = innermostContainer.floating || this._isFloating(this.currentItem);
				posProperty = floating ? "left" : "top";
				sizeProperty = floating ? "width" : "height";
				axis = floating ? "pageX" : "pageY";

				for (j = this.items.length - 1; j >= 0; j--) {
					if (!$.contains(this.containers[innermostIndex].element[0], this.items[j].item[0])) {
						continue;
					}
					if (this.items[j].item[0] === this.currentItem[0]) {
						continue;
					}

					cur = this.items[j].item.offset()[posProperty];
					nearBottom = false;
					if (event[axis] - cur > this.items[j][sizeProperty] / 2) {
						nearBottom = true;
					}

					if (Math.abs(event[axis] - cur) < dist) {
						dist = Math.abs(event[axis] - cur);
						itemWithLeastDistance = this.items[j];
						this.direction = nearBottom ? "up" : "down";
					}
				}

				//Check if dropOnEmpty is enabled
				if (!itemWithLeastDistance && !this.options.dropOnEmpty) {
					return;
				}

				if (this.currentContainer === this.containers[innermostIndex]) {
					if (!this.currentContainer.containerCache.over) {
						this.containers[innermostIndex]._trigger("over", event, this._uiHash());
						this.currentContainer.containerCache.over = 1;
					}
					return;
				}

				itemWithLeastDistance ? this._rearrange(event, itemWithLeastDistance, null, true) : this._rearrange(event, null, this.containers[innermostIndex].element, true);
				this._trigger("change", event, this._uiHash());
				this.containers[innermostIndex]._trigger("change", event, this._uiHash(this));
				this.currentContainer = this.containers[innermostIndex];

				//Update the placeholder
				this.options.placeholder.update(this.currentContainer, this.placeholder);

				this.containers[innermostIndex]._trigger("over", event, this._uiHash(this));
				this.containers[innermostIndex].containerCache.over = 1;
			}
		},

		_createHelper: function _createHelper(event) {

			var o = this.options,
			    helper = $.isFunction(o.helper) ? $(o.helper.apply(this.element[0], [event, this.currentItem])) : o.helper === "clone" ? this.currentItem.clone() : this.currentItem;

			//Add the helper to the DOM if that didn't happen already
			if (!helper.parents("body").length) {
				$(o.appendTo !== "parent" ? o.appendTo : this.currentItem[0].parentNode)[0].appendChild(helper[0]);
			}

			if (helper[0] === this.currentItem[0]) {
				this._storedCSS = {
					width: this.currentItem[0].style.width,
					height: this.currentItem[0].style.height,
					position: this.currentItem.css("position"),
					top: this.currentItem.css("top"),
					left: this.currentItem.css("left")
				};
			}

			if (!helper[0].style.width || o.forceHelperSize) {
				helper.width(this.currentItem.width());
			}
			if (!helper[0].style.height || o.forceHelperSize) {
				helper.height(this.currentItem.height());
			}

			return helper;
		},

		_adjustOffsetFromHelper: function _adjustOffsetFromHelper(obj) {
			if (typeof obj === "string") {
				obj = obj.split(" ");
			}
			if ($.isArray(obj)) {
				obj = { left: +obj[0], top: +obj[1] || 0 };
			}
			if ("left" in obj) {
				this.offset.click.left = obj.left + this.margins.left;
			}
			if ("right" in obj) {
				this.offset.click.left = this.helperProportions.width - obj.right + this.margins.left;
			}
			if ("top" in obj) {
				this.offset.click.top = obj.top + this.margins.top;
			}
			if ("bottom" in obj) {
				this.offset.click.top = this.helperProportions.height - obj.bottom + this.margins.top;
			}
		},

		_getParentOffset: function _getParentOffset() {

			//Get the offsetParent and cache its position
			this.offsetParent = this.helper.offsetParent();
			var po = this.offsetParent.offset();

			// This is a special case where we need to modify a offset calculated on start, since the
			// following happened:
			// 1. The position of the helper is absolute, so it's position is calculated based on the
			// next positioned parent
			// 2. The actual offset parent is a child of the scroll parent, and the scroll parent isn't
			// the document, which means that the scroll is included in the initial calculation of the
			// offset of the parent, and never recalculated upon drag
			if (this.cssPosition === "absolute" && this.scrollParent[0] !== this.document[0] && $.contains(this.scrollParent[0], this.offsetParent[0])) {
				po.left += this.scrollParent.scrollLeft();
				po.top += this.scrollParent.scrollTop();
			}

			// This needs to be actually done for all browsers, since pageX/pageY includes this
			// information with an ugly IE fix
			if (this.offsetParent[0] === this.document[0].body || this.offsetParent[0].tagName && this.offsetParent[0].tagName.toLowerCase() === "html" && $.ui.ie) {
				po = { top: 0, left: 0 };
			}

			return {
				top: po.top + (parseInt(this.offsetParent.css("borderTopWidth"), 10) || 0),
				left: po.left + (parseInt(this.offsetParent.css("borderLeftWidth"), 10) || 0)
			};
		},

		_getRelativeOffset: function _getRelativeOffset() {

			if (this.cssPosition === "relative") {
				var p = this.currentItem.position();
				return {
					top: p.top - (parseInt(this.helper.css("top"), 10) || 0) + this.scrollParent.scrollTop(),
					left: p.left - (parseInt(this.helper.css("left"), 10) || 0) + this.scrollParent.scrollLeft()
				};
			} else {
				return { top: 0, left: 0 };
			}
		},

		_cacheMargins: function _cacheMargins() {
			this.margins = {
				left: parseInt(this.currentItem.css("marginLeft"), 10) || 0,
				top: parseInt(this.currentItem.css("marginTop"), 10) || 0
			};
		},

		_cacheHelperProportions: function _cacheHelperProportions() {
			this.helperProportions = {
				width: this.helper.outerWidth(),
				height: this.helper.outerHeight()
			};
		},

		_setContainment: function _setContainment() {

			var ce,
			    co,
			    over,
			    o = this.options;
			if (o.containment === "parent") {
				o.containment = this.helper[0].parentNode;
			}
			if (o.containment === "document" || o.containment === "window") {
				this.containment = [0 - this.offset.relative.left - this.offset.parent.left, 0 - this.offset.relative.top - this.offset.parent.top, o.containment === "document" ? this.document.width() : this.window.width() - this.helperProportions.width - this.margins.left, (o.containment === "document" ? this.document.height() || document.body.parentNode.scrollHeight : this.window.height() || this.document[0].body.parentNode.scrollHeight) - this.helperProportions.height - this.margins.top];
			}

			if (!/^(document|window|parent)$/.test(o.containment)) {
				ce = $(o.containment)[0];
				co = $(o.containment).offset();
				over = $(ce).css("overflow") !== "hidden";

				this.containment = [co.left + (parseInt($(ce).css("borderLeftWidth"), 10) || 0) + (parseInt($(ce).css("paddingLeft"), 10) || 0) - this.margins.left, co.top + (parseInt($(ce).css("borderTopWidth"), 10) || 0) + (parseInt($(ce).css("paddingTop"), 10) || 0) - this.margins.top, co.left + (over ? Math.max(ce.scrollWidth, ce.offsetWidth) : ce.offsetWidth) - (parseInt($(ce).css("borderLeftWidth"), 10) || 0) - (parseInt($(ce).css("paddingRight"), 10) || 0) - this.helperProportions.width - this.margins.left, co.top + (over ? Math.max(ce.scrollHeight, ce.offsetHeight) : ce.offsetHeight) - (parseInt($(ce).css("borderTopWidth"), 10) || 0) - (parseInt($(ce).css("paddingBottom"), 10) || 0) - this.helperProportions.height - this.margins.top];
			}
		},

		_convertPositionTo: function _convertPositionTo(d, pos) {

			if (!pos) {
				pos = this.position;
			}
			var mod = d === "absolute" ? 1 : -1,
			    scroll = this.cssPosition === "absolute" && !(this.scrollParent[0] !== this.document[0] && $.contains(this.scrollParent[0], this.offsetParent[0])) ? this.offsetParent : this.scrollParent,
			    scrollIsRootNode = /(html|body)/i.test(scroll[0].tagName);

			return {
				top:

				// The absolute mouse position
				pos.top +

				// Only for relative positioned nodes: Relative offset from element to offset parent
				this.offset.relative.top * mod +

				// The offsetParent's offset without borders (offset + border)
				this.offset.parent.top * mod - (this.cssPosition === "fixed" ? -this.scrollParent.scrollTop() : scrollIsRootNode ? 0 : scroll.scrollTop()) * mod,
				left:

				// The absolute mouse position
				pos.left +

				// Only for relative positioned nodes: Relative offset from element to offset parent
				this.offset.relative.left * mod +

				// The offsetParent's offset without borders (offset + border)
				this.offset.parent.left * mod - (this.cssPosition === "fixed" ? -this.scrollParent.scrollLeft() : scrollIsRootNode ? 0 : scroll.scrollLeft()) * mod
			};
		},

		_generatePosition: function _generatePosition(event) {

			var top,
			    left,
			    o = this.options,
			    pageX = event.pageX,
			    pageY = event.pageY,
			    scroll = this.cssPosition === "absolute" && !(this.scrollParent[0] !== this.document[0] && $.contains(this.scrollParent[0], this.offsetParent[0])) ? this.offsetParent : this.scrollParent,
			    scrollIsRootNode = /(html|body)/i.test(scroll[0].tagName);

			// This is another very weird special case that only happens for relative elements:
			// 1. If the css position is relative
			// 2. and the scroll parent is the document or similar to the offset parent
			// we have to refresh the relative offset during the scroll so there are no jumps
			if (this.cssPosition === "relative" && !(this.scrollParent[0] !== this.document[0] && this.scrollParent[0] !== this.offsetParent[0])) {
				this.offset.relative = this._getRelativeOffset();
			}

			/*
    * - Position constraining -
    * Constrain the position to a mix of grid, containment.
    */

			if (this.originalPosition) {
				//If we are not dragging yet, we won't check for options

				if (this.containment) {
					if (event.pageX - this.offset.click.left < this.containment[0]) {
						pageX = this.containment[0] + this.offset.click.left;
					}
					if (event.pageY - this.offset.click.top < this.containment[1]) {
						pageY = this.containment[1] + this.offset.click.top;
					}
					if (event.pageX - this.offset.click.left > this.containment[2]) {
						pageX = this.containment[2] + this.offset.click.left;
					}
					if (event.pageY - this.offset.click.top > this.containment[3]) {
						pageY = this.containment[3] + this.offset.click.top;
					}
				}

				if (o.grid) {
					top = this.originalPageY + Math.round((pageY - this.originalPageY) / o.grid[1]) * o.grid[1];
					pageY = this.containment ? top - this.offset.click.top >= this.containment[1] && top - this.offset.click.top <= this.containment[3] ? top : top - this.offset.click.top >= this.containment[1] ? top - o.grid[1] : top + o.grid[1] : top;

					left = this.originalPageX + Math.round((pageX - this.originalPageX) / o.grid[0]) * o.grid[0];
					pageX = this.containment ? left - this.offset.click.left >= this.containment[0] && left - this.offset.click.left <= this.containment[2] ? left : left - this.offset.click.left >= this.containment[0] ? left - o.grid[0] : left + o.grid[0] : left;
				}
			}

			return {
				top:

				// The absolute mouse position
				pageY -

				// Click offset (relative to the element)
				this.offset.click.top -

				// Only for relative positioned nodes: Relative offset from element to offset parent
				this.offset.relative.top -

				// The offsetParent's offset without borders (offset + border)
				this.offset.parent.top + (this.cssPosition === "fixed" ? -this.scrollParent.scrollTop() : scrollIsRootNode ? 0 : scroll.scrollTop()),
				left:

				// The absolute mouse position
				pageX -

				// Click offset (relative to the element)
				this.offset.click.left -

				// Only for relative positioned nodes: Relative offset from element to offset parent
				this.offset.relative.left -

				// The offsetParent's offset without borders (offset + border)
				this.offset.parent.left + (this.cssPosition === "fixed" ? -this.scrollParent.scrollLeft() : scrollIsRootNode ? 0 : scroll.scrollLeft())
			};
		},

		_rearrange: function _rearrange(event, i, a, hardRefresh) {

			a ? a[0].appendChild(this.placeholder[0]) : i.item[0].parentNode.insertBefore(this.placeholder[0], this.direction === "down" ? i.item[0] : i.item[0].nextSibling);

			//Various things done here to improve the performance:
			// 1. we create a setTimeout, that calls refreshPositions
			// 2. on the instance, we have a counter variable, that get's higher after every append
			// 3. on the local scope, we copy the counter variable, and check in the timeout,
			// if it's still the same
			// 4. this lets only the last addition to the timeout stack through
			this.counter = this.counter ? ++this.counter : 1;
			var counter = this.counter;

			this._delay(function () {
				if (counter === this.counter) {

					//Precompute after each DOM insertion, NOT on mousemove
					this.refreshPositions(!hardRefresh);
				}
			});
		},

		_clear: function _clear(event, noPropagation) {

			this.reverting = false;

			// We delay all events that have to be triggered to after the point where the placeholder
			// has been removed and everything else normalized again
			var i,
			    delayedTriggers = [];

			// We first have to update the dom position of the actual currentItem
			// Note: don't do it if the current item is already removed (by a user), or it gets
			// reappended (see #4088)
			if (!this._noFinalSort && this.currentItem.parent().length) {
				this.placeholder.before(this.currentItem);
			}
			this._noFinalSort = null;

			if (this.helper[0] === this.currentItem[0]) {
				for (i in this._storedCSS) {
					if (this._storedCSS[i] === "auto" || this._storedCSS[i] === "static") {
						this._storedCSS[i] = "";
					}
				}
				this.currentItem.css(this._storedCSS);
				this._removeClass(this.currentItem, "ui-sortable-helper");
			} else {
				this.currentItem.show();
			}

			if (this.fromOutside && !noPropagation) {
				delayedTriggers.push(function (event) {
					this._trigger("receive", event, this._uiHash(this.fromOutside));
				});
			}
			if ((this.fromOutside || this.domPosition.prev !== this.currentItem.prev().not(".ui-sortable-helper")[0] || this.domPosition.parent !== this.currentItem.parent()[0]) && !noPropagation) {

				// Trigger update callback if the DOM position has changed
				delayedTriggers.push(function (event) {
					this._trigger("update", event, this._uiHash());
				});
			}

			// Check if the items Container has Changed and trigger appropriate
			// events.
			if (this !== this.currentContainer) {
				if (!noPropagation) {
					delayedTriggers.push(function (event) {
						this._trigger("remove", event, this._uiHash());
					});
					delayedTriggers.push(function (c) {
						return function (event) {
							c._trigger("receive", event, this._uiHash(this));
						};
					}.call(this, this.currentContainer));
					delayedTriggers.push(function (c) {
						return function (event) {
							c._trigger("update", event, this._uiHash(this));
						};
					}.call(this, this.currentContainer));
				}
			}

			//Post events to containers
			function delayEvent(type, instance, container) {
				return function (event) {
					container._trigger(type, event, instance._uiHash(instance));
				};
			}
			for (i = this.containers.length - 1; i >= 0; i--) {
				if (!noPropagation) {
					delayedTriggers.push(delayEvent("deactivate", this, this.containers[i]));
				}
				if (this.containers[i].containerCache.over) {
					delayedTriggers.push(delayEvent("out", this, this.containers[i]));
					this.containers[i].containerCache.over = 0;
				}
			}

			//Do what was originally in plugins
			if (this.storedCursor) {
				this.document.find("body").css("cursor", this.storedCursor);
				this.storedStylesheet.remove();
			}
			if (this._storedOpacity) {
				this.helper.css("opacity", this._storedOpacity);
			}
			if (this._storedZIndex) {
				this.helper.css("zIndex", this._storedZIndex === "auto" ? "" : this._storedZIndex);
			}

			this.dragging = false;

			if (!noPropagation) {
				this._trigger("beforeStop", event, this._uiHash());
			}

			//$(this.placeholder[0]).remove(); would have been the jQuery way - unfortunately,
			// it unbinds ALL events from the original node!
			this.placeholder[0].parentNode.removeChild(this.placeholder[0]);

			if (!this.cancelHelperRemoval) {
				if (this.helper[0] !== this.currentItem[0]) {
					this.helper.remove();
				}
				this.helper = null;
			}

			if (!noPropagation) {
				for (i = 0; i < delayedTriggers.length; i++) {

					// Trigger all delayed events
					delayedTriggers[i].call(this, event);
				}
				this._trigger("stop", event, this._uiHash());
			}

			this.fromOutside = false;
			return !this.cancelHelperRemoval;
		},

		_trigger: function _trigger() {
			if ($.Widget.prototype._trigger.apply(this, arguments) === false) {
				this.cancel();
			}
		},

		_uiHash: function _uiHash(_inst) {
			var inst = _inst || this;
			return {
				helper: inst.helper,
				placeholder: inst.placeholder || $([]),
				position: inst.position,
				originalPosition: inst.originalPosition,
				offset: inst.positionAbs,
				item: inst.currentItem,
				sender: _inst ? _inst.element : null
			};
		}

	});

	// jscs:disable maximumLineLength
	/* jscs:disable requireCamelCaseOrUpperCaseIdentifiers */
	/*!
  * jQuery UI Datepicker 1.12.0
  * http://jqueryui.com
  *
  * Copyright jQuery Foundation and other contributors
  * Released under the MIT license.
  * http://jquery.org/license
  */

	//>>label: Datepicker
	//>>group: Widgets
	//>>description: Displays a calendar from an input or inline for selecting dates.
	//>>docs: http://api.jqueryui.com/datepicker/
	//>>demos: http://jqueryui.com/datepicker/
	//>>css.structure: ../../themes/base/core.css
	//>>css.structure: ../../themes/base/datepicker.css
	//>>css.theme: ../../themes/base/theme.css


	$.extend($.ui, { datepicker: { version: "1.12.0" } });

	var datepicker_instActive;

	function datepicker_getZindex(elem) {
		var position, value;
		while (elem.length && elem[0] !== document) {

			// Ignore z-index if position is set to a value where z-index is ignored by the browser
			// This makes behavior of this function consistent across browsers
			// WebKit always returns auto if the element is positioned
			position = elem.css("position");
			if (position === "absolute" || position === "relative" || position === "fixed") {

				// IE returns 0 when zIndex is not specified
				// other browsers return a string
				// we ignore the case of nested elements with an explicit value of 0
				// <div style="z-index: -10;"><div style="z-index: 0;"></div></div>
				value = parseInt(elem.css("zIndex"), 10);
				if (!isNaN(value) && value !== 0) {
					return value;
				}
			}
			elem = elem.parent();
		}

		return 0;
	}
	/* Date picker manager.
    Use the singleton instance of this class, $.datepicker, to interact with the date picker.
    Settings for (groups of) date pickers are maintained in an instance object,
    allowing multiple different settings on the same page. */

	function Datepicker() {
		this._curInst = null; // The current instance in use
		this._keyEvent = false; // If the last event was a key event
		this._disabledInputs = []; // List of date picker inputs that have been disabled
		this._datepickerShowing = false; // True if the popup picker is showing , false if not
		this._inDialog = false; // True if showing within a "dialog", false if not
		this._mainDivId = "ui-datepicker-div"; // The ID of the main datepicker division
		this._inlineClass = "ui-datepicker-inline"; // The name of the inline marker class
		this._appendClass = "ui-datepicker-append"; // The name of the append marker class
		this._triggerClass = "ui-datepicker-trigger"; // The name of the trigger marker class
		this._dialogClass = "ui-datepicker-dialog"; // The name of the dialog marker class
		this._disableClass = "ui-datepicker-disabled"; // The name of the disabled covering marker class
		this._unselectableClass = "ui-datepicker-unselectable"; // The name of the unselectable cell marker class
		this._currentClass = "ui-datepicker-current-day"; // The name of the current day marker class
		this._dayOverClass = "ui-datepicker-days-cell-over"; // The name of the day hover marker class
		this.regional = []; // Available regional settings, indexed by language code
		this.regional[""] = { // Default regional settings
			closeText: "Done", // Display text for close link
			prevText: "Prev", // Display text for previous month link
			nextText: "Next", // Display text for next month link
			currentText: "Today", // Display text for current month link
			monthNames: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"], // Names of months for drop-down and formatting
			monthNamesShort: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"], // For formatting
			dayNames: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"], // For formatting
			dayNamesShort: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"], // For formatting
			dayNamesMin: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"], // Column headings for days starting at Sunday
			weekHeader: "Wk", // Column header for week of the year
			dateFormat: "mm/dd/yy", // See format options on parseDate
			firstDay: 0, // The first day of the week, Sun = 0, Mon = 1, ...
			isRTL: false, // True if right-to-left language, false if left-to-right
			showMonthAfterYear: false, // True if the year select precedes month, false for month then year
			yearSuffix: "" // Additional text to append to the year in the month headers
		};
		this._defaults = { // Global defaults for all the date picker instances
			showOn: "focus", // "focus" for popup on focus,
			// "button" for trigger button, or "both" for either
			showAnim: "fadeIn", // Name of jQuery animation for popup
			showOptions: {}, // Options for enhanced animations
			defaultDate: null, // Used when field is blank: actual date,
			// +/-number for offset from today, null for today
			appendText: "", // Display text following the input box, e.g. showing the format
			buttonText: "...", // Text for trigger button
			buttonImage: "", // URL for trigger button image
			buttonImageOnly: false, // True if the image appears alone, false if it appears on a button
			hideIfNoPrevNext: false, // True to hide next/previous month links
			// if not applicable, false to just disable them
			navigationAsDateFormat: false, // True if date formatting applied to prev/today/next links
			gotoCurrent: false, // True if today link goes back to current selection instead
			changeMonth: false, // True if month can be selected directly, false if only prev/next
			changeYear: false, // True if year can be selected directly, false if only prev/next
			yearRange: "c-10:c+10", // Range of years to display in drop-down,
			// either relative to today's year (-nn:+nn), relative to currently displayed year
			// (c-nn:c+nn), absolute (nnnn:nnnn), or a combination of the above (nnnn:-n)
			showOtherMonths: false, // True to show dates in other months, false to leave blank
			selectOtherMonths: false, // True to allow selection of dates in other months, false for unselectable
			showWeek: false, // True to show week of the year, false to not show it
			calculateWeek: this.iso8601Week, // How to calculate the week of the year,
			// takes a Date and returns the number of the week for it
			shortYearCutoff: "+10", // Short year values < this are in the current century,
			// > this are in the previous century,
			// string value starting with "+" for current year + value
			minDate: null, // The earliest selectable date, or null for no limit
			maxDate: null, // The latest selectable date, or null for no limit
			duration: "fast", // Duration of display/closure
			beforeShowDay: null, // Function that takes a date and returns an array with
			// [0] = true if selectable, false if not, [1] = custom CSS class name(s) or "",
			// [2] = cell title (optional), e.g. $.datepicker.noWeekends
			beforeShow: null, // Function that takes an input field and
			// returns a set of custom settings for the date picker
			onSelect: null, // Define a callback function when a date is selected
			onChangeMonthYear: null, // Define a callback function when the month or year is changed
			onClose: null, // Define a callback function when the datepicker is closed
			numberOfMonths: 1, // Number of months to show at a time
			showCurrentAtPos: 0, // The position in multipe months at which to show the current month (starting at 0)
			stepMonths: 1, // Number of months to step back/forward
			stepBigMonths: 12, // Number of months to step back/forward for the big links
			altField: "", // Selector for an alternate field to store selected dates into
			altFormat: "", // The date format to use for the alternate field
			constrainInput: true, // The input is constrained by the current date format
			showButtonPanel: false, // True to show button panel, false to not show it
			autoSize: false, // True to size the input for the date format, false to leave as is
			disabled: false // The initial disabled state
		};
		$.extend(this._defaults, this.regional[""]);
		this.regional.en = $.extend(true, {}, this.regional[""]);
		this.regional["en-US"] = $.extend(true, {}, this.regional.en);
		this.dpDiv = datepicker_bindHover($("<div id='" + this._mainDivId + "' class='ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all'></div>"));
	}

	$.extend(Datepicker.prototype, {
		/* Class name added to elements to indicate already configured with a date picker. */
		markerClassName: "hasDatepicker",

		//Keep track of the maximum number of rows displayed (see #7043)
		maxRows: 4,

		// TODO rename to "widget" when switching to widget factory
		_widgetDatepicker: function _widgetDatepicker() {
			return this.dpDiv;
		},

		/* Override the default settings for all instances of the date picker.
   * @param  settings  object - the new settings to use as defaults (anonymous object)
   * @return the manager object
   */
		setDefaults: function setDefaults(settings) {
			datepicker_extendRemove(this._defaults, settings || {});
			return this;
		},

		/* Attach the date picker to a jQuery selection.
   * @param  target	element - the target input field or division or span
   * @param  settings  object - the new settings to use for this date picker instance (anonymous)
   */
		_attachDatepicker: function _attachDatepicker(target, settings) {
			var nodeName, inline, inst;
			nodeName = target.nodeName.toLowerCase();
			inline = nodeName === "div" || nodeName === "span";
			if (!target.id) {
				this.uuid += 1;
				target.id = "dp" + this.uuid;
			}
			inst = this._newInst($(target), inline);
			inst.settings = $.extend({}, settings || {});
			if (nodeName === "input") {
				this._connectDatepicker(target, inst);
			} else if (inline) {
				this._inlineDatepicker(target, inst);
			}
		},

		/* Create a new instance object. */
		_newInst: function _newInst(target, inline) {
			var id = target[0].id.replace(/([^A-Za-z0-9_\-])/g, "\\\\$1"); // escape jQuery meta chars
			return { id: id, input: target, // associated target
				selectedDay: 0, selectedMonth: 0, selectedYear: 0, // current selection
				drawMonth: 0, drawYear: 0, // month being drawn
				inline: inline, // is datepicker inline or not
				dpDiv: !inline ? this.dpDiv : // presentation div
				datepicker_bindHover($("<div class='" + this._inlineClass + " ui-datepicker ui-widget ui-widget-content ui-helper-clearfix ui-corner-all'></div>")) };
		},

		/* Attach the date picker to an input field. */
		_connectDatepicker: function _connectDatepicker(target, inst) {
			var input = $(target);
			inst.append = $([]);
			inst.trigger = $([]);
			if (input.hasClass(this.markerClassName)) {
				return;
			}
			this._attachments(input, inst);
			input.addClass(this.markerClassName).on("keydown", this._doKeyDown).on("keypress", this._doKeyPress).on("keyup", this._doKeyUp);
			this._autoSize(inst);
			$.data(target, "datepicker", inst);

			//If disabled option is true, disable the datepicker once it has been attached to the input (see ticket #5665)
			if (inst.settings.disabled) {
				this._disableDatepicker(target);
			}
		},

		/* Make attachments based on settings. */
		_attachments: function _attachments(input, inst) {
			var showOn,
			    buttonText,
			    buttonImage,
			    appendText = this._get(inst, "appendText"),
			    isRTL = this._get(inst, "isRTL");

			if (inst.append) {
				inst.append.remove();
			}
			if (appendText) {
				inst.append = $("<span class='" + this._appendClass + "'>" + appendText + "</span>");
				input[isRTL ? "before" : "after"](inst.append);
			}

			input.off("focus", this._showDatepicker);

			if (inst.trigger) {
				inst.trigger.remove();
			}

			showOn = this._get(inst, "showOn");
			if (showOn === "focus" || showOn === "both") {
				// pop-up date picker when in the marked field
				input.on("focus", this._showDatepicker);
			}
			if (showOn === "button" || showOn === "both") {
				// pop-up date picker when button clicked
				buttonText = this._get(inst, "buttonText");
				buttonImage = this._get(inst, "buttonImage");
				inst.trigger = $(this._get(inst, "buttonImageOnly") ? $("<img/>").addClass(this._triggerClass).attr({ src: buttonImage, alt: buttonText, title: buttonText }) : $("<button type='button'></button>").addClass(this._triggerClass).html(!buttonImage ? buttonText : $("<img/>").attr({ src: buttonImage, alt: buttonText, title: buttonText })));
				input[isRTL ? "before" : "after"](inst.trigger);
				inst.trigger.on("click", function () {
					if ($.datepicker._datepickerShowing && $.datepicker._lastInput === input[0]) {
						$.datepicker._hideDatepicker();
					} else if ($.datepicker._datepickerShowing && $.datepicker._lastInput !== input[0]) {
						$.datepicker._hideDatepicker();
						$.datepicker._showDatepicker(input[0]);
					} else {
						$.datepicker._showDatepicker(input[0]);
					}
					return false;
				});
			}
		},

		/* Apply the maximum length for the date format. */
		_autoSize: function _autoSize(inst) {
			if (this._get(inst, "autoSize") && !inst.inline) {
				var findMax,
				    max,
				    maxI,
				    i,
				    date = new Date(2009, 12 - 1, 20),
				    // Ensure double digits
				dateFormat = this._get(inst, "dateFormat");

				if (dateFormat.match(/[DM]/)) {
					findMax = function findMax(names) {
						max = 0;
						maxI = 0;
						for (i = 0; i < names.length; i++) {
							if (names[i].length > max) {
								max = names[i].length;
								maxI = i;
							}
						}
						return maxI;
					};
					date.setMonth(findMax(this._get(inst, dateFormat.match(/MM/) ? "monthNames" : "monthNamesShort")));
					date.setDate(findMax(this._get(inst, dateFormat.match(/DD/) ? "dayNames" : "dayNamesShort")) + 20 - date.getDay());
				}
				inst.input.attr("size", this._formatDate(inst, date).length);
			}
		},

		/* Attach an inline date picker to a div. */
		_inlineDatepicker: function _inlineDatepicker(target, inst) {
			var divSpan = $(target);
			if (divSpan.hasClass(this.markerClassName)) {
				return;
			}
			divSpan.addClass(this.markerClassName).append(inst.dpDiv);
			$.data(target, "datepicker", inst);
			this._setDate(inst, this._getDefaultDate(inst), true);
			this._updateDatepicker(inst);
			this._updateAlternate(inst);

			//If disabled option is true, disable the datepicker before showing it (see ticket #5665)
			if (inst.settings.disabled) {
				this._disableDatepicker(target);
			}

			// Set display:block in place of inst.dpDiv.show() which won't work on disconnected elements
			// http://bugs.jqueryui.com/ticket/7552 - A Datepicker created on a detached div has zero height
			inst.dpDiv.css("display", "block");
		},

		/* Pop-up the date picker in a "dialog" box.
   * @param  input element - ignored
   * @param  date	string or Date - the initial date to display
   * @param  onSelect  function - the function to call when a date is selected
   * @param  settings  object - update the dialog date picker instance's settings (anonymous object)
   * @param  pos int[2] - coordinates for the dialog's position within the screen or
   *					event - with x/y coordinates or
   *					leave empty for default (screen centre)
   * @return the manager object
   */
		_dialogDatepicker: function _dialogDatepicker(input, date, onSelect, settings, pos) {
			var id,
			    browserWidth,
			    browserHeight,
			    scrollX,
			    scrollY,
			    inst = this._dialogInst; // internal instance

			if (!inst) {
				this.uuid += 1;
				id = "dp" + this.uuid;
				this._dialogInput = $("<input type='text' id='" + id + "' style='position: absolute; top: -100px; width: 0px;'/>");
				this._dialogInput.on("keydown", this._doKeyDown);
				$("body").append(this._dialogInput);
				inst = this._dialogInst = this._newInst(this._dialogInput, false);
				inst.settings = {};
				$.data(this._dialogInput[0], "datepicker", inst);
			}
			datepicker_extendRemove(inst.settings, settings || {});
			date = date && date.constructor === Date ? this._formatDate(inst, date) : date;
			this._dialogInput.val(date);

			this._pos = pos ? pos.length ? pos : [pos.pageX, pos.pageY] : null;
			if (!this._pos) {
				browserWidth = document.documentElement.clientWidth;
				browserHeight = document.documentElement.clientHeight;
				scrollX = document.documentElement.scrollLeft || document.body.scrollLeft;
				scrollY = document.documentElement.scrollTop || document.body.scrollTop;
				this._pos = // should use actual width/height below
				[browserWidth / 2 - 100 + scrollX, browserHeight / 2 - 150 + scrollY];
			}

			// Move input on screen for focus, but hidden behind dialog
			this._dialogInput.css("left", this._pos[0] + 20 + "px").css("top", this._pos[1] + "px");
			inst.settings.onSelect = onSelect;
			this._inDialog = true;
			this.dpDiv.addClass(this._dialogClass);
			this._showDatepicker(this._dialogInput[0]);
			if ($.blockUI) {
				$.blockUI(this.dpDiv);
			}
			$.data(this._dialogInput[0], "datepicker", inst);
			return this;
		},

		/* Detach a datepicker from its control.
   * @param  target	element - the target input field or division or span
   */
		_destroyDatepicker: function _destroyDatepicker(target) {
			var nodeName,
			    $target = $(target),
			    inst = $.data(target, "datepicker");

			if (!$target.hasClass(this.markerClassName)) {
				return;
			}

			nodeName = target.nodeName.toLowerCase();
			$.removeData(target, "datepicker");
			if (nodeName === "input") {
				inst.append.remove();
				inst.trigger.remove();
				$target.removeClass(this.markerClassName).off("focus", this._showDatepicker).off("keydown", this._doKeyDown).off("keypress", this._doKeyPress).off("keyup", this._doKeyUp);
			} else if (nodeName === "div" || nodeName === "span") {
				$target.removeClass(this.markerClassName).empty();
			}

			if (datepicker_instActive === inst) {
				datepicker_instActive = null;
			}
		},

		/* Enable the date picker to a jQuery selection.
   * @param  target	element - the target input field or division or span
   */
		_enableDatepicker: function _enableDatepicker(target) {
			var nodeName,
			    inline,
			    $target = $(target),
			    inst = $.data(target, "datepicker");

			if (!$target.hasClass(this.markerClassName)) {
				return;
			}

			nodeName = target.nodeName.toLowerCase();
			if (nodeName === "input") {
				target.disabled = false;
				inst.trigger.filter("button").each(function () {
					this.disabled = false;
				}).end().filter("img").css({ opacity: "1.0", cursor: "" });
			} else if (nodeName === "div" || nodeName === "span") {
				inline = $target.children("." + this._inlineClass);
				inline.children().removeClass("ui-state-disabled");
				inline.find("select.ui-datepicker-month, select.ui-datepicker-year").prop("disabled", false);
			}
			this._disabledInputs = $.map(this._disabledInputs, function (value) {
				return value === target ? null : value;
			}); // delete entry
		},

		/* Disable the date picker to a jQuery selection.
   * @param  target	element - the target input field or division or span
   */
		_disableDatepicker: function _disableDatepicker(target) {
			var nodeName,
			    inline,
			    $target = $(target),
			    inst = $.data(target, "datepicker");

			if (!$target.hasClass(this.markerClassName)) {
				return;
			}

			nodeName = target.nodeName.toLowerCase();
			if (nodeName === "input") {
				target.disabled = true;
				inst.trigger.filter("button").each(function () {
					this.disabled = true;
				}).end().filter("img").css({ opacity: "0.5", cursor: "default" });
			} else if (nodeName === "div" || nodeName === "span") {
				inline = $target.children("." + this._inlineClass);
				inline.children().addClass("ui-state-disabled");
				inline.find("select.ui-datepicker-month, select.ui-datepicker-year").prop("disabled", true);
			}
			this._disabledInputs = $.map(this._disabledInputs, function (value) {
				return value === target ? null : value;
			}); // delete entry
			this._disabledInputs[this._disabledInputs.length] = target;
		},

		/* Is the first field in a jQuery collection disabled as a datepicker?
   * @param  target	element - the target input field or division or span
   * @return boolean - true if disabled, false if enabled
   */
		_isDisabledDatepicker: function _isDisabledDatepicker(target) {
			if (!target) {
				return false;
			}
			for (var i = 0; i < this._disabledInputs.length; i++) {
				if (this._disabledInputs[i] === target) {
					return true;
				}
			}
			return false;
		},

		/* Retrieve the instance data for the target control.
   * @param  target  element - the target input field or division or span
   * @return  object - the associated instance data
   * @throws  error if a jQuery problem getting data
   */
		_getInst: function _getInst(target) {
			try {
				return $.data(target, "datepicker");
			} catch (err) {
				throw "Missing instance data for this datepicker";
			}
		},

		/* Update or retrieve the settings for a date picker attached to an input field or division.
   * @param  target  element - the target input field or division or span
   * @param  name	object - the new settings to update or
   *				string - the name of the setting to change or retrieve,
   *				when retrieving also "all" for all instance settings or
   *				"defaults" for all global defaults
   * @param  value   any - the new value for the setting
   *				(omit if above is an object or to retrieve a value)
   */
		_optionDatepicker: function _optionDatepicker(target, name, value) {
			var settings,
			    date,
			    minDate,
			    maxDate,
			    inst = this._getInst(target);

			if (arguments.length === 2 && typeof name === "string") {
				return name === "defaults" ? $.extend({}, $.datepicker._defaults) : inst ? name === "all" ? $.extend({}, inst.settings) : this._get(inst, name) : null;
			}

			settings = name || {};
			if (typeof name === "string") {
				settings = {};
				settings[name] = value;
			}

			if (inst) {
				if (this._curInst === inst) {
					this._hideDatepicker();
				}

				date = this._getDateDatepicker(target, true);
				minDate = this._getMinMaxDate(inst, "min");
				maxDate = this._getMinMaxDate(inst, "max");
				datepicker_extendRemove(inst.settings, settings);

				// reformat the old minDate/maxDate values if dateFormat changes and a new minDate/maxDate isn't provided
				if (minDate !== null && settings.dateFormat !== undefined && settings.minDate === undefined) {
					inst.settings.minDate = this._formatDate(inst, minDate);
				}
				if (maxDate !== null && settings.dateFormat !== undefined && settings.maxDate === undefined) {
					inst.settings.maxDate = this._formatDate(inst, maxDate);
				}
				if ("disabled" in settings) {
					if (settings.disabled) {
						this._disableDatepicker(target);
					} else {
						this._enableDatepicker(target);
					}
				}
				this._attachments($(target), inst);
				this._autoSize(inst);
				this._setDate(inst, date);
				this._updateAlternate(inst);
				this._updateDatepicker(inst);
			}
		},

		// Change method deprecated
		_changeDatepicker: function _changeDatepicker(target, name, value) {
			this._optionDatepicker(target, name, value);
		},

		/* Redraw the date picker attached to an input field or division.
   * @param  target  element - the target input field or division or span
   */
		_refreshDatepicker: function _refreshDatepicker(target) {
			var inst = this._getInst(target);
			if (inst) {
				this._updateDatepicker(inst);
			}
		},

		/* Set the dates for a jQuery selection.
   * @param  target element - the target input field or division or span
   * @param  date	Date - the new date
   */
		_setDateDatepicker: function _setDateDatepicker(target, date) {
			var inst = this._getInst(target);
			if (inst) {
				this._setDate(inst, date);
				this._updateDatepicker(inst);
				this._updateAlternate(inst);
			}
		},

		/* Get the date(s) for the first entry in a jQuery selection.
   * @param  target element - the target input field or division or span
   * @param  noDefault boolean - true if no default date is to be used
   * @return Date - the current date
   */
		_getDateDatepicker: function _getDateDatepicker(target, noDefault) {
			var inst = this._getInst(target);
			if (inst && !inst.inline) {
				this._setDateFromField(inst, noDefault);
			}
			return inst ? this._getDate(inst) : null;
		},

		/* Handle keystrokes. */
		_doKeyDown: function _doKeyDown(event) {
			var onSelect,
			    dateStr,
			    sel,
			    inst = $.datepicker._getInst(event.target),
			    handled = true,
			    isRTL = inst.dpDiv.is(".ui-datepicker-rtl");

			inst._keyEvent = true;
			if ($.datepicker._datepickerShowing) {
				switch (event.keyCode) {
					case 9:
						$.datepicker._hideDatepicker();
						handled = false;
						break; // hide on tab out
					case 13:
						sel = $("td." + $.datepicker._dayOverClass + ":not(." + $.datepicker._currentClass + ")", inst.dpDiv);
						if (sel[0]) {
							$.datepicker._selectDay(event.target, inst.selectedMonth, inst.selectedYear, sel[0]);
						}

						onSelect = $.datepicker._get(inst, "onSelect");
						if (onSelect) {
							dateStr = $.datepicker._formatDate(inst);

							// Trigger custom callback
							onSelect.apply(inst.input ? inst.input[0] : null, [dateStr, inst]);
						} else {
							$.datepicker._hideDatepicker();
						}

						return false; // don't submit the form
					case 27:
						$.datepicker._hideDatepicker();
						break; // hide on escape
					case 33:
						$.datepicker._adjustDate(event.target, event.ctrlKey ? -$.datepicker._get(inst, "stepBigMonths") : -$.datepicker._get(inst, "stepMonths"), "M");
						break; // previous month/year on page up/+ ctrl
					case 34:
						$.datepicker._adjustDate(event.target, event.ctrlKey ? +$.datepicker._get(inst, "stepBigMonths") : +$.datepicker._get(inst, "stepMonths"), "M");
						break; // next month/year on page down/+ ctrl
					case 35:
						if (event.ctrlKey || event.metaKey) {
							$.datepicker._clearDate(event.target);
						}
						handled = event.ctrlKey || event.metaKey;
						break; // clear on ctrl or command +end
					case 36:
						if (event.ctrlKey || event.metaKey) {
							$.datepicker._gotoToday(event.target);
						}
						handled = event.ctrlKey || event.metaKey;
						break; // current on ctrl or command +home
					case 37:
						if (event.ctrlKey || event.metaKey) {
							$.datepicker._adjustDate(event.target, isRTL ? +1 : -1, "D");
						}
						handled = event.ctrlKey || event.metaKey;

						// -1 day on ctrl or command +left
						if (event.originalEvent.altKey) {
							$.datepicker._adjustDate(event.target, event.ctrlKey ? -$.datepicker._get(inst, "stepBigMonths") : -$.datepicker._get(inst, "stepMonths"), "M");
						}

						// next month/year on alt +left on Mac
						break;
					case 38:
						if (event.ctrlKey || event.metaKey) {
							$.datepicker._adjustDate(event.target, -7, "D");
						}
						handled = event.ctrlKey || event.metaKey;
						break; // -1 week on ctrl or command +up
					case 39:
						if (event.ctrlKey || event.metaKey) {
							$.datepicker._adjustDate(event.target, isRTL ? -1 : +1, "D");
						}
						handled = event.ctrlKey || event.metaKey;

						// +1 day on ctrl or command +right
						if (event.originalEvent.altKey) {
							$.datepicker._adjustDate(event.target, event.ctrlKey ? +$.datepicker._get(inst, "stepBigMonths") : +$.datepicker._get(inst, "stepMonths"), "M");
						}

						// next month/year on alt +right
						break;
					case 40:
						if (event.ctrlKey || event.metaKey) {
							$.datepicker._adjustDate(event.target, +7, "D");
						}
						handled = event.ctrlKey || event.metaKey;
						break; // +1 week on ctrl or command +down
					default:
						handled = false;
				}
			} else if (event.keyCode === 36 && event.ctrlKey) {
				// display the date picker on ctrl+home
				$.datepicker._showDatepicker(this);
			} else {
				handled = false;
			}

			if (handled) {
				event.preventDefault();
				event.stopPropagation();
			}
		},

		/* Filter entered characters - based on date format. */
		_doKeyPress: function _doKeyPress(event) {
			var chars,
			    chr,
			    inst = $.datepicker._getInst(event.target);

			if ($.datepicker._get(inst, "constrainInput")) {
				chars = $.datepicker._possibleChars($.datepicker._get(inst, "dateFormat"));
				chr = String.fromCharCode(event.charCode == null ? event.keyCode : event.charCode);
				return event.ctrlKey || event.metaKey || chr < " " || !chars || chars.indexOf(chr) > -1;
			}
		},

		/* Synchronise manual entry and field/alternate field. */
		_doKeyUp: function _doKeyUp(event) {
			var date,
			    inst = $.datepicker._getInst(event.target);

			if (inst.input.val() !== inst.lastVal) {
				try {
					date = $.datepicker.parseDate($.datepicker._get(inst, "dateFormat"), inst.input ? inst.input.val() : null, $.datepicker._getFormatConfig(inst));

					if (date) {
						// only if valid
						$.datepicker._setDateFromField(inst);
						$.datepicker._updateAlternate(inst);
						$.datepicker._updateDatepicker(inst);
					}
				} catch (err) {}
			}
			return true;
		},

		/* Pop-up the date picker for a given input field.
   * If false returned from beforeShow event handler do not show.
   * @param  input  element - the input field attached to the date picker or
   *					event - if triggered by focus
   */
		_showDatepicker: function _showDatepicker(input) {
			input = input.target || input;
			if (input.nodeName.toLowerCase() !== "input") {
				// find from button/image trigger
				input = $("input", input.parentNode)[0];
			}

			if ($.datepicker._isDisabledDatepicker(input) || $.datepicker._lastInput === input) {
				// already here
				return;
			}

			var inst, beforeShow, beforeShowSettings, isFixed, offset, showAnim, duration;

			inst = $.datepicker._getInst(input);
			if ($.datepicker._curInst && $.datepicker._curInst !== inst) {
				$.datepicker._curInst.dpDiv.stop(true, true);
				if (inst && $.datepicker._datepickerShowing) {
					$.datepicker._hideDatepicker($.datepicker._curInst.input[0]);
				}
			}

			beforeShow = $.datepicker._get(inst, "beforeShow");
			beforeShowSettings = beforeShow ? beforeShow.apply(input, [input, inst]) : {};
			if (beforeShowSettings === false) {
				return;
			}
			datepicker_extendRemove(inst.settings, beforeShowSettings);

			inst.lastVal = null;
			$.datepicker._lastInput = input;
			$.datepicker._setDateFromField(inst);

			if ($.datepicker._inDialog) {
				// hide cursor
				input.value = "";
			}
			if (!$.datepicker._pos) {
				// position below input
				$.datepicker._pos = $.datepicker._findPos(input);
				$.datepicker._pos[1] += input.offsetHeight; // add the height
			}

			isFixed = false;
			$(input).parents().each(function () {
				isFixed |= $(this).css("position") === "fixed";
				return !isFixed;
			});

			offset = { left: $.datepicker._pos[0], top: $.datepicker._pos[1] };
			$.datepicker._pos = null;

			//to avoid flashes on Firefox
			inst.dpDiv.empty();

			// determine sizing offscreen
			inst.dpDiv.css({ position: "absolute", display: "block", top: "-1000px" });
			$.datepicker._updateDatepicker(inst);

			// fix width for dynamic number of date pickers
			// and adjust position before showing
			offset = $.datepicker._checkOffset(inst, offset, isFixed);
			inst.dpDiv.css({ position: $.datepicker._inDialog && $.blockUI ? "static" : isFixed ? "fixed" : "absolute", display: "none",
				left: offset.left + "px", top: offset.top + "px" });

			if (!inst.inline) {
				showAnim = $.datepicker._get(inst, "showAnim");
				duration = $.datepicker._get(inst, "duration");
				inst.dpDiv.css("z-index", datepicker_getZindex($(input)) + 1);
				$.datepicker._datepickerShowing = true;

				if ($.effects && $.effects.effect[showAnim]) {
					inst.dpDiv.show(showAnim, $.datepicker._get(inst, "showOptions"), duration);
				} else {
					inst.dpDiv[showAnim || "show"](showAnim ? duration : null);
				}

				if ($.datepicker._shouldFocusInput(inst)) {
					inst.input.trigger("focus");
				}

				$.datepicker._curInst = inst;
			}
		},

		/* Generate the date picker content. */
		_updateDatepicker: function _updateDatepicker(inst) {
			this.maxRows = 4; //Reset the max number of rows being displayed (see #7043)
			datepicker_instActive = inst; // for delegate hover events
			inst.dpDiv.empty().append(this._generateHTML(inst));
			this._attachHandlers(inst);

			var origyearshtml,
			    numMonths = this._getNumberOfMonths(inst),
			    cols = numMonths[1],
			    width = 17,
			    activeCell = inst.dpDiv.find("." + this._dayOverClass + " a");

			if (activeCell.length > 0) {
				datepicker_handleMouseover.apply(activeCell.get(0));
			}

			inst.dpDiv.removeClass("ui-datepicker-multi-2 ui-datepicker-multi-3 ui-datepicker-multi-4").width("");
			if (cols > 1) {
				inst.dpDiv.addClass("ui-datepicker-multi-" + cols).css("width", width * cols + "em");
			}
			inst.dpDiv[(numMonths[0] !== 1 || numMonths[1] !== 1 ? "add" : "remove") + "Class"]("ui-datepicker-multi");
			inst.dpDiv[(this._get(inst, "isRTL") ? "add" : "remove") + "Class"]("ui-datepicker-rtl");

			if (inst === $.datepicker._curInst && $.datepicker._datepickerShowing && $.datepicker._shouldFocusInput(inst)) {
				inst.input.trigger("focus");
			}

			// Deffered render of the years select (to avoid flashes on Firefox)
			if (inst.yearshtml) {
				origyearshtml = inst.yearshtml;
				setTimeout(function () {

					//assure that inst.yearshtml didn't change.
					if (origyearshtml === inst.yearshtml && inst.yearshtml) {
						inst.dpDiv.find("select.ui-datepicker-year:first").replaceWith(inst.yearshtml);
					}
					origyearshtml = inst.yearshtml = null;
				}, 0);
			}
		},

		// #6694 - don't focus the input if it's already focused
		// this breaks the change event in IE
		// Support: IE and jQuery <1.9
		_shouldFocusInput: function _shouldFocusInput(inst) {
			return inst.input && inst.input.is(":visible") && !inst.input.is(":disabled") && !inst.input.is(":focus");
		},

		/* Check positioning to remain on screen. */
		_checkOffset: function _checkOffset(inst, offset, isFixed) {
			var dpWidth = inst.dpDiv.outerWidth(),
			    dpHeight = inst.dpDiv.outerHeight(),
			    inputWidth = inst.input ? inst.input.outerWidth() : 0,
			    inputHeight = inst.input ? inst.input.outerHeight() : 0,
			    viewWidth = document.documentElement.clientWidth + (isFixed ? 0 : $(document).scrollLeft()),
			    viewHeight = document.documentElement.clientHeight + (isFixed ? 0 : $(document).scrollTop());

			offset.left -= this._get(inst, "isRTL") ? dpWidth - inputWidth : 0;
			offset.left -= isFixed && offset.left === inst.input.offset().left ? $(document).scrollLeft() : 0;
			offset.top -= isFixed && offset.top === inst.input.offset().top + inputHeight ? $(document).scrollTop() : 0;

			// Now check if datepicker is showing outside window viewport - move to a better place if so.
			offset.left -= Math.min(offset.left, offset.left + dpWidth > viewWidth && viewWidth > dpWidth ? Math.abs(offset.left + dpWidth - viewWidth) : 0);
			offset.top -= Math.min(offset.top, offset.top + dpHeight > viewHeight && viewHeight > dpHeight ? Math.abs(dpHeight + inputHeight) : 0);

			return offset;
		},

		/* Find an object's position on the screen. */
		_findPos: function _findPos(obj) {
			var position,
			    inst = this._getInst(obj),
			    isRTL = this._get(inst, "isRTL");

			while (obj && (obj.type === "hidden" || obj.nodeType !== 1 || $.expr.filters.hidden(obj))) {
				obj = obj[isRTL ? "previousSibling" : "nextSibling"];
			}

			position = $(obj).offset();
			return [position.left, position.top];
		},

		/* Hide the date picker from view.
   * @param  input  element - the input field attached to the date picker
   */
		_hideDatepicker: function _hideDatepicker(input) {
			var showAnim,
			    duration,
			    postProcess,
			    onClose,
			    inst = this._curInst;

			if (!inst || input && inst !== $.data(input, "datepicker")) {
				return;
			}

			if (this._datepickerShowing) {
				showAnim = this._get(inst, "showAnim");
				duration = this._get(inst, "duration");
				postProcess = function postProcess() {
					$.datepicker._tidyDialog(inst);
				};

				// DEPRECATED: after BC for 1.8.x $.effects[ showAnim ] is not needed
				if ($.effects && ($.effects.effect[showAnim] || $.effects[showAnim])) {
					inst.dpDiv.hide(showAnim, $.datepicker._get(inst, "showOptions"), duration, postProcess);
				} else {
					inst.dpDiv[showAnim === "slideDown" ? "slideUp" : showAnim === "fadeIn" ? "fadeOut" : "hide"](showAnim ? duration : null, postProcess);
				}

				if (!showAnim) {
					postProcess();
				}
				this._datepickerShowing = false;

				onClose = this._get(inst, "onClose");
				if (onClose) {
					onClose.apply(inst.input ? inst.input[0] : null, [inst.input ? inst.input.val() : "", inst]);
				}

				this._lastInput = null;
				if (this._inDialog) {
					this._dialogInput.css({ position: "absolute", left: "0", top: "-100px" });
					if ($.blockUI) {
						$.unblockUI();
						$("body").append(this.dpDiv);
					}
				}
				this._inDialog = false;
			}
		},

		/* Tidy up after a dialog display. */
		_tidyDialog: function _tidyDialog(inst) {
			inst.dpDiv.removeClass(this._dialogClass).off(".ui-datepicker-calendar");
		},

		/* Close date picker if clicked elsewhere. */
		_checkExternalClick: function _checkExternalClick(event) {
			if (!$.datepicker._curInst) {
				return;
			}

			var $target = $(event.target),
			    inst = $.datepicker._getInst($target[0]);

			if ($target[0].id !== $.datepicker._mainDivId && $target.parents("#" + $.datepicker._mainDivId).length === 0 && !$target.hasClass($.datepicker.markerClassName) && !$target.closest("." + $.datepicker._triggerClass).length && $.datepicker._datepickerShowing && !($.datepicker._inDialog && $.blockUI) || $target.hasClass($.datepicker.markerClassName) && $.datepicker._curInst !== inst) {
				$.datepicker._hideDatepicker();
			}
		},

		/* Adjust one of the date sub-fields. */
		_adjustDate: function _adjustDate(id, offset, period) {
			var target = $(id),
			    inst = this._getInst(target[0]);

			if (this._isDisabledDatepicker(target[0])) {
				return;
			}
			this._adjustInstDate(inst, offset + (period === "M" ? this._get(inst, "showCurrentAtPos") : 0), // undo positioning
			period);
			this._updateDatepicker(inst);
		},

		/* Action for current link. */
		_gotoToday: function _gotoToday(id) {
			var date,
			    target = $(id),
			    inst = this._getInst(target[0]);

			if (this._get(inst, "gotoCurrent") && inst.currentDay) {
				inst.selectedDay = inst.currentDay;
				inst.drawMonth = inst.selectedMonth = inst.currentMonth;
				inst.drawYear = inst.selectedYear = inst.currentYear;
			} else {
				date = new Date();
				inst.selectedDay = date.getDate();
				inst.drawMonth = inst.selectedMonth = date.getMonth();
				inst.drawYear = inst.selectedYear = date.getFullYear();
			}
			this._notifyChange(inst);
			this._adjustDate(target);
		},

		/* Action for selecting a new month/year. */
		_selectMonthYear: function _selectMonthYear(id, select, period) {
			var target = $(id),
			    inst = this._getInst(target[0]);

			inst["selected" + (period === "M" ? "Month" : "Year")] = inst["draw" + (period === "M" ? "Month" : "Year")] = parseInt(select.options[select.selectedIndex].value, 10);

			this._notifyChange(inst);
			this._adjustDate(target);
		},

		/* Action for selecting a day. */
		_selectDay: function _selectDay(id, month, year, td) {
			var inst,
			    target = $(id);

			if ($(td).hasClass(this._unselectableClass) || this._isDisabledDatepicker(target[0])) {
				return;
			}

			inst = this._getInst(target[0]);
			inst.selectedDay = inst.currentDay = $("a", td).html();
			inst.selectedMonth = inst.currentMonth = month;
			inst.selectedYear = inst.currentYear = year;
			this._selectDate(id, this._formatDate(inst, inst.currentDay, inst.currentMonth, inst.currentYear));
		},

		/* Erase the input field and hide the date picker. */
		_clearDate: function _clearDate(id) {
			var target = $(id);
			this._selectDate(target, "");
		},

		/* Update the input field with the selected date. */
		_selectDate: function _selectDate(id, dateStr) {
			var onSelect,
			    target = $(id),
			    inst = this._getInst(target[0]);

			dateStr = dateStr != null ? dateStr : this._formatDate(inst);
			if (inst.input) {
				inst.input.val(dateStr);
			}
			this._updateAlternate(inst);

			onSelect = this._get(inst, "onSelect");
			if (onSelect) {
				onSelect.apply(inst.input ? inst.input[0] : null, [dateStr, inst]); // trigger custom callback
			} else if (inst.input) {
				inst.input.trigger("change"); // fire the change event
			}

			if (inst.inline) {
				this._updateDatepicker(inst);
			} else {
				this._hideDatepicker();
				this._lastInput = inst.input[0];
				if (_typeof(inst.input[0]) !== "object") {
					inst.input.trigger("focus"); // restore focus
				}
				this._lastInput = null;
			}
		},

		/* Update any alternate field to synchronise with the main field. */
		_updateAlternate: function _updateAlternate(inst) {
			var altFormat,
			    date,
			    dateStr,
			    altField = this._get(inst, "altField");

			if (altField) {
				// update alternate field too
				altFormat = this._get(inst, "altFormat") || this._get(inst, "dateFormat");
				date = this._getDate(inst);
				dateStr = this.formatDate(altFormat, date, this._getFormatConfig(inst));
				$(altField).val(dateStr);
			}
		},

		/* Set as beforeShowDay function to prevent selection of weekends.
   * @param  date  Date - the date to customise
   * @return [boolean, string] - is this date selectable?, what is its CSS class?
   */
		noWeekends: function noWeekends(date) {
			var day = date.getDay();
			return [day > 0 && day < 6, ""];
		},

		/* Set as calculateWeek to determine the week of the year based on the ISO 8601 definition.
   * @param  date  Date - the date to get the week for
   * @return  number - the number of the week within the year that contains this date
   */
		iso8601Week: function iso8601Week(date) {
			var time,
			    checkDate = new Date(date.getTime());

			// Find Thursday of this week starting on Monday
			checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay() || 7));

			time = checkDate.getTime();
			checkDate.setMonth(0); // Compare with Jan 1
			checkDate.setDate(1);
			return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
		},

		/* Parse a string value into a date object.
   * See formatDate below for the possible formats.
   *
   * @param  format string - the expected format of the date
   * @param  value string - the date in the above format
   * @param  settings Object - attributes include:
   *					shortYearCutoff  number - the cutoff year for determining the century (optional)
   *					dayNamesShort	string[7] - abbreviated names of the days from Sunday (optional)
   *					dayNames		string[7] - names of the days from Sunday (optional)
   *					monthNamesShort string[12] - abbreviated names of the months (optional)
   *					monthNames		string[12] - names of the months (optional)
   * @return  Date - the extracted date value or null if value is blank
   */
		parseDate: function parseDate(format, value, settings) {
			if (format == null || value == null) {
				throw "Invalid arguments";
			}

			value = (typeof value === "undefined" ? "undefined" : _typeof(value)) === "object" ? value.toString() : value + "";
			if (value === "") {
				return null;
			}

			var iFormat,
			    dim,
			    extra,
			    iValue = 0,
			    shortYearCutoffTemp = (settings ? settings.shortYearCutoff : null) || this._defaults.shortYearCutoff,
			    shortYearCutoff = typeof shortYearCutoffTemp !== "string" ? shortYearCutoffTemp : new Date().getFullYear() % 100 + parseInt(shortYearCutoffTemp, 10),
			    dayNamesShort = (settings ? settings.dayNamesShort : null) || this._defaults.dayNamesShort,
			    dayNames = (settings ? settings.dayNames : null) || this._defaults.dayNames,
			    monthNamesShort = (settings ? settings.monthNamesShort : null) || this._defaults.monthNamesShort,
			    monthNames = (settings ? settings.monthNames : null) || this._defaults.monthNames,
			    year = -1,
			    month = -1,
			    day = -1,
			    doy = -1,
			    literal = false,
			    date,


			// Check whether a format character is doubled
			lookAhead = function lookAhead(match) {
				var matches = iFormat + 1 < format.length && format.charAt(iFormat + 1) === match;
				if (matches) {
					iFormat++;
				}
				return matches;
			},


			// Extract a number from the string value
			getNumber = function getNumber(match) {
				var isDoubled = lookAhead(match),
				    size = match === "@" ? 14 : match === "!" ? 20 : match === "y" && isDoubled ? 4 : match === "o" ? 3 : 2,
				    minSize = match === "y" ? size : 1,
				    digits = new RegExp("^\\d{" + minSize + "," + size + "}"),
				    num = value.substring(iValue).match(digits);
				if (!num) {
					throw "Missing number at position " + iValue;
				}
				iValue += num[0].length;
				return parseInt(num[0], 10);
			},


			// Extract a name from the string value and convert to an index
			getName = function getName(match, shortNames, longNames) {
				var index = -1,
				    names = $.map(lookAhead(match) ? longNames : shortNames, function (v, k) {
					return [[k, v]];
				}).sort(function (a, b) {
					return -(a[1].length - b[1].length);
				});

				$.each(names, function (i, pair) {
					var name = pair[1];
					if (value.substr(iValue, name.length).toLowerCase() === name.toLowerCase()) {
						index = pair[0];
						iValue += name.length;
						return false;
					}
				});
				if (index !== -1) {
					return index + 1;
				} else {
					throw "Unknown name at position " + iValue;
				}
			},


			// Confirm that a literal character matches the string value
			checkLiteral = function checkLiteral() {
				if (value.charAt(iValue) !== format.charAt(iFormat)) {
					throw "Unexpected literal at position " + iValue;
				}
				iValue++;
			};

			for (iFormat = 0; iFormat < format.length; iFormat++) {
				if (literal) {
					if (format.charAt(iFormat) === "'" && !lookAhead("'")) {
						literal = false;
					} else {
						checkLiteral();
					}
				} else {
					switch (format.charAt(iFormat)) {
						case "d":
							day = getNumber("d");
							break;
						case "D":
							getName("D", dayNamesShort, dayNames);
							break;
						case "o":
							doy = getNumber("o");
							break;
						case "m":
							month = getNumber("m");
							break;
						case "M":
							month = getName("M", monthNamesShort, monthNames);
							break;
						case "y":
							year = getNumber("y");
							break;
						case "@":
							date = new Date(getNumber("@"));
							year = date.getFullYear();
							month = date.getMonth() + 1;
							day = date.getDate();
							break;
						case "!":
							date = new Date((getNumber("!") - this._ticksTo1970) / 10000);
							year = date.getFullYear();
							month = date.getMonth() + 1;
							day = date.getDate();
							break;
						case "'":
							if (lookAhead("'")) {
								checkLiteral();
							} else {
								literal = true;
							}
							break;
						default:
							checkLiteral();
					}
				}
			}

			if (iValue < value.length) {
				extra = value.substr(iValue);
				if (!/^\s+/.test(extra)) {
					throw "Extra/unparsed characters found in date: " + extra;
				}
			}

			if (year === -1) {
				year = new Date().getFullYear();
			} else if (year < 100) {
				year += new Date().getFullYear() - new Date().getFullYear() % 100 + (year <= shortYearCutoff ? 0 : -100);
			}

			if (doy > -1) {
				month = 1;
				day = doy;
				do {
					dim = this._getDaysInMonth(year, month - 1);
					if (day <= dim) {
						break;
					}
					month++;
					day -= dim;
				} while (true);
			}

			date = this._daylightSavingAdjust(new Date(year, month - 1, day));
			if (date.getFullYear() !== year || date.getMonth() + 1 !== month || date.getDate() !== day) {
				throw "Invalid date"; // E.g. 31/02/00
			}
			return date;
		},

		/* Standard date formats. */
		ATOM: "yy-mm-dd", // RFC 3339 (ISO 8601)
		COOKIE: "D, dd M yy",
		ISO_8601: "yy-mm-dd",
		RFC_822: "D, d M y",
		RFC_850: "DD, dd-M-y",
		RFC_1036: "D, d M y",
		RFC_1123: "D, d M yy",
		RFC_2822: "D, d M yy",
		RSS: "D, d M y", // RFC 822
		TICKS: "!",
		TIMESTAMP: "@",
		W3C: "yy-mm-dd", // ISO 8601

		_ticksTo1970: ((1970 - 1) * 365 + Math.floor(1970 / 4) - Math.floor(1970 / 100) + Math.floor(1970 / 400)) * 24 * 60 * 60 * 10000000,

		/* Format a date object into a string value.
   * The format can be combinations of the following:
   * d  - day of month (no leading zero)
   * dd - day of month (two digit)
   * o  - day of year (no leading zeros)
   * oo - day of year (three digit)
   * D  - day name short
   * DD - day name long
   * m  - month of year (no leading zero)
   * mm - month of year (two digit)
   * M  - month name short
   * MM - month name long
   * y  - year (two digit)
   * yy - year (four digit)
   * @ - Unix timestamp (ms since 01/01/1970)
   * ! - Windows ticks (100ns since 01/01/0001)
   * "..." - literal text
   * '' - single quote
   *
   * @param  format string - the desired format of the date
   * @param  date Date - the date value to format
   * @param  settings Object - attributes include:
   *					dayNamesShort	string[7] - abbreviated names of the days from Sunday (optional)
   *					dayNames		string[7] - names of the days from Sunday (optional)
   *					monthNamesShort string[12] - abbreviated names of the months (optional)
   *					monthNames		string[12] - names of the months (optional)
   * @return  string - the date in the above format
   */
		formatDate: function formatDate(format, date, settings) {
			if (!date) {
				return "";
			}

			var iFormat,
			    dayNamesShort = (settings ? settings.dayNamesShort : null) || this._defaults.dayNamesShort,
			    dayNames = (settings ? settings.dayNames : null) || this._defaults.dayNames,
			    monthNamesShort = (settings ? settings.monthNamesShort : null) || this._defaults.monthNamesShort,
			    monthNames = (settings ? settings.monthNames : null) || this._defaults.monthNames,


			// Check whether a format character is doubled
			lookAhead = function lookAhead(match) {
				var matches = iFormat + 1 < format.length && format.charAt(iFormat + 1) === match;
				if (matches) {
					iFormat++;
				}
				return matches;
			},


			// Format a number, with leading zero if necessary
			formatNumber = function formatNumber(match, value, len) {
				var num = "" + value;
				if (lookAhead(match)) {
					while (num.length < len) {
						num = "0" + num;
					}
				}
				return num;
			},


			// Format a name, short or long as requested
			formatName = function formatName(match, value, shortNames, longNames) {
				return lookAhead(match) ? longNames[value] : shortNames[value];
			},
			    output = "",
			    literal = false;

			if (date) {
				for (iFormat = 0; iFormat < format.length; iFormat++) {
					if (literal) {
						if (format.charAt(iFormat) === "'" && !lookAhead("'")) {
							literal = false;
						} else {
							output += format.charAt(iFormat);
						}
					} else {
						switch (format.charAt(iFormat)) {
							case "d":
								output += formatNumber("d", date.getDate(), 2);
								break;
							case "D":
								output += formatName("D", date.getDay(), dayNamesShort, dayNames);
								break;
							case "o":
								output += formatNumber("o", Math.round((new Date(date.getFullYear(), date.getMonth(), date.getDate()).getTime() - new Date(date.getFullYear(), 0, 0).getTime()) / 86400000), 3);
								break;
							case "m":
								output += formatNumber("m", date.getMonth() + 1, 2);
								break;
							case "M":
								output += formatName("M", date.getMonth(), monthNamesShort, monthNames);
								break;
							case "y":
								output += lookAhead("y") ? date.getFullYear() : (date.getFullYear() % 100 < 10 ? "0" : "") + date.getFullYear() % 100;
								break;
							case "@":
								output += date.getTime();
								break;
							case "!":
								output += date.getTime() * 10000 + this._ticksTo1970;
								break;
							case "'":
								if (lookAhead("'")) {
									output += "'";
								} else {
									literal = true;
								}
								break;
							default:
								output += format.charAt(iFormat);
						}
					}
				}
			}
			return output;
		},

		/* Extract all possible characters from the date format. */
		_possibleChars: function _possibleChars(format) {
			var iFormat,
			    chars = "",
			    literal = false,


			// Check whether a format character is doubled
			lookAhead = function lookAhead(match) {
				var matches = iFormat + 1 < format.length && format.charAt(iFormat + 1) === match;
				if (matches) {
					iFormat++;
				}
				return matches;
			};

			for (iFormat = 0; iFormat < format.length; iFormat++) {
				if (literal) {
					if (format.charAt(iFormat) === "'" && !lookAhead("'")) {
						literal = false;
					} else {
						chars += format.charAt(iFormat);
					}
				} else {
					switch (format.charAt(iFormat)) {
						case "d":case "m":case "y":case "@":
							chars += "0123456789";
							break;
						case "D":case "M":
							return null; // Accept anything
						case "'":
							if (lookAhead("'")) {
								chars += "'";
							} else {
								literal = true;
							}
							break;
						default:
							chars += format.charAt(iFormat);
					}
				}
			}
			return chars;
		},

		/* Get a setting value, defaulting if necessary. */
		_get: function _get(inst, name) {
			return inst.settings[name] !== undefined ? inst.settings[name] : this._defaults[name];
		},

		/* Parse existing date and initialise date picker. */
		_setDateFromField: function _setDateFromField(inst, noDefault) {
			if (inst.input.val() === inst.lastVal) {
				return;
			}

			var dateFormat = this._get(inst, "dateFormat"),
			    dates = inst.lastVal = inst.input ? inst.input.val() : null,
			    defaultDate = this._getDefaultDate(inst),
			    date = defaultDate,
			    settings = this._getFormatConfig(inst);

			try {
				date = this.parseDate(dateFormat, dates, settings) || defaultDate;
			} catch (event) {
				dates = noDefault ? "" : dates;
			}
			inst.selectedDay = date.getDate();
			inst.drawMonth = inst.selectedMonth = date.getMonth();
			inst.drawYear = inst.selectedYear = date.getFullYear();
			inst.currentDay = dates ? date.getDate() : 0;
			inst.currentMonth = dates ? date.getMonth() : 0;
			inst.currentYear = dates ? date.getFullYear() : 0;
			this._adjustInstDate(inst);
		},

		/* Retrieve the default date shown on opening. */
		_getDefaultDate: function _getDefaultDate(inst) {
			return this._restrictMinMax(inst, this._determineDate(inst, this._get(inst, "defaultDate"), new Date()));
		},

		/* A date may be specified as an exact value or a relative one. */
		_determineDate: function _determineDate(inst, date, defaultDate) {
			var offsetNumeric = function offsetNumeric(offset) {
				var date = new Date();
				date.setDate(date.getDate() + offset);
				return date;
			},
			    offsetString = function offsetString(offset) {
				try {
					return $.datepicker.parseDate($.datepicker._get(inst, "dateFormat"), offset, $.datepicker._getFormatConfig(inst));
				} catch (e) {

					// Ignore
				}

				var date = (offset.toLowerCase().match(/^c/) ? $.datepicker._getDate(inst) : null) || new Date(),
				    year = date.getFullYear(),
				    month = date.getMonth(),
				    day = date.getDate(),
				    pattern = /([+\-]?[0-9]+)\s*(d|D|w|W|m|M|y|Y)?/g,
				    matches = pattern.exec(offset);

				while (matches) {
					switch (matches[2] || "d") {
						case "d":case "D":
							day += parseInt(matches[1], 10);break;
						case "w":case "W":
							day += parseInt(matches[1], 10) * 7;break;
						case "m":case "M":
							month += parseInt(matches[1], 10);
							day = Math.min(day, $.datepicker._getDaysInMonth(year, month));
							break;
						case "y":case "Y":
							year += parseInt(matches[1], 10);
							day = Math.min(day, $.datepicker._getDaysInMonth(year, month));
							break;
					}
					matches = pattern.exec(offset);
				}
				return new Date(year, month, day);
			},
			    newDate = date == null || date === "" ? defaultDate : typeof date === "string" ? offsetString(date) : typeof date === "number" ? isNaN(date) ? defaultDate : offsetNumeric(date) : new Date(date.getTime());

			newDate = newDate && newDate.toString() === "Invalid Date" ? defaultDate : newDate;
			if (newDate) {
				newDate.setHours(0);
				newDate.setMinutes(0);
				newDate.setSeconds(0);
				newDate.setMilliseconds(0);
			}
			return this._daylightSavingAdjust(newDate);
		},

		/* Handle switch to/from daylight saving.
   * Hours may be non-zero on daylight saving cut-over:
   * > 12 when midnight changeover, but then cannot generate
   * midnight datetime, so jump to 1AM, otherwise reset.
   * @param  date  (Date) the date to check
   * @return  (Date) the corrected date
   */
		_daylightSavingAdjust: function _daylightSavingAdjust(date) {
			if (!date) {
				return null;
			}
			date.setHours(date.getHours() > 12 ? date.getHours() + 2 : 0);
			return date;
		},

		/* Set the date(s) directly. */
		_setDate: function _setDate(inst, date, noChange) {
			var clear = !date,
			    origMonth = inst.selectedMonth,
			    origYear = inst.selectedYear,
			    newDate = this._restrictMinMax(inst, this._determineDate(inst, date, new Date()));

			inst.selectedDay = inst.currentDay = newDate.getDate();
			inst.drawMonth = inst.selectedMonth = inst.currentMonth = newDate.getMonth();
			inst.drawYear = inst.selectedYear = inst.currentYear = newDate.getFullYear();
			if ((origMonth !== inst.selectedMonth || origYear !== inst.selectedYear) && !noChange) {
				this._notifyChange(inst);
			}
			this._adjustInstDate(inst);
			if (inst.input) {
				inst.input.val(clear ? "" : this._formatDate(inst));
			}
		},

		/* Retrieve the date(s) directly. */
		_getDate: function _getDate(inst) {
			var startDate = !inst.currentYear || inst.input && inst.input.val() === "" ? null : this._daylightSavingAdjust(new Date(inst.currentYear, inst.currentMonth, inst.currentDay));
			return startDate;
		},

		/* Attach the onxxx handlers.  These are declared statically so
   * they work with static code transformers like Caja.
   */
		_attachHandlers: function _attachHandlers(inst) {
			var stepMonths = this._get(inst, "stepMonths"),
			    id = "#" + inst.id.replace(/\\\\/g, "\\");
			inst.dpDiv.find("[data-handler]").map(function () {
				var handler = {
					prev: function prev() {
						$.datepicker._adjustDate(id, -stepMonths, "M");
					},
					next: function next() {
						$.datepicker._adjustDate(id, +stepMonths, "M");
					},
					hide: function hide() {
						$.datepicker._hideDatepicker();
					},
					today: function today() {
						$.datepicker._gotoToday(id);
					},
					selectDay: function selectDay() {
						$.datepicker._selectDay(id, +this.getAttribute("data-month"), +this.getAttribute("data-year"), this);
						return false;
					},
					selectMonth: function selectMonth() {
						$.datepicker._selectMonthYear(id, this, "M");
						return false;
					},
					selectYear: function selectYear() {
						$.datepicker._selectMonthYear(id, this, "Y");
						return false;
					}
				};
				$(this).on(this.getAttribute("data-event"), handler[this.getAttribute("data-handler")]);
			});
		},

		/* Generate the HTML for the current state of the date picker. */
		_generateHTML: function _generateHTML(inst) {
			var maxDraw,
			    prevText,
			    prev,
			    nextText,
			    next,
			    currentText,
			    gotoDate,
			    controls,
			    buttonPanel,
			    firstDay,
			    showWeek,
			    dayNames,
			    dayNamesMin,
			    monthNames,
			    monthNamesShort,
			    beforeShowDay,
			    showOtherMonths,
			    selectOtherMonths,
			    defaultDate,
			    html,
			    dow,
			    row,
			    group,
			    col,
			    selectedDate,
			    cornerClass,
			    calender,
			    thead,
			    day,
			    daysInMonth,
			    leadDays,
			    curRows,
			    numRows,
			    printDate,
			    dRow,
			    tbody,
			    daySettings,
			    otherMonth,
			    unselectable,
			    tempDate = new Date(),
			    today = this._daylightSavingAdjust(new Date(tempDate.getFullYear(), tempDate.getMonth(), tempDate.getDate())),
			    // clear time
			isRTL = this._get(inst, "isRTL"),
			    showButtonPanel = this._get(inst, "showButtonPanel"),
			    hideIfNoPrevNext = this._get(inst, "hideIfNoPrevNext"),
			    navigationAsDateFormat = this._get(inst, "navigationAsDateFormat"),
			    numMonths = this._getNumberOfMonths(inst),
			    showCurrentAtPos = this._get(inst, "showCurrentAtPos"),
			    stepMonths = this._get(inst, "stepMonths"),
			    isMultiMonth = numMonths[0] !== 1 || numMonths[1] !== 1,
			    currentDate = this._daylightSavingAdjust(!inst.currentDay ? new Date(9999, 9, 9) : new Date(inst.currentYear, inst.currentMonth, inst.currentDay)),
			    minDate = this._getMinMaxDate(inst, "min"),
			    maxDate = this._getMinMaxDate(inst, "max"),
			    drawMonth = inst.drawMonth - showCurrentAtPos,
			    drawYear = inst.drawYear;

			if (drawMonth < 0) {
				drawMonth += 12;
				drawYear--;
			}
			if (maxDate) {
				maxDraw = this._daylightSavingAdjust(new Date(maxDate.getFullYear(), maxDate.getMonth() - numMonths[0] * numMonths[1] + 1, maxDate.getDate()));
				maxDraw = minDate && maxDraw < minDate ? minDate : maxDraw;
				while (this._daylightSavingAdjust(new Date(drawYear, drawMonth, 1)) > maxDraw) {
					drawMonth--;
					if (drawMonth < 0) {
						drawMonth = 11;
						drawYear--;
					}
				}
			}
			inst.drawMonth = drawMonth;
			inst.drawYear = drawYear;

			prevText = this._get(inst, "prevText");
			prevText = !navigationAsDateFormat ? prevText : this.formatDate(prevText, this._daylightSavingAdjust(new Date(drawYear, drawMonth - stepMonths, 1)), this._getFormatConfig(inst));

			prev = this._canAdjustMonth(inst, -1, drawYear, drawMonth) ? "<a class='ui-datepicker-prev ui-corner-all' data-handler='prev' data-event='click'" + " title='" + prevText + "'><span class='ui-icon ui-icon-circle-triangle-" + (isRTL ? "e" : "w") + "'>" + prevText + "</span></a>" : hideIfNoPrevNext ? "" : "<a class='ui-datepicker-prev ui-corner-all ui-state-disabled' title='" + prevText + "'><span class='ui-icon ui-icon-circle-triangle-" + (isRTL ? "e" : "w") + "'>" + prevText + "</span></a>";

			nextText = this._get(inst, "nextText");
			nextText = !navigationAsDateFormat ? nextText : this.formatDate(nextText, this._daylightSavingAdjust(new Date(drawYear, drawMonth + stepMonths, 1)), this._getFormatConfig(inst));

			next = this._canAdjustMonth(inst, +1, drawYear, drawMonth) ? "<a class='ui-datepicker-next ui-corner-all' data-handler='next' data-event='click'" + " title='" + nextText + "'><span class='ui-icon ui-icon-circle-triangle-" + (isRTL ? "w" : "e") + "'>" + nextText + "</span></a>" : hideIfNoPrevNext ? "" : "<a class='ui-datepicker-next ui-corner-all ui-state-disabled' title='" + nextText + "'><span class='ui-icon ui-icon-circle-triangle-" + (isRTL ? "w" : "e") + "'>" + nextText + "</span></a>";

			currentText = this._get(inst, "currentText");
			gotoDate = this._get(inst, "gotoCurrent") && inst.currentDay ? currentDate : today;
			currentText = !navigationAsDateFormat ? currentText : this.formatDate(currentText, gotoDate, this._getFormatConfig(inst));

			controls = !inst.inline ? "<button type='button' class='ui-datepicker-close ui-state-default ui-priority-primary ui-corner-all' data-handler='hide' data-event='click'>" + this._get(inst, "closeText") + "</button>" : "";

			buttonPanel = showButtonPanel ? "<div class='ui-datepicker-buttonpane ui-widget-content'>" + (isRTL ? controls : "") + (this._isInRange(inst, gotoDate) ? "<button type='button' class='ui-datepicker-current ui-state-default ui-priority-secondary ui-corner-all' data-handler='today' data-event='click'" + ">" + currentText + "</button>" : "") + (isRTL ? "" : controls) + "</div>" : "";

			firstDay = parseInt(this._get(inst, "firstDay"), 10);
			firstDay = isNaN(firstDay) ? 0 : firstDay;

			showWeek = this._get(inst, "showWeek");
			dayNames = this._get(inst, "dayNames");
			dayNamesMin = this._get(inst, "dayNamesMin");
			monthNames = this._get(inst, "monthNames");
			monthNamesShort = this._get(inst, "monthNamesShort");
			beforeShowDay = this._get(inst, "beforeShowDay");
			showOtherMonths = this._get(inst, "showOtherMonths");
			selectOtherMonths = this._get(inst, "selectOtherMonths");
			defaultDate = this._getDefaultDate(inst);
			html = "";

			for (row = 0; row < numMonths[0]; row++) {
				group = "";
				this.maxRows = 4;
				for (col = 0; col < numMonths[1]; col++) {
					selectedDate = this._daylightSavingAdjust(new Date(drawYear, drawMonth, inst.selectedDay));
					cornerClass = " ui-corner-all";
					calender = "";
					if (isMultiMonth) {
						calender += "<div class='ui-datepicker-group";
						if (numMonths[1] > 1) {
							switch (col) {
								case 0:
									calender += " ui-datepicker-group-first";
									cornerClass = " ui-corner-" + (isRTL ? "right" : "left");break;
								case numMonths[1] - 1:
									calender += " ui-datepicker-group-last";
									cornerClass = " ui-corner-" + (isRTL ? "left" : "right");break;
								default:
									calender += " ui-datepicker-group-middle";cornerClass = "";break;
							}
						}
						calender += "'>";
					}
					calender += "<div class='ui-datepicker-header ui-widget-header ui-helper-clearfix" + cornerClass + "'>" + (/all|left/.test(cornerClass) && row === 0 ? isRTL ? next : prev : "") + (/all|right/.test(cornerClass) && row === 0 ? isRTL ? prev : next : "") + this._generateMonthYearHeader(inst, drawMonth, drawYear, minDate, maxDate, row > 0 || col > 0, monthNames, monthNamesShort) + // draw month headers
					"</div><table class='ui-datepicker-calendar'><thead>" + "<tr>";
					thead = showWeek ? "<th class='ui-datepicker-week-col'>" + this._get(inst, "weekHeader") + "</th>" : "";
					for (dow = 0; dow < 7; dow++) {
						// days of the week
						day = (dow + firstDay) % 7;
						thead += "<th scope='col'" + ((dow + firstDay + 6) % 7 >= 5 ? " class='ui-datepicker-week-end'" : "") + ">" + "<span title='" + dayNames[day] + "'>" + dayNamesMin[day] + "</span></th>";
					}
					calender += thead + "</tr></thead><tbody>";
					daysInMonth = this._getDaysInMonth(drawYear, drawMonth);
					if (drawYear === inst.selectedYear && drawMonth === inst.selectedMonth) {
						inst.selectedDay = Math.min(inst.selectedDay, daysInMonth);
					}
					leadDays = (this._getFirstDayOfMonth(drawYear, drawMonth) - firstDay + 7) % 7;
					curRows = Math.ceil((leadDays + daysInMonth) / 7); // calculate the number of rows to generate
					numRows = isMultiMonth ? this.maxRows > curRows ? this.maxRows : curRows : curRows; //If multiple months, use the higher number of rows (see #7043)
					this.maxRows = numRows;
					printDate = this._daylightSavingAdjust(new Date(drawYear, drawMonth, 1 - leadDays));
					for (dRow = 0; dRow < numRows; dRow++) {
						// create date picker rows
						calender += "<tr>";
						tbody = !showWeek ? "" : "<td class='ui-datepicker-week-col'>" + this._get(inst, "calculateWeek")(printDate) + "</td>";
						for (dow = 0; dow < 7; dow++) {
							// create date picker days
							daySettings = beforeShowDay ? beforeShowDay.apply(inst.input ? inst.input[0] : null, [printDate]) : [true, ""];
							otherMonth = printDate.getMonth() !== drawMonth;
							unselectable = otherMonth && !selectOtherMonths || !daySettings[0] || minDate && printDate < minDate || maxDate && printDate > maxDate;
							tbody += "<td class='" + ((dow + firstDay + 6) % 7 >= 5 ? " ui-datepicker-week-end" : "") + ( // highlight weekends
							otherMonth ? " ui-datepicker-other-month" : "") + ( // highlight days from other months
							printDate.getTime() === selectedDate.getTime() && drawMonth === inst.selectedMonth && inst._keyEvent || // user pressed key
							defaultDate.getTime() === printDate.getTime() && defaultDate.getTime() === selectedDate.getTime() ?

							// or defaultDate is current printedDate and defaultDate is selectedDate
							" " + this._dayOverClass : "") + ( // highlight selected day
							unselectable ? " " + this._unselectableClass + " ui-state-disabled" : "") + ( // highlight unselectable days
							otherMonth && !showOtherMonths ? "" : " " + daySettings[1] + ( // highlight custom dates
							printDate.getTime() === currentDate.getTime() ? " " + this._currentClass : "") + ( // highlight selected day
							printDate.getTime() === today.getTime() ? " ui-datepicker-today" : "")) + "'" + ( // highlight today (if different)
							(!otherMonth || showOtherMonths) && daySettings[2] ? " title='" + daySettings[2].replace(/'/g, "&#39;") + "'" : "") + ( // cell title
							unselectable ? "" : " data-handler='selectDay' data-event='click' data-month='" + printDate.getMonth() + "' data-year='" + printDate.getFullYear() + "'") + ">" + ( // actions
							otherMonth && !showOtherMonths ? "&#xa0;" : // display for other months
							unselectable ? "<span class='ui-state-default'>" + printDate.getDate() + "</span>" : "<a class='ui-state-default" + (printDate.getTime() === today.getTime() ? " ui-state-highlight" : "") + (printDate.getTime() === currentDate.getTime() ? " ui-state-active" : "") + ( // highlight selected day
							otherMonth ? " ui-priority-secondary" : "") + // distinguish dates from other months
							"' href='#'>" + printDate.getDate() + "</a>") + "</td>"; // display selectable date
							printDate.setDate(printDate.getDate() + 1);
							printDate = this._daylightSavingAdjust(printDate);
						}
						calender += tbody + "</tr>";
					}
					drawMonth++;
					if (drawMonth > 11) {
						drawMonth = 0;
						drawYear++;
					}
					calender += "</tbody></table>" + (isMultiMonth ? "</div>" + (numMonths[0] > 0 && col === numMonths[1] - 1 ? "<div class='ui-datepicker-row-break'></div>" : "") : "");
					group += calender;
				}
				html += group;
			}
			html += buttonPanel;
			inst._keyEvent = false;
			return html;
		},

		/* Generate the month and year header. */
		_generateMonthYearHeader: function _generateMonthYearHeader(inst, drawMonth, drawYear, minDate, maxDate, secondary, monthNames, monthNamesShort) {

			var inMinYear,
			    inMaxYear,
			    month,
			    years,
			    thisYear,
			    determineYear,
			    year,
			    endYear,
			    changeMonth = this._get(inst, "changeMonth"),
			    changeYear = this._get(inst, "changeYear"),
			    showMonthAfterYear = this._get(inst, "showMonthAfterYear"),
			    html = "<div class='ui-datepicker-title'>",
			    monthHtml = "";

			// Month selection
			if (secondary || !changeMonth) {
				monthHtml += "<span class='ui-datepicker-month'>" + monthNames[drawMonth] + "</span>";
			} else {
				inMinYear = minDate && minDate.getFullYear() === drawYear;
				inMaxYear = maxDate && maxDate.getFullYear() === drawYear;
				monthHtml += "<select class='ui-datepicker-month' data-handler='selectMonth' data-event='change'>";
				for (month = 0; month < 12; month++) {
					if ((!inMinYear || month >= minDate.getMonth()) && (!inMaxYear || month <= maxDate.getMonth())) {
						monthHtml += "<option value='" + month + "'" + (month === drawMonth ? " selected='selected'" : "") + ">" + monthNamesShort[month] + "</option>";
					}
				}
				monthHtml += "</select>";
			}

			if (!showMonthAfterYear) {
				html += monthHtml + (secondary || !(changeMonth && changeYear) ? "&#xa0;" : "");
			}

			// Year selection
			if (!inst.yearshtml) {
				inst.yearshtml = "";
				if (secondary || !changeYear) {
					html += "<span class='ui-datepicker-year'>" + drawYear + "</span>";
				} else {

					// determine range of years to display
					years = this._get(inst, "yearRange").split(":");
					thisYear = new Date().getFullYear();
					determineYear = function determineYear(value) {
						var year = value.match(/c[+\-].*/) ? drawYear + parseInt(value.substring(1), 10) : value.match(/[+\-].*/) ? thisYear + parseInt(value, 10) : parseInt(value, 10);
						return isNaN(year) ? thisYear : year;
					};
					year = determineYear(years[0]);
					endYear = Math.max(year, determineYear(years[1] || ""));
					year = minDate ? Math.max(year, minDate.getFullYear()) : year;
					endYear = maxDate ? Math.min(endYear, maxDate.getFullYear()) : endYear;
					inst.yearshtml += "<select class='ui-datepicker-year' data-handler='selectYear' data-event='change'>";
					for (; year <= endYear; year++) {
						inst.yearshtml += "<option value='" + year + "'" + (year === drawYear ? " selected='selected'" : "") + ">" + year + "</option>";
					}
					inst.yearshtml += "</select>";

					html += inst.yearshtml;
					inst.yearshtml = null;
				}
			}

			html += this._get(inst, "yearSuffix");
			if (showMonthAfterYear) {
				html += (secondary || !(changeMonth && changeYear) ? "&#xa0;" : "") + monthHtml;
			}
			html += "</div>"; // Close datepicker_header
			return html;
		},

		/* Adjust one of the date sub-fields. */
		_adjustInstDate: function _adjustInstDate(inst, offset, period) {
			var year = inst.selectedYear + (period === "Y" ? offset : 0),
			    month = inst.selectedMonth + (period === "M" ? offset : 0),
			    day = Math.min(inst.selectedDay, this._getDaysInMonth(year, month)) + (period === "D" ? offset : 0),
			    date = this._restrictMinMax(inst, this._daylightSavingAdjust(new Date(year, month, day)));

			inst.selectedDay = date.getDate();
			inst.drawMonth = inst.selectedMonth = date.getMonth();
			inst.drawYear = inst.selectedYear = date.getFullYear();
			if (period === "M" || period === "Y") {
				this._notifyChange(inst);
			}
		},

		/* Ensure a date is within any min/max bounds. */
		_restrictMinMax: function _restrictMinMax(inst, date) {
			var minDate = this._getMinMaxDate(inst, "min"),
			    maxDate = this._getMinMaxDate(inst, "max"),
			    newDate = minDate && date < minDate ? minDate : date;
			return maxDate && newDate > maxDate ? maxDate : newDate;
		},

		/* Notify change of month/year. */
		_notifyChange: function _notifyChange(inst) {
			var onChange = this._get(inst, "onChangeMonthYear");
			if (onChange) {
				onChange.apply(inst.input ? inst.input[0] : null, [inst.selectedYear, inst.selectedMonth + 1, inst]);
			}
		},

		/* Determine the number of months to show. */
		_getNumberOfMonths: function _getNumberOfMonths(inst) {
			var numMonths = this._get(inst, "numberOfMonths");
			return numMonths == null ? [1, 1] : typeof numMonths === "number" ? [1, numMonths] : numMonths;
		},

		/* Determine the current maximum date - ensure no time components are set. */
		_getMinMaxDate: function _getMinMaxDate(inst, minMax) {
			return this._determineDate(inst, this._get(inst, minMax + "Date"), null);
		},

		/* Find the number of days in a given month. */
		_getDaysInMonth: function _getDaysInMonth(year, month) {
			return 32 - this._daylightSavingAdjust(new Date(year, month, 32)).getDate();
		},

		/* Find the day of the week of the first of a month. */
		_getFirstDayOfMonth: function _getFirstDayOfMonth(year, month) {
			return new Date(year, month, 1).getDay();
		},

		/* Determines if we should allow a "next/prev" month display change. */
		_canAdjustMonth: function _canAdjustMonth(inst, offset, curYear, curMonth) {
			var numMonths = this._getNumberOfMonths(inst),
			    date = this._daylightSavingAdjust(new Date(curYear, curMonth + (offset < 0 ? offset : numMonths[0] * numMonths[1]), 1));

			if (offset < 0) {
				date.setDate(this._getDaysInMonth(date.getFullYear(), date.getMonth()));
			}
			return this._isInRange(inst, date);
		},

		/* Is the given date in the accepted range? */
		_isInRange: function _isInRange(inst, date) {
			var yearSplit,
			    currentYear,
			    minDate = this._getMinMaxDate(inst, "min"),
			    maxDate = this._getMinMaxDate(inst, "max"),
			    minYear = null,
			    maxYear = null,
			    years = this._get(inst, "yearRange");
			if (years) {
				yearSplit = years.split(":");
				currentYear = new Date().getFullYear();
				minYear = parseInt(yearSplit[0], 10);
				maxYear = parseInt(yearSplit[1], 10);
				if (yearSplit[0].match(/[+\-].*/)) {
					minYear += currentYear;
				}
				if (yearSplit[1].match(/[+\-].*/)) {
					maxYear += currentYear;
				}
			}

			return (!minDate || date.getTime() >= minDate.getTime()) && (!maxDate || date.getTime() <= maxDate.getTime()) && (!minYear || date.getFullYear() >= minYear) && (!maxYear || date.getFullYear() <= maxYear);
		},

		/* Provide the configuration settings for formatting/parsing. */
		_getFormatConfig: function _getFormatConfig(inst) {
			var shortYearCutoff = this._get(inst, "shortYearCutoff");
			shortYearCutoff = typeof shortYearCutoff !== "string" ? shortYearCutoff : new Date().getFullYear() % 100 + parseInt(shortYearCutoff, 10);
			return { shortYearCutoff: shortYearCutoff,
				dayNamesShort: this._get(inst, "dayNamesShort"), dayNames: this._get(inst, "dayNames"),
				monthNamesShort: this._get(inst, "monthNamesShort"), monthNames: this._get(inst, "monthNames") };
		},

		/* Format the given date for display. */
		_formatDate: function _formatDate(inst, day, month, year) {
			if (!day) {
				inst.currentDay = inst.selectedDay;
				inst.currentMonth = inst.selectedMonth;
				inst.currentYear = inst.selectedYear;
			}
			var date = day ? (typeof day === "undefined" ? "undefined" : _typeof(day)) === "object" ? day : this._daylightSavingAdjust(new Date(year, month, day)) : this._daylightSavingAdjust(new Date(inst.currentYear, inst.currentMonth, inst.currentDay));
			return this.formatDate(this._get(inst, "dateFormat"), date, this._getFormatConfig(inst));
		}
	});

	/*
  * Bind hover events for datepicker elements.
  * Done via delegate so the binding only occurs once in the lifetime of the parent div.
  * Global datepicker_instActive, set by _updateDatepicker allows the handlers to find their way back to the active picker.
  */
	function datepicker_bindHover(dpDiv) {
		var selector = "button, .ui-datepicker-prev, .ui-datepicker-next, .ui-datepicker-calendar td a";
		return dpDiv.on("mouseout", selector, function () {
			$(this).removeClass("ui-state-hover");
			if (this.className.indexOf("ui-datepicker-prev") !== -1) {
				$(this).removeClass("ui-datepicker-prev-hover");
			}
			if (this.className.indexOf("ui-datepicker-next") !== -1) {
				$(this).removeClass("ui-datepicker-next-hover");
			}
		}).on("mouseover", selector, datepicker_handleMouseover);
	}

	function datepicker_handleMouseover() {
		if (!$.datepicker._isDisabledDatepicker(datepicker_instActive.inline ? datepicker_instActive.dpDiv.parent()[0] : datepicker_instActive.input[0])) {
			$(this).parents(".ui-datepicker-calendar").find("a").removeClass("ui-state-hover");
			$(this).addClass("ui-state-hover");
			if (this.className.indexOf("ui-datepicker-prev") !== -1) {
				$(this).addClass("ui-datepicker-prev-hover");
			}
			if (this.className.indexOf("ui-datepicker-next") !== -1) {
				$(this).addClass("ui-datepicker-next-hover");
			}
		}
	}

	/* jQuery extend now ignores nulls! */
	function datepicker_extendRemove(target, props) {
		$.extend(target, props);
		for (var name in props) {
			if (props[name] == null) {
				target[name] = props[name];
			}
		}
		return target;
	}

	/* Invoke the datepicker functionality.
    @param  options  string - a command, optionally followed by additional parameters or
 					Object - settings for attaching new datepicker functionality
    @return  jQuery object */
	$.fn.datepicker = function (options) {

		/* Verify an empty collection wasn't passed - Fixes #6976 */
		if (!this.length) {
			return this;
		}

		/* Initialise the date picker. */
		if (!$.datepicker.initialized) {
			$(document).on("mousedown", $.datepicker._checkExternalClick);
			$.datepicker.initialized = true;
		}

		/* Append datepicker main container to body if not exist. */
		if ($("#" + $.datepicker._mainDivId).length === 0) {
			$("body").append($.datepicker.dpDiv);
		}

		var otherArgs = Array.prototype.slice.call(arguments, 1);
		if (typeof options === "string" && (options === "isDisabled" || options === "getDate" || options === "widget")) {
			return $.datepicker["_" + options + "Datepicker"].apply($.datepicker, [this[0]].concat(otherArgs));
		}
		if (options === "option" && arguments.length === 2 && typeof arguments[1] === "string") {
			return $.datepicker["_" + options + "Datepicker"].apply($.datepicker, [this[0]].concat(otherArgs));
		}
		return this.each(function () {
			typeof options === "string" ? $.datepicker["_" + options + "Datepicker"].apply($.datepicker, [this].concat(otherArgs)) : $.datepicker._attachDatepicker(this, options);
		});
	};

	$.datepicker = new Datepicker(); // singleton instance
	$.datepicker.initialized = false;
	$.datepicker.uuid = new Date().getTime();
	$.datepicker.version = "1.12.0";

	var widgetsDatepicker = $.datepicker;
});

/***/ }),
/* 18 */
/***/ (function(module, exports) {

module.exports = jQuery;

/***/ }),
/* 19 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

/*
* MultiSelect v0.9.12
* Copyright (c) 2012 Louis Cuny
*
* This program is free software. It comes without any warranty, to
* the extent permitted by applicable law. You can redistribute it
* and/or modify it under the terms of the Do What The Fuck You Want
* To Public License, Version 2, as published by Sam Hocevar. See
* http://sam.zoy.org/wtfpl/COPYING for more details.
*/

!function ($) {

  "use strict";

  /* MULTISELECT CLASS DEFINITION
   * ====================== */

  var MultiSelect = function MultiSelect(element, options) {
    this.options = options;
    this.$element = $(element);
    this.$container = $('<div/>', { 'class': "ms-container" });
    this.$selectableContainer = $('<div/>', { 'class': 'ms-selectable' });
    this.$selectionContainer = $('<div/>', { 'class': 'ms-selection' });
    this.$selectableUl = $('<ul/>', { 'class': "ms-list", 'tabindex': '-1' });
    this.$selectionUl = $('<ul/>', { 'class': "ms-list", 'tabindex': '-1' });
    this.scrollTo = 0;
    this.elemsSelector = 'li:visible:not(.ms-optgroup-label,.ms-optgroup-container,.' + options.disabledClass + ')';
  };

  MultiSelect.prototype = {
    constructor: MultiSelect,

    init: function init() {
      var that = this,
          ms = this.$element;

      if (ms.next('.ms-container').length === 0) {
        ms.css({ position: 'absolute', left: '-9999px' });
        ms.attr('id', ms.attr('id') ? ms.attr('id') : Math.ceil(Math.random() * 1000) + 'multiselect');
        this.$container.attr('id', 'ms-' + ms.attr('id'));
        this.$container.addClass(that.options.cssClass);
        ms.find('option').each(function () {
          that.generateLisFromOption(this);
        });

        this.$selectionUl.find('.ms-optgroup-label').hide();

        if (that.options.selectableHeader) {
          that.$selectableContainer.append(that.options.selectableHeader);
        }
        that.$selectableContainer.append(that.$selectableUl);
        if (that.options.selectableFooter) {
          that.$selectableContainer.append(that.options.selectableFooter);
        }

        if (that.options.selectionHeader) {
          that.$selectionContainer.append(that.options.selectionHeader);
        }
        that.$selectionContainer.append(that.$selectionUl);
        if (that.options.selectionFooter) {
          that.$selectionContainer.append(that.options.selectionFooter);
        }

        that.$container.append(that.$selectableContainer);
        that.$container.append(that.$selectionContainer);
        ms.after(that.$container);

        that.activeMouse(that.$selectableUl);
        that.activeKeyboard(that.$selectableUl);

        var action = that.options.dblClick ? 'dblclick' : 'click';

        that.$selectableUl.on(action, '.ms-elem-selectable', function () {
          that.select($(this).data('ms-value'));
        });
        that.$selectionUl.on(action, '.ms-elem-selection', function () {
          that.deselect($(this).data('ms-value'));
        });

        that.activeMouse(that.$selectionUl);
        that.activeKeyboard(that.$selectionUl);

        ms.on('focus', function () {
          that.$selectableUl.focus();
        });
      }

      var selectedValues = ms.find('option:selected').map(function () {
        return $(this).val();
      }).get();
      that.select(selectedValues, 'init');

      if (typeof that.options.afterInit === 'function') {
        that.options.afterInit.call(this, this.$container);
      }
    },

    'generateLisFromOption': function generateLisFromOption(option, index, $container) {
      var that = this,
          ms = that.$element,
          attributes = "",
          $option = $(option);

      for (var cpt = 0; cpt < option.attributes.length; cpt++) {
        var attr = option.attributes[cpt];

        if (attr.name !== 'value' && attr.name !== 'disabled') {
          attributes += attr.name + '="' + attr.value + '" ';
        }
      }
      var selectableLi = $('<li ' + attributes + '><span>' + that.escapeHTML($option.text()) + '</span></li>'),
          selectedLi = selectableLi.clone(),
          value = $option.val(),
          elementId = that.sanitize(value);

      selectableLi.data('ms-value', value).addClass('ms-elem-selectable').attr('id', elementId + '-selectable');

      selectedLi.data('ms-value', value).addClass('ms-elem-selection').attr('id', elementId + '-selection').hide();

      if ($option.prop('disabled') || ms.prop('disabled')) {
        selectedLi.addClass(that.options.disabledClass);
        selectableLi.addClass(that.options.disabledClass);
      }

      var $optgroup = $option.parent('optgroup');

      if ($optgroup.length > 0) {
        var optgroupLabel = $optgroup.attr('label'),
            optgroupId = that.sanitize(optgroupLabel),
            $selectableOptgroup = that.$selectableUl.find('#optgroup-selectable-' + optgroupId),
            $selectionOptgroup = that.$selectionUl.find('#optgroup-selection-' + optgroupId);

        if ($selectableOptgroup.length === 0) {
          var optgroupContainerTpl = '<li class="ms-optgroup-container"></li>',
              optgroupTpl = '<ul class="ms-optgroup"><li class="ms-optgroup-label"><span>' + optgroupLabel + '</span></li></ul>';

          $selectableOptgroup = $(optgroupContainerTpl);
          $selectionOptgroup = $(optgroupContainerTpl);
          $selectableOptgroup.attr('id', 'optgroup-selectable-' + optgroupId);
          $selectionOptgroup.attr('id', 'optgroup-selection-' + optgroupId);
          $selectableOptgroup.append($(optgroupTpl));
          $selectionOptgroup.append($(optgroupTpl));
          if (that.options.selectableOptgroup) {
            $selectableOptgroup.find('.ms-optgroup-label').on('click', function () {
              var values = $optgroup.children(':not(:selected, :disabled)').map(function () {
                return $(this).val();
              }).get();
              that.select(values);
            });
            $selectionOptgroup.find('.ms-optgroup-label').on('click', function () {
              var values = $optgroup.children(':selected:not(:disabled)').map(function () {
                return $(this).val();
              }).get();
              that.deselect(values);
            });
          }
          that.$selectableUl.append($selectableOptgroup);
          that.$selectionUl.append($selectionOptgroup);
        }
        index = index === undefined ? $selectableOptgroup.find('ul').children().length : index + 1;
        selectableLi.insertAt(index, $selectableOptgroup.children());
        selectedLi.insertAt(index, $selectionOptgroup.children());
      } else {
        index = index === undefined ? that.$selectableUl.children().length : index;

        selectableLi.insertAt(index, that.$selectableUl);
        selectedLi.insertAt(index, that.$selectionUl);
      }
    },

    'addOption': function addOption(options) {
      var that = this;

      if (options.value !== undefined && options.value !== null) {
        options = [options];
      }
      $.each(options, function (index, option) {
        if (option.value !== undefined && option.value !== null && that.$element.find("option[value='" + option.value + "']").length === 0) {
          var $option = $('<option value="' + option.value + '">' + option.text + '</option>'),
              $container = option.nested === undefined ? that.$element : $("optgroup[label='" + option.nested + "']"),
              index = parseInt(typeof option.index === 'undefined' ? $container.children().length : option.index);

          if (option.optionClass) {
            $option.addClass(option.optionClass);
          }

          if (option.disabled) {
            $option.prop('disabled', true);
          }

          $option.insertAt(index, $container);
          that.generateLisFromOption($option.get(0), index, option.nested);
        }
      });
    },

    'escapeHTML': function escapeHTML(text) {
      return $("<div>").text(text).html();
    },

    'activeKeyboard': function activeKeyboard($list) {
      var that = this;

      $list.on('focus', function () {
        $(this).addClass('ms-focus');
      }).on('blur', function () {
        $(this).removeClass('ms-focus');
      }).on('keydown', function (e) {
        switch (e.which) {
          case 40:
          case 38:
            e.preventDefault();
            e.stopPropagation();
            that.moveHighlight($(this), e.which === 38 ? -1 : 1);
            return;
          case 37:
          case 39:
            e.preventDefault();
            e.stopPropagation();
            that.switchList($list);
            return;
          case 9:
            if (that.$element.is('[tabindex]')) {
              e.preventDefault();
              var tabindex = parseInt(that.$element.attr('tabindex'), 10);
              tabindex = e.shiftKey ? tabindex - 1 : tabindex + 1;
              $('[tabindex="' + tabindex + '"]').focus();
              return;
            } else {
              if (e.shiftKey) {
                that.$element.trigger('focus');
              }
            }
        }
        if ($.inArray(e.which, that.options.keySelect) > -1) {
          e.preventDefault();
          e.stopPropagation();
          that.selectHighlighted($list);
          return;
        }
      });
    },

    'moveHighlight': function moveHighlight($list, direction) {
      var $elems = $list.find(this.elemsSelector),
          $currElem = $elems.filter('.ms-hover'),
          $nextElem = null,
          elemHeight = $elems.first().outerHeight(),
          containerHeight = $list.height(),
          containerSelector = '#' + this.$container.prop('id');

      $elems.removeClass('ms-hover');
      if (direction === 1) {
        // DOWN

        $nextElem = $currElem.nextAll(this.elemsSelector).first();
        if ($nextElem.length === 0) {
          var $optgroupUl = $currElem.parent();

          if ($optgroupUl.hasClass('ms-optgroup')) {
            var $optgroupLi = $optgroupUl.parent(),
                $nextOptgroupLi = $optgroupLi.next(':visible');

            if ($nextOptgroupLi.length > 0) {
              $nextElem = $nextOptgroupLi.find(this.elemsSelector).first();
            } else {
              $nextElem = $elems.first();
            }
          } else {
            $nextElem = $elems.first();
          }
        }
      } else if (direction === -1) {
        // UP

        $nextElem = $currElem.prevAll(this.elemsSelector).first();
        if ($nextElem.length === 0) {
          var $optgroupUl = $currElem.parent();

          if ($optgroupUl.hasClass('ms-optgroup')) {
            var $optgroupLi = $optgroupUl.parent(),
                $prevOptgroupLi = $optgroupLi.prev(':visible');

            if ($prevOptgroupLi.length > 0) {
              $nextElem = $prevOptgroupLi.find(this.elemsSelector).last();
            } else {
              $nextElem = $elems.last();
            }
          } else {
            $nextElem = $elems.last();
          }
        }
      }
      if ($nextElem.length > 0) {
        $nextElem.addClass('ms-hover');
        var scrollTo = $list.scrollTop() + $nextElem.position().top - containerHeight / 2 + elemHeight / 2;

        $list.scrollTop(scrollTo);
      }
    },

    'selectHighlighted': function selectHighlighted($list) {
      var $elems = $list.find(this.elemsSelector),
          $highlightedElem = $elems.filter('.ms-hover').first();

      if ($highlightedElem.length > 0) {
        if ($list.parent().hasClass('ms-selectable')) {
          this.select($highlightedElem.data('ms-value'));
        } else {
          this.deselect($highlightedElem.data('ms-value'));
        }
        $elems.removeClass('ms-hover');
      }
    },

    'switchList': function switchList($list) {
      $list.blur();
      this.$container.find(this.elemsSelector).removeClass('ms-hover');
      if ($list.parent().hasClass('ms-selectable')) {
        this.$selectionUl.focus();
      } else {
        this.$selectableUl.focus();
      }
    },

    'activeMouse': function activeMouse($list) {
      var that = this;

      this.$container.on('mouseenter', that.elemsSelector, function () {
        $(this).parents('.ms-container').find(that.elemsSelector).removeClass('ms-hover');
        $(this).addClass('ms-hover');
      });

      this.$container.on('mouseleave', that.elemsSelector, function () {
        $(this).parents('.ms-container').find(that.elemsSelector).removeClass('ms-hover');
      });
    },

    'refresh': function refresh() {
      this.destroy();
      this.$element.multiSelect(this.options);
    },

    'destroy': function destroy() {
      $("#ms-" + this.$element.attr("id")).remove();
      this.$element.off('focus');
      this.$element.css('position', '').css('left', '');
      this.$element.removeData('multiselect');
    },

    'select': function select(value, method) {
      if (typeof value === 'string') {
        value = [value];
      }

      var that = this,
          ms = this.$element,
          msIds = $.map(value, function (val) {
        return that.sanitize(val);
      }),
          selectables = this.$selectableUl.find('#' + msIds.join('-selectable, #') + '-selectable').filter(':not(.' + that.options.disabledClass + ')'),
          selections = this.$selectionUl.find('#' + msIds.join('-selection, #') + '-selection').filter(':not(.' + that.options.disabledClass + ')'),
          options = ms.find('option:not(:disabled)').filter(function () {
        return $.inArray(this.value, value) > -1;
      });

      if (method === 'init') {
        selectables = this.$selectableUl.find('#' + msIds.join('-selectable, #') + '-selectable'), selections = this.$selectionUl.find('#' + msIds.join('-selection, #') + '-selection');
      }

      if (selectables.length > 0) {
        selectables.addClass('ms-selected').hide();
        selections.addClass('ms-selected').show();

        options.prop('selected', true);

        that.$container.find(that.elemsSelector).removeClass('ms-hover');

        var selectableOptgroups = that.$selectableUl.children('.ms-optgroup-container');
        if (selectableOptgroups.length > 0) {
          selectableOptgroups.each(function () {
            var selectablesLi = $(this).find('.ms-elem-selectable');
            if (selectablesLi.length === selectablesLi.filter('.ms-selected').length) {
              $(this).find('.ms-optgroup-label').hide();
            }
          });

          var selectionOptgroups = that.$selectionUl.children('.ms-optgroup-container');
          selectionOptgroups.each(function () {
            var selectionsLi = $(this).find('.ms-elem-selection');
            if (selectionsLi.filter('.ms-selected').length > 0) {
              $(this).find('.ms-optgroup-label').show();
            }
          });
        } else {
          if (that.options.keepOrder && method !== 'init') {
            var selectionLiLast = that.$selectionUl.find('.ms-selected');
            if (selectionLiLast.length > 1 && selectionLiLast.last().get(0) != selections.get(0)) {
              selections.insertAfter(selectionLiLast.last());
            }
          }
        }
        if (method !== 'init') {
          ms.trigger('change');
          if (typeof that.options.afterSelect === 'function') {
            that.options.afterSelect.call(this, value);
          }
        }
      }
    },

    'deselect': function deselect(value) {
      if (typeof value === 'string') {
        value = [value];
      }

      var that = this,
          ms = this.$element,
          msIds = $.map(value, function (val) {
        return that.sanitize(val);
      }),
          selectables = this.$selectableUl.find('#' + msIds.join('-selectable, #') + '-selectable'),
          selections = this.$selectionUl.find('#' + msIds.join('-selection, #') + '-selection').filter('.ms-selected').filter(':not(.' + that.options.disabledClass + ')'),
          options = ms.find('option').filter(function () {
        return $.inArray(this.value, value) > -1;
      });

      if (selections.length > 0) {
        selectables.removeClass('ms-selected').show();
        selections.removeClass('ms-selected').hide();
        options.prop('selected', false);

        that.$container.find(that.elemsSelector).removeClass('ms-hover');

        var selectableOptgroups = that.$selectableUl.children('.ms-optgroup-container');
        if (selectableOptgroups.length > 0) {
          selectableOptgroups.each(function () {
            var selectablesLi = $(this).find('.ms-elem-selectable');
            if (selectablesLi.filter(':not(.ms-selected)').length > 0) {
              $(this).find('.ms-optgroup-label').show();
            }
          });

          var selectionOptgroups = that.$selectionUl.children('.ms-optgroup-container');
          selectionOptgroups.each(function () {
            var selectionsLi = $(this).find('.ms-elem-selection');
            if (selectionsLi.filter('.ms-selected').length === 0) {
              $(this).find('.ms-optgroup-label').hide();
            }
          });
        }
        ms.trigger('change');
        if (typeof that.options.afterDeselect === 'function') {
          that.options.afterDeselect.call(this, value);
        }
      }
    },

    'select_all': function select_all() {
      var ms = this.$element,
          values = ms.val();

      ms.find('option:not(":disabled")').prop('selected', true);
      this.$selectableUl.find('.ms-elem-selectable').filter(':not(.' + this.options.disabledClass + ')').addClass('ms-selected').hide();
      this.$selectionUl.find('.ms-optgroup-label').show();
      this.$selectableUl.find('.ms-optgroup-label').hide();
      this.$selectionUl.find('.ms-elem-selection').filter(':not(.' + this.options.disabledClass + ')').addClass('ms-selected').show();
      this.$selectionUl.focus();
      ms.trigger('change');
      if (typeof this.options.afterSelect === 'function') {
        var selectedValues = $.grep(ms.val(), function (item) {
          return $.inArray(item, values) < 0;
        });
        this.options.afterSelect.call(this, selectedValues);
      }
    },

    'deselect_all': function deselect_all() {
      var ms = this.$element,
          values = ms.val();

      ms.find('option').prop('selected', false);
      this.$selectableUl.find('.ms-elem-selectable').removeClass('ms-selected').show();
      this.$selectionUl.find('.ms-optgroup-label').hide();
      this.$selectableUl.find('.ms-optgroup-label').show();
      this.$selectionUl.find('.ms-elem-selection').removeClass('ms-selected').hide();
      this.$selectableUl.focus();
      ms.trigger('change');
      if (typeof this.options.afterDeselect === 'function') {
        this.options.afterDeselect.call(this, values);
      }
    },

    sanitize: function sanitize(value) {
      var hash = 0,
          i,
          character;
      if (value.length == 0) return hash;
      var ls = 0;
      for (i = 0, ls = value.length; i < ls; i++) {
        character = value.charCodeAt(i);
        hash = (hash << 5) - hash + character;
        hash |= 0; // Convert to 32bit integer
      }
      return hash;
    }
  };

  /* MULTISELECT PLUGIN DEFINITION
   * ======================= */

  $.fn.multiSelect = function () {
    var option = arguments[0],
        args = arguments;

    return this.each(function () {
      var $this = $(this),
          data = $this.data('multiselect'),
          options = $.extend({}, $.fn.multiSelect.defaults, $this.data(), (typeof option === 'undefined' ? 'undefined' : _typeof(option)) === 'object' && option);

      if (!data) {
        $this.data('multiselect', data = new MultiSelect(this, options));
      }

      if (typeof option === 'string') {
        data[option](args[1]);
      } else {
        data.init();
      }
    });
  };

  $.fn.multiSelect.defaults = {
    keySelect: [32],
    selectableOptgroup: false,
    disabledClass: 'disabled',
    dblClick: false,
    keepOrder: false,
    cssClass: ''
  };

  $.fn.multiSelect.Constructor = MultiSelect;

  $.fn.insertAt = function (index, $parent) {
    return this.each(function () {
      if (index === 0) {
        $parent.prepend(this);
      } else {
        $parent.children().eq(index - 1).after(this);
      }
    });
  };
}(window.jQuery);

/***/ }),
/* 20 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


$(function () {
    $(".account").on("click", ".login", function (e) {
        e.preventDefault();
        $("#loginModal").modal("show");
    });

    $('#loginModal').on('shown.bs.modal', function () {
        $('#username').focus();
    });

    $("#mobile-menu").on("click", ".login", function (e) {
        e.preventDefault();
        $("#loginModal").modal("show");
    });
    $("#loginForm").on("submit", function (e) {
        e.preventDefault();
        var form = $(this);
        var success = form.find(".alert-success");
        var _error = form.find(".alert-danger");
        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function success(res) {
                if (res.success) {
                    window.location.reload();
                } else {
                    _error.text(res.message);
                    _error.slideDown().delay(4000).slideUp();
                }
            },
            error: function error(err) {
                _error.text("Misslyckades att logga in");
                _error.slideDown().delay(4000).slideUp();
            }
        });
    });

    $("#open-mobile-menu").on("click", function (e) {
        e.preventDefault();
        $("#mobile-menu").slideToggle();
    });

    $("#mobile-menu").on("click", "a", function (e) {
        var target = $(e.target);
        if (target.hasClass("exp-submenu")) {
            e.preventDefault();
            if (target.hasClass("glyphicon-plus")) {
                target.addClass("glyphicon-minus");
                target.removeClass("glyphicon-plus");
            } else {
                target.addClass("glyphicon-plus");
                target.removeClass("glyphicon-minus");
            }

            $(target.data("id")).slideToggle();
        }
    });

    if ($(".youtubelist").length > 0) $(".youtubelist").youtubegallery();

    $('#recruit-form').on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var _success = form.find(".alert-success");
        var _error2 = form.find(".alert-danger");

        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function success(res) {
                if (res.success) {
                    form.trigger("reset");
                    _success.text(res.message);
                    _success.slideDown().delay(3000).slideUp();
                } else {
                    _error2.text(res.message);
                    _error2.slideDown().delay(5000).slideUp();
                }
            },
            error: function error(err) {
                _error2.text("Ett fel uppstod när ansökan skickades");
                _error2.slideDown().delay(5000).slideUp();
            }
        });
    });

    $('#hire-form').on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var _success2 = form.find(".alert-success");
        var _error3 = form.find(".alert-danger");

        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function success(res) {
                if (res.success) {
                    form.trigger("reset");
                    _success2.text(res.message);
                    _success2.slideDown().delay(3000).slideUp();
                } else {
                    _error3.text(res.message);
                    _error3.slideDown().delay(5000).slideUp();
                }
            },
            error: function error(err) {
                _error3.text("Ett fel uppstod när ansökan skickades");
                _error3.slideDown().delay(5000).slideUp();
            }
        });
    });

    var allowedKeys = { 70: "f", 76: "l", 192: "ö", 74: "j", 84: "t" },
        code = ["f", "l", "ö", "j", "t"],
        pos = 0;document.addEventListener("keydown", function (a) {
        var b = allowedKeys[a.keyCode],
            c = code[pos];b == c ? (pos++, pos == code.length && flojt()) : pos = 0;
    });
});
function flojt() {
    var a = document,
        b = a.getElementById("__cornify_nodes"),
        c = null,
        d = ["https://cornify.com/js/cornify.js", "https://cornify.com/js/cornify_run.js"];if (b) cornify_add();else {
        c = a.createElement("div"), c.id = "__cornify_nodes", a.getElementsByTagName("body")[0].appendChild(c);for (var e = 0; e < d.length; e++) {
            b = a.createElement("script"), b.src = d[e], c.appendChild(b);
        }
    }
}

/***/ }),
/* 21 */,
/* 22 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


$(".videos-app").each(function () {
    var widgetId = $(this).data("id");
    new Vue({
        el: '#videos-app-' + widgetId,
        data: {
            videos: videos[widgetId],
            offset: 0,
            visibleWidth: 0,
            totalWidth: 0,
            showVideo: false,
            videoSrc: ""
        },
        methods: {
            rightClick: function rightClick() {
                if (this.offset + this.visibleWidth < this.totalWidth) {
                    if (this.offset + this.visibleWidth * 2 > this.totalWidth) {
                        this.offset = this.totalWidth - this.visibleWidth;
                    } else {
                        this.offset += this.visibleWidth;
                    }
                }
            },
            leftClick: function leftClick() {
                if (this.offset - this.visibleWidth > 0) {
                    this.offset -= this.visibleWidth;
                } else {
                    this.offset = 0;
                }
            },
            onVideoClick: function onVideoClick(video) {
                this.videoSrc = "https://www.youtube.com/embed/" + video.link + "?autoplay=1";
                this.showVideo = true;
            },
            closeVideo: function closeVideo() {
                this.showVideo = false;
            }
        },
        computed: {
            showRight: function showRight() {
                return this.offset < this.totalWidth - this.visibleWidth;
            },
            showLeft: function showLeft() {
                return this.offset > 0;
            }
        },
        mounted: function mounted() {
            var self = this;
            var container = this.$refs.container;
            this.visibleWidth = container.offsetWidth;
            this.totalWidth = container.scrollWidth;
            this.$nextTick(function () {
                window.addEventListener('resize', function () {
                    self.visibleWidth = container.offsetWidth;
                    self.totalWidth = container.scrollWidth;
                });
            });
        }
    });
});

/***/ }),
/* 23 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


$("#update-profile-form").on("submit", function (e) {
    e.preventDefault();
    var form = $(this);
    var _success = form.find(".alert-success");
    var _error = form.find(".alert-danger");
    $.ajax({
        url: form.attr("action"),
        type: form.attr("method"),
        data: form.serialize(),
        success: function success(res) {
            if (res.success) {
                form.parent().get(0).scrollIntoView();
                _success.text("Din profil uppdaterades");
                _success.slideDown().delay(3000).slideUp();
            } else {
                _error.text(res.message);
                _error.slideDown().delay(3500).slideUp();
            }
        },
        error: function error(err) {
            _error.text("Uppdateringen misslyckades");
            _error.slideDown().delay(3500).slideUp();
        }
    });
});

$("#change-profile-password").on("submit", function (e) {
    e.preventDefault();
    var form = $(this);
    var _success2 = form.find(".alert-success");
    var _error2 = form.find(".alert-danger");
    var newpass = form.find("#newpass");
    var confirmPass = form.find("#confirmpass");
    if (newpass.val() !== confirmPass.val()) {
        _error2.text("Lösenord matchar ej");
        _error2.slideDown().delay(3500).slideUp();
        return true;
    }

    $.ajax({
        url: form.attr("action"),
        type: form.attr("method"),
        data: form.serialize(),
        success: function success(res) {
            if (res.success) {
                _success2.slideDown().delay(3000).slideUp();
                newpass.val("");
                confirmPass.val("");
            } else {
                _error2.text(res.message);
                _error2.slideDown().delay(3500).slideUp();
            }
        },
        error: function error(err) {
            _error2.text("Misslyckades med att ändra lösenord");
            _error2.slideDown().delay(3500).slideUp();
        }
    });
});

$('#update-profile-form .multi-select').multiSelect({});

/***/ }),
/* 24 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


$(function () {
    $('#signup-form').on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var _error = form.find(".alert-danger");
        var _success = form.find(".alert-success");
        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function success(res) {
                if (res.success) {
                    reloadSignups();
                    _success.text("Anmälan uppdaterad");
                    _success.slideDown().delay(3000).slideUp();
                } else {
                    _error.text(res.message);
                    _error.slideDown().delay(4000).slideUp();
                }
            },
            error: function error() {
                _error.text("Misslyckades med att anmäla dig");
                _error.slideDown().delay(4000).slideUp();
            }
        });
    });

    $('.expandable').on('click', function (e) {
        var self = $(this);
        var target = $(e.target);
        if (!target.is("a")) {
            self.toggleClass("expanded");
        }
    });

    function reloadSignups() {
        $('#signup-list').load(' #signup-list > *');
    }
    $('#ical-link-anchor').on('click', function (e) {
        e.preventDefault();
        $('.ical-copy').toggleClass('hide');
    });

    $("#admin-add-signups").on('click', function (e) {
        e.preventDefault();
        $("#edit-signup-modal").modal("show");
    });

    $("#edit-signup-form").on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var _error2 = form.find(".alert-danger");
        var _success2 = form.find(".alert-success");
        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function success(res) {
                if (res.success) {
                    _success2.text("Anmälan uppdaterad");
                    _success2.slideDown().delay(3000).slideUp();
                    form.trigger("reset");
                } else {
                    _error2.text(res.message);
                    _error2.slideDown().delay(4000).slideUp();
                }
            },
            error: function error() {
                _error2.text("Misslyckades med att anmäla dig");
                _error2.slideDown().delay(4000).slideUp();
            }
        });
    });
});

/***/ }),
/* 25 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


$(function () {
    var adressList = $('.adress-list');
    if (adressList.length > 0) {
        var renderList = function renderList() {
            var instruments = Object.keys(memberList);
            adressList.empty();
            instruments.forEach(function (instr) {
                var instrCat = $('<div class="instr-cat" data-instr="' + instr + '">');
                instrCat.append('<h2>' + instr + '</h2>');
                var persons = memberList[instr];
                persons.forEach(function (pers) {
                    var k = new Kamerer(pers);
                    kamererList.push(k);
                    instrCat.append(k.dom);
                });
                adressList.append(instrCat);
            });
        };

        var kamererList = [];


        renderList();
        $("#member-search").on("keyup", function (e) {
            e.preventDefault();
            var self = $(this);
            kamererList.forEach(function (pers) {
                pers.filter(self.val());
            });
            $('.instr-cat').each(function () {
                if ($(this).find('.show').length > 0) {
                    $(this).show();
                } else {
                    $(this).hide();
                }
            });
        });
        $('#select-instrument').on('change', function (e) {
            var val = $(this).val();
            $('.instr-cat').each(function () {
                if (val.length < 1) {
                    $(this).show();
                } else if ($(this).data('instr') === val) {
                    $(this).show();
                } else {
                    $(this).hide();
                }
            });
        });
    }
});

function Kamerer(pers) {
    this.pers = pers;
    this.dom = this.renderPerson();
};

Kamerer.prototype.filter = function (phrase) {
    if (phrase.length < 1) {
        this.show();
        return;
    }
    var lPhrase = phrase.toLowerCase();
    if (this.pers.name.toLowerCase().indexOf(lPhrase) > -1) {
        this.show();
        return;
    } else if (this.pers.instrument.toLowerCase().indexOf(lPhrase) > -1) {
        this.show();
        return;
    } else if (this.pers.phone.toLowerCase().indexOf(lPhrase) > -1) {
        this.show();
        return;
    } else {
        this.hide();
        return;
    }
};

Kamerer.prototype.hide = function () {
    this.dom.addClass('hide');
    this.dom.removeClass('show');
};

Kamerer.prototype.show = function () {
    this.dom.removeClass('hide');
    this.dom.addClass('show');
};

Kamerer.prototype.renderPerson = function () {
    var kamrer = $('<div class="kamerer">');
    var group1 = $('<div class="name-email">');
    group1.append(this.pers.name + '<br><a href="mailto:' + this.pers.email + '">' + this.pers.email + '</a>');
    var group3 = $('<div class="phone-nation">');
    group3.append('<a href="tel:' + this.pers.phone + '">' + this.pers.phone + '</a>');
    kamrer.append(group1);
    kamrer.append(group3);
    return kamrer;
};

/***/ }),
/* 26 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


$(function () {
    $('.music-player').each(function () {
        var self = $(this);
        var albums = widgetAlbums[self.data('id')];
        new MusicPlayer(self, albums);
    });
});
function fmtMSS(s) {
    if (isNaN(s)) {
        return '0:00';
    }
    s = Math.floor(s);
    return (s - (s %= 60)) / 60 + (9 < s ? ':' : ':0') + s;
}
function MusicPlayer(container, albums) {
    this.container = container;
    this.albums = albums;
    this.playList = {};
    this.element = $('<div></div>');
    this.createPlayer();
    this.createAlbums();
    this.selectAlbum(Object.keys(albums)[0]);
    this.renderElement();
    this.initBinds();
};

MusicPlayer.prototype.initBinds = function () {
    var self = this;
    this.container.on('click', '.album-link', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        self.selectAlbum(id);
    });
    function downloadURI(uri, name) {
        var link = document.createElement("a");
        link.download = name;
        link.href = uri;
        link.click();
    }
    this.container.on('click', '.playlist-element', function (e) {
        e.preventDefault();
        if (!$(e.target).hasClass('glyphicon-download')) {
            self.playTrack($(this));
        } else {
            downloadURI($(this).attr('href'), $(this).find('.name').text());
        }
    });

    this.container.on('click', '.play-all', function (e) {
        e.preventDefault();
        var els = self.container.find('.playlist-element');
        if (els.length > 0) {
            els.addClass('queued');
            self.playTrack($(self.container.find('.playlist-element')[0]));

            var player = self.container.find('.player');
            player.off('ended');
            player.on('ended', function (e) {
                e.stopPropagation();
                self.playNext(self.container.find('.playlist-element.active'));
            });
        }
    });
    this.loadPlayer();
};

MusicPlayer.prototype.loadPlayer = function () {
    var player = this.container.find('.player');
    var self = this;
    player.on("timeupdate", function (event) {
        self.updateProgress(event.target.currentTime, this.duration);
    });
    var playpause = this.container.find('.pauseplay');

    playpause.on('click', function (e) {
        e.preventDefault();
        if (playpause.hasClass('glyphicon-play')) {
            playpause.removeClass('glyphicon-play');
            playpause.addClass('glyphicon-pause');
            player.trigger('play');
        } else {
            playpause.addClass('glyphicon-play');
            playpause.removeClass('glyphicon-pause');
            player.trigger('pause');
        }
    });

    player.on('ended', function () {
        playpause.removeClass('glyphicon-pause');
        playpause.addClass('glyphicon-play');
        self.updateProgress(0);
    });

    var progressContainer = this.container.find('.progress-container');

    progressContainer.on('click', function (event) {
        event.preventDefault();
        var decimalProgress = event.offsetX / progressContainer.width();
        var time = decimalProgress * player[0].duration;
        self.updateProgress(time, player[0].duration);
        player[0].currentTime = time;
    });
};

MusicPlayer.prototype.updateProgress = function (time, duration) {
    var progressTime = this.container.find('.progresstime');
    var durationBar = this.container.find('.music-progress-bar');
    var progress = time / duration * 100;
    durationBar.css({ 'width': progress + '%' });
    progressTime.text(fmtMSS(time) + '/' + fmtMSS(duration));
};

MusicPlayer.prototype.playNext = function (linkElement) {
    linkElement.removeClass('queued');
    linkElement.removeClass('active');
    var next = linkElement.next();
    if (next.length > 0) {
        if (!next.hasClass('queued')) {
            this.playNext(next);
        } else {
            this.playTrack(next);
        }
    }
};

MusicPlayer.prototype.playTrack = function (linkElement) {
    var player = this.container.find('.player');
    var playingnow = this.container.find('.playingnow');
    var link = linkElement.attr('href');
    playingnow.text('Spelar nu: ' + linkElement.find('span').text());
    player.attr('src', link);
    player.trigger('load');
    player.trigger('play');
    var controls = this.container.find('.controls');
    controls.removeClass('hide');
    var playpause = this.container.find('.pauseplay');
    playpause.removeClass('glyphicon-play');
    playpause.addClass('glyphicon-pause');
    this.container.find('.playlist-element.active').removeClass('active');
    linkElement.addClass('active');
};

MusicPlayer.prototype.resetPlayer = function () {
    var player = this.container.find('.player');
    player.attr('src', '');
    player.trigger('load');
};
MusicPlayer.prototype.createAlbums = function () {
    var self = this;
    var albumsContainer = $('<div class="album-list"></div>');
    var albumKeys = Object.keys(this.albums);
    var nameKeyMap = {};
    albumKeys.forEach(function (key) {
        nameKeyMap[self.albums[key].name] = key;
    });
    Object.keys(nameKeyMap).sort().forEach(function (name) {
        var key = nameKeyMap[name];
        var album = self.albums[key];
        albumsContainer.append('<div class="album-element"><a class="album-link" data-id="' + key + '" href="#"><img class="album-img" src="' + album.image + '" /><p class="album-name">' + album.name + '</p></a></div>');
    });

    this.element.append(albumsContainer);
};
MusicPlayer.prototype.createPlayer = function () {
    var playerModule = $('<div class="player-module"></div>');
    playerModule.append($('<div class="playingnow"></div>'));
    playerModule.append($('<div class="controls hide"><a href="#" class="pauseplay glyphicon glyphicon-play"></a><div class="progress-container"><div class="music-progress"><span class="music-progress-bar" style="width: 0%;"></span></div></div><div class="progresstime"></div></div>'));
    playerModule.append($('<audio class="player" src=""><p>Your browser does not support the <code>audio</code> element.</p></audio>'));
    this.playListModule = $('<div class="playlist"></div>');
    playerModule.append(this.playListModule);
    var playerContainer = $('<div class="player-container"></div>');
    this.albumDisplay = $('<div class="album-display"><img class="album-display-image" src="" /><a href="#" class="play-all">Spela alla spår <span class="glyphicon glyphicon-play"></span></a></div>');
    this.albumTitle = $('<h2 class="album-title"></h2>');
    playerContainer.append(this.albumDisplay);
    playerContainer.append(playerModule);
    this.element.append(this.albumTitle);
    this.element.append(playerContainer);
};
MusicPlayer.prototype.selectAlbum = function (id) {
    this.currentAlbumId = id;
    this.selectedAlbum = this.albums[id];
    var displayImage = this.albumDisplay.find('.album-display-image');
    displayImage.attr('src', this.selectedAlbum.image);
    this.albumTitle.html(this.selectedAlbum.name);
    this.playList = this.selectedAlbum.tracks;
    this.buildPlayList();
};

MusicPlayer.prototype.buildPlayList = function () {
    var self = this;
    this.playListModule.empty();
    var keys = Object.keys(this.playList);
    keys.forEach(function (el) {
        var track = self.playList[el];
        self.playListModule.append(self.createListElement(el, track.filename, track.name));
    });
};
MusicPlayer.prototype.createListElement = function (number, filename, name) {

    return $('<a href="/albums/' + this.currentAlbumId + '/' + filename + '" class="playlist-element"><span class="name">' + name + '</span><span class="glyphicon glyphicon-download"></span></a>');
};

MusicPlayer.prototype.renderElement = function () {
    this.container.append(this.element);
};

/***/ }),
/* 27 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var timeDay = 24 * 60 * 60 * 1000;
var today = new Date();
var calendarModal = $('#event-info-modal');

var monthNames = ["Januari", "Februari", "Mars", "April", "Maj", "Juni", "Juli", "Augusti", "September", "Oktober", "November", "December"];

Date.prototype.addDays = function (days) {
    var dat = new Date(this.valueOf());
    dat.setDate(dat.getDate() + days);
    return dat;
};

$(function () {
    var calendarElement = $("#calendar");
    var calendarList = $('#calendar-list');
    if (calendarElement.length > 0) {
        var Calendar = function Calendar(container, events) {
            this.events = events;
            this.container = container;
            this.month = today.getMonth();
            this.year = today.getFullYear();
            this.day = today.getDay();
            this.render();
        };

        var Month = function Month(year, month, events) {
            this.weeks = [];
            var firstDayOfMonth = new Date(year, month, 1);
            var lastDayOfMonth = new Date(year, month + 1, 0);
            var firstDayWeekDay = firstDayOfMonth.getDay() - 1;
            if (firstDayWeekDay < 0) firstDayWeekDay = 7 + firstDayWeekDay;
            var firstDayOfCalendar = new Date(firstDayOfMonth.getTime() - firstDayWeekDay * timeDay);
            this.tableHeading = $('<thead> <tr><th>Måndag</th> <th>Tisdag</th> <th>Onsdag</th> <th>Torsdag</th> <th>Fredag</th> <th>Lördag</th> <th>Söndag</th> </tr> </thead>');
            var monday = new Date(firstDayOfCalendar.getTime());
            while (monday < lastDayOfMonth) {
                var week = new Week(monday, firstDayOfMonth, lastDayOfMonth, events);
                this.weeks.push(week);
                monday = monday.addDays(7);
            }
            this.dom = $('<table class="month table table-bordered"></div>');
        };

        var Week = function Week(firstDay, firstDayOfMonth, lastDayOfMonth, events) {
            this.firstDay = firstDay;
            this.days = [];
            for (var i = 0; i < 7; i++) {
                var date = this.firstDay.addDays(i);
                var shown = date >= firstDayOfMonth && date <= lastDayOfMonth && (today <= date || today.getDate() <= date.getDate());
                if (events) {
                    this.days.push(new Day(date, shown, events[date.getDate()]));
                } else {
                    this.days.push(new Day(date, shown, null));
                }
            }
            this.dom = $('<tr class="week"></div>');
        };

        var Day = function Day(date, shown, events) {
            this.date = date;
            this.shown = shown;
            this.dom = $('<td class="day"><span class="date">' + this.date.getDate() + '</span></div>');
            if (!shown) this.dom.addClass('outside');
            if (shown && events && events.length > 0) {
                for (var i = 0; i < events.length; i++) {
                    var event = events[i];
                    var eventDom = $('<a href="#" class="dayEvent">' + event.halan + ' ' + event.name + '</a>');
                    this.dom.append(eventDom);
                    eventDom.on('click', function (e) {
                        e.preventDefault();
                        modalTitle.html(event.name);
                        modalDay.html(event.day);
                        modalPlace.html(event.place);
                        modalHalan.html('Samling i hålan: ' + event.halan);
                        if (event.there !== '00:00') {
                            modalThere.html('Samling på plats: ' + event.there);
                        } else {
                            modalThere.empty();
                        }
                        if (event.start !== '00:00') {
                            modalStart.html('Spelning startar: ' + event.start);
                        } else {
                            modalStart.empty();
                        }
                        if (event.type === 'Spelning') {
                            modalSignup.attr("href", '/upcoming/Event/' + event.id);
                            modalSignup.html('Anmäl');
                            modalStand.html('Speltyp: ' + event.stand);
                            modalComming.html(event.cancome + ' Kommer - ' + event.cantcome + ' Kommer inte');
                            modalFika.empty();
                        } else {
                            modalSignup.empty();
                            modalStand.empty();
                            modalComming.empty();
                            modalFika.html('Fika: ' + event.fika);
                        }
                        if (event.desc) {
                            modalDesc.html(event.desc);
                        } else {
                            modalDesc.empty();
                        }
                        if (event.intdesc) {
                            modalIntDesc.html(event.intdesc);
                        } else {
                            modalIntDesc.empty();
                        }
                        calendarModal.modal('show');
                    });
                }
            }
        };

        var modalTitle = calendarModal.find('.modal-title');
        var modalDay = calendarModal.find('.modal-day');
        var modalPlace = calendarModal.find('.modal-place');
        var modalHalan = calendarModal.find('.modal-halan');
        var modalThere = calendarModal.find('.modal-there');
        var modalStart = calendarModal.find('.modal-start');
        var modalSignup = calendarModal.find('.modal-signup');
        var modalComming = calendarModal.find('.modal-comming');
        var modalStand = calendarModal.find('.modal-stand');
        var modalFika = calendarModal.find('.modal-fika');
        var modalDesc = calendarModal.find('.modal-description');
        var modalIntDesc = calendarModal.find('.modal-intdescription');

        ;

        Calendar.prototype.nextMonth = function () {
            this.month++;
            if (this.month > 11) {
                this.month = 0;
                this.year++;
            }
            this.render();
        };

        Calendar.prototype.prevMonth = function () {
            if (this.year > today.getFullYear() || this.month - 1 >= today.getMonth()) {
                this.month--;
                if (this.month < 0) {
                    this.month = 11;
                    this.year--;
                }
                this.render();
            }
        };

        Calendar.prototype.render = function () {
            var monthEvents;
            if (this.events[this.year] != null) {
                monthEvents = this.events[this.year][monthNames[this.month].toLowerCase()];
            }
            var shownMonth = new Month(this.year, this.month, monthEvents);
            var controls = $('<div class="controls"><a href="" class="prev-month glyphicon glyphicon-chevron-left"></a><span class="date">' + monthNames[this.month] + ' ' + this.year + '</span><a href="" class="next-month glyphicon glyphicon-chevron-right"></a></div>');
            this.container.empty();
            this.container.append(controls);
            this.container.append(shownMonth.render());
        };

        ;

        Month.prototype.render = function () {
            var self = this;
            self.dom.empty();
            self.dom.append(this.tableHeading);
            var tableBody = $('<tbody></tbody>');
            this.weeks.forEach(function (week) {
                tableBody.append(week.render());
            });
            self.dom.append(tableBody);
            return this.dom;
        };

        ;

        Week.prototype.render = function () {
            var self = this;
            this.days.forEach(function (day) {
                self.dom.append(day.render());
            });
            return this.dom;
        };

        ;

        Day.prototype.render = function () {
            return this.dom;
        };

        var cal = new Calendar(calendarElement, calendarEvents);
        $('#calendar').on('click', '.prev-month', function (e) {
            e.preventDefault();
            cal.prevMonth();
        });
        $('#calendar').on('click', '.next-month', function (e) {
            e.preventDefault();
            cal.nextMonth();
        });
        $('.calendar-toggle').on('click', function (e) {
            e.preventDefault();
            $('.calendar-toggle').removeClass('active');
            $(this).addClass('active');
            if ($(this).hasClass('event')) {
                calendarElement.addClass('hide');
                calendarList.removeClass('hide');
            } else {
                calendarElement.removeClass('hide');
                calendarList.addClass('hide');
            }
        });
    }
});

/***/ }),
/* 28 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


/* Chosen v1.7.0 | (c) 2011-2017 by Harvest | MIT License, https://github.com/harvesthq/chosen/blob/master/LICENSE.md */
(function () {
  var a,
      b,
      c,
      d,
      e,
      f = function f(a, b) {
    return function () {
      return a.apply(b, arguments);
    };
  },
      g = {}.hasOwnProperty,
      h = function h(a, b) {
    function c() {
      this.constructor = a;
    }for (var d in b) {
      g.call(b, d) && (a[d] = b[d]);
    }return c.prototype = b.prototype, a.prototype = new c(), a.__super__ = b.prototype, a;
  };d = function () {
    function a() {
      this.options_index = 0, this.parsed = [];
    }return a.prototype.add_node = function (a) {
      return "OPTGROUP" === a.nodeName.toUpperCase() ? this.add_group(a) : this.add_option(a);
    }, a.prototype.add_group = function (a) {
      var b, c, d, e, f, g;for (b = this.parsed.length, this.parsed.push({ array_index: b, group: !0, label: this.escapeExpression(a.label), title: a.title ? a.title : void 0, children: 0, disabled: a.disabled, classes: a.className }), f = a.childNodes, g = [], d = 0, e = f.length; e > d; d++) {
        c = f[d], g.push(this.add_option(c, b, a.disabled));
      }return g;
    }, a.prototype.add_option = function (a, b, c) {
      return "OPTION" === a.nodeName.toUpperCase() ? ("" !== a.text ? (null != b && (this.parsed[b].children += 1), this.parsed.push({ array_index: this.parsed.length, options_index: this.options_index, value: a.value, text: a.text, html: a.innerHTML, title: a.title ? a.title : void 0, selected: a.selected, disabled: c === !0 ? c : a.disabled, group_array_index: b, group_label: null != b ? this.parsed[b].label : null, classes: a.className, style: a.style.cssText })) : this.parsed.push({ array_index: this.parsed.length, options_index: this.options_index, empty: !0 }), this.options_index += 1) : void 0;
    }, a.prototype.escapeExpression = function (a) {
      var b, c;return null == a || a === !1 ? "" : /[\&\<\>\"\'\`]/.test(a) ? (b = { "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#x27;", "`": "&#x60;" }, c = /&(?!\w+;)|[\<\>\"\'\`]/g, a.replace(c, function (a) {
        return b[a] || "&amp;";
      })) : a;
    }, a;
  }(), d.select_to_array = function (a) {
    var b, c, e, f, g;for (c = new d(), g = a.childNodes, e = 0, f = g.length; f > e; e++) {
      b = g[e], c.add_node(b);
    }return c.parsed;
  }, b = function () {
    function a(b, c) {
      this.form_field = b, this.options = null != c ? c : {}, this.label_click_handler = f(this.label_click_handler, this), a.browser_is_supported() && (this.is_multiple = this.form_field.multiple, this.set_default_text(), this.set_default_values(), this.setup(), this.set_up_html(), this.register_observers(), this.on_ready());
    }return a.prototype.set_default_values = function () {
      var a = this;return this.click_test_action = function (b) {
        return a.test_active_click(b);
      }, this.activate_action = function (b) {
        return a.activate_field(b);
      }, this.active_field = !1, this.mouse_on_container = !1, this.results_showing = !1, this.result_highlighted = null, this.is_rtl = this.options.rtl || /\bchosen-rtl\b/.test(this.form_field.className), this.allow_single_deselect = null != this.options.allow_single_deselect && null != this.form_field.options[0] && "" === this.form_field.options[0].text ? this.options.allow_single_deselect : !1, this.disable_search_threshold = this.options.disable_search_threshold || 0, this.disable_search = this.options.disable_search || !1, this.enable_split_word_search = null != this.options.enable_split_word_search ? this.options.enable_split_word_search : !0, this.group_search = null != this.options.group_search ? this.options.group_search : !0, this.search_contains = this.options.search_contains || !1, this.single_backstroke_delete = null != this.options.single_backstroke_delete ? this.options.single_backstroke_delete : !0, this.max_selected_options = this.options.max_selected_options || 1 / 0, this.inherit_select_classes = this.options.inherit_select_classes || !1, this.display_selected_options = null != this.options.display_selected_options ? this.options.display_selected_options : !0, this.display_disabled_options = null != this.options.display_disabled_options ? this.options.display_disabled_options : !0, this.include_group_label_in_selected = this.options.include_group_label_in_selected || !1, this.max_shown_results = this.options.max_shown_results || Number.POSITIVE_INFINITY, this.case_sensitive_search = this.options.case_sensitive_search || !1, this.hide_results_on_select = null != this.options.hide_results_on_select ? this.options.hide_results_on_select : !0;
    }, a.prototype.set_default_text = function () {
      return this.form_field.getAttribute("data-placeholder") ? this.default_text = this.form_field.getAttribute("data-placeholder") : this.is_multiple ? this.default_text = this.options.placeholder_text_multiple || this.options.placeholder_text || a.default_multiple_text : this.default_text = this.options.placeholder_text_single || this.options.placeholder_text || a.default_single_text, this.default_text = this.escape_html(this.default_text), this.results_none_found = this.form_field.getAttribute("data-no_results_text") || this.options.no_results_text || a.default_no_result_text;
    }, a.prototype.choice_label = function (a) {
      return this.include_group_label_in_selected && null != a.group_label ? "<b class='group-name'>" + a.group_label + "</b>" + a.html : a.html;
    }, a.prototype.mouse_enter = function () {
      return this.mouse_on_container = !0;
    }, a.prototype.mouse_leave = function () {
      return this.mouse_on_container = !1;
    }, a.prototype.input_focus = function (a) {
      var b = this;if (this.is_multiple) {
        if (!this.active_field) return setTimeout(function () {
          return b.container_mousedown();
        }, 50);
      } else if (!this.active_field) return this.activate_field();
    }, a.prototype.input_blur = function (a) {
      var b = this;return this.mouse_on_container ? void 0 : (this.active_field = !1, setTimeout(function () {
        return b.blur_test();
      }, 100));
    }, a.prototype.label_click_handler = function (a) {
      return this.is_multiple ? this.container_mousedown(a) : this.activate_field();
    }, a.prototype.results_option_build = function (a) {
      var b, c, d, e, f, g, h;for (b = "", e = 0, h = this.results_data, f = 0, g = h.length; g > f && (c = h[f], d = "", d = c.group ? this.result_add_group(c) : this.result_add_option(c), "" !== d && (e++, b += d), (null != a ? a.first : void 0) && (c.selected && this.is_multiple ? this.choice_build(c) : c.selected && !this.is_multiple && this.single_set_selected_text(this.choice_label(c))), !(e >= this.max_shown_results)); f++) {}return b;
    }, a.prototype.result_add_option = function (a) {
      var b, c;return a.search_match && this.include_option_in_results(a) ? (b = [], a.disabled || a.selected && this.is_multiple || b.push("active-result"), !a.disabled || a.selected && this.is_multiple || b.push("disabled-result"), a.selected && b.push("result-selected"), null != a.group_array_index && b.push("group-option"), "" !== a.classes && b.push(a.classes), c = document.createElement("li"), c.className = b.join(" "), c.style.cssText = a.style, c.setAttribute("data-option-array-index", a.array_index), c.innerHTML = a.search_text, a.title && (c.title = a.title), this.outerHTML(c)) : "";
    }, a.prototype.result_add_group = function (a) {
      var b, c;return (a.search_match || a.group_match) && a.active_options > 0 ? (b = [], b.push("group-result"), a.classes && b.push(a.classes), c = document.createElement("li"), c.className = b.join(" "), c.innerHTML = a.search_text, a.title && (c.title = a.title), this.outerHTML(c)) : "";
    }, a.prototype.results_update_field = function () {
      return this.set_default_text(), this.is_multiple || this.results_reset_cleanup(), this.result_clear_highlight(), this.results_build(), this.results_showing ? this.winnow_results() : void 0;
    }, a.prototype.reset_single_select_options = function () {
      var a, b, c, d, e;for (d = this.results_data, e = [], b = 0, c = d.length; c > b; b++) {
        a = d[b], a.selected ? e.push(a.selected = !1) : e.push(void 0);
      }return e;
    }, a.prototype.results_toggle = function () {
      return this.results_showing ? this.results_hide() : this.results_show();
    }, a.prototype.results_search = function (a) {
      return this.results_showing ? this.winnow_results() : this.results_show();
    }, a.prototype.winnow_results = function () {
      var a, b, c, d, e, f, g, h, i, j, k, l;for (this.no_results_clear(), e = 0, g = this.get_search_text(), a = g.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, "\\$&"), d = this.get_search_regex(a), b = this.get_highlight_regex(a), l = this.results_data, j = 0, k = l.length; k > j; j++) {
        c = l[j], c.search_match = !1, f = null, this.include_option_in_results(c) && (c.group && (c.group_match = !1, c.active_options = 0), null != c.group_array_index && this.results_data[c.group_array_index] && (f = this.results_data[c.group_array_index], 0 === f.active_options && f.search_match && (e += 1), f.active_options += 1), c.search_text = c.group ? c.label : c.html, (!c.group || this.group_search) && (c.search_match = this.search_string_match(c.search_text, d), c.search_match && !c.group && (e += 1), c.search_match ? (g.length && (h = c.search_text.search(b), i = c.search_text.substr(0, h + g.length) + "</em>" + c.search_text.substr(h + g.length), c.search_text = i.substr(0, h) + "<em>" + i.substr(h)), null != f && (f.group_match = !0)) : null != c.group_array_index && this.results_data[c.group_array_index].search_match && (c.search_match = !0)));
      }return this.result_clear_highlight(), 1 > e && g.length ? (this.update_results_content(""), this.no_results(g)) : (this.update_results_content(this.results_option_build()), this.winnow_results_set_highlight());
    }, a.prototype.get_search_regex = function (a) {
      var b, c;return b = this.search_contains ? "" : "^", c = this.case_sensitive_search ? "" : "i", new RegExp(b + a, c);
    }, a.prototype.get_highlight_regex = function (a) {
      var b, c;return b = this.search_contains ? "" : "\\b", c = this.case_sensitive_search ? "" : "i", new RegExp(b + a, c);
    }, a.prototype.search_string_match = function (a, b) {
      var c, d, e, f;if (b.test(a)) return !0;if (this.enable_split_word_search && (a.indexOf(" ") >= 0 || 0 === a.indexOf("[")) && (d = a.replace(/\[|\]/g, "").split(" "), d.length)) for (e = 0, f = d.length; f > e; e++) {
        if (c = d[e], b.test(c)) return !0;
      }
    }, a.prototype.choices_count = function () {
      var a, b, c, d;if (null != this.selected_option_count) return this.selected_option_count;for (this.selected_option_count = 0, d = this.form_field.options, b = 0, c = d.length; c > b; b++) {
        a = d[b], a.selected && (this.selected_option_count += 1);
      }return this.selected_option_count;
    }, a.prototype.choices_click = function (a) {
      return a.preventDefault(), this.activate_field(), this.results_showing || this.is_disabled ? void 0 : this.results_show();
    }, a.prototype.keydown_checker = function (a) {
      var b, c;switch (b = null != (c = a.which) ? c : a.keyCode, this.search_field_scale(), 8 !== b && this.pending_backstroke && this.clear_backstroke(), b) {case 8:
          this.backstroke_length = this.get_search_field_value().length;break;case 9:
          this.results_showing && !this.is_multiple && this.result_select(a), this.mouse_on_container = !1;break;case 13:
          this.results_showing && a.preventDefault();break;case 27:
          this.results_showing && a.preventDefault();break;case 32:
          this.disable_search && a.preventDefault();break;case 38:
          a.preventDefault(), this.keyup_arrow();break;case 40:
          a.preventDefault(), this.keydown_arrow();}
    }, a.prototype.keyup_checker = function (a) {
      var b, c;switch (b = null != (c = a.which) ? c : a.keyCode, this.search_field_scale(), b) {case 8:
          this.is_multiple && this.backstroke_length < 1 && this.choices_count() > 0 ? this.keydown_backstroke() : this.pending_backstroke || (this.result_clear_highlight(), this.results_search());break;case 13:
          a.preventDefault(), this.results_showing && this.result_select(a);break;case 27:
          this.results_showing && this.results_hide();break;case 9:case 16:case 17:case 18:case 38:case 40:case 91:
          break;default:
          this.results_search();}
    }, a.prototype.clipboard_event_checker = function (a) {
      var b = this;if (!this.is_disabled) return setTimeout(function () {
        return b.results_search();
      }, 50);
    }, a.prototype.container_width = function () {
      return null != this.options.width ? this.options.width : "" + this.form_field.offsetWidth + "px";
    }, a.prototype.include_option_in_results = function (a) {
      return this.is_multiple && !this.display_selected_options && a.selected ? !1 : !this.display_disabled_options && a.disabled ? !1 : a.empty ? !1 : !0;
    }, a.prototype.search_results_touchstart = function (a) {
      return this.touch_started = !0, this.search_results_mouseover(a);
    }, a.prototype.search_results_touchmove = function (a) {
      return this.touch_started = !1, this.search_results_mouseout(a);
    }, a.prototype.search_results_touchend = function (a) {
      return this.touch_started ? this.search_results_mouseup(a) : void 0;
    }, a.prototype.outerHTML = function (a) {
      var b;return a.outerHTML ? a.outerHTML : (b = document.createElement("div"), b.appendChild(a), b.innerHTML);
    }, a.prototype.get_single_html = function () {
      return '<a class="chosen-single chosen-default">\n  <span>' + this.default_text + '</span>\n  <div><b></b></div>\n</a>\n<div class="chosen-drop">\n  <div class="chosen-search">\n    <input class="chosen-search-input" type="text" autocomplete="off" />\n  </div>\n  <ul class="chosen-results"></ul>\n</div>';
    }, a.prototype.get_multi_html = function () {
      return '<ul class="chosen-choices">\n  <li class="search-field">\n    <input class="chosen-search-input" type="text" autocomplete="off" value="' + this.default_text + '" />\n  </li>\n</ul>\n<div class="chosen-drop">\n  <ul class="chosen-results"></ul>\n</div>';
    }, a.prototype.get_no_results_html = function (a) {
      return '<li class="no-results">\n  ' + this.results_none_found + " <span>" + a + "</span>\n</li>";
    }, a.browser_is_supported = function () {
      return "Microsoft Internet Explorer" === window.navigator.appName ? document.documentMode >= 8 : /iP(od|hone)/i.test(window.navigator.userAgent) || /IEMobile/i.test(window.navigator.userAgent) || /Windows Phone/i.test(window.navigator.userAgent) || /BlackBerry/i.test(window.navigator.userAgent) || /BB10/i.test(window.navigator.userAgent) || /Android.*Mobile/i.test(window.navigator.userAgent) ? !1 : !0;
    }, a.default_multiple_text = "Select Some Options", a.default_single_text = "Select an Option", a.default_no_result_text = "No results match", a;
  }(), a = jQuery, a.fn.extend({ chosen: function chosen(d) {
      return b.browser_is_supported() ? this.each(function (b) {
        var e, f;return e = a(this), f = e.data("chosen"), "destroy" === d ? void (f instanceof c && f.destroy()) : void (f instanceof c || e.data("chosen", new c(this, d)));
      }) : this;
    } }), c = function (b) {
    function c() {
      return e = c.__super__.constructor.apply(this, arguments);
    }return h(c, b), c.prototype.setup = function () {
      return this.form_field_jq = a(this.form_field), this.current_selectedIndex = this.form_field.selectedIndex;
    }, c.prototype.set_up_html = function () {
      var b, c;return b = ["chosen-container"], b.push("chosen-container-" + (this.is_multiple ? "multi" : "single")), this.inherit_select_classes && this.form_field.className && b.push(this.form_field.className), this.is_rtl && b.push("chosen-rtl"), c = { "class": b.join(" "), title: this.form_field.title }, this.form_field.id.length && (c.id = this.form_field.id.replace(/[^\w]/g, "_") + "_chosen"), this.container = a("<div />", c), this.container.width(this.container_width()), this.is_multiple ? this.container.html(this.get_multi_html()) : this.container.html(this.get_single_html()), this.form_field_jq.hide().after(this.container), this.dropdown = this.container.find("div.chosen-drop").first(), this.search_field = this.container.find("input").first(), this.search_results = this.container.find("ul.chosen-results").first(), this.search_field_scale(), this.search_no_results = this.container.find("li.no-results").first(), this.is_multiple ? (this.search_choices = this.container.find("ul.chosen-choices").first(), this.search_container = this.container.find("li.search-field").first()) : (this.search_container = this.container.find("div.chosen-search").first(), this.selected_item = this.container.find(".chosen-single").first()), this.results_build(), this.set_tab_index(), this.set_label_behavior();
    }, c.prototype.on_ready = function () {
      return this.form_field_jq.trigger("chosen:ready", { chosen: this });
    }, c.prototype.register_observers = function () {
      var a = this;return this.container.bind("touchstart.chosen", function (b) {
        a.container_mousedown(b);
      }), this.container.bind("touchend.chosen", function (b) {
        a.container_mouseup(b);
      }), this.container.bind("mousedown.chosen", function (b) {
        a.container_mousedown(b);
      }), this.container.bind("mouseup.chosen", function (b) {
        a.container_mouseup(b);
      }), this.container.bind("mouseenter.chosen", function (b) {
        a.mouse_enter(b);
      }), this.container.bind("mouseleave.chosen", function (b) {
        a.mouse_leave(b);
      }), this.search_results.bind("mouseup.chosen", function (b) {
        a.search_results_mouseup(b);
      }), this.search_results.bind("mouseover.chosen", function (b) {
        a.search_results_mouseover(b);
      }), this.search_results.bind("mouseout.chosen", function (b) {
        a.search_results_mouseout(b);
      }), this.search_results.bind("mousewheel.chosen DOMMouseScroll.chosen", function (b) {
        a.search_results_mousewheel(b);
      }), this.search_results.bind("touchstart.chosen", function (b) {
        a.search_results_touchstart(b);
      }), this.search_results.bind("touchmove.chosen", function (b) {
        a.search_results_touchmove(b);
      }), this.search_results.bind("touchend.chosen", function (b) {
        a.search_results_touchend(b);
      }), this.form_field_jq.bind("chosen:updated.chosen", function (b) {
        a.results_update_field(b);
      }), this.form_field_jq.bind("chosen:activate.chosen", function (b) {
        a.activate_field(b);
      }), this.form_field_jq.bind("chosen:open.chosen", function (b) {
        a.container_mousedown(b);
      }), this.form_field_jq.bind("chosen:close.chosen", function (b) {
        a.close_field(b);
      }), this.search_field.bind("blur.chosen", function (b) {
        a.input_blur(b);
      }), this.search_field.bind("keyup.chosen", function (b) {
        a.keyup_checker(b);
      }), this.search_field.bind("keydown.chosen", function (b) {
        a.keydown_checker(b);
      }), this.search_field.bind("focus.chosen", function (b) {
        a.input_focus(b);
      }), this.search_field.bind("cut.chosen", function (b) {
        a.clipboard_event_checker(b);
      }), this.search_field.bind("paste.chosen", function (b) {
        a.clipboard_event_checker(b);
      }), this.is_multiple ? this.search_choices.bind("click.chosen", function (b) {
        a.choices_click(b);
      }) : this.container.bind("click.chosen", function (a) {
        a.preventDefault();
      });
    }, c.prototype.destroy = function () {
      return a(this.container[0].ownerDocument).unbind("click.chosen", this.click_test_action), this.form_field_label.length > 0 && this.form_field_label.unbind("click.chosen"), this.search_field[0].tabIndex && (this.form_field_jq[0].tabIndex = this.search_field[0].tabIndex), this.container.remove(), this.form_field_jq.removeData("chosen"), this.form_field_jq.show();
    }, c.prototype.search_field_disabled = function () {
      return this.is_disabled = this.form_field.disabled || this.form_field_jq.parents("fieldset").is(":disabled"), this.container.toggleClass("chosen-disabled", this.is_disabled), this.search_field[0].disabled = this.is_disabled, this.is_multiple || this.selected_item.unbind("focus.chosen", this.activate_field), this.is_disabled ? this.close_field() : this.is_multiple ? void 0 : this.selected_item.bind("focus.chosen", this.activate_field);
    }, c.prototype.container_mousedown = function (b) {
      var c;if (!this.is_disabled) return !b || "mousedown" !== (c = b.type) && "touchstart" !== c || this.results_showing || b.preventDefault(), null != b && a(b.target).hasClass("search-choice-close") ? void 0 : (this.active_field ? this.is_multiple || !b || a(b.target)[0] !== this.selected_item[0] && !a(b.target).parents("a.chosen-single").length || (b.preventDefault(), this.results_toggle()) : (this.is_multiple && this.search_field.val(""), a(this.container[0].ownerDocument).bind("click.chosen", this.click_test_action), this.results_show()), this.activate_field());
    }, c.prototype.container_mouseup = function (a) {
      return "ABBR" !== a.target.nodeName || this.is_disabled ? void 0 : this.results_reset(a);
    }, c.prototype.search_results_mousewheel = function (a) {
      var b;return a.originalEvent && (b = a.originalEvent.deltaY || -a.originalEvent.wheelDelta || a.originalEvent.detail), null != b ? (a.preventDefault(), "DOMMouseScroll" === a.type && (b = 40 * b), this.search_results.scrollTop(b + this.search_results.scrollTop())) : void 0;
    }, c.prototype.blur_test = function (a) {
      return !this.active_field && this.container.hasClass("chosen-container-active") ? this.close_field() : void 0;
    }, c.prototype.close_field = function () {
      return a(this.container[0].ownerDocument).unbind("click.chosen", this.click_test_action), this.active_field = !1, this.results_hide(), this.container.removeClass("chosen-container-active"), this.clear_backstroke(), this.show_search_field_default(), this.search_field_scale(), this.search_field.blur();
    }, c.prototype.activate_field = function () {
      return this.is_disabled ? void 0 : (this.container.addClass("chosen-container-active"), this.active_field = !0, this.search_field.val(this.search_field.val()), this.search_field.focus());
    }, c.prototype.test_active_click = function (b) {
      var c;return c = a(b.target).closest(".chosen-container"), c.length && this.container[0] === c[0] ? this.active_field = !0 : this.close_field();
    }, c.prototype.results_build = function () {
      return this.parsing = !0, this.selected_option_count = null, this.results_data = d.select_to_array(this.form_field), this.is_multiple ? this.search_choices.find("li.search-choice").remove() : this.is_multiple || (this.single_set_selected_text(), this.disable_search || this.form_field.options.length <= this.disable_search_threshold ? (this.search_field[0].readOnly = !0, this.container.addClass("chosen-container-single-nosearch")) : (this.search_field[0].readOnly = !1, this.container.removeClass("chosen-container-single-nosearch"))), this.update_results_content(this.results_option_build({ first: !0 })), this.search_field_disabled(), this.show_search_field_default(), this.search_field_scale(), this.parsing = !1;
    }, c.prototype.result_do_highlight = function (a) {
      var b, c, d, e, f;if (a.length) {
        if (this.result_clear_highlight(), this.result_highlight = a, this.result_highlight.addClass("highlighted"), d = parseInt(this.search_results.css("maxHeight"), 10), f = this.search_results.scrollTop(), e = d + f, c = this.result_highlight.position().top + this.search_results.scrollTop(), b = c + this.result_highlight.outerHeight(), b >= e) return this.search_results.scrollTop(b - d > 0 ? b - d : 0);if (f > c) return this.search_results.scrollTop(c);
      }
    }, c.prototype.result_clear_highlight = function () {
      return this.result_highlight && this.result_highlight.removeClass("highlighted"), this.result_highlight = null;
    }, c.prototype.results_show = function () {
      return this.is_multiple && this.max_selected_options <= this.choices_count() ? (this.form_field_jq.trigger("chosen:maxselected", { chosen: this }), !1) : (this.container.addClass("chosen-with-drop"), this.results_showing = !0, this.search_field.focus(), this.search_field.val(this.get_search_field_value()), this.winnow_results(), this.form_field_jq.trigger("chosen:showing_dropdown", { chosen: this }));
    }, c.prototype.update_results_content = function (a) {
      return this.search_results.html(a);
    }, c.prototype.results_hide = function () {
      return this.results_showing && (this.result_clear_highlight(), this.container.removeClass("chosen-with-drop"), this.form_field_jq.trigger("chosen:hiding_dropdown", { chosen: this })), this.results_showing = !1;
    }, c.prototype.set_tab_index = function (a) {
      var b;return this.form_field.tabIndex ? (b = this.form_field.tabIndex, this.form_field.tabIndex = -1, this.search_field[0].tabIndex = b) : void 0;
    }, c.prototype.set_label_behavior = function () {
      return this.form_field_label = this.form_field_jq.parents("label"), !this.form_field_label.length && this.form_field.id.length && (this.form_field_label = a("label[for='" + this.form_field.id + "']")), this.form_field_label.length > 0 ? this.form_field_label.bind("click.chosen", this.label_click_handler) : void 0;
    }, c.prototype.show_search_field_default = function () {
      return this.is_multiple && this.choices_count() < 1 && !this.active_field ? (this.search_field.val(this.default_text), this.search_field.addClass("default")) : (this.search_field.val(""), this.search_field.removeClass("default"));
    }, c.prototype.search_results_mouseup = function (b) {
      var c;return c = a(b.target).hasClass("active-result") ? a(b.target) : a(b.target).parents(".active-result").first(), c.length ? (this.result_highlight = c, this.result_select(b), this.search_field.focus()) : void 0;
    }, c.prototype.search_results_mouseover = function (b) {
      var c;return c = a(b.target).hasClass("active-result") ? a(b.target) : a(b.target).parents(".active-result").first(), c ? this.result_do_highlight(c) : void 0;
    }, c.prototype.search_results_mouseout = function (b) {
      return a(b.target).hasClass("active-result") ? this.result_clear_highlight() : void 0;
    }, c.prototype.choice_build = function (b) {
      var c,
          d,
          e = this;return c = a("<li />", { "class": "search-choice" }).html("<span>" + this.choice_label(b) + "</span>"), b.disabled ? c.addClass("search-choice-disabled") : (d = a("<a />", { "class": "search-choice-close", "data-option-array-index": b.array_index }), d.bind("click.chosen", function (a) {
        return e.choice_destroy_link_click(a);
      }), c.append(d)), this.search_container.before(c);
    }, c.prototype.choice_destroy_link_click = function (b) {
      return b.preventDefault(), b.stopPropagation(), this.is_disabled ? void 0 : this.choice_destroy(a(b.target));
    }, c.prototype.choice_destroy = function (a) {
      return this.result_deselect(a[0].getAttribute("data-option-array-index")) ? (this.active_field ? this.search_field.focus() : this.show_search_field_default(), this.is_multiple && this.choices_count() > 0 && this.get_search_field_value().length < 1 && this.results_hide(), a.parents("li").first().remove(), this.search_field_scale()) : void 0;
    }, c.prototype.results_reset = function () {
      return this.reset_single_select_options(), this.form_field.options[0].selected = !0, this.single_set_selected_text(), this.show_search_field_default(), this.results_reset_cleanup(), this.trigger_form_field_change(), this.active_field ? this.results_hide() : void 0;
    }, c.prototype.results_reset_cleanup = function () {
      return this.current_selectedIndex = this.form_field.selectedIndex, this.selected_item.find("abbr").remove();
    }, c.prototype.result_select = function (a) {
      var b, c;return this.result_highlight ? (b = this.result_highlight, this.result_clear_highlight(), this.is_multiple && this.max_selected_options <= this.choices_count() ? (this.form_field_jq.trigger("chosen:maxselected", { chosen: this }), !1) : (this.is_multiple ? b.removeClass("active-result") : this.reset_single_select_options(), b.addClass("result-selected"), c = this.results_data[b[0].getAttribute("data-option-array-index")], c.selected = !0, this.form_field.options[c.options_index].selected = !0, this.selected_option_count = null, this.is_multiple ? this.choice_build(c) : this.single_set_selected_text(this.choice_label(c)), (!this.is_multiple || this.hide_results_on_select && !a.metaKey && !a.ctrlKey) && (this.results_hide(), this.show_search_field_default()), (this.is_multiple || this.form_field.selectedIndex !== this.current_selectedIndex) && this.trigger_form_field_change({ selected: this.form_field.options[c.options_index].value }), this.current_selectedIndex = this.form_field.selectedIndex, a.preventDefault(), this.search_field_scale())) : void 0;
    }, c.prototype.single_set_selected_text = function (a) {
      return null == a && (a = this.default_text), a === this.default_text ? this.selected_item.addClass("chosen-default") : (this.single_deselect_control_build(), this.selected_item.removeClass("chosen-default")), this.selected_item.find("span").html(a);
    }, c.prototype.result_deselect = function (a) {
      var b;return b = this.results_data[a], this.form_field.options[b.options_index].disabled ? !1 : (b.selected = !1, this.form_field.options[b.options_index].selected = !1, this.selected_option_count = null, this.result_clear_highlight(), this.results_showing && this.winnow_results(), this.trigger_form_field_change({ deselected: this.form_field.options[b.options_index].value }), this.search_field_scale(), !0);
    }, c.prototype.single_deselect_control_build = function () {
      return this.allow_single_deselect ? (this.selected_item.find("abbr").length || this.selected_item.find("span").first().after('<abbr class="search-choice-close"></abbr>'), this.selected_item.addClass("chosen-single-with-deselect")) : void 0;
    }, c.prototype.get_search_field_value = function () {
      return this.search_field.val();
    }, c.prototype.get_search_text = function () {
      return this.escape_html(a.trim(this.get_search_field_value()));
    }, c.prototype.escape_html = function (b) {
      return a("<div/>").text(b).html();
    }, c.prototype.winnow_results_set_highlight = function () {
      var a, b;return b = this.is_multiple ? [] : this.search_results.find(".result-selected.active-result"), a = b.length ? b.first() : this.search_results.find(".active-result").first(), null != a ? this.result_do_highlight(a) : void 0;
    }, c.prototype.no_results = function (a) {
      var b;return b = this.get_no_results_html(a), this.search_results.append(b), this.form_field_jq.trigger("chosen:no_results", { chosen: this });
    }, c.prototype.no_results_clear = function () {
      return this.search_results.find(".no-results").remove();
    }, c.prototype.keydown_arrow = function () {
      var a;return this.results_showing && this.result_highlight ? (a = this.result_highlight.nextAll("li.active-result").first()) ? this.result_do_highlight(a) : void 0 : this.results_show();
    }, c.prototype.keyup_arrow = function () {
      var a;return this.results_showing || this.is_multiple ? this.result_highlight ? (a = this.result_highlight.prevAll("li.active-result"), a.length ? this.result_do_highlight(a.first()) : (this.choices_count() > 0 && this.results_hide(), this.result_clear_highlight())) : void 0 : this.results_show();
    }, c.prototype.keydown_backstroke = function () {
      var a;return this.pending_backstroke ? (this.choice_destroy(this.pending_backstroke.find("a").first()), this.clear_backstroke()) : (a = this.search_container.siblings("li.search-choice").last(), a.length && !a.hasClass("search-choice-disabled") ? (this.pending_backstroke = a, this.single_backstroke_delete ? this.keydown_backstroke() : this.pending_backstroke.addClass("search-choice-focus")) : void 0);
    }, c.prototype.clear_backstroke = function () {
      return this.pending_backstroke && this.pending_backstroke.removeClass("search-choice-focus"), this.pending_backstroke = null;
    }, c.prototype.search_field_scale = function () {
      var b, c, d, e, f, g, h, i;if (this.is_multiple) {
        for (e = { position: "absolute", left: "-1000px", top: "-1000px", display: "none", whiteSpace: "pre" }, f = ["fontSize", "fontStyle", "fontWeight", "fontFamily", "lineHeight", "textTransform", "letterSpacing"], h = 0, i = f.length; i > h; h++) {
          d = f[h], e[d] = this.search_field.css(d);
        }return c = a("<div />").css(e), c.text(this.get_search_field_value()), a("body").append(c), g = c.width() + 25, c.remove(), b = this.container.outerWidth(), g = Math.min(b - 10, g), this.search_field.width(g);
      }
    }, c.prototype.trigger_form_field_change = function (a) {
      return this.form_field_jq.trigger("input", a), this.form_field_jq.trigger("change", a);
    }, c;
  }(b);
}).call(undefined);

/***/ }),
/* 29 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


$("#add-menu").on("click", function (e) {
    e.preventDefault();
    $("#edit-menu-container").toggle();
});
$("#add-top-menu").on("submit", function (e) {
    e.preventDefault();
    var form = $(this);
    var _error = form.find(".alert-danger");
    var _success = form.find(".alert-success");
    $.ajax({
        url: form.attr("action"),
        type: "POST",
        data: form.serialize(),
        success: function success(res) {
            if (res.success) {
                updateMenuList();
                _success.slideDown().delay(3000).slideUp();
                setTimeout(function () {
                    $("#edit-menu-container").slideUp();
                    $("#name").val("");
                }, 2000);
            } else {
                _error.text(res.message);
                _error.slideDown().delay(4000).slideUp();
            }
        },
        error: function error(err) {
            _error.text("Misslyckades med att spara sida");
            _error.slideDown().delay(4000).slideUp();
        }
    });
});

$("#menus").on("click", ".remove-menu", function (e) {
    e.preventDefault();
    var _success2 = $("#menualerts").find(".alert-success");
    var _error2 = $("#menualerts").find(".alert-danger");
    var self = $(this);
    if (window.confirm("Vill du verkligen ta bort menyn: " + self.data("name"))) {
        $.ajax({
            url: "/MenuEdit/RemoveTopMenu?id=" + self.data("id"),
            type: "POST",
            success: function success(res) {
                if (res.success) {
                    updateMenuList();
                    _success2.text("Meny borttagen");
                    _success2.slideDown().delay(3500).slideUp();
                } else {
                    _error2.text(res.message);
                    _error2.slideDown().delay(3500).slideUp();
                }
            },
            error: function error(err) {
                _error2.text("Lyckades ej ta bort meny");
                _error2.slideDown().delay(3500).slideUp();
            }
        });
    }
});
$("#menus").on("click", ".add-sub-menu", function (e) {
    e.preventDefault();
    $("#addSubMenuModal").find(".parentId").val($(this).data("id"));
    $("#addSubMenuModal").modal("show");
});

$("#add-submenu-form").on("submit", function (e) {
    e.preventDefault();
    var form = $(this);
    var _success3 = $("#menualerts").find(".alert-success");
    var _error3 = $("#menualerts").find(".alert-danger");
    $.ajax({
        url: form.attr("action"),
        type: "POST",
        data: form.serialize(),
        success: function success(res) {
            $("#addSubMenuModal").modal("hide");
            if (res.success) {
                updateMenuList();
                _success3.text("Submeny tillagd");
                _success3.slideDown().delay(3500).slideUp();
            } else {
                _error3.text(res.message);
                _error3.slideDown().delay(4000).slideUp();
            }
        },
        error: function error(err) {
            $("#addSubMenuModal").modal("hide");
            _error3.text("Misslyckades med att spara sida");
            _error3.slideDown().delay(4000).slideUp();
        }
    });
});
$("#menus").on("click", ".remove-sub-menu", function (e) {
    e.preventDefault();
    var _success4 = $("#menualerts").find(".alert-success");
    var _error4 = $("#menualerts").find(".alert-danger");
    var self = $(this);
    if (window.confirm("Vill du verkligen ta bort menyn: " + self.data("name"))) {
        $.ajax({
            url: "/MenuEdit/RemoveSubMenu?id=" + self.data("id"),
            type: "POST",
            success: function success(res) {
                if (res.success) {
                    updateMenuList();
                    _success4.text("Meny borttagen");
                    _success4.slideDown().delay(3500).slideUp();
                } else {
                    _error4.text(res.message);
                    _error4.slideDown().delay(3500).slideUp();
                }
            },
            error: function error(err) {
                _error4.text("Lyckades ej ta bort meny");
                _error4.slideDown().delay(3500).slideUp();
            }
        });
    }
});

$("#menus").on("click", ".menu", function (e) {
    e.preventDefault();
    var self = $(this);
    var form = $('#edit-menu-form');
    var id = form.find('.menuId');
    var name = form.find('.name');
    var link = form.find('.page');
    var parent = form.find('.parentId');
    var loggedIn = form.find('.logged');
    var balett = form.find('.balett');
    loggedIn.prop('checked', self.data('logged') === "True");
    balett.prop('checked', self.data('balett') === "True");
    name.val(self.text());
    link.val(self.data('link'));
    link.removeAttr('required');
    link.trigger("chosen:updated");
    id.val(self.data('id'));
    parent.val('false');
    $('#editMenuModal').modal('show');
});

$("#menus").on("click", ".submenu", function (e) {
    e.preventDefault();
    var self = $(this);
    var form = $('#edit-menu-form');
    var id = form.find('.menuId');
    var parent = form.find('.parentId');
    var name = form.find('.name');
    var link = form.find('.page');
    link.attr('required', true);
    name.val(self.text());
    link.val(self.data('link'));
    link.trigger("chosen:updated");
    id.val(self.data('id'));
    parent.val('true');
    $('#editMenuModal').modal('show');
});

$("#edit-menu-form").on("submit", function (e) {
    e.preventDefault();
    var form = $(this);
    var _success5 = $("#menualerts").find(".alert-success");
    var _error5 = form.find(".alert-danger");
    $.ajax({
        url: form.attr("action"),
        type: "POST",
        data: form.serialize(),
        success: function success(res) {
            if (res.success) {
                updateMenuList();
                $('#editMenuModal').modal('hide');
                _success5.text("Meny redigerad");
                _success5.slideDown().delay(3000).slideUp();
            } else {
                _error5.text(res.message);
                _error5.slideDown().delay(4000).slideUp();
            }
        },
        error: function error(err) {
            _error5.text("Misslyckades med att redigera sida");
            _error5.slideDown().delay(4000).slideUp();
        }
    });
});

$("#menus").on("click", ".move-left", function (e) {
    e.preventDefault();
    var self = $(this);

    $.ajax({
        url: "/MenuEdit/MoveLeft?id=" + self.data("id"),
        type: "POST",
        success: function success(res) {
            if (res.success) {
                updateMenuList();
            } else {
                console.log(res);
            }
        },
        error: function error(err) {
            console.log(err);
        }
    });
});
$("#menus").on("click", ".move-right", function (e) {
    e.preventDefault();
    var self = $(this);

    $.ajax({
        url: "/MenuEdit/MoveRight?id=" + self.data("id"),
        type: "POST",
        success: function success(res) {
            if (res.success) {
                updateMenuList();
            } else {
                console.log(res);
            }
        },
        error: function error(err) {
            console.log(err);
        }
    });
});
$("#menus").on("click", ".move-up", function (e) {
    e.preventDefault();
    var self = $(this);

    $.ajax({
        url: "/MenuEdit/MoveUp?id=" + self.data("id") + "&parent=" + self.data("top"),
        type: "POST",
        success: function success(res) {
            if (res.success) {
                updateMenuList();
            } else {
                console.log(res);
            }
        },
        error: function error(err) {
            console.log(err);
        }
    });
});
$("#menus").on("click", ".move-down", function (e) {
    e.preventDefault();
    var self = $(this);

    $.ajax({
        url: "/MenuEdit/MoveDown?id=" + self.data("id") + "&parent=" + self.data("top"),
        type: "POST",
        success: function success(res) {
            if (res.success) {
                updateMenuList();
            } else {
                console.log(res);
            }
        },
        error: function error(err) {
            console.log(err);
        }
    });
});

function updateMenuList() {
    $.get("/MenuEdit/MenuList", function (data) {
        $("#menus").empty().append(data);
    });
}

/***/ }),
/* 30 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


$("#new-page-button").on("click", function (e) {
    e.preventDefault();
    $("#new-page-form").toggle();
});

$(".chosen-select").chosen({ width: '100%' });

$(".remove-page").on("click", function (e) {
    e.preventDefault();
    if (window.confirm("Är du säker på att du vill ta bort denna sida?")) {
        window.location.href = $(this).attr("href");
    }
});

$("#new-page-form").on("submit", "form", function (e) {
    e.preventDefault();
    var form = $(this);
    var _error = form.find(".alert-danger");
    $.ajax({
        url: form.attr("action"),
        type: "POST",
        data: form.serialize(),
        success: function success(res) {
            if (res.success) {
                window.location.reload();
            } else {
                _error.text(res.message);
                _error.slideDown().delay(4000).slideUp();
            }
        },
        error: function error(err) {
            _error.text("Misslyckades med att skapa sida");
            _error.slideDown().delay(4000).slideUp();
        }
    });
});

function updateRevisions(revlink) {
    var revisions = $(".revisions");
    if (revisions) {
        var revisionList = revisions.find(".revision");
        if (revisionList.length > 4) {
            revisionList.first().remove();
        }
        revisions.append(revlink);
    }
}

$("#page-edit").on("submit", function (e) {
    e.preventDefault();
    var form = $(this);
    var isRev = form.data("isrev");
    var _error2 = form.find(".alert-danger");
    var _success = form.find(".alert-success");
    if (!isRev || window.confirm("Är du säker på att du vill ersätta sidan med denna version?")) {
        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize() + "&WidgetsJson=" + jsonifyWidgets(),
            success: function success(res) {
                if (res.success) {
                    if (isRev) {
                        window.location.href = form.attr("action");
                    } else {
                        _success.text(res.message);
                        _success.slideDown().delay(4000).slideUp();
                        updateRevisions(res.revlink);
                    }
                } else {
                    _error2.text(res.message);
                    _error2.slideDown().delay(4000).slideUp();
                }
            },
            error: function error(err) {
                _error2.text("Misslyckades med att spara sida");
                _error2.slideDown().delay(4000).slideUp();
            }
        });
    }
});

$('#widget-area .multi-select').multiSelect({
    selectableHeader: "Uppladdade album",
    selectionHeader: "Valda album"
});

$("#start-page").on("click", function () {
    var slug = $("#slug");
    if (slug.prop("readonly")) {
        slug.prop("readonly", false);
        slug.val(slug.data("oldslug"));
    } else {
        slug.data("oldslug", slug.val());
        slug.val("/");
        slug.prop("readonly", true);
    }
});

var options = {
    selector: "#widget-area .mce-content",
    theme: "modern",
    plugins: ["advlist link image imagetools lists charmap print hr anchor spellchecker searchreplace wordcount code fullscreen media nonbreaking", "table contextmenu directionality emoticons template textcolor paste textcolor colorpicker textpattern"],
    toolbar1: "bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | styleselect code fullscreen",
    toolbar2: "searchreplace bullist | undo redo | link unlink image | hr removeformat | charmap table",
    table_appearance_options: false,
    menubar: false,
    elementpath: true,
    convert_urls: false,
    style_formats: [{ title: 'Vanlig text', block: 'p' }, { title: 'Stor text', block: 'p', classes: 'big' }, { title: 'Rubrik 1', block: 'h1' }, { title: 'Rubrik 2', block: 'h2' }, { title: 'Infobox', selector: 'p', classes: 'infobox' }, { title: '3-delskolumn', block: 'p', classes: 'col-sm-4' }, { title: 'Block med marginal', block: 'p', classes: 'col-xs-12' }],
    toolbar_items_size: "small",
    height: "200",
    content_css: "/css/akstyle.css",
    body_class: "body-content",
    browser_spellcheck: true,
    file_browser_callback: function file_browser_callback(field_name, url, type, win) {
        if (type === "image") {
            $("#imagePickerModal").modal("show");
            $("#picker-images").off("click", ".image-box");
            $("#picker-images").on("click", ".image-box", function (e) {
                $("#imagePickerModal").modal("hide");
                $("#" + field_name).val($(this).data("path"));
            });
        }
        if (type === "file") {
            {
                $("#filePickerModal").modal("show");
                $("#picker-files").off("click", ".image-box");
                $("#picker-files").on("click", ".image-box", function (e) {
                    $("#filePickerModal").modal("hide");
                    $("#" + field_name).val($(this).data("path"));
                });
            }
        }
    }
};

$("#picker-images").on("click", ".pagination li", function (e) {
    e.preventDefault();
    var self = $(this);
    if (!self.hasClass("active")) {
        $("#imagePickerModal .pagination li").removeClass("active");
        self.addClass("active");
        updatePickerSearch();
    }
});

$("#picker-files").on("click", ".pagination li", function (e) {
    e.preventDefault();
    var self = $(this);
    if (!self.hasClass("active")) {
        $("#filePickerModal .pagination li").removeClass("active");
        self.addClass("active");
        updateFilePickerSearch();
    }
});

$("#search-pickermedia-form").on("submit", function (e) {
    e.preventDefault();
    $("#imagePickerModal .pagination li").removeClass("active");
    $($("#imagePickerModal .pagination li")[0]).addClass("active");
    updatePickerSearch();
});

$("#filesearch-pickermedia-form").on("submit", function (e) {
    e.preventDefault();
    $("#filePickerModal .pagination li").removeClass("active");
    $($("#filePickerModal .pagination li")[0]).addClass("active");
    updateFilePickerSearch();
});

$("#search-pickermedia-form").on("change", "#search-mediatags", function (e) {
    $("#imagePickerModal .pagination li").removeClass("active");
    $($("#imagePickerModal .pagination li")[0]).addClass("active");
    updatePickerSearch();
});

$("#imagePickerModal").on("shown.bs.modal", function (e) {
    updateMediaPickerList("", "");
});
$("#filePickerModal").on("shown.bs.modal", function (e) {
    updateFilePickerList("", "");
});

$("#imagePickerModal").on("hidden.bs.modal", function (e) {
    $("#searchtext").val("");
});
$("#filePickerModal").on("hidden.bs.modal", function (e) {
    $("#filesearchtext").val("");
});

$("#widget-area").on("click", '.add-video-link', function (e) {
    e.preventDefault();
    var area = $(this).parent().prev();
    area.append('<div class="form-group video-area row"><div class="col-sm-6"><input class="form-control video-link" value=""></div><div class="col-sm-6"><input class="form-control video-title" value=""><a href="#" class="btn remove-video glyphicon glyphicon-remove"></a></div></div>');
});
$("#widget-area").on("click", '.remove-video', function (e) {
    e.preventDefault();
    var link = $(this).parent().parent();
    link.remove();
});

$("#widget-area").on("click", ".choose-picture-btn", function (e) {
    e.preventDefault();
    var self = $(this);
    var target = self.parent().find(".selected-image");
    $("#imagePickerModal").modal("show");
    $("#picker-images").off("click", ".image-box");
    $("#picker-images").on("click", ".image-box", function (e) {
        $("#imagePickerModal").modal("hide");
        target.attr("src", $(this).data("path"));
    });
});

$("#widget-area").on("click", ".remove-widget", function (e) {
    e.preventDefault();
    if (window.confirm("Är du säker att du vill ta bort den här widgeten?")) {
        $(this).parent().parent().remove();
    }
});
$("#widget-area").on("click", ".min-widget", function (e) {
    e.preventDefault();
    var widget = $(this).parent().parent();
    widget.toggleClass("minimized");
});

function updatePickerSearch() {
    var st = $("#searchtext").val();
    var tag = $("#search-mediatags").val();
    var page = $("#imagePickerModal .pagination li.active a").data("page");
    updateMediaPickerList(st, tag, page);
}

function updateMediaPickerList(search, tag, page) {
    $.get("/Media/MediaPickerList?SearchPhrase=" + search + "&Page=" + page + "&Type=Image" + "&Tag=" + tag, function (data) {
        $("#picker-images").empty().append(data);
    });
}

function updateFilePickerSearch() {
    var st = $("#filesearchtext").val();
    var page = $("#filePickerModal .pagination li.active a").data("page");
    updateFilePickerList(st, page);
}

function updateFilePickerList(search, page) {
    $.get("/Media/MediaPickerList?SearchPhrase=" + search + "&Page=" + page + "&Type=Document", function (data) {
        $("#picker-files").empty().append(data);
    });
}

var templates = $("#widget-templates");
if (templates.length > 0) {
    var textImageTemplate = templates.find(".TextImage");
    var headerTextTemplate = templates.find(".HeaderText");
    var threePuffsTemplate = templates.find(".ThreePuffs");
    var textTemplate = templates.find(".Text");
    var imageTemplate = templates.find(".Image");
    var videoTemplate = templates.find(".Video");
    var musicTemplate = templates.find(".Music");
    var joinTemplate = templates.find(".Join");
    var hireTemplate = templates.find(".Hire");
    var memberListTemplate = templates.find(".MemberList");
    var postListTemplate = templates.find(".PostList");
    tinymce.init(options);
    $(".widget-choose").on("click", "a", function (e) {
        e.preventDefault();
        var type = $(this).data("type");
        if (type === "TextImage") {
            $("#widget-area").append(textImageTemplate.clone());
            tinymce.init(options);
        } else if (type === "Text") {
            $("#widget-area").append(textTemplate.clone());
            tinymce.init(options);
        } else if (type === "HeaderText") {
            $("#widget-area").append(headerTextTemplate.clone());
            tinymce.init(options);
        } else if (type === "Image") {
            $("#widget-area").append(imageTemplate.clone());
        } else if (type === "Video") {
            $("#widget-area").append(videoTemplate.clone());
        } else if (type === "Music") {
            $("#widget-area").append(musicTemplate.clone());
            $('#widget-area .multi-select').multiSelect({
                selectableHeader: "Uppladdade album",
                selectionHeader: "Valda album"
            });
        }
    });
    $(".widget-choose-special").on("click", "a", function (e) {
        e.preventDefault();
        var type = $(this).data("type");
        if (type === "Join") {
            $("#widget-area").append(joinTemplate.clone());
            tinymce.init(options);
        } else if (type === "Hire") {
            $("#widget-area").append(hireTemplate.clone());
            tinymce.init(options);
        } else if (type === "MemberList") {
            $("#widget-area").append(memberListTemplate.clone());
        } else if (type === "PostList") {
            $("#widget-area").append(postListTemplate.clone());
        } else if (type === "ThreePuffs") {
            $("#widget-area").append(threePuffsTemplate.clone());
            tinymce.init(options);
        }
    });

    $("#widget-area").sortable({
        axis: "y",
        handle: ".widget-header",
        distance: 30,
        start: function start(e, ui) {
            $(ui.item).find("textarea").each(function () {
                tinymce.execCommand("mceRemoveEditor", false, $(this).attr("id"));
            });
        },
        stop: function stop(e, ui) {
            $(ui.item).find("textarea").each(function () {
                tinymce.execCommand("mceAddEditor", true, $(this).attr("id"));
            });
        }
    });
}

function jsonifyWidgets() {
    var widgets = [];
    $("#widget-area").find(".widget").each(function (i, o) {
        var wig = new Object();
        var type = $(o).data("type");
        wig.Type = type;
        var tId;
        if (type === "Text") {
            tId = $(o).find(".mce-content").attr("id");
            wig.Text = tinymce.get(tId).getContent();
        } else if (type === "Image") {
            wig.Image = $(o).find(".selected-image").attr("src");
        } else if (type === "Video") {
            wig.Videos = [];
            $(o).find(".video-area").each(function () {
                var Video = new Object();
                Video.Link = $(this).find('.video-link').val();
                Video.Title = $(this).find('.video-title').val();
                wig.Videos.push(Video);
            });
        } else if (type === "Music") {
            wig.Albums = $(o).find(".album").val();
        } else if (type === "TextImage" || type === "Join" || type === "Hire" || type === "HeaderText" || type === "ThreePuffs") {
            tId = $(o).find(".mce-content").attr("id");
            wig.Text = tinymce.get(tId).getContent();
            wig.Image = $(o).find(".selected-image").attr("src");
        }
        widgets.push(wig);
    });
    var res = JSON.stringify(widgets);
    return encodeURIComponent(res);
}

/***/ }),
/* 31 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var usertable = $("#user-table");
if (usertable.length > 0) {
    $('[data-toggle="tooltip"]').tooltip({});

    $("#create-user").on("click", function (e) {
        e.preventDefault();
        $("#createUserModal").modal("show");
    });

    $("#create-user-form").on("submit", function (e) {
        e.preventDefault();
        var form = $(this);
        var _success = $(".alert-success");
        var _error = form.find(".alert-danger");
        $.ajax({
            url: form.attr("action"),
            type: form.attr("method"),
            data: form.serialize(),
            success: function success(res) {
                if (res.success) {
                    updateUserList("");
                    $("#createUserModal").modal("hide");
                    _success.text(res.message);
                    _success.slideDown().delay(3000).slideUp();
                    $('#create-user-form').trigger("reset");
                } else {
                    _error.text(res.message);
                    _error.slideDown().delay(3500).slideUp();
                }
            },
            error: function error(err) {
                _error.text("Lyckades ej lägga till användare");
                _error.slideDown().delay(3500).slideUp();
            }
        });
    });

    $("#user-table").on("click", ".edit-user-info", function (e) {
        e.preventDefault();
        var userName = $(this).data("user");

        $.get("/User/EditUserInfo?userName=" + userName, function (data) {
            $("#editUserModal").empty().append(data);
            $("#editUserModal").modal("show");
            $("#editUserModal .multi-select").multiSelect({});
        });
    });

    $("#editUserModal").on("submit", "#edit-user-form", function (e) {
        e.preventDefault();
        e.preventDefault();
        var form = $(this);
        var _error2 = $(".alert-danger");
        var success = $(".alert-success");
        $.ajax({
            url: form.attr("action"),
            type: form.attr("method"),
            data: form.serialize(),
            success: function success(res) {
                if (res.success) {
                    $("#editUserModal").modal("hide");
                } else {
                    _error2.text(res.message);
                    _error2.slideDown().delay(4000).slideUp();
                }
            },
            error: function error(err) {
                _error2.text("Misslyckades med att spara sida");
                _error2.slideDown().delay(4000).slideUp();
            }
        });
    });

    $("#user-table").on("click", ".remove-user", function (e) {
        e.preventDefault();
        e.stopPropagation();
        var self = $(this);
        if (confirm("Vill du verkligen ta bort " + self.data("user") + "?")) {
            var _success2 = $(".alert-success");
            var _error3 = $(".alert-danger");
            $.ajax({
                url: "/User/RemoveUser?userName=" + self.data("user"),
                type: "POST",
                success: function success(res) {
                    if (res.success) {
                        updateUserList("");
                        _success2.text(res.message);
                        _success2.slideDown().delay(3000).slideUp();
                    } else {
                        _error3.text(res.message);
                        _error3.slideDown().delay(3500).slideUp();
                    }
                },
                error: function error(err) {
                    _error3.text("Lyckades ej lägga till användare");
                    _error3.slideDown().delay(3500).slideUp();
                }
            });
        }
    });

    $("#search-user-form").on("submit", function (e) {
        e.preventDefault();
        updateUserList($("#searchtext").val(), $('#inactive-members').is(':checked'));
    });

    $("#user-table").on("submit", ".add-role", function (e) {
        e.preventDefault();
        var form = $(this);
        var _error4 = $(".alert-danger");
        var success = $(".alert-success");
        $.ajax({
            url: form.attr("action"),
            type: form.attr("method"),
            data: form.serialize(),
            success: function success(res) {
                if (res.success) {
                    var id = "#roles-" + form.find("input[name=UserName]").val();
                    updateRoles(id);
                } else {
                    _error4.text(res.message);
                    _error4.slideDown().delay(4000).slideUp();
                }
            },
            error: function error(err) {
                _error4.text("Misslyckades med att spara sida");
                _error4.slideDown().delay(4000).slideUp();
            }
        });
    });
    $("#user-table").on("submit", ".save-medal", function (e) {
        e.preventDefault();
        var form = $(this);
        var _error5 = $(".alert-danger");
        $.ajax({
            url: form.attr("action"),
            type: form.attr("method"),
            data: form.serialize(),
            success: function success(res) {
                if (!res.success) {
                    _error5.text(res.message);
                    _error5.slideDown().delay(4000).slideUp();
                }
            },
            error: function error(err) {
                _error5.text("Misslyckades med att spara sida");
                _error5.slideDown().delay(4000).slideUp();
            }
        });
    });

    $('.add-post .multi-select').multiSelect({});
    $("#user-table").on("reset", ".add-post", function (e) {
        e.preventDefault();
        var multi = $(this).find('.multi-select');
        multi.val([]);
        multi.multiSelect('refresh');
    });

    $("#user-table").on("submit", ".add-post", function (e) {
        e.preventDefault();
        var form = $(this);
        var _success3 = $(".alert-success");
        var _error6 = $(".alert-danger");
        $.ajax({
            url: form.attr("action"),
            type: form.attr("method"),
            data: form.serialize(),
            success: function success(res) {
                if (res.success) {
                    _success3.text("Post(er) tillagda");
                    _success3.slideDown().delay(4000).slideUp();
                } else {
                    _error6.text(res.message);
                    _error6.slideDown().delay(4000).slideUp();
                }
            },
            error: function error(err) {
                _error6.text("Misslyckades med att lägga till poster");
                _error6.slideDown().delay(4000).slideUp();
            }
        });
    });
    $("#user-table").on("reset", ".add-post", function (e) {
        e.preventDefault();
        var form = $(this);
        form.find('select').val('');
    });

    $("#user-table").on('click', '.reset-pass-btn', function (e) {
        e.preventDefault();
        $('#change-user-name').val($(this).data("user"));
        $('#changePasswordModal').modal('show');
    });
    $("#change-pass-form").on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var _error7 = form.find(".alert-danger");
        var _success4 = $(".alert-success");
        $.ajax({
            url: form.attr("action"),
            type: form.attr("method"),
            data: form.serialize(),
            success: function success(res) {
                if (res.success) {
                    $('#change-user-pass').val("");
                    $('#changePasswordModal').modal('hide');
                    _success4.text('Användarens lösenord ändrat');
                    _success4.slideDown().delay(3000).slideUp();
                } else {
                    _error7.text(res.message);
                    _error7.slideDown().delay(3000).slideUp();
                }
            },
            error: function error(err) {
                _error7.text("Misslyckades med att byta lösenord");
                _error7.slideDown().delay(3000).slideUp();
            }
        });
    });

    $("#user-table").on('click', '.remove-role', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var self = $(this);

        var _error8 = $(".alert-danger");
        $.ajax({
            url: '/User/RemoveRole?UserName=' + self.data('user') + '&Role=' + self.data('role'),
            type: 'POST',
            success: function success(res) {
                if (res.success) {
                    self.parent().remove();
                } else {
                    _error8.text(res.message);
                    _error8.slideDown().delay(4000).slideUp();
                }
            },
            error: function error(err) {
                _error8.text("Misslyckades med att spara sida");
                _error8.slideDown().delay(4000).slideUp();
            }
        });
    });
}

function updateUserList(search, inactive) {
    $.get("/User/UserList?SearchPhrase=" + search + "&Inactive=" + inactive, function (data) {
        $("#user-table").empty().append(data);
        $('[data-toggle="tooltip"]').tooltip();
        $('.add-post .multi-select').multiSelect({});
    });
}

function updateRoles(roleId) {
    $.get("/User/UserList", function (data) {
        $(roleId).empty().append($(data).find(roleId).children());
        $('[data-toggle="tooltip"]').tooltip();
    });
}

/***/ }),
/* 32 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var uploadForm = $("#upload-form");
var droppedFiles = false;

if (uploadForm.length > 0) {
    var $input = uploadForm.find('input[type="file"]'),
        $choose = uploadForm.find(".choose-file"),
        $label = uploadForm.find(".file-name"),
        showFiles = function showFiles(files) {
        droppedFiles = files;
        $label.text(files[0].name);
        $choose.hide();
        $label.show();
    };

    uploadForm.on("drop", function (e) {
        showFiles(e.originalEvent.dataTransfer.files);
    });

    $input.on("change", function (e) {
        showFiles(e.target.files);
    });

    uploadForm.on("drag dragstart dragend dragover dragenter dragleave drop", function (e) {
        e.preventDefault();
        e.stopPropagation();
    }).on("dragover dragenter", function () {
        uploadForm.addClass("is-dragover");
    }).on("dragleave dragend drop", function () {
        uploadForm.removeClass("is-dragover");
    }).on("drop", function (e) {
        droppedFiles = e.originalEvent.dataTransfer.files;
    });
}

$("#upload-file-btn").on("click", function (e) {
    e.preventDefault();
    uploadForm.submit();
});

uploadForm.on("submit", function (e) {
    e.preventDefault();
    var form = $(this);
    var mediaData = new FormData();
    mediaData.append("UploadFile", droppedFiles[0]);
    mediaData.append("Tag", $('#upload-mediatags').val());
    var _success = $(".alert-success");
    var _error = $(".alert-danger");
    $.ajax({
        type: "POST",
        url: "/Media/UploadFile",
        contentType: false,
        processData: false,
        data: mediaData,
        success: function success(res) {
            if (res.success) {
                _success.slideDown().delay(3000).slideUp();
                updateMediaList("", "Image", "1");
                form.find('.box__file').val('');
                droppedFiles = false;
                form.find('.file-name').text('Dra hit filen');
                $('#upload-mediatags').val("Allmän");
            } else {
                _error.text(res.message);
                _error.slideDown().delay(3000).slideUp();
            }
        },
        error: function error(err) {
            _error.text("Misslyckades med att ladda upp filen");
            _error.slideDown().delay(3000).slideUp();
        }
    });
});
$("#search-media-form").on("submit", function (e) {
    e.preventDefault();
    $(".pagination li").removeClass("active");
    $($(".pagination li")[0]).addClass("active");
    updateSearch();
});
$("#search-media-form").on("change", "#search-mediatags", function (e) {
    $(".pagination li").removeClass("active");
    $($(".pagination li")[0]).addClass("active");
    updateSearch();
});
$("#uploaded-files").on("click", ".remove-media", function (e) {
    e.preventDefault();
    var self = $(this);
    if (window.confirm("Är du säker på att du vill ta bort filen: " + self.data("name"))) {
        var _error2 = $(".alert-danger");
        $.ajax({
            url: "/Media/RemoveFile?filename=" + self.data("name"),
            type: "POST",
            success: function success(res) {
                if (res.success) {
                    updateSearch();
                } else {
                    _error2.text(res.message);
                    _error2.slideDown().delay(4000).slideUp();
                }
            },
            error: function error() {
                _error2.text("Misslyckades med att ta bort fil");
                _error2.slideDown().delay(4000).slideUp();
            }
        });
    }
});

$("#uploaded-files").on("click", ".pagination li", function (e) {
    e.preventDefault();
    var self = $(this);
    if (!self.hasClass("active")) {
        $(".pagination li").removeClass("active");
        self.addClass("active");
        updateSearch();
    }
});

$("#uploaded-files").on("click", ".folder-box", function (e) {
    e.preventDefault();
    var tag = $(this).data("tag");
    $("#search-mediatags").val(tag);
    updateSearch();
});

$("#uploaded-files").on("click", ".image-box", function (e) {
    e.preventDefault();
    var self = $(this);
    if (!$(e.target).is("a")) {
        $("#fileeditmodal").modal("show");
        $("#fileedit-tag").val(self.data("tag"));
        $("#fileedit-name").text(self.data("name"));
        $("#fileedit-id").val(self.data("id"));
        if (self.data("type") !== "Document") {
            $("#fileedit-img").attr("src", self.data("src"));
        } else {
            $("#fileedit-img").attr("src", "");
        }
    }
});

$("#fileeditform").on("submit", function (e) {
    e.preventDefault();
    var form = $(this);
    $.ajax({
        url: form.attr("action"),
        type: "POST",
        data: form.serialize(),
        success: function success(res) {
            if (res.success) {
                $("#fileeditmodal").modal("hide");
                updateSearch();
            } else {}
        },
        error: function error(err) {}
    });
});

$("#uploaded-files").on("click", ".back-arrow", function (e) {
    e.preventDefault();
    $("#searchtext").val("");
    $("#search-mediatags").val("");
    updateMediaList("", "", 1);
});

function updateSearch() {
    var st = $("#searchtext").val();
    var tag = $("#search-mediatags").val();
    var page = $(".pagination li.active a").data('page');
    updateMediaList(st, tag, page);
}

function updateMediaList(search, tag, page) {
    $.get("/Media/MediaList?SearchPhrase=" + search + "&Page=" + page + "&Tag=" + tag, function (data) {
        $("#uploaded-files").empty().append(data);
    });
}

/***/ }),
/* 33 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


$(function () {
    $('#create-new-event').on('click', function (e) {
        e.preventDefault();
        $('#edit-event-modal').modal('show');
    });

    $('#edit-event-form').on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var _error = form.find(".alert-danger");
        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function success(res) {
                if (res.success) {
                    reloadEvents('', $('#sort-future-select').val());
                    $('#edit-event-modal').modal('hide');
                    clearEventModal();
                } else {
                    _error.text(res.message);
                    _error.slideDown().delay(4000).slideUp();
                }
            },
            error: function error(err) {
                _error.text("Misslyckades med att spara eventet");
                _error.slideDown().delay(4000).slideUp();
            }
        });
    });
    $('#admin-event-list').on('click', '.remove-event', function (e) {
        e.preventDefault();
        if (window.confirm("Är du säker på att du vill ta bort detta event?")) {
            var self = $(this);
            $.ajax({
                url: self.attr("href"),
                type: "POST",
                success: function success(res) {
                    if (res.success) {
                        reloadEvents('', $('#sort-future-select').val());
                    } else {
                        console.log(res.message);
                    }
                },
                error: function error(err) {
                    console.log("FEL!");
                }
            });
        }
    });
    function fTime(n) {
        return n < 10 ? '0' + n : n;
    }
    $('#admin-event-list').on('click', '.event-row', function (e) {
        e.preventDefault();
        if (!$(e.target).hasClass('remove-event')) {
            $.ajax({
                url: "/AdminEvent/GetEvent/" + $(this).data('id'),
                type: "GET",
                success: function success(res) {
                    if (res.success) {
                        var event = JSON.parse(res.e);
                        $('#Id').val(event.Id);
                        $('#Name').val(event.Name);
                        $('#Secret').prop('checked', event.Secret);
                        $('#Place').val(event.Place);
                        var date = new Date(event.Day);
                        if (date.getFullYear() > 2000) {
                            $('#Day').val(date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear());
                        } else {
                            $('#Day').val('');
                        }
                        $('#Halan').val(event.HalanTime);
                        $('#There').val(event.ThereTime);
                        $('#Starts').val(event.StartsTime);
                        $('#Type').val(event.Type);
                        $('#Fika').val(event.Fika);
                        $('#Stand').val(event.Stand);
                        $('#Description').val(event.Description);
                        $('#InternalDescription').val(event.InternalDescription);
                        replaceEventType(event.Type);
                        $('#edit-event-modal').modal('show');
                    }
                },
                error: function error(err) {
                    console.log("FEL!");
                }
            });
        }
    });

    $("#admin-event-list").on("click", ".pagination li", function (e) {
        e.preventDefault();
        var self = $(this);
        if (!self.hasClass("active")) {
            $("#admin-event-list .active").removeClass("active");
            self.addClass("active");
            reloadEventPage(self.data("page"));
        }
    });

    $('.datepicker').datepicker();
    $('#Day').val('');

    $('#edit-event-modal').on('hidden.bs.modal', function () {
        clearEventModal();
    });
    $('#sort-future-select').on('change', function () {
        reloadEvents('', $(this).val());
    });

    $('#edit-event-modal').on('change', '#Type', function () {
        replaceEventType($(this).val());
    });
    $('#edit-event-modal').on('hidden.bs.modal', function () {
        replaceEventType("");
    });
});
function clearEventModal() {
    $('#edit-event-form')[0].reset();
    $('#Id').val(0);
    $('#Day').val('');
}

function reloadEvents(searchString, sort) {
    $.get('?SearchString=' + searchString + '&Future=' + sort, function (data) {
        $('#admin-event-list').empty().append($(data).find('#admin-event-list').children());
    });
}

function reloadEventPage(page) {
    $.get('?Future=' + $('#sort-future-select').val() + '&page=' + page, function (data) {
        $('#admin-event-list').empty().append($(data).find('#admin-event-list').children());
    });
}

function replaceEventType(type) {
    $('#edit-event-modal').removeClass('Kårhusrep');
    $('#edit-event-modal').removeClass('Fikarep');
    $('#edit-event-modal').removeClass('Rep');
    $('#edit-event-modal').removeClass('Fika');
    $('#edit-event-modal').removeClass('Fest');
    $('#edit-event-modal').removeClass('Spelning');
    $('#edit-event-modal').addClass(type);
}

/***/ }),
/* 34 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


$(function () {
    var recruitDom = $('#recruitList');
    if (recruitDom.length > 0) {
        var list = new RecruitList(recruitListData, recruitDom);
        $('#select-instrument').on('change', function (e) {
            list.filter();
        });
        $('#interest-search').on('keyup', function (e) {
            list.filter();
        });
        $('#archived-flip').on('change', function (e) {
            list.filter();
        });
        $('#recruitList').on('click', '.archive', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            var arch = !$(this).hasClass('green');
            $.ajax({
                url: "/Signup/Archive",
                type: "POST",
                data: { id: id, arch: arch },
                success: function success(res) {
                    if (res.success) {
                        list.archive(id, arch);
                    }
                },
                error: function error(err) {
                    console.log(err);
                }
            });
        });
        $('#recruitList').on('click', '.remove', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            if (window.confirm("Är du säker att du vill ta bort den här intresseanmälan?")) {
                $.ajax({
                    url: "/Signup/Remove",
                    type: "POST",
                    data: { id: id },
                    success: function success(res) {
                        if (res.success) {
                            list.remove(id);
                        }
                    },
                    error: function error(err) {
                        console.log(err);
                    }
                });
            }
        });
        $('#export-interested').on('click', function (e) {
            e.preventDefault();
            $('#export-modal').modal('show');
            list.export();
        });
    }
});

function RecruitList(data, dom) {
    var self = this;
    this.dom = dom;
    this.data = data;
    this.recruits = {};
    var recruitKeys = Object.keys(data);
    recruitKeys.forEach(function (id) {
        self.recruits[id] = new Recruit(data[id]);
    });
    //this.dom.append($('<div class="row recruit-head"><div class="col-md-2">Skapad</div><div class="col-md-2">Namn</div><div class="col-md-2">Instrument</div><div class="col-md-3 col-lg-2 contact">Kontaktinformation</div><div class="col-md-2 col-lg-3 other">Övrigt</div><div class="col-md-1 actions"></div>'));
    this.dom.append($('<div class="row recruit-head"><div class="col-md-2">Skapad</div><div class="col-md-3">Namn</div><div class="col-md-2">Instrument</div><div class="col-md-4 contact">Kontaktinformation</div><div class="col-md-1 actions"></div>'));
    this.render();
};

RecruitList.prototype.archive = function (id, arch) {
    this.recruits[id].archive(arch);
};

RecruitList.prototype.export = function () {
    var self = this;
    var field = $('#export-field');
    field.empty();
    var keys = Object.keys(this.recruits);
    keys.forEach(function (key) {
        var rec = self.recruits[key];
        if (rec.isVisible) {
            field.append(rec.export() + '\n');
        }
    });
};

RecruitList.prototype.remove = function (id) {
    this.recruits[id].hide();
    delete this.recruits[id];
};

RecruitList.prototype.filter = function () {
    var self = this;
    var keys = Object.keys(this.recruits);
    keys.forEach(function (rec) {
        self.recruits[rec].filter();
    });
};

RecruitList.prototype.render = function () {
    var self = this;
    var keys = Object.keys(this.recruits);
    keys.forEach(function (rec) {
        self.dom.append(self.recruits[rec].render());
    });
};

function Recruit(data) {
    this.data = data;
    this.isVisible = true;
    this.dom = $('<div class="row recruit hover-grey">');
    this.dom.append($('<div class="col-md-2">' + data.created.getFullYear() + '-' + ('0' + (data.created.getMonth() + 1)).slice(-2) + '-' + ('0' + data.created.getDate()).slice(-2) + '</div>'));
    this.dom.append($('<div class="col-md-3">' + data.fname + ' ' + data.lname + '</div>'));
    this.dom.append($('<div class="col-md-2">' + data.instrument + '</div>'));
    this.dom.append($('<div class="col-md-4 contact">' + data.email + (data.email.length > 1 ? '<br>' : '') + data.phone + '</div>'));
    //this.dom.append($('<div class="col-md-2 col-lg-3 other">' + data.other + '</div>'));
    this.actions = $('<div class="col-md-1 actions"></div>');
    this.actions.append($('<a href="#" title="Arkivera" data-id="' + data.id + '" class="archive glyphicon glyphicon-folder-open"></a>'));
    this.actions.append($('<a href="#" title="Ta bort" data-id="' + data.id + '" class="remove glyphicon glyphicon-remove"></a>'));
    this.dom.append(this.actions);
    if (data.archived) {
        this.archive(true);
    }
};

Recruit.prototype.archive = function (arch) {
    this.data.archived = arch;
    if (arch) {
        var el = this.dom.find('.archive');
        el.addClass('green');
        el.attr('title', 'Aktivera');
    } else {
        var el = this.dom.find('.archive');
        el.removeClass('green');
        el.attr('title', 'Arkivera');
    }
    this.filter();
};

Recruit.prototype.export = function () {
    var data = this.data;
    return data.fname + '\t' + data.lname + '\t' + data.instrument + '\t' + data.email + '\t' + data.phone;
};

Recruit.prototype.render = function () {
    return this.dom;
};

Recruit.prototype.filter = function () {
    var instr = $('#select-instrument').val();
    if (!!($('#archived-flip').is(':checked') ^ this.data.archived)) {
        this.hide();
        return;
    }
    if (instr.length > 0) {
        if (instr !== this.data.instrument) {
            this.hide();
            return;
        }
    }
    var search = $('#interest-search').val().toLowerCase();
    if (search.length > 0) {
        if ((this.data.fname.toLowerCase() + ' ' + this.data.lname.toLowerCase()).indexOf(search) < 0) {
            this.hide();
            return;
        }
    }
    this.show();
};

Recruit.prototype.hide = function () {
    this.isVisible = false;
    this.dom.addClass('hide');
};

Recruit.prototype.show = function () {
    this.isVisible = true;
    this.dom.removeClass('hide');
};

/***/ }),
/* 35 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


if ($('#album-list').length > 0) {
    var createListElement = function createListElement(number, name, id) {
        return $('<li><span data-id="' + id + '" class="name">' + name + '</span><a class="rem-track" data-id="' + id + '" href="#">x</a></li>');
    };

    var renderAlbums = function renderAlbums() {
        $('#album-list').empty();
        Object.keys(albums).forEach(function (key) {
            var a = albumTemplate.clone();
            a.find('.name-input').val($("<div>").html(albums[key].name).text());
            a.find('.tracks-info').html(albums[key].tracksCount + " spår uppladdade.<br>Klicka här för att hantera.");
            a.find('.del-album').data('id', albums[key].id);
            a.find('.album-img').data('id', albums[key].id);
            a.find('.name-input').data('id', albums[key].id);
            a.find('.tracks').data('id', albums[key].id);
            a.find('.album-img').prop('src', albums[key].image);
            $('#album-list').append(a);
        });
    };

    var updateAlbumImage = function updateAlbumImage(target) {
        var id = target.data('id');
        $.ajax({
            url: "/AlbumEdit/UpdateImage",
            type: "POST",
            data: { id: id, src: target.attr('src') },
            success: function success(res) {
                if (!res.success) {
                    target.prop('src', '');
                } else {
                    albums[id].image = target.attr('src');
                }
            },
            error: function error() {
                target.prop('src', '');
            }
        });
    };

    var albumTemplate = $('<div class="row"><div class="col-md-2 image">' + '<img class="album-img" /></div><div class="col-md-4 name">' + '<input type="text" class="name-input"></div><div class="col-md-2 actions">' + '<a href="#" class="del-album btn glyphicon glyphicon-remove"></a></div>' + '<div class="col-md-4 tracks"><span class="tracks-info"></span></div></div>');

    var tListTemplate = $('<ul class="tracklist"></ul>');

    $(function () {

        var currentAlbum = -1;
        $('#add-album').on('click', function (e) {
            e.preventDefault();
            $('#add-album-container').toggle();
        });
        $('#album-list').on('click', '.del-album', function (e) {
            e.preventDefault();
            var self = $(this);
            var id = self.data('id');
            $.ajax({
                url: "/AlbumEdit/DeleteAlbum/" + id,
                type: "POST",
                success: function success(res) {
                    if (res.success) {
                        delete albums[id];
                        renderAlbums();
                    }
                }
            });
        });
        function changeName(input) {
            var id = input.data('id');
            var name = input.val();
            $.ajax({
                url: "/AlbumEdit/ChangeName",
                type: "POST",
                data: { id: id, name: name },
                success: function success(res) {
                    if (res.success) {
                        albums[id].name = name;
                    }
                }
            });
        }
        $("#album-list").on('focusout', '.name-input', function (e) {
            changeName($(this));
        });
        $("#album-list").on('keypress', '.name-input', function (e) {
            if (e.which === 13) {
                $(this).blur();
            }
        });

        function renderTracks(id) {
            var album = albums[id];
            $('#trackmanagmentmodal').find('.modal-title').html(album.name);
            var tracks = album.tracks;
            $('#tracklist').empty();
            var list = tListTemplate.clone();
            if (Object.keys(tracks).length > 0) {
                Object.keys(tracks).forEach(function (el) {
                    list.append(createListElement(el, tracks[el].name, tracks[el].id));
                });
                $('#tracklist').append(list);
            } else {
                $('#tracklist').html('Inga spår uppladdade hittills');
            }
            album.tracksCount = Object.keys(tracks).length;
            renderAlbums();
        }

        $("#album-list").on('click', '.tracks', function () {
            currentAlbum = $(this).data('id');
            renderTracks(currentAlbum);
            $('#trackmanagmentmodal').modal('show');
        });

        $("#album-list").on("click", ".album-img", function (e) {
            e.preventDefault();
            var target = $(this);
            $("#imagePickerModal").modal("show");
            $("#picker-images").off("click", ".image-box");
            $("#picker-images").on("click", ".image-box", function (e) {
                $("#imagePickerModal").modal("hide");
                target.attr("src", $(this).data("path"));
                updateAlbumImage(target);
            });
        });

        $('#tracklist').on('click', '.rem-track', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            $.ajax({
                url: "/AlbumEdit/DeleteTrack",
                type: "POST",
                data: { id: id, album: currentAlbum },
                success: function success(res) {
                    if (res.success) {
                        var tracksres = JSON.parse(res.tracks);
                        albums[currentAlbum].tracks = {};
                        Object.keys(tracksres).forEach(function (el) {
                            albums[currentAlbum].tracks[el] = {};
                            albums[currentAlbum].tracks[el].filename = tracksres[el].FileName;
                            var clearName = tracksres[el].Name;
                            if (!clearName) {
                                clearName = tracksres[el].FileName;
                            }
                            albums[currentAlbum].tracks[el].name = clearName;
                            albums[currentAlbum].tracks[el].id = tracksres[el].Id;
                        });
                        renderTracks(currentAlbum);
                    }
                }
            });
        });

        $('#add-album-form').on('submit', function (e) {
            e.preventDefault();
            var form = $(this);
            var _error = form.find(".alert-danger");
            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: form.serialize(),
                success: function success(res) {
                    if (res.success) {
                        $('#add-album-container').slideUp();
                        albums[res.id] = {};
                        albums[res.id].id = res.id;
                        albums[res.id].tracksCount = 0;
                        albums[res.id].tracks = {};
                        albums[res.id].name = form.find('#name').val();
                        renderAlbums();
                        form.trigger("reset");
                    } else {
                        _error.text(res.message);
                        _error.slideDown().delay(4000).slideUp();
                    }
                },
                error: function error(err) {
                    _error.text("Misslyckades med att skapa album sida");
                    _error.slideDown().delay(4000).slideUp();
                }
            });
        });

        renderAlbums();

        function uploadTracks(files) {
            var mediaData = new FormData();
            for (var i = 0; i < files.length; i++) {
                mediaData.append("TrackFiles", files[i]);
            }
            mediaData.append("AlbumId", currentAlbum);
            $.ajax({
                type: "POST",
                url: "/AlbumEdit/UploadTracks",
                contentType: false,
                processData: false,
                data: mediaData,
                success: function success(res) {
                    if (res.success) {
                        var tracksres = JSON.parse(res.tracks);
                        albums[currentAlbum].tracks = {};
                        Object.keys(tracksres).forEach(function (el) {
                            albums[currentAlbum].tracks[el] = {};
                            var clearName = tracksres[el].Name;
                            if (!clearName) {
                                clearName = tracksres[el].FileName;
                            }
                            albums[currentAlbum].tracks[el].name = clearName;
                            albums[currentAlbum].tracks[el].filename = tracksres[el].FileName;
                            albums[currentAlbum].tracks[el].id = tracksres[el].Id;
                        });
                        renderTracks(currentAlbum);
                    }
                },
                error: function error(err) {
                    console.log(err);
                }
            });
        }

        var trackform = $("#trackform");
        if (trackform.length > 0) {
            var submitTrackName = function submitTrackName(element) {

                $.ajax({
                    url: "/AlbumEdit/ChangeTrackName",
                    type: "POST",
                    data: { id: element.data("id"), name: element.val() },
                    success: function success(res) {}
                });
                element.replaceWith('<span class="name" data-id="' + element.data("id") + '">' + element.val() + '</span>');
            };

            var input = trackform.find('input[type="file"]'),
                showFiles = function showFiles(files) {
                uploadTracks(files);
            };

            trackform.on("drop", function (e) {
                showFiles(e.originalEvent.dataTransfer.files);
            });

            trackform.on("change", 'input[type="file"]', function (e) {
                showFiles(e.target.files);
            });

            trackform.on("drag dragstart dragend dragover dragenter dragleave drop", function (e) {
                e.preventDefault();
                e.stopPropagation();
            }).on("dragover dragenter", function () {
                trackform.addClass("is-dragover");
            }).on("dragleave dragend drop", function () {
                trackform.removeClass("is-dragover");
            }).on("drop", function (e) {
                trackform = e.originalEvent.dataTransfer.files;
            });

            trackform.on("click", ".name", function (e) {
                e.preventDefault();
                var self = $(this);
                self.replaceWith('<input type="text" class="name-input" data-id="' + self.data("id") + '" value="' + self.text() + '"\>');
                $(".name-input").focus();
            });

            trackform.on("keypress", ".name-input", function (e) {
                if (e.which == 13) {
                    trackform.off("blur", ".name-input");
                    submitTrackName($(this));
                    trackform.on("blur", ".name-input", function (e) {
                        submitTrackName($(this));
                    });
                }
            });
            trackform.on("blur", ".name-input", function (e) {
                submitTrackName($(this));
            });

            trackform.on("submit", function (e) {
                e.preventDefault();
            });
        }
    });
}

/***/ })
/******/ ]);