const std = @import("std");

// This needs to have basically two functions. : Get Feeds (Plural) Get Feed (Singular)
// Get Feeds -> Lemmy.Feed(n)

pub fn main() !void {
    // Prints to stderr (it's a shortcut based on `std.io.getStdErr()`)
    std.debug.print("Hello World", .{});

    const stdout_file = std.io.getStdOut().writer();
    var bw = std.io.bufferedWriter(stdout_file);
    const stdout = bw.writer();
    stdout.print("//END//");
    try bw.flush();

    const stdout_file = std.io.getStdOut().writer();
    var bw = std.io.bufferedWriter(stdout_file);
    try bw.flush();
}

// Why is this needed?
// This is needed so I can reuse code across platforms with native UI
// Plus there could be a need for performance like uh pretty stuff.
//
