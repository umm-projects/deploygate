# What?

* [DeployGate](https://deploygate.com/) へビルドした ipa や apk をデプロイするためのメソッドを提供します

# Why?

* 毎回ビルドサーバにログインして `dg` コマンドを叩くのが面倒だったから実装しました

# Install

```shell
$ npm install github:umm-projects/deploygate
```

# Usage

1. ビルド環境で [`dg` コマンド](https://docs.deploygate.com/v1.1/docs)を用いて DeployGate へのデプロイができるようにセットアップします
1. `Asset` &gt; `Create` &gt; `UnityModule` &gt; `Settings` &gt; `EnvironmentSetting` から生成される設定ファイルを開きます
1. `dg` コマンドへのパスを設定します
    * rbenv を用いている場合は `~/.rbenv/shims/dg`
    * それ以外の場合は `/usr/local/bin/dg`
    * など
1. ビルドを行うと PostprocessBuild の処理に依って自動的にデプロイされます

# License

Copyright (c) 2017 Tetsuya Mori

Released under the MIT license, see [LICENSE.txt](LICENSE.txt)

