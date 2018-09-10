const webpack = require('webpack');
const LoaderOptionsPlugin = require("webpack/lib/LoaderOptionsPlugin");
const path = require('path');
const ExtractTextPlugin = require("extract-text-webpack-plugin");
const BabiliPlugin = require('babili-webpack-plugin');

var appName = 'main';
var exportPath = path.resolve(__dirname, './wwwroot/dist/');

var plugins = [];
var extractSASS = new ExtractTextPlugin("[name].css",
    {
        allChunks: true
    });

var sassbuild = extractSASS.extract({
    use: ['css-loader', 'postcss-loader', 'sass-loader']
});

plugins.push(extractSASS);

if (process.env.NODE_ENV === 'production') {
    plugins.push(new webpack.DefinePlugin({
            'process.env': {
                NODE_ENV: '"production"'
            }
        }
    ));
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

plugins.push(new webpack.optimize.CommonsChunkPlugin({
    name: "vendor",
    minChunks: function (module) {
        if (module.resource && (/^.*\.(css|scss)$/).test(module.resource)) {
            return false;
        }
        return module.context && module.context.includes('node_modules');
    }
}));

appName = appName + '.js';

module.exports = {
    entry: {
        main: './Scripts/main.js',
        vendor: ["vue"],
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
                use: sassbuild
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