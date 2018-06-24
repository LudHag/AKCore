const webpack = require('webpack');
const LoaderOptionsPlugin = require("webpack/lib/LoaderOptionsPlugin");
const path = require('path');
const ExtractTextPlugin = require("extract-text-webpack-plugin");

var appName = 'app';
var entryPoint = './Scripts/main.js';
var exportPath = path.resolve(__dirname, './wwwroot/dist/');

var plugins = [];
var extractSASS = new ExtractTextPlugin("style.css");

if (process.env.NODE_ENV === 'production') {
    var UglifyJsPlugin = webpack.optimize.UglifyJsPlugin;

    plugins.push(new UglifyJsPlugin({ minimize: true }));
    plugins.push(new webpack.DefinePlugin({
            'process.env': {
                NODE_ENV: '"production"'
            }
        }
    ));
} else {
    plugins.push(new LoaderOptionsPlugin({
        options: {
            eslint: {
                emitErrors: true,
                failOnHint: true
            }
        }
    }));
}

plugins.push(extractSASS);
appName = appName + '.js';

module.exports = {
    entry: { main: entryPoint },
    output: {
        path: exportPath,
        filename: appName,
        publicPath: '/dist/',
        hotUpdateChunkFilename: 'hot/hot-update.js',
        hotUpdateMainFilename: 'hot/hot-update.json'
    },
    module: {
        loaders: [
            {
                test: /\.(js|vue)$/,
                exclude: /(node_modules|bower_components)/,
                loader: 'babel-loader',
                query: {
                    presets: ['env']
                }
            },
            {
                test: /\.vue$/,
                loader: 'vue-loader',
                options: {
                    extractCSS: true
                }
            },
            {
                test: /\.(scss|sass)$/,
                loader: 'style-loader!css-loader!postcss-loader!sass-loader'
            },
            {
                test: /\.(png|jpg|gif|svg|woff|woff2|eot|ttf)$/,
                loader: 'file-loader',
                options: {
                    name: '[name].[ext]?[hash]'
                }
            }
        ]
    },
    resolve: {
        alias: {
            'vue$': 'vue/dist/vue.esm.js'
        },
        extensions: ['*', '.js', '.vue']
    },
    externals: {
        "jquery": "jQuery"
    },
    plugins
};