open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open BlackFox.Fake

let mutable dotNetOptions = fun _ -> DotNet.Options.Create()

let slnDir = Path.getFullName "../"
let projDir =  slnDir @@ "Feliz.React.Msal"
let paketLockFile = slnDir @@ "paket.lock"

let installDeps = BuildTask.create "InstallDependencies" [] {
    let printRes (operation : string) (res : ProcessResult) =
        match res with
        | res when res.ExitCode = 0 -> ()
        | res ->
            let concat = String.concat "; "
            failwith $"{operation} failed: {System.Environment.NewLine}{res.Errors |> concat}{System.Environment.NewLine}"

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

let build = BuildTask.create "Build" [installDeps.IfNeeded; clean.IfNeeded] {
    !! "src/**/*.*proj"
    |> Seq.iter (DotNet.build id)
}

let _all = BuildTask.createEmpty "All" [clean; build]

[<EntryPoint>]
let main args =
    BuildTask.setupContextFromArgv args
    BuildTask.runOrDefaultApp _all
