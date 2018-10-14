const webpack = require('webpack');
const LoaderOptionsPlugin = require("webpack/lib/LoaderOptionsPlugin");
const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const BabiliPlugin = require('babili-webpack-plugin');
const VueLoaderPlugin = require('vue-loader/lib/plugin')
const devMode = process.env.NODE_ENV !== 'production'

var appName = 'main';
var exportPath = path.resolve(__dirname, './wwwroot/dist/');

var plugins = [];

if (!devMode) {
    plugins.push(new BabiliPlugin());
    plugins.push(new webpack.LoaderOptionsPlugin({
        minimize: true
    }));
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

// plugins.push(new webpack.optimize.CommonsChunkPlugin({
//     name: "vendor",
//     minChunks: function (module) {
//         if (module.resource && (/^.*\.(css|scss)$/).test(module.resource)) {
//             return false;
//         }
//         return module.context && module.context.includes('node_modules');
//     }
// }));

plugins.push(new VueLoaderPlugin());
plugins.push( new MiniCssExtractPlugin());

appName = appName + '.js';

module.exports = {
    mode: process.env.NODE_ENV,
    entry: {
        main: './Scripts/main.js',
        //vendor: ["vue"],
        admin: './Scripts/admin.js'
    },
    output: {
        path: exportPath,
        filename: '[name].js',
        publicPath: '/dist/',
        hotUpdateChunkFilename: 'hot/hot-update.js',
        hotUpdateMainFilename: 'hot/hot-update.json'
    },
    module: {
        rules: [
            {
                test: /\.(js)$/,
                exclude: /(node_modules|bower_components)/,
                loader: 'babel-loader',
                query: {
                    presets: ['env']
                }
            },
            {
                test: /\.vue$/,
                loader: 'vue-loader'
            },
            {
                test: /\.(sa|sc|c)ss$/,
                use: [
                  devMode ? 'style-loader' : MiniCssExtractPlugin.loader,
                  'css-loader',
                  'postcss-loader',
                  'sass-loader',
                ],
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