# SitemapScanner

A small utility program written in C# to scan sitemaps and report on broken links.

## Usage
### `SitemapScanner <url>`
`url` must be a direct link to a `sitemap.xml` file.

Runs a GET request on every URL listed in the sitemap. Sitemap indexes also work and redirects get logged and verified as well.
After finishing execution you will find three log files in the executable directory (`*.log`, `*.error.log`, `*.report.log`).
`*.log` contains all logs from execution, `*.error.log` contains all errors encountered during execution, and `*.report.log`
contains a summary and simplified list of all broken links.

### `SitemapScanner <logFolder> <url>`

Same as above, but write the log files into a custom folder.