// webpack v4
const path = require('path');
// update from 23.12.2018
const nodeExternals = require('webpack-node-externals');
const VueLoaderPlugin = require('vue-loader/lib/plugin');
module.exports = {
    plugins: [
        new VueLoaderPlugin()
    ],
    entry: {
        dashboard: './src/dashboard.ts',
        game: './src/game.ts',
        cookies: './src/cookies.ts',
        editor: './src/editor.ts',
        recaptcha: './src/recaptcha.ts',
        site: './src/site.ts'
    },
    output: {
        path: path.resolve(__dirname, './wwwroot/dist'),
        filename: '[name].js'
    },
    target: 'node', // update from 23.12.2018
   // externals: [nodeExternals()], // update from 23.12.2018
    module: {
        rules: [
            {
                test: /\.css$/,
                use: [
                    'vue-style-loader',
                    'css-loader'
                ]
            },
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: {
                    loader: "babel-loader"
                }
            },
            {
                test: /\.ts(x?)$/,
                exclude: /node_modules/,
                use: [                    
                    {
                        loader: 'babel-loader'
                    },
                    {
                        loader: "ts-loader",
                        options: {
                            appendTsSuffixTo: [/\.ts\.vue$/],
                            appendTsxSuffixTo: [/\.tsx\.vue$/]
                        }
                    }
                ]
            },
            {
                test: /\.vue$/,
                loader: 'vue-loader',
                exclude: /node_modules/,
                options: {
                    loaders: {
                        // Since sass-loader (weirdly) has SCSS as its default parse mode, we map
                        // the "scss" and "sass" values for the lang attribute to the right configs here.
                        // other preprocessors should work out of the box, no loader config like this necessary.
                        'scss': [
                            'vue-style-loader',
                            'css-loader',
                            'sass-loader'
                        ],
                        'sass': [
                            'vue-style-loader',
                            'css-loader',
                            'sass-loader?indentedSyntax'
                        ]
                    }
                    // other vue-loader options go here
                }
            }
        ]
    }
};