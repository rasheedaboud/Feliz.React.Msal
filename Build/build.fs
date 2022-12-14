open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open BlackFox.Fake

let mutable dotNetOptions = fun _ -> DotNet.Options.Create()

let slnDir = Path.getFullName "../"
let projName = "Feliz.React.Msal"
let projDir =  slnDir @@ projName
let paketLockFile = slnDir @@ "paket.lock"

let printRes (operation : string) (res : ProcessResult) =
    match res with
    | res when res.ExitCode = 0 -> ()
    | res ->
        let concat = String.concat "; "
        failwith $"{operation} failed: {System.Environment.NewLine}{res.Errors |> concat}{System.Environment.NewLine}"

let installDeps = BuildTask.create "InstallDependencies" [] {
    dotNetOptions <- DotNet.install (fun opts ->
        { opts with
            Channel = DotNet.CliChannel.LTS})

    DotNet.exec dotNetOptions "tool" "restore" |> printRes "Tool restore"

    let paketCmd =
        match File.exists paketLockFile with
        | true -> "restore"
        | false -> "install"

    DotNet.exec dotNetOptions "paket" paketCmd |> printRes "Paket install"

    DotNet.exec dotNetOptions "femto" $"{projDir} --resolve"  |> printRes "Femto package resolution"
}

let clean = BuildTask.create "Clean" [] {
    !! (projDir @@ "/**/bin")
    ++ (projDir @@ "/**/obj")
    |> Shell.cleanDirs

    !! (projDir @@ "/**/*.nupkg")
    |> Seq.iter (Shell.rm)
}

let build config =
    !! (projDir @@ $"{projName}.fsproj")
    |> string
    |> DotNet.build (fun opts ->
        { opts with
            Configuration = config
        }
    )

let buildDebug = BuildTask.create "BuildDebug" [installDeps.IfNeeded; clean.IfNeeded] {
    build DotNet.Debug
}

let buildRelease = BuildTask.create "BuildRelease" [installDeps.IfNeeded; clean.IfNeeded] {
    build DotNet.Release
}

let pack = BuildTask.create "Pack" [buildRelease] {
    DotNet.exec id "paket" $"pack --symbols {projDir}" |> printRes "Pack"
}

let _all = BuildTask.createEmpty "All" [installDeps; clean; pack]

let defaultBuild = BuildTask.createEmpty "Build" [buildDebug]

[<EntryPoint>]
let main args =
    BuildTask.setupContextFromArgv args
    BuildTask.runOrDefaultApp _all